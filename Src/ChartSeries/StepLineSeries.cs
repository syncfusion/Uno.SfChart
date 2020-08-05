using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// StepLineSeries displays its data points using line segments.
    /// </summary>
    /// <seealso cref="StepAreaSeries"/>   
    public partial class StepLineSeries : XyDataSeries, ISegmentSelectable
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="CustomTemplate"/> property.       .
        /// </summary>
        public static readonly DependencyProperty CustomTemplateProperty =
            DependencyProperty.Register(
                "CustomTemplate",
                typeof(DataTemplate),
                typeof(StepLineSeries),
                new PropertyMetadata(null, OnCustomTemplateChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property.       .
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex",
                typeof(int), 
                typeof(StepLineSeries),
                new PropertyMetadata(-1, OnSelectedIndexChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSelectionBrush"/> property.       .
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register(
                "SegmentSelectionBrush",
                typeof(Brush),
                typeof(StepLineSeries),
                new PropertyMetadata(null, OnSegmentSelectionBrush));

        #endregion

        #region Fields

        #region Private Fields

        Point hitPoint = new Point();
        
        private RectAnimation animation;

        Storyboard sb;

        #endregion

        #endregion

        #region Properties

        #region Public Properties

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
        ///                         YBindingPath="Year2010" ScatterWidth="40"&gt;
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

        #endregion

        #region Protected Internal Override Properties

        /// <summary>
        /// The property confirms the linearity of this series.
        /// </summary>
        /// <remarks>
        ///  Returns <c>true</c> if its linear, otherwise it returns <c>false</c>.
        /// </remarks>
        protected internal override bool IsLinear
        {
            get
            {
                return true;
            }
        }

        protected internal override List<ChartSegment> SelectedSegments
        {
            get
            {
                if (SelectedSegmentsIndexes.Count > 0 && Segments.Count != 0)
                    return (from index in SelectedSegmentsIndexes
                            where index <= Segments.Count
                            select index == Segments.Count ? Segments[index - 1] : Segments[index]).ToList();
                else
                    return null;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Creates the segments of StepLineSeries.
        /// </summary>      
        public override void CreateSegments()
        {
            List<double> xValues = null;
            bool isGrouping = this.ActualXAxis is CategoryAxis && !(this.ActualXAxis as CategoryAxis).IsIndexed;
            if (isGrouping)
                xValues = GroupedXValuesIndexes;
            else
                xValues = GetXValues();
            if (xValues == null) return;
            double xOffset = 0d;
            if (ActualXAxis is CategoryAxis &&
                (ActualXAxis as CategoryAxis).LabelPlacement == LabelPlacement.BetweenTicks)
                xOffset = 0.5d;

            if (isGrouping)
            {
                Segments.Clear();
                Adornments.Clear();

                for (int i = 0; i < xValues.Count; i++)
                {
                    int index = i + 1;
                    ChartPoint point1, point2, stepPoint;

                    if (AdornmentsInfo != null)
                    {
                        Adornments.Add(this.CreateAdornment(this, xValues[i], GroupedSeriesYValues[0][i], xValues[i], GroupedSeriesYValues[0][i]));
                        Adornments[i].Item = ActualData[i];
                    }

                    if (index < xValues.Count)
                    {
                        point1 = new ChartPoint(xValues[i] - xOffset, GroupedSeriesYValues[0][i]);
                        point2 = new ChartPoint(xValues[index] - xOffset, GroupedSeriesYValues[0][i]);
                        stepPoint = new ChartPoint(xValues[index] - xOffset, GroupedSeriesYValues[0][index]);
                    }
                    else
                    {
                        if (!(xOffset > 0)) continue;
                        point1 = new ChartPoint(xValues[i] - xOffset, GroupedSeriesYValues[0][i]);
                        point2 = new ChartPoint(xValues[i] + xOffset, GroupedSeriesYValues[0][i]);
                        stepPoint = new ChartPoint(xValues[i] - xOffset, GroupedSeriesYValues[0][i]);
                    }

                    var segment = new StepLineSegment(point1, stepPoint, point2, this)
                    {
                        Item = ActualData[i]
                    };

                    segment.Series = this;
                    List<ChartPoint> listPoints = new List<ChartPoint>();
                    listPoints.Add(point1);
                    listPoints.Add(stepPoint);
                    listPoints.Add(point2);
                    segment.SetData(listPoints);
                    Segments.Add(segment);
                    listPoints = null;
                }
            }
            else
            {
                ClearUnUsedStepLineSegment(DataCount);
                ClearUnUsedAdornments(DataCount);
                for (int i = 0; i < DataCount; i++)
                {
                    int index = i + 1;
                    ChartPoint point1, point2, stepPoint;

                    if (AdornmentsInfo != null)
                    {
                        if (i < Adornments.Count)
                        {
                            Adornments[i].SetData(xValues[i], YValues[i], xValues[i], YValues[i]);
                            Adornments[i].Item = ActualData[i];
                        }
                        else
                        {
                            Adornments.Add(this.CreateAdornment(this, xValues[i], YValues[i], xValues[i], YValues[i]));
                            Adornments[i].Item = ActualData[i];
                        }
                    }

                    if (index < DataCount)
                    {
                        point1 = new ChartPoint(xValues[i] - xOffset, YValues[i]);
                        point2 = new ChartPoint(xValues[index] - xOffset, YValues[i]);
                        stepPoint = new ChartPoint(xValues[index] - xOffset, YValues[index]);
                    }
                    else
                    {
                        if (!(xOffset > 0)) continue;
                        point1 = new ChartPoint(xValues[i] - xOffset, YValues[i]);
                        point2 = new ChartPoint(xValues[i] + xOffset, YValues[i]);
                        stepPoint = new ChartPoint(xValues[i] - xOffset, YValues[i]);
                    }

                    if (i < Segments.Count)
                    {
                        Segments[i].SetData(new List<ChartPoint> { point1, stepPoint, point2 });
                        Segments[i].Item = ActualData[i];
                        (Segments[i] as StepLineSegment).YData = YValues[i];
                        if (SegmentColorPath != null && !Segments[i].IsEmptySegmentInterior && ColorValues.Count > 0 && !Segments[i].IsSelectedSegment)
                            Segments[i].Interior = (Interior != null) ? Interior : ColorValues[i];
                    }
                    else
                    {
                        var segment = new StepLineSegment(point1, stepPoint, point2, this) { Item = ActualData[i] };
                        segment.Series = this;
                        List<ChartPoint> listPoints = new List<ChartPoint>();
                        listPoints.Add(point1);
                        listPoints.Add(stepPoint);
                        listPoints.Add(point2);
                        segment.SetData(listPoints);
                        Segments.Add(segment);
                        listPoints = null;
                    }
                }
            }

            if (ShowEmptyPoints)
                UpdateEmptyPointSegments(xValues, false);
        }

        #endregion

        #region Internal Override Methods

        internal override bool GetAnimationIsActive()
        {
            return animation != null && animation.IsActive;
        }

        internal override void ResetAdornmentAnimationState()
        {
            if (adornmentInfo != null)
            {
                foreach (var child in this.AdornmentPresenter.Children)
                {
                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null)
                    {
                        frameworkElement.ClearValue(FrameworkElement.RenderTransformProperty);
                        frameworkElement.ClearValue(FrameworkElement.OpacityProperty);
                    }
                }
            }
        }

        internal override void Animate()
        {
            if (animation != null)
            {
                animation.Stop();

                if (sb != null)
                    sb.Stop();

                if (!canAnimate)
                {
                    ResetAdornmentAnimationState();
                    return;
                }
            }

            var seriesRect = Area.SeriesClipRect;
            animation = new RectAnimation()
            {
                From = (IsActualTransposed) ? new Rect(0, seriesRect.Bottom, seriesRect.Width, seriesRect.Height) : new Rect(0, seriesRect.Y, 0, seriesRect.Height),
                To = (IsActualTransposed) ? new Rect(0, seriesRect.Y, 0, seriesRect.Height) : new Rect(0, seriesRect.Y, seriesRect.Width, seriesRect.Height),

                Duration = AnimationDuration.TotalSeconds == 1 ? TimeSpan.FromSeconds(0.4) : AnimationDuration
            };

            animation.SetTarget(SeriesRootPanel);
            animation.Begin();
            AnimateAdornments();
        }

        internal override void SelectedSegmentsIndexes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ChartSelectionChangedEventArgs chartSelectionChangedEventArgs;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null && !(ActualArea.SelectionBehaviour.SelectionStyle == SelectionStyle.Single))
                    {
                        int oldIndex = PreviousSelectedIndex;

                        int newIndex = (int)e.NewItems[0];

                        if (newIndex >= 0 && ActualArea.GetEnableSegmentSelection())
                        {
                            // Selects the adornment when the mouse is over or clicked on adornments(adornment selection).
                            if (adornmentInfo != null && adornmentInfo.HighlightOnSelection)
                            {
                                UpdateAdornmentSelection(newIndex);
                            }

                            if (ActualArea != null && newIndex < Segments.Count)
                            {
                                chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                                {
                                    SelectedSegment = Segments[newIndex],
                                    SelectedSegments = Area.SelectedSegments,
                                    SelectedSeries = this,
                                    SelectedIndex = newIndex,
                                    PreviousSelectedIndex = oldIndex,
                                    NewPointInfo = Segments[newIndex].Item,
                                    PreviousSelectedSegment = null,
                                    PreviousSelectedSeries = oldIndex != -1 ? this.ActualArea.PreviousSelectedSeries : null,
                                    IsSelected = true
                                };

                                if (oldIndex != -1)
                                {
                                    if (oldIndex == Segments.Count)
                                    {
                                        chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[oldIndex - 1];
                                        chartSelectionChangedEventArgs.OldPointInfo = Segments[oldIndex - 1].Item;
                                    }
                                    else
                                    {
                                        chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[oldIndex];
                                        chartSelectionChangedEventArgs.OldPointInfo = Segments[oldIndex].Item;
                                    }
                                }

                                (ActualArea as SfChart).OnSelectionChanged(chartSelectionChangedEventArgs);
                                PreviousSelectedIndex = newIndex;
                            }
                            else if (ActualArea != null && Segments.Count > 0 && newIndex == Segments.Count)
                            {
                                chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                                {
                                    SelectedSegment = Segments[newIndex - 1],
                                    SelectedSegments = Area.SelectedSegments,
                                    SelectedSeries = this,
                                    SelectedIndex = newIndex,
                                    PreviousSelectedIndex = oldIndex,
                                    NewPointInfo = Segments[newIndex - 1].Item,
                                    PreviousSelectedSeries = oldIndex != -1 ? this.ActualArea.PreviousSelectedSeries : null,
                                    PreviousSelectedSegment = null,
                                    IsSelected = true
                                };

                                if (oldIndex != -1)
                                {
                                    if (oldIndex == Segments.Count)
                                    {
                                        chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[oldIndex - 1];
                                        chartSelectionChangedEventArgs.OldPointInfo = Segments[oldIndex - 1].Item;
                                    }
                                    else
                                    {
                                        chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[oldIndex];
                                        chartSelectionChangedEventArgs.OldPointInfo = Segments[oldIndex].Item;
                                    }
                                }

                                (ActualArea as SfChart).OnSelectionChanged(chartSelectionChangedEventArgs);
                                PreviousSelectedIndex = newIndex;
                            }
                            else if (Segments.Count == 0)
                            {
                                triggerSelectionChangedEventOnLoad = true;
                            }
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:

                    if (e.OldItems != null && !(ActualArea.SelectionBehaviour.SelectionStyle == SelectionStyle.Single))
                    {
                        int newIndex = (int)e.OldItems[0];

                        chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                        {
                            SelectedSegment = null,
                            SelectedSegments = Area.SelectedSegments,
                            SelectedSeries = null,
                            SelectedIndex = newIndex,
                            PreviousSelectedIndex = PreviousSelectedIndex,
                            PreviousSelectedSegment = Segments[PreviousSelectedIndex],
                            OldPointInfo = Segments[PreviousSelectedIndex].Item,
                            PreviousSelectedSeries = this,
                            IsSelected = false
                        };

                        (ActualArea as SfChart).OnSelectionChanged(chartSelectionChangedEventArgs);
                        OnResetSegment(newIndex);
                        PreviousSelectedIndex = newIndex;
                    }

                    break;
            }
        }

        internal override void OnResetSegment(int index)
        {
            if (index >= 0 && adornmentInfo != null)
            {
                AdornmentPresenter.ResetAdornmentSelection(index, false);
            }
        }

        /// <summary>
        /// This method used to gets the chart data point at a position.
        /// </summary>
        /// <param name="mousePos"></param>
        /// <returns></returns>
        internal override ChartDataPointInfo GetDataPoint(Point mousePos)
        {
            Rect rect;
            int startIndex, endIndex;
            List<int> hitIndexes = new List<int>();
            IList<double> xValues = (ActualXValues is IList<double>) ? ActualXValues as IList<double> : GetXValues();

            CalculateHittestRect(mousePos, out startIndex, out endIndex, out rect);

            for (int i = startIndex; i <= endIndex; i++)
            {
                hitPoint.X = IsIndexed ? i : xValues[i];
                hitPoint.Y = YValues[i];

                if (rect.Contains(hitPoint))
                    hitIndexes.Add(i);
            }

            if (hitIndexes.Count > 0)
            {
                int i = hitIndexes[hitIndexes.Count / 2];
                hitIndexes = null;

                dataPoint = new ChartDataPointInfo();
                dataPoint.Index = i;
                dataPoint.XData = xValues[i];
                dataPoint.YData = YValues[i];
                dataPoint.Series = this;
                if (i > -1 && ActualData.Count > i)
                    dataPoint.Item = ActualData[i];

                return dataPoint;
            }
            else
                return dataPoint;
        }

        internal override void UpdateTooltip(object originalSource)
        {
            if (ShowTooltip)
            {
                FrameworkElement element = originalSource as FrameworkElement;
                object chartSegment = null;

                if (element != null)
                {
                    if (element.Tag is ChartSegment)
                        chartSegment = element.Tag;
                    else if (element.DataContext is ChartSegment && !(element.DataContext is ChartAdornment))
                        chartSegment = element.DataContext;
                    else
                    {
                        int index = ChartExtensionUtils.GetAdornmentIndex(element);
                        if (index != -1)
                        {
                            if (index < Segments.Count)
                                chartSegment = Segments[index];
                            else if (Segments.Count > 0)
                                chartSegment = Segments[index - 1];
                            else if (index < Adornments.Count)
                            {
                                // WPF-28526- Tooltip not shown when set the single data point with adornments for continuous series.
                                ChartAdornment adornment = Adornments[index];
                                chartSegment = new StepLineSegment()
                                {
                                    X1Value = adornment.XData,
                                    Y1Value = adornment.YData,
                                    X1Data = adornment.XData,
                                    Y1Data = adornment.YData,
                                    X1 = adornment.XData,
                                    Y1 = adornment.YData,
                                    XData = adornment.XData,
                                    YData = adornment.YData,
                                    Item = adornment.Item
                                };
                            }
                        }
                    }
                }

                if (chartSegment == null) return;

                SetTooltipDuration();
                var canvas = this.Area.GetAdorningCanvas();
                if (this.Area.Tooltip == null)
                    this.Area.Tooltip = new ChartTooltip();
                var chartTooltip = this.Area.Tooltip as ChartTooltip;
                if (chartTooltip != null)
                {
                    var lineSegment = chartSegment as StepLineSegment;
                    if (canvas.Children.Count == 0 || (canvas.Children.Count > 0 && !IsTooltipAvailable(canvas)))
                    {
                        SetTooltipSegmentItem(lineSegment);
                        chartTooltip.Content = chartSegment;

                        if (ChartTooltip.GetInitialShowDelay(this) == 0)
                        {
                            canvas.Children.Add(chartTooltip);
                        }

                        chartTooltip.ContentTemplate = this.GetTooltipTemplate();
                        AddTooltip();

                        if (ChartTooltip.GetEnableAnimation(this))
                        {
                            SetDoubleAnimation(chartTooltip);
                            storyBoard.Children.Add(leftDoubleAnimation);
                            storyBoard.Children.Add(topDoubleAnimation);
                            Canvas.SetLeft(chartTooltip, chartTooltip.LeftOffset);
                            Canvas.SetTop(chartTooltip, chartTooltip.TopOffset);

                            _stopwatch.Start();
                        }
                        else
                        {
                            Canvas.SetLeft(chartTooltip, chartTooltip.LeftOffset);
                            Canvas.SetTop(chartTooltip, chartTooltip.TopOffset);
                            _stopwatch.Start();
                        }
                    }
                    else
                    {
                        foreach (var child in canvas.Children)
                        {
                            var tooltip = child as ChartTooltip;
                            if (tooltip != null)
                                chartTooltip = tooltip;
                        }

                        SetTooltipSegmentItem(lineSegment);                
                        chartTooltip.Content = chartSegment;
                        chartTooltip.ContentTemplate = this.GetTooltipTemplate();
                        AddTooltip();
#if NETFX_CORE 
                        if (_stopwatch.ElapsedMilliseconds > 100)
                        {
#endif
                            if (ChartTooltip.GetEnableAnimation(this))
                            {
                                _stopwatch.Restart();

                                if (leftDoubleAnimation == null || topDoubleAnimation == null || storyBoard == null)
                                {
                                    SetDoubleAnimation(chartTooltip);
                                }
                                else
                                {
                                    leftDoubleAnimation.To = chartTooltip.LeftOffset;
                                    topDoubleAnimation.To = chartTooltip.TopOffset;
                                    storyBoard.Begin();
                                }
                            }
                            else
                            {
                                Canvas.SetLeft(chartTooltip, chartTooltip.LeftOffset);
                                Canvas.SetTop(chartTooltip, chartTooltip.TopOffset);
                            }
#if NETFX_CORE
                        }
                        else if (EnableAnimation == false)
                        {
                            Canvas.SetLeft(chartTooltip, chartTooltip.LeftOffset);
                            Canvas.SetTop(chartTooltip, chartTooltip.TopOffset);
                        }
#endif
                    }
                }
            }
        }

        private void SetTooltipSegmentItem(StepLineSegment lineSegment)
        {
            double xVal = 0;
            double yVal = 0;
            double stackValue = double.NaN;
            var point = new Point(mousePos.X - this.Area.SeriesClipRect.Left, mousePos.Y - this.Area.SeriesClipRect.Top);

            FindNearestChartPoint(point, out xVal, out yVal, out stackValue);
            if (double.IsNaN(xVal)) return;
            if (lineSegment != null)
                lineSegment.YData = yVal == lineSegment.Y1Value ? lineSegment.Y1Value : lineSegment.Y1Data;
            var segmentIndex = this.GetXValues().IndexOf(xVal);
            if (!IsIndexed)
            {
                IList<double> xValues = this.ActualXValues as IList<double>;
                int i = segmentIndex;
                double nearestY = this.ActualSeriesYValues[0][i];
                while (xValues.Count > i && xValues[i] == xVal)
                {
                    double yValue = ActualArea.PointToValue(ActualYAxis, point);
                    var validateYValue = ActualSeriesYValues[0][i];
                    if (Math.Abs(yValue - validateYValue) <= Math.Abs(yValue - nearestY))
                    {
                        segmentIndex = i;
                        nearestY = validateYValue;
                    }

                    i++;
                }
            }

            lineSegment.Item = this.ActualData[segmentIndex];
        }

        internal override void Dispose()
        {
            if (animation != null)
            {
                animation.Stop();
                animation = null;
            }

            if (sb != null)
            {
                sb.Stop();
                sb.Children.Clear();
                sb = null;
            }
            base.Dispose();
        }

        #endregion

        #region Protected Internal Override Methods

        /// <summary>
        /// Method used to set SegmentSelectionBrush to selectedindex chartsegment
        /// </summary>
        /// <param name="newIndex"></param>
        /// <param name="oldIndex"></param>
        protected internal override void SelectedIndexChanged(int newIndex, int oldIndex)
        {
            ChartSelectionChangedEventArgs chartSelectionChangedEventArgs;
            if (ActualArea != null && ActualArea.SelectionBehaviour != null)
            {
                // Resets the adornment selection when the mouse pointer moved away from the adornment or clicked the same adornment.
                if (ActualArea.SelectionBehaviour.SelectionStyle == SelectionStyle.Single)
                {
                    if (SelectedSegmentsIndexes.Contains(oldIndex))
                        SelectedSegmentsIndexes.Remove(oldIndex);

                    OnResetSegment(oldIndex);
                }

                if (newIndex >= 0 && ActualArea.GetEnableSegmentSelection())
                {
                    if (!SelectedSegmentsIndexes.Contains(newIndex))
                        SelectedSegmentsIndexes.Add(newIndex);

                    // Selects the adornment when the mouse is over or clicked on adornments(adornment selection).
                    if (adornmentInfo != null && adornmentInfo.HighlightOnSelection)
                    {
                        UpdateAdornmentSelection(newIndex);
                    }

                    if (ActualArea != null && newIndex < Segments.Count)
                    {
                        chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                        {
                            SelectedSegment = Segments[newIndex],
                            SelectedSegments = Area.SelectedSegments,
                            SelectedSeries = this,
                            SelectedIndex = newIndex,
                            PreviousSelectedIndex = oldIndex,
                            NewPointInfo = Segments[newIndex].Item,
                            PreviousSelectedSegment = null,
                            PreviousSelectedSeries = oldIndex != -1 ? this.ActualArea.PreviousSelectedSeries : null,
                            IsSelected = true
                        };

                        if (oldIndex >= 0 && oldIndex <= Segments.Count)
                        {
                            if (oldIndex == Segments.Count)
                            {
                                chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[oldIndex - 1];
                                chartSelectionChangedEventArgs.OldPointInfo = Segments[oldIndex - 1].Item;
                            }
                            else
                            {
                                chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[oldIndex];
                                chartSelectionChangedEventArgs.OldPointInfo = Segments[oldIndex].Item;
                            }
                        }

                        (ActualArea as SfChart).OnSelectionChanged(chartSelectionChangedEventArgs);
                    }
                    else if (ActualArea != null && Segments.Count > 0 && newIndex == Segments.Count)
                    {
                        chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                        {
                            SelectedSegment = Segments[newIndex - 1],
                            SelectedSegments = Area.SelectedSegments,
                            SelectedSeries = this,
                            SelectedIndex = newIndex,
                            PreviousSelectedIndex = oldIndex,
                            NewPointInfo = Segments[newIndex - 1].Item,
                            PreviousSelectedSeries = oldIndex != -1 ? this.ActualArea.PreviousSelectedSeries : null,
                            PreviousSelectedSegment = null,
                            IsSelected = true
                        };

                        if (oldIndex >= 0 && oldIndex <= Segments.Count)
                        {
                            if (oldIndex == Segments.Count)
                            {
                                chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[oldIndex - 1];
                                chartSelectionChangedEventArgs.OldPointInfo = Segments[oldIndex - 1].Item;
                            }
                            else
                            {
                                chartSelectionChangedEventArgs.PreviousSelectedSegment = Segments[oldIndex];
                                chartSelectionChangedEventArgs.OldPointInfo = Segments[oldIndex].Item;
                            }
                        }

                        (ActualArea as SfChart).OnSelectionChanged(chartSelectionChangedEventArgs);
                        PreviousSelectedIndex = newIndex;
                    }
                    else if (Segments.Count == 0)
                    {
                        triggerSelectionChangedEventOnLoad = true;
                    }
                }
                else if (newIndex == -1 && ActualArea != null && oldIndex < Segments.Count)
                {
                    (ActualArea as SfChart).OnSelectionChanged(new ChartSelectionChangedEventArgs()
                    {
                        SelectedSegment = null,
                        SelectedSegments = Area.SelectedSegments,
                        SelectedSeries = null,
                        SelectedIndex = newIndex,
                        PreviousSelectedIndex = oldIndex,
                        PreviousSelectedSegment = Segments[oldIndex],
                        OldPointInfo = Segments[oldIndex].Item,
                        PreviousSelectedSeries = this,
                        IsSelected = false
                    });
                    PreviousSelectedIndex = newIndex;
                }
                else if (newIndex == -1 && ActualArea != null && Segments.Count > 0 && oldIndex == Segments.Count)
                {
                    (ActualArea as SfChart).OnSelectionChanged(new ChartSelectionChangedEventArgs()
                    {
                        SelectedSegment = null,
                        SelectedSegments = Area.SelectedSegments,
                        SelectedSeries = null,
                        SelectedIndex = newIndex,
                        PreviousSelectedIndex = oldIndex,
                        PreviousSelectedSegment = Segments[oldIndex - 1],
                        OldPointInfo = Segments[oldIndex - 1].Item,
                        PreviousSelectedSeries = this,
                        IsSelected = false
                    });
                    PreviousSelectedIndex = newIndex;
                }
            }
            else if (newIndex >= 0 && Segments.Count == 0)
            {
                triggerSelectionChangedEventOnLoad = true;
            }
        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Event to show tooltip
        /// </summary>
        /// <param name="e"> Event Arguments</param>
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            var canvas = this.Area.GetAdorningCanvas();
            mousePos = e.GetCurrentPoint(canvas).Position;
            if (e.Pointer.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
                UpdateTooltip(e.OriginalSource);
        }

#if NETFX_CORE
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                UpdateTooltip(e.OriginalSource);
        }
#endif
        
        protected override void SetDoubleAnimation(ChartTooltip chartTooltip)
        {
            storyBoard = new Storyboard();
            leftDoubleAnimation = new DoubleAnimation
            {
                To = chartTooltip.LeftOffset,
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200)),
                EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(leftDoubleAnimation, chartTooltip);
#if !NETFX_CORE
            Storyboard.SetTargetProperty(leftDoubleAnimation, new PropertyPath("(Canvas.Left)"));
#else
            Storyboard.SetTargetProperty(leftDoubleAnimation, "(Canvas.Left)");
#endif
            topDoubleAnimation = new DoubleAnimation
            {
                To = chartTooltip.TopOffset,
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200)),
                EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(topDoubleAnimation, chartTooltip);
#if !NETFX_CORE 
            Storyboard.SetTargetProperty(topDoubleAnimation, new PropertyPath("(Canvas.Top)"));
#else
            Storyboard.SetTargetProperty(topDoubleAnimation, "(Canvas.Top)");
#endif
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new StepLineSeries()
            {
                CustomTemplate = this.CustomTemplate,
                SegmentSelectionBrush = this.SegmentSelectionBrush,
                SelectedIndex = this.SelectedIndex,
            });
        }

        #endregion

        #region Private Static Methods

        private static void OnCustomTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as StepLineSeries;

            if (series.Area == null) return;

            series.Segments.Clear();
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
        
        private static void OnSegmentSelectionBrush(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StepLineSeries).UpdateArea();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Removes the unused segments
        /// </summary>       
        private void ClearUnUsedStepLineSegment(int startIndex)
        {
            var emptySegments = new List<ChartSegment>();

            foreach (var segment in Segments.Where(item => item is EmptyPointSegment))
            {
                emptySegments.Add(segment);
            }

            foreach (var segment in emptySegments)
            {
                Segments.Remove(segment);
            }

            if (this.Segments.Count >= startIndex && this.Segments.Count != 0)
            {
                int count = this.Segments.Count;

                for (int i = startIndex; i <= count; i++)
                {
                    if (i == count)
                        this.Segments.RemoveAt(startIndex - 1);
                    else
                        this.Segments.RemoveAt(startIndex);
                }
            }
        }

        private void AnimateAdornments()
        {
            if (this.AdornmentsInfo != null)
            {
                sb = new Storyboard();
                double totalDuration = AnimationDuration.TotalSeconds;

                // UWP-185-RectAnimation takes some delay to render series.
                totalDuration *= 1.2;

                foreach (var child in this.AdornmentPresenter.Children)
                {
                    DoubleAnimationUsingKeyFrames keyFrames1 = new DoubleAnimationUsingKeyFrames();
                    SplineDoubleKeyFrame frame1 = new SplineDoubleKeyFrame();

                    frame1.KeyTime = TimeSpan.FromSeconds(0);
                    frame1.Value = 0;
                    keyFrames1.KeyFrames.Add(frame1);

                    frame1 = new SplineDoubleKeyFrame();
                    frame1.KeyTime = TimeSpan.FromSeconds(totalDuration);
                    frame1.Value = 0;
                    keyFrames1.KeyFrames.Add(frame1);

                    frame1 = new SplineDoubleKeyFrame();
                    frame1.KeyTime = TimeSpan.FromSeconds(totalDuration + 1);
                    frame1.Value = 1;
                    keyFrames1.KeyFrames.Add(frame1);

                    KeySpline keySpline = new KeySpline();
                    keySpline.ControlPoint1 = new Point(0.64, 0.84);
                    keySpline.ControlPoint2 = new Point(0, 1); // Animation have to provide same easing effect in all platforms.
                    keyFrames1.EnableDependentAnimation = true;
                    Storyboard.SetTargetProperty(keyFrames1, "(Opacity)");
                    frame1.KeySpline = keySpline;

                    Storyboard.SetTarget(keyFrames1, child as FrameworkElement);
                    sb.Children.Add(keyFrames1);
                }

                sb.Begin();
            }
        }

        #endregion

        #endregion
    }
}
