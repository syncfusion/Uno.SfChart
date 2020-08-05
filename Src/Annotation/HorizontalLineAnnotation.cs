using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls.Primitives;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Provides a light weight UIElement that displays a horizontal line on chart. 
    /// </summary>
    /// <seealso cref="Syncfusion.UI.Xaml.Charts.StraightLineAnnotation" />
    public partial class HorizontalLineAnnotation : StraightLineAnnotation
    {
        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Updates the annotation
        /// </summary>
        public override void UpdateAnnotation()
        {
            if (shape != null)
            {
                ValidateSelection();
                switch (CoordinateUnit)
                {
                    case CoordinateUnit.Axis:
                        SetAxisFromName();
                        if (XAxis != null && YAxis != null)
                        {
                            if (Chart.AnnotationManager != null && ShowAxisLabel
                                && !Chart.ChartAnnotationCanvas.Children.Contains(AxisMarkerObject.MarkerCanvas))
                                Chart.AnnotationManager.AddOrRemoveAnnotations(AxisMarkerObject, false);
                            if (XAxis.Orientation == Orientation.Vertical)
                            {
                                if (X1 == null) break;
                                y1 = (Y1 == null) ? YAxis.VisibleRange.Start : Annotation.ConvertData(Y1, YAxis);
                                y2 = (Y2 == null) ? YAxis.VisibleRange.End : Annotation.ConvertData(Y2, YAxis);
                                X2 = X1;
                                x1 = Annotation.ConvertData(X1, XAxis);
                                x2 = Annotation.ConvertData(X2, XAxis);
                                if (ShowAxisLabel)
                                    SetAxisMarkerValue(X1, X2, YAxis.VisibleRange.Start, YAxis.VisibleRange.End, AxisMode.Horizontal);
                                this.DraggingMode = AxisMode.Horizontal;
                            }
                            else
                            {
                                if (Y1 == null) break;
                                x1 = (X1 == null) ? XAxis.VisibleRange.Start : Annotation.ConvertData(X1, XAxis);
                                x2 = (X2 == null) ? XAxis.VisibleRange.End : Annotation.ConvertData(X2, XAxis);
                                Y2 = Y1;
                                y1 = Annotation.ConvertData(Y1, YAxis);
                                y2 = Annotation.ConvertData(Y2, YAxis);
                                if (ShowAxisLabel)
                                    SetAxisMarkerValue(XAxis.VisibleRange.Start, XAxis.VisibleRange.End, Y1, Y2, AxisMode.Vertical);
                                this.DraggingMode = AxisMode.Vertical;
                            }

                            if (ShowAxisLabel)
                                AxisMarkerObject.UpdateAnnotation();
                            if (ShowLine)
                            {
                                if (CoordinateUnit == CoordinateUnit.Axis && EnableClipping)
                                {
                                    x1 = GetClippingValues(x1, XAxis);
                                    y1 = GetClippingValues(y1, YAxis);
                                    x2 = GetClippingValues(x2, XAxis);
                                    y2 = GetClippingValues(y2, YAxis);
                                }

                                Point point = (XAxis.Orientation == Orientation.Horizontal) 
                                    ? new Point(
                                        this.Chart.ValueToPointRelativeToAnnotation(XAxis, x1) - XAxis.ActualPlotOffset,
                                        this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1)) 
                                    : new Point(
                                        this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1) - YAxis.ActualPlotOffset,
                                        this.Chart.ValueToPointRelativeToAnnotation(XAxis, x1));
                                Point point2 = (XAxis.Orientation == Orientation.Horizontal)
                                    ? new Point(
                                        Chart.ValueToPointRelativeToAnnotation(XAxis, x2) + XAxis.ActualPlotOffset,
                                        this.Chart.ValueToPointRelativeToAnnotation(YAxis, y2)) 
                                    : new Point(
                                        this.Chart.ValueToPointRelativeToAnnotation(YAxis, y2) + YAxis.ActualPlotOffset,
                                        Chart.ValueToPointRelativeToAnnotation(XAxis, x2));
                                DrawLine(point, point2, shape);
                            }
                        }

                        break;
                    case CoordinateUnit.Pixel:
                        if (ShowLine && this.Chart != null && this.Chart.AnnotationManager != null && Y1 != null)
                        {
                            this.DraggingMode = AxisMode.Vertical;
                            if (Y1 == null)
                                Y1 = 0;
                            Y2 = Y1;
                            X1 = (X1 == null) ? 0 : X1;
                            X2 = (X2 == null || Convert.ToDouble(X2) == 0) ? this.Chart.DesiredSize.Width : X2;
                            Point elementPoint1 = new Point(Convert.ToDouble(X1), Convert.ToDouble(Y1));
                            Point elementPoint2 = new Point(Convert.ToDouble(X2), Convert.ToDouble(Y2));
                            DrawLine(elementPoint1, elementPoint2, shape);
                        }

                        break;
                }

                if ((XAxis != null && XAxis.Orientation == Orientation.Vertical && X1 == null) || Y1 == null)
                {
                    ClearValues();
                    if (ShowAxisLabel)
                    {
                        AxisMarkerObject.ClearValue(AxisMarker.X1Property);
                        AxisMarkerObject.ClearValue(AxisMarker.X2Property);
                        AxisMarkerObject.ClearValue(AxisMarker.Y1Property);
                        AxisMarkerObject.ClearValue(AxisMarker.Y2Property);
                    }
                }
            }
        }

        #endregion

        #region Internal Override Methods
        
        /// <summary>
        /// Upates the hit rect.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="point2"></param>
        internal override void UpdateHitArea()
        {
            if (LinePoints == null) return;

            var p1 = LinePoints[0];
            var p2 = LinePoints[1];
            var ensurePoint = EnsurePoint(p1, p2);
            var width = Math.Abs(p2.X - p1.X);
            var height = Math.Abs(p2.Y - p1.Y);
            RotatedRect = new Rect(ensurePoint.X, ensurePoint.Y - GrabExtent, width, height + 2 * GrabExtent);
        }

        #endregion

        #region Internal Methods

        internal Point GetAxisLabelPosition(Size desiredSize, Point originalPosition, Size textSize)
        {
            Point point = originalPosition;
            ChartAxis axis = (XAxis.Orientation == Orientation.Vertical) ? XAxis : YAxis;
            ChartAxis axis1 = (XAxis.Orientation != Orientation.Vertical) ? XAxis : YAxis;
            point.Y -= textSize.Height / 2;
            double left = 0;
            var chartAxes = Chart.Axes.Where(axes => (axes.Orientation == axis.Orientation)).Where(position => (!position.OpposedPosition));
            point.X -= axis1.ActualPlotOffset;
            if (axis.OpposedPosition)
            {
                if (chartAxes.Count() > 0)
                    left = chartAxes.ElementAt(0).RenderedRect.Right;
                point.X += (axis.RenderedRect.Left - left);
            }
            else
            {
                if (chartAxes.Count() > 0)
                    left = chartAxes.ElementAt(0).RenderedRect.Left;
                point.X -= (textSize.Width + (left - axis.RenderedRect.Left));
            }

            return point;
        }

        #endregion

        #endregion
    }
}
