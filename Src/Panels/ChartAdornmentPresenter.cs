// <copyright file="ChartAdornmentPresenter.cs" company="Syncfusion. Inc">
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
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using Windows.Foundation;
    using System.Threading.Tasks;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;

    /// <summary>
    /// Represents <see cref="ChartAdornmentPresenter"/> class.
    /// </summary>
    public partial class ChartAdornmentPresenter : Canvas
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="VisibleSeries"/> property.
        /// </summary>
        public static readonly DependencyProperty VisibleSeriesProperty =
            DependencyProperty.Register(
                "VisibleSeries",
                typeof(ObservableCollection<ChartSeriesBase>),
                typeof(ChartAdornmentPresenter), new PropertyMetadata(null,
                new PropertyChangedCallback(OnVisibleSeriesPropertyChanged)));

        /// <summary>
        ///  The DependencyProperty for <see cref="Series"/> property.
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(
                "Series",
                typeof(ChartSeriesBase),
                typeof(ChartAdornmentPresenter),
                new PropertyMetadata(null));

        #endregion

        #region Fields

        private List<int> resetIndexes = new List<int>();

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the VisibleSeries. This is a dependency property.
        /// </summary>
        /// <value>The VisibleSeries.</value>
        public ObservableCollection<ChartSeriesBase> VisibleSeries
        {
            get { return (ObservableCollection<ChartSeriesBase>)GetValue(VisibleSeriesProperty); }
            set { SetValue(VisibleSeriesProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Series collection in Chart.
        /// </summary>
        public ChartSeriesBase Series
        {
            get { return (ChartSeriesBase)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        #endregion

        #endregion

        #region Methods

        #region Internal Methods

        /// <summary>
        /// Method is used to highlight the adornment.
        /// </summary>
        /// <param name="adornmentSelectedIndex">The Adornment Index</param>
        internal void UpdateAdornmentSelection(List<int> selectedAdornmentIndexes, bool isDataPointSelection)
        {
            Brush selectionBrush = null;

            if (Series.ActualArea.GetEnableSeriesSelection() && !isDataPointSelection)
                selectionBrush = Series.ActualArea.GetSeriesSelectionBrush(Series);
            else if (Series.ActualArea.GetEnableSegmentSelection())
                selectionBrush = (Series as ISegmentSelectable).SegmentSelectionBrush;

            if (selectionBrush != null)
            {
                if (selectedAdornmentIndexes != null && selectedAdornmentIndexes.Count > 0)
                {
                    foreach (var index in selectedAdornmentIndexes)
                    {
                        // Set selection brush to adornment label.
                        if (Series.adornmentInfo.ShowLabel && Series.adornmentInfo.LabelTemplate == null
                         && (Series.adornmentInfo.UseSeriesPalette || Series.adornmentInfo.Background != null || Series.adornmentInfo.BorderBrush != null))
                        {
                            Border border = null;

                            if (Series.adornmentInfo.LabelPresenters.Count > 0 && VisualTreeHelper.GetChildrenCount(Series.adornmentInfo.LabelPresenters[index]) > 0)
                            {
                                ContentPresenter contentPresenter = VisualTreeHelper.GetChild(Series.adornmentInfo.LabelPresenters[index], 0) as ContentPresenter;

                                if (VisualTreeHelper.GetChildrenCount(contentPresenter) > 0)
                                {
                                    border = VisualTreeHelper.GetChild(contentPresenter, 0) as Border;
                                }
                            }

                            if (border != null)
                            {
                                if (border.Background != null)
                                    border.Background = selectionBrush;
                                if (border.BorderBrush != null)
                                    border.BorderBrush = selectionBrush;
                            }

                        }

                        // Set selection brush to adornment symbol
                        if (Series.adornmentInfo.ShowMarker && Series.adornmentInfo.adormentContainers.Count > 0)
                        {
                            ChartAdornmentContainer symbol = Series.adornmentInfo.adormentContainers[index];

                            if (symbol.PredefinedSymbol != null)
                            {
                                symbol.PredefinedSymbol.Background = selectionBrush;
                                symbol.PredefinedSymbol.BorderBrush = selectionBrush;
                            }
                        }

                        // Set selection brush to adornment connector line.
                        if (Series.adornmentInfo.ConnectorLines.Count > 0 && Series.adornmentInfo.ShowConnectorLine)
                        {
                            Path line = Series.adornmentInfo.ConnectorLines[index];
                            line.Stroke = selectionBrush;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method is used to reset the adornment.
        /// </summary>
        internal void ResetAdornmentSelection(int? selectedIndex, bool isResetAll)
        {

            if (!isResetAll && selectedIndex != null
                && Series.ActualArea.SelectedSeriesCollection.Contains(Series)
                && Series.ActualArea.GetEnableSeriesSelection())
            {
                List<int> indexes = (from adorment in Series.Adornments
                                     where Series.ActualData[selectedIndex.Value] == adorment.Item
                                     select Series.Adornments.IndexOf(adorment)).ToList();

                UpdateAdornmentSelection(indexes, false);
            }
            else if (selectedIndex < Series.DataCount)
            {
                resetIndexes.Clear();

                if (selectedIndex != null && !isResetAll)
                    if (Series is CircularSeriesBase && !double.IsNaN(((CircularSeriesBase)Series).GroupTo))
                        resetIndexes = (from adorment in Series.Adornments
                                   where Series.Segments[selectedIndex.Value].Item == adorment.Item
                                   select Series.Adornments.IndexOf(adorment)).ToList();
                    else if (Series.ActualXAxis is CategoryAxis && !(Series.ActualXAxis as CategoryAxis).IsIndexed
                    && (Series.IsSideBySide && (!(Series is RangeSeriesBase))
                    && (!(Series is FinancialSeriesBase)) && !(Series is WaterfallSeries)))
                        resetIndexes = (from adorment in Series.Adornments
                                        where Series.GroupedActualData[selectedIndex.Value] == adorment.Item
                                        select Series.Adornments.IndexOf(adorment)).ToList();
                    else
                        resetIndexes = (from adorment in Series.Adornments
                                        where Series.ActualData[selectedIndex.Value] == adorment.Item
                                        select Series.Adornments.IndexOf(adorment)).ToList();
                else if (isResetAll)
                    resetIndexes = (from adorment in Series.Adornments
                                    select Series.Adornments.IndexOf(adorment)).ToList();

                foreach (var index in resetIndexes)
                {
                    // Reset the adornment label
                    if (Series.adornmentInfo.LabelPresenters.Count > index && Series.adornmentInfo.ShowLabel
                        && (Series.adornmentInfo.UseSeriesPalette || Series.adornmentInfo.Background != null || Series.adornmentInfo.BorderBrush != null))
                    {
                        Border border = null;
                        if (VisualTreeHelper.GetChildrenCount(Series.adornmentInfo.LabelPresenters[index]) > 0)
                        {
                            ContentPresenter contentPresenter = VisualTreeHelper.GetChild(Series.adornmentInfo.LabelPresenters[index], 0) as ContentPresenter; ;

                            if (VisualTreeHelper.GetChildrenCount(contentPresenter) > 0)
                                border = VisualTreeHelper.GetChild(contentPresenter as ContentPresenter, 0) as Border;
                        }

                        if (border != null)
                        {
                            if (Series.Adornments[index].Background != null)
                                border.Background = Series.Adornments[index].Background;
                            else if (Series.adornmentInfo.UseSeriesPalette)
                                border.Background = Series.Adornments[index].Interior;

                            if (Series.adornmentInfo.BorderBrush != null)
                                border.BorderBrush = Series.Adornments[index].BorderBrush;
                            else if (Series.adornmentInfo.UseSeriesPalette)
                                border.BorderBrush = Series.Adornments[index].Interior;
                        }
                    }

                    // Reset the adornment connector line
                    if (Series.adornmentInfo.ConnectorLines.Count > index && Series.adornmentInfo.ShowConnectorLine)
                    {
                        Path path = Series.adornmentInfo.ConnectorLines[index];

                        if (Series.adornmentInfo.UseSeriesPalette && Series.adornmentInfo.ConnectorLineStyle == null)
                            path.Stroke = Series.Adornments[index].Interior;
                        else
                            path.ClearValue(Line.StrokeProperty);
                    }

                    // Reset the adornment symbol
                    if (Series.adornmentInfo.adormentContainers.Count > index)
                    {
                        SymbolControl symbol = Series.adornmentInfo.adormentContainers[index].PredefinedSymbol;

                        if (symbol != null && Series.adornmentInfo.ShowMarker)
                        {
                            Binding binding;

                            if (Series.adornmentInfo.SymbolInterior == null)
                            {
                                binding = new Binding();
                                binding.Source = Series.Adornments[index];
                                binding.Path = new PropertyPath("Interior");
                                symbol.SetBinding(SymbolControl.BackgroundProperty, binding);
                            }
                            else
                            {
                                binding = new Binding();
                                binding.Source = Series.adornmentInfo;
                                binding.Path = new PropertyPath("SymbolInterior");
                                symbol.SetBinding(SymbolControl.BackgroundProperty, binding);
                            }

                            if (Series.adornmentInfo.SymbolStroke == null)
                            {
                                binding = new Binding();
                                binding.Source = Series.Adornments[index];
                                binding.Path = new PropertyPath("Interior");
                                symbol.SetBinding(SymbolControl.BorderBrushProperty, binding);
                            }
                            else
                            {
                                binding = new Binding();
                                binding.Source = Series.adornmentInfo;
                                binding.Path = new PropertyPath("SymbolStroke");
                                symbol.SetBinding(SymbolControl.BorderBrushProperty, binding);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the <see cref="ChartAdornmentPresenter"/>.
        /// </summary>
        /// <param name="availableSize">The Available Size</param>
        internal void Update(Size availableSize)
        {
            if (Series != null && Series.adornmentInfo != null)
            {
                Series.adornmentInfo.Measure(availableSize, this);
            }
        }

        /// <summary>
        /// Arranges the adornment elements.
        /// </summary>
        /// <param name="finalSize">The Final Size.</param>
        internal void Arrange(Size finalSize)
        {
            if (Series != null && Series.adornmentInfo != null)
            {
                Series.adornmentInfo.Arrange(finalSize);
            }
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Updates the visibility of the <see cref="ChartAdornmentPresenter"/>.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnVisibleSeriesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #endregion
    }
}
