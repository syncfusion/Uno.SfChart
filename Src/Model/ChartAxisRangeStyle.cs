using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents the <see cref="ChartAxisRangeStyle"/> class to customize the axis GridLines, TickLines and LabelStyle for specific region.
    /// </summary>
    public partial class ChartAxisRangeStyle : DependencyObject
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="Start"/> property.
        /// </summary>
        public static readonly DependencyProperty StartProperty =
            DependencyProperty.Register(
                "Start",
                typeof(object),
                typeof(ChartAxisRangeStyle),
                new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="End"/> property.
        /// </summary>
        public static readonly DependencyProperty EndProperty =
            DependencyProperty.Register(
                "End",
                typeof(object),
                typeof(ChartAxisRangeStyle),
                new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="MajorGridLineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty MajorGridLineStyleProperty =
            DependencyProperty.Register(
                "MajorGridLineStyle", 
                typeof(Style), 
                typeof(ChartAxisRangeStyle),
                new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="MinorGridLineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty MinorGridLineStyleProperty =
            DependencyProperty.Register("MinorGridLineStyle", 
                typeof(Style), 
                typeof(ChartAxisRangeStyle),
                new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="MajorTickLineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty MajorTickLineStyleProperty =
            DependencyProperty.Register(
                "MajorTickLineStyle",
                typeof(Style),
                typeof(ChartAxisRangeStyle),
                new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="MinorTickLineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty MinorTickLineStyleProperty =
            DependencyProperty.Register(
                "MinorTickLineStyle",
                typeof(Style),
                typeof(ChartAxisRangeStyle),
                new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="LabelStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelStyleProperty =
            DependencyProperty.Register(
                "LabelStyle",
                typeof(LabelStyle),
                typeof(ChartAxisRangeStyle),
                new PropertyMetadata(null));

        #endregion

        #region Fields

        private DoubleRange range = DoubleRange.Empty;

        #endregion

        #region Constructor

        public ChartAxisRangeStyle()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the start range for customizing the axis style.
        /// </summary>
        public object Start
        {
            get { return (object)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }

        /// <summary>
        /// Gets or sets the end range for customizing the axis style.
        /// </summary>
        public object End
        {
            get { return (object)GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }

        /// <summary>
        /// Gets or sets options for customizing the axis labels.
        /// </summary>
        public LabelStyle LabelStyle
        {
            get { return (LabelStyle)GetValue(LabelStyleProperty); }
            set { SetValue(LabelStyleProperty, value); }
        }

        /// <summary>
        ///  Gets or sets options for customizing the major gridlines.
        /// </summary>
        public Style MajorGridLineStyle
        {
            get { return (Style)GetValue(MajorGridLineStyleProperty); }
            set { SetValue(MajorGridLineStyleProperty, value); }
        }

        /// <summary>
		///  Gets or sets options for customizing the minor gridline.
		/// </summary>
        public Style MinorGridLineStyle
        {
            get { return (Style)GetValue(MinorGridLineStyleProperty); }
            set { SetValue(MinorGridLineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets options for customizing the major tick lines.
        /// </summary>
        public Style MajorTickLineStyle
        {
            get { return (Style)GetValue(MajorTickLineStyleProperty); }
            set { SetValue(MajorTickLineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets options for customizing the minor tick lines.
        /// </summary>
        public Style MinorTickLineStyle
        {
            get { return (Style)GetValue(MinorTickLineStyleProperty); }
            set { SetValue(MinorTickLineStyleProperty, value); }
        }

        #endregion

        #region internal properties

        internal DoubleRange Range
        {
            get
            {
                return range;
            }

            set
            {
                if (range == value)
                {
                    return;
                }

                range = value;
            }
        }

        #endregion
    }
}