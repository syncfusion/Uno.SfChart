using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Provides a light weight UIElement that displays image on chart. 
    /// </summary>
    /// <seealso cref="Syncfusion.UI.Xaml.Charts.SingleAnnotation" />
    public partial class ImageAnnotation : SingleAnnotation
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="Angle"/> property.
        /// </summary>
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register(
                "Angle",
                typeof(double),
                typeof(ImageAnnotation),
                new PropertyMetadata(0d, OnUpdatePropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Y2"/> property.
        /// </summary>
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register(
                "Y2",
                typeof(object),
                typeof(ImageAnnotation),
                new PropertyMetadata(null, OnHeightWidthChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="X2"/> property.
        /// </summary>
        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register(
                "X2",
                typeof(object),
                typeof(ImageAnnotation),
                new PropertyMetadata(null, OnHeightWidthChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="HorizontalTextAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty HorizontalTextAlignmentProperty =
            DependencyProperty.Register(
                "HorizontalTextAlignment",
                typeof(HorizontalAlignment),
                typeof(ImageAnnotation),
                new PropertyMetadata(HorizontalAlignment.Center, OnAlignmentChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="VerticalTextAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty VerticalTextAlignmentProperty =
            DependencyProperty.Register(
                "VerticalTextAlignment",
                typeof(VerticalAlignment),
                typeof(ImageAnnotation),
                new PropertyMetadata(VerticalAlignment.Bottom, OnTextAlignmentChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ImageSource"/> property.
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(
                "ImageSource",
                typeof(string),
                typeof(ImageAnnotation),
                new PropertyMetadata(string.Empty, OnImageSourceChanged));

        #endregion

        #region Fields

        private Image _image;
        private Border _imageBorder;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ImageAnnotation() : base()
        {

        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the rotation angle for the <c>Annotation</c>.
        /// </summary>
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the source for the image to be added as annotation.
        /// </summary>
        /// <value>
        /// This accepts image source path as <see cref="System.String"/>.
        /// </value>
        public string ImageSource
        {
            get { return (string)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Y2 value.
        /// </summary>
        public object Y2
        {
            get { return (object)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }

        /// <summary>
        /// Gets or sets the X2 value.
        /// </summary>
        public object X2
        {
            get { return (object)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        /// <summary>
        /// Gets or sets the horizontal text alignment.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.HorizontalAlignment"/>.
        /// </value>
        public HorizontalAlignment HorizontalTextAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalTextAlignmentProperty); }
            set { SetValue(HorizontalTextAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the vertical text alignment.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.VerticalAlignment"/>.
        /// </value>
        public VerticalAlignment VerticalTextAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalTextAlignmentProperty); }
            set { SetValue(VerticalTextAlignmentProperty, value); }
        }

        #endregion

        #region Internal Properties

        internal double ImageWidth
        {
            get;
            set;
        }

        internal double ImageHeight
        {
            get;
            set;
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
            if (_image != null && X1 != null && Y1 != null)
            {
                Point textPosition = new Point();
                Size desiredSize;
                Rect heightAndWidthRect;
                RotateTransform rotate = new RotateTransform { Angle = this.Angle };
                Point positionedPoint = new Point(0, 0);
                TextElement.Visibility = Visibility.Visible;
                switch (CoordinateUnit)
                {
                    case CoordinateUnit.Axis:
                        base.UpdateAnnotation();
                        if (XAxis != null && YAxis != null)
                        {
                            ImageHeight = Annotation.ConvertData(Y2, YAxis);
                            ImageWidth = Annotation.ConvertData(X2, XAxis);
                            if (CoordinateUnit == CoordinateUnit.Axis && EnableClipping)
                            {
                                x1 = GetClippingValues(x1, XAxis);
                                y1 = GetClippingValues(y1, YAxis);
                                ImageWidth = GetClippingValues(ImageWidth, XAxis);
                                ImageHeight = GetClippingValues(ImageHeight, YAxis);
                            }

                            Point point = (XAxis.Orientation == Orientation.Horizontal) 
                                          ? new Point(
                                            this.Chart.ValueToPointRelativeToAnnotation(XAxis, x1),
                                            this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1)) 
                                            : new Point(
                                            this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1),
                                            this.Chart.ValueToPointRelativeToAnnotation(XAxis, x1));
                            Point point2 = (XAxis.Orientation == Orientation.Horizontal) 
                                                     ? new Point(
                                                     Chart.ValueToPointRelativeToAnnotation(XAxis, ImageWidth),
                                                     this.Chart.ValueToPointRelativeToAnnotation(YAxis, ImageHeight)) 
                                                     : new Point(
                                                         this.Chart.ValueToPointRelativeToAnnotation(YAxis, ImageHeight),
                                                         Chart.ValueToPointRelativeToAnnotation(XAxis, ImageWidth));
                            point.Y = (double.IsNaN(point.Y)) ? 0 : point.Y;
                            point.X = (double.IsNaN(point.X)) ? 0 : point.X;
                            point2.Y = (double.IsNaN(point2.Y)) ? 0 : point2.Y;
                            point2.X = (double.IsNaN(point2.X)) ? 0 : point2.X;
                            heightAndWidthRect = new Rect(point, point2);
                            _image.Height = heightAndWidthRect.Height;
                            _image.Width = heightAndWidthRect.Width;
                            AnnotationElement.Height = heightAndWidthRect.Height;
                            AnnotationElement.Width = heightAndWidthRect.Width;
                            Point ensurePoint = this.EnsurePoint(point, point2);
                            desiredSize = new Size(heightAndWidthRect.Width, heightAndWidthRect.Height);
                            positionedPoint = GetElementPosition(
                                new Size(heightAndWidthRect.Width, heightAndWidthRect.Height), ensurePoint);

                            TextElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                            if (X2 != null && Y2 != null)
                            {
                                textPosition = GetTextPosition(desiredSize, new Point(0, 0), new Size(TextElement.DesiredSize.Width, TextElement.DesiredSize.Height));
                                Canvas.SetLeft(TextElement, textPosition.X);
                                Canvas.SetTop(TextElement, textPosition.Y);
                                Canvas.SetLeft(AnnotationElement, positionedPoint.X);
                                Canvas.SetTop(AnnotationElement, positionedPoint.Y);
                            }
                            else
                            {
                                _image.Height = 0;
                                _image.Width = 0;
                                if (ContentTemplate != null)
                                {
                                    AnnotationElement.Height = TextElement.DesiredSize.Height;
                                    AnnotationElement.Width = TextElement.DesiredSize.Width;

                                    textPosition = GetTextPositionWithX1Y1(point, new Size(TextElement.DesiredSize.Width, TextElement.DesiredSize.Height));
                                    Canvas.SetLeft(TextElement, 0);
                                    Canvas.SetTop(TextElement, 0);
                                    Canvas.SetLeft(AnnotationElement, textPosition.X);
                                    Canvas.SetTop(AnnotationElement, textPosition.Y);
                                }
                            }

                            Point centerPoint = new Point(
                                positionedPoint.X + (AnnotationElement.Width / 2),
                                positionedPoint.Y + (AnnotationElement.Height / 2));
                            Rect rotated = this.RotateElement(this.Angle, AnnotationElement);
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
                                    centerPoint.X - (AnnotationElement.Width / 2),
                                    centerPoint.Y - (AnnotationElement.Height / 2),
                                    AnnotationElement.Width, 
                                    AnnotationElement.Height);
                        }

                        break;
                    case CoordinateUnit.Pixel:
                        Point elementPoint1 = new Point(Convert.ToDouble(X1), Convert.ToDouble(Y1));
                        Point elementPoint2 = new Point(Convert.ToDouble(X2), Convert.ToDouble(Y2));
                        heightAndWidthRect = new Rect(elementPoint1, elementPoint2);
                        _image.Height = heightAndWidthRect.Height;
                        _image.Width = heightAndWidthRect.Width;
                        AnnotationElement.Height = heightAndWidthRect.Height;
                        AnnotationElement.Width = heightAndWidthRect.Width;
#if __IOS__ || __ANDROID__
                        positionedPoint = GetElementPosition(_imageBorder, elementPoint1);
#else
                        positionedPoint = GetElementPosition(_image, elementPoint1);
#endif
                        desiredSize = new Size(_image.Width, _image.Height);
                        TextElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                        if (X2 != null && Y2 != null)
                        {
                            textPosition = GetTextPosition(
                                desiredSize, 
                                new Point(0, 0),
                                new Size(TextElement.DesiredSize.Width, TextElement.DesiredSize.Height));
                            Canvas.SetLeft(AnnotationElement, positionedPoint.X);
                            Canvas.SetTop(AnnotationElement, positionedPoint.Y);
                            Canvas.SetLeft(TextElement, textPosition.X);
                            Canvas.SetTop(TextElement, textPosition.Y);
                        }
                        else
                        {
                            _image.Height = 0;
                            _image.Width = 0;
                            if (ContentTemplate != null)
                            {
                                AnnotationElement.Height = TextElement.DesiredSize.Height;
                                AnnotationElement.Width = TextElement.DesiredSize.Width;
                                textPosition = GetTextPositionWithX1Y1(elementPoint1, new Size(TextElement.DesiredSize.Width, TextElement.DesiredSize.Height));
                                Canvas.SetLeft(TextElement, 0);
                                Canvas.SetTop(TextElement, 0);
                                Canvas.SetLeft(AnnotationElement, textPosition.X);
                                Canvas.SetTop(AnnotationElement, textPosition.Y);
                            }
                        }

                        Rect _rotated = this.RotateElement(this.Angle, AnnotationElement);
                        AnnotationElement.RenderTransformOrigin = new Point(0.5, 0.5);
                        AnnotationElement.RenderTransform = rotate;
                        Point _centerPoint = new Point(
                            positionedPoint.X + (AnnotationElement.Width / 2),
                            positionedPoint.Y + (AnnotationElement.Height / 2));
                        if (this.Angle > 0)
                            RotatedRect = new Rect(
                                _centerPoint.X - (_rotated.Width / 2),
                                _centerPoint.Y - (_rotated.Height / 2), 
                                _rotated.Width, 
                                _rotated.Height);
                        else
                            RotatedRect = new Rect(
                                _centerPoint.X - (AnnotationElement.Width / 2),
                                _centerPoint.Y - (AnnotationElement.Height / 2),
                                AnnotationElement.Width, 
                                AnnotationElement.Height);
                        break;
                }
            }
            else if (_image != null)
            {
                _image.Height = 0;
                _image.Width = 0;
                AnnotationElement.ClearValue(Canvas.LeftProperty);
                AnnotationElement.ClearValue(Canvas.TopProperty);
                TextElement.Visibility = Visibility.Collapsed;
            }
        }

#endregion

#region Internal Override Methods

        internal override UIElement CreateAnnotation()
        {
            if (AnnotationElement != null && AnnotationElement.Children.Count == 0)
            {
                _image = new Image();
#if __IOS__ || __ANDROID__
                _imageBorder = new Border { Child = _image };
#endif
                SetBindings();
                _image.Tag = this;
#if __IOS__ || __ANDROID__
                AnnotationElement.Children.Add(_imageBorder);
#else
                AnnotationElement.Children.Add(_image);
#endif
                TextElementCanvas.Children.Add(TextElement);
                AnnotationElement.Children.Add(TextElementCanvas);
            }

            return AnnotationElement;
        }

#endregion

#region Internal Methods

        internal void HeightWidthChanged()
        {
            switch (CoordinateUnit)
            {
                case CoordinateUnit.Axis:
                    this.ImageHeight = Annotation.ConvertData(this.Y2, this.YAxis);
                    this.ImageWidth = Annotation.ConvertData(this.X2, this.XAxis);
                    break;
            }

            UpdateAnnotation();
            if (Chart != null && this.CoordinateUnit == CoordinateUnit.Axis && CanUpdateRange(this.X2, this.Y2))
                Chart.ScheduleUpdate();
        }

#endregion

#region Protected Override Methods

        protected override void SetBindings()
        {
            base.SetBindings();
#if NETFX_CORE
            if (!(string.IsNullOrEmpty(ImageSource)))
            {
                BitmapImage imageSource = new BitmapImage { UriSource = new Uri(ImageSource, UriKind.Absolute) };
                _image.Source = imageSource;
            }
#else
            _image.Source = ImageSource;
#endif
            _image.Stretch = Stretch.Fill;
        }

        protected override DependencyObject CloneAnnotation(Annotation annotation)
        {
            ImageAnnotation imageAnnotation = new ImageAnnotation();
            imageAnnotation.Angle = this.Angle;
            imageAnnotation.HorizontalTextAlignment = this.HorizontalTextAlignment;
            imageAnnotation.ImageSource = this.ImageSource;
            imageAnnotation.VerticalTextAlignment = this.VerticalTextAlignment;
            imageAnnotation.X2 = this.X2;
            imageAnnotation.Y2 = this.Y2;
            return base.CloneAnnotation(imageAnnotation);
        }

#endregion

#region Protected Methods

        protected Point GetTextPosition(Size desiredSize, Point originalPosition)
        {
            Point point = originalPosition;
            HorizontalAlignment horizontalAlignment = this.HorizontalTextAlignment;
            VerticalAlignment verticalAlignment = this.VerticalTextAlignment;
            Size textSize = new Size(TextElement.ActualWidth, TextElement.ActualHeight);
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    point.X += (desiredSize.Width / 2);
                    point.X -= (textSize.Width / 2);
                    break;
                case HorizontalAlignment.Left:
                    point.X -= (textSize.Width);
                    break;
                case HorizontalAlignment.Right:
                    point.X += (desiredSize.Width);
                    break;
                default:
                    break;
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    point.Y += (desiredSize.Height);
                    break;
                case VerticalAlignment.Center:
                    point.Y += (desiredSize.Height / 2);
                    point.Y -= (textSize.Height / 2);
                    break;
                case VerticalAlignment.Top:
                    point.Y -= (textSize.Height);
                    break;
                default:
                    break;
            }

            return point;
        }

        protected Point GetTextPosition(Size desiredSize, Point originalPosition, Size textSize)
        {
            Point point = originalPosition;
            HorizontalAlignment horizontalAlignment = this.HorizontalTextAlignment;
            VerticalAlignment verticalAlignment = this.VerticalTextAlignment;

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    point.X += (desiredSize.Width / 2);
                    point.X -= (textSize.Width / 2);
                    break;
                case HorizontalAlignment.Left:
                    point.X -= (textSize.Width);
                    break;
                case HorizontalAlignment.Right:
                    point.X += (desiredSize.Width);
                    break;
                default:
                    break;
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    point.Y += (desiredSize.Height);
                    break;
                case VerticalAlignment.Center:
                    point.Y += (desiredSize.Height / 2);
                    point.Y -= (textSize.Height / 2);
                    break;
                case VerticalAlignment.Top:
                    point.Y -= (textSize.Height);
                    break;
            }

            return point;
        }


#endregion

#region Private Static Methods

        private static void OnUpdatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var annotation = sender as Annotation;
            if (annotation != null) annotation.UpdatePropertyChanged(args);
        }

        private static void OnImageSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var _imageAnnotation = sender as ImageAnnotation;
            if (_imageAnnotation != null && _imageAnnotation.XAxis != null && _imageAnnotation.YAxis != null) _imageAnnotation.SetBindings();
        }

        private static void OnHeightWidthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var _imageAnnotation = sender as ImageAnnotation;
            if (_imageAnnotation != null && _imageAnnotation.XAxis != null && _imageAnnotation.YAxis != null) _imageAnnotation.HeightWidthChanged();
        }

        private static void OnAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Annotation.OnTextAlignmentChanged(d, e);
        }

#endregion

#region Private Methods

        private Point GetTextPositionWithX1Y1(Point specifiedPoints, Size textSize)
        {
            Point point = specifiedPoints;
            HorizontalAlignment horizontalAlignment = this.HorizontalTextAlignment;
            VerticalAlignment verticalAlignment = this.VerticalTextAlignment;
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    point.X -= (textSize.Width / 2);
                    break;
                case HorizontalAlignment.Left:
                    point.X -= (textSize.Width);
                    break;
                default:
                    break;
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Center:
                    point.Y -= (textSize.Height / 2);
                    break;
                case VerticalAlignment.Top:
                    point.Y -= (textSize.Height);
                    break;
            }

            return point;
        }

#endregion

#endregion
    }
}
