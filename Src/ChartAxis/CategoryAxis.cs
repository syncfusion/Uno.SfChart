using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for CategoryAxis
    /// </summary>
    public partial class CategoryAxis : ChartAxisBase2D
    {
        #region Dependency Property Registrations
        
#if NETFX_CORE

        /// <summary>
        /// The DependencyProperty for <see cref="Interval"/> property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(
                "Interval", 
                typeof(object), 
                typeof(CategoryAxis),
                new PropertyMetadata(null, OnIntervalChanged));

#else

        /// <summary>
        /// The DependencyProperty for <see cref="Interval"/> property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register(
                "Interval", 
                typeof(double?),
                typeof(CategoryAxis),
                new PropertyMetadata(null, OnIntervalChanged));

#endif
        
        /// <summary>
        /// The DependencyProperty for <see cref="LabelPlacement"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelPlacementProperty =
            DependencyProperty.Register(
                "LabelPlacement",
                typeof(LabelPlacement),
                typeof(CategoryAxis),
                new PropertyMetadata(LabelPlacement.OnTicks, OnIntervalChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="IsIndexed"/> property.
        /// </summary>
        public static readonly DependencyProperty IsIndexedProperty =
            DependencyProperty.Register("IsIndexed", typeof(bool), typeof(CategoryAxis), new PropertyMetadata(true, OnIntervalChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="AggregateFunctions"/> property.
        /// </summary>
        public static readonly DependencyProperty AggregateFunctionsProperty =
            DependencyProperty.Register("AggregateFunctions", typeof(AggregateFunctions), typeof(CategoryAxis), new PropertyMetadata(AggregateFunctions.None, OnIntervalChanged));

        #endregion

        #region Properties

        #region Public Properties

#if NETFX_CORE

        /// <summary>
        /// Gets or sets a value that determines the interval between labels.
        /// If this property is not set, interval will be calculated automatically.
        /// </summary>
        public object Interval
        {
            get { return (object)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

#else

        /// <summary>
        /// Gets or sets a value that determines the interval between labels. Its nullable.
        /// If this property is not set, interval will be calculated automatically.
        /// </summary>
        public double? Interval
        {
            get { return (double?)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }     

#endif

        /// <summary>
        /// Gets or sets the axis label placement with respect to ticklines.
        /// </summary>
        /// <value>
        ///     <c>LabelPlacement.BetweenTicks</c>, to place label between the ticks;
        ///     <c>LabelPlacement.OnTicks</c>, to place label with tick as center. This is default value.
        /// </value>
        public LabelPlacement LabelPlacement
        {
            get { return (LabelPlacement)GetValue(LabelPlacementProperty); }
            set { SetValue(LabelPlacementProperty, value); }
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
            set { SetValue(IsIndexedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the aggregate for the grouped values.
        /// </summary>
        public AggregateFunctions AggregateFunctions
        {
            get { return (AggregateFunctions)GetValue(AggregateFunctionsProperty); }
            set { SetValue(AggregateFunctionsProperty, value); }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Method implementation for Get LabelContent for given position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override object GetLabelContent(double position)
        {
            return CategoryAxisHelper.GetLabelContent(this, position);
        }

        #endregion

        #region Protected Internal Override Methods

        /// <summary>
        /// Calculates actual interval
        /// </summary>
        /// <param name="range"></param>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected internal override double CalculateActualInterval(DoubleRange range, Size availableSize)
        {
            return CategoryAxisHelper.CalculateActualInterval(this, range, availableSize, Interval);
        }

        #endregion

        #region Protected Virtual Methods

        /// <summary>
        /// Called when interval property changed 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnIntervalChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Area != null)
                this.Area.ScheduleUpdate();
        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Apply padding based on interval
        /// </summary>
        /// <param name="range"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        protected override DoubleRange ApplyRangePadding(DoubleRange range, double interval)
        {
            return CategoryAxisHelper.ApplyRangePadding(this, range, interval, LabelPlacement);
        }

        /// <summary>
        /// Method implementation for generating visible labels for CategoryAxis. 
        /// </summary>
        protected override void GenerateVisibleLabels()
        {
            SetRangeForAxisStyle();
            CategoryAxisHelper.GenerateVisibleLabels(this, LabelPlacement);
        }

        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            var categoryAxis = new CategoryAxis();
            categoryAxis.Interval = this.Interval;
            categoryAxis.LabelPlacement = this.LabelPlacement;
            categoryAxis.IsIndexed = this.IsIndexed;
            categoryAxis.AggregateFunctions = this.AggregateFunctions;
            obj = categoryAxis;
            return base.CloneAxis(obj);
        }

        #endregion
        
        #region Private Static Methods

        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CategoryAxis).OnIntervalChanged(e);
        }
        
        #endregion

        #endregion
    }
}
