using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using System.Collections;
using Windows.Foundation;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for RangeSeriesBase
    /// </summary>
    public abstract partial class RangeSeriesBase : CartesianSeries
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="High"/> property.       
        /// </summary>
        public static readonly DependencyProperty HighProperty =
            DependencyProperty.Register(
                "High", 
                typeof(string), 
                typeof(RangeSeriesBase),
                new PropertyMetadata(null, OnYPathChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Low"/> property.       
        /// </summary>
        public static readonly DependencyProperty LowProperty =
            DependencyProperty.Register(
                "Low",
                typeof(string),
                typeof(RangeSeriesBase),
                new PropertyMetadata(null, OnYPathChanged));

        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for RangeSeriesBase
        /// </summary>
        public RangeSeriesBase()
        {
            HighValues = new List<double>();
            LowValues = new List<double>();
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the property path to be bound with high value of HiLo series to render it. This is a bindable property.
        /// </summary>
        public string High
        {
            get { return (string)GetValue(HighProperty); }
            set { SetValue(HighProperty, value); }
        }

        /// <summary>
        /// Gets or sets the property path to be bind with low value of HiLo series.
        /// </summary>
        public string Low
        {
            get { return (string)GetValue(LowProperty); }
            set { SetValue(LowProperty, value); }
        }

        #endregion

        #region Protected Internal Properties

        /// <summary>
        /// Gets or sets HighValues property
        /// </summary>
        protected internal IList<double> HighValues { get; set; }

        /// <summary>
        /// Gets or sets LowValues property
        /// </summary>
        protected internal IList<double> LowValues { get; set; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets or sets Segment property
        /// </summary>
        protected ChartSegment Segment { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Internal Override Methods

        /// <summary>
        /// This method used to get the chart data at an index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal override ChartDataPointInfo GetDataPoint(int index)
        {
            IList<double> xValues = (ActualXValues is IList<double>) ? ActualXValues as IList<double> : GetXValues();
            dataPoint = null;
            dataPoint = new ChartDataPointInfo();

            if (xValues.Count > index)
                dataPoint.XData = IsIndexed ? index : xValues[index];
            if (ActualSeriesYValues != null && ActualSeriesYValues.Count() > 0)
            {
                if (ActualSeriesYValues[0].Count > index)
                    dataPoint.High = ActualSeriesYValues[0][index];

                if (ActualSeriesYValues[1].Count > index)
                    dataPoint.Low = ActualSeriesYValues[1][index];
            }

            dataPoint.Index = index;
            dataPoint.Series = this;

            if (ActualData.Count > index)
                dataPoint.Item = ActualData[index];

            return dataPoint;
        }

        /// <summary>
        /// Validate the datapoints for segment implementation.
        /// </summary>
        internal override void ValidateYValues()
        {
            foreach (var highValue in HighValues)
            {
                if (double.IsNaN(highValue) && ShowEmptyPoints)
                    ValidateDataPoints(HighValues); break;
            }

            foreach (var lowValue in LowValues)
            {
                if (double.IsNaN(lowValue) && ShowEmptyPoints)
                    ValidateDataPoints(LowValues); break;
            }
        }

        /// <summary>
        /// Validate the datapoints for segment implementation.
        /// </summary>
        internal override void ReValidateYValues(List<int>[] emptyPointIndexs)
        {
            foreach (var index in emptyPointIndexs[0])
            {
                HighValues[index] = double.NaN;
            }

            foreach (var index in emptyPointIndexs[1])
            {
                LowValues[index] = double.NaN;
            }
        }

        #endregion

        #region Internal Methods

        internal void AddAdornments(double xVal, double xOffset, double high, double low, int index)
        {
            double adornX = 0d, adornTop = 0d, adornBottom = 0d;
            ActualLabelPosition topLabelPosition;
            ActualLabelPosition bottomLabelPosition;
            ChartAdornment chartAdornment;
            var isRangeColumnSingleValue = this is RangeColumnSeries && !IsMultipleYPathRequired;

            if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top && !isRangeColumnSingleValue)
            {
                adornX = xVal + xOffset;

                if (high > low)
                {
                    adornTop = high;
                }
                else
                {
                    adornTop = low;
                }

                topLabelPosition = this.IsActualTransposed ? this.ActualYAxis.IsInversed ? ActualLabelPosition.Left : ActualLabelPosition.Right
                                                           : this.ActualYAxis.IsInversed ? ActualLabelPosition.Bottom : ActualLabelPosition.Top;

                if (index < Adornments.Count)
                {
                    Adornments[index].ActualLabelPosition = topLabelPosition;
                    Adornments[index].SetData(xVal, adornTop, adornX, adornTop);
                }
                else
                {
                    chartAdornment = this.CreateAdornment(this, xVal, adornTop, adornX, adornTop);
                    chartAdornment.ActualLabelPosition = topLabelPosition;
                    Adornments.Add(chartAdornment);
                }

                Adornments[index].Item = ActualData[index];
            }
            else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom && !isRangeColumnSingleValue)
            {
                adornX = xVal + xOffset;
                if (high > low)
                {
                    adornBottom = low;
                }
                else
                {
                    adornBottom = high;
                }

                bottomLabelPosition = this.IsActualTransposed ? this.ActualYAxis.IsInversed ?
                                     ActualLabelPosition.Right : ActualLabelPosition.Left
                                     : this.ActualYAxis.IsInversed ?
                                     ActualLabelPosition.Top : ActualLabelPosition.Bottom;

                if (index < Adornments.Count)
                {
                    Adornments[index].ActualLabelPosition = bottomLabelPosition;
                    Adornments[index].SetData(xVal, adornBottom, adornX, adornBottom);
                }
                else
                {
                    chartAdornment = this.CreateAdornment(this, xVal, adornBottom, adornX, adornBottom);
                    chartAdornment.ActualLabelPosition = bottomLabelPosition;
                    Adornments.Add(chartAdornment);
                }

                Adornments[index].Item = ActualData[index];
            }
            else
            {
                adornX = xVal + xOffset;

                if (this.IsActualTransposed)
                {
                    if (this.ActualYAxis.IsInversed)
                    {
                        topLabelPosition = ActualLabelPosition.Left;
                        bottomLabelPosition = ActualLabelPosition.Right;
                    }
                    else
                    {
                        topLabelPosition = ActualLabelPosition.Right;
                        bottomLabelPosition = ActualLabelPosition.Left;
                    }
                }
                else
                {
                    if (this.ActualYAxis.IsInversed)
                    {
                        topLabelPosition = ActualLabelPosition.Bottom;
                        bottomLabelPosition = ActualLabelPosition.Top;
                    }
                    else
                    {
                        topLabelPosition = ActualLabelPosition.Top;
                        bottomLabelPosition = ActualLabelPosition.Bottom;
                    }
                }

                if (high > low)
                {
                    adornTop = high;
                    adornBottom = low;
                }
                else
                {
                    adornTop = low;
                    adornBottom = high;
                }

                var adornmentCount = isRangeColumnSingleValue ? Adornments.Count : Adornments.Count / 2;

                if (index < adornmentCount)
                {
					if (isRangeColumnSingleValue)
					{
                        Adornments[index].ActualLabelPosition = topLabelPosition;
                        Adornments[index].SetData(xVal, adornTop, adornX, adornTop);
					}
                    else
					{
                        int j = 2 * index;
                        Adornments[j].ActualLabelPosition = topLabelPosition;
                        Adornments[j++].SetData(xVal, adornTop, adornX, adornTop);

                        Adornments[j].ActualLabelPosition = bottomLabelPosition;
                        Adornments[j].SetData(xVal, adornBottom, adornX, adornBottom);                
					}
                }
                else
                {
                    chartAdornment = this.CreateAdornment(this, xVal, adornTop, adornX, adornTop);
                    chartAdornment.ActualLabelPosition = topLabelPosition;
                    Adornments.Add(chartAdornment);

					if (isRangeColumnSingleValue)
					{
                        Adornments[index].Item = ActualData[index];
						return;
					}
                                   
                    chartAdornment = this.CreateAdornment(this, xVal, adornBottom, adornX, adornBottom);
                    chartAdornment.ActualLabelPosition = bottomLabelPosition;
                    Adornments.Add(chartAdornment);
                }

                if (!isRangeColumnSingleValue)
                {
                    int k = 2 * index;
                    Adornments[k++].Item = ActualData[index];
                    Adornments[k].Item = ActualData[index];
                }
                else
                {
                    Adornments[index].Item = ActualData[index];
                }
            }
        }

        #endregion

        #region Protected Internal Override Methods

        /// <summary>
        /// Method implementation  for GeneratePoints for Adornments
        /// </summary>
        protected internal override void GeneratePoints()
        {
            GeneratePoints(new string[] { High, Low }, HighValues, LowValues);
        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Called when DataSource property changed 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnDataSourceChanged(oldValue, newValue);
            HighValues.Clear();
            LowValues.Clear();
            Segment = null;
            GeneratePoints(new string[] { High, Low }, HighValues, LowValues);
            isPointValidated = false;
            this.UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            HighValues.Clear();
            LowValues.Clear();
            Segment = null;
            base.OnBindingPathChanged(args);
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            var rangeSeriesBase = obj as RangeSeriesBase;
            if (rangeSeriesBase != null)
            {
                rangeSeriesBase.High = this.High;
                rangeSeriesBase.Low = this.Low;
            }

            return base.CloneSeries(obj);
        }

        #endregion

        #region Private Static Methods

        private static void OnYPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RangeSeriesBase).OnBindingPathChanged(e);
        }

        #endregion

        #endregion

        internal override List<object> GetDataPoints(double startX, double endX, double startY, double endY, int minimum, int maximum, List<double> xValues, bool validateYValues)
        {
            List<object> dataPoints = new List<object>();
            int count = xValues.Count;
            if (count == HighValues.Count && count == LowValues.Count)
            {
                for (int i = minimum; i <= maximum; i++)
                {
                    double xValue = xValues[i];
                    if (validateYValues || (startX <= xValue && xValue <= endX))
                    {
                        if (startY <= HighValues[i] && HighValues[i] <= endY)
                        {
                            dataPoints.Add(ActualData[i]);
                        }
                        else if (startY <= LowValues[i] && LowValues[i] <= endY)
                        {
                            dataPoints.Add(ActualData[i]);
                        }
                    }
                }

                return dataPoints;
            }
            else
            {
                return null;
            }
        }
    }
}
