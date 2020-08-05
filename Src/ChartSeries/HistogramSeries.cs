using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Threading.Tasks;
using System.Collections;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Histogram consists tabular frequencies, which are shown as adjacent rectangles and erected over discrete intervals.
    /// </summary>
    /// <remarks>   
    /// The rectangle area is equal to the frequency of the observations in <see cref="HistogramSeries.HistogramInterval"/>
    /// The height of a rectangle is also equal to the frequency density of the interval.A histogram may also be normalized displaying relative frequencies.
    /// You can also draw a normal distribution curve for the given data points, by enabling the <see cref="HistogramSeries.ShowNormalDistributionCurve"/>
    /// </remarks>
    /// <seealso cref="HistogramSegment"/>
    /// <seealso cref="ColumnSeries"/>
    /// <seealso cref="BarSeries"/>
    public partial class HistogramSeries : AdornmentSeries, ISupportAxes2D
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="YBindingPath"/> property. 
        /// </summary>
        public static readonly DependencyProperty YBindingPathProperty =
            DependencyProperty.Register(
                "YBindingPath",
                typeof(string),
                typeof(HistogramSeries),
                new PropertyMetadata(null, OnYPathChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="HistogramInterval"/> property.       
        /// </summary>
        public static readonly DependencyProperty HistogramIntervalProperty =
            DependencyProperty.Register(
                "HistogramInterval", 
                typeof(double),
                typeof(HistogramSeries),
                new PropertyMetadata(1d, OnPropertyChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="ShowNormalDistributionCurve"/> property.       
        /// </summary>
        public static readonly DependencyProperty ShowNormalDistributionCurveProperty =
            DependencyProperty.Register(
                "ShowNormalDistributionCurve", 
                typeof(bool), 
                typeof(HistogramSeries),
                new PropertyMetadata(true, OnPropertyChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="XAxis"/> property.  
        /// </summary>   
        public static readonly DependencyProperty XAxisProperty =
            DependencyProperty.Register(
                "XAxis", 
                typeof(ChartAxisBase2D),
                typeof(HistogramSeries),
                new PropertyMetadata(null, OnXAxisChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="YAxis"/> property.       
        /// </summary>
        public static readonly DependencyProperty YAxisProperty =
            DependencyProperty.Register(
                "YAxis", 
                typeof(RangeAxisBase), 
                typeof(HistogramSeries),
                new PropertyMetadata(null, OnYAxisChanged));

        #endregion

        #region Fields

        #region Constants

        /// <summary>
        /// Initializes c_distributionPointsCount
        /// </summary>
        private const int C_distributionPointsCount = 500;

        /// <summary>
        /// Initializes c_sqrtDoublePI
        /// </summary>
        private readonly static double c_sqrtDoublePI = Math.Sqrt(2 * Math.PI);
        
        #endregion

        #region Private Fields

        private List<double> yValues;

        private Storyboard sb;

        private Point hitPoint = new Point();

        private Point startPoint = new Point();

        private Point endPoint = new Point();

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for HistogramSeries
        /// </summary>
        public HistogramSeries()
        {
            yValues = new List<double>();
            ActualYValues = new List<double>();
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the property path bind with y axis.
        /// </summary>
        public string YBindingPath
        {
            get { return (string)GetValue(YBindingPathProperty); }
            set { SetValue(YBindingPathProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that specifies the interval to combine in histogram axis. This is a bindable property.
        /// </summary>
        public double HistogramInterval
        {
            get { return (double)GetValue(HistogramIntervalProperty); }
            set { SetValue(HistogramIntervalProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable the distribution curve in the histogram series.
        /// </summary>
        public bool ShowNormalDistributionCurve
        {
            get { return (bool)GetValue(ShowNormalDistributionCurveProperty); }
            set { SetValue(ShowNormalDistributionCurveProperty, value); }
        }

        /// <summary>
        /// Gets or sets the x axis for the Histogram series.
        /// </summary>
        public ChartAxisBase2D XAxis
        {
            get { return (ChartAxisBase2D)GetValue(XAxisProperty); }
            set { SetValue(XAxisProperty, value); }
        }

        /// <summary>
        /// Gets or sets the y axis for the Histogram series.
        /// </summary>
        public RangeAxisBase YAxis
        {
            get { return (RangeAxisBase)GetValue(YAxisProperty); }
            set { SetValue(YAxisProperty, value); }
        }

        ChartAxis ISupportAxes.ActualXAxis
        {
            get { return ActualXAxis; }
        }

        ChartAxis ISupportAxes.ActualYAxis
        {
            get { return ActualYAxis; }
        }
        
        /// <summary>
        /// Gets or sets the x axis range.
        /// </summary>
        public DoubleRange XRange { get; internal set; }

        /// <summary>
        /// Gets or sets the y axis range.
        /// </summary>
        public DoubleRange YRange { get; internal set; }

        #endregion

        #region Internal Properies

        internal List<double> ActualYValues
        {
            get;
            set;
        }

        private Brush curveColor;
        internal Brush CurveColor
        {
            get
            {
                return curveColor;
            }
            set
            {
                curveColor = value;
                OnCurveColorPropertyChanged(this);
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Creates the segments of HistogramSeries.
        /// </summary>
        public override void CreateSegments()
        {
            List<double> xValues = GetXValues();
            int index = 0;
            double x1, y1, y2;
            float median;

            double origin = ActualXAxis != null ? ActualXAxis.Origin : 0;

            if (ActualXAxis != null && ActualXAxis.Origin == 0 && ActualYAxis is LogarithmicAxis &&
                (ActualYAxis as LogarithmicAxis).Minimum != null)
                origin = (double)(ActualYAxis as LogarithmicAxis).Minimum;

            if (xValues != null)
            {
                Segments.Clear();
                ClearUnUsedAdornments(Segments.Count);
                ActualYValues.Clear();

                List<Point> dataPoints = new List<Point>();

                List<ChartSegment> segmentsCollection = new List<ChartSegment>();

                for (int i = 0; i < DataCount; i++)
                {
                    dataPoints.Add(new Point(xValues[i], yValues[i]));
                }

                Array.Sort(dataPoints.ToArray(), new PointsSortByXComparer());

                double interval = HistogramInterval;
                double start = 0;

                if (dataPoints.Count > 0)
                {
                    start = ChartMath.Round(dataPoints[0].X, interval, false);
                }

                int pointsCount;
                double position = start;
                int intervalsCount = 0;
                List<Point> points = new List<Point>();
                List<object> data = new List<object>();
                HistogramSegment histogramSegment;

                for (int i = 0, ci = dataPoints.Count; i < ci; i++)
                {
                    Point cdpt = dataPoints[i];

                    while (cdpt.X > position + interval)
                    {
                        pointsCount = points.Count;
                        if (pointsCount > 0)
                        {
                            histogramSegment = new HistogramSegment(position, pointsCount, position + interval, 0, this);
                            histogramSegment.XData = index;
                            histogramSegment.YData = pointsCount;
                            histogramSegment.Data = data;
                            data = new List<object>();
                            segmentsCollection.Add(histogramSegment);
                            ActualYValues.Add(pointsCount);
                            points.Clear();
                            index++;
                        }

                        position += interval;
                        intervalsCount++;
                    }

                    points.Add(dataPoints[i]);
                    data.Add(ActualData[i]);
                }

                pointsCount = points.Count;
                if (pointsCount > 0)
                {
                    intervalsCount++;
                    histogramSegment = new HistogramSegment(position, pointsCount, position + interval, 0, this);
                    histogramSegment.XData = index;
                    histogramSegment.YData = pointsCount;
                    histogramSegment.Data = data;
                    data = new List<object>();
                    segmentsCollection.Add(histogramSegment);
                    ActualYValues.Add(pointsCount);
                    points.Clear();
                }

                median = (float)interval / 2;

                if (AdornmentsInfo != null)
                {
                    for (int i = 0; i < segmentsCollection.Count; i++)
                    {
                        x1 = (segmentsCollection[i] as HistogramSegment).XRange.Start;
                        y1 = (segmentsCollection[i] as HistogramSegment).YData;
                        y2 = origin;

                        if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                            AddColumnAdornments(
                                (segmentsCollection[i] as HistogramSegment).XData,
                                (segmentsCollection[i] as HistogramSegment).YData,
                                x1, 
                                y1,
                                i,
                                median);
                        else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                            AddColumnAdornments(
                                (segmentsCollection[i] as HistogramSegment).XData,
                                (segmentsCollection[i] as HistogramSegment).YData,
                                x1,
                                y2,
                                i,
                                median);
                        else
                            AddColumnAdornments(
                                (segmentsCollection[i] as HistogramSegment).XData,
                                (segmentsCollection[i] as HistogramSegment).YData,
                                x1,
                                y1 + (y2 - y1) / 2, 
                                i,
                                median);
                    }
                }

                #region Normal Distribution

                if (ShowNormalDistributionCurve)
                {
                    double m, dev;
                    GetHistogramMeanAndDeviation(dataPoints, out m, out dev);

                    PointCollection distributionPoints = new PointCollection();

                    double min = start;
                    double max = start + intervalsCount * interval;
                    double del = (max - min) / (C_distributionPointsCount - 1);

                    for (int i = 0; i < C_distributionPointsCount; i++)
                    {
                        double tx = min + i * del;
                        double ty = NormalDistribution(tx, m, dev) * dataPoints.Count * interval;
                        distributionPoints.Add(new Point(tx, ty));
                    }

                    segmentsCollection.Add(new HistogramDistributionSegment(distributionPoints, this));
                }

                foreach (var item in segmentsCollection)
                {
                    Segments.Add(item);
                }

                #endregion
            }
        }

        /// <summary>
        /// Updates the segment at the specified index
        /// </summary>
        /// <param name="index">The index of the segment.</param>
        /// <param name="action">The action that caused the segments collection changed event</param>
        public override void UpdateSegments(int index, NotifyCollectionChangedAction action)
        {
            Area.ScheduleUpdate();
        }

        #endregion

        #region Internal Override Methods

        internal override void ResetAdornmentAnimationState()
        {
            if (adornmentInfo != null)
            {
                foreach (FrameworkElement child in this.AdornmentPresenter.Children)
                {
                    child.ClearValue(FrameworkElement.RenderTransformProperty);
                    child.ClearValue(FrameworkElement.OpacityProperty);
                }
            }
        }

        internal override bool GetAnimationIsActive()
        {
            return sb != null && sb.GetCurrentState() == ClockState.Active;
        }

        internal override void Animate()
        {
            // WPF-25124 Animation not working properly when resize the window.
            if (sb != null)
                sb.Stop();

            if (!canAnimate)
            {
                ResetAdornmentAnimationState();
                return;
            }

            sb = new Storyboard();

            if (this.AdornmentsInfo != null)
            {
                foreach (UIElement child in this.AdornmentPresenter.Children)
                {
                    OpacityAnimation(child);
                }
            }

            string path = "(UIElement.RenderTransform).(ScaleTransform.ScaleY)";

            foreach (ChartSegment segment in Segments)
            {
                var element = (FrameworkElement)segment.GetRenderedVisual();
                if (element == null) return;

                if (!(segment is HistogramDistributionSegment))
                {
                    element.RenderTransform = new ScaleTransform();
                    element.RenderTransformOrigin = new Point(1, 1);

                    DoubleAnimationUsingKeyFrames keyFrames1 = new DoubleAnimationUsingKeyFrames();
                    SplineDoubleKeyFrame keyFrame1 = new SplineDoubleKeyFrame();

                    keyFrame1.KeyTime = TimeSpan.FromSeconds(0);
                    keyFrame1.Value = 0;
                    keyFrames1.KeyFrames.Add(keyFrame1);

                    keyFrame1 = new SplineDoubleKeyFrame();
                    keyFrame1.KeyTime = TimeSpan.FromSeconds(AnimationDuration.TotalSeconds);
                    keyFrame1.Value = 1;
                    keyFrames1.KeyFrames.Add(keyFrame1);

                    KeySpline keySpline1 = new KeySpline();
                    keySpline1.ControlPoint1 = new Point(0.64, 0.84);
#if WINDOWS_UAP
                    keySpline1.ControlPoint2 = new Point(0, 1); // Animation have to provide same easing effect in all platforms.
#else
                    keySpline1.ControlPoint2 = new Point(0.67, 0.95);
#endif
                    keyFrame1.KeySpline = keySpline1;
                    keyFrames1.EnableDependentAnimation = true;
                    Storyboard.SetTargetProperty(keyFrames1, path);
                    Storyboard.SetTarget(keyFrames1, element);
                    sb.Children.Add(keyFrames1);
                }
                else
                {
                    OpacityAnimation(element);
                }
            }

            sb.Begin();
        }

        internal override void UpdateRange()
        {
            XRange = DoubleRange.Empty;
            YRange = DoubleRange.Empty;

            foreach (ChartSegment segment in Segments.Where(item => item is HistogramSegment))
            {
                XRange += segment.XRange;
                YRange += segment.YRange;
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

        #region Protected Internal Override Methods

        /// <summary>
        /// method declaration for generatepoints in Chartseries
        /// </summary>
        protected internal override void GeneratePoints()
        {
            GeneratePoints(new string[] { YBindingPath }, yValues);
        }

        #endregion

        #region Protected Virtual Methods

        /// <summary>
        /// Called when YAxis property changed 
        /// </summary>
        /// <param name="oldAxis"></param>
        /// <param name="newAxis"></param>
        protected virtual void OnYAxisChanged(ChartAxis oldAxis, ChartAxis newAxis)
        {
            if (oldAxis != null && oldAxis.RegisteredSeries != null)
            {
                if (oldAxis.RegisteredSeries.Contains(this))
                {
                    oldAxis.RegisteredSeries.Remove(this);
                }

                if (Area != null && oldAxis.RegisteredSeries.Count == 0)
                {
                    if (Area.Axes.Contains(oldAxis) && Area.InternalPrimaryAxis != oldAxis && Area.InternalSecondaryAxis != oldAxis)
                    {
                        Area.Axes.RemoveItem(oldAxis, Area.DependentSeriesAxes.Contains(oldAxis));
                        Area.DependentSeriesAxes.Remove(oldAxis);
                    }
                }
            }
            else if (ActualArea != null && ActualArea.InternalSecondaryAxis != null
                    && ActualArea.InternalSecondaryAxis.RegisteredSeries.Contains(this))
                ActualArea.InternalSecondaryAxis.RegisteredSeries.Remove(this);

            if (newAxis != null)
            {
                if (Area != null && !Area.Axes.Contains(newAxis))
                {
                    Area.Axes.Add(newAxis);
                    Area.DependentSeriesAxes.Add(newAxis);
                }
                newAxis.Area = Area;
                newAxis.Orientation = Orientation.Vertical;
                if (!newAxis.RegisteredSeries.Contains(this))
                    newAxis.RegisteredSeries.Add(this);
            }

            if (Area != null) Area.ScheduleUpdate();
        }

        /// <summary>
        /// Called when XAxis value changed
        /// </summary>
        /// <param name="oldAxis"></param>
        /// <param name="newAxis"></param>
        protected virtual void OnXAxisChanged(ChartAxis oldAxis, ChartAxis newAxis)
        {
            if (oldAxis != null && oldAxis.RegisteredSeries != null)
            {
                if (oldAxis.RegisteredSeries.Contains(this))
                    oldAxis.RegisteredSeries.Remove(this);

                if (Area != null && oldAxis.RegisteredSeries.Count == 0)
                {
                    if (Area.Axes.Contains(oldAxis) && Area.InternalPrimaryAxis != oldAxis && Area.InternalSecondaryAxis != oldAxis)
                    {
                        Area.Axes.RemoveItem(oldAxis, Area.DependentSeriesAxes.Contains(oldAxis));
                        Area.DependentSeriesAxes.Remove(oldAxis);
                    }
                }
            }
            else if (ActualArea != null && ActualArea.InternalPrimaryAxis != null
                     && ActualArea.InternalPrimaryAxis.RegisteredSeries.Contains(this))
                ActualArea.InternalPrimaryAxis.RegisteredSeries.Remove(this);

            if (newAxis != null)
            {
                if (Area != null && !Area.Axes.Contains(newAxis))
                {
                    Area.Axes.Add(newAxis);
                    Area.DependentSeriesAxes.Add(newAxis);
                }
                newAxis.Area = Area;
                newAxis.Orientation = Orientation.Horizontal;
                if (!newAxis.RegisteredSeries.Contains(this))
                    newAxis.RegisteredSeries.Add(this);
            }

            if (Area != null) Area.ScheduleUpdate();
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
            yValues.Clear();
            GeneratePoints(new string[] { YBindingPath }, yValues);
            this.UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            yValues.Clear();
            base.OnBindingPathChanged(args);
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            if (ShowTooltip && e.OriginalSource is Polyline)
            {
                Canvas canvas = ActualArea.GetAdorningCanvas();
                mousePos = e.GetCurrentPoint(canvas).Position;
                ChartDataPointInfo dataPointInfo = GetCurveDataPoint(mousePos, (e.OriginalSource as Polyline).Tag);
                UpdateSeriesTooltip(dataPointInfo);
            }
            else
                base.OnPointerMoved(e);
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new HistogramSeries()
            {
                XAxis = this.XAxis,
                YAxis = this.YAxis,
                HistogramInterval = this.HistogramInterval,
                ShowNormalDistributionCurve = this.ShowNormalDistributionCurve,
                YBindingPath = this.YBindingPath,
            });
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Gets the histogram mean and deviation.
        /// </summary>
        /// <param name="points">The cpwi A.</param>
        /// <param name="mean">The mean value.</param>
        /// <param name="standartDeviation">The standart deviation.</param>
        private static void GetHistogramMeanAndDeviation(List<Point> points, out double mean, out double standartDeviation)
        {
            int count = points.Count;
            double sum = 0;

            for (int i = 0; i < count; i++)
            {
                sum += points[i].X;
            }

            mean = sum / count;

            sum = 0;

            for (int i = 0; i < count; i++)
            {
                double dif = points[i].X - mean;
                sum += dif * dif;
            }

            standartDeviation = Math.Sqrt(sum / count);
        }

        /// <summary>
        /// Normal Distribution function.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="m">The m value.</param>
        /// <param name="sigma">The sigma value.</param>
        /// <returns>The Normal Distribution</returns>
        private static double NormalDistribution(double x, double m, double sigma)
        {
            return Math.Exp(-(x - m) * (x - m) / (2 * sigma * sigma)) / (sigma * c_sqrtDoublePI);
        }

        private static void OnYPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HistogramSeries).OnBindingPathChanged(e);
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HistogramSeries series = d as HistogramSeries;

            if (series.Area != null)
                series.Area.ScheduleUpdate();
        }

        private static void OnCurveColorPropertyChanged(DependencyObject d)
        {
            HistogramSeries series = d as HistogramSeries;

            if (series.Area != null)
                series.Area.ScheduleUpdate();
        }

        private static void OnYAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HistogramSeries).OnYAxisChanged(e.OldValue as ChartAxis, e.NewValue as ChartAxis);
        }

        private static void OnXAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HistogramSeries).OnXAxisChanged(e.OldValue as ChartAxis, e.NewValue as ChartAxis);
        }

        #endregion

        #region Private Methods
        
        private void OpacityAnimation(UIElement child)
        {
            DoubleAnimationUsingKeyFrames keyFrames1 = new DoubleAnimationUsingKeyFrames();
            SplineDoubleKeyFrame frame1 = new SplineDoubleKeyFrame();

            frame1.KeyTime = TimeSpan.FromSeconds(0);
            frame1.Value = 0;
            keyFrames1.KeyFrames.Add(frame1);

            frame1 = new SplineDoubleKeyFrame();
            frame1.KeyTime = AnimationDuration;
            frame1.Value = 0;
            keyFrames1.KeyFrames.Add(frame1);

            frame1 = new SplineDoubleKeyFrame();
            frame1.KeyTime = TimeSpan.FromSeconds(AnimationDuration.TotalSeconds + 1);
            frame1.Value = 1;
            keyFrames1.KeyFrames.Add(frame1);

            KeySpline keySpline = new KeySpline();
            keySpline.ControlPoint1 = new Point(0.64, 0.84);

            keySpline.ControlPoint2 = new Point(0, 1); // Animation have to provide same easing effect in all platforms.
            keyFrames1.EnableDependentAnimation = true;
            Storyboard.SetTargetProperty(keyFrames1, "(Opacity)");
            frame1.KeySpline = keySpline;

            Storyboard.SetTarget(keyFrames1, child);
            sb.Children.Add(keyFrames1);
        }

        private ChartDataPointInfo GetCurveDataPoint(Point mousePos, object tag)
        {
            hitPoint.X = mousePos.X - this.Area.SeriesClipRect.Left;
            hitPoint.Y = mousePos.Y - this.Area.SeriesClipRect.Top;

            // Calculate the bounds region from the area width  & height
            double x = Math.Floor(Area.SeriesClipRect.Width * 0.025);
            double y = Math.Floor(Area.SeriesClipRect.Height * 0.025);

            hitPoint.X = hitPoint.X - x;
            hitPoint.Y = hitPoint.Y - y;

            startPoint.X = ActualArea.PointToValue(ActualXAxis, hitPoint);
            startPoint.Y = ActualArea.PointToValue(ActualYAxis, hitPoint);

            hitPoint.X = hitPoint.X + (2 * x);
            hitPoint.Y = hitPoint.Y + (2 * y);

            endPoint.X = ActualArea.PointToValue(ActualXAxis, hitPoint);
            endPoint.Y = ActualArea.PointToValue(ActualYAxis, hitPoint);

            Rect rect = new Rect(startPoint, endPoint);

            dataPoint = null;
            PointCollection distributionPoints = (tag as HistogramDistributionSegment).distributionPoints;

            for (int i = 0; i < distributionPoints.Count; i++)
            {
                hitPoint.X = distributionPoints[i].X;
                hitPoint.Y = distributionPoints[i].Y;
                if (rect.Contains(hitPoint))
                {
                    dataPoint = new ChartDataPointInfo();
                    dataPoint.Index = i;
                    dataPoint.XData = distributionPoints[i].X;
                    dataPoint.YData = distributionPoints[i].Y;
                    dataPoint.Series = this;
                    if (i > -1 && ActualData.Count > i)
                        dataPoint.Item = ActualData[i];

                    break;
                }
            }

            return dataPoint;
        }

        #endregion

        #endregion
    }
}
