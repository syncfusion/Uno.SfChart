using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// FastStepLineBitmapSeries is another version of StepLineSeries which uses different technology for rendering Stepline in order to boost performance.
    /// </summary>
    /// <remarks>
    /// It uses WriteableBitmap for rendering; Its advantage is that it will render the series with large quantity of data in a fraction of milliseconds.
    /// </remarks>
    /// <seealso cref="FastStepLineBitmapSegment"/>   
    public partial class FastStepLineBitmapSeries : XyDataSeries, ISegmentSelectable
    {
        #region Dependency Property Registration

        /// <summary>
        // The DependencyProperty for <see cref="EnableAntiAliasing"/> property. 
        /// </summary>
        public static readonly DependencyProperty EnableAntiAliasingProperty =
            DependencyProperty.Register("EnableAntiAliasing", typeof(bool), typeof(FastStepLineBitmapSeries),
            new PropertyMetadata(false));

        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSelectionBrush"/> property. 
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register("SegmentSelectionBrush", typeof(Brush),
            typeof(FastStepLineBitmapSeries), new PropertyMetadata(null, OnSegmentSelectionBrush));

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property. 
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(FastStepLineBitmapSeries),
            new PropertyMetadata(-1, OnSelectedIndexChanged));

        #endregion

        #region Fields

        #region Private Fields

        private IList<double> xValues;

        private Point hitPoint = new Point();
        
        bool isAdornmentPending;

        #endregion

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets a value that indicates whether to enable anti-aliasing for <see cref="FastStepLineBitmapSeries"/>, to draw smooth edges.
        /// </summary>
        public bool EnableAntiAliasing
        {
            get { return (bool)GetValue(EnableAntiAliasingProperty); }
            set { SetValue(EnableAntiAliasingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the interior(brush) for the selected segment(s).
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        /// <example>
        ///     <code>
        ///     series.SegmentSelectionBrush = new SolidColorBrush(Colors.Red);
        ///     </code>
        /// </example>
        public Brush SegmentSelectionBrush
        {
            get { return (Brush)GetValue(SegmentSelectionBrushProperty); }
            set { SetValue(SegmentSelectionBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the index of the selected segment.
        /// </summary>
        /// <value>
        /// <c>Int</c> value represents the index of the data point(or segment) to be selected. 
        /// </value>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }


        #endregion

        #region Protected Internal Override Properties

        /// <summary>
        /// This indicates whether its a bitmap series or not.
        /// </summary>
        protected internal override bool IsBitmapSeries
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The property confirms the linearity of this series.
        /// </summary>
        /// <remarks>
        ///  Returns <c>true</c> if its linear, otherwise it returns <c>false</c>.
        /// </remarks>
        protected internal override bool IsLinear
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the selected segments in this series, when we enable the multiple selection.
        /// </summary>
        /// <returns>
        /// It returns <c>List<ChartSegment></c>.
        /// </returns>
        protected internal override List<ChartSegment> SelectedSegments
        {
            get
            {
                if (SelectedSegmentsIndexes.Count > 0)
                {
                    selectedSegments.Clear();

                    foreach (var index in SelectedSegmentsIndexes)
                    {
                        selectedSegments.Add(GetDataPoint(index));
                    }

                    return selectedSegments;
                }
                else
                    return null;
            }
        }

        #endregion

        #region Private Properties

        private FastStepLineBitmapSegment Segment { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Creates the segments of FastStepLineBitmapSeries.
        /// </summary>
        public override void CreateSegments()
        {
            bool isGrouping = this.ActualXAxis is CategoryAxis ? (this.ActualXAxis as CategoryAxis).IsIndexed : true;
            if (!isGrouping)
                xValues = GroupedXValuesIndexes;
            else
                xValues = GetXValues();
            if (!isGrouping)
            {
                Segments.Clear();
                Adornments.Clear();
                if (Segment == null || Segments.Count == 0)
                {
                    Segment = new FastStepLineBitmapSegment(xValues, GroupedSeriesYValues[0], this);
                    Segments.Add(Segment);
                }
            }
            else
            {
                ClearUnUsedAdornments(DataCount);
                if (Segment == null || Segments.Count == 0)
                {
                    Segment = new FastStepLineBitmapSegment(xValues, YValues, this);
                    Segments.Add(Segment);
                }
                else if (ActualXValues != null)
                {
                    Segment.SetData(xValues, YValues);
                    Segment.SetRange();
                    Segment.Item = ActualData;
                }
            }

            isAdornmentPending = true;
        }

        #endregion

        #region Internal Override Methods

        internal override void UpdateTooltip(object originalSource)
        {
            if (ShowTooltip)
            {
                ChartDataPointInfo dataPoint = new ChartDataPointInfo();
                int index = ChartExtensionUtils.GetAdornmentIndex(originalSource);
                if (index > -1)
                {
                    dataPoint.Index = index;
                    if (ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed)
                    {
                        if (GroupedXValuesIndexes.Count > index)
                            dataPoint.XData = GroupedXValuesIndexes[index];
                        if (GroupedSeriesYValues[0].Count > index)
                            dataPoint.YData = GroupedSeriesYValues[0][index];
                    }
                    else
                    {
                        if (xValues.Count > index)
                            dataPoint.XData = xValues[index];
                        if (YValues.Count > index)
                            dataPoint.YData = YValues[index];
                    }

                    dataPoint.Series = this;
                    if (ActualData.Count > index)
                        dataPoint.Item = ActualData[index];
                    UpdateSeriesTooltip(dataPoint);
                }
            }
        }

        internal override void SelectedSegmentsIndexes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ChartSelectionChangedEventArgs chartSelectionChangedEventArgs;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null && !(ActualArea.SelectionBehaviour.SelectionStyle == SelectionStyle.Single))
                    {
                        int oldIndex = PreviousSelectedIndex;

                        int newIndex = (int)e.NewItems[0];

                        if (newIndex >= 0 && ActualArea.GetEnableSegmentSelection())
                        {
                            // Selects the adornment when the mouse is over or clicked on adornments(adornment selection).
                            if (adornmentInfo != null && adornmentInfo.HighlightOnSelection)
                            {
                                UpdateAdornmentSelection(newIndex);
                            }

                            if (ActualArea != null && Segments.Count > 0)
                            {
                                chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                                {
                                    SelectedSegment = Segments[0],
                                    SelectedSegments = Area.SelectedSegments,
                                    SelectedSeries = this,
                                    SelectedIndex = newIndex,
                                    PreviousSelectedIndex = oldIndex,
                                    NewPointInfo = GetDataPoint(newIndex),
                                    PreviousSelectedSeries = null,
                                    PreviousSelectedSegment = null,
                                    IsSelected = true
                                };

                                chartSelectionChangedEventArgs.PreviousSelectedSeries = this.ActualArea.PreviousSelectedSeries;

                                if (oldIndex != -1)
                                {
                                    chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[0];
                                    chartSelectionChangedEventArgs.OldPointInfo = GetDataPoint(oldIndex);
                                }

                                (ActualArea as SfChart).OnSelectionChanged(chartSelectionChangedEventArgs);
                                PreviousSelectedIndex = newIndex;
                            }
                            else if (Segments.Count == 0)
                            {
                                triggerSelectionChangedEventOnLoad = true;
                            }
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null && !(ActualArea.SelectionBehaviour.SelectionStyle == SelectionStyle.Single))
                    {
                        int newIndex = (int)e.OldItems[0];

                        chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                        {
                            SelectedSegment = null,
                            SelectedSegments = Area.SelectedSegments,
                            SelectedSeries = null,
                            SelectedIndex = newIndex,
                            PreviousSelectedIndex = PreviousSelectedIndex,
                            PreviousSelectedSegment = null,
                            PreviousSelectedSeries = this,
                            IsSelected = false
                        };

                        if (PreviousSelectedIndex != -1)
                        {
                            chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[0];
                            chartSelectionChangedEventArgs.OldPointInfo = GetDataPoint(PreviousSelectedIndex);
                        }

                            (ActualArea as SfChart).OnSelectionChanged(chartSelectionChangedEventArgs);
                        OnResetSegment(newIndex);
                        PreviousSelectedIndex = newIndex;
                    }

                    break;
            }
        }

        internal override void OnResetSegment(int index)
        {
            if (index >= 0 && adornmentInfo != null)
                AdornmentPresenter.ResetAdornmentSelection(index, false);
        }

        /// <summary>
        /// This method used to gets the chart data point at a position.
        /// </summary>
        /// <param name="mousePos"></param>
        /// <returns></returns>
        internal override ChartDataPointInfo GetDataPoint(Point mousePos)
        {
            Rect rect;
            int startIndex, endIndex;
            List<int> hitIndexes = new List<int>();
            IList<double> xValues = (ActualXValues is IList<double>) ? ActualXValues as IList<double> : GetXValues();

            CalculateHittestRect(mousePos, out startIndex, out endIndex, out rect);

            for (int i = startIndex; i <= endIndex; i++)
            {
                hitPoint.X = IsIndexed ? i : xValues[i];
                hitPoint.Y = YValues[i];

                if (rect.Contains(hitPoint))
                    hitIndexes.Add(i);
            }

            if (hitIndexes.Count > 0)
            {
                int i = hitIndexes[hitIndexes.Count / 2];
                hitIndexes = null;

                dataPoint = new ChartDataPointInfo();
                dataPoint.Index = i;
                dataPoint.XData = xValues[i];
                dataPoint.YData = YValues[i];
                dataPoint.Series = this;
                if (i > -1 && ActualData.Count > i)
                    dataPoint.Item = ActualData[i];

                return dataPoint;
            }
            else
                return dataPoint;
        }

        #endregion

        #region Protected Internal Override MEthods

        /// <summary>
        /// Method used to set SegmentSelectionBrush to SelectedIndex segment
        /// </summary>
        /// <param name="newIndex"></param>
        /// <param name="oldIndex"></param>
        protected internal override void SelectedIndexChanged(int newIndex, int oldIndex)
        {
            ChartSelectionChangedEventArgs chartSelectionChangedEventArgs;
            if (ActualArea != null && ActualArea.SelectionBehaviour != null)
            {
                // Resets the adornment selection when the mouse pointer moved away from the adornment or clicked the same adornment.
                if (ActualArea.SelectionBehaviour.SelectionStyle == SelectionStyle.Single)
                {
                    if (SelectedSegmentsIndexes.Contains(oldIndex))
                        SelectedSegmentsIndexes.Remove(oldIndex);

                    OnResetSegment(oldIndex);
                }

                if (newIndex >= 0 && ActualArea.GetEnableSegmentSelection())
                {
                    if (!SelectedSegmentsIndexes.Contains(newIndex))
                        SelectedSegmentsIndexes.Add(newIndex);

                    // Selects the adornment when the mouse is over or clicked on adornments(adornment selection).
                    if (adornmentInfo != null && adornmentInfo.HighlightOnSelection)
                    {
                        UpdateAdornmentSelection(newIndex);
                    }

                    if (ActualArea != null && Segments.Count > 0)
                    {
                        chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                        {
                            SelectedSegment = Segments[0],
                            SelectedSegments = Area.SelectedSegments,
                            SelectedSeries = this,
                            SelectedIndex = newIndex,
                            PreviousSelectedIndex = oldIndex,
                            NewPointInfo = GetDataPoint(newIndex),
                            PreviousSelectedSeries = null,
                            PreviousSelectedSegment = null,
                            IsSelected = true
                        };

                        chartSelectionChangedEventArgs.PreviousSelectedSeries = this.ActualArea.PreviousSelectedSeries;

                        if (oldIndex != -1)
                        {
                            chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[0];
                            chartSelectionChangedEventArgs.OldPointInfo = GetDataPoint(oldIndex);
                        }

                        (ActualArea as SfChart).OnSelectionChanged(chartSelectionChangedEventArgs);
                        PreviousSelectedIndex = newIndex;
                    }
                    else if (Segments.Count == 0)
                    {
                        triggerSelectionChangedEventOnLoad = true;
                    }
                }
                else if (newIndex == -1 && ActualArea != null)
                {
                    chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                    {
                        SelectedSegment = null,
                        SelectedSegments = Area.SelectedSegments,
                        SelectedSeries = null,
                        SelectedIndex = newIndex,
                        PreviousSelectedIndex = oldIndex,
                        PreviousSelectedSegment = null,
                        PreviousSelectedSeries = this,
                        IsSelected = false
                    };

                    if (oldIndex != -1)
                    {
                        chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[0];
                        chartSelectionChangedEventArgs.OldPointInfo = GetDataPoint(oldIndex);
                    }

                    (ActualArea as SfChart).OnSelectionChanged(chartSelectionChangedEventArgs);
                    PreviousSelectedIndex = newIndex;
                }
            }
            else if (newIndex >= 0 && Segments.Count == 0)
            {
                triggerSelectionChangedEventOnLoad = true;
            }
        }

        #endregion

        #region Protected Override Methods

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            Canvas canvas = ActualArea.GetAdorningCanvas();

            mousePos = e.GetCurrentPoint(canvas).Position;
            if (e.Pointer.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
                UpdateTooltip(e.OriginalSource);
        }

#if NETFX_CORE
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                UpdateTooltip(e.OriginalSource);
        }
#endif

        protected override void OnVisibleRangeChanged(VisibleRangeChangedEventArgs e)
        {
            if (AdornmentsInfo != null && isAdornmentPending)
            {
                List<double> xValues = null;
                var isGrouped = ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed;

                if (isGrouped)
                    xValues = GroupedXValuesIndexes;
                else
                    xValues = GetXValues();

                if (xValues != null && ActualXAxis != null
                    && !ActualXAxis.VisibleRange.IsEmpty)
                {
                    double xBase = ActualXAxis.IsLogarithmic ? (ActualXAxis as LogarithmicAxis).LogarithmicBase : 1;
                    double start = ActualXAxis.VisibleRange.Start;
                    double end = ActualXAxis.VisibleRange.End;

                    if (isGrouped)
                    {
                        for (int i = 0; i < xValues.Count; i++)
                        {
                            if (i < xValues.Count)
                            {
                                double x = xValues[i];
                                double actualX = ActualXAxis.IsLogarithmic ? Math.Log(x, xBase) : x;

                                if (actualX >= start && actualX <= end)
                                {
                                    double y = GroupedSeriesYValues[0][i];

                                    if (i < Adornments.Count)
                                    {
                                        Adornments[i].SetData(x, y, x, y);
                                    }
                                    else
                                    {
                                        Adornments.Add(this.CreateAdornment(this, x, y, x, y));
                                    }

                                    Adornments[i].Item = ActualData[i];
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < DataCount; i++)
                        {
                            double x = xValues[i];
                            double actualX = ActualXAxis.IsLogarithmic ? Math.Log(x, xBase) : x;

                            if (actualX >= start && actualX <= end)
                            {
                                double y = YValues[i];

                                if (i < Adornments.Count)
                                {
                                    Adornments[i].SetData(x, y, x, y);
                                }
                                else
                                {
                                    Adornments.Add(this.CreateAdornment(this, x, y, x, y));
                                }

                                Adornments[i].Item = ActualData[i];
                            }
                        }
                    }
                }

                isAdornmentPending = false;
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new FastStepLineBitmapSeries()
            {
                EnableAntiAliasing = this.EnableAntiAliasing,
                SegmentSelectionBrush = this.SegmentSelectionBrush,
                SelectedIndex = this.SelectedIndex,
            });
        }

        #endregion

        #region Private Static Methods

        private static void OnSegmentSelectionBrush(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FastStepLineBitmapSeries).UpdateArea();
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.ActualArea == null || series.ActualArea.SelectionBehaviour == null) return;
            if (series.ActualArea.SelectionBehaviour.SelectionStyle == SelectionStyle.Single)
                series.SelectedIndexChanged((int)e.NewValue, (int)e.OldValue);
            else if ((int)e.NewValue != -1)
                series.SelectedSegmentsIndexes.Add((int)e.NewValue);
        }

        #endregion

        #endregion        
    }
}