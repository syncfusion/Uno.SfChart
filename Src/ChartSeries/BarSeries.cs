using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// BarSeries represents its datapoint using a set of horizontal rectangles.
    /// </summary>
    /// <seealso cref="BarSegment"/>
    /// <seealso cref="ColumnSeries"/>
    /// <seealso cref="StackingBarSeries"/>
    /// <seealso cref="StackingColumnSeries"/>    
    public partial class BarSeries : XySegmentDraggingBase, ISegmentSelectable, ISegmentSpacing
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSelectionBrush"/> property.
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register("SegmentSelectionBrush", typeof(Brush), typeof(BarSeries),
            new PropertyMetadata(null, OnSegmentSelectionBrush));

        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSpacing"/> property.
        /// </summary>
        public static readonly DependencyProperty SegmentSpacingProperty =
            DependencyProperty.Register("SegmentSpacing", typeof(double), typeof(BarSeries),
            new PropertyMetadata(0.0, OnSegmentSpacingChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(BarSeries),
            new PropertyMetadata(-1, OnSelectedIndexChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="CustomTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty CustomTemplateProperty =
            DependencyProperty.Register("CustomTemplate", typeof(DataTemplate), typeof(BarSeries),
            new PropertyMetadata(null, OnCustomTemplateChanged));

        #endregion

        #region Fields

        #region Private Fields

        private Storyboard sb;

        bool hasTemplate;

        double initialWidth;

        private Rectangle previewRect;

        private bool isDragged;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for BarSeries
        /// </summary>
        public BarSeries()
        {
            IsActualTransposed = true;
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the interior (brush) for the selected segment(s).
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
        /// Gets or sets the spacing between the segments across the series in cluster mode.
        /// </summary>
        /// <value>
        ///     The value ranges from 0 to 1.
        /// </value>
        public double SegmentSpacing
        {
            get { return (double)GetValue(SegmentSpacingProperty); }
            set { SetValue(SegmentSpacingProperty, value); }
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
        ///         &lt;syncfusion:ScatterSeries ItemsSource="{Binding Demands}" XBindingPath="Demand" ScatterHeight="40" 
        ///                        YBindingPath="Year2010" ScatterWidth="40"&gt;
        ///             &lt;syncfusion:ScatterSeries.CustomTemplate&gt;
        ///                 &lt;DataTemplate&gt;
        ///                     &lt;Canvas&gt;
        ///                        &lt;Path Data="M20.125,32l0.5,12.375L10.3125,12.375L10.3125,0.5L29.9375,0.5L29.9375,12.375L39.75,12.375Z" 
        ///                        Stretch="Fill" Fill="{Binding Interior}" Height="{Binding ScatterHeight}" Width="{Binding ScatterWidth}" 
        ///                        Canvas.Left="{Binding RectX}" Canvas.Top="{Binding RectY}"/&gt;
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

        #region Protected Internal Properties

        /// <summary>
        /// Gets a value indicating whether this series is placed side by side.
        /// </summary>
        /// <returns>
        /// It returns <c>true</c>, if the series is placed side by side [cluster mode].
        /// </returns>
        protected internal override bool IsSideBySide
        {
            get
            {
                return true;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Interface Methods
        
        double ISegmentSpacing.CalculateSegmentSpacing(double spacing, double Right, double Left)
        {
            return CalculateSegmentSpacing(spacing, Right, Left);
        }

        #endregion
        
        #region Public Override Methods

        /// <summary>
        /// Creates the segments of BarSeries.
        /// </summary>
        public override void CreateSegments()
        {
            double x1, x2, y1, y2;
            List<double> xValues = null;
            bool isGrouping = (ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed);
            if (isGrouping)
                xValues = GroupedXValuesIndexes;
            else
                xValues = GetXValues();
            DoubleRange sbsInfo = this.GetSideBySideInfo(this);
            double median = sbsInfo.Delta / 2;
            double origin = ActualXAxis != null ? ActualXAxis.Origin : 0;
            if (ActualXAxis != null && ActualXAxis.Origin == 0 && ActualYAxis is LogarithmicAxis &&
                (ActualYAxis as LogarithmicAxis).Minimum != null)
                origin = (double)(ActualYAxis as LogarithmicAxis).Minimum;
            if (this.ActualXValues != null)
            {
                if (isGrouping)
                {
                    int segmentCount = 0;
                    Segments.Clear();
                    Adornments.Count();
                    GroupedActualData.Clear();
                    for (int i = 0; i < DistinctValuesIndexes.Count; i++)
                    {
                        var list = (from index in DistinctValuesIndexes[i]
                                    where GroupedSeriesYValues[0].Count > index
                                    select new List<double> { GroupedSeriesYValues[0][index], index }).
                            OrderByDescending(val => val[0]).ToList();
                        for (int j = 0; j < list.Count; j++)
                        {
                            var yValue = list[j][0];
                            x1 = i + sbsInfo.Start;
                            x2 = i + sbsInfo.End;
                            y1 = yValue;
                            y2 = origin; // setting origin value for column segment
                            GroupedActualData.Add(ActualData[(int)list[j][1]]);
                            BarSegment barSegment = new BarSegment(x1, y1, x2, y2, this) { Item = GroupedActualData[segmentCount] };
                            barSegment.XData = xValues[segmentCount];
                            barSegment.YData = yValue;
                            Segments.Add(barSegment);
                            if (AdornmentsInfo != null)
                            {
                                if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                                    AddColumnAdornments(i, yValue, x1, y1, segmentCount, median);
                                else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                                    AddColumnAdornments(i, yValue, x1, y2, segmentCount, median);
                                else
                                    AddColumnAdornments(i, yValue, x1, y1 + (y2 - y1) / 2, segmentCount, median);
                            }

                            segmentCount++;
                        }
                    }
                }
                else
                {
                    ClearUnUsedSegments(DataCount);
                    ClearUnUsedAdornments(DataCount);
                    for (int i = 0; i < DataCount; i++)
                    {
                        x1 = xValues[i] + sbsInfo.Start;
                        x2 = xValues[i] + sbsInfo.End;
                        y1 = YValues[i];
                        y2 = origin; // setting origin value for bar segment
                        if (i < Segments.Count)
                        {
                            (Segments[i]).SetData(x1, y1, x2, y2);
                            (Segments[i] as BarSegment).XData = xValues[i];
                            (Segments[i] as BarSegment).YData = YValues[i];
                            (Segments[i] as BarSegment).Item = ActualData[i];
                            if (SegmentColorPath != null && !Segments[i].IsEmptySegmentInterior && ColorValues.Count > 0 && !Segments[i].IsSelectedSegment)
                                Segments[i].Interior = (Interior != null) ? Interior : ColorValues[i];
                        }
                        else
                        {
                            BarSegment barSegment = new BarSegment(x1, y1, x2, y2, this) { Item = ActualData[i] };
                            barSegment.XData = xValues[i];
                            barSegment.YData = YValues[i];
                            Segments.Add(barSegment);
                        }

                        if (AdornmentsInfo != null)
                        {
                            if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                                AddColumnAdornments(xValues[i], YValues[i], x1, y1, i, median);
                            else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                                AddColumnAdornments(xValues[i], YValues[i], x1, y2, i, median);
                            else
                                AddColumnAdornments(xValues[i], YValues[i], x1, y1 + (y2 - y1) / 2, i, median);
                        }
                    }
                }
            }

            if (ShowEmptyPoints)
                UpdateEmptyPointSegments(xValues, true);
        }

        #endregion
                
        #region Internal Override Methods

        internal override void OnTransposeChanged(bool val)
        {
            IsActualTransposed = !val;
        }

        internal override bool GetAnimationIsActive()
        {
            return sb != null && sb.GetCurrentState() == ClockState.Active;
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        internal override void Animate()
        {
            int i = 0, j = 0;

            // WPF-25124 Animation not working properly when resize the window.
            if (sb != null)
            {
                sb.Stop();
                if (!canAnimate)
                {
                    ResetAdornmentAnimationState();
                    return;
                }
            }

            sb = new Storyboard();

            string path = IsActualTransposed ? "(UIElement.RenderTransform).(ScaleTransform.ScaleX)" : "(UIElement.RenderTransform).(ScaleTransform.ScaleY)";
            string adornTransPath = IsActualTransposed ? "(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.X)" : "(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.Y)";
            
            foreach (ChartSegment segment in Segments)
            {
                double elementSize = 0d;
                var element = (FrameworkElement)segment.GetRenderedVisual();
                if (segment is EmptyPointSegment && (!((segment as EmptyPointSegment).IsEmptySegmentInterior) || EmptyPointStyle == EmptyPointStyle.SymbolAndInterior))
                    elementSize = IsActualTransposed ? ((EmptyPointSegment)segment).EmptyPointSymbolWidth : ((EmptyPointSegment)segment).EmptyPointSymbolHeight;
                else
                    elementSize = IsActualTransposed ? ((BarSegment)segment).XData : ((BarSegment)segment).YData;
                if (!double.IsNaN(elementSize) && !double.IsNaN(YValues[i]))
                {
                    element.RenderTransform = new ScaleTransform();
                    if (YValues[i] < 0 && IsActualTransposed)
                        element.RenderTransformOrigin = new Point(1, 1);
                    else if (YValues[i] > 0 && !IsActualTransposed)
                        element.RenderTransformOrigin = new Point(1, 1);

                    DoubleAnimationUsingKeyFrames keyFrames1 = new DoubleAnimationUsingKeyFrames();
                    SplineDoubleKeyFrame keyFrame1 = new SplineDoubleKeyFrame();
                    keyFrame1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                    keyFrame1.Value = 0;
                    keyFrames1.KeyFrames.Add(keyFrame1);
                    keyFrame1 = new SplineDoubleKeyFrame();
                    keyFrame1.KeyTime = KeyTime.FromTimeSpan(AnimationDuration);

                    KeySpline keySpline1 = new KeySpline();
                    keySpline1.ControlPoint1 = new Point(0.64, 0.84);
#if WINDOWS_UAP
                    keySpline1.ControlPoint2 = new Point(0, 1); // Animation have to provide same easing effect in all platforms.
#else
                    keySpline1.ControlPoint2 = new Point(0.67, 0.95);
#endif
                    keyFrame1.KeySpline = keySpline1;
                    keyFrames1.KeyFrames.Add(keyFrame1);
                    keyFrame1.Value = 1;
                    keyFrames1.EnableDependentAnimation = true;
                    Storyboard.SetTargetProperty(keyFrames1, path);
                    Storyboard.SetTarget(keyFrames1, element);
                    sb.Children.Add(keyFrames1);
                    if (this.AdornmentsInfo != null && AdornmentsInfo.ShowLabel)
                    {
                        FrameworkElement label = this.AdornmentsInfo.LabelPresenters[j];

                        var transformGroup = label.RenderTransform as TransformGroup;
                        var translateTransform = new TranslateTransform();

                        if (transformGroup != null)
                        {
                            if (transformGroup.Children.Count > 0 && transformGroup.Children[0] is TranslateTransform)
                            {
                                transformGroup.Children[0] = translateTransform;
                            }
                            else
                            {
                                transformGroup.Children.Insert(0, translateTransform);
                            }
                        }                       

                        keyFrames1 = new DoubleAnimationUsingKeyFrames();
                        keyFrame1 = new SplineDoubleKeyFrame();
                        keyFrame1.KeyTime =
                            KeyTime.FromTimeSpan(TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 80) / 100));
                        keyFrame1.Value = (YValues[i] > 0) ? -(elementSize * 10) / 100 : (elementSize * 10) / 100;
                        keyFrames1.KeyFrames.Add(keyFrame1);
                        keyFrame1 = new SplineDoubleKeyFrame();
                        keyFrame1.KeyTime = KeyTime.FromTimeSpan(AnimationDuration);

                        keySpline1 = new KeySpline();
                        keySpline1.ControlPoint1 = new Point(0.64, 0.84);
#if WINDOWS_UAP
                        keySpline1.ControlPoint2 = new Point(0, 1); // Animation have to provide same easing effect in all platforms.
#else
                        keySpline1.ControlPoint2 = new Point(0.67, 0.95);
#endif

                        keyFrame1.KeySpline = keySpline1;
                        keyFrames1.KeyFrames.Add(keyFrame1);
                        keyFrame1.Value = 0;
                        keyFrames1.EnableDependentAnimation = true;
                        Storyboard.SetTargetProperty(keyFrames1, adornTransPath);
                        Storyboard.SetTarget(keyFrames1, label);
                        sb.Children.Add(keyFrames1);
                        label.Opacity = 0;

                        DoubleAnimation animation = new DoubleAnimation()
                        {
                            From = 0,
                            To = 1,
                            Duration = TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 20) / 100),
                            BeginTime = TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 80) / 100)
                        };

                        Storyboard.SetTarget(animation, label);
                        Storyboard.SetTargetProperty(animation, "(UIElement.Opacity)");
                        sb.Children.Add(animation);
                        j++;
                    }
                }

                i++;
            }

            sb.Begin();
        }

        internal override void ActivateDragging(Point mousePos, object element)
        {
            try
            {
                if (previewRect != null) return;
                if (CustomTemplate != null)
                {
                    hasTemplate = true;
                    base.ActivateDragging(mousePos, element);
                    if (SegmentIndex < 0) return;
                }
                else
                {
                    var rectangle = element as Rectangle;
                    if (rectangle == null) return;
                    if (!(rectangle.Tag is BarSegment)) return;
                    base.ActivateDragging(mousePos, element);
                    if (SegmentIndex < 0) return;
                    initialWidth = Canvas.GetLeft(rectangle);
                    var brush = rectangle.Fill as SolidColorBrush;
                    previewRect = new Rectangle
                    {
                        Fill = brush != null
                            ? new SolidColorBrush(Color.FromArgb(brush.Color.A, (byte)(brush.Color.R * 0.6),
                                (byte)(brush.Color.G * 0.6), (byte)(brush.Color.B * 0.6)))
                            : rectangle.Fill,
                        Opacity = 0.5,
                        Stroke = rectangle.Stroke,
                        StrokeThickness = rectangle.StrokeThickness
                    };
                    previewRect.SetValue(Canvas.TopProperty, Canvas.GetTop(rectangle));
                    previewRect.SetValue(Canvas.LeftProperty, initialWidth);
                    previewRect.Height = rectangle.ActualHeight;
                    previewRect.Width = rectangle.ActualWidth;
                    SeriesPanel.Children.Add(previewRect);
                }
            }
            catch
            {
                ResetDraggingElements("Exception", true);
            }
        }

        #endregion
        
        #region Protected Override Methods

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new BarSeries()
            {
                SegmentSelectionBrush = this.SegmentSelectionBrush,
                SegmentSpacing = this.SegmentSpacing,
                CustomTemplate = this.CustomTemplate
            });
        }

        protected override void ResetDraggingElements(string reason, bool dragEndEvent)
        {
            isDragged = false;
            hasTemplate = false;
            base.ResetDraggingElements(reason, dragEndEvent);
            if (previewRect == null) return;
            SeriesPanel.Children.Remove(previewRect);
            previewRect = null;
        }

        protected override void OnChartDragStart(Point mousePos, object originalSource)
        {
            ActivateDragging(mousePos, originalSource);
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

        protected override void OnChartDragEntered(Point mousePos, object originalSource)
        {
            FrameworkElement element = originalSource as FrameworkElement;
            BarSegment barSegment = null;

            if (element != null)
            {
                if (element.Tag is BarSegment) barSegment = element.Tag as BarSegment;
            }

            if (barSegment == null) return;
            double segmentPosition = YValues[Segments.IndexOf(barSegment)];

            if (IsActualTransposed)
                UpdateDragSpliter(element, barSegment,
                        segmentPosition < 0 ? "Left" : "Right");
            else
                UpdateDragSpliter(element, barSegment,
                    segmentPosition < 0 ? "Bottom" : "Top");

            base.OnChartDragEntered(mousePos, originalSource);
        }

        #endregion

        #region Protected Methods

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        protected double CalculateSegmentSpacing(double spacing, double Right, double Left)
        {
            double diff = Right - Left;
            double totalspacing = diff * spacing / 2;
            Left = Left + totalspacing;
            Right = Right - totalspacing;
            return Left;
        }

        #endregion

        #region Private Static Methods

        private static void OnSegmentSelectionBrush(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BarSeries).UpdateArea();
        }

        private static void OnSegmentSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as BarSeries;
            if (series.Area != null)
                series.Area.ScheduleUpdate();
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
            var series = d as BarSeries;

            if (series.Area == null) return;

            series.Segments.Clear();
            series.Area.ScheduleUpdate();
        }

        #endregion

        #region Private Methods

        private void UpdateDraggedSource()
        {
            try
            {
                DraggedValue = GetSnapToPoint(DraggedValue);
                var baseValue = YValues[SegmentIndex];
                var dragPreviewEnd = new XyPreviewEndEventArgs { BaseYValue = baseValue, NewYValue = DraggedValue };
                RaisePreviewEnd(dragPreviewEnd);
                if (dragPreviewEnd.Cancel)
                {
                    ResetDraggingElements("", false);
                    return;
                }

                YValues[SegmentIndex] = DraggedValue;
                if (UpdateSource && !IsSortData)
                    UpdateUnderLayingModel(YBindingPath, SegmentIndex, DraggedValue);
                UpdateArea();
                isDragged = false;
                RaiseDragEnd(new ChartDragEndEventArgs { BaseYValue = baseValue, NewYValue = DraggedValue });
            }
            catch
            {
                ResetDraggingElements("Exception", true);
            }
        }

        private void SegmentPreview(Point mousePos)
        {
            try
            {
                if (previewRect == null && !hasTemplate) return;

                DraggedValue = Area.PointToValue(ActualYAxis, mousePos);
                var dragEvent = new XySegmentDragEventArgs
                {
                    NewYValue = DraggedValue,
                    BaseYValue = YValues[SegmentIndex],
                    Segment = Segments[SegmentIndex],
                    Delta = GetActualDelta()
                };

                prevDraggedValue = DraggedValue;

                RaiseDragDelta(dragEvent);
                if (dragEvent.Cancel)
                {
                    ResetDraggingElements("Cancel", true);
                    return;
                }

                if (CustomTemplate != null)
                {
                    var barSegment = Segments[SegmentIndex] as BarSegment;

                    if (!IsActualTransposed)
                    {
                        double originalPos = barSegment.RectY;
                        double posY = originalPos > mousePos.Y ? mousePos.Y : originalPos;
                        UpdateSegmentDragValueToolTip(
                            new Point(barSegment.RectX + barSegment.Width / 2, posY),
                            Segments[SegmentIndex], 0, DraggedValue, barSegment.Width / 2, 0);
                    }
                    else
                    {
                        double originalPos = barSegment.RectX + barSegment.Width;
                        double posX = originalPos < mousePos.X ? mousePos.X : originalPos;

                        UpdateSegmentDragValueToolTip(
                            new Point(posX, barSegment.RectY + barSegment.Height / 2),
                            Segments[SegmentIndex], 0, DraggedValue, 0, barSegment.Height / 2);
                    }
                }
                else
                {
                    if (!this.IsActualTransposed)
                    {
                        double currPos = mousePos.Y;

                        double movingOffset = Canvas.GetTop(previewRect) - currPos;
                        if (currPos > Area.ValueToPoint(ActualYAxis, 0))
                        {
                            previewRect.Height = Math.Abs(movingOffset);
                        }
                        else
                        {
                            previewRect.SetValue(Canvas.TopProperty, currPos);
                            previewRect.Height += movingOffset;
                        }

                        double originalPos = Canvas.GetTop(Segments[SegmentIndex].GetRenderedVisual());
                        double posY = originalPos > mousePos.Y ? mousePos.Y : originalPos + 20;
                        UpdateSegmentDragValueToolTip(
                            new Point(Canvas.GetLeft(previewRect) + previewRect.Width / 2, posY),
                            Segments[SegmentIndex], 0, DraggedValue, previewRect.Width / 2, 0);
                    }
                    else
                    {
                        var currPos = mousePos.X;

                        var movingOffset = Canvas.GetLeft(previewRect) - currPos;
                        if (currPos > Area.ValueToPoint(ActualYAxis, 0))
                        {
                            previewRect.SetValue(Canvas.LeftProperty, Canvas.GetLeft(previewRect));
                            previewRect.Width = Math.Abs(movingOffset);
                        }
                        else
                        {
                            previewRect.SetValue(Canvas.LeftProperty, currPos);
                            previewRect.Width += movingOffset;
                        }

                        var rect = Segments[SegmentIndex].GetRenderedVisual() as FrameworkElement;
                        double originalPos = Canvas.GetLeft(rect) + rect.Width;
                        double posX = originalPos < mousePos.X ? mousePos.X : originalPos;

                        UpdateSegmentDragValueToolTip(
                            new Point(posX, Canvas.GetTop(previewRect) + previewRect.Height / 2),
                            Segments[SegmentIndex], 0, DraggedValue, 0, previewRect.Height / 2);
                    }
                }

                ResetDragSpliter();
                isDragged = true;
            }
            catch
            {
                ResetDraggingElements("Exception", true);
            }
        }

        #endregion

        #endregion
    }
}
