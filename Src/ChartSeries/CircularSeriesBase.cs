using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Animation;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Serves as a base class for pie and doughnut series. This type of chart is divided into slices to illustrate numerical proportions.
    /// </summary>
    public abstract partial class CircularSeriesBase : AccumulationSeriesBase
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="EnableSmartLabels"/> property. 
        /// </summary>
        public static readonly DependencyProperty EnableSmartLabelsProperty =
            DependencyProperty.Register("EnableSmartLabels", typeof(bool), typeof(CircularSeriesBase),
            new PropertyMetadata(false, OnAdornmentPorpertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ShowMarkerAtLineEnd"/> property. 
        /// </summary>
        public static readonly DependencyProperty ShowMarkerAtLineEndProperty =
            DependencyProperty.Register("ShowMarkerAtLineEnd", typeof(bool), typeof(CircularSeriesBase),
            new PropertyMetadata(false, OnAdornmentPorpertyChanged));


        /// <summary>
        ///  The DependencyProperty for <see cref="ConnectorType"/> property. 
        /// </summary>
        public static readonly DependencyProperty ConnectorTypeProperty =
            DependencyProperty.Register("ConnectorType", typeof(ConnectorMode), typeof(CircularSeriesBase),
            new PropertyMetadata(ConnectorMode.Line, OnAdornmentPorpertyChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="ConnectorPosition"/> property. 
        /// </summary>
        public static readonly DependencyProperty ConnectorLinePositionProperty =
            DependencyProperty.Register("ConnectorLinePosition", typeof(ConnectorLinePosition), typeof(CircularSeriesBase),
            new PropertyMetadata(ConnectorLinePosition.Center, OnAdornmentPorpertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="LabelPosition"/> property. 
        /// </summary>
        public static readonly DependencyProperty LabelPositionProperty =
            DependencyProperty.Register("LabelPosition", typeof(CircularSeriesLabelPosition), typeof(CircularSeriesBase),
                new PropertyMetadata(CircularSeriesLabelPosition.Inside, OnAdornmentPorpertyChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="StartAngle"/> property. 
        /// </summary>
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(CircularSeriesBase),
                                        new PropertyMetadata(0d, OnStartAngleChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="EndAngle"/> property. 
        /// </summary>
        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(CircularSeriesBase),
            new PropertyMetadata(360d, OnStartAngleChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ExplodeRadius"/> property.
        /// </summary>
        public static readonly DependencyProperty ExplodeRadiusProperty =
            DependencyProperty.Register("ExplodeRadius", typeof(double), typeof(CircularSeriesBase),
            new PropertyMetadata(30d, OnExplodeRadiusChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="GroupMode"/> property.
        /// </summary>
        public static readonly DependencyProperty GroupModeProperty =
            DependencyProperty.Register("GroupMode", typeof(PieGroupMode), typeof(CircularSeriesBase),
            new PropertyMetadata(PieGroupMode.Value, OnGroupToPropertiesChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="GroupTo"/> property.
        /// </summary>
        public static readonly DependencyProperty GroupToProperty =
            DependencyProperty.Register("GroupTo", typeof(double), typeof(CircularSeriesBase),
            new PropertyMetadata(double.NaN, OnGroupToPropertiesChanged));

        #endregion

        #region Fields

        #region Constants

        internal const double TotalArcLength = Math.PI * 2;

        #endregion

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to enable the smart adornment labels, which will place the around series without overlapping.
        /// </summary>
        public bool EnableSmartLabels
        {
            get { return (bool)GetValue(EnableSmartLabelsProperty); }
            set { SetValue(EnableSmartLabelsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the marker placed in start position or at line end position.
        /// </summary>
        public bool ShowMarkerAtLineEnd
        {
            get { return (bool)GetValue(ShowMarkerAtLineEndProperty); }
            set { SetValue(ShowMarkerAtLineEndProperty, value); }
        }

        /// <summary>
        /// Gets or sets the type of connector line to be drawn.
        /// </summary>
        /// <value>
        ///     <see cref="Syncfusion.UI.Xaml.Charts.ConnectorMode"/>.
        /// </value>
        public ConnectorMode ConnectorType
        {
            get { return (ConnectorMode)GetValue(ConnectorTypeProperty); }
            set { SetValue(ConnectorTypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to render the straight connector line in auto available space.
        /// </summary>
        /// <remarks>
        /// Provides better alignment to the straight connector lines with outside extended label position for minimum number of data points.
        /// </remarks>
        /// <value>
        ///     <see cref="Syncfusion.UI.Xaml.Charts.ConnectorLinePosition"/>.
        /// </value>
        public ConnectorLinePosition ConnectorLinePosition
        {
            get { return (ConnectorLinePosition )GetValue(ConnectorLinePositionProperty); }
            set { SetValue(ConnectorLinePositionProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the adornment label position inside, outside or outside extend.
        /// </summary>
        /// <value>
        ///     <see cref="Syncfusion.UI.Xaml.Charts.CircularSeriesLabelPosition"/>.
        /// </value>
        public CircularSeriesLabelPosition LabelPosition
        {
            get { return (CircularSeriesLabelPosition)GetValue(LabelPositionProperty); }
            set { SetValue(LabelPositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that specifies the start angle for the circular series. This is a bindable property.
        /// </summary>
        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that specifies the end angle for the circular series. This is a bindable property. 
        /// </summary>
        public double EndAngle
        {
            get { return (double)GetValue(EndAngleProperty); }
            set { SetValue(EndAngleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the radial distance for the exploded segment from center.
        /// </summary>
        public double ExplodeRadius
        {
            get { return (double)GetValue(ExplodeRadiusProperty); }
            set { SetValue(ExplodeRadiusProperty, value); }
        }

        /// <summary>
        /// Gets or sets the group mode, which indicates the series segments grouping. This is a bindable property.
        /// </summary>
        public PieGroupMode GroupMode
        {
            get { return (PieGroupMode)GetValue(GroupModeProperty); }
            set { SetValue(GroupModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the double value, which indicates series segments grouping. This is a bindable property.
        /// </summary>
        public double GroupTo
        {
            get { return (double)GetValue(GroupToProperty); }
            set { SetValue(GroupToProperty, value); }
        }

        internal List<object> GroupedData
        {
            get { return groupedData; }
            set
            {
                groupedData = value;
            }
        }


        #endregion

        #region Internal Properties

        internal double Radius { get; set; }

        internal Point Center { get; set; }

        internal string GroupingLabel
        {
            get { return groupingLabel; }
            set { groupingLabel = value; }
        }

        #endregion
        private string groupingLabel = SfChartResourceWrapper.Others;

        private List<object> groupedData;

        #endregion

        #region Methods

        #region Internal Override Methods

        internal override void ResetAdornmentAnimationState()
        {
            if (adornmentInfo != null)
            {
                foreach (var child in this.AdornmentPresenter.Children)
                {
                    (child as FrameworkElement).ClearValue(FrameworkElement.OpacityProperty);
                }
            }
        }

        #endregion

        #region Internal Methods

        internal void AnimateAdornments(Storyboard sb)
        {
            if (this.AdornmentsInfo != null)
            {
                double totalDuration = AnimationDuration.TotalSeconds;
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
            }
        }

        internal Point GetActualCenter(Point centerPoint, double radius)
        {
            if (Area != null && Area.Series.IndexOf(this) > 0) return centerPoint;
            Point actualCenter = centerPoint;
            double startAngle = StartAngle;
            double endAngle = EndAngle;

            // WPF-29938 PieSeries is not getting aligned properly. The array is generated according to the start angle and end angle.
            var arraySize = ((Math.Max(Math.Abs((int)startAngle / 90), Math.Abs((int)endAngle / 90)) + 1) * 2) + 1;
            double[] regions = new double[(arraySize)];

            int arrayIndex = 0;
            for (int i = -(arraySize / 2); i < arraySize / 2 + 1; i++)
            {
                regions[arrayIndex] = i * 90;
                arrayIndex++;
            }

            List<int> region = new List<int>();
            if (startAngle < endAngle)
            {
                for (int i = 0; i < regions.Count(); i++)
                {
                    if (regions[i] > startAngle && regions[i] < endAngle)
                        region.Add((int)((regions[i] % 360) < 0 ? (regions[i] % 360) + 360 : (regions[i] % 360)));
                }
            }
            else
            {
                for (int i = 0; i < regions.Count(); i++)
                {
                    if (regions[i] < startAngle && regions[i] > endAngle)
                        region.Add((int)((regions[i] % 360) < 0 ? (regions[i] % 360) + 360 : (regions[i] % 360)));
                }
            }

            var startRadian = 2 * Math.PI * (startAngle) / 360;
            var endRadian = 2 * Math.PI * (endAngle) / 360;
            Point startPoint = new Point(centerPoint.X + radius * Math.Cos(startRadian), centerPoint.Y + radius * Math.Sin(startRadian));
            Point endPoint = new Point(centerPoint.X + radius * Math.Cos(endRadian), centerPoint.Y + radius * Math.Sin(endRadian));

            switch (region.Count)
            {
                case 0:
                    var longX = Math.Abs(centerPoint.X - startPoint.X) > Math.Abs(centerPoint.X - endPoint.X) ? startPoint.X : endPoint.X;
                    var longY = Math.Abs(centerPoint.Y - startPoint.Y) > Math.Abs(centerPoint.Y - endPoint.Y) ? startPoint.Y : endPoint.Y;
                    var midPoint = new Point(Math.Abs((centerPoint.X + longX)) / 2, Math.Abs((centerPoint.Y + longY)) / 2);
                    actualCenter.X = centerPoint.X + (centerPoint.X - midPoint.X);
                    actualCenter.Y = centerPoint.Y + (centerPoint.Y - midPoint.Y);
                    break;

                case 1:
                    Point point1 = new Point(), point2 = new Point();
                    var maxRadian = 2 * Math.PI * region[0] / 360;
                    var maxPoint = new Point(centerPoint.X + radius * Math.Cos(maxRadian), centerPoint.Y + radius * Math.Sin(maxRadian));
                    switch (region.ElementAt(0))
                    {
                        case 270:
                            point1 = new Point(startPoint.X, maxPoint.Y);
                            point2 = new Point(endPoint.X, centerPoint.Y);
                            break;
                        case 0:
                        case 360:
                            point1 = new Point(centerPoint.X, endPoint.Y);
                            point2 = new Point(maxPoint.X, startPoint.Y);
                            break;
                        case 90:
                            point1 = new Point(endPoint.X, centerPoint.Y);
                            point2 = new Point(startPoint.X, maxPoint.Y);
                            break;
                        case 180:
                            point1 = new Point(maxPoint.X, startPoint.Y);
                            point2 = new Point(centerPoint.X, endPoint.Y);
                            break;
                    }

                    midPoint = new Point((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2);
                    actualCenter.X = centerPoint.X + ((centerPoint.X - midPoint.X) >= radius ? 0 : (centerPoint.X - midPoint.X));
                    actualCenter.Y = centerPoint.Y + ((centerPoint.Y - midPoint.Y) >= radius ? 0 : (centerPoint.Y - midPoint.Y));
                    break;

                case 2:
                    var minRadian = 2 * Math.PI * region[0] / 360;
                    maxRadian = 2 * Math.PI * (region[1]) / 360;
                    maxPoint = new Point(centerPoint.X + radius * Math.Cos(maxRadian), centerPoint.Y + radius * Math.Sin(maxRadian));
                    Point minPoint = new Point(centerPoint.X + radius * Math.Cos(minRadian), centerPoint.Y + radius * Math.Sin(minRadian));
                    if (region[0] == 0 && region[1] == 90 || region[0] == 180 && region[1] == 270)
                        point1 = new Point(minPoint.X, maxPoint.Y);
                    else
                        point1 = new Point(maxPoint.X, minPoint.Y);
                    if (region[0] == 0 || region[0] == 180)
                        point2 = new Point(GetMinMaxValue(startPoint, endPoint, region[0]), GetMinMaxValue(startPoint, endPoint, region[1]));
                    else
                        point2 = new Point(GetMinMaxValue(startPoint, endPoint, region[1]), GetMinMaxValue(startPoint, endPoint, region[0]));
                    midPoint = new Point(Math.Abs(point1.X - point2.X) / 2 >= radius ? 0 : (point1.X + point2.X) / 2, y: Math.Abs(point1.Y - point2.Y) / 2 >= radius ? 0 : (point1.Y + point2.Y) / 2);
                    actualCenter.X = centerPoint.X + (midPoint.X == 0 ? 0 : (centerPoint.X - midPoint.X) >= radius ? 0 : (centerPoint.X - midPoint.X));
                    actualCenter.Y = centerPoint.Y + (midPoint.Y == 0 ? 0 : (centerPoint.Y - midPoint.Y) >= radius ? 0 : (centerPoint.Y - midPoint.Y));

                    break;
            }

            return actualCenter;
        }

        internal Tuple<List<double>, List<object>> GetGroupToYValues()
        {
            List<double> yValues = new List<double>();
            List<object> actualData = new List<object>();
            GroupedData = new List<object>();
            double lessThanGroupTo = 0;
            var sumOfYValues = (from val in YValues
                                select (val) > 0 ? val : Math.Abs(double.IsNaN(val) ? 0 : val)).Sum();

            for (int i = 0; i < DataCount; i++)
            {
                double yValue = YValues[i];

                if (GetGroupModeValue(yValue, sumOfYValues) > GroupTo)
                {
                    yValues.Add(yValue);
                    actualData.Add(ActualData[i]);
                }
                else if (!double.IsNaN(yValue))
                {
                    lessThanGroupTo += yValue;
                    GroupedData.Add(ActualData[i]);
                }

                if (i == DataCount - 1 && GroupedData.Count > 0)
                {
                    yValues.Add(lessThanGroupTo);
                    actualData.Add(GroupedData);
                }

            }

            return new Tuple<List<double>, List<object>>(yValues, actualData);
        }

        internal double GetGroupModeValue(double yValue, double sumOfYValues)
        {
            double value = 0;

            switch (GroupMode)
            {
                case PieGroupMode.Value:
                    value = yValue;
                    break;
                case PieGroupMode.Percentage:
                    float percentage = (float)(yValue / sumOfYValues * 100);
                    percentage = (float)Math.Floor(percentage * 100) / 100;
                    value = percentage;
                    break;
                case PieGroupMode.Angle:
                    var arcStartAngle = DegreeToRadianConverter(StartAngle);
                    var arcEndAngle = DegreeToRadianConverter(EndAngle);
                    var ARCLENGTH = arcEndAngle - arcStartAngle;

                    if (Math.Abs(Math.Round(ARCLENGTH, 2)) > TotalArcLength)
                        ARCLENGTH = ARCLENGTH % TotalArcLength;

                    value = (Math.Abs(double.IsNaN(yValue) ? 0 : yValue) * (ARCLENGTH / sumOfYValues));
                    break;
            }

            return value;
        }

        internal List<double> GetToggleYValues(List<double> groupToYValues)
        {
            var yvalues = new List<double>();

            for (int i = 0; i < groupToYValues.Count; i++)
            {
                double yvalue = groupToYValues[i];

                if (!ToggledLegendIndex.Contains(i))
                {
                    yvalues.Add(yvalue);
                }
                else
                    yvalues.Add(double.NaN);
            }

            return yvalues;
        }

        #endregion

        #region Protected Override Methods

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            var circularSeriesBase = obj as CircularSeriesBase;
            circularSeriesBase.LabelPosition = this.LabelPosition;
            circularSeriesBase.EnableSmartLabels = this.EnableSmartLabels;
            circularSeriesBase.ConnectorType = this.ConnectorType;
            circularSeriesBase.StartAngle = this.StartAngle;
            circularSeriesBase.EndAngle = this.EndAngle;
            circularSeriesBase.ExplodeRadius = this.ExplodeRadius;
            circularSeriesBase.GroupMode = this.GroupMode;
            circularSeriesBase.GroupTo = this.GroupTo;
            circularSeriesBase.GroupedData = this.GroupedData;

            return base.CloneSeries(circularSeriesBase);
        }

        #endregion

        #region Protected Methods

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        protected internal double DegreeToRadianConverter(double degree)
        {
            return degree * Math.PI / 180;
        }

        #endregion

        #region Private Static Methods

        private static void OnStartAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularSeriesBase series = d as CircularSeriesBase;
            if (series != null)
            {
                if (!double.IsNaN(series.GroupTo))
                {
                    OnGroupToPropertiesChanged(d, e);
                }
                else
                    series.UpdateArea();
            }
        }

        private static void OnExplodeRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var circularSeriesBase = d as CircularSeriesBase;
            if (circularSeriesBase != null) circularSeriesBase.SetExplodeRadius();
        }

        private static double GetMinMaxValue(Point point1, Point point2, int degree)
        {
            var minX = Math.Min(point1.X, point2.Y);
            var minY = Math.Min(point1.Y, point2.Y);
            var maxX = Math.Max(point1.X, point2.X);
            var maxY = Math.Max(point1.Y, point2.Y);
            switch (degree)
            {
                case 270:
                    return maxY;
                case 0:
                case 360:
                    return minX;
                case 90:
                    return minY;
                case 180:
                    return maxX;
            }

            return 0d;
        }

        private static void OnAdornmentPorpertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var series = d as CircularSeriesBase;
            if (series != null && series.adornmentInfo != null)
            {
                series.adornmentInfo.OnAdornmentPropertyChanged();
            }
        }
        
        private static void OnGroupToPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularSeriesBase series = d as CircularSeriesBase;
            if (series != null && series.ActualArea != null)
            {
                series.ActualArea.IsUpdateLegend = true;
                series.Segments.Clear();
                series.UpdateArea();
            }
        }


        #endregion

        #endregion
    }
}
