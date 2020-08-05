using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// SplineSeries is similar to that of <see cref="LineSeries"/> except that the points here are connected using smooth Bezier curves.
    /// </summary>
    /// <seealso cref="SplineSegment"/>

    public partial class SplineSeries : XySeriesDraggingBase, ISegmentSelectable
    {
        #region Dependency Property Registration

        /// <summary>
        ///  The DependencyProperty for <see cref="CustomTemplate"/> property.       
        /// </summary>
        public static readonly DependencyProperty CustomTemplateProperty =
            DependencyProperty.Register(
                "CustomTemplate",
                typeof(DataTemplate), 
                typeof(SplineSeries),
                new PropertyMetadata(null, OnCustomTemplateChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property.       
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex",
                typeof(int),
                typeof(SplineSeries),
                new PropertyMetadata(-1, OnSelectedIndexChanged));

        /// <summary>
        ///The DependencyProperty for <see cref="SegmentSelectionBrush"/> property.       
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register(
                "SegmentSelectionBrush", 
                typeof(Brush),
                typeof(SplineSeries),
                new PropertyMetadata(null, OnSegmentSelectionBrush));

        /// <summary>
        /// Using a DependencyProperty as the backing store for SplineType.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SplineTypeProperty =
            DependencyProperty.Register(
                "SplineType", 
                typeof(SplineType),
                typeof(SplineSeries), 
                new PropertyMetadata(SplineType.Natural, OnSplineTypeChanged));
        
        /// <summary>
        /// The Dependency property for<see cref="StrokeDashArray"/>
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register(
                "StrokeDashArray",
                typeof(DoubleCollection),
                typeof(SplineSeries),
                new PropertyMetadata(null));

        #endregion

        #region Fields

        Storyboard sb;

        double offsetPosition, initialPosition;

        bool isDragged, isSeriesCaptured;

        List<SplineSegment> segments;

        List<double> previewYValues;

        Point hitPoint = new Point();

        List<ChartPoint> startControlPoints;

        List<ChartPoint> endControlPoints;

        private RectAnimation animation;

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
        ///         &lt;syncfusion:ScatterSeries ItemsSource="{Binding Demands}" XBindingPath="Demand" 
        ///                         ScatterHeight="40" YBindingPath="Year2010" ScatterWidth="40"&gt;
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

        /// <summary>
        /// Gets or sets SplineType enum value which indicates the spline series type. 
        /// </summary>
        public SplineType SplineType
        {
            get { return (SplineType)GetValue(SplineTypeProperty); }
            set { SetValue(SplineTypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke dash array for the spline series.
        /// </summary>
        public DoubleCollection StrokeDashArray
        {
            get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
            set { SetValue(StrokeDashArrayProperty, value); }
        }

        #endregion

        #region Protected Internal Override

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
        /// Creates the segments of SplineSeries.
        /// </summary>
        public override void CreateSegments()
        {
            int index = -1;
            double[] yCoef = null;
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

                    if (SplineType == SplineType.Monotonic)
                        GetMonotonicSpline(xValues, GroupedSeriesYValues[0]);
                    else if (SplineType == SplineType.Cardinal)
                        GetCardinalSpline(xValues, GroupedSeriesYValues[0]);
                    else
                        this.NaturalSpline(xValues, GroupedSeriesYValues[0], out yCoef);//Calculate natural curve points. 

                    for (int i = 0; i < xValues.Count; i++)
                    {
                        index = i + 1;
                        ChartPoint startPoint = new ChartPoint(xValues[i], GroupedSeriesYValues[0][i]);
                        if (index < xValues.Count && index < GroupedSeriesYValues[0].Count)
                        {
                            ChartPoint endPoint = new ChartPoint(xValues[index], GroupedSeriesYValues[0][index]);
                            ChartPoint startControlPoint;
                            ChartPoint endControlPoint;
                            if (SplineType == SplineType.Monotonic)
                            {
                                startControlPoint = startControlPoints[index - 1];
                                endControlPoint = endControlPoints[index - 1];
                            }
                            else if (SplineType == SplineType.Cardinal)
                            {
                                startControlPoint = startControlPoints[index - 1];
                                endControlPoint = endControlPoints[index - 1];
                            }
                            else
                                GetBezierControlPoints(
                                    startPoint, 
                                    endPoint,
                                    yCoef[i],
                                    yCoef[index],
                                    out startControlPoint,
                                    out endControlPoint);
                            SplineSegment splineSegment = new SplineSegment(startPoint, startControlPoint, endControlPoint, endPoint, this);
                            splineSegment.SetData(startPoint, startControlPoint, endControlPoint, endPoint);
                            splineSegment.X1 = xValues[i];
                            splineSegment.X2 = xValues[index];
                            splineSegment.Y1 = GroupedSeriesYValues[0][i];
                            splineSegment.Y2 = GroupedSeriesYValues[0][index];
                            splineSegment.Item = this.ActualData[i];
                            Segments.Add(splineSegment);
                        }
                        else if (index == Segments.Count)
                            Segments.RemoveAt(i);
                        if (AdornmentsInfo != null)
                            AddAdornmentAtXY(startPoint.X, startPoint.Y, i);
                    }
                }
                else
                {
                    ClearUnUsedSegments(this.DataCount);
                    ClearUnUsedAdornments(this.DataCount);
                    if (SplineType == SplineType.Monotonic)
                        GetMonotonicSpline(xValues, YValues);
                    else if (SplineType == SplineType.Cardinal)
                        GetCardinalSpline(xValues, YValues);
                    else
                        this.NaturalSpline(xValues, YValues, out yCoef);

                    for (int i = 0; i < DataCount; i++)
                    {
                        index = i + 1;
                        ChartPoint startPoint = new ChartPoint(xValues[i], YValues[i]);
                        if (index < DataCount)
                        {
                            ChartPoint endPoint = new ChartPoint(xValues[index], YValues[index]);
                            ChartPoint startControlPoint;
                            ChartPoint endControlPoint;

                            // Calculate curve points. 
                            if (SplineType == SplineType.Monotonic)
                            {
                                startControlPoint = startControlPoints[index - 1];
                                endControlPoint = endControlPoints[index - 1];
                            }
                            else if (SplineType == SplineType.Cardinal)
                            {
                                startControlPoint = startControlPoints[index - 1];
                                endControlPoint = endControlPoints[index - 1];
                            }
                            else
                                GetBezierControlPoints(startPoint, endPoint, yCoef[i], yCoef[index], out startControlPoint, out endControlPoint);

                            if (i < Segments.Count && Segments[i] is SplineSegment)
                            {
                                (Segments[i]).SetData(startPoint, startControlPoint, endControlPoint, endPoint);
                                (Segments[i] as SplineSegment).X1 = xValues[i];
                                (Segments[i] as SplineSegment).X2 = xValues[index];
                                (Segments[i] as SplineSegment).Y1 = YValues[i];
                                (Segments[i] as SplineSegment).Y2 = YValues[index];
                                (Segments[i] as SplineSegment).Item = this.ActualData[i];
                                (Segments[i] as SplineSegment).YData = YValues[i];
                                if (SegmentColorPath != null && !Segments[i].IsEmptySegmentInterior && ColorValues.Count > 0 && !Segments[i].IsSelectedSegment)
                                    Segments[i].Interior = (Interior != null) ? Interior : ColorValues[i];
                            }
                            else
                            {
                                SplineSegment splineSegment = new SplineSegment(startPoint, startControlPoint, endControlPoint, endPoint, this);
                                splineSegment.SetData(startPoint, startControlPoint, endControlPoint, endPoint);
                                splineSegment.X1 = xValues[i];
                                splineSegment.X2 = xValues[index];
                                splineSegment.Y1 = YValues[i];
                                splineSegment.Y2 = YValues[index];
                                splineSegment.Item = this.ActualData[i];
                                Segments.Add(splineSegment);
                            }
                        }
                        else if (index == Segments.Count)
                            Segments.RemoveAt(i);
                        if (AdornmentsInfo != null)
                            AddAdornmentAtXY(startPoint.X, startPoint.Y, i);
                    }
                }
                
                if (ShowEmptyPoints)
                    UpdateEmptyPointSegments(xValues, false);
            }
        }

        #endregion

        #region Internal Override Methods

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
                                    PreviousSelectedSegment = null,
                                    PreviousSelectedSeries = null,
                                    NewPointInfo = Segments[newIndex].Item,
                                    IsSelected = true
                                };

                                chartSelectionChangedEventArgs.PreviousSelectedSeries = this.ActualArea.PreviousSelectedSeries;

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
                                    PreviousSelectedSegment = null,
                                    PreviousSelectedSeries = null,
                                    IsSelected = true
                                };

                                chartSelectionChangedEventArgs.PreviousSelectedSeries = this.ActualArea.PreviousSelectedSeries;

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

                        if (PreviousSelectedIndex != -1 && PreviousSelectedIndex < Segments.Count)
                            selectionChangedEventArgs.PreviousSelectedSegment = Segments[PreviousSelectedIndex];

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
                int index = -1;
                if (element != null)
                {
                    if (element.Tag is ChartSegment)
                        chartSegment = element.Tag;
                    else if (element.DataContext is ChartSegment && !(element.DataContext is ChartAdornment))
                        chartSegment = element.DataContext;
                    else
                    {
                        index = ChartExtensionUtils.GetAdornmentIndex(element);
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
                                chartSegment = new SplineSegment()
                                {
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
                    var lineSegment = chartSegment as SplineSegment;

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

        private void SetTooltipSegmentItem(SplineSegment lineSegment)
        {
            double xVal = 0;
            double yVal = 0;
            double stackValue = double.NaN;
            var point = new Point(mousePos.X - this.Area.SeriesClipRect.Left, mousePos.Y - this.Area.SeriesClipRect.Top);

            FindNearestChartPoint(point, out xVal, out yVal, out stackValue);
            if (lineSegment != null)
                lineSegment.YData = yVal == lineSegment.Y1 ? lineSegment.Y1 : lineSegment.Y2;
            lineSegment.XData = xVal;
            var segmentIndex = this.GetXValues().IndexOf(xVal);
            if (!IsIndexed)
            {
                IList<double> xValues = this.ActualXValues as IList<double>;
                int i = segmentIndex;
                double nearestY = this.ActualSeriesYValues[0][i];
                while (!IsIndexed && xValues.Count > i && xValues[i] == xVal)
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

        internal override bool GetAnimationIsActive()
        {
            return animation != null && animation.IsActive;
        }

        internal override void Animate()
        {
            var seriesRect = Area.SeriesClipRect;

            if (animation != null)
            {
                animation.Stop();

                if (!canAnimate)
                {
                    ResetAdornmentAnimationState();
                    return;
                }
            }

            animation = new RectAnimation()
            {
                From = (IsActualTransposed) ? new Rect(0, seriesRect.Bottom, seriesRect.Width, seriesRect.Height) : new Rect(0, seriesRect.Y, 0, seriesRect.Height),
                To = (IsActualTransposed) ? new Rect(0, seriesRect.Y, 0, seriesRect.Height) : new Rect(0, seriesRect.Y, seriesRect.Width, seriesRect.Height),
                Duration = AnimationDuration.TotalSeconds == 1 ? TimeSpan.FromSeconds(0.4) : AnimationDuration
            };

            animation.SetTarget(SeriesRootPanel);
            animation.Begin();

            if (this.AdornmentsInfo != null)
            {
                var adornTransXPath = "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)";
                var adornTransYPath = "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)";

                sb = new Storyboard();
                double secondsPerPoint = (AnimationDuration.TotalSeconds / YValues.Count);

                // UWP-185-RectAnimation takes some delay to render series.
                secondsPerPoint *= 1.2;
                int i = 0;
                foreach (FrameworkElement label in this.AdornmentsInfo.LabelPresenters)
                {
                    var transformGroup = label.RenderTransform as TransformGroup;
                    var scaleTransform = new ScaleTransform() { ScaleX = 0, ScaleY = 0 };

                    if (transformGroup != null)
                    {
                        if (transformGroup.Children.Count > 0 && transformGroup.Children[0] is ScaleTransform)
                        {
                            transformGroup.Children[0] = scaleTransform;
                        }
                        else
                        {
                            transformGroup.Children.Insert(0, scaleTransform);
                        }
                    }

                    label.RenderTransformOrigin = new Point(0.5, 0.5);
                    DoubleAnimation keyFrames1 = new DoubleAnimation()
                    {
                        From = 0.3,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(AnimationDuration.TotalSeconds / 2),
                        BeginTime = TimeSpan.FromSeconds(i * secondsPerPoint)
                    };

                    keyFrames1.EnableDependentAnimation = true;
                    Storyboard.SetTargetProperty(keyFrames1, adornTransXPath);
                    Storyboard.SetTarget(keyFrames1, label);
                    sb.Children.Add(keyFrames1);
                    keyFrames1 = new DoubleAnimation()
                    {
                        From = 0.3,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(AnimationDuration.TotalSeconds / 2),
                        BeginTime = TimeSpan.FromSeconds(i * secondsPerPoint)
                    };

                    keyFrames1.EnableDependentAnimation = true;
                    Storyboard.SetTargetProperty(keyFrames1, adornTransYPath);
                    Storyboard.SetTarget(keyFrames1, label);
                    sb.Children.Add(keyFrames1);
                    i++;
                }

                sb.Begin();
            }
        }

        internal override void ActivateDragging(Point mousePos, object chartElement)
        {
            if (DraggingSegment != null || segments != null) return;
            var element = chartElement as FrameworkElement;
            if (element == null)
                return;
            base.ActivateDragging(mousePos, chartElement);
            if (SegmentIndex < 0) return;

            DraggingSegment = element.Tag as SplineSegment;
            if (EnableSeriesDragging && (DraggingSegment != null
                || element.DataContext is ChartAdornmentInfo
                || element.DataContext is ChartAdornmentContainer))
            {
                PreviewSeries = this;
                isSeriesCaptured = true;
                offsetPosition = initialPosition = IsActualTransposed ? mousePos.X : mousePos.Y;
            }
            else if (DraggingSegment != null
                    || element.DataContext is ChartAdornmentInfo
                    || element.DataContext is ChartAdornmentContainer)
            {
                double x, y, stackedValue;
                FindNearestChartPoint(mousePos, out x, out y, out stackedValue);
                SegmentIndex = (int)(IsIndexed || ActualXValues is IList<string> ? x : ((IList<double>)ActualXValues).IndexOf(x));
                if (SegmentIndex == Segments.Count)
                    DraggingSegment = Segments[SegmentIndex - 1] as SplineSegment;
                else
                    DraggingSegment = Segments[SegmentIndex] as SplineSegment;
            }
        }

        internal override void UpdatePreviewSegmentDragging(Point mousePos)
        {
            UpdatePreviewSegmentAndSeries(mousePos);
            base.UpdatePreviewSegmentDragging(mousePos);
        }

        internal override void UpdatePreivewSeriesDragging(Point mousePos)
        {
            UpdatePreviewSegmentAndSeries(mousePos);
            base.UpdatePreivewSeriesDragging(mousePos);
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
        /// Method used to set SegmentSelectionBrush to SelectedIndex segment
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
                            PreviousSelectedSegment = null,
                            PreviousSelectedSeries = null,
                            NewPointInfo = Segments[newIndex].Item,
                            IsSelected = true
                        };

                        chartSelectionChangedEventArgs.PreviousSelectedSeries = this.ActualArea.PreviousSelectedSeries;

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
                    else if (ActualArea != null && Segments.Count > 0 && newIndex == Segments.Count)
                    {
                        chartSelectionChangedEventArgs = new ChartSelectionChangedEventArgs()
                        {
                            SelectedSegment = Segments[newIndex - 1],
                            SelectedSegments = Area.SelectedSegments,
                            SelectedSeries = this,
                            SelectedIndex = newIndex,
                            PreviousSelectedIndex = oldIndex,
                            PreviousSelectedSegment = null,
                            PreviousSelectedSeries = null,
                            IsSelected = true
                        };

                        chartSelectionChangedEventArgs.PreviousSelectedSeries = this.ActualArea.PreviousSelectedSeries;

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
            base.OnPointerMoved(e);
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
            return base.CloneSeries(new SplineSeries()
            {
                CustomTemplate = this.CustomTemplate,
                SegmentSelectionBrush = this.SegmentSelectionBrush,
                SelectedIndex = this.SelectedIndex,
            });
        }

        protected override void ResetDraggingElements(string reason, bool dragEndEvent)
        {
            if (SeriesPanel == null) return;
            base.ResetDraggingElements(reason, dragEndEvent);
            if (segments != null)
            {
                foreach (var segment in segments)
                {
                    SeriesPanel.Children.Remove(segment.GetRenderedVisual());
                }

                segments.Clear();
                segments = null;
            }

            isSeriesCaptured = false;
            DraggingSegment = null;
            isDragged = false;
            PreviewSeries = null;
        }

        protected override void OnChartDragStart(Point mousePos, object originalSource)
        {
            if (EnableSeriesDragging || EnableSegmentDragging)
            {
                ActivateDragging(mousePos, originalSource);
            }
        }

        protected override void OnChartDragEnd(Point mousePos, object originalSource)
        {
            UpdateDraggedSource();
            base.OnChartDragEnd(mousePos, originalSource);
        }

        #endregion

        #region Protected Methods

        protected void GetCardinalSpline(List<double> xValues, IList<double> yValues)
        {
            int count = 0;
            startControlPoints = new List<ChartPoint>(DataCount);
            endControlPoints = new List<ChartPoint>(DataCount);
            bool isGrouping = this.ActualXAxis is CategoryAxis ? (this.ActualXAxis as CategoryAxis).IsIndexed : true;

            if (!isGrouping)
                count = (int)xValues.Count;
            else
                count = (int)DataCount;

            double[] tangentsX = new double[count];
            double[] tangentsY = new double[count];

            for (int i = 0; i < count; i++)
            {
                if (i == 0 && xValues.Count > 2)
                    tangentsX[i] = (0.5 * (xValues[i + 2] - xValues[i]));
                else if (i == count - 1 && count - 3 >= 0)
                    tangentsX[i] = (0.5 * (xValues[count - 1] - xValues[count - 3]));
                else if (i - 1 >= 0 && xValues.Count > i + 1)
                    tangentsX[i] = (0.5 * (xValues[i + 1] - xValues[i - 1]));
                if (double.IsNaN(tangentsX[i]))
                    tangentsX[i] = 0;
                if (ActualXAxis is DateTimeAxis)
                {
                    DateTime date = xValues[i].FromOADate();
                    if ((ActualXAxis as DateTimeAxis).IntervalType == DateTimeIntervalType.Auto ||
                            (ActualXAxis as DateTimeAxis).IntervalType == DateTimeIntervalType.Years)
                    {
                        int year = DateTime.IsLeapYear(date.Year) ? 366 : 365;
                        tangentsY[i] = tangentsX[i] / year;
                    }
                    else if ((ActualXAxis as DateTimeAxis).IntervalType == DateTimeIntervalType.Months)
                    {
                        double month = DateTime.DaysInMonth(date.Year, date.Month);
                        tangentsY[i] = tangentsX[i] / month;
                    }
                }
                else if (ActualXAxis is LogarithmicAxis)
                {
                    tangentsX[i] = Math.Log(tangentsX[i], (ActualXAxis as LogarithmicAxis).LogarithmicBase);
                    tangentsY[i] = tangentsX[i];
                }
                else
                    tangentsY[i] = tangentsX[i];
            }

            for (int i = 0; i < tangentsX.Length - 1; i++)
            {
                startControlPoints.Add(new ChartPoint(xValues[i] + tangentsX[i] / 3, yValues[i] + tangentsY[i] / 3));
                endControlPoints.Add(new ChartPoint(xValues[i + 1] - tangentsX[i + 1] / 3, yValues[i + 1] - tangentsY[i + 1] / 3));
            }
        }

        protected void GetMonotonicSpline(List<double> xValues, IList<double> yValues)
        {
            int count = 0;
            startControlPoints = new List<ChartPoint>(DataCount);
            endControlPoints = new List<ChartPoint>(DataCount);
            bool isGrouping = this.ActualXAxis is CategoryAxis ? (this.ActualXAxis as CategoryAxis).IsIndexed : true;
            if (!isGrouping)
                count = (int)xValues.Count;
            else
                count = (int)DataCount;
            double[] dx = new double[count - 1];
            double[] slope = new double[count - 1];
            List<double> coefficent = new List<double>();

            // Find the slope between the values.
            for (int i = 0; i < count - 1; i++)
            {
                if (!double.IsNaN(yValues[i + 1]) && !double.IsNaN(yValues[i])
                    && !double.IsNaN(xValues[i + 1]) && !double.IsNaN(xValues[i]))
                {
                    dx[i] = xValues[i + 1] - xValues[i];
                    slope[i] = (yValues[i + 1] - yValues[i]) / dx[i];
                    if (double.IsInfinity(slope[i]))
                        slope[i] = 0;
                }
            }

            // Add the first and last coefficent value as Slope[0] and Slope[n-1]
            if (slope.Length == 0) return;
            coefficent.Add(double.IsNaN(slope[0]) ? 0 : slope[0]);
            for (int i = 0; i < dx.Length - 1; i++)
            {
                if (slope.Length > i + 1)
                {
                    double m = slope[i], m_next = slope[i + 1];
                    if (m * m_next <= 0)
                    {
                        coefficent.Add(0);
                    }
                    else
                    {
                        if (dx[i] == 0)
                            coefficent.Add(0);
                        else
                        {
                            double firstPoint = dx[i], nextPoint = dx[i + 1];
                            double interPoint = firstPoint + nextPoint;
                            coefficent.Add(3 * interPoint / ((interPoint + nextPoint) / m + (interPoint + firstPoint) / m_next));
                        }
                    }
                }
            }

            coefficent.Add(double.IsNaN(slope[slope.Length - 1]) ? 0 : slope[slope.Length - 1]);

            for (int i = 0; i < coefficent.Count; i++)
            {
                if (i + 1 < coefficent.Count && dx.Length > 0)
                {
                    double value = (dx[i] / 3);
                    startControlPoints.Add(new ChartPoint(xValues[i] + value, yValues[i] + coefficent[i] * value));
                    endControlPoints.Add(new ChartPoint(xValues[i + 1] - value, yValues[i + 1] - coefficent[i + 1] * value));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        /// <param name="ys2"></param>
        protected void NaturalSpline(List<double> xValues, IList<double> yValues, out double[] ys2)
        {
            int count = 0;

            bool isGrouping = this.ActualXAxis is CategoryAxis ? (this.ActualXAxis as CategoryAxis).IsIndexed : true;
            if (!isGrouping)
                count = (int)xValues.Count;
            else
                count = (int)DataCount;
            ys2 = new double[count];

            double a = 6;
            double[] u = new double[count];
            double p;

            if (SplineType == SplineType.Natural)
            {
                ys2[0] = u[0] = 0;
                ys2[count - 1] = 0;
            }
            else if (xValues.Count > 1)
            {
                double d0 = (xValues[1] - xValues[0]) / (yValues[1] - yValues[0]);
                double dn = (xValues[count - 1] - xValues[count - 2]) / (yValues[count - 1] - yValues[count - 2]);
                u[0] = 0.5;
                ys2[0] = (3 * (yValues[1] - yValues[0])) / (xValues[1] - xValues[0]) - 3 * d0;
                ys2[count - 1] = 3 * dn - (3 * (yValues[count - 1] - yValues[count - 2])) / (xValues[count - 1] - xValues[count - 2]);
                if (double.IsInfinity(ys2[0]) || double.IsNaN(ys2[0]))
                    ys2[0] = 0;
                if (double.IsInfinity(ys2[count - 1]) || double.IsNaN(ys2[count - 1]))
                    ys2[count - 1] = 0;
            }

            for (int i = 1; i < count - 1; i++)
            {
                if (yValues.Count > i + 1 && !double.IsNaN(yValues[i + 1]) && !double.IsNaN(yValues[i - 1]) && !double.IsNaN(yValues[i]))
                {
                    double d1 = xValues[i] - xValues[i - 1];
                    double d2 = xValues[i + 1] - xValues[i - 1];
                    double d3 = xValues[i + 1] - xValues[i];
                    double dy1 = yValues[i + 1] - yValues[i];
                    double dy2 = yValues[i] - yValues[i - 1];

                    if (xValues[i] == xValues[i - 1] || xValues[i] == xValues[i + 1])
                    {
                        ys2[i] = 0;
                        u[i] = 0;
                    }
                    else
                    {
                        p = 1 / (d1 * ys2[i - 1] + 2 * d2);
                        ys2[i] = -p * d3;
                        u[i] = p * (a * (dy1 / d3 - dy2 / d1) - d1 * u[i - 1]);
                    }
                }
            }

            for (int k = count - 2; k >= 0; k--)
            {
                ys2[k] = ys2[k] * ys2[k + 1] + u[k];
            }
        }

        /// <summary>
        /// Returns the controlPoints of the curve
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="ys1"></param>
        /// <param name="ys2"></param>
        /// <param name="controlPoint1"></param>
        /// <param name="controlPoint2"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        protected void GetBezierControlPoints(ChartPoint point1, ChartPoint point2, double ys1, double ys2, out ChartPoint controlPoint1, out ChartPoint controlPoint2)
        {
            const double One_thrid = 1 / 3.0d;

            double deltaX2 = point2.X - point1.X;

            deltaX2 = deltaX2 * deltaX2;

            double dx1 = 2 * point1.X + point2.X;
            double dx2 = point1.X + 2 * point2.X;

            double dy1 = 2 * point1.Y + point2.Y;
            double dy2 = point1.Y + 2 * point2.Y;

            double y1 = One_thrid * (dy1 - One_thrid * deltaX2 * (ys1 + 0.5f * ys2));
            double y2 = One_thrid * (dy2 - One_thrid * deltaX2 * (0.5f * ys1 + ys2));

            controlPoint1 = new ChartPoint(dx1 * One_thrid, y1);
            controlPoint2 = new ChartPoint(dx2 * One_thrid, y2);
        }

        #endregion

        #region Private Static Methods

        private static void OnCustomTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as SplineSeries;

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
            (d as SplineSeries).UpdateArea();
        }
        
        private static void OnSplineTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SplineSeries)d).UpdateArea();
        }
        
        #endregion

        #region Private Methods

        private void UpdatePreviewSegmentAndSeries(Point mousePos)
        {
            try
            {
                Brush brush = (Segments[0] as SplineSegment).Interior;
                var xValues = GetXValues();
                double[] yCoef = null;

                if (segments == null)
                {
                    segments = new List<SplineSegment>();
                    previewYValues = new List<double>();
                    if (SplineType == SplineType.Monotonic)
                        GetMonotonicSpline(xValues, YValues);
                    else if (SplineType == SplineType.Cardinal)
                        GetCardinalSpline(xValues, YValues);
                    else
                        NaturalSpline(xValues, YValues, out yCoef);
                    var chartTransformer = this.ActualArea != null ? CreateTransformer(GetAvailableSize(), true) : null;
                    if (chartTransformer == null) return;

                    for (var i = 0; i < DataCount; i++)
                    {
                        var index = i + 1;
                        var startPoint = new ChartPoint(xValues[i], YValues[i]);
                        previewYValues.Add(YValues[i]);
                        if (index >= DataCount) continue;
                        var endPoint = new ChartPoint(xValues[index], YValues[index]);
                        ChartPoint startControlPoint;
                        ChartPoint endControlPoint;
                        //Calculate curve points. 
                        if (SplineType == SplineType.Monotonic)
                        {
                            startControlPoint = startControlPoints[index - 1];
                            endControlPoint = endControlPoints[index - 1];
                        }
                        else if (SplineType == SplineType.Cardinal)
                        {
                            startControlPoint = startControlPoints[index - 1];
                            endControlPoint = endControlPoints[index - 1];
                        }
                        else
                            GetBezierControlPoints(startPoint, endPoint, yCoef[i], yCoef[index], out startControlPoint, out endControlPoint);
                        var splineSegment = new SplineSegment(startPoint, startControlPoint, endControlPoint, endPoint, this);
                        splineSegment.SetData(startPoint, startControlPoint, endControlPoint, endPoint);
                        var segment = splineSegment.CreateVisual(Size.Empty);
                        splineSegment.Update(chartTransformer);

                        if (CustomTemplate == null)
                        {
                            var segmentPath = segment as Path;
                            segmentPath.Stroke = ((Path)Segments[0].GetRenderedVisual()).Stroke;
                            segmentPath.StrokeThickness = this.StrokeThickness;
                            segment.Opacity = ((Path)Segments[0].GetRenderedVisual()).StrokeThickness;
                            segment.Opacity = 0.5;
                            segments.Add(splineSegment);
                            SeriesPanel.Children.Add(segment);
                        }
                        else
                        {
                            segments.Add(splineSegment);
                            SeriesPanel.Children.Add(segment);
                        }
                    }
                }
                else
                {
                    if (isSeriesCaptured)
                    {
                        var newValue = Area.PointToValue(
                            ActualYAxis, 
                            new Point(mousePos.X, mousePos.Y));
                        var baseValue = Area.PointToValue(
                            ActualYAxis, 
                            IsActualTransposed ? new Point(offsetPosition, mousePos.Y)
                                               : new Point(mousePos.X, offsetPosition));
                        var offset = newValue - baseValue;

                        for (var i = 0; i < previewYValues.Count; i++)
                        {
                            previewYValues[i] = previewYValues[i] + offset;
                        }

                        offsetPosition = IsActualTransposed ? mousePos.X : mousePos.Y;
                        if (IsActualTransposed)
                            DraggedValue = Area.PointToValue(ActualXAxis, new Point(0, initialPosition)) - Area.PointToValue(ActualXAxis, new Point(mousePos.Y, mousePos.X));
                        else
                            DraggedValue = Area.PointToValue(ActualYAxis, mousePos) - Area.PointToValue(ActualYAxis, new Point(0, initialPosition));

                        var dragEvent = new XySeriesDragEventArgs { Delta = DraggedValue, BaseXValue = SegmentIndex };
                        RaiseDragDelta(dragEvent);

                        if (dragEvent.Cancel)
                        {
                            ResetDraggingElements("Cancel", true);
                            return;
                        }

                        if (!IsActualTransposed)
                            UpdateSeriesDragValueToolTip(mousePos, brush, DraggedValue, YValues[0], Area.ValueToPoint(ActualXAxis, 0));
                        else
                            UpdateSeriesDragValueToolTip(mousePos, brush, DraggedValue, 0, Area.ValueToPoint(ActualYAxis, YValues[0]));
                    }
                    else
                    {
                        var xPos = Segments.Count == SegmentIndex
                                   ? (DraggingSegment as SplineSegment).X2 : (DraggingSegment as SplineSegment).X1;
                        previewYValues[SegmentIndex] = Area.PointToValue(ActualYAxis, mousePos);

                        DraggedValue = Area.PointToValue(ActualYAxis, mousePos);
                        var dragEvent = new XySegmentDragEventArgs
                        {
                            BaseYValue = YValues[SegmentIndex],
                            NewYValue = DraggedValue,
                            Segment = DraggingSegment,
                            Delta = GetActualDelta()
                        };

                        prevDraggedValue = DraggedValue;

                        RaiseDragDelta(dragEvent);
                        if (dragEvent.Cancel)
                            return;
                        if (IsActualTransposed)
                            UpdateSegmentDragValueToolTip(new Point(mousePos.X, Area.ValueToPoint(ActualXAxis, xPos)), DraggingSegment, 0, DraggedValue, 0, 0);
                        else
                            UpdateSegmentDragValueToolTip(new Point(Area.ValueToPoint(ActualXAxis, xPos), mousePos.Y), DraggingSegment, 0, DraggedValue, 0, 0);
                    }

                    if (SplineType == SplineType.Monotonic)
                        GetMonotonicSpline(xValues, previewYValues);//Calculate monotone curve points.
                    else if (SplineType == SplineType.Cardinal)
                        GetCardinalSpline(xValues, YValues);
                    else
                        NaturalSpline(xValues, previewYValues, out yCoef);

                    var chartTransformer = this.ActualArea != null ? CreateTransformer(GetAvailableSize(), true) : null;
                    if (chartTransformer == null) return;

                    for (var i = 0; i < DataCount; i++)
                    {
                        var index = i + 1;
                        var startPoint = new ChartPoint(xValues[i], previewYValues[i]);
                        if (index >= DataCount) continue;
                        var endPoint = new ChartPoint(xValues[index], previewYValues[index]);
                        ChartPoint startControlPoint;
                        ChartPoint endControlPoint;

                        // Calculate curve points. 
                        if (SplineType == SplineType.Monotonic)
                        {
                            startControlPoint = startControlPoints[index - 1];
                            endControlPoint = endControlPoints[index - 1];
                        }
                        else if (SplineType == SplineType.Cardinal)
                        {
                            startControlPoint = startControlPoints[index - 1];
                            endControlPoint = endControlPoints[index - 1];
                        }
                        else
                            GetBezierControlPoints(startPoint, endPoint, yCoef[i], yCoef[index], out startControlPoint, out endControlPoint);

                        segments[i].SetData(startPoint, startControlPoint, endControlPoint, endPoint);
                        segments[i].Update(chartTransformer);
                    }
                }

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
                if (isDragged)
                {
                    var baseValue = YValues[SegmentIndex];
                    var dragPreviewEnd = new XyPreviewEndEventArgs { BaseYValue = baseValue, NewYValue = DraggedValue };
                    RaisePreviewEnd(dragPreviewEnd);

                    if (dragPreviewEnd.Cancel)
                    {
                        ResetDraggingElements("", false);
                        return;
                    }

                    if (isSeriesCaptured)
                    {
                        for (var i = 0; i < YValues.Count; i++)
                        {
                            YValues[i] = GetSnapToPoint(YValues[i] + DraggedValue);
                        }

                        if (UpdateSource)
                            UpdateUnderLayingModel(YBindingPath, YValues);
                    }
                    else
                    {
                        DraggedValue = GetSnapToPoint(DraggedValue);
                        ActualSeriesYValues[0][SegmentIndex] = DraggedValue;
                        if (UpdateSource && !IsSortData)
                            UpdateUnderLayingModel(YBindingPath, SegmentIndex, DraggedValue);
                    }

                    UpdateArea();
                    RaiseDragEnd(new ChartDragEndEventArgs { BaseYValue = baseValue, NewYValue = DraggedValue });
                }
                ResetDraggingElements("", false);
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
