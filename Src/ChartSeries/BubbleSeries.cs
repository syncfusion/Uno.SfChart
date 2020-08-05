using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using System.Collections;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.Foundation;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Bubble series displays a set of circular symbols of varying size. It is represented by closely packed circles, whose areas are proportional to the quantities.
    /// </summary>
    /// <remarks>
    /// BubbleSeries requires an additional data binding parameter <see cref="BubbleSeries.Size"/> in addition to X,Y parameters.
    /// The size of each bubble depends on the size value given in the data point.<see cref="BubbleSeries.MinimumRadius"/> and <see cref="MaximumRadius"/> properties can be used to 
    /// control the minimum and maximum radius of the symbols.
    /// </remarks>
    /// <seealso cref="BubbleSegment"/>
    /// <seealso cref="ScatterSeries"/>
    public partial class BubbleSeries : XyDataSeries, ISegmentSelectable
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty of <see cref="ShowZeroBubbles"/> property
        /// </summary>
        public static readonly DependencyProperty ShowZeroBubblesProperty =
            DependencyProperty.Register("ShowZeroBubbles", typeof(bool), typeof(BubbleSeries), new PropertyMetadata(true, OnShowZeroBubblesPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSelectionBrush"/> property.
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register("SegmentSelectionBrush", typeof(Brush), typeof(BubbleSeries),
            new PropertyMetadata(null, OnSegmentSelectionBrush));

        /// <summary>
        /// The DependencyProperty for <see cref="MinimumRadius"/> property.
        /// </summary>
        public static readonly DependencyProperty MinimumRadiusProperty =
            DependencyProperty.Register("MinimumRadius", typeof(double), typeof(BubbleSeries),
            new PropertyMetadata(10d, new PropertyChangedCallback(OnSizeChanged)));

        /// <summary>
        /// The DependencyProperty for <see cref="MaximumRadius"/> property.
        /// </summary>
        public static readonly DependencyProperty MaximumRadiusProperty =
            DependencyProperty.Register("MaximumRadius", typeof(double), typeof(BubbleSeries),
            new PropertyMetadata(30d, new PropertyChangedCallback(OnSizeChanged)));

        /// <summary>
        /// The DependencyProperty for <see cref="Size"/> property.
        /// </summary>
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(string), typeof(BubbleSeries),
            new PropertyMetadata(null, new PropertyChangedCallback(OnSizeChanged)));

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(BubbleSeries),
            new PropertyMetadata(-1, OnSelectedIndexChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="CustomTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty CustomTemplateProperty =
            DependencyProperty.Register("CustomTemplate", typeof(DataTemplate), typeof(BubbleSeries),
            new PropertyMetadata(null, OnCustomTemplateChanged));

        #endregion

        #region Fields

        #region Private Fields

        private List<double> sizeValues;

        private Storyboard sb;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for BubbleSeries
        /// </summary>
        public BubbleSeries()
        {
            sizeValues = new List<double>();
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets a value that indicates whether to show the bubble segment when apply size value to 0. This is a bindable property.
        /// </summary>
        public bool ShowZeroBubbles
        {
            get { return (bool)GetValue(ShowZeroBubblesProperty); }
            set { SetValue(ShowZeroBubblesProperty, value); }
        }

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
        /// Gets or sets the maximum size for each bubble.
        /// </summary>
        public double MinimumRadius
        {
            get { return (double)GetValue(MinimumRadiusProperty); }
            set { SetValue(MinimumRadiusProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that specifies the maximum size for each bubble segment. This is a bindable property.
        /// </summary>
        public double MaximumRadius
        {
            get { return (double)GetValue(MaximumRadiusProperty); }
            set { SetValue(MaximumRadiusProperty, value); }
        }

        /// <summary>
        /// Gets or sets the property binding path that specifies the bubble series size. This is a bindable property. 
        /// </summary>
        public string Size
        {
            get { return (string)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
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
        ///                        ScatterHeight="40" ScatterWidth="40"&gt;
        ///            &lt;syncfusion:ScatterSeries.CustomTemplate&gt;
        ///                 &lt;DataTemplate&gt;
        ///                     &lt;Canvas&gt;
        ///                        &lt;Path Data="M20.125,32l0.5,12.375L10.3125,12.375L10.3125,0.5L29.9375,0.5L29.9375,12.375L39.75,12.375Z" 
        ///                                  Stretch="Fill" Fill="{Binding Interior}" Height="{Binding ScatterHeight}" Width="{Binding ScatterWidth}" 
        ///                                  Canvas.Left="{Binding RectX}" Canvas.Top="{Binding RectY}"/&gt;
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

        internal override bool IsMultipleYPathRequired
        {
            get
            {
                return true;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Creates the Segments of BubbleSeries
        /// </summary>
        public override void CreateSegments()
        {
            double maximumSize = 0d, segmentRadius = 0d;
            List<double> xValues = null;
            var isGrouped = (ActualXAxis is CategoryAxis && !(ActualXAxis as CategoryAxis).IsIndexed);
            if (isGrouped)
                xValues = GroupedXValuesIndexes;
            else
                xValues = GetXValues();
            maximumSize = (from val in sizeValues select val).Max();
            double minRadius = this.MinimumRadius;
            double maxradius = this.MaximumRadius;
            double radius = maxradius - minRadius;

            if (xValues != null)
            {
                if (isGrouped)
                {
                    Segments.Clear();
                    Adornments.Clear();
                    for (int i = 0; i < xValues.Count; i++)
                    {
                        var relativeSize = radius * Math.Abs(sizeValues[i] / maximumSize);
                        relativeSize = double.IsNaN(relativeSize) ? 0 : relativeSize;
                        if (ShowZeroBubbles)
                            segmentRadius = minRadius + relativeSize;
                        else
                            segmentRadius = (sizeValues[i] != 0) ? (minRadius + relativeSize) : 0;
                        if (i < xValues.Count && GroupedSeriesYValues[0].Count > i)
                        {
                            BubbleSegment bubbleSegment = new BubbleSegment(xValues[i], GroupedSeriesYValues[0][i], segmentRadius, this);
                            bubbleSegment.Series = this;
                            bubbleSegment.Size = segmentRadius;
                            bubbleSegment.SetData(xValues[i], GroupedSeriesYValues[0][i]);
                            bubbleSegment.Item = ActualData[i];
                            bubbleSegment.Size = sizeValues[i];
                            Segments.Add(bubbleSegment);
                        }

                        if (AdornmentsInfo != null)
                            AddAdornmentAtXY(xValues[i], GroupedSeriesYValues[0][i], i);
                    }
                }
                else
                {
                    ClearUnUsedSegments(this.DataCount);
                    ClearUnUsedAdornments(this.DataCount);
                    for (int i = 0; i < this.DataCount; i++)
                    {
                        var relativeSize = radius * Math.Abs(sizeValues[i] / maximumSize);
                        relativeSize = double.IsNaN(relativeSize) ? 0 : relativeSize;
                        if (ShowZeroBubbles)
                            segmentRadius = minRadius + relativeSize;
                        else
                            segmentRadius = (sizeValues[i] != 0) ? (minRadius + relativeSize) : 0;
                        if (i < Segments.Count)
                        {
                            (Segments[i]).SetData(xValues[i], YValues[i]);
                            (Segments[i] as BubbleSegment).SegmentRadius = segmentRadius;
                            (Segments[i] as BubbleSegment).Item = ActualData[i];
                            (Segments[i] as BubbleSegment).Size = sizeValues[i];
                            (Segments[i] as BubbleSegment).YData = YValues[i];
                            (Segments[i] as BubbleSegment).XData = xValues[i];
                            if (SegmentColorPath != null && !Segments[i].IsEmptySegmentInterior && ColorValues.Count > 0 && !Segments[i].IsSelectedSegment)
                                Segments[i].Interior = (Interior != null) ? Interior : ColorValues[i];
                        }
                        else
                        {
                            BubbleSegment bubbleSegment = new BubbleSegment(xValues[i], YValues[i], segmentRadius, this);
                            bubbleSegment.Series = this;
                            bubbleSegment.SetData(xValues[i], YValues[i]);
                            bubbleSegment.Item = ActualData[i];
                            bubbleSegment.Size = sizeValues[i];
                            Segments.Add(bubbleSegment);
                        }

                        if (AdornmentsInfo != null)
                            AddAdornmentAtXY(xValues[i], YValues[i], i);
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
                var ellipse = segment.GetRenderedVisual() as Ellipse;

                if (ellipse != null && EllipseContainsPoint(ellipse, point))
                    return Segments.IndexOf(segment);
            }

            return -1;
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
                    ResetAdornmentAnimationState();
                    return;
                }
            }

            sb = new Storyboard();

            foreach (ChartSegment segment in this.Segments)
            {
                int randomValue = rand.Next(0, 20);
                TimeSpan beginTime = TimeSpan.FromMilliseconds(randomValue * 20);
                var element = (FrameworkElement)segment.GetRenderedVisual();
                element.RenderTransform = null;
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
                keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 50) / 100));
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
                keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 50) / 100));

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

                    DoubleAnimation animation = new DoubleAnimation() { To = 1, From = 0, BeginTime = TimeSpan.FromSeconds(beginTime.TotalSeconds + (beginTime.Seconds * 90) / 100), Duration = TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 80) / 100) };
                    Storyboard.SetTargetProperty(animation, "FrameworkElement.Opacity");
                    Storyboard.SetTarget(animation, label);
                    sb.Children.Add(animation);
                }

                i++;
            }

            sb.Begin();
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

        #endregion

        #region Protected Internal Override Methods

        /// <summary>
        /// Method for Generate Points for XYDataSeries
        /// </summary>
        protected internal override void GeneratePoints()
        {
            sizeValues.Clear();
            GeneratePoints(new string[] { YBindingPath, Size }, YValues, sizeValues);
        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Called when DataSource property changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            YValues.Clear();
            sizeValues.Clear();
            GeneratePoints(new string[] { YBindingPath, Size }, YValues, sizeValues);
            this.UpdateArea();
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new BubbleSeries()
            {
                MaximumRadius = this.MaximumRadius,
                MinimumRadius = this.MinimumRadius,
                SegmentSelectionBrush = this.SegmentSelectionBrush,
                SelectedIndex = this.SelectedIndex,
                ShowZeroBubbles = this.ShowZeroBubbles,
                Size = this.Size,
                CustomTemplate = this.CustomTemplate
            });
        }

        #endregion
        
        #region Private Static Methods

        /// <summary>
        /// This method used to check the position within the ellipse
        /// </summary>
        /// <param name="Ellipse"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private static bool EllipseContainsPoint(Ellipse Ellipse, Point point)
        {
            Point center = new Point(Canvas.GetLeft(Ellipse) + (Ellipse.Width / 2),
                  Canvas.GetTop(Ellipse) + (Ellipse.Height / 2));

            double x = Ellipse.Width / 2;
            double y = Ellipse.Height / 2;

            if (x <= 0.0 || y <= 0.0)
                return false;

            Point result = new Point(point.X - center.X,
                                         point.Y - center.Y);

            return ((double)(result.X * result.X)
                     / (x * x)) + ((double)(result.Y * result.Y) / (y * y))
                <= 1.0;
        }

        private static void OnShowZeroBubblesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BubbleSeries).UpdateArea();
        }

        private static void OnSegmentSelectionBrush(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BubbleSeries).UpdateArea();
        }

        private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BubbleSeries).OnBindingPathChanged(e);
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
            var series = d as BubbleSeries;

            if (series.Area == null) return;

            series.Segments.Clear();
            series.Area.ScheduleUpdate();
        }

        #endregion

        #endregion
    }
}
