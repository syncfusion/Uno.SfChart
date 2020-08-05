using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// HiLoSeries is used primarily to analyze price movements of a stock market over a period of time.
    /// </summary>
    /// <seealso cref="HiLoOpenCloseSeries"/>
    /// <seealso cref="HiLoSegment"/>
    /// <seealso cref="CandleSeries"/>
    public partial class HiLoSeries : RangeSeriesBase, ISegmentSelectable
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="SegmentSelectionBrush"/> property. 
        /// </summary>
        public static readonly DependencyProperty SegmentSelectionBrushProperty =
            DependencyProperty.Register(
                "SegmentSelectionBrush",
                typeof(Brush),
                typeof(HiLoSeries),
                new PropertyMetadata(null, OnSegmentSelectionBrush));

        /// <summary>
        /// The DependencyProperty for <see cref="SelectedIndex"/> property. 
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex",
                typeof(int),
                typeof(HiLoSeries),
                new PropertyMetadata(-1, OnSelectedIndexChanged));

        #endregion

        #region Fields

        #region Private Fields

        Point y1Value = new Point();

        Point y2Value = new Point();

        #endregion

        #endregion

        #region Properties

        #region Public Properties

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

        #region Public Override Methods

        /// <summary>
        /// Creates the segments of HiLoSeries.
        /// </summary>
        public override void CreateSegments()
        {
            List<double> xValues = null;
            bool isGrouping = this.ActualXAxis is CategoryAxis ? (this.ActualXAxis as CategoryAxis).IsIndexed : true;
            if (!isGrouping)
                xValues = GroupedXValuesIndexes;
            else
                xValues = GetXValues();
            if (xValues != null)
            {
                double center = this.GetSideBySideInfo(this).Median;
                if (!isGrouping)
                {
                    Segments.Clear();
                    Adornments.Clear();
                    for (int i = 0; i < xValues.Count; i++)
                    {
                        if (i < xValues.Count && GroupedSeriesYValues[0].Count > i)
                        {
                            xValues[i] += center;
                            HiLoSegment line = new HiLoSegment(xValues[i], GroupedSeriesYValues[0][i], GroupedSeriesYValues[1][i], this, ActualData[i]);
                            line.High = GroupedSeriesYValues[0][i];
                            line.Low = GroupedSeriesYValues[1][i];
                            line.XValue = xValues[i];
                            Segments.Add(line);
                            if (AdornmentsInfo != null)
                                AddAdornments(xValues[i], 0, GroupedSeriesYValues[0][i], GroupedSeriesYValues[1][i], i);
                        }
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
                            ClearUnUsedAdornments(this.DataCount * 2);
                        else
                            ClearUnUsedAdornments(this.DataCount);
                    }

                    for (int i = 0; i < this.DataCount; i++)
                    {
                        if (i < Segments.Count)
                        {
                            xValues[i] += center;
                            (Segments[i]).SetData(xValues[i], HighValues[i], LowValues[i]);
                            (Segments[i]).Item = ActualData[i];
                            (Segments[i] as HiLoSegment).High = HighValues[i];
                            (Segments[i] as HiLoSegment).Low = LowValues[i];
                        }
                        else
                        {
                            xValues[i] += center;
                            HiLoSegment line = new HiLoSegment(xValues[i], HighValues[i], LowValues[i], this, ActualData[i]);
                            line.High = HighValues[i];
                            line.Low = LowValues[i];
                            line.XValue = xValues[i];
                            Segments.Add(line);
                        }

                        if (AdornmentsInfo != null)
                            AddAdornments(xValues[i], 0, HighValues[i], LowValues[i], i);
                    }
                }

                if (ShowEmptyPoints)
                    UpdateEmptyPointSegments(xValues, true);
            }
        }

        #endregion

        #region Internal Override Methods

        /// <summary>
        /// This method used to gets the chart data point at a position.
        /// </summary>
        /// <param name="mousePos"></param>
        /// <returns></returns>
        internal override ChartDataPointInfo GetDataPoint(Point mousePos)
        {
            Rect rect;
            int startIndex, endIndex;
            List<int> hitIndexes = new List<int>();
            IList<double> xValues = (ActualXValues is IList<double>) ? ActualXValues as IList<double> : GetXValues();

            CalculateHittestRect(mousePos, out startIndex, out endIndex, out rect);

            for (int i = startIndex; i <= endIndex; i++)
            {
                y1Value.X = IsIndexed ? i : xValues[i];
                y1Value.Y = ActualSeriesYValues[0][i];

                y2Value.X = IsIndexed ? i : xValues[i];
                y2Value.Y = ActualSeriesYValues[1][i];

                if (rect.Contains(y1Value) || rect.Contains(y2Value))
                    hitIndexes.Add(i);
            }

            if (hitIndexes.Count > 0)
            {
                int i = hitIndexes[hitIndexes.Count / 2];
                hitIndexes = null;

                dataPoint = new ChartDataPointInfo();
                dataPoint.Index = i;
                dataPoint.XData = xValues[i];
                dataPoint.High = ActualSeriesYValues[0][i];
                dataPoint.Low = ActualSeriesYValues[1][i];
                dataPoint.Series = this;
                if (i > -1 && ActualData.Count > i)
                    dataPoint.Item = ActualData[i];

                return dataPoint;
            }
            else
                return dataPoint;
        }

        #endregion

        #region Protected Override Methods

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new HiLoSeries()
            {
                SegmentSelectionBrush = this.SegmentSelectionBrush,
                SelectedIndex = this.SelectedIndex,
            });
        }

        #endregion

        #region Private Static Methods

        private static void OnSegmentSelectionBrush(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HiLoSeries).UpdateArea();
        }
        
        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
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
        /// <returns></returns>
        private bool IsPointOnLine(Point startPoint, Point endPoint, Point checkPoint)
        {
            if (!IsTransposed)
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
                    || Math.Round(startPoint.X) == Math.Floor(checkPoint.X))
                {
                    return endPoint.X - startPoint.X ==
                        (endPoint.X - checkPoint.X) + (checkPoint.X - startPoint.X) ? true : false;
                }

                return false;
            }
        }

        #endregion

        #endregion
    }
}
