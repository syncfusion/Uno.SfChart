using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Contains static methods for performing certain mathematical calculations.
    /// </summary>

    public static class ChartMath
    {
        #region Constants

        internal const float MARGINS_RATIO = 0.03f;

        /// <summary>
        /// Initializes ToDegree
        /// </summary>
        public const double ToDegree = 180 / Math.PI;

        /// <summary>
        /// Initializes ToRadial
        /// </summary>
        public const double ToRadial = Math.PI / 180;

        /// <summary>
        /// Initializes Percent
        /// </summary>
        public const double Percent = 0.01d;

        /// <summary>
        /// Initializes DoublePI
        /// </summary>
        public const double DoublePI = 2 * Math.PI;

        /// <summary>
        /// Initializes HalfPI
        /// </summary>
        public const double HalfPI = 0.5 * Math.PI;

        /// <summary>
        /// Initializes OneAndHalfPI
        /// </summary>
        public const double OneAndHalfPI = 1.5 * Math.PI;

        /// <summary>
        /// The epsilon
        /// </summary>
        public const double Epsilon = 0.00001;
        #endregion

        #region Public methods

        /// <summary>
        /// Method used to gets or sets intersect of two rectangle.
        /// </summary>
        /// <param name="rectCollection"></param>
        /// <param name="newRect"></param>
        /// <returns></returns>
        public static bool IntersectWith(this IList<Rect> rectCollection, Rect newRect)
        {
            foreach (var existingRect in rectCollection.Reverse())
            {
                if (existingRect.IntersectsWith(newRect))
                {
                    return true;
                }
            }
            return false;
        }

        ///<summary>
        /// Method used to get interpolarated point
        ///</summary>
        ///<param name="x1"></param>
        ///<param name="x2"></param>
        ///<param name="y1"></param>
        ///<param name="y2"></param>
        ///<param name="x"></param>
        ///<returns></returns>

        internal static double GetInterpolarationPoint(double x1, double x2, double y1, double y2, double x)
        {
            double y = (((y2 - y1) / (x2 - x1)) * (x - x1)) + y1;
            return y;
        }

        /// <summary>
        /// Method used to get the normal.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static Vector3D GetNormal(Vector3D v1, Vector3D v2, Vector3D v3)
        {
            Vector3D n = (v1 - v2) * (v3 - v2);
            double l = n.GetLength();

            if (l < Epsilon)
            {
                l = 0;
            }

            return new Vector3D(n.X / l, n.Y / l, n.Z / l);
        }

        /// <summary>
        /// Solves quadratic equation in form a*x^2 + b*x + c = 0
        /// </summary>
        /// <param name="a">The A component</param>
        /// <param name="b">The B component</param>
        /// <param name="c">The C component</param>
        /// <param name="root1">First root.</param>
        /// <param name="root2">Second root.</param>
        /// <returns>Bool value</returns>
        public static bool SolveQuadraticEquation(double a, double b, double c, out double root1, out double root2)
        {
            root1 = 0;
            root2 = 0;

            if (a != 0)
            {
                double d = b * b - 4 * a * c;

                if (d >= 0)
                {
                    double sd = Math.Sqrt(d);

                    root1 = (-b - sd) / (2 * a);
                    root2 = (-b + sd) / (2 * a);

                    return true;
                }
            }
            else if (b != 0)
            {
                root1 = -c / b;
                root2 = -c / b;

                return true;
            }

            return false;
        }

       

        /// <summary>
        /// Gets minimal value from <c>value</c> or <c>min</c> and maximal from <c>value</c> or <c>max</c>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimal value.</param>
        /// <param name="max">The maximal value.</param>
        /// <returns>The MinMax value</returns>
        public static double MinMax(double value, double min, double max)
        {
            return value > max ? max : (value < min ? min : value);
        }

        /// <summary>
        /// Gets minimal value from parameters.
        /// </summary>
        /// <param name="values">The parameters</param>
        /// <returns>The minimal value.</returns>
        public static double Min(params double[] values)
        {
            double result = values[0];

            for (int i = 1; i < values.Length; i++)
            {
                result = Math.Min(result, values[i]);
            }

            return result;
        }

        /// <summary>
        /// Gets maximal value from parameters.
        /// </summary>
        /// <param name="values">The parameters</param>
        /// <returns>The maximal value.</returns>
        public static double Max(params double[] values)
        {
            double result = values[0];

            for (int i = 1; i < values.Length; i++)
            {
                result = Math.Max(result, values[i]);
            }

            return result;
        }

        /// <summary>
        /// Gets maximal value from parameter or zero.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The double value</returns>
        public static double MaxZero(double value)
        {
            return value > 0d ? value : 0d;
        }

        /// <summary>
        /// Gets minimal value from parameter or zero.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The MinZero value</returns>
        public static double MinZero(double value)
        {
            return value < 0d ? value : 0d;
        }

        /// <summary>
        /// Rounds the specified value.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="div">The divider.</param>
        /// <param name="up">if set to <c>true</c> value will be rounded up.</param>
        /// <returns>The Round off value</returns>
        public static double Round(double x, double div, bool up)
        {
            return (int)(up ? Math.Ceiling(x / div) : Math.Floor(x / div)) * div;
        }

        /// <summary>
        /// Gets the cross point.
        /// </summary>
        /// <param name="p11">The P11 value.</param>
        /// <param name="p12">The P12 value.</param>
        /// <param name="p21">The P21 value.</param>
        /// <param name="p22">The P22 value.</param>
        /// <returns>The CrossPoint</returns>
        internal static ChartPoint? GetCrossPoint(ChartPoint p11, ChartPoint p12, ChartPoint p21, ChartPoint p22)
        {
            ChartPoint pt = new ChartPoint();
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

        /// <summary>
        /// Returns the angle between the points.
        /// </summary>
        /// <param name="startPoint">The start point</param>
        /// <param name="endPoint">The end point</param>
        /// <returns>The Angle</returns>
        internal static double GetAngle(Point startPoint, Point endPoint)
        {
            double radians = Math.Atan2(-(endPoint.Y - startPoint.Y), endPoint.X - startPoint.X);
            radians = radians < 0 ? Math.Abs(radians) : 2 * Math.PI - radians;
            return radians * (180 / Math.PI);
        }

        /// <summary>
        /// Reduces the points using Douglas-Peucker line approximation algorithm.
        /// </summary>
        /// <param name="totalPoints"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reviewed")]
        internal static PointCollection ReducePointsUsingDPAlg(PointCollection totalPoints, double tolerance)
        {
            PointCollection reducedPoints = new PointCollection();

            if (totalPoints == null || totalPoints.Count < 3)
                return totalPoints;

            SortedDictionary<int, Point> sortedPoints = new SortedDictionary<int, Point>();

            //List<int> indexes = new List<int>();

            ReducePointsRecursively(totalPoints, 0, totalPoints.Count - 1, tolerance, sortedPoints);

            reducedPoints.Concat<Point>(sortedPoints.Values);

            sortedPoints.Clear();

            return reducedPoints;
        }

        /// <summary>
        /// Eliminates the points by calculating the perpendicular distance of a point from a line.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="firstPoint"></param>
        /// <param name="lastPoint"></param>
        /// <param name="tolerance"></param>
        /// <param name="sortedPoints"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reviewed")]
        private static void ReducePointsRecursively(PointCollection points, int firstPoint, int lastPoint, double tolerance
            , SortedDictionary<int, Point> sortedPoints)
        {

            double maxDistance = 0;
            int maxDistanceIndex = 0;

            for (int index = firstPoint; index < lastPoint; index++)
            {
                double distance = CalcPerpendicularDistance
                    (points[firstPoint], points[lastPoint], points[index]);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxDistanceIndex = index;
                }
            }

            if (maxDistance > tolerance && maxDistanceIndex != 0)
            {
                sortedPoints.Add(maxDistanceIndex, points[maxDistanceIndex]);

                //sortedPoints.Add(maxDistanceIndex);

                ReducePointsRecursively(points, firstPoint,
                maxDistanceIndex, tolerance, sortedPoints);

                ReducePointsRecursively(points, maxDistanceIndex,
                lastPoint, tolerance, sortedPoints);
            }

        }

        /// <summary>
        /// Calculates the perpendicular distance of point from a line.
        /// </summary>
        /// <param name="Point1">Starting point of the line.</param>
        /// <param name="Point2">Ending point of the line</param>
        /// <param name="Point">The point</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static double CalcPerpendicularDistance(Point Point1, Point Point2, Point Point)
        {
            //Calculating the area of the tringle
            double area = Math.Abs(.5 * (Point1.X * Point2.Y + Point2.X *
            Point.Y + Point.X * Point1.Y - Point2.X * Point1.Y - Point.X *
            Point2.Y - Point1.X * Point.Y));

            //Calculating the base of the triangle
            double bottom = Math.Sqrt(Math.Pow(Point1.X - Point2.X, 2) +
            Math.Pow(Point1.Y - Point2.Y, 2));

            //Calculating the height of the triangle i.e., the perpendicular distance...
            double height = area / bottom * 2;

            return height;
        }

        /// <summary>
        /// return point values from the given origin,end and angle points
        /// </summary>
        /// <param name="originpoint"></param>
        /// <param name="endpoint"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Point GeneralPointRotation(Point originpoint, Point endpoint, double angle)
        {
            double ang = angle * Math.PI / 180;
            double radius = endpoint.X / 2;
            endpoint.X = ((radius) * Math.Cos(ang));
            endpoint.Y = ((radius) * Math.Sin(ang));
            return endpoint;
        }

        /// <summary>
        /// Method used to check a point inside a rectangle.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="rectanglePoints"></param>
        /// <returns></returns>
        internal static bool IsPointInsideRectangle(Point point, Point[] rectanglePoints)
        {
            // Hit point check for the normal line annotation and rotated rect check for the horizontal and vertical line annotation.
            var rectangleArea = CalculateRectangleArea(rectanglePoints[0], rectanglePoints[1], rectanglePoints[2], rectanglePoints[3]);
            var triangleArea1 = CalculateTriangleArea(point, rectanglePoints[0], rectanglePoints[1]);
            var triangleArea2 = CalculateTriangleArea(point, rectanglePoints[1], rectanglePoints[2]);
            var triangleArea3 = CalculateTriangleArea(point, rectanglePoints[2], rectanglePoints[3]);
            var triangleArea4 = CalculateTriangleArea(point, rectanglePoints[3], rectanglePoints[0]);

            return (Math.Round(rectangleArea) >= Math.Round(triangleArea1 + triangleArea2 + triangleArea3 + triangleArea4));
        }

        /// <summary>
        /// Calculates the area of the given triangle.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        internal static double CalculateTriangleArea(Point p1, Point p2, Point p3)
        {
            return 0.5 * Math.Abs(p1.X * (p2.Y - p3.Y) + p2.X * (p3.Y - p1.Y) + p3.X * (p1.Y - p2.Y));
        }

        /// <summary>
        /// Calculates the area of the given rectangle.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <returns></returns>
        internal static double CalculateRectangleArea(Point p1, Point p2, Point p3, Point p4)
        {
            return 0.5 * Math.Abs((p1.Y - p3.Y) * (p4.X - p2.X) + (p2.Y - p4.Y) * (p1.X - p3.X));
        }

        /// <summary>
        /// Calculates the distance between two points.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        internal static double CalculateDistanceBetweenTwoPoints(Point p1, Point p2)
        {
            return Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));           
        }

        /// <summary>
        /// Calculates the perpendicular distant point with the given points and distance.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        internal static Point CalculatePerpendicularDistantPoint(Point p1, Point p2, double d)
        {
            var xdifference = p1.X - p2.X;
            var ydifference = p1.Y - p2.Y;

            var distance = CalculateDistanceBetweenTwoPoints(p1, p2);

            xdifference /= distance;
            ydifference /= distance;

            var p3 = new Point(p1.X + d * ydifference, p1.Y - d * xdifference);
            return p3;
        }

        /// <summary>
        /// Checks whether the given point is inside the circle.
        /// </summary>
        /// <param name="circleCenter">The center point of the circle.</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="testPoint">The test point to be checked inside a circle.</param>
        internal static bool IsPointInsideCircle(Point circleCenter, double radius, Point testPoint)
        {
            var xDifference = testPoint.X - circleCenter.X;
            var yDifference = testPoint.Y - circleCenter.Y;
            double centerTestPointDistance = Math.Sqrt((xDifference * xDifference) + (yDifference * yDifference));

            return centerTestPointDistance < radius;
        }

        #endregion
    }

    internal static class ChartLayoutUtils
    {
        #region Constants
        /// <summary>
        /// Initializes c_half
        /// </summary>
        private const double C_half = 0.5d;
        #endregion

        #region Public methods

        /// <summary>
        /// Gets the rect by center.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="size">The size value.</param>
        /// <returns>The Rect value</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reviewed")]
        public static Rect GetRectByCenter(Point center, Size size)
        {
            return new Rect(center.X - size.Width / 2, center.Y - size.Height / 2, size.Width, size.Height);
        }

        /// <summary>
        /// Gets the rect by center.
        /// </summary>
        /// <param name="cx">The cx value.</param>
        /// <param name="cy">The cy value.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The Rect value</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static Rect GetRectByCenter(double cx, double cy, double width, double height)
        {
            return new Rect(cx - width / 2, cy - height / 2, width, height);
        }

        /// <summary>
        /// Gets the center.
        /// </summary>
        /// <param name="size">The size value.</param>
        /// <returns>The vector center value</returns>
        public static Point GetCenter(Size size)
        {
            return new Point(C_half * size.Width, C_half * size.Height);
        }

        /// <summary>
        /// Gets the center.
        /// </summary>
        /// <param name="rect">The rect value.</param>
        /// <returns>The center point value</returns>
        public static Point GetCenter(Rect rect)
        {
            Point centerPoint = GetCenter(new Size(rect.Width, rect.Height));
            return new Point(centerPoint.X + rect.Left, centerPoint.Y + rect.Top);
        }

        /// <summary>
        /// Subtracts the thickness.
        /// </summary>
        /// <param name="rect">The rect value.</param>
        /// <param name="thickness">The thickness.</param>
        /// <returns>The Rectangle</returns>
        public static Rect Subtractthickness(Rect rect, Thickness thickness)
        {
            rect.X += thickness.Left;
            rect.Y += thickness.Top;
            if (rect.Width > thickness.Left + thickness.Right)
            {
                rect.Width -= thickness.Left + thickness.Right;
            }
            else
                rect.Width = 0;

            if (rect.Height > (thickness.Top + thickness.Bottom))
            {
                rect.Height -= thickness.Top + thickness.Bottom;
            }
            else
                rect.Height = 0;

            return rect;
        }

        /// <summary>
        /// Subtracts the thickness.
        /// </summary>
        /// <param name="size">The size value.</param>
        /// <param name="thickness">The thickness.</param>
        /// <returns>Returns the size</returns>
        public static Size Subtractthickness(Size size, Thickness thickness)
        {
            size.Width = Math.Max(size.Width - thickness.Left - thickness.Right, 0);
            size.Height = Math.Max(size.Height - thickness.Top - thickness.Bottom, 0);

            return size;
        }

        /// <summary>
        /// The Addthickness method
        /// </summary>
        /// <param name="rect">The Rect value</param>
        /// <param name="thickness">The thickness</param>
        /// <returns>The rectangle</returns>
        /// <seealso cref="ChartLayoutUtils"/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reviewed")]
        public static Rect Addthickness(Rect rect, Thickness thickness)
        {
            rect.X -= thickness.Left;
            rect.Y -= thickness.Top;
            rect.Width += thickness.Left + thickness.Right;
            rect.Height += thickness.Top + thickness.Bottom;

            return rect;
        }

        /// <summary>
        /// The Addthickness method
        /// </summary>
        /// <param name="size">The size value</param>
        /// <param name="thickness">The thickness value</param>
        /// <returns>Returns the size</returns>
        ///  <seealso cref="ChartLayoutUtils"/>
        public static Size Addthickness(Size size, Thickness thickness)
        {
            if (thickness.Left >= 0 && thickness.Right >= 0)
                size.Width += thickness.Left + thickness.Right;
            if (thickness.Top >= 0 && thickness.Bottom >= 0)
                size.Height += thickness.Top + thickness.Bottom;
            return size;
        }

        /// <summary>
        /// Checks the members of size by infinity.
        /// </summary>
        /// <param name="size">The size value.</param>
        /// <returns>Returns the size</returns>
        public static Size CheckSize(Size size)
        {
            size.Width = double.IsInfinity(size.Width) ? 0d : size.Width;
            size.Height = double.IsInfinity(size.Height) ? 0d : size.Height;

            return size;
        }


internal static T GetVisualChild<T>(DependencyObject parent) where T : FrameworkElement
{
    T child = default(T);

    int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
    for (int i = 0; i < numVisuals; i++)
    {
        DependencyObject v = (DependencyObject)VisualTreeHelper.GetChild(parent, i);
        child = v as T;
        if (child == null)
        {
            child = GetVisualChild<T>(v);
        }
        if (child != null)
        {
            break;
        }
    }
    return child;
}
        #endregion
    }
}
