using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
using System.Reflection;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Contains Chart extension methods.
    /// </summary>
  
    public static class ChartExtensionUtils
    {
        internal static DateTime ValidateNonWorkingDate(this DateTime date, string days, bool isToBack,int nonWokingDays)
        {
            var i = 0;
            var isNonWorkingDay = false;
            do
            {
                isNonWorkingDay = false;
                if (!days.Contains(date.DayOfWeek.ToString()))
                {
                    i++;
                    isNonWorkingDay = true;
                    date = date.AddDays(isToBack ? -nonWokingDays : nonWokingDays);
                    break;
                }
            } while (isNonWorkingDay);
            return date;
        }

        internal static DateTime ValidateNonWorkingHours(this DateTime date, double startTime, double endTime, bool isToBack)
        {

            var time = date.TimeOfDay.TotalHours;
            if (isToBack)
            {
                if (time < startTime)
                {
                    date = date.AddDays(-1);//.AddHours(-(endTime - (time - startTime)));
                    date = new DateTime(date.Year, date.Month, date.Day, (int)(endTime - (startTime - time)), date.Minute, date.Second);
                }
                else if (time > endTime)
                {
                    date = date.AddHours(-(time - endTime));
                }
            }
            else
            {
                if (time < startTime)
                {
                    date = date.AddHours(startTime - time);
                }
                else if (time > endTime)
                {
                    date = new DateTime(date.Year, date.Month, date.Day).AddHours(24).AddHours(startTime);
                }
            }
            return date;
        }

        /// <summary>
        /// Method used to gets or sets intersect of two rectangle.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool IntersectsWith(this Rect rect, Rect other)
        {
            if (other.Bottom < rect.Top || other.Right < rect.Left
             || other.Top > rect.Bottom || other.Left > rect.Right)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Method  used to set the offset value.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Rect Offset(this Rect rect, double x, double y)
        {
            return new Rect(rect.X + x, rect.Y + y, rect.Width - x, rect.Height - y);
        }

        internal static List<Vector3D> Get3DVector(this List<Point> points, double depth)
        {
            var vector3D = new List<Vector3D>();
            foreach (var point in points)
            {
                vector3D.Add(new Vector3D(point, depth));
            }
            return vector3D;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
           (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            return source != null ? source.Where(element => seenKeys.Add(keySelector(element))) : null;
        }

        private static DateTime BaseDate = new DateTime(1899, 12, 30);

        /// <summary>
        /// Converts the value of this instance to the equivalent OLE Automation date.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
       
        public static double ToOADate(this DateTime time)
        {
            return time.Subtract(BaseDate).TotalDays;
        }

        /// <summary>
        /// Returns a DateTime equivalent to the specified OLE Automation Date.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
       
        public static DateTime FromOADate(this Double value)
        {
            return BaseDate.AddDays(value);
        }

#if NETFX_CORE 

        internal static MethodInfo GetSetMethod(this PropertyInfo propertyInfo)
        {
            return propertyInfo.SetMethod;
        }

        internal static MethodInfo GetGetMethod(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetMethod;
        }

#endif

        /// <summary>
        /// Returns sum of DoubleRange
        /// </summary>
        /// <param name="ranges">Collection of DoubleRange</param>
        /// <returns></returns>
        public static DoubleRange Sum(this IEnumerable<DoubleRange> ranges)
        {
            var sum = DoubleRange.Empty;
            IEnumerator<DoubleRange> enumerator = ranges.GetEnumerator();
            while (enumerator.MoveNext())
            {
                sum += enumerator.Current;
            }
            return sum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static ChartSeriesBase Max(this IEnumerable<ChartSeriesBase> source, Func<ChartSeriesBase, double> selector)
        {
            var chartSeries = source as ChartSeriesBase[] ?? source.ToArray();
            if (chartSeries.Any())
            {
                ChartSeriesBase maxObject = chartSeries[0];
                double maxVal = selector(maxObject);
                for (int i = 0; i < chartSeries.Count(); i++)
                {
                    double value = selector(chartSeries[i]);
                    if (value > maxVal)
                    {
                        maxVal = value;
                        maxObject = chartSeries[i];
                    }
                }

                return maxObject;
            }
            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        internal static int GetAdornmentIndex(object source)
        {
            var element = source as FrameworkElement;
            int index = -1;

            var chartAdornmentContainer = element.DataContext as ChartAdornmentContainer;
            if (chartAdornmentContainer != null)
                index = (int)chartAdornmentContainer.Tag;
            else
            {
                while (!(element is ChartAdornmentContainer) && element != null)
                {
                    element = VisualTreeHelper.GetParent(element) as FrameworkElement;
                    //Get the Adornment index when set the Content control for LabelTemplate. 
                    if (element is ContentControl && (element as ContentControl).Tag is int)
                    {
                        index = (int)(element as ContentControl).Tag;
                        return index;
                    }
                    //Get the Adornment index when set the SymbolTemplate for Adornments. 
                    else if (element is ChartAdornmentContainer)
                        index = (int)(element as ChartAdornmentContainer).Tag;
                }
            }
            return index;
        }
        
        //StrokeDashArray applied only for the first line element when it is applied through style. 
        //It is bug in the framework.
        //And hence manually setting stroke dash array for each and every line.
        public static void SetStrokeDashArray(UIElementsRecycler<Line> lineRecycler)
        {
            if (lineRecycler.Count > 0)
            {
                DoubleCollection collection = lineRecycler[0].StrokeDashArray;
                if (collection != null && collection.Count > 0)
                {
                    foreach (Line line in lineRecycler)
                    {
                        DoubleCollection doubleCollection = new DoubleCollection();
                        foreach (double value in collection)
                        {
                            doubleCollection.Add(value);
                        }
                        line.StrokeDashArray = doubleCollection;
                    }
                }
            }            
        }

        /// <summary>
        /// Get the bool value for current series is draggable or not
        /// </summary>
        /// <param name="chartSeries">Current Series</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        internal static bool IsDraggable(ChartSeriesBase chartSeries)
        {
            if ((chartSeries is RangeSegmentDraggingBase && (chartSeries as RangeSegmentDraggingBase).EnableSegmentDragging)
                || (chartSeries is XySegmentDraggingBase && (chartSeries as XySegmentDraggingBase).EnableSegmentDragging)
                || (chartSeries is XySeriesDraggingBase && (chartSeries as XySeriesDraggingBase).EnableSeriesDragging))
            {
                return true;
            }
            else
                return false;
        }

        internal static int BinarySearch(List<double> xValues, double touchValue, int min, int max)
        {
            var closerIndex = 0;
            var closerDelta = double.MaxValue;

            while (min <= max)
            {
                int mid = (min + max) / 2;
                var xValue = xValues[mid];
                var delta = Math.Abs(touchValue - xValue);

                if (delta < closerDelta)
                {
                    closerDelta = delta;
                    closerIndex = mid;
                }

                if (touchValue == xValue)
                {
                    return mid;
                }
                else if (touchValue < xValues[mid])
                {
                    max = mid - 1;
                }
                else
                {
                    min = mid + 1;
                }
            }

            return closerIndex;
        }

        internal static Brush GetInterior(ChartSeriesBase series, int segmentIndex)
        {
            ChartSeriesBase serObj = series;

            if (serObj != null)
            {
                if (serObj.Interior != null)
                    return serObj.Interior;
                else if (serObj.SegmentColorPath != null && serObj.ColorValues.Count > 0)
                {
                    if (segmentIndex != -1 && segmentIndex < serObj.ActualData.Count)
                    {
                        if (!(serObj.ColorValues[segmentIndex] == null))
                            return serObj.ColorValues[segmentIndex];
                        else if (serObj.Palette != ChartColorPalette.None && (serObj.ColorValues[segmentIndex] == null))
                        {
                            serObj.ColorValues[segmentIndex] = serObj.ColorModel.GetBrush(segmentIndex);
                            return serObj.ColorModel.GetBrush(segmentIndex);
                        }
                        else
                        {
                            int serIndex = serObj.ActualArea.GetSeriesIndex(serObj);
                            serObj.ColorValues[segmentIndex] = serObj.ActualArea.ColorModel.GetBrush(serIndex);
                            return serObj.ActualArea.ColorModel.GetBrush(serIndex);
                        }
                    }
                }
                else if (serObj.Palette != ChartColorPalette.None)
                {
                    if (segmentIndex != -1 && serObj.ColorModel != null)
                        return serObj.ColorModel.GetBrush(segmentIndex);
                }
                else if (serObj.ActualArea != null
                    && serObj.ActualArea.Palette != ChartColorPalette.None && serObj.ActualArea.ColorModel != null)
                {
                    int serIndex = serObj.ActualArea.GetSeriesIndex(serObj);
                    SfChart chart = serObj.ActualArea as SfChart;
                    if (serIndex >= 0)
                        return serObj.ActualArea.ColorModel.GetBrush(serIndex);
                    else if (chart != null && chart.TechnicalIndicators != null && chart.TechnicalIndicators.Count > 0)
                    {
                        serIndex = chart.TechnicalIndicators.IndexOf(serObj as ChartSeries);
                        return serObj.ActualArea.ColorModel.GetBrush(serIndex);
                    }
                }
            }

            return new SolidColorBrush(Windows.UI.Colors.Transparent);
        }

        /// <summary>
        /// Gets the multiple area rectangle of the provided mouse point.
        /// Also returns a <see cref="bool"/> value indicating whether the point is inside rect. 
        /// This bool is used since the <see cref="Rect"/> is value type and the null conditions for the outcoming rect cannot be checked.
        /// </summary>
        /// <param name="mousePoint">The mouse point.</param>
        /// <param name="axis">The axis to be checked.</param>
        /// <param name="isPointInsideRect">The property indicates whether the point is inside the axis area rectangle.</param>
        /// <returns>Returns the point captured <see cref="Rect"/>.</returns>
        internal static Rect GetAxisArrangeRect(Point mousePoint, ChartAxis axis, out bool isPointInsideRect)
        {
            Rect clipRect = new Rect();
            double left = axis.ArrangeRect.Left;
            double top = axis.ArrangeRect.Top;

            foreach (var supportAxis in axis.AssociatedAxes)
            {
                if (axis.Orientation == Orientation.Horizontal)
                {
                    top = supportAxis.ArrangeRect.Top;
                    clipRect = new Rect(left, top, axis.ArrangeRect.Width, supportAxis.ArrangeRect.Height);
                }
                else
                {
                    left = supportAxis.ArrangeRect.Left;
                    clipRect = new Rect(left, top, supportAxis.ArrangeRect.Width, axis.ArrangeRect.Height);
                }

                if (clipRect.Contains(mousePoint))
                {
                    isPointInsideRect = true;
                    return clipRect;
                }
            }

            isPointInsideRect = false;
            return clipRect;
        }
    }


    internal static class ClearUIElementProperties
    {
        internal static void ClearUIValues(this Shape element)
        {
            if (element is Line)
            {
                element.ClearValue(Line.X1Property);
                element.ClearValue(Line.X2Property);
                element.ClearValue(Line.Y1Property);
                element.ClearValue(Line.Y2Property);
            }
            else if (element is Rectangle)
            {
                element.ClearValue(Rectangle.WidthProperty);
                element.ClearValue(Rectangle.HeightProperty);
            }
            else if (element is Ellipse)
            {
                element.ClearValue(Ellipse.WidthProperty);
                element.ClearValue(Ellipse.HeightProperty);
            }
        }
    }

   public struct ChartPoint : IEquatable<ChartPoint>
   {
        public double X
        {
            get;
            set;
        }

        public double Y
        {
            get;
            set;
        }

       public ChartPoint(double x, double y) : this()
       {
           X = x;
           Y = y;
       }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
        public override bool Equals(object obj)
        {
            if (!(obj is ChartPoint))
            {
                return false;
            }

            return Equals((ChartPoint)obj);
        }

        public bool Equals(ChartPoint point)
        {
            if (X != point.X)
            {
                return false;
            }

            return Y == point.Y;
        }

        public static bool operator ==(ChartPoint point1, ChartPoint point2)
        {
            return point1.Equals(point2);
        }

        public static bool operator !=(ChartPoint point1, ChartPoint point2)
        {
            return !point1.Equals(point2);
        }       
    }
}
