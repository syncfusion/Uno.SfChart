using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// DoughnutSeries displays data as a proportion of the whole.DoughnutSeries are most commonly used to make comparisons among a set of given data.
    /// </summary>
    /// <remarks>
    /// DoughnutSeries does not have any axis. The segments in DoughnutSeries can be exploded to a certain distance from the center using <see>
    /// <cref>DoughnutSeries.ExplodeIndex</cref>
    /// </see>
    /// or <see>
    /// <cref>DoughnutSeries.ExplodeAll</cref>
    /// </see>
    /// property.
    /// The segments can be filled with a custom set of colors using <see cref="ChartColorModel.CustomBrushes"/> property.
    /// </remarks>
    /// <seealso cref="DoughnutSegment"/>
    /// <seealso cref="PieSeries"/>
    /// <seealso cref="PieSegment"/>
    public partial class DoughnutSeries : CircularSeriesBase, ISegmentSelectable, INotifyPropertyChanged
    {
        #region Dependency Property Registration

        // Using a DependencyProperty as the backing store for TrackBorderWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrackBorderWidthProperty =
            DependencyProperty.Register("TrackBorderWidth", typeof(double), typeof(DoughnutSeries), new PropertyMetadata(0d));
        
        // Using a DependencyProperty as the backing store for brushTrackBorderColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrackBorderColorProperty =
            DependencyProperty.Register("TrackBorderColor", typeof(Brush), typeof(DoughnutSeries), new PropertyMetadata(null));
        
        // Using a DependencyProperty as the backing store for IsStackedDoughnut.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsStackedDoughnutProperty =
            DependencyProperty.Register("IsStackedDoughnut", typeof(bool), typeof(DoughnutSeries), new PropertyMetadata(false, OnDoughnutSeriesPropertyChanged));
        
        // Using a DependencyProperty as the backing store for MaximumValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumValueProperty =
            DependencyProperty.Register("MaximumValue", typeof(double?), typeof(DoughnutSeries), new PropertyMetadata(double.NaN, OnDoughnutSeriesPropertyChanged));

        // Using a DependencyProperty as the backing store for RimColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrackColorProperty =
            DependencyProperty.Register("TrackColor", typeof(Brush), typeof(DoughnutSeries), new PropertyMetadata(null, OnTrackColorPropertyChanged));

        // Using a DependencyProperty as the backing store for CapStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CapStyleProperty =
            DependencyProperty.Register("CapStyle", typeof(DoughnutCapStyle), typeof(DoughnutSeries), new PropertyMetadata(DoughnutCapStyle.BothFlat, OnDoughnutSeriesPropertyChanged));
        
        // Using a DependencyProperty as the backing store for GapRatio.This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SegmentSpacingProperty =
            DependencyProperty.Register("SegmentSpacing", typeof(double), typeof(DoughnutSeries), new PropertyMetadata(0d, OnDoughnutSeriesPropertyChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="DoughnutCoefficient"/> property. 
        /// </summary>
        public static readonly DependencyProperty DoughnutCoefficientProperty =
            DependencyProperty.Register("DoughnutCoefficient", typeof(double), typeof(DoughnutSeries),
            new PropertyMetadata(0.8d, new PropertyChangedCallback(OnDoughnutCoefficientChanged)));

        /// <summary>
        ///  The DependencyProperty for <see cref="DoughnutSize"/> property. 
        /// </summary>
        public static readonly DependencyProperty DoughnutSizeProperty =
            DependencyProperty.Register("DoughnutSize", typeof(double), typeof(DoughnutSeries),
            new PropertyMetadata(0.8d, new PropertyChangedCallback(OnDoughnutSizeChanged)));

        // Using a DependencyProperty as the backing store for DoughnutHoleSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DoughnutHoleSizeProperty =
            DependencyProperty.RegisterAttached("DoughnutHoleSize", typeof(double), typeof(DoughnutSeries), new PropertyMetadata(0.4, OnDoughnutHoleSizeChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="CenterView"/> property. 
        /// </summary>
        public static readonly DependencyProperty CenterViewProperty =
            DependencyProperty.Register("CenterView", typeof(ContentControl), typeof(DoughnutSeries), new PropertyMetadata(null, OnCenterViewPropertyChanged));

        #endregion

        #region Fields

        #region Internal Fields

        internal double InternalDoughnutCoefficient = 0.8;

        internal double InternalDoughnutSize = 0.8;

        #endregion

        #region Private Fields

        private double innerRadius = 0d;

        private double ARCLENGTH;

        private Storyboard sb;

        private int animateCount = 0;

        #endregion

        #endregion

        #region constructor

        /// <summary>
        /// Called when instance created for DoughnutSeries
        /// </summary>
        public DoughnutSeries()
        {
            DefaultStyleKey = typeof(DoughnutSeries);
            InnerRadius = double.NaN;
            
#if NETFX_CORE
            TrackColor = new SolidColorBrush(Windows.UI.Color.FromArgb(51, 128, 128, 128));
#endif
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets a value that specifies the track area border width for stacked doughnut. This is a bindable property.
        /// </summary>
        public double TrackBorderWidth
        {
            get { return (double)GetValue(TrackBorderWidthProperty); }
            set { SetValue(TrackBorderWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the brush that specifies the track area border color for stacked doughnut. This is a bindable property.
        /// </summary>
        public Brush TrackBorderColor
        {
            get { return (Brush)GetValue(TrackBorderColorProperty); }
            set { SetValue(TrackBorderColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to draw stacked doughnut segment.
        /// </summary>
        public bool IsStackedDoughnut
        {
            get { return (bool)GetValue(IsStackedDoughnutProperty); }
            set { SetValue(IsStackedDoughnutProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the maximum value for the doughnut segment when stacked doughnut is used.
        /// </summary>
        public double MaximumValue
        {
            get { return (double)GetValue(MaximumValueProperty); }
            set { SetValue(MaximumValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the brush that specifies the track area segment color for stacked doughnut. This is a bindable property.
        /// </summary>
        public Brush TrackColor
        {
            get { return (Brush)GetValue(TrackColorProperty); }
            set { SetValue(TrackColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the capstyle that specifies the start and end points of doughtnut segment. This is a bindable property. 
        /// </summary>
        public DoughnutCapStyle CapStyle
        {
            get { return (DoughnutCapStyle)GetValue(CapStyleProperty); }
            set { SetValue(CapStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that specifies the gap ratio for the doughnut segments. This is a bindable property.
        /// </summary>
        /// <value>
        /// The double value ranges from 0 to 1.
        /// </value>
        public double SegmentSpacing
        {
            get { return (double)GetValue(SegmentSpacingProperty); }
            set { SetValue(SegmentSpacingProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that specifies the inner circular radius of the DoughnutSeries. This is a bindable property.
        /// </summary>
        /// <value>
        /// The double value ranges from 0 to 1.
        /// </value>
        public double DoughnutCoefficient
        {
            get { return (double)GetValue(DoughnutCoefficientProperty); }
            set { SetValue(DoughnutCoefficientProperty, value); }
        }

        /// <summary>
        /// Gets or sets the size of the <c>DoughnutSeries</c>.
        /// </summary>
        public double DoughnutSize
        {
            get { return (double)GetValue(DoughnutSizeProperty); }
            set { SetValue(DoughnutSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the view to be added to the center of the <c>DoughnutSeries</c>.
        /// </summary>
        public ContentControl CenterView
        {
            get { return (ContentControl)GetValue(CenterViewProperty); }
            set { SetValue(CenterViewProperty, value); }
        }

#if NETFX_CORE
        /// <summary>
        /// Gets for internal usage
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        public DoughnutSegment Segment
        {
            get { return null; }
        }
#endif

        #endregion

        #region Internal Properties

        public double InnerRadius
        {
            get
            {
                return innerRadius;
            }
            internal set
            {
                innerRadius = value;
                OnPropertyChanged("InnerRadius");
            }
        }

        internal double SegmentGapAngle { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Static Methods

        public static double GetDoughnutHoleSize(DependencyObject obj)
        {
            return (double)obj.GetValue(DoughnutHoleSizeProperty);
        }

        public static void SetDoughnutHoleSize(DependencyObject obj, double value)
        {
            obj.SetValue(DoughnutHoleSizeProperty, value);
        }

        #endregion

        #region Public Override Methods

        /// <summary>
        /// Creates the doughnut segments.
        /// </summary>
        public override void CreateSegments()
        {
            IList<double> toggledYValues = null;
            List<double> xValues = null;
            var actualData = ActualData;

            if (double.IsNaN(GroupTo))
            {
                if (ToggledLegendIndex.Count > 0)
                    toggledYValues = GetYValues();
                else
                    toggledYValues = YValues;

                xValues = GetXValues();
            }
            else
            {
                if (Adornments != null) Adornments.Clear();
                if (Segments != null) Segments.Clear();

                var sumOfYValues = (from val in YValues
                                    select (val) > 0 ? val : Math.Abs(double.IsNaN(val) ? 0 : val)).Sum();
                double xIndexValues = 0d;
                xValues = (from val in YValues where GetGroupModeValue(val, sumOfYValues) > GroupTo select (xIndexValues++)).ToList();
                if (YValues.Count > xValues.Count) xValues.Add(xIndexValues);

                var groupToValues = GetGroupToYValues();
                actualData = groupToValues.Item2;

                if (ToggledLegendIndex.Count > 0)
                    toggledYValues = GetToggleYValues(groupToValues.Item1);
                else
                    toggledYValues = groupToValues.Item1;

            }

            double arcEndAngle = DegreeToRadianConverter(EndAngle), arcStartAngle = DegreeToRadianConverter(StartAngle);
            if (arcStartAngle == arcEndAngle)
                Segments.Clear();
            ARCLENGTH = arcEndAngle - arcStartAngle;
            if (Math.Abs(Math.Round(ARCLENGTH, 2)) > TotalArcLength)
                ARCLENGTH = ARCLENGTH % TotalArcLength;
            ClearUnUsedAdornments(this.DataCount);
            ClearUnUsedSegments(this.DataCount);
            int explodedIndex = ExplodeIndex;
            bool explodedAll = ExplodeAll;

            if (xValues != null)
            {
                var grandTotal = (from val in toggledYValues
                                  select (val) > 0 ? val : Math.Abs(double.IsNaN(val) ? 0 : val)).Sum();
                
                var isMultipleDoughnut = !double.IsNaN(MaximumValue) && IsStackedDoughnut && GetDoughnutSeriesCount() == 1;                    
                bool isEndValueExceed = false;
                var visibleSegmentCount = 0;

                for (int i = 0; i < xValues.Count; i++)
                {
                    isEndValueExceed = false;
                    if (isMultipleDoughnut)
                    {
                        isEndValueExceed = (toggledYValues[i] >= MaximumValue);
                        arcEndAngle = grandTotal == 0 ? 0 : (Math.Abs(double.IsNaN(toggledYValues[i]) ? 0 : isEndValueExceed ? MaximumValue : toggledYValues[i]) * (ARCLENGTH / MaximumValue));
                    }
                    else
                    {
                        arcEndAngle = grandTotal == 0 ? 0 : (Math.Abs(double.IsNaN(toggledYValues[i]) ? 0 : toggledYValues[i]) * (ARCLENGTH / grandTotal));
                    }

                    if (i < Segments.Count)
                    {
                        var doughnutSegment = Segments[i] as DoughnutSegment;

                        doughnutSegment.SetData(arcStartAngle, arcStartAngle + arcEndAngle, this);
                        doughnutSegment.XData = xValues[i];
                        doughnutSegment.YData = !double.IsNaN(GroupTo) ? Math.Abs(toggledYValues[i]) : Math.Abs(YValues[i]);
                        doughnutSegment.AngleOfSlice = (2 * arcStartAngle + arcEndAngle) / 2;
                        doughnutSegment.IsExploded = explodedAll || (explodedIndex == i);
                        doughnutSegment.Item = actualData[i];
                        doughnutSegment.IsEndValueExceed = isEndValueExceed;
                        doughnutSegment.DoughnutSegmentIndex = visibleSegmentCount;
                        if (SegmentColorPath != null && !Segments[i].IsEmptySegmentInterior && ColorValues.Count > 0 && !Segments[i].IsSelectedSegment)
                            Segments[i].Interior = (Interior != null) ? Interior : ColorValues[i];
                        if (ToggledLegendIndex.Contains(i))
                            Segments[i].IsSegmentVisible = false;
                        else
                            Segments[i].IsSegmentVisible = true;

                        doughnutSegment.UpdateTrackInterior(i);
                    }
                    else
                    {
                        DoughnutSegment segment = new DoughnutSegment(arcStartAngle, arcStartAngle + arcEndAngle,
                            this)
                        {
                            XData = xValues[i],
                            YData = !double.IsNaN(GroupTo) ? Math.Abs(toggledYValues[i]) : Math.Abs(YValues[i]),
                            AngleOfSlice = (2 * arcStartAngle + arcEndAngle) / 2,
                            IsExploded = explodedAll || (explodedIndex == i),
                            Item = actualData[i],
                            IsEndValueExceed = isEndValueExceed,
                            DoughnutSegmentIndex = visibleSegmentCount
                        };

                        segment.SetData(arcStartAngle, arcStartAngle + arcEndAngle,
                                this);
                        if (ToggledLegendIndex.Contains(i))
                            segment.IsSegmentVisible = false;
                        else
                            segment.IsSegmentVisible = true;
                        Segments.Add(segment);

                        segment.UpdateTrackInterior(i);
                    }
                    
                    if (AdornmentsInfo != null)
                        AddDoughnutAdornments(
                            xValues[i],
                            toggledYValues[i],
                            arcStartAngle,
                            arcStartAngle + arcEndAngle,
                            Segments.Count - 1,
                            i);

                    if (!double.IsNaN(toggledYValues[i]))
                    {
                        visibleSegmentCount++;
                    }

                    if (!IsStackedDoughnut)
                    {
                        arcStartAngle += arcEndAngle;
                    }
                }

                if (ShowEmptyPoints)
                    UpdateEmptyPointSegments(xValues, false);

                UpdateSegmentGapAngle();
            }
        }

        internal void ManipulateAdditionalVisual(UIElement element, NotifyCollectionChangedAction action)
        {
            var frameworkElement = element as FrameworkElement;
            DoughnutSegment doughnutSegment;
            if (frameworkElement != null)
            {
                switch (action)
                {
                    case NotifyCollectionChangedAction.Add:
                        doughnutSegment = frameworkElement.Tag as DoughnutSegment;

                        if (doughnutSegment != null)
                        {
                            if (!SeriesPanel.Children.Contains(doughnutSegment.CircularDoughnutPath))
                                SeriesPanel.Children.Add(doughnutSegment.CircularDoughnutPath);
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                         doughnutSegment = frameworkElement.Tag as DoughnutSegment;

                            if (doughnutSegment != null)
                            {
                                if (SeriesPanel.Children.Contains(doughnutSegment.CircularDoughnutPath))
                                    SeriesPanel.Children.Remove(doughnutSegment.CircularDoughnutPath);
                            }
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        doughnutSegment = frameworkElement.Tag as DoughnutSegment;

                        if (doughnutSegment != null)
                        {
                            if (!SeriesPanel.Children.Contains(doughnutSegment.CircularDoughnutPath))
                                SeriesPanel.Children.Add(doughnutSegment.CircularDoughnutPath);
                        }
                        break;
                }
            }
        }

        #endregion

        #region Internal Static Methods

        internal static void RemoveCenterView(ContentControl centerView)
        {
            if (centerView != null)
            {
                centerView.Content = null;
                var parent = centerView.Parent as Grid;
                if (parent != null)
                {
                    parent.Children.Remove(centerView);
                }
            }
        }

        #endregion

        #region Internal Override Methods

        internal override void UpdateEmptyPointSegments(List<double> xValues, bool isSidebySideSeries)
        {
            if (EmptyPointIndexes != null)
                foreach (var item in EmptyPointIndexes[0])
                {
                    DoughnutSegment segment = Segments[item] as DoughnutSegment;
                    bool explode = segment.IsExploded;
                    Segments[item].IsEmptySegmentInterior = true;
                    (Segments[item] as DoughnutSegment).AngleOfSlice = segment.AngleOfSlice;
                    (Segments[item] as DoughnutSegment).IsExploded = explode;
                    if (Adornments.Count > 0)
                        Adornments[item].IsEmptySegmentInterior = true;
                }
        }

        internal override bool GetAnimationIsActive()
        {
            return sb != null && sb.GetCurrentState() == ClockState.Active;
        }

        /// <summary>
        /// Virtual Method for Animate
        /// </summary>
        internal override void Animate()
        {
            double startAngle = DegreeToRadianConverter(StartAngle);

            if (this.Segments.Count > 0)
            {
                // WPF-25124 Animation not working properly when resize the window.
                if (sb != null && (animateCount <= Segments.Count))
                    sb = new Storyboard();
                else if (sb != null)
                {
                    sb.Stop();
                    if (!canAnimate)
                    {
                        foreach (DoughnutSegment segment in this.Segments)
                        {
                            if ((segment.EndAngle - segment.StartAngle) == 0) continue;
                            segment.ActualStartAngle = segment.StartAngle;
                            segment.ActualEndAngle = segment.EndAngle;
                        }

                        ResetAdornmentAnimationState();
                        return;
                    }
                }
                else
                    sb = new Storyboard();

                AnimateAdornments(sb);

                foreach (DoughnutSegment segment in this.Segments)
                {
                    animateCount++;
                    double segStartAngle = segment.StartAngle;
                    double segEndAngle = segment.EndAngle;

                    if ((segEndAngle - segStartAngle) == 0) continue;

                    DoubleAnimationUsingKeyFrames keyFrames = new DoubleAnimationUsingKeyFrames();
                    SplineDoubleKeyFrame keyFrame = new SplineDoubleKeyFrame
                    {
                        KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                        Value = startAngle
                    };
                    keyFrames.KeyFrames.Add(keyFrame);

                    keyFrame = new SplineDoubleKeyFrame
                    {
                        KeyTime = KeyTime.FromTimeSpan(AnimationDuration),
                        Value = segStartAngle
                    };
                    var keySpline = new KeySpline();
                    keySpline.ControlPoint1 = new Point(0.64, 0.84);
                    keySpline.ControlPoint2 = new Point(0.67, 0.95);
                    keyFrame.KeySpline = keySpline;

                    keyFrames.KeyFrames.Add(keyFrame);
                    Storyboard.SetTargetProperty(keyFrames, "DoughnutSegment.ActualStartAngle");
                    keyFrames.EnableDependentAnimation = true;
                    Storyboard.SetTarget(keyFrames, segment);
                    sb.Children.Add(keyFrames);

                    keyFrames = new DoubleAnimationUsingKeyFrames();
                    keyFrame = new SplineDoubleKeyFrame
                    {
                        KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                        Value = startAngle
                    };
                    keyFrames.KeyFrames.Add(keyFrame);

                    keyFrame = new SplineDoubleKeyFrame
                    {
                        KeyTime = KeyTime.FromTimeSpan(AnimationDuration),
                        Value = segEndAngle
                    };
                    keySpline = new KeySpline
                    {
                        ControlPoint1 = new Point(0.64, 0.84),
                        ControlPoint2 = new Point(0.67, 0.95)
                    };
                    keyFrame.KeySpline = keySpline;

                    keyFrames.KeyFrames.Add(keyFrame);
                    Storyboard.SetTargetProperty(keyFrames, "DoughnutSegment.ActualEndAngle");
                    keyFrames.EnableDependentAnimation = true;
                    Storyboard.SetTarget(keyFrames, segment);
                    sb.Children.Add(keyFrames);
                }

                sb.Begin();
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

        #region Internal Methods

        internal void AddCenterView(ContentControl centerView)
        {
            if (centerView != null)
            {
                var parent = this.Parent as Grid;
                if (parent != null && centerView.Parent == null)
                {
                    parent.Children.Add(centerView);
                }
            }
        }

        /// <summary>
        /// Gets the doughnut series count.
        /// </summary>
        /// <returns></returns>
        internal int GetDoughnutSeriesCount()
        {
            return (from series in Area.VisibleSeries where series is DoughnutSeries select series).ToList().Count;
        }

        #endregion

        #region Protected Internal Override Methods

        /// <summary>
        /// Return IChartTransformer value from the given size
        /// </summary>
        /// <param name="size"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        protected internal override IChartTransformer CreateTransformer(Size size, bool create)
        {
            if (create || ChartTransformer == null)
            {
                ChartTransformer = ChartTransform.CreateSimple(size);
            }

            return ChartTransformer;
        }


        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Method implementation for Create Adornments
        /// </summary>
        /// <param name="series"></param>
        /// <param name="xVal"></param>
        /// <param name="yVal"></param>
        /// <param name="angle"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        protected override ChartAdornment CreateAdornment(AdornmentSeries series, double xVal, double yVal, double angle, double radius)
        {
            var segment = new ChartPieAdornment(xVal, yVal, angle, radius, series);
            segment.SetValues(xVal, yVal, angle, radius, series);
            return segment;
        }

        /// <summary>
        /// Method implementation for ExplodeIndex
        /// </summary>
        /// <param name="i"></param>
        protected override void SetExplodeIndex(int i)
        {
            if (Segments.Count > 0)
            {
                foreach (DoughnutSegment segment in Segments)
                {
                    int index = !double.IsNaN(GroupTo) ? Segments.IndexOf(segment) : ActualData.IndexOf(segment.Item);
                    if (i == index)
                    {
                        segment.IsExploded = !segment.IsExploded;
                        UpdateSegments(i, NotifyCollectionChangedAction.Remove);
                    }
                    else if (i == -1)
                    {
                        segment.IsExploded = false;
                        UpdateSegments(i, NotifyCollectionChangedAction.Remove);
                    }
                }
            }
        }

        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (oldValue != null)
                animateCount = 0;
            base.OnDataSourceChanged(oldValue, newValue);
        }

        /// <summary>
        /// Virtual Method for ExplodeRadius
        /// </summary>
        protected override void SetExplodeRadius()
        {
            if (Segments.Count > 0)
            {
                foreach (DoughnutSegment segment in Segments)
                {
                    int index = Segments.IndexOf(segment);
                    UpdateSegments(index, NotifyCollectionChangedAction.Replace);
                }
            }
        }

        /// <summary>
        /// Virtual method for ExplodeAll
        /// </summary>
        protected override void SetExplodeAll()
        {
            if (Segments.Count > 0)
            {
                foreach (DoughnutSegment segment in Segments)
                {
                    int index = Segments.IndexOf(segment);
                    segment.IsExploded = true;
                    UpdateSegments(index, NotifyCollectionChangedAction.Replace);
                }
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new DoughnutSeries() { DoughnutCoefficient = this.DoughnutCoefficient });
        }

        #endregion

        #region Private Static Methods

        private static void OnTrackColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var doughnutSeries = d as DoughnutSeries;

            if (doughnutSeries != null && doughnutSeries.Segments != null)
            {
                for (int i = 0; i < doughnutSeries.Segments.Count; i++)
                {
                    (doughnutSeries.Segments[i] as DoughnutSegment).UpdateTrackInterior(i);
                }
            }
        }

        private static void OnDoughnutSeriesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as DoughnutSeries;
            if(series != null)
            {
                series.UpdateArea();
            }
        }

        private static void OnDoughnutCoefficientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as DoughnutSeries;
            series.InternalDoughnutCoefficient = ChartMath.MinMax((double)e.NewValue, 0, 1);
            series.UpdateArea();
        }

        private static void OnDoughnutSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as DoughnutSeries;
            series.InternalDoughnutSize = ChartMath.MinMax((double)e.NewValue, 0, 1);
            series.UpdateArea();
        }

        private static void OnDoughnutHoleSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var chartArea = sender as SfChart;
            if (chartArea != null)
            {
                chartArea.InternalDoughnutHoleSize = ChartMath.MinMax((double)e.NewValue, 0, 1);
                chartArea.ScheduleUpdate();
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            AddCenterView(CenterView);
        }

        private static void OnCenterViewPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var series = sender as DoughnutSeries;
            DoughnutSeries.RemoveCenterView(e.OldValue as ContentControl);
            series.AddCenterView(e.NewValue as ContentControl);
        }

#endregion

#region Private Methods

        private void UpdateSegmentGapAngle()
        {
            if (!IsStackedDoughnut && GetDoughnutSeriesCount() == 1)
            {
                SegmentGapAngle = ((double)SegmentSpacing * Math.Abs(ARCLENGTH)) / Segments.Count;
            }
        }

        private void AddDoughnutAdornments(double x, double y, double startAngle, double endAngle, int i, int index)
        {
            double angle = (startAngle + endAngle) / 2;
            if (Area == null || Area.RootPanelDesiredSize == null || Area.RootPanelDesiredSize.Value == null)
                return;

            var actualheight = Area.RootPanelDesiredSize.Value.Height;
            var actualwidth = Area.RootPanelDesiredSize.Value.Width;

            double doughnutIndex = 0d, actualRadius = 0d, remainingWidth =0 , equalParts = 0d, radius = 0d;        

            if (IsStackedDoughnut)
            {
                int doughnutSegmentsCount = DataCount;
                actualRadius = this.InternalDoughnutSize * (Math.Min(actualwidth, actualheight)) / 2;
                remainingWidth = actualRadius - (actualRadius * ActualArea.InternalDoughnutHoleSize);
                equalParts = (remainingWidth / doughnutSegmentsCount) * InternalDoughnutCoefficient;

                // Segments count is not updated so datacount is used. For more safer update in dynamic updates the radius is also updated in the ChartPieAdornment.Update.
                radius = actualRadius - (equalParts * (doughnutSegmentsCount - (index + 1)));
            }
            else
            {
                var doughnutSeries = (from series in Area.VisibleSeries where series is DoughnutSeries select series).ToList();
                var doughnutSeriesCount = doughnutSeries.Count();
                doughnutIndex = doughnutSeries.IndexOf(this);
                actualRadius = this.InternalDoughnutSize * (Math.Min(actualwidth, actualheight)) / 2;
                remainingWidth = actualRadius - (actualRadius * Area.InternalDoughnutHoleSize);
                equalParts = remainingWidth / doughnutSeriesCount;
                radius = actualRadius - (equalParts * (doughnutSeriesCount - (doughnutIndex + 1)));
            }
            if (index < Adornments.Count)
            {
                Adornments[index].SetData(x, y, angle, radius);
            }
            else
                Adornments.Add(this.CreateAdornment(this, x, y, angle, radius));
            Adornments[index].Item = !double.IsNaN(GroupTo) ? Segments[index].Item :  ActualData[index];
        }

        #endregion

        #endregion
    }
}
