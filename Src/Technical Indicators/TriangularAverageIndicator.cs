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
    /// Represents TriangularAverage technical indicator.
    /// </summary>
    /// <seealso cref="FinancialTechnicalIndicator"/>
    /// <seealso cref="TechnicalIndicatorSegment"/>
   
    public partial class TriangularAverageIndicator : FinancialTechnicalIndicator
    {
        #region constructor

        public TriangularAverageIndicator()
        {
#if UNIVERSALWINDOWS
            SignalLineColor = new SolidColorBrush(Colors.Green);
#endif
        }

        #endregion

        #region fields

        IList<double> YValues = new List<double>();

        List<double> xValues;

        List<double> xPoints = new List<double>();

        List<double> yPoints = new List<double>();

        TechnicalIndicatorSegment fastLineSegment;

#endregion
        
        #region properties

        /// <summary>
        /// Gets or sets the triangular average period.
        /// </summary>
        /// <remarks>
        /// The default value is 2 days.
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
            DependencyProperty.Register("Period", typeof(int), typeof(TriangularAverageIndicator), 
            new PropertyMetadata(2, OnMovingAverageChanged));


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
            DependencyProperty.Register("SignalLineColor", typeof(Brush), typeof(TriangularAverageIndicator),
#if UNIVERSALWINDOWS
                new PropertyMetadata(null, OnColorChanged));
#else
                new PropertyMetadata(new SolidColorBrush(Colors.Green),OnColorChanged));
#endif

#endregion
        
        #region Methods

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TriangularAverageIndicator indicator = d as TriangularAverageIndicator;
            indicator.UpdateArea();
        }

        private static void OnMovingAverageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TriangularAverageIndicator indicator = d as TriangularAverageIndicator;
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
            fastLineSegment = null;
            YValues.Clear();
            GeneratePoints(new string[] { Close }, YValues);
            this.UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
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
        /// Creates the segments of TriangularAverageIndicator.
        /// </summary>
        public override void CreateSegments()
        {
            xValues = GetXValues();
            if (Period < 1) return;
            Period = Period < xValues.Count ? Period : xValues.Count - 1;
            double average = Period;   
            if (average < this.DataCount)
            {
                this.AddTriangularPoints((int)average);
            }

            if (fastLineSegment == null)
            {
                fastLineSegment = new TechnicalIndicatorSegment(xPoints, yPoints, SignalLineColor, this);
                fastLineSegment.SetValues(xPoints, yPoints, SignalLineColor, this, Period);
                Segments.Add(fastLineSegment);
            }
            else if (ActualXValues != null)
            {
                fastLineSegment.SetData(xPoints, yPoints,SignalLineColor,Period);
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

        private void AddTriangularPoints(int avg)
        {
            ComputeMovingAverage(avg, xValues, YValues, xPoints, yPoints);

            double sum = 0d;
            var smaYValues = new List<double>(yPoints);

            for (int i = 0; i < avg - 1; i++)
            {
                sum = 0;
                for (int j = 0; j < i + 1; j++)
                {
                    sum += YValues[j];
                }
                sum = sum / (i + 1);
                smaYValues[i] = sum;
            }

            ComputeMovingAverage(avg, xValues, smaYValues, xPoints, yPoints);
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            obj = new TriangularAverageIndicator() { SignalLineColor = this.SignalLineColor, Period = this.Period };
            return base.CloneSeries(obj);
        }

#endregion

    }
}
