// <copyright file="ChartSeriesPanel.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System;
    using System.Collections.Specialized;
    using System.Windows;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Represents the panel where the series segments and adornments will be placed.
    /// </summary>
    public partial class ChartSeriesPanel : Canvas
    {
        #region Fields

        private ChartSeries series;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSeriesPanel"/> class.
        /// </summary>
        public ChartSeriesPanel()
        {
        }

        #endregion

        #region Properties

        #region Internal Properties

        /// <summary>
        /// Gets or sets the <see cref="ChartSeriesPanel"/> series.
        /// </summary>
        internal ChartSeries Series
        {
            get
            {
                return series;
            }

            set
            {
                if (series != null)
                {
                    series.Segments.CollectionChanged -= OnSegmentsCollectionChanged;
                }

                series = value;

                if (series != null)
                {
                    series.Segments.CollectionChanged += OnSegmentsCollectionChanged;
                    AddItems();
                }
            }
        }

        #endregion

        #endregion

        #region Methods
        
        #region Internal Methods

        internal void Dispose()
        {
            if (series != null && series.Segments != null)
            {
                series.Segments.CollectionChanged -= OnSegmentsCollectionChanged;
            }
            Children.Clear();
            series = null;
        }

        /// <summary>
        /// Updates the panel.
        /// </summary>
        /// <param name="finalSize"></param>
        internal void Update(Size finalSize)
        {
            bool canUpdate = !(Series is ISupportAxes);
            if (Series is ISupportAxes &&
                Series.ActualXAxis != null && Series.ActualYAxis != null)
            {
                canUpdate = true;
                if (Series.Area != null)
                {
                    Series.Area.ClearBuffer();
                }
            }

            if (canUpdate)
            {
                IChartTransformer chartTransformer = Series.CreateTransformer(finalSize, true);
                if (Series is CircularSeriesBase)
                {
                    CircularSeriesBase circularSeries = Series as CircularSeriesBase;
                    Rect rect = ChartLayoutUtils.Subtractthickness(new Rect(new Point(), finalSize), Series.Margin);
                    chartTransformer = Series.CreateTransformer(new Size(rect.Width, rect.Height), true);
                    var pieSeries = circularSeries as PieSeries;
                    double coefficient = pieSeries != null ? pieSeries.InternalPieCoefficient : (circularSeries as DoughnutSeries).InternalDoughnutCoefficient;
                    double radius = coefficient * Math.Min(chartTransformer.Viewport.Width, chartTransformer.Viewport.Height) / 2;
                    circularSeries.Center = circularSeries.GetActualCenter(new Point(chartTransformer.Viewport.Width * 0.5d, chartTransformer.Viewport.Height * 0.5d), radius);
                }

                Series.Pixels.Clear();
                Series.bitmapPixels.Clear();
                Series.bitmapRects.Clear();
                foreach (ChartSegment segment in Series.Segments)
                {
                    segment.Update(chartTransformer);
                }

                if (Series.CanAnimate && Series.Segments.Count > 0)
                {
                    Series.Animate();
                    Series.CanAnimate = false;
                }

                if (Series.IsLoading)
                {
                    Series.IsLoading = false;
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the chart on segment collection changed.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Collection Changed Event Arguments</param>
        private void OnSegmentsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                ChartSegment segment = e.NewItems[0] as ChartSegment;

                if (!segment.IsAddedToVisualTree)
                {
                    UIElement element = segment.CreateSegmentVisual(Size.Empty);
                    if (element != null)
                    {
                        var doughnutSeries = Series as DoughnutSeries;
                        if (doughnutSeries != null)
                        {
                            doughnutSeries.ManipulateAdditionalVisual(element, e.Action);
                        }

                        Children.Add(element);
                        segment.IsAddedToVisualTree = true;
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                ChartSegment segment = e.OldItems[0] as ChartSegment;
                if (segment.IsAddedToVisualTree)
                {
                    UIElement element = segment.GetRenderedVisual();
                    if (element != null && Children.Contains(element))
                    {
                        var doughnutSeries = Series as DoughnutSeries;
                        if (doughnutSeries != null)
                        {
                            doughnutSeries.ManipulateAdditionalVisual(element, e.Action);                           
                        }

                        Children.Remove(element);
                        segment.IsAddedToVisualTree = false;
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                for (int i = Children.Count - 1; i > -1; i--)
                {
                    if (!(Children[i] is TrendlineBase))
                    {
                        Children.RemoveAt(i);
                    }
                }

            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                ChartSegment segment = e.NewItems[0] as ChartSegment;
                if (!segment.IsAddedToVisualTree)
                {
                    UIElement element = segment.CreateSegmentVisual(Size.Empty);

                    var doughnutSeries = Series as DoughnutSeries;
                    if (doughnutSeries != null)
                    {
                        doughnutSeries.ManipulateAdditionalVisual(element, e.Action);
                    }

                    Children.Add(element);
                    segment.IsAddedToVisualTree = true;
                }

                foreach (ChartSegment item in e.OldItems)
                {
                    var element = item.GetRenderedVisual();
                    if (Children.Contains(element))
                        Children.Remove(element);
                    item.IsAddedToVisualTree = false;
                }
            }
        }

        /// <summary>
        /// Adds the segment visuals to <see cref="ChartSeriesPanel"/>
        /// </summary>
        private void AddItems()
        {
            foreach (ChartSegment segment in Series.Segments)
            {
                if (!segment.IsAddedToVisualTree)
                {
                    UIElement element = segment.CreateSegmentVisual(Size.Empty);
                    if (element != null)
                    {
                        var doughnutSeries = Series as DoughnutSeries;
                        if (doughnutSeries != null)
                        {
                            doughnutSeries.ManipulateAdditionalVisual(element, NotifyCollectionChangedAction.Add);
                        }

                        Children.Add(element);
                        segment.IsAddedToVisualTree = true;
                    }
                }
            }
        }
        
        #endregion

        #endregion
    }
}
