// <copyright file="ChartAxisBase3D.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{

    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Shapes;

    /// <summary>
    /// Class implementation for ChartAxisBase3D
    /// </summary>
    public abstract partial class ChartAxisBase3D : ChartAxis
    {
        #region Fields

        private ChartCartesianAxisPanel3D axisPanel;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAxisBase3D"/> class.
        /// </summary>
        public ChartAxisBase3D()
        {
            IsZAxis = false;
            IsManhattanAxis = false;
        }

        #endregion

        #region Properties

        #region Internal Properties
        
        /// <summary>
        /// Gets or sets a value indicating whether the axis is z axis.
        /// </summary>
        internal bool IsZAxis { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the axis is manhattan axis.
        /// </summary>
        internal bool IsManhattanAxis { get; set; }

        /// <summary>
        /// Gets or sets the actual position of the whole axis.
        /// Please Note: It is not enabled in horizontal label scenarios.
        /// </summary>
        internal AxisPosition3D AxisPosition3D { get; set; }

        /// <summary>
        /// Gets or sets the axis depth.
        /// </summary>
        internal double AxisDepth { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Converts co-ordinate of point related to chart control to axis units.
        /// </summary>
        /// <param name="value">The absolute point value.</param>
        /// <returns>The value of point on axis.</returns>
        /// <seealso cref="ChartAxis.ValueToCoefficientCalc(double)"/>
        public override double CoefficientToValue(double value)
        {
            double result = double.NaN;

            value = this.IsInversed ? 1d - value : value;

            result = VisibleRange.Start + VisibleRange.Delta * value;

            return result;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Creates the line recycler.
        /// </summary>
        internal override void CreateLineRecycler()
        {
            if (this.Area == null)
            {
                return;
            }

            this.GridLinesRecycler = new UIElementsRecycler<Line>(null);
            this.MinorGridLinesRecycler = new UIElementsRecycler<Line>(null);
        }

        /// <summary>
        /// Computes the desired size.
        /// </summary>
        /// <param name="size">The Size</param>
        internal override void ComputeDesiredSize(Size size)
        {
            this.AvailableSize = size;
            this.CalculateRangeAndInterval(size);
            if (this.Visibility != Visibility.Collapsed)
            {
                this.UpdatePanels();
                this.UpdateLabels();
                this.ComputedDesiredSize = this.axisPanel.ComputeSize(size);
            }
            else
            {
                this.ActualPlotOffset = this.PlotOffset;
                this.InsidePadding = 0;
                this.UpdateLabels();
                this.ComputedDesiredSize = this.Orientation == Orientation.Horizontal
                                          ? new Size(size.Width, 0)
                                          : new Size(0, size.Height);
            }
        }

        #endregion

        #region Protected Internal Methods

        /// <summary>
        /// Raises the <see cref="E:AxisBoundsChanged" /> event.
        /// </summary>
        /// <param name="args">The <see cref="ChartAxisBoundsEventArgs"/> instance containing the event data.</param>
        protected internal override void OnAxisBoundsChanged(ChartAxisBoundsEventArgs args)
        {
            base.OnAxisBoundsChanged(args);
            if (this.axisPanel != null)
            {
                this.axisPanel.ArrangeElements(new Size(ArrangeRect.Width, ArrangeRect.Height));
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Updates the axis panels.
        /// </summary>
        private void UpdatePanels()
        {
            if (this.axisLabelsPanel != null)
            {
                axisLabelsPanel.DetachElements();
            }

            if (this.axisElementsPanel != null)
            {
                axisElementsPanel.DetachElements();
            }

            if (this.axisPanel != null)
            {
                this.axisPanel.LayoutCalc.Clear();
            }
            else
            {
                this.axisPanel = new ChartCartesianAxisPanel3D { Axis = this };
            }

            this.axisLabelsPanel = new ChartCartesianAxisLabelsPanel(null)
            {
                Axis = this
            };
            this.axisElementsPanel = new ChartCartesianAxisElementsPanel(null)
            {
                Axis = this
            };

            if (this.headerContent == null)
            {
                this.headerContent = new ContentControl { Content = Header, ContentTemplate = HeaderTemplate, RenderTransformOrigin = new Point(0.5, 0.5) };
                Binding binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("Visibility");
                headerContent.SetBinding(ContentControl.VisibilityProperty, binding);
            }
            else
            {
                headerContent.Content = this.Header;
                headerContent.ContentTemplate = this.HeaderTemplate;
            }

            this.axisPanel.HeaderContent = this.headerContent;
            if (this.HeaderTemplate == null && this.headerContent != null && this.HeaderStyle != null)
            {
                if (HeaderStyle.Foreground != null)
                {
                    headerContent.Foreground = HeaderStyle.Foreground;
                }

                if (HeaderStyle.FontSize != 0.0)
                {
                    headerContent.FontSize = HeaderStyle.FontSize;
                }

                if (HeaderStyle.FontFamily != null)
                {
                    headerContent.FontFamily = HeaderStyle.FontFamily;
                }
            }

            this.axisPanel.LayoutCalc.Add(this.axisLabelsPanel);
            this.axisPanel.LayoutCalc.Add(this.axisElementsPanel);
        }

        #endregion

        #endregion
    }
}
