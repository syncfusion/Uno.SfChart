using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// HiLoOpenCloseSeries is used primarily to analyze price movements of a stock market over a period of time.
    /// </summary>
    /// <remarks>
    /// Each data point contains two values namely high, low. Typically, the high and low values are connected using a vertical straight line.
    /// </remarks>
    /// <seealso cref="HiLoOpenCloseSegment"/>
    /// <seealso cref="HiLoSeries"/>
    /// <seealso cref="CandleSeries"/>
    public partial class HiLoOpenCloseSeries : FinancialSeriesBase, ISegmentSpacing, ISegmentSelectable
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSpacing"/> property. 
        /// </summary>
        public static readonly DependencyProperty SegmentSpacingProperty =
            DependencyProperty.Register(
                "SegmentSpacing", 
                typeof(double), 
                typeof(HiLoOpenCloseSeries),
                new PropertyMetadata(0.0, OnSegmentSpacingChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="SegmentSelectionBrush"/> property. 
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register(
                "SegmentSelectionBrush",
                typeof(Brush),
                typeof(HiLoOpenCloseSeries),
                new PropertyMetadata(null, OnSegmentSelectionBrush));

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property. 
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex",
                typeof(int),
                typeof(HiLoOpenCloseSeries),
                new PropertyMetadata(-1, OnSelectedIndexChanged));

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the spacing between the segments across the series in cluster mode.
        /// </summary>
        /// <value>
        ///     The value ranges from 0 to 1.
        /// </value>
        public double SegmentSpacing
        {
            get { return (double)GetValue(SegmentSpacingProperty); }
            set { SetValue(SegmentSpacingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the interior(brush) for the selected segment(s).
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        /// <example>
        ///     <code>
        ///     series.SegmentSelectionBrush = new SolidColorBrush(Colors.Red);
        ///     </code>
        /// </example>
        public Brush SegmentSelectionBrush
        {
            get { return (Brush)GetValue(SegmentSelectionBrushProperty); }
            set { SetValue(SegmentSelectionBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the index of the selected segment.
        /// </summary>
        /// <value>
        /// <c>Int</c> value represents the index of the data point(or segment) to be selected. 
        /// </value>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        #endregion

        #region Internal Override Properties

        /// <summary>
        /// Indicates that this series requires multiple y values.
        /// </summary>
        internal override bool IsMultipleYPathRequired
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Protected Internal Override Properties

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
                return true;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Interface Methods
        
        double ISegmentSpacing.CalculateSegmentSpacing(double spacing, double Right, double Left)
        {
            return CalculateSegmentSpacing(spacing, Right, Left);
        }

        #endregion

        #region Public Override Methods

        /// <summary>
        /// Creates the segments of HiLoOpenCloseSeries
        /// </summary>
        public override void CreateSegments()
        {
            List<double> xValues = null;
            bool isGrouping = this.ActualXAxis is CategoryAxis ? (this.ActualXAxis as CategoryAxis).IsIndexed : true;
            if (!isGrouping)
                xValues = GroupedXValuesIndexes;
            else
                xValues = GetXValues();
            IList<double> values = GetComparisionModeValues();
            bool isBull = false;
            if (xValues != null)
            {
                DoubleRange sbsInfo = this.GetSideBySideInfo(this);
                double median = sbsInfo.Delta / 2;
                double center = sbsInfo.Median;
                double Left = sbsInfo.Start;
                double Right = sbsInfo.End;
                double leftpos = (this as ISegmentSpacing).CalculateSegmentSpacing(SegmentSpacing, Right, Left);
                double rightpos = (this as ISegmentSpacing).CalculateSegmentSpacing(SegmentSpacing, Left, Right);

                if (SegmentSpacing > 0 && SegmentSpacing <= 1)
                {
                    Left = leftpos;
                    Right = rightpos;
                }

                if (!isGrouping)
                {
                    Segments.Clear();
                    Adornments.Clear();

                    for (int i = 0; i < xValues.Count; i++)
                    {
                        ChartPoint highPt = new ChartPoint(xValues[i] + center, GroupedSeriesYValues[0][i]);
                        ChartPoint lowPt = new ChartPoint(xValues[i] + center, GroupedSeriesYValues[1][i]);
                        ChartPoint startOpenPt = new ChartPoint(xValues[i] + Left, GroupedSeriesYValues[2][i]);
                        ChartPoint endOpenPt = new ChartPoint(xValues[i] + center, GroupedSeriesYValues[2][i]);
                        ChartPoint startClosePt = new ChartPoint(xValues[i] + Right, GroupedSeriesYValues[3][i]);
                        ChartPoint endClosePt = new ChartPoint(xValues[i] + center, GroupedSeriesYValues[3][i]);
                        if (i == 0 || this.ComparisonMode == FinancialPrice.None)
                            isBull = GroupedSeriesYValues[2][i] < GroupedSeriesYValues[3][i];
                        else
                            isBull = values[i] >= values[i - 1];

                        HiLoOpenCloseSegment hiloOpenClose = new HiLoOpenCloseSegment(highPt, lowPt, startOpenPt, endOpenPt, startClosePt, endClosePt, isBull, this, ActualData[i]);
                        hiloOpenClose.SetData(highPt, lowPt, startOpenPt, endOpenPt, startClosePt, endClosePt, isBull);
                        hiloOpenClose.BullFillColor = BullFillColor;
                        hiloOpenClose.BearFillColor = BearFillColor;
                        hiloOpenClose.High = GroupedSeriesYValues[0][i];
                        hiloOpenClose.Low = GroupedSeriesYValues[1][i];
                        hiloOpenClose.Open = GroupedSeriesYValues[2][i];
                        hiloOpenClose.Close = GroupedSeriesYValues[3][i];
                        hiloOpenClose.Item = ActualData[i];
                        Segments.Add(hiloOpenClose);
                        if (AdornmentsInfo != null)
                            AddAdornments(xValues[i], highPt, lowPt, startOpenPt, endOpenPt, startClosePt, endClosePt, i, median);
                    }
                }
                else
                {
                    if (Segments.Count > this.DataCount)
                    {
                        ClearUnUsedSegments(this.DataCount);
                    }

                    if (AdornmentsInfo != null)
                    {
                        if (AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                            ClearUnUsedAdornments(this.DataCount * 4);
                        else
                            ClearUnUsedAdornments(this.DataCount * 2);
                    }

                    for (int i = 0; i < this.DataCount; i++)
                    {
                        ChartPoint highPt = new ChartPoint(xValues[i] + center, HighValues[i]);
                        ChartPoint lowPt = new ChartPoint(xValues[i] + center, LowValues[i]);
                        ChartPoint startOpenPt = new ChartPoint(xValues[i] + Left, OpenValues[i]);
                        ChartPoint endOpenPt = new ChartPoint(xValues[i] + center, OpenValues[i]);
                        ChartPoint startClosePt = new ChartPoint(xValues[i] + Right, CloseValues[i]);
                        ChartPoint endClosePt = new ChartPoint(xValues[i] + center, CloseValues[i]);

                        if (i == 0 || this.ComparisonMode == FinancialPrice.None)
                            isBull = OpenValues[i] < CloseValues[i];
                        else
                            isBull = values[i] >= values[i - 1];

                        if (i < Segments.Count)
                        {
                            Segments[i].Item = ActualData[i];
                            (Segments[i]).SetData(highPt, lowPt, startOpenPt, endOpenPt, startClosePt, endClosePt, isBull);
                            (Segments[i] as HiLoOpenCloseSegment).High = HighValues[i];
                            (Segments[i] as HiLoOpenCloseSegment).Low = LowValues[i];
                            (Segments[i] as HiLoOpenCloseSegment).Open = OpenValues[i];
                            (Segments[i] as HiLoOpenCloseSegment).Close = CloseValues[i];
                        }
                        else
                        {
                            HiLoOpenCloseSegment hiloOpenClose = new HiLoOpenCloseSegment(highPt, lowPt, startOpenPt, endOpenPt, startClosePt, endClosePt, isBull, this, ActualData[i]);
                            hiloOpenClose.SetData(highPt, lowPt, startOpenPt, endOpenPt, startClosePt, endClosePt, isBull);
                            hiloOpenClose.BullFillColor = BullFillColor;
                            hiloOpenClose.BearFillColor = BearFillColor;
                            hiloOpenClose.High = HighValues[i];
                            hiloOpenClose.Low = LowValues[i];
                            hiloOpenClose.Open = OpenValues[i];
                            hiloOpenClose.Close = CloseValues[i];
                            hiloOpenClose.Item = ActualData[i];
                            Segments.Add(hiloOpenClose);
                        }

                        if (AdornmentsInfo != null)
                            AddAdornments(xValues[i], highPt, lowPt, startOpenPt, endOpenPt, startClosePt, endClosePt, i, median);
                    }
                }

                if (ShowEmptyPoints)
                    UpdateEmptyPointSegments(xValues, true);
            }
        }

        #endregion

        #region Protected Override Methods

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new HiLoOpenCloseSeries()
            {
                SegmentSelectionBrush = this.SegmentSelectionBrush,
                SelectedIndex = this.SelectedIndex,
                SegmentSpacing = this.SegmentSpacing
            });
        }

        #endregion

        #region Protected Methods

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        protected double CalculateSegmentSpacing(double spacing, double Right, double Left)
        {
            double diff = Right - Left;
            double totalspacing = diff * spacing / 2;
            Left = Left + totalspacing;
            Right = Right - totalspacing;
            return Left;
        }

        #endregion

        #region Private Static Methods

        private static void OnSegmentSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = d as HiLoOpenCloseSeries;
            if (series.Area != null)
                series.Area.ScheduleUpdate();
        }

        private static void OnSegmentSelectionBrush(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HiLoOpenCloseSeries).UpdateArea();
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            series.OnPropertyChanged("SelectedIndex");
            if (series.ActualArea == null || series.ActualArea.SelectionBehaviour == null) return;
            if (series.ActualArea.SelectionBehaviour.SelectionStyle == SelectionStyle.Single)
                series.SelectedIndexChanged((int)e.NewValue, (int)e.OldValue);
            else if ((int)e.NewValue != -1)
                series.SelectedSegmentsIndexes.Add((int)e.NewValue);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method used to check the given checkPoint within the startPoint and endPoint
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="checkPoint"></param>
        /// <param name="isOpenCloseLine"></param>
        /// <returns></returns>
        private bool IsPointOnLine(Point startPoint, Point endPoint, Point checkPoint, bool isOpenCloseLine)
        {
            if (!IsTransposed)
            {
                if (!isOpenCloseLine)
                {
                    if (Math.Round(startPoint.X) == Math.Ceiling(checkPoint.X)
                    || Math.Round(startPoint.X) == Math.Floor(checkPoint.X))
                    {
                        return endPoint.Y - startPoint.Y ==
                            (endPoint.Y - checkPoint.Y) + (checkPoint.Y - startPoint.Y) ? true : false;
                    }

                    return false;
                }
                else
                {
                    if (Math.Round(startPoint.Y) == Math.Ceiling(checkPoint.Y)
                        || Math.Round(startPoint.Y) == Math.Floor(checkPoint.Y))
                    {
                        return endPoint.X - startPoint.X ==
                            (endPoint.X - checkPoint.X) + (checkPoint.X - startPoint.X) ? true : false;
                    }

                    return false;
                }
            }
            else
            {
                if (!isOpenCloseLine)
                {
                    if (Math.Round(startPoint.Y) == Math.Ceiling(checkPoint.Y)
                        || Math.Round(startPoint.Y) == Math.Floor(checkPoint.Y))
                    {
                        return endPoint.X - startPoint.X ==
                            (endPoint.X - checkPoint.X) + (checkPoint.X - startPoint.X) ? true : false;
                    }

                    return false;
                }
                else
                {
                    if (Math.Round(startPoint.X) == Math.Ceiling(checkPoint.X)
                        || Math.Round(startPoint.X) == Math.Floor(checkPoint.X))
                    {
                        return endPoint.Y - startPoint.Y ==
                            (endPoint.Y - checkPoint.Y) + (checkPoint.Y - startPoint.Y) ? true : false;
                    }

                    return false;
                }
            }
        }

        #endregion

        #endregion
    }
}
