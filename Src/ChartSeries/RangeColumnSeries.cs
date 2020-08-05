using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// RangeColumnSeries displays data points as a set of vertical bars of varying heights,starting at different points within a area of<see cref="SfChart"/>.
    /// </summary>
    /// <seealso cref="RangeColumnSegment"/>
    /// <seealso cref="RangeAreaSeries"/>
    /// <seealso cref="ColumnSeries"/>
    /// <seealso cref="BarSeries"/>
    public partial class RangeColumnSeries : RangeSegmentDraggingBase, ISegmentSelectable, ISegmentSpacing
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="CustomTemplate"/> property. 
        /// </summary>
        public static readonly DependencyProperty CustomTemplateProperty =
            DependencyProperty.Register(
                "CustomTemplate",
                typeof(DataTemplate), 
                typeof(RangeColumnSeries),
                new PropertyMetadata(null, OnCustomTemplateChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="SegmentSpacing"/> property.       
        /// </summary>
        public static readonly DependencyProperty SegmentSpacingProperty =
            DependencyProperty.Register(
                "SegmentSpacing",
                typeof(double),
                typeof(RangeColumnSeries),
                new PropertyMetadata(0.0, OnSegmentSpacingChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="SegmentSelectionBrush"/> property.       
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register(
                "SegmentSelectionBrush",
                typeof(Brush),
                typeof(RangeColumnSeries),
                new PropertyMetadata(null, OnSegmentSelectionBrush));
        
        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property.       
        /// </summary>       
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex",
                typeof(int), 
                typeof(RangeColumnSeries),
                new PropertyMetadata(-1, OnSelectedIndexChanged));

        #endregion

        #region Fields

        #region Private Fields

        double initialHeight, initialValue;

        int draggingMode;

        private Rectangle previewRect;

        private RangeColumnSegment selectedSegment;

        private bool dragged;

        private Storyboard sb;

        #endregion

        #endregion

        #region Properties

        #region Public Properties

        public DataTemplate CustomTemplate
        {
            get { return (DataTemplate)GetValue(CustomTemplateProperty); }
            set { SetValue(CustomTemplateProperty, value); }
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

        #endregion

        #region Internal Override Properties

        /// <summary>
        /// Indicates that this series requires multiple y values.
        /// </summary>
        internal override bool IsMultipleYPathRequired
        {
            get
            {
                return !string.IsNullOrEmpty(High) && !string.IsNullOrEmpty(Low);
            }
        }

        #endregion

        #region Protected Internal Override Properties

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
        /// Creates the segments of RangeColumn Series.
        /// </summary>
        public override void CreateSegments()
        {
            double x1, x2, y1, y2;
            List<double> xValues = null;
            bool isGrouping = this.ActualXAxis is CategoryAxis && !(this.ActualXAxis as CategoryAxis).IsIndexed;
            if (isGrouping)
                xValues = GroupedXValuesIndexes;
            else
                xValues = GetXValues();

            if (xValues != null)
            {
                DoubleRange sbsInfo = this.GetSideBySideInfo(this);
                if (isGrouping)
                {
                    Segments.Clear();
                    Adornments.Clear();
                    for (int i = 0; i < this.DataCount; i++)
                    {
                        if (i < xValues.Count && GroupedSeriesYValues[0].Count > i)
                        {
                            x1 = xValues[i] + sbsInfo.Start;
                            x2 = xValues[i] + sbsInfo.End;
                            y1 = GroupedSeriesYValues[0][i];
                            y2 = IsMultipleYPathRequired ? GroupedSeriesYValues[1][i] : 0;
                            RangeColumnSegment rangeColumn = new RangeColumnSegment(x1, y1, x2, y2, this, ActualData[i]);
                            rangeColumn.High = GroupedSeriesYValues[0][i];
                            rangeColumn.Low = IsMultipleYPathRequired ? GroupedSeriesYValues[1][i] : 0;
                            Segments.Add(rangeColumn);
                            if (AdornmentsInfo != null)
                                AddAdornments(xValues[i], sbsInfo.Start + sbsInfo.Delta / 2, y1, y2, i);
                        }
                    }
                }
                else
                {
                    if (Segments.Count > this.DataCount)
                    {
                        ClearUnUsedSegments(this.DataCount);
                    }

                    if (AdornmentsInfo != null)
                    {
                        if (AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                            ClearUnUsedAdornments(this.DataCount * 2);
                        else
                            ClearUnUsedAdornments(this.DataCount);
                    }

                    for (int i = 0; i < this.DataCount; i++)
                    {
                        x1 = xValues[i] + sbsInfo.Start;
                        x2 = xValues[i] + sbsInfo.End;
                        y1 = HighValues[i];
                        y2 = IsMultipleYPathRequired ? LowValues[i] : 0;

                        if (i < Segments.Count)
                        {
                            (Segments[i].Item) = ActualData[i];
                            (Segments[i]).SetData(x1, y1, x2, y2);
                            (Segments[i] as RangeColumnSegment).High = y1;
                            (Segments[i] as RangeColumnSegment).Low = y2;
                            if (SegmentColorPath != null && !Segments[i].IsEmptySegmentInterior && ColorValues.Count > 0 && !Segments[i].IsSelectedSegment)
                                Segments[i].Interior = (Interior != null) ? Interior : ColorValues[i];
                        }
                        else
                        {
                            RangeColumnSegment rangeColumn = new RangeColumnSegment(x1, y1, x2, y2, this, ActualData[i]);
                            rangeColumn.High = y1;
                            rangeColumn.Low = y2;
                            Segments.Add(rangeColumn);
                        }

                        if (AdornmentsInfo != null)
                            AddAdornments(xValues[i], sbsInfo.Start + sbsInfo.Delta / 2, y1, y2, i);
                    }
                }

                if (ShowEmptyPoints)
                    UpdateEmptyPointSegments(xValues, true);
            }
        }

        #endregion

        #region Internal Override Methods

        internal override bool GetAnimationIsActive()
        {
            return sb != null && sb.GetCurrentState() == ClockState.Active;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Reviewed")]
        internal override void Animate()
        {
            int i = 0;

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
                element.RenderTransform = new ScaleTransform();
                if (segment is EmptyPointSegment && (!((segment as EmptyPointSegment).IsEmptySegmentInterior) || EmptyPointStyle == EmptyPointStyle.SymbolAndInterior))
                    elementSize = IsActualTransposed ? ((EmptyPointSegment)segment).EmptyPointSymbolWidth : ((EmptyPointSegment)segment).EmptyPointSymbolHeight;
                else
                    elementSize = IsActualTransposed ? ((ColumnSegment)segment).Width : ((ColumnSegment)segment).Height;

                if (!double.IsNaN(elementSize))
                {
                    element.RenderTransformOrigin = new Point(0.5, 0.5);
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
                        for (int j = this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom ? 0 : 1; j < 2; j++)
                        {
                            FrameworkElement label = this.AdornmentsInfo.LabelPresenters[i];

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
                            if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                                keyFrame1.Value = -(elementSize * 10) / 100;
                            else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                                keyFrame1.Value = (elementSize * 10) / 100;
                            else
                                keyFrame1.Value = i % 2 == 0 ? (elementSize * 10) / 100 : -(elementSize * 10) / 100;
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
                            i++;
                        }
                    }
                }
            }

            sb.Begin();
        }
        
        internal override void ResetDraggingElements(string reason, bool dragEndEvent)
        {
            base.ResetDraggingElements(reason, dragEndEvent);
            if (SeriesPanel.Children.Contains(previewRect))
                SeriesPanel.Children.Remove(previewRect);
            previewRect = null;
            draggingMode = -1;
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
            return base.CloneSeries(new RangeColumnSeries()
            {
                SelectedIndex = this.SelectedIndex,
                SegmentSpacing = this.SegmentSpacing,
                SegmentSelectionBrush = this.SegmentSelectionBrush,
            });
        }

        protected override void OnChartDragStart(Point mousePos, object originalSource)
        {
            if (EnableSegmentDragging)
                ActivateDragging(mousePos, originalSource);
            base.OnChartDragStart(mousePos, originalSource);
        }

        protected override void OnChartDragEnd(Point mousePos, object originalSource)
        {
            if (dragged)
                UpdateDraggedSource();
            ResetDraggingElements("", false);
            base.OnChartDragEnd(mousePos, originalSource);
        }

        protected override void OnChartDragEntered(Point mousePos, object originalSource)
        {
            var shape = originalSource as Shape;
            if (shape != null && shape.Tag is RangeColumnSegment)
            {
                var rectangle = originalSource as Rectangle;
                if (rectangle != null)
                {
                    UpdateDragSpliterHigh(rectangle);
                    UpdateDragSpliterLow(rectangle);
                }
            }

            base.OnChartDragEntered(mousePos, originalSource);
        }

        protected override void OnChartDragDelta(Point mousePos, object originalSource)
        {
            if (EnableSegmentDragging)
                SegmentPreview(mousePos);
            base.OnChartDragDelta(mousePos, originalSource);
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

        private static void OnCustomTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as RangeColumnSeries;

            if (series.Area == null) return;

            series.Segments.Clear();
            series.Area.ScheduleUpdate();
        }
        
        private static void OnSegmentSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as RangeColumnSeries;
            if (series.Area != null)
                series.Area.ScheduleUpdate();
        }
        
        private static void OnSegmentSelectionBrush(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RangeColumnSeries).UpdateArea();
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

        #endregion

        #region Private Methods
        
        private int GetSegmentMousePosition(RangeColumnSegment rangeColumnSegment, Point mousePos)
        {
            var high = rangeColumnSegment.High;
            var low = rangeColumnSegment.Low;
            var currentValue = Area.PointToValue(ActualYAxis, mousePos);
            var diffPercentValue = (Math.Abs(high - low) * 25 / 100);
            if (low < high)
            {
                var finalHigh = high - diffPercentValue;
                var finalLow = low + diffPercentValue;

                if (currentValue > finalHigh && high > currentValue)
                    return 1;
                if (currentValue < finalLow && currentValue > low)
                    return 2;
                return 3;
            }
            else
            {
                var finalHigh = high + diffPercentValue;
                var finalLow = low - diffPercentValue;

                if (currentValue < finalHigh && high < currentValue)
                    return 1;
                if (currentValue > finalLow && currentValue < low)
                    return 2;
                return 3;
            }
        }

        private void SegmentPreview(Point mousePos)
        {
            try
            {
                if (previewRect == null) return;
                DraggedValue = Area.PointToValue(ActualYAxis, mousePos);
                var currPos = IsActualTransposed ? mousePos.X : mousePos.Y;
                var selectedRect = selectedSegment.GetRenderedVisual() as Rectangle;
                double segmentLeft = 0d, segmentTop = 0d, segmentWidth = 0d, newHigh = double.NaN, newLow = double.NaN;
                ResetDragSpliter();
                dragged = true;

                if (IsActualTransposed)
                {
                    switch (draggingMode)
                    {
                        case 1:
                            {
                                segmentWidth = Canvas.GetLeft(selectedRect);
                                var movingOffset = segmentWidth - currPos;
                                if (currPos < segmentWidth)
                                {
                                    previewRect.SetValue(Canvas.LeftProperty, segmentWidth - movingOffset);
                                    previewRect.Width = Math.Abs(movingOffset);
                                }
                                else
                                {
                                    previewRect.SetValue(Canvas.LeftProperty, segmentWidth);
                                    previewRect.Width = Math.Abs(movingOffset);
                                }

                                newHigh = DraggedValue;
                                newLow = double.NaN;
                                UpdateSegmentDragValueToolTipHigh(new Point(mousePos.X, Canvas.GetTop(previewRect) + previewRect.Height / 2), Segments[SegmentIndex], DraggedValue, previewRect.Height / 2);
                            }

                            break;

                        case 2:
                            {
                                if (selectedRect != null)
                                    segmentWidth = Canvas.GetLeft(selectedRect) + selectedRect.ActualWidth;
                                var movingOffset = segmentWidth - currPos;
                                if (currPos < segmentWidth)
                                {
                                    previewRect.SetValue(Canvas.LeftProperty, segmentWidth - movingOffset);
                                    previewRect.Width = Math.Abs(movingOffset);
                                }
                                else
                                {
                                    previewRect.SetValue(Canvas.LeftProperty, segmentWidth);
                                    previewRect.Width = Math.Abs(movingOffset);
                                }

                                newLow = DraggedValue;
                                newHigh = double.NaN;
                                UpdateSegmentDragValueToolTipLow(new Point(mousePos.X, Canvas.GetTop(previewRect) + previewRect.Height / 2), Segments[SegmentIndex], DraggedValue);
                            }

                            break;

                        case 3:
                            {
                                segmentLeft = Canvas.GetLeft(previewRect);
                                var movingOffset = mousePos.X - initialHeight;
                                previewRect.SetValue(Canvas.LeftProperty, segmentLeft + movingOffset);
                                initialHeight = mousePos.X;
                                newHigh = HighValues[SegmentIndex] + DraggedValue - initialValue;
                                newLow = LowValues[SegmentIndex] + DraggedValue - initialValue;
                                UpdateSegmentDragValueToolTipHigh(new Point(Canvas.GetLeft(previewRect) + previewRect.Width, Canvas.GetTop((previewRect)) + previewRect.Height / 2), Segments[SegmentIndex], newHigh, previewRect.Width / 2);
                                UpdateSegmentDragValueToolTipLow(new Point(Canvas.GetLeft(previewRect), Canvas.GetTop(previewRect) + previewRect.Height / 2), Segments[SegmentIndex], newLow);
                            }

                            break;
                    }
                }
                else
                {
                    switch (draggingMode)
                    {
                        case 1:
                            {
                                if (selectedRect != null)
                                    segmentTop = Canvas.GetTop(selectedRect) + selectedRect.ActualHeight;
                                var movingOffset = segmentTop - currPos;
                                if (currPos < segmentTop)
                                {
                                    previewRect.SetValue(Canvas.TopProperty, segmentTop - movingOffset);
                                    previewRect.Height = Math.Abs(movingOffset);
                                }
                                else
                                {
                                    previewRect.SetValue(Canvas.TopProperty, segmentTop);
                                    previewRect.Height = Math.Abs(movingOffset);
                                }

                                newHigh = DraggedValue;
                                newLow = double.NaN;
                                UpdateSegmentDragValueToolTipHigh(new Point(Canvas.GetLeft(previewRect) + previewRect.Width / 2, mousePos.Y), Segments[SegmentIndex], DraggedValue, previewRect.Width / 2);
                            }

                            break;

                        case 2:
                            {
                                segmentTop = Canvas.GetTop(selectedRect);
                                var movingOffset = segmentTop - currPos;
                                if (currPos < segmentTop)
                                {
                                    previewRect.SetValue(Canvas.TopProperty, segmentTop - movingOffset);
                                    previewRect.Height = Math.Abs(movingOffset);
                                }
                                else
                                {
                                    previewRect.SetValue(Canvas.TopProperty, segmentTop);
                                    previewRect.Height = Math.Abs(movingOffset);
                                }

                                newLow = DraggedValue;
                                newHigh = double.NaN;
                                UpdateSegmentDragValueToolTipLow(new Point(Canvas.GetLeft(previewRect) + previewRect.Width / 2, mousePos.Y), Segments[SegmentIndex], DraggedValue);
                            }

                            break;

                        case 3:
                            {
                                segmentTop = Canvas.GetTop(previewRect);
                                var movingOffset = initialHeight - mousePos.Y;
                                previewRect.SetValue(Canvas.TopProperty, segmentTop - movingOffset);
                                initialHeight = mousePos.Y;
                                newHigh = HighValues[SegmentIndex] + DraggedValue - initialValue;
                                newLow = LowValues[SegmentIndex] + DraggedValue - initialValue;
                                UpdateSegmentDragValueToolTipHigh(new Point(Canvas.GetLeft(previewRect) + previewRect.Width / 2, Canvas.GetTop((previewRect))), Segments[SegmentIndex], newHigh, previewRect.Width / 2);
                                UpdateSegmentDragValueToolTipLow(new Point(Canvas.GetLeft(previewRect) + previewRect.Width / 2, Canvas.GetTop(previewRect) + previewRect.Height), Segments[SegmentIndex], newLow);
                            }

                            break;
                    }
                }

                var dragEvent = new RangeDragEventArgs { NewHighValue = newHigh, NewLowValue = newLow, BaseHighValue = HighValues[SegmentIndex], BaseLowValue = LowValues[SegmentIndex] };
                RaiseDragDelta(dragEvent);
                if (dragEvent.Cancel)
                    ResetDraggingElements("Cancel", true);
            }
            catch
            {
                ResetDraggingElements("Exception", true);
            }
        }


        private void ActivateDragging(Point mousePos, object element)
        {
            try
            {
                if (previewRect != null) return;
                var rectangle = element as Rectangle;
                if (rectangle == null || !EnableSegmentDragging) return;
                var rangeColumnSegment = rectangle.Tag as RangeColumnSegment;
                if (rangeColumnSegment == null) return;
                initialHeight = Canvas.GetTop(rectangle);
                var brush = rectangle.Fill as SolidColorBrush;
                previewRect = new Rectangle
                {
                    Fill = brush != null 
                    ? new SolidColorBrush(
                        Color.FromArgb(
                            brush.Color.A, 
                            (byte)(brush.Color.R * 0.6),
                            (byte)(brush.Color.G * 0.6), 
                            (byte)(brush.Color.B * 0.6))) 
                            : rectangle.Fill,
                    Opacity = 0.5,
                    Stroke = rectangle.Stroke,
                    StrokeThickness = rectangle.StrokeThickness
                };
                previewRect.SetValue(Canvas.LeftProperty, Canvas.GetLeft(rectangle));
                previewRect.SetValue(Canvas.TopProperty, initialHeight);
                previewRect.Height = rectangle.ActualHeight;
                previewRect.Width = rectangle.ActualWidth;
                SeriesPanel.Children.Add(previewRect);
                SegmentIndex = Segments.IndexOf(rectangle.Tag as ChartSegment);
                draggingMode = GetSegmentMousePosition(rangeColumnSegment, mousePos);
                selectedSegment = rangeColumnSegment;
                initialHeight = IsActualTransposed ? mousePos.X : mousePos.Y;
                initialValue = Area.PointToValue(ActualYAxis, mousePos);
                var dragEventArgs = new ChartDragStartEventArgs
                {
                    BaseXValue = GetActualXValue(SegmentIndex)
                };
                RaiseDragStart(dragEventArgs);
                if (dragEventArgs.Cancel)
                    ResetDraggingElements("Cancel", true);
#if NETFX_CORE
                Focus(FocusState.Keyboard);
#endif
                UnHoldPanning(false);
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
                double high = HighValues[SegmentIndex], low = LowValues[SegmentIndex];
                double baseHigh = high, baseLow = low;

                DraggedValue = GetSnapToPoint(DraggedValue);
                var offset = GetSnapToPoint(DraggedValue - initialValue);
                switch (draggingMode)
                {
                    case 1:
                        high = DraggedValue;
                        break;
                    case 2:
                        low = DraggedValue;
                        break;
                    case 3:
                        high = GetSnapToPoint(HighValues[SegmentIndex] + offset);
                        low = GetSnapToPoint(LowValues[SegmentIndex] + offset);
                        break;
                }

                var args = new RangeDragEventArgs { BaseHighValue = baseHigh, BaseLowValue = baseLow, NewHighValue = high, NewLowValue = low };
                RaisePreviewEnd(args);

                if (args.Cancel)
                {
                    ResetDraggingElements("", false);
                    return;
                }

                HighValues[SegmentIndex] = high;
                LowValues[SegmentIndex] = low;

                if (UpdateSource && !IsSortData)
                {
                    UpdateUnderLayingModel(Low, SegmentIndex, LowValues[SegmentIndex]);
                    UpdateUnderLayingModel(High, SegmentIndex, HighValues[SegmentIndex]);
                }

                dragged = false;
                UpdateArea();
                var dragEvent = new RangeDragEndEventArgs { BaseHighValue = baseHigh, BaseLowValue = baseLow, NewHighValue = high, NewLowValue = low };
                RaiseDragEnd(dragEvent);
            }
            catch
            {
                ResetDraggingElements("", false);
            }
        }

        #endregion

        #endregion 
    }
}
