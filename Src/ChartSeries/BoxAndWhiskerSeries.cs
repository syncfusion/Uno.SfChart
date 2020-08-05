using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.Foundation;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// BoxAndWhiskerSeries plots a combination of rectangle and lines to show the distribution of data set.
    /// </summary>
    public partial class BoxAndWhiskerSeries : XyDataSeries, ISegmentSpacing, ISegmentSelectable
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property. 
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(BoxAndWhiskerSeries),
            new PropertyMetadata(-1, OnSelectedIndexChanged));

        // Using a DependencyProperty as the backing store for Percentile.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PercentileProperty =
            DependencyProperty.Register("Percentile", typeof(double), typeof(BoxAndWhiskerSeries), new PropertyMetadata(0.25, OnPropertyChanged));

        // Using a DependencyProperty as the backing store for BoxPlotMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoxPlotModeProperty =
            DependencyProperty.Register("BoxPlotMode", typeof(BoxPlotMode), typeof(BoxAndWhiskerSeries), new PropertyMetadata(BoxPlotMode.Exclusive, OnPropertyChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="OutlierTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty OutlierTemplateProperty =
            DependencyProperty.Register("SymbolTemplate", typeof(DataTemplate), typeof(BoxAndWhiskerSeries),
            new PropertyMetadata(null, new PropertyChangedCallback(OnOutlierTemplateChanged)));

        // Using a DependencyProperty as the backing store for ShowMedian.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowMedianProperty =
            DependencyProperty.Register("ShowMedian", typeof(bool), typeof(BoxAndWhiskerSeries), new PropertyMetadata(false, OnShowMedianChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSpacing"/> property. 
        /// </summary>
        public static readonly DependencyProperty SegmentSpacingProperty =
            DependencyProperty.Register("SegmentSpacing", typeof(double), typeof(BoxAndWhiskerSeries),
            new PropertyMetadata(0.0, OnSegmentSpacingChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSelectionBrush"/> property. 
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register("SegmentSelectionBrush", typeof(Brush), typeof(BoxAndWhiskerSeries),
            new PropertyMetadata(null, OnPropertyChanged));

        #endregion

        #region Fields

        #region Private Fields

        private double whiskerWidth;

        bool isEvenList = false;

        private List<ChartSegment> outlierSegments;

        private ActualLabelPosition topLabelPosition;

        private ActualLabelPosition bottomLabelPosition;

        // private double upperPercentile;

        // private double lowerPercentile;

        #endregion

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the index of the selected segment.
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Gets or sets the plotting mode for drawing the series. 
        /// </summary>
        public BoxPlotMode BoxPlotMode
        {
            get { return (BoxPlotMode)GetValue(BoxPlotModeProperty); }
            set { SetValue(BoxPlotModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the template for outliers.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        public DataTemplate OutlierTemplate
        {
            get { return (DataTemplate)GetValue(OutlierTemplateProperty); }
            set { SetValue(OutlierTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable the median symbol.
        /// </summary>
        public bool ShowMedian
        {
            get { return (bool)GetValue(ShowMedianProperty); }
            set { SetValue(ShowMedianProperty, value); }
        }

        /// <summary>
        /// Gets or sets the spacing between the segments across the series in cluster mode.
        /// </summary>
        /// <value>
        ///     The value ranges from 0 to 1.
        /// </value>
        public double SegmentSpacing
        {
            get { return (double)GetValue(SegmentSpacingProperty); }
            set { SetValue(SegmentSpacingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the interior for the selected segment.
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

        ///// <summary>
        ///// Gets or sets the lower percentile value when percentile mode is enabled.
        ///// </summary>
        //// public double Percentile
        //// {
        ////    get { return (double)GetValue(PercentileProperty); }
        ////    set { SetValue(PercentileProperty, value); }
        //// }

        #endregion

        #region Protected Internal Properties

        /// <summary>
        /// Gets or sets the whisker width.
        /// </summary>
        protected internal double? WhiskerWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this series is placed side by side.
        /// </summary>
        /// <returns>
        /// It returns <c>true</c>, if the series is placed side by side [cluster mode].
        /// </returns>
        protected internal override bool IsSideBySide
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the y values collection.
        /// </summary>
        protected internal List<IList<double>> YCollection
        {
            get;
            set;
        }

        #endregion

        #endregion

        #region Methods

        #region Interface Methods

        double ISegmentSpacing.CalculateSegmentSpacing(double spacing, double Right, double Left)
        {
            return CalculateSegmentSpacing(spacing, Right, Left);
        }

        #endregion

        #region Public Override Methods

        /// <summary>
        /// Finds the nearest point in ChartSeries relative to the mouse point/touch position.
        /// </summary>
        /// <param name="point">The co-ordinate point representing the current mouse point /touch position.</param>
        /// <param name="x">x-value of the nearest point.</param>
        /// <param name="y">y-value of the nearest point</param>
        /// <param name="stackedYValue"></param>
        public override void FindNearestChartPoint(Point point, out double x, out double y, out double stackedYValue)
        {
            x = double.NaN;
            y = 0d;
            stackedYValue = double.NaN;
            Point nearPoint = new Point();
            if (this.IsIndexed || !(this.ActualXValues is IList<double>))
            {
                if (ActualArea != null)
                {
                    double xStart = ActualXAxis.VisibleRange.Start;
                    double xEnd = ActualXAxis.VisibleRange.End;
                    point = new Point(ActualArea.PointToValue(ActualXAxis, point), ActualArea.PointToValue(ActualYAxis, point));
                    double range = Math.Round(point.X);
                    {
                        var count = this.YCollection.Count;
                        if (range <= xEnd && range >= xStart && range < count && range >= 0)
                        {
                            x = range;
                        }
                    }
                }
            }
            else
            {
                IList<double> xValues = this.ActualXValues as IList<double>;
                nearPoint.X = ActualXAxis.VisibleRange.Start;

                if (IsSideBySide)
                {
                    DoubleRange sbsInfo = this.GetSideBySideInfo(this);
                    nearPoint.X = ActualXAxis.VisibleRange.Start + sbsInfo.Start;
                }

                point = new Point(ActualArea.PointToValue(ActualXAxis, point), ActualArea.PointToValue(ActualYAxis, point));

                for (int i = 0; i < DataCount; i++)
                {
                    double x1 = xValues[i];
                    double y1 = 0d;

                    if (this.ActualXAxis is LogarithmicAxis)
                    {
                        var logAxis = ActualXAxis as LogarithmicAxis;
                        if (Math.Abs(point.X - x1) <= Math.Abs(point.X - nearPoint.X) &&
                           (Math.Log(point.X, logAxis.LogarithmicBase) > ActualXAxis.VisibleRange.Start &&
                            Math.Log(point.X, logAxis.LogarithmicBase) < ActualXAxis.VisibleRange.End))
                        {
                            nearPoint = new Point(x1, y1);
                            x = xValues[i];
                        }
                    }
                    else if (Math.Abs((point.X - x1)) <= Math.Abs((point.X - nearPoint.X)) &&
                        (point.X > ActualXAxis.VisibleRange.Start) && (point.X < ActualXAxis.VisibleRange.End))
                    {
                        nearPoint = new Point(x1, y1);
                        x = xValues[i];
                    }
                }
            }
        }

        /// <summary>
        /// Creates the segments of Box and Whisker Series.
        /// </summary>
        public override void CreateSegments()
        {
            ClearUnUsedSegments(this.DataCount);
            ClearUnUsedAdornments(this.DataCount * 5);
            DoubleRange sbsInfo = this.GetSideBySideInfo(this);
            double start = sbsInfo.Start;
            double end = sbsInfo.End;
            List<double> xValues = GetXValues();
            ////UpdatePercentile();
            UpdateWhiskerWidth();

            if (adornmentInfo != null)
                UpdateAdornmentLabelPositiion();

            outlierSegments = new List<ChartSegment>();

            if (YCollection == null || YCollection.Count == 0)
                return;

            for (int i = 0; i < DataCount; i++)
            {
                double median, lowerQuartile, upperQuartile, minimum, maximum, x1, x2, average = 0d;

                List<double> outliers = new List<double>();

                var ylist = YCollection[i].Where(x => !double.IsNaN(x)).ToArray();

                if (ylist.Count() > 0)
                {
                    Array.Sort(ylist);
                    average = ylist.Average();
                }

                int yCount = ylist.Length;

                isEvenList = yCount % 2 == 0;

                // Getting the required values.
                x1 = xValues[i] + start;
                x2 = xValues[i] + end;

                if (yCount == 0)
                {
                    ////To create an empty segment for the additional space requirement in range calculation.
                    if (i < Segments.Count)
                    {
                        var segment = Segments[i] as BoxAndWhiskerSegment;
                        segment.SetData(x1, x2, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, xValues[i] + sbsInfo.Median, double.NaN);
                        segment.Item = ActualData[i];
                    }
                    else
                    {
                        BoxAndWhiskerSegment boxEmptySegment = new BoxAndWhiskerSegment(this);
                        boxEmptySegment.SetData(x1, x2, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, xValues[i] + sbsInfo.Median, double.NaN);
                        boxEmptySegment.Item = ActualData[i];
                        Segments.Add(boxEmptySegment);
                    }

                    if (AdornmentsInfo != null)
                        if (i < Adornments.Count / 5)
                            SetBoxWhiskerAdornments(sbsInfo, xValues[i], double.NaN, double.NaN, x1, double.NaN, double.NaN, double.NaN, outliers, i);
                        else
                            AddBoxWhiskerAdornments(sbsInfo, xValues[i], double.NaN, double.NaN, x1, double.NaN, double.NaN, double.NaN, outliers, i);

                    continue;
                }

                if (BoxPlotMode == BoxPlotMode.Exclusive)
                {
                    lowerQuartile = GetExclusiveQuartileValue(ylist, yCount, 0.25d);
                    upperQuartile = GetExclusiveQuartileValue(ylist, yCount, 0.75d);
                    median = GetExclusiveQuartileValue(ylist, yCount, 0.5d);
                }
                else if (BoxPlotMode == BoxPlotMode.Inclusive)
                {
                    lowerQuartile = GetInclusiveQuartileValue(ylist, yCount, 0.25d);
                    upperQuartile = GetInclusiveQuartileValue(ylist, yCount, 0.75d);
                    median = GetInclusiveQuartileValue(ylist, yCount, 0.5d);
                }
                else
                {
                    // Take the first half of the list.
                    median = GetMedianValue(ylist);
                    GetQuartileValues(ylist, yCount, out lowerQuartile, out upperQuartile);
                }

                GetMinMaxandOutlier(lowerQuartile, upperQuartile, ylist, out minimum, out maximum, outliers);

                double actualMinimum = minimum;
                double actualMaximum = maximum;

                if (outliers.Count > 0)
                {
                    actualMinimum = Math.Min(outliers.Min(), actualMinimum);
                    actualMaximum = Math.Max(outliers.Max(), actualMaximum);
                }

                ChartSegment currentSegment = null;
                if (i < Segments.Count)
                {
                    currentSegment = Segments[i];
                    var segment = Segments[i] as BoxAndWhiskerSegment;
                    segment.SetData(x1, x2, actualMinimum, minimum, lowerQuartile, median, upperQuartile, maximum, actualMaximum, xValues[i] + sbsInfo.Median, average);
                    WhiskerWidth = whiskerWidth;
                    segment.Item = ActualData[i];
                }
                else
                {
                    BoxAndWhiskerSegment boxSegment = new BoxAndWhiskerSegment(this);
                    boxSegment.SetData(x1, x2, actualMinimum, minimum, lowerQuartile, median, upperQuartile, maximum, actualMaximum, xValues[i] + sbsInfo.Median, average);
                    boxSegment.Item = ActualData[i];
                    boxSegment.Outliers = outliers;
                    boxSegment.WhiskerWidth = whiskerWidth;
                    currentSegment = boxSegment;
                    Segments.Add(boxSegment);
                }

                if (AdornmentsInfo != null)
                    if (i < Adornments.Count / 5)
                        SetBoxWhiskerAdornments(sbsInfo, xValues[i], minimum, maximum, x1, median, lowerQuartile, upperQuartile, outliers, i);
                    else
                        AddBoxWhiskerAdornments(sbsInfo, xValues[i], minimum, maximum, x1, median, lowerQuartile, upperQuartile, outliers, i);

                if (outliers.Count > 0)
                {
                    foreach (var outlier in outliers)
                    {
                        ScatterSegment scatterSegment = new ScatterSegment();
                        var position = x1 + (x2 - x1) / 2;
                        scatterSegment.SetData(position, outlier);
                        scatterSegment.ScatterWidth = 10;
                        scatterSegment.ScatterHeight = 10;
                        scatterSegment.Item = ActualData[i];
                        scatterSegment.Series = this;
                        scatterSegment.CustomTemplate = OutlierTemplate;

                        // The bindings are done with the segment not with the series.
                        BindProperties(scatterSegment, currentSegment);
                        outlierSegments.Add(scatterSegment);
                    }
                }

                isEvenList = false;
            }

            foreach (ScatterSegment outlierSegment in outlierSegments)
            {
                this.Segments.Add(outlierSegment);
                if (AdornmentsInfo != null)
                {
                    var adornment = this.CreateAdornment(this, outlierSegment.XData, outlierSegment.YData, outlierSegment.XData, outlierSegment.YData);
                    adornment.ActualLabelPosition = topLabelPosition;
                    adornment.Item = outlierSegment.Item;
                    this.Adornments.Add(adornment);
                }
            }
        }

        #endregion

        #region Internal Override Methods

        internal override void GeneratePropertyPoints(string[] yPaths, IList<double>[] yLists)
        {
            IEnumerator enumerator = ItemsSource.GetEnumerator();

            if (enumerator.MoveNext())
            {
                for (int i = 0; i < UpdateStartedIndex; i++)
                    enumerator.MoveNext();

                // To get the type of the x value and  xGetMethod.
                PropertyInfo xPropertyInfo = ChartDataUtils.GetPropertyInfo(enumerator.Current, this.XBindingPath);
                IPropertyAccessor xPropertyAccessor = null;
                if (xPropertyInfo != null)
                    xPropertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(xPropertyInfo);
                if (xPropertyAccessor == null) return;
                Func<object, object> xGetMethod = xPropertyAccessor.GetMethod;

                XAxisValueType = GetDataType(xPropertyAccessor, ItemsSource as IEnumerable);

                if (XAxisValueType == ChartValueType.DateTime || XAxisValueType == ChartValueType.Double ||
                    XAxisValueType == ChartValueType.Logarithmic || XAxisValueType == ChartValueType.TimeSpan)
                {
                    if (!(ActualXValues is List<double>))
                        this.ActualXValues = this.XValues = new List<double>();
                }
                else
                {
                    if (!(ActualXValues is List<string>))
                        this.ActualXValues = this.XValues = new List<string>();
                }

                // To get the yGetMethod.
                PropertyInfo yPropertyInfo = ChartDataUtils.GetPropertyInfo(enumerator.Current, yPaths[0]);
                IPropertyAccessor yPropertyAccessor = null;
                if (yPropertyInfo != null)
                    yPropertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(yPropertyInfo);
                if (yPropertyAccessor == null) return;
                if (yPropertyAccessor == null) return;
                Func<object, object> yGetMethod = yPropertyAccessor.GetMethod;

                if (XAxisValueType == ChartValueType.String)
                {
                    IList<string> xValue = this.XValues as List<string>;
                    do
                    {
                        object xVal = xGetMethod(enumerator.Current);
                        object yVal = yGetMethod(enumerator.Current);
                        xValue.Add(xVal != null ? (string)xVal : string.Empty);
                        YCollection.Add((IList<double>)yVal ?? new double[0]);
                        ActualData.Add(enumerator.Current);
                    }
                    while (enumerator.MoveNext());
                    DataCount = xValue.Count;
                }
                else if (XAxisValueType == ChartValueType.Double || XAxisValueType == ChartValueType.Logarithmic)
                {
                    IList<double> xValue = this.XValues as List<double>;
                    do
                    {
                        object xVal = xGetMethod(enumerator.Current);
                        object yVal = yGetMethod(enumerator.Current);
                        XData = Convert.ToDouble(xVal != null ? xVal : double.NaN);
                        xValue.Add(XData);
                        YCollection.Add((IList<double>)yVal ?? new double[0]);
                        ActualData.Add(enumerator.Current);
                    }
                    while (enumerator.MoveNext());
                    DataCount = xValue.Count;
                }
                else if (XAxisValueType == ChartValueType.DateTime)
                {
                    IList<double> xValue = this.XValues as List<double>;
                    do
                    {
                        object xVal = xGetMethod(enumerator.Current);
                        object yVal = yGetMethod(enumerator.Current);
                        XData = ((DateTime)xVal).ToOADate();
                        xValue.Add(XData);
                        YCollection.Add((IList<double>)yVal ?? new double[0]);
                        ActualData.Add(enumerator.Current);
                    }
                    while (enumerator.MoveNext());
                    DataCount = xValue.Count;
                }
                else if (XAxisValueType == ChartValueType.TimeSpan)
                {
                    IList<double> xValue = this.XValues as List<double>;
                    do
                    {
                        object xVal = xGetMethod(enumerator.Current);
                        object yVal = yGetMethod(enumerator.Current);
                        XData = ((TimeSpan)xVal).TotalMilliseconds;
                        xValue.Add(XData);
                        YCollection.Add((IList<double>)yVal ?? new double[0]);
                        ActualData.Add(enumerator.Current);
                    }
                    while (enumerator.MoveNext());
                    DataCount = xValue.Count;
                }
            }

            IsPointGenerated = true;
        }

        internal override void GenerateComplexPropertyPoints(string[] yPaths, IList<double>[] yLists, GetReflectedProperty getPropertyValue)
        {
            IEnumerator enumerator = (ItemsSource as IEnumerable).GetEnumerator();
            if (enumerator.MoveNext())
            {
                for (int i = 0; i < UpdateStartedIndex; i++)
                    enumerator.MoveNext();

                XAxisValueType = GetDataType(ItemsSource as IEnumerable, XComplexPaths);

                if (XAxisValueType == ChartValueType.DateTime || XAxisValueType == ChartValueType.Double ||
                   XAxisValueType == ChartValueType.Logarithmic || XAxisValueType == ChartValueType.TimeSpan)
                {
                    if (!(XValues is List<double>))
                        this.ActualXValues = this.XValues = new List<double>();
                }
                else
                {
                    if (!(XValues is List<string>))
                        this.ActualXValues = this.XValues = new List<string>();
                }

                string[] tempYPath = YComplexPaths[0];
                if (string.IsNullOrEmpty(yPaths[0]))
                    return;

                object xVal = null, yVal = null;

                if (XAxisValueType == ChartValueType.String)
                {
                    IList<string> xValue = this.XValues as List<string>;
                    do
                    {
                        xVal = getPropertyValue(enumerator.Current, XComplexPaths);
                        yVal = getPropertyValue(enumerator.Current, tempYPath);
                        if (xVal == null) return;
                        xValue.Add((string)xVal);
                        YCollection.Add((IList<double>)yVal ?? new double[0]);
                        ActualData.Add(enumerator.Current);
                    }
                    while (enumerator.MoveNext());
                    DataCount = xValue.Count;
                }
                else if (XAxisValueType == ChartValueType.Double ||
                    XAxisValueType == ChartValueType.Logarithmic)
                {
                    IList<double> xValue = this.XValues as List<double>;
                    do
                    {
                        xVal = getPropertyValue(enumerator.Current, XComplexPaths);
                        yVal = getPropertyValue(enumerator.Current, tempYPath);
                        if (xVal == null) return;
                        XData = Convert.ToDouble(xVal);
                        xValue.Add(XData);
                        YCollection.Add((IList<double>)yVal ?? new double[0]);
                        ActualData.Add(enumerator.Current);
                    }
                    while (enumerator.MoveNext());
                    DataCount = xValue.Count;
                }
                else if (XAxisValueType == ChartValueType.DateTime)
                {
                    IList<double> xValue = this.XValues as List<double>;
                    do
                    {
                        xVal = getPropertyValue(enumerator.Current, XComplexPaths);
                        yVal = getPropertyValue(enumerator.Current, tempYPath);
                        if (xVal == null) return;
                        XData = ((DateTime)xVal).ToOADate();
                        xValue.Add(XData);
                        YCollection.Add((IList<double>)yVal ?? new double[0]);
                        ActualData.Add(enumerator.Current);
                    }
                    while (enumerator.MoveNext());
                    DataCount = xValue.Count;
                }
                else if (XAxisValueType == ChartValueType.TimeSpan)
                {
                    IList<double> xValue = this.XValues as List<double>;
                    do
                    {
                        xVal = getPropertyValue(enumerator.Current, XComplexPaths);
                        yVal = getPropertyValue(enumerator.Current, tempYPath);
                        if (xVal == null) return;
                        XData = ((TimeSpan)xVal).TotalMilliseconds;
                        xValue.Add(XData);
                        YCollection.Add((IList<double>)yVal ?? new double[0]);
                        ActualData.Add(enumerator.Current);
                    }
                    while (enumerator.MoveNext());
                    DataCount = xValue.Count;
                }
            }

            IsPointGenerated = true;
        }

        internal override void OnDataCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        #endregion
        
        #region Protected Internal Methods

        /// <summary>
        /// Method for Generate Points for XYDataSeries
        /// </summary>
        protected internal override void GeneratePoints()
        {
            if (YCollection != null)
                YCollection.Clear();
            else
                YCollection = new List<IList<double>>();
            base.GeneratePoints();
        }

        #endregion

        #region Protected Override Methods

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            base.OnBindingPathChanged(args);
        }

        /// <summary>
        /// Called when DataSource property changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (YCollection != null)
                YCollection.Clear();
            else
                YCollection = new List<IList<double>>();
            base.OnDataSourceChanged(oldValue, newValue);
        }

        protected override void ClearUnUsedSegments(int startIndex)
        {
            // Clearing the scattersegments.
            var scatterSegments = new List<ChartSegment>();
            foreach (var segment in Segments.Where(item => item is ScatterSegment))
            {
                scatterSegments.Add(segment);
            }

            foreach (var segment in scatterSegments)
            {
                Segments.Remove(segment);
            }

            if (this.Segments.Count > startIndex)
            {
                int count = this.Segments.Count;

                for (int i = startIndex; i < count; i++)
                {
                    this.Segments.RemoveAt(startIndex);
                }
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new BoxAndWhiskerSeries()
            {
                SegmentSpacing = this.SegmentSpacing,
                SelectedIndex = this.SelectedIndex,
                SegmentSelectionBrush = this.SegmentSelectionBrush,
                SeriesSelectionBrush = this.SeriesSelectionBrush,
                ////Percentile = this.Percentile,
                BoxPlotMode = this.BoxPlotMode,
            });
        }

        #endregion

        #region Protected Methods

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        protected double CalculateSegmentSpacing(double spacing, double Right, double Left)
        {
            double diff = Right - Left;
            double totalspacing = diff * spacing / 2;
            Left = Left + totalspacing;
            Right = Right - totalspacing;
            return Left;
        }

        #endregion

        #region Private Static Methods

        private static void OnShowMedianChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var boxSeries = d as BoxAndWhiskerSeries;
            var boxSegments = boxSeries.Segments.Where(x => x is BoxAndWhiskerSegment);
            var showMedian = (bool)e.NewValue;
            foreach (BoxAndWhiskerSegment boxSegment in boxSegments)
            {
                boxSegment.UpdateMeanSymbol(boxSeries.ChartTransformer, showMedian);
            }
        }

        private static void OnYBindingPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BoxAndWhiskerSeries).OnBindingPathChanged(e);
        }

        private static void OnOutlierTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as BoxAndWhiskerSeries;
            if (series.Area == null) return;
            series.Segments.Clear();
            series.Area.ScheduleUpdate();
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

        private static void OnSegmentSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as BoxAndWhiskerSeries;
            if (series.Area != null)
                series.Area.ScheduleUpdate();
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as BoxAndWhiskerSeries;
            series.UpdateArea();
        }

        private static void BindProperties(ScatterSegment scatterSegment, ChartSegment currentSegment)
        {
            Binding binding = new Binding();
            binding.Source = currentSegment;
            binding.Path = new PropertyPath("Interior");
            BindingOperations.SetBinding(scatterSegment, ChartSegment.InteriorProperty, binding);
        }

        private static double GetExclusiveQuartileValue(double[] ylist, int count, double percentile)
        {
            if (count == 0)
                return 0;
            else if (count == 1)
                return ylist[0];

            double rank = percentile * (count + 1);
            int integerRank = (int)Math.Abs(rank);
            double fractionRank = rank - integerRank;
            double value = 0d;
            if (integerRank == 0)
                value = ylist[0];
            else if (integerRank > count - 1)
                value = ylist[count - 1];
            else
                value = fractionRank * (ylist[integerRank] - ylist[integerRank - 1]) + ylist[integerRank - 1];
            return value;
        }

        private static double GetInclusiveQuartileValue(double[] ylist, int count, double percentile)
        {
            if (count == 0)
                return 0;
            else if (count == 1)
                return ylist[0];

            double rank = percentile * (count - 1);
            int integerRank = (int)Math.Abs(rank);
            double fractionRank = rank - integerRank;
            double value = fractionRank * (ylist[integerRank + 1] - ylist[integerRank]) + ylist[integerRank];
            return value;
        }

        private static void GetMinMaxandOutlier(double lowerQuartile, double upperQuartile, double[] ylist,
                                         out double minimum, out double maximum, List<double> outliers)
        {
            minimum = 0d;
            maximum = 0d;
            double interquartile = upperQuartile - lowerQuartile;
            double rangeIQR = 1.5 * interquartile;

            for (int i = 0; i < ylist.Length; i++)
                if (ylist[i] < lowerQuartile - rangeIQR)
                    outliers.Add(ylist[i]);
                else
                {
                    minimum = ylist[i];
                    break;
                }

            for (int i = ylist.Length - 1; i >= 0; i--)
                if (ylist[i] > upperQuartile + rangeIQR)
                    outliers.Add(ylist[i]);
                else
                {
                    maximum = ylist[i];
                    break;
                }
        }

        #endregion

        #region Private Methods
        
        private void UpdateAdornmentLabelPositiion()
        {
            if (this.IsActualTransposed)
            {
                topLabelPosition = this.ActualYAxis.IsInversed ? ActualLabelPosition.Left : ActualLabelPosition.Right;
                bottomLabelPosition = this.ActualYAxis.IsInversed ? ActualLabelPosition.Right : ActualLabelPosition.Left;
            }
            else
            {
                topLabelPosition = this.ActualYAxis.IsInversed ? ActualLabelPosition.Bottom : ActualLabelPosition.Top;
                bottomLabelPosition = this.ActualYAxis.IsInversed ? ActualLabelPosition.Top : ActualLabelPosition.Bottom;
            }
        }

        private void UpdateWhiskerWidth()
        {
            if (WhiskerWidth == null)
                whiskerWidth = 1;
            else if (WhiskerWidth < 0)
                whiskerWidth = 0;
            else if (WhiskerWidth > 1)
                whiskerWidth = 1;
            else
                whiskerWidth = (double)WhiskerWidth;
        }

        private void SetBoxWhiskerAdornments(DoubleRange sbsInfo, double xValue, double minimum, double maximum, double x1, double median, double lowerQuartile, double upperQuartile, List<double> outliers, int index)
        {
            if (AdornmentsInfo != null)
            {
                var sbsMedian = sbsInfo.Delta / 2;
                var adornmentIndex = index * 5;

                Adornments[adornmentIndex].SetData(xValue, minimum, x1 + sbsMedian, minimum);
                Adornments[adornmentIndex].ActualLabelPosition = bottomLabelPosition;
                Adornments[adornmentIndex].Item = ActualData[index];

                Adornments[++adornmentIndex].SetData(xValue, lowerQuartile, x1 + sbsMedian, lowerQuartile);
                Adornments[adornmentIndex].ActualLabelPosition = bottomLabelPosition;
                Adornments[adornmentIndex].Item = ActualData[index];

                Adornments[++adornmentIndex].SetData(xValue, median, x1 + sbsMedian, median);
                Adornments[adornmentIndex].ActualLabelPosition = topLabelPosition;
                Adornments[adornmentIndex].Item = ActualData[index];

                Adornments[++adornmentIndex].SetData(xValue, upperQuartile, x1 + sbsMedian, upperQuartile);
                Adornments[adornmentIndex].ActualLabelPosition = topLabelPosition;
                Adornments[adornmentIndex].Item = ActualData[index];

                Adornments[++adornmentIndex].SetData(xValue, maximum, x1 + sbsMedian, maximum);
                Adornments[adornmentIndex].ActualLabelPosition = topLabelPosition;
                Adornments[adornmentIndex].Item = ActualData[index];
            }
        }

        private void AddBoxWhiskerAdornments(DoubleRange sbsInfo, double xValue, double minimum, double maximum, double x1, double median, double lowerQuartile, double upperQuartile, List<double> outliers, int index)
        {
            if (AdornmentsInfo != null)
            {
                ChartAdornment adornment = null;
                var sbsMedian = sbsInfo.Delta / 2;

                adornment = this.CreateAdornment(this, xValue, minimum, x1 + sbsMedian, minimum);
                adornment.ActualLabelPosition = bottomLabelPosition;
                adornment.Item = ActualData[index];
                Adornments.Add(adornment);

                adornment = this.CreateAdornment(this, xValue, lowerQuartile, x1 + sbsMedian, lowerQuartile);
                adornment.ActualLabelPosition = bottomLabelPosition;
                adornment.Item = ActualData[index];
                Adornments.Add(adornment);

                adornment = this.CreateAdornment(this, xValue, median, x1 + sbsMedian, median);
                adornment.ActualLabelPosition = topLabelPosition;
                adornment.Item = ActualData[index];
                Adornments.Add(adornment);

                adornment = this.CreateAdornment(this, xValue, upperQuartile, x1 + sbsMedian, upperQuartile);
                adornment.ActualLabelPosition = topLabelPosition;
                adornment.Item = ActualData[index];
                Adornments.Add(adornment);

                adornment = this.CreateAdornment(this, xValue, maximum, x1 + sbsMedian, maximum);
                adornment.ActualLabelPosition = topLabelPosition;
                adornment.Item = ActualData[index];
                Adornments.Add(adornment);
            }
        }

        ////private void UpdatePercentile()
        ////{
        ////    if (Percentile < 0)
        ////    {
        ////        lowerPercentile = 0;
        ////        upperPercentile = 1;

        ////    }
        ////    else if (Percentile > 0.25)
        ////    {
        ////        lowerPercentile = 0.25;
        ////        upperPercentile = 0.75;
        ////    }
        ////    else
        ////    {
        ////        lowerPercentile = Percentile;
        ////        upperPercentile = 1 - Percentile;
        ////    }
        ////}

        private void GetQuartileValues(double[] ylist, int len, out double lowerQuartile, out double upperQuartile)
        {
            double[] lowerQuartileArray;
            double[] upperQuartileArray;

            if (len == 1)
            {
                lowerQuartile = ylist[0];
                upperQuartile = ylist[0];
                return;
            }

            var halfLength = len / 2;

            lowerQuartileArray = new double[halfLength];
            upperQuartileArray = new double[halfLength];

            Array.Copy(ylist, 0, lowerQuartileArray, 0, halfLength);
            Array.Copy(ylist, isEvenList ? halfLength : halfLength + 1, upperQuartileArray, 0, halfLength);

            lowerQuartile = GetMedianValue(lowerQuartileArray);
            upperQuartile = GetMedianValue(upperQuartileArray);
        }

        private static double GetMedianValue(double[] ylist)
        {
            int len = ylist.Length;

            if (len == 0)
                return 0;
            else if (len == 1)
                return ylist[0];

            double median = 0d;
            int middleindex = (int)Math.Round(len / 2.0, MidpointRounding.AwayFromZero);

            if (len % 2 == 0)
                median = (ylist[middleindex - 1] + ylist[middleindex]) / 2;
            else
                median = ylist[middleindex - 1];
            return median;
        }

        #endregion

        #endregion
    }
}
