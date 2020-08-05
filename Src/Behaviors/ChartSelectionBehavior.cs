using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Input;
using Windows.Foundation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Core;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// ChartSelectionBehavior enables the selection of segments in a Chart.
    /// </summary>
    /// <remarks>
    /// The selected segment can be displayed with a different color specified using SegmentSelectionBrush property available in corresponding series.
    /// ChartSelectionBehavior is applicable only to certain series such as <see cref="ColumnSeries"/>,<see cref="BarSeries"/>,
    /// <see cref="RangeColumnSeries"/>,<see cref="StackingBarSeries"/>,<see cref="StackingColumnSeries"/>,<see cref="ScatterSeries"/>,
    /// <see cref="BubbleSeries"/>,<see cref="PieSeries"/>.
    /// </remarks>
    public partial class ChartSelectionBehavior : ChartBehavior
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="SelectionMode"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(
                "SelectionMode",
                typeof(SelectionMode),
                typeof(ChartSelectionBehavior),
                new PropertyMetadata(SelectionMode.MouseClick));

        /// <summary>
        /// The DependencyProperty for <see cref="EnableSeriesSelection"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableSeriesSelectionProperty =
            DependencyProperty.Register(
                "EnableSeriesSelection",
                typeof(bool),
                typeof(ChartSelectionBehavior),
                new PropertyMetadata(false, OnEnableSeriesSelectionChanged));


        /// <summary>
        /// The DependencyProperty for <see cref="EnableSegmentSelection"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableSegmentSelectionProperty =
            DependencyProperty.Register(
                "EnableSegmentSelection",
                typeof(bool),
                typeof(ChartSelectionBehavior),
                new PropertyMetadata(true, OnEnableSegmentSelectionChanged));


        /// <summary>
        /// The DependencyProperty for <see cref="SelectionStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectionStyleProperty =
           DependencyProperty.Register(
               "SelectionStyle",
               typeof(SelectionStyle),
               typeof(ChartSelectionBehavior),
               new PropertyMetadata(SelectionStyle.Single, OnSelectionStyleChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SelectionCursor"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectionCursorProperty =
            DependencyProperty.Register(
                "SelectionCursor",
                typeof(CoreCursorType),
                typeof(ChartSelectionBehavior),
                new PropertyMetadata(CoreCursorType.Arrow));

        #endregion

        #region Fields

        private ChartSegment mouseUnderSegment;
        private List<ChartSeries> seriesCollection;
        private ChartAdornmentPresenter selectedAdornmentPresenter;
        private int index;

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the selection mode value, which indicates that this series is how to selectable
        /// </summary>
        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the series selection is enabled or not.
        /// </summary>
        public bool EnableSeriesSelection
        {
            get { return (bool)GetValue(EnableSeriesSelectionProperty); }
            set { SetValue(EnableSeriesSelectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the segement (or) datapoint selection is enabled or not.
        /// </summary>
        public bool EnableSegmentSelection
        {
            get { return (bool)GetValue(EnableSegmentSelectionProperty); }
            set { SetValue(EnableSegmentSelectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the SelectionStyle value that indicated the selection type in SfChart.
        /// </summary>
        public SelectionStyle SelectionStyle
        {
            get { return (SelectionStyle)GetValue(SelectionStyleProperty); }
            set { SetValue(SelectionStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the pointer cursor for the series, which indicates that this series is selectable
        /// </summary>
        /// <remarks>
        /// Default value is Arrow
        /// </remarks>
        public CoreCursorType SelectionCursor
        {
            get { return (CoreCursorType)GetValue(SelectionCursorProperty); }
            set { SetValue(SelectionCursorProperty, value); }
        }
        
        #endregion

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Method used to get selection brush for series selection.
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public virtual Brush GetSeriesSelectionBrush(ChartSeriesBase series)
        {
            if (series.SeriesSelectionBrush != null)
                return series.SeriesSelectionBrush;
            else
                return null;
        }

        #endregion

        #region Protected Internal Override Methods

        /// <summary>
        /// Called when Pointer Released in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if(ChartArea == null)
            {
                return;
            }

            if (SelectionMode == Charts.SelectionMode.MouseClick)
            {
                FrameworkElement element = e.OriginalSource as FrameworkElement;
                ChartSegment segment = null;
                ChartArea.CurrentSelectedSeries = null;

                if (element != null)
                {
                    if (element.Tag != null) segment = element.Tag as ChartSegment;
                }

                if (segment is TrendlineSegment)
                    return;

#if __IOS__ || __ANDROID__
                var image = (element as Border)?.Child as Image;
#else
                var image = element as Image;
#endif
                if (image != null && image.Source is WriteableBitmap)
                {
                    // Bitmap segment selection process handling.
                    OnBitmapSeriesMouseDownSelection(element, e);
                }
                else if (segment != null && segment == mouseUnderSegment && segment.Series is ISegmentSelectable
                         && !(segment.Item is Trendline))
                {
                    if (!segment.Series.IsSideBySide && segment.Series is CartesianSeries
                        && !(segment.Series is ScatterSeries) && !(segment.Series is BubbleSeries))
                    {
                        Point canvasPoint = e.GetCurrentPoint(segment.Series.ActualArea.GetAdorningCanvas()).Position;
                        ChartDataPointInfo data = (segment.Series as ChartSeries).GetDataPoint(canvasPoint);

                        OnMouseDownSelection(segment.Series, data);
                    }
                    else
                    {
                        int index = -1;
                        if ((segment.Series.ActualXAxis is CategoryAxis) && !(segment.Series.ActualXAxis as CategoryAxis).IsIndexed
                            && segment.Series.IsSideBySide && !(segment.Series is FinancialSeriesBase) && (!(segment.Series is RangeSeriesBase))
                            && !(segment.Series is WaterfallSeries))
                            index = segment.Series.GroupedActualData.IndexOf(segment.Item);
                        else
                            index = segment.Series is CircularSeriesBase && !double.IsNaN(((CircularSeriesBase)segment.Series).GroupTo)? segment.Series.Segments.IndexOf(segment): segment.Series.ActualData.IndexOf(segment.Item);
                        OnMouseDownSelection(segment.Series, index);
                    }
                }
                else
                {
                    // Get the selected adornment index and select the adornment marker
                    index = ChartExtensionUtils.GetAdornmentIndex(element);
                    FrameworkElement frameworkElement = e.OriginalSource as FrameworkElement;
                    var chartAdornmentPresenter = frameworkElement as ChartAdornmentPresenter;

                    while (frameworkElement != null && chartAdornmentPresenter == null)
                    {
                        frameworkElement = VisualTreeHelper.GetParent(frameworkElement) as FrameworkElement;
                        chartAdornmentPresenter = frameworkElement as ChartAdornmentPresenter;
                    }

                    if (chartAdornmentPresenter != null &&
                        chartAdornmentPresenter.Series is ISegmentSelectable)
                        OnMouseDownSelection(chartAdornmentPresenter.Series, index);
                }

                if (selectedAdornmentPresenter != null)
                {
                    selectedAdornmentPresenter = null;
                }
            }

            AdorningCanvas.ReleasePointerCapture(e.Pointer);
        }

        /// <summary>
        /// Called when Pointer pressed in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if(ChartArea == null)
            {
                return;
            }

            FrameworkElement element = e.OriginalSource as FrameworkElement;
            ChartSegment segment = null;
            ChartArea.CurrentSelectedSeries = null;

            if (element != null)
            {
                if (element.Tag != null) segment = element.Tag as ChartSegment;
            }

            if (segment != null && segment.Series is ISegmentSelectable)
            {
                mouseUnderSegment = segment;
            }
        }

        /// <summary>
        /// Called when Pointer moved in Chart
        /// </summary>
        /// <param name="e"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        protected internal override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            if (ChartArea == null)
            {
                return;
            }

            if (this.EnableSegmentSelection || this.EnableSeriesSelection)
            {
                FrameworkElement element = e.OriginalSource as FrameworkElement;
                ChartSegment segment = null;

                if (element != null)
                {
                    if (element.Tag != null) segment = element.Tag as ChartSegment;
                }

                if (segment is TrendlineSegment)
                    return;

                if (element.DataContext is LegendItem)
                    return;

#if __IOS__ || __ANDROID__
                var image = (element as Border)?.Child as Image;
#else
                var image = element as Image;
#endif


                if (segment != null && segment.Series is ISegmentSelectable && !(segment.Item is Trendline))
                {
                    // Scatter series supports selection and dragging at the same time.
                    if (!(segment.Series is ScatterSeries) && IsDraggableSeries(segment.Series))
                        return;
                    if (!segment.Series.IsLinear || EnableSeriesSelection)
                        ChangeSelectionCursor(true);
                    else
                        ChangeSelectionCursor(false);

                    if (SelectionMode == Charts.SelectionMode.MouseMove)
                    {
                        if (!segment.Series.IsSideBySide && segment.Series is CartesianSeries
                            && !(segment.Series is ScatterSeries) && !(segment.Series is BubbleSeries))
                        {
                            Point canvasPoint = e.GetCurrentPoint(segment.Series.ActualArea.GetAdorningCanvas()).Position;
                            ChartDataPointInfo data = (segment.Series as ChartSeries).GetDataPoint(canvasPoint);
                            OnMouseMoveSelection(segment.Series, data);
                        }
                        else
                        {
                            int segIndex = segment.Series is  CircularSeriesBase && !double.IsNaN(((CircularSeriesBase)segment.Series).GroupTo) ?segment.Series.Segments.IndexOf(segment): segment.Series.ActualData.IndexOf(segment.Item);
                            OnMouseMoveSelection(segment.Series, segIndex);
                        }
                    }
                }
                else if (e.OriginalSource is Shape && (e.OriginalSource as Shape).DataContext is ChartAdornmentContainer
                    && ((e.OriginalSource as Shape).DataContext as ChartAdornmentContainer).Tag is int)
                {
                    // Check the selected element is adornment shape
                    selectedAdornmentPresenter = VisualTreeHelper.GetParent((e.OriginalSource as Shape).DataContext
                            as ChartAdornmentContainer) as ChartAdornmentPresenter;
                    if (IsDraggableSeries(selectedAdornmentPresenter.Series))
                        return;
                    ChangeSelectionCursor(true);
                    if (SelectionMode == Charts.SelectionMode.MouseMove)
                    {
                        index = (int)((e.OriginalSource as Shape).DataContext as ChartAdornmentContainer).Tag;

                        if (selectedAdornmentPresenter != null && selectedAdornmentPresenter.Series is ISegmentSelectable)
                            OnMouseMoveSelection(selectedAdornmentPresenter.Series, index);
                    }
                }
                else if (image != null && image.Source is WriteableBitmap)
                {
                    GetBitmapSeriesCollection(element, e);

                    // Bitmap segment selection process handling.
                    if (SelectionMode == Charts.SelectionMode.MouseMove)
                        OnBitmapSeriesMouseMoveSelection(element, e);
                }
                else if (element is Border || element is TextBlock || element is Shape) // check the selected element is adornment label 
                {
                    ChangeSelectionCursor(false);

                    FrameworkElement frameworkElement = e.OriginalSource as FrameworkElement;
                    int count = e.OriginalSource is TextBlock ? 3 : 2;
                    for (int i = 0; i < count; i++)
                    {
                        if (frameworkElement != null)
                            frameworkElement = VisualTreeHelper.GetParent(frameworkElement) as FrameworkElement;
                        else
                            break;
                    }

                    if (frameworkElement is ContentPresenter)
                    {
                        index = ChartExtensionUtils.GetAdornmentIndex(frameworkElement);
                        if (index != -1)
                            ChangeSelectionCursor(true);
                        if (SelectionMode == Charts.SelectionMode.MouseMove)
                        {
                            frameworkElement = VisualTreeHelper.GetParent(frameworkElement) as FrameworkElement;

                            if (frameworkElement is ChartAdornmentPresenter || frameworkElement is ChartAdornmentContainer)
                            {
                                while (!(frameworkElement is ChartAdornmentPresenter) && frameworkElement != null)
                                {
                                    frameworkElement = VisualTreeHelper.GetParent(frameworkElement) as FrameworkElement;
                                }

                                selectedAdornmentPresenter = frameworkElement as ChartAdornmentPresenter;
                                if (IsDraggableSeries(selectedAdornmentPresenter.Series))
                                    return;
                                if (selectedAdornmentPresenter != null && selectedAdornmentPresenter.Series is ISegmentSelectable)
                                    OnMouseMoveSelection(selectedAdornmentPresenter.Series, index);
                            }
                        }
                    }

                    var contentControl = frameworkElement as ContentControl;
                    if (contentControl != null && contentControl.Tag is int)
                    {
                        ChangeSelectionCursor(true);
                        if (SelectionMode == Charts.SelectionMode.MouseMove)
                        {
                            index = (int)contentControl.Tag;
                            frameworkElement = VisualTreeHelper.GetParent(frameworkElement) as FrameworkElement;

                            var chartAdornmentPresenter = frameworkElement as ChartAdornmentPresenter;
                            if (chartAdornmentPresenter != null)
                            {
                                selectedAdornmentPresenter = chartAdornmentPresenter;
                                if (IsDraggableSeries(selectedAdornmentPresenter.Series))
                                    return;
                                if (selectedAdornmentPresenter != null && selectedAdornmentPresenter.Series is ISegmentSelectable)
                                    OnMouseMoveSelection(selectedAdornmentPresenter.Series, index);
                            }
                        }
                    }
                }
                else if (ChartArea.PreviousSelectedSeries != null && ChartArea.CurrentSelectedSeries != null && SelectionMode == Charts.SelectionMode.MouseMove
                         && ChartArea.VisibleSeries.Contains(ChartArea.PreviousSelectedSeries))
                {
                    ChangeSelectionCursor(false);
                    bool isCancel;
                    if (EnableSeriesSelection)
                        isCancel = ChartArea.CurrentSelectedSeries.RaiseSelectionChanging(-1, ChartArea.SeriesSelectedIndex);
                    else
                        isCancel = ChartArea.CurrentSelectedSeries.RaiseSelectionChanging(
                            -1,
                            (ChartArea.CurrentSelectedSeries as ISegmentSelectable).SelectedIndex);

                    if (!isCancel)
                    {
                        Deselect();
                    }
                }
                else
                    ChangeSelectionCursor(false);
            }
            else
                ChangeSelectionCursor(false);
        }
        
#if WINDOWS_UAP
        /// <summary>
        /// Called when Pointer Exited in the Chart
        /// </summary>
        protected internal override void OnPointerExited(PointerRoutedEventArgs e)
        {
            if (ChartArea != null)
            {
                ChangeSelectionCursor(false);
            }
        }
#endif

#endregion

#region Protected Internal Virtual Methods

        /// <summary>
        /// Invoked whenever the SelectionChanging event have raised.
        /// </summary>
        /// <param name="args"></param>
        protected internal virtual void OnSelectionChanging(ChartSelectionChangingEventArgs eventArgs)
        {
        }

        /// <summary>
        /// Invoked whenever the SelectionChanged event have raised.
        /// </summary>
        /// <param name="args"></param>
        protected internal virtual void OnSelectionChanged(ChartSelectionChangedEventArgs eventArgs)
        {
        }

#endregion

#region Protected Methods

        protected override DependencyObject CloneBehavior(DependencyObject obj)
        {
            return base.CloneBehavior(new ChartSelectionBehavior()
            {
                EnableSeriesSelection = this.EnableSeriesSelection,
                EnableSegmentSelection = this.EnableSegmentSelection,
                SelectionMode = this.SelectionMode,
                SelectionStyle = this.SelectionStyle,
                SelectionCursor = this.SelectionCursor,
            });
        }

#endregion

#region Private Static Methods
        
        private static void OnEnableSeriesSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SfChart chartBase = (d as ChartSelectionBehavior).ChartArea;

            if (chartBase != null && !(bool)e.NewValue)
            {
                foreach (ChartSeries series in chartBase.Series)
                {
                    if (chartBase.SelectedSeriesCollection.Contains(series))
                    {
                        chartBase.SelectedSeriesCollection.Remove(series);
                        chartBase.OnResetSeries(series);
                    }
                }

                chartBase.SeriesSelectedIndex = -1;
                chartBase.SelectedSeriesCollection.Clear();
            }
            else if (chartBase != null && (bool)e.NewValue && chartBase.SeriesSelectedIndex != -1)
                chartBase.SeriesSelectedIndexChanged(chartBase.SeriesSelectedIndex, -1);
        }

        private static void OnEnableSegmentSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartBase chartBase = (d as ChartSelectionBehavior).ChartArea;

            if (chartBase != null && !(bool)e.NewValue)
            {
                foreach (var series in chartBase.VisibleSeries)
                {
                    for (int i = 0; i < series.ActualData.Count; i++)
                    {
                        if (series.SelectedSegmentsIndexes.Contains(i))
                        {
                            series.SelectedSegmentsIndexes.Remove(i);
                            series.OnResetSegment(i);
                        }
                    }

                    var selectableSegment = series as ISegmentSelectable;
                    if (selectableSegment != null)
                        selectableSegment.SelectedIndex = -1;
                    series.SelectedSegmentsIndexes.Clear();
                }
            }
            else if (chartBase != null && (bool)e.NewValue)
            {
                for (int index = 0; index < chartBase.VisibleSeries.Count; index++)
                {
                    ChartSeriesBase series = chartBase.VisibleSeries[index];
                    var selectableSegment = series as ISegmentSelectable;

                    if (selectableSegment != null && selectableSegment.SelectedIndex != -1)
                        series.SelectedIndexChanged(selectableSegment.SelectedIndex, -1);
                }
            }
        }

        private static void OnSelectionStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSelectionBehavior behavior = d as ChartSelectionBehavior;

            SfChart chartBase = behavior.ChartArea;

            // WPF-26121, When selection style is changed the last selected segment remains selected
            if (chartBase == null || chartBase.Series == null)
                return;

            chartBase.SeriesSelectedIndex = -1;

            foreach (ChartSeries series in chartBase.Series)
            {
                var segmentSelectableSeries = series as ISegmentSelectable;
                if (segmentSelectableSeries != null)
                    segmentSelectableSeries.SelectedIndex = -1;

                if (chartBase.SelectedSeriesCollection.Contains(series) &&
                    chartBase.SeriesSelectedIndex != chartBase.Series.IndexOf(series))
                {
                    chartBase.SelectedSeriesCollection.Remove(series);
                    chartBase.OnResetSeries(series);
                }

                // Need to revisit the code.
                for (int i = 0; i < series.ActualData.Count; i++)
                {
                    if (series.SelectedSegmentsIndexes.Contains(i))
                    {
                        series.SelectedSegmentsIndexes.Remove(i);
                        series.OnResetSegment(i);
                    }
                }
            }
        }

#endregion

#region Private Methods
        
        /// <summary>
        /// In MouseMove selection, for deselecting the selected segment or series.
        /// </summary>
        private void Deselect()
        {
            (ChartArea.PreviousSelectedSeries as ISegmentSelectable).SelectedIndex = -1;
            ChartArea.SeriesSelectedIndex = -1;
            ChartArea.PreviousSelectedSeries = null;
            ChartArea.CurrentSelectedSeries = null;
            seriesCollection = null;
        }

        /// <summary>
        /// Method used to change the cursor for series and segments and adornments
        /// </summary>
        private void ChangeSelectionCursor(bool isCursorChanged)
        {
            if (isCursorChanged)
            {
#if WINDOWS_UAP && CHECKLATER
                if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
                    if (Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor.Type == CoreCursorType.Arrow)
                        Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new CoreCursor(SelectionCursor, 1);
#endif
            }
            else
            {
#if WINDOWS_UAP && CHECKLATER
                if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
                    if (Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor.Type != CoreCursorType.Arrow)
                        Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 2);

#endif
            }
        }

        /// <summary>
        /// Method used to get the bool value for series or segment has dragging base.
        /// </summary>
        private bool IsDraggableSeries(ChartSeriesBase chartSeries)
        {
            if (ChartExtensionUtils.IsDraggable(chartSeries))
            {
                ChangeSelectionCursor(false);
                return true;
            }
            else
                return false;
        }
        
        /// <summary>
        /// Method used to set SelectedIndex while mouse move in segment/adornment.
        /// </summary>
        /// <param name="series"></param>
        /// <param name="value"></param>
        private void OnMouseMoveSelection(ChartSeriesBase series, object value)
        {
            if (SelectionStyle != Charts.SelectionStyle.Single)
                return;

            ChartArea.CurrentSelectedSeries = series;

            bool seriesSelection = EnableSeriesSelection
                && (series.IsSideBySide || series is ScatterSeries
                || series is BubbleSeries
                || series is AccumulationSeriesBase
                || series is FastScatterBitmapSeries
                || (!series.IsSideBySide
                && ((!EnableSegmentSelection)
                || (EnableSegmentSelection && value == null))));

            var chartDataPointInfo = value as ChartDataPointInfo;
            if (seriesSelection)
            {
                if (ChartArea.PreviousSelectedSeries != null && ChartArea.PreviousSelectedSeries != ChartArea.CurrentSelectedSeries)
                    (ChartArea.PreviousSelectedSeries as ISegmentSelectable).SelectedIndex = -1;
                
                int seriesIndex = ChartArea.VisibleSeries.IndexOf(series);

                // Call OnSelectionChanging method to raise SelectionChanging event
                ChartArea.CurrentSelectedSeries.selectionChangingEventArgs.IsDataPointSelection = false;
                bool isCancel = ChartArea.CurrentSelectedSeries.RaiseSelectionChanging(seriesIndex, ChartArea.SeriesSelectedIndex);

                if (!isCancel)
                {
                    ChartArea.SeriesSelectedIndex = seriesIndex;
                    ChartArea.PreviousSelectedSeries = ChartArea.CurrentSelectedSeries;
                }
            }
            else if (ChartArea.CurrentSelectedSeries is ISegmentSelectable && EnableSegmentSelection
                && (value != null && value.GetType() == typeof(int) || chartDataPointInfo != null))
            {
                ChartArea.SeriesSelectedIndex = -1;

                if (ChartArea.PreviousSelectedSeries != null && ChartArea.PreviousSelectedSeries != ChartArea.CurrentSelectedSeries)
                    (ChartArea.PreviousSelectedSeries as ISegmentSelectable).SelectedIndex = -1;

                int pointIndex = value.GetType() == typeof(int) ? (int)value : chartDataPointInfo.Index;

                // Call OnSelectionChanging method to raise SelectionChanging event
                ChartArea.CurrentSelectedSeries.selectionChangingEventArgs.IsDataPointSelection = true;
                bool isCancel = ChartArea.CurrentSelectedSeries.RaiseSelectionChanging(
                    pointIndex,
                    (ChartArea.CurrentSelectedSeries as ISegmentSelectable).SelectedIndex);

                if (!isCancel)
                {
                    (ChartArea.CurrentSelectedSeries as ISegmentSelectable).SelectedIndex = pointIndex;
                    ChartArea.PreviousSelectedSeries = ChartArea.CurrentSelectedSeries;
                }
            }
            else if (ChartArea.PreviousSelectedSeries != null
                     && ChartArea.VisibleSeries.Contains(ChartArea.PreviousSelectedSeries))
            {
                bool isCancel;
                if (seriesSelection)
                    isCancel = ChartArea.CurrentSelectedSeries.RaiseSelectionChanging(
                        -1, 
                        ChartArea.SeriesSelectedIndex);
                else
                    isCancel = ChartArea.CurrentSelectedSeries.RaiseSelectionChanging(
                        -1,
                        (ChartArea.CurrentSelectedSeries as ISegmentSelectable).SelectedIndex);

                if (!isCancel)
                {
                    Deselect();
                }
            }
        }

        /// <summary>
        /// Method used to set SelectedIndex while mouse down in segment/adornment.
        /// </summary>
        /// <param name="series"></param>
        /// <param name="value"></param>
        private void OnMouseDownSelection(ChartSeriesBase series, object value)
        {
            var isScatterSeries = series is ScatterSeries;

            // Scatter series supports selection and dragging at the same time.
            if (!isScatterSeries && IsDraggableSeries(series))
                return;

            ChartArea.CurrentSelectedSeries = series;

            bool seriesSelection = EnableSeriesSelection
                && (series.IsSideBySide
                || isScatterSeries
                || series is BubbleSeries
                || series is AccumulationSeriesBase
                || series is FastScatterBitmapSeries
                || (!series.IsSideBySide && ((!EnableSegmentSelection) || (EnableSegmentSelection && value == null))));

            var chartDataPointInfo = value as ChartDataPointInfo;
            if (seriesSelection)
            {
                int index = ChartArea.VisibleSeries.IndexOf(series);
                ChartArea.CurrentSelectedSeries.selectionChangingEventArgs.IsDataPointSelection = false;

                // Call OnSelectionChanging method to raise SelectionChanging event 
                bool isCancel = ChartArea.CurrentSelectedSeries.RaiseSelectionChanging(index, ChartArea.SeriesSelectedIndex);
                if (!isCancel)
                {
                    if (SelectionStyle != Charts.SelectionStyle.Single && ChartArea.SelectedSeriesCollection.Contains(ChartArea.CurrentSelectedSeries))
                    {
                        ChartArea.SelectedSeriesCollection.Remove(ChartArea.CurrentSelectedSeries);
                        ChartArea.SeriesSelectedIndex = -1;
                        ChartArea.OnResetSeries(ChartArea.CurrentSelectedSeries as ChartSeries);
                    }
                    else if (ChartArea.SeriesSelectedIndex == index)
                        ChartArea.SeriesSelectedIndex = -1;
                    else
                    {
                        ChartArea.SeriesSelectedIndex = index;
                        ChartArea.PreviousSelectedSeries = ChartArea.CurrentSelectedSeries;
                    }
                }
            }
            else if (ChartArea.CurrentSelectedSeries is ISegmentSelectable && EnableSegmentSelection
                && ((value != null && value.GetType() == typeof(int)) || chartDataPointInfo != null))
            {
                int pointIndex = value.GetType() == typeof(int) ? (int)value : chartDataPointInfo.Index;
                ChartArea.CurrentSelectedSeries.selectionChangingEventArgs.IsDataPointSelection = true;

                // Call OnSelectionChanging method to raise SelectionChanging event  
                bool isCancel = ChartArea.CurrentSelectedSeries.RaiseSelectionChanging(
                    pointIndex,
                    (ChartArea.CurrentSelectedSeries as ISegmentSelectable).SelectedIndex);
                if (!isCancel)
                {
                    if (SelectionStyle != Charts.SelectionStyle.Single)
                    {
                        if (ChartArea.CurrentSelectedSeries.SelectedSegmentsIndexes.Contains(pointIndex))
                            ChartArea.CurrentSelectedSeries.SelectedSegmentsIndexes.Remove(pointIndex);
                        else
                        {
                            ChartArea.CurrentSelectedSeries.SelectedSegmentsIndexes.Add(pointIndex);
                            ChartArea.PreviousSelectedSeries = ChartArea.CurrentSelectedSeries;
                        }
                    }
                    else
                    {
                        ISegmentSelectable currentSelectedSeries = (ChartArea.CurrentSelectedSeries as ISegmentSelectable);
                        if (currentSelectedSeries.SelectedIndex == pointIndex)
                            currentSelectedSeries.SelectedIndex = -1;
                        else
                        {
                            currentSelectedSeries.SelectedIndex = pointIndex;
                            ChartArea.PreviousSelectedSeries = ChartArea.CurrentSelectedSeries;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method used to select bitmap series in mouse move
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private void OnBitmapSeriesMouseMoveSelection(FrameworkElement element, PointerRoutedEventArgs e)
        {
            Point canvasPoint = e.GetCurrentPoint(ChartArea.GetAdorningCanvas()).Position;

            if (seriesCollection.Count > 0)
            {
                ChartArea.CurrentSelectedSeries = seriesCollection[seriesCollection.Count - 1];
                if (IsDraggableSeries(ChartArea.CurrentSelectedSeries))
                    return;

                if (ChartArea.CurrentSelectedSeries is ISegmentSelectable)
                {
                    ChartDataPointInfo data = ChartArea.CurrentSelectedSeries.GetDataPoint(canvasPoint);
                    OnMouseMoveSelection(ChartArea.CurrentSelectedSeries, data);
                }
            }
            else if (ChartArea.PreviousSelectedSeries != null && ChartArea.VisibleSeries.Contains(ChartArea.PreviousSelectedSeries))
            {
                bool isCancel;
                if (EnableSeriesSelection)
                    isCancel = ChartArea.CurrentSelectedSeries.RaiseSelectionChanging(-1, ChartArea.SeriesSelectedIndex);
                else
                    isCancel = ChartArea.CurrentSelectedSeries.RaiseSelectionChanging(
                        -1,
                        (ChartArea.CurrentSelectedSeries as ISegmentSelectable).SelectedIndex);

                if (!isCancel)
                {
                    Deselect();
                }
            }
        }

        /// <summary>
        /// Method used to get the fast series in the mouse point
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
#if NETFX_CORE
        private void GetBitmapSeriesCollection(FrameworkElement element, PointerRoutedEventArgs e)
#else
        private void GetBitmapSeriesCollection(FrameworkElement element, MouseEventArgs e)
#endif
        {
#if __IOS__ || __ANDROID__
            Image bitmapImage = (element as Border)?.Child as Image;
#else
            Image bitmapImage = element as Image;
#endif

            PointerPoint mousePoint = e.GetCurrentPoint(element);
            int position = ((bitmapImage.Source as WriteableBitmap).PixelWidth *
                        (int)mousePoint.Position.Y + (int)mousePoint.Position.X) * 4;

            if (!ChartArea.isBitmapPixelsConverted)
                ChartArea.ConvertBitmapPixels();

            seriesCollection = (from series in ChartArea.Series
                                where (series.Pixels.Count > 0 && series.Pixels.Contains(position))
                                select series).ToList();

            if (seriesCollection.Count > 0)
            {
                foreach (ChartSeries series in seriesCollection)
                {
                    if (!series.IsLinear || EnableSeriesSelection)
                        ChangeSelectionCursor(true);
                }
            }
            else
            {
                ChangeSelectionCursor(false);
            }
        }

        /// <summary>
        /// Method used to select bitmap series in mouse down
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private void OnBitmapSeriesMouseDownSelection(FrameworkElement element, PointerRoutedEventArgs e)
        {
            Point canvasPoint = e.GetCurrentPoint(ChartArea.GetAdorningCanvas()).Position;

            if (seriesCollection.Count > 0)
            {
                ChartArea.CurrentSelectedSeries = seriesCollection[seriesCollection.Count - 1];

                if (ChartArea.CurrentSelectedSeries is ISegmentSelectable)
                {
                    ChartDataPointInfo data = ChartArea.CurrentSelectedSeries.GetDataPoint(canvasPoint);
                    OnMouseDownSelection(ChartArea.CurrentSelectedSeries, data);
                }
            }
        }

#endregion

#endregion
    }

    /// <summary>
    /// Represents chart segment selection changed event arguments.
    /// </summary>
    /// <remarks>
    /// It contains information like selected segment and series.
    /// </remarks>
    public partial class ChartSelectionChangedEventArgs : EventArgs
    {
#region Fields

        private bool isDataPointSelection = true;

#endregion

#region Properties

#region Public Properties

        /// <summary>
        /// Gets the series which has been selected through mouse interaction or SelectedIndex.
        /// </summary>
        public ChartSeriesBase SelectedSeries
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the series which had been selected through mouse interaction or SelectedIndex.
        /// </summary>
        public ChartSeriesBase PreviousSelectedSeries
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the series collection which has been selected through rectangle selection or mouse interaction.
        /// </summary>
        public List<ChartSeriesBase> SelectedSeriesCollection
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the segment which has been selected through mouse interaction or SelectedIndex.
        /// </summary>
        public ChartSegment SelectedSegment
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the segments  collection which has been selected through rectangle selection or mouse interaction.
        /// </summary>
        public List<ChartSegment> SelectedSegments
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the segments  collection which has been selected through rectangle selection or mouse interaction previously.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reviewed")]
        public List<ChartSegment> PreviousSelectedSegments
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the segment which had been selected through mouse interaction or SelectedIndex.
        /// </summary>
        public ChartSegment PreviousSelectedSegment
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the current index of the segment which has been selected through mouse interaction or SelectedIndex.
        /// </summary>
        public int SelectedIndex { get; internal set; }

        /// <summary>
        /// Gets the previous index of the segment which had been selected through mouse interaction or SelectedIndex.
        /// </summary>
        public int PreviousSelectedIndex { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the segment or series is selected.
        /// </summary>
        public bool IsSelected
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating whether the selection is segment selection or series selection.
        /// </summary>
        public bool IsDataPointSelection
        {
            get
            {
                return isDataPointSelection;
            }

            internal set
            {
                isDataPointSelection = value;
            }
        }

        /// <summary>
        /// Gets the selected segment item value.
        /// </summary>
        public object NewPointInfo
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the previous selected segment item value.
        /// </summary>
        public object OldPointInfo
        {
            get;
            internal set;
        }

#endregion
        
#endregion
    }

    /// <summary>
    /// Represents chart segment selection changing event arguments.
    /// </summary>
    /// <remarks>
    /// It contains information like selected segment and series.
    /// </remarks>
    public partial class ChartSelectionChangingEventArgs : EventArgs
    {
#region Fields

        private bool isDataPointSelection = true;
        
#endregion

#region Properties

#region Public Properties

        /// <summary>
        /// Gets the series which has been selected through mouse interaction or SelectedIndex.
        /// </summary>
        public ChartSeriesBase SelectedSeries
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the segment which has been selected through mouse interaction or SelectedIndex.
        /// </summary>
        public ChartSegment SelectedSegment
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the segments collection which has been selected through rectangle selection or mouse interaction.
        /// </summary>
        public List<ChartSegment> SelectedSegments
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the current index of the segment which has been selected through mouse interaction or SelectedIndex.
        /// </summary>
        public int SelectedIndex { get; internal set; }

        /// <summary>
        /// Gets the previous index of the segment which had been selected through mouse interaction or SelectedIndex.
        /// </summary>
        public int PreviousSelectedIndex { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether to avoid selection.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets a value indicating whether the selection is segment selection or series selection.
        /// </summary>
        public bool IsDataPointSelection
        {
            get
            {
                return isDataPointSelection;
            }

            internal set
            {
                isDataPointSelection = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the segment or series is selected.
        /// </summary>
        public bool IsSelected
        {
            get;
            internal set;
        }

#endregion
        
#endregion
    }
}
