// <copyright file="LineAnnotation.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;

    /// <summary>
    /// Represents the <see cref="LineAnnotation"/> class.
    /// </summary>
    public partial class LineAnnotation : ShapeAnnotation
    {
        #region Dependency Property Registration        

        // Using a DependencyProperty as the backing store for GrabExtent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GrabExtentProperty =
            DependencyProperty.Register(
                "GrabExtent",
                typeof(double),
                typeof(LineAnnotation),
                new PropertyMetadata(5d, OnGrabExtentChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ShowLine"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowLineProperty =
            DependencyProperty.Register(
                "ShowLine",
                typeof(bool),
                typeof(LineAnnotation),
                new PropertyMetadata(true, OnShowLinePropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="LineCap"/> property.
        /// </summary>
        public static readonly DependencyProperty LineAnnotionCapProperty =
            DependencyProperty.Register(
                "LineCap",
                typeof(LineCap),
                typeof(LineAnnotation),
                new PropertyMetadata(LineCap.None));

        #endregion

        #region Fields

        protected ArrowLine arrowLine;
        private double minimumSize = 0d;
        private AnnotationManager annotationManager;
        private bool isRotated;
        private RotateTransform rotate;
        private Point[] hitRectPoints;

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the grab extent for the dragging line annotation.
        /// </summary>        
        public double GrabExtent
        {
            get { return (double)GetValue(GrabExtentProperty); }
            set { SetValue(GrabExtentProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the line or not.
        /// </summary>
        /// <value>
        ///  if <c>true</c>, Line will be visible.
        /// </value>
        public bool ShowLine
        {
            get { return (bool)GetValue(ShowLineProperty); }
            set { SetValue(ShowLineProperty, value); }
        }

        /// <summary>
        /// Gets or sets the line cap for the <c>LineAnnotation</c>.
        /// </summary>
        /// <value>
        ///     <see cref="Syncfusion.UI.Xaml.Charts.LineCap"/>
        /// </value>
        public LineCap LineCap
        {
            get { return (LineCap)GetValue(LineAnnotionCapProperty); }
            set { SetValue(LineAnnotionCapProperty, value); }
        }

        #endregion

        #region Internal Properties

        internal Canvas LineCanvas { get; set; }

        internal Point[] LinePoints { get; set; }

        #endregion

        #region Protected Internal Properties

        /// <summary>
        /// Gets or sets the horizontal change.
        /// </summary>
        protected internal double HorizontalChange { get; set; }

        /// <summary>
        /// Gets or sets the vertical change.
        /// </summary>
        protected internal double VerticalChange { get; set; }

        #endregion

        #region Private Properties

        private bool IsWithCap
        {
            get
            {
                return (LineCap == LineCap.Arrow);
            }
        }

        private bool IsAxis
        {
            get
            {
                return (CoordinateUnit == Charts.CoordinateUnit.Axis);
            }
        }

        private bool IsVertical
        {
            get
            {
                return !(this is HorizontalLineAnnotation);
            }
        }

        private bool IsHorizontal
        {
            get
            {
                return !(this is VerticalLineAnnotation);
            }
        }

        private double ActualX1
        {
            get
            {
                if (IsAxis)
                    return Chart.ValueToPointRelativeToAnnotation(XAxis, this.x1);
                return Convert.ToDouble(X1);
            }

            set
            {
                X1 = value;
            }
        }

        private double ActualX2
        {
            get
            {
                if (IsAxis)
                    return Chart.ValueToPointRelativeToAnnotation(XAxis, this.x2);
                return Convert.ToDouble(X2);
            }

            set
            {
                X2 = value;
            }
        }

        private double ActualY1
        {
            get
            {
                if (IsAxis)
                    return Chart.ValueToPointRelativeToAnnotation(YAxis, this.y1);
                return Convert.ToDouble(Y1);
            }

            set
            {
                Y1 = value;
            }
        }

        private double ActualY2
        {
            get
            {
                if (IsAxis)
                    return Chart.ValueToPointRelativeToAnnotation(YAxis, this.y2);
                return Convert.ToDouble(Y2);
            }

            set
            {
                Y2 = value;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Updates the annotation.
        /// </summary>
        public override void UpdateAnnotation()
        {
            if (shape != null && X1 != null && X2 != null && Y1 != null && Y2 != null)
            {
                ValidateSelection();
                switch (CoordinateUnit)
                {
                    case CoordinateUnit.Axis:
                        SetAxisFromName();
                        if (XAxis != null && YAxis != null && ShowLine)
                        {
                            x1 = Annotation.ConvertData(X1, XAxis);
                            y1 = Annotation.ConvertData(Y1, YAxis);
                            y2 = Annotation.ConvertData(Y2, YAxis);
                            x2 = Annotation.ConvertData(X2, XAxis);
                            if (CoordinateUnit == CoordinateUnit.Axis && EnableClipping)
                            {
                                x1 = GetClippingValues(x1, XAxis);
                                y1 = GetClippingValues(y1, YAxis);
                                x2 = GetClippingValues(x2, XAxis);
                                y2 = GetClippingValues(y2, YAxis);
                            }

                            Point point = (XAxis.Orientation == Orientation.Horizontal) ?
                                new Point(
                                    this.Chart.ValueToPointRelativeToAnnotation(XAxis, x1),
                                    this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1))
                              : new Point(
                                    this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1),
                                    this.Chart.ValueToPointRelativeToAnnotation(XAxis, x1));
                            Point point2 = (XAxis.Orientation == Orientation.Horizontal) ?
                                new Point(
                                this.Chart.ValueToPointRelativeToAnnotation(XAxis, x2),
                                this.Chart.ValueToPointRelativeToAnnotation(YAxis, y2))
                              : new Point(
                                this.Chart.ValueToPointRelativeToAnnotation(YAxis, y2),
                                this.Chart.ValueToPointRelativeToAnnotation(XAxis, x2));
                            DrawLine(point, point2, shape);
                        }

                        break;
                    case CoordinateUnit.Pixel:
                        if (ShowLine)
                        {
                            Point elementPoint1 = new Point(Convert.ToDouble(X1), Convert.ToDouble(Y1));
                            Point elementPoint2 = new Point(Convert.ToDouble(X2), Convert.ToDouble(Y2));
                            DrawLine(elementPoint1, elementPoint2, shape);
                        }

                        break;
                }
            }
            else if (shape != null)
                ClearValues();
        }

        #endregion

        #region Internal Methods

        internal override UIElement CreateAnnotation()
        {
            if (AnnotationElement != null && AnnotationElement.Children.Count == 0)
            {
                LineCanvas = new Canvas();
                if (IsWithCap)
                {
                    shape = new Path();
                    arrowLine = new ArrowLine();
                }
                else
                    shape = new Line();
                shape.Tag = this;
                if (ShowLine)
                {
                    if (CanResize)
                        AddThumb();
                    LineCanvas.Children.Add(TextElement);
                    LineCanvas.Children.Add(shape);
                }

                SetBindings();
                AnnotationElement.Children.Add(LineCanvas);
            }

            return AnnotationElement;
        }

        internal void AddThumb()
        {
            nearThumb = new Thumb();
            farThumb = new Thumb();
            nearThumb.Style = ChartDictionaries.GenericCommonDictionary["roundthumbstyle"] as Style;
            farThumb.Style = ChartDictionaries.GenericCommonDictionary["roundthumbstyle"] as Style;
            nearThumb.Visibility = Visibility.Collapsed;
            farThumb.Visibility = Visibility.Collapsed;
            nearThumb.Margin = new Thickness(-10);
            farThumb.Margin = new Thickness(-10);

            LineCanvas.Children.Add(nearThumb);
            LineCanvas.Children.Add(farThumb);
            nearThumb.DragDelta += OnNearThumbDragDelta;
            farThumb.DragDelta += OnFarThumbDragDelta;
            Canvas.SetZIndex(nearThumb, 1);
            Canvas.SetZIndex(farThumb, 1);

            nearThumb.DragCompleted += OnDragCompleted;
            farThumb.DragCompleted += OnDragCompleted;
        }

        internal void ValidateSelection()
        {
            if (CanResize && Chart != null && Chart.AnnotationManager != null &&
                    Chart.AnnotationManager.SelectedAnnotation != null &&
                    Chart.AnnotationManager.SelectedAnnotation.Equals(this))
            {
                nearThumb.Visibility = Visibility.Visible;
                farThumb.Visibility = Visibility.Visible;
            }
        }

        internal void ClearValues()
        {
            if (ShowLine)
                shape.ClearUIValues();
            if (TextElement.Content != null)
            {
                TextElement.ClearValue(Canvas.LeftProperty);
                TextElement.ClearValue(Canvas.TopProperty);
            }

            if (CanResize)
            {
                nearThumb.Visibility = Visibility.Collapsed;
                farThumb.Visibility = Visibility.Collapsed;
            }
        }

        internal virtual void UpdateHitArea()
        {
            if (LinePoints != null)
                hitRectPoints = GetHitRectPoints(LinePoints[0], LinePoints[1]);
        }

        internal virtual bool IsPointInsideRectangle(Point point)
        {
            if (hitRectPoints != null)
                return ChartMath.IsPointInsideRectangle(point, hitRectPoints);
            else
                return false;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Updates the drag completed interactions.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The drag completed event arguments.</param>
        protected virtual void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            Chart.AnnotationManager.RaiseDragCompleted();
        }

        /// <summary>
        /// Clones the annotation.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        /// <returns>Returns the cloned annotation.</returns>
        protected override DependencyObject CloneAnnotation(Annotation annotation)
        {
            return base.CloneAnnotation(new LineAnnotation() { ShowLine = this.ShowLine, LineCap = this.LineCap, });
        }

        /// <summary>
        /// Draws the line.
        /// </summary>
        /// <param name="point">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <param name="shape">The shape to be used.</param>
        protected void DrawLine(Point point, Point point2, Shape shape)
        {
            Size desiredSize;
            Point ensurePoint;
            Rect heightAndWidthRect;
            Point positionedPoint = new Point(0, 0);
            Point centerPoint = new Point(0, 0);
            double height = 0, width = 0;
            Path path = null;
            Line line = null;
            if (IsWithCap)
                path = shape as Path;
            else
                line = shape as Line;

            point.Y = (double.IsNaN(point.Y)) ? 0 : point.Y;
            point.X = (double.IsNaN(point.X)) ? 0 : point.X;
            point2.Y = (double.IsNaN(point2.Y)) ? 0 : point2.Y;
            point2.X = (double.IsNaN(point2.X)) ? 0 : point2.X;
            heightAndWidthRect = new Rect(point, point2);
            ensurePoint = this.EnsurePoint(point, point2);
            desiredSize = new Size(heightAndWidthRect.Width, heightAndWidthRect.Height);
            positionedPoint = GetElementPosition(new Size(heightAndWidthRect.Width, heightAndWidthRect.Height), ensurePoint);
            if (IsWithCap)
            {
                arrowLine.X1 = point.X;
                arrowLine.Y1 = point.Y;
                arrowLine.X2 = point2.X;
                arrowLine.Y2 = point2.Y;
                path.Data = arrowLine.GetGeometry() as PathGeometry;
            }
            else
            {
                line.X1 = point.X;
                line.Y1 = point.Y;
                line.X2 = point2.X;
                line.Y2 = point2.Y;
            }

            if (CanResize && farThumb != null && nearThumb != null)
            {
                Canvas.SetLeft(farThumb, point.X);
                Canvas.SetTop(farThumb, point.Y);
                Canvas.SetLeft(nearThumb, point2.X);
                Canvas.SetTop(nearThumb, point2.Y);
            }

            AnnotationElement.Width = heightAndWidthRect.Width;
            AnnotationElement.Height = heightAndWidthRect.Height;
            centerPoint = new Point(
                positionedPoint.X + (heightAndWidthRect.Width / 2),
                positionedPoint.Y + (heightAndWidthRect.Height / 2));
            minimumSize = GrabExtent * 2;
            height = heightAndWidthRect.Height < minimumSize ? minimumSize : heightAndWidthRect.Height;
            width = heightAndWidthRect.Width < minimumSize ? minimumSize : heightAndWidthRect.Width;

            RotatedRect = new Rect(centerPoint.X - (width / 2), centerPoint.Y - (height / 2), width, height);
            LinePoints = new Point[2] { point, point2 };
            if (!(this is HorizontalLineAnnotation || this is VerticalLineAnnotation))
                UpdateHitArea();

            if (TextElement.Content != null)
                SetTextElementPosition(point, point2, desiredSize, positionedPoint, TextElement);

            // This is to avoid line visibility collapse while dragging.
            if (LineCanvas != null && IsDragging)
                LineCanvas.UpdateLayout();
        }

        /// <summary>
        /// Sets the text element position.
        /// </summary>
        /// <param name="point">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <param name="desiredSize">The desired size.</param>
        /// <param name="positionedPoint">The position point.</param>
        /// <param name="textElement">The text element.</param>
        protected virtual void SetTextElementPosition(Point point, Point point2, Size desiredSize, Point positionedPoint, ContentControl textElement)
        {
            double slope;
            rotate = new RotateTransform();
            textElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            slope = (point2.Y - point.Y) / (point2.X - point.X);
            rotate.Angle = double.IsNaN(slope) ? 0 : Math.Atan(slope) * (180 / Math.PI);
            textElement.RenderTransformOrigin = new Point(0.5, 0.5);
            textElement.RenderTransform = rotate;
            Point textPosition = GetTextPosition(desiredSize, positionedPoint, new Size(textElement.DesiredSize.Width, textElement.DesiredSize.Height));
            Canvas.SetLeft(textElement, textPosition.X);
            Canvas.SetTop(textElement, textPosition.Y);
        }

        /// <summary>
        /// Gets the text position.
        /// </summary>
        /// <param name="desiredSize">The desired size.</param>
        /// <param name="originalPosition">The original position.</param>
        /// <param name="textSize">The text size.</param>
        /// <returns>Returns the text position after the alignment.</returns>
        protected override Point GetTextPosition(Size desiredSize, Point originalPosition, Size textSize)
        {
            Point point = originalPosition;
            HorizontalAlignment horizontalAlignment = this.HorizontalTextAlignment;
            VerticalAlignment verticalAlignment = this.VerticalTextAlignment;
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    point.X += (desiredSize.Width / 2);
                    if (Math.Abs(rotate.Angle) < 80)
                        point.X -= (textSize.Width / 2);
                    break;
                case HorizontalAlignment.Left:
                    break;
                case HorizontalAlignment.Right:
                    point.X += desiredSize.Width - textSize.Width;
                    break;
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Center:
                    point.Y += (desiredSize.Height / 2);
                    point.Y -= (textSize.Height / 2);
                    break;
                case VerticalAlignment.Bottom:
                    point.Y += (desiredSize.Height / 2);
                    point.Y -= (textSize.Height / 8);
                    TextElement.Margin = new Thickness(0, minimumSize, 0, 0);
                    break;
                case VerticalAlignment.Top:
                    point.Y += (desiredSize.Height / 2);
                    point.Y -= (textSize.Height);
                    TextElement.Margin = new Thickness(0, 0, 0, minimumSize);
                    break;
            }

            return point;
        }

        /// <summary>
        /// Sets the binding between the annotation and <see cref="UIElement"/>.
        /// </summary>
        protected override void SetBindings()
        {
            base.SetBindings();
            if (this.LineCap == Charts.LineCap.Arrow)
            {
                Binding strokeBinding = new Binding { Source = this, Path = new PropertyPath("Stroke") };
                shape.SetBinding(Path.FillProperty, strokeBinding);
            }
        }

        #endregion

        #region Private Static Methods

        private static void OnShowLinePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var annotation = d as LineAnnotation;
            if ((bool)e.NewValue)
                annotation.AddLine();
            else
                annotation.RemoveLine();
        }

        private static void OnGrabExtentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lineAnnotation = d as LineAnnotation;
            if (lineAnnotation.LinePoints != null)
                lineAnnotation.UpdateHitArea();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Add the line into Canvas
        /// </summary>
        private void AddLine()
        {
            if (LineCanvas != null && LineCanvas.Children != null && !LineCanvas.Children.Contains(TextElement) && !LineCanvas.Children.Contains(shape))
            {
                shape.Tag = this;
                if (CanResize)
                    AddThumb();
                LineCanvas.Children.Add(TextElement);
                LineCanvas.Children.Add(shape);
                UpdateAnnotation();
            }
        }

        /// <summary>
        /// Remove the line from Canvas
        /// </summary>
        private void RemoveLine()
        {
            if (LineCanvas != null && LineCanvas.Children != null && LineCanvas.Children.Contains(TextElement) && LineCanvas.Children.Contains(shape))
            {
                if (Chart != null && CanResize && Chart.AnnotationManager.SelectedAnnotation != null)
                {
                    nearThumb.DragDelta -= OnNearThumbDragDelta;
                    farThumb.DragDelta -= OnFarThumbDragDelta;
                    nearThumb.DragCompleted -= OnDragCompleted;
                    farThumb.DragCompleted -= OnDragCompleted;
                    Chart.AnnotationManager.SelectedAnnotation = null;
                    nearThumb = null;
                    farThumb = null;
                }

                LineCanvas.Children.Clear();
            }
        }

        private void OnFarThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            this.IsResizing = true;
            annotationManager = Chart.AnnotationManager;
            double x1 = 0, y1 = 0;
            isRotated = (XAxis != null && XAxis.Orientation == Orientation.Vertical && IsAxis);
            HorizontalChange = isRotated ? e.VerticalChange : e.HorizontalChange;
            VerticalChange = isRotated ? e.HorizontalChange : e.VerticalChange;

            ShapeAnnotation annotation = annotationManager.CurrentAnnotation as ShapeAnnotation;

            AnnotationManager.SetPosition(
                annotationManager.PreviousPoints,
                annotation.X1,
                annotation.X2,
                annotation.Y1,
                annotation.Y2);

            if (!isRotated)
            {
                if (IsHorizontal)
                {
                    x1 = IsAxis ? SfChart.PointToAnnotationValue(XAxis, new Point(ActualX1 + HorizontalChange, 0)) : ActualX1 + HorizontalChange;

                    AnnotationManager.SetPosition(annotationManager.CurrentPoints, x1, annotation.X2, annotation.Y1, annotation.Y2);
                }

                if (IsVertical)
                {
                    y1 = IsAxis ? SfChart.PointToAnnotationValue(YAxis, new Point(0, ActualY1 + VerticalChange)) : ActualY1 + VerticalChange;

                    AnnotationManager.SetPosition(annotationManager.CurrentPoints, annotation.X1, annotation.X2, y1, annotation.Y2);
                }
            }
            else
            {
                if (IsVertical)
                {
                    x1 = IsAxis ? SfChart.PointToAnnotationValue(XAxis, new Point(0, ActualX1 + HorizontalChange)) : ActualX1 + HorizontalChange;

                    AnnotationManager.SetPosition(annotationManager.CurrentPoints, x1, annotation.X2, annotation.Y1, annotation.Y2);
                }

                if (IsHorizontal)
                {
                    y1 = IsAxis ? SfChart.PointToAnnotationValue(YAxis, new Point(ActualY1 + VerticalChange, 0)) : ActualY1 + VerticalChange;

                    AnnotationManager.SetPosition(annotationManager.CurrentPoints, annotation.X1, annotation.X2, y1, annotation.Y2);
                }
            }

            annotationManager.RaiseDragStarted(); // Raise the DragStarted event
            annotationManager.RaiseDragDelta(); // Raise the DragDelta event

            if (!annotationManager.DragDeltaArgs.Cancel)
            {
                if ((!isRotated && IsHorizontal) || (isRotated && IsVertical))
                    ActualX1 = x1;
                if ((!isRotated && IsVertical) || (isRotated && IsHorizontal))
                    ActualY1 = y1;
            }
        }

        private void OnNearThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            this.IsResizing = true;
            annotationManager = Chart.AnnotationManager;
            double x2 = 0, y2 = 0;
            isRotated = (XAxis != null && XAxis.Orientation == Orientation.Vertical && IsAxis);
            HorizontalChange = isRotated ? e.VerticalChange : e.HorizontalChange;
            VerticalChange = isRotated ? e.HorizontalChange : e.VerticalChange;

            ShapeAnnotation annotation = annotationManager.CurrentAnnotation as ShapeAnnotation;

            AnnotationManager.SetPosition(
                annotationManager.PreviousPoints,
                annotation.X1,
                annotation.X2,
                annotation.Y1,
                annotation.Y2);

            if (!isRotated)
            {
                if (IsHorizontal)
                {
                    x2 = IsAxis ? SfChart.PointToAnnotationValue(XAxis, new Point(ActualX2 + HorizontalChange, 0)) : ActualX2 + HorizontalChange;

                    AnnotationManager.SetPosition(
                        annotationManager.CurrentPoints,
                        annotation.X1,
                        x2,
                        annotation.Y1,
                        annotation.Y2);
                }

                if (IsVertical)
                {
                    y2 = IsAxis ? SfChart.PointToAnnotationValue(YAxis, new Point(0, ActualY2 + VerticalChange)) : ActualY2 + VerticalChange;

                    AnnotationManager.SetPosition(
                        annotationManager.CurrentPoints,
                        annotation.X1,
                        annotation.X2,
                        annotation.Y1,
                        y2);
                }
            }
            else
            {
                if (IsVertical)
                {
                    x2 = IsAxis ? SfChart.PointToAnnotationValue(XAxis, new Point(0, ActualX2 + HorizontalChange)) : ActualX2 + HorizontalChange;

                    AnnotationManager.SetPosition(
                        annotationManager.CurrentPoints,
                        annotation.X1,
                        x2,
                        annotation.Y1,
                        annotation.Y2);
                }

                if (IsHorizontal)
                {
                    y2 = IsAxis ? SfChart.PointToAnnotationValue(YAxis, new Point(ActualY2 + VerticalChange, 0)) : ActualY2 + VerticalChange;

                    AnnotationManager.SetPosition(
                        annotationManager.CurrentPoints,
                        annotation.X1,
                        annotation.X2,
                        annotation.Y1,
                        y2);
                }
            }

            annotationManager.RaiseDragStarted(); // Raise the DragStarted event
            annotationManager.RaiseDragDelta(); // Raise the DragDelta event

            if (!annotationManager.DragDeltaArgs.Cancel)
            {
                if ((!isRotated && IsHorizontal) || (isRotated && IsVertical))
                    ActualX2 = x2;
                if ((!isRotated && IsVertical) || (isRotated && IsHorizontal))
                    ActualY2 = y2;
            }
        }

        private Point[] GetHitRectPoints(Point point1, Point point2)
        {
            var extendedPoints = new Point[4];
            extendedPoints[0] = ChartMath.CalculatePerpendicularDistantPoint(point1, point2, -GrabExtent);
            extendedPoints[1] = ChartMath.CalculatePerpendicularDistantPoint(point2, point1, GrabExtent);
            extendedPoints[2] = ChartMath.CalculatePerpendicularDistantPoint(point2, point1, -GrabExtent);
            extendedPoints[3] = ChartMath.CalculatePerpendicularDistantPoint(point1, point2, GrabExtent);

            return extendedPoints;
        }

        #endregion

        #endregion                
    }
}
