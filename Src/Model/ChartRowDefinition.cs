using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents a row definition.
    /// </summary>
    /// <remarks>
    /// The height of the row can be defined either in terms of fixed pixels units mode or auto adjust mode, by using <see cref="ChartRowDefinition.Unit"/> property.
    /// </remarks>
    public partial class ChartRowDefinition : DependencyObject, ICloneable
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="Height"/> property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(
                "Height", 
                typeof(double),
                typeof(ChartRowDefinition),
                new PropertyMetadata(1d, OnRowPropertyChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="Unit"/> property.
        /// </summary>
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register(
                "Unit",
                typeof(ChartUnitType), 
                typeof(ChartRowDefinition),
                new PropertyMetadata(ChartUnitType.Star, OnRowPropertyChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="BorderThickness"/> property.
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(
                "BorderThickness", 
                typeof(double),
                typeof(ChartRowDefinition),
                new PropertyMetadata(0d));

        /// <summary>
        ///  The DependencyProperty for <see cref="BorderStroke"/> property.
        /// </summary>
        public static readonly DependencyProperty BorderStrokeProperty =
            DependencyProperty.Register(
                "BorderStroke", 
                typeof(Brush), 
                typeof(ChartRowDefinition),
#if UNIVERSALWINDOWS
                new PropertyMetadata(null));
#else
                new PropertyMetadata(new SolidColorBrush(Colors.Red)));
#endif
        #endregion

        #region Fields

        #region Internal Fields

        internal Line BorderLine;

        #endregion

        #region Private Fields

        private List<ChartAxis> axis;

        private double headerMargin = 2; // Add some gap between the axis header and plot area.

        private double computedHeight = 0;

        private double computedTop = 0;

        private List<ChartLegend> legends = new List<ChartLegend>();

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for ChartRowdefinitions
        /// </summary>
        public ChartRowDefinition()
        {
#if UNIVERSALWINDOWS
            BorderStroke = new SolidColorBrush(Colors.Red);
#endif
            axis = new List<ChartAxis>();
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets RowTap property
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
        /// Gets or sets unit of the value to be specified for row height.
        /// </summary>
        /// <value>
        /// <see cref="Syncfusion.UI.Xaml.Charts.ChartUnitType"/>
        /// </value>
        public ChartUnitType Unit
        {
            get { return (ChartUnitType)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        /// <summary>
        /// Gets or sets the thickness of the border.
        /// </summary>
        public double BorderThickness
        {
            get { return (double)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the brush for the border of the row.
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

        internal double ComputedHeight
        {
            get { return computedHeight; }
            set { computedHeight = value; }
        }


        internal double ComputedTop
        {
            get { return computedTop; }
            set { computedTop = value; }
        }

        internal List<ChartAxis> Axis
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

        internal List<ChartLegend> Legends
        {
            get
            {
                return legends;
            }
            set
            {
                legends = value;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Methods
        
        /// <summary>
        /// Clone the Row
        /// </summary>
        /// <returns></returns>
        public DependencyObject Clone()
        {
            return new ChartRowDefinition()
            {
                BorderStroke = this.BorderStroke,
                BorderThickness = this.BorderThickness,
                Height = this.Height,
                Unit = this.Unit
            };
        }

        #endregion

        #region Internal Methods

        internal void MeasureLegends(Size size, List<double> nearSizes, List<double> farSizes)
        {
            int nearIndex = 0;
            int farIndex = 0;
            foreach (ChartLegend content in Legends)
            {
                if (content != null && content.DockPosition != ChartDock.Floating && content.LegendPosition == LegendPosition.Outside)
                {
                    if (content.DesiredSize.Width == 0d)
                        content.Measure(size);
                    if (content.DockPosition == ChartDock.Left)
                    {
                        if (nearSizes.Count <= nearIndex)
                        {
                            nearSizes.Add(content.DesiredSize.Width);
                        }
                        else if (nearSizes[nearIndex] < (content.DesiredSize.Width))
                        {
                            nearSizes[nearIndex] = content.DesiredSize.Width;
                        }
                        nearIndex++;
                    }
                    else
                    {
                        if (farSizes.Count <= farIndex)
                        {
                            farSizes.Add(content.DesiredSize.Width);
                        }
                        else if (farSizes[farIndex] < (content.DesiredSize.Width))
                        {
                            farSizes[farIndex] = content.DesiredSize.Width;
                        }
                        farIndex++;
                    }
                }
            }
        }

        internal void Measure(Size size, List<double> nearSizes, List<double> farSizes, bool isFirstLayout)
        {
            int nearIndex = 0;
            int farIndex = 0;
            bool isOpposedFirstElement = true;
            bool isFirstElement = true;
            double innerPadding = 0;
            double axisHeight = 0, top = 0;

            foreach (ChartAxis content in axis)
            {
                if (content != null)
                {

                    if (content.Area != null)
                    {
                        CalcRowSpanAxisWidthandTop(top, content.Area.GetActualRowSpan(content), size.Height, content, out top, out axisHeight);
                        if (content.Area.GetActualRow(content) == content.Area.RowDefinitions.IndexOf(this))
                            content.ComputeDesiredSize(new Size(size.Width, axisHeight));
                    }

                    if (content.ShowAxisNextToOrigin && content.Area != null &&
                        content.Area.ColumnDefinitions.Count <= 1)
                    {
                        bool isContinue = false;
                        MeasureRectBasedOnOrigin(content, nearSizes, farSizes, ref isContinue, isFirstLayout);
                        if (isContinue)
                            continue;
                    }
                    if (content.OpposedPosition)
                    {
                        innerPadding = isOpposedFirstElement ? content.InsidePadding : 0;
                        if (farSizes.Count <= farIndex)
                        {
                            farSizes.Add(content.ComputedDesiredSize.Width - innerPadding);
                        }
                        else if (farSizes[farIndex] < (content.ComputedDesiredSize.Width - innerPadding))
                        {
                            farSizes[farIndex] = content.ComputedDesiredSize.Width - innerPadding;
                        }
                        farIndex++;
                        isOpposedFirstElement = false;
                    }
                    else
                    {
                        innerPadding = isFirstElement ? content.InsidePadding : 0;
                        if (nearSizes.Count <= nearIndex)
                        {
                            nearSizes.Add(content.ComputedDesiredSize.Width - innerPadding);
                        }
                        else if (nearSizes[nearIndex] < (content.ComputedDesiredSize.Width - innerPadding))
                        {
                            nearSizes[nearIndex] = content.ComputedDesiredSize.Width - innerPadding;
                        }
                        nearIndex++;
                        isFirstElement = false;
                    }
                }
            }
            
        }

        internal void UpdateLegendArrangeRect(double top, double height, double areaWidth, List<double> nearSizes, List<double> farSizes)
        {
            int nearIndex = 0;
            int farIndex = 0;
            double nearTotalSize = nearSizes.Sum();
            double farTotalSize = farSizes.Sum();
            double sum = farTotalSize;
            double axisheight = height;
            double axistop = top;
            for (int i = 0; i < Legends.Count; i++)
            {
                ChartLegend element = Legends[i];
                if (element != null && element.DockPosition != ChartDock.Floating && element.LegendPosition == LegendPosition.Outside)
                {
                    //Set RowSpan height and top value
                    if (element.ChartArea != null && element.YAxis != null)
                        CalcRowSpanAxisWidthandTop(top, element.ChartArea.GetActualRowSpan(element), height, element.YAxis, out axistop, out axisheight);
                    var desiredSize = element.DesiredSize;
                    if (element.DockPosition == ChartDock.Left)
                    {
                        element.ArrangeRect = new Rect(
                            (nearTotalSize - desiredSize.Width), 
                            axistop,
                            desiredSize.Width,
                            axisheight);
                        nearTotalSize -= nearSizes[nearIndex];
                        nearIndex++;
                    }
                    else
                    {
                        element.ArrangeRect = new Rect(
                            ((areaWidth + sum) - farTotalSize), 
                            axistop,
                            desiredSize.Width, 
                            axisheight);
                        farTotalSize -= farSizes[farIndex];
                        farIndex++;
                    }
                }
            }
        }

        internal void UpdateArrangeRect(double top, double height, double areaWidth, List<double> nearSizes, List<double> farSizes)
        {
            int nearIndex = 0;
            int farIndex = 0;
            bool isOpposedFirstElement = true;
            bool isFirstElement = true;
            double nearTotalSize = nearSizes.Sum();
            double farTotalSize = farSizes.Sum();
            double innerPadding = 0;
            double axisHeight = 0;
            double axisTop = 0;
            int actualRowIndex = 0, elementRowIndex = 0;

            for (int i = 0; i < Axis.Count; i++)
            {
                ChartAxis element = Axis[i];
                if (element != null)
                {
                    //Set RowSpan height and top value
                    if (element.Area != null)
                    {
                        elementRowIndex = element.Area.GetActualRow(element);
                        actualRowIndex = element.Area.RowDefinitions.IndexOf(this);
                        CalcRowSpanAxisWidthandTop(top, element.Area.GetActualRowSpan(element), height, element, out axisTop, out axisHeight);
                    }

                    Size desiredSize = element.ComputedDesiredSize;
                    try
                    {
                        if (element.ShowAxisNextToOrigin && element.Area != null &&
                            element.Area.ColumnDefinitions.Count <= 1)
                        {
                            bool isContinue = false;
                            ArragneRectBasedOnOrigin(element, elementRowIndex, actualRowIndex, axisTop, areaWidth, axisHeight, ref isContinue);
                            if (isContinue)
                                continue;
                        }

                        if (element.OpposedPosition)
                        {
                            innerPadding = isOpposedFirstElement ? element.InsidePadding : 0;
                            if (elementRowIndex == actualRowIndex)
                                element.ArrangeRect = new Rect(
                                    (areaWidth - farTotalSize) - innerPadding,
                                    axisTop,
                                    desiredSize.Width,
                                    axisHeight);
                            element.Measure(new Size(element.ArrangeRect.Width, element.ArrangeRect.Height));
                            farTotalSize -= farSizes[farIndex];
                            farIndex++;
                            isOpposedFirstElement = false;
                        }
                        else
                        {
                            innerPadding = isFirstElement ? element.InsidePadding : 0;
                            if (elementRowIndex == actualRowIndex)
                                element.ArrangeRect = new Rect(
                                    (nearTotalSize - desiredSize.Width) + innerPadding,
                                    axisTop,
                                    desiredSize.Width,
                                    axisHeight);
                            element.Measure(new Size(element.ArrangeRect.Width, element.ArrangeRect.Height));
                            nearTotalSize -= nearSizes[nearIndex];
                            nearIndex++;
                            isFirstElement = false;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            
        }

        internal void Arrange()
        {
            foreach (ChartAxis chartAxis in Axis)
            {
                Canvas.SetLeft(chartAxis, chartAxis.ArrangeRect.Left);
                Canvas.SetTop(chartAxis, chartAxis.ArrangeRect.Top);
            }

            RenderBorderLine();
        }

        internal void Dispose()
        {
            if (Legends != null)
            {
                Legends.Clear();
                Legends = null;
            }

            if (Axis != null)
            {
                Axis.Clear();
                Axis = null;
            }
        }

        #endregion

        #region Private Static Methods
        
        private static void OnRowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var row = d as ChartRowDefinition;

            if(row != null && row.Axis != null && row.Axis.Count > 0)
            {
                var area = row.Axis[0].Area;
                area.ScheduleUpdate();
            }
        }

        #endregion

        #region Private Methods

        private void MeasureRectBasedOnOrigin(ChartAxis content, List<double> nearSizes, List<double> farSizes, ref bool isContinue, bool isFirstLayout)
        {
            double value = 0d;
            ChartAxis chartAxis = null;
            if (content.Area.InternalSecondaryAxis == content)
            {
                value = content.Area.InternalPrimaryAxis.ValueToCoefficientCalc(content.Origin);
                chartAxis = content.Area.InternalPrimaryAxis;
            }
            else if (content.Area.InternalSecondaryAxis.Orientation == Orientation.Horizontal &&
                     content.Area.InternalPrimaryAxis == content)
            {
                value = content.Area.InternalSecondaryAxis.ValueToCoefficientCalc(content.Origin);
                chartAxis = content.Area.InternalSecondaryAxis;
            }
            Size desiredSize = content.ComputedDesiredSize;
            if ((content.Origin != 0 && isFirstLayout) || 0 < value && 1 > value)
            {
                if (content.HeaderPosition == AxisHeaderPosition.Far)
                {
                    var isOpposed = content.OpposedPosition;
                    double position = isOpposed ? (1 - value) * (content.Area.SeriesClipRect.Width - (chartAxis.ActualPlotOffset * 2)) + chartAxis.ActualPlotOffset + content.InsidePadding :
                    value * (content.Area.SeriesClipRect.Width - (chartAxis.ActualPlotOffset * 2)) + chartAxis.ActualPlotOffset + content.InsidePadding;
                    if (isOpposed && position > desiredSize.Width - content.headerContent.DesiredSize.Height)
                    {
                        farSizes.Add(content.headerContent.DesiredSize.Height + headerMargin);
                    }
                    else if (position > desiredSize.Width - content.headerContent.DesiredSize.Height)
                    {
                        nearSizes.Add(content.headerContent.DesiredSize.Height + headerMargin);
                    }
                    isContinue = true;
                }
                else
                    isContinue = true;
            }
        }

        //Calculate row span height and top value
        private void CalcRowSpanAxisWidthandTop(double oldTop, int rowSpan, double oldHeight, ChartAxis axis, out double newTop, out double newHeight)
        {
            int row = axis.Area.GetActualRow(axis);
            if (axis.Area != null && rowSpan > 1 && row == axis.Area.RowDefinitions.IndexOf(this))
            {
                var rows = axis.Area.RowDefinitions;
                int j = rows.IndexOf(this), i = 0;
                newTop = 0;
                newHeight = 0;
                for (; j < rows.Count; j++)
                {
                    if (i < rowSpan)
                    {
                        newHeight += rows[j].computedHeight;
                        newTop = rows[j].ComputedTop;
                        i++;
                    }

                }
            }
            else
            {
                newTop = oldTop;
                newHeight = oldHeight;
            }
        }

        private void ArragneRectBasedOnOrigin(ChartAxis element, int elementRowIndex, int actualRowIndex, double axisTop, double areaWidth, double axisHeight, ref bool isContinue)
        {
            ChartAxis chartAxis = null;
            var desiredSize = element.ComputedDesiredSize;
            if (element.Area.InternalSecondaryAxis == element)
                chartAxis = element.Area.InternalPrimaryAxis;
            else if (element.Area.InternalSecondaryAxis.Orientation == Orientation.Horizontal
                     && element.Area.InternalPrimaryAxis == element)
                chartAxis = element.Area.InternalSecondaryAxis;
            if (chartAxis != null)
            {
                Rect rect = ChartLayoutUtils.Subtractthickness(new Rect(new Point(0, 0), element.AvailableSize), element.Area.AxisThickness);
                double value = chartAxis.ValueToCoefficientCalc(element.Origin);
                double plotOffset = chartAxis.ActualPlotOffset * 2;
                if (0 < value && 1 > value)
                {
                    var isOpposed = element.OpposedPosition;

                    if (isOpposed && elementRowIndex == actualRowIndex)
                    {
                        double position = (element.HeaderPosition == AxisHeaderPosition.Far) ? (1 - value) * (rect.Width - plotOffset) + chartAxis.ActualPlotOffset + element.InsidePadding : 0;
                        if (position > 0 && position > desiredSize.Width - element.headerContent.DesiredSize.Height)
                            element.ArrangeRect = new Rect(
                                ((rect.Width - plotOffset) * value)
                                - element.InsidePadding + chartAxis.ActualPlotOffset,
                                axisTop,
                                desiredSize.Width, 
                                axisHeight);
                        else
                            element.ArrangeRect = new Rect(
                                ((areaWidth - plotOffset) * value)
                                - element.InsidePadding + chartAxis.ActualPlotOffset,
                                axisTop,
                                desiredSize.Width, 
                                axisHeight);
                    }
                    else if (elementRowIndex == actualRowIndex)
                    {
                        double position = (element.HeaderPosition == AxisHeaderPosition.Far) ? value * (rect.Width - plotOffset) + chartAxis.ActualPlotOffset + element.InsidePadding : 0;
                        if (position > 0 && position > desiredSize.Width - element.headerContent.DesiredSize.Height)
                            element.ArrangeRect =
                           new Rect(
                               position - desiredSize.Width + element.headerContent.DesiredSize.Height
                                   + headerMargin,
                                    axisTop,
                                    desiredSize.Width,
                                    axisHeight);
                        else
                            element.ArrangeRect =
                                new Rect(
                                    ((areaWidth - plotOffset) * value) - desiredSize.Width
                                    + element.InsidePadding + chartAxis.ActualPlotOffset,
                                         axisTop,
                                         desiredSize.Width,
                                         axisHeight);
                    }
                    element.Measure(new Size(element.ArrangeRect.Width, element.ArrangeRect.Height));
                    isContinue = true;
                }
            }
        }

        private void RenderBorderLine()
        {
            if (BorderLine == null)
            {
                BorderLine = new Line();
                BindBorder(BorderLine);
            }
            if (Axis != null && this.Axis.Count > 0)
            {
                ChartAxis element = this.Axis.FirstOrDefault();
                if (element.Area != null)
                {
                    BorderLine.X1 = 0;
                    BorderLine.X2 = element.Area.SeriesClipRect.Width;
                    BorderLine.Y1 = BorderLine.Y2 = element.ArrangeRect.Top - element.Area.SeriesClipRect.Top;
                }
            }
        }

        private void BindBorder(UIElement element)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("BorderStroke");
            BindingOperations.SetBinding(element, Line.StrokeProperty, binding);

            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("BorderThickness");
            BindingOperations.SetBinding(element, Line.StrokeThicknessProperty, binding);
        }

        #endregion

        #endregion
    }
}
