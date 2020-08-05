using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.Foundation;

namespace Syncfusion.UI.Xaml.Charts
{
    ///<summary>
    /// StackingBarSeries is typically preferred in cases of multiple series of type <see cref="BarSeries"/>.
    /// Each Series is stacked horizontally side by side to each other.
    /// </summary>
    /// <seealso cref="StackingBarSegment"/>
    /// <seealso cref="StackingColumnSeries"/>
    /// <seealso cref="StackingAreaSeries"/>
    /// <seealso cref="BarSeries"/> 
    public partial class StackingBarSeries : StackingSeriesBase, ISegmentSelectable, ISegmentSpacing
    {
        #region Dependency Property Registration

        /// <summary>
        ///The DependencyProperty for <see cref="SegmentSelectionBrush"/> property.       
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register(
                "SegmentSelectionBrush",
                typeof(Brush), 
                typeof(StackingBarSeries),
                new PropertyMetadata(null, OnSegmentSelectionBrush));

        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSpacing"/> property.       
        /// </summary>
        public static readonly DependencyProperty SegmentSpacingProperty =
            DependencyProperty.Register(
                "SegmentSpacing", 
                typeof(double), 
                typeof(StackingBarSeries),
                new PropertyMetadata(0.0, OnSegmentSpacingChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property.       .
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex",
                typeof(int),
                typeof(StackingBarSeries),
                new PropertyMetadata(-1, OnSelectedIndexChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="CustomTemplate"/> property.       .
        /// </summary>
        public static readonly DependencyProperty CustomTemplateProperty =
            DependencyProperty.Register(
                "CustomTemplate",
                typeof(DataTemplate), 
                typeof(StackingBarSeries),
                new PropertyMetadata(null, OnCustomTemplateChanged));

        #endregion

        #region Fields

        #region Private Fields

        private Storyboard sb;
        
        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for StackingBarSeries
        /// </summary>
        public StackingBarSeries()
        {
            IsActualTransposed = true;
        }

        #endregion

        #region Properties

        #region Public Properties

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
        /// <example>This example, we are using <see cref="ScatterSeries"/>.</example>
        /// <example>
        ///     <code language="XAML">
        ///         &lt;syncfusion:ScatterSeries ItemsSource="{Binding Demands}" XBindingPath="Demand" ScatterHeight="40" 
        ///                     YBindingPath="Year2010" ScatterWidth="40"&gt;
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

        #region Protected Override Properties

        /// <summary>
        /// This indicates whether this series is stacked or not.
        /// </summary>
        protected override bool IsStacked
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
        /// Creates the segments of StackingBarSeries.
        /// </summary>       
        public override void CreateSegments()
        {
            double x1, x2, y1, y2;
            List<double> xValues = null;
            double Origin = ActualXAxis != null ? ActualXAxis.Origin : 0d;
            if (ActualXAxis != null && ActualXAxis.Origin == 0 && ActualYAxis is LogarithmicAxis &&
                (ActualYAxis as LogarithmicAxis).Minimum != null)
                Origin = (double)(ActualYAxis as LogarithmicAxis).Minimum;
            if (ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed)
                xValues = GroupedXValuesIndexes;
            else
                xValues = GetXValues();
            var stackingValues = GetCumulativeStackValues(this);

            if (stackingValues != null)
            {
                YRangeStartValues = stackingValues.StartValues;
                YRangeEndValues = stackingValues.EndValues;
#if NETFX_CORE
                // Designer crashes when rebuild the SfChart
                bool _isInDesignMode = Windows.ApplicationModel.DesignMode.DesignModeEnabled;
                if (_isInDesignMode)
                    if (YRangeStartValues.Count == 0 && YRangeEndValues.Count == 0)
                        return;
#endif
                if (YRangeStartValues == null)
                {
                    YRangeStartValues = (from val in xValues select Origin).ToList();
                }

                if (xValues != null)
                {
                    DoubleRange sbsInfo = this.GetSideBySideInfo(this);

                    double median = sbsInfo.Delta / 2;
                    int segmentCount = 0;
                    if (ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed)
                    {
                        Segments.Clear();
                        Adornments.Clear();

                        for (int i = 0; i < DistinctValuesIndexes.Count; i++)
                        {
                            for (int j = 0; j < DistinctValuesIndexes[i].Count; j++)
                            {
                                if (j < xValues.Count)
                                {
                                    int index = DistinctValuesIndexes[i][j];
                                    x1 = i + sbsInfo.Start;
                                    x2 = i + sbsInfo.End;
                                    y2 = double.IsNaN(YRangeStartValues[segmentCount]) ? Origin : YRangeStartValues[segmentCount];
                                    y1 = double.IsNaN(YRangeEndValues[segmentCount]) ? Origin : YRangeEndValues[segmentCount];
                                    StackingBarSegment barSegment = new StackingBarSegment(x1, y1, x2, y2, this);
                                    barSegment.XData = xValues[segmentCount];
                                    barSegment.YData = GroupedSeriesYValues[0][index];
                                    barSegment.Item = GroupedActualData[segmentCount];
                                    Segments.Add(barSegment);

                                    if (AdornmentsInfo != null)
                                    {
                                        if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                                            AddColumnAdornments(i, GroupedSeriesYValues[0][index], x1, y1, segmentCount, median);
                                        else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                                            AddColumnAdornments(i, GroupedSeriesYValues[0][index], x1, y2, segmentCount, median);
                                        else
                                            AddColumnAdornments(i, GroupedSeriesYValues[0][index], x1, y1 + (y2 - y1) / 2, segmentCount, median);
                                    }

                                    segmentCount++;
                                }
                            }
                        }
                    }
                    else
                    {
                        ClearUnUsedSegments(this.DataCount);
                        ClearUnUsedAdornments(this.DataCount);
                        for (int i = 0; i < DataCount; i++)
                        {
                            x1 = xValues[i] + sbsInfo.Start;
                            x2 = xValues[i] + sbsInfo.End;
                            y2 = double.IsNaN(YRangeStartValues[i]) ? Origin : YRangeStartValues[i];
                            y1 = double.IsNaN(YRangeEndValues[i]) ? Origin : YRangeEndValues[i];

                            if (i < Segments.Count)
                            {
                                (Segments[i]).Item = ActualData[i];
                                (Segments[i]).SetData(x1, y1, x2, y2);
                                (Segments[i] as StackingBarSegment).XData = xValues[i];
                                (Segments[i] as StackingBarSegment).YData = YValues[i];
                                if (SegmentColorPath != null && !Segments[i].IsEmptySegmentInterior && ColorValues.Count > 0 && !Segments[i].IsSelectedSegment)
                                    Segments[i].Interior = (Interior != null) ? Interior : ColorValues[i];
                            }
                            else
                            {
                                StackingBarSegment barSegment = new StackingBarSegment(x1, y1, x2, y2, this);
                                barSegment.XData = xValues[i];
                                barSegment.YData = ActualSeriesYValues[0][i];
                                barSegment.Item = ActualData[i];
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

                    if (ShowEmptyPoints)
                    {
                        if (this is StackingBar100Series)
                        {
                            var index = EmptyPointIndexes[0];
                            if (EmptyPointStyle == Charts.EmptyPointStyle.Symbol || EmptyPointStyle == Charts.EmptyPointStyle.SymbolAndInterior)

                                foreach (var n in index)
                                {
                                    this.ActualSeriesYValues[0][n] = double.IsNaN(YRangeEndValues[n]) ? 0 : this.YRangeEndValues[n];
                                }

                            UpdateEmptyPointSegments(xValues, true);
                            ReValidateYValues(EmptyPointIndexes);
                            ValidateYValues();
                        }
                        else
                            UpdateEmptyPointSegments(xValues, true);
                    }
                }
            }
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
            double startWidth = Area.ValueToPoint(Area.InternalSecondaryAxis, 0);
            string path = IsActualTransposed ? "(Canvas.Left)" : "(Canvas.Top)";
            string scalePath = IsActualTransposed ? "(UIElement.RenderTransform).(ScaleTransform.ScaleX)" : "(UIElement.RenderTransform).(ScaleTransform.ScaleY)";
            string adornTransPath = IsActualTransposed ? "(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.X)" : "(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.Y)";

            foreach (ChartSegment segment in Segments)
            {
                double elementSize;
                var element = (FrameworkElement)segment.GetRenderedVisual();
                if (this.ShowEmptyPoints)
                    elementSize = IsActualTransposed ? element.Width : element.Height;
                else
                {
                    var barSegment = segment as BarSegment;
                    elementSize = IsActualTransposed
                        ? barSegment.segmentSize.Width
                        : barSegment.segmentSize.Height;
                }

                if (!double.IsNaN(elementSize))
                {
                    double canvasSize = IsActualTransposed ? Canvas.GetLeft(element) : Canvas.GetTop(element);
                    element.RenderTransform = new ScaleTransform();
                    DoubleAnimationUsingKeyFrames keyFrames = new DoubleAnimationUsingKeyFrames();
                    SplineDoubleKeyFrame keyFrame = new SplineDoubleKeyFrame();
                    keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                    keyFrame.Value = startWidth;
                    keyFrames.KeyFrames.Add(keyFrame);

                    keyFrame = new SplineDoubleKeyFrame();
                    keyFrame.KeyTime = KeyTime.FromTimeSpan(AnimationDuration);
                    keyFrame.Value = canvasSize;
                    KeySpline keySpline = new KeySpline();
                    keySpline.ControlPoint1 = new Point(0.64, 0.84);
#if WINDOWS_UAP
                    keySpline.ControlPoint2 = new Point(0, 1);//Animation have to provide same easing effect in all platforms.
#else
                    keySpline.ControlPoint2 = new Point(0.67, 0.95);
#endif
                    keyFrame.KeySpline = keySpline;

                    keyFrames.KeyFrames.Add(keyFrame);
                    Storyboard.SetTargetProperty(keyFrames, path);
                    Storyboard.SetTarget(keyFrames, element);
                    sb.Children.Add(keyFrames);

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
                    keySpline1.ControlPoint2 = new Point(0, 1);//Animation have to provide same easing effect in all platforms.
#else
                    keySpline1.ControlPoint2 = new Point(0.67, 0.95);
#endif
                    keyFrame1.KeySpline = keySpline1;
                    keyFrames1.KeyFrames.Add(keyFrame1);
                    keyFrame1.Value = 1;
                    keyFrames1.EnableDependentAnimation = true;
                    Storyboard.SetTargetProperty(keyFrames1, scalePath);
                    Storyboard.SetTarget(keyFrames1, element);
                    sb.Children.Add(keyFrames1);
                    if (this.AdornmentsInfo != null && AdornmentsInfo.ShowLabel)
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
                        //keyFrame1.Value = (YValues[i] > 0) ? -(elementWidth * 10) / 100 : (elementWidth * 10) / 100;
                        keyFrame1.Value = -(elementSize * 10) / 100;
                        keyFrames1.KeyFrames.Add(keyFrame1);
                        keyFrame1 = new SplineDoubleKeyFrame();
                        keyFrame1.KeyTime = KeyTime.FromTimeSpan(AnimationDuration);

                        keySpline1 = new KeySpline();
                        keySpline1.ControlPoint1 = new Point(0.64, 0.84);
#if WINDOWS_UAP
                        keySpline1.ControlPoint2 = new Point(0, 1);//Animation have to provide same easing effect in all platforms.
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
                    }
                 
                    i++;
                }
            }

            sb.Begin();
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
            return base.CloneSeries(new StackingBarSeries()
            {
                SegmentSelectionBrush = this.SegmentSelectionBrush,
                SelectedIndex = this.SelectedIndex,
                SegmentSpacing = this.SegmentSpacing,
                CustomTemplate = this.CustomTemplate
            });
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
            (d as StackingBarSeries).UpdateArea();
        }
        
        private static void OnSegmentSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as StackingBarSeries;
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
            var series = d as StackingBarSeries;

            if (series.Area == null) return;

            series.Segments.Clear();
            series.Area.ScheduleUpdate();
        }

        #endregion

        #region Private Methods

        #endregion

        #endregion
    }
}
