// <copyright file="RangeAxisBase3D.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using Windows.UI.Xaml;

    /// <summary>
    /// Class implementation for RangeAxisBase
    /// </summary>
    public abstract partial class RangeAxisBase3D : ChartAxisBase3D, IRangeAxis
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="SmallTicksPerInterval"/> property.
        /// </summary>
        public static readonly DependencyProperty SmallTicksPerIntervalProperty =
            DependencyProperty.Register(
                "SmallTicksPerInterval", 
                typeof(int), 
                typeof(RangeAxisBase3D),
                new PropertyMetadata(0, OnSmallTicksPerIntervalPropertyChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="SmallTickLineSize"/> property.
        /// </summary>
        public static readonly DependencyProperty SmallTickLineSizeProperty =
            DependencyProperty.Register(
                "SmallTickLineSize", 
                typeof(double), 
                typeof(RangeAxisBase3D), 
                new PropertyMetadata(5d));
        
        /// <summary>
        /// The DependencyProperty for <see cref="SmallTickLinesPosition"/> property.
        /// </summary>
        public static readonly DependencyProperty SmallTickLinesPositionProperty =
            DependencyProperty.Register(
                    "SmallTickLinesPosition", 
                    typeof(AxisElementPosition), 
                    typeof(RangeAxisBase3D),
                    new PropertyMetadata(AxisElementPosition.Outside));
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets small tick’s interval
        /// </summary>
        public int SmallTicksPerInterval
        {
            get { return (int)GetValue(SmallTicksPerIntervalProperty); }
            set { this.SetValue(SmallTicksPerIntervalProperty, value); }
        }
                
        /// <summary>
        /// Gets or sets small tick line size
        /// </summary>
        public double SmallTickLineSize
        {
            get { return (double)GetValue(SmallTickLineSizeProperty); }
            set { this.SetValue(SmallTickLineSizeProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets small tick lines position
        /// </summary>
        public AxisElementPosition SmallTickLinesPosition
        {
            get { return (AxisElementPosition)GetValue(SmallTickLinesPositionProperty); }
            set { this.SetValue(SmallTickLinesPositionProperty, value); }
        }
        
        /// <summary>
        /// Gets the interface range.
        /// </summary>
        DoubleRange IRangeAxis.Range
        {
            get { return this.ActualRange; }
        }
        
        /// <summary>
        /// Gets the range.
        /// </summary>
        protected DoubleRange Range
        {
            get { return this.ActualRange; }
        }

        #endregion

        #region Methods

        #region Protected Internal Methods

        /// <summary>
        /// Method implementation for Add SmallTicksPoint
        /// </summary>
        /// <param name="position">The Position</param>
        protected internal override void AddSmallTicksPoint(double position)
        {
            RangeAxisBaseHelper.AddSmallTicksPoint(this, position, this.VisibleInterval, this.SmallTicksPerInterval);
        }

        /// <summary>
        /// Method implementation for Add smallTicks to axis.
        /// </summary>
        /// <param name="position">The Position</param>
        /// <param name="interval">The Interval</param>
        protected internal override void AddSmallTicksPoint(double position, double interval)
        {
            RangeAxisBaseHelper.AddSmallTicksPoint(this, position, interval, this.SmallTicksPerInterval);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Method implementation for Generate Labels in ChartAxis
        /// </summary>
        protected override void GenerateVisibleLabels()
        {
            RangeAxisBaseHelper.GenerateVisibleLabels(this, this.SmallTickLineSize);
        }

        /// <summary>
        /// Clones the given axis.
        /// </summary>
        /// <param name="obj">The Object</param>
        /// <returns>Returns the cloned axis.</returns>
        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            var rangeAxisBase3D = (RangeAxisBase3D)obj;
            rangeAxisBase3D.SmallTicksPerInterval = this.SmallTicksPerInterval;
            rangeAxisBase3D.SmallTickLinesPosition = this.SmallTickLinesPosition;
            rangeAxisBase3D.SmallTickLineSize = this.SmallTickLineSize;
            return base.CloneAxis(obj);
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Updates the axis when the <see cref="SmallTicksPerInterval"/> property changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnSmallTicksPerIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var axis = d as RangeAxisBase3D;
            if (axis != null)
            {
                axis.smallTicksRequired = (int)e.NewValue > 0 || axis.smallTicksRequired;
            }
        }
        
        #endregion
              
        #endregion
    }
}
