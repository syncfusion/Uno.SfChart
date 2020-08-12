using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Markup;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.Specialized;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Data;

namespace Syncfusion.UI.Xaml.Charts
{
    public partial class ChartBase
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="Legend"/> property.
        /// </summary>
        public static readonly DependencyProperty LegendProperty =
            DependencyProperty.Register(
                "Legend",
                typeof(object), 
                typeof(ChartBase),
                new PropertyMetadata(
                    null, 
                    OnLegendPropertyChanged));

        #endregion

        #region Fields

        #region Internal Fields

        internal IList<UIElement> LegendCollection;

        #endregion

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the legend.
        /// </summary>
        /// <value>
        /// The legend.
        /// </value>
        public object Legend
        {
            get { return (object)GetValue(LegendProperty); }
            set { SetValue(LegendProperty, value); }
        }

        #endregion

        #region Internal Properties

        internal ObservableCollection<ObservableCollection<LegendItem>> LegendItems
        {
            get;
            set;
        }

        #endregion

        #endregion

        #region Methods

        #region Internal Methods

        internal bool HasDataPointBasedLegend()
        {
            if (Legend != null)
            {
                var legendCollection = Legend as ChartLegendCollection;
                if (legendCollection != null)
                {
                    foreach (var chartLegend in legendCollection)
                    {
                        var legend = chartLegend as ChartLegend;
                        return legend.Series != null && VisibleSeries.Contains(legend.Series);
                    }
                }
                else
                {
                    var legend = Legend as ChartLegend;
                    if (legend != null)
                    {
                        return legend.Series != null && VisibleSeries.Contains(legend.Series);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Updates the legend arrange rect.
        /// </summary>
        /// <param name="legend">The legend.</param>
        internal void UpdateLegendArrangeRect(ChartLegend legend)
        {
            if (legend == null && RootPanelDesiredSize == null) return;
            var elemSize = legend.DesiredSize;
            if (Legend is IList && AreaType == ChartAreaType.CartesianAxes)
            {
                if (legend.XAxis == null || legend.YAxis == null) return;

                if (legend.InternalDockPosition == ChartDock.Floating)
                {
                    var xAxis = legend.XAxis;
                    var yAxis = legend.YAxis;
                    var rect = new Rect(xAxis.ArrangeRect.X, yAxis.ArrangeRect.Y, xAxis.ArrangeRect.Width, yAxis.ArrangeRect.Height);
                    UpdateLegendInside(legend, rect);
                }
            }
            else
            {
                if (AreaType == ChartAreaType.PolarAxes && legend.LegendPosition == LegendPosition.Outside)
                {
                    var actualRect = new Rect(0, 0, RootPanelDesiredSize.Value.Width, RootPanelDesiredSize.Value.Height);
                    switch (legend.InternalDockPosition)
                    {
                        case ChartDock.Top:
                            legend.ArrangeRect = new Rect(actualRect.Left, actualRect.Top, actualRect.Width, elemSize.Height);
                            break;
                        case ChartDock.Left:
                            legend.ArrangeRect = new Rect(actualRect.Left, actualRect.Top, elemSize.Width, actualRect.Height);
                            break;
                        case ChartDock.Right:
                            legend.ArrangeRect = new Rect(actualRect.Width + actualRect.Left, 0, elemSize.Width, actualRect.Height);
                            break;
                        case ChartDock.Bottom:
                            legend.ArrangeRect = new Rect(actualRect.Left, actualRect.Bottom, actualRect.Width, elemSize.Height);
                            break;
                        case ChartDock.Floating:
                            {
                                UpdateLegendInside(legend, actualRect);
                                break;
                            }
                    }
                }
                else
                {
                    var actualRect = AreaType == ChartAreaType.None ? new Rect(0, 0, RootPanelDesiredSize.Value.Width, RootPanelDesiredSize.Value.Height) : SeriesClipRect;
                    switch (legend.InternalDockPosition)
                    {
                        case ChartDock.Top:
                            legend.ArrangeRect = new Rect(actualRect.Left, actualRect.Top - AxisThickness.Top, actualRect.Width, elemSize.Height);
                            break;
                        case ChartDock.Left:
                            legend.ArrangeRect = new Rect(actualRect.Left - AxisThickness.Left, actualRect.Top, elemSize.Width, actualRect.Height);
                            break;
                        case ChartDock.Right:
                            legend.ArrangeRect = new Rect(actualRect.Width + actualRect.Left + AxisThickness.Right, 0, elemSize.Width, actualRect.Height);
                            break;
                        case ChartDock.Bottom:
                            legend.ArrangeRect = new Rect(actualRect.Left, actualRect.Bottom + AxisThickness.Bottom, actualRect.Width, elemSize.Height);
                            break;
                        case ChartDock.Floating:
                            {
                                UpdateLegendInside(legend, AreaType == ChartAreaType.CartesianAxes ? seriesClipRect : actualRect);
                                break;
                            }
                    }
                }
            }
        }

        internal void InitializeLegendItems()
        {
            if (Legend is ChartLegendCollection)
            {
                this.LegendItems = new ObservableCollection<ObservableCollection<LegendItem>>();
                for (int i = 0; i < (Legend as ChartLegendCollection).Count; i++)
                {
                    LegendItems.Add(new ObservableCollection<LegendItem>());
                }
            }
            else
                this.LegendItems = new ObservableCollection<ObservableCollection<LegendItem>>() { new ObservableCollection<LegendItem>() };
        }

        /// <summary>
        /// Updates the legend arrange rect.
        /// </summary>
        internal void UpdateLegendArrangeRect()
        {
            if (LegendCollection == null) return;
            foreach (var legend in LegendCollection.OfType<ChartLegend>())
            {
                UpdateLegendArrangeRect(legend);
            }
            var axisLayout = ChartAxisLayoutPanel as ChartCartesianAxisLayoutPanel;
            if (axisLayout != null)
                axisLayout.UpdateLegendsArrangeRect();
        }

        internal void LayoutLegends()
        {
            for (int i = 0; i < RowDefinitions.Count; i++)
            {
                int leftIndex = 0, rightIndex = 0, currLeftPos = 0, currRightPos = 0;
                RowDefinitions[i].Legends.Clear();

                foreach (ChartLegend item in LegendCollection)
                {
                    if (GetActualRow(item) == i && item.LegendPosition != LegendPosition.Inside)
                    {
                        if (item.DockPosition == ChartDock.Left)
                        {
                            RowDefinitions[i].Legends.Add(item);
                            if (leftIndex < currLeftPos)
                                leftIndex++;
                            item.RowColumnIndex = leftIndex;
                            currLeftPos++;
                        }
                        else if (item.DockPosition == ChartDock.Right)
                        {
                            RowDefinitions[i].Legends.Add(item);
                            if (rightIndex < currRightPos)
                                rightIndex++;
                            item.RowColumnIndex = rightIndex;
                            currRightPos++;
                        }
                        if (ChartDockPanel.GetDock(item) != item.InternalDockPosition)
                            ChartDockPanel.SetDock(item, item.InternalDockPosition);
                    }
                }
            }

            for (int i = 0; i < ColumnDefinitions.Count; i++)
            {
                int topIndex = 0, bottomIndex = 0, currTopIndex = 0, currBottomIndex = 0;
                ColumnDefinitions[i].Legends.Clear();

                foreach (ChartLegend item in LegendCollection)
                {
                    if (GetActualColumn(item) == i && item.LegendPosition != LegendPosition.Inside)
                    {
                        if (item.DockPosition == ChartDock.Top)
                        {
                            if (topIndex < currTopIndex)
                                topIndex++;
                            item.RowColumnIndex = topIndex;
                            currTopIndex++;
                            ColumnDefinitions[i].Legends.Add(item);
                        }
                        else if (item.DockPosition == ChartDock.Bottom)
                        {
                            if (bottomIndex < currBottomIndex)
                                bottomIndex++;
                            item.RowColumnIndex = bottomIndex;
                            currBottomIndex++;
                            ColumnDefinitions[i].Legends.Add(item);
                        }
                    }
                    if (ChartDockPanel.GetDock(item) != item.InternalDockPosition)
                        ChartDockPanel.SetDock(item, item.InternalDockPosition);
                }
            }
        }

        internal void UpdateLegend(object newLegend, bool isCollectionChanged)
        {
            try
            {
                if (ChartDockPanel == null) return;
                if (LegendCollection != null && isCollectionChanged)
                {
                    foreach (var item in LegendCollection.Where(item => ChartDockPanel.Children.Contains(item)))
                    {
                        ChartDockPanel.Children.Remove(item);
                    }
                }

                var sfChart = this as SfChart;
                if (newLegend == null || (sfChart != null && sfChart.Series == null)) return;

                LegendCollection = new List<UIElement>();

                var legendColl = newLegend as ChartLegendCollection;

                if (legendColl != null)
                {
                    legendColl.CollectionChanged -= LegendCollectionChanged;
                    legendColl.CollectionChanged += LegendCollectionChanged;

                    foreach (var item in legendColl)
                    {
                        LegendCollection.Add(item);
                    }
                    LayoutLegends();
                }
                else
                {
                    LegendCollection.Add(newLegend as UIElement);
                }
                int k = 0;
                if (LegendItems != null)
                {
                    foreach (ChartLegend item in LegendCollection)
                    {
                        if (!ChartDockPanel.Children.Contains(item))
                        {
                            ChartDockPanel.Children.Add(item);
                        }
                        if (item.Items.Count == 0)
                            item.ItemsSource = LegendItems[k];
                        SetLegendItemsSource(item, k);
                        k++;
                    }
                }
            }
            catch
            {
            }
        }

        internal void SetLegendItemsSource(ChartLegend legend, int index)
        {
            if (legend == null || ActualSeries == null) return;
            legend.ChartArea = this;
            //HasDataPointBasedLegend() will return true when need to generate data point based legend items. 
            if ((VisibleSeries.Count == 1 && VisibleSeries[0].IsSingleAccumulationSeries) || HasDataPointBasedLegend())
            {
                SetLegendItems(legend, index);
            }
            else if (VisibleSeries.Count == 1 && VisibleSeries[0] is WaterfallSeries)
            {
                SetWaterfallLegendItems(legend, index);
                legend.XAxis = VisibleSeries[0].IsActualTransposed ? (VisibleSeries[0] as ISupportAxes).ActualYAxis : (VisibleSeries[0] as ISupportAxes).ActualXAxis;
                legend.YAxis = VisibleSeries[0].IsActualTransposed ? (VisibleSeries[0] as ISupportAxes).ActualXAxis : (VisibleSeries[0] as ISupportAxes).ActualYAxis;
            }
            else
            {
                var technicalIndicators = new List<ChartSeriesBase>();
                var sfChart = this as SfChart;
                if (sfChart != null)
                {
                    foreach (ChartSeries indicator in sfChart.TechnicalIndicators)
                    {
                        technicalIndicators.Add(indicator as ChartSeriesBase);
                    }
                }

                List<ChartSeriesBase> actualLegendSeries = sfChart != null ? ActualSeries.Union(technicalIndicators).ToList() : ActualSeries;
                if (AreaType == ChartAreaType.PolarAxes || AreaType == ChartAreaType.CartesianAxes && Legend is ChartLegendCollection)
                    actualLegendSeries = (from actualSeries in actualLegendSeries
                                          where
                                              GetActualRow(legend) == GetActualRow(actualSeries.IsActualTransposed ? actualSeries.ActualXAxis : actualSeries.ActualYAxis) &&
                                              GetActualColumn(legend) == GetActualColumn(actualSeries.IsActualTransposed ? actualSeries.ActualYAxis : actualSeries.ActualXAxis)
                                          select actualSeries).ToList();

                if (actualLegendSeries.Count > 0 && actualLegendSeries[0] is ISupportAxes)
                {
                    legend.XAxis = actualLegendSeries[0].IsActualTransposed ? (actualLegendSeries[0] as ISupportAxes).ActualYAxis : (actualLegendSeries[0] as ISupportAxes).ActualXAxis;
                    legend.YAxis = actualLegendSeries[0].IsActualTransposed ? (actualLegendSeries[0] as ISupportAxes).ActualXAxis : (actualLegendSeries[0] as ISupportAxes).ActualYAxis;
                }

                IEnumerable<ChartSeriesBase> legendSeries;
                switch (AreaType)
                {
                    case ChartAreaType.CartesianAxes:

                        legendSeries = from series in actualLegendSeries
                                       where series is ISupportAxes2D
                                       select series;
                        break;
                    case ChartAreaType.PolarAxes:
                        legendSeries = from series in actualLegendSeries
                                       where series is PolarRadarSeriesBase
                                       select series;
                        break;
                    default:
                        legendSeries = from series in actualLegendSeries
                                       where series is AccumulationSeriesBase
                                       select series;
                        break;
                }
                var chartSeries = legendSeries as ChartSeries[] ?? legendSeries.ToArray();

                foreach (var series in chartSeries)
                {
                    if (series.IsSeriesVisible)
                    {
                        if (LegendItems[index].Count > 0 && index < LegendItems[index].Count)//Legend not cleared when bind Series in MVVM view model-WPF-18625
                        {
                            LegendItems[index].Clear();
                        }
                    }
                }
                foreach (var item in chartSeries.Where(series => series.VisibilityOnLegend == Visibility.Visible))
                {
                    var containlegenditem = LegendItems[index].Where(it => it.Series == item).ToList();
                    if (LegendItems.Count == 0 || containlegenditem.Count() == 0)
                    {
                        var legenditem = new LegendItem { Legend = legend, Series = item };
                        if (item.IsSingleAccumulationSeries)
                        {
                            legenditem.Legend.isSegmentsUpdated = true;
                            if (item.IsSeriesVisible)
                                legenditem.IsSeriesVisible = true;
                            else
                                legenditem.IsSeriesVisible = false;
                            legenditem.Legend.isSegmentsUpdated = false;
                        }
                        LegendItems[index].Add(legenditem);
                    }
                }
                foreach (var item in chartSeries)
                {
                    var cartesian = item as CartesianSeries;
                    if (cartesian != null)
                        foreach (var trend in cartesian.Trendlines.Where(
                                    trendline => trendline.VisibilityOnLegend == Visibility.Visible))
                        {
                            var containlegenditem = LegendItems[index].Where(it => it.Trendline == trend).ToList();
                            if (LegendItems.Count == 0 || containlegenditem.Count() == 0)
                            {
                                var trendline = new LegendItem { Legend = legend, Trendline = trend };
                                LegendItems[index].Add(trendline);
                                trend.UpdateLegendIconTemplate(true);
                            }
                        }
                }

            }
        }

        internal void LegendCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var newLegend = e.NewItems[0] as ChartLegend;
                        LegendItems.Add(new ObservableCollection<LegendItem>());
                        newLegend.ItemsSource = LegendItems[LegendItems.Count - 1];
                        SetLegendItemsSource(newLegend, LegendItems.Count - 1);
                        LegendCollection.Add(newLegend);
                        ChartDockPanel.Children.Add(newLegend);
                        LayoutLegends();
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        var removedLegend = e.OldItems[0] as ChartLegend;
                        if (LegendCollection.Contains(removedLegend))
                            LegendCollection.Remove(removedLegend);
                        if (ChartDockPanel.Children.Contains(removedLegend))
                            ChartDockPanel.Children.Remove(removedLegend);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (var item in LegendCollection)
                    {
                        ChartDockPanel.Children.Remove(item);
                    }
                    LegendCollection.Clear();
                    break;
            }
        }

        #endregion

        #region Private Static Methods
        
        private static void OnLegendPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var area = d as ChartBase;
            if (area != null)
            {
                area.OnLegendPropertyChanged(e);
            }
        }

        private static void UpdateLegendInside(ChartLegend legend, Rect rect)
        {
            double x = 0d, y = 0d, width = 0d, height = 0d;
            switch (legend.DockPosition)
            {
                case ChartDock.Bottom:
                    y = rect.Y + rect.Height - legend.DesiredSize.Height;
                    x = GetHorizontalLegendAlignment(legend.HorizontalAlignment, rect, legend.DesiredSize);
                    width = rect.Width;
                    height = legend.DesiredSize.Height;
                    break;
                case ChartDock.Top:
                    y = rect.Y;
                    x = GetHorizontalLegendAlignment(legend.HorizontalAlignment, rect, legend.DesiredSize);
                    width = rect.Width;
                    height = legend.DesiredSize.Height;
                    break;
                case ChartDock.Right:
                    y = GetVerticalLegendAlignment(legend.VerticalAlignment, rect, legend.DesiredSize);
                    x = rect.X + rect.Width - legend.DesiredSize.Width;
                    width = legend.DesiredSize.Width;
                    height = rect.Height;
                    break;
                case ChartDock.Left:
                    y = GetVerticalLegendAlignment(legend.VerticalAlignment, rect, legend.DesiredSize);
                    x = rect.X;
                    width = legend.DesiredSize.Width;
                    height = rect.Height;
                    break;
                case ChartDock.Floating:
                    x = rect.Left;
                    y = rect.Top;
                    width = legend.DesiredSize.Width;
                    height = legend.DesiredSize.Height;
                    break;
            }
            legend.ArrangeRect = new Rect(x, y, width, height);
        }

        private static double GetHorizontalLegendAlignment(HorizontalAlignment alignment, Rect rect, Size desiredSize)
        {
            var left = rect.Left;
            if (alignment == HorizontalAlignment.Center)
            {
                left = left + (rect.Width / 2) - desiredSize.Width / 2;
            }
            else if (alignment == HorizontalAlignment.Right)
            {
                left = left + rect.Width - desiredSize.Width;
            }
            return left;
        }

        private static double GetVerticalLegendAlignment(VerticalAlignment alignment, Rect rect, Size desiredSize)
        {
            var top = rect.Top;
            if (alignment == VerticalAlignment.Center)
            {
                top = top + (rect.Height / 2) - desiredSize.Height / 2;
            }
            else if (alignment == VerticalAlignment.Bottom)
            {
                top = top + rect.Height - desiredSize.Height;
            }
            return top;
        }

        #endregion

        #region Private Methods

        private void OnLegendPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            InitializeLegendItems();
            UpdateLegend(e.NewValue, true);
        }

