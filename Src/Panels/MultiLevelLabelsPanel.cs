// <copyright file="MultiLevelLabelsPanel.cs" company="Syncfusion. Inc">
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
    using System.Threading.Tasks;
    using System.Windows;
    using Syncfusion.UI.Xaml.Charts;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;

    /// <summary>
    /// Represents the <see cref="MultiLevelLabelsPanel"/> class.
    /// </summary>
    public partial class MultiLevelLabelsPanel
    {
        #region Fields

        private Panel labelsPanel;
        private UIElementsRecycler<TextBlock> textBlockRecycler;
        private UIElementsRecycler<Border> borderRecycler;
        private UIElementsRecycler<Polyline> polylineRecycler;
        private List<Size> desiredSizes;
        private Size desiredSize;
        private IEnumerable<IGrouping<int, ChartMultiLevelLabel>> groupedLabels;
        private Thickness margin = new Thickness(2, 2, 2, 2);
        private double borderPadding = 10;
        private double top = 0;
        private double left = 0;
        private double startValue = 0;
        private double endValue = 0;
        private double start = 0;
        private double end = 0;
        private bool isOpposed;
        private int currentBorderPos;
        private int currentBracePos;
        private ChartMultiLevelLabel currentLabel;
        private double height;
        private double width;
        private double txtBlockLeft = 0;
        private double txtBlockTop = 0;
        private PointCollection pointCollection1;
        private PointCollection pointCollection2;
        private int currentRow;

        #endregion

        #region Constructor.
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiLevelLabelsPanel"/> class.
        /// </summary>
        /// <param name="panel">The Panel</param>
        public MultiLevelLabelsPanel(Panel panel)
        {
            labelsPanel = panel;
            textBlockRecycler = new UIElementsRecycler<TextBlock>(panel);
            polylineRecycler = new UIElementsRecycler<Polyline>(panel);
            borderRecycler = new UIElementsRecycler<Border>(panel);
        }

        #endregion

        #region Properties

        #region Internal Properties

        /// <summary>
        /// Gets the desired size of the panel
        /// </summary>
        internal Size DesiredSize
        {
            get
            {
                return desiredSize;
            }
        }

        /// <summary>
        /// Gets the corresponding <see cref="MultiLevelLabelsPanel"/>.
        /// </summary>
        internal Panel Panel
        {
            get
            {
                return labelsPanel;
            }
        }

        /// <summary>
        /// Gets or sets the corresponding chart axis
        /// </summary>
        internal ChartAxisBase2D Axis { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Internal Methods

        internal void Dispose()
        {
            Axis = null;
            if (labelsPanel != null)
            {
                labelsPanel.Children.Clear();
            }
        }

        /// <summary>
        /// To clear the children of <see cref="MultiLevelLabelsPanel"/>.
        /// </summary>
        internal void DetachElements()
        {
            labelsPanel = null;
            if (textBlockRecycler != null)
                textBlockRecycler.Clear();
            if (borderRecycler != null)
                borderRecycler.Clear();
            if (polylineRecycler != null)
                polylineRecycler.Clear();
        }

        /// <summary>
        /// To measure the size of <see cref="MultiLevelLabelsPanel"/>.
        /// </summary>
        /// <param name="availableSize">The Available Size</param>
        /// <returns>Returns the desired size.</returns>
        internal Size Measure(Size availableSize)
        {
            desiredSizes = new List<Size>();

            groupedLabels = Axis.MultiLevelLabels.OrderBy(label => label.Level).GroupBy(label => label.Level);

            foreach (TextBlock txtBlock in textBlockRecycler)
            {
                if (txtBlock.Width > 0)
                {
                    txtBlock.ClearValue(TextBlock.WidthProperty);
                    txtBlock.UpdateLayout();
                }

                txtBlock.Measure(availableSize);
                desiredSizes.Add(txtBlock.DesiredSize);

                if (Axis.Orientation == Orientation.Vertical) 
                {
                    // To rotate the textblock.
                    var angle = Axis.OpposedPosition ? 90 : -90;
                    txtBlock.RenderTransform = new RotateTransform() { Angle = angle };
                }
                else
                    txtBlock.RenderTransform = null;
            }

            CalculateActualPlotOffset();
            desiredSize = CalculateDesiredSize(availableSize);
            return desiredSize;
        }

        /// <summary>
        /// To update the children of <see cref="MultiLevelLabelsPanel"/>.
        /// </summary>
        internal void UpdateElements()
        {
            if (Axis != null && Axis.MultiLevelLabels != null
                && Axis.MultiLevelLabels.Count > 0)
                GenerateContainer(Axis.MultiLevelLabels.Count);
            else
            {
                if (textBlockRecycler.Count > 0)
                    textBlockRecycler.Clear();
                if (borderRecycler.Count > 0)
                    borderRecycler.Clear();
                if (polylineRecycler.Count > 0)
                    polylineRecycler.Clear();
            }
        }

        /// <summary>
        /// To arrange the children in <see cref="MultiLevelLabelsPanel"/> based on its final size
        /// </summary>
        /// <param name="finalSize">The Final Size</param>
        /// <returns>Returns the arranged size.</returns>
        internal Size Arrange(Size finalSize)
        {
            isOpposed = IsOpposed();
            Rect rect;
            double maxThickness = Axis.LabelBorderWidth;
            if (Axis.Orientation == Orientation.Horizontal)
            {
                var top = !Axis.ShowLabelBorder ? -maxThickness : -margin.Top;
                rect = new Rect(
                    Axis.ActualPlotOffset - maxThickness,
                    top,
                    ((finalSize.Width + 2 * maxThickness - (2 * Axis.ActualPlotOffset))),
                    finalSize.Height + margin.Top + margin.Bottom);
                rect.Height = !Axis.ShowLabelBorder ? rect.Height + Axis.LabelBorderWidth : rect.Height;
                this.labelsPanel.Clip = new RectangleGeometry()
                {
                    Rect = rect
                };
                ArrangeHorizontalLabels(finalSize);
            }
            else
            {
                var left = !Axis.ShowLabelBorder ? -maxThickness : -margin.Left;
                rect = new Rect(
                    left,
                    Axis.ActualPlotOffset - maxThickness,
                    finalSize.Width + margin.Left + margin.Right,
                    (finalSize.Height + 2 * maxThickness - (2 * Axis.ActualPlotOffset)));
                rect.Width = Axis.ShowLabelBorder ? rect.Width : rect.Width + Axis.LabelBorderWidth;
                this.labelsPanel.Clip = new RectangleGeometry()
                {
                    Rect = rect
                };
                ArrangeVerticalLables(finalSize);
            }

            return finalSize;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// To arrange the text blocks and border of vertical axis's <see cref="MultiLevelLabelsPanel"/>.
        /// </summary>
        /// <param name="finalSize">The Final Size</param>
        private void ArrangeVerticalLables(Size finalSize)
        {
            int j = 0, row = 0;
            left = isOpposed ? width - margin.Right :
             (finalSize.Width - (width - (margin.Right + margin.Left)));
            foreach (IGrouping<int, ChartMultiLevelLabel> level in groupedLabels)
            {
                var collection = Axis.IsInversed ? level.Reverse() : level;
                foreach (ChartMultiLevelLabel label in collection)
                {
                    currentLabel = label;
                    if (SetValues(label))
                    {
                        CheckLabelRange();
                        continue;
                    }

                    SetTextBlockPosition();
                    SetBorderStyle(j, row, collection, false);
                    j++;
                }

                if (isOpposed)
                    left += width;
                else
                    left -= width;
                j = 0;
                row++;
            }

            ResetLabelValues();
        }

        /// <summary>
        /// To calculate the left values for text block and border of vertical axis's <see cref="MultiLevelLabelsPanel"/>.
        /// </summary>
        /// <returns>Returns the calculated actual left value.</returns>
        private double CalculateActualLeft()
        {
            double actualLeft = 0;
            actualLeft = isOpposed ? (left - width) : left;
            return actualLeft;
        }

        /// <summary>
        /// To calculate the top value for text block and border of horizontal axis's <see cref="MultiLevelLabelsPanel"/>.
        /// </summary>
        /// <returns>Returns the Calculated actual top value.</returns>
        private double CalculateActualTop()
        {
            double actualTop = isOpposed ? (top - height) : top;
            return actualTop;
        }

        /// <summary>
        /// To reset the current values of <see cref="MultiLevelLabelsPanel"/>
        /// </summary>
        private void ResetLabelValues()
        {
            currentBorderPos = 0;
            currentBracePos = 0;
            currentLabel = null;
            currentRow = 0;
            if (Axis.Orientation == Orientation.Horizontal)
            {
                height = 0;
                top = 0;
            }
            else
            {
                width = 0;
                left = 0;
            }
        }

        /// <summary>
        /// To calculate the current label's start and end position
        /// </summary>
        /// <param name="label">The Label</param>
        /// <returns>Returns a value indicating whether the range permissible.</returns>
        private bool SetValues(ChartMultiLevelLabel label)
        {
            startValue = CalculatePosition(label.Start);
            endValue = CalculatePosition(label.End);
            if (startValue > endValue || startValue == endValue)
                return true;
            if (Axis.Orientation == Orientation.Horizontal)
            {
                start = Math.Round(Axis.ValueToCoefficient(startValue)
                    * (Axis.RenderedRect.Width)) + Axis.ActualPlotOffset;
                end = Math.Round(Axis.ValueToCoefficient(endValue)
                    * (Axis.RenderedRect.Width)) + Axis.ActualPlotOffset;
            }
            else
            {
                start = (1 - Axis.ValueToCoefficientCalc(startValue)) *
                     Axis.RenderedRect.Height + Axis.ActualPlotOffset;
                end = (1 - Axis.ValueToCoefficientCalc(endValue)) *
                    Axis.RenderedRect.Height + Axis.ActualPlotOffset;
            }

            return false;
        }

        /// <summary>
        /// To set the thickness for rectangle border style of vertical axis
        /// </summary>
        /// <param name="j">The Index.</param>
        /// <param name="row">The Row</param>
        /// <param name="collection">The Collection</param>
        private void SetVerticalLabelRectangle(
            int j,
            int row,
            IEnumerable<ChartMultiLevelLabel> collection)
        {
            var thickness = Axis.LabelBorderWidth;
            var border = borderRecycler[currentBorderPos];
            if (row == 0 && Axis.ShowLabelBorder)
                border.BorderThickness = isOpposed ?
                new Thickness(0, thickness, thickness, thickness) :
                new Thickness(thickness, thickness, 0, thickness);
            else
                border.BorderThickness = new Thickness(thickness);

            SetBorderPosition(collection.Count());
        }

        /// <summary>
        /// To set the thickness for WithoutTopAndBottom border style of vertical axis.
        /// </summary>
        /// <param name="j">The Index</param>
        /// <param name="collection">The Collection</param>
        private void SetVerticalBorderWithoutTopAndBottom(
            int j,
           IEnumerable<ChartMultiLevelLabel> collection)
        {
            var thickness = Axis.LabelBorderWidth;
            var border = borderRecycler[currentBorderPos];
            border.BorderThickness = new Thickness(0, thickness, 0, thickness);
            SetBorderPosition(collection.Count());
        }

        /// <summary>
        /// To set current border position
        /// </summary>
        /// <param name="count">The Count</param>
        private void SetBorderPosition(int count)
        {
            var border = borderRecycler[currentBorderPos];
            if (Axis.MultiLevelLabelsBorderType == BorderType.None)
                border.BorderThickness = new Thickness(0);
            if (Axis.Orientation == Orientation.Horizontal)
                SetHorizontalBorder(border, count);
            else
                SetVerticalBorder(border, count);
        }

        /// <summary>
        /// To set horizontal multi -level labels border
        /// </summary>
        /// <param name="border">The Border</param>
        /// <param name="count">The Count</param>
        private void SetHorizontalBorder(Border border, int count)
        {
            double topValue = 0;
            double leftValue = 0;
            if (Axis.MultiLevelLabelsBorderType != BorderType.None)
            {
                if (currentRow == 0 && Axis.ShowLabelBorder)
                    border.Height = height;
                else
                    border.Height = height + Axis.LabelBorderWidth;
                border.Width = (Axis.IsInversed ? (start - end) : (end - start)) + Axis.LabelBorderWidth;
                leftValue = (Axis.IsInversed ? end : start) - Axis.LabelBorderWidth / 2;
                if (((currentRow == 0 && !isOpposed) || isOpposed) && Axis.ShowLabelBorder)
                    topValue = CalculateActualTop();
                else
                    topValue = CalculateActualTop() - Axis.LabelBorderWidth;
            }
            else
            {
                border.Height = height;
                border.Width = (Axis.IsInversed ? (start - end) : (end - start));
                leftValue = Axis.IsInversed ? end : start;
                topValue = CalculateActualTop();
            }

            Canvas.SetLeft(border, leftValue);
            Canvas.SetTop(border, topValue);
        }

        /// <summary>
        /// To set vertical multi-level labels border
        /// </summary>
        /// <param name="border">The Border</param>
        /// <param name="count">The Count</param>
        private void SetVerticalBorder(Border border, int count)
        {
            double topValue = 0;
            double leftValue = 0;
            if (Axis.MultiLevelLabelsBorderType != BorderType.None)
            {
                border.Width = width + Axis.LabelBorderWidth;
                border.Height = (Axis.IsInversed ? end - start : start - end)
                    + Axis.LabelBorderWidth;
                if (!Axis.ShowLabelBorder)
                    leftValue = isOpposed ? CalculateActualLeft() + margin.Right :
                        CalculateActualLeft() - margin.Left - Axis.LabelBorderWidth;
                else
                    leftValue = isOpposed ? CalculateActualLeft() - Axis.LabelBorderWidth : CalculateActualLeft();
                topValue = (Axis.IsInversed ? start : end) - Axis.LabelBorderWidth / 2;
            }
            else
            {
                border.Width = width;
                border.Height = (Axis.IsInversed ? end - start : start - end);
                leftValue = CalculateActualLeft();
                topValue = (Axis.IsInversed ? start : end);
            }

            Canvas.SetLeft(border, leftValue);
            Canvas.SetTop(border, topValue);
        }

        /// <summary>
        /// To place the vertical axis's label
        /// </summary>
        /// <param name="txtBlock">The Text Block</param>
        private void SetVerticalLabelAlignment(TextBlock txtBlock)
        {
            double xPos = 0;
            CalculateTextBlockPosition(txtBlock);
            if (Axis.OpposedPosition)
                xPos = Axis.LabelsPosition == AxisElementPosition.Outside ? (left - borderPadding) + Axis.LabelBorderWidth / 2 :
                    (left + width) - borderPadding + Axis.LabelBorderWidth / 2;
            else
                xPos = CalculateActualLeft() + borderPadding / 2;
            if (!Axis.ShowLabelBorder)
                xPos = isOpposed ? xPos + Axis.LabelBorderWidth / 2 :
                    xPos - Axis.LabelBorderWidth / 2;
            else
            {
                if (Axis.MultiLevelLabelsBorderType == BorderType.Brace)
                    xPos = isOpposed ? xPos - Axis.LabelBorderWidth / 2 :
                        xPos;
                else
                    xPos = isOpposed ? xPos - Axis.LabelBorderWidth / 2 :
                        xPos + Axis.LabelBorderWidth / 2;
            }

            Canvas.SetLeft(txtBlock, xPos);
            Canvas.SetTop(txtBlock, txtBlockTop);
        }

        /// <summary>
        /// To align the vertical axis's label
        /// </summary>
        /// <param name="txtBlock">The TextBlock To Calculate Position</param>
        private void CalculateTextBlockPosition(TextBlock txtBlock)
        {
            switch (currentLabel.LabelAlignment)
            {
                case LabelAlignment.Far:
                    {
                        double position = 0;
                        if (Axis.OpposedPosition)
                            position = (Axis.IsInversed ? end : start)
                                - ((borderPadding - (margin.Right)) + Axis.LabelBorderWidth);
                        else
                            position = (Axis.IsInversed ? start : end)
                                + ((borderPadding - (margin.Right)) + Axis.LabelBorderWidth);
                        txtBlockTop = Axis.OpposedPosition ?
                            position - txtBlock.Width
                            : position + txtBlock.Width;
                    }

                    break;
                case LabelAlignment.Center:
                    {
                        var midValue = startValue + (endValue - startValue) / 2;
                        txtBlockTop = Axis.OpposedPosition ? CalculateMidValue(midValue)
                        - txtBlock.Width / 2 :
                        CalculateMidValue(midValue) + txtBlock.Width / 2;
                    }

                    break;
                case LabelAlignment.Near:
                    {
                        if (Axis.OpposedPosition)
                            txtBlockTop = (Axis.IsInversed ? start : end)
                               + (margin.Left + margin.Right + Axis.LabelBorderWidth);
                        else
                            txtBlockTop = (Axis.IsInversed ? end : start)
                                - (margin.Left + margin.Right + Axis.LabelBorderWidth);
                    }

                    break;
            }
        }

        /// <summary>
        /// Calculates the corresponding  screen point of vertical axis value.
        /// </summary>
        /// <param name="midValue">The Mid Value</param>
        /// <returns>Returns the middle screen point value.</returns>
        private double CalculateMidValue(double midValue)
        {
            return ((1 - Axis.ValueToCoefficientCalc(midValue))
                                * (Axis.RenderedRect.Height)) + Axis.ActualPlotOffset;
        }

        /// <summary>
        /// To set the text blocks and borders of horizontal axis's <see cref="MultiLevelLabelsPanel"/>.
        /// </summary>
        /// <param name="finalSize">The Final Size</param>
        private void ArrangeHorizontalLabels(Size finalSize)
        {
            int j = 0, row = 0;
            top = isOpposed ? finalSize.Height : 0;
            foreach (IGrouping<int, ChartMultiLevelLabel> level in groupedLabels)
            {
                var collection = Axis.IsInversed ? level.Reverse() : level;
                foreach (ChartMultiLevelLabel label in collection)
                {
                    currentLabel = label;
                    if (SetValues(currentLabel))
                    {
                        CheckLabelRange();
                        continue;
                    }

                    SetTextBlockPosition();
                    SetBorderStyle(j, row, collection, true);
                    j++;
                }

                if (isOpposed)
                    top -= height;
                else
                    top += height;
                j = 0;
                row++;
                currentRow = row;
            }

            ResetLabelValues();
        }

        /// <summary>
        /// Removes the panel's children when label's start and end are equal
        /// </summary>
        private void CheckLabelRange()
        {
            if (Axis.MultiLevelLabelsBorderType == BorderType.Brace)
            {
                polylineRecycler.Remove(polylineRecycler[currentBracePos]);
                polylineRecycler.Remove(polylineRecycler[currentBracePos + 1]);
            }
            else
                borderRecycler.Remove(borderRecycler[currentBorderPos]);
            var textBlock = textBlockRecycler.Where(txtBlock => txtBlock.Tag == currentLabel).First();
            textBlockRecycler.Remove(textBlock);
        }

        /// <summary>
        /// Sets the border style.
        /// </summary>
        /// <param name="j">The Index</param>
        /// <param name="row">The Row</param>
        /// <param name="collection">The Collection</param>
        /// <param name="isHorizontalAxis">Indicates Horizontal Axis</param>
        private void SetBorderStyle(
            int j,
            int row,
            IEnumerable<ChartMultiLevelLabel> collection,
            bool isHorizontalAxis)
        {
            if (Axis.MultiLevelLabelsBorderType == BorderType.Brace)
            {
                if (isHorizontalAxis)
                    SetHorizontalBrace(j, row, collection);
                else
                    SetVerticalBrace(j, row, collection);
                currentBracePos += 2;
            }
            else
            {
                if (Axis.MultiLevelLabelsBorderType == BorderType.None)
                    SetBorderPosition(collection.Count());
                else if (Axis.MultiLevelLabelsBorderType == BorderType.Rectangle)
                {
                    if (isHorizontalAxis)
                        SetHorizontalLabelRectangle(j, row, collection);
                    else
                        SetVerticalLabelRectangle(j, row, collection);
                }
                else
                {
                    if (isHorizontalAxis)
                        SetHorizontalBorderWithoutTopandBottom(j, collection);
                    else
                        SetVerticalBorderWithoutTopAndBottom(j, collection);
                }

                currentBorderPos++;
            }
        }

        /// <summary>
        /// To set the thickness for rectangle border style
        /// of horizontal axis's <see cref="MultiLevelLabelsPanel"/>
        /// </summary>
        /// <param name="j">The Index</param>
        /// <param name="row">The Row</param>
        /// <param name="collection">The Collection</param>
        private void SetHorizontalLabelRectangle(
            int j,
            int row,
            IEnumerable<ChartMultiLevelLabel> collection)
        {
            var border = borderRecycler[currentBorderPos];
            var thickness = Axis.LabelBorderWidth;
            if (row == 0 && Axis.ShowLabelBorder)
                border.BorderThickness = isOpposed ?
                    new Thickness(thickness, thickness, thickness, 0) :
                    new Thickness(thickness, 0, thickness, thickness);
            else
                border.BorderThickness = new Thickness(thickness);
            SetBorderPosition(collection.Count());
        }

        /// <summary>
        /// To set the thickness for Without Top and Bottom border style
        /// of vertical axis's <see cref="MultiLevelLabelsPanel"/>
        /// </summary>
        /// <param name="j">The Index</param>
        /// <param name="collection">The Collection</param>
        private void SetHorizontalBorderWithoutTopandBottom(
            int j,
           IEnumerable<ChartMultiLevelLabel> collection)
        {
            var border = borderRecycler[currentBorderPos];
            var thickness = Axis.LabelBorderWidth;
            border.BorderThickness = new Thickness(thickness, 0, thickness, 0);
            SetBorderPosition(collection.Count());
        }

        /// <summary>
        /// To draw the brace border style of horizontal axis's <see cref="MultiLevelLabelsPanel"/>
        /// </summary>
        /// <param name="j">The Index</param>
        /// <param name="row">The Row</param>
        /// <param name="collection">The Collection</param>
        private void SetHorizontalBrace(int j, int row, IEnumerable<ChartMultiLevelLabel> collection)
        {
            var polyline1 = polylineRecycler[currentBracePos];
            var polyline2 = polylineRecycler[currentBracePos + 1];
            double txtBlockHeight = 0;
            double txtBlockwidth = 0;
            var textBlock = textBlockRecycler.Where(child => child.Tag == currentLabel).FirstOrDefault();
            if (textBlock != null)
            {
                txtBlockHeight = textBlock.ActualHeight > 0 ?
                    (textBlock.ActualHeight / 2) : (textBlock.DesiredSize.Height / 2);
                txtBlockHeight = isOpposed ? txtBlockHeight - margin.Top :
                    txtBlockHeight + margin.Top;
                txtBlockwidth = textBlock.Width;
            }

            var x1 = (Axis.IsInversed ? end : start);
            var x2 = (Axis.IsInversed ? start : end);
            var x3 = txtBlockLeft + txtBlockwidth + margin.Right;
            var y1 = top;
            if (!(Axis.ShowLabelBorder && currentRow == 0))
                y1 = isOpposed ? y1 - (margin.Top + margin.Bottom) : y1 + (margin.Top + margin.Bottom);
            var y2 = (CalculateActualTop() + borderPadding + txtBlockHeight);
            pointCollection1 = new PointCollection();
            pointCollection2 = new PointCollection();

            SetHorizontalBracePoints(x1, x2, x3, y1, y2);
            polyline1.Points = pointCollection1;
            polyline2.Points = pointCollection2;
            txtBlockLeft = 0;
        }

        /// <summary>
        /// To set the points for polyline
        /// </summary>
        /// <param name="x1">The x 1 Value</param>
        /// <param name="x2">The x 2 Value</param>
        /// <param name="x3">The x 3 Value</param>
        /// <param name="y1">The y 1 Value</param>
        /// <param name="y2">The y 2 Value</param>
        private void SetHorizontalBracePoints(
            double x1,
            double x2,
            double x3,
            double y1,
            double y2)
        {
            pointCollection1.Add(new Point(x1, y1));
            pointCollection1.Add(new Point(x1, y2));
            pointCollection1.Add(new Point(txtBlockLeft - margin.Left, y2));

            pointCollection2.Add(new Point(x2, y1));
            pointCollection2.Add(new Point(x2, y2));
            pointCollection2.Add(new Point(x3, y2));
        }

        /// <summary>
        /// To draw the brace border style of vertical axis's <see cref="MultiLevelLabelsPanel"/>
        /// </summary>
        /// <param name="j">The Index</param>
        /// <param name="row">The Row</param>
        /// <param name="collection">The Collection</param>
        private void SetVerticalBrace(int j, int row, IEnumerable<ChartMultiLevelLabel> collection)
        {
            var polyline1 = polylineRecycler[currentBracePos];
            var polyline2 = polylineRecycler[currentBracePos + 1];
            double txtBlockHeight = 0;
            double txtBlockwidth = 0;
            var textBlock = textBlockRecycler.Where(child => child.Tag == currentLabel).FirstOrDefault();
            var currentLeft = CalculateActualLeft();
            if (textBlock != null)
            {
                txtBlockHeight = textBlock.ActualHeight > 0 ?
               (textBlock.ActualHeight / 2) :
                (textBlock.DesiredSize.Height / 2);
                txtBlockwidth = textBlock.Width + Axis.LabelBorderWidth / 2;
            }

            var x1 = isOpposed ? currentLeft : currentLeft + width + Axis.LabelBorderWidth;
            if (!(Axis.ShowLabelBorder && currentRow == 0))
                x1 = isOpposed ? x1 + (margin.Left + margin.Right) : x1 - (margin.Left + margin.Right);
            var x2 = currentLeft + borderPadding / 2 + txtBlockHeight;
            var y1 = Axis.IsInversed ? start : end;
            var y2 = Axis.IsInversed ? end : start;
            var y3 = (Axis.OpposedPosition ? txtBlockTop : txtBlockTop - txtBlockwidth) - margin.Left;
            var y4 = (Axis.OpposedPosition ? txtBlockTop + txtBlockwidth
                : txtBlockTop) + margin.Left;
            pointCollection1 = new PointCollection();
            pointCollection2 = new PointCollection();
            SetVerticalBracePoints(x1, x2, y1, y2, y3, y4);
            polyline1.Points = pointCollection1;
            polyline2.Points = pointCollection2;
            txtBlockTop = 0;
        }

        /// <summary>
        /// To set the polyline points for vertical label's brace
        /// </summary>
        /// <param name="x1">The x 1 Value</param>
        /// <param name="x2">The x 2 Value</param>
        /// <param name="y1">The y 1 Value</param>
        /// <param name="y2">The y 2 Value</param>
        /// <param name="y3">The y 3 Value</param>
        /// <param name="y4">The y 4 Value</param>
        private void SetVerticalBracePoints(
            double x1,
            double x2,
            double y1,
            double y2,
            double y3,
            double y4)
        {
            pointCollection1.Add(new Point(x1, y1));
            pointCollection1.Add(new Point(x2, y1));
            pointCollection1.Add(new Point(x2, y3));

            pointCollection2.Add(new Point(x1, y2));
            pointCollection2.Add(new Point(x2, y2));
            pointCollection2.Add(new Point(x2, y4));
        }

        /// <summary>
        /// To position the text blocks of horizontal axis's <see cref="MultiLevelLabelsPanel"/>
        /// </summary>
        private void SetTextBlockPosition()
        {
            double calculatedWidth = 0;
            var textBlock = textBlockRecycler.Where(child => child.Tag == currentLabel).FirstOrDefault();
            if (textBlock == null)
                return;
            if (Axis.Orientation == Orientation.Vertical)
                calculatedWidth = (Axis.IsInversed ? end - start : start - end)
                    - (margin.Left + margin.Right);
            else
                calculatedWidth = (Axis.IsInversed ? start - end : end - start)
                    - (margin.Left + margin.Right);

            textBlock.Width = textBlock.ActualWidth < textBlock.DesiredSize.Width ?
                textBlock.DesiredSize.Width : textBlock.ActualWidth;

            if (textBlock.Width > calculatedWidth)
            {
                textBlock.Width = calculatedWidth;
                textBlock.TextTrimming = TextTrimming.CharacterEllipsis;
                textBlock.TextWrapping = TextWrapping.NoWrap;
                ToolTipService.SetToolTip(textBlock, currentLabel.Text);
            }
            else
                ToolTipService.SetToolTip(textBlock, null);
            if (Axis.Orientation == Orientation.Horizontal)
                SetHorizontalLabelAlignment(textBlock);
            else
                SetVerticalLabelAlignment(textBlock);
        }

        /// <summary>
        /// To Align the text of horizontal axis's label based on label alignment
        /// </summary>
        /// <param name="txtBlock">The TextBlock</param>        
        private void SetHorizontalLabelAlignment(TextBlock txtBlock)
        {
            switch (currentLabel.LabelAlignment)
            {
                case LabelAlignment.Far:
                    {
                        var position = Axis.IsInversed ? start : end;
                        txtBlockLeft = position - txtBlock.Width;
                        txtBlockLeft -= (margin.Left + margin.Right + Axis.LabelBorderWidth);
                    }

                    break;
                case LabelAlignment.Near:
                    {
                        txtBlockLeft = Axis.IsInversed ? end : start;
                        txtBlockLeft += (margin.Left + margin.Right + Axis.LabelBorderWidth);
                    }

                    break;
                case LabelAlignment.Center:
                    {
                        var midValue = startValue + ((endValue - startValue) / 2);
                        txtBlockLeft = Axis.ActualPlotOffset +
                            ((Axis.ValueToCoefficientCalc(midValue)
                            * Axis.RenderedRect.Width) - txtBlock.Width / 2);
                    }

                    break;
            }

            SetTextBlockPosition(txtBlock);
        }

        /// <summary>
        /// To set horizontal multi-level axis labels text block position
        /// </summary>
        /// <param name="txtBlock">The Label <see cref="TextBlock"/></param>
        private void SetTextBlockPosition(TextBlock txtBlock)
        {
            Canvas.SetLeft(txtBlock, txtBlockLeft);
            var padding = isOpposed ? (borderPadding - margin.Top - margin.Bottom) :
                (borderPadding + margin.Top);
            if (Axis.MultiLevelLabelsBorderType == BorderType.Rectangle)
            {
                if (Axis.ShowLabelBorder)
                {
                    if (isOpposed)
                        Canvas.SetTop(txtBlock, CalculateActualTop() + (borderPadding / 2 + margin.Top) + Axis.LabelBorderWidth / 2);
                    else
                        Canvas.SetTop(txtBlock, CalculateActualTop() + (borderPadding / 2 + margin.Top) - Axis.LabelBorderWidth / 2);
                }
                else
                    Canvas.SetTop(txtBlock, CalculateActualTop() + (borderPadding / 2 + margin.Top) - Axis.LabelBorderWidth / 2);
            }
            else if (Axis.MultiLevelLabelsBorderType == BorderType.Brace)
                Canvas.SetTop(txtBlock, CalculateActualTop() + padding);
            else
                Canvas.SetTop(txtBlock, CalculateActualTop() + (borderPadding / 2 + margin.Top));
        }

        /// <summary>
        /// Calculates the double value corresponding object.
        /// </summary>
        /// <param name="data">The Object Data</param>
        /// <returns>Returns the double value corresponding object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private double CalculatePosition(object data)
        {
            if (Axis is NumericalAxis)
                return Convert.ToDouble(data);
            else if (Axis is DateTimeAxis)
            {
                if (data is DateTime)
                    return ((DateTime)data).ToOADate();
                else if (data is string)
                    return (Convert.ToDateTime(data.ToString())).ToOADate();
                else
                    return Convert.ToDouble(data);
            }
            else if (Axis is TimeSpanAxis)
            {
                if (data is TimeSpan)
                    return ((TimeSpan)data).TotalMilliseconds;
                else if (data is string)
                    return (TimeSpan.Parse(data.ToString())).TotalMilliseconds;
                else
                    return Convert.ToDouble(data);
            }
            else if (Axis is LogarithmicAxis)
            {
                data = Convert.ToDouble(data) > 0 ? data : 1;
                return Math.Log(Convert.ToDouble(data), (Axis as LogarithmicAxis).LogarithmicBase);
            }

            return Convert.ToDouble(data);
        }

        /// <summary>
        /// Checks the opposed position based on axis and it's label position.
        /// </summary>
        /// <returns>Returns the <see cref="bool"/> value based on axis and its label position</returns>
        private bool IsOpposed()
        {
            return ((Axis.OpposedPosition)
                && Axis.LabelsPosition == AxisElementPosition.Outside)
                 || (!(Axis.OpposedPosition)
                 && Axis.LabelsPosition == AxisElementPosition.Inside);
        }

        /// <summary>
        /// To calculate the desired size of <see cref="MultiLevelLabelsPanel"/> based on its children's available size
        /// </summary>
        /// <param name="availableSize">The Available Size</param>
        /// <returns>Returns the desired size of <see cref="MultiLevelLabelsPanel"/></returns>
        private Size CalculateDesiredSize(Size availableSize)
        {
            double actualValue = 0;
            actualValue = textBlockRecycler.Max(txtBlock => txtBlock.DesiredSize.Height)
                  + (borderPadding + margin.Top + margin.Bottom);
            var desiredValue = Math.Max(actualValue, Axis.LabelExtent)
                    * groupedLabels.Count();
            if (!Axis.ShowLabelBorder
                && Axis.MultiLevelLabelsBorderType != BorderType.None)
                desiredValue += Axis.LabelBorderWidth;
            if (Axis.Orientation == Orientation.Horizontal)
            {
                height = actualValue;
                desiredSize = new Size(availableSize.Width, desiredValue);
            }
            else
            {
                width = actualValue;
                desiredSize = new Size(desiredValue, availableSize.Height);
            }

            return desiredSize;
        }

        /// <summary>
        /// To calculate the actual plot offset of axis
        /// </summary>
        private void CalculateActualPlotOffset()
        {
            Axis.ActualPlotOffset = Axis.PlotOffset < 0 ? 0 : Axis.PlotOffset;
        }

        /// <summary>
        /// Generates the children of <see cref="MultiLevelLabelsPanel"/>
        /// </summary>
        /// <param name="labelsCount">The Labels Count</param>
        private void GenerateContainer(int labelsCount)
        {
            Binding binding = new Binding();
            binding.Source = Axis;
            binding.Path = new PropertyPath("Visibility");
            if (!textBlockRecycler.BindingProvider.Keys.Contains(UIElement.VisibilityProperty))
                textBlockRecycler.BindingProvider.Add(UIElement.VisibilityProperty, binding);
            textBlockRecycler.GenerateElements(labelsCount);
            if (Axis.MultiLevelLabelsBorderType != BorderType.Brace)
            {
                if (polylineRecycler != null && polylineRecycler.Count > 0)
                    polylineRecycler.Clear();
                if (!borderRecycler.BindingProvider.Keys.Contains(UIElement.VisibilityProperty))
                    borderRecycler.BindingProvider.Add(UIElement.VisibilityProperty, binding);
                borderRecycler.GenerateElements(labelsCount);
            }
            else
            {
                if (borderRecycler != null && borderRecycler.Count > 0)
                    borderRecycler.Clear();
                if (!polylineRecycler.BindingProvider.Keys.Contains(UIElement.VisibilityProperty))
                    polylineRecycler.BindingProvider.Add(UIElement.VisibilityProperty, binding);
                polylineRecycler.GenerateElements(2 * labelsCount);
            }

            SetLabelProperty();
        }

        /// <summary>
        /// To set the properties of current label to generated <see cref="TextBlock"/> and <see cref="Border"/>
        /// </summary>
        private void SetLabelProperty()
        {
            int pos = 0;
            int borderPos = 0;
            int bracePos = 0;
            foreach (ChartMultiLevelLabel label in Axis.MultiLevelLabels)
            {
                textBlockRecycler[pos].TextTrimming = TextTrimming.None;
                if (Axis.MultiLevelLabelsBorderType == BorderType.Brace)
                {
                    label.SetBraceVisualBinding(
                        textBlockRecycler[pos],
                        polylineRecycler[bracePos],
                        polylineRecycler[bracePos + 1],
                        Axis);
                    bracePos += 2;
                }
                else
                {
                    label.SetVisualBinding(textBlockRecycler[pos], borderRecycler[borderPos], Axis);
                    borderPos++;
                }

                pos++;
            }
        }

        #endregion

        #endregion
    }
}
