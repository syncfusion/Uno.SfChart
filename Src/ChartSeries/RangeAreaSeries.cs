using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// RangeAreaSeries displays data points as a set of continuous lines with the areas between the high value and low value are filled in.
    /// </summary>
    /// <seealso cref="RangeAreaSegment"/>
    /// <seealso cref="RangeColumnSeries"/>
    /// <seealso cref="AreaSeries"/>
    /// <seealso cref="SplineAreaSeries"/>
    public partial class RangeAreaSeries : RangeSeriesBase, ISegmentSelectable
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="HighValueInterior"/> property.       
        /// </summary>
        public static readonly DependencyProperty HighValueInteriorProperty =
            DependencyProperty.Register(
                "HighValueInterior", 
                typeof(Brush),
                typeof(RangeAreaSeries),
                new PropertyMetadata(null, OnHighValueChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="LowValueInterior"/> property.       
        /// </summary>
        public static readonly DependencyProperty LowValueInteriorProperty =
            DependencyProperty.Register(
                "LowValueInterior",
                typeof(Brush),
                typeof(RangeAreaSeries),
                new PropertyMetadata(null, OnLowValueChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property.       
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex",
                typeof(int), 
                typeof(RangeAreaSeries),
                new PropertyMetadata(-1, OnSelectedIndexChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSelectionBrush"/> property.       
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register(
                "SegmentSelectionBrush", 
                typeof(Brush), 
                typeof(RangeAreaSeries),
                new PropertyMetadata(null, OnSegmentSelectionBrush));

        #endregion

        #region Fields

        #region Private Fields

        Storyboard sb;

        private RectAnimation animation;

        Point y1Value = new Point();

        Point y2Value = new Point();

        #endregion

        #endregion

        #region Properties

        #region Public Properties

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
        /// Gets or sets the interior brush that specifies the color for the high value segment. This is a bindable property.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush HighValueInterior
        {
            get { return (Brush)GetValue(HighValueInteriorProperty); }
            set { SetValue(HighValueInteriorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the interior brush that specifies the color for the low value segment. This is a bindable property.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush LowValueInterior
        {
            get { return (Brush)GetValue(LowValueInteriorProperty); }
            set { SetValue(LowValueInteriorProperty, value); }
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

        #region Internal Override Properties

        internal override bool IsMultipleYPathRequired
        {
            get
            {
                return true;
            }
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
        /// This property used to confirm whether it is area typed series.
        /// </summary>
        /// <remarks>
        ///  Returns <c>true</c> if its linear, otherwise it returns <c>false</c>.
        /// </remarks>
        protected internal override bool IsAreaTypeSeries
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

        #endregion

        #region Methods

        #region Pubic Override Methods

        /// <summary>
        /// Creates the segments of RangeAreaSeries
        /// </summary>
        public override void CreateSegments()
        {
            ChartPoint point1;
            ChartPoint point2;
            ChartPoint point3;
            ChartPoint point4;

            ChartPoint? crossPoint = null;
            List<ChartPoint> segPoints = new List<ChartPoint>();

            List<double> xValues = null;
            if (ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed)
                xValues = GroupedXValuesIndexes;
            else
                xValues = GetXValues();
            if (xValues != null)
            {
                bool isGrouping = this.ActualXAxis is CategoryAxis ? (this.ActualXAxis as CategoryAxis).IsIndexed : true;
                if (!isGrouping)
                {
                    Segments.Clear();
                    Adornments.Clear();

                    if (double.IsNaN(GroupedSeriesYValues[1][0]) || !double.IsNaN(GroupedSeriesYValues[0][0]))
                    {
                        segPoints.Add(new ChartPoint(xValues[0], GroupedSeriesYValues[1][0]));
                        segPoints.Add(new ChartPoint(xValues[0], GroupedSeriesYValues[0][0]));
                        var segment = new RangeAreaSegment(segPoints, false, this)
                        {
                            High = GroupedSeriesYValues[0][0],
                            Low = GroupedSeriesYValues[1][0],
                            Item = ActualData[0]
                        };
                        segment.Series = this;
                        segment.HighValueInterior = HighValueInterior;
                        segment.LowValueInterior = LowValueInterior;
                        segment.SetData(segPoints);
                        Segments.Add(segment);
                    }

                    segPoints = new List<ChartPoint>();
                    int i;

                    for (i = 0; i < xValues.Count - 1; i++)
                    {
                        if (!double.IsNaN(GroupedSeriesYValues[1][i]) && !double.IsNaN(GroupedSeriesYValues[0][i]))
                        {
                            point1 = new ChartPoint(xValues[i], GroupedSeriesYValues[1][i]);
                            point3 = new ChartPoint(xValues[i], GroupedSeriesYValues[0][i]);
                            if (i == 0 || (i < xValues.Count - 1 && (double.IsNaN(GroupedSeriesYValues[1][i - 1]) || double.IsNaN(GroupedSeriesYValues[0][i - 1]))))
                            {
                                segPoints.Add(point1);
                                segPoints.Add(point3);
                            }

                            if (!double.IsNaN(GroupedSeriesYValues[1][i + 1]) && !double.IsNaN(GroupedSeriesYValues[0][i + 1]))
                            {
                                point2 = new ChartPoint(xValues[i + 1], GroupedSeriesYValues[1][i + 1]);
                                point4 = new ChartPoint(xValues[i + 1], GroupedSeriesYValues[0][i + 1]);

                                // UWP-8718 Use ChartMath.GetCrossPoint since it returns the ChartDataPoint withou rounding the values.
                                crossPoint = ChartMath.GetCrossPoint(point1, point2, point3, point4);

                                if (crossPoint != null)
                                {
                                    var crossPointValue = new ChartPoint(crossPoint.Value.X, crossPoint.Value.Y);
                                    segPoints.Add(crossPointValue);
                                    segPoints.Add(crossPointValue);
                                    var segment = new RangeAreaSegment(
                                        segPoints,
                                        (GroupedSeriesYValues[1][i] > GroupedSeriesYValues[0][i]),
                                        this)
                                    {
                                        High = GroupedSeriesYValues[0][i],
                                        Low = GroupedSeriesYValues[1][i],
                                        Item = ActualData[i]
                                    };
                                    segment.Series = this;
                                    segment.HighValueInterior = HighValueInterior;
                                    segment.LowValueInterior = LowValueInterior;
                                    segment.SetData(segPoints);
                                    Segments.Add(segment);
                                    segPoints = new List<ChartPoint>();
                                    segPoints.Add(crossPointValue);
                                    segPoints.Add(crossPointValue);
                                }

                                segPoints.Add(point2);
                                segPoints.Add(point4);
                            }
                            else if (i != 0 && !double.IsNaN(GroupedSeriesYValues[1][i - 1]) && !double.IsNaN(GroupedSeriesYValues[0][i - 1]))
                            {
                                segPoints.Add(point1);
                                segPoints.Add(point3);
                            }
                        }
                        else
                        {
                            if (segPoints.Count > 0)
                            {
                                if (!double.IsNaN(GroupedSeriesYValues[1][i - 1]) && !double.IsNaN(GroupedSeriesYValues[0][i - 1]))
                                {
                                    var segment = new RangeAreaSegment(segPoints, false, this)
                                    {
                                        High = GroupedSeriesYValues[0][i - 1],
                                        Low = GroupedSeriesYValues[1][i - 1],
                                        Item = ActualData[i - 1]
                                    };
                                    segment.Series = this;
                                    segment.HighValueInterior = HighValueInterior;
                                    segment.LowValueInterior = LowValueInterior;
                                    segment.SetData(segPoints);
                                    Segments.Add(segment);
                                }
                            }

                            segPoints = new List<ChartPoint>();
                        }
                    }

                    if (segPoints.Count > 0)
                    {
                        var segment = new RangeAreaSegment(
                            segPoints,
                            (GroupedSeriesYValues[1][i] > GroupedSeriesYValues[0][i]),
                            this)
                        {
                            High = GroupedSeriesYValues[0][i],
                            Low = GroupedSeriesYValues[1][i],
                            Item = ActualData[i]
                        };
                        segment.Series = this;
                        segment.HighValueInterior = HighValueInterior;
                        segment.LowValueInterior = LowValueInterior;
                        segment.SetData(segPoints);
                        Segments.Add(segment);
                    }
                    else if (i == xValues.Count - 1 && (double.IsNaN(GroupedSeriesYValues[1][i]) || double.IsNaN(GroupedSeriesYValues[0][i])))
                    {
                        segPoints.Add(new ChartPoint(xValues[i], GroupedSeriesYValues[1][i]));
                        segPoints.Add(new ChartPoint(xValues[i], GroupedSeriesYValues[0][i]));
                        var segment = new RangeAreaSegment(segPoints, false, this)
                        {
                            High = GroupedSeriesYValues[0][i],
                            Low = GroupedSeriesYValues[1][i],
                            Item = ActualData[i]
                        };
                        segment.Series = this;
                        segment.HighValueInterior = HighValueInterior;
                        segment.LowValueInterior = LowValueInterior;
                        segment.SetData(segPoints);
                        Segments.Add(segment);
                    }

                    for (int j = 0; j < xValues.Count; j++)
                    {
                        if (AdornmentsInfo != null)
                            AddAdornments(xValues[j], 0, GroupedSeriesYValues[0][j], GroupedSeriesYValues[1][j], j);
                    }
                }
                else
                {
                    Segments.Clear();
                    if (AdornmentsInfo != null)
                    {
                        if (AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                            ClearUnUsedAdornments(this.DataCount * 2);
                        else
                            ClearUnUsedAdornments(this.DataCount);
                    }

                    if (xValues != null)
                    {
                        if (double.IsNaN(LowValues[0]) || !double.IsNaN(HighValues[0]))
                        {
                            segPoints.Add(new ChartPoint(xValues[0], LowValues[0]));
                            segPoints.Add(new ChartPoint(xValues[0], HighValues[0]));
                            var segment = new RangeAreaSegment(segPoints, false, this)
                            {
                                High = HighValues[0],
                                Low = LowValues[0],
                                Item = ActualData[0]
                            };
                            segment.Series = this;
                            segment.HighValueInterior = HighValueInterior;
                            segment.LowValueInterior = LowValueInterior;
                            segment.SetData(segPoints);
                            Segments.Add(segment);
                        }

                        segPoints = new List<ChartPoint>();
                        int i;
                        for (i = 0; i < DataCount - 1; i++)
                        {
                            if (!double.IsNaN(LowValues[i]) && !double.IsNaN(HighValues[i]))
                            {
                                point1 = new ChartPoint(xValues[i], LowValues[i]);
                                point3 = new ChartPoint(xValues[i], HighValues[i]);
                                if (i == 0 || (i < DataCount - 1 && (double.IsNaN(LowValues[i - 1]) || double.IsNaN(HighValues[i - 1]))))
                                {
                                    segPoints.Add(point1);
                                    segPoints.Add(point3);
                                }

                                if (!double.IsNaN(LowValues[i + 1]) && !double.IsNaN(HighValues[i + 1]))
                                {
                                    point2 = new ChartPoint(xValues[i + 1], LowValues[i + 1]);
                                    point4 = new ChartPoint(xValues[i + 1], HighValues[i + 1]);

                                    // UWP-8718 Use ChartMath.GetCrossPoint since it returns the ChartDataPoint withou rounding the values.
                                    crossPoint = ChartMath.GetCrossPoint(point1, point2, point3, point4);

                                    if (crossPoint != null)
                                    {
                                        var crossPointValue = new ChartPoint(crossPoint.Value.X, crossPoint.Value.Y);
                                        segPoints.Add(crossPointValue);
                                        segPoints.Add(crossPointValue);
                                        var segment = new RangeAreaSegment(
                                            segPoints, 
                                            (LowValues[i] > HighValues[i]),
                                            this)
                                        {
                                            High = HighValues[i],
                                            Low = LowValues[i],
                                            Item = ActualData[i]
                                        };
                                        segment.Series = this;
                                        segment.HighValueInterior = HighValueInterior;
                                        segment.LowValueInterior = LowValueInterior;
                                        segment.SetData(segPoints);
                                        Segments.Add(segment);
                                        segPoints = new List<ChartPoint>();
                                        segPoints.Add(crossPointValue);
                                        segPoints.Add(crossPointValue);
                                    }

                                    segPoints.Add(point2);
                                    segPoints.Add(point4);
                                }
                                else if (i != 0 && !double.IsNaN(LowValues[i - 1]) && !double.IsNaN(HighValues[i - 1]))
                                {
                                    segPoints.Add(point1);
                                    segPoints.Add(point3);
                                }
                            }
                            else
                            {
                                if (segPoints.Count > 0)
                                {
                                    if (!double.IsNaN(LowValues[i - 1]) && !double.IsNaN(HighValues[i - 1]))
                                    {
                                        var segment = new RangeAreaSegment(segPoints, false, this)
                                        {
                                            High = HighValues[i - 1],
                                            Low = LowValues[i - 1],
                                            Item = ActualData[i - 1]
                                        };
                                        segment.Series = this;
                                        segment.HighValueInterior = HighValueInterior;
                                        segment.LowValueInterior = LowValueInterior;
                                        segment.SetData(segPoints);
                                        Segments.Add(segment);
                                    }
                                }

                                segPoints = new List<ChartPoint>();
                            }
                        }

                        if (segPoints.Count > 0)
                        {
                            var segment = new RangeAreaSegment(segPoints, (LowValues[i] > HighValues[i]), this)
                            {
                                High = HighValues[i],
                                Low = LowValues[i],
                                Item = ActualData[i]
                            };
                            segment.Series = this;
                            segment.HighValueInterior = HighValueInterior;
                            segment.LowValueInterior = LowValueInterior;
                            segment.SetData(segPoints);
                            Segments.Add(segment);
                        }
                        else if (i == DataCount - 1 && (double.IsNaN(LowValues[i]) || double.IsNaN(HighValues[i])))
                        {
                            segPoints.Add(new ChartPoint(xValues[i], LowValues[i]));
                            segPoints.Add(new ChartPoint(xValues[i], HighValues[i]));
                            var segment = new RangeAreaSegment(segPoints, false, this)
                            {
                                High = HighValues[i],
                                Low = LowValues[i],
                                Item = ActualData[i]
                            };
                            segment.Series = this;
                            segment.HighValueInterior = HighValueInterior;
                            segment.LowValueInterior = LowValueInterior;
                            segment.SetData(segPoints);
                            Segments.Add(segment);
                        }
                    }

                    for (int i = 0; i < xValues.Count; i++)
                    {
                        if (AdornmentsInfo != null)
                            AddAdornments(xValues[i], 0, HighValues[i], LowValues[i], i);
                    }
                }
            }
        }

        #endregion

        #region Internal Override Methods

        internal override void ResetAdornmentAnimationState()
        {
            if (adornmentInfo != null)
            {
                foreach (var child in this.AdornmentPresenter.Children)
                {
                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null)
                    {
                        frameworkElement.ClearValue(FrameworkElement.RenderTransformProperty);
                        frameworkElement.ClearValue(FrameworkElement.OpacityProperty);
                    }
                }
            }
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

                if (sb != null)
                    sb.Stop();
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
            AnimateAdornments();
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

                            if (ActualArea != null && Segments.Count != 0)
                            {
                                chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                                {
                                    SelectedSegment = Segments[0],
                                    SelectedSegments = Area.SelectedSegments,
                                    SelectedSeries = this,
                                    SelectedIndex = newIndex,
                                    PreviousSelectedIndex = oldIndex,
                                    PreviousSelectedSegment = null,
                                    NewPointInfo = GetDataPoint(newIndex),
                                    IsSelected = true
                                };

                                chartSelectionChangedEventArgs.PreviousSelectedSeries = this.ActualArea.PreviousSelectedSeries;

                                if (oldIndex != -1 && oldIndex < ActualData.Count)
                                {
                                    chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[0];
                                    chartSelectionChangedEventArgs.OldPointInfo = GetDataPoint(oldIndex);
                                }

                                (ActualArea as SfChart).OnSelectionChanged(chartSelectionChangedEventArgs);
                                PreviousSelectedIndex = newIndex;
                            }
                            else
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

                        if (PreviousSelectedIndex != -1 && PreviousSelectedIndex < ActualData.Count)
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
            {
                AdornmentPresenter.ResetAdornmentSelection(index, false);
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
            IList<double> xValues = (ActualXValues is IList<double>) ? ActualXValues as IList<double> : GetXValues();

            CalculateHittestRect(mousePos, out startIndex, out endIndex, out rect);

            for (int i = startIndex; i <= endIndex; i++)
            {
                y1Value.X = IsIndexed ? i : xValues[i];
                y1Value.Y = ActualSeriesYValues[0][i];

                y2Value.X = IsIndexed ? i : xValues[i];
                y2Value.Y = ActualSeriesYValues[1][i];

                if (rect.Contains(y1Value) || rect.Contains(y2Value))
                    hitIndexes.Add(i);
            }

            if (hitIndexes.Count > 0)
            {
                int i = hitIndexes[hitIndexes.Count / 2];
                hitIndexes = null;

                dataPoint = new ChartDataPointInfo();
                dataPoint.Index = i;
                dataPoint.XData = xValues[i];
                dataPoint.High = ActualSeriesYValues[0][i];
                dataPoint.Low = ActualSeriesYValues[1][i];
                dataPoint.Series = this;
                if (i > -1 && ActualData.Count > i)
                    dataPoint.Item = ActualData[i];

                return dataPoint;
            }
            else
                return dataPoint;
        }

        internal override void UpdateTooltip(object originalSource)
        {
            if (ShowTooltip)
            {
                FrameworkElement element = originalSource as FrameworkElement;
                object chartSegment = null;
                int index = -1;

                if (element != null)
                {
                    if (element.Tag is ChartSegment)
                        chartSegment = element.Tag;
                    else if (Segments.Count > 0)
                        chartSegment = Segments[0];
                    else
                    {
                        // WPF-28526- Tooltip not shown when set the single data point with adornments for continuous series.
                        index = ChartExtensionUtils.GetAdornmentIndex(element);
                        chartSegment = index != -1 ? new RangeAreaSegment() : null;
                    }
                }

                if (chartSegment == null) return;

                SetTooltipDuration();

                var canvas = this.Area.GetAdorningCanvas();
                double xVal = 0;
                double yVal = 0;
                double stackedYValue = double.NaN;
                object data = null;
                index = 0;

                if (this.Area.SeriesClipRect.Contains(mousePos))
                {
                    var point = new Point(
                        mousePos.X - this.Area.SeriesClipRect.Left,
                        mousePos.Y - this.Area.SeriesClipRect.Top);

                    this.FindNearestChartPoint(point, out xVal, out yVal, out stackedYValue);
                    if (double.IsNaN(xVal)) return;
                    index = this.GetXValues().IndexOf(xVal);
                    data = this.ActualData[index];
                }

                if (data == null) return; // To ignore tooltip when mousePos is not inside the SeriesClipRect.
                if (this.Area.Tooltip == null)
                    this.Area.Tooltip = new ChartTooltip();
                var chartTooltip = this.Area.Tooltip as ChartTooltip;
                var rangeAreaSegment = chartSegment as RangeAreaSegment;
                rangeAreaSegment.Item = data;

                if (this.ActualSeriesYValues.Count() > 1)
                {
                    rangeAreaSegment.High = this.ActualSeriesYValues[0][index];
                    rangeAreaSegment.Low = this.ActualSeriesYValues[1][index];
                }

                if (chartTooltip != null)
                {
                    if (canvas.Children.Count == 0 || (canvas.Children.Count > 0 && !IsTooltipAvailable(canvas)))
                    {
                        chartTooltip.Content = chartSegment;
                        if (chartTooltip.Content == null)
                            return;

                        if (ChartTooltip.GetInitialShowDelay(this) == 0)
                        {
                            canvas.Children.Add(chartTooltip);
                        }

                        chartTooltip.ContentTemplate = this.GetTooltipTemplate();
                        AddTooltip();

                        if (ChartTooltip.GetEnableAnimation(this))
                        {
                            SetDoubleAnimation(chartTooltip);
                            storyBoard.Children.Add(leftDoubleAnimation);
                            storyBoard.Children.Add(topDoubleAnimation);
                            Canvas.SetLeft(chartTooltip, chartTooltip.LeftOffset);
                            Canvas.SetTop(chartTooltip, chartTooltip.TopOffset);

                            _stopwatch.Start();
                        }
                        else
                        {
                            Canvas.SetLeft(chartTooltip, chartTooltip.LeftOffset);
                            Canvas.SetTop(chartTooltip, chartTooltip.TopOffset);

                            _stopwatch.Start();
                        }
                    }
                    else
                    {
                        foreach (var child in canvas.Children)
                        {
                            var tooltip = child as ChartTooltip;
                            if (tooltip != null)
                                chartTooltip = tooltip;
                        }

                        chartTooltip.Content = chartSegment;

                        if (chartTooltip.Content == null)
                        {
                            RemoveTooltip();
                            return;
                        }

                        AddTooltip();
#if NETFX_CORE
                        if (_stopwatch.ElapsedMilliseconds > 100)
                        {
#endif
                            if (ChartTooltip.GetEnableAnimation(this))
                            {
                                _stopwatch.Restart();

                                if (leftDoubleAnimation == null || topDoubleAnimation == null || storyBoard == null)
                                {
                                    SetDoubleAnimation(chartTooltip);
                                }
                                else
                                {
                                    leftDoubleAnimation.To = chartTooltip.LeftOffset;
                                    topDoubleAnimation.To = chartTooltip.TopOffset;
                                    storyBoard.Begin();
                                }
                            }
                            else
                            {
                                _stopwatch.Restart();

                                Canvas.SetLeft(chartTooltip, chartTooltip.LeftOffset);
                                Canvas.SetTop(chartTooltip, chartTooltip.TopOffset);
                            }
#if NETFX_CORE
                        }
                        else if (EnableAnimation == false)
                        {
                            Canvas.SetLeft(chartTooltip, chartTooltip.LeftOffset);
                            Canvas.SetTop(chartTooltip, chartTooltip.TopOffset);
                        }
#endif
                    }
                }
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

                    if (ActualArea != null && Segments.Count != 0)
                    {
                        chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                        {
                            SelectedSegment = Segments[0],
                            SelectedSegments = Area.SelectedSegments,
                            SelectedSeries = this,
                            SelectedIndex = newIndex,
                            PreviousSelectedIndex = oldIndex,
                            NewPointInfo = GetDataPoint(newIndex),
                            IsSelected = true
                        };

                        chartSelectionChangedEventArgs.PreviousSelectedSeries = this.ActualArea.PreviousSelectedSeries;

                        if (oldIndex != -1 && oldIndex < ActualData.Count)
                        {
                            chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[0];
                            chartSelectionChangedEventArgs.OldPointInfo = GetDataPoint(oldIndex);
                        }

                        (ActualArea as SfChart).OnSelectionChanged(chartSelectionChangedEventArgs);
                    }
                    else
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

                    if (oldIndex != -1 && oldIndex < ActualData.Count)
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

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new RangeAreaSeries()
            {
                SegmentSelectionBrush = this.SegmentSelectionBrush,
                SelectedIndex = this.SelectedIndex,
                HighValueInterior = this.HighValueInterior,
                LowValueInterior = this.LowValueInterior
            });
        }
        
        /// <summary>
        /// Event to show tooltip
        /// </summary>
        /// <param name="e"> Event Arguments</param>
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            var canvas = this.Area.GetAdorningCanvas();
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

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the cross point.
        /// </summary>
        /// <param name="p11">The P11 value.</param>
        /// <param name="p12">The P12 value.</param>
        /// <param name="p21">The P21 value.</param>
        /// <param name="p22">The P22 value.</param>
        /// <returns>The CrossPoint</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        protected Point? GetCrossPoint(ChartPoint p11, ChartPoint p12, ChartPoint p21, ChartPoint p22)
        {
            Point pt = new Point();
            double z = (p12.Y - p11.Y) * (p21.X - p22.X) - (p21.Y - p22.Y) * (p12.X - p11.X);
            double ca = (p12.Y - p11.Y) * (p21.X - p11.X) - (p21.Y - p11.Y) * (p12.X - p11.X);
            double cb = (p21.Y - p11.Y) * (p21.X - p22.X) - (p21.Y - p22.Y) * (p21.X - p11.X);

            if ((z == 0) && (ca == 0) && (cb == 0))
            {
                return null;
            }

            double ua = ca / z;
            double ub = cb / z;

            pt.X = p11.X + (p12.X - p11.X) * ub;
            pt.Y = p11.Y + (p12.Y - p11.Y) * ub;

            if ((0 <= ua) && (ua <= 1) && (0 <= ub) && (ub <= 1))
            {
                return pt;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Private Static Methods

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

        private static void OnSegmentSelectionBrush(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RangeAreaSeries).UpdateArea();
        }

        private static void OnHighValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeAreaSeries rangeAreaSeries = d as RangeAreaSeries;

            foreach (ChartSegment segment in rangeAreaSeries.Segments)
            {
                (segment as RangeAreaSegment).HighValueInterior = rangeAreaSeries.HighValueInterior;
            }
        }

        private static void OnLowValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeAreaSeries rangeAreaSeries = d as RangeAreaSeries;

            foreach (ChartSegment segment in rangeAreaSeries.Segments)
            {
                (segment as RangeAreaSegment).LowValueInterior = rangeAreaSeries.LowValueInterior;
            }
        }

        #endregion

        #region Private Methods

        private void AnimateAdornments()
        {
            if (this.AdornmentsInfo != null)
            {
                var adornTransXPath = "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)";
                var adornTransYPath = "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)";

                sb = new Storyboard();
                double secondsPerPoint = (AnimationDuration.TotalSeconds / (HighValues.Count));

                // UWP-185-RectAnimation takes some delay to render series.
                secondsPerPoint *= 1.2;

                int i = 0;
                int j = 0;
                foreach (FrameworkElement label in this.AdornmentsInfo.LabelPresenters)
                {
                    var transformGroup = label.RenderTransform as TransformGroup;
                    var scaleTransform = new ScaleTransform() { ScaleX = 0, ScaleY = 0 };

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
                    DoubleAnimation keyFrames1 = new DoubleAnimation();
                    keyFrames1.From = 0.3;
                    keyFrames1.To = 1;
                    keyFrames1.Duration = TimeSpan.FromSeconds(AnimationDuration.TotalSeconds / 2);

                    if (adornmentInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                        keyFrames1.BeginTime = TimeSpan.FromSeconds(j * secondsPerPoint);
                    else
                        keyFrames1.BeginTime = TimeSpan.FromSeconds(i * secondsPerPoint);

                    keyFrames1.EnableDependentAnimation = true;
                    Storyboard.SetTargetProperty(keyFrames1, adornTransXPath);
                    Storyboard.SetTarget(keyFrames1, label);
                    sb.Children.Add(keyFrames1);
                    keyFrames1 = new DoubleAnimation();
                    keyFrames1.From = 0.3;
                    keyFrames1.To = 1;
                    keyFrames1.Duration = TimeSpan.FromSeconds(AnimationDuration.TotalSeconds / 2);
                    if (adornmentInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                        keyFrames1.BeginTime = TimeSpan.FromSeconds(j * secondsPerPoint);
                    else
                        keyFrames1.BeginTime = TimeSpan.FromSeconds(i * secondsPerPoint);

                    keyFrames1.EnableDependentAnimation = true;
                    Storyboard.SetTargetProperty(keyFrames1, adornTransYPath);
                    Storyboard.SetTarget(keyFrames1, label);
                    sb.Children.Add(keyFrames1);

                    i++;

                    if (adornmentInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                    {
                        if (i % 2 == 0)
                            j = j + 1;
                    }
                }

                sb.Begin();
            }
        }

        #endregion

        #endregion              
    }
}
