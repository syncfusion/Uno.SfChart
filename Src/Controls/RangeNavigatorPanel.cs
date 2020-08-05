// <copyright file="RangeNavigatorPanel.cs" company="Syncfusion. Inc">
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
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;

    /// <summary>
    /// Represents the <see cref="RangeNavigatorPanel"/> class.
    /// </summary>
    public partial class RangeNavigatorPanel : Panel
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="Row"/> property.
        /// </summary>
        public static readonly DependencyProperty RowProperty =
            DependencyProperty.RegisterAttached(
                "Row",
                typeof(int),
                typeof(RangeNavigatorPanel),
                new PropertyMetadata(0));

        #endregion

        #region Fields

        private RangeNavigatorRowDefinitions rowDefinitions;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the range navigator rows
        /// </summary>
        public RangeNavigatorRowDefinitions RowDefinitions
        {
            get
            {
                if (rowDefinitions == null)
                    rowDefinitions = new RangeNavigatorRowDefinitions();

                return rowDefinitions;
            }

            set
            {
                rowDefinitions = value;
            }
        }

        #endregion

        #region Methods

        #region Public Static Methods

        /// <summary>
        /// Get the row 
        /// </summary>
        /// <param name="obj">UI Element</param>
        /// <returns>Return the row index.</returns>
        public static int GetRow(UIElement obj)
        {
            return (int)obj.GetValue(RowProperty);
        }

        /// <summary>
        /// Set the row
        /// </summary>
        /// <param name="obj">UI Element</param>
        /// <param name="value">The Value</param>
        public static void SetRow(UIElement obj, int value)
        {
            obj.SetValue(RowProperty, value);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Measures the children of the panel.
        /// </summary>
        /// <param name="availableSize">The Available Size</param>
        /// <returns>Returns the measure size.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement element in this.Children)
            {
                int rowIndex = GetRow(element);
                this.RowDefinitions[rowIndex].Element.Add(element);
            }
            
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Arranges the children of the panel.
        /// </summary>
        /// <param name="finalSize">The Final Size</param>
        /// <returns>Returns the arrange size.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double rowTop = 0;
            double usedHeight = 0;
            double rowStarCount = this.RowDefinitions.Sum((rowDef)
                =>
            {
                if (rowDef.Unit == ChartUnitType.Star)
                    return rowDef.Height;
                return 0;
            });

            double rowFixedHeight = this.RowDefinitions.Sum((rowDef)
                =>
            {
                if (rowDef.Unit == ChartUnitType.Pixels)
                    return rowDef.Height;
                return 0;
            });

            double remainingHeight = Math.Max(0, finalSize.Height - rowFixedHeight);
            double singleStarHeight = remainingHeight / rowStarCount;

            for (int i = 0; i < this.RowDefinitions.Count; i++)
            {
                RangeNavigatorRowDefinition row = this.RowDefinitions[i];
                double remainingSize = finalSize.Height - usedHeight;
                double height = 0;
                if (row.Unit == ChartUnitType.Star)
                {
                    height = Math.Min(remainingSize, row.Height * singleStarHeight);
                }
                else
                {
                    height = Math.Min(remainingSize, row.Height);
                }

                row.Height = double.IsNaN(height) ? 1d : height;
                RowDefinitions[i].Arrange(finalSize, rowTop);
                usedHeight += double.IsNaN(height) ? 1d : height;
                row.RowTop = rowTop;
                rowTop += double.IsNaN(height) ? 1d : height;
            }

            return base.ArrangeOverride(finalSize);
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Represents the <see cref="RangeNavigatorRowDefinitions"/> class.
    /// </summary>
    public partial class RangeNavigatorRowDefinitions : ObservableCollection<RangeNavigatorRowDefinition>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeNavigatorRowDefinitions"/> class.
        /// </summary>
        public RangeNavigatorRowDefinitions()
        {
        }

        #endregion
    }

    /// <summary>
    /// Represents the <see cref="RangeNavigatorRowDefinition"/> class.
    /// </summary>
    public partial class RangeNavigatorRowDefinition : DependencyObject
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="Height"/> property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(
                "Height",
                typeof(double),
                typeof(RangeNavigatorRowDefinition),
                new PropertyMetadata(1d));

        /// <summary>
        /// The DependencyProperty for <see cref="Unit"/> property.
        /// </summary>
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register(
                "Unit",
                typeof(ChartUnitType),
                typeof(RangeNavigatorRowDefinition),
                new PropertyMetadata(ChartUnitType.Star));

        /// <summary>
        ///  The DependencyProperty for <see cref="BorderThickness"/> property.
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(
                "BorderThickness",
                typeof(double),
                typeof(RangeNavigatorRowDefinition),
                new PropertyMetadata(0d));

        /// <summary>
        ///  The DependencyProperty for <see cref="BorderStroke"/> property.
        /// </summary>
        public static readonly DependencyProperty BorderStrokeProperty =
            DependencyProperty.Register(
                "BorderStroke",
                typeof(Brush),
                typeof(RangeNavigatorRowDefinition),
                new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        #endregion

        #region Fields

        private List<UIElement> element;

        private Rect[] elementBounds;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeNavigatorRowDefinition"/> class.
        /// </summary>
        public RangeNavigatorRowDefinition()
        {
            element = new List<UIElement>();
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets the row top value
        /// </summary>
        public double RowTop
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets height of this row.
        /// </summary>
        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets unit of the value specified in Height.
        /// </summary>
        public ChartUnitType Unit
        {
            get { return (ChartUnitType)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        /// <summary>
        /// Gets or sets thickness of the border.
        /// </summary>
        public double BorderThickness
        {
            get { return (double)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets border stroke.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush BorderStroke
        {
            get { return (Brush)GetValue(BorderStrokeProperty); }
            set { SetValue(BorderStrokeProperty, value); }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the <see cref="UIElement"/>.
        /// </summary>
        internal List<UIElement> Element
        {
            get
            {
                return element;
            }

            set
            {
                element = value;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Internal Methods

        /// <summary>
        /// Measures the <see cref="RangeNavigatorRowDefinition"/>
        /// </summary>
        /// <param name="size">The Size</param>
        /// <param name="rowIndex">The Row Index</param>
        /// <param name="rowHeight">The Row Height</param>
        internal void Measure(Size size, int rowIndex, double rowHeight)
        {
            elementBounds = new Rect[Element.Count];
            for (int i = 0; i < elementBounds.Length; i++)
            {
                if (Unit == ChartUnitType.Pixels)
                {
                    elementBounds[i].Height = Height;
                }
            }
        }

        /// <summary>
        /// Arranges the <see cref="RangeNavigatorRowDefinition"/>
        /// </summary>
        /// <param name="availableSize">The Available Size</param>
        /// <param name="top">The Top</param>
        internal void Arrange(Size availableSize, double top)
        {
            foreach (UIElement content in Element)
            {
                double newheight = Height < 0 ? 1 : Height;
                content.Measure(new Size(availableSize.Width, newheight));
                content.Arrange(new Rect(0, top, availableSize.Width, newheight));
            }
        }

        #endregion

        #endregion
    }
}
