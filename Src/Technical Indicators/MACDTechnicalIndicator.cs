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
    /// Represents MACD technical indicator.
    /// </summary>
    /// <seealso cref="FinancialTechnicalIndicator"/>
    /// <seealso cref="TechnicalIndicatorSegment"/>
    public partial class MACDTechnicalIndicator : FinancialTechnicalIndicator
    {
        #region ctor

        public MACDTechnicalIndicator()
        {
#if UNIVERSALWINDOWS
            ConvergenceLineColor = new SolidColorBrush(Colors.Red);
            DivergenceLineColor = new SolidColorBrush(Colors.Red);
            HistogramColor = default(Brush);
            SignalLineColor = new SolidColorBrush(Colors.Blue);
#endif
        }

#endregion

        #region fields

        IList<double> CloseValues = new List<double>();

        List<double> xValues;

        List<double> FastEMA = new List<double>();

        List<double> SlowEMA = new List<double>();

        List<double> MACDPoints = new List<double>();

        List<double> SignalPoints = new List<double>();

        List<double> xPoints = new List<double>();

        List<double> HistogramYPoints = new List<double>();

        List<double> CenterYPoints = new List<double>();

        IList<double> x1Values, x2Values, y1Values, y2Values;

        TechnicalIndicatorSegment MACDLineSegment;

        TechnicalIndicatorSegment SignalLineSegment;

        TechnicalIndicatorSegment CenterlLineSegment;

        FastColumnBitmapSegment HistogramSegment; 

        #endregion

        #region properties        

        /// <summary>
        /// Gets a value indicating whether this series is placed side by side.
        /// </summary>
        /// <returns>
        /// It returns <c>true</c>, if the series is placed side by side [cluster mode].
        /// </returns>
        protected internal override bool IsSideBySide
        {
            get
            {
                return Type == MACDType.Histogram || Type == MACDType.Both ? true : false;
            }
        }

        /// <summary>
        /// Gets or sets the type of MACD indicator.
        /// </summary>
        /// <value>
        /// <see cref="Syncfusion.UI.Xaml.Charts.MACDType"/>
        /// </value>
        public MACDType Type
        {
            get { return (MACDType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="Type"/> property.
        /// </summary>
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(MACDType), typeof(MACDTechnicalIndicator),
            new PropertyMetadata(MACDType.Line, OnValueChanged));
        

        /// <summary>
        /// Gets or sets the short time period(no of days) for calculating EMA.
        /// </summary>
        /// <remarks>
        /// By default, its value is 12 days.
        /// </remarks>
        public int ShortPeriod
        {
            get { return (int)GetValue(ShortPeriodProperty); }
            set { SetValue(ShortPeriodProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="ShortPeriod"/> property.
        /// </summary>
        public static readonly DependencyProperty ShortPeriodProperty =
            DependencyProperty.Register("ShortPeriod", typeof(int), typeof(MACDTechnicalIndicator), 
            new PropertyMetadata(12, OnValueChanged));


        /// <summary>
        /// Gets or sets the long time period(no of days) for calculating EMA.
        /// </summary>
        /// <remarks>
        /// By default, its value is 26 days.
        /// </remarks>
        public int LongPeriod
        {
            get { return (int)GetValue(LongPeriodProperty); }
            set { SetValue(LongPeriodProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="LongPeriod"/> property.
        /// </summary>
        public static readonly DependencyProperty LongPeriodProperty =
            DependencyProperty.Register("LongPeriod", typeof(int), typeof(MACDTechnicalIndicator),
            new PropertyMetadata(26, OnValueChanged));
        

        /// <summary>
        /// Gets or sets the moving average period for MACD.
        /// </summary>
        public int Period
        {
            get { return (int)GetValue(PeriodProperty); }
            set { SetValue(PeriodProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="Period"/> property.
        /// </summary>
        public static readonly DependencyProperty PeriodProperty =
            DependencyProperty.Register("Period", typeof(int), typeof(MACDTechnicalIndicator),
            new PropertyMetadata(9, OnValueChanged));
        

        /// <summary>
        /// Gets or sets the convergence line color
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush ConvergenceLineColor
        {
            get { return (Brush)GetValue(ConvergenceLineColorProperty); }
            set { SetValue(ConvergenceLineColorProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="ConvergenceLineColor"/> property.
        /// </summary>
        public static readonly DependencyProperty ConvergenceLineColorProperty =
            DependencyProperty.Register("ConvergenceLineColor", typeof(Brush), typeof(MACDTechnicalIndicator),
#if UNIVERSALWINDOWS
                new PropertyMetadata(null, OnColorChanged));
#else
                new PropertyMetadata(new SolidColorBrush(Colors.Red), OnColorChanged));
#endif


        /// <summary>
        /// Gets or sets the divergence line color.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>      
        public Brush DivergenceLineColor
        {
            get { return (Brush)GetValue(DivergenceLineColorProperty); }
            set { SetValue(DivergenceLineColorProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref=" DivergenceLineColor"/> property.
        /// </summary>
        public static readonly DependencyProperty DivergenceLineColorProperty =
            DependencyProperty.Register("DivergenceLineColor", typeof(Brush), typeof(MACDTechnicalIndicator),
#if UNIVERSALWINDOWS
                new PropertyMetadata(null, OnColorChanged));
#else
                new PropertyMetadata(new SolidColorBrush(Colors.Red), OnColorChanged));
#endif



        /// <summary>
        /// Gets or sets the histogram interior color.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush HistogramColor
        {
            get { return (Brush) GetValue(HistogramColorProperty); }
            set { SetValue(HistogramColorProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="HistogramColor"/> property.
        /// </summary>
        public static readonly DependencyProperty HistogramColorProperty = DependencyProperty.Register(
           "HistogramColor", typeof(Brush), typeof(MACDTechnicalIndicator),
#if UNIVERSALWINDOWS
           new PropertyMetadata(null, OnColorChanged));
#else
           new PropertyMetadata(default(Brush), OnColorChanged));
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
        /// The DependencyProperty for <see cref=" SignalLineColor"/> property.
        /// </summary>
        public static readonly DependencyProperty SignalLineColorProperty =
            DependencyProperty.Register("SignalLineColor", typeof(Brush), typeof(MACDTechnicalIndicator),
#if UNIVERSALWINDOWS
            new PropertyMetadata(null, OnColorChanged));
#else
            new PropertyMetadata(new SolidColorBrush(Colors.Blue),OnColorChanged));
#endif


        #endregion

        #region Methods


        internal override void SetIndicatorInfo(ChartPointInfo info, List<double> yValue, bool seriesPalette)
        {
            if (Type == MACDType.Both)
            {
                if (yValue.Count > 0)
                {
                    info.UpperLine = double.IsNaN(yValue[0])? "null" : Math.Round(yValue[0],2).ToString();
                    info.LowerLine = double.IsNaN(yValue[1]) ? "null" : Math.Round(yValue[1],2).ToString();
                    info.SignalLine = double.IsNaN(yValue[2]) ? "null" : Math.Round(yValue[2],2).ToString();
                }
            }
            if (Type == MACDType.Line)
            {
                if (yValue.Count > 0)
                {
                    info.LowerLine = double.IsNaN(yValue[0]) ? "null" : Math.Round(yValue[0],2).ToString();
                    info.SignalLine = double.IsNaN(yValue[1]) ? "null" : Math.Round(yValue[1],2).ToString();
                }
            }
            if (Type == MACDType.Histogram)
            {
                if (yValue.Count > 0)
                    info.UpperLine = double.IsNaN(yValue[0]) ? "null" : Math.Round(yValue[0],2).ToString();
            }
        }
        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MACDTechnicalIndicator indicator = d as MACDTechnicalIndicator;
            indicator.UpdateArea();
        }


        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MACDTechnicalIndicator indicator = d as MACDTechnicalIndicator;
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
            MACDLineSegment = null;
            CenterlLineSegment = null;
            HistogramSegment = null;
            CloseValues.Clear();
            GeneratePoints(new string[] { Close }, CloseValues);
            this.UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            CloseValues.Clear();
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
                this.CloseValues = Clone(series.ActualSeriesYValues[0]);
                this.Area.ScheduleUpdate();
            }
        }

        /// <summary>
        /// Method implementation for GeneratePoints for TechnicalIndicator
        /// </summary>
        protected internal override void GeneratePoints()
        {
            GeneratePoints(new string[] { Close }, CloseValues);
        }

        /// <summary>
        /// Creates the segments of MACDTechnicalIndicator.
        /// </summary>
      
        public override void CreateSegments()
        {
            if (Period + LongPeriod >= DataCount || Period < 1 || ShortPeriod >= DataCount)
            {
                Segments.Clear();
                return;
            }
            xValues = GetXValues();
			if (Period < 1) return;
            AddMACDPoints();

            if (!Segments.Contains(MACDLineSegment) || !Segments.Contains(SignalLineSegment)|| !Segments.Contains(CenterlLineSegment) || !Segments.Contains(HistogramSegment)||Segments.Count==0)
            {
                Segments.Clear();
                if (Type == MACDType.Histogram || Type == MACDType.Both)
                {
                    CalculateHistogram();
                    HistogramSegment = new FastColumnBitmapSegment(x1Values, y1Values, x2Values, y2Values, this);
                    HistogramSegment.SetData(x1Values, y1Values, x2Values, y2Values);
                    this.Interior = HistogramColor;
                    Segments.Add(HistogramSegment);
                }
                if (Type == MACDType.Line || Type == MACDType.Both)
                {
                    MACDLineSegment = new TechnicalIndicatorSegment(xPoints, MACDPoints, ConvergenceLineColor, this, LongPeriod);
                    MACDLineSegment.SetValues(xPoints, MACDPoints, ConvergenceLineColor, this, LongPeriod);
                    Segments.Add(MACDLineSegment);

                    SignalLineSegment = new TechnicalIndicatorSegment(xPoints, SignalPoints, DivergenceLineColor, this, (LongPeriod + Period - 1));
                    SignalLineSegment.SetValues(xPoints, SignalPoints, DivergenceLineColor, this, (LongPeriod + Period - 1));
                    Segments.Add(SignalLineSegment);

                    CenterlLineSegment = new TechnicalIndicatorSegment(xPoints, CenterYPoints, SignalLineColor, this);
                    CenterlLineSegment.SetValues(xPoints, CenterYPoints, SignalLineColor, this);
                    Segments.Add(CenterlLineSegment);
                }
            }
            else
            {
                if (Type == MACDType.Both)
                {
                    HistogramSegment.SetData(x1Values, y1Values, x2Values, y2Values);
                    MACDLineSegment.SetData(xPoints, MACDPoints, ConvergenceLineColor);
                    MACDLineSegment.SetRange();
                    SignalLineSegment.SetData(xPoints, SignalPoints, DivergenceLineColor);
                    SignalLineSegment.SetRange();
                    CenterlLineSegment.SetData(xPoints, CenterYPoints, SignalLineColor);
                    CenterlLineSegment.SetRange();
                }
                else if (Type == MACDType.Line)
                {
                    Segments.Remove(HistogramSegment);
                    HistogramSegment = null;
                    MACDLineSegment.SetData(xPoints, MACDPoints, ConvergenceLineColor);
                    MACDLineSegment.SetRange();
                    SignalLineSegment.SetData(xPoints, SignalPoints, DivergenceLineColor);
                    SignalLineSegment.SetRange();
                    CenterlLineSegment.SetData(xPoints, CenterYPoints, SignalLineColor);
                    CenterlLineSegment.SetRange();
                }
                else if (Type == MACDType.Histogram)
                {
                    Segments.Remove(MACDLineSegment);
                    Segments.Remove(SignalLineSegment);
                    Segments.Remove(CenterlLineSegment);
                    MACDLineSegment = null;
                    SignalLineSegment = null;
                    CenterlLineSegment = null;
                    HistogramSegment.SetData(x1Values, y1Values, x2Values, y2Values);
                }
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
        /// Method implementation  for Add MACD property values
        /// </summary>
        public void AddMACDPoints()
        {
            MACDPoints.Clear();
            SignalPoints.Clear();
            CenterYPoints.Clear();
            xPoints.Clear();
            FastEMA = CalculateEMA(ShortPeriod);
            SlowEMA = CalculateEMA(LongPeriod);
            for (int i = 0; i < DataCount; i++)
            {
                xPoints.Add(xValues[i]);
                CenterYPoints.Add(0);
                MACDPoints.Add(double.IsNaN(FastEMA[i] - SlowEMA[i]) ? 0 : (FastEMA[i] - SlowEMA[i]));
            }
            double alpha = 2 / (Period + 1d);
            double signalValue = 0,count=0;
            int index = LongPeriod - 1;
            do
            {
                signalValue += MACDPoints[index];
                index++;
                count++;
            } while (Period != count);
            signalValue /= Period;
            for(int i=0;i<LongPeriod+Period-2;i++)
            {
                SignalPoints.Add(0);
            }
            SignalPoints.Add (signalValue);
            for(int i=LongPeriod+Period-1;i<DataCount;i++)
            {
                SignalPoints.Add((MACDPoints[i] * alpha) + (SignalPoints[i - 1] * (1 - alpha)));
            }
        }

        private List<double> CalculateEMA(int length)
        {
            double alpha = 2 / (length + 1d);
            double firstEMA = 0;
            List<double> EMAPoints = new List<double>();
            for (int i = 0; i < length; i++)
            {
                EMAPoints.Add(double.NaN);
                firstEMA += CloseValues[i];
            }
            firstEMA /= length;
            EMAPoints[length - 1] = firstEMA;
            for (int i = length; i < DataCount; i++)
            {
                EMAPoints.Add((CloseValues[i] * alpha) + (EMAPoints[i - 1] * (1 - alpha)));
            }
            return EMAPoints;
        }

        private void CalculateHistogram()
        {
            x1Values = new List<double>();
            x2Values = new List<double>();
            y1Values = new List<double>();
            y2Values = new List<double>();
            this.Area.SBSInfoCalculated = false;
            DoubleRange sbsInfo = this.GetSideBySideInfo(this);
            for (int j = 0; j < DataCount; j++)
            {
                HistogramYPoints.Add(MACDPoints[j] - SignalPoints[j]);
                if (!this.IsIndexed)
                {
                    x1Values.Add(xValues[j] + sbsInfo.Start);
                    x2Values.Add(xValues[j] + sbsInfo.End);
                    y1Values.Add(HistogramYPoints[j]);
                    y2Values.Add(0);
                }
                else
                {
                    x1Values.Add(j + sbsInfo.Start);
                    x2Values.Add(j + sbsInfo.End);
                    y1Values.Add(HistogramYPoints[j]);
                    y2Values.Add(0);
                }
            }
        }

        private static void OnAverageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MACDTechnicalIndicator indicator = d as MACDTechnicalIndicator;
            indicator.Invalidate();
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            obj = new MACDTechnicalIndicator() 
            { 
                Period = this.Period, 
                Type=this.Type,
                ShortPeriod=this.ShortPeriod,
                LongPeriod=this.LongPeriod,
                SignalLineColor = this.SignalLineColor,
                ConvergenceLineColor = this.ConvergenceLineColor, 
                DivergenceLineColor = this.DivergenceLineColor };
            return base.CloneSeries(obj);
        }

#endregion

    }
}
