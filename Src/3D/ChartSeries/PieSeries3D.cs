// <copyright file="PieSeries3D.cs" company="Syncfusion. Inc">
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
    using System.ComponentModel;
    using System.Linq;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Animation;
    using Windows.UI.Xaml.Shapes;
    using WindowsLineSegment = Windows.UI.Xaml.Media.LineSegment;

    /// <summary>
    /// Class implementation for PieSeries3D
    /// </summary>
    public partial class PieSeries3D : CircularSeriesBase3D
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="ExplodeOnMouseClick"/> property.
        /// </summary>
        public static readonly DependencyProperty ExplodeOnMouseClickProperty =
            DependencyProperty.Register(
                "ExplodeOnMouseClick", 
                typeof(bool), 
                typeof(PieSeries3D),
                new PropertyMetadata(false));

        #endregion

        #region Fields

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "This is a potected property")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "This is a protected property")]
        protected double actualWidth, actualHeight;

        private bool allowExplode;

        private ChartSegment mouseUnderSegment;

        private int animateCount = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PieSeries3D"/> class.
        /// </summary>
        public PieSeries3D()
        {
            InnerRadius = 0;
            Radius = 0;
            this.DefaultStyleKey = typeof(PieSeries3D);
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether to enable the segment explode on mouse click.
        /// </summary>
        /// <value>
        /// <c>true</c> to enable the explode on mouse click.
        /// </value>
        public bool ExplodeOnMouseClick
        {
            get { return (bool)GetValue(ExplodeOnMouseClickProperty); }
            set { this.SetValue(ExplodeOnMouseClickProperty, value); }
        }

#if NETFX_CORE
        /// <summary>
        /// Gets or sets the segment property for internal usage
        /// </summary>
        public PieSegment3D Segment { get; set; }
#endif

        #endregion

        #region Protected Internal Properties
        
        /// <summary>
        /// Gets or sets the inner radius of the series.
        /// </summary>
        protected internal double InnerRadius { get; set; }

        /// <summary>
        /// Gets or sets the radius of the series.
        /// </summary>
        protected internal double Radius { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// An abstract method which will be called over to create segments.
        /// </summary>
        public override void CreateSegments()
        {
            Area.ActualDepth = Area.Depth;
            this.InnerRadius = 0d;
            if (this.Area != null)
            {
                this.CreatePoints();
            }

            if (this.ShowEmptyPoints)
            {
                UpdateEmptyPointSegments(this.GetXValues(), false);
            }
        }

        #endregion

        #region Internal Methods
        
        /// <summary>
        /// Updates the series on bounds changed.
        /// </summary>
        /// <param name="size">The Size</param>
        internal override void UpdateOnSeriesBoundChanged(Size size)
        {
            if (this.AdornmentsInfo != null)
            {
                AdornmentsInfo.UpdateElements();
                AdornmentsInfo.Measure(size, null);
            }

            var isupportAxes = this as ISupportAxes;
            var canUpdate = isupportAxes == null || isupportAxes != null && ActualXAxis != null && ActualYAxis != null;

            if (!canUpdate)
            {
                return;
            }

            CreateTransformer(size, true);
            List<Polygon3D>[] poligons = new List<Polygon3D>[]
            {
                new List<Polygon3D>(),
                new List<Polygon3D>(),
                new List<Polygon3D>(),
                new List<Polygon3D>()
            };

            foreach (var segment in this.Segments)
            {
                var pieSegment3D = segment as PieSegment3D;
                if (pieSegment3D != null)
                {
                    pieSegment3D.CreateSegmentVisual(size);
                    var plgs = pieSegment3D.CreateSector();
                    if (plgs != null)
                    {
                        for (int ai = 0; ai < plgs.Length; ai++)
                        {
                            if (plgs[ai] != null)
                            {
                                for (int pi = 0; pi < plgs[ai].Length; pi++)
                                {
                                    poligons[ai].Add(plgs[ai][pi]);
                                }
                            }
                        }
                    }
                }
            }

            for (int ai = 0; ai < poligons.Length; ai++)
            {
                foreach (var item in poligons[ai])
                {
                    Area.Graphics3D.AddVisual(item);
                }
            }
        }

        /// <summary>
        /// Updates the empty points.
        /// </summary>
        /// <param name="xValues">The X Values</param>
        /// <param name="isSidebySideSeries">Is Side By Side Series</param>
        internal override void UpdateEmptyPointSegments(List<double> xValues, bool isSidebySideSeries)
        {
            if (this.EmptyPointIndexes != null)
            {
                foreach (var item in this.EmptyPointIndexes[0])
                {
                    Segments[item].IsEmptySegmentInterior = true;
                    if (Adornments.Count > 0)
                    {
                        Adornments[item].IsEmptySegmentInterior = true;
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets the animation active state.
        /// </summary>
        /// <returns>Returns a <see cref="bool"/> value indicating whether the animation is active.</returns>
        internal override bool GetAnimationIsActive()
        {
            return this.AnimationStoryboard != null && AnimationStoryboard.GetCurrentState() == ClockState.Active;
        }
        
        /// <summary>
        /// Animates this instance.
        /// </summary>
        internal override void Animate()
        {
            PieSegment3D segment = null;

            // WPF-25124 Animation not working properly when resize the window.
            if (this.AnimationStoryboard != null && (this.animateCount <= Segments.Count))
            {
                this.AnimationStoryboard = new Storyboard();
            }
            else if (this.AnimationStoryboard != null)
            {
                AnimationStoryboard.Stop();
                if (!this.canAnimate)
                {
                    foreach (ChartSegment pieSegment in this.Segments)
                    {
                        segment = pieSegment as PieSegment3D;
                        segment.ActualStartValue = (this.StartAngle < this.EndAngle) ? segment.StartValue : segment.EndValue;
                        double value = (StartAngle < EndAngle) ? segment.EndValue - segment.StartValue : segment.StartValue - segment.EndValue;
                        segment.ActualEndValue = value == 0 ? 0.1 : value;
                    }

                    return;
                }
            }
            else
            {
                this.AnimationStoryboard = new Storyboard();
            }

            foreach (ChartSegment pieSegment in this.Segments)
            {
                this.animateCount++;
                if (pieSegment is EmptyPointSegment)
                {
                    continue;
                }

                segment = pieSegment as PieSegment3D;
                var segmentStartAngle = segment.StartValue;
                var segmentEndAngle = segment.EndValue;
                var keyFrames = new DoubleAnimationUsingKeyFrames();
                var keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                    Value = StartAngle
                };
                keyFrames.KeyFrames.Add(keyFrame);

                keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(AnimationDuration),
                    Value = (StartAngle < EndAngle) ? segmentStartAngle : segmentEndAngle
                };
                var keySpline = new KeySpline
                {
                    ControlPoint1 = new Point(0.64, 0.84),
                    ControlPoint2 = new Point(0.67, 0.95)
                };
                keyFrame.KeySpline = keySpline;

                keyFrames.KeyFrames.Add(keyFrame);
                Storyboard.SetTargetProperty(keyFrames, "PieSegment3D.ActualStartValue");
                keyFrames.EnableDependentAnimation = true;
                Storyboard.SetTarget(keyFrames, segment);
                AnimationStoryboard.Children.Add(keyFrames);

                keyFrames = new DoubleAnimationUsingKeyFrames();
                keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                    Value = 0
                };
                keyFrames.KeyFrames.Add(keyFrame);
                double value = (StartAngle < EndAngle) ? segmentEndAngle - segmentStartAngle : segmentStartAngle - segmentEndAngle; // rotate segment clocwise/counterclockwise;
                keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(AnimationDuration),
                    Value = value == 0 ? 0.1 : value
                };
                keySpline = new KeySpline
                {
                    ControlPoint1 = new Point(0.64, 0.84),
                    ControlPoint2 = new Point(0.67, 0.95)
                };
                keyFrame.KeySpline = keySpline;

                keyFrames.KeyFrames.Add(keyFrame);
                Storyboard.SetTargetProperty(keyFrames, "PieSegment3D.ActualEndValue");
                keyFrames.EnableDependentAnimation = true;
                Storyboard.SetTarget(keyFrames, segment);
                AnimationStoryboard.Children.Add(keyFrames);
            }

            AnimationStoryboard.Begin();
        }
        
        #endregion

        #region Protected Internal Methods

        /// <summary>
        /// Creates the <see cref="IChartTransformer"/> value based upon the given size.
        /// </summary>
        /// <param name="size">The Size</param>
        /// <param name="create">The Create Indication</param>
        /// <returns>Returns the created chart transformer.</returns>
        protected internal override IChartTransformer CreateTransformer(Size size, bool create)
        {
            if (create || this.ChartTransformer == null)
            {
                this.ChartTransformer = ChartTransform.CreateSimple(size);
            }

            return this.ChartTransformer;
        }
        
        /// <summary>
        /// Called when [series mouse move].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pos">The position.</param>
        protected internal override void OnSeriesMouseMove(object source, Point pos)
        {
            base.OnSeriesMouseMove(source, pos);
        }
        
        /// <summary>
        /// Called when [series mouse up].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="position">The position.</param>
        protected internal override void OnSeriesMouseUp(object source, Point position)
        {
            var element = source as FrameworkElement;
            var segment = element != null ? element.Tag as ChartSegment : null;
            var newIndex = -1;
            if (this.ExplodeOnMouseClick && this.mouseUnderSegment == segment && this.allowExplode)
            {
                if (segment != null)
                {
                    newIndex = (segment as PieSegment3D).Index;
                }
                else if (Adornments.Count > 0)
                {
                    newIndex = ChartExtensionUtils.GetAdornmentIndex(source);
                }

                var oldIndex = ExplodeIndex;
                if (newIndex != oldIndex)
                {
                    this.ExplodeIndex = newIndex;
                }
                else if (this.ExplodeIndex >= 0)
                {
                    this.ExplodeIndex = -1;
                }

                this.allowExplode = false;
            }

            base.OnSeriesMouseUp(source, position);
        }

        /// <summary>
        /// Called when [series mouse down].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="position">The position.</param>
        protected internal override void OnSeriesMouseDown(object source, Point position)
        {
            this.allowExplode = true;
            var element = source as FrameworkElement;
            this.mouseUnderSegment = element != null ? element.Tag as ChartSegment : null;
            base.OnSeriesMouseDown(source, position);
        }

        #endregion

        #region Protected Static Methods

        /// <summary>
        /// Method implementation for Create Adornments
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="xVal">The x value.</param>
        /// <param name="yVal">The y value.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="startDepth">The start depth.</param>
        /// <returns>Returns the created adornment</returns>
        protected static ChartAdornment CreateAdornment(ChartSeries3D series, double xVal, double yVal, double angle, double radius, double startDepth)
        {
            ChartPieAdornment3D adornment = new ChartPieAdornment3D(startDepth, xVal, yVal, angle, radius, series);
            adornment.XPos = adornment.XData = xVal;
            adornment.YPos = adornment.YData = yVal;
            adornment.Radius = radius;
            adornment.Angle = angle;
            adornment.Series = series;
            adornment.StartDepth = startDepth;

            return adornment;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Clones the series.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Returns the cloned series.</returns>
        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new PieSeries3D() { ExplodeOnMouseClick = this.ExplodeOnMouseClick });
        }
        
        /// <summary>
        /// Creates the points.
        /// </summary>
        protected virtual void CreatePoints()
        {
            var seriesCount = GetCircularSeriesCount();
            var pieIndex = GetPieSeriesIndex();
            Segments.Clear(); // PieSeries3D is not rendered properly while removing data dynamically-WPF-17521
            Adornments.Clear();
            IList<double> toggledYValues = null;
            if (ToggledLegendIndex.Count > 0)
            {
                toggledYValues = this.GetYValues();
            }
            else
            {
                toggledYValues = this.YValues;
            }

            if (Area.RootPanelDesiredSize != null)
            {
                this.actualWidth = Area.RootPanelDesiredSize.Value.Width;
                this.actualHeight = Area.RootPanelDesiredSize.Value.Height;
            }

            var all = toggledYValues.Select(item => Math.Abs(double.IsNaN(item) ? 0d : item)).Sum();
            var count = toggledYValues.Count;

            if (this.InnerRadius == 0)
            {
                double actualRadius = Math.Min(this.actualWidth, this.actualHeight) / 2;
                double equalParts = actualRadius / seriesCount;
                this.Radius = (equalParts * (pieIndex + 1)) - (equalParts * (1 - this.InternalCircleCoefficient));
                this.InnerRadius = equalParts * pieIndex;
            }

            var pieHeight = Area.Depth;
            if (this.ExplodeIndex >= 0 && this.ExplodeIndex < count || this.ExplodeAll)
            {
                this.Radius -= this.ExplodeRadius;
            }

            double arcStartAngle = StartAngle, arcEndAngle = EndAngle, diffAngle;
            diffAngle = arcEndAngle - arcStartAngle;
            if (Math.Abs(diffAngle) > 360)
            {
                diffAngle = diffAngle % 360;
            }

            int segindex = 0;
            for (var i = 0; i < count; i++)
            {
                if (!double.IsNaN(this.YValues[i]))
                {
                    var val = Math.Abs(double.IsNaN(toggledYValues[i]) ? 0 : toggledYValues[i]);
                    arcEndAngle = all == 0 ? 0 : (Math.Abs(val) * ((diffAngle) / all));

                    var rect = new Rect(0, 0, this.actualWidth, this.actualHeight);

                    if (this.ExplodeIndex == i || this.ExplodeAll)
                    {
                        var offset = new Point(
                            (float)(Math.Cos(Math.PI * (2 * arcStartAngle + arcEndAngle) / 360)),
                            (float)(Math.Sin(Math.PI * (2 * arcStartAngle + arcEndAngle) / 360)));

                        rect = rect.Offset(
                            0.01f * this.Radius * offset.X * this.ExplodeRadius,
                            0.01f * this.Radius * offset.Y * this.ExplodeRadius);
                    }

                    if (seriesCount == 1)
                    {
                        this.Center = this.GetActualCenter(new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2), this.Radius);
                    }
                    else
                    {
                        this.Center = new Point(this.actualWidth / 2, this.actualHeight / 2);
                        if (this.ExplodeAll || this.ExplodeIndex == i)
                        {
                            this.Center = new Point(rect.X + Center.X, rect.Y + Center.Y);
                        }
                    }

                    var center = new Vector3D(Center.X, Center.Y, 0);
                    if (segindex < Segments.Count)
                    {
                        Segments[segindex].SetData(arcStartAngle, arcStartAngle + arcEndAngle, pieHeight, this.Radius, val, center.X, center.Y, center.Z, this.InnerRadius);
                        if (ToggledLegendIndex.Contains(i))
                        {
                            Segments[segindex].IsSegmentVisible = false;
                        }
                        else
                        {
                            Segments[segindex].IsSegmentVisible = true;
                        }
                    }
                    else
                    {
                        Segments.Add(new PieSegment3D(this, center, arcStartAngle, arcStartAngle + arcEndAngle, pieHeight, this.Radius, i, val, this.InnerRadius));
                    }

                    if (this.AdornmentsInfo != null)
                    {
                        this.AddPieAdornments(segindex, toggledYValues[i], arcStartAngle, arcStartAngle + arcEndAngle, i, this.Radius, Area.IsChartRotated() ? Area.Depth + 5d : 0d);
                    }

                        segindex++;
                    arcStartAngle += arcEndAngle;
                }
            }
        }
        
        /// <summary>
        /// Updates the series when data source changed.
        /// </summary>
        /// <param name="oldValue">The Old Value</param>
        /// <param name="newValue">The New Value</param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (oldValue != null)
            {
                this.animateCount = 0;
            }

            base.OnDataSourceChanged(oldValue, newValue);
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Adds the pie adornments.
        /// </summary>
        /// <param name="x">The X Value</param>
        /// <param name="y">The Y Value</param>
        /// <param name="startAngle">The Start Angle</param>
        /// <param name="endAngle">The End Angle</param>
        /// <param name="index">The Index</param>
        /// <param name="radius">The Radius</param>
        /// <param name="startDepth">The Start Depth</param>
        private void AddPieAdornments(double x, double y, double startAngle, double endAngle, int index, double radius, double startDepth)
        {
            startAngle = CircularSeriesBase3D.DegreeToRadianConverter(startAngle);
            endAngle = DegreeToRadianConverter(endAngle);
            var angle = (startAngle + endAngle) / 2;

            Adornments.Add(PieSeries3D.CreateAdornment(this, x, y, angle, radius, startDepth));
            Adornments[(int)x].Item = this.ActualData[index];
        }
        
        #endregion

        #endregion
    }
}
