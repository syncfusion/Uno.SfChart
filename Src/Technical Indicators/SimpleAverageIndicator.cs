using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using System.Collections;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents SimpleAverage technical indicator.
    /// </summary>
    /// <seealso cref="FinancialTechnicalIndicator"/>
    /// <seealso cref="TechnicalIndicatorSegment"/>
    public partial class SimpleAverageIndicator:FinancialTechnicalIndicator
    {

        #region constructor

        public SimpleAverageIndicator()
        {
#if UNIVERSALWINDOWS
            SignalLineColor = new SolidColorBrush(Colors.Blue);
#endif
        }

        #endregion

        #region fields

        IList<double> YValues = new List<double>();

        List<double> xValues;

        List<double> xPoints=new List<double>();

        List<double> yPoints=new List<double>();

        TechnicalIndicatorSegment fastLineSegment;

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets the moving average period for indicator.
        /// </summary>
        /// <remarks>
        /// The default value is 14 days.
        /// </remarks>
        public int Period
        {
            get { return (int)GetValue(PeriodProperty); }
            set { SetValue(PeriodProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="Period"/> property.
        /// </summary>
        public static readonly DependencyProperty PeriodProperty =
            DependencyProperty.Register("Period", typeof(int), typeof(SimpleAverageIndicator), 
            new PropertyMetadata(2,OnMovingAverageChanged));


        /// <summary>
        /// Gets or sets the signal line color.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush SignalLineColor
        {
            get { return (Brush)GetValue(SignalLineColorProperty); }
            set { SetValue(SignalLineColorProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="SignalLineColor"/> property.
        /// </summary>
        public static readonly DependencyProperty SignalLineColorProperty =
            DependencyProperty.Register("SignalLineColor", typeof(Brush), typeof(SimpleAverageIndicator),
#if UNIVERSALWINDOWS
            new PropertyMetadata(null, OnColorChanged));
#else
            new PropertyMetadata(new SolidColorBrush(Colors.Blue),OnColorChanged));
#endif


        #endregion

        #region Methods

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SimpleAverageIndicator indicator = d as SimpleAverageIndicator;
            if (indicator != null) indicator.UpdateArea();
        }

        private static void OnMovingAverageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SimpleAverageIndicator indicator = d as SimpleAverageIndicator;
            indicator.UpdateArea();
        }
        /// <summary>
        /// Called when DataSource changed 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnDataSourceChanged(oldValue, newValue);
            fastLineSegment = null;
            YValues.Clear();
            GeneratePoints(new string[] { Close }, YValues);
            this.UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            fastLineSegment = null;
            YValues.Clear();
            base.OnBindingPathChanged(args);
        }
        /// <summary>
        /// Method implementation for Set ItemSource to Series
        /// </summary>
        /// <param name="series"></param>
        protected internal override void SetSeriesItemSource(ChartSeriesBase series)
        {
            if (series.ActualSeriesYValues.Length > 0)
            {
                this.ActualXValues = Clone(series.ActualXValues);
                this.YValues = Clone(series.ActualSeriesYValues[0]);
                this.Area.ScheduleUpdate();
            }
        }

        /// <summary>
        /// Method implementation for GeneratePoints for TechnicalIndicator
        /// </summary>
        protected internal override void GeneratePoints()
        {
            GeneratePoints(new string[] { Close }, YValues);
        }

        /// <summary>
        /// Creates the segments of SimpleAverageIndicator.
        /// </summary>
       
        public override void CreateSegments()
        {
            xValues = GetXValues();
            if (Period < 1) return;
            Period = Period < xValues.Count ? Period : xValues.Count - 1;
            ComputeMovingAverage(Period, xValues, YValues, xPoints, yPoints);

            if (fastLineSegment == null)
            {
                TechnicalIndicatorSegment segment = new TechnicalIndicatorSegment(xValues, yPoints, SignalLineColor, this,Period);
                segment.SetValues(xValues, yPoints, SignalLineColor, this, Period);
                fastLineSegment = segment;
                Segments.Add(segment);
                
            }
            else if (ActualXValues != null)
            {
                fastLineSegment.SetData(xValues, yPoints, SignalLineColor, Period);
                fastLineSegment.SetRange();
            }
        }

        /// <summary>
        /// Updates the segment at the specified index
        /// </summary>
        /// <param name="index">The index of the segment.</param>
        /// <param name="action">The action that caused the segments collection changed event</param>
       
        public override void UpdateSegments(int index, NotifyCollectionChangedAction action)
        {
            this.Area.ScheduleUpdate();
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            obj = new SimpleAverageIndicator() { SignalLineColor = this.SignalLineColor, Period = this.Period };
            return base.CloneSeries(obj);
        }

#endregion

    }
}
