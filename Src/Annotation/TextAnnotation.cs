using System;
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace Syncfusion.UI.Xaml.Charts
{
    public partial class TextAnnotation : SingleAnnotation
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="Angle"/> property.
        /// </summary>
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register(
                "Angle",
                typeof(double),
                typeof(SingleAnnotation),
                new PropertyMetadata(0d, OnUpdatePropertyChanged));

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the rotation angle for Annotation.
        /// </summary>
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
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

            if (AnnotationElement != null && TextElement != null && X1 != null && Y1 != null)
            {
                TextElement.Visibility = Visibility.Visible;
                RotateTransform rotate = new RotateTransform { Angle = this.Angle };
                Rect rotated;
                Point centerPoint;
                switch (CoordinateUnit)
                {
                    case CoordinateUnit.Axis:
                        base.UpdateAnnotation();
                        if (XAxis != null && YAxis != null)
                        {
                            if (CoordinateUnit == CoordinateUnit.Axis && EnableClipping)
                            {
                                x1 = GetClippingValues(x1, XAxis);
                                y1 = GetClippingValues(y1, YAxis);
                            }

                            Point point = (XAxis.Orientation == Orientation.Horizontal) 
                                ? new Point(
                                  Chart.ValueToPointRelativeToAnnotation(XAxis, x1),
                                  Chart.ValueToPointRelativeToAnnotation(YAxis, y1)) 
                                : new Point(
                                  Chart.ValueToPointRelativeToAnnotation(YAxis, y1),
                                  Chart.ValueToPointRelativeToAnnotation(XAxis, x1));

                            point.Y = (double.IsNaN(point.Y)) ? 0 : point.Y;
                            point.X = (double.IsNaN(point.X)) ? 0 : point.X;
                            TextElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                            Size TextSize = TextElement.DesiredSize;
                            Point positionedPoint = GetElementPosition(TextSize, point);
                            centerPoint = new Point(
                                            positionedPoint.X + (TextSize.Width / 2),
                                            positionedPoint.Y + (TextSize.Height / 2));
                            rotated = this.RotateElement(this.Angle, TextElement, TextSize);
                            AnnotationElement.Height = TextSize.Height;
                            AnnotationElement.Width = TextSize.Width;

                            Canvas.SetLeft(AnnotationElement, positionedPoint.X);
                            Canvas.SetTop(AnnotationElement, positionedPoint.Y);

                            AnnotationElement.RenderTransformOrigin = new Point(0.5, 0.5);
                            AnnotationElement.RenderTransform = rotate;

                            if (this.Angle > 0)
                                RotatedRect = new Rect( 
                                    centerPoint.X - (rotated.Width / 2),
                                    centerPoint.Y - (rotated.Height / 2),
                                    rotated.Width, 
                                    rotated.Height);
                            else
                                RotatedRect = new Rect(
                                    centerPoint.X - (TextSize.Width / 2),
                                    centerPoint.Y - (TextSize.Height / 2),
                                    TextSize.Width, 
                                    TextSize.Height);
                        }

                        break;

                    case CoordinateUnit.Pixel:
                        TextElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        Size textSize = TextElement.DesiredSize;
                        AnnotationElement.Height = textSize.Height;
                        AnnotationElement.Width = textSize.Width;
                        Point positionedPixel = GetElementPosition(
                            textSize,
                            new Point(Convert.ToDouble(X1), Convert.ToDouble(Y1)));
                        centerPoint = new Point(
                            positionedPixel.X + (textSize.Width / 2),
                            positionedPixel.Y + (textSize.Height / 2));
                        Canvas.SetLeft(AnnotationElement, positionedPixel.X);
                        Canvas.SetTop(AnnotationElement, positionedPixel.Y);
                        AnnotationElement.RenderTransformOrigin = new Point(0.5, 0.5);
                        AnnotationElement.RenderTransform = rotate;
                        rotated = this.RotateElement(this.Angle, TextElement, textSize);
                        if (this.Angle > 0)
                            RotatedRect = new Rect(
                                centerPoint.X - (rotated.Width / 2),
                                centerPoint.Y - (rotated.Height / 2), 
                                rotated.Width, 
                                rotated.Height);
                        else
                            RotatedRect = new Rect(
                                centerPoint.X - (textSize.Width / 2),
                                centerPoint.Y - (textSize.Height / 2),
                                textSize.Width, 
                                textSize.Height);
                        break;
                }
            }
            else if (TextElement != null)
            {
                TextElement.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Returns the annotation UI element
        /// </summary>
        /// <returns></returns>
        public override UIElement GetRenderedAnnotation()
        {
            return AnnotationElement;
        }

        #endregion

        #region Internal Override Methods

        internal override UIElement CreateAnnotation()
        {
            if (AnnotationElement != null && AnnotationElement.Children.Count == 0)
            {
                SetBindings();
                TextElement.Tag = this;
                AnnotationElement.Children.Add(TextElementCanvas);
                TextElementCanvas.Children.Add(TextElement);
            }

            return AnnotationElement;
        }

        #endregion

        #region Protected Overriede Methods

        protected override DependencyObject CloneAnnotation(Annotation annotation)
        {
            TextAnnotation textAnnotation = new TextAnnotation();
            textAnnotation.Angle = this.Angle;
            return base.CloneAnnotation(textAnnotation);
        }

        #endregion

        #region Private Static Methods

        private static void OnUpdatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var annotation = sender as Annotation;
            if (annotation != null) annotation.UpdatePropertyChanged(args);
        }

        #endregion

        #endregion
    }
}
