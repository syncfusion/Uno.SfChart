using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Windows.UI;
using Windows.UI.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using Windows.Devices.Input;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// ChartTrackBallBehavior enables tracking of data points nearer to mouse over position or at touch contact point in a Chart.
    /// </summary>
    /// <remarks>
    /// ChartTrackBallBehavior displays a vertical line,a tracker ball symbol and a popup like control displaying information about the data point, at mouse move positions/ at touch contact positions over a <see cref="ChartSeriesBase"/>.
    /// </remarks>
    public partial class ChartTrackBallBehavior : ChartBehavior
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="AxisLabelAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty AxisLabelAlignmentProperty =
            DependencyProperty.Register(
                "AxisLabelAlignment",
                typeof(ChartAlignment),
                typeof(ChartTrackBallBehavior),
                new PropertyMetadata(ChartAlignment.Center));

        /// <summary>
        /// The DependencyProperty for <see cref="EnableAnimation"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableAnimationProperty =
            DependencyProperty.Register(
                "EnableAnimation",
                typeof(bool),
                typeof(ChartTrackBallBehavior),
                new PropertyMetadata(false));

        /// <summary>
        /// The DependencyProperty for <see cref="LabelDisplayMode"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelDisplayModeProperty =
            DependencyProperty.Register(
             "LabelDisplayMode",
             typeof(TrackballLabelDisplayMode),
             typeof(ChartTrackBallBehavior),
             new PropertyMetadata(TrackballLabelDisplayMode.FloatAllPoints));

        /// <summary>
        /// The DependencyProperty for <see cref="LineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty LineStyleProperty =
            DependencyProperty.Register(
                "LineStyle",
                typeof(Style),
                typeof(ChartTrackBallBehavior),
#if UNIVERSALWINDOWS
                null);
#else                 
                new PropertyMetadata(ChartDictionaries.GenericCommonDictionary["trackBallLineStyle"]));
