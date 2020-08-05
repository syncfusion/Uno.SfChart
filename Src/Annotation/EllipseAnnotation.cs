using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Provides a light weight UIElement which is overlay on chart. 
    /// </summary>
    /// <seealso cref="Syncfusion.UI.Xaml.Charts.SolidShapeAnnotation" />
    public partial class EllipseAnnotation : SolidShapeAnnotation
    {
        #region Dependency Property Registration.

        // Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
        public new static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(
                "Width", 
                typeof(double), 
                typeof(EllipseAnnotation), 
                new PropertyMetadata(10d, OnSizePropertyChanged));

        // Using a DependencyProperty as the backing store for Height.  This enables animation, styling, binding, etc...
        public new static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(
                "Height", 
                typeof(double), 
                typeof(EllipseAnnotation), 
                new PropertyMetadata(10d, OnSizePropertyChanged));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the width of the annotation.
        /// </summary>
        public new double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height of the annotation.
        /// </summary>
        public new double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// Updates the annotation.
        /// </summary>
        public override void UpdateAnnotation()
        {
            if (IsRenderSize())
            {
                if ((shape != null || ResizerControl != null) && X1 != null && Y1 != null)
                {
                    switch (CoordinateUnit)
                    {
                        case CoordinateUnit.Axis:
                            SetData();
                            if (XAxis != null && YAxis != null)
                            {
                                Point point = (XAxis.Orientation == Orientation.Horizontal) 
                                    ? new Point(
                                      this.Chart.ValueToPointRelativeToAnnotation(XAxis, x1),
                                      this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1)) 
                                    : new Point(
                                      this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1),
                                      this.Chart.ValueToPointRelativeToAnnotation(XAxis, x1));
                                Point point2 = new Point((double.IsNaN(point.X) ? 0 : point.X) + Width, (double.IsNaN(point.Y) ? 0 : point.Y) + Height);
                                                            
                                UpdateAxisAnnotation(point, point2);
                            }
                            break;

                        case CoordinateUnit.Pixel:
                            var pixelX1 = Convert.ToDouble(X1);
                            var pixely1 = Convert.ToDouble(Y1);
                            Point elementPoint1 = new Point(pixelX1, pixely1);
                            Point elementPoint2 = new Point(pixelX1 + Width, pixely1 + Height);

                            UpdatePixelAnnotation(elementPoint1, elementPoint2);
                            break;
                    }
                    CheckResizerValues();
                }
                else if (shape != null || ResizerControl != null)
                {
                    ClearAnnotationElements();
                }
            }
            else
            {
                base.UpdateAnnotation();
            }
        }

        #endregion

        #region Internal Methods
        
        /// <summary>
        /// Creates the <see cref="UIElement"/> for the annotation.
        /// </summary>
        /// <returns>Returns the created annotation element.</returns>
        internal override UIElement CreateAnnotation()
        {
            if (AnnotationElement != null && AnnotationElement.Children.Count == 0)
            {
                shape = new Ellipse();
                shape.Tag = this;
                SetBindings();
                AnnotationElement.Children.Add(shape);
                TextElementCanvas.Children.Add(TextElement);
                AnnotationElement.Children.Add(TextElementCanvas);
            }

            return AnnotationElement;
        }

        #endregion

        #region Protected Methods
        
        /// <summary>
        /// Clones the annotation.
        /// </summary>
        /// <param name="annotation">The annotation to be cloned.</param>
        /// <returns>Returns the cloned annotation.</returns>
        protected override DependencyObject CloneAnnotation(Annotation annotation)
        {
            return base.CloneAnnotation(new EllipseAnnotation());
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the annotation on size changed.
        /// </summary>
        /// <param name="d">The dependency object ellipse annotation.</param>
        /// <param name="e">The dependency property changed event arguments of the annotation.</param>
        private static void OnSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ellipseAnnotation = d as EllipseAnnotation;
            if (ellipseAnnotation != null)
            {
                ellipseAnnotation.UpdateAnnotation();
            }
        }

        /// <summary>
        /// Updates the annotation.
        /// </summary>
        /// <param name="d">The ellipse annotation which has to be updated.</param>
        /// <param name="e">The ellipse annotation size changed event arguments.</param>
        private static void UpdateAnnotation(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ellipseAnnotation = d as EllipseAnnotation;
            if (ellipseAnnotation != null)
                ellipseAnnotation.UpdateAnnotation();
        }
        
        /// <summary>
        /// Checks whether to render the annotation with respect to size or co - ordinate units.
        /// </summary>
        /// <returns>Returns a value indicating whether to render the annotation with respect to size or co - ordinate units.</returns>
        private bool IsRenderSize()
        {
            return X2 == null || (X2.GetType() == typeof(double) && double.IsNaN((double)X2))
                   || Y2 == null || (Y2.GetType() == typeof(double) && double.IsNaN((double)Y2));     
        }

        #endregion
    }
}
