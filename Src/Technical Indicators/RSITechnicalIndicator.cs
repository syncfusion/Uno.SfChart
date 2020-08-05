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
    /// Represents RelativeStrengthIndex technical indicator.
    /// </summary>
    /// <seealso cref="FinancialTechnicalIndicator"/>
    /// <seealso cref="TechnicalIndicatorSegment"/>
   
    public partial class RSITechnicalIndicator:FinancialTechnicalIndicator
    {
        #region constructor  

        public RSITechnicalIndicator()
        {
#if UNIVERSALWINDOWS
            UpperLineColor = new SolidColorBrush(Colors.Red);
            LowerLineColor = new SolidColorBrush(Colors.Red);
            SignalLineColor = new SolidColorBrush(Colors.Blue);
#endif
        }

        #endregion

        #region fields

        IList<double> CloseValues = new List<double>();

        List<double> xValues;

        List<double> xPoints = new List<double>();

        List<double> yPoints = new List<double>();

        List<double> upperXPoints = new List<double>();

        List<double> upperYPoints = new List<double>();

        List<double> lowerXPoints = new List<double>();

        List<double> lowerYPoints = new List<double>();

        TechnicalIndicatorSegment upperLineSegment;

        TechnicalIndicatorSegment lowerLineSegment;

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
            DependencyProperty.Register("Period", typeof(int), typeof(RSITechnicalIndicator),
            new PropertyMetadata(14, OnMovingAverageChanged));
        

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
            DependencyProperty.Register("UpperLineColor", typeof(Brush), typeof(RSITechnicalIndicator),
#if UNIVERSALWINDOWS
            new PropertyMetadata(null, OnColorChanged));
#else
            new PropertyMetadata(new SolidColorBrush(Colors.Red), OnColorChanged));
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
            DependencyProperty.Register("LowerLineColor", typeof(Brush), typeof(RSITechnicalIndicator),
# if UNIVERSALWINDOWS
            new PropertyMetadata(null, OnColorChanged));
#else
            new PropertyMetadata(new SolidColorBrush(Colors.Red), OnColorChanged));
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
            DependencyProperty.Register("SignalLineColor", typeof(Brush), typeof(RSITechnicalIndicator),
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
                info.SignalLine = double.IsNaN(yValue[2])? "null" : Math.Round(yValue[2], 2).ToString();
        }

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RSITechnicalIndicator indicator = d as RSITechnicalIndicator;
            if (indicator != null) indicator.UpdateArea();
        }

        private static void OnMovingAverageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RSITechnicalIndicator indicator = d as RSITechnicalIndicator;
            if (indicator != null) indicator.UpdateArea();
        }

        /// <summary>
        /// Called when DataSource changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnDataSourceChanged(oldValue, newValue);
            upperLineSegment = null;
            lowerLineSegment = null;
            signalLineSegment = null;
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
        /// Creates the segments of RelativeStrengthIndexIndicator.
        /// </summary>
        public override void CreateSegments()
        {
            if (DataCount <= Period || Period < 1)
            {
                Segments.Clear();
                return;
            }
             xValues = GetXValues();
             ComputeRSI(Period);
             var tempxPoints = (from val in xValues select val).ToList();
             upperXPoints.Clear();
             lowerXPoints.Clear();
             upperXPoints.AddRange(tempxPoints);
             lowerXPoints.AddRange(tempxPoints);
             upperYPoints.Clear();
             lowerYPoints.Clear();

             for (int i = 0; i < this.DataCount; ++i)
             {
                 upperYPoints.Add(70);
                 lowerYPoints.Add(30);
             }

             if (upperLineSegment == null || lowerLineSegment == null || signalLineSegment == null || Segments.Count == 0)
             {
                 Segments.Clear();
                 upperLineSegment = new TechnicalIndicatorSegment(upperXPoints, upperYPoints,UpperLineColor, this);
                upperLineSegment.SetValues(upperXPoints, upperYPoints, UpperLineColor, this);
                 Segments.Add(upperLineSegment);

                lowerLineSegment = new TechnicalIndicatorSegment(lowerXPoints, lowerYPoints,LowerLineColor, this);
                lowerLineSegment.SetValues(lowerXPoints, lowerYPoints, LowerLineColor, this);
                 Segments.Add(lowerLineSegment);

                signalLineSegment = new TechnicalIndicatorSegment(xPoints, yPoints, SignalLineColor, this, Period + 1);
                signalLineSegment.SetValues(xPoints, yPoints, SignalLineColor, this, Period + 1);
                 Segments.Add(signalLineSegment);
             }
             else
             {
                 upperLineSegment.SetData(upperXPoints, upperYPoints, UpperLineColor);
                 upperLineSegment.SetRange();
                 lowerLineSegment.SetData(lowerXPoints, lowerYPoints, LowerLineColor);
                 lowerLineSegment.SetRange();
                 signalLineSegment.SetData(xPoints, yPoints, SignalLineColor, Period + 1);
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

        private void ComputeRSI(int period)
        {
            xPoints.Clear();
            yPoints.Clear();
            double c2 = 0, c1 = 0;
            double pmf = 0d;
            double nmf = 0d;
            c1 = CloseValues[0];
            for (int i = 1; i <= period; ++i)
            {
                c2 = CloseValues[i];                
                if (c2 > c1)
                    pmf += (c2 - c1);
                else if (c2 < c1)
                    nmf += (c1 - c2);
                c1 = c2;
                yPoints.Add(0);
            }
            double avg_gain = 0, avg_loss = 0;
            avg_gain = pmf / period;
            avg_loss = nmf / period;
            double rs = 0, smoothed_rs = 0;
            rs = avg_gain / avg_loss;
            yPoints.Add(100 - 100 / (1 + rs));
            for (int i = period+1; i < DataCount; ++i)
            {
                c2 = CloseValues[i];
                if (c2 > c1)
                {
                    avg_gain = (avg_gain * (period - 1) + (c2 - c1)) / period;
                    avg_loss = (avg_loss * (period - 1)) / period;
                }
                else if (c2 < c1)
                {
                    avg_loss = (avg_loss * (period - 1) + (c1 - c2)) / period;
                    avg_gain = (avg_gain * (period - 1)) / period;
                }
                c1 = c2;
                smoothed_rs = avg_gain / avg_loss;
                yPoints.Add(100 - 100 / (1 + smoothed_rs));
            }
            xPoints = (from val in xValues select val).ToList();
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            obj = new RSITechnicalIndicator() 
            {
                LowerLineColor = this.LowerLineColor, 
                UpperLineColor = this.UpperLineColor, 
                SignalLineColor = this.SignalLineColor,
                Period = this.Period 
            };
            return base.CloneSeries(obj);
        }

#endregion
    }
}
