using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// WaterfallSeries displays its positive and negative data points using a set of bars.
    /// </summary>
    /// <seealso cref="ColumnSeries"/>
    /// <seealso cref="StepLineSeries"/>
    public partial class WaterfallSeries : XyDataSeries, ISegmentSelectable, ISegmentSpacing
    {
        #region Dependency Property Registration

        /// <summary>
        /// Using a DependencyProperty as the backing store for AllowAutoSum.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty AllowAutoSumProperty =
            DependencyProperty.Register(
                "AllowAutoSum", 
                typeof(bool),
                typeof(WaterfallSeries),
                new PropertyMetadata(true, OnAllowAutoSum));

        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSelectionBrush"/> property. 
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register(
                "SegmentSelectionBrush",
                typeof(Brush),
                typeof(WaterfallSeries),
                new PropertyMetadata(null, OnSegmentSelectionBrush));
        
        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property. 
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex",
                typeof(int), 
                typeof(WaterfallSeries),
                new PropertyMetadata(-1, OnSelectedIndexChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSpacing"/> property. 
        /// </summary>
        public static readonly DependencyProperty SegmentSpacingProperty =
            DependencyProperty.Register(
                "SegmentSpacing", 
                typeof(double), 
                typeof(WaterfallSeries),
                new PropertyMetadata(0.0, OnSegmentSpacingChanged));

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowConnector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowConnectorProperty =
            DependencyProperty.Register("ShowConnector", typeof(bool), typeof(WaterfallSeries), new PropertyMetadata(true, OnShowConnectorChanged));

        /// <summary>
        /// Using a DependencyProperty as the backing store for SummaryBindingPath.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SummaryBindingPathProperty =
            DependencyProperty.Register(
                "SummaryBindingPath", 
                typeof(string), 
                typeof(WaterfallSeries),
                new PropertyMetadata(null, OnSummaryBindingPathChanged));

        /// <summary>
        /// Using a DependencyProperty as the backing store for ConnectorLineStyle.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty ConnectorLineStyleProperty =
            DependencyProperty.Register("ConnectorLineStyle", typeof(Style), typeof(WaterfallSeries), new PropertyMetadata(null, OnConnectorLineStyleChanged));

        /// <summary>
        /// Using a DependencyProperty as the backing store for NegativeSegmentBrush.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty NegativeSegmentBrushProperty =
            DependencyProperty.Register("NegativeSegmentBrush", typeof(Brush), typeof(WaterfallSeries), new PropertyMetadata(null, OnNegativeSegmentBrushChanged));

        /// <summary>
        /// Using a DependencyProperty as the backing store for SummarySegmentBrush.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty SummarySegmentBrushProperty =
            DependencyProperty.Register("SummarySegmentBrush", typeof(Brush), typeof(WaterfallSeries), new PropertyMetadata(null, OnSummarySegmentBrushChanged));

        #endregion

        #region Fields

        #region Private Fields

        private Storyboard sb;

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Event raised while the segment have created.
        /// </summary>
        public event EventHandler<WaterfallSegmentCreatedEventArgs> SegmentCreated;
        
        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to auto sum. 
        /// </summary>
        public bool AllowAutoSum
        {
            get { return (bool)GetValue(AllowAutoSumProperty); }
            set { SetValue(AllowAutoSumProperty, value); }
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
        /// Gets or sets a value indicating whether the segments connector line is visible.
        /// </summary>
        public bool ShowConnector
        {
            get { return (bool)GetValue(ShowConnectorProperty); }
            set { SetValue(ShowConnectorProperty, value); }
        }

        /// <summary>
        /// Gets or sets string that indicates sum segment of series. 
        /// </summary>
        public string SummaryBindingPath
        {
            get { return (string)GetValue(SummaryBindingPathProperty); }
            set { SetValue(SummaryBindingPathProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style value that indicates the segments connector line visual representation.
        /// </summary>
        public Style ConnectorLineStyle
        {
            get { return (Style)GetValue(ConnectorLineStyleProperty); }
            set { SetValue(ConnectorLineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the brush value that indicates the interior color of negative segment.
        /// </summary>
        public Brush NegativeSegmentBrush
        {
            get { return (Brush)GetValue(NegativeSegmentBrushProperty); }
            set { SetValue(NegativeSegmentBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the brush value that indicates the consolidated segment's interior.
        /// </summary>
        public Brush SummarySegmentBrush
        {
            get { return (Brush)GetValue(SummarySegmentBrushProperty); }
            set { SetValue(SummarySegmentBrushProperty, value); }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the y values collection.
        /// </summary>
        internal List<bool> SummaryValues
        {
            get;
            set;
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
        /// Creates the segments of WaterfallSeries.
        /// </summary>
        public override void CreateSegments()
        {
            List<double> xValues = GetXValues();
            double median = 0d;
            DoubleRange sbsInfo = this.GetSideBySideInfo(this);
            median = sbsInfo.Delta / 2;
            double origin = ActualXAxis != null ? ActualXAxis.Origin : 0;

            if (ActualXAxis != null && ActualXAxis.Origin == 0
                && ActualYAxis is LogarithmicAxis
                && (ActualYAxis as LogarithmicAxis).Minimum != null)
                origin = (double)(ActualYAxis as LogarithmicAxis).Minimum;

            if (xValues != null)
            {
                double x1, x2, y1, y2;
                ClearUnUsedSegments(this.DataCount);
                ClearUnUsedAdornments(this.DataCount);

                for (int i = 0; i < this.DataCount; i++)
                {
                    if (i < this.DataCount)
                    {
                        // Calculate the waterfall segment rendering values. 
                        OnCalculateSegmentValues(
                            out x1, 
                            out x2, 
                            out y1,
                            out y2,
                            i, 
                            origin,
                            xValues[i]);

                        WaterfallSegment segment = null;
                        bool isSum = false;
                        if (i < Segments.Count)
                        {
                            segment = Segments[i] as WaterfallSegment;

                            segment.SetData(x1, y1, x2, y2);
                            segment.XData = xValues[i];
                            segment.YData = YValues[i];
                            segment.Item = ActualData[i];

                            if (segment.SegmentType == WaterfallSegmentType.Sum)
                                isSum = true;

                            // Update sum segment values.
                            OnUpdateSumSegmentValues(segment, i, isSum, origin);

                            segment.BindProperties();
                        }
                        else
                        {
                            segment = new WaterfallSegment(x1, y1, x2, y2, this)
                            {
                                XData = xValues[i],
                                YData = YValues[i],
                                Item = ActualData[i]
                            };

                            // Raise segment created event.
                            isSum = RaiseSegmentCreatedEvent(segment, i);

                            // Update sum segment values.
                            OnUpdateSumSegmentValues(segment, i, isSum, origin);

                            Segments.Add(segment);
                        }

                        #region Adornment calculation

                        if (AdornmentsInfo != null)
                        {
                            if (segment.SegmentType == WaterfallSegmentType.Sum)
                            {
                                if (Segments.IndexOf(segment) > 0 && AllowAutoSum)
                                {
                                    if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                                        AddColumnAdornments(xValues[i], segment.WaterfallSum, x1, segment.WaterfallSum / 2, i, median);
                                    else if (segment.WaterfallSum >= 0)
                                        AddColumnAdornments(xValues[i], segment.WaterfallSum, x1, segment.Top, i, median);
                                    else
                                        AddColumnAdornments(xValues[i], segment.WaterfallSum, x1, segment.Bottom, i, median);
                                }
                                else
                                {
                                    if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                                        AddColumnAdornments(xValues[i], YValues[i], x1, y1 + (y2 - y1) / 2, i, median);
                                    else if (YValues[i] >= 0)
                                        AddColumnAdornments(xValues[i], YValues[i], x1, segment.Top, i, median);
                                    else
                                        AddColumnAdornments(xValues[i], YValues[i], x1, segment.Bottom, i, median);
                                }
                            }
                            else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                            {
                                if (segment.SegmentType == WaterfallSegmentType.Positive)
                                    AddColumnAdornments(xValues[i], YValues[i], x1, y1, i, median);
                                else
                                    AddColumnAdornments(xValues[i], YValues[i], x1, y2, i, median);
                            }
                            else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                            {
                                if (segment.SegmentType == WaterfallSegmentType.Positive)
                                    AddColumnAdornments(xValues[i], YValues[i], x1, y2, i, median);
                                else
                                    AddColumnAdornments(xValues[i], YValues[i], x1, y1, i, median);
                            }
                            else
                                AddColumnAdornments(xValues[i], YValues[i], x1, y1 + (y2 - y1) / 2, i, median);
                        }

                        #endregion
                    }
                }
            }

            this.ActualArea.IsUpdateLegend = true;
        }

        #endregion

        #region Internal Override Methods

        #region Animation
        
        internal override bool GetAnimationIsActive()
        {
            return sb != null && sb.GetCurrentState() == ClockState.Active;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Reviewed")]
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
                double elementHeight = 0d;
                var element = (FrameworkElement)segment.GetRenderedVisual();
                if (segment is EmptyPointSegment && (!((segment as EmptyPointSegment).IsEmptySegmentInterior) || EmptyPointStyle == EmptyPointStyle.SymbolAndInterior))
                    elementHeight = IsActualTransposed ? ((EmptyPointSegment)segment).EmptyPointSymbolWidth : ((EmptyPointSegment)segment).EmptyPointSymbolHeight;
                else
                    elementHeight = IsActualTransposed ? ((WaterfallSegment)segment).Width : ((WaterfallSegment)segment).Height;
                if (!double.IsNaN(elementHeight) && !double.IsNaN(YValues[i]))
                {
                    if (element == null) return;
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
                        keyFrame1.Value = (YValues[i] > 0) ? (elementHeight * 10) / 100 : -(elementHeight * 10) / 100;
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

        #endregion

        #region Internal Methods

        internal bool RaiseSegmentCreatedEvent(WaterfallSegment segment, int index)
        {
            WaterfallSegmentCreatedEventArgs args = new WaterfallSegmentCreatedEventArgs();
            args.Segment = segment;
            args.Index = index;
            OnSegmentCreated(args);
            return args.IsSummary;
        }

        #endregion

        #region Protected Internal Virtual Methods

        /// <summary>
        /// Occurs when segment created for waterfall series. 
        /// </summary>
        /// <param name="args"></param>
        protected internal virtual void OnSegmentCreated(WaterfallSegmentCreatedEventArgs args)
        {
            if (SegmentCreated != null)
                SegmentCreated(this, args);
        }

        #endregion

        #region Protected Internal Override Methods

        /// <summary>
        /// Method for Generate Points for XYDataSeries
        /// </summary>
        protected internal override void GeneratePoints()
        {
            if (YBindingPath != null)
                GeneratePoints(new string[] { YBindingPath }, YValues);

            // Get summary segment values. 
            GetSummaryValues();
        }

        #endregion

        #region Protected Override Methods

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            base.OnBindingPathChanged(args);
        }

        /// <summary>
        /// Called when DataSource property changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnDataSourceChanged(oldValue, newValue);

            // Get summary segment values. 
            GetSummaryValues();
        }

        /// <summary>
        /// Method implementation for Set points to given index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="obj"></param>
        /// <param name="replace"></param>
        protected override void SetIndividualPoint(int index, object obj, bool replace)
        {
            // Updating summary value of the series.
            if (SummaryBindingPath != null)
            {
                object summaryValue = GetArrayPropertyValue(obj, new string[] { SummaryBindingPath });
                if (replace && SummaryValues.Count > index)
                {
                    SummaryValues[index] = Convert.ToBoolean(summaryValue);
                }
                else
                {
                    SummaryValues.Insert(index, Convert.ToBoolean(summaryValue));
                }
            }

            // Updating x and y value of the series.
            base.SetIndividualPoint(index, obj, replace);

            // Updating segment's SegmentType property based on yvalue. 
            if (SummaryValues != null && index < Segments.Count && index < YValues.Count)
            {
                WaterfallSegment segment = Segments[index] as WaterfallSegment;
                if (SummaryValues[index] == false)
                {
                    if (YValues[index] < 0)
                        segment.SegmentType = WaterfallSegmentType.Negative;
                    else
                        segment.SegmentType = WaterfallSegmentType.Positive;
                }
                else
                    segment.SegmentType = WaterfallSegmentType.Sum;

                // Updating changed segment interior property.
                segment.BindProperties();
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new WaterfallSeries()
            {
                SegmentSpacing = this.SegmentSpacing,
                SelectedIndex = this.SelectedIndex,
                SegmentSelectionBrush = this.SegmentSelectionBrush,
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

        private static void OnAllowAutoSum(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as WaterfallSeries).UpdateArea();
        }
        
        private static void OnSegmentSelectionBrush(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as WaterfallSeries).UpdateArea();
        }
        
        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.ActualArea == null || series.ActualArea.SelectionBehaviour == null) return;
            if (series.ActualArea.SelectionBehaviour.SelectionStyle == SelectionStyle.Single)
                series.SelectedIndexChanged((int)e.NewValue, (int)e.OldValue);
            else if ((int)e.NewValue != -1)
                series.SelectedSegmentsIndexes.Add((int)e.NewValue);
        }
        
        private static void OnSegmentSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as WaterfallSeries;
            if (series.Area != null)
                series.Area.ScheduleUpdate();
        }
        
        private static void OnShowConnectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as WaterfallSeries;

            if (series != null && series.Area != null)
                series.Area.ScheduleUpdate();
        }

        private static void OnNegativeSegmentBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as WaterfallSeries;
            OnUpdateSegmentandAdornmentInterior(series);
        }
        
        private static void OnSummarySegmentBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as WaterfallSeries;
            OnUpdateSegmentandAdornmentInterior(series);
        }

        private static void OnConnectorLineStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WaterfallSeries series = d as WaterfallSeries;

            if (e.NewValue != null)
                foreach (WaterfallSegment segment in series.Segments)
                {
                    segment.LineSegment.Style = e.NewValue as Style;
                }
        }

        private static void OnSummaryBindingPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as WaterfallSeries).OnBindingPathChanged(e);
        }

        /// <summary>
        /// Method used to update the segment and adornment interior color. 
        /// </summary>
        /// <param name="series"></param>
        private static void OnUpdateSegmentandAdornmentInterior(WaterfallSeries series)
        {
            if (series.Area != null)
            {
                foreach (WaterfallSegment segment in series.Segments)
                {
                    segment.BindProperties();
                }

                foreach (var adornment in series.Adornments)
                {
                    var segment = series.Segments.FirstOrDefault(seg => seg.Item == adornment.Item) as WaterfallSegment;

                    if (segment is WaterfallSegment)
                        adornment.BindWaterfallSegmentInterior(segment);
                }
            }
        }

        #endregion

        #region Private Methods

        private void OnCalculateSegmentValues(
            out double x1,
            out double x2, 
            out double y1,
            out double y2,
           int i, 
           double origin, 
           double xVal)
        {
            DoubleRange sbsInfo = this.GetSideBySideInfo(this);

            #region Datapoint calculation

            x1 = xVal + sbsInfo.Start;
            x2 = xVal + sbsInfo.End;

            if (i == 0)
            {
                if (YValues[i] >= 0)
                {
                    y1 = YValues[i];
                    y2 = origin;
                }
                else if (double.IsNaN(YValues[i]))
                {
                    y2 = origin;
                    y1 = origin;
                }
                else
                {
                    y2 = YValues[i];
                    y1 = origin;
                }
            }
            else
            {
                WaterfallSegment prevSegment = Segments[i - 1] as WaterfallSegment;

                // Positive value calculation                            
                if (YValues[i] >= 0)
                {
                    if (YValues[i - 1] >= 0 || prevSegment.SegmentType == WaterfallSegmentType.Sum)
                    {
                        y1 = YValues[i] + prevSegment.Top;
                        y2 = prevSegment.Top;
                    }
                    else if (double.IsNaN(YValues[i - 1]))
                    {
                        y1 = YValues[i] == 0 ? prevSegment.Bottom
                            : prevSegment.Bottom + YValues[i];
                        y2 = prevSegment.Bottom;
                    }
                    else
                    {
                        y1 = YValues[i] + prevSegment.Bottom;
                        y2 = prevSegment.Bottom;
                    }
                }
                else if (double.IsNaN(YValues[i]))
                {
                    // Empty value calculation
                    if (YValues[i - 1] >= 0 || prevSegment.SegmentType == WaterfallSegmentType.Sum)
                        y1 = y2 = prevSegment.Top;
                    else
                        y1 = y2 = prevSegment.Bottom;
                }
                else
                {
                    // Negative value calculation
                    if (YValues[i - 1] >= 0 || prevSegment.SegmentType == WaterfallSegmentType.Sum)
                    {
                        y1 = prevSegment.Top;
                        y2 = YValues[i] + prevSegment.Top;
                    }
                    else
                    {
                        y1 = prevSegment.Bottom;
                        y2 = YValues[i] + prevSegment.Bottom;
                    }
                }
            }
            #endregion
        }

        private void OnUpdateSumSegmentValues(WaterfallSegment segment, int i, bool isSum, double origin)
        {
            double yData = !double.IsNaN(segment.YData) ? segment.YData : 0;

            // Setting previous segment
            if (i - 1 >= 0)
            {
                segment.PreviousWaterfallSegment = (Segments[i - 1] as WaterfallSegment);
            }

            // Set values for sum type segment
            if (isSum || (SummaryValues != null && i < SummaryValues.Count && SummaryValues[i]))
            {
                segment.SegmentType = WaterfallSegmentType.Sum;

                if (segment.PreviousWaterfallSegment != null)
                    segment.WaterfallSum = segment.PreviousWaterfallSegment.Sum;
                else
                    segment.WaterfallSum = yData;

                if (i - 1 >= 0 && AllowAutoSum)
                {
                    segment.Bottom = origin;
                    segment.Top = segment.WaterfallSum;
                }
                else
                {
                    ////segment.Sum = YValues[i];
                    if (YValues[i] >= 0)
                    {
                        segment.Bottom = origin;
                        segment.Top = YValues[i];
                    }
                    else if (double.IsNaN(YValues[i]))
                    {
                        segment.Bottom = segment.Top = origin;
                    }
                    else
                    {
                        segment.Top = origin;
                        segment.Bottom = YValues[i];
                    }
                }

                segment.YRange = new DoubleRange(segment.Top, segment.Bottom);
            }
            else
            {
                if (YValues[i] < 0)
                    segment.SegmentType = WaterfallSegmentType.Negative;
                else
                    segment.SegmentType = WaterfallSegmentType.Positive;
            }

            if (!AllowAutoSum && segment.SegmentType == WaterfallSegmentType.Sum)
                segment.Sum = yData;
            else if (segment.PreviousWaterfallSegment != null && segment.SegmentType != WaterfallSegmentType.Sum)
                segment.Sum = yData + segment.PreviousWaterfallSegment.Sum;
            else if (segment.PreviousWaterfallSegment != null)
                segment.Sum = segment.PreviousWaterfallSegment.Sum;
            else
                segment.Sum = yData;
        }

        private void GetSummaryValues()
        {
            if (ItemsSource == null || string.IsNullOrEmpty(SummaryBindingPath))
                return;

            IEnumerator enumerator = ItemsSource.GetEnumerator();
            PropertyInfo summaryPropertyInfo;
            IPropertyAccessor summaryPropertyAccessor = null;

            if (SummaryValues != null)
                SummaryValues.Clear();
            else
                SummaryValues = new List<bool>();

            IList<bool> summaryValues = SummaryValues;

            if (enumerator.MoveNext())
            {
                summaryPropertyInfo = ChartDataUtils.GetPropertyInfo(enumerator.Current, SummaryBindingPath);

                if (summaryPropertyInfo != null)
                    summaryPropertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(summaryPropertyInfo);

                if (summaryPropertyAccessor == null) return;

                Func<object, object> summaryGetMethod = summaryPropertyAccessor.GetMethod;

                if (summaryGetMethod(enumerator.Current) != null && summaryGetMethod(enumerator.Current).GetType().IsArray)
                    return;

                try
                {
                    do
                    {
                        object summaryVal = summaryGetMethod(enumerator.Current);
                        summaryValues.Add(Convert.ToBoolean(summaryVal));
                    }
                    while (enumerator.MoveNext());
                }
                catch
                {
                }
            }
        }
        
        #endregion

        #endregion
    }

    /// <summary>
    /// Handler implementation for waterfall Segment created event. 
    /// </summary>
    public partial class WaterfallSegmentCreatedEventArgs : EventArgs
    {
        #region Public
        
        #region Public Properties

        /// <summary>
        /// Sets the bool value, which used to identify the corresponding segment is sum segment or not. 
        /// </summary>
        public bool IsSummary { internal get; set; }

        /// <summary>
        /// Gets the corresponding created segment. 
        /// </summary>
        public ChartSegment Segment { get; internal set; }

        /// <summary>
        /// Gets the corresponding created segment's index. 
        /// </summary>
        public int Index { get; internal set; }

        #endregion
        
        #endregion
    }
}