        private void SetLegendItems(ChartLegend legend, int index)
        {
            var series = HasDataPointBasedLegend() ? legend.Series : VisibleSeries[0];
            var yvalues = series.ActualSeriesYValues[0] as List<double>;//When EmptyPoint is set last legend is not rendered.-WPF-18794
            var circularSeries = series as CircularSeriesBase;
            bool isGroupTo = circularSeries != null && !double.IsNaN(circularSeries.GroupTo);

            if (isGroupTo)
            {
                var groupToYValues = circularSeries.GetGroupToYValues();
                yvalues = groupToYValues.Item1 as List<double>;
            }

            LegendItems[index].Clear();
            int j = 0;
            var Segments = series.Segments.ToList();
            if (series is FunnelSeries)
            {
                Segments.Reverse();
            }
            for (int i = 0; i < yvalues.Count; i++)
            {
                var legendIndex = i;
                if (double.IsNaN(yvalues[i]))
                {
                    if (!(series is TriangularSeriesBase))
                        j++;
                    continue;
                }
                LegendItem legenditem;

                //Created duplicate legends to measure if segments are not created WPF-26728
                if (Segments.Count == 0)
                {
                    legenditem = new LegendItem
                    {
                        Index = legendIndex,
                        Series = series,
                        Legend = legend
                    };
                }
                else
                {
                    var item = Segments[j];
                    legenditem = new LegendItem
                    {
                        Index = legendIndex, //WPF-14337 Legend value wrong while using empty point in accumulation series
                        Item = item.Item,
                        Series = series,
                        Legend = legend
                    };

                    if (series.ToggledLegendIndex.Count > 0 && (legend.CheckBoxVisibility == Visibility.Visible || legend.ToggleSeriesVisibility))
                    {
                        if (!item.IsSegmentVisible)
                        {
                            legend.isSegmentsUpdated = true;
                            legenditem.IsSeriesVisible = false;
                            legend.isSegmentsUpdated = false;
                        }
                    }

                    Binding binding = new Binding();
                    binding.Source = item;
                    binding.Path = new PropertyPath("Interior");
                    BindingOperations.SetBinding(legenditem, LegendItem.InteriorProperty, binding);
                }

                if (isGroupTo && circularSeries.GroupedData.Count > 0 && legenditem.Index == Segments.Count - 1)
                    legenditem.Label = circularSeries.GroupingLabel;
                else if (legenditem.Item != null)
                    legenditem.Label = series.GetActualXValue(series.ActualData.IndexOf(legenditem.Item)).ToString();
                else
                    legenditem.Label = series.GetActualXValue(legenditem.Index).ToString();
                LegendItems[index].Add(legenditem);

                j++;
            }
        }

