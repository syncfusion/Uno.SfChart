using System;
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
    public abstract partial class ShapeAnnotation : SingleAnnotation
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="HorizontalTextAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty HorizontalTextAlignmentProperty =
            DependencyProperty.Register(
                "HorizontalTextAlignment",
                typeof(HorizontalAlignment),
                typeof(ShapeAnnotation),
                new PropertyMetadata(HorizontalAlignment.Center, OnTextAlignmentChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="DraggingMode"/> property.
        /// </summary>
        public static readonly DependencyProperty DraggingModeProperty =
            DependencyProperty.Register(
                "DraggingMode",
                typeof(AxisMode),
                typeof(ShapeAnnotation),
                new PropertyMetadata(AxisMode.All));

#if WINDOWS_UAP

        /// <summary>
        /// The DependencyProperty for <see cref="CanDrag"/> property.
        /// </summary>
        new public static readonly DependencyProperty CanDragProperty =
            DependencyProperty.Register(
                "CanDrag",
                typeof(bool),
                typeof(ShapeAnnotation),
                new PropertyMetadata(false));

#else

        /// <summary>
        /// The DependencyProperty for <see cref="CanDrag"/> property.
        /// </summary>
        public static readonly DependencyProperty CanDragProperty =
            DependencyProperty.Register(
                "CanDrag",
                typeof(bool),
                typeof(ShapeAnnotation),
                new PropertyMetadata(false));

#endif

        /// <summary>
        /// The DependencyProperty for <see cref="CanResize"/> property.
        /// </summary>
        public static readonly DependencyProperty CanResizeProperty =
            DependencyProperty.Register(
                "CanResize",
                typeof(bool),
                typeof(ShapeAnnotation),
                new PropertyMetadata(false, OnCanResizeChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="VerticalTextAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty VerticalTextAlignmentProperty =
            DependencyProperty.Register(
                "VerticalTextAlignment",
                typeof(VerticalAlignment),
                typeof(ShapeAnnotation),
                new PropertyMetadata(VerticalAlignment.Bottom, OnAlignmentChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Fill"/> property.
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(
                "Fill",
                typeof(Brush),
                typeof(ShapeAnnotation),
#if UNIVERSALWINDOWS
                new PropertyMetadata(null, OnFillChanged));
#else
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(50, 30, 144, 255)), OnFillChanged));
#endif

        private static void OnFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var shapeAnnotation = d as ShapeAnnotation;
            if (shapeAnnotation != null && shapeAnnotation.shape != null)
                shapeAnnotation.shape.Fill = (Brush)e.NewValue;
        }

        /// <summary>
        /// The DependencyProperty for <see cref="Y2"/> property.
        /// </summary>
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register(
                "Y2",
                typeof(object),
                typeof(ShapeAnnotation),
                new PropertyMetadata(null, OnHeightWidthChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="X2"/> property.
        /// </summary>
        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register(
                "X2",
                typeof(object),
                typeof(ShapeAnnotation),
                new PropertyMetadata(null, OnHeightWidthChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeThickness"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(
                "StrokeThickness",
                typeof(double),
                typeof(ShapeAnnotation),
                new PropertyMetadata(1d, OnStrokeThicknessChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Stroke"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(
                "Stroke",
                typeof(Brush),
                typeof(ShapeAnnotation),

#if UNIVERSALWINDOWS
                new PropertyMetadata(null, OnStrokeChanged));
#else
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 30, 144, 255)), OnStrokeChanged));
#endif


        private static void OnStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var shapeAnnotation = d as ShapeAnnotation;
            if (shapeAnnotation != null && shapeAnnotation.shape != null)
                shapeAnnotation.shape.Stroke = (Brush)e.NewValue;
        }

        private static void OnStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var shapeAnnotation = d as ShapeAnnotation;
            if (shapeAnnotation != null && shapeAnnotation.shape != null)
                shapeAnnotation.shape.StrokeThickness = (double)e.NewValue;
        }

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeDashArray"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register(
                "StrokeDashArray",
                typeof(DoubleCollection),
                typeof(ShapeAnnotation),
                new PropertyMetadata(null, OnStrokeDashArrayChanged));

        private static void OnStrokeDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var shapeAnnotation = d as ShapeAnnotation;
            if (shapeAnnotation != null && shapeAnnotation.shape != null)
                shapeAnnotation.shape.StrokeDashArray = (DoubleCollection)e.NewValue;
        }
        /// <summary>
        /// The DependencyProperty for <see cref="StrokeDashCap"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeDashCapProperty =
            DependencyProperty.Register(
                "StrokeDashCap",
                typeof(PenLineCap),
                typeof(ShapeAnnotation),
                new PropertyMetadata(PenLineCap.Flat,OnStrokeDashCapChanged));

        private static void OnStrokeDashCapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var shapeAnnotation = d as ShapeAnnotation;
            if (shapeAnnotation != null && shapeAnnotation.shape != null)
                shapeAnnotation.StrokeDashCap = (PenLineCap)e.NewValue;
        }

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeDashOffset"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeDashOffsetProperty =
            DependencyProperty.Register(
                "StrokeDashOffset",
                typeof(double),
                typeof(ShapeAnnotation),
                new PropertyMetadata(Shape.StrokeDashOffsetProperty.GetMetadata(typeof(Shape)).DefaultValue));

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeEndLineCap"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeEndLineCapProperty =
            DependencyProperty.Register(
                "StrokeEndLineCap",
                typeof(PenLineCap),
                typeof(ShapeAnnotation),
                new PropertyMetadata(PenLineCap.Flat, OnStrokeEndLineCapChanged));

        private static void OnStrokeEndLineCapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var shapeAnnotation = d as ShapeAnnotation;
            if (shapeAnnotation != null && shapeAnnotation.shape != null)
                shapeAnnotation.shape.StrokeEndLineCap = (PenLineCap)e.NewValue;
        }

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeLineJoin"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeLineJoinProperty =
            DependencyProperty.Register(
                "StrokeLineJoin",
                typeof(PenLineJoin),
                typeof(ShapeAnnotation),
                new PropertyMetadata(Shape.StrokeLineJoinProperty.GetMetadata(typeof(Shape)).DefaultValue));

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeMiterLimit"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeMiterLimitProperty =
            DependencyProperty.Register(
                "StrokeMiterLimit",
                typeof(double),
                typeof(ShapeAnnotation),
                new PropertyMetadata(Shape.StrokeMiterLimitProperty.GetMetadata(typeof(Shape)).DefaultValue));

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeStartLineCap" property.
        /// </summary>
        public static readonly DependencyProperty StrokeStartLineCapProperty =
            DependencyProperty.Register(
                "StrokeStartLineCap",
                typeof(PenLineCap),
                typeof(ShapeAnnotation),
                new PropertyMetadata(Shape.StrokeStartLineCapProperty.GetMetadata(typeof(Shape)).DefaultValue));

        #endregion

        #region Fields

        #region Internal Fields

        internal Thumb nearThumb, farThumb;

        internal Shape shape;

        #endregion

        #region Private Fields

        protected double x2;

        protected double y2;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ShapeAnnotation() : base()
        {
            Fill = new SolidColorBrush(Color.FromArgb(50, 30, 144, 255));
            Stroke = new SolidColorBrush(Color.FromArgb(255, 30, 144, 255));
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when annotation drag is started.
        /// </summary>
        public event EventHandler DragStarted;

        /// <summary>
        /// Occurs when annotation dragging.
        /// </summary>
        public event EventHandler<AnnotationDragDeltaEventArgs> DragDelta;

        /// <summary>
        /// Occurs when annotation drag is completed.
        /// </summary>
        public event EventHandler<AnnotationDragCompletedEventArgs> DragCompleted;

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the horizontal text alignment.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.HorizontalAlignment"/> value.
        /// </value>
        public HorizontalAlignment HorizontalTextAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalTextAlignmentProperty); }
            set { SetValue(HorizontalTextAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the dragging direction for the annotation.
        /// </summary>
        /// <value>
        ///     <c>AxisMode.Horizontal</c> 
        ///     <c>AxisMode.Vertical</c> 
        ///     <c>AxisMode.All</c> 
        /// </value>
        public AxisMode DraggingMode
        {
            get { return (AxisMode)GetValue(DraggingModeProperty); }
            set { SetValue(DraggingModeProperty, value); }
        }

#if WINDOWS_UAP
        /// <summary>
        /// Gets or sets a value indicating whether dragging is enabled for the annotation.
        /// </summary>
        /// <value>
        ///     If <c>true</c>, we can drag the annotation.
        /// </value>
        new public bool CanDrag
        {
            get { return (bool)GetValue(CanDragProperty); }
            set { SetValue(CanDragProperty, value); }
        }

#else
        /// <summary>
        /// Gets or sets a value indicating whether dragging is enabled for the annotation.
        /// </summary>
        /// <value>
        ///     If <c>true</c>, we can drag the annotation.
        /// </value>
        public bool CanDrag
        {
            get { return (bool)GetValue(CanDragProperty); }
            set { SetValue(CanDragProperty, value); }
        }

#endif

        /// <summary>
        /// Gets or sets a value indicating whether resizing is enabled for the annotation.
        /// </summary>
        /// <value>
        ///     If <c>true</c>, we can resize the annotation.
        /// </value>
        public bool CanResize
        {
            get { return (bool)GetValue(CanResizeProperty); }
            set { SetValue(CanResizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the vertical alignment of the text description in ShapeAnnotation.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.VerticalAlignment"/> property.
        /// </value>
        public VerticalAlignment VerticalTextAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalTextAlignmentProperty); }
            set { SetValue(VerticalTextAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the fill color of the ShapeAnnotation.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Y2 value for the ShapeAnnotation.
        /// </summary>
        public object Y2
        {
            get { return (object)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }

        /// <summary>
        /// Gets or sets the X2 value for the ShapeAnnotation.
        /// </summary>
        public object X2
        {
            get { return (object)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        /// <summary>
        /// Gets or sets the stroke thickness.
        /// </summary>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke for the annotation.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke dash array for the annotation stroke.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.Shapes.StrokeDashArray"/>.
        /// </value>
        public DoubleCollection StrokeDashArray
        {
            get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
            set { SetValue(StrokeDashArrayProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke dash cap for the stroke.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.Media.PenLineCap"/>.
        /// </value>
        public PenLineCap StrokeDashCap
        {
            get { return (PenLineCap)GetValue(StrokeDashCapProperty); }
            set { SetValue(StrokeDashCapProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke dash offset for the annotation.
        /// </summary>
        /// <value>
        /// The double value.
        /// </value>
        public double StrokeDashOffset
        {
            get { return (double)GetValue(StrokeDashOffsetProperty); }
            set { SetValue(StrokeDashOffsetProperty, value); }
        }

        /// <summary>
        /// Gets or sets the end line cap for the stroke.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.Media.PenLineCap"/>.
        /// </value>
        public PenLineCap StrokeEndLineCap
        {
            get { return (PenLineCap)GetValue(StrokeEndLineCapProperty); }
            set { SetValue(StrokeEndLineCapProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke line join for the stroke of the shape.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.Media.PenLineJoin"/>
        /// </value>
        public PenLineJoin StrokeLineJoin
        {
            get { return (PenLineJoin)GetValue(StrokeLineJoinProperty); }
            set { SetValue(StrokeLineJoinProperty, value); }
        }

        /// <summary>
        /// Gets or sets a limit on the ratio of the miter length to half the <see cref="StrokeThickness"/> of the shape.
        /// </summary>
        /// <value>
        /// See <see cref="System.Windows.Shapes.StrokeMiterLimit"/> property.
        /// </value>
        public double StrokeMiterLimit
        {
            get { return (double)GetValue(StrokeMiterLimitProperty); }
            set { SetValue(StrokeMiterLimitProperty, value); }
        }

        /// <summary>
        /// Gets or sets the start line cap for the stroke.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.Media.PenLineCap"/>.
        /// </value>
        public PenLineCap StrokeStartLineCap
        {
            get { return (PenLineCap)GetValue(StrokeStartLineCapProperty); }
            set { SetValue(StrokeStartLineCapProperty, value); }
        }

        #endregion

        #region Internal Properties

        internal bool IsDragging { get; set; }

        internal Resizer ResizerControl { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Updates the annotation.
        /// </summary>
        public override void UpdateAnnotation()
        {
            if ((shape != null || ResizerControl != null) && X1 != null && Y1 != null && X2 != null && Y2 != null)
            {
                switch (CoordinateUnit)
                {
                    case CoordinateUnit.Axis:
                        base.UpdateAnnotation();
                        if (XAxis != null && YAxis != null)
                        {
                            y2 = ConvertData(Y2, YAxis);
                            x2 = ConvertData(X2, XAxis);
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
                                    this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1)) 
                                : new Point(
                                    this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1),
                                    this.Chart.ValueToPointRelativeToAnnotation(XAxis, x1));
                            Point point2 = (XAxis.Orientation == Orientation.Horizontal) 
                                ? new Point(
                                    Chart.ValueToPointRelativeToAnnotation(XAxis, x2),
                                    this.Chart.ValueToPointRelativeToAnnotation(YAxis, y2)) 
                                : new Point(
                                    this.Chart.ValueToPointRelativeToAnnotation(YAxis, y2),
                                    Chart.ValueToPointRelativeToAnnotation(XAxis, x2));

                            UpdateAxisAnnotation(point, point2);
                        }
                        break;

                    case CoordinateUnit.Pixel:
                        Point elementPoint1 = new Point(Convert.ToDouble(X1), Convert.ToDouble(Y1));
                        Point elementPoint2 = new Point(Convert.ToDouble(X2), Convert.ToDouble(Y2));

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

        #endregion

        #region Internal Methods

        internal void HeightWidthChanged()
        {
            switch (CoordinateUnit)
            {
                case CoordinateUnit.Axis:
                    this.y2 = ConvertData(this.Y2, this.YAxis);
                    this.x2 = ConvertData(this.X2, this.XAxis);
                    break;
            }

            UpdateAnnotation();
            if (Chart != null && this.CoordinateUnit == CoordinateUnit.Axis && CanUpdateRange(this.X2, this.Y2))
                Chart.ScheduleUpdate();
        }

        /// <summary>
        /// Updates the axis annotation.
        /// </summary>
        /// <param name="point">The first point of annotation.</param>
        /// <param name="point2">The second point of annotation.</param>
        internal void UpdateAxisAnnotation(Point point, Point point2)
        {
            Point textPosition = new Point();
            Size desiredSize;
            Point centerPoint;
            Rect rotated;
            Rect heightAndWidthRect;
            double angle = (this as SolidShapeAnnotation).Angle;
            RotateTransform rotate = new RotateTransform { Angle = angle };
            Point positionedPoint = new Point(0, 0);
            Point ensurePoint;

            point.Y = (double.IsNaN(point.Y)) ? 0 : point.Y;
            point.X = (double.IsNaN(point.X)) ? 0 : point.X;
            point2.Y = (double.IsNaN(point2.Y)) ? 0 : point2.Y;
            point2.X = (double.IsNaN(point2.X)) ? 0 : point2.X;

            heightAndWidthRect = new Rect(point, point2);

            if (shape != null)
            {
                shape.Height = heightAndWidthRect.Height;
                shape.Width = heightAndWidthRect.Width;
            }
            else
            {
                ResizerControl.Height = heightAndWidthRect.Height;
                ResizerControl.Width = heightAndWidthRect.Width;
            }

            AnnotationElement.Height = heightAndWidthRect.Height;
            AnnotationElement.Width = heightAndWidthRect.Width;
            ensurePoint = this.EnsurePoint(point, point2);
            desiredSize = new Size(heightAndWidthRect.Width, heightAndWidthRect.Height);
            positionedPoint = GetElementPosition(
                new Size(heightAndWidthRect.Width, heightAndWidthRect.Height), ensurePoint);
            TextElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            textPosition = GetTextPosition(desiredSize, new Point(0, 0), new Size(TextElement.DesiredSize.Width, TextElement.DesiredSize.Height));
            Canvas.SetLeft(AnnotationElement, positionedPoint.X);
            Canvas.SetTop(AnnotationElement, positionedPoint.Y);
            Canvas.SetLeft(TextElement, textPosition.X);
            Canvas.SetTop(TextElement, textPosition.Y);
            centerPoint = new Point(
                positionedPoint.X + (AnnotationElement.Width / 2),
                positionedPoint.Y + (AnnotationElement.Height / 2));
            rotated = this.RotateElement(angle, AnnotationElement, new Size(this.AnnotationElement.Width, this.AnnotationElement.Height));
            AnnotationElement.RenderTransformOrigin = new Point(0.5, 0.5);
            AnnotationElement.RenderTransform = rotate;

            if (angle > 0)
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

        /// <summary>
        /// Updates the pixel annotation.
        /// </summary>
        /// <param name="elementPoint1">The first point of annotation.</param>
        /// <param name="elementPoint2">The second point of annotation.</param>
        internal void UpdatePixelAnnotation(Point elementPoint1, Point elementPoint2)
        {
            Point textPosition = new Point();
            Size desiredSize;
            Point centerPoint;
            Rect rotated;
            Rect heightAndWidthRect;
            double angle = (this as SolidShapeAnnotation).Angle;
            RotateTransform rotate = new RotateTransform { Angle = angle };
            Point positionedPoint = new Point(0, 0);
            Point ensurePoint;

            heightAndWidthRect = new Rect(elementPoint1, elementPoint2);

            if (shape != null)
            {
                shape.Height = heightAndWidthRect.Height;
                shape.Width = heightAndWidthRect.Width;
            }
            else
            {
                ResizerControl.Height = heightAndWidthRect.Height;
                ResizerControl.Width = heightAndWidthRect.Width;
            }

            AnnotationElement.Height = heightAndWidthRect.Height;
            AnnotationElement.Width = heightAndWidthRect.Width;
            ensurePoint = this.EnsurePoint(elementPoint1, elementPoint2);
            positionedPoint = shape != null ? GetElementPosition(shape, ensurePoint) : GetElementPosition(ResizerControl, ensurePoint); ;
            desiredSize = (shape != null) ? new Size(shape.Width, shape.Height) : new Size(ResizerControl.Width, ResizerControl.Height);
            TextElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            textPosition = GetTextPosition(
                desiredSize, 
                new Point(0, 0),
                new Size(
                    TextElement.DesiredSize.Width, 
                    TextElement.DesiredSize.Height));
            Canvas.SetLeft(AnnotationElement, positionedPoint.X);
            Canvas.SetTop(AnnotationElement, positionedPoint.Y);

            Canvas.SetLeft(TextElement, textPosition.X);
            Canvas.SetTop(TextElement, textPosition.Y);
            centerPoint = new Point(
                positionedPoint.X + (AnnotationElement.Width / 2),
                positionedPoint.Y + (AnnotationElement.Height / 2));
            rotated = this.RotateElement(angle, AnnotationElement, new Size(this.AnnotationElement.Width, this.AnnotationElement.Height));
            AnnotationElement.RenderTransformOrigin = new Point(0.5, 0.5);
            AnnotationElement.RenderTransform = rotate;

            if (angle > 0)
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

        /// <summary>
        /// Checks and updates the resizer values.
        /// </summary>
        internal void CheckResizerValues()
        {
            if (Chart != null && Chart.AnnotationManager != null && Chart.AnnotationManager.SelectedAnnotation == this && CanResize && shape != null && !IsDragging)
            {
                AnnotationResizer resizer = this.Chart.AnnotationManager.AnnotationResizer;
                if (resizer.X1 == null && resizer.Y1 == null && resizer.X2 == null && resizer.Y2 == null)
                {
                    resizer.X1 = this.X1;
                    resizer.X2 = this.X2;
                    resizer.Y1 = this.Y1;
                    resizer.Y2 = this.Y2;
                    resizer.MapActualValueToPixels();
                }
                resizer.InternalHorizontalAlignment = this.HorizontalAlignment;
                resizer.InternalVerticalAlignment = this.VerticalAlignment;
            }
        }

        /// <summary>
        /// Clears the annotation elements values.
        /// </summary>
        internal void ClearAnnotationElements()
        {
            AnnotationElement.ClearValue(Canvas.LeftProperty);
            AnnotationElement.ClearValue(Canvas.TopProperty);
            AnnotationElement.Width = 0;
            AnnotationElement.Height = 0;

            if (TextElement.Content != null)
            {
                TextElement.ClearValue(Canvas.LeftProperty);
                TextElement.ClearValue(Canvas.TopProperty);
            }
            if (Chart != null && Chart.AnnotationManager != null && Chart.AnnotationManager.SelectedAnnotation != null &&
                Chart.AnnotationManager.SelectedAnnotation.Equals(this) && CanResize)
            {
                if (X1 == null && Y1 == null && X2 == null && Y2 == null)
                {
                    AnnotationResizer resizer = this.Chart.AnnotationManager.AnnotationResizer;
                    resizer.ClearValue(AnnotationResizer.X1Property);
                    resizer.ClearValue(AnnotationResizer.X2Property);
                    resizer.ClearValue(AnnotationResizer.Y1Property);
                    resizer.ClearValue(AnnotationResizer.Y2Property);
                }
            }
        }

        #endregion

        #region Protected Internal Methods

        //
        // Summary:
        //     Invoked when an unhandled DragDelta routed
        //     event reaches an element in its route that is derived from this class. Implement
        //     this method to add class handling for this event.
        //
        // Parameters:
        //   args:
        //     The Syncfusion.UI.Xaml.Charts.AnnotationDragDeltaEventArgs that contains the event data.
        protected internal virtual void OnDragDelta(AnnotationDragDeltaEventArgs args)
        {
            if (DragDelta != null)
                DragDelta(this, args);
        }

        //
        // Summary:
        //     Invoked when an unhandled DragCompleted routed
        //     event reaches an element in its route that is derived from this class. Implement
        //     this method to add class handling for this event.
        //
        // Parameters:
        //   args:
        //     The Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs that contains the event data.
        protected internal virtual void OnDragCompleted(AnnotationDragCompletedEventArgs args)
        {
            if (DragCompleted != null)
                DragCompleted(this, args);
        }

        //
        // Summary:
        //     Invoked when an unhandled DragStarted routed
        //     event reaches an element in its route that is derived from this class. Implement
        //     this method to add class handling for this event.
        //
        // Parameters:
        //   args:
        //     The System.EventArgs that contains the event data.
        protected internal virtual void OnDragStarted(EventArgs args)
        {
            if (DragStarted != null)
                DragStarted(this, args);
        }

        #endregion

        #region Protected Override Methods

        protected override void SetBindings()
        {
            base.SetBindings();
            Binding fillBinding = new Binding { Source = this, Path = new PropertyPath("Fill") };
            shape.SetBinding(Shape.FillProperty, fillBinding);

            Binding opacityBinding = new Binding { Source = this, Path = new PropertyPath("Opacity") };
            shape.SetBinding(Shape.OpacityProperty, opacityBinding);

            Binding strokeBinding = new Binding { Source = this, Path = new PropertyPath("Stroke") };
            shape.SetBinding(Shape.StrokeProperty, strokeBinding);

            Binding strokeThicknessBinding = new Binding { Source = this, Path = new PropertyPath("StrokeThickness") };
            shape.SetBinding(Shape.StrokeThicknessProperty, strokeThicknessBinding);

            DoubleCollection strokeDashArray = this.StrokeDashArray;
            DoubleCollection strokeArrayCollection = new DoubleCollection();
            if (strokeDashArray != null && strokeDashArray.Count > 0)
            {
                foreach (double value in strokeDashArray)
                    strokeArrayCollection.Add(value);
                shape.StrokeDashArray = strokeArrayCollection;
            }

            Binding strokeDashCapBinding = new Binding { Source = this, Path = new PropertyPath("StrokeDashCap") };
            shape.SetBinding(Shape.StrokeDashCapProperty, strokeDashCapBinding);

            Binding strokeDashOffsetBinding = new Binding { Source = this, Path = new PropertyPath("StrokeDashOffset") };
            shape.SetBinding(Shape.StrokeDashOffsetProperty, strokeDashOffsetBinding);

            Binding strokeEndLineCapBinding = new Binding { Source = this, Path = new PropertyPath("StrokeEndLineCap") };
            shape.SetBinding(Shape.StrokeEndLineCapProperty, strokeEndLineCapBinding);

            Binding strokeLineJoinBinding = new Binding { Source = this, Path = new PropertyPath("StrokeLineJoin") };
            shape.SetBinding(Shape.StrokeLineJoinProperty, strokeLineJoinBinding);

            Binding strokeMiterBinding = new Binding { Source = this, Path = new PropertyPath("StrokeMiterLimit") };
            shape.SetBinding(Shape.StrokeMiterLimitProperty, strokeMiterBinding);

            Binding strokeStartLineCapBinding = new Binding { Source = this, Path = new PropertyPath("StrokeStartLineCap") };
            shape.SetBinding(Shape.StrokeStartLineCapProperty, strokeStartLineCapBinding);
        }

        protected override DependencyObject CloneAnnotation(Annotation annotation)
        {
            ShapeAnnotation shapeAnnotation = annotation as ShapeAnnotation;

            var solidShapeAnnotation = shapeAnnotation as SolidShapeAnnotation;
            if (solidShapeAnnotation != null)
                solidShapeAnnotation.Angle = (this as SolidShapeAnnotation).Angle;
            shapeAnnotation.Fill = this.Fill;
            shapeAnnotation.HorizontalTextAlignment = this.HorizontalTextAlignment;
            shapeAnnotation.Stroke = this.Stroke;
            shapeAnnotation.StrokeDashArray = this.StrokeDashArray;
            shapeAnnotation.StrokeDashCap = this.StrokeDashCap;
            shapeAnnotation.StrokeDashOffset = this.StrokeDashOffset;
            shapeAnnotation.StrokeEndLineCap = this.StrokeEndLineCap;
            shapeAnnotation.StrokeLineJoin = this.StrokeLineJoin;
            shapeAnnotation.StrokeMiterLimit = this.StrokeMiterLimit;
            shapeAnnotation.StrokeStartLineCap = this.StrokeStartLineCap;
            shapeAnnotation.StrokeThickness = this.StrokeThickness;
            shapeAnnotation.VerticalTextAlignment = this.VerticalTextAlignment;
            shapeAnnotation.X2 = this.X2;
            shapeAnnotation.Y2 = this.Y2;
            shapeAnnotation.CanDrag = this.CanDrag;
            shapeAnnotation.CanResize = this.CanResize;
            return base.CloneAnnotation(shapeAnnotation);
        }

        #endregion

        #region Protected Virtual Methods

        protected virtual Point GetTextPosition(Size desiredSize, Point originalPosition, Size textSize)
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

        private static void OnCanResizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var shapeAnnotation = d as ShapeAnnotation;
            if (e.NewValue != e.OldValue && shapeAnnotation != null && shapeAnnotation.Chart != null)
                shapeAnnotation.UpdateResizer((bool)e.NewValue);
        }

        private static void OnAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Annotation.OnTextAlignmentChanged(d, e);
        }

        private static void OnHeightWidthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue != null && args.OldValue.Equals(args.NewValue))
                return;

            var shapeAnnotation = sender as ShapeAnnotation;
            if (shapeAnnotation != null &&
                (shapeAnnotation.XAxis != null || shapeAnnotation.CoordinateUnit == CoordinateUnit.Pixel) &&
                shapeAnnotation.YAxis != null)
            {
                if (shapeAnnotation is AnnotationResizer && args.OldValue != null)
                {
                    (shapeAnnotation.Chart.AnnotationManager.SelectedAnnotation as ShapeAnnotation).X2 = shapeAnnotation.X2;
                    (shapeAnnotation.Chart.AnnotationManager.SelectedAnnotation as ShapeAnnotation).Y2 = shapeAnnotation.Y2;
                }

                shapeAnnotation.HeightWidthChanged();
            }
        }

        #endregion

        #region Private Methods

        private void UpdateResizer(bool value)
        {
            if (value && Chart.AnnotationManager.SelectedAnnotation != null)
            {
                Chart.AnnotationManager.OnAnnotationSelected();
            }
            else
            {
                if (Chart.AnnotationManager.AnnotationResizer != null)
                {
                    Chart.AnnotationManager.AddOrRemoveAnnotationResizer(Chart.AnnotationManager.AnnotationResizer, true);
                    Chart.AnnotationManager.AnnotationResizer = null;
                }
                else if (Chart.AnnotationManager.SelectedAnnotation is LineAnnotation)
                {
                    Chart.AnnotationManager.HideLineResizer();
                }
            }
        }

        #endregion

        #endregion
    }
}
