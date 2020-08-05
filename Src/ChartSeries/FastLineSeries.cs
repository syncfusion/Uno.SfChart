using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// FastLineSeries is another version of LineSeries which uses different technology for rendering line in order to boost performance.
    /// </summary>
    /// <remarks>
    /// FastLineSeries uses polyline and is capable of rendering large amount of data in a milliseconds.
    /// </remarks>
    /// <seealso cref="FastLineSegment"/>
    /// <seealso cref="FastLineBitmapSeries"/>
    /// <seealso cref="LineSeries"/>   
    public partial class FastLineSeries : XyDataSeries, ISegmentSelectable
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="CustomTemplate"/> property.       
        /// </summary>
        public static readonly DependencyProperty CustomTemplateProperty =
            DependencyProperty.Register("CustomTemplate", typeof(DataTemplate), typeof(FastLineSeries),
            new PropertyMetadata(null, OnCustomTemplateChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeDashArray"/> property. 
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(FastLineSeries),
            new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeDashOffset"/> property. 
        /// </summary>
        public static readonly DependencyProperty StrokeDashOffsetProperty =
            DependencyProperty.Register("StrokeDashOffset", typeof(double), typeof(FastLineSeries),
            new PropertyMetadata(0d));

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeDashCap"/> property. 
        /// </summary>
        public static readonly DependencyProperty StrokeDashCapProperty =
            DependencyProperty.Register("StrokeDashCap", typeof(PenLineCap), typeof(FastLineSeries),
            new PropertyMetadata(PenLineCap.Flat));

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeLineJoin"/> property. 
        /// </summary>
        public static readonly DependencyProperty StrokeLineJoinProperty =
            DependencyProperty.Register("StrokeLineJoin", typeof(PenLineJoin), typeof(FastLineSeries),
            new PropertyMetadata(PenLineJoin.Miter));

        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSelectionBrush"/> property. 
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register("SegmentSelectionBrush", typeof(Brush), typeof(FastLineSeries),
            new PropertyMetadata(null, OnSegmentSelectionBrush));

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property. 
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(FastLineSeries),
            new PropertyMetadata(-1, OnSelectedIndexChanged));

        #endregion

        #region Fields

        #region Private Fields

        private Storyboard sb;

        private RectAnimation animation;

        private IList<double> xValues;

        private Point hitPoint = new Point();

        private ChartDataPointInfo prevDataPoint;

        bool isAdornmentPending;

        #endregion

        #endregion

        #region Properties

        #region Public Properties

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

        public DataTemplate CustomTemplate
        {
            get { return (DataTemplate)GetValue(CustomTemplateProperty); }
            set { SetValue(CustomTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke dash array forline to customize the appearance of FastLineSeries. This is a bindable property.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.Shapes.StrokeDashArray"/>.
        /// </value>
        public DoubleCollection StrokeDashArray
        {
            get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
            set { SetValue(StrokeDashArrayProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke dash offset for line to customize the appearance of FastLineSeries. This is a bindable property.
        /// </summary>
        /// <value>
        /// The double value.
        /// </value>
        public double StrokeDashOffset
        {
            get { return (double)GetValue(StrokeDashOffsetProperty); }
            set { SetValue(StrokeDashOffsetProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke dash cap for the stroke.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.Media.PenLineCap"/>.
        /// </value>
        public PenLineCap StrokeDashCap
        {
            get { return (PenLineCap)GetValue(StrokeDashCapProperty); }
            set { SetValue(StrokeDashCapProperty, value); }
        }

        /// <summary>
        /// Gets or sets the line join for the stroke of the line.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.Media.PenLineJoin"/>
        /// </value>
        public PenLineJoin StrokeLineJoin
        {
            get { return (PenLineJoin)GetValue(StrokeLineJoinProperty); }
            set { SetValue(StrokeLineJoinProperty, value); }
        }

        /// <summary>
        /// Gets or sets the interior (brush) for the selected segment(s).
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

        #endregion

        #region Protected Internal Override Properties

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
        
        private FastLineSegment Segment { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods
        
        /// <summary>
        /// Creates the segments of FastLineSeries.
        /// </summary>
        public override void CreateSegments()
        {
            if (GroupedSeriesYValues != null && GroupedSeriesYValues[0].Contains(double.NaN))
            {
                List<List<double>> yValList;
                List<List<double>> xValList;
                this.CreateEmptyPointSegments(GroupedSeriesYValues[0], out yValList, out xValList);
            }
            else if (YValues.Contains(double.NaN))
            {
                List<List<double>> yValList;
                List<List<double>> xValList;
                this.CreateEmptyPointSegments(YValues, out yValList, out xValList);
            }
            else
            {
                bool isGrouping = this.ActualXAxis is CategoryAxis ? (this.ActualXAxis as CategoryAxis).IsIndexed : true;
                if (!isGrouping)
                    xValues = GroupedXValuesIndexes;
                else
                    xValues = (ActualXValues is IList<double> && !IsIndexed) ? ActualXValues as IList<double> : GetXValues();
                if (!isGrouping)
                {
                    Segments.Clear();
                    Adornments.Clear();

                    if (Segment == null || Segments.Count == 0)
                    {
                        FastLineSegment segment = new FastLineSegment(xValues, GroupedSeriesYValues[0], this);
                        Segment = segment;
                        Segments.Add(segment);
                    }
                }
                else
                {
                    ClearUnUsedAdornments(this.DataCount);

                    if (Segment == null || Segments.Count == 0)
                    {
                        FastLineSegment segment = new FastLineSegment(xValues, YValues, this);
                        Segment = segment;
                        Segments.Add(segment);
                    }
                    else if (ActualXValues != null)
                    {
                        Segment.SetData(xValues, YValues);
                        (Segment as FastLineSegment).SetRange();
                        Segment.Item = ActualData;
                    }
                }

                isAdornmentPending = true;
            }
        }

        public override void CreateEmptyPointSegments(IList<double> YValues, out List<List<double>> yValList, out List<List<double>> xValList)
        {
            if (this.ActualXAxis is CategoryAxis && (!(this.ActualXAxis as CategoryAxis).IsIndexed))
            {
                xValues = GroupedXValuesIndexes;
                base.CreateEmptyPointSegments(GroupedSeriesYValues[0], out yValList, out xValList);
            }
            else
            {
                xValues = (ActualXValues is IList<double> && !IsIndexed) ? ActualXValues as IList<double> : GetXValues();
                base.CreateEmptyPointSegments(YValues, out yValList, out xValList);
            }

            int j = 0;

            // EmptyPoint calculation
            if (Segments.Count != yValList.Count)
            {
                Segments.Clear();
            }

            ClearUnUsedAdornments(this.DataCount);
            if (Segment == null || Segments.Count == 0)
            {
                for (int i = 0; i < yValList.Count && i < xValList.Count; i++)
                {
                    if (i < xValList.Count && i < yValList.Count && xValList[i].Count > 0 && yValList[i].Count > 0)
                    {
                        Segment = new FastLineSegment(xValList[i], yValList[i], this);
                        Segments.Add(Segment);
                    }
                }
            }
            else if (xValues != null)
            {
                foreach (var segment in Segments)
                {
                    if (j < xValList.Count && j < yValList.Count && xValList[j].Count > 0 && yValList[j].Count > 0)
                    {
                        segment.SetData(xValList[j], yValList[j]);
                        (segment as FastLineSegment).SetRange();
                    }

                    j++;
                }
            }

            isAdornmentPending = true;
        }

        #endregion

        #region Internal Override Methods

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Reviewed")]
        internal override void SelectedSegmentsIndexes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ChartSelectionChangedEventArgs chartSelectionChangedEventArgs;
            ChartSegment prevSelectedSegment = null;

            if (prevDataPoint != null && Segments.Count > 0)
                prevSelectedSegment = (from segment in Segments
                                       where segment is FastLineSegment
                                       && (segment as FastLineSegment).xChartVals.Contains(prevDataPoint.XData)
                                       select segment).FirstOrDefault() as ChartSegment;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null && !(ActualArea.SelectionBehaviour.SelectionStyle == SelectionStyle.Single))
                    {
                        int oldIndex = PreviousSelectedIndex;

                        int newIndex = (int)e.NewItems[0];

                        // For adornment selection implementation
                        if (newIndex >= 0 && ActualArea.GetEnableSegmentSelection())
                        {
                            // Selects the adornment when the mouse is over or clicked on adornments(adornment selection).
                            if (adornmentInfo != null && adornmentInfo.HighlightOnSelection)
                            {
                                UpdateAdornmentSelection(newIndex);
                            }

                            if (ActualArea != null && Segments.Count > 0)
                            {
                                var selectedSegment = (from segment in Segments
                                                       where segment is FastLineSegment
                                                       && (segment as FastLineSegment).xChartVals.Contains(dataPoint.XData)
                                                       select segment).FirstOrDefault();

                                if (selectedSegment != null)
                                {
                                    chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                                    {
                                        SelectedSegment = selectedSegment,
                                        SelectedSegments = Area.SelectedSegments,
                                        SelectedSeries = this,
                                        SelectedIndex = newIndex,
                                        PreviousSelectedIndex = oldIndex,
                                        NewPointInfo = GetDataPoint(newIndex),
                                        IsSelected = true
                                    };

                                    chartSelectionChangedEventArgs.PreviousSelectedSeries = this.ActualArea.PreviousSelectedSeries;

                                    if (oldIndex != -1)
                                    {
                                        chartSelectionChangedEventArgs.PreviousSelectedSegment = prevSelectedSegment;
                                        chartSelectionChangedEventArgs.OldPointInfo = GetDataPoint(oldIndex);
                                    }

                                    (ActualArea as SfChart).OnSelectionChanged(chartSelectionChangedEventArgs);
                                    PreviousSelectedIndex = newIndex;
                                    prevDataPoint = dataPoint;
                                }
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
                            NewPointInfo = null,
                            OldPointInfo = GetDataPoint(PreviousSelectedIndex),
                            PreviousSelectedSeries = this,
                            IsSelected = false
                        };

                        if (PreviousSelectedIndex != -1)
                            chartSelectionChangedEventArgs.PreviousSelectedSegment = prevSelectedSegment;

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
            {
                AdornmentPresenter.ResetAdornmentSelection(index, false);
            }
        }

        internal override void UpdateTooltip(object originalSource)
        {
            if (ShowTooltip)
            {
                ChartDataPointInfo dataPoint = new ChartDataPointInfo();
                if (((originalSource as FrameworkElement) is Polyline))
                {
                    dataPoint = GetDataPoint(mousePos);
                }
                else
                {
                    int index = ChartExtensionUtils.GetAdornmentIndex(originalSource);
                    if (index > -1)
                    {
                        dataPoint.Index = index;
                        if (xValues.Count > index)
                            dataPoint.XData = xValues[index];
                        if (ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed
                            && GroupedSeriesYValues[0].Count > index)
                            dataPoint.YData = GroupedSeriesYValues[0][index];
                        else if (YValues.Count > index)
                            dataPoint.YData = YValues[index];
                        dataPoint.Series = this;
                        if (ActualData.Count > index)
                            dataPoint.Item = ActualData[index];
                    }
                }

                UpdateSeriesTooltip(dataPoint);
            }
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

        internal override bool GetAnimationIsActive()
        {
            return animation != null && animation.IsActive;
        }

        internal override void Animate()
        {
            if (animation != null)
            {
                animation.Stop();

                if (!canAnimate)
                {
                    ResetAdornmentAnimationState();
                    return;
                }
            }

            var seriesRect = Area.SeriesClipRect;
            animation = new RectAnimation()
            {
                From = (IsActualTransposed) ? new Rect(0, seriesRect.Bottom, seriesRect.Width, seriesRect.Height) : new Rect(0, seriesRect.Y, 0, seriesRect.Height),
                To = (IsActualTransposed) ? new Rect(0, seriesRect.Y, 0, seriesRect.Height) : new Rect(0, seriesRect.Y, seriesRect.Width, seriesRect.Height),
                Duration = AnimationDuration.TotalSeconds == 1 ? TimeSpan.FromSeconds(0.4) : AnimationDuration
            };

            animation.SetTarget(SeriesRootPanel);
            animation.Begin();

            if (this.AdornmentsInfo != null)
            {
                sb = new Storyboard();
                double secondsPerPoint = AnimationDuration.TotalSeconds / YValues.Count;

                var adornTransXPath = "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)";
                var adornTransYPath = "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)";

                // UWP-185-RectAnimation takes some delay to render series.
                secondsPerPoint *= 1.2;
                int i = 0;

                foreach (FrameworkElement label in this.AdornmentsInfo.LabelPresenters)
                {
                    var transformGroup = label.RenderTransform as TransformGroup;
                    var scaleTransform = new ScaleTransform() { ScaleX = 0.6, ScaleY = 0.6 };

                    if (transformGroup != null)
                    {
                        if (transformGroup.Children.Count > 0 && transformGroup.Children[0] is ScaleTransform)
                        {
                            transformGroup.Children[0] = scaleTransform;
                        }
                        else
                        {
                            transformGroup.Children.Insert(0, scaleTransform);
                        }
                    }

                    label.RenderTransformOrigin = new Point(0.5, 0.5);
                    DoubleAnimation keyFrames1 = new DoubleAnimation()
                    {
                        From = 0.6,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(AnimationDuration.TotalSeconds / 2),
                        BeginTime = TimeSpan.FromSeconds(i * secondsPerPoint)
                    };

                    keyFrames1.EnableDependentAnimation = true;
                    Storyboard.SetTargetProperty(keyFrames1, adornTransXPath);
                    Storyboard.SetTarget(keyFrames1, label);
                    sb.Children.Add(keyFrames1);
                    keyFrames1 = new DoubleAnimation()
                    {
                        From = 0.6,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(AnimationDuration.TotalSeconds / 2),
                        BeginTime = TimeSpan.FromSeconds(i * secondsPerPoint)
                    };

                    keyFrames1.EnableDependentAnimation = true;
                    Storyboard.SetTargetProperty(keyFrames1, adornTransYPath);
                    Storyboard.SetTarget(keyFrames1, label);
                    sb.Children.Add(keyFrames1);
                    i++;
                }

                sb.Begin();
            }
        }

        internal override void Dispose()
        {
            if (animation != null)
            {
                animation.Stop();
                animation = null;
            }

            if (sb != null)
            {
                sb.Stop();
                sb.Children.Clear();
                sb = null;
            }
            base.Dispose();
        }

        #endregion

        #region Protected Internal Override Methods

        /// <summary>
        /// Method used to trigger SelectionChanged event to SelectedIndex segment
        /// </summary>
        /// <param name="newIndex"></param>
        /// <param name="oldIndex"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        protected internal override void SelectedIndexChanged(int newIndex, int oldIndex)
        {
            ChartSelectionChangedEventArgs chartSelectionChangedEventArgs;
            ChartSegment prevSelectedSegment = null;
            if (ActualArea != null && ActualArea.SelectionBehaviour != null)
            {
                // Resets the adornment selection when the mouse pointer moved away from the adornment or clicked the same adornment.
                if (ActualArea.SelectionBehaviour.SelectionStyle == SelectionStyle.Single)
                {
                    if (SelectedSegmentsIndexes.Contains(oldIndex))
                        SelectedSegmentsIndexes.Remove(oldIndex);

                    OnResetSegment(oldIndex);
                }

                if (prevDataPoint != null && Segments.Count > 0)
                    prevSelectedSegment = (from segment in Segments
                                           where segment is FastLineSegment
                                           && (segment as FastLineSegment).xChartVals.Contains(prevDataPoint.XData)
                                           select segment).FirstOrDefault() as ChartSegment;

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
                        var selectedSegment = (from segment in Segments
                                               where segment is FastLineSegment
                                               && (segment as FastLineSegment).xChartVals.Contains(dataPoint.XData)
                                               select segment).FirstOrDefault();

                        if (selectedSegment != null)
                        {
                            chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                            {
                                SelectedSegment = selectedSegment,
                                SelectedSegments = Area.SelectedSegments,
                                SelectedSeries = this,
                                SelectedIndex = newIndex,
                                PreviousSelectedIndex = oldIndex,
                                NewPointInfo = GetDataPoint(newIndex),
                                IsSelected = true
                            };

                            chartSelectionChangedEventArgs.PreviousSelectedSeries = this.ActualArea.PreviousSelectedSeries;

                            if (oldIndex != -1)
                            {
                                chartSelectionChangedEventArgs.PreviousSelectedSegment = prevSelectedSegment;
                                chartSelectionChangedEventArgs.OldPointInfo = GetDataPoint(oldIndex);
                            }

                            (ActualArea as SfChart).OnSelectionChanged(chartSelectionChangedEventArgs);
                            PreviousSelectedIndex = newIndex;
                            prevDataPoint = dataPoint;
                        }
                    }
                    else if (Segments.Count == 0)
                    {
                        triggerSelectionChangedEventOnLoad = true;
                    }
                }
                else if (newIndex == -1)
                {
                    chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                    {
                        SelectedSegment = null,
                        SelectedSegments = Area.SelectedSegments,
                        SelectedSeries = null,
                        SelectedIndex = newIndex,
                        PreviousSelectedIndex = oldIndex,
                        NewPointInfo = null,
                        OldPointInfo = GetDataPoint(oldIndex),
                        PreviousSelectedSeries = this,
                        IsSelected = false
                    };

                    if (oldIndex != -1)
                        chartSelectionChangedEventArgs.PreviousSelectedSegment = prevSelectedSegment;

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

        /// <summary>
        /// Called when DataSource property changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            Segment = null;
            base.OnDataSourceChanged(oldValue, newValue);
        }

        /// <summary>
        /// Called when VisibleRange property changed
        /// </summary>
        protected override void OnVisibleRangeChanged(VisibleRangeChangedEventArgs e)
        {
            if (AdornmentsInfo != null && isAdornmentPending)
            {
                if (xValues != null && ActualXAxis != null
                    && !ActualXAxis.VisibleRange.IsEmpty)
                {
                    double xBase = ActualXAxis.IsLogarithmic ? (ActualXAxis as LogarithmicAxis).LogarithmicBase : 1;
                    bool xIsLogarithmic = ActualXAxis.IsLogarithmic;
                    double start = ActualXAxis.VisibleRange.Start;
                    double end = ActualXAxis.VisibleRange.End;

                    for (int i = 0; i < DataCount; i++)
                    {
                        double x, y;
                        if (this.ActualXAxis is CategoryAxis && !(this.ActualXAxis as CategoryAxis).IsIndexed)
                        {
                            if (i < xValues.Count)
                            {
                                y = GroupedSeriesYValues[0][i];
                                x = xValues[i];
                            }
                            else
                                return;
                        }
                        else
                        {
                            x = xValues[i];
                            y = YValues[i];
                        }

                        double edgeValue = xIsLogarithmic ? Math.Log(x, xBase) : x;
                        if (edgeValue >= start && edgeValue <= end && !double.IsNaN(y))
                        {
                            if (i < Adornments.Count)
                            {
                                Adornments[i].SetData(x, y, x, y);
                                Adornments[i].Item = ActualData[i];
                            }
                            else
                            {
                                Adornments.Add(this.CreateAdornment(this, x, y, x, y));
                                Adornments[Adornments.Count - 1].Item = ActualData[i];
                            }
                        }
                    }
                }

                isAdornmentPending = false;
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new FastLineSeries()
            {
                StrokeDashArray = this.StrokeDashArray,
                StrokeDashOffset = this.StrokeDashOffset,
                StrokeLineJoin = this.StrokeLineJoin,
                SegmentSelectionBrush = this.SegmentSelectionBrush,
                SelectedIndex = this.SelectedIndex,
                StrokeDashCap = this.StrokeDashCap
            });
        }

        #endregion

        #region Private Static Methods

        private static void OnCustomTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as FastLineSeries;

            if (series.Area == null) return;

            series.Segments.Clear();
            series.Area.ScheduleUpdate();
        }

        private static void OnSegmentSelectionBrush(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FastLineSeries).UpdateArea();
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            series.OnPropertyChanged("SelectedIndex");
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
