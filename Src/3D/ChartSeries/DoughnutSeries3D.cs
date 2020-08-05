// <copyright file="DoughnutSeries3D.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System;
    using Windows.UI.Xaml;

    /// <summary>
    /// Class implementation for DoughnutSeries3D
    /// </summary>
    public partial class DoughnutSeries3D : PieSeries3D
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="DoughnutCoefficient"/> property.
        /// </summary>
        public static readonly DependencyProperty DoughnutCoefficientProperty =
            DependencyProperty.Register(
                "DoughnutCoefficient", 
                typeof(double), 
                typeof(DoughnutSeries3D),
                new PropertyMetadata(0.8d, OnDoughnutCoefficientChanged));
        
        // Using a DependencyProperty as the backing store for DoughnutHoleSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DoughnutHoleSizeProperty =
            DependencyProperty.RegisterAttached(
                "DoughnutHoleSize", 
                typeof(double), 
                typeof(DoughnutSeries3D), 
                new PropertyMetadata(0.5, OnDoughnutHoleSizeChanged));
        
        #endregion
        
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DoughnutSeries3D"/> class.
        /// </summary>
        public DoughnutSeries3D()
        {
            InternlDoughnutCoefficient = 0.8;
        }

        #endregion
        
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the inner circular radius of the <c>DoughnutSeries3D</c>.
        /// </summary>
        /// <value>
        /// The double value ranges from 0 to 1.
        /// </value>
        public double DoughnutCoefficient
        {
            get { return (double)GetValue(DoughnutCoefficientProperty); }
            set { this.SetValue(DoughnutCoefficientProperty, value); }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the internal doughnut co-efficient.
        /// </summary>
        internal double InternlDoughnutCoefficient { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Gets the doughnut hole size.
        /// </summary>
        /// <param name="obj">The Object</param>
        /// <returns>Returns the doughnut hole size.</returns>
        public static double GetDoughnutHoleSize(DependencyObject obj)
        {
            return (double)obj.GetValue(DoughnutHoleSizeProperty);
        }

        /// <summary>
        /// Sets the doughnut hole size.
        /// </summary>
        /// <param name="obj">The Object</param>
        /// <param name="value">The Value</param>
        public static void SetDoughnutHoleSize(DependencyObject obj, double value)
        {
            obj.SetValue(DoughnutHoleSizeProperty, value);
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
            return base.CloneSeries(new DoughnutSeries3D()
            {
                DoughnutCoefficient = this.DoughnutCoefficient
            });
        }

        /// <summary>
        /// Creates the point.
        /// </summary>
        protected override void CreatePoints()
        {
            if (Area.RootPanelDesiredSize != null)
            {
                this.actualWidth = Area.RootPanelDesiredSize.Value.Width;
                this.actualHeight = Area.RootPanelDesiredSize.Value.Height;
            }

            var doughnutIndex = GetPieSeriesIndex();
            var doughnutCount = GetCircularSeriesCount();

            double actualRadius = InternalCircleCoefficient * Math.Min(actualWidth, actualHeight) / 2;
            double remainingWidth = actualRadius - (actualRadius * Area.InternalDoughnutHoleSize);
            double equalParts = remainingWidth / doughnutCount;
            this.Radius = actualRadius - (equalParts * (doughnutCount - (doughnutIndex + 1)));
            this.InnerRadius = this.Radius - (equalParts * this.InternlDoughnutCoefficient);
            this.InnerRadius = ChartMath.MaxZero(this.InnerRadius);

            base.CreatePoints();
        }

        #endregion

        #region Private Static Methods
        
        /// <summary>
        /// Updates the series when doughnut coefficient changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnDoughnutCoefficientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var doughnutSeries3D = d as DoughnutSeries3D;
            doughnutSeries3D.InternlDoughnutCoefficient = ChartMath.MinMax((double)e.NewValue, 0, 1);
            doughnutSeries3D.UpdateArea();
        }

        /// <summary>
        /// Updates the doughnut series when doughnut hole size changed.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnDoughnutHoleSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var chartArea = sender as SfChart3D;
            if (chartArea != null)
            {
                chartArea.InternalDoughnutHoleSize = ChartMath.MinMax((double)e.NewValue, 0, 1);
                chartArea.ScheduleUpdate();
            }
        }

        #endregion

        #endregion
    }
}
