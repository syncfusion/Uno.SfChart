// <copyright file="ChartPolarAxisLayoutPanel.cs" company="Syncfusion. Inc">
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
    /// Represents <see cref="ChartPolarAxisLayoutPanel"/>
    /// </summary>
    public partial class ChartPolarAxisLayoutPanel : ILayoutCalculator
    {
        #region Fields

        private Size desiredSize;
        private Panel panel;
        private double radius;
        private bool isRadiusCalculating;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPolarAxisLayoutPanel"/> class.
        /// </summary>
        /// <param name="panel">The Required Panel</param>
        /// <exception cref="ArgumentNullException"><see cref="ArgumentNullException"/> may be thrown</exception>
        public ChartPolarAxisLayoutPanel(Panel panel)
        {
            if (panel == null)
                throw new ArgumentNullException();

            this.panel = panel;
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the Chart area of the panel.
        /// </summary>
        public SfChart Area
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
        /// Gets or sets the polar axis of the Chart area.
        /// </summary>      
        public ChartAxisBase2D PolarAxis
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Cartesian axis of the Chart area.
        /// </summary>      
        public ChartAxisBase2D CartesianAxis
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the radius of the panel
        /// </summary>
        public double Radius
        {
            get
            {
                return radius;
            }

            set
            {
                if (radius != value)
                {
                    radius = value;

                    if (!isRadiusCalculating
                        && Area != null && Area.GridLinesLayout is ChartPolarGridLinesPanel)
                    {
                        CalculateSeriesRect(this.desiredSize);
                        (Area.GridLinesLayout as ChartPolarGridLinesPanel).UpdateElements();
                        (Area.GridLinesLayout as ChartPolarGridLinesPanel).Measure(this.desiredSize);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the desired sze of a panel.
        /// </summary>
        public Size DesiredSize
        {
            get { return desiredSize; }
        }

        /// <summary>
        /// Gets the Children count in the panel.
        /// </summary>
        public List<UIElement> Children
        {
            get
            {
                if (panel != null)
                {
                    return panel.Children.Cast<UIElement>().ToList();
                }

                return null;
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

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Measures the elements in the panel
        /// </summary>
        /// <param name="availableSize">Available Size of the panel</param>
        /// <returns></returns>
        public Size Measure(Size availableSize)
        {
            isRadiusCalculating = true;
            if (PolarAxis != null)
            {
                PolarAxis.ComputeDesiredSize(availableSize);
                if (PolarAxis.EnableScrollBar)
                {
                    PolarAxis.DisableScrollbar = true;
                    PolarAxis.EnableScrollBar = !PolarAxis.DisableScrollbar;
                }
            }
            if (CartesianAxis != null)
            {
                if (CartesianAxis.EnableScrollBar)
                {
                    CartesianAxis.DisableScrollbar = true;
                    CartesianAxis.EnableScrollBar = !CartesianAxis.DisableScrollbar;
                }

                CalculateSeriesRect(availableSize);

                //Based on StartAngle cartesian axis has been measured
                if (CartesianAxis.PolarAngle == ChartPolarAngle.Rotate0 ||
                    CartesianAxis.PolarAngle == ChartPolarAngle.Rotate180)
                    CartesianAxis.ComputeDesiredSize(new Size
                        ((Math.Min(Area.SeriesClipRect.Width, Area.SeriesClipRect.Height) / 2),
                        Area.SeriesClipRect.Height));
                else
                    CartesianAxis.ComputeDesiredSize(new Size(Area.SeriesClipRect.Width,
                      Math.Min(Area.SeriesClipRect.Width, Area.SeriesClipRect.Height) / 2));
            }

            isRadiusCalculating = false;
            desiredSize = availableSize;
            return availableSize;
        }

        /// <summary>
        /// Seek the elements.
        /// </summary>
        public void DetachElements()
        {
            if (CartesianAxis != null)
            {
                if (CartesianAxis.GridLinesRecycler != null)
                    CartesianAxis.GridLinesRecycler.Clear();
                if (CartesianAxis.MinorGridLinesRecycler != null)
                    CartesianAxis.MinorGridLinesRecycler.Clear();
            }

            panel.Children.Clear();
            panel = null;
        }

        /// <summary>
        /// Arranges the elements in a panel
        /// </summary>
        /// <param name="finalSize">final size of the panel.</param>
        /// <returns>returns Size</returns>
        public Size Arrange(Size finalSize)
        {
            Rect clientRect = new Rect(0, 0, finalSize.Width, finalSize.Height);
            if (PolarAxis != null)
            {
                PolarAxis.ArrangeRect = clientRect;
                PolarAxis.Measure(new Size(clientRect.Width, clientRect.Height));
                PolarAxis.Arrange(clientRect);
                Canvas.SetLeft(PolarAxis, clientRect.Left);
                Canvas.SetTop(PolarAxis, clientRect.Top);
            }

            ChartAxis yAxis = this.CartesianAxis;

            if (yAxis != null)
            {
                Point center = ChartLayoutUtils.GetCenter(clientRect);
                CalculateCartesianArrangeRect(center, yAxis);

                Rect rect = new Rect(this.CartesianAxis.ArrangeRect.Left,
                    this.CartesianAxis.ArrangeRect.Top,
                    this.CartesianAxis.ComputedDesiredSize.Width,
                    this.CartesianAxis.ComputedDesiredSize.Height);
                if (CartesianAxis.PolarAngle != ChartPolarAngle.Rotate90 &&
                    (!yAxis.OpposedPosition
                    || CartesianAxis.PolarAngle == ChartPolarAngle.Rotate180))
                    CartesianAxis.Measure(new Size(rect.Width, rect.Height));
                else
                    CartesianAxis.Measure(new Size(rect.Left, rect.Height));
                CartesianAxis.Arrange(rect);
                Canvas.SetLeft(CartesianAxis, CartesianAxis.ArrangeRect.Left);
                Canvas.SetTop(CartesianAxis, CartesianAxis.ArrangeRect.Top);
                if (CartesianAxis.PolarAngle == ChartPolarAngle.Rotate90 ||
                    CartesianAxis.PolarAngle == ChartPolarAngle.Rotate180)
                    Area.InternalSecondaryAxis.IsInversed = !Area.InternalSecondaryAxis.IsInversed;
            }

            return finalSize;
        }

        /// <summary>
        /// Method declaration for UpdateElements
        /// </summary>
        public void UpdateElements()
        {
            if (this.Area != null && this.Area.InternalPrimaryAxis != null)
            {
                List<UIElement> removedElements = new List<UIElement>();
                if (Children == null) return;
                PolarAxis = this.Area.InternalPrimaryAxis as ChartAxisBase2D;
                PolarAxis.AxisLayoutPanel = this;
                var angle = (this.Area.InternalSecondaryAxis as ChartAxisBase2D).PolarAngle;
                if ((angle == ChartPolarAngle.Rotate0) || (angle == ChartPolarAngle.Rotate180)) // Based on StartAngle the oriention                                                                                      
                {                                                                    //of secondary axis has changed
                    this.Area.InternalSecondaryAxis.Orientation = Orientation.Horizontal;
                    var numericalAxis = this.Area.InternalSecondaryAxis as NumericalAxis;
                    if (numericalAxis != null)
                        numericalAxis.RangePadding = NumericalPadding.Round;
                }
                else
                    this.Area.InternalSecondaryAxis.Orientation = Orientation.Vertical;
                if (Children.Count > 0 && (angle == ChartPolarAngle.Rotate90 || angle == ChartPolarAngle.Rotate180))
                    this.Area.InternalSecondaryAxis.IsInversed = !this.Area.InternalSecondaryAxis.IsInversed;
                CartesianAxis = this.Area.InternalSecondaryAxis as ChartAxisBase2D;
                foreach (UIElement element in Children)
                {
                    ChartAxis chartAxis = element as ChartAxis;
                    if (chartAxis != null && !Area.Axes.Contains(chartAxis))
                        removedElements.Add(chartAxis);
                }

                foreach (UIElement removedElement in removedElements)
                {
                    panel.Children.Remove(removedElement);
                }

                removedElements.Clear();
                removedElements = null;
                var children = Children;
                foreach (ChartAxis content in this.Area.Axes)
                {

                    if (!children.Contains(content))
                    {
                        panel.Children.Add(content);
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates the series rectangle.
        /// </summary>
        /// <param name="availableSize">The Available Size</param>
        private void CalculateSeriesRect(Size availableSize)
        {
            double width = Math.Max(availableSize.Width / 2 - Radius, 0);
            double height = Math.Max(availableSize.Height / 2 - Radius, 0);

            this.Area.AxisThickness = new Thickness(width, height, width, height);

            Rect rect = ChartLayoutUtils.Subtractthickness(new Rect(new Point(0, 0), availableSize), this.Area.AxisThickness);

            this.Area.SeriesClipRect = new Rect(rect.Left, rect.Top, rect.Width, rect.Height);

            Area.InternalCanvas.Clip = new RectangleGeometry()
            {
                Rect = new Rect(0, 0, this.Area.SeriesClipRect.Width, this.Area.SeriesClipRect.Height)
            };
        }

        /// <summary>
        /// To calculate the cartesian arrange rect based on StartAngle property 
        /// </summary>
        /// <param name="center">The Center Point</param>
        /// <param name="axis">The Axis</param>
        private void CalculateCartesianArrangeRect(Point center, ChartAxis axis)
        {
            double left;
            double top;
            switch (CartesianAxis.PolarAngle)
            {
                case ChartPolarAngle.Rotate270:
                    {
                        left = axis.OpposedPosition ? center.X :
                            center.X - this.CartesianAxis.ComputedDesiredSize.Width;

                        CartesianAxis.ArrangeRect = new Rect(left,
                        center.Y - radius,
                        this.CartesianAxis.ComputedDesiredSize.Width, radius);
                    }

                    break;
                case ChartPolarAngle.Rotate0:
                    {
                        top = axis.OpposedPosition ?
                            center.Y - this.CartesianAxis.ComputedDesiredSize.Height : center.Y;

                        CartesianAxis.ArrangeRect = new Rect(center.X, top, radius,
                          this.CartesianAxis.ComputedDesiredSize.Height);
                    }

                    break;
                case ChartPolarAngle.Rotate90:
                    {
                        left = axis.OpposedPosition ? center.X :
                            center.X - this.CartesianAxis.ComputedDesiredSize.Width;

                        CartesianAxis.ArrangeRect = new Rect(
                         left,
                         center.Y,
                         this.CartesianAxis.DesiredSize.Width, radius);
                    }

                    break;
                case ChartPolarAngle.Rotate180:
                    {
                        top = axis.OpposedPosition ?
                            center.Y - this.CartesianAxis.ComputedDesiredSize.Height : center.Y;

                        CartesianAxis.ArrangeRect = new Rect(center.X - radius,
                           top, radius,
                           this.CartesianAxis.ComputedDesiredSize.Height);
                    }

                    break;
                default:
                    break;
            }
        }

        #endregion

        #endregion
    }
}
