// <copyright file="AnnotationManager.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;

    /// <summary>
    /// Represents a dependency object for calculation.  
    /// </summary>
    /// <seealso cref="System.Windows.DependencyObject" />
    internal partial class AnnotationManager : DependencyObject
    {
        #region Fields

        private Annotation previouseSelectedAnnotation;
        private DataTemplate contentTemplate = null;
        private double toolTipDuration = 0;
        private DataTemplate defaultToolTipTemplate;
        private Annotation selectedAnnotation;
        private DispatcherTimer timer;
        private AnnotationDragCompletedEventArgs dragCompletedArgs = new AnnotationDragCompletedEventArgs();
        private Annotation previousAnnotation;
        private AnnotationCollection annotations;
        private SfChart chart;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnotationManager"/> class.
        /// </summary>
        public AnnotationManager()
        {
            PreviousPoints = new Position();
            CurrentPoints = new Position();
            DragDeltaArgs = new AnnotationDragDeltaEventArgs();
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the Selected Annotation
        /// </summary>
        public Annotation SelectedAnnotation
        {
            get
            {
                return selectedAnnotation;
            }

            set
            {
                if (selectedAnnotation != value)
                {
                    selectedAnnotation = value;
                    OnSelectionChanged();
                }
            }
        }

        #endregion

        #region Internal Properties

        internal AnnotationCollection Annotations
        {
            get
            {
                return annotations;
            }

            set
            {
                if (annotations != value)
                {
                    if (annotations != null)
                    {
                        annotations.CollectionChanged -= OnAnnotationsCollectionChanged;
                        SelectedAnnotation = null;
                        foreach (var oldAnnotation in annotations)
                        {
                            AddOrRemoveAnnotations(oldAnnotation, true);
                        }
                    }

                    annotations = value;
                    if (annotations != null)
                    {
                        annotations.CollectionChanged += OnAnnotationsCollectionChanged;
                        AddAnnotations();
                    }
                }

                if (chart != null)
                {
                    foreach (ChartAxis axis in chart.Axes)
                    {
                        axis.AxisBoundsChanged -= OnAxisBoundsChanged;
                        axis.VisibleRangeChanged -= OnAxisVisibleRangeChanged;
                    }

                    foreach (ChartAxis axis in chart.Axes)
                    {
                        axis.AxisBoundsChanged += OnAxisBoundsChanged;
                        axis.VisibleRangeChanged += OnAxisVisibleRangeChanged;
                    }
                }
            }
        }

        internal SfChart Chart
        {
            get
            {
                return chart;
            }

            set
            {
                if (chart != null)
                {
                    chart.SeriesBoundsChanged -= OnSeriesBoundsChanged;
                    chart.SizeChanged -= OnChartSizeChanged;
                    chart.Axes.CollectionChanged -= OnAxesCollectionChanged;
                    if (this.chart.ChartAnnotationCanvas.Children.Contains(Tooltip))
                        this.chart.ChartAnnotationCanvas.Children.Remove(Tooltip);

                    chart.PointerPressed -= OnPointerPressed;
                    chart.ManipulationDelta -= OnManipulationDelta;
                    chart.ManipulationStarting -= ManipulationStarting;
                    chart.ManipulationCompleted -= ManipulationCompleted;
                    chart.PointerReleased -= OnPointerReleased;
                }

                chart = value;
                if (chart != null)
                {
                    Tooltip = new ChartTooltip();
                    Tooltip.IsHitTestVisible = false;
                    this.chart.ChartAnnotationCanvas.Children.Add(Tooltip);
                    chart.SeriesBoundsChanged += OnSeriesBoundsChanged;
                    chart.SizeChanged += OnChartSizeChanged;
                    chart.Axes.CollectionChanged += OnAxesCollectionChanged;
                    chart.PointerMoved += OnPointerMoved;
                    chart.PointerPressed += OnPointerPressed;
                    chart.ManipulationDelta += OnManipulationDelta;
                    chart.ManipulationStarting += ManipulationStarting;
                    chart.ManipulationCompleted += ManipulationCompleted;
                    chart.PointerReleased += OnPointerReleased;
                }
            }
        }

        internal TextBox TextBox { get; set; }

        internal Annotation TextAnnotation { get; set; }

        internal bool IsEditing { get; set; }

        internal ContentControl EditAnnotation { get; set; }

        internal ChartTooltip Tooltip { get; set; }

        internal bool IsDragStarted { get; set; }

        internal Annotation CurrentAnnotation { get; set; }

        internal AnnotationResizer AnnotationResizer { get; set; }

        internal Position PreviousPoints { get; set; }

        internal Position CurrentPoints { get; set; }

        internal AnnotationDragDeltaEventArgs DragDeltaArgs { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Internal Static Methods

        /// <summary>
        /// Method used to set annotation position
        /// </summary>
        /// <param name="position">The Position</param>
        /// <param name="x1">The x 1 value.</param>
        /// <param name="x2">The x 2 value.</param>
        /// <param name="y1">The y 1 value.</param>
        /// <param name="y2">The y 2 value.</param>
        internal static void SetPosition(Position position, object x1, object x2, object y1, object y2)
        {
            if (position != null)
            {
                position.X1 = x1 != null ? x1 : 0;
                position.X2 = x2 != null ? x2 : 0;
                position.Y1 = y1 != null ? y1 : 0;
                position.Y2 = y2 != null ? y2 : 0;
            }
        }

        #endregion

        #region Internal Methods

        internal void HideLineResizer()
        {
            (previouseSelectedAnnotation as LineAnnotation).nearThumb.Visibility = Visibility.Collapsed;
            (previouseSelectedAnnotation as LineAnnotation).farThumb.Visibility = Visibility.Collapsed;
        }

        internal void OnAnnotationSelected()
        {
            if ((SelectedAnnotation is SolidShapeAnnotation && (SelectedAnnotation as ShapeAnnotation).CanResize))
            {
                AnnotationResizer = new AnnotationResizer();
                AnnotationResizer.X1 = SelectedAnnotation.X1;
                AnnotationResizer.Y1 = SelectedAnnotation.Y1;
                AnnotationResizer.X2 = (SelectedAnnotation as ShapeAnnotation).X2;
                AnnotationResizer.Y2 = (SelectedAnnotation as ShapeAnnotation).Y2;
                AnnotationResizer.XAxisName = SelectedAnnotation.XAxisName;
                AnnotationResizer.YAxisName = SelectedAnnotation.YAxisName;
                AnnotationResizer.XAxis = selectedAnnotation.XAxis;
                AnnotationResizer.YAxis = selectedAnnotation.YAxis;
                AnnotationResizer.CoordinateUnit = SelectedAnnotation.CoordinateUnit;
                AnnotationResizer.Angle = (SelectedAnnotation as SolidShapeAnnotation).Angle;
                AnnotationResizer.InternalHorizontalAlignment = SelectedAnnotation.InternalHorizontalAlignment;
                AnnotationResizer.InternalVerticalAlignment = SelectedAnnotation.InternalVerticalAlignment;

                if (SelectedAnnotation is SolidShapeAnnotation)
                    AnnotationResizer.ResizingMode = (SelectedAnnotation as SolidShapeAnnotation).ResizingMode;
                AddOrRemoveAnnotationResizer(AnnotationResizer, false);
            }
            else if ((SelectedAnnotation is LineAnnotation && (SelectedAnnotation as LineAnnotation).CanResize))
            {
                if ((SelectedAnnotation as LineAnnotation).nearThumb == null)
                    (selectedAnnotation as LineAnnotation).AddThumb();
                (SelectedAnnotation as LineAnnotation).nearThumb.Visibility = Visibility.Visible;
                (SelectedAnnotation as LineAnnotation).farThumb.Visibility = Visibility.Visible;
                (SelectedAnnotation as LineAnnotation).UpdateAnnotation();
            }
        }

        internal void OnTextEditing()
        {
            if (IsEditing)
            {
                contentTemplate = ChartDictionaries.GenericCommonDictionary["textBlockAnnotation"] as DataTemplate;
                var textBlock = contentTemplate.LoadContent() as TextBlock;
                EditAnnotation.SetValue(ContentControl.ContentProperty, textBlock);
                Binding textBinding = new Binding { Path = new PropertyPath("Text"), Source = TextBox };
                textBlock.SetBinding(TextBlock.TextProperty, textBinding);
                TextAnnotation.Text = textBlock.Text;
                IsEditing = false;
            }
        }

        /// <summary>
        /// Method used to call DragStarted event handler
        /// </summary>
        internal void RaiseDragStarted()
        {
            if (!IsDragStarted)
            {
                // If anyone position value of selectedannotation differs from previous position value means we initiate the events 
                if (!CurrentPoints.X1.Equals(PreviousPoints.X1) || !CurrentPoints.X2.Equals(PreviousPoints.X2)
                    || !CurrentPoints.Y1.Equals(PreviousPoints.Y1) || !CurrentPoints.Y2.Equals(PreviousPoints.Y2))
                {
                    (selectedAnnotation as ShapeAnnotation).OnDragStarted(new EventArgs()); // Call DragStarted event
                    IsDragStarted = true;
                }
            }
        }

        /// <summary>
        /// Method used to call DragDelta event handler
        /// </summary>
        internal void RaiseDragDelta()
        {
            ShapeAnnotation selectedAnnotation = SelectedAnnotation as ShapeAnnotation;

            // If anyone position value of selectedannotation differs from previous position value means we initiate the events 
            if (!CurrentPoints.X1.Equals(PreviousPoints.X1) || !CurrentPoints.X2.Equals(PreviousPoints.X2)
                || !CurrentPoints.Y1.Equals(PreviousPoints.Y1) || !CurrentPoints.Y2.Equals(PreviousPoints.Y2))
            {
                // setting event arguments value
                DragDeltaArgs.NewValue = CurrentPoints;
                DragDeltaArgs.OldValue = PreviousPoints;
                DragDeltaArgs.Cancel = false;

                selectedAnnotation.OnDragDelta(DragDeltaArgs); // Call DragDelta event
            }
        }

        /// <summary>
        /// Method used to call DragCompleted event handler
        /// </summary>
        internal void RaiseDragCompleted()
        {
            if (IsDragStarted)
            {
                dragCompletedArgs.NewValue = CurrentPoints; // setting selectedannotation current position to NewValue

                (SelectedAnnotation as ShapeAnnotation).OnDragCompleted(dragCompletedArgs);

                IsDragStarted = false;
            }
        }

        internal void AddOrRemoveAnnotationResizer(Annotation annotation, bool isRemoval)
        {
            annotation.Chart = chart;

            UIElement resizerControl = null;

            resizerControl = !isRemoval ? annotation.CreateAnnotation() : annotation.GetRenderedAnnotation();

            if (resizerControl != null)
            {
                switch (annotation.CoordinateUnit)
                {
                    case CoordinateUnit.Axis:
                        if (isRemoval)
                        {
                            if (this.chart.SeriesAnnotationCanvas.Children.Contains(annotation))
                            {
                                this.chart.SeriesAnnotationCanvas.Children.Remove(annotation);
                                RemoveAxisMarker(annotation);
                            }

                            Grid annotationElement = this.previouseSelectedAnnotation.GetRenderedAnnotation() as Grid;

                            if (annotationElement.Children.Contains(resizerControl))
                                annotationElement.Children.Remove(resizerControl);
                        }
                        else
                        {
                            Grid annotationElement = this.SelectedAnnotation.GetRenderedAnnotation() as Grid;

                            this.chart.SeriesAnnotationCanvas.Children.Add(annotation);

                            annotationElement.Children.Add(resizerControl);

                            annotation.UpdateAnnotation();
                        }

                        break;
                    case CoordinateUnit.Pixel:
                        if (isRemoval)
                        {
                            if (this.chart.ChartAnnotationCanvas.Children.Contains(annotation))
                                this.chart.ChartAnnotationCanvas.Children.Remove(annotation);

                            Grid annotationElement = this.previouseSelectedAnnotation.GetRenderedAnnotation() as Grid;

                            if (annotationElement.Children.Contains(resizerControl))
                                annotationElement.Children.Remove(resizerControl);
                        }
                        else
                        {
                            Grid annotationElement = this.SelectedAnnotation.GetRenderedAnnotation() as Grid;

                            this.chart.ChartAnnotationCanvas.Children.Add(annotation);

                            annotationElement.Children.Add(resizerControl);

                            annotation.UpdateAnnotation();
                        }

                        break;
                }
            }
        }

        internal void AddOrRemoveAnnotations(Annotation annotation, bool isRemoval)
        {
            annotation.Chart = chart;
            UIElement annotationElement = null;
            if (annotation.IsVisbilityChanged)
                annotationElement = annotation.GetRenderedAnnotation();
            else
                annotationElement = !isRemoval ? annotation.CreateAnnotation() : annotation.GetRenderedAnnotation();

            var axisMarker = annotation as AxisMarker;

            if (annotationElement != null && axisMarker == null)
            {
                switch (annotation.CoordinateUnit)
                {
                    case CoordinateUnit.Axis:
                        if (this.chart.SeriesAnnotationCanvas.Children.Contains(annotationElement) && isRemoval)
                            RemoveSeriesAnnotation(annotation, annotationElement);
                        else if(annotation.Visibility == Visibility.Visible)
                        {
                            // The following code is dynamic changing the Coordinate from Pixel to Axis.
                            if (this.chart.ChartAnnotationCanvas.Children.Contains(annotationElement))
                                RemoveChartAnnotation(annotation, annotationElement);
                            if (!this.chart.SeriesAnnotationCanvas.Children.Contains(annotation))
                                this.chart.SeriesAnnotationCanvas.Children.Add(annotation);
                            this.chart.SeriesAnnotationCanvas.Children.Add(annotationElement);
                            annotation.UpdateAnnotation();
                        }

                        break;
                    case CoordinateUnit.Pixel:
                        if (this.chart.ChartAnnotationCanvas.Children.Contains(annotationElement) && isRemoval)
                            RemoveChartAnnotation(annotation, annotationElement);
                        else if (annotation.Visibility == Visibility.Visible)
                        {
                            // The following code is dynamic changing the Coordinate from Axis to Pixel.
                            if (this.chart.SeriesAnnotationCanvas.Children.Contains(annotationElement))
                                RemoveSeriesAnnotation(annotation, annotationElement);
                            if (!this.chart.ChartAnnotationCanvas.Children.Contains(annotation))
                                this.chart.ChartAnnotationCanvas.Children.Add(annotation);
                            this.chart.ChartAnnotationCanvas.Children.Add(annotationElement);
                            annotation.UpdateAnnotation();
                        }

                        break;
                }
            }
            else
            {
                if (axisMarker != null)
                {
                    if (this.chart.ChartAnnotationCanvas.Children.Contains(axisMarker.MarkerCanvas) && isRemoval)
                    {
                        this.chart.ChartAnnotationCanvas.Children.Remove(axisMarker.MarkerCanvas);
                    }
                    else if (axisMarker.ParentAnnotation.Visibility != Visibility.Collapsed)
                    {
                        this.chart.ChartAnnotationCanvas.Children.Add(annotationElement);
                        annotation.UpdateAnnotation();
                    }
                }
            }

            annotation.IsVisbilityChanged = false;
        }

        internal void Dispose()
        {
            if (chart != null)
                foreach (ChartAxis axis in chart.Axes)
                {
                    axis.AxisBoundsChanged -= OnAxisBoundsChanged;
                    axis.VisibleRangeChanged -= OnAxisVisibleRangeChanged;
                }

            Chart = null;

            previouseSelectedAnnotation = null;
            selectedAnnotation = null;
            previousAnnotation = null;
            TextAnnotation = null;
            EditAnnotation = null;
            CurrentAnnotation = null;
            AnnotationResizer = null;

            if (Annotations != null)
            {
                Annotations.Clear();
                Annotations = null;
            }
        }

#endregion

        #region Private Static Methods

        private static bool CheckPointInsideAnnotation(Annotation annotation, Point currentPosition)
        {
            var lineAnnotation = annotation as LineAnnotation;
            return (lineAnnotation != null) ? lineAnnotation.IsPointInsideRectangle(currentPosition) :
                                              annotation.RotatedRect.Contains(currentPosition);
        }

        #endregion

        #region Private Methods

        private void OnSelectionChanged()
        {
            if ((SelectedAnnotation == null && AnnotationResizer != null) || (AnnotationResizer != null) || (previouseSelectedAnnotation is LineAnnotation && (previouseSelectedAnnotation as LineAnnotation).CanResize))
            {
                if (previouseSelectedAnnotation is LineAnnotation)
                {
                    HideLineResizer();
                    previouseSelectedAnnotation = null;
                }
                else
                {
                    AddOrRemoveAnnotationResizer(AnnotationResizer, true);
                    AnnotationResizer = null;
                }
            }

            if (SelectedAnnotation != null)
            {
                OnAnnotationSelected();
                previouseSelectedAnnotation = SelectedAnnotation;
            }
        }

        private bool OnEnableDrag(Point point)
        {
            if (SelectedAnnotation != null)
            {
                if (selectedAnnotation.CoordinateUnit == CoordinateUnit.Axis && selectedAnnotation.EnableClipping)
                {
                    var left = Chart.SeriesClipRect.Left;
                    var top = Chart.SeriesClipRect.Top;
                    double xval = this.Chart.PointToValue(selectedAnnotation.XAxis, new Point(point.X - left, point.Y - top));
                    double yval = this.Chart.PointToValue(selectedAnnotation.YAxis, new Point(point.X - left, point.Y - top));
                    if (selectedAnnotation.XAxis.VisibleRange.Inside(xval) && selectedAnnotation.YAxis.VisibleRange.Inside(yval))
                        return true;
                }
                else
                    return true;
            }

            return false;
        }

        private void OnPointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ShowToolTip((Point)e.GetCurrentPoint(Chart.ChartAnnotationCanvas).Position, e.OriginalSource);
        }

        private void OnPointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            MouseUp();
        }

        private void ManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {
            UnHoldPanning(true);
        }

        private void ManipulationStarting(object sender, Windows.UI.Xaml.Input.ManipulationStartingRoutedEventArgs e)
        {
            UnHoldPanning(false);
        }

        private void OnManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (SelectedAnnotation != null && !e.IsInertial && OnEnableDrag(e.Position))
            {
                if (SelectedAnnotation.XAxis.Orientation == Orientation.Vertical && SelectedAnnotation.CoordinateUnit == CoordinateUnit.Axis)
                    AnnotationDrag(-e.Delta.Translation.Y, -e.Delta.Translation.X);
                else
                    AnnotationDrag(e.Delta.Translation.X, e.Delta.Translation.Y);
            }
        }

        private void OnPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            OnTextEditing();
            MouseDown(e.GetCurrentPoint(Chart.ChartAnnotationCanvas).Position, e.GetCurrentPoint(Chart.SeriesAnnotationCanvas).Position);
        }

        private void MouseDown(Point pixelPos, Point axisPos)
        {
            Annotation annotation = null;
            CurrentAnnotation = null;
            IsDragStarted = false;
            var annotations = Annotations.Where(axis => axis.CoordinateUnit == CoordinateUnit.Axis).ToList();
            if (annotations.Any())
                annotation = GetSelectedAnnotation(annotations, axisPos);
            annotations = Annotations.Where(pixel => pixel.CoordinateUnit == CoordinateUnit.Pixel).ToList();
            if (annotations.Any() && annotation == null)
                annotation = GetSelectedAnnotation(annotations, pixelPos);
            SelectedAnnotation = annotation;

            RaiseSelectionChanged(); // Raise the Selected/UnSelected event 
        }

        private void UnHoldPanning(bool value)
        {
            if (SelectedAnnotation is ShapeAnnotation)
            {
                var shapeAnnotation = SelectedAnnotation as ShapeAnnotation;
                if (shapeAnnotation.CanDrag || shapeAnnotation.CanResize)
                {
                    var eumerator = chart.Behaviors.OfType<ChartZoomPanBehavior>();
                    foreach (var behavior in eumerator)
                    {
                        behavior.InternalEnablePanning = value;
                        behavior.InternalEnableSelectionZooming = value;
                    }
                }
            }
        }

        private void MouseUp()
        {
            if (AnnotationResizer != null)
                AnnotationResizer.IsResizing = false;
            else if (SelectedAnnotation is LineAnnotation)
                (SelectedAnnotation as LineAnnotation).IsResizing = false;

            if (SelectedAnnotation is ShapeAnnotation)
                RaiseDragCompleted(); // Raise the DragCompleted event 
        }

        /// <summary>
        /// Method used to call Selected/UnSelected event handler
        /// </summary>
        private void RaiseSelectionChanged()
        {
            if (CurrentAnnotation == previousAnnotation) return;

            if (previousAnnotation != null && CurrentAnnotation != previousAnnotation)
                previousAnnotation.OnUnSelected(new EventArgs()); // Call UnSelected event
            if (CurrentAnnotation != null && CurrentAnnotation != previousAnnotation)
                CurrentAnnotation.OnSelected(new EventArgs()); // Call Selected event

            previousAnnotation = CurrentAnnotation;
        }

        /// <summary>
        /// Enables the tool tip in the visual.
        /// </summary>
        /// <param name="currentPoint">The current position.</param>
        /// <param name="source">The source object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private void ShowToolTip(Point currentPoint, object source)
        {
            FrameworkElement parent = source as FrameworkElement;
            if (parent == null) return;
            Annotation annotation = null;
            if (source is Shape)
                annotation = (source as Shape).Tag as Annotation;
            else if (source is Image)
                annotation = (source as Image).Tag as Annotation;
            else if (CheckBounds(currentPoint) != null)
            {
                parent = VisualTreeHelper.GetParent(parent as UIElement) as FrameworkElement;
                while (parent != null)
                {
                    if (parent.Tag is Annotation)
                    {
                        annotation = parent.Tag as Annotation;
                        break;
                    }

                    parent = VisualTreeHelper.GetParent(parent as UIElement) as FrameworkElement;
                }
            }

            if (annotation != null && annotation.ShowToolTip)
            {
                if (defaultToolTipTemplate == null)
                    defaultToolTipTemplate = ChartDictionaries.GenericCommonDictionary["AnnotationTooltipTemplate"] as DataTemplate;

                Tooltip.Visibility = Visibility.Visible;
                toolTipDuration = annotation.ToolTipShowDuration;
                if (!double.IsNaN(toolTipDuration))
                    ResetTimer();
                Point actualPosition = GetToolTipPosition(currentPoint, annotation);
                Tooltip.ContentTemplate = annotation.ToolTipTemplate == null ? defaultToolTipTemplate : annotation.ToolTipTemplate;
                Tooltip.Content = annotation.ToolTipContent;
                Canvas.SetLeft(Tooltip, actualPosition.X);
                Canvas.SetTop(Tooltip, actualPosition.Y);
                Canvas.SetZIndex(Tooltip, 1);
            }
            else
                Tooltip.Visibility = Visibility.Collapsed;
        }

        private object CheckBounds(Point currentPoint)
        {
            Annotation selectedAxisAnnotation = null, selectedPixelAnnotation = null;
            var annotations = Annotations.Where(axis => axis.CoordinateUnit == CoordinateUnit.Axis).Where(annotation => annotation is TextAnnotation);
            if (annotations.Count() > 0)
            {
                foreach (Annotation annotation in annotations)
                {
                    if (annotation.RotatedRect.Contains(currentPoint))
                        selectedAxisAnnotation = annotation;
                }
            }

            annotations = Annotations.Where(pixel => pixel.CoordinateUnit == CoordinateUnit.Pixel).Where(annotation => annotation is TextAnnotation);
            if (annotations.Count() > 0)
            {
                foreach (Annotation annotation in annotations)
                {
                    if (annotation.RotatedRect.Contains(currentPoint))
                        selectedPixelAnnotation = annotation;
                }
            }

            return (selectedPixelAnnotation != null ? selectedPixelAnnotation : selectedAxisAnnotation);
        }

        private void ResetTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Interval = TimeSpan.FromMilliseconds(toolTipDuration);
                timer.Start();
            }
            else
            {
                timer = new DispatcherTimer();
                timer.Tick += OnTimeout;
                timer.Start();
            }
        }

        private void OnTimeout(object sender, object e)
        {
            if (Tooltip != null)
            {
                Tooltip.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Generate the position of the tooltip according the tooltip placement.
        /// </summary>
        /// <param name="currentPosition">The current position.</param>       
        /// <param name="annotation">The required annotation.</param>
        /// <returns>Returns the tooltip position.</returns>
        private Point GetToolTipPosition(Point currentPosition, Annotation annotation)
        {
            Tooltip.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            switch (annotation.ToolTipPlacement)
            {
                case ToolTipLabelPlacement.Left:
                    currentPosition.X -= Tooltip.DesiredSize.Width;
                    break;
                case ToolTipLabelPlacement.Top:
                    currentPosition.Y -= Tooltip.DesiredSize.Height;
                    break;
                case ToolTipLabelPlacement.Bottom:
                    currentPosition.Y += Tooltip.DesiredSize.Height;
                    break;
                case ToolTipLabelPlacement.Right:
                    Tooltip.Margin = new Thickness(10, 0, 0, 0);
                    break;
            }

            return currentPosition;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private Annotation GetSelectedAnnotation(IEnumerable<Annotation> annotations, Point currentPosition)
        {
            Annotation selectedAnnotation = null;
            foreach (Annotation annotation in annotations)
            {
                var isPointInside = AnnotationManager.CheckPointInsideAnnotation(annotation, currentPosition);
                annotation.IsSelected = isPointInside && annotation is ShapeAnnotation;
                if (annotation.IsSelected)
                    selectedAnnotation = annotation;
                else if (annotation is VerticalLineAnnotation && (annotation as VerticalLineAnnotation).AxisMarkerObject != null
                    && (annotation as VerticalLineAnnotation).AxisMarkerObject.RotatedRect.Contains(currentPosition))
                    selectedAnnotation = (annotation as VerticalLineAnnotation).AxisMarkerObject;
                else if (annotation is HorizontalLineAnnotation && (annotation as HorizontalLineAnnotation).AxisMarkerObject != null
                    && (annotation as HorizontalLineAnnotation).AxisMarkerObject.RotatedRect.Contains(currentPosition))
                    selectedAnnotation = (annotation as HorizontalLineAnnotation).AxisMarkerObject;

                if (selectedAnnotation == null && isPointInside)
                    CurrentAnnotation = annotation;
                else if (selectedAnnotation != null)
                    CurrentAnnotation = selectedAnnotation;
            }

            return selectedAnnotation;
        }

        private void AnnotationDrag(double xTranslate, double yTranslate)
        {
            ShapeAnnotation selectedAnnotation = (SelectedAnnotation as ShapeAnnotation);
            var lineAnnotation = selectedAnnotation as LineAnnotation;
            bool isDraggable = (selectedAnnotation.CanResize && AnnotationResizer != null) ?
                !AnnotationResizer.IsResizing :
                    (lineAnnotation != null) ?
                        !lineAnnotation.IsResizing :
                        true;
            object x1, x2, y1, y2;
            if (selectedAnnotation.CanDrag && isDraggable)
            {
                selectedAnnotation.IsDragging = true;
                bool isXAxis = selectedAnnotation.DraggingMode == AxisMode.Horizontal;
                bool isYAxis = selectedAnnotation.DraggingMode == AxisMode.Vertical;
                bool isAll = selectedAnnotation.DraggingMode == AxisMode.All;
                if (selectedAnnotation.CoordinateUnit == CoordinateUnit.Pixel)
                {
                    x1 = (isAll || isXAxis) ? Convert.ToDouble(selectedAnnotation.X1) + xTranslate : selectedAnnotation.X1;
                    x2 = (isAll || isXAxis) ? Convert.ToDouble(selectedAnnotation.X2) + xTranslate : selectedAnnotation.X2;
                    y1 = (isAll || isYAxis) ? Convert.ToDouble(selectedAnnotation.Y1) + yTranslate : selectedAnnotation.Y1;
                    y2 = (isAll || isYAxis) ? Convert.ToDouble(selectedAnnotation.Y2) + yTranslate : selectedAnnotation.Y2;
                }
                else
                {
                    xTranslate = selectedAnnotation.XAxis.IsInversed ? -xTranslate : xTranslate;
                    yTranslate = selectedAnnotation.YAxis.IsInversed ? -yTranslate : yTranslate;
                    double xAxisChange = selectedAnnotation.XAxis.PixelToCoefficientValue(xTranslate);
                    double yAxisChange = selectedAnnotation.YAxis.PixelToCoefficientValue(yTranslate);

                    x2 = (isAll || isXAxis) ? Annotation.ConvertToObject(CalculatePointValue(selectedAnnotation.X2, xAxisChange, true, false), selectedAnnotation.XAxis) : selectedAnnotation.X2;
                    x1 = (isAll || isXAxis) ? Annotation.ConvertToObject(CalculatePointValue(selectedAnnotation.X1, xAxisChange, true, false), selectedAnnotation.XAxis) : selectedAnnotation.X1;
                    y2 = (isAll || isYAxis) ? Annotation.ConvertToObject(CalculatePointValue(selectedAnnotation.Y2, yAxisChange, false, false), selectedAnnotation.YAxis) : selectedAnnotation.Y2;
                    y1 = (isAll || isYAxis) ? Annotation.ConvertToObject(CalculatePointValue(selectedAnnotation.Y1, yAxisChange, false, false), selectedAnnotation.YAxis) : selectedAnnotation.Y1;
                }

                AnnotationManager.SetPosition(
                    PreviousPoints,
                    selectedAnnotation.X1,
                    selectedAnnotation.X2,
                    selectedAnnotation.Y1,
                    selectedAnnotation.Y2);

                AnnotationManager.SetPosition(CurrentPoints, x1, x2, y1, y2);
                RaiseDragStarted(); // Raise DragStarted event
                RaiseDragDelta(); // Raise the DragDelta event

                if (!DragDeltaArgs.Cancel)
                {
                    if (AnnotationResizer != null && !AnnotationResizer.IsResizing)
                    {
                        selectedAnnotation.X1 = AnnotationResizer.X1 = x1;
                        selectedAnnotation.X2 = AnnotationResizer.X2 = x2;
                        selectedAnnotation.Y1 = AnnotationResizer.Y1 = y1;
                        selectedAnnotation.Y2 = AnnotationResizer.Y2 = y2;
                    }
                    else
                    {
                        selectedAnnotation.X1 = x1;
                        selectedAnnotation.X2 = x2;
                        selectedAnnotation.Y1 = y1;
                        selectedAnnotation.Y2 = y2;
                    }

                    var axisMarker = selectedAnnotation as AxisMarker;
                    if (axisMarker != null)
                    {
                        if (axisMarker.ParentAnnotation is VerticalLineAnnotation)
                        {
                            if (axisMarker.ParentAnnotation.XAxis.Orientation == Orientation.Horizontal)
                                axisMarker.ParentAnnotation.X1 = selectedAnnotation.X1;
                            else
                                axisMarker.ParentAnnotation.Y1 = selectedAnnotation.Y1;
                        }
                        else
                        {
                            if (axisMarker.ParentAnnotation.XAxis.Orientation == Orientation.Vertical)
                                axisMarker.ParentAnnotation.X1 = selectedAnnotation.X1;
                            else
                                axisMarker.ParentAnnotation.Y1 = selectedAnnotation.Y1;
                        }
                    }

                    if (AnnotationResizer != null)
                        AnnotationResizer.MapActualValueToPixels();
                }

                selectedAnnotation.IsDragging = false;
            }
        }

        private double CalculatePointValue(object value, double change, bool isXAxis, bool isAngleInPhone)
        {
            ShapeAnnotation selectedAnnotation = (SelectedAnnotation is ShapeAnnotation) ? (SelectedAnnotation as ShapeAnnotation) : null;
            change = isAngleInPhone ? change * -1 : change;
            if (isXAxis)
                return Annotation.ConvertData(value, selectedAnnotation.XAxis) + change;
            return Annotation.ConvertData(value, selectedAnnotation.YAxis) - change;
        }

        private void OnAxesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var annotation in Annotations)
            {
                annotation.SetAxisFromName();
            }
        }

        private void OnChartSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.chart.ChartAnnotationCanvas.Clip = new RectangleGeometry() { Rect = new Rect(new Point(0, 0), e.NewSize) };
        }

        private void OnSeriesBoundsChanged(object sender, ChartSeriesBoundsEventArgs e)
        {
            this.chart.SeriesAnnotationCanvas.Clip = new RectangleGeometry() { Rect = this.chart.SeriesClipRect };
        }

        private void OnAxisVisibleRangeChanged(object sender, VisibleRangeChangedEventArgs e)
        {
            if (Annotations != null)
                foreach (Annotation annotation in this.Annotations)
                {
                    if (annotation.XAxis == sender || annotation.YAxis == sender)
                        annotation.UpdateAnnotation();
                }

            if (AnnotationResizer != null && (AnnotationResizer.XAxis == sender || AnnotationResizer.YAxis == sender))
            {
                AnnotationResizer.UpdateAnnotation();
                AnnotationResizer.MapActualValueToPixels();
            }
        }

        private void OnAxisBoundsChanged(object sender, ChartAxisBoundsEventArgs e)
        {
            foreach (Annotation annotation in this.Annotations)
            {
                if (annotation.XAxis == sender || annotation.YAxis == sender)
                    annotation.UpdateAnnotation();
            }

            if (AnnotationResizer != null && (AnnotationResizer.XAxis == sender || AnnotationResizer.YAxis == sender))
            {
                AnnotationResizer.UpdateAnnotation();
                AnnotationResizer.MapActualValueToPixels();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private void AddAnnotations()
        {
            foreach (Annotation annotation in this.Annotations)
            {
                UIElement annotationElement = annotation.CreateAnnotation();
                annotation.Chart = chart;
                if (annotation.Visibility != Visibility.Collapsed)
                {
                    if (annotationElement != null && !(annotation is AxisMarker))
                    {
                        if (annotation.Parent is Panel)
                        {
                            (annotation.Parent as Panel).Children.Remove(annotation);
                        }
                        if (annotationElement is FrameworkElement && (annotationElement as FrameworkElement).Parent is Panel)
                        {
                            ((annotationElement as FrameworkElement).Parent as Panel).Children.Remove(annotationElement);
                        }
                        switch (annotation.CoordinateUnit)
                        {
                            case CoordinateUnit.Axis:
                                this.chart.SeriesAnnotationCanvas.Children.Add(annotation);
                                if (annotation.Visibility == Visibility.Visible)
                                    this.chart.SeriesAnnotationCanvas.Children.Add(annotationElement);
                                break;

                            case CoordinateUnit.Pixel:
                                this.chart.ChartAnnotationCanvas.Children.Add(annotation);
                                if (annotation.Visibility == Visibility.Visible)
                                    this.chart.ChartAnnotationCanvas.Children.Add(annotationElement);
                                break;
                        }
                    }
                    else
                    {
                        this.chart.ChartAnnotationCanvas.Children.Add(annotationElement);
                    }

                    annotation.UpdateAnnotation();
                }
            }
        }

        private void RemoveChartAnnotation(Annotation annotation, UIElement annotationElement)
        {
            if (!annotation.IsVisbilityChanged)
                this.chart.ChartAnnotationCanvas.Children.Remove(annotation);
            this.chart.ChartAnnotationCanvas.Children.Remove(annotationElement);
        }

        private void RemoveSeriesAnnotation(Annotation annotation, UIElement annotationElement)
        {
            if (!annotation.IsVisbilityChanged)
                this.chart.SeriesAnnotationCanvas.Children.Remove(annotation);
            RemoveAxisMarker(annotation);
            this.chart.SeriesAnnotationCanvas.Children.Remove(annotationElement);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private void RemoveAxisMarker(Annotation annotation)
        {
            if (annotation is VerticalLineAnnotation && (annotation as VerticalLineAnnotation).ShowAxisLabel)
                this.chart.ChartAnnotationCanvas.Children.Remove((annotation as VerticalLineAnnotation).AxisMarkerObject.MarkerCanvas);
            else if (annotation is HorizontalLineAnnotation && (annotation as HorizontalLineAnnotation).ShowAxisLabel)
                this.chart.ChartAnnotationCanvas.Children.Remove((annotation as HorizontalLineAnnotation).AxisMarkerObject.MarkerCanvas);
        }

        private void OnAnnotationsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.chart != null && this.chart.ChartAnnotationCanvas != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (Annotation annotation in e.NewItems)
                        {
                            AddOrRemoveAnnotations(annotation, false);
                        }

                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (Annotation annotation in e.OldItems)
                        {
                            AddOrRemoveAnnotations(annotation, true);
                            if (SelectedAnnotation == annotation) // WPF-13543-Annotation is not clearing properly
                                SelectedAnnotation = null;
                        }

                        break;
                    case NotifyCollectionChangedAction.Replace:
                        foreach (Annotation annotation in e.OldItems)
                        {
                            AddOrRemoveAnnotations(annotation, true);
                            if (SelectedAnnotation == annotation) // WPF-13543-Annotation is not clearing properly
                                SelectedAnnotation = null;
                        }

                        foreach (Annotation annotation in e.NewItems)
                        {
                            AddOrRemoveAnnotations(annotation, false);
                        }

                        break;
                    case NotifyCollectionChangedAction.Reset:
                        SelectedAnnotation = null; // WPF-13543-Annotation is not clearing properly
                        this.chart.ChartAnnotationCanvas.Children.Clear();
                        this.chart.SeriesAnnotationCanvas.Children.Clear();
                        break;
                }
            }
        }

        #endregion

        #endregion
    }
}
