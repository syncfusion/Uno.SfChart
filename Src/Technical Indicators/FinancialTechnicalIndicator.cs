using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Base class for all the Financial technical indicators available in <see cref="SfChart"/> control.
    /// </summary>
    /// <seealso cref="AccumulationDistributionIndicator"/>
    /// <seealso cref="AverageTrueRangeIndicator"/>
    /// <seealso cref="BollingerBandIndicator"/>
    /// <seealso cref="ExponentialAverageIndicator"/>
    /// <seealso cref="MACDTechnicalIndicator"/>
    /// <seealso cref="MomentumTechnicalIndicator"/>
    /// <seealso cref="RSITechnicalIndicator"/>
    /// <seealso cref="SimpleAverageIndicator"/>
    /// <seealso cref="StochasticTechnicalIndicator"/>
    /// <seealso cref="TriangularAverageIndicator"/>
    public abstract partial class FinancialTechnicalIndicator:ChartSeries, ISupportAxes2D
    {
        /// <summary>
        /// Gets the values of XRange.
        /// </summary>
        public DoubleRange XRange { get; internal set; }

        /// <summary>
        /// Gets the values of YRange.
        /// </summary>
        public DoubleRange YRange { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show/hide indicator value.
        /// </summary>
        public bool ShowTrackballInfo
        {
            get { return (bool)GetValue(ShowTrackballInfoProperty); }
            set { SetValue(ShowTrackballInfoProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="ShowTrackballInfo"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowTrackballInfoProperty =
            DependencyProperty.Register("ShowTrackballInfo", typeof(bool), typeof(FinancialTechnicalIndicator), new PropertyMetadata(false));


        /// <summary>        
        /// Gets or sets the XAxis.
        /// </summary>
        public ChartAxisBase2D XAxis
        {
            get { return (ChartAxisBase2D)GetValue(XAxisProperty); }
            set { SetValue(XAxisProperty, value); }
        }


        /// <summary>
        /// The DependencyProperty for <see cref="XAxis"/> property.
        /// </summary>
        public static readonly DependencyProperty XAxisProperty =
            DependencyProperty.Register("XAxis", typeof(ChartAxisBase2D), typeof(FinancialTechnicalIndicator),
            new PropertyMetadata(null, OnXAxisChanged));

        /// <summary>
        /// Gets or sets the associated YAxis.
        /// </summary>
        public RangeAxisBase YAxis
        {
            get { return (RangeAxisBase)GetValue(YAxisProperty); }
            set { SetValue(YAxisProperty, value); }
        }


        /// <summary>
        /// The DependencyProperty for <see cref="YAxis"/> property.
        /// </summary>
        public static readonly DependencyProperty YAxisProperty =
            DependencyProperty.Register("YAxis", typeof(RangeAxisBase), typeof(FinancialTechnicalIndicator), 
            new PropertyMetadata(null, OnYAxisChanged));


        ChartAxis ISupportAxes.ActualXAxis
        {
            get { return ActualXAxis; }
        }

        ChartAxis ISupportAxes.ActualYAxis
        {
            get { return ActualYAxis; }
        }


        /// <summary>
        /// Gets or sets a value indicating whether to exchange the orientation of the series.
        /// </summary>
        /// <value>
        ///     <c>True</c> exchanges the horizontal axis to vertical and vice versa. 
        ///     <c>False</c> is the default behavior.
        /// </value>
        public bool IsTransposed
        {
            get { return (bool)GetValue(IsTransposedProperty); }
            set { SetValue(IsTransposedProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="IsTransposed"/> property.
        /// </summary>
        public static readonly DependencyProperty IsTransposedProperty =
            DependencyProperty.Register("IsTransposed", typeof(bool), typeof(FinancialTechnicalIndicator), 
            new PropertyMetadata(false, OnTransposeChanged));

        private static void OnTransposeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FinancialTechnicalIndicator).OnTransposeChanged(Convert.ToBoolean(e.NewValue));
        }
        

        public DataTemplate CustomTemplate
        {
            get { return (DataTemplate)GetValue(CustomTemplateProperty); }
            set { SetValue(CustomTemplateProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="CustomTemplate"/> property. 
        /// </summary>
        public static readonly DependencyProperty CustomTemplateProperty =
            DependencyProperty.Register("CustomTemplate", typeof(DataTemplate), typeof(FinancialTechnicalIndicator),
            new PropertyMetadata(null, OnCustomTemplateChanged));

        private static void OnCustomTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as FinancialTechnicalIndicator;

            if (series.Area == null) return;

            series.Segments.Clear();
            series.Area.ScheduleUpdate();
        }

        /// <summary>
        /// Gets or sets the stroke dash array for the line.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.Shapes.StrokeDashArray"/>.
        /// </value>
        public DoubleCollection StrokeDashArray
        {
            get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
            set { SetValue(StrokeDashArrayProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeDashArray"/> property. 
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(FinancialTechnicalIndicator),
            new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the name of the series to which this indicator is associated with.
        /// </summary>
        public string SeriesName
        {
            get { return (string)GetValue(SeriesNameProperty); }
            set { SetValue(SeriesNameProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="SeriesName"/> property.
        /// </summary>
        public static readonly DependencyProperty SeriesNameProperty =
            DependencyProperty.Register("SeriesName", typeof(string), typeof(FinancialTechnicalIndicator),
            new PropertyMetadata(string.Empty));


        /// <summary>
        /// Gets or sets the property path to retrieve high value from ItemsSource.
        /// </summary>
        public string High
        {
            get { return (string)GetValue(HighProperty); }
            set { SetValue(HighProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="High"/> property.
        /// </summary>
        public static readonly DependencyProperty HighProperty =
            DependencyProperty.Register("High", typeof(string), typeof(FinancialTechnicalIndicator),
            new PropertyMetadata(null, OnYPathChanged));

        private static void OnYPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FinancialTechnicalIndicator).OnBindingPathChanged(e);
        }

                
        /// <summary>
        /// Gets or sets the property path to retrieve low value from ItemsSource.
        /// </summary>
        public string Low
        {
            get { return (string)GetValue(LowProperty); }
            set { SetValue(LowProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="Low"/> property.
        /// </summary>
        public static readonly DependencyProperty LowProperty =
            DependencyProperty.Register("Low", typeof(string), typeof(FinancialTechnicalIndicator), 
            new PropertyMetadata(null,OnYPathChanged));


        /// <summary>
        /// Gets or sets the property path to retrieve open value from ItemsSource.
        /// </summary>
        public string Open
        {
            get { return (string)GetValue(OpenProperty); }
            set { SetValue(OpenProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="Open"/> property.
        /// </summary>
        public static readonly DependencyProperty OpenProperty =
            DependencyProperty.Register("Open", typeof(string), typeof(FinancialTechnicalIndicator),
            new PropertyMetadata(null,OnYPathChanged));


        /// <summary>
        /// Gets or sets the property path to retrieve close value from ItemsSource
        /// </summary>
        public string Close
        {
            get { return (string)GetValue(CloseProperty); }
            set { SetValue(CloseProperty, value); }
        }
        

        /// <summary>
        /// The DependencyProperty for <see cref="Close"/> property.
        /// </summary>
        public static readonly DependencyProperty CloseProperty =
            DependencyProperty.Register("Close", typeof(string), typeof(FinancialTechnicalIndicator),
            new PropertyMetadata(null, OnYPathChanged));


        /// <summary>
        /// Gets or sets the property path to retrieve volume data from ItemsSource.
        /// </summary>
        public string Volume
        {
            get { return (string)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="Volume"/> property.
        /// </summary>
        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register("Volume", typeof(string), typeof(FinancialTechnicalIndicator),
            new PropertyMetadata(null, OnYPathChanged));


        /// <summary>
        /// Creates the segments of financial technical indicators.
        /// </summary>
        public override void CreateSegments()
        {
        }

        private static void OnYAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var financialTechnicalIndicator = d as FinancialTechnicalIndicator;
            if (financialTechnicalIndicator != null)
                financialTechnicalIndicator.OnYAxisChanged(e.OldValue as ChartAxis, e.NewValue as ChartAxis);
        }

        private static void OnXAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var financialTechnicalIndicator = d as FinancialTechnicalIndicator;
            if (financialTechnicalIndicator != null)
                financialTechnicalIndicator.OnXAxisChanged(e.OldValue as ChartAxis, e.NewValue as ChartAxis);
        }
        internal virtual void SetIndicatorInfo(ChartPointInfo info, List<double> yValue, bool seriesPalette)
        {
            if (yValue.Count > 0)
              info.SignalLine = double.IsNaN(yValue[0]) ? "null" : Math.Round(yValue[0],2).ToString();          
        }

        /// <summary>
        /// Called when [data source changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            ActualXValues = null;
            base.OnDataSourceChanged(oldValue, newValue);
        }

        /// <summary>
        /// Called when YAxis property changed
        /// </summary>
        /// <param name="oldAxis"></param>
        /// <param name="newAxis"></param>
        protected virtual void OnYAxisChanged(ChartAxis oldAxis, ChartAxis newAxis)
        {
            if (XAxis != null)
            {
                if (Area != null && Area.InternalSecondaryAxis != null && Area.InternalSecondaryAxis.AssociatedAxes.Contains(XAxis))
                {
                    Area.InternalSecondaryAxis.AssociatedAxes.Remove(XAxis);
                    if (XAxis.AssociatedAxes.Contains(Area.InternalSecondaryAxis))
                        XAxis.AssociatedAxes.Remove(Area.InternalSecondaryAxis);

                }
            }
            if (oldAxis != null && oldAxis.RegisteredSeries != null)
            {
                if (oldAxis.RegisteredSeries.Contains(this))
                {
                    oldAxis.RegisteredSeries.Remove(this);
                }

                if (Area != null && oldAxis.RegisteredSeries.Count == 0)
                {
                    if (Area.Axes.Contains(oldAxis))
                        Area.Axes.Remove(oldAxis);
                }
            }
            else if (ActualArea != null && ActualArea.InternalSecondaryAxis != null
                    && ActualArea.InternalSecondaryAxis.RegisteredSeries.Contains(this))
                ActualArea.InternalSecondaryAxis.RegisteredSeries.Remove(this);

            if (newAxis != null && !newAxis.RegisteredSeries.Contains(this))
            {
                if (Area != null && !Area.Axes.Contains(newAxis))
                {
                    Area.Axes.Add(newAxis);
                    newAxis.Area = Area;
                    newAxis.RegisteredSeries.Add(this);
                }
            }

            if (newAxis != null)
                newAxis.Orientation = IsActualTransposed ? Orientation.Horizontal : Orientation.Vertical;
            if (Area != null) Area.ScheduleUpdate();
        }

        /// <summary>
        /// Called when XAxis property changed
        /// </summary>
        /// <param name="oldAxis"></param>
        /// <param name="newAxis"></param>
        protected virtual void OnXAxisChanged(ChartAxis oldAxis, ChartAxis newAxis)
        {
            if (YAxis != null)
            {
                if (Area != null && Area.InternalPrimaryAxis != null && Area.InternalPrimaryAxis.AssociatedAxes.Contains(YAxis))
                {
                    Area.InternalPrimaryAxis.AssociatedAxes.Remove(YAxis);
                    if (YAxis.AssociatedAxes.Contains(Area.InternalPrimaryAxis))
                        YAxis.AssociatedAxes.Remove(Area.InternalPrimaryAxis);
                }
            }

            if (oldAxis != null && oldAxis.RegisteredSeries != null)
            {
                if (oldAxis.RegisteredSeries.Contains(this))
                    oldAxis.RegisteredSeries.Remove(this);

                if (Area != null && oldAxis.RegisteredSeries.Count == 0)
                {
                    if (Area.Axes.Contains(oldAxis))
                        Area.Axes.Remove(oldAxis);
                }
            }
            else if (ActualArea != null && ActualArea.InternalPrimaryAxis != null
                     && ActualArea.InternalPrimaryAxis.RegisteredSeries.Contains(this))
                ActualArea.InternalPrimaryAxis.RegisteredSeries.Remove(this);

            if (newAxis != null)
            {
                if (Area != null && !Area.Axes.Contains(newAxis))
                {
                    Area.Axes.Add(newAxis);
                    newAxis.Area = Area;
                    newAxis.RegisteredSeries.Add(this);
                }
            }
            if (newAxis != null)
                newAxis.Orientation = IsActualTransposed ? Orientation.Vertical : Orientation.Horizontal;
            if (Area != null) Area.ScheduleUpdate();
        }

        /// <summary>
        /// Updates the segment at the specified index
        /// </summary>
        /// <param name="index">The index of the segment.</param>
        /// <param name="action">The action that caused the segments collection changed event</param>
        public override void UpdateSegments(int index, NotifyCollectionChangedAction action)
        {
         
        }

        /// <summary>
        /// Method implementation for Set ItemSource to Series
        /// </summary>
        /// <param name="series"></param>
        protected internal virtual void SetSeriesItemSource(ChartSeriesBase series)
        { 
          
        }
        
        /// <summary>
        /// Method implementation for GeneratePoints for TechnicalIndicator
        /// </summary>
        protected internal override void GeneratePoints()
        {
            throw new NotImplementedException();
        }

        internal static void ComputeMovingAverage(int len, List<double> xValues, IList<double> yValues, List<double> xPoints, List<double> yPoints)
        {
            double sum = 0d;
            if (len <= (yValues.Count - 1))
            {
                xPoints.Clear();
                yPoints.Clear();
                double pad = yValues[len - 1];
                double padDate = xValues[len - 1];
                int limit = xValues.Count;
                for (int i = 0; i < limit; ++i)
                {
                    xPoints.Add(0);
                    yPoints.Add(0);
                    if (i >= len - 1 && i < limit)
                    {
                        if (i - len >= 0)
                        {
                            sum += yValues[i] - yValues[i - len];
                        }
                        else
                        {
                            sum += yValues[i];
                        }
                        xPoints[i] = xValues[i];
                        yPoints[i] = sum / len;
                    }
                    else
                    {
                        if (i < len - 1)
                        {
                            sum += yValues[i];
                        }
                        xPoints[i] = padDate;
                        yPoints[i] = pad;
                    }
                }
            }
        }

        internal static void ComputeMovingAverage(double len, List<double> xValues, IList<double> yValues, List<double> xPoints, List<double> yPoints)
        {
            xPoints.Clear();
            yPoints.Clear();
            double sum = 0d;
            int limit = xValues.Count();
            var length = (int)len;

            for (int i = 0; i < limit; ++i)
            {
                xPoints.Add(0);
                yPoints.Add(0);
                if (i >= length - 1 && i < limit)
                {
                    if (i - len >= 0)
                    {
                        sum += yValues[i] - yValues[i - length];
                    }
                    else
                    {
                        sum += yValues[i];
                    }
                    xPoints[i] = (xValues[i]);
                    yPoints[i] = (sum / len);

                }
                else
                {
                    if (i < len - 1)
                        sum += yValues[i];
                }
            }
        }

        internal override void UpdateRange()
        {
            XRange = DoubleRange.Empty;
            YRange = DoubleRange.Empty;

            foreach (ChartSegment segment in Segments)
            {
                XRange += segment.XRange;
                YRange += segment.YRange;
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            FinancialTechnicalIndicator indicator = obj as FinancialTechnicalIndicator;
            if (this.XAxis != null && this.XAxis != Area.InternalPrimaryAxis)
                indicator.XAxis = (ChartAxisBase2D)this.XAxis.Clone();
            if (this.YAxis != null && this.YAxis != Area.InternalSecondaryAxis)
                indicator.YAxis = (RangeAxisBase)this.YAxis.Clone();
            indicator.High = this.High;
            indicator.Low = this.Low;
            indicator.Open = this.Open;
            indicator.Close = this.Close;
            indicator.Volume = this.Volume;
            return base.CloneSeries(indicator);
        }

    }
}
