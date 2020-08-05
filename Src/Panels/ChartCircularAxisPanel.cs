// <copyright file="ChartCircularAxisPanel.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Shapes;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents <see cref="ChartCircularAxisPanel"/> class.
    /// </summary>   
    public partial class ChartCircularAxisPanel : ILayoutCalculator
    {
        #region Fields

        private Size desiredSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
        private Panel panel;
        private UIElementsRecycler<Line> lineRecycler;
        private UIElementsRecycler<ContentControl> contentControlRecycler;
        private ChartAxis axis;
        private Size m_maxLabelsSize;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartCircularAxisPanel"/> class.
        /// </summary>
        /// <param name="panel">The Required Panel</param>        
        public ChartCircularAxisPanel(Panel panel)
        {
            if (panel == null)
                throw new ArgumentNullException();

            this.panel = panel;

            lineRecycler = new UIElementsRecycler<Line>(panel);
            contentControlRecycler = new UIElementsRecycler<ContentControl>(panel);
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the center point of the panel.
        /// </summary>      
        public Point Center
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the panel.
        /// </summary>
        /// <value>
        /// The panel.
        /// </value>
        public Panel Panel
        {
            get { return panel; }
        }

        /// <summary>
        /// Gets or sets the radius of the circular panel.
        /// </summary>     
        public double Radius
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the axis of the panel.
        /// </summary>       
        public ChartAxis Axis
        {
            get
            {
                return axis;
            }
            set
            {
                axis = value;
            }
        }

        /// <summary>
        /// Gets the desired size of the panel.
        /// </summary>       
        public Size DesiredSize
        {
            get { return desiredSize; }
        }

        /// <summary>
        /// Gets the Children count of the panel.
        /// </summary>      
        public List<UIElement> Children
        {
            get { return this.panel.Children.Cast<UIElement>().ToList(); }
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

        #endregion

        #region Methods

        #region Public Methods
        
        /// <summary>
        /// Method implementation for Render labels and Ticks
        /// </summary>
        public void RenderElements()
        {
            RenderLabels();
            RenderTicks();
        }

        /// <summary>
        /// Measures the elements in a panel.
        /// </summary>
        /// <param name="availableSize">available size of the panel.</param>
        /// <returns>returns Size.</returns>
        public Size Measure(Size availableSize)
        {
            m_maxLabelsSize = new Size();

            IEnumerable enumerator = contentControlRecycler as IEnumerable;

            foreach (UIElement element in enumerator)
            {
                element.Measure(availableSize);

                m_maxLabelsSize.Width = Math.Max(element.DesiredSize.Width, m_maxLabelsSize.Width);
                m_maxLabelsSize.Height = Math.Max(element.DesiredSize.Height, m_maxLabelsSize.Height);
            }

            double dx = availableSize.Width - 2 * m_maxLabelsSize.Width;
            double dy = availableSize.Height - 2 * m_maxLabelsSize.Height;

            this.Radius = 0.5 * Math.Min(dx, dy) - Math.Max(Axis.TickLineSize, 0);
            this.Center = (Point)ChartLayoutUtils.GetCenter(availableSize);

            return availableSize;
        }

        /// <summary>
        /// Seek the elements.
        /// </summary>
        public void DetachElements()
        {
            panel = null;

            if (lineRecycler != null)
                lineRecycler.Clear();
            if (contentControlRecycler != null)
                contentControlRecycler.Clear();
        }

        /// <summary>
        /// Arranges the elements in a panel.
        /// </summary>
        /// <param name="finalSize">final Size of the panel.</param>
        /// <returns>returns Size.</returns>      
        public Size Arrange(Size finalSize)
        {
            double dx = finalSize.Width - 2 * m_maxLabelsSize.Width;
            double dy = finalSize.Height - 2 * m_maxLabelsSize.Height;

            this.Radius = Math.Max(0, 0.5 * Math.Min(dx, dy) - Math.Max(Axis.TickLineSize, 0));
            this.Center = (Point)ChartLayoutUtils.GetCenter(finalSize);

            RenderElements();
            return finalSize;
        }

        /// <summary>
        /// Adds the elements in a panel.
        /// </summary>       
        public void UpdateElements()
        {
            UpdateLabels();
            UpdateTickLines();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the labels.
        /// </summary>
        private void UpdateLabels()
        {
            int pos = 0;

            ObservableCollection<ChartAxisLabel> visibleLabels = this.Axis.VisibleLabels;

            contentControlRecycler.GenerateElements(visibleLabels.Count);
            var prefixLabeltemplate = this.Axis.PrefixLabelTemplate;
            var postfixLabelTemplate = this.Axis.PostfixLabelTemplate;

            foreach (var item in visibleLabels)
            {

                if (this.Axis is NumericalAxis && pos == Axis.VisibleLabels.Count - 1)
                {
                    return;
                }

                ContentControl control = contentControlRecycler[pos];
                control.Tag = visibleLabels[pos];

                if (this.Axis.LabelTemplate == null)
                {
                    control.ContentTemplate = ChartDictionaries.GenericCommonDictionary["AxisLabelsCustomTemplate"] as DataTemplate;
                    control.ApplyTemplate();
                    item.PrefixLabelTemplate = prefixLabeltemplate;
                    item.PostfixLabelTemplate = postfixLabelTemplate;
                    if (Axis.LabelStyle != null)
                    {
                        if (Axis.LabelStyle.Foreground != null)
                        {
                            var foregroundBinding = new Binding { Source = Axis.LabelStyle, Path = new PropertyPath("Foreground") };
                            control.SetBinding(Control.ForegroundProperty, foregroundBinding);
                        }
                        if (Axis.LabelStyle.FontSize > 0.0)
                        {
                            var fontSizeBinding = new Binding { Source = Axis.LabelStyle, Path = new PropertyPath("FontSize") };
                            control.SetBinding(Control.FontSizeProperty, fontSizeBinding);
                        }
                        if (Axis.LabelStyle.FontFamily != null)
                        {
                            var fontFamilyBinding = new Binding { Source = Axis.LabelStyle, Path = new PropertyPath("FontFamily") };
                            control.SetBinding(Control.FontFamilyProperty, fontFamilyBinding);
                        }
                    }
                    control.Content = item;
                }
                else
                {
                    control.ContentTemplate = this.Axis.LabelTemplate;
                    control.ApplyTemplate();
                    control.Content = item;
                }

                pos++;
            }
        }

        /// <summary>
        /// Updates the tick lines.
        /// </summary>
        private void UpdateTickLines()
        {
            int linesCount = this.Axis.VisibleLabels.Count;

            if (!lineRecycler.BindingProvider.Keys.Contains(Line.StyleProperty))
            {
                Binding binding = new Binding();
                binding.Source = Axis;
                binding.Path = new PropertyPath("MajorTickLineStyle");
                lineRecycler.BindingProvider.Add(Line.StyleProperty, binding);
            }

            lineRecycler.GenerateElements(linesCount);
        }

        /// <summary>
        /// Renders the tick lines.
        /// </summary>
        private void RenderTicks()
        {
            Point center = this.Center;
            double radius = this.Radius;
            int pos = 0;
            foreach (ChartAxisLabel label in Axis.VisibleLabels)
            {
                Point vector = ChartTransform.ValueToVector(this.Axis, label.Position);
                Line line = lineRecycler[pos];
                Point connectPoint = new Point(center.X + radius * vector.X, center.Y + radius * vector.Y);
                line.X1 = connectPoint.X;
                line.Y1 = connectPoint.Y;
                line.X2 = connectPoint.X + Axis.TickLineSize * vector.X;
                line.Y2 = connectPoint.Y + Axis.TickLineSize * vector.Y;
                pos++;
            }
        }

        /// <summary>
        /// Renders the labels.
        /// </summary>
        private void RenderLabels()
        {
            double aroundRadius = this.Radius + Math.Max(Axis.TickLineSize, 0);

            int pos = 0;


            foreach (ChartAxisLabel label in Axis.VisibleLabels)
            {

                double coef = this.Axis.ValueToPolarCoefficient(label.Position);

                FrameworkElement element = (FrameworkElement)contentControlRecycler[pos];

                Point vector = ChartTransform.ValueToVector(this.Axis, label.Position);
                Point connectPoint = new Point(this.Center.X + aroundRadius * vector.X,
                    this.Center.Y + aroundRadius * vector.Y);

                var labelwidth = ((element as UIElement).DesiredSize.Width) / 2;

                if (coef == 0.25d)
                {
                    connectPoint.X -= element.DesiredSize.Width;
                    connectPoint.Y -= element.DesiredSize.Height / 2;
                }
                else if (coef == 0.5d)
                {
                    connectPoint.X -= element.DesiredSize.Width / 2;
                }
                else if (coef == 0.75d)
                {
                    connectPoint.Y -= element.DesiredSize.Height / 2;
                }
                else if(coef == 1d || coef == 0d)
                {
                    connectPoint.X -= element.DesiredSize.Width / 2;
                    connectPoint.Y -= element.DesiredSize.Height;
                }
                else if (0 < coef && coef < 0.25d)
                {
                    connectPoint.X -= element.DesiredSize.Width;
                    connectPoint.Y -= element.DesiredSize.Height;
                }
                else if (0.25d < coef && coef < 0.5d)
                {
                        connectPoint.X -= element.DesiredSize.Width;
                }
                else if (0.75d < coef && coef < 1d)
                {
                    connectPoint.Y -= element.DesiredSize.Height;
                }

                // element.Arrange(new Rect(connectPoint, element.DesiredSize));
                Canvas.SetLeft(element, connectPoint.X);
                Canvas.SetTop(element, connectPoint.Y);
                pos++;

            }
        }

        #endregion

        #endregion
    }
}
