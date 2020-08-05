// <copyright file="CategoryAxis3D.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{

    using Windows.Foundation;
    using Windows.UI.Xaml;

    /// <summary>
    /// Class implementation for CategoryAxis
    /// </summary>
    public partial class CategoryAxis3D : ChartAxisBase3D
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="LabelPlacement"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelPlacementProperty =
            DependencyProperty.Register("LabelPlacement", typeof(LabelPlacement), typeof(CategoryAxis3D), new PropertyMetadata(LabelPlacement.OnTicks, OnIntervalChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="IsIndexed"/> property.
        /// </summary>
        public static readonly DependencyProperty IsIndexedProperty =
            DependencyProperty.Register("IsIndexed", typeof(bool), typeof(CategoryAxis3D), new PropertyMetadata(true, OnIntervalChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="AggregateFunctions"/> property.
        /// </summary>
        public static readonly DependencyProperty AggregateFunctionsProperty =
            DependencyProperty.Register("AggregateFunctions", typeof(AggregateFunctions), typeof(CategoryAxis3D), new PropertyMetadata(AggregateFunctions.None, OnIntervalChanged));

#if NETFX_CORE

        /// <summary>
        ///  The DependencyProperty for <see cref="Interval"/> property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(object), typeof(CategoryAxis3D), new PropertyMetadata(null, OnIntervalChanged));


#endif
        #endregion

        #region Properties

#if NETFX_CORE

        /// <summary>
        /// Gets or sets a value that determines the interval between labels are null-able/>.
        /// If this property is not set, interval will be calculated automatically.
        /// </summary>
        public object Interval
        {
            get { return this.GetValue(IntervalProperty); }
            set { this.SetValue(IntervalProperty, value); }
        }

#endif

        /// <summary>
        /// Gets or sets a property used to define the axis label placement with respect to tick lines.
        /// </summary>
        /// <value>
        ///     <c>LabelPlacement.BetweenTicks</c>, to place label between the ticks;
        ///     <c>LabelPlacement.OnTicks</c>, to place label with tick as center. This is default value.
        /// </value>
        public LabelPlacement LabelPlacement
        {
            get { return (LabelPlacement)GetValue(LabelPlacementProperty); }
            set { this.SetValue(LabelPlacementProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to group the Category axis label values and create the segment based on it.
        /// </summary>
        /// <value>
        ///     <c>True</c>, to created the segment based on the index value. This is default value.;
        ///     <c>False</c>, to created the segment based on the axis label values.
        /// </value>
        public bool IsIndexed
        {
            get { return (bool)GetValue(IsIndexedProperty); }
            set { this.SetValue(IsIndexedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a property used to aggregate the grouped values.
        /// </summary>
        public AggregateFunctions AggregateFunctions
        {
            get { return (AggregateFunctions)GetValue(AggregateFunctionsProperty); }
            set { this.SetValue(AggregateFunctionsProperty, value); }
        }

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Method implementation for Get LabelContent for given position
        /// </summary>
        /// <param name="position">The Position</param>
        /// <returns>The Label Content</returns>
        public override object GetLabelContent(double position)
        {
            return CategoryAxisHelper.GetLabelContent(this, position);
        }

        #endregion

        #region Protected Internal Methods

        /// <summary>
        /// Calculates actual interval
        /// </summary>
        /// <param name="range">The Range</param>
        /// <param name="availableSize">The Available Size</param>
        /// <returns>Actual Interval</returns>
        protected internal override double CalculateActualInterval(DoubleRange range, Size availableSize)
        {
            return CategoryAxisHelper.CalculateActualInterval(this, range, availableSize, this.Interval);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Called when interval property changed 
        /// </summary>
        /// <param name="e">The Event Arguments</param>
        protected virtual void OnIntervalChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Area != null)
            {
                this.Area.ScheduleUpdate();
            }
        }

        /// <summary>
        /// Apply padding based on interval
        /// </summary>
        /// <param name="range">The Range</param>
        /// <param name="interval">The Interval</param>
        /// <returns>The Range Padding</returns>
        protected override DoubleRange ApplyRangePadding(DoubleRange range, double interval)
        {
            return CategoryAxisHelper.ApplyRangePadding(this, range, interval, LabelPlacement);
        }

        /// <summary>
        /// Method implementation for Generate Visible labels for CategoryAxis
        /// </summary>
        protected override void GenerateVisibleLabels()
        {
            CategoryAxisHelper.GenerateVisibleLabels(this, LabelPlacement);
        }

        /// <summary>
        /// Clones the axis.
        /// </summary>
        /// <param name="obj">The Object</param>
        /// <returns>Cloned Axis</returns>
        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            var categoryAxis = new CategoryAxis3D();
            categoryAxis.Interval = this.Interval;
            categoryAxis.LabelPlacement = LabelPlacement;
            categoryAxis.IsIndexed = this.IsIndexed;
            categoryAxis.AggregateFunctions = this.AggregateFunctions;

            obj = categoryAxis;
            return base.CloneAxis(obj);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the axis when the interval property is changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CategoryAxis3D)d).OnIntervalChanged(e);
        }

        #endregion

        #endregion
    }
}