#endif

        /// <summary>
        /// The DependencyProperty for <see cref="ShowLine"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowLineProperty =
            DependencyProperty.Register(
                "ShowLine",
                typeof(bool),
                typeof(ChartTrackBallBehavior),
                new PropertyMetadata(true, OnShowLinePropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="LabelVerticalAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelVerticalAlignmentProperty =
            DependencyProperty.Register(
                "LabelVerticalAlignment",
                typeof(ChartAlignment),
                typeof(ChartTrackBallBehavior),
                new PropertyMetadata(ChartAlignment.Auto));

        /// <summary>
        /// The DependencyProperty for <see cref="LabelHorizontalAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelHorizontalAlignmentProperty =
            DependencyProperty.Register(
                "LabelHorizontalAlignment",
                typeof(ChartAlignment),
                typeof(ChartTrackBallBehavior),
                new PropertyMetadata(ChartAlignment.Auto));

        /// <summary>
        /// The DependencyProperty for <see cref="ChartTrackBallStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty ChartTrackBallStyleProperty =
            DependencyProperty.Register(
                "ChartTrackBallStyle",
                typeof(Style),
                typeof(ChartTrackBallBehavior),
                null);

        /// <summary>
        /// The DependencyProperty for <see cref="UseSeriesPalette"/> property.
        /// </summary>
        public static readonly DependencyProperty UseSeriesPaletteProperty =
        DependencyProperty.Register(
            "UseSeriesPalette",
            typeof(bool),
            typeof(ChartTrackBallBehavior),
            new PropertyMetadata(false, OnLayoutUpdated));
        
        #endregion

        #region Fields

        #region Internal Fields

        internal string labelXValue;
        internal string labelYValue;
        internal bool isOpposedAxis = false;

        #endregion

        #region Private Fields

        private const int seriesTipHeight = 6;

        private const int axisTipHeight = 6;

        private const int trackLabelSpacing = 8;

        internal string previousXLabel { get; set; }

        string currentXLabel = string.Empty;

        internal string previousYLabel { get; set; } 

        string currentYLabel = string.Empty;

        private bool isActivated;

        private bool isCancel;

        private int seriesCount;

        private Border border = null;

        private int fingerCount = 0;

        private Line line;

        private List<FrameworkElement> elements;

        private ObservableCollection<ChartPointInfo> pointInfos;

        private List<ChartTrackBallControl> trackBalls;

        private ObservableCollection<ChartPointInfo> previousPointInfos;

        private double trackballWidth;

        Dictionary<double, List<double>> groupedYValues;

        List<double> seriesYVal;

        List<double> indicatorYVal;

        List<double> yValues;

        private double tempXPos = double.MinValue;

        List<double> Values = new List<double>();

        IAsyncAction updateTrackBallAction;

        private List<ContentControl> labelElements;

        private List<ContentControl> axisLabelElements;

        private List<ContentControl> groupLabelElements;

        private Dictionary<ChartAxis, ChartPointInfo> axisLabels;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for ChartrackBallBehaviour
        /// </summary>
        public ChartTrackBallBehavior()
        {
#if UNIVERSALWINDOWS
            LineStyle = ChartDictionaries.GenericCommonDictionary["trackBallLineStyle"] as Style;
#endif
            elements = new List<FrameworkElement>();
            pointInfos = new ObservableCollection<ChartPointInfo>();
            line = new Line();
            labelElements = new List<ContentControl>();
            groupLabelElements = new List<ContentControl>();
            axisLabelElements = new List<ContentControl>();
            axisLabels = new Dictionary<ChartAxis, ChartPointInfo>();
            trackBalls = new List<ChartTrackBallControl>();
            previousXLabel = string.Empty;
            previousYLabel = string.Empty;
        }

        #endregion

        #region Events

        /// <summary>
        /// Event correspond to trackball position changing. It invokes before position changing from current position to new mouse position.
        /// </summary>
        /// <remarks>
        ///     <see cref="PositionChangingEventArgs"/>
        /// </remarks>
        public event EventHandler<PositionChangingEventArgs> PositionChanging;

        /// <summary>
        /// Event correspond to trackball position changed. It invokes after position changed to new mouse pointer position
        /// </summary>
        /// <remarks>
        ///  <see cref="PositionChangedEventArgs"/>
        /// </remarks>
        public event EventHandler<PositionChangedEventArgs> PositionChanged;

        #endregion

        #region Properties

        #region Public Properties
        
        /// <summary>
        /// Gets the collection of ChartPointInfo.
        /// </summary>
        public ObservableCollection<ChartPointInfo> PointInfos
        {
            get
            {
                return pointInfos;
            }

            internal set
            {
                pointInfos = value;
            }
        }

        /// <summary>
        /// Gets or sets the alignment for the label appearing in axis.
        /// </summary>
        public ChartAlignment AxisLabelAlignment
        {
            get { return (ChartAlignment)GetValue(AxisLabelAlignmentProperty); }
            set { SetValue(AxisLabelAlignmentProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the line style
        /// </summary>
        public Style LineStyle
        {
            get { return (Style)GetValue(LineStyleProperty); }
            set { SetValue(LineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the trackball display mode for label
        /// </summary>
        public TrackballLabelDisplayMode LabelDisplayMode
        {
            get { return (TrackballLabelDisplayMode)GetValue(LabelDisplayModeProperty); }
            set { SetValue(LabelDisplayModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show/hide line.
        /// </summary>
        public bool ShowLine
        {
            get { return (bool)GetValue(ShowLineProperty); }
            set { SetValue(ShowLineProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets vertical alignment for label.
        /// </summary>
        public ChartAlignment LabelVerticalAlignment
        {
            get { return (ChartAlignment)GetValue(LabelVerticalAlignmentProperty); }
            set { SetValue(LabelVerticalAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets horizontal alignment for label.
        /// </summary>
        public ChartAlignment LabelHorizontalAlignment
        {
            get { return (ChartAlignment)GetValue(LabelHorizontalAlignmentProperty); }
            set { SetValue(LabelHorizontalAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for ChartTrackBallControl.
        /// </summary>
        public Style ChartTrackBallStyle
        {
            get { return (Style)GetValue(ChartTrackBallStyleProperty); }
            set { SetValue(ChartTrackBallStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to change the color for the labels according to the series color.
        /// </summary>
        public bool UseSeriesPalette
        {
            get { return (bool)GetValue(UseSeriesPaletteProperty); }
            set { SetValue(UseSeriesPaletteProperty, value); }
        }

        #endregion

        #region Internal Properties

        internal Point CurrentPoint { get; set; }
        
        #endregion

        #region Protected Internal Properties

        /// <summary>
        /// Gets or sets a value indicating whether the trackball IsActivated.
        /// </summary>
        protected internal bool IsActivated
        {
            get
            {
                return isActivated;
            }

            set
            {
                isActivated = value;
                Activate(isActivated);
            }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets or sets a value indicating whether to enable animation.
        /// </summary>
        private bool EnableAnimation
        {
            get { return (bool)GetValue(EnableAnimationProperty); }
            set { SetValue(EnableAnimationProperty, value); }
        }

        #endregion

        #endregion

        #region Methods

        #region Internal Methods

        internal void ScheduleTrackBallUpdate()
        {
            if (updateTrackBallAction == null)
            {
                updateTrackBallAction = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, OnPointerPositionChanged);
            }
        }

        #endregion

        #region Protected Internal Methods

        /// <summary>
        /// Called when Pointer position Changed in Chart
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Reviewed")]
        protected internal virtual void OnPointerPositionChanged()
        {
            if(ChartArea == null)
            {
                return;
            }

            updateTrackBallAction = null;

            if (!IsActivated)
            {
                return;
            }

            SetPositionChangingEventArgs();
            if (isCancel)
                return;
            previousPointInfos = new ObservableCollection<ChartPointInfo>(pointInfos);
            Point point = CurrentPoint;

            int index = 0;
            double leastX = 0;

            IEnumerable<IGrouping<ChartAxis, ChartSeriesBase>> groupedSeries = this.ChartArea.VisibleSeries.GroupBy<ChartSeriesBase, ChartAxis>((series) => series.ActualXAxis);
            PointInfos.Clear();
            axisLabels.Clear();
            int count = 0;

            foreach (IGrouping<ChartAxis, ChartSeriesBase> group in groupedSeries)
            {
                ChartAxis axis = group.Key;
                seriesCount = group.Count();
                if (axis == null)
                    continue;
                double leastXPoint = 0, leastYPoint = 0;
                double leastIndex = 0;
                double leastXVal = 0;
                double tempValue = 0;
                Values.Clear();

                foreach (ChartSeriesBase series in group)
                {
                    if (series is CartesianSeries && (!(series as CartesianSeries).ShowTrackballInfo))
                        continue;

                    bool isRangeColumnSingleValue = series is RangeColumnSeries && !series.IsMultipleYPathRequired;
                    bool isGrouping = series.ActualXAxis is CategoryAxis && !(series.ActualXAxis as CategoryAxis).IsIndexed;
                    if (series.IsActualTransposed)
                        isReversed = true;
                    else
                        isReversed = false;

                    if (series.DataCount > 0)
                    {
                        double xVal = 0;
                        double yVal = 0;
                        double stackedYValue = double.NaN;
                        double x = 0;
                        double y = 0;
                        yValues = new List<double>();
                        groupedYValues = new Dictionary<double, List<double>>();
                        bool isStackedSeries = series is StackingSeriesBase;
                        var technicalIndicator = (from indicator in ChartArea.TechnicalIndicators where indicator.ItemsSource == series.ItemsSource select indicator as FinancialTechnicalIndicator).FirstOrDefault();
                        if (isGrouping && !(series is StackingColumn100Series) && !(series is StackingBar100Series)
                            && !(series is StackingArea100Series) && !(series is WaterfallSeries) && !(series is ErrorBarSeries))
                        {
                            point = CurrentPoint;
                            double xStart = series.ActualXAxis.VisibleRange.Start;
                            double xEnd = series.ActualXAxis.VisibleRange.End;
                            point = new Point(series.ActualArea.PointToValue(series.ActualXAxis, point), series.ActualArea.PointToValue(series.ActualYAxis, point));
                            double range = Math.Round(point.X);
                            if (range <= xEnd && range >= xStart && range >= 0)
                            {
                                xVal = range;
                                if (series.DistinctValuesIndexes.Count > 0
                                    && series.DistinctValuesIndexes.ContainsKey(xVal))
                                {
                                    var value = series.DistinctValuesIndexes[xVal];
                                    for (int i = 0; i < value.Count; i++)
                                    {
                                        yValues.Add(series.GroupedSeriesYValues[0][value[i]]);
                                        if (series is FinancialSeriesBase || series is RangeSeriesBase)
                                        {
                                            if (LabelDisplayMode == TrackballLabelDisplayMode.NearestPoint)
                                            {
                                                for (int k = 1; k < series.GroupedSeriesYValues.Count(); k++)
                                                {
                                                    if (isRangeColumnSingleValue)
                                                    {
                                                        yValues.Add(series.GroupedSeriesYValues[0][value[i]]);
                                                        break;
                                                    }
                                                    
                                                    yValues.Add(series.GroupedSeriesYValues[k][value[i]]);
                                                }
                                            }
                                            else
                                            {
                                                groupedYValues.Add(i, new List<double>());
                                                for (int k = 0; k < series.GroupedSeriesYValues.Count(); k++)
                                                {
                                                    groupedYValues[i].Add(series.GroupedSeriesYValues[k][value[i]]);
                                                    
                                                    if (isRangeColumnSingleValue)
                                                    {
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (isStackedSeries)
                                    yValues.Reverse();
                            }
                        }
                        else
                        {
                            if (series is FinancialSeriesBase || series is RangeSeriesBase)
                            {
                                series.FindNearestFinancialChartPoint(technicalIndicator, point, out xVal, out seriesYVal, out indicatorYVal);
                                if (series is FinancialSeriesBase && technicalIndicator != null && technicalIndicator.ShowTrackballInfo || LabelDisplayMode != TrackballLabelDisplayMode.NearestPoint)
                                    AddYValues(series);
                                else if (LabelDisplayMode == TrackballLabelDisplayMode.NearestPoint)
                                {
                                    if (series.IsIndexed || !(series.ActualXValues is IList<double>))
                                    {
                                        if (seriesYVal.Count > 0)
                                            yValues = seriesYVal;
                                    }
                                    else
                                        yValues = GetYValuesBasedOnValue(xVal, series) as List<double>;
                                }
                            }
                            else
                            {
                                series.FindNearestChartPoint(point, out xVal, out yVal, out stackedYValue);
                                if (series is BoxAndWhiskerSeries)
                                {
                                    if (!double.IsNaN(xVal))
                                    {
                                        seriesYVal = GetYValuesBasedOnCollection(xVal, series);                                 
                                        if (LabelDisplayMode == TrackballLabelDisplayMode.NearestPoint)
                                            yValues = seriesYVal;
                                        else
                                        {
                                            var temp = seriesYVal.ToList();
                                            if (temp.Count > 0)
                                                temp.RemoveRange(0, 5);
                                            if (LabelHorizontalAlignment != ChartAlignment.Center)
                                                temp.Insert(0, seriesYVal[3]);
                                            else
                                                temp.Insert(0, seriesYVal[4]);
                                            yValues = temp;
                                        }
                                    }
                                }
                                else if (series.IsIndexed || !(series.ActualXValues is IList<double>))
                                    yValues.Add(yVal);
                                else
                                    yValues = GetYValuesBasedOnValue(xVal, series) as List<double>;
                            }
                        }

                        x = this.ChartArea.ValueToLogPoint(series.ActualXAxis, xVal);
                        for (int k = 0; k < yValues.Count; k++)
                        {
                            double yValue = yValues[k];
                            if (isGrouping && !(series is StackingColumn100Series) && !(series is StackingBar100Series)
                            && !(series is StackingArea100Series) && !(series is WaterfallSeries) && !(series is ErrorBarSeries))
                            {
                                if (series.ActualArea.VisibleSeries.IndexOf(series) == 0)
                                    tempValue = double.IsNaN(yValue) ? 0 : yValue;
                                else
                                {
                                    if (Values.Count > 0)
                                    {
                                        tempValue = yValue + Values[count];
                                        count++;
                                    }
                                    else
                                        tempValue = yValue;
                                }

                                Values.Add(tempValue);

                                if (isRangeColumnSingleValue)
                                {
                                    var yPosition = GetRangeColumnSingleYValuePosition(series, xVal);
                                    y = this.ChartArea.ValueToLogPoint(series.ActualYAxis, yPosition);
                                }
                                else
                                {
                                    y = this.ChartArea.ValueToLogPoint(series.ActualYAxis, (isStackedSeries ? tempValue : yValue));
                                }
                            }
                            else if (series is WaterfallSeries)
                            {
                                List<double> xValues = series.GetXValues();

                                if (xValues.IndexOf(xVal) == -1)
                                    break;

                                WaterfallSegment segment = series.Segments[xValues.IndexOf(xVal)] as WaterfallSegment;

                                if (series.Segments.IndexOf(segment) == 0)
                                    y = this.ChartArea.ValueToLogPoint(series.ActualYAxis, yValue);
                                else if (segment.SegmentType == WaterfallSegmentType.Sum && (series as WaterfallSeries).AllowAutoSum)
                                {
                                    yValue = segment.WaterfallSum;
                                    y = this.ChartArea.ValueToLogPoint(series.ActualYAxis, yValue);
                                }
                                else
                                    y = this.ChartArea.ValueToLogPoint(series.ActualYAxis, segment.Sum);
                            }
                            else
                            {
                                if (isStackedSeries && k != 0)
                                    stackedYValue += yValue;

                                if (isRangeColumnSingleValue)
                                {
                                    var yPosition = GetRangeColumnSingleYValuePosition(series, xVal);
                                    y = this.ChartArea.ValueToLogPoint(series.ActualYAxis, yPosition);
                                }
                                else
                                {
                                    y = this.ChartArea.ValueToLogPoint(series.ActualYAxis, (isStackedSeries ? stackedYValue : yValue));
                                }
                            }

                            if (!double.IsNaN(x) && !double.IsNaN(y))
                            {
                                if ((!(((group.ElementAt(0) == series || group.ElementAt(group.Count() - 1) == series)) && series.DataCount == 1)) || group.Count() == 1)
                                {
                                    if (index == 0)
                                        leastX = x;

                                    if (leastIndex == 0)
                                    {
                                        leastYPoint = y;
                                        leastXPoint = x;
                                        leastXVal = xVal;
                                    }
                                    
                                    if (Math.Abs(leastX - point.X) > Math.Abs(point.X - x))
                                    {
                                        leastX = x;
                                    }

                                    if (Math.Abs(leastXPoint - point.X) > Math.Abs(point.X - x))
                                    {
                                        leastXPoint = x;
                                        leastXVal = xVal;
                                    }

                                    if (Math.Abs(leastYPoint - point.Y) > Math.Abs(leastYPoint - y))
                                    {
                                        leastYPoint = y;
                                    }
                                }

                                Rect rect = new Rect(
                                    this.ChartArea.SeriesClipRect.Left - 1, 
                                    this.ChartArea.SeriesClipRect.Top - 1,
                                    this.ChartArea.SeriesClipRect.Width + 2, 
                                    this.ChartArea.SeriesClipRect.Height + 2);
                                if (isReversed)
                                {
                                    if (!rect.Contains(
                                        new Point(
                                            leastYPoint + this.ChartArea.SeriesClipRect.Left,
                                            x + this.ChartArea.SeriesClipRect.Top)))
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (!rect.Contains(
                                        new Point(
                                            leastXPoint + this.ChartArea.SeriesClipRect.Left,
                                            y + this.ChartArea.SeriesClipRect.Top)))
                                    {
                                        continue;
                                    }
                                }

                                ChartPointInfo pointInfo = new ChartPointInfo();
                                pointInfo.X = x + this.ChartArea.SeriesClipRect.Left;
                                pointInfo.Y = y + this.ChartArea.SeriesClipRect.Top;
                                pointInfo.Series = series;
                                if (series is BoxAndWhiskerSeries && k > 0 && LabelDisplayMode != TrackballLabelDisplayMode.NearestPoint)
                                {
                                    pointInfo.isOutlier = true;
                                }

                                if (series.IsIndexed)
                                {
                                    pointInfo.ValueX = series.ActualXAxis.GetLabelContent((int)xVal).ToString();
                                }
                                else
                                {
                                    pointInfo.ValueX = series.ActualXAxis.GetLabelContent(xVal).ToString();
                                }

                                labelXValue = xVal.ToString();
                                pointInfo.SeriesValues.Add(xVal.ToString());
                                int xIndex = -1;
                                if (isGrouping && series.GroupedXValuesIndexes != null && series.GroupedXValuesIndexes.Count > 0
                                   && !(series is StackingColumn100Series) && !(series is StackingBar100Series)
                                   && !(series is StackingArea100Series) && !(series is WaterfallSeries) && !(series is ErrorBarSeries)
                                   && series.DistinctValuesIndexes.ContainsKey(xVal) && !(series is WaterfallSeries))
                                {
                                    if (series.IsSideBySide && (!(series is RangeSeriesBase)) && (!(series is FinancialSeriesBase)))
                                    {
                                        if ((series.ActualXAxis as CategoryAxis).AggregateFunctions != AggregateFunctions.None)
                                            xIndex = series.GroupedXValuesIndexes.IndexOf(xVal);
                                        else
                                            xIndex = series.GroupedActualData.IndexOf(series.ActualData[(int)xVal]);
                                        pointInfo.Item = series.GroupedActualData[xIndex];
                                    }
                                    else
                                    {
                                        if ((series is FinancialSeriesBase || series is RangeSeriesBase) && LabelDisplayMode == TrackballLabelDisplayMode.NearestPoint)
                                        {
                                            int segIndex = 0;
                                            int ycount = series.GroupedSeriesYValues.Count();
                                            segIndex = k / ycount;
                                            xIndex = series.DistinctValuesIndexes[xVal][segIndex];
                                        }
                                        else
                                            xIndex = series.DistinctValuesIndexes[xVal][k];
                                    }
                                }
                                else
                                {
                                    xIndex = series.GetXValues().IndexOf(xVal);
                                    pointInfo.Item = series.ActualData[xIndex];
                                }

                                if (series is FinancialSeriesBase)
                                    pointInfo.Interior = series.GetFinancialSeriesInterior(xIndex);
                                else
                                    pointInfo.Interior = series.GetInteriorColor(xIndex);
                                pointInfo.ValueY = series.ActualYAxis.GetLabelContent(yValue).ToString();
                                labelYValue = yValue.ToString();

                                if (series is FinancialSeriesBase && !isReversed)
                                {
                                    SetPointInfoValues(series, pointInfo, isGrouping, k);

                                    if (technicalIndicator != null && technicalIndicator.ShowTrackballInfo)
                                        technicalIndicator.SetIndicatorInfo(pointInfo, indicatorYVal, UseSeriesPalette);

                                    if (series.ActualSeriesYValues.Count() > 0 && series.ActualSeriesYValues[0].Contains(yValue))
                                    {
                                        int indexofy = series.ActualSeriesYValues[0].IndexOf(yValue);
                                        for (int i = 0; i < series.ActualSeriesYValues.Count(); i++)
                                        {
                                            pointInfo.SeriesValues.Add(series.ActualSeriesYValues[i][indexofy].ToString());
                                        }
                                    }
                                }
                                else if (series is RangeSeriesBase && !isReversed)
                                {
                                    SetPointInfoValues(series, pointInfo, isGrouping, k);
                                    if (series.ActualSeriesYValues.Count() > 0 && series.ActualSeriesYValues[0].Contains(yValue))
                                    {
                                        int indexofy = series.ActualSeriesYValues[0].IndexOf(yValue);
                                        for (int i = 0; i < series.ActualSeriesYValues.Count(); i++)
                                        {
                                            pointInfo.SeriesValues.Add(series.ActualSeriesYValues[i][indexofy].ToString());
                                            
                                            if (isRangeColumnSingleValue)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (series is BubbleSeries && !isReversed)
                                {
                                    pointInfo.SeriesValues.Add(yValue.ToString());
                                    pointInfo.SeriesValues.Add((series.Segments[0] as BubbleSegment).Size.ToString());
                                }
                                else if (series is BoxAndWhiskerSeries && !isReversed)
                                {
                                    SetPointInfoValues(series, pointInfo, isGrouping, k);
                                }
                                else if (isReversed)
                                {
                                    SetPointInfoValues(series, pointInfo, isGrouping, k);
                                    pointInfo.SeriesValues.Add(yValue.ToString());
                                    pointInfo.X = this.ChartArea.SeriesClipRect.Height - (x + this.ChartArea.SeriesClipRect.Top);
                                    pointInfo.Y = y + this.ChartArea.SeriesClipRect.Left;
                                }
                                else
                                {
                                    pointInfo.SeriesValues.Add(yValue.ToString());
                                }

                                if (UseSeriesPalette)
                                {
                                    pointInfo.Foreground = new SolidColorBrush(Colors.White);
                                    pointInfo.BorderBrush = new SolidColorBrush(Colors.White);
                                }
                                else
                                {
#if NETFX_CORE
                                    pointInfo.Interior = ChartDictionaries.GenericCommonDictionary["ButtonPressedBackgroundThemeBrush"] as SolidColorBrush;
                                    pointInfo.Foreground = ChartDictionaries.GenericCommonDictionary["ButtonPressedForegroundThemeBrush"] as SolidColorBrush;
                                    pointInfo.BorderBrush = ChartDictionaries.GenericCommonDictionary["ButtonBorderThemeBrush"] as SolidColorBrush;
#endif
                                }

                                if (isReversed)
                                {
                                    double tempX = pointInfo.X;
                                    pointInfo.BaseX = pointInfo.Y;
                                    pointInfo.BaseY = ChartArea.SeriesClipRect.Height - tempX;
                                    pointInfo.HorizontalAlignment = LabelVerticalAlignment == ChartAlignment.Auto ? (pointInfo.Series is FinancialSeriesBase || (pointInfo.Series is RangeSeriesBase)) ? ChartAlignment.Near : ChartAlignment.Center
                                                                   : LabelVerticalAlignment;
                                    pointInfo.VerticalAlignment = LabelHorizontalAlignment == ChartAlignment.Auto ? (pointInfo.Series is FinancialSeriesBase || (pointInfo.Series is RangeSeriesBase)) ? ChartAlignment.Center : ChartAlignment.Far
                                                                   : LabelHorizontalAlignment;
                                }
                                else
                                {
                                    pointInfo.BaseX = pointInfo.X;
                                    pointInfo.BaseY = pointInfo.Y;
                                    pointInfo.HorizontalAlignment = LabelHorizontalAlignment == ChartAlignment.Auto ? (pointInfo.Series is FinancialSeriesBase || (pointInfo.Series is RangeSeriesBase)) ? ChartAlignment.Center : ChartAlignment.Far
                                                                                     : LabelHorizontalAlignment;
                                    pointInfo.VerticalAlignment = LabelVerticalAlignment == ChartAlignment.Auto ? (pointInfo.Series is FinancialSeriesBase || (pointInfo.Series is RangeSeriesBase)) ? ChartAlignment.Near : ChartAlignment.Center
                                                                                                        : LabelVerticalAlignment;
                                }

                                PointInfos.Add(pointInfo);

                                if (ChartArea.Series.Count > 1 && ChartArea.Series.All(chartSeries => chartSeries.DataCount == 1))
                                {
                                    leastXVal = xVal;
                                    leastX = leastXPoint = x;
                                }

                                if ((!(((group.ElementAt(0) == series || group.ElementAt(group.Count() - 1) == series)) && series.DataCount == 1)) || group.Count() == 1)
                                {
                                    index++;
                                    leastIndex++;
                                }
                            }
                        }
                    }
                }

                if (seriesCount > 1)
                {
                    ObservableCollection<ChartPointInfo> chartPointInfos;

                    if (isReversed)
                    {
                        chartPointInfos = new ObservableCollection<ChartPointInfo>((from info in PointInfos
                                                                                    where (info.X == this.ChartArea.SeriesClipRect.Height - (leastX + this.ChartArea.SeriesClipRect.Top))
                                                                                    select info));
                    }
                    else
                    {
                        chartPointInfos = new ObservableCollection<ChartPointInfo>((from info in PointInfos
                                                                                    where (Math.Abs((info.X - (leastX + this.ChartArea.SeriesClipRect.Left))) < 0.0001)
                                                                                    select info));
                    }

                    pointInfos = chartPointInfos;
                }

                if (PointInfos.Count == 0)
                    continue;
                ChartPointInfo ptInfo = new ChartPointInfo();
                ptInfo.Axis = axis;
                if (isReversed)
                {
                    if (axis.Orientation == Orientation.Vertical)
                    {
                        if (ChartArea.VisibleSeries.Count > 0 && ChartArea.VisibleSeries[0].IsIndexed)
                        {
                            ptInfo.ValueX = axis.GetLabelContent((int)leastXVal).ToString();
                        }
                        else
                        {
                            ptInfo.ValueX = axis.GetLabelContent(leastXVal).ToString();
                        }

                        if (LabelDisplayMode == TrackballLabelDisplayMode.NearestPoint)
                        {
                            foreach (ChartPointInfo pointInfo in PointInfos)
                            {
                                if (pointInfo.Y == GetNearestYValue())
                                {
                                    ptInfo.X = pointInfo.X;
                                    ptInfo.ValueX = pointInfo.ValueX;
                                    currentYLabel = pointInfo.ValueY;
                                }
                            }
                        }
                        else
                            ptInfo.X = this.ChartArea.SeriesClipRect.Height - (leastXPoint + this.ChartArea.SeriesClipRect.Top);
                    }
                    else
                    {
                        ptInfo.ValueY = axis.GetLabelContent(leastXVal).ToString();
                        ptInfo.Y = point.Y;
                    }
                }
                else
                {
                    if (axis.Orientation == Orientation.Horizontal)
                    {
                        if (ChartArea.VisibleSeries.Count > 0 && ChartArea.VisibleSeries[0].IsIndexed)
                        {
                            ptInfo.ValueX = axis.GetLabelContent((int)leastXVal).ToString();
                        }
                        else
                        {
                            ptInfo.ValueX = axis.GetLabelContent(leastXVal).ToString();
                        }

                        if (LabelDisplayMode == TrackballLabelDisplayMode.NearestPoint)
                        {
                            foreach (ChartPointInfo pointInfo in PointInfos)
                            {
                                if (pointInfo.Y == GetNearestYValue())
                                {
                                    ptInfo.X = pointInfo.X;
                                    ptInfo.ValueX = pointInfo.ValueX;
                                    currentYLabel = pointInfo.ValueY;
                                }
                            }
                        }
                        else
                            ptInfo.X = leastXPoint + this.ChartArea.SeriesClipRect.Left;
                    }
                    else
                    {
                        ptInfo.ValueY = axis.GetLabelContent(leastXVal).ToString();
                        ptInfo.Y = point.Y;
                    }
                }

                currentXLabel = ptInfo.ValueX;
                ApplyDefaultBrushes(ptInfo);

                if (isReversed)
                {
                    double tempX = ptInfo.X;
                    ptInfo.BaseX = ptInfo.Y;
                    ptInfo.BaseY = ChartArea.SeriesClipRect.Height - tempX;
                }
                else
                {
                    ptInfo.BaseX = ptInfo.X;
                    ptInfo.BaseY = ptInfo.Y;
                }

                isOpposedAxis = axis.OpposedPosition;
                axisLabels.Add(axis, ptInfo);
            }

            if (previousXLabel == currentXLabel && tempXPos == leastX)
            {
                if ((LabelDisplayMode == TrackballLabelDisplayMode.NearestPoint && previousYLabel != currentYLabel && this.ChartArea.VisibleSeries.Count > 1))
                    previousYLabel = currentYLabel;
                else
                    return;
            }
            else
                previousXLabel = currentXLabel;

            tempXPos = leastX;
            ClearItems();
            GenerateAxisLabels();
            GenerateTrackBalls();

            if (trackBalls.Count > 0)
                trackballWidth = trackBalls[0].Width;

            double xPos = leastX + this.ChartArea.SeriesClipRect.Left;

            if (isReversed && pointInfos.Count > 0)
            {
                xPos = leastX + this.ChartArea.SeriesClipRect.Top;
                line.X1 = this.ChartArea.SeriesClipRect.Left;
                line.X2 = this.ChartArea.SeriesClipRect.Left + this.ChartArea.SeriesClipRect.Width;
                line.Y2 = line.Y1 = xPos;
            }
            else if (pointInfos.Count > 0)
            {
                line.Y1 = this.ChartArea.SeriesClipRect.Top;
                line.Y2 = this.ChartArea.SeriesClipRect.Top + this.ChartArea.SeriesClipRect.Height;
                line.X1 = line.X2 = xPos;
            }

            GenerateLabels();
            elements.Add(line);

            if (labelElements != null && labelElements.Count > 1 && labelElements[0].Content is ChartPointInfo)
            {
                if (!isReversed)
                    labelElements = new List<ContentControl>(labelElements.OrderByDescending(x => (x.Content as ChartPointInfo).Y));
                else
                    labelElements = new List<ContentControl>(labelElements.OrderBy(x => (x.Content as ChartPointInfo).X));
                SmartAlignLabels();
                RenderSeriesBeakForSmartAlignment();
            }

            SetPositionChangedEventArgs();
        }

        private static double GetRangeColumnSingleYValuePosition(ChartSeriesBase series, double xValue)
        {
            var rangeSeriesBase = series as RangeSeriesBase;
            List<double> xValues = series.GetXValues();
            int i = xValues.IndexOf(xValue);
            var axisYRange = series.ActualYAxis.VisibleRange;
            var median = (axisYRange.End - Math.Abs(axisYRange.Start)) / 2;
            var segmentMiddle = (rangeSeriesBase.High == null ? rangeSeriesBase.LowValues[i] : rangeSeriesBase.HighValues[i]) / 2;
            return median + segmentMiddle;
        }

        /// <summary>
        /// Method implementation for DetachElements
        /// </summary>
        protected internal override void DetachElements()
        {
            if (this.AdorningCanvas != null)
            {
                foreach (var element in elements)
                {
                    AdorningCanvas.Children.Remove(element);
                }
            }
        }

        /// <summary>
        /// Called when Size Changed
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnSizeChanged(SizeChangedEventArgs e)
        {
            if(ChartArea == null)
            {
                return;
            }

            double y1 = this.ChartArea.ValueToLogPoint(this.ChartArea.InternalSecondaryAxis, (Convert.ToDouble(labelYValue)));
            double x1 = this.ChartArea.ValueToLogPoint(this.ChartArea.InternalPrimaryAxis, (Convert.ToDouble(labelXValue)));
            if (!double.IsNaN(y1) && !double.IsNaN(x1))
            {
                foreach (ContentControl control in labelElements)
                {
                    if (this.AdorningCanvas.Children.Contains(control))
                        this.AdorningCanvas.Children.Remove(control);
                }

                foreach (ContentControl control in axisLabelElements)
                {
                    if (this.AdorningCanvas.Children.Contains(control))
                        this.AdorningCanvas.Children.Remove(control);
                }

                foreach (Control control in trackBalls)
                {
                    if (this.AdorningCanvas.Children.Contains(control))
                        this.AdorningCanvas.Children.Remove(control);
                }

                PointInfos.Clear();

                labelElements.Clear();

                axisLabelElements.Clear();

                trackBalls.Clear();

                axisLabels.Clear();

                elements.Clear();

                CurrentPoint = new Point(x1, y1);
                OnPointerPositionChanged();
            }
        }

        /// <summary>
        /// Called when Holding the Focus in UIElement
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnHolding(HoldingRoutedEventArgs e)
        {
            if(ChartArea == null)
            {
                return;
            }

            IsActivated = true;
            if (e.PointerDeviceType == PointerDeviceType.Touch)
                ChartArea.HoldUpdate = true;

            if (this.ChartArea != null && IsActivated)
            {
                Point point = e.GetPosition(this.AdorningCanvas);

                if (ChartArea.SeriesClipRect.Contains(point))
                {
                    point = new Point(
                        point.X - ChartArea.SeriesClipRect.Left,
                        point.Y - ChartArea.SeriesClipRect.Top);

                    CurrentPoint = point;
                    OnPointerPositionChanged();
                }
            }
        }

        /// <summary>
        /// Called when Pointer pressed in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
                IsActivated = false;
            else
                fingerCount++;
        }
        
        /// <summary>
        /// Called when layout updated in chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnLayoutUpdated()
        {
            if (IsActivated)
                ScheduleTrackBallUpdate();
        }

        /// <summary>
        /// Called when Pointer moved in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal sealed override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            PointerPoint pointer = e.GetCurrentPoint(this.AdorningCanvas);
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
                if (fingerCount > 1) return;
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse &&
                !pointer.Properties.IsLeftButtonPressed)
                IsActivated = true;

            if (this.ChartArea != null && this.ChartArea.AreaType == ChartAreaType.CartesianAxes && IsActivated)
            {
                Point point = new Point(pointer.Position.X, pointer.Position.Y);

                if (ChartArea.SeriesClipRect.Contains(point))
                {
                    point = new Point(
                        point.X - ChartArea.SeriesClipRect.Left,
                        point.Y - ChartArea.SeriesClipRect.Top);

                    if (CurrentPoint.X != point.X || CurrentPoint.Y != point.Y)
                    {
                        CurrentPoint = point;
                        ScheduleTrackBallUpdate();
                    }
                }
            }
        }

        /// <summary>
        /// Called when pointer is exited.
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnPointerExited(PointerRoutedEventArgs e)
        {
            if (ChartArea == null)
            {
                return;
            }

            if (IsActivated)
            {
                IsActivated = false; // Trackball behavior when moved out of Chart area-WRT-3895
                ChartArea.HoldUpdate = false;
#if NETFX_CORE
                fingerCount--;
#endif
            }
        }

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

            if (e.Pointer.PointerDeviceType != PointerDeviceType.Mouse)
            {
                if (IsActivated)
                {
                    IsActivated = false;
                    ChartArea.HoldUpdate = false;

                }

                fingerCount--;
            }
        }
        
        protected internal override void AlignDefaultLabel(
            ChartAlignment verticalAlignemnt, 
            ChartAlignment horizontalAlignment,
            double x, 
            double y, 
            ContentControl control)
        {
            if (control != null && !double.IsInfinity(x)
                && !double.IsInfinity(y) && !double.IsNaN(x) && !double.IsNaN(y))
            {
                if (horizontalAlignment == ChartAlignment.Far && control is ContentControl)
                {
                    if (((control as ContentControl).Content as ChartPointInfo).Series != null)
                    {
                        x = x + (trackballWidth * 0.75 + seriesTipHeight - 2);
                    }

                    ((control as ContentControl).Content as ChartPointInfo).X = x;
                }

                if (horizontalAlignment == ChartAlignment.Near)
                {
                    x = x - control.DesiredSize.Width;
                    if (control is ContentControl)
                    {
                        if (((control as ContentControl).Content as ChartPointInfo).Series != null)
                        {
                            x = x - (trackballWidth * 0.75 + seriesTipHeight - 2);
                        }

                        ((control as ContentControl).Content as ChartPointInfo).X = x;
                    }
                }
                else if (horizontalAlignment == ChartAlignment.Center)
                {
                    x = x - control.DesiredSize.Width / 2;
                    if (control is ContentControl)
                        ((control as ContentControl).Content as ChartPointInfo).X = x;
                }

                if (verticalAlignemnt == ChartAlignment.Far && control is ContentControl)
                {
                    if (((control as ContentControl).Content as ChartPointInfo).Series != null)
                    {
                        y = y + (trackballWidth * 0.75 + seriesTipHeight);
                    }

                    ((control as ContentControl).Content as ChartPointInfo).Y = y;
                }

                if (verticalAlignemnt == ChartAlignment.Near)
                {
                    y = y - control.DesiredSize.Height;
                    if (control is ContentControl)
                    {
                        if (((control as ContentControl).Content as ChartPointInfo).Series != null)
                        {
                            y = y - (trackballWidth * 0.75 + seriesTipHeight);
                        }

                        ((control as ContentControl).Content as ChartPointInfo).Y = y;
                    }
                }
                else if (verticalAlignemnt == ChartAlignment.Center)
                {
                    y = y - control.DesiredSize.Height / 2;
                    if (control is ContentControl)
                        ((control as ContentControl).Content as ChartPointInfo).Y = y;
                }

                Canvas.SetLeft(control, x);
                Canvas.SetTop(control, y);
            }
        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Method implementation for AttachElements
        /// </summary>
        protected override void AttachElements()
        {
            Binding binding = new Binding();
            binding.Path = new PropertyPath("LineStyle");
            binding.Source = this;
            line.SetBinding(Line.StyleProperty, binding);

            if (this.AdorningCanvas != null && !AdorningCanvas.Children.Contains(line) && this.ShowLine)
            {
                AdorningCanvas.Children.Add(line);
                elements.Add(line);
            }
        }

        protected override DependencyObject CloneBehavior(DependencyObject obj)
        {
            return base.CloneBehavior(new ChartTrackBallBehavior()
            {
                CurrentPoint = this.CurrentPoint,
                AxisLabelAlignment = this.AxisLabelAlignment,
                LabelHorizontalAlignment = this.LabelHorizontalAlignment,
                LabelVerticalAlignment = this.LabelVerticalAlignment,
                ChartTrackBallStyle = this.ChartTrackBallStyle,
                LineStyle = this.LineStyle,
                UseSeriesPalette = this.UseSeriesPalette,
                LabelDisplayMode = this.LabelDisplayMode,
                ShowLine = this.ShowLine,
            });
        }

        #endregion

        #region Protected Virtual Methods
        
        /// <summary>
        /// Called when Pointer position Changed
        /// </summary>
        /// <param name="point"></param>
        protected virtual void OnPointerPositionChanged(Point point)
        {
            CurrentPoint = point;
            OnPointerPositionChanged();
        }

        /// <summary>
        /// Method implementation for GenerateLabels 
        /// </summary>
        protected virtual void GenerateLabels()
        {
            if (PointInfos.Count == 0)
                return;

            if (LabelDisplayMode == TrackballLabelDisplayMode.GroupAllPoints)
            {
                AddGroupLabels();
                return;
            }

            foreach (ChartPointInfo pointInfo in PointInfos)
            {
                bool canAddLabel = pointInfo.Series.IsActualTransposed ? ChartArea.SeriesClipRect.Contains(new Point(pointInfo.Y, pointInfo.X)) : ChartArea.SeriesClipRect.Contains(new Point(pointInfo.X, pointInfo.Y));
                if (canAddLabel)
                {
                    if (LabelDisplayMode == TrackballLabelDisplayMode.FloatAllPoints)
                    {
                        if (seriesCount > 1 && PointInfos.Any(info => info.Series.IsSideBySide))
                        {
                            AddGroupLabels();
                            break;
                        }
                        else
                            AddLabel(
                                pointInfo, 
                                pointInfo.VerticalAlignment,
                                pointInfo.HorizontalAlignment,
                                GetLabelTemplate(pointInfo));
                    }
                    else if (LabelDisplayMode == TrackballLabelDisplayMode.NearestPoint)
                    {
                        if (pointInfo.Y == GetNearestYValue())
                        {
                            if (isReversed)
                                line.Y1 = line.Y2 = ChartArea.SeriesClipRect.Height - pointInfo.X;
                            else
                                line.X1 = line.X2 = pointInfo.X;
                            AddLabel(
                                pointInfo, 
                                pointInfo.VerticalAlignment, 
                                pointInfo.HorizontalAlignment,
                                GetLabelTemplate(pointInfo));
                            return;
                        }
                    }
                }
            }
        }

        protected virtual void AddGroupLabels()
        {
            foreach (ChartPointInfo pointInfo in PointInfos)
            {
                ContentControl control = new ContentControl();
                control.Content = pointInfo;
#if WINDOWS_UAP
                pointInfo.Foreground = UseSeriesPalette ? pointInfo.Interior : new SolidColorBrush(Colors.Black);
#else
                pointInfo.Foreground = UseSeriesPalette ? pointInfo.Interior : pointInfo.Foreground;
#endif
                if (pointInfo.Series is FinancialSeriesBase || pointInfo.Series is RangeSeriesBase || pointInfo.Series is BoxAndWhiskerSeries)
                    control.ContentTemplate = GetLabelTemplate(pointInfo);
                else
                {
                    if ((pointInfo.Series.TrackBallLabelTemplate != null && UseSeriesPalette && pointInfo.Series.Tag != null &&
                         pointInfo.Series.Tag.Equals("FromTheme")) || pointInfo.Series.TrackBallLabelTemplate == null)
                    {
                        control.ContentTemplate = ChartDictionaries.GenericCommonDictionary["groupLabel"] as DataTemplate;
                    }
                    else
                    {
                        control.ContentTemplate = pointInfo.Series.TrackBallLabelTemplate;
                    }
                }

                groupLabelElements.Add(control);
            }

            var stackPanel = new StackPanel { Orientation = Orientation.Vertical };
            stackPanel.Margin = new Thickness(3, 0, 3, 0);

            foreach (var label in groupLabelElements)
            {
                stackPanel.Children.Add(label);
                if (groupLabelElements.Count > 1 && label != groupLabelElements.Last())
                {
                    Rectangle separator = new Rectangle();
                    separator.Fill = new SolidColorBrush(Colors.Gray);
                    separator.StrokeThickness = 1;
                    separator.Height = 0.5;
                    label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    separator.Width = label.Width;
                    stackPanel.Children.Add(separator);
                }
            }

            border = new Border();
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            border.BorderThickness = new Thickness(1);
            border.Background = new SolidColorBrush(Colors.White);
            border.CornerRadius = new CornerRadius(1);
            border.Child = stackPanel;
            AddElement(border);
            ArrangeGroupLabel();
        }

        /// <summary>
        /// Method implementation for AddLabels in Chart
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="verticalAlignment"></param>
        /// <param name="horizontalAlignment"></param>
        /// <param name="template"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected virtual void AddLabel(
            object obj,
            ChartAlignment verticalAlignment, 
            ChartAlignment horizontalAlignment,
            DataTemplate template, 
            double x, 
            double y)
        {
            if (template == null)
                return;
            ContentControl control = new ContentControl();
            control.Content = obj;
            control.IsHitTestVisible = false;
            control.ContentTemplate = template;
            AddElement(control);
            control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var pointInfo = control.Content as ChartPointInfo;

            if (CanApplyDefaultTemplate(obj))
            {
                if (pointInfo == null)
                    return;

                // Align Series labels.
                if (pointInfo.Series != null)
                {
                    // The labels get's aligned for the smart labels as well as for the boundry conditions.
                    labelElements.Add(control);

                    // Please note inversed case for the axis labels are already done.         
                    AlignDefaultLabel(verticalAlignment, horizontalAlignment, x, y, control);
                    ArrangeLabelsOnBounds(control, true);
                    AlignSeriesToolTipPolygon(control);
                }
                else if (pointInfo.Axis != null)
                {
                    // Align Axis labels.
                    axisLabelElements.Add(control);
                    if (isReversed)
                        AlignDefaultLabel(horizontalAlignment, verticalAlignment, y, ChartArea.SeriesClipRect.Height - x, control);
                    else
                        AlignDefaultLabel(verticalAlignment, horizontalAlignment, x, y, control);
                    AlignAxisToolTipPolygon(control, verticalAlignment, horizontalAlignment, x, y, this);
                }
            }
            else
            {
                // Check for the case of datatemplate labels.

                if (pointInfo == null || (pointInfo != null && pointInfo.Series == null && pointInfo.Axis == null))
                {
                    // Check for the case of custom labels.
                    labelElements.Add(control);
                    if (isReversed)
                        AlignElement(control, horizontalAlignment, verticalAlignment, y, ChartArea.SeriesClipRect.Height - x);
                    else
                        AlignElement(control, verticalAlignment, horizontalAlignment, x, y);
                }
                else
                {
                    if (pointInfo != null)
                    {
                        // Align Series labels.
                        if (pointInfo.Series != null)
                        {
                            labelElements.Add(control);
                            AlignElement(control, verticalAlignment, horizontalAlignment, x, y);
                            ArrangeLabelsOnBounds(control, false);
                            AlignSeriesToolTipPolygon(control);
                        }
                        else if (pointInfo.Axis != null)
                        {
                            // Align axis labels.
                            axisLabelElements.Add(control);
                            if (isReversed)
                                AlignElement(control, horizontalAlignment, verticalAlignment, y, ChartArea.SeriesClipRect.Height - x);
                            else
                                AlignElement(control, verticalAlignment, horizontalAlignment, x, y);
                            AlignAxisToolTipPolygon(control, verticalAlignment, horizontalAlignment, x, y, this);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method implementation for generate TrackBalls
        /// </summary>
        protected virtual void GenerateTrackBalls()
        {
            foreach (ChartPointInfo pointInfo in PointInfos)
            {
                bool canAddTrackball = pointInfo.Series.IsActualTransposed ? ChartArea.SeriesClipRect.Contains(new Point(pointInfo.Y, pointInfo.X)) : ChartArea.SeriesClipRect.Contains(new Point(pointInfo.X, pointInfo.Y));
                if (canAddTrackball)
                {
                    if (LabelDisplayMode == TrackballLabelDisplayMode.FloatAllPoints || LabelDisplayMode == TrackballLabelDisplayMode.GroupAllPoints)
                    {
                        if (pointInfo.Series is FinancialSeriesBase || pointInfo.Series is RangeSeriesBase)
                            GenerateAdditionalTrackball(pointInfo);
                        else
                            AddTrackBall(pointInfo);
                    }
                    else if (LabelDisplayMode == TrackballLabelDisplayMode.NearestPoint)
                    {
                        if (pointInfo.Y == GetNearestYValue())
                        {
                            AddTrackBall(pointInfo);
                            return;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Method implementation for Add Trackball to Corresponding chartpoint
        /// </summary>
        /// <param name="pointInfo"></param>
        protected virtual void AddTrackBall(ChartPointInfo pointInfo)
        {
            ChartTrackBallControl control = new ChartTrackBallControl(pointInfo.Series);

            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("ChartTrackBallStyle");
            control.SetBinding(ChartTrackBallControl.StyleProperty, binding);

            trackBalls.Add(control);
            AddElement(control);
            control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            if (isReversed)
            {
                AlignElement(control, ChartAlignment.Center, ChartAlignment.Center, pointInfo.Y, ChartArea.SeriesClipRect.Height - pointInfo.X);
            }
            else
            {
                AlignElement(control, ChartAlignment.Center, ChartAlignment.Center, pointInfo.X, pointInfo.Y);
            }
        }

        #endregion

        #region Protected Methods
        
        /// <summary>
        /// Return collection of double values from the given ChartSeries
        /// </summary>
        /// <param name="x"></param>
        /// <param name="series"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        protected IList<double> GetYValuesBasedOnValue(double x, ChartSeriesBase series)
        {
            List<double> Values = new List<double>();
            List<double> xValues = series.ActualXValues as List<double>;

            for (int i = 0; i < series.DataCount; i++)
            {
                if (xValues[i] == x)
                {
                    foreach (var item in series.ActualSeriesYValues)
                    {
                        Values.Add(item[i]);                
                        var rangeSeriesBase = series as RangeSeriesBase; 

                        if (rangeSeriesBase != null)
                        {
                            if (rangeSeriesBase is RangeColumnSeries && (string.IsNullOrEmpty(rangeSeriesBase.High) || string.IsNullOrEmpty(rangeSeriesBase.Low)))
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return Values;
        }
        
        /// <summary>
        /// Method implementation for Clearitems in ChartTrackBallbehaviour
        /// </summary>
        protected void ClearItems()
        {
            foreach (ContentControl control in labelElements)
            {
                if (this.AdorningCanvas.Children.Contains(control))
                    this.AdorningCanvas.Children.Remove(control);
            }

            foreach (ContentControl control in axisLabelElements)
            {
                if (this.AdorningCanvas.Children.Contains(control))
                    this.AdorningCanvas.Children.Remove(control);
            }

            foreach (Control control in trackBalls)
            {
                if (this.AdorningCanvas.Children.Contains(control))
                    this.AdorningCanvas.Children.Remove(control);
            }

            foreach (ContentControl control in groupLabelElements)
            {
                if (this.AdorningCanvas.Children.Contains(control))
                    this.AdorningCanvas.Children.Remove(control);
            }

            if (AdorningCanvas != null && this.AdorningCanvas.Children.Contains(border))
                this.AdorningCanvas.Children.Remove(border);
            groupLabelElements.Clear();

            labelElements.Clear();

            axisLabelElements.Clear();

            trackBalls.Clear();

            line.ClearUIValues();

            elements.Clear();
        }

        /// <summary>
        /// Mathod implementation for Add labels in Chart
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="verticalAlignment"></param>
        /// <param name="horizontalAlignment"></param>
        /// <param name="template"></param>
        protected void AddLabel(ChartPointInfo obj, ChartAlignment verticalAlignment, ChartAlignment horizontalAlignment, DataTemplate template)
        {
            if (obj != null && template != null)
            {
                if (obj.Series == null)
                    AddLabel(obj, verticalAlignment, horizontalAlignment, template, obj.X, obj.Y);
                else
                    AddLabel(obj, verticalAlignment, horizontalAlignment, template, obj.BaseX, obj.BaseY);
            }
        }

        /// <summary>
        /// Method implementation for Add UIElements 
        /// </summary>
        /// <param name="element"></param>
        protected void AddElement(UIElement element)
        {
            if (!this.AdorningCanvas.Children.Contains(element))
            {
                this.AdorningCanvas.Children.Add(element);
                elements.Add(element as FrameworkElement);
            }
        }

        #endregion

        #region Private Static Methods
        
        private static void OnShowLinePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartTrackBallBehavior).OnShowLinePropertyChanged(e);
        }

        private static void OnLayoutUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartTrackBallBehavior).OnLayoutUpdated();
        }
        
        private static List<double> GetYValuesBasedOnCollection(double x, ChartSeriesBase series)
        {
            List<double> Values = new List<double>();
            List<double> xValues = series.ActualXValues as List<double>;

            var boxSeries = series as BoxAndWhiskerSeries;

            // To prevent the scatter segments from retriving.
            var boxSegments = series.Segments.Where(segment => segment is BoxAndWhiskerSegment);

            int i = 0;
            if (boxSeries.IsIndexed || xValues == null)
            {
                foreach (BoxAndWhiskerSegment segment in boxSegments)
                {
                    if (i == x)
                    {
                        GetSegmentValues(segment, Values);
                        break;
                    }

                    i++;
                }
            }
            else
            {
                foreach (BoxAndWhiskerSegment segment in boxSegments)
                {
                    if (xValues[i] == x)
                    {
                        GetSegmentValues(segment, Values);
                        break;
                    }

                    i++;
                }
            }

            return Values;
        }

        private static void GetSegmentValues(BoxAndWhiskerSegment boxWhiskerSegment, List<double> labelValues)
        {
            labelValues.Add(boxWhiskerSegment.Median);
            labelValues.Add(boxWhiskerSegment.LowerQuartile);
            labelValues.Add(boxWhiskerSegment.Minimum);
            labelValues.Add(boxWhiskerSegment.UppperQuartile);
            labelValues.Add(boxWhiskerSegment.Maximum);
            foreach (var outlier in boxWhiskerSegment.Outliers)
            {
                labelValues.Add(outlier);
            }
        }
        private static ChartAlignment GetChartAlignment(bool isOpposed, ChartAlignment alignment)
        {
            if (isOpposed)
            {
                if (alignment == ChartAlignment.Near)
                    return ChartAlignment.Far;
                else if (alignment == ChartAlignment.Far)
                    return ChartAlignment.Near;
                else
                    return ChartAlignment.Center;
            }
            else
                return alignment;
        }
        
        /// <summary>
        /// To determine whether two labels are collided or not.
        /// </summary>
        /// <param name="previousLabel"></param>
        /// <param name="currentLabel"></param>
        /// <returns></returns>
        private static bool CheckLabelCollision(ContentControl previousLabel, ContentControl currentLabel)
        {
            Rect rect1 = GetRenderedRect(previousLabel);
            Rect rect2 = GetRenderedRect(currentLabel);

            return CheckLabelCollisionRect(rect1, rect2);
        }

        /// <summary>
        /// To determine whether two labels are collided or not.
        /// </summary>
        /// <param name="previousLabel"></param>
        /// <param name="currentLabel"></param>
        /// <returns></returns>
        private static bool CheckLabelCollisionRect(Rect rect1, Rect rect2)
        {
            return !(Math.Round((rect1.Y + rect1.Height), 2) <= Math.Round((rect2.Y), 2) ||
                                Math.Round(rect1.Y, 2) >= Math.Round(rect2.Y + rect2.Height, 2) ||
                                Math.Round(rect1.X + rect1.Width, 2) <= Math.Round(rect2.X, 2) ||
                                Math.Round(rect1.X, 2) >= (Math.Round(rect2.X + rect2.Width)));
        }

        /// <summary>
        /// To get the rendered rect of the label.
        /// </summary>
        /// <param name="control">Label's content control.</param>
        /// <returns></returns>
        private static Rect GetRenderedRect(ContentControl control)
        {
            ChartPointInfo pointInfo = control.Content as ChartPointInfo;
            return new Rect(pointInfo.X, pointInfo.Y, control.DesiredSize.Width, control.DesiredSize.Height);
        }

        private static bool CanApplyDefaultTemplate(object obj)
        {
            var chartPointInfo = obj as ChartPointInfo;
            if (chartPointInfo != null
                &&
                (chartPointInfo.Series != null &&
                 ((chartPointInfo.Series.TrackBallLabelTemplate != null &&
                  chartPointInfo.Series.Tag != null && chartPointInfo.Series.Tag.Equals("FromTheme")) ||
                 chartPointInfo.Series.TrackBallLabelTemplate == null)))
                return true;
            else if (chartPointInfo != null
                     &&
                     (chartPointInfo.Axis != null &&
                      ((chartPointInfo.Axis.TrackBallLabelTemplate != null &&
                       chartPointInfo.Axis.Tag != null && chartPointInfo.Axis.Tag.Equals("FromTheme")) ||
                      chartPointInfo.Axis.TrackBallLabelTemplate == null)))
                return true;
            return false;
        }

        private static void ApplyDefaultBrushes(object obj)
        {
            var chartPointInfo = obj as ChartPointInfo;
#if NETFX_CORE
            chartPointInfo.Interior = ChartDictionaries.GenericCommonDictionary["ButtonPressedBackgroundThemeBrush"] as SolidColorBrush;
            chartPointInfo.Foreground = ChartDictionaries.GenericCommonDictionary["ButtonPressedForegroundThemeBrush"] as SolidColorBrush;
            chartPointInfo.BorderBrush = ChartDictionaries.GenericCommonDictionary["ButtonBorderThemeBrush"] as SolidColorBrush;
#endif
        }

        private static void AlignElement(
            Control control,
            ChartAlignment verticalAlignemnt,
            ChartAlignment horizontalAlignment,
           double x,
           double y)
        {
            if (control != null && !double.IsInfinity(x)
                && !double.IsInfinity(y) && !double.IsNaN(x) && !double.IsNaN(y))
            {
                if (horizontalAlignment == ChartAlignment.Near)
                {
                    x = x - control.DesiredSize.Width;
                }
                else if (horizontalAlignment == ChartAlignment.Center)
                {
                    x = x - control.DesiredSize.Width / 2;
                }

                if (verticalAlignemnt == ChartAlignment.Near)
                {
                    y = y - control.DesiredSize.Height;
                }
                else if (verticalAlignemnt == ChartAlignment.Center)
                {
                    y = y - control.DesiredSize.Height / 2;
                }

                var contentControl = control as ContentControl;
                if (contentControl != null)
                {
                    var chartPointInfo = contentControl.Content as ChartPointInfo;
                    if (chartPointInfo != null)
                    {
                        chartPointInfo.X = x;
                        chartPointInfo.Y = y;
                    }
                }

                Canvas.SetLeft(control, x);
                Canvas.SetTop(control, y);
            }
        }

        #endregion

        #region Private Methods
        
        private void OnShowLinePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                if ((bool)e.NewValue)
                    AttachElements();
                else if (this.AdorningCanvas != null)
                {
                    DetachElement(line);
                    elements.Remove(line);
                }
            }
        }

        private void SetPositionChangedEventArgs()
        {
            PositionChangedEventArgs e = new PositionChangedEventArgs();
            e.CurrentPointInfos = PointInfos;
            e.PreviousPointInfos = previousPointInfos;
            if (PositionChanged != null)
                PositionChanged(this, e);
        }

        private void SetPositionChangingEventArgs()
        {
            PositionChangingEventArgs e = new PositionChangingEventArgs();
            e.PointInfos = PointInfos;
            if (PositionChanging != null)
                PositionChanging(this, e);
            isCancel = e.Cancel;
        }

        private void SetPointInfoValues(ChartSeriesBase series, ChartPointInfo pointInfo, bool isGrouping, int yCount)
        {
            var isFinancialSerisBase = series is FinancialSeriesBase;
            if (isFinancialSerisBase || series is RangeSeriesBase)
            {
                if (series is RangeColumnSeries && !series.IsMultipleYPathRequired)
                {
                    pointInfo.ValueY = yValues[0].ToString();
                }
                else
                {
                    pointInfo.High = isGrouping ? yCount < groupedYValues.Values.Count() ? groupedYValues[yCount][0].ToString() : null
                                  : seriesYVal[0].ToString();

                    pointInfo.Low = isGrouping ? yCount < groupedYValues.Values.Count() ? groupedYValues[yCount][1].ToString() : null
                                     : seriesYVal[1].ToString();
                }

                if (isFinancialSerisBase)
                {
                    pointInfo.Open = isGrouping ? yCount < groupedYValues.Values.Count() ? groupedYValues[yCount][2].ToString() : null
                              : seriesYVal[2].ToString();

                    pointInfo.Close = isGrouping ? yCount < groupedYValues.Values.Count() ? groupedYValues[yCount][3].ToString() : null
                                    : seriesYVal[3].ToString();
                }
            }

            if (series is BoxAndWhiskerSeries)
            {
                if (seriesYVal.Count > 0)
                {
                    pointInfo.Median = seriesYVal[0].ToString();
                    pointInfo.Open = seriesYVal[1].ToString();
                    pointInfo.Close = seriesYVal[3].ToString();
                    pointInfo.Low = seriesYVal[2].ToString();
                    pointInfo.High = seriesYVal[4].ToString();
                }
            }
        }

        private void AddYValues(ChartSeriesBase series)
        {
            if (seriesYVal.Count > 0)
            {
                if (LabelHorizontalAlignment != ChartAlignment.Center && series is FinancialSeriesBase)
                {
                    if (seriesYVal[2] > seriesYVal[3])
                        yValues.Add(seriesYVal[2]);
                    else
                        yValues.Add(seriesYVal[3]);
                }
                else
                    yValues.Add(seriesYVal[0]);
            }
        }

        private void RenderSeriesBeakForSmartAlignment()
        {
            foreach (var label in labelElements)
            {
                AlignSeriesToolTipPolygon(label as ContentControl);
            }
        }

        private void ArrangeLabelsOnBounds(ContentControl label, bool withPolygon)
        {
            bool leftCollision = false;
            bool rightCollision = false;

            ChartPointInfo pointInfo = label.Content as ChartPointInfo;

            // Calculate the shift width for the tips.
            double xFactor = (trackballWidth * 0.75 + seriesTipHeight - 2) * 2;
            double yFactor = (trackballWidth * 0.75 + seriesTipHeight) * 2;

            double left = pointInfo.X;
            double top = pointInfo.Y;

            if (ChartArea.SeriesClipRect.Left > pointInfo.X)
                leftCollision = true;
            if (ChartArea.SeriesClipRect.Right < pointInfo.X + label.DesiredSize.Width)
                rightCollision = true;

            // Left Collision
            if (ChartArea.SeriesClipRect.Left > left)
            {
                if (pointInfo.HorizontalAlignment == ChartAlignment.Center)
                    pointInfo.X += withPolygon ? (label.DesiredSize.Width + xFactor) / 2 : label.DesiredSize.Width / 2;
                else
                    pointInfo.X += withPolygon ? label.DesiredSize.Width + xFactor : label.DesiredSize.Width;
                left = pointInfo.X;
                pointInfo.HorizontalAlignment = ChartAlignment.Far;
            }

            // Top Collision
            if (ChartArea.SeriesClipRect.Top > top)
            {
                if (pointInfo.VerticalAlignment == ChartAlignment.Center)
                    pointInfo.Y += withPolygon ? (label.DesiredSize.Height + yFactor) / 2 : label.DesiredSize.Height / 2;
                else
                    pointInfo.Y += withPolygon ? label.DesiredSize.Height + yFactor : label.DesiredSize.Height;
                top = pointInfo.Y;
                pointInfo.VerticalAlignment = ChartAlignment.Far;

                if (pointInfo.HorizontalAlignment == ChartAlignment.Center && !leftCollision)
                {
                    if (!rightCollision)
                    {
                        pointInfo.X = withPolygon ? pointInfo.X + ((label.DesiredSize.Width + xFactor) / 2) : pointInfo.X + label.DesiredSize.Width / 2;
                        pointInfo.HorizontalAlignment = ChartAlignment.Far;
                    }
                    else
                    {
                        pointInfo.X = withPolygon ? pointInfo.X - ((label.DesiredSize.Width + xFactor) / 2) : pointInfo.X - label.DesiredSize.Width / 2;
                        pointInfo.HorizontalAlignment = ChartAlignment.Near;
                    }

                    left = pointInfo.X;
                }
            }

            // Right Collision
            if (ChartArea.SeriesClipRect.Right < left + label.DesiredSize.Width)
            {
                if (pointInfo.HorizontalAlignment == ChartAlignment.Center)
                    pointInfo.X = withPolygon ? pointInfo.X - (label.DesiredSize.Width + xFactor) / 2 : pointInfo.X - label.DesiredSize.Width / 2;
                else
                    pointInfo.X = withPolygon ? pointInfo.X - label.DesiredSize.Width - xFactor : pointInfo.X - label.DesiredSize.Width;

                left = pointInfo.X;
                pointInfo.HorizontalAlignment = ChartAlignment.Near;
            }

            // Bottom Collision
            if (ChartArea.SeriesClipRect.Bottom < top + label.DesiredSize.Height)
            {
                if (pointInfo.VerticalAlignment == ChartAlignment.Center)
                    pointInfo.Y = withPolygon ? pointInfo.Y - (label.DesiredSize.Height + yFactor) / 2 : pointInfo.Y - label.DesiredSize.Height / 2;
                else
                    pointInfo.Y = withPolygon ? pointInfo.Y - label.DesiredSize.Height - yFactor : pointInfo.Y - label.DesiredSize.Height;

                top = pointInfo.Y;
                pointInfo.VerticalAlignment = ChartAlignment.Near;
            }

            Canvas.SetLeft(label, left);
            Canvas.SetTop(label, top);
        }

        private void ArrangeAxisLabelsOnBounds(ContentControl label)
        {
            ChartPointInfo pointInfo = label.Content as ChartPointInfo;

            if (!isReversed)
            {
                if (ChartArea.SeriesClipRect.Left > pointInfo.X)
                    pointInfo.X += ChartArea.SeriesClipRect.Left - pointInfo.X;
                if (ChartArea.SeriesClipRect.Right < pointInfo.X + label.DesiredSize.Width)
                    pointInfo.X -= (pointInfo.X + label.DesiredSize.Width) - ChartArea.SeriesClipRect.Right;
            }
            else
            {
                if (ChartArea.SeriesClipRect.Top > pointInfo.Y)
                    pointInfo.Y += ChartArea.SeriesClipRect.Top - pointInfo.Y;
                if (ChartArea.SeriesClipRect.Bottom < pointInfo.Y + label.DesiredSize.Height)
                    pointInfo.Y -= (pointInfo.Y + label.DesiredSize.Height) - ChartArea.SeriesClipRect.Bottom;
            }

            Canvas.SetLeft(label, pointInfo.X);
            Canvas.SetTop(label, pointInfo.Y);
        }

        private void GenerateAxisLabels()
        {
            foreach (KeyValuePair<ChartAxis, ChartPointInfo> keyVal in axisLabels)
            {
                ChartAxis axis = keyVal.Key;
                axis.ActualTrackBallLabelTemplate = ChartDictionaries.GenericCommonDictionary["axisTrackBallLabel"] as DataTemplate;
                var trackBallLabelTemplate = axis.TrackBallLabelTemplate ?? axis.ActualTrackBallLabelTemplate;
                ChartPointInfo pointInfo = keyVal.Value;
                if (axis.ShowTrackBallInfo)
                {
                    chartAxis = axis;
                    if (isReversed)
                    {
                        if (axis.Orientation == Orientation.Vertical)
                        {
                            if (axis.TrackBallLabelTemplate == null || (axis.Tag != null && axis.Tag.Equals("FromTheme")))
                            {
                                pointInfo.Y = axis.OpposedPosition ? axis.ArrangeRect.Left + axisTipHeight : axis.ArrangeRect.Right - axisTipHeight;
                                AddLabel(
                                    pointInfo,
                                    GetChartAlignment(axis.OpposedPosition, ChartAlignment.Near),
                                    AxisLabelAlignment,
                                    trackBallLabelTemplate);
                            }
                            else
                            {
                                pointInfo.Y = axis.OpposedPosition ? axis.ArrangeRect.Right : axis.ArrangeRect.Left;
                                AddLabel(
                                    pointInfo, 
                                    GetChartAlignment(axis.OpposedPosition, ChartAlignment.Far),
                                    AxisLabelAlignment, 
                                    trackBallLabelTemplate);
                            }
                        }
                        else
                        {
                            // Need to revisit the codes since else condition is not hit
                            pointInfo.X = axis.OpposedPosition ? axis.ArrangeRect.Left : axis.ArrangeRect.Right;
                            AddLabel(
                                pointInfo, 
                                AxisLabelAlignment,
                                GetChartAlignment(axis.OpposedPosition, ChartAlignment.Near),
                                trackBallLabelTemplate);
                        }
                    }
                    else
                    {
                        // Need to revisit the codes since this if condition is not hit.
                        if (axis.Orientation == Orientation.Vertical)
                        {
                            pointInfo.X = axis.OpposedPosition ? axis.ArrangeRect.Left : axis.ArrangeRect.Right;
                            AddLabel(
                                pointInfo, 
                                AxisLabelAlignment,
                                GetChartAlignment(axis.OpposedPosition, ChartAlignment.Near), 
                                trackBallLabelTemplate);
                        }
                        else
                        {
                            if (axis.TrackBallLabelTemplate == null || (axis.Tag != null && axis.Tag.Equals("FromTheme")))
                            {
                                pointInfo.Y = axis.OpposedPosition ? axis.ArrangeRect.Bottom - axisTipHeight : axis.ArrangeRect.Top + axisTipHeight;
                                AddLabel(
                                    pointInfo, 
                                    GetChartAlignment(axis.OpposedPosition, ChartAlignment.Far),
                                    AxisLabelAlignment, 
                                    trackBallLabelTemplate);
                            }
                            else
                            {
                                pointInfo.Y = axis.OpposedPosition ? axis.ArrangeRect.Bottom : axis.ArrangeRect.Top;
                                AddLabel(
                                    pointInfo, 
                                    GetChartAlignment(axis.OpposedPosition, ChartAlignment.Far),
                                    AxisLabelAlignment, 
                                    trackBallLabelTemplate);
                            }
                        }
                    }
                }
            }
        }

        private double GetNearestYValue()
        {
            double preValue = Double.MaxValue;
            var yValues = (from info in PointInfos select info.Y);
            double y = 0d;
            foreach (double value in yValues)
            {
                var diff = isReversed ? Math.Abs(value - (CurrentPoint.X + this.ChartArea.SeriesClipRect.Left)) :
                      Math.Abs(value - CurrentPoint.Y);
                if (diff < preValue)
                {
                    y = value;
                    preValue = diff;
                }
            }

            return y;
        }

        private DataTemplate GetLabelTemplate(ChartPointInfo pointInfo)
        {
            DataTemplate trackBallLabelTemplate = null;

            var technicalIndicator = (from indicator in ChartArea.TechnicalIndicators where indicator.ItemsSource == pointInfo.Series.ItemsSource select indicator as FinancialTechnicalIndicator).FirstOrDefault();
            if (pointInfo.Series is FinancialSeriesBase && technicalIndicator != null && technicalIndicator.ShowTrackballInfo)
            {
                var macdTechnicalIndicator = technicalIndicator as MACDTechnicalIndicator;
                if (technicalIndicator is BollingerBandIndicator)
                    pointInfo.Series.ActualTrackBallLabelTemplate = ChartDictionaries.GenericCommonDictionary["bollingerBandTrackBallLabel"] as DataTemplate;
                else if (technicalIndicator is StochasticTechnicalIndicator)
                    pointInfo.Series.ActualTrackBallLabelTemplate = ChartDictionaries.GenericCommonDictionary["stochasticTrackBallLabel"] as DataTemplate;
                else if (macdTechnicalIndicator != null)
                {
                    if (macdTechnicalIndicator.Type == MACDType.Both)
                        pointInfo.Series.ActualTrackBallLabelTemplate = ChartDictionaries.GenericCommonDictionary["macd_both_TrackBallLabel"] as DataTemplate;
                    if (macdTechnicalIndicator.Type == MACDType.Line)
                        pointInfo.Series.ActualTrackBallLabelTemplate = ChartDictionaries.GenericCommonDictionary["macd_line_TrackBallLabel"] as DataTemplate;
                    if (macdTechnicalIndicator.Type == MACDType.Histogram)
                        pointInfo.Series.ActualTrackBallLabelTemplate = ChartDictionaries.GenericCommonDictionary["macd_histogram_TrackBallLabel"] as DataTemplate;
                }
                else
                    pointInfo.Series.ActualTrackBallLabelTemplate = ChartDictionaries.GenericCommonDictionary["defaultIndicatorTrackBallLabel"] as DataTemplate;
            }
            else
            {
                if (pointInfo.Series is FinancialSeriesBase && LabelDisplayMode != TrackballLabelDisplayMode.NearestPoint)
                    pointInfo.Series.ActualTrackBallLabelTemplate = ChartDictionaries.GenericCommonDictionary["defaultFinancialTrackBallLabel"] as DataTemplate;
                else if (pointInfo.Series is RangeSeriesBase && LabelDisplayMode != TrackballLabelDisplayMode.NearestPoint)
                {
                    if (pointInfo.Series is RangeColumnSeries && !pointInfo.Series.IsMultipleYPathRequired)
                    {
                        pointInfo.Series.ActualTrackBallLabelTemplate = ChartDictionaries.GenericCommonDictionary["defaultTrackBallLabel"] as DataTemplate;
                    }
                    else
                    {
                        pointInfo.Series.ActualTrackBallLabelTemplate = ChartDictionaries.GenericCommonDictionary["rangeSeriesTrackBallLabel"] as DataTemplate;
                    }
                }
                else if (pointInfo.Series is BoxAndWhiskerSeries && LabelDisplayMode != TrackballLabelDisplayMode.NearestPoint && !pointInfo.isOutlier)
                    pointInfo.Series.ActualTrackBallLabelTemplate = ChartDictionaries.GenericCommonDictionary["BoxWhiskerTrackBallLabel"] as DataTemplate;
                else
                    pointInfo.Series.ActualTrackBallLabelTemplate = ChartDictionaries.GenericCommonDictionary["defaultTrackBallLabel"] as DataTemplate;
            }

            if ((pointInfo.Series.TrackBallLabelTemplate != null && UseSeriesPalette && pointInfo.Series.Tag != null &&
                pointInfo.Series.Tag.Equals("FromTheme")) || pointInfo.Series.TrackBallLabelTemplate == null)
            {
                trackBallLabelTemplate = pointInfo.Series.ActualTrackBallLabelTemplate;
            }
            else
            {
                trackBallLabelTemplate = pointInfo.Series.TrackBallLabelTemplate;
            }

            return trackBallLabelTemplate;
        }

        private void ArrangeGroupLabel()
        {
            double yPos = 0;
            double xPos = 0;
            border.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            if (isReversed)
            {
                if (pointInfos.Count == 1)
                    xPos = CalculateHorizontalAlignment(pointInfos[0]);
                else
                    xPos = this.ChartArea.SeriesClipRect.Left + (this.ChartArea.SeriesClipRect.Width / 2 - border.DesiredSize.Width / 2);
                yPos = CalculateVerticalAlignment(pointInfos[0]);
            }
            else
            {
                if (pointInfos.Count == 1)
                    yPos = CalculateVerticalAlignment(pointInfos[0]);
                else
                    yPos = this.ChartArea.SeriesClipRect.Top + (this.ChartArea.SeriesClipRect.Height / 2 - border.DesiredSize.Height / 2);
                xPos = CalculateHorizontalAlignment(pointInfos[0]);
            }
            if (LabelDisplayMode == TrackballLabelDisplayMode.GroupAllPoints)
            {
                if (isReversed)
                {
                    xPos = this.ChartArea.SeriesClipRect.Right - border.DesiredSize.Width;
                }
                else
                {
                    yPos = this.ChartArea.SeriesClipRect.Top;
                }
            }

            CheckCollision(xPos, yPos);
        }

        private double CalculateVerticalAlignment(ChartPointInfo chartPointInfo)
        {
            double yPos = 0;
            if (chartPointInfo.VerticalAlignment == ChartAlignment.Center)
                yPos = pointInfos[0].BaseY - border.DesiredSize.Height / 2;
            else if (chartPointInfo.VerticalAlignment == ChartAlignment.Near)
                yPos = pointInfos[0].BaseY - border.DesiredSize.Height - 5;
            else
                yPos = pointInfos[0].BaseY + 5;
            if (LabelDisplayMode == TrackballLabelDisplayMode.GroupAllPoints)
            {
                yPos = pointInfos[0].BaseY - border.DesiredSize.Height / 2;
            }
            return yPos;
        }

        private double CalculateHorizontalAlignment(ChartPointInfo chartPointInfo)
        {
            double xPos = 0;
            if (chartPointInfo.HorizontalAlignment == ChartAlignment.Center)
                xPos = pointInfos[0].BaseX - border.DesiredSize.Width / 2;
            else if (chartPointInfo.HorizontalAlignment == ChartAlignment.Near)
                xPos = pointInfos[0].BaseX - border.DesiredSize.Width - 5;
            else
                xPos = pointInfos[0].BaseX + 5;
            if (LabelDisplayMode == TrackballLabelDisplayMode.GroupAllPoints)
            {
                xPos = pointInfos[0].BaseX - border.DesiredSize.Width / 2;
            }
            return xPos;
        }

        private void CheckCollision(double xPos, double yPos)
        {
            double left = xPos;
            double top = yPos;

            // Left Collision
            if (ChartArea.SeriesClipRect.Left > xPos)
            {
                if (pointInfos[0].HorizontalAlignment == ChartAlignment.Center || pointInfos[0].HorizontalAlignment == ChartAlignment.Auto)
                    xPos += (border.DesiredSize.Width) / 2;
                else
                    xPos += border.DesiredSize.Width + 10;
                if (!isReversed)
                {
                    if (LabelDisplayMode == TrackballLabelDisplayMode.GroupAllPoints)
                    {
                        xPos = pointInfos[0].BaseX;
                    }
                }
                left = xPos;
            }

            // Top Collision
            if (ChartArea.SeriesClipRect.Top > yPos)
            {
                if (pointInfos[0].VerticalAlignment == ChartAlignment.Center)
                    yPos += (border.DesiredSize.Height) / 2;
                else
                    yPos += border.DesiredSize.Height + 10;
                top = yPos;
            }

            // Right Collision
            if (ChartArea.SeriesClipRect.Right < (xPos + border.DesiredSize.Width))
            {
                if (pointInfos[0].HorizontalAlignment == ChartAlignment.Center || pointInfos[0].HorizontalAlignment == ChartAlignment.Auto)
                    xPos = xPos - (border.DesiredSize.Width) / 2;
                else
                    xPos = xPos - border.DesiredSize.Width - 10;
                if (!isReversed)
                {
                    if (LabelDisplayMode == TrackballLabelDisplayMode.GroupAllPoints)
                    {
                        xPos = pointInfos[0].BaseX - border.DesiredSize.Width;
                    }
                }
                left = xPos;
            }

            // Bottom Collision
            if (ChartArea.SeriesClipRect.Bottom < (yPos + border.DesiredSize.Height))
            {
                if (pointInfos[0].VerticalAlignment == ChartAlignment.Center)
                    yPos = yPos - (border.DesiredSize.Height) / 2;
                else
                    yPos = yPos - border.DesiredSize.Height - 10;
                top = yPos;
            }

            Canvas.SetLeft(border, left);
            Canvas.SetTop(border, top);
        }
             
        /// <summary>
        /// To align the labels smartly
        /// </summary>
        /// <param name="index">Index of the label</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Reviewed")]
        private void SmartAlignLabels()
        {
            List<List<Control>> intersectedGroups = new List<List<Control>>();
            List<Control> tempIntersectedLabels = new List<Control>();

            // Label Intersection logic
            tempIntersectedLabels.Add(labelElements[0]);
            for (int i = 0; i + 1 < labelElements.Count; i++)
            {
                if (CheckLabelCollision(labelElements[i], labelElements[i + 1]))
                {
                    tempIntersectedLabels.Add(labelElements[i + 1]);
                }
                else
                {
                    intersectedGroups.Add(new List<Control>(tempIntersectedLabels));
                    tempIntersectedLabels.Clear();
                    tempIntersectedLabels.Add(labelElements[i + 1]);
                }
            }

            // To add the last collided labels.
            if (tempIntersectedLabels.Count > 0)
            {
                intersectedGroups.Add(new List<Control>(tempIntersectedLabels));
                tempIntersectedLabels.Clear();
            }

            if (isReversed)
            {
                foreach (var intersectGroupLabels in intersectedGroups)
                {
                    // Smart align the labels inside group.
                    if (intersectGroupLabels.Count > 1)
                    {
                        ChartPointInfo pointInfo = ((intersectGroupLabels[0] as ContentControl).Content as ChartPointInfo);
                        ChartAlignment tempHorizontalAlign = pointInfo.HorizontalAlignment;
                        double tempXValue = pointInfo.X;

                        if (tempHorizontalAlign == ChartAlignment.Far)
                        {
                            for (int i = 0; i < intersectGroupLabels.Count; i++)
                            {
                                double width = intersectGroupLabels[i].DesiredSize.Width;
                                ChartPointInfo currentPointInfo = ((intersectGroupLabels[i] as ContentControl).Content as ChartPointInfo);

                                currentPointInfo.HorizontalAlignment = tempHorizontalAlign;
                                currentPointInfo.X = tempXValue + (width + trackLabelSpacing) * i;
                                Canvas.SetLeft(intersectGroupLabels[i], currentPointInfo.X);
                            }
                        }
                        else if (tempHorizontalAlign == ChartAlignment.Near)
                        {
                            double halfHeight = ((intersectGroupLabels[0].DesiredSize.Width * intersectGroupLabels.Count)
                                                 + trackLabelSpacing * (intersectGroupLabels.Count - 1))
                                                 - intersectGroupLabels[0].DesiredSize.Width;

                            for (int i = 0; i < intersectGroupLabels.Count; i++)
                            {
                                double width = intersectGroupLabels[i].DesiredSize.Width;
                                ChartPointInfo currentPointInfo = (intersectGroupLabels[i] as ContentControl).Content as ChartPointInfo;
                                currentPointInfo.HorizontalAlignment = tempHorizontalAlign;

                                currentPointInfo.X = tempXValue - halfHeight + i * (width + trackLabelSpacing);
                                Canvas.SetLeft(intersectGroupLabels[i], currentPointInfo.X);
                            }
                        }
                        else
                        {
                            double halfHeight = ((intersectGroupLabels[0].DesiredSize.Width * intersectGroupLabels.Count)
                                                  + trackLabelSpacing * (intersectGroupLabels.Count - 1)) / 2
                                                  - intersectGroupLabels[0].DesiredSize.Width / 2;

                            for (int i = 0; i < intersectGroupLabels.Count; i++)
                            {
                                double width = intersectGroupLabels[i].DesiredSize.Width;
                                ChartPointInfo currentPointInfo = (intersectGroupLabels[i] as ContentControl).Content as ChartPointInfo;
                                currentPointInfo.HorizontalAlignment = tempHorizontalAlign;

                                currentPointInfo.X = tempXValue - halfHeight + i * (width + trackLabelSpacing);
                                Canvas.SetLeft(intersectGroupLabels[i], currentPointInfo.X);
                            }
                        }
                    }
                }

                // Smart align the entire group.
                if (isOpposedAxis)
                {
                    // Arrange the lowest label according to the border.
                    ArrangeLowestLabelonBounds(intersectedGroups[intersectedGroups.Count - 1]);

                    if (intersectedGroups.Count > 1)
                        for (int i = intersectedGroups.Count - 1; i - 1 > -1; i--)
                        {
                            if (CheckGroupsCollision(intersectedGroups[i], intersectedGroups[i - 1]))
                            {
                                ChartPointInfo ptInfo = (intersectedGroups[i][0] as ContentControl).Content as ChartPointInfo;
                                double halfHeight = ptInfo.X - intersectedGroups[i - 1].Count * (intersectedGroups[i - 1][0].DesiredSize.Width + trackLabelSpacing);

                                for (int k = 0; k < intersectedGroups[i - 1].Count; k++)
                                {
                                    double width = intersectedGroups[i - 1][k].DesiredSize.Width;
                                    ChartPointInfo pointInfo = (intersectedGroups[i - 1][k] as ContentControl).Content as ChartPointInfo;

                                    pointInfo.X = halfHeight + k * (width + trackLabelSpacing);
                                    Canvas.SetLeft(intersectedGroups[i - 1][k], pointInfo.X);
                                }
                            }
                        }
                }
                else
                {
                    // Arrange the lowest label according to the border.
                    ArrangeLowestLabelonBounds(intersectedGroups[0]);

                    if (intersectedGroups.Count > 1)
                        for (int i = 0; i + 1 < intersectedGroups.Count; i++)
                        {
                            if (CheckGroupsCollision(intersectedGroups[i], intersectedGroups[i + 1]))
                            {
                                ChartPointInfo ptInfo = (intersectedGroups[i][intersectedGroups[i].Count - 1] as ContentControl).Content as ChartPointInfo;
                                double halfHeight = ptInfo.X + (intersectedGroups[i][0].DesiredSize.Width + trackLabelSpacing);

                                for (int k = 0; k < intersectedGroups[i + 1].Count; k++)
                                {
                                    double width = intersectedGroups[i + 1][k].DesiredSize.Width;
                                    ChartPointInfo pointInfo = (intersectedGroups[i + 1][k] as ContentControl).Content as ChartPointInfo;

                                    pointInfo.X = halfHeight + k * (width + trackLabelSpacing);
                                    Canvas.SetLeft(intersectedGroups[i + 1][k], pointInfo.X);
                                }
                            }
                        }
                }
            }
            else
            {
                foreach (var intersectGroupLabels in intersectedGroups)
                {
                    // Smart align the labels inside group.
                    if (intersectGroupLabels.Count > 1)
                    {
                        ChartPointInfo pointInfo = (intersectGroupLabels[0] as ContentControl).Content as ChartPointInfo;
                        ChartAlignment tempVerticalAlign = pointInfo.VerticalAlignment;
                        double tempYValue = pointInfo.Y;
                        if (tempVerticalAlign == ChartAlignment.Far)
                        {
                            for (int i = 0; i < intersectGroupLabels.Count; i++)
                            {
                                double height = intersectGroupLabels[i].DesiredSize.Height;
                                ChartPointInfo currentPointInfo = ((intersectGroupLabels[i] as ContentControl).Content as ChartPointInfo);
                                currentPointInfo.VerticalAlignment = tempVerticalAlign;
                                currentPointInfo.Y = tempYValue + ((height + trackLabelSpacing) * i);
                                Canvas.SetTop(intersectGroupLabels[i], currentPointInfo.Y);
                            }
                        }
                        else if (tempVerticalAlign == ChartAlignment.Near)
                        {
                            double halfHeight = ((intersectGroupLabels[0].DesiredSize.Height * intersectGroupLabels.Count)
                                                 + trackLabelSpacing * (intersectGroupLabels.Count - 1))
                                                 - intersectGroupLabels[0].DesiredSize.Height;

                            for (int i = 0; i < intersectGroupLabels.Count; i++)
                            {
                                double height = intersectGroupLabels[i].DesiredSize.Height;
                                ChartPointInfo currentPointInfo = (intersectGroupLabels[i] as ContentControl).Content as ChartPointInfo;
                                currentPointInfo.VerticalAlignment = tempVerticalAlign;
                                currentPointInfo.Y = tempYValue + halfHeight - i * (height + trackLabelSpacing);
                                Canvas.SetTop(intersectGroupLabels[i], currentPointInfo.Y);
                            }
                        }
                        else
                        {
                            double halfHeight = ((intersectGroupLabels[0].DesiredSize.Height * intersectGroupLabels.Count)
                                                  + trackLabelSpacing * (intersectGroupLabels.Count - 1)) / 2
                                                  - intersectGroupLabels[0].DesiredSize.Height / 2;

                            for (int i = 0; i < intersectGroupLabels.Count; i++)
                            {
                                double height = intersectGroupLabels[i].DesiredSize.Height;
                                ChartPointInfo currentPointInfo = (intersectGroupLabels[i] as ContentControl).Content as ChartPointInfo;
                                currentPointInfo.VerticalAlignment = tempVerticalAlign;
                                currentPointInfo.Y = tempYValue + halfHeight - i * (height + trackLabelSpacing);
                                Canvas.SetTop(intersectGroupLabels[i], currentPointInfo.Y);
                            }
                        }
                    }
                }

                // Smart align the entire group.
                if (isOpposedAxis)
                {
                    // Arrange the lowest label according to the border.
                    ArrangeLowestLabelonBounds(intersectedGroups[intersectedGroups.Count - 1]);

                    if (intersectedGroups.Count > 1)
                        for (int i = intersectedGroups.Count - 1; i - 1 > -1; i--)
                        {
                            if (CheckGroupsCollision(intersectedGroups[i], intersectedGroups[i - 1]))
                            {
                                ChartPointInfo ptInfo = (intersectedGroups[i][0] as ContentControl).Content as ChartPointInfo;
                                double halfHeight = ptInfo.Y + intersectedGroups[i - 1].Count * (intersectedGroups[i][0].DesiredSize.Height + trackLabelSpacing);

                                for (int k = 0; k < intersectedGroups[i - 1].Count; k++)
                                {
                                    double height = intersectedGroups[i - 1][k].DesiredSize.Height;
                                    ChartPointInfo pointInfo = (intersectedGroups[i - 1][k] as ContentControl).Content as ChartPointInfo;

                                    pointInfo.Y = halfHeight - k * (height + trackLabelSpacing);
                                    Canvas.SetTop(intersectedGroups[i - 1][k], pointInfo.Y);
                                }
                            }
                        }
                }
                else
                {
                    // Arrange the lowest label according to the border.
                    ArrangeLowestLabelonBounds(intersectedGroups[0]);

                    if (intersectedGroups.Count > 1)
                        for (int i = 0; i + 1 < intersectedGroups.Count; i++)
                        {
                            if (CheckGroupsCollision(intersectedGroups[i], intersectedGroups[i + 1]))
                            {
                                ChartPointInfo ptInfo = (intersectedGroups[i][intersectedGroups[i].Count - 1] as ContentControl).Content as ChartPointInfo;
                                double halfHeight = ptInfo.Y - (intersectedGroups[i + 1][0].DesiredSize.Height + trackLabelSpacing);

                                // Reverse Counter is used since the hight of the pixel is from top to bottom.
                                int reverseCounter = 0;
                                for (int k = 0; k < intersectedGroups[i + 1].Count; k++)
                                {
                                    double height = intersectedGroups[i + 1][k].DesiredSize.Height;
                                    ChartPointInfo pointInfo = (intersectedGroups[i + 1][k] as ContentControl).Content as ChartPointInfo;

                                    pointInfo.Y = halfHeight - k * (height + trackLabelSpacing);
                                    Canvas.SetTop(intersectedGroups[i + 1][k], pointInfo.Y);
                                    reverseCounter++;
                                }
                            }
                        }
                }
            }
        }

        private void ArrangeLowestLabelonBounds(List<Control> list)
        {
            if (isReversed)
            {
                if (isOpposedAxis)
                {
                    ChartPointInfo ptInfo = ((list[list.Count - 1] as ContentControl).Content as ChartPointInfo);
                    double width = (list[list.Count - 1] as ContentControl).DesiredSize.Width;

                    double X = ptInfo.X + width;
                    if (X > Math.Round(ChartArea.SeriesClipRect.Right, 2))
                    {
                        double halfHeight = ChartArea.SeriesClipRect.Right - list.Count * (width + trackLabelSpacing);

                        for (int i = 0; i < list.Count; i++)
                        {
                            ChartPointInfo pointInfo = (list[i] as ContentControl).Content as ChartPointInfo;
                            pointInfo.X = halfHeight + i * (width + trackLabelSpacing);
                            Canvas.SetLeft(list[i], pointInfo.X);
                        }
                    }
                }
                else
                {
                    ChartPointInfo ptInfo = ((list[0] as ContentControl).Content as ChartPointInfo);

                    double width = (list[0] as ContentControl).DesiredSize.Width;
                    double X = ptInfo.X;
                    if (X < Math.Round(ChartArea.SeriesClipRect.Left, 2))
                    {
                        double halfHeight = ChartArea.SeriesClipRect.Left + trackLabelSpacing;

                        for (int i = 0; i < list.Count; i++)
                        {
                            ChartPointInfo pointInfo = (list[i] as ContentControl).Content as ChartPointInfo;
                            pointInfo.X = halfHeight + i * (width + trackLabelSpacing);
                            Canvas.SetLeft(list[i], pointInfo.X);
                        }
                    }
                }
            }
            else
            {
                if (isOpposedAxis)
                {
                    ChartPointInfo ptInfo = ((list[list.Count - 1] as ContentControl).Content as ChartPointInfo);
                    double height = (list[list.Count - 1] as ContentControl).DesiredSize.Height;

                    double Y = ptInfo.Y;
                    if (Y < Math.Round(ChartArea.SeriesClipRect.Top, 2))
                    {
                        double halfHeight = ChartArea.SeriesClipRect.Top + (list.Count - 1) * height + list.Count * trackLabelSpacing;

                        for (int i = 0; i < list.Count; i++)
                        {
                            ChartPointInfo pointInfo = (list[i] as ContentControl).Content as ChartPointInfo;
                            pointInfo.Y = halfHeight - i * (height + trackLabelSpacing);
                            Canvas.SetTop(list[i], pointInfo.Y);
                        }
                    }
                }
                else
                {
                    ChartPointInfo ptInfo = ((list[0] as ContentControl).Content as ChartPointInfo);

                    double height = (list[0] as ContentControl).DesiredSize.Height;
                    double Y = ptInfo.Y + height;
                    if (Y > Math.Round(ChartArea.SeriesClipRect.Bottom, 2))
                    {
                        double halfHeight = ChartArea.SeriesClipRect.Bottom - (height + trackLabelSpacing);

                        for (int i = 0; i < list.Count; i++)
                        {
                            ChartPointInfo pointInfo = (list[i] as ContentControl).Content as ChartPointInfo;
                            pointInfo.Y = halfHeight - i * (height + trackLabelSpacing);
                            Canvas.SetTop(list[i], pointInfo.Y);
                        }
                    }
                }
            }
        }

        private bool CheckGroupsCollision(List<Control> list1, List<Control> list2)
        {
            ChartPointInfo list1_PtInfoFirst = (list1[0] as ContentControl).Content as ChartPointInfo;
            ChartPointInfo list1_PtInfoLast = (list1[list1.Count - 1] as ContentControl).Content as ChartPointInfo;
            ChartPointInfo list2_PtInfoFirst = (list2[0] as ContentControl).Content as ChartPointInfo;
            ChartPointInfo list2_PtInfoLast = (list2[list2.Count - 1] as ContentControl).Content as ChartPointInfo;

            Point point1;
            Point point2;
            Rect rect1;
            Rect rect2;

            if (isReversed)
            {
                point1 = new Point(list1_PtInfoFirst.X, list1_PtInfoFirst.Y);
                point2 = new Point(list1_PtInfoLast.X + list1[list1.Count - 1].DesiredSize.Width, list1_PtInfoLast.Y + list1[list1.Count - 1].DesiredSize.Height);
                rect1 = new Rect(point1, point2);

                point1 = new Point(list2_PtInfoFirst.X, list2_PtInfoFirst.Y);
                point2 = new Point(list2_PtInfoLast.X + list2[list2.Count - 1].DesiredSize.Width, list2_PtInfoLast.Y + list2[list2.Count - 1].DesiredSize.Height);
                rect2 = new Rect(point1, point2);
            }
            else
            {
                point1 = new Point(list1_PtInfoLast.X, list1_PtInfoLast.Y);
                point2 = new Point(list1_PtInfoFirst.X + list1[0].DesiredSize.Width, list1_PtInfoFirst.Y + list1[0].DesiredSize.Height);
                rect1 = new Rect(point1, point2);

                point1 = new Point(list2_PtInfoLast.X, list2_PtInfoLast.Y);
                point2 = new Point(list2_PtInfoFirst.X + list2[0].DesiredSize.Width, list2_PtInfoFirst.Y + list2[0].DesiredSize.Height);
                rect2 = new Rect(point1, point2);
            }

            return CheckLabelCollisionRect(rect1, rect2);
        }

        private void AlignSeriesToolTipPolygon(ContentControl control)
        {
            double labelHeight = control.DesiredSize.Height;
            double labelWidth = control.DesiredSize.Width;
            var label = (control.Content as ChartPointInfo);
            var baseX = label.BaseX;
            var baseY = label.BaseY;
            double seriesTipHypotenuse;
            double hypotenuse;

            if ((label.VerticalAlignment == ChartAlignment.Center && label.HorizontalAlignment != ChartAlignment.Center) ||
                (label.HorizontalAlignment == ChartAlignment.Center && label.VerticalAlignment != ChartAlignment.Center))
            {
                seriesTipHypotenuse = (2 * seriesTipHeight) / Math.Sqrt(3);
                hypotenuse = seriesTipHypotenuse / 2;
            }
            else
            {
                seriesTipHypotenuse = labelHeight / 3;
                hypotenuse = seriesTipHypotenuse > 25 ? (seriesTipHypotenuse * 2) / 3 : seriesTipHypotenuse;
            }

            PointCollection polygonPoints = new PointCollection();

            polygonPoints.Add(new Point(0, 0));

            // Fix the series label tip at left.
            if (label.HorizontalAlignment == ChartAlignment.Far)
            {
                polygonPoints.Add(new Point(0, +labelHeight / 2 - hypotenuse));

                if (!isReversed || label.VerticalAlignment == ChartAlignment.Center)
                {
                    if (label.VerticalAlignment == ChartAlignment.Near)
                        polygonPoints.Add(new Point(-seriesTipHeight, (baseY - label.Y - 4)));
                    else if (label.VerticalAlignment == ChartAlignment.Far)
                        polygonPoints.Add(new Point(-seriesTipHeight, baseY - label.Y + 4));
                    else
                        polygonPoints.Add(new Point(-seriesTipHeight, baseY - label.Y));
                }

                polygonPoints.Add(new Point(0, labelHeight / 2 + hypotenuse));
            }

            polygonPoints.Add(new Point(0, labelHeight));

            // Fix the series label tip at bottom.
            if (label.VerticalAlignment == ChartAlignment.Near)
            {
                polygonPoints.Add(new Point(labelWidth / 2 - hypotenuse, labelHeight));

                if (isReversed || label.HorizontalAlignment == ChartAlignment.Center)
                {
                    if (label.HorizontalAlignment == ChartAlignment.Far)
                        polygonPoints.Add(new Point(baseX - label.X, seriesTipHeight + labelHeight));
                    else if (label.HorizontalAlignment == ChartAlignment.Near)
                        polygonPoints.Add(new Point(baseX - label.X - 4, seriesTipHeight + labelHeight));
                    else
                        polygonPoints.Add(new Point(baseX - label.X, seriesTipHeight + labelHeight));
                }

                polygonPoints.Add(new Point(labelWidth / 2 + hypotenuse, labelHeight));
            }

            polygonPoints.Add(new Point(labelWidth, labelHeight));

            // Fix the series label tip right.
            if (label.HorizontalAlignment == ChartAlignment.Near)
            {
                polygonPoints.Add(new Point(labelWidth, labelHeight / 2 + hypotenuse));

                if (!isReversed || label.VerticalAlignment == ChartAlignment.Center)
                {
                    if (label.VerticalAlignment == ChartAlignment.Near)
                        polygonPoints.Add(new Point(seriesTipHeight + labelWidth, (baseY - label.Y - 4)));
                    else if (label.VerticalAlignment == ChartAlignment.Far)
                        polygonPoints.Add(new Point(seriesTipHeight + labelWidth, (baseY - label.Y + 4)));
                    else
                        polygonPoints.Add(new Point(seriesTipHeight + labelWidth, (baseY - label.Y)));
                }

                polygonPoints.Add(new Point(labelWidth, labelHeight / 2 - hypotenuse));
            }

            polygonPoints.Add(new Point(labelWidth, 0));

            // Fix series label tip top
            if (label.VerticalAlignment == ChartAlignment.Far)
            {
                polygonPoints.Add(new Point(labelWidth / 2 + hypotenuse, 0));

                if (isReversed || label.HorizontalAlignment == ChartAlignment.Center)
                {
                    if (label.HorizontalAlignment == ChartAlignment.Far)
                        polygonPoints.Add(new Point(baseX - label.X, -seriesTipHeight));
                    else if (label.HorizontalAlignment == ChartAlignment.Near)
                        polygonPoints.Add(new Point(baseX - label.X - 4, -seriesTipHeight));
                    else
                        polygonPoints.Add(new Point(baseX - label.X, -seriesTipHeight));
                }

                polygonPoints.Add(new Point(labelWidth / 2 - hypotenuse, 0));
            }

            polygonPoints.Add(new Point(0, 0));

            (control.Content as ChartPointInfo).PolygonPoints = polygonPoints;
        }

        private void GenerateAdditionalTrackball(ChartPointInfo pointInfo)
        {
            ChartPointInfo info = new ChartPointInfo();
            List<double> yValue = new List<double>();

            if (pointInfo.Series is RangeColumnSeries && !pointInfo.Series.IsMultipleYPathRequired)
            {
                var yAxisRange = pointInfo.Series.ActualYAxis.VisibleRange;
                var axisCenter = (yAxisRange.End - Math.Abs(yAxisRange.Start)) / 2;
                var segmentMiddle = Convert.ToDouble(pointInfo.ValueY) / 2;
                var yPosition = axisCenter + segmentMiddle;

                yValue.Add(yPosition);
            }
            else
            {
                yValue.Add(Convert.ToDouble(pointInfo.High));
                yValue.Add(Convert.ToDouble(pointInfo.Low));
            }

            if (pointInfo.Series is FinancialSeriesBase)
            {
                yValue.Add(Convert.ToDouble(pointInfo.Open));
                yValue.Add(Convert.ToDouble(pointInfo.Close));
            }

            foreach (double y in yValue)
            {
                info = new ChartPointInfo();
                info.Series = pointInfo.Series;
                info.Y = this.ChartArea.ValueToLogPoint(pointInfo.Series.ActualYAxis, y);
                if (isReversed)
                    info.Y = info.Y + ChartArea.SeriesClipRect.Left;
                else
                    info.Y = info.Y + ChartArea.SeriesClipRect.Top;
                info.X = pointInfo.X;
                if (pointInfo.Series.IsActualTransposed && ChartArea.SeriesClipRect.Contains(new Point(info.Y, info.X))
                       || !pointInfo.Series.IsActualTransposed && ChartArea.SeriesClipRect.Contains(new Point(info.X, info.Y)))
                    AddTrackBall(info);
            }
        }

        internal void Activate(bool activate)
        {
            foreach (UIElement element in elements)
            {
                element.Visibility = activate ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Method to check whether the point is in trackball or not.
        /// </summary>
        /// <param name="pointX">The x point</param>
        /// <param name="pointY">The y point</param>
        /// <returns>Point is in trackball or not</returns>
        internal bool HitTest(double pointX, double pointY)
        {
            Rect rect;
            //Added 10 with stroke thickness of trackball line to make it easily recognize on mouse actions.
            double strokeThickness = (line.StrokeThickness >= 10) ? line.StrokeThickness / 2 : (line.StrokeThickness / 2) + 10;
        
            if (isReversed)
            {
                rect = new Rect(line.X1, line.Y1 - strokeThickness, line.X2 - line.X1, (line.Y2 + strokeThickness) - (line.Y1 - strokeThickness));
            }
            else
            {
                rect = new Rect(line.X1 - strokeThickness, line.Y1, (line.X2 + strokeThickness) - (line.X1 - strokeThickness), line.Y2 - line.Y1);
            }

            return rect.Contains(new Point(pointX, pointY));
        }
    }

    /// <summary>
    /// Sets the fill color for the track ball control.
    /// </summary>
    public partial class ChartTrackBallColorConverter : IValueConverter
    {
        #region Public Methods

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ChartTrackBallControl trackBall = value as ChartTrackBallControl;

            if (trackBall != null)
            {
                if (trackBall.Background != null)
                    return trackBall.Background;
                if (trackBall.Series != null)
                {
                    if (trackBall.Series is FinancialSeriesBase)
                        return trackBall.Series.GetFinancialSeriesInterior(1);
                    else
                        return trackBall.Series.GetInteriorColor(1);
                }
            }

            return null;
        }

        /// <summary>
        ///  Modifies the target data before passing it to the source object. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)

        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Defines the control Template for the trackball
    /// </summary>
    public partial class ChartTrackBallControl : Control
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="Series"/> property.
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(
                "Series",
                typeof(ChartSeriesBase),
                typeof(ChartTrackBallControl),
                new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="Stroke"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(
                "Stroke",
                typeof(Brush),
                typeof(ChartTrackBallControl),
                new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeThickness"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(
                "StrokeThickness",
                typeof(double),
                typeof(ChartTrackBallControl),
                new PropertyMetadata(0.5d));

        #endregion

        #region Constructor
        
        /// <summary>
        /// Called when instance created for ChartTrackBall
        /// </summary>
        /// <param name="series"></param>
        public ChartTrackBallControl(ChartSeriesBase series)
        {
            this.Series = series;
            DefaultStyleKey = typeof(ChartTrackBallControl);
        }

        #endregion

        #region Properties

        #region Public Properties
        
        /// <summary>
        /// Gets or sets Series property
        /// </summary>
        public ChartSeriesBase Series
        {
            get { return (ChartSeriesBase)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        /// <summary>
        /// Gets or sets strokeproperty
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary>
        /// Gets or sets StrokeThickness property
        /// </summary>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Defines the PositionChangingEventArgs
    /// </summary>
    public partial class PositionChangingEventArgs : EventArgs
    {
        #region Properties
    
        #region Public Properties

        /// <summary>
        /// Gets or sets the current ChartPointInfo 
        /// </summary>        
        public ObservableCollection<ChartPointInfo> PointInfos
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the trackball on new mouse pointer position
        /// </summary>
        public bool Cancel
        {
            get;
            set;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Defines the PositionChangedEventArgs
    /// </summary>
    public partial class PositionChangedEventArgs : EventArgs
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the previous PointInfo 
        /// </summary>
        public ObservableCollection<ChartPointInfo> PreviousPointInfos
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the current PointInfos
        /// </summary>
        public ObservableCollection<ChartPointInfo> CurrentPointInfos
        {
            get;
            internal set;
        }

        #endregion

        #endregion

    }
}
