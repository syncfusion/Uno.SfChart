// <copyright file="ChartCartesianAxisLabelsPanel.cs" company="Syncfusion. Inc">
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
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Text;

    /// <summary>
    /// Represents layout panel for chart axis labels.
    /// </summary>
    /// <remarks>
    /// The elements inside the panel comprises of <see cref="ChartAxis"/> labels.You can customize the label elements appearance using  
    /// <see cref="ChartAxis.LabelTemplate"/> property.
    /// </remarks>
    public partial class ChartCartesianAxisLabelsPanel : ILayoutCalculator
    {
        #region Fields

        private Rect bounds;

        private Panel labelsPanels;

        private Size desiredSize;

        private UIElementsRecycler<TextBlock> textBlockRecycler;

        private UIElementsRecycler<ContentControl> contentControlRecycler;

        private UIElementsRecycler<Border> borderRecycler;

        private List<Border> borders;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartCartesianAxisLabelsPanel"/>.
        /// </summary>
        /// <param name="panel">The Panel</param>
        public ChartCartesianAxisLabelsPanel(Panel panel)
        {
            LabelLayout = null;
            labelsPanels = panel;
            textBlockRecycler = new UIElementsRecycler<TextBlock>(panel);
            contentControlRecycler = new UIElementsRecycler<ContentControl>(panel);
            borderRecycler = new UIElementsRecycler<Border>(panel);
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets the panel.
        /// </summary>
        /// <value>
        /// The panel.
        /// </value>
        public Panel Panel
        {
            get { return labelsPanels; }
        }

        /// <summary>
        /// Gets the desired size of the panel.
        /// </summary>
        public Size DesiredSize
        {
            get
            {
                return desiredSize;
            }
        }

        /// <summary>
        /// Gets or sets the chart axis of the panel.
        /// </summary>
        public ChartAxis Axis { get; set; }

        /// <summary>
        /// Gets the children count in the panel.
        /// </summary>
        public List<UIElement> Children
        {
            get
            {
                children = textBlockRecycler.generatedElements.Cast<UIElement>().ToList();
                if (children.Count < 1)
                    children = contentControlRecycler.generatedElements.Cast<UIElement>().ToList();
                return children;
            }
        }

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>
        /// The left.
        /// </value>
        public double Left
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>
        /// The top.
        /// </value>
        public double Top
        {
            get;
            set;
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the direct children of <see cref="ChartCartesianAxisLabelsPanel"/>
        /// </summary>
        internal List<UIElement> children { get; set; }
        
        /// <summary>
        /// Gets or sets the <see cref="AxisLabelLayout"/>.
        /// </summary>
        internal AxisLabelLayout LabelLayout { get; set; }

        internal Rect Bounds
        {
            get { return bounds; }
            set { bounds = value; }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Method declaration for Measure
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        public Size Measure(Size availableSize)
        {
            LabelLayout = AxisLabelLayout.CreateAxisLayout(Axis, Children);
            if (borders != null)
                LabelLayout.Borders = borders;
            desiredSize = LabelLayout.Measure(availableSize);
            return desiredSize;
        }

        /// <summary>
        /// Method declaration for Arrange
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        public Size Arrange(Size finalSize)
        {
            if (LabelLayout != null)
            {
                LabelLayout.Left = Left;
                LabelLayout.Top = Top;
                LabelLayout.Arrange(DesiredSize);
                LabelLayout = null;
            }
            return finalSize;
        }

        /// <summary>
        /// Seek the elements.
        /// </summary>
        public void DetachElements()
        {
            labelsPanels = null;
            if (textBlockRecycler != null)
                textBlockRecycler.Clear();
            if (contentControlRecycler != null)
                contentControlRecycler.Clear();
            if (borderRecycler != null)
                borderRecycler.Clear();
        }

        /// <summary>
        /// Method declaration for UpdateElements
        /// </summary>
        public void UpdateElements()
        {
            var axis = Axis as ChartAxisBase2D;
            if (axis != null && (!axis.ShowLabelBorder || axis.LabelBorderWidth == 0)
                && borderRecycler.Count > 0)
                borderRecycler.Clear();
            GenerateContainers();
        }

        #endregion

        #region Internal Methods
        
        internal void Dispose()
        {
            UnbindAndDetachContentControlRecyclerElements(true);

            if (textBlockRecycler != null && textBlockRecycler.Count > 0)
            {
                foreach (var textBlock in textBlockRecycler)
                {
                    textBlock.ClearValue(TextBlock.TagProperty);
                }

                textBlockRecycler.Clear();
                textBlockRecycler = null;
            }

            Axis = null;
        }

        internal void SetOffsetValues(double left, double top, double width, double height)
        {
            int count = Children.Count;
            if (count == 0) return;

            if (Axis.Orientation == Orientation.Horizontal)
            {
                var firstLabelLeft = Canvas.GetLeft(Children[0]);
                var lastLabelRight = Canvas.GetLeft(Children[count - 1]) + Children[count - 1].DesiredSize.Width;

                if (firstLabelLeft < Axis.ArrangeRect.Left)
                {
                    left = firstLabelLeft;
                    width += (Axis.ArrangeRect.Left - firstLabelLeft);
                }

                if (Axis.ArrangeRect.Right < lastLabelRight)
                    width += (lastLabelRight - Axis.ArrangeRect.Width);
            }
            else
            {
                var lastLabelTop = Canvas.GetTop(this.Children[count - 1]);
                var firstLabelBottom = Canvas.GetTop(Children[0]) + Children[0].DesiredSize.Height;
                if (lastLabelTop < Axis.ArrangeRect.Top)
                {
                    top = lastLabelTop;
                    height += (Axis.ArrangeRect.Top - lastLabelTop);
                }
                if (Axis.ArrangeRect.Bottom < firstLabelBottom)
                    height += (firstLabelBottom - DesiredSize.Height);
            }
            Bounds = new Rect(left, top, width, height);
        }

        internal void GenerateContainers()
        {
            int pos = 0;

            ObservableCollection<ChartAxisLabel> visibleLabels = Axis.VisibleLabels;
            var prefixLabeltemplate = this.Axis.PrefixLabelTemplate;
            var postfixLabelTemplate = this.Axis.PostfixLabelTemplate;
            var axis = Axis as ChartAxisBase2D;
            if (axis != null && axis.ShowLabelBorder && axis.LabelBorderWidth > 0)
                GenerateBorder();
            if (Axis.LabelTemplate == null && Axis.PrefixLabelTemplate == null && Axis.PostfixLabelTemplate == null && Axis.LabelStyle == null)
            {
#if WINDOWS_UAP
                //UWP-764 - https://social.msdn.microsoft.com/Forums/windowsapps/en-US/79b55ad0-9479-4d99-92c2-58b79480849a/uwp-problems-with-printing?forum=wpdevelop 
                //This issue is framework issue and the above forum link mentioned that this issue will be fixed.
                if (Axis.Area is SfChart)
                {
                    textBlockRecycler.Clear();
                    contentControlRecycler.GenerateElements(visibleLabels.Count);
                }
                else
                {
                    contentControlRecycler.Clear();
                    textBlockRecycler.GenerateElements(visibleLabels.Count);
                }
#else
                contentControlRecycler.Clear();
                textBlockRecycler.GenerateElements(visibleLabels.Count);
#endif
                foreach (var item in visibleLabels)
                {
                    if (item.LabelContent != null)
                    {
#if WINDOWS_UAP
                        if (Axis.Area is SfChart)
                        {
                            ClearLabelBinding(contentControlRecycler[pos]);
                            ContentControl control = contentControlRecycler[pos];

                            SetLabelStyle(item, control);
                            
                            control.Content = item.LabelContent.ToString();
                            control.Tag = visibleLabels[pos];
                        }
                        else
                        {
                            var textblock = textBlockRecycler[pos];
                            textblock.Text = item.LabelContent.ToString();
                            textblock.Tag = visibleLabels[pos];
                        }
#else
                        var textblock = textBlockRecycler[pos];
                        textblock.Text = item.LabelContent.ToString();
                        textblock.Tag = visibleLabels[pos];
#endif
                    }
                    pos++;
                }
            }
            else if (this.Axis.LabelTemplate == null)
            {
                textBlockRecycler.Clear();
                UnbindAndDetachContentControlRecyclerElements(false);
                contentControlRecycler.GenerateElements(visibleLabels.Count);

                foreach (var item in visibleLabels)
                {
                    ClearLabelBinding(contentControlRecycler[pos]);

                    ContentControl control = contentControlRecycler[pos];
                    item.PrefixLabelTemplate = prefixLabeltemplate;
                    item.PostfixLabelTemplate = postfixLabelTemplate;
                    SetLabelStyle(item, control);
                    control.Content = item;
                    control.Tag = visibleLabels[pos];
                    control.ContentTemplate = ChartDictionaries.GenericCommonDictionary["AxisLabelsCustomTemplate"] as DataTemplate;
                    control.ApplyTemplate();
                    pos++;
                }
            }
            else
            {
                textBlockRecycler.Clear();
                contentControlRecycler.GenerateElements(visibleLabels.Count);
                foreach (var item in visibleLabels)
                {
                    ContentControl control = contentControlRecycler[pos];
                    control.ContentTemplate = this.Axis.LabelTemplate;
                    control.ApplyTemplate();
                    control.Content = item;
                    control.Tag = visibleLabels[pos];
                    pos++;
                }
            }
        }

        #endregion

        #region Private Methods

        private void UnbindAndDetachContentControlRecyclerElements(bool isDisposing)
        {
            if (contentControlRecycler != null && contentControlRecycler.Count > 0)
            {
                foreach (var contentControl in contentControlRecycler)
                {
                    contentControl.ClearValue(ContentControl.ForegroundProperty);
                    contentControl.ClearValue(ContentControl.FontSizeProperty);
                    contentControl.ClearValue(ContentControl.FontFamilyProperty);
                    contentControl.ClearValue(ContentControl.TagProperty);
                    contentControl.ClearValue(ContentControl.ContentProperty);
                    contentControl.ClearValue(ContentControl.ContentTemplateProperty);
                    contentControl.ClearValue(ContentControl.DataContextProperty);

                    contentControl.Content = null;
                    contentControl.ContentTemplate = null;
                }

                if(isDisposing)
                {
                    contentControlRecycler.Clear();
                }
            }
        }
        
        private void SetLabelStyle(ChartAxisLabel chartAxisLabel, ContentControl control)
        {
            var style = Axis.LabelStyle;
            var rangeStyles = Axis.RangeStyles;
            if (rangeStyles != null && rangeStyles.Count > 0)
            {
                foreach (var chartAxisRangeStyle in rangeStyles)
                {
                    var range = chartAxisRangeStyle.Range;
                    if (range.Start <= chartAxisLabel.Position && range.End >= chartAxisLabel.Position && chartAxisRangeStyle.LabelStyle != null)
                    {
                        style = chartAxisRangeStyle.LabelStyle;
                        break;
                    }
                }
            }

            if (style != null)
            {
                if (style.Foreground != null)
                {
                    var foregroundBinding = new Binding { Source = style, Path = new PropertyPath("Foreground") };
                    control.SetBinding(Control.ForegroundProperty, foregroundBinding);
                }
                if (style.FontSize > 0.0)
                {
                    var fontSizeBinding = new Binding { Source = style, Path = new PropertyPath("FontSize") };
                    control.SetBinding(Control.FontSizeProperty, fontSizeBinding);
                }
                if (style.FontFamily != null)
                {
                    var fontFamilyBinding = new Binding { Source = style, Path = new PropertyPath("FontFamily") };
                    control.SetBinding(Control.FontFamilyProperty, fontFamilyBinding);
                }
            }
        }

        private void ClearLabelBinding(ContentControl contentControl)
        {
            contentControl.ClearValue(ContentControl.ForegroundProperty);
            contentControl.ClearValue(ContentControl.FontSizeProperty);
            contentControl.ClearValue(ContentControl.FontFamilyProperty);
        }

        private void GenerateBorder()
        {
            if (!borderRecycler.BindingProvider.Keys.Contains(Border.VisibilityProperty))
            {
                Binding binding = new Binding();
                binding.Source = Axis;
                binding.Path = new PropertyPath("Visibility");
                borderRecycler.BindingProvider.Add(Border.VisibilityProperty, binding);
            }
            if (!borderRecycler.BindingProvider.Keys.Contains(Border.BorderBrushProperty))
            {
                Binding binding = new Binding();
                binding.Source = Axis;
                binding.Path = new PropertyPath("LabelBorderBrush");
                borderRecycler.BindingProvider.Add(Border.BorderBrushProperty, binding);
            }

            borderRecycler.GenerateElements(Axis.VisibleLabels.Count);
            borders = borderRecycler.generatedElements;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Represents a base of chart axis label layout. 
    /// </summary>
    public abstract partial class AxisLabelLayout
    {
        #region Fields

        private readonly List<UIElement> children;

        protected double BorderPadding = 10;

        protected double AngleForAutoRotate = 0;

        protected Thickness Margin = new Thickness(2, 2, 2, 2);
        
        #endregion

        #region Constructor

        /// <summary>
        /// initializes a new instance of the <see cref="ChartCartesianAxisLabelsPanel"/> class.
        /// </summary>
        /// <param name="axis">The Axis</param>
        /// <param name="elements">The Elements</param>
        protected AxisLabelLayout(ChartAxis axis, List<UIElement> elements)
        {
            Axis = axis;
            children = elements;
            DesiredSizes = new List<Size>();
        }

        #endregion

        #region Properties

        #region Internal Properties

        /// <summary>
        /// Gets or sets the left of the <see cref="ChartCartesianAxisLabelsPanel"/>
        /// </summary>
        internal double Left { get; set; }

        /// <summary>
        /// Gets or sets the top of the <see cref="ChartCartesianAxisLabelsPanel"/>
        /// </summary>
        internal double Top { get; set; }

        /// <summary>
        /// Gets or sets the borders of the <see cref="ChartCartesianAxisLabelsPanel"/>
        /// </summary>
        internal List<Border> Borders { get; set; }

        internal Size AvailableSize { get; set; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets or sets the rects of rows and columns of labels.
        /// </summary>
        protected internal List<Dictionary<int, Rect>> RectssByRowsAndCols { get; set; }

        /// <summary>
        /// Gets or sets the width and height of the element after rotating.
        /// </summary>
        protected List<Size> ComputedSizes { get; set; }

        /// <summary>
        /// Gets or sets the width and height of the element without rotating.
        /// </summary>
        protected List<Size> DesiredSizes { get; set; }

        /// <summary>
        /// Gets or sets the axis of the <see cref="ChartCartesianAxisLabelsPanel"/>
        /// </summary>
        protected ChartAxis Axis { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ChartCartesianAxisLabelsPanel"/> children.
        /// </summary>
        protected List<UIElement> Children
        {
            get { return children; }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Method used to create the axis layout.
        /// </summary>
        /// <param name="chartAxis">The Chart Axis</param>
        /// <param name="elements">The Elements</param>
        /// <returns>Returns the created layout.</returns>
        public static AxisLabelLayout CreateAxisLayout(ChartAxis chartAxis, List<UIElement> elements)
        {
            if (chartAxis.Orientation == Orientation.Horizontal)
            {
                return new HorizontalLabelLayout(chartAxis, elements);
            }
            return new VerticalLabelLayout(chartAxis, elements);
        }

        /// <summary>
        /// Method declaration for Measure.
        /// </summary>
        /// <param name="availableSize">The Available Size</param>
        /// <returns>Returns the size required for arranging the elements.</returns>
        public virtual Size Measure(Size availableSize)
        {
            if (Axis != null && Children.Count > 0)
            {
                bool needToRotate = (!double.IsNaN(Axis.LabelRotationAngle) && Axis.LabelRotationAngle != 0.0) || AngleForAutoRotate != 0;
                ComputedSizes = DesiredSizes;
                if (needToRotate) ComputedSizes = new List<Size>();
                AxisLabelsVisibilityBinding(); // Axis elements visibilty binding done here
                var labelMeaureSize = new Size();

                foreach (FrameworkElement element in Children)
                {
                    // WPF-41527 Axis label get shift after resizing window to minimum size with label rotation angle.
                    labelMeaureSize.Width = Math.Max(availableSize.Width, element.DesiredSize.Width);
                    labelMeaureSize.Height = Math.Max(availableSize.Height, element.DesiredSize.Height);
                    element.Measure(labelMeaureSize);

                    DesiredSizes.Add(element.DesiredSize);

                    if (needToRotate)
                    {
                        int labelIndex = Children.IndexOf(element);
                        double angle;
                        double tempAngle;
                        if (AngleForAutoRotate != 0)
                        {
                            angle = tempAngle = AngleForAutoRotate;
                        }
                        else
                        {
                            angle = Axis.LabelRotationAngle;
                            tempAngle = Math.Abs(Axis.LabelRotationAngle);
                        }

                        TransformGroup transformGroup;
                        TranslateTransform translateTransform;
                        RotateTransform rotateTransform;

                        if (angle < -360 || angle > 360)
                            angle %= 360;

                        element.RenderTransformOrigin = new Point(0.5, 0.5);

                        rotateTransform = new RotateTransform() { Angle = angle };

                        double tempAngleForRadians = angle;

                        if ((!Axis.OpposedPosition && Axis.LabelsPosition == AxisElementPosition.Outside)
                            || (Axis.OpposedPosition && Axis.LabelsPosition == AxisElementPosition.Inside))
                        {
                            if (Axis.Orientation == Orientation.Horizontal)
                            {
                                if ((tempAngleForRadians > 180 && tempAngleForRadians < 360)
                                    || (tempAngleForRadians < 0 && tempAngleForRadians > -180))
                                    tempAngleForRadians -= 180;
                            }
                            else
                            {
                                if ((tempAngle > 0 && tempAngle < 90) || (tempAngle > 270 && tempAngle < 360))
                                    tempAngleForRadians += 180;
                            }
                        }
                        else
                        {
                            if (Axis.Orientation == Orientation.Horizontal)
                            {
                                if ((tempAngleForRadians > 0 && tempAngleForRadians < 180)
                                    || (tempAngleForRadians < -180 && tempAngleForRadians > -360))
                                    tempAngleForRadians += 180;
                            }
                            else
                            {
                                if ((tempAngle > 90 && tempAngle < 180) || (tempAngle > 180 && tempAngle < 270))
                                    tempAngleForRadians += 180;
                            }
                        }

                        double angleRadians = (Math.PI * tempAngleForRadians) / 180;
                        double hypotenuse;
                        hypotenuse = element.DesiredSize.Width / 2;
                        double opposite = Math.Sin(angleRadians) * hypotenuse;
                        double adjacent = Math.Cos(angleRadians) * hypotenuse;

                        if (Axis.EdgeLabelsDrawingMode == EdgeLabelsDrawingMode.Shift && (labelIndex == 0 || labelIndex == Children.Count - 1))
                        {
                            adjacent = 0;
                            opposite = 0;
                        }

                        if (Axis.Orientation == Orientation.Horizontal)
                        {
                            if ((tempAngle >= 0 && tempAngle < 1) || (tempAngle > 359 && tempAngle <= 360)
                                || (tempAngle > 179 && tempAngle < 181)
                                || (Axis is CategoryAxis && (Axis as CategoryAxis).LabelPlacement == LabelPlacement.BetweenTicks))
                            {
                                translateTransform = new TranslateTransform();
                            }
                            else
                            {
                                translateTransform = new TranslateTransform() { X = adjacent };
                            }
                        }
                        else
                        {
                            if ((tempAngle > 89 && tempAngle < 91) || (tempAngle > 269 && tempAngle < 271))
                            {
                                translateTransform = new TranslateTransform();
                            }
                            else
                            {
                                translateTransform = new TranslateTransform() { Y = opposite };
                            }
                        }

                        transformGroup = new TransformGroup();
                        transformGroup.Children.Add(rotateTransform);
                        transformGroup.Children.Add(translateTransform);
                        element.RenderTransform = transformGroup;

                        ComputedSizes.Add(AxisLabelLayout.GetRotatedSize(angle, DesiredSizes.Last()));
                    }
                    else
                    {
                        element.RenderTransform = null;
                    }
                }

                CalculateActualPlotOffset(availableSize);
            }

            return new Size();
        }

        /// <summary>
        /// Method declaration for Arrange.
        /// </summary>
        /// <param name="finalSize">The Final Size</param>
        public virtual void Arrange(Size finalSize)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Checks for the side by side series.
        /// </summary>
        /// <returns>Returns true when any of registered series is side by side series</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        protected bool CheckCartesianSeries()
        {
            return (Axis.RegisteredSeries != null && Axis.RegisteredSeries.Count > 0 &&
                  Axis.RegisteredSeries.Any(
                     series => (series is CartesianSeries) &&
                     (series as CartesianSeries).IsSideBySide));
        }

        /// <summary>
        /// Checks the label placement
        /// </summary>
        /// <param name="isSidebySideSeries">Is Side By Side</param>
        /// <returns>Returns true when the label placement is between ticks</returns>
        protected bool CheckLabelPlacement(bool isSidebySideSeries)
        {
            var axis = Axis as CategoryAxis;
            var chartAxis = Axis as ChartAxisBase2D;
            return (chartAxis != null && !chartAxis.IsZoomed
                && RectssByRowsAndCols.Count == 1 &&
                ((axis != null &&
                     axis.LabelPlacement == LabelPlacement.BetweenTicks)
                     || !Axis.IsSecondaryAxis && isSidebySideSeries));
        }

        /// <summary>
        /// Checks for the intersection of the rectangles.
        /// </summary>
        /// <param name="r1">The First Rectangle</param>
        /// <param name="r2">The Second Rectangle</param>
        /// <param name="prevIndex">The Previous Index</param>
        /// <param name="currentIndex">The Current Index</param>
        /// <returns>Returns a value indicating whether the rectanges are intersected.</returns>
        protected bool IntersectsWith(Rect r1, Rect r2, int prevIndex, int currentIndex)
        {
            double angle = AngleForAutoRotate == 45 ? 45 : Axis.LabelRotationAngle;
            if (angle != 0)
            {
                var shape1Points = GetRotatedPoints(r1, prevIndex, angle);
                var shape2Points = GetRotatedPoints(r2, currentIndex, angle);

                return AxisLabelLayout.IntersectsWith(shape1Points, shape2Points);
            }

            return !(r2.Left > r1.Right ||
                     r2.Right < r1.Left ||
                     r2.Top > r1.Bottom ||
                     r2.Bottom < r1.Top);
        }

        /// <summary>
        /// Insert the <see cref="Rect"/> at the given row column index.
        /// </summary>
        /// <param name="rowOrColIndex">The Row Column Index</param>
        /// <param name="itemIndex">The Item Index</param>
        /// <param name="rect">The Rect</param>
        protected void InsertToRowOrColumn(int rowOrColIndex, int itemIndex, Rect rect)
        {
            if (RectssByRowsAndCols.Count <= rowOrColIndex)
            {
                RectssByRowsAndCols.Add(new Dictionary<int, Rect>());
                RectssByRowsAndCols[rowOrColIndex].Add(itemIndex, rect);
            }
            else
            {
                var rowOrColumn = RectssByRowsAndCols[rowOrColIndex].Last();
                Rect prevRect = rowOrColumn.Value;

                if (IntersectsWith(prevRect, rect, rowOrColumn.Key, itemIndex))
                {
                    InsertToRowOrColumn(++rowOrColIndex, itemIndex, rect);
                }
                else
                {
                    RectssByRowsAndCols[rowOrColIndex].Add(itemIndex, rect);
                }
            }
        }

        /// <summary>
        /// Calulates the bounds
        /// </summary>
        /// <param name="size">The Size</param>
        protected virtual void CalcBounds(double size)
        {
        }

        /// <summary>
        /// Checks the actual opposed position of the labels.
        /// </summary>
        /// <param name="axis">The Axis</param>
        /// <param name="isAxisOpposed">The Axis Opposed Indication</param>
        /// <returns>Returns the actual opposed position.</returns>
        protected static bool IsOpposed(ChartAxis axis, bool isAxisOpposed)
        {
            if (axis != null)
            {
                return ((isAxisOpposed) && axis.LabelsPosition == AxisElementPosition.Outside)
                     || (!(isAxisOpposed) && axis.LabelsPosition == AxisElementPosition.Inside);
            }

            return false;
        }

        /// <summary>
        /// Layuouts the axis labels.
        /// </summary>
        /// <returns>Returns desired height</returns>
        protected virtual double LayoutElements()
        {
            int i = 1;
            int prevIndex = 0;

            if (Axis.LabelsIntersectAction == AxisLabelsIntersectAction.Hide || AngleForAutoRotate == 90)
            {
                for (; i < Children.Count; i++)
                {
                    if (IntersectsWith(RectssByRowsAndCols[0][prevIndex], RectssByRowsAndCols[0][i], prevIndex, i))
                    {
                        if (i == Children.Count - 1 &&
                            (Axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.AlwaysVisible
                            || (Axis.EdgeLabelsVisibilityMode == EdgeLabelsVisibilityMode.Visible
                            && Axis is ChartAxisBase2D && !(Axis as ChartAxisBase2D).IsZoomed)))
                            Children[prevIndex].Visibility = Visibility.Collapsed;
                        else
                        {
                            var axis = Axis as NumericalAxis;
                            if (axis != null && axis.BreakExistence())
                            {
                                double previousContent = Convert.ToDouble(axis.VisibleLabels[prevIndex].LabelContent);
                                double currentContent = Convert.ToDouble(axis.VisibleLabels[i].LabelContent);
                                bool isBrkLabel = axis.BreakRanges.Any(item => item.Start == previousContent || item.End == previousContent);
                                if (isBrkLabel)
                                {
                                    if (axis.BreakRanges.Any(item => item.Start == currentContent || item.End == currentContent))
                                        Children[i + 1].Visibility = Visibility.Collapsed;
                                    else
                                        Children[i].Visibility = Visibility.Collapsed;
                                }
                                else
                                    Children[prevIndex].Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                Children[i].Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                    else
                    {
                        prevIndex = i;
                    }
                }
            }
            else if (Axis.LabelsIntersectAction == AxisLabelsIntersectAction.MultipleRows || Axis.LabelsIntersectAction == AxisLabelsIntersectAction.Wrap)
            {
                i = 1;
                prevIndex = 0;

                for (; i < Children.Count; i++)
                {
                    if (IntersectsWith(RectssByRowsAndCols[0][prevIndex], RectssByRowsAndCols[0][i], prevIndex, i))
                    {
                        Rect rect = RectssByRowsAndCols[0][i];
                        RectssByRowsAndCols[0].Remove(i);
                        InsertToRowOrColumn(1, i, rect);
                    }
                    else
                    {
                        prevIndex = i;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Calculates the actual plotoffset.
        /// </summary>
        /// <param name="availableSize">The Available Size</param>
        protected virtual void CalculateActualPlotOffset(Size availableSize)
        {
            Axis.ActualPlotOffset = Axis.PlotOffset < 0 ? 0 : Axis.PlotOffset;
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Checks whether two line segments are intersecting
        /// </summary>
        /// <param name="point11">The Point 11</param>
        /// <param name="point12">The Point 12</param>
        /// <param name="point21">The Point 21</param>
        /// <param name="point22">The Point 22</param>
        /// <returns>Returns a value indicating whether the lines are intersecting.</returns>
        private static bool DoLinesIntersect(Point point11, Point point12, Point point21, Point point22)
        {
            double d = (point22.Y - point21.Y) * (point12.X - point11.X) -
                (point22.X - point21.X) * (point12.Y - point11.Y);
            double na = (point22.X - point21.X) * (point11.Y - point21.Y) -
                         (point22.Y - point21.Y) * (point11.X - point21.X);
            double nb = (point12.X - point11.X) * (point11.Y - point21.Y) -
                         (point12.Y - point11.Y) * (point11.X - point21.X);

            if (d == 0)
                return false;

            double ua = na / d;
            double ub = nb / d;

            return (ua >= 0d && ua <= 1d && ub >= 0d && ub <= 1d);
        }

        /// <summary>
        /// Calculates the rotated size.
        /// </summary>
        /// <param name="angle">The Angle</param>
        /// <param name="size">The Size</param>
        /// <returns>Returns the rotated size.</returns>
        private static Size GetRotatedSize(double angle, Size size)
        {
            var angleRadians = (2 * Math.PI * angle) / 360;
            var sine = Math.Sin(angleRadians);
            var cosine = Math.Cos(angleRadians);
            var matrix = new Matrix(cosine, sine, -sine, cosine, 0, 0);

            var leftTop = matrix.Transform(new Point(0, 0));
            var rightTop = matrix.Transform(new Point(size.Width, 0));
            var leftBottom = matrix.Transform(new Point(0, size.Height));
            var rightBottom = matrix.Transform(new Point(size.Width, size.Height));
            var left = Math.Min(Math.Min(leftTop.X, rightTop.X), Math.Min(leftBottom.X, rightBottom.X));
            var top = Math.Min(Math.Min(leftTop.Y, rightTop.Y), Math.Min(leftBottom.Y, rightBottom.Y));
            var right = Math.Max(Math.Max(leftTop.X, rightTop.X), Math.Max(leftBottom.X, rightBottom.X));
            var bottom = Math.Max(Math.Max(leftTop.Y, rightTop.Y), Math.Max(leftBottom.Y, rightBottom.Y));

            return new Size(right - left, bottom - top);
        }

        /// <summary>
        /// Returns the points after translating the rect about (0,0) and then translating it by some x and y.
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        /// <param name="rect">Rect</param>
        /// <param name="translateX">Offset x to be translated after rotating</param>
        /// <param name="translateY">Offset y to be translated after rotating</param>
        /// <returns>Returns the rotated points.</returns>
        private static List<Point> GetRotatedPoints(double angle, Rect rect, double translateX, double translateY)
        {
            var angleRadians = (2 * Math.PI * angle) / 360;
            var sine = Math.Sin(angleRadians);
            var cosine = Math.Cos(angleRadians);
            var matrix = new Matrix(cosine, sine, -sine, cosine, translateX, translateY);
            var transformedPoints = new List<Point>();
            transformedPoints.Add(matrix.Transform(new Point(rect.Left, rect.Top)));
            transformedPoints.Add(matrix.Transform(new Point(rect.Right, rect.Top)));
            transformedPoints.Add(matrix.Transform(new Point(rect.Right, rect.Bottom)));
            transformedPoints.Add(matrix.Transform(new Point(rect.Left, rect.Bottom)));
            return transformedPoints;
        }

        /// <summary>
        /// Checks whether two polygons intersects.
        /// </summary>
        /// <param name="shape1Points">Polygon</param>
        /// <param name="shape2Points">Polygon</param>
        /// <returns></returns>
        private static bool IntersectsWith(List<Point> shape1Points, List<Point> shape2Points)
        {
            //Checks whether two lines from both the shapes intersects. 
            //If it intersects, it means both the shapes are intersecting.
            for (int i = 0; i < shape1Points.Count; i++)
            {
                var point11 = shape1Points[i];
                var nextIndex = i == shape1Points.Count - 1 ? 0 : i + 1;
                var point12 = shape1Points[nextIndex];
                for (int j = 0; j < shape2Points.Count; j++)
                {
                    var point21 = shape2Points[j];
                    nextIndex = j == shape2Points.Count - 1 ? 0 : j + 1;
                    var point22 = shape2Points[nextIndex];

                    if (AxisLabelLayout.DoLinesIntersect(point11, point12, point21, point22))
                        return true;
                }
            }

            return false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the points after rotating a rectangle.
        /// </summary>
        /// <param name="rect">The Rectangle</param>
        /// <param name="index">The Index</param>
        /// <returns>Returns the rotated points.</returns>
        private List<Point> GetRotatedPoints(Rect rect, int index, double angle)
        {
            //Get actual left and actual top of the element without rotating
            var left = rect.Left + (ComputedSizes[index].Width - DesiredSizes[index].Width) / 2;
            var top = rect.Top + (ComputedSizes[index].Height - DesiredSizes[index].Height) / 2;

            //Rotating the points about the origin (0,0) and translating it to actual left and top
            var offsetX = DesiredSizes[index].Width / 2;
            var offsetY = DesiredSizes[index].Height / 2;
            rect = new Rect(-offsetX, -offsetY, DesiredSizes[index].Width, DesiredSizes[index].Height);
            var translateX = left + offsetX;
            var translateY = top + offsetY;

            return AxisLabelLayout.GetRotatedPoints(angle, rect, translateX, translateY);
        }

        /// <summary>
        /// Binds the visiblilty of the axis labels with <see cref="TextBlock"/>.
        /// </summary>
        private void AxisLabelsVisibilityBinding()
        {
            foreach (FrameworkElement element in Children)
            {
                Binding binding = new Binding();
                binding.Source = Axis;
                binding.Path = new PropertyPath("Visibility");
                element.SetBinding(TextBlock.VisibilityProperty, binding);
            }
        }

        #endregion

        internal void CalculateWrapLabelRect()
        {
            bool isLabelWrap = true;
            int previousIndex = 0;
            int i = 1;
            List<Dictionary<int, Rect>> labelRects = RectssByRowsAndCols;
            ObservableCollection<ChartAxisLabel> labels = Axis.VisibleLabels;
            var length = labels.Count;

            for (; i < length; i++)
            {
                var previousRect = labelRects[0][previousIndex];
                var currentRect = labelRects[0][i];

                if (IntersectsWith(previousRect, currentRect, previousIndex, i))
                {
                    var prevWrapWidth = (currentRect.Left - previousRect.Left) - (Margin.Left + Margin.Right);
                    isLabelWrap = LabelContainWrapWidth(labels[previousIndex].LabelContent.ToString(), prevWrapWidth);

                    if (isLabelWrap || (isLabelWrap && i == length - 1))
                    {
                        Size availableSize = new Size(prevWrapWidth, double.MaxValue);
                        (Children[previousIndex] as UIElement).Measure(availableSize);
                        ComputedSizes[previousIndex] = (Children[previousIndex] as UIElement).DesiredSize;

                        RectssByRowsAndCols[0].Remove(previousIndex);
                        RectssByRowsAndCols[0].Add(previousIndex, new Rect(previousRect.X, previousRect.Y, ComputedSizes[previousIndex].Width, ComputedSizes[previousIndex].Height));

                        if (i == length - 1)
                        {
                            var x = currentRect.Left + (previousRect.Right - currentRect.Left);
                            var wrapWidth = (currentRect.Right - x) - (Margin.Left + Margin.Right);

                            isLabelWrap = LabelContainWrapWidth(labels[i].LabelContent.ToString(), wrapWidth);

                            if (isLabelWrap)
                            {
                                Size wrapSize = new Size(wrapWidth, double.MaxValue);
                                (Children[i] as UIElement).Measure(wrapSize);
                                ComputedSizes[i] = (Children[i] as UIElement).DesiredSize;

                                RectssByRowsAndCols[0].Remove(i);
                                RectssByRowsAndCols[0].Add(i, new Rect(currentRect.X, currentRect.Y, ComputedSizes[i].Width, ComputedSizes[i].Height));
                            }                       
                        }
                    }
                }

                previousIndex = i;
            }
        }

        private static bool LabelContainWrapWidth(string label, double wrapWidth)
        {
            string[] words = label.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                TextBlock text = new TextBlock() { Text = words[i] };
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                var labelSize = text.DesiredSize;

                if (labelSize.Width > wrapWidth)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }

    /// <summary>
    /// Represents a axis layout in chart control that indicates the layout orientation as horizontal. 
    /// </summary>
    /// <seealso cref="Syncfusion.UI.Xaml.Charts.AxisLabelLayout" />
    public partial class HorizontalLabelLayout : AxisLabelLayout
    {
        #region Fields

        private int currentPos;
        private Border currentBorder;
        private bool isOpposed;
        private double maxHeight;
        private double axisLineThickness;
        private double prevEnd = 0;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalLabelLayout"/> class.
        /// </summary>
        /// <param name="axis">The Axis</param>
        /// <param name="elements">The Elements</param>
        public HorizontalLabelLayout(ChartAxis axis, List<UIElement> elements) : base(axis, elements)
        {
        }

        #endregion
        
        #region Methods

        #region Public Methods

        /// <summary>
        /// Measures the labels in the <see cref="HorizontalLabelLayout"/>.
        /// </summary>
        /// <param name="availableSize">The Available Size</param>
        /// <returns>Returns the size required to arrange the elements</returns>
        public override Size Measure(Size availableSize)
        {
            double desiredHeight = 0;

            if (Axis != null && Children.Count > 0)
            {
                AvailableSize = availableSize;
                base.Measure(availableSize);
                CalcBounds(availableSize.Width - Axis.ActualPlotOffset * 2);
                if (Axis.LabelsIntersectAction == AxisLabelsIntersectAction.Auto
                    && Axis.LabelRotationAngle == 0 && AngleForAutoRotate != 90)
                {
                    int prevIndex = 0;
                    double angle = 0;

                    for (int i = 1; i < Children.Count; i++)
                    {
                        if (IntersectsWith(RectssByRowsAndCols[0][prevIndex], RectssByRowsAndCols[0][i], prevIndex, i))
                        {
                            angle = AngleForAutoRotate == 45 ? 90 : 45;
                        }
                        else
                        {
                            prevIndex = i;
                        }
                    }

                    if (angle != 0)
                    {
                        AngleForAutoRotate = angle;
                        Measure(availableSize);
                    }
                }

                var axis = Axis as ChartAxisBase2D;
                desiredHeight = LayoutElements();
                if (axis != null)
                {
                    if ((axis.ShowLabelBorder && axis.LabelBorderWidth > 0) ||
                    (axis.MultiLevelLabels != null && axis.MultiLevelLabels.Count > 0))
                        desiredHeight += BorderPadding;
                    if (axis.MultiLevelLabels != null && axis.MultiLevelLabels.Count == 0)
                        desiredHeight = Math.Max(desiredHeight, Axis.LabelExtent);
                }
                else
                    desiredHeight = Math.Max(desiredHeight, Axis.LabelExtent);

                desiredHeight += ((Margin.Top + Margin.Bottom) * RectssByRowsAndCols.Count);
                return new Size(availableSize.Width, desiredHeight);
            }

            return new Size(availableSize.Width, 0);
        }

        /// <summary>
        /// Arranges the labels in the <see cref="HorizontalLabelLayout"/>
        /// </summary>
        /// <param name="finalSize"></param>
        public override void Arrange(Size finalSize)
        {
            if (RectssByRowsAndCols == null)
                return;

            isOpposed = false;
            bool needToRotate = (!double.IsNaN(Axis.LabelRotationAngle) && Axis.LabelRotationAngle != 0.0) || AngleForAutoRotate != 0;
            axisLineThickness = (this.Axis.axisElementsPanel as ChartCartesianAxisElementsPanel).MainAxisLine.StrokeThickness;

            if (Axis.Area is SfChart3D)
            {
                isOpposed = AxisLabelLayout.IsOpposed(Axis, Axis.Area.Axes.Where(x => x.Orientation == Orientation.Horizontal && x.OpposedPosition).Any());
                var top = isOpposed && Axis.LabelsPosition == AxisElementPosition.Outside ? Axis.ArrangeRect.Bottom : Axis.ArrangeRect.Top + Top;
                var left = Axis.ArrangeRect.Left;
                var graphics3D = (Axis.Area as SfChart3D).Graphics3D;
                foreach (var dictionary in RectssByRowsAndCols)
                {
                    foreach (var keyValue in dictionary)
                    {
                        var axis = Axis as ChartAxisBase3D;

                        var element = Children[keyValue.Key];
                        var actualTop = isOpposed ? top - keyValue.Value.Bottom - DesiredSizes[keyValue.Key].Height : top;
                        var actualLeft = axis.IsManhattanAxis ? keyValue.Value.Left : keyValue.Value.Left + Axis.ActualPlotOffset;
                        var area = Axis.Area as SfChart3D;

                        var actualRotationAngle = area.ActualRotationAngle;
                        var actualtTiltAngle = area.ActualTiltAngle;
                        var depth = axis.AxisDepth;

                        actualLeft += element.DesiredSize.Width / 2;

                        if (needToRotate)
                        {
                            actualTop += (ComputedSizes[keyValue.Key].Height - DesiredSizes[keyValue.Key].Height) / 2;
                            actualLeft += (ComputedSizes[keyValue.Key].Width - DesiredSizes[keyValue.Key].Width) / 2;
                        }

                        UIElementLeftShift leftElementShift = UIElementLeftShift.Default;
                        UIElementTopShift topElementShift = UIElementTopShift.Default;

                        GetLeftandShift(ref leftElementShift, ref topElementShift, isOpposed, actualRotationAngle, actualtTiltAngle);

                        var leftSpacing = 0d;
                        //To prevent the addition space in the zaxis labels placement.

                        var verticalOpposed = this.Axis.Area.Axes.Any(x => x.Orientation == Orientation.Vertical && x.OpposedPosition);

                        if ((axis.IsZAxis))
                        {
                            if (verticalOpposed)
                                leftSpacing -= axis.TickLineSize / 2;
                            else
                            {
                                if (actualRotationAngle >= 0 && actualRotationAngle < 45
                                    || actualRotationAngle >= 135 && actualRotationAngle < 180)
                                    leftSpacing += axis.TickLineSize / 2;
                                else if (actualRotationAngle >= 180 && actualRotationAngle < 225
                                    || actualRotationAngle >= 315 && actualRotationAngle < 360)
                                    leftSpacing -= axis.TickLineSize / 2;
                            }
                        }
                        else
                        {
                            actualLeft += left;

                            if (verticalOpposed)
                                depth -= axis.TickLineSize / 2;
                            else
                            {
                                if (actualtTiltAngle >= 45 && actualtTiltAngle < 315)
                                {
                                    if (actualRotationAngle >= 45 && actualRotationAngle < 90
                                        || actualRotationAngle >= 270 && actualRotationAngle < 315)
                                        depth -= axis.TickLineSize / 2;
                                    else if (actualRotationAngle >= 90 && actualRotationAngle < 135
                                        || actualRotationAngle >= 225 && actualRotationAngle < 270)
                                        depth += axis.TickLineSize / 2;
                                }
                            }
                        }

                        if (axis.IsZAxis)
                            graphics3D.AddVisual(Polygon3D.CreateUIElement(new Vector3D(left + leftSpacing, actualTop, actualLeft), element, 10, 10, false, leftElementShift, topElementShift));
                        else
                            graphics3D.AddVisual(Polygon3D.CreateUIElement(new Vector3D(actualLeft, actualTop, depth), element, 10, 10, true, leftElementShift, topElementShift));
                    }

                    if (isOpposed)
                    {
                        top -= (dictionary.Values.Max(rect => rect.Height) + Margin.Bottom);
                    }
                    else
                    {
                        top += (dictionary.Values.Max(rect => rect.Height) + Margin.Top);
                    }
                }
            }
            else
            {
                int row = 0;
                double top = 0;
                var axis = Axis as ChartAxisBase2D;
                isOpposed = AxisLabelLayout.IsOpposed(Axis, Axis.OpposedPosition);
                top = isOpposed ? finalSize.Height - Margin.Bottom : Margin.Top;
                if (axis != null && axis.LabelBorderWidth > 0)
                {
                    maxHeight = RectssByRowsAndCols.Select(dictionary =>
                     dictionary.Values.Max(rect => rect.Height)).FirstOrDefault();
                    maxHeight = RectssByRowsAndCols.Count > 1 ? maxHeight + Margin.Top :
                      maxHeight + BorderPadding;
                }

                foreach (Dictionary<int, Rect> dictionary in RectssByRowsAndCols)
                {
                    foreach (KeyValuePair<int, Rect> keyValue in dictionary)
                    {
                        UIElement element = Children[keyValue.Key];
                        double actualTop = isOpposed ? top - ComputedSizes[keyValue.Key].Height : top;

                        //Positioning the labels according to the axis line stroke thickness.
                        if (Axis.TickLinesPosition == AxisElementPosition.Inside)
                            actualTop += Axis.OpposedPosition ? -axisLineThickness : axisLineThickness;

                        double actualLeft = keyValue.Value.Left + Axis.ActualPlotOffset;

                        if (needToRotate)
                        {
                            actualTop += (ComputedSizes[keyValue.Key].Height - DesiredSizes[keyValue.Key].Height) / 2;
                            actualLeft += (ComputedSizes[keyValue.Key].Width - DesiredSizes[keyValue.Key].Width) / 2;
                        }

                        Canvas.SetLeft(element, actualLeft);
                        Canvas.SetTop(element, actualTop);

                        // Draw the Border for AxisLabels
                        if (axis != null && axis.ShowLabelBorder && axis.LabelBorderWidth > 0)
                        {
                            double tickSize = (axis is CategoryAxis || axis is DateTimeCategoryAxis) ? 5 :
                            (axis as RangeAxisBase).SmallTickLineSize;
                            tickSize = Math.Max(tickSize, axis.TickLineSize);
                            currentBorder = Borders[keyValue.Key];
                            SetBorderThickness(row, dictionary, axis);
                            SetBorderPosition(dictionary, row, axis, tickSize);
                            SetBorderTop(row, top, tickSize);
                            currentPos++;
                        }
                    }
                    if (isOpposed)
                        top -= (dictionary.Values.Max(rect => rect.Height) + Margin.Bottom);
                    else
                        top += (dictionary.Values.Max(rect => rect.Height) + Margin.Top);
                    currentPos = 0;

                    row++;
                }
                prevEnd = 0;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Calculates the actual plot offset.
        /// </summary>
        /// <param name="availableSize">The Available Size</param>
        protected override void CalculateActualPlotOffset(Size availableSize)
        {
            if (Axis.EdgeLabelsDrawingMode == EdgeLabelsDrawingMode.Fit)
            {
                double coeff = Axis.ValueToCoefficientCalc(Axis.VisibleLabels[0].Position);
                double position = (coeff * availableSize.Width) - (ComputedSizes[0].Width / 2);
                double firstElementWidth = 0;
                double lastElementWidth = 0;

                if ((position - ComputedSizes[0].Width / 2) + Axis.PlotOffset < 0)
                {
                    firstElementWidth = ComputedSizes[0].Width;
                }

                int index = Children.Count - 1;
                if ((position + ComputedSizes[index].Width / 2) - Axis.PlotOffset < availableSize.Width)
                {
                    lastElementWidth = ComputedSizes[index].Width;
                }

                double offset = Math.Max(firstElementWidth / 2, lastElementWidth / 2);
                Axis.ActualPlotOffset = Math.Max(offset, Axis.PlotOffset);
            }
            else
            {
                base.CalculateActualPlotOffset(availableSize);
            }
        }

        /// <summary>
        /// Layouts the elements
        /// </summary>
        /// <returns>Returns the desired height.</returns>
        protected override double LayoutElements()
        {
            if (Axis.LabelsIntersectAction == AxisLabelsIntersectAction.Wrap)
            {
                CalculateWrapLabelRect();
                CalcBounds(AvailableSize.Width - Axis.ActualPlotOffset * 2);
            }

            base.LayoutElements();

            return RectssByRowsAndCols.Sum(dictionary => dictionary.Values.Max(rect => rect.Height));
        }

        /// <summary>
        /// Calculates the bounds.
        /// </summary>
        /// <param name="availableWidth">The Available Width</param>
        protected override void CalcBounds(double availableWidth)
        {
            RectssByRowsAndCols = new List<Dictionary<int, Rect>>();
            RectssByRowsAndCols.Add(new Dictionary<int, Rect>());

            for (int j = 0; j < Children.Count; j++)
            {
                double position = 0d;

                var axis = Axis as ChartAxisBase3D;
                var linearAxis = Axis as NumericalAxis;
                if (axis != null && axis.IsManhattanAxis && (axis.RegisteredSeries[j] as ChartSeries3D).Segments != null && (axis.RegisteredSeries[j] as ChartSeries3D).Segments.Count > 0)
                {
                    var segment = (axis.RegisteredSeries[j] as ChartSeries3D).Segments[0] as ChartSegment3D;
                    position = (segment.startDepth + (segment.endDepth - segment.startDepth) / 2) - (ComputedSizes[j].Width / 2);
                }
                else if (linearAxis != null && linearAxis.BreakExistence())
                {
                    for (int i = 0; i < linearAxis.AxisRanges.Count; i++)
                    {
                        if (!linearAxis.AxisRanges[i].Inside(Axis.VisibleLabels[j].Position)) continue;
                        double coeff = Axis.ValueToCoefficientCalc(Axis.VisibleLabels[j].Position);
                        position = (coeff * availableWidth) - (ComputedSizes[j].Width / 2);

                        foreach (DoubleRange range in linearAxis.BreakRanges)
                        {
                            if (Math.Round(range.Start, 6) == Convert.ToDouble((Axis.VisibleLabels[j].LabelContent)))
                            {
                                position = linearAxis.IsInversed ? position + (ComputedSizes[j].Width / 2) :
                                    position - (ComputedSizes[j].Width / 2);
                            }
                            else if (Math.Round(range.End, 6) == Convert.ToDouble((Axis.VisibleLabels[j].LabelContent)))
                            {
                                position = linearAxis.IsInversed ? position - (ComputedSizes[j].Width / 2) :
                                    position + (ComputedSizes[j].Width / 2);
                            }
                        }
                    }
                }
                else
                {
                    double coeff = Axis.ValueToCoefficientCalc(Axis.VisibleLabels[j].Position);
                    position = (coeff * availableWidth) - (ComputedSizes[j].Width / 2);
                }

                var labelAlignment = Axis.AxisLabelAlignment;
                if (Axis.VisibleLabels[j].LabelStyle != null)
                {
                    labelAlignment = Axis.VisibleLabels[j].AxisLabelAlignment;
                }
                if (labelAlignment == LabelAlignment.Far)
                {
                    position += ComputedSizes[j].Width / 2;
                }
                else if (labelAlignment == LabelAlignment.Near)
                {
                    position -= ComputedSizes[j].Width / 2;
                }

                RectssByRowsAndCols[0].Add(j, new Rect(new Point(position, 0), ComputedSizes[j]));
            }

            if (Axis.EdgeLabelsDrawingMode == EdgeLabelsDrawingMode.Shift)
            {
                if (RectssByRowsAndCols[0][0].Left < 0)
                {
                    RectssByRowsAndCols[0][0] = new Rect(0, 0, ComputedSizes[0].Width, ComputedSizes[0].Height);
                }
                int index = Children.Count - 1;
                if (RectssByRowsAndCols[0][index].Right > availableWidth)
                {
                    double position = availableWidth - ComputedSizes[Children.Count - 1].Width;
                    RectssByRowsAndCols[0][index] = new Rect(position, 0, ComputedSizes[index].Width,
                                                             ComputedSizes[index].Height);
                }
            }
            else if (Axis.EdgeLabelsDrawingMode == EdgeLabelsDrawingMode.Hide)
            {
                if (RectssByRowsAndCols[0][0].Left < 0)
                {
                    RectssByRowsAndCols[0][0] = new Rect(0, 0, 0, 0);
                    Children[0].Visibility = Visibility.Collapsed;
                }
                int index = Children.Count - 1;
                if (RectssByRowsAndCols[0][index].Right > availableWidth)
                {
                    RectssByRowsAndCols[0][index] = new Rect(0, 0, 0, 0);
                    Children[index].Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Positions the labels back.
        /// </summary>
        /// <param name="isOpposed">Is Oppoosed Indication</param>
        /// <param name="leftElementShift">The Left Element Shift.</param>
        /// <param name="topElementShift">The Top Element Shift</param>
        /// <param name="actualTiltAngle">The Actual Tilt Angle</param>
        private static void PositionLabelsBack(bool isOpposed, ref UIElementLeftShift leftElementShift, ref UIElementTopShift topElementShift, double actualTiltAngle)
        {
            leftElementShift = UIElementLeftShift.LeftHalfShift;

            if (isOpposed)
            {
                if (actualTiltAngle >= 45 && actualTiltAngle < 315)
                    topElementShift = UIElementTopShift.Default;
            }
            else
            {
                if (actualTiltAngle >= 45 && actualTiltAngle < 315)
                    topElementShift = UIElementTopShift.TopShift;
            }
        }

        /// <summary>
        /// Positions the label right.
        /// </summary>
        /// <param name="isOpposed">Is Opposed Indication</param>
        /// <param name="leftElementShift">The Left Element Shift</param>
        /// <param name="topElementShift">The Top Element Shift</param>
        /// <param name="actualTiltAngle">The Actaul Tilt Angle</param>
        private static void PositionLabelsRight(bool isOpposed, ref UIElementLeftShift leftElementShift, ref UIElementTopShift topElementShift, double actualTiltAngle)
        {
            leftElementShift = UIElementLeftShift.Default;

            if (actualTiltAngle >= 45 && actualTiltAngle < 315)
                topElementShift = UIElementTopShift.TopHalfShift;
        }

        /// <summary>
        /// Positions the label left.
        /// </summary>
        /// <param name="isOpposed">Is Opposed Indication</param>
        /// <param name="leftElementShift">The Left Element Shift</param>
        /// <param name="topElementShift">The Top Element Shift</param>
        /// <param name="actualTiltAngle">The Actual Tilt Angle</param>
        private static void PositionLabelsLeft(bool isOpposed, ref UIElementLeftShift leftElementShift, ref UIElementTopShift topElementShift, double actualTiltAngle)
        {
            leftElementShift = UIElementLeftShift.LeftShift;

            if (actualTiltAngle >= 45 && actualTiltAngle < 315)
                topElementShift = UIElementTopShift.TopHalfShift;
        }

        /// <summary>
        /// Positions the label front.
        /// </summary>
        /// <param name="isOpposed">Is Opposed Indication</param>
        /// <param name="leftElementShift">The Left Element Shift </param>
        /// <param name="topElementShift">The Top Element Shift</param>
        /// <param name="actualTiltAngle">The Actual Tilt Angle</param>
        private static void PositionLabelsFront(bool isOpposed, ref UIElementLeftShift leftElementShift, ref UIElementTopShift topElementShift, double actualTiltAngle)
        {
            leftElementShift = UIElementLeftShift.LeftHalfShift;

            if (isOpposed)
            {
                if (actualTiltAngle >= 45 && actualTiltAngle < 315)
                    topElementShift = UIElementTopShift.TopShift;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calcuales the point.
        /// </summary>
        /// <param name="value">The Value</param>
        /// <returns>Returns the calculated point.</returns>
        private double CalculatePoint(double value)
        {
            return Axis.ActualPlotOffset +
                        Math.Round(Axis.RenderedRect.Width *
                        Axis.ValueToCoefficient(value));
        }
        
        /// <summary>
        /// To place the label border when the label placement is OnTicks
        /// </summary>
        /// <param name="dictionary">The Dictionary</param>
        /// <param name="row">The Row</param>
        /// <param name="axis">The Axis</param>
        /// <param name="tickSize">The Tick Size</param>
        private void SetBorderPosition(Dictionary<int, Rect> dictionary, int row,
            ChartAxisBase2D axis, double tickSize)
        {
            if (currentPos == 0)
            {
                prevEnd = (Axis.IsInversed ? CalculatePoint(Axis.VisibleRange.End) :
                 CalculatePoint(Axis.VisibleRange.Start));
            }

            if (currentPos + 1 < dictionary.Count)
            {
                KeyValuePair<int, Rect> key1 = dictionary.ElementAt(currentPos);
                KeyValuePair<int, Rect> key2 = dictionary.ElementAt(currentPos + 1);
                var value = (Axis.VisibleLabels[key1.Key].Position +
                    Axis.VisibleLabels[key2.Key].Position) / 2;
                double value1 = CalculatePoint(value);
                currentBorder.Width = value1 - prevEnd;
                currentBorder.Width = currentBorder.Width + axis.LabelBorderWidth;
                Canvas.SetLeft(currentBorder, prevEnd - axis.LabelBorderWidth / 2);
                prevEnd = value1;
            }
            else
            {
                currentBorder.Width = (Axis.IsInversed ? CalculatePoint(Axis.VisibleRange.Start)
                    : CalculatePoint(Axis.VisibleRange.End));
                currentBorder.Width -= prevEnd;
                currentBorder.Width += (axis.LabelBorderWidth);
                Canvas.SetLeft(currentBorder, prevEnd - axis.LabelBorderWidth / 2);
            }

            if (Axis.LabelsPosition == Axis.TickLinesPosition)
                currentBorder.Height = row == 0 ? maxHeight + tickSize + Margin.Top + Margin.Bottom :
                    maxHeight + Margin.Top + Margin.Bottom;
            else
                currentBorder.Height = maxHeight + Margin.Top + Margin.Bottom;
        }

        /// <summary>
        /// To set the label border thickness.
        /// </summary>
        /// <param name="row">The Row</param>
        /// <param name="dictionary">The Dictionary</param>
        /// <param name="axis">The Axis</param>
        private void SetBorderThickness(int row, Dictionary<int, Rect> dictionary,
            ChartAxisBase2D axis)
        {
            bool isSidebySideSeries = CheckCartesianSeries();
            var thickness = axis.LabelBorderWidth;
            if (CheckLabelPlacement(isSidebySideSeries))
                currentBorder.BorderThickness = isOpposed ?
                 new Thickness(thickness, thickness, thickness, 0) :
                 new Thickness(thickness, 0, thickness, thickness);
            else
            {
                if (currentPos == 0)
                    currentBorder.BorderThickness = isOpposed ?
                       new Thickness(0, thickness, thickness, 0) :
                       new Thickness(0, 0, thickness, thickness);
                else if (currentPos + 1 < dictionary.Count)
                    currentBorder.BorderThickness = isOpposed ?
                        new Thickness(thickness, thickness, thickness, 0) :
                        new Thickness(thickness, 0, thickness, thickness);
                else
                    currentBorder.BorderThickness = isOpposed ?
                     new Thickness(thickness, thickness, 0, 0) :
                     new Thickness(thickness, 0, 0, thickness);
            }
        }

        /// <summary>
        /// To position the label border on its top value
        /// </summary>
        /// <param name="row">The Row</param>
        /// <param name="top">The Top</param>
        /// <param name="tickSize">The Tick Size</param>
        private void SetBorderTop(int row, double top, double tickSize)
        {
            double topValue;
            if (RectssByRowsAndCols.Count > 1 && row > 0)
                if (isOpposed)
                    topValue = (top - currentBorder.Height);
                else
                    topValue = top;
            else
            {
                if (isOpposed)
                    topValue = Axis.LabelsPosition != Axis.TickLinesPosition ?
                           (top - currentBorder.Height) + Margin.Bottom :
                           (top - currentBorder.Height) + Margin.Bottom + tickSize;
                else
                    topValue = Axis.LabelsPosition != Axis.TickLinesPosition ?
                          top - (Margin.Top) :
                         top - (tickSize + Margin.Top);
            }

            Canvas.SetTop(currentBorder, topValue);
        }

        /// <summary>
        /// Shifts the labels according to the shifts.
        /// </summary>
        /// <param name="leftElementShift">The Left Element Shift</param>
        /// <param name="topElementShift">The Top Element Shift</param>
        /// <param name="isOpposed">The Opposed Check</param>
        /// <param name="actualRotationAngle">The Actual Rotation Angle</param>
        /// <param name="actualTiltAngle">The Actual Tilt Angle</param>
        private void GetLeftandShift(ref UIElementLeftShift leftElementShift, ref UIElementTopShift topElementShift, bool isOpposed, double actualRotationAngle, double actualTiltAngle)
        {
            var verticalOpposed = this.Axis.Area.Axes.Any(x => x.Orientation == Orientation.Vertical && x.OpposedPosition);

            if ((this.Axis as ChartAxisBase3D).IsZAxis)
            {
                if (verticalOpposed)
                {
                    if ((actualRotationAngle < 45 || actualRotationAngle >= 315))
                        HorizontalLabelLayout.PositionLabelsLeft(isOpposed, ref leftElementShift, ref topElementShift, actualTiltAngle);
                    else if ((actualRotationAngle >= 45 && actualRotationAngle < 135))
                        HorizontalLabelLayout.PositionLabelsBack(isOpposed, ref leftElementShift, ref topElementShift, actualTiltAngle);
                    else if ((actualRotationAngle >= 135 && actualRotationAngle < 225))
                        HorizontalLabelLayout.PositionLabelsRight(isOpposed, ref leftElementShift, ref topElementShift, actualTiltAngle);
                    else if ((actualRotationAngle >= 225 && actualRotationAngle < 315))
                        HorizontalLabelLayout.PositionLabelsFront(isOpposed, ref leftElementShift, ref topElementShift, actualTiltAngle);
                }
                else
                {
                    if ((actualRotationAngle >= 0 && actualRotationAngle < 45) || (actualRotationAngle >= 180 && actualRotationAngle < 225))
                        HorizontalLabelLayout.PositionLabelsRight(isOpposed, ref leftElementShift, ref topElementShift, actualTiltAngle);
                    else if ((actualRotationAngle >= 45 && actualRotationAngle < 135) || (actualRotationAngle >= 225 && actualRotationAngle < 315))
                        HorizontalLabelLayout.PositionLabelsFront(isOpposed, ref leftElementShift, ref topElementShift, actualTiltAngle);
                    else if (actualRotationAngle >= 135 && actualRotationAngle < 180 || actualRotationAngle >= 315 && actualRotationAngle < 360)
                        HorizontalLabelLayout.PositionLabelsLeft(isOpposed, ref leftElementShift, ref topElementShift, actualTiltAngle);
                }
            }
            else
            {
                if (verticalOpposed)
                {
                    if ((actualRotationAngle < 45 || actualRotationAngle >= 315))
                        HorizontalLabelLayout.PositionLabelsFront(isOpposed, ref leftElementShift, ref topElementShift, actualTiltAngle);
                    else if ((actualRotationAngle >= 45 && actualRotationAngle < 135))
                        HorizontalLabelLayout.PositionLabelsLeft(isOpposed, ref leftElementShift, ref topElementShift, actualTiltAngle);
                    else if ((actualRotationAngle >= 135 && actualRotationAngle < 225))
                        HorizontalLabelLayout.PositionLabelsBack(isOpposed, ref leftElementShift, ref topElementShift, actualTiltAngle);
                    else if ((actualRotationAngle >= 225 && actualRotationAngle < 315))
                        HorizontalLabelLayout.PositionLabelsRight(isOpposed, ref leftElementShift, ref topElementShift, actualTiltAngle);
                }
                else
                {
                    if ((actualRotationAngle < 45 || actualRotationAngle >= 315) || (actualRotationAngle >= 135 && actualRotationAngle < 225))
                        HorizontalLabelLayout.PositionLabelsFront(isOpposed, ref leftElementShift, ref topElementShift, actualTiltAngle);
                    else if ((actualRotationAngle >= 45 && actualRotationAngle < 90) || (actualRotationAngle >= 225 && actualRotationAngle < 270))
                        HorizontalLabelLayout.PositionLabelsLeft(isOpposed, ref leftElementShift, ref topElementShift, actualTiltAngle);
                    else if ((actualRotationAngle >= 90 && actualRotationAngle < 135) || (actualRotationAngle >= 270 && actualRotationAngle < 315))
                        HorizontalLabelLayout.PositionLabelsRight(isOpposed, ref leftElementShift, ref topElementShift, actualTiltAngle);
                }
            }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Represents the <see cref="VerticalLabelLayout"/> class.
    /// </summary>
    public partial class VerticalLabelLayout : AxisLabelLayout
    {
        #region Fields

        private int currentPos;
        private Border currentBorder;
        private bool isOpposed;
        private double maxWidth;
        private double axisLineThickness;
        private double prevEnd;

        #endregion
        
        #region Constructor

        public VerticalLabelLayout(ChartAxis axis, List<UIElement> elements) : base(axis, elements)
        {
        }

        #endregion
        
        #region Methods

        #region Public Methods

        /// <summary>
        /// Method declaration for Measure.
        /// </summary>
        /// <param name="availableSize">The Available Size</param>
        /// <returns>Returns the desired height</returns>
        public override Size Measure(Size availableSize)
        {
            double desiredWidth = 0;
            if (Axis != null && Children.Count > 0)
            {
                AvailableSize = availableSize;
                base.Measure(availableSize);
                CalcBounds(availableSize.Height - Axis.ActualPlotOffset * 2);
                var axis = (Axis as ChartAxisBase2D);
                desiredWidth = LayoutElements();
                if (axis != null)
                {
                    if ((axis.MultiLevelLabels != null && axis.MultiLevelLabels.Count > 0)
                         || (axis.ShowLabelBorder && axis.LabelBorderWidth > 0))
                        desiredWidth += BorderPadding;
                    if (axis.MultiLevelLabels != null && axis.MultiLevelLabels.Count == 0)
                        desiredWidth = Math.Max(desiredWidth, Axis.LabelExtent);
                }
                else
                    desiredWidth = Math.Max(desiredWidth, Axis.LabelExtent);
                desiredWidth += ((Margin.Left + Margin.Right) * RectssByRowsAndCols.Count);
                return new Size(desiredWidth, availableSize.Height);
            }
            return new Size(0, availableSize.Height);
        }

        /// <summary>
        /// Method declaration for Arrange.
        /// </summary>
        /// <param name="finalSize">The Final Size.</param>
        public override void Arrange(Size finalSize)
        {
            if (RectssByRowsAndCols == null)
                return;

            isOpposed = AxisLabelLayout.IsOpposed(Axis, Axis.OpposedPosition);
            bool needToRotate = !double.IsNaN(Axis.LabelRotationAngle) && Axis.LabelRotationAngle != 0.0;
            double left = isOpposed ? Margin.Left : finalSize.Width - Margin.Right;
            axisLineThickness = (this.Axis.axisElementsPanel as ChartCartesianAxisElementsPanel).MainAxisLine.StrokeThickness;

            if (Axis.Area is SfChart3D)
            {
                var axis = Axis as ChartAxisBase3D;
                var g3 = (Axis.Area as SfChart3D).Graphics3D;
                foreach (var dictionary in RectssByRowsAndCols)
                {
                    foreach (var keyValue in dictionary)
                    {
                        var elemSize = Size.Empty;
                        var element = Children[keyValue.Key];
                        var frameworkElement = element as FrameworkElement;
                        if (element is TextBlock)
                        {
                            elemSize = new Size(frameworkElement.ActualWidth, frameworkElement.ActualHeight);
                        }
                        else
                        {
                            elemSize = frameworkElement.DesiredSize;
                        }

                        double depth = 0d;
                        double actualLeft = 0d;
                        double actualTop = 0d;
                        var leftElementType = UIElementLeftShift.Default;
                        actualTop = keyValue.Value.Top + Axis.ActualPlotOffset + Axis.ArrangeRect.Top;
                        var topElementType = UIElementTopShift.TopHalfShift;

                        actualTop += DesiredSizes[keyValue.Key].Height / 2;
                        if (needToRotate)
                            actualTop += (elemSize.Height - DesiredSizes[keyValue.Key].Height) / 2;

                        if (axis.ShowAxisNextToOrigin)
                        {
                            var area = Axis.Area as SfChart3D;
                            double actualRotationAngle = area.ActualRotationAngle;

                            bool orgOpposed = axis.OpposedPosition || (actualRotationAngle >= 180 && actualRotationAngle < 360);
                            orgOpposed = AxisLabelLayout.IsOpposed(axis, orgOpposed);
                            double orgLeft = orgOpposed ? Margin.Left : finalSize.Width - Margin.Right;

                            leftElementType = UIElementLeftShift.LeftHalfShift;
                            topElementType = UIElementTopShift.TopHalfShift;

                            if (actualRotationAngle >= 90 && actualRotationAngle < 270)
                                depth = area.ActualDepth;

                            actualLeft += orgOpposed ? (orgLeft + elemSize.Width / 2) : (orgLeft - elemSize.Width / 2);
                            actualLeft += Axis.ArrangeRect.Left + Left;

                            if (needToRotate)
                                actualLeft += (elemSize.Width - DesiredSizes[keyValue.Key].Width) / 2;

                            g3.AddVisual(Polygon3D.CreateUIElement(new Vector3D(actualLeft, actualTop, depth), element, 10, 10, true, leftElementType, topElementType));
                        }
                        else
                        {
                            UIElement3D uiElement3D = null;
                            switch (axis.AxisPosition3D)
                            {
                                case AxisPosition3D.FrontRight:

                                    // Opposed position is not checked since the FrontRight enum is set automatically when opposed position is set.
                                    if (axis.LabelsPosition == AxisElementPosition.Inside)
                                    {
                                        leftElementType = UIElementLeftShift.LeftShift;
                                        actualLeft += (finalSize.Width - Margin.Left);
                                    }
                                    else
                                        actualLeft += Margin.Left;
                                    actualLeft += Axis.ArrangeRect.Left + Left;
                                    if (needToRotate)
                                        actualLeft += (elemSize.Width - DesiredSizes[keyValue.Key].Width) / 2;
                                    uiElement3D = Polygon3D.CreateUIElement(new Vector3D(actualLeft, actualTop, axis.AxisDepth), element, 10, 10, true, leftElementType, topElementType);
                                    break;

                                case AxisPosition3D.BackRight:
                                    if (isOpposed)
                                        actualLeft += (finalSize.Width - Margin.Left);
                                    else
                                    {
                                        leftElementType = UIElementLeftShift.LeftShift;
                                        actualLeft += Margin.Left;
                                    }
                                    actualLeft += Axis.ArrangeRect.Left + Left;
                                    if (needToRotate)
                                        actualLeft += (elemSize.Width - DesiredSizes[keyValue.Key].Width) / 2;
                                    uiElement3D = Polygon3D.CreateUIElement(new Vector3D(actualLeft, actualTop, axis.AxisDepth), element, 10, 10, true, leftElementType, topElementType);
                                    break;

                                case AxisPosition3D.BackLeft:
                                    if (isOpposed)
                                    {
                                        leftElementType = UIElementLeftShift.LeftShift;
                                        actualLeft += left;
                                    }
                                    else
                                    {
                                        actualLeft += DesiredSizes[keyValue.Key].Width;
                                        actualLeft += (finalSize.Width - elemSize.Width - Margin.Left);
                                    }
                                    actualLeft += Axis.ArrangeRect.Left + Left;
                                    if (needToRotate)
                                        actualLeft += (elemSize.Width - DesiredSizes[keyValue.Key].Width) / 2;
                                    uiElement3D = Polygon3D.CreateUIElement(new Vector3D(actualLeft, actualTop, axis.AxisDepth), element, 10, 10, true, leftElementType, topElementType);
                                    break;

                                case AxisPosition3D.DepthBackRight:
                                    if (isOpposed)
                                    {
                                        leftElementType = UIElementLeftShift.LeftShift;
                                        depth += (axis.AxisDepth + Left + finalSize.Width - Margin.Left);
                                    }
                                    else
                                        depth += axis.AxisDepth + Left + Margin.Left;

                                    actualLeft += this.Axis.ArrangeRect.Left;
                                    if (needToRotate)
                                        depth += (elemSize.Width - DesiredSizes[keyValue.Key].Width) / 2;
                                    uiElement3D = Polygon3D.CreateUIElement(new Vector3D(actualLeft, actualTop, depth), element, 10, 10, false, leftElementType, topElementType);
                                    break;

                                case AxisPosition3D.DepthFrontLeft:

                                    if (isOpposed)
                                    {
                                        leftElementType = UIElementLeftShift.LeftShift;
                                        depth += (axis.AxisDepth + Left + Margin.Left);
                                    }
                                    else
                                        depth += (axis.AxisDepth + finalSize.Width + Left - Margin.Left);

                                    actualLeft += this.Axis.ArrangeRect.Left;
                                    if (needToRotate)
                                        depth += (elemSize.Width - DesiredSizes[keyValue.Key].Width) / 2;
                                    uiElement3D = Polygon3D.CreateUIElement(new Vector3D(actualLeft, actualTop, depth), element, 10, 10, false, leftElementType, topElementType);
                                    break;

                                case AxisPosition3D.DepthFrontRight:
                                    if (isOpposed)
                                    {
                                        leftElementType = UIElementLeftShift.RightHalfShift;
                                        depth += axis.AxisDepth + Margin.Left - elemSize.Width - finalSize.Width;
                                    }
                                    else
                                    {
                                        leftElementType = UIElementLeftShift.LeftShift;
                                        depth += (axis.AxisDepth - Left - Margin.Left);
                                    }
                                    actualLeft += axis.ArrangeRect.Left;
                                    if (needToRotate)
                                        depth += (elemSize.Width - DesiredSizes[keyValue.Key].Width) / 2;
                                    uiElement3D = Polygon3D.CreateUIElement(new Vector3D(actualLeft, actualTop, depth), element, 10, 10, false, leftElementType, topElementType);
                                    break;

                                case AxisPosition3D.DepthBackLeft:
                                    if (isOpposed)
                                    {
                                        depth += axis.AxisDepth - Left - Margin.Left;
                                    }
                                    else
                                    {
                                        leftElementType = UIElementLeftShift.LeftShift;
                                        depth = axis.AxisDepth - Left - left;
                                    }
                                    actualLeft += axis.ArrangeRect.Left;
                                    if (needToRotate)
                                        depth += (elemSize.Width - DesiredSizes[keyValue.Key].Width) / 2;
                                    uiElement3D = Polygon3D.CreateUIElement(new Vector3D(actualLeft, actualTop, depth), element, 10, 10, false, leftElementType, topElementType);
                                    break;

                                default:
                                    if (isOpposed)
                                        actualLeft += left;
                                    else
                                    {
                                        leftElementType = UIElementLeftShift.LeftShift;
                                        actualLeft += left;
                                    }

                                    actualLeft += Axis.ArrangeRect.Left + Left;
                                    if (needToRotate)
                                        actualLeft += (elemSize.Width - DesiredSizes[keyValue.Key].Width) / 2;
                                    uiElement3D = Polygon3D.CreateUIElement(new Vector3D(actualLeft, actualTop, axis.AxisDepth), element, 10, 10, true, leftElementType, topElementType);
                                    break;
                            }

                            g3.AddVisual(uiElement3D);
                        }
                    }

                    if (isOpposed)
                    {
                        left += (dictionary.Values.Max(rect => rect.Width) + Margin.Left);
                    }
                    else
                    {
                        left -= (dictionary.Values.Max(rect => rect.Width) + Margin.Right);
                    }
                }
            }
            else
            {
                int row = 0;

                var axis = Axis as ChartAxisBase2D;
                if (axis != null && axis.LabelBorderWidth > 0)
                {
                    maxWidth = RectssByRowsAndCols.Select(dictionary => dictionary.Values.Max(rect => rect.Width)).FirstOrDefault();
                    maxWidth = RectssByRowsAndCols.Count > 1 ? maxWidth : maxWidth + BorderPadding;
                }

                foreach (Dictionary<int, Rect> dictionary in RectssByRowsAndCols)
                {
                    foreach (KeyValuePair<int, Rect> keyValue in dictionary)
                    {
                        UIElement element = Children[keyValue.Key];

                        double actualLeft = isOpposed ? left : left - ComputedSizes[keyValue.Key].Width;

                        // Positioning the labels according to the axis line stroke thickness.
                        if (Axis.TickLinesPosition == AxisElementPosition.Inside)
                            actualLeft += Axis.OpposedPosition ? axisLineThickness : -axisLineThickness;

                        double actualTop = keyValue.Value.Top + Axis.ActualPlotOffset;

                        if (needToRotate)
                        {
                            actualLeft += (ComputedSizes[keyValue.Key].Width - DesiredSizes[keyValue.Key].Width) / 2;
                            actualTop += (ComputedSizes[keyValue.Key].Height - DesiredSizes[keyValue.Key].Height) / 2;
                        }

                        Canvas.SetLeft(element, actualLeft);
                        Canvas.SetTop(element, actualTop);

                        //To draw the border for axis label
                        if (axis != null && axis.ShowLabelBorder && axis.LabelBorderWidth > 0)
                        {
                            double tickSize = (axis is CategoryAxis || axis is DateTimeCategoryAxis) ? 5 :
                            (axis as RangeAxisBase).SmallTickLineSize;
                            tickSize = Math.Max(tickSize, axis.TickLineSize);
                            currentBorder = Borders[keyValue.Key];
                            SetBorderThickness(row, dictionary, axis);
                            SetBorderPosition(dictionary, row, axis, tickSize);
                            SetBorderLeft(row, left, tickSize);
                            currentPos++;
                        }
                    }

                    if (isOpposed)
                        left += (dictionary.Values.Max(rect => rect.Width) + Margin.Left);
                    else
                        left -= (dictionary.Values.Max(rect => rect.Width) + Margin.Right);
                    currentPos = 0;
                    row++;
                }

                prevEnd = 0;
            }
        }

        #endregion
        
        #region Protected Methods

        /// <summary>
        /// Returns desired width
        /// </summary>
        /// <returns>Returns the total width of the rows and columns collection</returns>
        protected override double LayoutElements()
        {
            if (Axis.LabelsIntersectAction == AxisLabelsIntersectAction.Wrap)
            {
                CalculateWrapLabelRect();
                CalcBounds(AvailableSize.Height - Axis.ActualPlotOffset * 2);
            }

            base.LayoutElements();

            return RectssByRowsAndCols.Sum(dictionary => dictionary.Values.Max(rect => rect.Width));
        }

        /// <summary>
        /// Calculates the bounds.
        /// </summary>
        /// <param name="availableHeight">The Available Height</param>
        protected override void CalcBounds(double availableHeight)
        {
            RectssByRowsAndCols = new List<Dictionary<int, Rect>>();
            RectssByRowsAndCols.Add(new Dictionary<int, Rect>());

            for (int j = 0; j < Children.Count; j++)
            {
                double position = 0d;
                double coeff;
                var axis = Axis as NumericalAxis;
                if (axis != null && axis.BreakExistence())
                {
                    for (int i = 0; i < axis.AxisRanges.Count; i++)
                    {
                        if (!axis.AxisRanges[i].Inside(Axis.VisibleLabels[j].Position)) continue;
                        coeff = Axis.ValueToCoefficientCalc(Axis.VisibleLabels[j].Position);
                        position = ((1 - coeff) * availableHeight) - (ComputedSizes[j].Height / 2);
                        foreach (DoubleRange range in axis.BreakRanges)
                        {
                            if (Math.Round(range.Start, 6) == Convert.ToDouble((Axis.VisibleLabels[j].LabelContent)))
                            {
                                position = axis.IsInversed ? position - (ComputedSizes[j].Height / 2) :
                                    position + (ComputedSizes[j].Height / 2);
                            }
                            else if (Math.Round(range.End, 6) == Convert.ToDouble((Axis.VisibleLabels[j].LabelContent)))
                            {
                                position = axis.IsInversed ? position + (ComputedSizes[j].Height / 2) :
                                    position - (ComputedSizes[j].Height / 2);
                            }
                        }
                    }
                }
                else
                {
                    coeff = Axis.ValueToCoefficientCalc(Axis.VisibleLabels[j].Position);
                    position = ((1 - coeff) * availableHeight) - (ComputedSizes[j].Height / 2);
                }

                var labelAlignment = Axis.AxisLabelAlignment;
                if (Axis.VisibleLabels[j].LabelStyle != null)
                {
                    labelAlignment = Axis.VisibleLabels[j].AxisLabelAlignment;
                }
                if (labelAlignment == LabelAlignment.Far)
                {
                    position += ComputedSizes[j].Height / 2;
                }
                else if (labelAlignment == LabelAlignment.Near)
                {
                    position -= ComputedSizes[j].Height / 2;
                }

                RectssByRowsAndCols[0].Add(j, new Rect(new Point(0, position), ComputedSizes[j]));
            }

            if (Axis.EdgeLabelsDrawingMode == EdgeLabelsDrawingMode.Shift)
            {
                if (RectssByRowsAndCols[0][0].Bottom > availableHeight)
                {
                    double position = availableHeight - ComputedSizes[0].Height;
                    RectssByRowsAndCols[0][0] = new Rect(0, position, ComputedSizes[0].Width, ComputedSizes[0].Height);
                }

                int index = Children.Count - 1;
                if (RectssByRowsAndCols[0][index].Top < 0)
                {
                    RectssByRowsAndCols[0][index] = new Rect(0, 0, ComputedSizes[index].Width,
                                                             ComputedSizes[index].Height);
                }
            }
            else if (Axis.EdgeLabelsDrawingMode == EdgeLabelsDrawingMode.Hide)
            {
                if (RectssByRowsAndCols[0][0].Bottom > availableHeight)
                {
                    RectssByRowsAndCols[0][0] = new Rect(0, 0, 0, 0);
                    Children[0].Visibility = Visibility.Collapsed;
                }

                int index = Children.Count - 1;
                if (RectssByRowsAndCols[0][index].Top < 0)
                {
                    RectssByRowsAndCols[0][index] = new Rect(0, 0, 0, 0);
                    Children[index].Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Calculates the actual plot offset.
        /// </summary>
        /// <param name="availableSize">The Available Size</param>
        protected override void CalculateActualPlotOffset(Size availableSize)
        {
            if (Axis.EdgeLabelsDrawingMode == EdgeLabelsDrawingMode.Fit)
            {
                double coeff = Axis.ValueToCoefficientCalc(Axis.VisibleLabels[0].Position);
                double position = ((1 - coeff) * availableSize.Height) - (ComputedSizes[0].Height / 2);
                double firstElementHeight = 0;
                double lastElementHeight = 0;
                if ((position + ComputedSizes[0].Height / 2) - Axis.PlotOffset > availableSize.Height)
                {
                    firstElementHeight = ComputedSizes[0].Height;
                }

                int index = Children.Count - 1;
                coeff = Axis.ValueToCoefficientCalc(Axis.VisibleLabels[index].Position);
                position = ((1 - coeff) * availableSize.Height) - (ComputedSizes[index].Height / 2);

                if ((position - ComputedSizes[index].Height / 2) + Axis.PlotOffset > availableSize.Height)
                {
                    lastElementHeight = ComputedSizes[index].Height;
                }

                double offset = Math.Max(firstElementHeight / 2, lastElementHeight / 2);
                Axis.ActualPlotOffset = Math.Max(offset, Axis.PlotOffset);
            }
            else
            {
                base.CalculateActualPlotOffset(availableSize);
            }
        }

        #endregion
        
        #region Private Methods

        /// <summary>
        /// To set the border thicknesss for axis label border
        /// </summary>
        /// <param name="row">The Row</param>
        /// <param name="dictionary">The Dictionary</param>
        /// <param name="axis">The Axis</param>
        private void SetBorderThickness(
            int row,
            Dictionary<int, Rect> dictionary,
            ChartAxisBase2D axis)
        {
            bool isSidebySideSeries = CheckCartesianSeries();
            var thickness = axis.LabelBorderWidth;
            if (CheckLabelPlacement(isSidebySideSeries))
                currentBorder.BorderThickness = isOpposed ?
                   new Thickness(0, thickness, thickness, thickness) :
                   new Thickness(thickness, thickness, 0, thickness);
            else
            {
                if (currentPos == 0)
                    currentBorder.BorderThickness = isOpposed ?
                        new Thickness(0, thickness, thickness, 0) :
                        new Thickness(thickness, thickness, 0, 0);
                else if (currentPos + 1 < dictionary.Count)
                    currentBorder.BorderThickness = isOpposed ?
                     new Thickness(0, thickness, thickness, thickness) :
                     new Thickness(thickness, thickness, 0, thickness);
                else
                    currentBorder.BorderThickness = isOpposed ?
                      new Thickness(0, 0, thickness, thickness) :
                      new Thickness(thickness, 0, 0, thickness);
            }
        }

        /// <summary>
        /// Calculates the corresponding screen co-ordinate value.
        /// </summary>
        /// <param name="value">The Value</param>
        /// <returns>Returns corresponding screen co-ordinate value</returns>
        private double CalculatePoint(double value)
        {
            return Axis.ActualPlotOffset
                         + Math.Round(Axis.RenderedRect.Height
                         * (1 - Axis.ValueToCoefficient(value)));
        }

        /// <summary>
        /// To place the label when the LabelPlacement property is OnTicks
        /// </summary>
        /// <param name="dictionary">The Dictionary</param>
        /// <param name="row">The Row</param>
        /// <param name="axis">The Axis</param>
        /// <param name="tickSize">The Tick Size</param>
        private void SetBorderPosition(Dictionary<int, Rect> dictionary, int row,
            ChartAxisBase2D axis, double tickSize)
        {
            if (currentPos == 0)
                prevEnd = Axis.IsInversed ? CalculatePoint(Axis.VisibleRange.End) :
                      prevEnd = CalculatePoint(Axis.VisibleRange.Start);
            if (currentPos + 1 < dictionary.Count)
            {
                KeyValuePair<int, Rect> key1 = dictionary.ElementAt(currentPos);
                KeyValuePair<int, Rect> key2 = dictionary.ElementAt(currentPos + 1);
                var value = (Axis.VisibleLabels[key1.Key].Position + Axis.VisibleLabels[key2.Key].Position) / 2;
                double value1 = CalculatePoint(value);

                currentBorder.Height = prevEnd - value1;
                currentBorder.Height += axis.LabelBorderWidth;
                Canvas.SetTop(currentBorder, value1 - axis.LabelBorderWidth / 2);
                prevEnd = value1;
            }
            else
            {
                var lastLabel = Axis.IsInversed ? CalculatePoint(Axis.VisibleRange.Start) :
                    CalculatePoint(Axis.VisibleRange.End);
                currentBorder.Height = prevEnd - lastLabel;
                currentBorder.Height += axis.LabelBorderWidth;
                Canvas.SetTop(currentBorder, lastLabel - axis.LabelBorderWidth / 2);
            }
            if (Axis.LabelsPosition == Axis.TickLinesPosition)
                currentBorder.Width = row == 0 ? maxWidth + tickSize
                    + Margin.Left + Margin.Right : maxWidth + Margin.Left + Margin.Right;
            else
                currentBorder.Width = maxWidth + Margin.Left + Margin.Right;
        }
        
        /// <summary>
        /// To position the border on its left value
        /// </summary>
        /// <param name="row">The Row</param>
        /// <param name="left">The Left</param>
        /// <param name="tickSize">The Tick Size</param>
        private void SetBorderLeft(int row, double left, double tickSize)
        {
            double leftValue = 0;
            if (RectssByRowsAndCols.Count > 1 && row > 0)
            {
                if (isOpposed)
                    leftValue = left;
                else
                    leftValue = (left - currentBorder.Width);
            }
            else
            {
                if (isOpposed)
                    leftValue = (Axis.LabelsPosition != Axis.TickLinesPosition) ?
                        left - Margin.Right :
                        left - (tickSize + Margin.Right);
                else
                    leftValue = (Axis.LabelsPosition != Axis.TickLinesPosition) ?
                        (left - currentBorder.Width) + Margin.Right :
                        (left - currentBorder.Width) + tickSize + Margin.Right;
            }

            Canvas.SetLeft(currentBorder, leftValue);
        }

        #endregion
        
        #endregion
    }
}
