// <copyright file="ChartCartesianAxisPanel.cs" company="Syncfusion. Inc">
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
    using System.Linq;
    using System.Text;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using System.Threading.Tasks;

    /// <summary>
    /// This interfaces defines the members and methods to create and arrange the child elements in a panel.
    /// </summary>
    public interface ILayoutCalculator
    {
        #region Properties

        /// <summary>
        /// Gets Children property
        /// </summary>
        List<UIElement> Children { get; }

        /// <summary>
        /// Gets the panel.
        /// </summary>
        /// <value>
        /// The panel.
        /// </value>
        Panel Panel { get; }

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>
        /// The left.
        /// </value>
        double Left { get; set; }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>
        /// The top.
        /// </value>
        double Top { get; set; }

        /// <summary>
        /// Gets desiredSize property
        /// </summary>
        Size DesiredSize { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Method declaration for Measure
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        Size Measure(Size availableSize);

        /// <summary>
        /// Method declaration for Arrange
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        Size Arrange(Size finalSize);

        /// <summary>
        /// Method declaration for UpdateElements
        /// </summary>
        void UpdateElements();

        /// <summary>
        /// Detachs elements from the panel
        /// </summary>
        void DetachElements();

        #endregion
    }

    /// <summary>
    /// Represents <see cref="ChartCartesianAxisPanel"/> class.
    /// </summary>  
    public partial class ChartCartesianAxisPanel : Canvas
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartCartesianAxisPanel"/> class.
        /// </summary>
        public ChartCartesianAxisPanel()
        {
            LayoutCalc = new List<ILayoutCalculator>();
        }

        #endregion

        #region Properties

        #region Internal Properties

        /// <summary>
        /// Gets or sets the chart axis.
        /// </summary>
        internal ChartAxisBase2D Axis { get; set; }

        /// <summary>
        /// Gets or sets the calculated layout.
        /// </summary>
        internal List<ILayoutCalculator> LayoutCalc { get; set; }

        #endregion

        #endregion

        #region Methods
        
        #region Internal Methods

        /// <summary>
        /// Computes the size of the <see cref="ChartCartesianAxisPanel"/>.
        /// </summary>
        /// <param name="availableSize">The Available Size</param>
        /// <returns>Returns the computed size.</returns>
        internal Size ComputeSize(Size availableSize)
        {
            Size size = Size.Empty;

            if (Axis.AxisLayoutPanel is ChartPolarAxisLayoutPanel)
            {
                foreach (UIElement element in this.Children)
                {
                    element.Measure(availableSize);
                }
                (this.Children[0] as FrameworkElement).Visibility = Visibility.Collapsed;//Collapsing the visibility of polar/radar series X-Axis header
                foreach (ILayoutCalculator element in this.LayoutCalc)
                {
                    element.Measure(availableSize);
                    size = element.DesiredSize;

                    ChartPolarAxisLayoutPanel axisLayoutPanel = Axis.AxisLayoutPanel as ChartPolarAxisLayoutPanel;
                    ChartCircularAxisPanel circularAxisPanel = element as ChartCircularAxisPanel;
                    axisLayoutPanel.Radius = circularAxisPanel.Radius;
                }
            }
            else
            {
                double horizontalPadding = 0;
                double verticalPadding = 0;
                double width = 0;
                double height = 0;
                double angle = 0d; //double.IsNaN(this.Axis.HeaderRotationAngle) ? 0d : this.Axis.HeaderRotationAngle;
                double direction = 1;

                if (Axis.headerContent != null)
                {
                    Axis.headerContent.HorizontalAlignment = HorizontalAlignment.Center;
                    Axis.headerContent.VerticalAlignment = VerticalAlignment.Center;

                    if (Axis.Orientation == Orientation.Vertical)
                    {
                        direction = this.Axis.OpposedPosition ? 1 : -1;
                        angle = direction * 90;
                        var transform = new RotateTransform { Angle = angle };
                        Axis.headerContent.RenderTransform = transform;
                    }
                    else
                        Axis.headerContent.RenderTransform = null;
                }

                foreach (UIElement element in this.Children)
                {
                    bool isHeader = Axis.headerContent == element
                                    && this.Axis.Orientation == Orientation.Vertical;

                    var measureSize = availableSize;
                    if (isHeader)
                    {
                        measureSize.Width = Math.Max(availableSize.Width, element.DesiredSize.Width);
                        measureSize.Height = Math.Max(availableSize.Height, element.DesiredSize.Height);
                    }

                    element.Measure(measureSize);

                    if (element is SfChartResizableBar && Axis.EnableTouchMode)
                        continue;

                    width += isHeader ? element.DesiredSize.Height : element.DesiredSize.Width;
                    height += isHeader ? element.DesiredSize.Width : element.DesiredSize.Height;
                }

                foreach (ILayoutCalculator element in LayoutCalc)
                {
                    element.Measure(availableSize);
                    if ((element is ChartCartesianAxisLabelsPanel
                         && Axis.LabelsPosition == AxisElementPosition.Inside)
                        || (element is ChartCartesianAxisElementsPanel
                            && Axis.TickLinesPosition == AxisElementPosition.Inside))
                    {
                        horizontalPadding += element.DesiredSize.Width;
                        verticalPadding += element.DesiredSize.Height;
                    }
                    width += element.DesiredSize.Width;
                    height += element.DesiredSize.Height;
                }

                // To measure the multi level labels panel
                if (Axis.MultiLevelLabels != null && Axis.MultiLevelLabels.Count > 0 &&
                    Axis.axisMultiLevelLabelsPanel != null)
                {

                    Axis.axisMultiLevelLabelsPanel.Measure(availableSize);
                    if (Axis.LabelsPosition == AxisElementPosition.Inside)
                    {
                        horizontalPadding += Axis.axisMultiLevelLabelsPanel.DesiredSize.Width;
                        verticalPadding += Axis.axisMultiLevelLabelsPanel.DesiredSize.Height;
                    }
                    width += Axis.axisMultiLevelLabelsPanel.DesiredSize.Width;
                    height += Axis.axisMultiLevelLabelsPanel.DesiredSize.Height;
                }

                if (Axis.Orientation == Orientation.Vertical)
                {
                    Axis.InsidePadding = horizontalPadding;
                    size = new Size(width, availableSize.Height);
                }
                else
                {
                    Axis.InsidePadding = verticalPadding;
                    size = new Size(availableSize.Width, height);
                }
            }

            return ChartLayoutUtils.CheckSize(size);
        }

        /// <summary>
        /// Arranges the elements of the <see cref="ChartCartesianAxisPanel"/>
        /// </summary>
        /// <param name="finalSize">The Final Size</param>
        internal void ArrangeElements(Size finalSize)
        {
            if (Axis.AxisLayoutPanel is ChartPolarAxisLayoutPanel)
            {
                foreach (UIElement element in this.Children)
                {
                    element.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                    SetLeft(element, 0);
                    SetTop(element, 0);
                }

                foreach (ILayoutCalculator layout in this.LayoutCalc)
                {
                    layout.Arrange(finalSize);

                    ChartPolarAxisLayoutPanel axisLayoutPanel = Axis.AxisLayoutPanel as ChartPolarAxisLayoutPanel;
                    ChartCircularAxisPanel circularAxisPanel = layout as ChartCircularAxisPanel;
                    axisLayoutPanel.Radius = circularAxisPanel.Radius;
                }
            }
            else
            {
                ArrangeCartesianElements(finalSize);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Arranges the cartesian elements.
        /// </summary>
        /// <param name="finalSize">The Final Size</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Reviewed")]
        private void ArrangeCartesianElements(Size finalSize)
        {
            if (Axis == null)
                return;

            foreach (UIElement element in Children)
            {
                SetLeft(element, 0);
                SetTop(element, 0);
            }

            var headerContent = Children[0];
            var labelsPanel = LayoutCalc.Count > 0 ? LayoutCalc[0] : null;
            var elementsPanel = LayoutCalc.Count > 1 ? LayoutCalc[1] : null;
            var multiLevelPanel = Axis.axisMultiLevelLabelsPanel;
            var scrollBar = Children.OfType<SfChartResizableBar>().FirstOrDefault();

            var elements = new List<UIElement>();
            var sizes = new List<Size>();
            var isVertical = Axis.Orientation == Orientation.Vertical;
            var isInversed = Axis.OpposedPosition ^ isVertical;
            double headerMargin = 2;  // Add some gap between the axis header and plot area.
            if (scrollBar != null)
            {
                elements.Add(scrollBar);
                sizes.Add(scrollBar.DesiredSize);
            }
            if (elementsPanel != null)
            {
                if (Axis.TickLinesPosition == AxisElementPosition.Inside)
                {
                    elements.Insert(0, elementsPanel.Panel);
                    sizes.Insert(0, elementsPanel.DesiredSize);
                }
                else
                {
                    elements.Add(elementsPanel.Panel);
                    sizes.Add(elementsPanel.DesiredSize);
                }
            }
            if (labelsPanel != null)
            {
                if (Axis.LabelsPosition == AxisElementPosition.Inside)
                {
                    elements.Insert(0, labelsPanel.Panel);
                    sizes.Insert(0, labelsPanel.DesiredSize);
                }
                else
                {
                    elements.Add(labelsPanel.Panel);
                    sizes.Add(labelsPanel.DesiredSize);
                }
            }
            if (Axis.MultiLevelLabels != null &&
                Axis.MultiLevelLabels.Count > 0 && multiLevelPanel != null)
            {
                if (Axis.LabelsPosition == AxisElementPosition.Inside)
                {
                    elements.Insert(0, multiLevelPanel.Panel);
                    sizes.Insert(0, multiLevelPanel.DesiredSize);
                }
                else
                {
                    elements.Add(multiLevelPanel.Panel);
                    sizes.Add(multiLevelPanel.DesiredSize);
                }
            }
            if (headerContent != null)
            {
                elements.Add(headerContent as FrameworkElement);
                Size headerSize = (headerContent as FrameworkElement).DesiredSize;
                if (isVertical && headerContent.GetType() == typeof(ContentControl))
                {
                    headerSize = new Size(headerSize.Height, headerSize.Width);
                }
                sizes.Add(headerSize);
            }

            if (isInversed)
            {
                elements.Reverse();
                sizes.Reverse();
            }

            double currentPos = 0;

            for (int i = 0; i < elements.Count; i++)
            {
                UIElement element = elements[i];
                var isResizableBar = element is SfChartResizableBar;
                double headerHeight = 0;
                double headerWidth = 0;
                if (isVertical)
                {
                    if (element == headerContent)
                    {
                        double leftPosition = currentPos - ((element.DesiredSize.Width - sizes[i].Width) / 2);
                        headerHeight = element.DesiredSize.Height;
                        headerWidth = element.DesiredSize.Width;
                        SetTop(element, (finalSize.Height - element.DesiredSize.Height) / 2);

                        if (Axis.TickLinesPosition == AxisElementPosition.Inside)
                        {
                            double tickFactor = (elementsPanel.DesiredSize.Width - Axis.TickLineSize);
                            // Positioning header according to the axis line thickness.
                            double position = Axis.OpposedPosition ? leftPosition + tickFactor : leftPosition - tickFactor;
                            SetLeft(element, position);
                        }

                        if (Axis.ShowAxisNextToOrigin && Axis.HeaderPosition == AxisHeaderPosition.Far)
                        {
                            ChartAxis axis = null;
                            double value = 0;
                            if (Axis.Area.InternalSecondaryAxis == Axis)
                            {
                                value = Axis.Area.InternalPrimaryAxis.ValueToCoefficientCalc(Axis.Origin);
                                axis = Axis.Area.InternalPrimaryAxis;
                            }
                            else if (Axis.Area.InternalSecondaryAxis.Orientation == Orientation.Horizontal &&
                                     Axis.Area.InternalPrimaryAxis == Axis)
                            {
                                value = Axis.Area.InternalSecondaryAxis.ValueToCoefficientCalc(Axis.Origin);
                                axis = Axis.Area.InternalSecondaryAxis;
                            }
                            if (0 < value && 1 > value && axis != null)
                            {
                                if (Axis.OpposedPosition)
                                {
                                    Rect rect = ChartLayoutUtils.Subtractthickness(new Rect(new Point(0, 0), Axis.AvailableSize), Axis.Area.AxisThickness);
                                    double position = (1 - value) * (rect.Width - (axis.ActualPlotOffset * 2)) + axis.ActualPlotOffset + Axis.InsidePadding;
                                    if (position > finalSize.Width - headerHeight)
                                    {
                                        if (axis.AxisLayoutPanel is ChartPolarAxisLayoutPanel)
                                            SetLeft(element, leftPosition);
                                        else
                                            SetLeft(element, position - (headerWidth - headerHeight) / 2 + headerMargin);
                                    }
                                    else
                                        SetLeft(element, leftPosition);
                                }
                                else if (Axis.ArrangeRect.Left + finalSize.Width - headerHeight > finalSize.Width - headerHeight)
                                {
                                    if (axis.AxisLayoutPanel is ChartPolarAxisLayoutPanel)
                                        SetLeft(element, leftPosition);
                                    else
                                        SetLeft(element, -(Axis.ArrangeRect.Left + (headerWidth - headerHeight) / 2));
                                }
                                else
                                    SetLeft(element, leftPosition);
                            }
                            else
                                SetLeft(element, leftPosition);
                        }
                        else
                            SetLeft(element, leftPosition);
                    }
                    else if (isResizableBar && Axis.EnableTouchMode)
                    {
                        SetLeft(element, currentPos - sizes[i].Width / 2);
                        continue;
                    }
                    else if ((isResizableBar && !Axis.EnableTouchMode) && Axis.TickLinesPosition == AxisElementPosition.Inside)
                    {
                        double tickFactor = elementsPanel.DesiredSize.Width - Axis.TickLineSize;
                        // Positioning scroll bar according to the axis line thickness.
                        double position = Axis.OpposedPosition ? currentPos + tickFactor : currentPos - tickFactor;
                        SetLeft(element, position);
                    }
                    else
                        SetLeft(element, currentPos);

                    currentPos += sizes[i].Width;
                }
                else
                {
                    if (element == headerContent)
                    {
                        SetLeft(element, (finalSize.Width - sizes[i].Width) / 2);
                        headerHeight = element.DesiredSize.Height;
                        if (Axis.TickLinesPosition == AxisElementPosition.Inside)
                        {
                            double tickFactor = elementsPanel.DesiredSize.Height - Axis.TickLineSize;
                            // Positioning header according to the axis line thickness.
                            double position = Axis.OpposedPosition ? currentPos - tickFactor : currentPos + tickFactor;
                            SetTop(element, position);
                        }

                        if (Axis.ShowAxisNextToOrigin && Axis.HeaderPosition == AxisHeaderPosition.Far)
                        {
                            ChartAxis axis = null;
                            double value = 0;
                            if (Axis.Area.InternalPrimaryAxis == Axis)
                            {
                                value = Axis.Area.InternalSecondaryAxis.ValueToCoefficientCalc(Axis.Origin);
                                axis = Axis.Area.InternalSecondaryAxis;
                            }
                            else if (Axis.Area.InternalPrimaryAxis.Orientation == Orientation.Vertical
                                     && Axis.Area.InternalSecondaryAxis == Axis)
                            {
                                value = Axis.Area.InternalPrimaryAxis.ValueToCoefficientCalc(Axis.Origin);
                                axis = Axis.Area.InternalPrimaryAxis;
                            }
                            if (0 < value && 1 > value && axis != null)
                            {
                                Rect rect = ChartLayoutUtils.Subtractthickness(new Rect(new Point(0, 0), Axis.AvailableSize), Axis.Area.AxisThickness);
                                double position = value * (rect.Height - (axis.ActualPlotOffset * 2)) + axis.ActualPlotOffset + Axis.InsidePadding;
                                if (Axis.OpposedPosition)
                                {
                                    if (Axis.ArrangeRect.Top + finalSize.Height - headerHeight > finalSize.Height - headerHeight)
                                        SetTop(element, -Axis.ArrangeRect.Top - headerMargin);
                                    else
                                        SetTop(element, currentPos);
                                }
                                else if (position > finalSize.Height - headerHeight)
                                    SetTop(element, (position + headerMargin));
                                else
                                    SetTop(element, currentPos);
                            }
                            else
                                SetTop(element, currentPos);
                        }

                        else
                            SetTop(element, currentPos);
                    }
                    else if (isResizableBar && Axis.EnableTouchMode)
                    {
                        SetTop(element, currentPos - sizes[i].Height / 2);
                        continue;
                    }
                    else if ((isResizableBar && !Axis.EnableTouchMode) && Axis.TickLinesPosition == AxisElementPosition.Inside)
                    {
                        double tickFactor = elementsPanel.DesiredSize.Height - Axis.TickLineSize;
                        // Position axis line according to the axis line thickness.
                        double position = Axis.OpposedPosition ? currentPos - tickFactor : currentPos + tickFactor;
                        SetTop(element, position);
                    }
                    else
                        SetTop(element, currentPos);
                    currentPos += sizes[i].Height;
                }
            }

            foreach (ILayoutCalculator layout in this.LayoutCalc)
            {
                layout.Arrange(layout.DesiredSize);
            }


            foreach (UIElement element in elements)
            {
                if ((element as FrameworkElement).Name == "axisLabelsPanel")
                {
                    SetLabelsPanelBounds(element, labelsPanel);
                    break;
                }
            }


            // To arrange the children of multi level labels panel
            if (Axis.MultiLevelLabels != null &&
                Axis.MultiLevelLabels.Count > 0 && multiLevelPanel != null)
                multiLevelPanel.Arrange(multiLevelPanel.DesiredSize);

        }

        /// <summary>
        /// Sets the labels panel bounds.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="labelsPanel"></param>
        private void SetLabelsPanelBounds(UIElement element, ILayoutCalculator labelsPanel)
        {
            var left = Canvas.GetLeft(element);
            var top = Canvas.GetTop(element);
            var width = labelsPanel.DesiredSize.Width;
            var height = labelsPanel.DesiredSize.Height;
            var cartesianAxisLabelsPanel = labelsPanel as ChartCartesianAxisLabelsPanel;

            if (Axis.Orientation == Orientation.Horizontal)
            {
                left += Axis.ArrangeRect.Left;
                top += Axis.ArrangeRect.Top;
                cartesianAxisLabelsPanel.SetOffsetValues(left, top, width, height);
            }
            else
            {
                left += Axis.ArrangeRect.Left;
                top += Axis.ArrangeRect.Top;
                cartesianAxisLabelsPanel.SetOffsetValues(left, top, width, height);
            }
        }

        #endregion

        #endregion
    }
}
