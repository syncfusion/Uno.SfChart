// <copyright file="ChartAdornmentContainer.cs" company="Syncfusion. Inc">
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
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the panel which contains all the ChartAdornment elements.
    /// </summary>
    /// <remarks>
    /// The elements inside the panel comprises of adornment labels, marker symbols and connector lines to connect the labels.
    /// </remarks>
    public partial class ChartAdornmentContainer : Panel
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="LabelVerticalAlignment"/> property.  
        /// </summary>
        public static readonly DependencyProperty LabelVerticalAlignmentProperty =
          DependencyProperty.Register(
              "LabelVerticalAlignment",
              typeof(VerticalAlignment),
              typeof(ChartAdornmentContainer),
              new PropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// The DependencyProperty for <see cref="LabelHorizontalAlignment"/> property.  
        /// </summary>
        public static readonly DependencyProperty LabelHorizontalAlignmentProperty =
          DependencyProperty.Register(
              "LabelHorizontalAlignment",
              typeof(HorizontalAlignment),
              typeof(ChartAdornmentContainer),
              new PropertyMetadata(HorizontalAlignment.Center));

        /// <summary>
        /// The DependencyProperty for <see cref="Symbol"/> property.  
        /// </summary>
        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register(
                "Symbol",
                typeof(Object),
                typeof(ChartAdornmentContainer),
                new PropertyMetadata(ChartSymbol.Custom, new PropertyChangedCallback(OnAdornmentsInfoChanged)));

        #endregion

        #region Fields

        private Point m_symbolOffset = new Point();

        /// <summary>
        /// Initializes m_symbolPresenter.
        /// </summary>
        private ContentPresenter m_symbolPresenter = new ContentPresenter();

        private ChartAdornment adornment;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the label vertical alignment.
        /// </summary>
        /// <value>The label vertical alignment.</value>
        public VerticalAlignment LabelVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(LabelVerticalAlignmentProperty); }
            set { SetValue(LabelVerticalAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label horizontal alignment.
        /// </summary>
        /// <value>The label horizontal alignment.</value>       
        public HorizontalAlignment LabelHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(LabelHorizontalAlignmentProperty); }
            set { SetValue(LabelHorizontalAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the chart symbol
        /// </summary>      
        public ChartSymbol Symbol
        {
            get { return (ChartSymbol)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        /// <summary>
        /// Gets the symbol offset.
        /// </summary>
        /// <value>The symbol offset.</value>       
        public Point SymbolOffset
        {
            get { return this.m_symbolOffset; }
        }

        /// <summary>
        /// Gets or sets the <see cref="ChartAdornment"/>.
        /// </summary>
        internal ChartAdornment Adornment
        {
            get
            {
                return adornment;
            }
            set
            {
                if (value != adornment)
                {
                    adornment = value;
                    UpdateContainers(true);
                }
                else
                {
                    UpdateContainers(false);
                }

            }
        }

        /// <summary>
        /// Gets or sets the pre-defined adornment symbol.
        /// </summary>
        internal SymbolControl PredefinedSymbol { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAdornmentContainer"/> class.
        /// </summary>
        public ChartAdornmentContainer()
        {
            PredefinedSymbol = new SymbolControl();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAdornmentContainer"/> class.
        /// </summary>
        /// <param name="adornment"></param>
        public ChartAdornmentContainer(ChartAdornment adornment)
        {
            PredefinedSymbol = new SymbolControl();
            Adornment = adornment;
        }

        #endregion

        #region Methods

        #region Internal Methods    

        internal void Dispose()
        {
            m_symbolPresenter.SetValue(ContentPresenter.ContentTemplateProperty, null);
            m_symbolPresenter.SetValue(ContentPresenter.ContentProperty, null);

            this.adornment = null;
            this.PredefinedSymbol = null;
            this.m_symbolPresenter = null;
        }

        //WPF-13972 While dynamically change the symbol property, the previous symbol not clear in view and While changing the symbol property from pre-defined to custom dynamically ,custom symbol is not showing.
        /// <summary>
        /// Updates the adornment containers.
        /// </summary>
        internal void UpdateContainers(bool setBinding)
        {
            if (this.Adornment != null && !adornment.CanHideLabel)
            {
                var adornmentInfo = Adornment.Series.adornmentInfo;
                this.LabelVerticalAlignment = adornmentInfo.VerticalAlignment;
                this.LabelHorizontalAlignment = adornmentInfo.HorizontalAlignment;

                this.Symbol = adornmentInfo.Symbol;

                this.SetSymbol(this.Symbol.ToString());

                if (setBinding)
                    this.SetContentBinding(Adornment);
            }
            else
            {
                if (this.Children.Contains(m_symbolPresenter))
                {
                    this.Children.Remove(m_symbolPresenter);
                }
                if (this.Children.Contains(PredefinedSymbol))
                {
                    this.Children.Remove(PredefinedSymbol);
                }
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect arrangeRect = new Rect(new Point(), this.DesiredSize);
            Rect arrangePieRect = new Rect(new Point(), this.DesiredSize);
            switch (this.LabelHorizontalAlignment)
            {
                case HorizontalAlignment.Stretch:
                case HorizontalAlignment.Center:
                    {
                        this.m_symbolPresenter.HorizontalAlignment = HorizontalAlignment.Center;
                        this.PredefinedSymbol.HorizontalAlignment = HorizontalAlignment.Center;
                    }

                    break;

                case HorizontalAlignment.Left:
                    {
                        this.m_symbolPresenter.HorizontalAlignment = HorizontalAlignment.Right;
                        this.PredefinedSymbol.HorizontalAlignment = HorizontalAlignment.Right;
                    }

                    break;

                case HorizontalAlignment.Right:
                    {
                        this.m_symbolPresenter.HorizontalAlignment = HorizontalAlignment.Left;
                        this.PredefinedSymbol.HorizontalAlignment = HorizontalAlignment.Left;
                    }

                    break;
            }

            switch (this.LabelVerticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    {
                        this.m_symbolPresenter.VerticalAlignment = VerticalAlignment.Top;
                        this.PredefinedSymbol.VerticalAlignment = VerticalAlignment.Top;
                    }

                    break;
                case VerticalAlignment.Stretch:
                case VerticalAlignment.Center:
                    {
                        this.m_symbolPresenter.VerticalAlignment = VerticalAlignment.Center;
                        this.PredefinedSymbol.VerticalAlignment = VerticalAlignment.Center;
                    }

                    break;
                case VerticalAlignment.Top:
                    {
                        this.m_symbolPresenter.VerticalAlignment = VerticalAlignment.Bottom;
                        this.PredefinedSymbol.VerticalAlignment = VerticalAlignment.Bottom;
                    }

                    break;
            }

            this.m_symbolPresenter.Arrange(arrangeRect);
            this.PredefinedSymbol.Arrange(arrangeRect);

            return this.DesiredSize;
        }

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="availableSize">The Available Size</param>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Adornment == null)
                return ChartLayoutUtils.CheckSize(availableSize);
            PredefinedSymbol.Measure(availableSize);
            m_symbolPresenter.Measure(availableSize);

            Size resultSize = new Size();
            Size lblSz = Size.Empty;
            Size sblSz;
            if (Adornment.Series.adornmentInfo.Symbol == ChartSymbol.Custom)
            {
                sblSz = m_symbolPresenter.DesiredSize;
            }
            else
            {
                sblSz = PredefinedSymbol.DesiredSize;
            }

            switch (LabelHorizontalAlignment)
            {
                case HorizontalAlignment.Stretch:
                case HorizontalAlignment.Center:
                    {
                        resultSize.Width = Math.Max(lblSz.Width, sblSz.Width);
                        m_symbolOffset.X = 0.5 * resultSize.Width;
                    }

                    break;

                case HorizontalAlignment.Left:
                    resultSize.Width = Math.Max(lblSz.Width, sblSz.Width);
                    m_symbolOffset.X = 0.5 * resultSize.Width;
                    break;

                case HorizontalAlignment.Right:
                    resultSize.Width = Math.Max(lblSz.Width, sblSz.Width);
                    m_symbolOffset.X = 0.5 * resultSize.Width;
                    break;
            }

            switch (LabelVerticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    resultSize.Height = Math.Max(lblSz.Height, sblSz.Height);
                    m_symbolOffset.Y = 0.5 * resultSize.Height;

                    break;
                case VerticalAlignment.Stretch:
                case VerticalAlignment.Center:
                    {
                        resultSize.Height = Math.Max(lblSz.Height, sblSz.Height);
                        m_symbolOffset.Y = 0.5 * resultSize.Height;
                    }

                    break;
                case VerticalAlignment.Top:
                    resultSize.Height = Math.Max(lblSz.Height, sblSz.Height);
                    m_symbolOffset.Y = 0.5 * resultSize.Height;

                    break;
            }

            return resultSize;
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Updates the adornment symbol on <see cref="ChartAdornmentInfo"/> changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments</param>
        private static void OnAdornmentsInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartAdornmentContainer = d as ChartAdornmentContainer;
            chartAdornmentContainer.SetSymbol(chartAdornmentContainer.Symbol.ToString());
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Sets the binding between the adornment symbol and <see cref="ChartAdornmentInfo"/>
        /// </summary>
        /// <param name="adornment"></param>
        private void SetContentBinding(ChartAdornment adornment)
        {
            ChartAdornmentInfoBase adornmentInfo = adornment.Series.adornmentInfo;

            if (adornmentInfo.ShowMarker)
            {
                this.SetBinding(ChartAdornmentContainer.SymbolProperty
                         , CreateAdormentBinding("Symbol", adornmentInfo));

                this.PredefinedSymbol.SetBinding(Control.HeightProperty
                    , CreateAdormentBinding("SymbolHeight", adornmentInfo));

                this.PredefinedSymbol.SetBinding(Control.WidthProperty
                    , CreateAdormentBinding("SymbolWidth", adornmentInfo));

                if (adornment.Series is ChartSeries
                    && adornment.Series.ActualArea.SelectedSeriesCollection.Contains(adornment.Series)
                    && adornmentInfo.HighlightOnSelection
                    && adornment.Series.ActualArea.GetSeriesSelectionBrush(adornment.Series) != null
                    && adornment.Series.ActualArea.GetEnableSeriesSelection())
                    this.PredefinedSymbol.SetBinding(Control.BackgroundProperty
                  , CreateAdormentBinding("SeriesSelectionBrush", adornment.Series));
                else if (adornment.Series is ISegmentSelectable
                    && adornment.Series.SelectedSegmentsIndexes.Contains(adornment.Series.ActualData.IndexOf(adornment.Item))
                    && adornmentInfo.HighlightOnSelection
                    && adornment.Series.ActualArea.GetEnableSegmentSelection()
                    && (adornment.Series as ISegmentSelectable).SegmentSelectionBrush != null)
                    this.PredefinedSymbol.SetBinding(Control.BackgroundProperty
                   , CreateAdormentBinding("SegmentSelectionBrush", adornment.Series));
                else if (adornmentInfo.SymbolInterior == null)
                    this.PredefinedSymbol.SetBinding(SymbolControl.BackgroundProperty
                   , CreateAdormentBinding("Interior", adornment));
                else
                    this.PredefinedSymbol.SetBinding(Control.BackgroundProperty
                   , CreateAdormentBinding("SymbolInterior", adornmentInfo));

                if (adornment.Series is ChartSeries
                    && adornment.Series.ActualArea.SelectedSeriesCollection.Contains(adornment.Series)
                    && adornmentInfo.HighlightOnSelection
                    && adornment.Series.ActualArea.GetSeriesSelectionBrush(adornment.Series) != null
                    && adornment.Series.ActualArea.GetEnableSeriesSelection())
                    this.PredefinedSymbol.SetBinding(Control.BorderBrushProperty
                 , CreateAdormentBinding("SeriesSelectionBrush", adornment.Series));
                else if (adornment.Series is ISegmentSelectable
                     && adornment.Series.SelectedSegmentsIndexes.Contains(adornment.Series.ActualData.IndexOf(adornment.Item))
                     && adornmentInfo.HighlightOnSelection
                     && adornment.Series.ActualArea.GetEnableSegmentSelection()
                     && (adornment.Series as ISegmentSelectable).SegmentSelectionBrush != null)
                    this.PredefinedSymbol.SetBinding(Control.BorderBrushProperty
                   , CreateAdormentBinding("SegmentSelectionBrush", adornment.Series));
                else
                    this.PredefinedSymbol.SetBinding(Control.BorderBrushProperty
                   , CreateAdormentBinding("SymbolStroke", adornmentInfo));

                if (adornmentInfo.SymbolTemplate != null)
                    this.m_symbolPresenter.SetBinding(ContentPresenter.ContentTemplateProperty
                         , CreateAdormentBinding("SymbolTemplate", adornmentInfo));

                this.m_symbolPresenter.SetBinding(ContentPresenter.ContentProperty
                    , CreateAdormentBinding("", adornment));
            }
        }

        /// <summary>
        /// Helper method to create the binding between symbol and <see cref="ChartAdornmentInfo"/>.
        /// </summary>
        /// <param name="path">The Binding Path</param>
        /// <param name="source">The Binding Source</param>
        /// <returns></returns>
        private Binding CreateAdormentBinding(string path, object source)
        {
            Binding bindingProvider = new Binding();
            bindingProvider.Path = new PropertyPath(path);
            bindingProvider.Source = source;
            bindingProvider.Mode = BindingMode.OneWay;

            if (path == "SeriesSelectionBrush")
            {
                bindingProvider.Converter = new SeriesSelectionBrushConverter(adornment.Series);
                bindingProvider.ConverterParameter = adornment.Series.ActualData.IndexOf(adornment.Item);
            }

            return bindingProvider;
        }

        /// <summary>
        /// Updates the symbol.
        /// </summary>
        /// <param name="symbol">The Symbol String</param>
        private void SetSymbol(string symbol)
        {
            ChartAdornmentInfoBase adornmentInfo = adornment.Series.adornmentInfo;
            if (symbol != "Custom")
            {
                if (this.Children.Contains(m_symbolPresenter))
                {
                    this.Children.Remove(m_symbolPresenter);
                }

                if (!this.Children.Contains(PredefinedSymbol))
                    this.Children.Add(PredefinedSymbol);
                if (Double.IsNaN(adornment.YData) || double.IsNaN(adornment.XData))
                {
                    this.Children.Remove(PredefinedSymbol);
                }

                PredefinedSymbol.DataContext = this;
                this.PredefinedSymbol.Template = ChartDictionaries.GenericSymbolDictionary[symbol+ "Template"] as ControlTemplate;
            }
            else
            {
                if (adornmentInfo.SymbolTemplate != null && !this.Children.Contains(m_symbolPresenter))
                {
                    this.Children.Add(m_symbolPresenter);
                }

                if (this.Children.Contains(PredefinedSymbol))
                {
                    this.Children.Remove(PredefinedSymbol);
                }

                if (Double.IsNaN(adornment.YData) || double.IsNaN(adornment.XData))
                {
                    this.Children.Remove(PredefinedSymbol);
                }
            }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// A control that represents symbol in chart adornments
    /// </summary>   
    public partial class SymbolControl : Control
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="Stroke"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(
                "Stroke",
                typeof(Brush),
                typeof(SymbolControl),
                new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the stroke
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        #endregion

        #endregion
    }
}
