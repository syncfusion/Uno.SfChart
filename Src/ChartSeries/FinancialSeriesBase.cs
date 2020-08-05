using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI;
using System.Collections;
using Windows.Foundation;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for FinancialSeriesBase
    /// </summary>
    public abstract partial class FinancialSeriesBase : CartesianSeries
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="High"/> property. 
        /// </summary>
        public static readonly DependencyProperty HighProperty =
            DependencyProperty.Register(
                "High",
                typeof(string),
                typeof(FinancialSeriesBase),
                new PropertyMetadata(null, OnYPathChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Low"/> property. 
        /// </summary>
        public static readonly DependencyProperty LowProperty =
            DependencyProperty.Register(
                "Low", 
                typeof(string),
                typeof(FinancialSeriesBase),
                new PropertyMetadata(null, OnYPathChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Open"/> property. 
        /// </summary>
        public static readonly DependencyProperty OpenProperty =
            DependencyProperty.Register(
                "Open",
                typeof(string),
                typeof(FinancialSeriesBase),
                new PropertyMetadata(null, OnYPathChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Close"/> property. 
        /// </summary>
        public static readonly DependencyProperty CloseProperty =
            DependencyProperty.Register(
                "Close", 
                typeof(string), 
                typeof(FinancialSeriesBase),
                new PropertyMetadata(null, OnYPathChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="BearFillColor"/> property. 
        /// </summary>
        public static readonly DependencyProperty BearFillColorProperty =
            DependencyProperty.Register(
                "BearFillColor",
                typeof(Brush), 
                typeof(FinancialSeriesBase),
                new PropertyMetadata(
                    new SolidColorBrush(Colors.Red),
                    new PropertyChangedCallback(OnBearFillColorPropertyChanged)));

        /// <summary>
        /// The DependencyProperty for <see cref="BullFillColor"/> property. 
        /// </summary>
        public static readonly DependencyProperty BullFillColorProperty =
            DependencyProperty.Register(
                "BullFillColor", 
                typeof(Brush), 
                typeof(FinancialSeriesBase),
                new PropertyMetadata(
                    new SolidColorBrush(Colors.Green),
                    new PropertyChangedCallback(OnBullFillColorPropertyChanged)));

        /// <summary>
        /// The DependencyProperty for <see cref="ComparisonMode"/> property. 
        /// </summary>
#if UNIVERSALWINDOWS
        public static readonly DependencyProperty ComparisonModeProperty =
            DependencyProperty.Register("ComparisonMode", typeof(FinancialPrice), typeof(FinancialSeriesBase), new PropertyMetadata(FinancialPrice.Close, new PropertyChangedCallback(OnComparisonModeChanged)));
#else
        public static readonly DependencyProperty ComparisonModeProperty =
            DependencyProperty.Register("ComparisonMode", typeof(FinancialPrice), typeof(FinancialSeriesBase), new PropertyMetadata(FinancialPrice.None, new PropertyChangedCallback(OnComparisonModeChanged)));
#endif

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public FinancialSeriesBase()
        {
            OpenValues = new List<double>();
            HighValues = new List<double>();
            LowValues = new List<double>();
            CloseValues = new List<double>();
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the property path bind with high value of financial series.
        /// </summary>
        public string High
        {
            get { return (string)GetValue(HighProperty); }
            set { SetValue(HighProperty, value); }
        }

        /// <summary>
        /// Gets or sets the property path bind with low value of financial series.
        /// </summary>
        public string Low
        {
            get { return (string)GetValue(LowProperty); }
            set { SetValue(LowProperty, value); }
        }

        /// <summary>
        /// Gets or sets the property path bind with close value of financial series.
        /// </summary>
        public string Close
        {
            get { return (string)GetValue(CloseProperty); }
            set { SetValue(CloseProperty, value); }
        }

        /// <summary>
        /// Gets or sets the interior of the segment that represents the bear value. This is a bindable property.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush BearFillColor
        {
            get { return (Brush)GetValue(BearFillColorProperty); }
            set { SetValue(BearFillColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the interior of the segment that represents the bull value. This is a bindable property.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush BullFillColor
        {
            get { return (Brush)GetValue(BullFillColorProperty); }
            set { SetValue(BullFillColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the property path bind with which price need to consider for fluctuation detection.
        /// </summary>
        public FinancialPrice ComparisonMode
        {
            get { return (FinancialPrice)GetValue(ComparisonModeProperty); }
            set { SetValue(ComparisonModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the property path bind with open value of financial series.
        /// </summary>
        public string Open
        {
            get { return (string)GetValue(OpenProperty); }
            set { SetValue(OpenProperty, value); }
        }

        #endregion

        #region Protected Internal Properties

        /// <summary>
        /// Gets or sets OpenValues property
        /// </summary>
        protected internal IList<double> OpenValues { get; set; }

        /// <summary>
        /// Gets or sets HighValues
        /// </summary>
        protected internal IList<double> HighValues { get; set; }

        /// <summary>
        /// Gets or sets LowValues property
        /// </summary>
        protected internal IList<double> LowValues { get; set; }

        /// <summary>
        /// Gets or sets CloseValues
        /// </summary>
        protected internal IList<double> CloseValues { get; set; }

        /// <summary>
        /// Gets or sets Segments property
        /// </summary>
        protected ChartSegment Segment { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Internal Override Methods

        /// <summary>
        /// This method used to get the chart data at a mouse position.
        /// </summary>
        /// <param name="mousePos"></param>
        /// <returns></returns>
        internal override ChartDataPointInfo GetDataPoint(Point mousePos)
        {
            double xVal = 0;
            double yVal = 0;
            double stackedYValue = double.NaN;
            dataPoint = null;
            dataPoint = new ChartDataPointInfo();
            if (this.Area.SeriesClipRect.Contains(mousePos))
            {
                var point = new Point(
                    mousePos.X - this.Area.SeriesClipRect.Left,
                    mousePos.Y - this.Area.SeriesClipRect.Top);

                this.FindNearestChartPoint(point, out xVal, out yVal, out stackedYValue);

                dataPoint.XData = xVal;

                int index = ((ActualXAxis is CategoryAxis) && (!(ActualXAxis as CategoryAxis).IsIndexed)) ?
                   GroupedXValuesIndexes.IndexOf(xVal) : this.GetXValues().IndexOf(xVal);

                if (index == -1)
                {
                    return null;
                }

                if ((ActualXAxis is CategoryAxis) && (!(ActualXAxis as CategoryAxis).IsIndexed))
                {
                    if (GroupedSeriesYValues[0].Count > index)
                        dataPoint.High = GroupedSeriesYValues[0][index];

                    if (GroupedSeriesYValues[1].Count > index)
                        dataPoint.Low = GroupedSeriesYValues[1][index];

                    if (GroupedSeriesYValues[2].Count > index)
                        dataPoint.Open = GroupedSeriesYValues[2][index];

                    if (GroupedSeriesYValues[3].Count > index)
                        dataPoint.Close = GroupedSeriesYValues[3][index];
                }
                else
                {
                    if (ActualSeriesYValues[0].Count > index)
                        dataPoint.High = ActualSeriesYValues[0][index];

                    if (ActualSeriesYValues[1].Count > index)
                        dataPoint.Low = ActualSeriesYValues[1][index];

                    if (ActualSeriesYValues[2].Count > index)
                        dataPoint.Open = ActualSeriesYValues[2][index];

                    if (ActualSeriesYValues[3].Count > index)
                        dataPoint.Close = ActualSeriesYValues[3][index];
                }

                dataPoint.Index = index;
                dataPoint.Series = this;

                if (ActualData.Count > index)
                    dataPoint.Item = ActualData[index];
            }

            return dataPoint;
        }

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

                if (ActualSeriesYValues[2].Count > index)
                    dataPoint.Open = ActualSeriesYValues[2][index];

                if (ActualSeriesYValues[3].Count > index)
                    dataPoint.Close = ActualSeriesYValues[3][index];
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

            foreach (var index in emptyPointIndexs[2])
            {
                OpenValues[index] = double.NaN;
            }

            foreach (var index in emptyPointIndexs[3])
            {
                CloseValues[index] = double.NaN;
            }
        }

        /// <summary>
        /// Validate the datapoints for segment implementation.
        /// </summary>
        internal override void ValidateYValues()
        {
            foreach (var highValue in HighValues)
            {
                if (double.IsNaN(highValue) && ShowEmptyPoints)
                {
                    ValidateDataPoints(HighValues); break;
                }
            }

            foreach (var lowValue in LowValues)
            {
                if (double.IsNaN(lowValue) && ShowEmptyPoints)
                {
                    ValidateDataPoints(LowValues); break;
                }
            }

            foreach (var openValue in OpenValues)
            {
                if (double.IsNaN(openValue) && ShowEmptyPoints)
                {
                    ValidateDataPoints(OpenValues); break;
                }
            }

            foreach (var closeValue in CloseValues)
            {
                if (double.IsNaN(closeValue) && ShowEmptyPoints)
                {
                    ValidateDataPoints(CloseValues); break;
                }
            }
        }

        #endregion

        #region Internal Methods

        internal IList<double> GetComparisionModeValues()
        {
            if (this.ActualXAxis is CategoryAxis && (!(ActualXAxis as CategoryAxis).IsIndexed))
            {
                switch (this.ComparisonMode)
                {
                    case FinancialPrice.Close:
                        return this.GroupedSeriesYValues[3];
                    case FinancialPrice.Open:
                        return this.GroupedSeriesYValues[2];
                    case FinancialPrice.High:
                        return this.GroupedSeriesYValues[0];
                    case FinancialPrice.Low:
                        return this.GroupedSeriesYValues[1];
                }
            }
            else
            {
                switch (this.ComparisonMode)
                {
                    case FinancialPrice.Close:
                        return this.CloseValues;
                    case FinancialPrice.Open:
                        return this.OpenValues;
                    case FinancialPrice.High:
                        return this.HighValues;
                    case FinancialPrice.Low:
                        return this.LowValues;
                }
            }

            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Reviewed")]
        internal void AddAdornments(double xVal, ChartPoint highPt, ChartPoint lowPt, ChartPoint startOpenPt, ChartPoint endOpenPt, ChartPoint startClosePt, ChartPoint endClosePt, int i, double median)
        {
            double adornX1, adornX2, adornX3, adornX4, adornMax, adornMin, adornOpen, adornClose;

            ActualLabelPosition topLabelPosition;
            ActualLabelPosition leftLabelPosition;
            ActualLabelPosition bottomLabelPosition;
            ActualLabelPosition rightLabelPosition;
            ChartAdornment chartAdornment;

            // To check whether the high or low value is empty
            bool isEmpty = double.IsNaN(highPt.Y) || double.IsNaN(lowPt.Y);

            // When high or low value is empty, the empty point must be considered instead of the greatest value for AdornmentPosition.
            if (isEmpty || highPt.Y > lowPt.Y)
            {
                adornMax = highPt.Y;
                adornMin = lowPt.Y;
            }
            else
            {
                adornMax = lowPt.Y;
                adornMin = highPt.Y;
            }

            // To check whether the open or close value is empty.
            isEmpty = double.IsNaN(startOpenPt.Y) || double.IsNaN(startClosePt.Y);

            if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
            {
                adornX1 = highPt.X;

                // When open or close value is empty, the empty point must be considered instead of the greatest value for the top AdornmentPostion.
                if (isEmpty || startOpenPt.Y > startClosePt.Y)
                {
                    adornOpen = startOpenPt.Y;

                    if (this is HiLoOpenCloseSeries || this is FastHiLoOpenCloseBitmapSeries)
                        adornX2 = startOpenPt.X;
                    else
                        adornX2 = highPt.X - median;

                    if (this.IsActualTransposed)
                    {
                        topLabelPosition = this.ActualYAxis.IsInversed ? ActualLabelPosition.Left : ActualLabelPosition.Right;
                        leftLabelPosition = this.ActualXAxis.IsInversed ? ActualLabelPosition.Top : ActualLabelPosition.Bottom;
                    }
                    else
                    {
                        topLabelPosition = this.ActualYAxis.IsInversed ? ActualLabelPosition.Bottom : ActualLabelPosition.Top;
                        leftLabelPosition = this.ActualXAxis.IsInversed ? ActualLabelPosition.Right : ActualLabelPosition.Left;
                    }
                }
                else
                {
                    adornOpen = startClosePt.Y;

                    if (this is HiLoOpenCloseSeries || this is FastHiLoOpenCloseBitmapSeries)
                        adornX2 = startClosePt.X;
                    else
                        adornX2 = highPt.X + median;

                    if (this.IsActualTransposed)
                    {
                        topLabelPosition = this.ActualYAxis.IsInversed ? ActualLabelPosition.Left : ActualLabelPosition.Right;
                        leftLabelPosition = this.ActualXAxis.IsInversed ? ActualLabelPosition.Bottom : ActualLabelPosition.Top;
                    }
                    else
                    {
                        topLabelPosition = this.ActualYAxis.IsInversed ? ActualLabelPosition.Bottom : ActualLabelPosition.Top;
                        leftLabelPosition = this.ActualXAxis.IsInversed ? ActualLabelPosition.Left : ActualLabelPosition.Right;
                    }
                }

                if (this is CandleSeries || this is FastCandleBitmapSeries)
                {
                    leftLabelPosition = topLabelPosition;
                }

                if (i < Adornments.Count / 2)
                {
                    int j = 2 * i;
                    Adornments[j].ActualLabelPosition = topLabelPosition;
                    Adornments[j++].SetData(xVal, adornMax, adornX1, adornMax);
                    Adornments[j].ActualLabelPosition = leftLabelPosition;
                    Adornments[j].SetData(xVal, adornOpen, adornX2, adornOpen);
                }
                else
                {
                    chartAdornment = this.CreateAdornment(this, xVal, adornMax, adornX1, adornMax);
                    chartAdornment.ActualLabelPosition = topLabelPosition;
                    Adornments.Add(chartAdornment);

                    chartAdornment = this.CreateAdornment(this, xVal, adornOpen, adornX2, adornOpen);
                    chartAdornment.ActualLabelPosition = leftLabelPosition;
                    Adornments.Add(chartAdornment);
                }

                int k = 2 * i;
                Adornments[k++].Item = ActualData[i];
                Adornments[k].Item = ActualData[i];
            }
            else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
            {
                adornX1 = lowPt.X;

                // When open or close value is empty, the empty point must be considered instead of the smallest value for the bottom AdornmentPosition.
                if (isEmpty || startClosePt.Y < startOpenPt.Y)
                {
                    adornClose = endClosePt.Y;

                    if (this is HiLoOpenCloseSeries || this is FastHiLoOpenCloseBitmapSeries)
                        adornX2 = startClosePt.X;
                    else
                        adornX2 = lowPt.X + median;

                    if (this.IsActualTransposed)
                    {
                        bottomLabelPosition = this.ActualYAxis.IsInversed ? ActualLabelPosition.Right : ActualLabelPosition.Left;
                        rightLabelPosition = this.ActualXAxis.IsInversed ? ActualLabelPosition.Bottom : ActualLabelPosition.Top;
                    }
                    else
                    {
                        bottomLabelPosition = this.ActualYAxis.IsInversed ? ActualLabelPosition.Top : ActualLabelPosition.Bottom;
                        rightLabelPosition = this.ActualXAxis.IsInversed ? ActualLabelPosition.Left : ActualLabelPosition.Right;
                    }
                }
                else
                {
                    adornClose = startOpenPt.Y;

                    if (this is HiLoOpenCloseSeries || this is FastHiLoOpenCloseBitmapSeries)
                        adornX2 = startOpenPt.X;
                    else
                        adornX2 = lowPt.X - median;

                    if (this.IsActualTransposed)
                    {
                        bottomLabelPosition = this.ActualYAxis.IsInversed ? ActualLabelPosition.Right : ActualLabelPosition.Left;
                        rightLabelPosition = this.ActualXAxis.IsInversed ? ActualLabelPosition.Top : ActualLabelPosition.Bottom;
                    }
                    else
                    {
                        bottomLabelPosition = this.ActualYAxis.IsInversed ? ActualLabelPosition.Top : ActualLabelPosition.Bottom;
                        rightLabelPosition = this.ActualXAxis.IsInversed ? ActualLabelPosition.Right : ActualLabelPosition.Left;
                    }
                }

                if (this is CandleSeries || this is FastCandleBitmapSeries)
                {
                    rightLabelPosition = bottomLabelPosition;
                }

                if (i < Adornments.Count / 2)
                {
                    int j = 2 * i;
                    Adornments[j].ActualLabelPosition = bottomLabelPosition;
                    Adornments[j++].SetData(xVal, adornMin, adornX1, adornMin);
                    Adornments[j].ActualLabelPosition = rightLabelPosition;
                    Adornments[j].SetData(xVal, adornClose, adornX2, adornClose);
                }
                else
                {
                    chartAdornment = this.CreateAdornment(this, xVal, adornMin, adornX1, adornMin);
                    chartAdornment.ActualLabelPosition = bottomLabelPosition;
                    Adornments.Add(chartAdornment);

                    chartAdornment = this.CreateAdornment(this, xVal, adornClose, adornX2, adornClose);
                    chartAdornment.ActualLabelPosition = rightLabelPosition;
                    Adornments.Add(chartAdornment);
                }

                int k = 2 * i;
                Adornments[k++].Item = ActualData[i];
                Adornments[k].Item = ActualData[i];
            }
            else
            {
                adornX1 = highPt.X;

                if (this is HiLoOpenCloseSeries || this is FastHiLoOpenCloseBitmapSeries)
                {
                    adornX2 = startClosePt.X;
                    adornX4 = startOpenPt.X;
                }
                else
                {
                    adornX2 = highPt.X + median;
                    adornX4 = lowPt.X - median;
                }

                adornX3 = lowPt.X;
                adornOpen = startOpenPt.Y;
                adornClose = endClosePt.Y;

                if (this.IsActualTransposed)
                {
                    topLabelPosition = this.ActualYAxis.IsInversed ? ActualLabelPosition.Left : ActualLabelPosition.Right;
                    leftLabelPosition = this.ActualXAxis.IsInversed ? ActualLabelPosition.Top : ActualLabelPosition.Bottom;
                    bottomLabelPosition = this.ActualYAxis.IsInversed ? ActualLabelPosition.Right : ActualLabelPosition.Left;
                    rightLabelPosition = this.ActualXAxis.IsInversed ? ActualLabelPosition.Bottom : ActualLabelPosition.Top;
                }
                else
                {
                    topLabelPosition = this.ActualYAxis.IsInversed ? ActualLabelPosition.Bottom : ActualLabelPosition.Top;
                    leftLabelPosition = this.ActualXAxis.IsInversed ? ActualLabelPosition.Right : ActualLabelPosition.Left;
                    bottomLabelPosition = this.ActualYAxis.IsInversed ? ActualLabelPosition.Top : ActualLabelPosition.Bottom;
                    rightLabelPosition = this.ActualXAxis.IsInversed ? ActualLabelPosition.Left : ActualLabelPosition.Right;
                }

                if (this is CandleSeries || this is FastCandleBitmapSeries)
                {
                    leftLabelPosition = topLabelPosition;
                    rightLabelPosition = bottomLabelPosition;
                }

                if (i < Adornments.Count / 4)
                {
                    int j = 4 * i;
                    Adornments[j].ActualLabelPosition = topLabelPosition;
                    Adornments[j++].SetData(xVal, adornMax, adornX1, adornMax);

                    Adornments[j].ActualLabelPosition = leftLabelPosition;
                    Adornments[j++].SetData(xVal, adornOpen, adornX4, adornOpen);

                    Adornments[j].ActualLabelPosition = bottomLabelPosition;
                    Adornments[j++].SetData(xVal, adornMin, adornX3, adornMin);

                    Adornments[j].ActualLabelPosition = rightLabelPosition;
                    Adornments[j].SetData(xVal, adornClose, adornX2, adornClose);
                }
                else
                {
                    chartAdornment = this.CreateAdornment(this, xVal, adornMax, adornX1, adornMax);
                    chartAdornment.ActualLabelPosition = topLabelPosition;
                    Adornments.Add(chartAdornment);

                    chartAdornment = this.CreateAdornment(this, xVal, adornOpen, adornX4, adornOpen);
                    chartAdornment.ActualLabelPosition = leftLabelPosition;
                    Adornments.Add(chartAdornment);

                    chartAdornment = this.CreateAdornment(this, xVal, adornMin, adornX3, adornMin);
                    chartAdornment.ActualLabelPosition = bottomLabelPosition;
                    Adornments.Add(chartAdornment);

                    chartAdornment = this.CreateAdornment(this, xVal, adornClose, adornX2, adornClose);
                    chartAdornment.ActualLabelPosition = rightLabelPosition;
                    Adornments.Add(chartAdornment);
                }

                int k = 4 * i;
                Adornments[k++].Item = ActualData[i];
                Adornments[k++].Item = ActualData[i];
                Adornments[k++].Item = ActualData[i];
                Adornments[k].Item = ActualData[i];
            }
        }

        #endregion

        #region Protected Internal Override Methods

        /// <summary>
        /// Method implementation  for GeneratePoints for Adornments
        /// </summary>
        protected internal override void GeneratePoints()
        {
            GeneratePoints(new string[] { High, Low, Open, Close }, HighValues, LowValues, OpenValues, CloseValues);
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
            OpenValues.Clear();
            HighValues.Clear();
            LowValues.Clear();
            CloseValues.Clear();
            Segment = null;
            GeneratePoints(new string[] { High, Low, Open, Close }, HighValues, LowValues, OpenValues, CloseValues);
            isPointValidated = false;
            this.UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            OpenValues.Clear();
            HighValues.Clear();
            LowValues.Clear();
            CloseValues.Clear();
            Segment = null;
            base.OnBindingPathChanged(args);
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            var financialSeriesBase = obj as FinancialSeriesBase;
            financialSeriesBase.High = High;
            financialSeriesBase.Low = Low;
            financialSeriesBase.Open = Open;
            financialSeriesBase.Close = Close;
            financialSeriesBase.BearFillColor = BearFillColor;
            financialSeriesBase.BullFillColor = BullFillColor;
            return base.CloneSeries(financialSeriesBase);
        }

        #endregion

        #region Private Static Methods

        private static void OnYPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FinancialSeriesBase).OnBindingPathChanged(e);
        }
                
        private static void OnComparisonModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FinancialSeriesBase financialSeries = d as FinancialSeriesBase;
            if (financialSeries.Segments != null)
            {
                financialSeries.Segments.Clear();
                financialSeries.UpdateArea();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private static void OnBearFillColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FinancialSeriesBase financialSeries = d as FinancialSeriesBase;
            if ((financialSeries is FastCandleBitmapSeries) || (financialSeries is FastHiLoOpenCloseBitmapSeries))
            {
                financialSeries.UpdateArea();
            }
            else
            {
                foreach (ChartSegment segment in financialSeries.Segments)
                {
                    if (segment is CandleSegment)
                        (segment as CandleSegment).BearFillColor = financialSeries.BearFillColor;
                    else if (segment is HiLoOpenCloseSegment)
                        (segment as HiLoOpenCloseSegment).BearFillColor = financialSeries.BearFillColor;
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private static void OnBullFillColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FinancialSeriesBase financialSeries = d as FinancialSeriesBase;
            if ((financialSeries is FastCandleBitmapSeries) || (financialSeries is FastHiLoOpenCloseBitmapSeries))
            {
                financialSeries.UpdateArea();
            }
            else
            {
                foreach (ChartSegment segment in financialSeries.Segments)
                {
                    if (segment is CandleSegment)
                        (segment as CandleSegment).BullFillColor = financialSeries.BullFillColor;
                    else if (segment is HiLoOpenCloseSegment)
                        (segment as HiLoOpenCloseSegment).BullFillColor = financialSeries.BullFillColor;
                }
            }
        }

        #endregion

        #endregion

        internal override List<object> GetDataPoints(double startX, double endX, double startY, double endY, int minimum, int maximum, List<double> xValues, bool validateYValues)
        {
            List<object> dataPoints = new List<object>();
            int count = xValues.Count;
            if (count == HighValues.Count && count == LowValues.Count && count == OpenValues.Count && count == CloseValues.Count)
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
                        else if (startY <= OpenValues[i] && OpenValues[i] <= endY)
                        {
                            dataPoints.Add(ActualData[i]);
                        }
                        else if (startY <= CloseValues[i] && CloseValues[i] <= endY)
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
