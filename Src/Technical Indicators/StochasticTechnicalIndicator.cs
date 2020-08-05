using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using System.Collections;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents Stochastic technical indicator.
    /// </summary>
    /// <seealso cref="FinancialTechnicalIndicator"/>
    /// <seealso cref="TechnicalIndicatorSegment"/>
    public partial class StochasticTechnicalIndicator:FinancialTechnicalIndicator
    {
        #region constructor

        public StochasticTechnicalIndicator()
        {
#if UNIVERSALWINDOWS
            PeriodLineColor = new SolidColorBrush(Colors.Green);
            UpperLineColor = new SolidColorBrush(Colors.Red);
            LowerLineColor = new SolidColorBrush(Colors.Red);
            SignalLineColor = new SolidColorBrush(Colors.Blue);
#endif
        }

        internal override bool IsMultipleYPathRequired
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region fields

        IList<double> HighValues = new List<double>();

        IList<double> LowValues = new List<double>();

        IList<double> CloseValues = new List<double>();

        List<double> xValues;

        List<double> upperXPoints = new List<double>();

        List<double> upperYPoints = new List<double>();

        List<double> lowerXPoints = new List<double>();

        List<double> lowerYPoints = new List<double>();

        List<double> periodXPoints = new List<double>();

        List<double> periodYPoints = new List<double>();

        List<double> signalXPoints = new List<double>();

        List<double> signalYPoints = new List<double>();

        List<double> percentK = new List<double>();


        TechnicalIndicatorSegment upperLineSegment;

        TechnicalIndicatorSegment lowerLineSegment;

        TechnicalIndicatorSegment periodLineSegment;

        TechnicalIndicatorSegment signalLineSegment;

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
            DependencyProperty.Register("Period", typeof(int), typeof(StochasticTechnicalIndicator), 
            new PropertyMetadata(14, OnPeriodChanged));

        
        /// <summary>
        /// Gets or sets the %K for the Stochastic indicator.
        /// </summary>
        /// <remarks>
        /// %K = (Current Close - Lowest Low)/(Highest High - Lowest Low) * 100
        /// </remarks>
        public int KPeriod
        {
            get { return (int)GetValue(KPeriodProperty); }
            set { SetValue(KPeriodProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="KPeriod"/> property.
        /// </summary>
        public static readonly DependencyProperty KPeriodProperty =
            DependencyProperty.Register("KPeriod", typeof(int), typeof(StochasticTechnicalIndicator), 
            new PropertyMetadata(5, OnPeriodChanged));


        /// <summary>
        /// Gets or sets the %D for the Stochastic indicator.
        /// </summary>
        /// <remarks>
        /// %D = 3-day SMA of %K
        /// </remarks>
        public int DPeriod
        {
            get { return (int)GetValue(DPeriodProperty); }
            set { SetValue(DPeriodProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="DPeriod"/> property.
        /// </summary>
        public static readonly DependencyProperty DPeriodProperty =
            DependencyProperty.Register("DPeriod", typeof(int), typeof(StochasticTechnicalIndicator),
            new PropertyMetadata(3, OnPeriodChanged));


        /// <summary>
        /// Gets or sets the period line color.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush PeriodLineColor
        {
            get { return (Brush)GetValue(PeriodLineColorProperty); }
            set { SetValue(PeriodLineColorProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref=" PeriodLineColor"/> property.
        /// </summary>
        public static readonly DependencyProperty PeriodLineColorProperty =
            DependencyProperty.Register("PeriodLineColor", typeof(Brush), typeof(StochasticTechnicalIndicator),
#if UNIVERSALWINDOWS
                new PropertyMetadata(null, OnColorChanged));
#else
                new PropertyMetadata(new SolidColorBrush(Colors.Green), OnColorChanged));
#endif


        /// <summary>
        /// Gets or sets the upper line color.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush UpperLineColor
        {
            get { return (Brush)GetValue(UpperLineColorProperty); }
            set { SetValue(UpperLineColorProperty, value); }
        }


        /// <summary>
        /// The DependencyProperty for <see cref="UpperLineColor"/> property.
        /// </summary>
        public static readonly DependencyProperty UpperLineColorProperty =
            DependencyProperty.Register("UpperLineColor", typeof(Brush), typeof(StochasticTechnicalIndicator),
#if UNIVERSALWINDOWS
                new PropertyMetadata(null, OnColorChanged));
#else
                new PropertyMetadata(new SolidColorBrush(Colors.Red),OnColorChanged));
#endif


        /// <summary>
        /// Gets or sets the lower line color.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush LowerLineColor
        {
            get { return (Brush)GetValue(LowerLineColorProperty); }
            set { SetValue(LowerLineColorProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="LowerLineColor"/> property.
        /// </summary>
        public static readonly DependencyProperty LowerLineColorProperty =
            DependencyProperty.Register("LowerLineColor", typeof(Brush), typeof(StochasticTechnicalIndicator),
#if UNIVERSALWINDOWS
                new PropertyMetadata(null, OnColorChanged));
#else
                new PropertyMetadata(new SolidColorBrush(Colors.Red),OnColorChanged));
#endif


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
            DependencyProperty.Register("SignalLineColor", typeof(Brush), typeof(StochasticTechnicalIndicator),
#if UNIVERSALWINDOWS
                new PropertyMetadata(null, OnColorChanged));
#else
                new PropertyMetadata(new SolidColorBrush(Colors.Blue),OnColorChanged));
#endif


        #endregion

        #region Methods

        internal override void SetIndicatorInfo(ChartPointInfo info, List<double> yValue, bool seriesPalette)
        {
            if (yValue.Count > 0)
            {
                info.UpperLine = double.IsNaN(yValue[2])? "null" : Math.Round(yValue[2], 2).ToString();
                info.SignalLine = double.IsNaN(yValue[3]) ? "null" : Math.Round(yValue[3], 2).ToString();
            }
        }          

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StochasticTechnicalIndicator indicator = d as StochasticTechnicalIndicator;
            indicator.UpdateArea();
        }

        private static void OnPeriodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StochasticTechnicalIndicator indicator = d as StochasticTechnicalIndicator;
            if (indicator != null) indicator.UpdateArea();
        }

        /// <summary>
        /// Called when DataSource property changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnDataSourceChanged(oldValue, newValue);
            upperLineSegment = null;
            lowerLineSegment = null;
            signalLineSegment = null;
            periodLineSegment = null;
            HighValues.Clear();
            LowValues.Clear();
            CloseValues.Clear();
            GeneratePoints(new string[] { High,Low,Close }, HighValues,LowValues,CloseValues);
            this.UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            HighValues.Clear();
            LowValues.Clear();
            CloseValues.Clear();
            base.OnBindingPathChanged(args);
        }
        
        /// <summary>
        /// Method implementation for Set ItemSource to Series
        /// </summary>
        /// <param name="series"></param>
        protected internal override void SetSeriesItemSource(ChartSeriesBase series)
        {
            if (series.ActualSeriesYValues.Length > 2)
            {
                this.ActualXValues = Clone(series.ActualXValues);
                this.HighValues = Clone(series.ActualSeriesYValues[0]);
                this.LowValues = Clone(series.ActualSeriesYValues[1]);
                this.CloseValues = Clone(series.ActualSeriesYValues[2]);
                this.Area.ScheduleUpdate();
            }
        }

        /// <summary>
        /// Method implementation for GeneratePoints for TechnicalIndicator
        /// </summary>
        protected internal override void GeneratePoints()
        {
            GeneratePoints(new string[] { High, Low, Close }, HighValues, LowValues, CloseValues);
        }

        /// <summary>
        /// Creates the segments of StochasticTechnicalIndicator
        /// </summary>
      
        public override void CreateSegments()
        {
            xValues = GetXValues();
			if (Period < 1 || DPeriod < 1 || KPeriod < 1) return;
            Period = Period < xValues.Count ? Period : xValues.Count - 1;
            BasePoints(Period, percentK);
            AddPoints(Period, KPeriod);
            AddSignalPoints(DPeriod);
            if (upperLineSegment == null || lowerLineSegment == null || periodLineSegment == null)
            {
                Segments.Clear();
                upperLineSegment = new TechnicalIndicatorSegment(upperXPoints, upperYPoints, UpperLineColor, this);
                upperLineSegment.SetValues(upperXPoints, upperYPoints, UpperLineColor, this);
                Segments.Add(upperLineSegment);

                lowerLineSegment = new TechnicalIndicatorSegment(lowerXPoints, lowerYPoints, LowerLineColor, this);
                lowerLineSegment.SetValues(lowerXPoints, lowerYPoints, LowerLineColor, this);
                Segments.Add(lowerLineSegment);

                periodLineSegment = new TechnicalIndicatorSegment(periodXPoints, periodYPoints, PeriodLineColor, this, Period + KPeriod-1);
                periodLineSegment.SetValues(periodXPoints, periodYPoints, PeriodLineColor, this, Period + KPeriod - 1);
                Segments.Add(periodLineSegment);

                signalLineSegment = new TechnicalIndicatorSegment(signalXPoints, signalYPoints, SignalLineColor, this, Period + KPeriod + DPeriod-2);
                signalLineSegment.SetValues(signalXPoints, signalYPoints, SignalLineColor, this, Period + KPeriod + DPeriod - 2);
                Segments.Add(signalLineSegment);
            }
            else
            {
                upperLineSegment.SetData(upperXPoints, upperYPoints, UpperLineColor);
                upperLineSegment.SetRange();
                lowerLineSegment.SetData(lowerXPoints, lowerYPoints, LowerLineColor);
                lowerLineSegment.SetRange();
                periodLineSegment.SetData(periodXPoints, periodYPoints, PeriodLineColor, Period + KPeriod-1);
                periodLineSegment.SetRange();
                signalLineSegment.SetData(signalXPoints, signalYPoints, SignalLineColor, Period + KPeriod + DPeriod-2 );
                signalLineSegment.SetRange();
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

        /// <summary>
        /// Calculates the percentage K value 
        /// </summary>
        /// <param name="period">Period value</param>
        /// <param name="ypoints">Percentage K points</param>
        private void BasePoints(int period,List<double> ypoints)
        {
            ypoints.Clear();

            List<double> mins = new List<double>();
            List<double> maxs = new List<double>();
            double max;
            double min;
            double top = 0;
            double bottom = 0;
            for (int i = 0; i < period - 1; ++i)
            {
                maxs.Add(0);
                ypoints.Add(0);
                mins.Add(0);
            }

            for (int i = period - 1; i < this.DataCount; ++i)
            {
                min = double.MaxValue;
                max = double.MinValue;
                for (int j = 0; j < period; ++j)
                {
                    min = Math.Min(min, this.LowValues[i - j]);
                    max = Math.Max(max, this.HighValues[i - j]);
                }
                maxs.Add(max);
                mins.Add(min);
            }

            for (int i = period - 1; i < this.DataCount; ++i)
            {

                top = 0;
                bottom = 0;
                top += this.CloseValues[i] - mins[i];
                bottom += maxs[i] - mins[i];
                
                ypoints.Add(top / bottom * 100);
            }
        }

        /// <summary>
        /// Adding points of Upper line and Lower line
        /// </summary>
        /// <param name="period">Period value</param>
        /// <param name="k_period">KPeriod value</param>
        /// <param name="xpoint">Period line X points</param>
        /// <param name="ypoint">Period line Y points</param>
        private void AddPoints(int period, int k_period)
        {
            if (this.DataCount > period + k_period)
            {
                AddPeriodPoints(k_period);
                var xPoints = (from val in xValues select val).ToList();
                upperXPoints.Clear();
                lowerXPoints.Clear();
                upperXPoints.AddRange(xPoints);
                lowerXPoints.AddRange(xPoints);
                periodXPoints = xPoints;
                upperYPoints.Clear();
                lowerYPoints.Clear();
                for (int i = 0; i < this.DataCount; ++i)
                {
                    upperYPoints.Add(80);
                    lowerYPoints.Add(20);
                }
            }
        }

        /// <summary>
        /// Calculates the Signal line points
        /// </summary>
        /// <param name="d_period">DPeriod points</param>
        /// <param name="xPoints">Signal X values</param>
        /// <param name="yPoints">Signal line points</param>
        private void AddSignalPoints(int d_period)
        {
            if (periodYPoints.Count < Period + KPeriod + d_period) return;
            signalXPoints = xValues;
            signalYPoints.Clear();
            for (int i = 0; i < d_period-1; i++)
            {
                signalYPoints.Add(0);
            }

            // To calculate the moving average of particular period in the collection of yPoints
            var mov_avg = Enumerable.Range(0, periodYPoints.Count - (d_period - 1)).Select(n => periodYPoints.Skip(n).Take(d_period).Average()).ToList();

            foreach (var point in mov_avg)
            {
                signalYPoints.Add(point);
            }
        }

        /// <summary>
        /// Calculates the Period line points
        /// </summary>
        /// <param name="k_period">KPeriod value</param>
        /// <param name="yPoints">Period line points </param>
        private void AddPeriodPoints(int k_period)
        {
            periodYPoints.Clear();

            for(int i=0;i<k_period-1;i++)
            {
                periodYPoints.Add(0);
            }

            // To calculate the moving average of particular period in the collection of PercentK
            var mov_avg = Enumerable.Range(0, percentK.Count - (k_period-1)).Select(n => percentK.Skip(n).Take(k_period).Average()).ToList();

            foreach(var point in mov_avg)
            {
                periodYPoints.Add(point);
            }

        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            obj = new StochasticTechnicalIndicator()
            { 
                SignalLineColor = this.SignalLineColor, 
                LowerLineColor = this.LowerLineColor, 
                UpperLineColor = this.UpperLineColor,
                Period=this.Period,
                KPeriod=this.KPeriod,
                DPeriod=this.DPeriod,
            };
            return base.CloneSeries(obj);
        }

#endregion

    }
}
