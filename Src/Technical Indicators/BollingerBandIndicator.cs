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
    /// Represents BollingerBand technical indicator.
    /// </summary>
    /// <seealso cref="FinancialTechnicalIndicator"/>
    /// <seealso cref="TechnicalIndicatorSegment"/>
   
    public partial class BollingerBandIndicator:FinancialTechnicalIndicator
    {
        #region Constructor

        public BollingerBandIndicator()
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

        List<double> upperXPoints = new List<double>();

        List<double> upperYPoints = new List<double>();

        List<double> lowerXPoints = new List<double>();

        List<double> lowerYPoints = new List<double>();

        List<double> signalXPoints = new List<double>();

        List<double> signalYPoints = new List<double>();

        TechnicalIndicatorSegment upperLineSegment;

        TechnicalIndicatorSegment lowerLineSegment;

        TechnicalIndicatorSegment signalLineSegment;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the moving average value for the indicator.
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
            DependencyProperty.Register("Period", typeof(int), typeof(BollingerBandIndicator),
            new PropertyMetadata(20, OnMovingAverageChanged));


        /// <summary>
        /// Gets or sets the fill color for the Upper Line of the BollingerBandIndicator.
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
            DependencyProperty.Register("UpperLineColor", typeof(Brush), typeof(BollingerBandIndicator),
#if UNIVERSALWINDOWS
            new PropertyMetadata(null, OnColorChanged));
#else
            new PropertyMetadata(new SolidColorBrush(Colors.Red), OnColorChanged));
#endif


        /// <summary>
        /// Gets or sets the fill color for the Lower Line of BollingerBand.
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
            DependencyProperty.Register("LowerLineColor", typeof(Brush), typeof(BollingerBandIndicator),
#if UNIVERSALWINDOWS
            new PropertyMetadata(null, OnColorChanged));
#else
            new PropertyMetadata(new SolidColorBrush(Colors.Red), OnColorChanged));
#endif


        /// <summary>
        /// Gets or sets the fill color for the Signal Line.
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
            DependencyProperty.Register("SignalLineColor", typeof(Brush), typeof(BollingerBandIndicator),
#if UNIVERSALWINDOWS
            new PropertyMetadata(null,OnColorChanged));
#else
            new PropertyMetadata(new SolidColorBrush(Colors.Blue),OnColorChanged));            
