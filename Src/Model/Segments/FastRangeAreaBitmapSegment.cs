// <copyright file="FastRangeAreaBitmapSegment.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart range area segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="FastRangeAreaBitmapSeries"/>
    public partial class FastRangeAreaSegment : ChartSegment
    {
        #region Fields

        #region Private Fields

        private bool isSeriesSelected;

        private Color seriesSelectionColor = Colors.Transparent;

        private WriteableBitmap bitmap;

        private byte[] fastBuffer;

        private double highValue;

        private double lowValue;

        private List<int> areaPoints;

        private FastRangeAreaBitmapSeries fastRangeAreaBitmapSeries;

        private bool isHighLow;

        private int[] points1 = new int[10];

        private int[] points2 = new int[10];

        private Point intersectingPoint;

        #endregion

        #endregion

        #region Contructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FastRangeAreaSegment"/> class.
        /// </summary>
        /// <param name="areaValues">The range area plot values.</param>
        /// <param name="isHighLow">Indicates a whether the high value is greater than low value.</param>
        /// <param name="series">The series of the segment.</param>
        public FastRangeAreaSegment(List<ChartPoint> areaValues, bool isHighLow, FastRangeAreaBitmapSeries series)
        {
            this.isHighLow = isHighLow;
            this.fastRangeAreaBitmapSeries = series;
            this.areaPoints = new List<int>();
            this.AreaValues = areaValues;
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the high(top) value bind with this segment.
        /// </summary>
        public double High
        {
            get
            {
                return highValue;
            }

            set
            {
                highValue = value;
                OnPropertyChanged("High");
            }
        }

        /// <summary>
        /// Gets or sets the low(bottom) value bind with this segment.
        /// </summary>
        public double Low
        {
            get
            {
                return lowValue;
            }

            set
            {
                lowValue = value;
                OnPropertyChanged("Low");
            }
        }

        #endregion

        #region Internal Properties

        internal List<ChartPoint> AreaValues { get; set; }

        internal EmptyStroke EmptyStroke { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>returns UIElement</returns>
        public override UIElement GetRenderedVisual()
        {
            return null;
        }

        /// <summary>
        /// Used for creating UIElement for rendering this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="size">Size of the panel</param>
        /// <returns>returns the created <see cref="UIElement"/> </returns>
        public override UIElement CreateVisual(Size size)
        {
            bitmap = fastRangeAreaBitmapSeries.Area.GetFastRenderSurface();
            fastBuffer = fastRangeAreaBitmapSeries.Area.GetFastBuffer();
            return null;
        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="areaValues">The range area plot values.</param>
        public override void SetData(List<ChartPoint> areaValues)
        {
            this.AreaValues = areaValues;
            double xMax = this.AreaValues.Max(x => x.X);
            double yMax = this.AreaValues.Max(y => y.Y);
            double xMin = this.AreaValues.Min(x => x.X);
            double min = this.AreaValues.Min(item => item.Y);
            double yMin;
            if (double.IsNaN(min))
            {
                var yVal = this.AreaValues.Where(item => !double.IsNaN(item.Y));
                yMin = (!yVal.Any()) ? 0 : yVal.Min(item => item.Y);
            }
            else
            {
                yMin = min;
            }

            XRange = new DoubleRange(xMin, xMax);
            YRange = new DoubleRange(yMin, yMax);
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Represents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        public override void Update(IChartTransformer transformer)
        {
            bitmap = fastRangeAreaBitmapSeries.Area.GetFastRenderSurface();
            if (transformer != null && fastRangeAreaBitmapSeries.DataCount > 0)
            {
                ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;

                // Removed the screen point calculation methods and added the point to value method.
                CalculatePoints(cartesianTransformer);
                UpdateVisual();
            }
        }

        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="size">The Size</param>
        public override void OnSizeChanged(Size size)
        {
            bitmap = fastRangeAreaBitmapSeries.Area.GetFastRenderSurface();
            fastBuffer = fastRangeAreaBitmapSeries.Area.GetFastBuffer();
        }

        #endregion

        #region Internal Methods

        internal void UpdateVisual()
        {
            SfChart chart = fastRangeAreaBitmapSeries.Area;
            isSeriesSelected = false;

            Color checkColor = GetSegmentColor();

            // Set SeriesSelectionBrush and Check EnableSeriesSelection        
            if (chart.GetEnableSeriesSelection())
            {
                Brush seriesSelectionBrush = chart.GetSeriesSelectionBrush(fastRangeAreaBitmapSeries);
                if (seriesSelectionBrush != null && chart.SelectedSeriesCollection.Contains(fastRangeAreaBitmapSeries))
                {
                    isSeriesSelected = true;
                    seriesSelectionColor = ((SolidColorBrush)seriesSelectionBrush).Color;
                }
            }

            Color color = isSeriesSelected ? seriesSelectionColor : checkColor;

            if (bitmap != null)
            {
                fastBuffer = fastRangeAreaBitmapSeries.Area.GetFastBuffer();
                int width = (int)fastRangeAreaBitmapSeries.Area.SeriesClipRect.Width;
                int height = (int)fastRangeAreaBitmapSeries.Area.SeriesClipRect.Height;

                int leftThickness = (int)fastRangeAreaBitmapSeries.StrokeThickness / 2;
                int rightThickness = (int)(fastRangeAreaBitmapSeries.StrokeThickness % 2 == 0
                    ? (fastRangeAreaBitmapSeries.StrokeThickness / 2) : fastRangeAreaBitmapSeries.StrokeThickness / 2 + 1);

                if (fastRangeAreaBitmapSeries is FastRangeAreaBitmapSeries)
                {
                    isSeriesSelected = false;

                    // Set SeriesSelectionBrush and Check EnableSeriesSelection        
                    if (chart.GetEnableSeriesSelection())
                    {
                        Brush seriesSelectionBrush = chart.GetSeriesSelectionBrush(fastRangeAreaBitmapSeries);
                        if (seriesSelectionBrush != null && chart.SelectedSeriesCollection.Contains(fastRangeAreaBitmapSeries))
                        {
                            isSeriesSelected = true;
                            seriesSelectionColor = ((SolidColorBrush)seriesSelectionBrush).Color;
                        }
                    }

                    UpdateVisual(width, height, color, leftThickness, rightThickness);
                }
            }

            fastRangeAreaBitmapSeries.Area.CanRenderToBuffer = true;
        }
        #endregion

        #region Private Static Methods

        private static void GetLinePoints(double x1, double y1, double x2, double y2, double leftThickness, double rightThickness, int[] points)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;
            var radian = Math.Atan2(dy, dx);
            var cos = Math.Cos(-radian);
            var sin = Math.Sin(-radian);
            var x11 = (x1 * cos) - (y1 * sin);
            var y11 = (x1 * sin) + (y1 * cos);
            var x12 = (x2 * cos) - (y2 * sin);
            var y12 = (x2 * sin) + (y2 * cos);
            cos = Math.Cos(radian);
            sin = Math.Sin(radian);
            var leftTopX = (x11 * cos) - ((y11 + leftThickness) * sin);
            var leftTopY = (x11 * sin) + ((y11 + leftThickness) * cos);
            var rightTopX = (x12 * cos) - ((y12 + leftThickness) * sin);
            var rightTopY = (x12 * sin) + ((y12 + leftThickness) * cos);
            var leftBottomX = (x11 * cos) - ((y11 - rightThickness) * sin);
            var leftBottomY = (x11 * sin) + ((y11 - rightThickness) * cos);
            var rightBottomX = (x12 * cos) - ((y12 - rightThickness) * sin);
            var rightBottomY = (x12 * sin) + ((y12 - rightThickness) * cos);
            points[0] = (int)leftTopX;
            points[1] = (int)leftTopY;
            points[2] = (int)rightTopX;
            points[3] = (int)rightTopY;
            points[4] = (int)rightBottomX;
            points[5] = (int)rightBottomY;
            points[6] = (int)leftBottomX;
            points[7] = (int)leftBottomY;
            points[8] = (int)leftTopX;
            points[9] = (int)leftTopY;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the segment color in accordance with priorities.
        /// </summary>
        /// <returns>Returns the prioritized segment color.</returns>
        private Color GetSegmentColor()
        {
            if (fastRangeAreaBitmapSeries.Interior != null)
            {
                return GetColor(fastRangeAreaBitmapSeries.Interior);
            }
            else if (isHighLow && fastRangeAreaBitmapSeries.HighValueInterior != null)
            {
                return GetColor(fastRangeAreaBitmapSeries.HighValueInterior);
            }
            else if (!isHighLow && fastRangeAreaBitmapSeries.LowValueInterior != null)
            {
                return GetColor(fastRangeAreaBitmapSeries.LowValueInterior);
            }
            else if (fastRangeAreaBitmapSeries.Palette != ChartColorPalette.None)
            {
                return GetColor(fastRangeAreaBitmapSeries.GetInteriorColor(0));
            }
            else if (this.Interior != null)
            {
                return GetColor(this.Interior);
            }
            else
            {
                return new SolidColorBrush(Colors.Transparent).Color;
            }
        }

        /// <summary>
        /// Calculates the transform points.
        /// </summary>
        /// <param name="cartesianTransformer">The transformer to get the required points from chart values.</param>
        private void CalculatePoints(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            int startIndex = 0;
            int endIndex = 0;
            ChartAxis xAxis = this.Series.ActualXAxis;
            var logarithmicAxis = xAxis as LogarithmicAxis;
            double rangeStart = logarithmicAxis != null ? Math.Pow(logarithmicAxis.LogarithmicBase, xAxis.VisibleRange.Start) : xAxis.VisibleRange.Start;
            double rangeEnd = logarithmicAxis != null ? Math.Pow(logarithmicAxis.LogarithmicBase, xAxis.VisibleRange.End) : xAxis.VisibleRange.End;

            if (this.Series.IsIndexed)
            {
                var categoryXAxis = xAxis as CategoryAxis;
                var isGrouping = categoryXAxis != null && categoryXAxis.IsIndexed;
                int start = 0, end = 0;
                var count = AreaValues.Count / 2;

                if (!isGrouping)
                {
                    start = 0;
                    end = count - 1;
                }
                else
                {
                    start = (int)Math.Floor(xAxis.VisibleRange.Start);
                    end = (int)Math.Ceiling(xAxis.VisibleRange.End);
                    end = end > count - 1 ? count - 1 : end;
                    start = start < 0 ? 0 : start;
                }

                startIndex = start * 2;

                for (int i = startIndex; i < AreaValues.Count; i += 2)
                {
                    var convertedPoint = cartesianTransformer.TransformToVisible(AreaValues[i].X, AreaValues[i].Y);
                    areaPoints.Add((int)convertedPoint.X);
                    areaPoints.Add((int)convertedPoint.Y);
                }

                endIndex = end * 2 + 1;
                for (int i = AreaValues.Count - 1; i >= 1; i -= 2)
                {
                    var convertedPoint = cartesianTransformer.TransformToVisible(AreaValues[i].X, AreaValues[i].Y);
                    areaPoints.Add((int)convertedPoint.X);
                    areaPoints.Add((int)convertedPoint.Y);
                }

                // Join with the first point.
                if (areaPoints.Count > 1)
                {
                    areaPoints.Add(areaPoints[0]);
                    areaPoints.Add(areaPoints[1]);
                }
            }
            else
            {
                areaPoints.Clear();

                // The point addition goes in the logic bottom left, bottom right, top right and top left.
                double xValue = 0d;
                Point convertedPoint = new Point();
                bool isFirstIndexSet = false;

                // Setting lower points from left to right.
                for (int i = 0; i < AreaValues.Count; i += 2)
                {
                    xValue = AreaValues[i].X;
                    if (rangeStart <= xValue && xValue <= rangeEnd)
                    {
                        convertedPoint = cartesianTransformer.TransformToVisible(AreaValues[i].X, AreaValues[i].Y);
                        areaPoints.Add((int)convertedPoint.X);
                        areaPoints.Add((int)convertedPoint.Y);
                    }
                    else if (xValue < rangeStart)
                    {
                        isFirstIndexSet = true;
                        startIndex = i;
                    }
                    else if (xValue > rangeEnd)
                    {
                        convertedPoint = cartesianTransformer.TransformToVisible(AreaValues[i].X, AreaValues[i].Y);
                        areaPoints.Add((int)convertedPoint.X);
                        areaPoints.Add((int)convertedPoint.Y);
                        break;
                    }
                }

                if (isFirstIndexSet)
                {
                    convertedPoint = cartesianTransformer.TransformToVisible(AreaValues[startIndex].X, AreaValues[startIndex].Y);
                    areaPoints.Insert(0, (int)convertedPoint.Y);
                    areaPoints.Insert(0, (int)convertedPoint.X);
                }

                // Setting upper points from right to left of the polygon.

                // The top right corner point index predefined.
                int midIndex = areaPoints.Count;
                endIndex = AreaValues.Count - 1;
                isFirstIndexSet = false;

                for (int i = AreaValues.Count - 1; i >= 1; i -= 2)
                {
                    xValue = AreaValues[i].X;
                    if (rangeStart <= xValue && xValue <= rangeEnd)
                    {
                        convertedPoint = cartesianTransformer.TransformToVisible(AreaValues[i].X, AreaValues[i].Y);
                        areaPoints.Add((int)convertedPoint.X);
                        areaPoints.Add((int)convertedPoint.Y);
                    }
                    else if (xValue > rangeEnd)
                    {
                        isFirstIndexSet = true;
                        endIndex = i;
                    }
                    else if (xValue < rangeStart)
                    {
                        convertedPoint = cartesianTransformer.TransformToVisible(AreaValues[i].X, AreaValues[i].Y);
                        areaPoints.Add((int)convertedPoint.X);
                        areaPoints.Add((int)convertedPoint.Y);
                        break;
                    }
                }

                if (midIndex < areaPoints.Count && isFirstIndexSet)
                {
                    convertedPoint = cartesianTransformer.TransformToVisible(AreaValues[endIndex].X, AreaValues[endIndex].Y);
                    areaPoints.Insert(midIndex, (int)convertedPoint.Y);
                    areaPoints.Insert(midIndex, (int)convertedPoint.X);
                }

                // Join with the first point.
                if (areaPoints.Count > 1)
                {
                    areaPoints.Add(areaPoints[0]);
                    areaPoints.Add(areaPoints[1]);
                }
            }
        }

        /// <summary>
        /// Updates the visual when series is placed in non transposed condition.
        /// </summary>
        /// <param name="width">The Width</param>
        /// <param name="height">The Height</param>
        /// <param name="color">The Color</param>
        /// <param name="leftThickness">The Left Thickness</param>
        /// <param name="rightThickness">The Right Thickness</param>
        private void UpdateVisual(int width, int height, Color color, int leftThickness, int rightThickness)
        {
            if (fastRangeAreaBitmapSeries.EnableAntiAliasing)
                DrawAnitAliasingPointsAroundArea(color, fastRangeAreaBitmapSeries.bitmapPixels, width, height);

            if (fastRangeAreaBitmapSeries.Area.IsMultipleArea && fastRangeAreaBitmapSeries.Clip != null)
            {
                var clip = this.fastRangeAreaBitmapSeries.Clip.Bounds;
                bitmap.FillPolygon(fastBuffer, areaPoints.ToArray(), width, height, color, fastRangeAreaBitmapSeries.bitmapPixels, clip);
            }
            else
            {
                bitmap.FillPolygon(fastBuffer, areaPoints.ToArray(), width, height, color, fastRangeAreaBitmapSeries.bitmapPixels);
            }

            if (fastRangeAreaBitmapSeries.StrokeThickness >= 1 && fastRangeAreaBitmapSeries.Stroke != null)
            {
                DrawStroke(fastRangeAreaBitmapSeries.bitmapPixels, width, height);
            }
        }

        private void DrawStroke(List<int> bitmapPixels, int width, int height)
        {
            var stroke = (fastRangeAreaBitmapSeries.Stroke as SolidColorBrush).Color;
            double leftThickness = fastRangeAreaBitmapSeries.StrokeThickness / 2;
            double rightThickness = (fastRangeAreaBitmapSeries.StrokeThickness % 2 == 0
                                            ? (fastRangeAreaBitmapSeries.StrokeThickness / 2) - 1
                                            : fastRangeAreaBitmapSeries.StrokeThickness / 2);
            int xStart, yStart, xEnd, yEnd;

            xStart = areaPoints[0];
            yStart = areaPoints[1];
            xEnd = areaPoints[2];
            yEnd = areaPoints[3];

            if (points1 == null)
            {
                points1 = new int[10];
            }

            GetLinePoints(xStart, yStart, xEnd, yEnd, leftThickness, rightThickness, points1);
            double[] emptyStrokeIndexes = GetEmptyStrokeIndexes();
            var isMultipleArea = fastRangeAreaBitmapSeries.Area.IsMultipleArea;

            for (int i = 2, j = 0; i < areaPoints.Count;)
            {
                points2 = new int[10];
                xStart = xEnd;
                yStart = yEnd;
                i = i + 2;

                if (i + 1 < areaPoints.Count)
                {
                    xEnd = areaPoints[i];
                    yEnd = areaPoints[i + 1];
                    UpdatePoints2(xStart, yStart, xEnd, yEnd, leftThickness, rightThickness);
                }

                if (emptyStrokeIndexes[0] != j && emptyStrokeIndexes[1] != j)
                {
                    if (isMultipleArea && fastRangeAreaBitmapSeries.Clip != null)
                    {
                        var clip = fastRangeAreaBitmapSeries.Clip.Bounds;

                        bitmap.DrawLineBresenham(fastBuffer, width, height, points1[0], points1[1], points1[2], points1[3], stroke, bitmapPixels, clip);
                        bitmap.FillPolygon(fastBuffer, points1, width, height, stroke, bitmapPixels, clip);
                        bitmap.DrawLineBresenham(fastBuffer, width, height, points1[4], points1[5], points1[6], points1[7], stroke, bitmapPixels, clip);
                    }
                    else
                    {

                        bitmap.DrawLineAa(fastBuffer, width, height, points1[0], points1[1], points1[2], points1[3], stroke, bitmapPixels);
                        bitmap.FillPolygon(fastBuffer, points1, width, height, stroke, bitmapPixels);
                        bitmap.DrawLineAa(fastBuffer, width, height, points1[4], points1[5], points1[6], points1[7], stroke, bitmapPixels);
                    }
                }

                j = j + 2;
                points1 = points2;
                points2 = null;
            }

            points1 = null;
            points2 = null;
        }

        private double[] GetEmptyStrokeIndexes()
        {
            var strokeIndexes = new double[2] { double.NaN, double.NaN };

            if (EmptyStroke == EmptyStroke.Right || EmptyStroke == EmptyStroke.Both)
            {
                strokeIndexes[0] = areaPoints.Count / 2 - 3;
            }

            if (EmptyStroke == EmptyStroke.Left || EmptyStroke == EmptyStroke.Both)
            {
                strokeIndexes[1] = areaPoints.Count - 4;
            }

            return strokeIndexes;
        }

        private void UpdatePoints2(double xStart, double yStart, double xEnd, double yEnd, double leftThickness, double rightThickness)
        {
            GetLinePoints(xStart, yStart, xEnd, yEnd, leftThickness, rightThickness, points2);

            bool parallelLine = false;

            if (points1[0] == points1[2])
                parallelLine = !(points2[0] == points2[2] && points1[0] != points2[0]);
            else if (points2[0] == points2[2])
                parallelLine = true;
            else
            {
                // both lines are not parallel to the y-axis;
                var m1 = Math.Floor(WriteableBitmapExtensions.Slope(points1[2], points1[1], points1[0], points1[3]));
                var m2 = Math.Floor(WriteableBitmapExtensions.Slope(points2[2], points2[1], points2[0], points2[3]));
                if (m1 != m2) parallelLine = true;
            }

            if (parallelLine)
            {
                bool line1 = FindIntersectingPoints(points1[0], points1[1], points1[2], points1[3], points2[0], points2[1], points2[2], points2[3]);
                if (line1)
                {
                    points1[2] = points2[0] = points2[8] = (int)(intersectingPoint.X);
                    points1[3] = points2[1] = points2[9] = (int)(intersectingPoint.Y);
                }
            }

            parallelLine = false;
            if (points1[4] == points1[6])
                parallelLine = !(points2[4] == points2[6] && points1[4] != points2[4]);
            else if (points2[4] == points2[6])
                parallelLine = true;
            else
            {
                var m3 = Math.Floor(WriteableBitmapExtensions.Slope(points1[6], points1[5], points1[4], points1[7]));
                var m4 = Math.Floor(WriteableBitmapExtensions.Slope(points2[6], points2[5], points2[4], points2[7]));
                if (m3 != m4) parallelLine = true;
            }

            if (parallelLine)
            {
                bool line = FindIntersectingPoints(points1[4], points1[5], points1[6], points1[7], points2[4], points2[5], points2[6], points2[7]);
                if (line)
                {
                    points1[5] = points2[7] = (int)(intersectingPoint.Y);
                    points1[4] = points2[6] = (int)(intersectingPoint.X);
                }
            }
        }

        private bool FindIntersectingPoints(double x11, double y11, double x12, double y12, double x21, double y21, double x22, double y22)
        {
            double pixelHeight;
            pixelHeight = bitmap.PixelHeight;
            if (y11 <= -pixelHeight) y11 = -2 * pixelHeight;
            if (y12 <= -pixelHeight) y12 = -2 * pixelHeight;
            if (y21 <= -pixelHeight) y21 = -2 * pixelHeight;
            if (y22 <= -pixelHeight) y22 = -2 * pixelHeight;

            if (y11 >= 2 * pixelHeight) y11 = 4 * pixelHeight;
            if (y12 >= 2 * pixelHeight) y12 = 4 * pixelHeight;
            if (y21 >= 2 * pixelHeight) y21 = 4 * pixelHeight;
            if (y22 >= 2 * pixelHeight) y22 = 4 * pixelHeight;

            intersectingPoint = new Point();

            // First  line  slope and intersect( y = mx+c )
            double m = WriteableBitmapExtensions.Slope(x11, y11, x12, y12);
            double c = double.NaN;
            if (!double.IsInfinity(m))
                c = WriteableBitmapExtensions.Intersect(x12, y12, m);

            // Second line slope and intersect
            double m1 = WriteableBitmapExtensions.Slope(x21, y21, x22, y22);
            double c1 = double.NaN;
            if (!double.IsInfinity(m1))
                c1 = WriteableBitmapExtensions.Intersect(x21, y21, m1);

            // point intersecting for both straight line.(Cross point for lines)
            double x = (c1 - c) / (m - m1);
            intersectingPoint.X = (int)x;
            intersectingPoint.Y = (int)(m * x) + c;

            return (double.IsNaN(x) || double.IsNaN(intersectingPoint.Y)) ? false : true;
        }

        private void DrawAnitAliasingPointsAroundArea(Color color, List<int> bitmapPixels, int width, int height)
        {
            if (fastRangeAreaBitmapSeries.Area.IsMultipleArea && fastRangeAreaBitmapSeries.Clip != null)
            {
                var clip = fastRangeAreaBitmapSeries.Clip.Bounds;
                var clipWidth = (int)clip.Width;
                var clipHeight = (int)clip.Height;
                for (int i = 0; i < areaPoints.Count; i = i + 2)
                {
                    if (i + 3 < areaPoints.Count)
                    {
                        bitmap.DrawLineAa(fastBuffer, clipWidth, clipHeight, areaPoints[i], areaPoints[i + 1], areaPoints[i + 2], areaPoints[i + 3], color, bitmapPixels, clip);
                    }
                }
            }
            else
            {
                for (int i = 0; i < areaPoints.Count; i = i + 2)
                {
                    if (i + 3 < areaPoints.Count)
                    {
                        bitmap.DrawLineAa(fastBuffer, width, height, areaPoints[i], areaPoints[i + 1], areaPoints[i + 2], areaPoints[i + 3], color, bitmapPixels);
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
