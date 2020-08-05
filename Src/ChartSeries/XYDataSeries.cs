using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using System.Collections;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Foundation;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for XyDataseries 
    /// </summary>
    public abstract partial class XyDataSeries : CartesianSeries
    {
        #region Dependency Property Registration
        
        /// <summary>
        /// The DependencyProperty for <see cref="YBindingPath"/> property.       .
        /// </summary>
        public static readonly DependencyProperty YBindingPathProperty =
            DependencyProperty.Register(
                "YBindingPath", 
                typeof(string),
                typeof(XyDataSeries),
                new PropertyMetadata(null, OnYBindingPathChanged));

        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for XyDataSeries 
        /// </summary>
        public XyDataSeries()
        {
            YValues = new List<double>();
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the property path bind with y axis.
        /// </summary>
        public string YBindingPath
        {
            get { return (string)GetValue(YBindingPathProperty); }
            set { SetValue(YBindingPathProperty, value); }
        }

        #endregion

        #region Protected Internal Properties

        /// <summary>
        /// Gets or sets the y values collection.
        /// </summary>
        protected internal IList<double> YValues
        {
            get;
            set;
        }

        #endregion

        #endregion

        #region Methods

        #region Internal Methods

        /// <summary>
        /// This method is used to gets the selected data point segment pixel positions
        /// </summary>
        internal void GenerateColumnPixels()
        {
            if (!double.IsNaN(dataPoint.YData))
            {
                WriteableBitmap bmp = Area.fastRenderSurface;

                IChartTransformer chartTransformer = CreateTransformer(
                    new Size(
                        Area.SeriesClipRect.Width,
                        Area.SeriesClipRect.Height),
                        true);
                bool x_isInversed = ActualXAxis.IsInversed;
                bool y_isInversed = ActualYAxis.IsInversed;

                DoubleRange sbsInfo = GetSideBySideInfo(this);
                double origin = ActualXAxis != null ? ActualXAxis.Origin : 0;
                double x1 = x_isInversed ? dataPoint.XData + sbsInfo.End : dataPoint.XData + sbsInfo.Start;
                double x2 = x_isInversed ? dataPoint.XData + sbsInfo.Start : dataPoint.XData + sbsInfo.End;
                double y1 = y_isInversed ? origin : dataPoint.YData;
                double y2 = y_isInversed ? dataPoint.YData : origin;

                Point tlpoint = chartTransformer.TransformToVisible(x1, y1);
                Point rbpoint = chartTransformer.TransformToVisible(x2, y2);

                double _x1 = tlpoint.X;
                double _x2 = rbpoint.X;
                double _y1 = y1 > 0 ? tlpoint.Y : rbpoint.Y;
                double _y2 = y1 > 0 ? rbpoint.Y : tlpoint.Y;
                int width = (int)Area.SeriesClipRect.Width;
                int height = (int)Area.SeriesClipRect.Height;
                var spacingSegment = this as ISegmentSpacing;
                if (spacingSegment != null)
                {
                    double spacing = spacingSegment.SegmentSpacing;
                    if (spacing > 0 && spacing <= 1)
                    {
                        double leftpos = spacingSegment.CalculateSegmentSpacing(spacing, _x2, _x1);
                        double rightpos = spacingSegment.CalculateSegmentSpacing(spacing, _x1, _x2);
                        _x1 = (float)(leftpos);
                        _x2 = (float)(rightpos);
                    }
                }

                selectedSegmentPixels.Clear();

                if (_y1 < _y2)
                    selectedSegmentPixels = bmp.GetRectangle(width, height, (int)(_x1), (int)_y1, (int)_x2, (int)_y2, selectedSegmentPixels);
                else
                    selectedSegmentPixels = bmp.GetRectangle(width, height, (int)(_x1), (int)_y2, (int)_x2, (int)_y1, selectedSegmentPixels);
            }
        }

        /// <summary>
        /// This method used to gets the selected data point segment pixel positions 
        /// </summary>
        internal void GenerateBarPixels()
        {
            WriteableBitmap bmp = Area.fastRenderSurface;

            ChartTransform.ChartCartesianTransformer cartesianTransformer = CreateTransformer(
                new Size(
                    Area.SeriesClipRect.Width,
                    Area.SeriesClipRect.Height),
                true) as ChartTransform.ChartCartesianTransformer;

            DoubleRange sbsInfo = this.GetSideBySideInfo(this);

            float x1Value = 0, x2Value = 0, y1Value = 0, y2Value = 0;

            double x1 = dataPoint.XData + sbsInfo.Start;
            double x2 = dataPoint.XData + sbsInfo.End;
            double y1 = dataPoint.YData;
            double y2 = ActualXAxis != null ? ActualXAxis.Origin : 0;

            double xStart = cartesianTransformer.XAxis.VisibleRange.Start;
            double xEnd = cartesianTransformer.XAxis.VisibleRange.End;

            double yStart = cartesianTransformer.YAxis.VisibleRange.Start;
            double yEnd = cartesianTransformer.YAxis.VisibleRange.End;

            double width = cartesianTransformer.XAxis.RenderedRect.Height;
            double height = cartesianTransformer.YAxis.RenderedRect.Width;

            // WPF-14441 - Calculating Bar Position for the Series  
            double left = Area.SeriesClipRect.Right - cartesianTransformer.YAxis.RenderedRect.Right;
            double top = Area.SeriesClipRect.Bottom - cartesianTransformer.XAxis.RenderedRect.Bottom;

            Size availableSize = new Size(width, height);
            bool isLogarithmic = cartesianTransformer.XAxis.IsLogarithmic || cartesianTransformer.YAxis.IsLogarithmic;

            if (ActualXAxis.IsInversed)
            {
                double temp = xStart;
                xStart = xEnd;
                xEnd = temp;
            }

            if (ActualYAxis.IsInversed)
            {
                double temp = yStart;
                yStart = yEnd;
                yEnd = temp;
            }

            if (!isLogarithmic)
            {
                double x1Val = ActualXAxis.IsInversed
                          ? x2 < xEnd ? xEnd : x2
                          : x1 < xStart ? xStart : x1;
                double x2Val = ActualXAxis.IsInversed
                                   ? x1 > xStart ? xStart : x1
                                   : x2 > xEnd ? xEnd : x2;

                double y1Val = ActualYAxis.IsInversed
                                   ? y2 > yStart ? yStart : y2 < yEnd ? yEnd : y2
                                   : y1 > yEnd ? yEnd : y1 < yStart ? yStart : y1;
                double y2Val = ActualYAxis.IsInversed
                                   ? y1 < yEnd ? yEnd : y1 > yStart ? yStart : y1
                                   : y2 < yStart ? yStart : y2 > yEnd ? yEnd : y2;
                x1Value = (float)(top + (availableSize.Width) * cartesianTransformer.XAxis.ValueToCoefficientCalc(x1Val));
                x2Value = (float)(top + (availableSize.Width) * cartesianTransformer.XAxis.ValueToCoefficientCalc(x2Val));
                y1Value = (float)(left + (availableSize.Height) * (1 - cartesianTransformer.YAxis.ValueToCoefficientCalc(y1Val)));
                y2Value = (float)(left + (availableSize.Height) * (1 - cartesianTransformer.YAxis.ValueToCoefficientCalc(y2Val)));
            }
            else
            {
                double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
                double yBase = cartesianTransformer.YAxis.IsLogarithmic ? (cartesianTransformer.YAxis as LogarithmicAxis).LogarithmicBase : 1;

                double logx1 = xBase == 1 ? x1 : Math.Log(x1, xBase);
                double logx2 = xBase == 1 ? x2 : Math.Log(x2, xBase);
                double logy1 = yBase == 1 ? y1 : Math.Log(y1, yBase);
                double logy2 = yBase == 1 ? y2 : Math.Log(y2, yBase);

                double x1Val = ActualXAxis.IsInversed ? logx2 < xEnd ? xEnd : logx2 : logx1 < xStart ? xStart : logx1;
                double x2Val = ActualXAxis.IsInversed ? logx1 > xStart ? xStart : logx1 : logx2 > xEnd ? xEnd : logx2;

                double y1Val = ActualYAxis.IsInversed
                                   ? logy2 > yStart ? yStart : logy2 < yEnd ? yEnd : logy2
                                   : logy1 > yEnd ? yEnd : logy1 < yStart ? yStart : logy1;

                double y2Val = ActualYAxis.IsInversed
                                   ? logy1 < yEnd ? yEnd : logy1 > yStart ? yStart : logy1
                                   : logy2 < yStart ? yStart : logy2 > yEnd ? yEnd : logy2;
                x1Value = (float)(top + (availableSize.Width) * cartesianTransformer.XAxis.ValueToCoefficientCalc(x1Val));
                x2Value = (float)(top + (availableSize.Width) * cartesianTransformer.XAxis.ValueToCoefficientCalc(x2Val));
                y1Value = (float)(left + (availableSize.Height) * (1 - cartesianTransformer.YAxis.ValueToCoefficientCalc(y1Val)));
                y2Value = (float)(left + (availableSize.Height) * (1 - cartesianTransformer.YAxis.ValueToCoefficientCalc(y2Val)));
            }

            double _x1 = x1Value;
            double _x2 = x2Value;
            double _y1 = y1 > 0 ? y1Value : y2Value;
            double _y2 = y1 > 0 ? y2Value : y1Value;

            var spacingSegment = this as ISegmentSpacing;

            if (spacingSegment != null)
            {
                double spacing = spacingSegment.SegmentSpacing;
                if (spacing > 0 && spacing <= 1)
                {
                    double leftpos = spacingSegment.CalculateSegmentSpacing(spacing, _x2, _x1);
                    double rightpos = spacingSegment.CalculateSegmentSpacing(spacing, _x1, _x2);
                    _x1 = (float)(leftpos);
                    _x2 = (float)(rightpos);
                }
            }

            double diff = _x2 - _x1;
            width = (int)Area.SeriesClipRect.Width;
            height = (int)Area.SeriesClipRect.Height;

            selectedSegmentPixels.Clear();

            if (_y1 < _y2)
                selectedSegmentPixels = bmp.GetRectangle((int)width, (int)height, (int)(width - _y2), (int)(height - _x1 - diff), (int)(width - _y1), (int)(height - _x1), selectedSegmentPixels);
            else
                selectedSegmentPixels = bmp.GetRectangle((int)width, (int)height, (int)(width - y1), (int)(height - x1 - diff), (int)(width - y2), (int)(height - x1), selectedSegmentPixels);
        }

        #endregion

        #region Internal Override Methods

        /// <summary>
        /// Validate the datapoints for segment implementation.
        /// </summary>
        internal override void ValidateYValues()
        {
            foreach (var yValue in YValues)
            {
                if (double.IsNaN(yValue) && ShowEmptyPoints)
                {
                    ValidateDataPoints(YValues); break;
                }
            }
        }

        /// <summary>
        /// Validate the datapoints for segment implementation.
        /// </summary>
        internal override void ReValidateYValues(List<int>[] emptyPointIndex)
        {
            foreach (var item in emptyPointIndex)
            {
                foreach (var index in item)
                    YValues[index] = double.NaN;
            }
        }

        /// <summary>
        /// This method used to get the chart data at an index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal override ChartDataPointInfo GetDataPoint(int index)
        {
            if (this.ActualXAxis is CategoryAxis && !(this.ActualXAxis as CategoryAxis).IsIndexed)
            {
                IList<double> xValues = GroupedXValuesIndexes;
                dataPoint = null;
                if (index >= 0 && index < xValues.Count)
                {
                    dataPoint = new ChartDataPointInfo();

                    if (xValues.Count > index)
                        dataPoint.XData = xValues[index];

                    dataPoint.Index = index;
                    dataPoint.Series = this;

                    if (this is ColumnSeries || this is FastColumnBitmapSeries
                        || this is FastStackingColumnBitmapSeries || this is StackingColumnSeries)
                    {
                        if (GroupedSeriesYValues[0].Count > index)
                            dataPoint.YData = GroupedSeriesYValues[0][index];
                        if (GroupedActualData.Count > index)
                            dataPoint.Item = GroupedActualData[index];
                    }
                    else
                    {
                        if (YValues.Count > index)
                            dataPoint.YData = YValues[index];
                        if (ActualData.Count > index)
                            dataPoint.Item = ActualData[index];
                    }
                }

                return dataPoint;
            }
            else
            {
                IList<double> xValues = GetXValues();
                dataPoint = null;
                if (index >= 0 && index < xValues.Count)
                {
                    dataPoint = new ChartDataPointInfo();

                    if (xValues.Count > index)
                        dataPoint.XData = IsIndexed ? index : xValues[index];

                    if (YValues.Count > index)
                        dataPoint.YData = YValues[index];

                    dataPoint.Index = index;
                    dataPoint.Series = this;

                    if (ActualData.Count > index)
                        dataPoint.Item = ActualData[index];
                }

                return dataPoint;
            }
        }

        /// <summary>
        /// This method used to get the chart data at an index.
        /// </summary>
        /// <param name="mousePos"></param>
        /// <returns></returns>
        internal override void GeneratePixels()
        {
            if (Area != null && dataPoint != null)
            {
                if (this is FastColumnBitmapSeries)
                {
                    if (!IsTransposed)
                        GenerateColumnPixels();
                    else
                        GenerateBarPixels();
                }
                else if (this is FastBarBitmapSeries)
                {
                    if (!IsTransposed)
                        GenerateBarPixels();
                    else
                        GenerateColumnPixels();
                }
            }
        }

        #endregion

        #region Protected Internal Methods

        /// <summary>
        /// Method for Generate Points for XYDataSeries
        /// </summary>
        protected internal override void GeneratePoints()
        {
            if (YBindingPath != null)
                GeneratePoints(new string[] { YBindingPath }, YValues);
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
            YValues.Clear();
            GeneratePoints(new string[] { YBindingPath }, YValues);
            isPointValidated = false;

            var axis2D = ActualXAxis as ChartAxisBase2D;
            if (axis2D != null)
            {
                axis2D.CanAutoScroll = true;
            }

            this.UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            YValues.Clear();
            base.OnBindingPathChanged(args);
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            (obj as XyDataSeries).YBindingPath = this.YBindingPath;
            return base.CloneSeries(obj);
        }

        #endregion

        #region Private Static Methods

        private static void OnYBindingPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as XyDataSeries).OnBindingPathChanged(e);
        }

        #endregion

        #endregion
    }
}
