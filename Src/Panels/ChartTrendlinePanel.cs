// <copyright file="ChartTrendlinePanel.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Represents a canvas panel that update the children while changing trend line collection. 
    /// </summary>
    /// <seealso cref="System.Windows.Controls.Canvas" />
    public partial class ChartTrendlinePanel : Canvas
    {
        #region Fields

        private TrendlineBase trend;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartTrendlinePanel"/> class.
        /// </summary>
        public ChartTrendlinePanel()
        {
        }

        #endregion

        #region Properties

        #region Internal Properties

        internal TrendlineBase Trend
        {
            get
            {
                return trend;
            }

            set
            {
                if (Trend != null)
                {
                    Trend.TrendlineSegments.CollectionChanged -= OnSegmentsCollectionChanged;
                }

                trend = value;

                if (trend != null)
                {
                    trend.TrendlineSegments.CollectionChanged += OnSegmentsCollectionChanged;
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Internal Methods

        /// <summary>
        /// Updates the <see cref="ChartTrendlinePanel"/>.
        /// </summary>
        /// <param name="finalSize">The Final Size</param>
        internal void Update(Size finalSize)
        {
            IChartTransformer chartTransformer = Trend.Series.CreateTransformer(finalSize, true);

            foreach (var segment in Trend.TrendlineSegments)
            {
                segment.Update(chartTransformer);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the <see cref="ChartTrendlinePanel"/> when segment collection changed.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event Arguments</param>
        private void OnSegmentsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var segment = e.NewItems[0] as ChartSegment;
                if (!segment.IsAddedToVisualTree)
                {
                    UIElement element = segment.CreateVisual(Size.Empty);
                    if (element != null)
                    {
                        Children.Add(element);
                        segment.IsAddedToVisualTree = true;
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var segment = e.OldItems[0] as ChartSegment;
                if (segment.IsAddedToVisualTree)
                {
                    UIElement element = segment.GetRenderedVisual();
                    if (element != null && Children.Contains(element))
                    {
                        Children.Remove(element);
                        segment.IsAddedToVisualTree = false;
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Children.Clear();
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                var segment = e.NewItems[0] as ChartSegment;
                if (!segment.IsAddedToVisualTree)
                {
                    UIElement element = segment.CreateSegmentVisual(Size.Empty);

                    if (element != null)
                    {
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
