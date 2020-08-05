using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.UI.Xaml.Charts
{
    public partial class VerticalLineAnnotation : StraightLineAnnotation
    {
        #region Dependency Property Registration 

        /// <summary>
        /// The DependencyProperty for <see cref="HorizontalTextAlignment"/> property.
        /// </summary>
        public static readonly new DependencyProperty HorizontalTextAlignmentProperty =
            DependencyProperty.Register(
                "HorizontalTextAlignment",
                typeof(HorizontalAlignment),
                typeof(VerticalLineAnnotation),
                new PropertyMetadata(HorizontalAlignment.Right, OnTextAlignmentPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="VerticalTextAlignment"/> property.
        /// </summary>
        public static readonly new DependencyProperty VerticalTextAlignmentProperty =
            DependencyProperty.Register(
                "VerticalTextAlignment",
                typeof(VerticalAlignment),
                typeof(VerticalLineAnnotation),
                new PropertyMetadata(VerticalAlignment.Center, OnTextAlignmentPropertyChanged));

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the horizontal text alignment.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.HorizontalAlignment"/> value.
        /// </value>
        public new HorizontalAlignment HorizontalTextAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalTextAlignmentProperty); }
            set { SetValue(HorizontalTextAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the vertical text alignment.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.VerticalTextAlignment"/> value.
        /// </value>
        public new VerticalAlignment VerticalTextAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalTextAlignmentProperty); }
            set { SetValue(VerticalTextAlignmentProperty, value); }
        }

        #endregion

        #endregion

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

                            if (XAxis.Orientation == Orientation.Horizontal)
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
                                x1 = (X1 == null) ? XAxis.VisibleRange.Start : ConvertData(X1, XAxis);
                                x2 = (X2 == null) ? XAxis.VisibleRange.End : ConvertData(X2, XAxis);
                                Y2 = Y1;
                                y1 = ConvertData(Y1, YAxis);
                                y2 = ConvertData(Y2, YAxis);
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
                                      this.Chart.ValueToPointRelativeToAnnotation(XAxis, x1),
                                      this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1) + YAxis.ActualPlotOffset) 
                                    : new Point(
                                      this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1),
                                      this.Chart.ValueToPointRelativeToAnnotation(XAxis, x1) + XAxis.ActualPlotOffset);
                                Point point2 = (XAxis.Orientation == Orientation.Horizontal) 
                                    ? new Point(
                                      Chart.ValueToPointRelativeToAnnotation(XAxis, x2),
                                      this.Chart.ValueToPointRelativeToAnnotation(YAxis, y2) - YAxis.ActualPlotOffset) 
                                    : new Point(
                                      this.Chart.ValueToPointRelativeToAnnotation(YAxis, y2),
                                      Chart.ValueToPointRelativeToAnnotation(XAxis, x2) - XAxis.ActualPlotOffset);
                                DrawLine(point, point2, shape);
                            }
                        }

                        break;

                    case CoordinateUnit.Pixel:
                        if (ShowLine && this.Chart != null && X1 != null)
                        {
                            this.DraggingMode = AxisMode.Horizontal;
                            X2 = X1;
                            Y1 = (Y1 == null) ? 0 : Y1;
                            if (this.Chart.DesiredSize.Height != 0)
                                Y2 = (Y2 == null) ? this.Chart.DesiredSize.Height : Y2;
                            Point elementPoint1 = new Point(Convert.ToDouble(X1), Convert.ToDouble(Y1));
                            Point elementPoint2 = new Point(Convert.ToDouble(X2), Convert.ToDouble(Y2));
                            DrawLine(elementPoint1, elementPoint2, shape);
                        }

                        break;
                }

                if ((YAxis != null && YAxis.Orientation == Orientation.Horizontal && Y1 == null) || X1 == null)
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
            RotatedRect = new Rect(ensurePoint.X - GrabExtent, ensurePoint.Y, width + 2 * GrabExtent, height);
        }

        #endregion

        #region Internal Methods

        internal Point GetAxisLabelPosition(Size desiredSize, Point originalPosition, Size textSize)
        {
            Point point = originalPosition;
            ChartAxis axis = (XAxis.Orientation == Orientation.Horizontal) ? XAxis : YAxis;
            ChartAxis axis1 = (XAxis.Orientation != Orientation.Horizontal) ? XAxis : YAxis;
            point.X += (desiredSize.Width / 2);
            point.X -= (textSize.Width / 2);
            point.Y = axis.OpposedPosition ? point.Y : 0;
            var chartAxes = Chart.Axes.Where(axes => (axes.Orientation == axis.Orientation))
                    .Where(position => (position.OpposedPosition));
            double bottom = (chartAxes.Count() > 0) ? chartAxes.ElementAt(0).RenderedRect.Bottom : 0;
            if (axis.OpposedPosition)
            {
                point.Y -= (textSize.Height);
                if (Chart.Axes.IndexOf(axis) != 0)
                    point.Y -= (bottom - axis.RenderedRect.Bottom);
                point.Y -= axis1.ActualPlotOffset;
            }
            else
                point.Y += (axis.RenderedRect.Top - bottom);
            return point;
        }

        #endregion

        #region Protected Override Methods

        protected override void SetTextElementPosition(Point point, Point point2, Size desiredSize, Point positionedPoint, ContentControl TextElement)
        {
            double slope;
            RotateTransform rotate = new RotateTransform();
            TextElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            slope = (point2.Y - point.Y) / (point2.X - point.X);
            rotate.Angle = double.IsNaN(slope) ? 0 : Math.Atan(slope) * (180 / Math.PI);
            TextElement.RenderTransformOrigin = new Point(0, 0);
            TextElement.RenderTransform = rotate;

            if (this.HorizontalTextAlignment == HorizontalAlignment.Left)
                Canvas.SetLeft(TextElement, point.X - 30);
            else if (this.HorizontalTextAlignment == HorizontalAlignment.Center)
                Canvas.SetLeft(TextElement, point.X - 10);
            else
                Canvas.SetLeft(TextElement, point.X);

            if (this.VerticalTextAlignment == VerticalAlignment.Bottom)
                Canvas.SetTop(TextElement, point.Y);
            else if (this.VerticalTextAlignment == VerticalAlignment.Top)
                Canvas.SetTop(TextElement, point2.Y + TextElement.DesiredSize.Width);
            else
                Canvas.SetTop(TextElement, (point.Y + point2.Y) / 2 + (TextElement.DesiredSize.Width / 2));
        }


        #endregion

        #region Private Static Methods
        
        private static void OnTextAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Annotation.OnTextAlignmentChanged(d, e);
        }

        #endregion

        #endregion
    }
}