        private void SetWaterfallLegendItems(ChartLegend legend, int index)
        {
            LegendItems[index].Clear();
            LegendItem legendItem;

            var positiveSegment = VisibleSeries[0].Segments.FirstOrDefault
                (seg => (seg as WaterfallSegment).SegmentType == WaterfallSegmentType.Positive);

            if (positiveSegment != null)
            {
                legendItem = GetWaterfallLegendItem(positiveSegment, SfChartResourceWrapper.Increase, 0, legend);

                LegendItems[index].Add(legendItem);
            }

            var negativeSegment = VisibleSeries[0].Segments.FirstOrDefault
                (seg => (seg as WaterfallSegment).SegmentType == WaterfallSegmentType.Negative);

            if (negativeSegment != null)
            {
                legendItem = GetWaterfallLegendItem(negativeSegment, SfChartResourceWrapper.Decrease, 1, legend);

                LegendItems[index].Add(legendItem);
            }

            var sumSegment = VisibleSeries[0].Segments.FirstOrDefault(seg => (seg as WaterfallSegment).SegmentType == WaterfallSegmentType.Sum);

            if (sumSegment != null)
            {
                legendItem = GetWaterfallLegendItem(sumSegment, SfChartResourceWrapper.Total, 2, legend);

                LegendItems[index].Add(legendItem);
            }
        }

        private LegendItem GetWaterfallLegendItem(object source, string label, int index, ChartLegend legend)
        {
            var legenditem = new LegendItem
            {
                Index = index,
                Series = VisibleSeries[0],
                Legend = legend
            };

            Binding binding = new Binding();
            binding.Source = source;
            binding.Path = new PropertyPath("Interior");
            BindingOperations.SetBinding(legenditem, LegendItem.InteriorProperty, binding);
            legenditem.Label = label;

            return legenditem;
        }

        #endregion

        #endregion
    }
}
