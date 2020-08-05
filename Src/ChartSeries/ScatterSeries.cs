using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// ScatterSeries displays data points as set of circular symbols. 
    /// Values are being represented by the position of the symbols on the chart.    
    /// </summary>
    /// <remarks>
    /// ScatterSeries are typically used to compare aggregated data across categories.
    /// </remarks>
    /// <seealso cref="ScatterSegment"/>
    /// <seealso cref="BubbleSeries"/>
    public partial class ScatterSeries : XySegmentDraggingBase, ISegmentSelectable
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="DragDirection"/> property.       
        /// </summary>
        /// Using a DependencyProperty as the backing store for DragDirection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DragDirectionProperty =
            DependencyProperty.Register(
                "DragDirection", 
                typeof(DragType),
                typeof(ScatterSeries), 
                new PropertyMetadata(DragType.XY));

        /// <summary>
        ///  The DependencyProperty for <see cref="SegmentSelectionBrush"/> property.       
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register(
                "SegmentSelectionBrush", 
                typeof(Brush),
                typeof(ScatterSeries),
                new PropertyMetadata(null, OnSegmentSelectionBrush));

        /// <summary>
        /// The DependencyProperty for <see cref="ScatterWidth"/> property.       
        /// </summary>
        public static readonly DependencyProperty ScatterWidthProperty =
            DependencyProperty.Register(
                "ScatterWidth",
                typeof(double),
                typeof(ScatterSeries),
                new PropertyMetadata(20d, OnScatterWidthChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ScatterHeight"/> property.       
        /// </summary>
        public static readonly DependencyProperty ScatterHeightProperty =
            DependencyProperty.Register(
                "ScatterHeight", 
                typeof(double), 
                typeof(ScatterSeries),
                new PropertyMetadata(20d, OnScatterHeightChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property.       
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex",
                typeof(int),
                typeof(ScatterSeries),
                new PropertyMetadata(-1, OnSelectedIndexChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="CustomTemplate"/> property.       
        /// </summary>
        public static readonly DependencyProperty CustomTemplateProperty =
            DependencyProperty.Register(
                "CustomTemplate", 
                typeof(DataTemplate), 
                typeof(ScatterSeries),
                new PropertyMetadata(null, OnCustomTemplateChanged));

        #endregion

        #region Fields

        #region Private Fields

        private Storyboard sb;

        private UIElement previewElement;

        private bool isDragged;

        private ScatterSegment draggingSegment;

        #endregion

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the dragging direction.
        /// </summary>
        public DragType DragDirection
        {
            get { return (DragType)GetValue(DragDirectionProperty); }
            set { SetValue(DragDirectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the interior(brush) for the selected segment(s).
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        /// <example>
        ///     <code>
        ///     series.SegmentSelectionBrush = new SolidColorBrush(Colors.Red);
        ///     </code>
        /// </example>
        public Brush SegmentSelectionBrush
        {
            get { return (Brush)GetValue(SegmentSelectionBrushProperty); }
            set { SetValue(SegmentSelectionBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that specifies the width of the chart scatter segment. This is a bindable property.
        /// </summary>
        /// <remarks>
        /// The default value is 20.
        /// </remarks>
        public double ScatterWidth
        {
            get { return (double)GetValue(ScatterWidthProperty); }
            set { SetValue(ScatterWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that specifies the height of the chart scatter segment. This is a bindable property.
        /// </summary>
        /// <remarks>
        /// The default value is 20.
        /// </remarks>
        public double ScatterHeight
        {
            get { return (double)GetValue(ScatterHeightProperty); }
            set { SetValue(ScatterHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the index of the selected segment.
        /// </summary>
        /// <value>
        /// <c>Int</c> value represents the index of the data point(or segment) to be selected. 
        /// </value>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom template for this series.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        /// <example>
        /// This example, we are using <see cref="ScatterSeries"/>.
        /// </example>
        /// <example>
        ///     <code language="XAML">
        ///         &lt;syncfusion:ScatterSeries ItemsSource="{Binding Demands}" XBindingPath="Demand" YBindingPath="Year2010" 
        ///                         ScatterHeight="40" ScatterWidth="40"&gt;
        ///            &lt;syncfusion:ScatterSeries.CustomTemplate&gt;
        ///                 &lt;DataTemplate&gt;
        ///                     &lt;Canvas&gt;
        ///                        &lt;Path Data="M20.125,32l0.5,12.375L10.3125,12.375L10.3125,0.5L29.9375,0.5L29.9375,12.375L39.75,12.375Z" Stretch="Fill"
        ///                                 Fill="{Binding Interior}" Height="{Binding ScatterHeight}" Width="{Binding ScatterWidth}" 
        ///                                 Canvas.Left="{Binding RectX}" Canvas.Top="{Binding RectY}"/&gt;
        ///                     &lt;/Canvas&gt;
        ///                 &lt;/DataTemplate&gt;
        ///             &lt;/syncfusion:ScatterSeries.CustomTemplate&gt;
        ///         &lt;/syncfusion:ScatterSeries&gt;
        ///     </code>
        /// </example>
        public DataTemplate CustomTemplate
        {
            get { return (DataTemplate)GetValue(CustomTemplateProperty); }
            set { SetValue(CustomTemplateProperty, value); }
        }

        #endregion

        #region Internal Override Properties

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Creates the segments of ScatterSeries
        /// </summary>
        public override void CreateSegments()
        {
            List<double> xValues = null;
            bool isGrouping = this.ActualXAxis is CategoryAxis && !(this.ActualXAxis as CategoryAxis).IsIndexed;
            if (isGrouping)
                xValues = GroupedXValuesIndexes;
            else
                xValues = GetXValues();
            if (xValues != null)
            {
                if (isGrouping)
                {
                    Segments.Clear();
                    Adornments.Clear();
                    for (int i = 0; i < xValues.Count; i++)
                    {
                        if (i < GroupedSeriesYValues[0].Count)
                        {
                            ScatterSegment scatterSegment = new ScatterSegment(xValues[i], GroupedSeriesYValues[0][i], this);
                            scatterSegment.Series = this;
                            scatterSegment.SetData(xValues[i], GroupedSeriesYValues[0][i]);
                            scatterSegment.YData = GroupedSeriesYValues[0][i];
                            scatterSegment.XData = xValues[i];
                            scatterSegment.Item = ActualData[i];
                            Segments.Add(scatterSegment);
                            if (AdornmentsInfo != null)
                                AddAdornments(xValues[i], GroupedSeriesYValues[0][i], i);
                        }
                    }
                }
                else
                {
                    ClearUnUsedSegments(this.DataCount);
                    ClearUnUsedAdornments(this.DataCount);
                    for (int i = 0; i < this.DataCount; i++)
                    {
                        if (i < Segments.Count)
                        {
                            (Segments[i].Item) = ActualData[i];
                            (Segments[i]).SetData(xValues[i], YValues[i]);
                            (Segments[i] as ScatterSegment).XData = xValues[i];
                            (Segments[i] as ScatterSegment).Item = ActualData[i];
                            if (SegmentColorPath != null && !Segments[i].IsEmptySegmentInterior && ColorValues.Count > 0 && !Segments[i].IsSelectedSegment)
                                Segments[i].Interior = (Interior != null) ? Interior : ColorValues[i];
                        }
                        else
                        {
                            ScatterSegment scatterSegment = new ScatterSegment(xValues[i], YValues[i], this);
                            scatterSegment.Series = this;
                            scatterSegment.SetData(xValues[i], YValues[i]);
                            scatterSegment.YData = YValues[i];
                            scatterSegment.XData = xValues[i];
                            scatterSegment.Item = ActualData[i];
                            Segments.Add(scatterSegment);
                        }

                        if (AdornmentsInfo != null)
                            AddAdornments(xValues[i], YValues[i], i);
                    }
                }

                if (ShowEmptyPoints)
                    UpdateEmptyPointSegments(xValues, false);
            }
        }

        #endregion

        #region Internal Override Methods

        /// <summary>
        /// This method used to get the chart data index at an SfChart co-ordinates
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal override int GetDataPointIndex(Point point)
        {
            Canvas canvas = Area.GetAdorningCanvas();
            double left = Area.ActualWidth - canvas.ActualWidth;
            double top = Area.ActualHeight - canvas.ActualHeight;

            point.X = point.X - left - Area.SeriesClipRect.Left + Area.Margin.Left;
            point.Y = point.Y - top - Area.SeriesClipRect.Top + Area.Margin.Top;

            foreach (var segment in Segments)
            {
                Ellipse ellipse = segment.GetRenderedVisual() as Ellipse;
                if (ellipse != null && EllipseContainsPoint(ellipse, point))
                    return Segments.IndexOf(segment);
            }

            return -1;
        }

        internal override bool GetAnimationIsActive()
        {
            return sb != null && sb.GetCurrentState() == ClockState.Active;
        }

        internal override void Animate()
        {
            int i = 0;
            Random rand = new Random();

            // WPF-25124 Animation not working properly when resize the window.
            if (sb != null)
            {
                sb.Stop();
                if (!canAnimate)
                {
                    foreach (ScatterSegment segment in this.Segments)
                    {
                        FrameworkElement element = segment.GetRenderedVisual() as FrameworkElement;
                        element.ClearValue(FrameworkElement.RenderTransformProperty);
                    }

                    ResetAdornmentAnimationState();
                    return;
                }
            }

            sb = new Storyboard();
            foreach (ScatterSegment segment in this.Segments)
            {
                int randomValue = rand.Next(0, 50);
                TimeSpan beginTime = TimeSpan.FromMilliseconds(randomValue * 20);

                var element = (FrameworkElement)segment.GetRenderedVisual();

                // UWP-8445 Fix for xamarin forms uwp animation for custom template in scatter series.
                if (CustomTemplate != null)
                {
                    var contentPresenter = VisualTreeHelper.GetChild(element, 0);
                    var canvas = VisualTreeHelper.GetChild(contentPresenter, 0) as Canvas;

                    if (canvas != null)
                    {
                        var customElement = VisualTreeHelper.GetChild(canvas, 0) as FrameworkElement;

                        if (customElement != null)
                        {
                            element = customElement;
                        }
                    }
                }

                element.RenderTransform = new ScaleTransform() { ScaleY = 0, ScaleX = 0 };
                element.RenderTransformOrigin = new Point(0.5, 0.5);
                DoubleAnimationUsingKeyFrames keyFrames = new DoubleAnimationUsingKeyFrames();
                keyFrames.BeginTime = beginTime;
                SplineDoubleKeyFrame keyFrame = new SplineDoubleKeyFrame();
                keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                keyFrame.Value = 0;
                keyFrames.KeyFrames.Add(keyFrame);
                keyFrame = new SplineDoubleKeyFrame();
                ////keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(AnimationDuration.TotalMilliseconds - (randomValue * (2 * AnimationDuration.TotalMilliseconds) / 100)));
                keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 30) / 100));

                KeySpline keySpline = new KeySpline();
                keySpline.ControlPoint1 = new Point(0.64, 0.84);
                keySpline.ControlPoint2 = new Point(0.67, 0.95);
                keyFrame.KeySpline = keySpline;
                keyFrames.KeyFrames.Add(keyFrame);
                keyFrame.Value = 1;
                keyFrames.EnableDependentAnimation = true;
                Storyboard.SetTargetProperty(keyFrames, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");
                Storyboard.SetTarget(keyFrames, element);
                sb.Children.Add(keyFrames);

                keyFrames = new DoubleAnimationUsingKeyFrames();
                keyFrames.BeginTime = beginTime;
                keyFrame = new SplineDoubleKeyFrame();
                keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                keyFrame.Value = 0;
                keyFrames.KeyFrames.Add(keyFrame);
                keyFrame = new SplineDoubleKeyFrame();
                ////keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(AnimationDuration.TotalMilliseconds - (randomValue * (2 * AnimationDuration.TotalMilliseconds) / 100)));
                keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 30) / 100));

                keySpline = new KeySpline();
                keySpline.ControlPoint1 = new Point(0.64, 0.84);
                keySpline.ControlPoint2 = new Point(0.67, 0.95);
                keyFrame.KeySpline = keySpline;
                keyFrames.KeyFrames.Add(keyFrame);
                keyFrame.Value = 1;
                keyFrames.EnableDependentAnimation = true;
                Storyboard.SetTargetProperty(keyFrames, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");
                Storyboard.SetTarget(keyFrames, element);
                sb.Children.Add(keyFrames);

                if (AdornmentsInfo != null && AdornmentsInfo.ShowLabel)
                {
                    UIElement label = this.AdornmentsInfo.LabelPresenters[i];
                    label.Opacity = 0;

                    DoubleAnimation animation = new DoubleAnimation() { To = 1, From = 0, BeginTime = TimeSpan.FromSeconds(beginTime.TotalSeconds + (beginTime.Seconds * 90) / 100), Duration = TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 50) / 100) };

                    Storyboard.SetTargetProperty(animation, "FrameworkElement.Opacity");
                    Storyboard.SetTarget(animation, label);
                    sb.Children.Add(animation);
                }

                i++;
            }

            sb.Begin();
        }
        
        internal override void ActivateDragging(Point mousePos, object element)
        {
            try
            {
#if NETFX_CORE
                Focus(FocusState.Keyboard);
#endif
                DeltaX = 0;
                delta = 0;

                KeyDown += CoreWindow_KeyDown;

                ChartDragStartEventArgs dragEventArgs = new ChartXyDragStartEventArgs
                {
                    BaseXValue = GetActualXValue(SegmentIndex),
                    BaseYValue = draggingSegment.YData
                };

                if (EmptyPointIndexes != null)
                {
                    var emptyPointIndex = EmptyPointIndexes[0];
                    foreach (var index in emptyPointIndex)
                        if (SegmentIndex == index)
                        {
                            dragEventArgs.EmptyPoint = true;
                            break;
                        }
                }

                RaiseDragStart(dragEventArgs);

                if (dragEventArgs.Cancel)
                {
                    ResetDraggingElements("Cancel", true);
                    SegmentIndex = -1;
                }

                UnHoldPanning(false);

                if (draggingSegment == null) return;
                if (previewElement != null) return;

                previewElement = GetPreviewEllipse();
                previewElement.Opacity = 0.5;
                SeriesPanel.Children.Add(previewElement);

#if NETFX_CORE
                // To return the underlaying segment instead of the preview segment in pointer released selection.
                var selectionBehavior = this.ActualArea.SelectionBehaviour;
                if (selectionBehavior != null && selectionBehavior.SelectionMode == SelectionMode.MouseClick)
                    previewElement.IsHitTestVisible = false;
#endif
            }
            catch
            {
                ResetDraggingElements("Exception", true);
            }
        }

        internal override void Dispose()
        {
            if (sb != null)
            {
                sb.Stop();
                sb.Children.Clear();
                sb = null;
            }
            base.Dispose();
        }

        #endregion

        #region Protected Override Methods

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new ScatterSeries()
            {
                ScatterHeight = ScatterHeight,
                ScatterWidth = ScatterWidth,
                SelectedIndex = this.SelectedIndex,
                SegmentSelectionBrush = SegmentSelectionBrush,
                CustomTemplate = this.CustomTemplate,
                EnableSegmentDragging = this.EnableSegmentDragging,
                DragDirection = this.DragDirection
            });
        }
        
        protected override void OnChartDragStart(Point mousePos, object originalSource)
        {
            ActivateDragging(mousePos, originalSource);
        }
     
        protected override void OnChartDragEntered(Point mousePos, object originalSource)
        {
            var frameworkElement = originalSource as FrameworkElement;
            if (frameworkElement == null) return;

            UpdatePreviewIndicatorPosition(mousePos, frameworkElement);
            base.OnChartDragEntered(mousePos, originalSource);
        }
        
        protected override void OnChartDragDelta(Point mousePos, object originalSource)
        {
            SegmentPreview(mousePos);
        }
        
        protected override void OnChartDragEnd(Point mousePos, object originalSource)
        {
            if (isDragged)
                UpdateDraggedSource();
            ResetDraggingElements("", false);
        }
        
        protected override void ResetDraggingElements(string reason, bool dragEndEvent)
        {
            ResetDraggingindicators();
            isDragged = false;
            base.ResetDraggingElements(reason, dragEndEvent);

            if (previewElement == null) return;
            SeriesPanel.Children.Remove(previewElement);
            previewElement = null;

            DraggedXValue = 0;
            DraggedValue = 0;
        }

        protected override void OnChartDragExited(Point mousePos, object originalSource)
        {
            if (EnableSegmentDragging)
            {
                ResetDraggingindicators();
            }
        }

        #endregion

        #region Private Static Methods

        private static void OnSegmentSelectionBrush(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ScatterSeries).UpdateArea();
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            series.OnPropertyChanged("SelectedIndex");
            if (series.ActualArea == null || series.ActualArea.SelectionBehaviour == null) return;
            if (series.ActualArea.SelectionBehaviour.SelectionStyle == SelectionStyle.Single)
                series.SelectedIndexChanged((int)e.NewValue, (int)e.OldValue);
            else if ((int)e.NewValue != -1)
                series.SelectedSegmentsIndexes.Add((int)e.NewValue);
        }

        private static void OnCustomTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as ScatterSeries;

            if (series.Area == null) return;

            series.Segments.Clear();
            series.Area.ScheduleUpdate();
        }

        /// <summary>
        /// This method used to check the position within the ellipse
        /// </summary>
        /// <param name="Ellipse"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private static bool EllipseContainsPoint(Ellipse Ellipse, Point point)
        {
            Point center = new Point(
                  Canvas.GetLeft(Ellipse) + (Ellipse.Width / 2),
                  Canvas.GetTop(Ellipse) + (Ellipse.Height / 2));

            double x = Ellipse.Width / 2;
            double y = Ellipse.Height / 2;

            if (x <= 0.0 || y <= 0.0)
                return false;

            Point result = new Point(
                point.X - center.X,
                point.Y - center.Y);

            return ((double)(result.X * result.X)
                     / (x * x)) + ((double)(result.Y * result.Y) / (y * y))
                <= 1.0;
        }

        private static void OnScatterHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScatterSeries series = d as ScatterSeries;

            foreach (ChartSegment segment in series.Segments)
            {
                (segment as ScatterSegment).ScatterHeight = series.ScatterHeight;
            }

            if (series != null)
                series.UpdateArea();
        }

        private static void OnScatterWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScatterSeries series = d as ScatterSeries;

            foreach (ChartSegment segment in series.Segments)
            {
                (segment as ScatterSegment).ScatterWidth = series.ScatterWidth;
            }

            if (series != null)
                series.UpdateArea();
        }

        #endregion

        #region Private Methods

        private void AddAdornments(double x, double yValue, int i)
        {
            double adornX = 0d, adornY = 0d;
            adornX = x;
            adornY = yValue;
            if (i < Adornments.Count)
            {
                Adornments[i].SetData(adornX, adornY, adornX, adornY);
            }
            else
            {
                Adornments.Add(this.CreateAdornment(this, adornX, adornY, adornX, adornY));
            }

            Adornments[i].Item = ActualData[i];
        }

        private UIElement GetPreviewEllipse()
        {
            ScatterSegment scatter = new ScatterSegment();
            scatter.SetData(draggingSegment.XData, draggingSegment.YData);
            scatter.CustomTemplate = this.CustomTemplate;
            scatter.Series = this;

            // This size is not used and empty size is given in segment calculation.
            var uIElement = scatter.CreateSegmentVisual(new Size(double.PositiveInfinity, double.PositiveInfinity));

            // In line series preview dragging segment value is assigned. It is binded for the good working of the segment selection 
            // and empty point interior condition.
            var colorBinding = new Binding() { Source = Segments[SegmentIndex], Path = new PropertyPath("Interior") };
            BindingOperations.SetBinding(scatter, ScatterSegment.InteriorProperty, colorBinding);

            scatter.Update(this.ChartTransformer);
            return uIElement;
        }

        // Animation Ellipse logic.
        private void UpdatePreviewIndicatorPosition(Point mousePos, FrameworkElement frameworkElement)
        {
            if (previewElement == null)
            {
                double positionX, positionY;
                bool isAdornment = false;

                // WPF_18250 DragDelta Event returns undesired delta value.
                // Here we have reset the prevDraggedValue when dragged segment is changed.
                int previousIndex = SegmentIndex;

                draggingSegment = null;

                SetScatterDraggingSegment(frameworkElement, ref isAdornment);

                if (draggingSegment == null) return;

                if (previousIndex != SegmentIndex)
                {
                    prevDraggedXValue = 0;
                    prevDraggedValue = 0;
                }

                XySegmentEnterEventArgs args = new XySegmentEnterEventArgs
                {
                    XValue = GetActualXValue(SegmentIndex),
                    YValue = draggingSegment.YData,
                    SegmentIndex = SegmentIndex,
                    CanDrag = true,
                };
                RaiseDragEnter(args);
                if (!args.CanDrag) return;

                positionY = Area.ValueToLogPoint(ActualYAxis, draggingSegment.YData);
                positionX = Area.ValueToLogPoint(ActualXAxis, draggingSegment.XData);

                if (CustomTemplate != null)
                {
                    AnimationElement = GetPreviewEllipse() as ContentControl;
                    AnimateSegmentTemplate(positionX, positionY, draggingSegment);
                }
                else if (isAdornment)
                {
                    if (AdornmentsInfo.Symbol.ToString() == "Custom")
                        AnimateAdornmentSymbolTemplate(positionX, positionY);
                    else
                    {
                        AddAnimationEllipse(
                            ChartDictionaries.GenericSymbolDictionary["Animation" + AdornmentsInfo.Symbol.ToString()+ "Template"] as ControlTemplate,
                            AdornmentsInfo.SymbolHeight,
                            AdornmentsInfo.SymbolWidth, 
                            positionX, 
                            positionY, 
                            Adornments[SegmentIndex],
                            true);
                    }
                }
                else
                {
                    AddAnimationEllipse(
                        ChartDictionaries.GenericSymbolDictionary["AnimationEllipseTemplate"] as ControlTemplate,
                        ScatterHeight,
                        ScatterWidth,
                        positionX, 
                        positionY,
                        Segments[SegmentIndex], 
                        false);
                }
            }
        }

        private void SetScatterDraggingSegment(FrameworkElement frameworkElement, ref bool isAdornment)
        {
            // Empty point support only given for the interior case and not for symbol case.
            if (frameworkElement.Tag is EmptyPointSegment || frameworkElement.DataContext is EmptyPointSegment)
                return;
            if (frameworkElement.Tag is ScatterSegment || frameworkElement.DataContext is ScatterSegment)
            {
                draggingSegment = GetDraggingSegment(frameworkElement) as ScatterSegment;
                if (ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed)
                    SegmentIndex = ActualData.IndexOf(draggingSegment.Item);
                else
                    SegmentIndex = Segments.IndexOf(draggingSegment);
            }
            else
            {
                isAdornment = true;

                // Get the selected adornment index and select the adornment marker
                SegmentIndex = ChartExtensionUtils.GetAdornmentIndex(frameworkElement);
                if (SegmentIndex > -1)
                    draggingSegment = Segments[SegmentIndex] as ScatterSegment;
            }
        }

        private void SegmentPreview(Point mousePos)
        {
            try
            {
                if (previewElement == null) return;

                double xPosition = mousePos.X, yPosition = mousePos.Y;

                DraggedXValue = (DragDirection == DragType.Y || IsIndexed) ?
                    draggingSegment.XData :
                    Area.PointToValue(ActualXAxis, mousePos);

                DraggedValue = (DragDirection == DragType.X) ?
                    draggingSegment.YData :
                    Area.PointToValue(ActualYAxis, mousePos);

                XySegmentDragEventArgs dragEvent;
                var currentDeltaX = IsIndexed ? 0d : GetActualXDelta(prevDraggedXValue, DraggedXValue, ref DeltaX);
                var currentDeltaY = GetActualDelta();
                var baseXValue = GetActualXValue(SegmentIndex);

                dragEvent = new XyDeltaDragEventArgs
                {
                    NewXValue = IsIndexed ? baseXValue : GetDraggedActualXValue(DraggedXValue),
                    NewYValue = DraggedValue,
                    BaseXValue = baseXValue,
                    BaseYValue = draggingSegment.YData,
                    Segment = draggingSegment,
                    Delta = currentDeltaY,
                    DeltaX = currentDeltaX
                };

                prevDraggedXValue = DraggedXValue;
                prevDraggedValue = DraggedValue;

                RaiseDragDelta(dragEvent);
                if (dragEvent.Cancel)
                {
                    ResetDraggingElements("Cancel", true);
                    return;
                }

                // For the drag type the XY position is already set.
                // The string condition is for the category axis not to drag in x direction.
                if (DragDirection == DragType.Y || IsIndexed)
                {
                    if (IsActualTransposed)
                        yPosition = this.Area.ValueToLogPoint(ActualXAxis, draggingSegment.XData);
                    else
                        xPosition = this.Area.ValueToLogPoint(ActualXAxis, draggingSegment.XData);
                }

                if (DragDirection == DragType.X)
                {
                    if (IsActualTransposed)
                        xPosition = this.Area.ValueToLogPoint(ActualYAxis, draggingSegment.YData);
                    else
                        yPosition = this.Area.ValueToLogPoint(ActualYAxis, draggingSegment.YData);
                }

                if (CustomTemplate != null)
                {
                    var segment = (previewElement as ContentControl).Content as ScatterSegment;

                    // For template case the rect value should be updated.
                    segment.RectY = yPosition - ScatterHeight / 2;
                    segment.RectX = xPosition - ScatterWidth / 2;
                }
                else
                {
                    previewElement.SetValue(Canvas.TopProperty, yPosition - ScatterHeight / 2);
                    previewElement.SetValue(Canvas.LeftProperty, xPosition - ScatterWidth / 2);
                }

                UpdateSegmentDragValueToolTip(
                    new Point(xPosition, yPosition),
                    draggingSegment, 
                    DraggedXValue,
                    DraggedValue,
                    ScatterWidth / 2, 
                    ScatterHeight / 2);

                ResetDraggingindicators();
                isDragged = true;
            }
            catch
            {
                ResetDraggingElements("Exception", true);
            }
        }

        private void UpdateDraggedSource()
        {
            try
            {
                DraggedXValue = GetSnapToPoint(DraggedXValue);
                DraggedValue = GetSnapToPoint(DraggedValue);

                var actualDraggedXValue = GetDraggedActualXValue(DraggedXValue);

                var baseYValue = draggingSegment.YData;
                var baseXValue = GetActualXValue(SegmentIndex);

                var updateSource = (UpdateSource && !IsSortData);

                if (!IsIndexed && (DragDirection == DragType.X || DragDirection == DragType.XY))
                {
                    var xValues = ActualXValues as List<double>;
                    if (xValues != null)
                        xValues[SegmentIndex] = DraggedXValue;

                    if (updateSource)
                        UpdateUnderLayingModel(XBindingPath, SegmentIndex, actualDraggedXValue);
                }

                if (DragDirection == DragType.Y || DragDirection == DragType.XY)
                {
                    YValues[SegmentIndex] = DraggedValue;

                    if (updateSource)
                        UpdateUnderLayingModel(YBindingPath, SegmentIndex, DraggedValue);
                }

                // For selection series index preservation.
                UpdateArea();
                RaiseDragEnd(new ChartXyDragEndEventArgs { BaseYValue = baseYValue, NewYValue = DraggedValue, BaseXValue = baseXValue, NewXValue = IsIndexed ? baseXValue : actualDraggedXValue });
                ResetDraggingElements("CaptureReleased", false);
            }
            catch
            {
                ResetDraggingElements("CaptureReleased", true);
            }
        }

        private void ResetDraggingindicators()
        {
            if (AnimationElement != null && SeriesPanel.Children.Contains(AnimationElement))
            {
                SeriesPanel.Children.Remove(AnimationElement);
                AnimationElement = null;
            }
        }

        #endregion

        #endregion         
    }
}