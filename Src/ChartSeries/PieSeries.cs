using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// PieSeries displays data as a proportion of the whole.PieSeries are most commonly used to make comparisons among a set of given data.
    /// </summary>
    /// <remarks>
    /// PieSeries does not have any axis. The segments in PieSeries can be exploded to a certain distance from the center using <see>
    /// <cref>PieSeries.ExplodeIndex</cref>
    /// </see>
    /// or <see>
    /// <cref>PieSeries.ExplodeAll</cref>
    /// </see>
    /// property.
    /// The segments can be filled with a custom set of colors using <see cref="ChartColorModel.CustomBrushes"/> property.
    /// </remarks>
    /// <seealso cref="PieSegment"/>
    /// <seealso cref="DoughnutSeries"/>
    /// <seealso cref="DoughnutSegment"/>
    public partial class PieSeries : CircularSeriesBase, ISegmentSelectable
    {
        #region Dependency Property Registration
        
        /// <summary>
        ///  The DependencyProperty for <see cref="PieCoefficient"/> property.       
        /// </summary>
        public static readonly DependencyProperty PieCoefficientProperty =
            DependencyProperty.Register(
                "PieCoefficient",
                typeof(double), 
                typeof(PieSeries),
                new PropertyMetadata(0.8d, OnPieCoefficientPropertyChanged));

        #endregion

        #region Fields

        #region Internal Fields

        internal double InternalPieCoefficient = 0.8;
        
        #endregion

        #region Private Fields

        private Storyboard sb;

        private double grandTotal = 0d;

        private double ARCLENGTH;

        private double arcStartAngle, arcEndAngle;
        
        private int animateCount = 0;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for PieSeries
        /// </summary>
        public PieSeries()
        {
            DefaultStyleKey = typeof(PieSeries);
        }

        #endregion

        #region Properties

        #region Public Properties

#if NETFX_CORE
        /// <summary>
        /// Gets for internal usage.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        public PieSegment Segment
        {
            get { return null; }
        }
#endif

        /// <summary>
        /// Gets or sets a value that specifies the ratio of pie size with respect to chart area. This is a bindable property.
        /// </summary>
        /// <value>
        /// The value ranges from 0 to 1.
        /// </value>
        public double PieCoefficient
        {
            get { return (double)GetValue(PieCoefficientProperty); }
            set { SetValue(PieCoefficientProperty, value); }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Creates the segments of PieSeries.
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

            ClearUnUsedAdornments(this.DataCount);
            ClearUnUsedSegments(this.DataCount);
            int explodedIndex = ExplodeIndex;
            bool explodedAll = ExplodeAll;
            arcStartAngle = DegreeToRadianConverter(StartAngle);
            arcEndAngle = DegreeToRadianConverter(EndAngle);
            if (arcStartAngle == arcEndAngle)
                Segments.Clear();
            ARCLENGTH = arcEndAngle - arcStartAngle;
            if (Math.Abs(Math.Round(ARCLENGTH, 2)) > TotalArcLength)
                ARCLENGTH = ARCLENGTH % TotalArcLength;
            if (xValues != null)
            {
                grandTotal = (from val in toggledYValues
                              select (val) > 0 ? val : Math.Abs(double.IsNaN(val) ? 0 : val)).Sum();
                for (int i = 0; i < xValues.Count; i++)
                {
                    arcEndAngle = grandTotal == 0 ? 0 : (Math.Abs(double.IsNaN(toggledYValues[i]) ? 0 : toggledYValues[i]) * (ARCLENGTH / grandTotal));

                    if (i < Segments.Count)
                    {
                        (Segments[i] as PieSegment).SetData(
                            arcStartAngle, 
                            arcStartAngle + arcEndAngle, 
                            this,
                            actualData[i]);
                        (Segments[i] as PieSegment).XData = xValues[i];
                        (Segments[i] as PieSegment).YData = !double.IsNaN(GroupTo) ? Math.Abs(toggledYValues[i]) : Math.Abs(YValues[i]);
                        (Segments[i] as PieSegment).AngleOfSlice = (2 * arcStartAngle + arcEndAngle) / 2;
                        (Segments[i] as PieSegment).IsExploded = explodedAll || (explodedIndex == i);
                        (Segments[i] as PieSegment).Item = actualData[i];
                        if (SegmentColorPath != null && !Segments[i].IsEmptySegmentInterior && ColorValues.Count > 0 && !Segments[i].IsSelectedSegment)
                            Segments[i].Interior = (Interior != null) ? Interior : ColorValues[i];
                        if (ToggledLegendIndex.Contains(i))
                            Segments[i].IsSegmentVisible = false;
                        else
                            Segments[i].IsSegmentVisible = true;
                    }
                    else
                    {
                        var segment = new PieSegment(arcStartAngle, arcStartAngle + arcEndAngle, this, actualData[i]);

                        segment.SetData(arcStartAngle, arcStartAngle + arcEndAngle, this, actualData[i]);

                        segment.XData = xValues[i];
                        segment.YData = !double.IsNaN(GroupTo) ? Math.Abs(toggledYValues[i]) : Math.Abs(YValues[i]);
                        segment.AngleOfSlice = (2 * arcStartAngle + arcEndAngle) / 2;
                        segment.IsExploded = explodedAll || explodedIndex == i;
                        segment.Item = actualData[i];
                        if (ToggledLegendIndex.Contains(i))
                            segment.IsSegmentVisible = false;
                        else
                            segment.IsSegmentVisible = true;
                        Segments.Add(segment);
                    }

                    if (AdornmentsInfo != null)
                        AddPieAdornments(xValues[i], toggledYValues[i], arcStartAngle, arcStartAngle + arcEndAngle, Segments.Count - 1, i, Segments[i] as PieSegment);
                    arcStartAngle += arcEndAngle;
                }

                if (ShowEmptyPoints)
                    UpdateEmptyPointSegments(xValues, false);
            }
        }

        #endregion

        #region Internal Override Methods

        internal override void UpdateEmptyPointSegments(List<double> xValues, bool isSidebySideSeries)
        {
            if (EmptyPointIndexes != null)
                foreach (var item in EmptyPointIndexes[0])
                {
                    PieSegment segment = Segments[item] as PieSegment;
                    bool explode = segment.IsExploded;
                    Segments[item].IsEmptySegmentInterior = true;
                    (Segments[item] as PieSegment).AngleOfSlice = segment.AngleOfSlice;
                    (Segments[item] as PieSegment).IsExploded = explode;
                    if (Adornments.Count > 0)
                        Adornments[item].IsEmptySegmentInterior = true;
                }
        }

        internal override bool GetAnimationIsActive()
        {
            return sb != null && sb.GetCurrentState() == ClockState.Active;
        }

        internal override void Animate()
        {
            if (this.Segments.Count > 0)
            {
                double startAngle = DegreeToRadianConverter(StartAngle);

                // WPF-25124 Animation not working properly when resize the window.
                if (sb != null && (animateCount <= Segments.Count))
                    sb = new Storyboard();
                else if (sb != null)
                {
                    sb.Stop();
                    if (!canAnimate)
                    {
                        foreach (PieSegment segment in this.Segments)
                        {
                            if ((segment.EndAngle - segment.StartAngle) == 0) continue;
                            FrameworkElement element = segment.GetRenderedVisual() as FrameworkElement;
                            element.ClearValue(FrameworkElement.RenderTransformProperty);
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

                foreach (PieSegment segment in this.Segments)
                {
                    animateCount++;
                    double segStartAngle = segment.StartAngle;
                    double segEndAngle = segment.EndAngle;
                    if ((segEndAngle - segStartAngle) == 0) continue;
                    FrameworkElement element = segment.GetRenderedVisual() as FrameworkElement;
                    element.Width = this.DesiredSize.Width;
                    element.Height = this.DesiredSize.Height;
                    element.RenderTransform = new ScaleTransform();
                    element.RenderTransformOrigin = new Point(0.5, 0.5);

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
                    Storyboard.SetTargetProperty(keyFrames, "PieSegment.ActualStartAngle");
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
                    Storyboard.SetTargetProperty(keyFrames, "PieSegment.ActualEndAngle");
                    keyFrames.EnableDependentAnimation = true;
                    Storyboard.SetTarget(keyFrames, segment);
                    sb.Children.Add(keyFrames);

                    keyFrames = new DoubleAnimationUsingKeyFrames();
                    keyFrame = new SplineDoubleKeyFrame
                    {
                        KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                        Value = 0
                    };
                    keyFrames.KeyFrames.Add(keyFrame);

                    keyFrame = new SplineDoubleKeyFrame
                    {
                        KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 80 / 100))),
                        Value = 1
                    };
                    keySpline = new KeySpline
                    {
                        ControlPoint1 = new Point(0.64, 0.84),
                        ControlPoint2 = new Point(0.67, 0.95)
                    };
                    keyFrame.KeySpline = keySpline;

                    keyFrames.KeyFrames.Add(keyFrame);
                    Storyboard.SetTargetProperty(keyFrames, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");
                    Storyboard.SetTarget(keyFrames, element);
                    sb.Children.Add(keyFrames);
                    keyFrames = new DoubleAnimationUsingKeyFrames();
                    keyFrame = new SplineDoubleKeyFrame();
                    keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                    keyFrame.Value = 0;
                    keyFrames.KeyFrames.Add(keyFrame);
                    keyFrame = new SplineDoubleKeyFrame
                    {
                        KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 80 / 100))),
                        Value = 1
                    };
                    keySpline = new KeySpline
                    {
                        ControlPoint1 = new Point(0.64, 0.84),
                        ControlPoint2 = new Point(0.67, 0.95)
                    };
                    keyFrame.KeySpline = keySpline;

                    keyFrames.KeyFrames.Add(keyFrame);
                    Storyboard.SetTargetProperty(keyFrames, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");
                    Storyboard.SetTarget(keyFrames, element);
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

        /// <summary>
        /// Gets the pie series count.
        /// </summary>
        /// <returns></returns>
        internal int GetPieSeriesCount()
        {
            return (from series in Area.VisibleSeries where series is PieSeries select series).ToList().Count;
        }

        #endregion

        #region Protected Internal Override Methods

        /// <summary>
        /// Return IChartTranform value based upon the given size
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

        #region Protected Methods

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
                foreach (PieSegment segment in Segments)
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
        
        /// <summary>
        /// Virtual Method for ExplodeRadius
        /// </summary>
        protected override void SetExplodeRadius()
        {
            if (Segments.Count > 0)
            {
                foreach (PieSegment segment in Segments)
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
                foreach (PieSegment segment in Segments)
                {
                    int index = Segments.IndexOf(segment);
                    segment.IsExploded = true;
                    UpdateSegments(index, NotifyCollectionChangedAction.Replace);
                }
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new PieSeries() { PieCoefficient = this.PieCoefficient });
        }

        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (oldValue != null)
                animateCount = 0;
            base.OnDataSourceChanged(oldValue, newValue);
        }

        #endregion

        #region Private Static Methods

        private static void OnPieCoefficientPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as PieSeries;
            series.InternalPieCoefficient = ChartMath.MinMax((double)e.NewValue, 0, 1);
            series.UpdateArea();
        }

        #endregion

        #region Private Methods

        private void AddPieAdornments(double x, double y, double startAngle, double endAngle, int i, int index, PieSegment pieSegment)
        {
            double angle = (startAngle + endAngle) / 2;

            if (Area == null || Area.RootPanelDesiredSize == null || Area.RootPanelDesiredSize.Value == null)
                return;

            var actualheight = Area.RootPanelDesiredSize.Value.Height;
            var actualwidth = Area.RootPanelDesiredSize.Value.Width;

            var pieSeries = (from series in Area.VisibleSeries where series is PieSeries select series).ToList();
            var pieSeriesCount = pieSeries.Count();

            double pieIndex = pieSeries.IndexOf(this);
            double actualRadius = (Math.Min(actualwidth, actualheight)) / 2;
            double equalParts = actualRadius / (pieSeriesCount);
            double radius = (equalParts * (pieIndex + 1)) - (equalParts * (1 - InternalPieCoefficient));

            if (index < Adornments.Count)
                (Adornments[index] as ChartPieAdornment).SetData(x, y, angle, radius);
            else
                Adornments.Add(this.CreateAdornment(this, x, y, angle, radius));

            Adornments[index].Item = !double.IsNaN(GroupTo) ? Segments[index].Item : ActualData[index];
        }

        #endregion

        #endregion
    }
}
