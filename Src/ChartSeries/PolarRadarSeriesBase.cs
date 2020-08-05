using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using System.Collections;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Animation;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for PolarRadarSeriesBase
    /// </summary>
    public abstract partial class PolarRadarSeriesBase : AdornmentSeries, ISupportAxes2D
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="YBindingPath"/> property.       
        /// </summary>
        public static readonly DependencyProperty YBindingPathProperty =
            DependencyProperty.Register(
                "YBindingPath", 
                typeof(string), 
                typeof(PolarRadarSeriesBase),
                new PropertyMetadata(null, OnYPathChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="IsClosed"/> property.       
        /// </summary>
        public static readonly DependencyProperty IsClosedProperty =
            DependencyProperty.Register(
                "IsClosed",
                typeof(bool), 
                typeof(PolarRadarSeriesBase),
                new PropertyMetadata(true, OnDrawValueChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="DrawType"/> property.       
        /// </summary>
        public static readonly DependencyProperty DrawTypeProperty =
            DependencyProperty.Register(
                "DrawType", 
                typeof(ChartSeriesDrawType),
                typeof(PolarRadarSeriesBase),
                new PropertyMetadata(ChartSeriesDrawType.Area, OnDrawValueChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="XAxis"/> property.       
        /// </summary>
        public static readonly DependencyProperty XAxisProperty =
            DependencyProperty.Register(
                "XAxis", 
                typeof(ChartAxisBase2D),
                typeof(PolarRadarSeriesBase),
                new PropertyMetadata(null, OnXAxisChanged));
                
        /// <summary>
        /// The DependencyProperty for <see cref="YAxis"/> property.       
        /// </summary>
        public static readonly DependencyProperty YAxisProperty =
            DependencyProperty.Register(
                "YAxis",
                typeof(RangeAxisBase),
                typeof(PolarRadarSeriesBase),
                new PropertyMetadata(null, OnYAxisChanged));

        /// <summary>
        /// The Dependency property for<see cref="StrokeDashArray"/>
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register(
                "StrokeDashArray",
                typeof(DoubleCollection),
                typeof(PolarRadarSeriesBase),
                new PropertyMetadata(null, OnDrawValueChanged));

        #endregion

        #region Fields

        #region Private Fields

        Storyboard sb;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for PolarRadarSeriesBase
        /// </summary>
        public PolarRadarSeriesBase()
        {
            YValues = new List<double>();
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
        /// Gets or sets a value that indicates whether area path should be closed or opened for Polar/Radar series. This is a bindable property.
        /// </summary>
        /// <value>
        ///  If its <c>true</c>, Area stroke will be closed; otherwise stroke will be applied on top of the series only.
        /// </value>
        public bool IsClosed
        {
            get { return (bool)GetValue(IsClosedProperty); }
            set { SetValue(IsClosedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the type of series to be drawn for Radar or Polar series. This is a bindable property. 
        /// </summary>
        /// <remarks>Either <c>Area</c> or <c>Line</c> can be drawn.
        /// </remarks>
        public ChartSeriesDrawType DrawType
        {
            get { return (ChartSeriesDrawType)GetValue(DrawTypeProperty); }
            set { SetValue(DrawTypeProperty, value); }
        }
                      
        /// <summary>
        /// Gets or sets XRange property
        /// </summary>
        public DoubleRange XRange { get; internal set; }

        /// <summary>
        /// Gets or sets YRange property
        /// </summary>
        public DoubleRange YRange { get; internal set; }

        /// <summary>
        /// Gets or sets the multiple axis is not applicable for Radar/polar series.
        /// </summary>
        public ChartAxisBase2D XAxis
        {
            get { return (ChartAxisBase2D)GetValue(XAxisProperty); }
            set { SetValue(XAxisProperty, value); }
        }

        /// <summary>
        /// Gets or sets the multiple axis is not applicable for Radar/polar series.
        /// </summary>
        public RangeAxisBase YAxis
        {
            get { return (RangeAxisBase)GetValue(YAxisProperty); }
            set { SetValue(YAxisProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke dash array for the line.
        /// </summary>
        public DoubleCollection StrokeDashArray
        {
            get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
            set { SetValue(StrokeDashArrayProperty, value); }
        }

        ChartAxis ISupportAxes.ActualXAxis
        {
            get { return ActualXAxis; }
        }

        ChartAxis ISupportAxes.ActualYAxis
        {
            get { return ActualYAxis; }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets or sets YValues property
        /// </summary>
        protected IList<double> YValues { get; set; }

        /// <summary>
        /// Gets or sets Segment property
        /// </summary>
        protected ChartSegment Segment { get; set; }

        #endregion

        #endregion

        #region Methods
                
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
            {
                sb.Stop();
                if (!canAnimate)
                {
                    ResetAdornmentAnimationState();
                    return;
                }
            }

            sb = new Storyboard();

            string propertyXPath = "(UIElement.RenderTransform).(ScaleTransform.ScaleX)";
            string propertyYPath = "(UIElement.RenderTransform).(ScaleTransform.ScaleY)";

            var panel = (ActualArea as SfChart).GridLinesPanel;
            Point center = new Point(panel.ActualWidth / 2, panel.ActualHeight / 2);

            if (this.DrawType == ChartSeriesDrawType.Area)
            {
                var segmentCanvas = this.Segment.GetRenderedVisual();
                var path = (segmentCanvas as Canvas).Children[0] as FrameworkElement;
                path.RenderTransform = new ScaleTransform() { CenterX = center.X, CenterY = center.Y };
                AnimateElement(path, propertyXPath, propertyYPath);
            }
            else
            {
                foreach (var segment in this.Segments)
                {
                    var lineSegment = segment.GetRenderedVisual();
                    lineSegment.RenderTransform = new ScaleTransform() { CenterX = center.X, CenterY = center.Y };
                    AnimateElement(lineSegment, propertyXPath, propertyYPath);
                }
            }

            AnimateAdornments();
            sb.Begin();
        }

        internal override void UpdateRange()
        {
            XRange = DoubleRange.Empty;
            YRange = DoubleRange.Empty;

            foreach (ChartSegment segment in Segments)
            {
                XRange += segment.XRange;
                YRange += segment.YRange;
            }
        }

        /// <summary>
        /// Validate the datapoints for segment implementation.
        /// </summary>
        internal override void ValidateYValues()
        {
            foreach (var yValue in YValues)
            {
                if (double.IsNaN(yValue) && ShowEmptyPoints)
                    ValidateDataPoints(YValues); break;
            }
        }

        /// <summary>
        /// Validate the datapoints for segment implementation.
        /// </summary>
        internal override void ReValidateYValues(List<int>[] emptyPointIndex)
        {
            foreach (var item in emptyPointIndex)
            {
                foreach (var index in item)
                    YValues[index] = double.NaN;
            }
        }

        internal override void RemoveTooltip()
        {
            var canvas = this.Area.GetAdorningCanvas();
            if (canvas.Children.Contains((this.Area.Tooltip as ChartTooltip)))
                canvas.Children.Remove(this.Area.Tooltip as ChartTooltip);
        }

        internal override int GetDataPointIndex(Point point)
        {
            Canvas canvas = Area.GetAdorningCanvas();
            double left = Area.ActualWidth - canvas.ActualWidth;
            double top = Area.ActualHeight - canvas.ActualHeight;

            point.X = point.X - left - Area.SeriesClipRect.Left + Area.Margin.Left;
            point.Y = point.Y - top - Area.SeriesClipRect.Top + Area.Margin.Top;
            double xVal = 0;
            double xStart = ActualXAxis.VisibleRange.Start;
            double xEnd = ActualXAxis.VisibleRange.End;
            int index = -1;
            double center = 0.5 * Math.Min(this.Area.SeriesClipRect.Width, this.Area.SeriesClipRect.Height);
            double radian = ChartTransform.PointToPolarRadian(point, center);
            double coeff = ChartTransform.RadianToPolarCoefficient(radian);
            xVal = Math.Round(this.Area.InternalPrimaryAxis.PolarCoefficientToValue(coeff));
            if (xVal <= xEnd && xVal >= xStart)
                index = this.GetXValues().IndexOf((int)xVal);
            return index;
        }

        internal override void UpdateTooltip(object originalSource)
        {
            if (ShowTooltip)
            {
                var shape = (originalSource as Shape);
                if (shape == null || (shape != null && shape.Tag == null))
                    return;
                SetTooltipDuration();
                var canvas = this.Area.GetAdorningCanvas();

                double xVal = 0;
                object data = null;
                double xStart = ActualXAxis.VisibleRange.Start;
                double xEnd = ActualXAxis.VisibleRange.End;
                int index = 0;

                if (this.Area.SeriesClipRect.Contains(mousePos))
                {
                    var point = new Point(
                        mousePos.X - this.Area.SeriesClipRect.Left,
                        mousePos.Y - this.Area.SeriesClipRect.Top);
                    double center = 0.5 * Math.Min(this.Area.SeriesClipRect.Width, this.Area.SeriesClipRect.Height);
                    double radian = ChartTransform.PointToPolarRadian(point, center);
                    double coeff = ChartTransform.RadianToPolarCoefficient(radian);
                    xVal = Math.Round(this.Area.InternalPrimaryAxis.PolarCoefficientToValue(coeff));
                    if (xVal <= xEnd && xVal >= xStart)
                        index = this.GetXValues().IndexOf((int)xVal);
                    data = this.ActualData[index];
                }

                var chartTooltip = this.Area.Tooltip as ChartTooltip;
                if (this.DrawType == ChartSeriesDrawType.Area)
                {
                    var areaSegment = shape.Tag as AreaSegment;
                    areaSegment.Item = data;
                    areaSegment.XData = xVal;
                    areaSegment.YData = this.YValues[(int)index];
                }
                else
                {
                    var lineSegment = shape.Tag as LineSegment;
                    lineSegment.Item = data;
                    lineSegment.YData = this.YValues[(int)index];
                }

                if (chartTooltip != null)
                {
                    var tag = shape.Tag;

                    if (canvas.Children.Count == 0 || (canvas.Children.Count > 0 && !IsTooltipAvailable(canvas)))
                    {
                        chartTooltip.Content = tag;
                        if (chartTooltip.Content == null)
                            return;

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

                        chartTooltip.Content = tag;
                        if (chartTooltip.Content == null)
                        {
                            RemoveTooltip();
                            return;
                        }

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
                                _stopwatch.Restart();

                                Canvas.SetLeft(chartTooltip, chartTooltip.LeftOffset);
                                Canvas.SetTop(chartTooltip, chartTooltip.TopOffset);
                            }
#if NETFX_CORE
                        }
#endif
                    }
                }
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
        /// Method implementation  for GeneratePoints for Adornments
        /// </summary>
        protected internal override void GeneratePoints()
        {
            GeneratePoints(new string[] { YBindingPath }, YValues);
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
            if (oldAxis != null)
            {
                if (oldAxis.RegisteredSeries.Contains(this))
                    oldAxis.RegisteredSeries.Remove(this);
                if (Area != null && oldAxis.RegisteredSeries.Count == 0)
                {
                    if (Area.Axes.Contains(oldAxis))
                    {
                        if (Area.InternalSecondaryAxis != null && Area.InternalSecondaryAxis == oldAxis)
                        {
                            Area.Axes.Remove(oldAxis);
                            if (Area.InternalSecondaryAxis.IsDefault)
                            {
                                Area.SecondaryAxis = null;
                                if (Area.InternalPrimaryAxis != null &&
                                    Area.InternalPrimaryAxis.AssociatedAxes.Contains(oldAxis))
                                    Area.InternalPrimaryAxis.AssociatedAxes.Remove(oldAxis);
                            }
                        }
                    }
                }
            }

            if (newAxis != null && !newAxis.RegisteredSeries.Contains(this))
            {
                newAxis.Area = Area;
                if (Area != null && !Area.Axes.Contains(newAxis))
                    Area.Axes.Add(newAxis);
                newAxis.Orientation = Orientation.Vertical;
                newAxis.RegisteredSeries.Add(this);
            }

            if (Area != null) Area.ScheduleUpdate();
        }

        /// <summary>
        /// Called when XAxis property changed
        /// </summary>
        /// <param name="oldAxis"></param>
        /// <param name="newAxis"></param>
        protected virtual void OnXAxisChanged(ChartAxis oldAxis, ChartAxis newAxis)
        {
            if (oldAxis != null)
            {
                if (oldAxis.RegisteredSeries.Contains(this))
                    oldAxis.RegisteredSeries.Remove(this);

                if (Area != null && oldAxis.RegisteredSeries.Count > 0)
                {
                    if (Area.Axes.Contains(oldAxis))
                    {
                        if (Area.InternalPrimaryAxis != null && Area.InternalPrimaryAxis == oldAxis)
                        {
                            Area.Axes.Remove(oldAxis);
                            if (Area.InternalPrimaryAxis.IsDefault)
                            {
                                Area.PrimaryAxis = null;
                                if (Area.InternalSecondaryAxis != null &&
                                    Area.InternalSecondaryAxis.AssociatedAxes.Contains(oldAxis))
                                    Area.InternalSecondaryAxis.AssociatedAxes.Remove(oldAxis);
                            }
                        }
                    }
                }
            }

            if (newAxis != null)
            {
                if (Area != null && !Area.Axes.Contains(newAxis))
                    Area.Axes.Add(newAxis);
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
            YValues.Clear();
            Segment = null;
            GeneratePoints(new string[] { YBindingPath }, YValues);
            isPointValidated = false;
            this.UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            YValues.Clear();
            Segment = null;
            base.OnBindingPathChanged(args);
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            var polarRadarSeriesBase = obj as PolarRadarSeriesBase;
            polarRadarSeriesBase.IsClosed = this.IsClosed;
            polarRadarSeriesBase.YBindingPath = this.YBindingPath;
            polarRadarSeriesBase.DrawType = this.DrawType;
            polarRadarSeriesBase.IsClosed = this.IsClosed;
            return base.CloneSeries(polarRadarSeriesBase);
        }

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

        #endregion

        #region Private Static Methods

        private static void OnYPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PolarRadarSeriesBase).OnBindingPathChanged(e);
        }

        private static void OnDrawValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PolarRadarSeriesBase).UpdateArea();
        }

        private static void OnYAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PolarRadarSeriesBase).OnYAxisChanged(e.OldValue as ChartAxis, e.NewValue as ChartAxis);
        }

        private static void OnXAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PolarRadarSeriesBase).OnXAxisChanged(e.OldValue as ChartAxis, e.NewValue as ChartAxis);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Timer Tick Handler for closing the Tooltip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Tick(object sender, object e)
        {
            RemoveTooltip();
            Timer.Stop();
        }

        private void AnimateAdornments()
        {
            if (this.AdornmentsInfo != null)
            {
                foreach (var child in this.AdornmentPresenter.Children)
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

                    Storyboard.SetTarget(keyFrames1, child as FrameworkElement);
                    sb.Children.Add(keyFrames1);
                }
            }
        }

        private void AnimateElement(UIElement element, string propertyXPath, string propertyYPath)
        {
            DoubleAnimation animation_X = new DoubleAnimation();
            animation_X.From = 0;
            animation_X.To = 1;
            animation_X.Duration = AnimationDuration;
            Storyboard.SetTarget(animation_X, element);
            Storyboard.SetTargetProperty(animation_X, propertyXPath);
            animation_X.EnableDependentAnimation = true;
            sb.Children.Add(animation_X);

            DoubleAnimation animation_Y = new DoubleAnimation();
            animation_Y.From = 0;
            animation_Y.To = 1;
            animation_Y.Duration = AnimationDuration;
            Storyboard.SetTarget(animation_Y, element);
            Storyboard.SetTargetProperty(animation_Y, propertyYPath);
            animation_Y.EnableDependentAnimation = true;
            sb.Children.Add(animation_Y);
        }

        #endregion

        #endregion     
    }
}
