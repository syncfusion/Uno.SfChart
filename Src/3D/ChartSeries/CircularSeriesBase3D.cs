// <copyright file="CircularSeriesBase3D.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Class implementation for CircularSeriesBase3D
    /// </summary>
    public abstract partial class CircularSeriesBase3D : ChartSeries3D
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="ConnectorType"/> property.
        /// </summary>
        public static readonly DependencyProperty ConnectorTypeProperty =
            DependencyProperty.Register(
                "ConnectorType",
                typeof(ConnectorMode), 
                typeof(CircularSeriesBase3D),
                new PropertyMetadata(ConnectorMode.Line, OnPropertyChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="EnableSmartLabels"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableSmartLabelsProperty =
            DependencyProperty.Register(
                "EnableSmartLabels", 
                typeof(bool), 
                typeof(CircularSeriesBase3D),
                new PropertyMetadata(false, OnPropertyChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="CircleCoefficient"/> property.
        /// </summary>
        public static readonly DependencyProperty CircleCoefficientProperty =
            DependencyProperty.Register(
                "CircleCoefficient", 
                typeof(double), 
                typeof(CircularSeriesBase3D),
                new PropertyMetadata(0.8d, OnCircleCoefficientChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="LabelPosition"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelPositionProperty =
            DependencyProperty.Register(
                "LabelPosition", 
                typeof(CircularSeriesLabelPosition), 
                typeof(CircularSeriesBase3D),
                new PropertyMetadata(CircularSeriesLabelPosition.Inside, OnPropertyChanged));
        
        /// <summary>
        ///  The DependencyProperty for <see cref="YBindingPath"/> property.
        /// </summary>
        public static readonly DependencyProperty YBindingPathProperty =
            DependencyProperty.Register(
                "YBindingPath",
                typeof(string), 
                typeof(CircularSeriesBase3D),
                new PropertyMetadata(null, OnYPathChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="ExplodeRadius"/> property.
        /// </summary>
        public static readonly DependencyProperty ExplodeRadiusProperty =
            DependencyProperty.Register(
                "ExplodeRadius", 
                typeof(double),
                typeof(CircularSeriesBase3D),
                new PropertyMetadata(30d, OnPropertyChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="ExplodeIndex"/> property.
        /// </summary>
        public static readonly DependencyProperty ExplodeIndexProperty =
            DependencyProperty.Register(
                "ExplodeIndex", 
                typeof(int), 
                typeof(CircularSeriesBase3D),
                new PropertyMetadata(-1, OnPropertyChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="ExplodeAll"/> property.
        /// </summary>
        public static readonly DependencyProperty ExplodeAllProperty =
            DependencyProperty.Register(
                "ExplodeAll", 
                typeof(bool),
                typeof(CircularSeriesBase3D),
                new PropertyMetadata(false, OnPropertyChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="StartAngle"/> property.
        /// </summary>
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register(
                "StartAngle", 
                typeof(double), 
                typeof(CircularSeriesBase3D),
                new PropertyMetadata(0d, OnAngleChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="EndAngle"/> property.
        /// </summary>
        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register(
                "EndAngle", 
                typeof(double), 
                typeof(CircularSeriesBase3D),
                new PropertyMetadata(360d, OnAngleChanged));
        
        #endregion
                
        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularSeriesBase3D"/> class.
        /// </summary>
        protected CircularSeriesBase3D()
        {
            InternalCircleCoefficient = 0.8;
            this.YValues = new List<double>();
        }

        #endregion

        #region Properties

        #region Public Properties
        /// <summary>
        /// Gets or sets the type of line to be use for connecting data labels and segments.
        /// </summary>
        /// <value>
        /// <c>ConnectorMode.Line</c> will draw straight line.
        /// <c>ConnectorMode.Bezier</c> will draw bezier curve.
        /// </value>
        public ConnectorMode ConnectorType
        {
            get { return (ConnectorMode)GetValue(ConnectorTypeProperty); }
            set { this.SetValue(ConnectorTypeProperty, value); }
        }       

        /// <summary>
        /// Gets or sets a value indicating whether to enable the smart label placement to avoid data label overlapping.
        /// </summary>
        /// <value>
        ///   <c>true</c> to avoid overlapping;
        /// </value>
        public bool EnableSmartLabels
        {
            get { return (bool)GetValue(EnableSmartLabelsProperty); }
            set { this.SetValue(EnableSmartLabelsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the coefficient defines the ratio of the series size with respect to chart area. 
        /// </summary>
        /// <remarks>
        ///     This will be useful for reducing the white spaces around the series.
        /// </remarks>
        /// <value>
        /// Gets or sets the double value ranges from 0 to 1.
        /// </value>
        public double CircleCoefficient
        {
            get { return (double)GetValue(CircleCoefficientProperty); }
            set { this.SetValue(CircleCoefficientProperty, value); }
        }

        /// <summary>
        /// Gets or sets the data labels position of the circular series.
        /// </summary>
        /// <value>
        ///     <c>CircularSeriesLabelPosition.Inside</c>, adornment labels will be placed inside over the series.
        ///     <c>CircularSeriesLabelPosition.Outside</c>, adornment labels will be  placed just outside over the series.
        ///     <c>CircularSeriesLabelPosition.OutsideExtend</c>, adornment labels will be placed outside over the series at a certain distance.
        /// </value>
        public CircularSeriesLabelPosition LabelPosition
        {
            get { return (CircularSeriesLabelPosition)GetValue(LabelPositionProperty); }
            set { this.SetValue(LabelPositionProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the binding path for y axis.
        /// </summary>
        public string YBindingPath
        {
            get { return (string)GetValue(YBindingPathProperty); }
            set { this.SetValue(YBindingPathProperty, value); }
        }
       
        /// <summary>
        /// Gets or sets the radial distance for the exploded segment from center.
        /// </summary>
        public double ExplodeRadius
        {
            get { return (double)GetValue(ExplodeRadiusProperty); }
            set { this.SetValue(ExplodeRadiusProperty, value); }
        }
      
        /// <summary>
        /// Gets or sets the index of data point (or segment) to be explode.
        /// </summary>
        public int ExplodeIndex
        {
            get { return (int)GetValue(ExplodeIndexProperty); }
            set { this.SetValue(ExplodeIndexProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether to explode all the pie slices (segments).
        /// </summary>
        /// <value>
        ///     <c>True</c> will explode all the segments.
        /// </value>
        public bool ExplodeAll
        {
            get { return (bool)GetValue(ExplodeAllProperty); }
            set { this.SetValue(ExplodeAllProperty, value); }
        }

        /// <summary>
        /// Gets or sets the start angle for drawing the circular series.
        /// </summary>
        /// <value>
        /// The double value ranges from 0 to 360 degree.
        /// </value>
        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { this.SetValue(StartAngleProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the end angle for drawing the circular series.
        /// </summary>
        /// <value>
        /// The double value ranges from 0 to 360 degree.
        /// </value>
        public double EndAngle
        {
            get { return (double)GetValue(EndAngleProperty); }
            set { this.SetValue(EndAngleProperty, value); }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the internal circle co-efficient.
        /// </summary>
        internal double InternalCircleCoefficient { get; set; }
        
        /// <summary>
        /// Gets or sets the center of the circular series.
        /// </summary>
        internal Point Center { get; set; }

        #endregion

        #region Protected Properties
        
        /// <summary>
        /// Gets or sets the YValues.
        /// </summary>
        protected internal IList<double> YValues { get; set; }
        
        #endregion

        #endregion

        #region Methods

        #region Internal Methods

        /// <summary>
        /// Validate the data points for segment implementation.
        /// </summary>
        internal override void ValidateYValues()
        {
            foreach (var yValue in this.YValues)
            {
                if (double.IsNaN(yValue) && this.ShowEmptyPoints)
                {
                    this.ValidateDataPoints(this.YValues);
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the circular series count.
        /// </summary>
        /// <returns>Gets the visible series count.</returns>
        internal int GetCircularSeriesCount()
        {
            return (from series in Area.VisibleSeries where series is CircularSeriesBase3D select series).ToList().Count();
        }

        /// <summary>
        /// Gets the pie series index.
        /// </summary>
        /// <returns>Returns series index.</returns>
        internal int GetPieSeriesIndex()
        {
            int index;
            var pieSeries = (from series in Area.VisibleSeries where series is PieSeries3D select series).ToList();
            return (index = pieSeries.IndexOf(this)) >= 0 ? index : -1;
        }
        
        /// <summary>
        /// Validate the data points for segment implementation.
        /// </summary>
        /// <param name="emptyPointIndex">The Empty Points</param>
        internal override void ReValidateYValues(List<int>[] emptyPointIndex)
        {
            foreach (var item in emptyPointIndex)
            {
                foreach (var index in item)
                {
                    this.YValues[index] = double.NaN;
                }
            }
        }

        /// <summary>
        /// Gets the actual center of the series.
        /// </summary>
        /// <param name="centerPoint">The Center Point</param>
        /// <param name="radius">The Radius</param>
        /// <returns>Returns the actual center point.</returns>
        internal Point GetActualCenter(Point centerPoint, double radius)
        {
            if (this.Area != null && Area.Series.IndexOf(this) > 0)
            {
                return centerPoint;
            }

            Point actualCenter = centerPoint;
            double startAngle = this.StartAngle;
            double endAngle = this.EndAngle;
            double[] regions = new double[] { -630, -540, -450, -360, -270, -180, -90, 0, 90, 180, 270, 360, 450, 540, 630 };
            List<int> region = new List<int>();
            if (startAngle < endAngle)
            {
                for (int i = 0; i < regions.Count(); i++)
                {
                    if (regions[i] > startAngle && regions[i] < endAngle)
                    {
                        region.Add((int)((regions[i] % 360) < 0 ? (regions[i] % 360) + 360 : (regions[i] % 360)));
                    }
                }
            }
            else
            {
                for (int i = 0; i < regions.Count(); i++)
                {
                    if (regions[i] < startAngle && regions[i] > endAngle)
                    {
                        region.Add((int)((regions[i] % 360) < 0 ? (regions[i] % 360) + 360 : (regions[i] % 360)));
                    }
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
                    {
                        point1 = new Point(minPoint.X, maxPoint.Y);
                    }
                    else
                    {
                        point1 = new Point(maxPoint.X, minPoint.Y);
                    }

                    if (region[0] == 0 || region[0] == 180)
                    {
                        point2 = new Point(CircularSeriesBase3D.GetMinMaxValue(startPoint, endPoint, region[0]), CircularSeriesBase3D.GetMinMaxValue(startPoint, endPoint, region[1]));
                    }
                    else
                    {
                        point2 = new Point(CircularSeriesBase3D.GetMinMaxValue(startPoint, endPoint, region[1]), CircularSeriesBase3D.GetMinMaxValue(startPoint, endPoint, region[0]));
                    }

                    midPoint = new Point(Math.Abs(point1.X - point2.X) / 2 >= radius ? 0 : (point1.X + point2.X) / 2, y: Math.Abs(point1.Y - point2.Y) / 2 >= radius ? 0 : (point1.Y + point2.Y) / 2);
                    actualCenter.X = centerPoint.X + (midPoint.X == 0 ? 0 : (centerPoint.X - midPoint.X) >= radius ? 0 : (centerPoint.X - midPoint.X));
                    actualCenter.Y = centerPoint.Y + (midPoint.Y == 0 ? 0 : (centerPoint.Y - midPoint.Y) >= radius ? 0 : (centerPoint.Y - midPoint.Y));
                    break;
            }

            return actualCenter;
        }

        #endregion

        #region Protected Internal Methods
        
        /// <summary>
        /// Degrees to radian converter.
        /// </summary>
        /// <param name="degree">The degree.</param>
        /// <returns>Returns the radian.</returns>
        protected internal static double DegreeToRadianConverter(double degree)
        {
            return degree * Math.PI / 180;
        }
        
        /// <summary>
        /// Method implementation for Generate points for Indicator
        /// </summary>
        protected internal override void GeneratePoints()
        {
            this.GeneratePoints(new[] { this.YBindingPath }, this.YValues);
        }
 
        #endregion

        #region Protected Methods

        /// <summary>
        /// Called when DataSource property get changed
        /// </summary>
        /// <param name="oldValue">The Old Value</param>
        /// <param name="newValue">The New Value</param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnDataSourceChanged(oldValue, newValue);
            this.YValues.Clear();
            Segments.Clear();
            this.GeneratePoints(new[] { this.YBindingPath }, this.YValues);
            if (this.Area != null)
            {
                Area.IsUpdateLegend = true;
            }

            this.UpdateArea();
        }

        /// <summary>
        /// Raises the <see>
        /// <cref>E:BindingPathChanged</cref>
        /// </see>
        /// event.
        /// </summary>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            this.YValues.Clear();
            Segments.Clear();
            if (this.Area != null)
            {
                Area.IsUpdateLegend = true;
            }

            base.OnBindingPathChanged(args);
        }

        /// <summary>
        /// Clones the series.
        /// </summary>
        /// <param name="obj">The Object</param>
        /// <returns>Returns the cloned series.</returns>
        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            var circularSeriesBase3D = obj as CircularSeriesBase3D;
            if (circularSeriesBase3D != null)
            {
                circularSeriesBase3D.YBindingPath = this.YBindingPath;
                circularSeriesBase3D.StartAngle = this.StartAngle;
                circularSeriesBase3D.EndAngle = this.EndAngle;
                circularSeriesBase3D.ExplodeAll = this.ExplodeAll;
                circularSeriesBase3D.ExplodeIndex = this.ExplodeIndex;
                circularSeriesBase3D.ExplodeRadius = this.ExplodeRadius;
                circularSeriesBase3D.LabelPosition = this.LabelPosition;
                circularSeriesBase3D.ConnectorType = this.ConnectorType;
                circularSeriesBase3D.EnableSmartLabels = this.EnableSmartLabels;
                circularSeriesBase3D.CircleCoefficient = this.CircleCoefficient;
            }

            return base.CloneSeries(circularSeriesBase3D);
        }

        #endregion

        #region Private Static Methods
        
        /// <summary>
        /// Updates the series when the circle coefficient changes.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Dependency Object</param>
        private static void OnCircleCoefficientChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var circularSeriesBase3D = sender as CircularSeriesBase3D;
            if (circularSeriesBase3D != null)
            {
                circularSeriesBase3D.InternalCircleCoefficient = ChartMath.MinMax((double)e.NewValue, 0, 1);
                circularSeriesBase3D.UpdateArea();
            }
        }

        /// <summary>
        /// Updates the series when the y path changes.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnYPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CircularSeriesBase3D)d).OnBindingPathChanged(e);
        }
        
        /// <summary>
        /// Updates the series when it's properties changes.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="args">The Event Arguments</param>
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            CircularSeriesBase3D series = d as CircularSeriesBase3D;
            if (series != null)
            {
                series.UpdateArea();
            }
        }
        
        /// <summary>
        /// Updates the series when the start or end angle changes.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularSeriesBase3D series = d as CircularSeriesBase3D;
            if (series != null)
            {
                series.UpdateArea();
            }
        }
        
        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the minimum and maximum value.
        /// </summary>
        /// <param name="point1">The First Point</param>
        /// <param name="point2">The Second Point</param>
        /// <param name="degree">The Degree</param>
        /// <returns>Returns the minimum and maximum value.</returns>
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

        #endregion

        #endregion
    }
}