#endif

        #endregion

        #region Methods

        internal override void SetIndicatorInfo(ChartPointInfo info, List<double> yValue, bool seriesPalette)
        {
            if (yValue.Count > 0)
            {
                info.UpperLine = double.IsNaN(yValue[0])? "null": Math.Round(yValue[0], 2).ToString();
                info.LowerLine = double.IsNaN(yValue[1]) ? "null" : Math.Round(yValue[1], 2).ToString();
                info.SignalLine = double.IsNaN(yValue[2]) ? "null" : Math.Round(yValue[2], 2).ToString();
            }
        }           

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BollingerBandIndicator indicator = d as BollingerBandIndicator;
            indicator.UpdateArea();
        }

        private static void OnMovingAverageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BollingerBandIndicator indicator = d as BollingerBandIndicator;
            indicator.UpdateArea();
        }
        /// <summary>
        /// Called when DataSource property changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnDataSourceChanged(oldValue, newValue);
            signalLineSegment = null;
            upperLineSegment = null;
            lowerLineSegment = null;
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
        /// Creates the segments of BollingerBandIndicator.
        /// </summary>
        public override void CreateSegments()
        {
            xValues = GetXValues();
			if (Period < 1) return;
            Period = Period < xValues.Count ? Period : xValues.Count - 1;
            AddBollinger();
            List<double> tmpUpXPoints = new List<double>();
            List<double> tmpUpYPoints = new List<double>();

            List<double> tmpLwXPoints = new List<double>();
            List<double> tmpLwYPoints = new List<double>();

            List<double> tmpSgXPoints = new List<double>();
            List<double> tmpSgYPoints = new List<double>();

            for (int i = 0; i < upperXPoints.Count; i++)
            {
                tmpUpXPoints.Add(upperXPoints[i]);
                tmpUpYPoints.Add(upperYPoints[i]);
                tmpLwXPoints.Add(lowerXPoints[i]);
                tmpLwYPoints.Add(lowerYPoints[i]);
                tmpSgXPoints.Add(signalXPoints[i]);
                tmpSgYPoints.Add(signalYPoints[i]);
            }
            if (upperLineSegment == null || lowerLineSegment == null || signalLineSegment == null)
            {
                upperLineSegment = new TechnicalIndicatorSegment(tmpUpXPoints, tmpUpYPoints, UpperLineColor, this,Period);
                upperLineSegment.SetValues(tmpUpXPoints, tmpUpYPoints, UpperLineColor, this, Period);
                Segments.Add(upperLineSegment);
                lowerLineSegment = new TechnicalIndicatorSegment(tmpLwXPoints, tmpLwYPoints, LowerLineColor, this,Period);
                lowerLineSegment.SetValues(tmpLwXPoints, tmpLwYPoints, LowerLineColor, this, Period);
                Segments.Add(lowerLineSegment);
                signalLineSegment = new TechnicalIndicatorSegment(tmpSgXPoints, tmpSgYPoints, SignalLineColor, this,Period);
                signalLineSegment.SetValues(tmpSgXPoints, tmpSgYPoints, SignalLineColor, this, Period);
                Segments.Add(signalLineSegment);
            }
            else
            {
                upperLineSegment.SetData(tmpUpXPoints, tmpUpYPoints, UpperLineColor,Period);
                upperLineSegment.SetRange();
                lowerLineSegment.SetData(tmpLwXPoints, tmpLwYPoints, LowerLineColor,Period);
                lowerLineSegment.SetRange();
                signalLineSegment.SetData(tmpSgXPoints, tmpSgYPoints, SignalLineColor, Period);
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

        private void AddBollinger()
        {
            signalXPoints.Clear();
            signalYPoints.Clear();
            upperXPoints.Clear();
            upperYPoints.Clear();
            lowerXPoints.Clear();
            lowerYPoints.Clear();
            int parmInt = 0;

            object[] parms = new object[] { Period, 2.0d };
            parmInt = Period;
            double bandWidth = GetDouble(1, parms, 2d);

            ComputeSMAandBollingerBands(parmInt, bandWidth);
        }

        private static int GetInt(int i, object[] parms, int def)
        {
            int val = def;
            if (parms != null && parms.GetLength(0) > i && parms[i] != null)
            {
                int.TryParse(parms[i].ToString(), out val);
            }
            return val;
        }

        private static double GetDouble(int i, object[] parms, double def)
        {
            double val = def;
            if (parms != null && parms.GetLength(0) > i && parms[i] != null)
            {
                double.TryParse(parms[i].ToString(), out val);
            }
            return val;
        }

        private void ComputeSMAandBollingerBands(int len, double bandWidth)
        {
            double sum = 0d;
            double sma = 0d;
            double[] deviations = new double[DataCount];
            double deviationSum = 0d;
            double std = 0d;
            double xPoint = 0d;
            double yPoint = 0d;

            for (int i = 0; i < len; i++)
                sum += CloseValues[i];

            sma = sum / len;

            for (int i = 0; i < DataCount; ++i)
            {          
                if (i >= len - 1)
                {                   
                    if (i - len >= 0)
                    {
                        var diff = CloseValues[i] - CloseValues[i - len];
                        sum += diff;
                        sma = sum / len;

                        xPoint = xValues[i];
                        yPoint = sma;

                        deviations[i] = Math.Pow(CloseValues[i] - sma, 2);
                        deviationSum += deviations[i] - deviations[i - len];
                        std = Math.Sqrt(deviationSum / len);
                    }
                    else
                    {
                        deviations[i] = Math.Pow(CloseValues[i] - sma, 2);
                        deviationSum += deviations[i];
                        std = Math.Sqrt(deviationSum / len);
                              
                        // Construct the starting points.                 
                        for (int k = 0; k < len - 1; k++)
                        {                         
                            xPoint = xValues[i];
                            yPoint = sma;

                            signalXPoints.Add(xPoint);
                            signalYPoints.Add(yPoint);

                            upperXPoints.Add(xPoint);
                            lowerXPoints.Add(xPoint);
                            upperYPoints.Add(signalYPoints[k] + (bandWidth * std));
                            lowerYPoints.Add(signalYPoints[k] - (bandWidth * std));
                        }

                        xPoint = xValues[i];
                        yPoint = sma;
                    }

                    signalXPoints.Add(xPoint);
                    signalYPoints.Add(yPoint);
                    upperXPoints.Add(xPoint);
                    lowerXPoints.Add(xPoint);
                    upperYPoints.Add(yPoint + (bandWidth * std));
                    lowerYPoints.Add(yPoint - (bandWidth * std));
                }
                else
                {
                    deviations[i] = Math.Pow(CloseValues[i] - sma, 2);
                    deviationSum += deviations[i];                  
                }
            }         
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            obj = new BollingerBandIndicator() { Period = this.Period, SignalLineColor = this.SignalLineColor, LowerLineColor = this.LowerLineColor, UpperLineColor = this.UpperLineColor };
            return base.CloneSeries(obj);
        }

#endregion

    }
}
