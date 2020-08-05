using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.Foundation;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// SplineRangeAreaSeries connects it data points using a smooth curve with the areas between the high value and low value are filled in.
    /// </summary>
    /// <seealso cref="SplineRangeAreaSegment"/>
    /// <seealso cref="RangeColumnSeries"/>
    /// <seealso cref="AreaSeries"/>
    /// <seealso cref="SplineAreaSeries"/>
    public partial class SplineRangeAreaSeries : RangeAreaSeries
    {
        #region Dependency Property Registration

        /// <summary>
        /// Using a DependencyProperty as the backing store for SplineType.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SplineTypeProperty =
            DependencyProperty.Register(
                "SplineType", 
                typeof(SplineType), 
                typeof(SplineRangeAreaSeries), 
                new PropertyMetadata(SplineType.Natural, OnSplineTypeChanged));

        #endregion

        #region Fields

        #region Private Fields

        List<ChartPoint> startControlPoints;

        List<ChartPoint> endControlPoints;

        #endregion

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets SplineType enum value which indicates the spline series type. 
        /// </summary>
        public SplineType SplineType
        {
            get { return (SplineType)GetValue(SplineTypeProperty); }
            set { SetValue(SplineTypeProperty, value); }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        public override void CreateSegments()
        {
            double[] highCoef = null;
            double[] lowCoef = null;

            List<ChartPoint> segmentPoints = new List<ChartPoint>();
            List<ChartPoint> HighStartControlPoints = new List<ChartPoint>();
            List<ChartPoint> HighEndControlPoints = new List<ChartPoint>();
            List<ChartPoint> LowStartControlPoints = new List<ChartPoint>();
            List<ChartPoint> LowEndControlPoints = new List<ChartPoint>();

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
                    var groupDataCount = xValues.Count;

                    Segments.Clear();
                    Adornments.Clear();
                    if (SplineType == SplineType.Monotonic)
                    {
                        GetMonotonicSpline(xValues, GroupedSeriesYValues[0]);
                        HighStartControlPoints = startControlPoints;
                        HighEndControlPoints = endControlPoints;
                        GetMonotonicSpline(xValues, GroupedSeriesYValues[1]);
                        LowStartControlPoints = startControlPoints;
                        LowEndControlPoints = endControlPoints;
                    }
                    else if (SplineType == SplineType.Cardinal)
                    {
                        GetCardinalSpline(xValues, GroupedSeriesYValues[0]);
                        HighStartControlPoints = startControlPoints;
                        HighEndControlPoints = endControlPoints;
                        GetCardinalSpline(xValues, GroupedSeriesYValues[1]);
                        LowStartControlPoints = startControlPoints;
                        LowEndControlPoints = endControlPoints;
                    }
                    else
                    {
                        this.NaturalSpline(xValues, GroupedSeriesYValues[0], out highCoef);
                        this.NaturalSpline(xValues, GroupedSeriesYValues[1], out lowCoef);
                    }

                    for (int i = 0; i < groupDataCount; i++)
                    {

                        if (!double.IsNaN(GroupedSeriesYValues[1][i]) && !double.IsNaN(GroupedSeriesYValues[0][i]))
                        {
                            if (i == 0 || (i < DataCount - 1 && (double.IsNaN(GroupedSeriesYValues[1][i - 1]) || double.IsNaN(GroupedSeriesYValues[0][i - 1]))))
                            {
                                ChartPoint highInitialPoint = new ChartPoint(xValues[i], GroupedSeriesYValues[1][i]);
                                segmentPoints.Add(highInitialPoint);
                                ChartPoint lowInitialPoint = new ChartPoint(xValues[i], GroupedSeriesYValues[0][i]);
                                segmentPoints.Add(lowInitialPoint);
                            }
                            else
                            {
                                ChartPoint highStartPoint = new ChartPoint(xValues[i - 1], GroupedSeriesYValues[0][i - 1]);
                                ChartPoint highEndPoint = new ChartPoint(xValues[i], GroupedSeriesYValues[0][i]);
                                ChartPoint lowStartPoint = new ChartPoint(xValues[i - 1], GroupedSeriesYValues[1][i - 1]);
                                ChartPoint lowEndPoint = new ChartPoint(xValues[i], GroupedSeriesYValues[1][i]);
                                ChartPoint startControlPoint;
                                ChartPoint endControlPoint;

                                if (SplineType == SplineType.Monotonic)
                                {

                                    segmentPoints.AddRange(new ChartPoint[] { HighStartControlPoints[i - 1], HighEndControlPoints[i - 1], highEndPoint });
                                    segmentPoints.AddRange(new ChartPoint[] { lowStartPoint, LowStartControlPoints[i - 1], LowEndControlPoints[i - 1] });
                                }
                                else if (SplineType == SplineType.Cardinal)
                                {
                                    segmentPoints.AddRange(new ChartPoint[] { HighStartControlPoints[i - 1], HighEndControlPoints[i - 1], highEndPoint });
                                    segmentPoints.AddRange(new ChartPoint[] { lowStartPoint, LowStartControlPoints[i - 1], LowEndControlPoints[i - 1] });
                                }
                                else
                                {
                                    GetBezierControlPoints(highStartPoint, highEndPoint, highCoef[i - 1], highCoef[i], out startControlPoint, out endControlPoint);
                                    segmentPoints.AddRange(new ChartPoint[] { startControlPoint, endControlPoint, highEndPoint });
                                    GetBezierControlPoints(lowStartPoint, lowEndPoint, lowCoef[i - 1], lowCoef[i], out startControlPoint, out endControlPoint);
                                    segmentPoints.AddRange(new ChartPoint[] { lowStartPoint, startControlPoint, endControlPoint });
                                }
                            }
                        }
                        else
                        {
                            if (segmentPoints.Count > 0)
                            {
                                ChartPoint endpoint = new ChartPoint(xValues[i - 1], GroupedSeriesYValues[1][i - 1]);
                                segmentPoints.Add(endpoint);

                                var Segment = new SplineRangeAreaSegment(segmentPoints, this);
                                Segment.SetData(segmentPoints);
                                Segments.Add(Segment);
                            }
                            segmentPoints = new List<ChartPoint>();
                        }
                    }

                    if (segmentPoints.Count > 0)
                    {
                        ChartPoint endpoint = new ChartPoint(xValues[groupDataCount - 1], GroupedSeriesYValues[1][groupDataCount - 1]);
                        segmentPoints.Add(endpoint);

                        var Segment = new SplineRangeAreaSegment(segmentPoints, this);
                        Segment.SetData(segmentPoints);
                        Segments.Add(Segment);
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

                    if (SplineType == SplineType.Monotonic)
                    {
                        GetMonotonicSpline(xValues, HighValues);
                        HighStartControlPoints = startControlPoints;
                        HighEndControlPoints = endControlPoints;
                        GetMonotonicSpline(xValues, LowValues);
                        LowStartControlPoints = startControlPoints;
                        LowEndControlPoints = endControlPoints;
                    }
                    else if (SplineType == SplineType.Cardinal)
                    {
                        GetCardinalSpline(xValues, HighValues);
                        HighStartControlPoints = startControlPoints;
                        HighEndControlPoints = endControlPoints;
                        GetCardinalSpline(xValues, LowValues);
                        LowStartControlPoints = startControlPoints;
                        LowEndControlPoints = endControlPoints;
                    }
                    else
                    {
                        this.NaturalSpline(xValues, HighValues, out highCoef);
                        this.NaturalSpline(xValues, LowValues, out lowCoef);
                    }

                    for (int i = 0; i < DataCount; i++)
                    {

                        if (!double.IsNaN(LowValues[i]) && !double.IsNaN(HighValues[i]))
                        {
                            if (i == 0 || (i < DataCount - 1 && (double.IsNaN(LowValues[i - 1]) || double.IsNaN(HighValues[i - 1]))))
                            {
                                ChartPoint highInitialPoint = new ChartPoint(xValues[i], LowValues[i]);
                                segmentPoints.Add(highInitialPoint);
                                ChartPoint lowInitialPoint = new ChartPoint(xValues[i], HighValues[i]);
                                segmentPoints.Add(lowInitialPoint);
                            }
                            else
                            {
                                ChartPoint highStartPoint = new ChartPoint(xValues[i - 1], HighValues[i - 1]);
                                ChartPoint highEndPoint = new ChartPoint(xValues[i], HighValues[i]);
                                ChartPoint lowStartPoint = new ChartPoint(xValues[i - 1], LowValues[i - 1]);
                                ChartPoint lowEndPoint = new ChartPoint(xValues[i], LowValues[i]);
                                ChartPoint startControlPoint;
                                ChartPoint endControlPoint;

                                if (SplineType == SplineType.Monotonic)
                                {
                                    segmentPoints.AddRange(new ChartPoint[] { HighStartControlPoints[i - 1], HighEndControlPoints[i - 1], highEndPoint });
                                    segmentPoints.AddRange(new ChartPoint[] { lowStartPoint, LowStartControlPoints[i - 1], LowEndControlPoints[i - 1] });
                                }
                                else if (SplineType == SplineType.Cardinal)
                                {
                                    segmentPoints.AddRange(new ChartPoint[] { HighStartControlPoints[i - 1], HighEndControlPoints[i - 1], highEndPoint });
                                    segmentPoints.AddRange(new ChartPoint[] { lowStartPoint, LowStartControlPoints[i - 1], LowEndControlPoints[i - 1] });
                                }
                                else
                                {
                                    GetBezierControlPoints(highStartPoint, highEndPoint, highCoef[i - 1], highCoef[i], out startControlPoint, out endControlPoint);
                                    segmentPoints.AddRange(new ChartPoint[] { startControlPoint, endControlPoint, highEndPoint });
                                    GetBezierControlPoints(lowStartPoint, lowEndPoint, lowCoef[i - 1], lowCoef[i], out startControlPoint, out endControlPoint);
                                    segmentPoints.AddRange(new ChartPoint[] { lowStartPoint, startControlPoint, endControlPoint });
                                }
                            }
                        }
                        else
                        {
                            if (segmentPoints.Count > 0)
                            {
                                ChartPoint endpoint = new ChartPoint(xValues[i - 1], LowValues[i - 1]);
                                segmentPoints.Add(endpoint);

                                var Segment = new SplineRangeAreaSegment(segmentPoints, this);
                                Segment.SetData(segmentPoints);
                                Segments.Add(Segment);
                            }
                            segmentPoints = new List<ChartPoint>();
                        }
                    }

                    if (segmentPoints.Count > 0)
                    {
                        ChartPoint endpoint = new ChartPoint(xValues[DataCount - 1], LowValues[DataCount - 1]);
                        segmentPoints.Add(endpoint);

                        var Segment = new SplineRangeAreaSegment(segmentPoints, this);
                        Segment.SetData(segmentPoints);
                        Segments.Add(Segment);
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

        #region Internal Methods

        internal void GetCardinalSpline(List<double> xValues, IList<double> yValues)
        {
            int count = 0;
            startControlPoints = new List<ChartPoint>(DataCount);
            endControlPoints = new List<ChartPoint>(DataCount);
            bool isGrouping = this.ActualXAxis is CategoryAxis ? (this.ActualXAxis as CategoryAxis).IsIndexed : true;

            if (!isGrouping)
                count = (int)xValues.Count;
            else
                count = (int)DataCount;

            double[] tangentsX = new double[count];
            double[] tangentsY = new double[count];

            for (int i = 0; i < count; i++)
            {
                if (i == 0 && xValues.Count > 2)
                    tangentsX[i] = (0.5 * (xValues[i + 2] - xValues[i]));
                else if (i == count - 1 && count - 3 >= 0)
                    tangentsX[i] = (0.5 * (xValues[count - 1] - xValues[count - 3]));
                else if (i - 1 >= 0 && xValues.Count > i + 1)
                    tangentsX[i] = (0.5 * (xValues[i + 1] - xValues[i - 1]));
                if (double.IsNaN(tangentsX[i]))
                    tangentsX[i] = 0;

                if (ActualXAxis is DateTimeAxis)
                {
                    DateTime date = xValues[i].FromOADate();
                    if ((ActualXAxis as DateTimeAxis).IntervalType == DateTimeIntervalType.Auto ||
                            (ActualXAxis as DateTimeAxis).IntervalType == DateTimeIntervalType.Years)
                    {
                        int year = DateTime.IsLeapYear(date.Year) ? 366 : 365;
                        tangentsY[i] = tangentsX[i] / year;
                    }
                    else if ((ActualXAxis as DateTimeAxis).IntervalType == DateTimeIntervalType.Months)
                    {
                        double month = DateTime.DaysInMonth(date.Year, date.Month);
                        tangentsY[i] = tangentsX[i] / month;
                    }
                }
                else if (ActualXAxis is LogarithmicAxis)
                {
                    tangentsX[i] = Math.Log(tangentsX[i], (ActualXAxis as LogarithmicAxis).LogarithmicBase);
                    tangentsY[i] = tangentsX[i];
                }
                else
                    tangentsY[i] = tangentsX[i];
            }

            for (int i = 0; i < tangentsX.Length - 1; i++)
            {
                startControlPoints.Add(new ChartPoint(xValues[i] + tangentsX[i] / 3, yValues[i] + tangentsY[i] / 3));
                endControlPoints.Add(new ChartPoint(xValues[i + 1] - tangentsX[i + 1] / 3, yValues[i + 1] - tangentsY[i + 1] / 3));
            }
        }

        internal void GetMonotonicSpline(List<double> xValues, IList<double> yValues)
        {
            int count = 0;
            startControlPoints = new List<ChartPoint>(DataCount);
            endControlPoints = new List<ChartPoint>(DataCount);
            bool isGrouping = this.ActualXAxis is CategoryAxis ? (this.ActualXAxis as CategoryAxis).IsIndexed : true;
            if (!isGrouping)
                count = (int)xValues.Count;
            else
                count = (int)DataCount;
            double[] dx = new double[count - 1];
            double[] slope = new double[count - 1];
            List<double> coefficent = new List<double>();

            // Find the slope between the values.
            for (int i = 0; i < count - 1; i++)
            {
                if (!double.IsNaN(yValues[i + 1]) && !double.IsNaN(yValues[i])
                    && !double.IsNaN(xValues[i + 1]) && !double.IsNaN(xValues[i]))
                {
                    dx[i] = xValues[i + 1] - xValues[i];
                    slope[i] = (yValues[i + 1] - yValues[i]) / dx[i];
                    if (double.IsInfinity(slope[i]))
                        slope[i] = 0;
                }
            }

            // Add the first and last coefficent value as Slope[0] and Slope[n-1]
            if (slope.Length == 0) return;
            coefficent.Add(double.IsNaN(slope[0]) ? 0 : slope[0]);
            for (int i = 0; i < dx.Length - 1; i++)
            {
                if (slope.Length > i + 1)
                {
                    double m = slope[i], m_next = slope[i + 1];
                    if (m * m_next <= 0)
                    {
                        coefficent.Add(0);
                    }
                    else
                    {
                        if (dx[i] == 0)
                            coefficent.Add(0);
                        else
                        {
                            double firstPoint = dx[i], nextPoint = dx[i + 1];
                            double interPoint = firstPoint + nextPoint;
                            coefficent.Add(3 * interPoint / ((interPoint + nextPoint) / m + (interPoint + firstPoint) / m_next));
                        }
                    }
                }
            }

            coefficent.Add(double.IsNaN(slope[slope.Length - 1]) ? 0 : slope[slope.Length - 1]);

            for (int i = 0; i < coefficent.Count; i++)
            {
                if (i + 1 < coefficent.Count && dx.Length > 0)
                {
                    double value = (dx[i] / 3);
                    startControlPoints.Add(new ChartPoint(xValues[i] + value, yValues[i] + coefficent[i] * value));
                    endControlPoints.Add(new ChartPoint(xValues[i + 1] - value, yValues[i + 1] - coefficent[i + 1] * value));
                }
            }
        }

        /// <summary>
        /// Method implementation for NaturalSpline
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        /// <param name="ys2"></param>
        internal void NaturalSpline(List<double> xValues, IList<double> yValues, out double[] ys2)
        {
            int count = 0;

            bool isGrouping = this.ActualXAxis is CategoryAxis ? (this.ActualXAxis as CategoryAxis).IsIndexed : true;
            if (!isGrouping)
                count = (int)xValues.Count;
            else
                count = (int)DataCount;
            ys2 = new double[count];
            if (count == 1)
                return;
            double a = 6;
            double[] u = new double[count - 1];
            double p;

            if (SplineType == SplineType.Natural)
            {
                ys2[0] = u[0] = 0;
                ys2[count - 1] = 0;
            }
            else if (xValues.Count > 1)
            {
                double d0 = (xValues[1] - xValues[0]) / (yValues[1] - yValues[0]);
                double dn = (xValues[count - 1] - xValues[count - 2]) / (yValues[count - 1] - yValues[count - 2]);
                u[0] = 0.5;
                ys2[0] = (3 * (yValues[1] - yValues[0])) / (xValues[1] - xValues[0]) - 3 * d0;
                ys2[count - 1] = 3 * dn - (3 * (yValues[count - 1] - yValues[count - 2])) / (xValues[count - 1] - xValues[count - 2]);
                if (double.IsInfinity(ys2[0]) || double.IsNaN(ys2[0]))
                    ys2[0] = 0;
                if (double.IsInfinity(ys2[count - 1]) || double.IsNaN(ys2[count - 1]))
                    ys2[count - 1] = 0;
            }

            for (int i = 1; i < count - 1; i++)
            {
                if (yValues.Count > i + 1 && !double.IsNaN(yValues[i + 1]) && !double.IsNaN(yValues[i - 1]) && !double.IsNaN(yValues[i]))
                {
                    double d1 = xValues[i] - xValues[i - 1];
                    double d2 = xValues[i + 1] - xValues[i - 1];
                    double d3 = xValues[i + 1] - xValues[i];
                    double dy1 = yValues[i + 1] - yValues[i];
                    double dy2 = yValues[i] - yValues[i - 1];

                    if (xValues[i] == xValues[i - 1] || xValues[i] == xValues[i + 1])
                    {
                        ys2[i] = 0;
                        u[i] = 0;
                    }
                    else
                    {
                        p = 1 / (d1 * ys2[i - 1] + 2 * d2);
                        ys2[i] = -p * d3;
                        u[i] = p * (a * (dy1 / d3 - dy2 / d1) - d1 * u[i - 1]);
                    }
                }
            }

            for (int k = count - 2; k >= 0; k--)
            {
                ys2[k] = ys2[k] * ys2[k + 1] + u[k];
            }
        }

        /// <summary>
        /// Method implementation for GetBezierControlPoints
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="ys1"></param>
        /// <param name="ys2"></param>
        /// <param name="controlPoint1"></param>
        /// <param name="controlPoint2"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        internal void GetBezierControlPoints(ChartPoint point1, ChartPoint point2, double ys1, double ys2, out ChartPoint controlPoint1, out ChartPoint controlPoint2)
        {
            const double One_thrid = 1 / 3.0d;
            double deltaX2 = point2.X - point1.X;

            deltaX2 = deltaX2 * deltaX2;

            double dx1 = 2 * point1.X + point2.X;
            double dx2 = point1.X + 2 * point2.X;

            double dy1 = 2 * point1.Y + point2.Y;
            double dy2 = point1.Y + 2 * point2.Y;

            double y1 = One_thrid * (dy1 - One_thrid * deltaX2 * (ys1 + 0.5f * ys2));
            double y2 = One_thrid * (dy2 - One_thrid * deltaX2 * (0.5f * ys1 + ys2));

            controlPoint1 = new ChartPoint(dx1 * One_thrid, y1);
            controlPoint2 = new ChartPoint(dx2 * One_thrid, y2);
        }

        #endregion

        #region Private Static Methods

        private static void OnSplineTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SplineRangeAreaSeries)d).UpdateArea();
        }

        #endregion

        #endregion
    }
}
