using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents a column definition.
    /// </summary>
    /// <remarks>
    /// The width of the row can be defined either in terms of fixed pixels units mode or in auto adjust mode, by using <see cref="ChartColumnDefinition.Unit"/> property.
    /// </remarks>
    public partial class ChartColumnDefinition: DependencyObject, ICloneable
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="Width"/> property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(
                "Width",
                typeof(double), 
                typeof(ChartColumnDefinition),
                new PropertyMetadata(1d, OnColumnPropertyChanged));
        
        /// <summary>
        ///  The DependencyProperty for <see cref="Unit"/> property.
        /// </summary>
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register(
                "Unit",
                typeof(ChartUnitType),
                typeof(ChartColumnDefinition),
                new PropertyMetadata(ChartUnitType.Star, OnColumnPropertyChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="BorderThickness"/> property.
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(
                "BorderThickness", 
                typeof(double),
                typeof(
                ChartColumnDefinition),
                new PropertyMetadata(0d));
        
        /// <summary>
        /// The DependencyProperty for <see cref="BorderStroke"/> property.
        /// </summary>
        public static readonly DependencyProperty BorderStrokeProperty =
            DependencyProperty.Register(
                "BorderStroke", 
                typeof(Brush),
                typeof(ChartColumnDefinition),
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

        private double computedWidth = 0;

        private double computedLeft = 0;

        private List<ChartLegend> legends = new List<ChartLegend>();

        #endregion
        
        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Called when instance created for ChartColumnDefinition
        /// </summary>
        public ChartColumnDefinition()
        {
#if UNIVERSALWINDOWS
            BorderStroke = new SolidColorBrush(Colors.Red);
#endif
            axis = new List<ChartAxis>();
        }

        /// <summary>
        /// Gets or sets the width of this column.
        /// </summary>
        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets unit of the value to be specified for row width.
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
        /// Gets or sets the thickness of the column border.
        /// </summary>
        public double BorderThickness
        {
            get { return (double)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the border stroke.
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

        internal double ComputedWidth
        {
            get { return computedWidth; }
            set { computedWidth = value; }
        }
        
        internal double ComputedLeft
        {
            get { return computedLeft; }
            set { computedLeft = value; }
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
        /// Clone the column
        /// </summary>
        /// <returns></returns>
        public DependencyObject Clone()
        {
            return new ChartColumnDefinition()
            {
                BorderStroke = this.BorderStroke,
                BorderThickness = this.BorderThickness,
                Width = this.Width,
                Unit = this.Unit
            };
        }

        #endregion

        #region Internal Methods

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        internal void Measure(Size size, List<double> nearSizes, List<double> farSizes)
        {
            int nearIndex = 0;
            int farIndex = 0;

            bool isOpposedFirstElement = true;
            bool isFirstElement = true;
            double innerPadding = 0;
            double axisWidth = 0;


            foreach (ChartAxis content in axis)
            {
                if (content != null)
                {
                    int columnSpan = content.Area != null ? content.Area.GetActualColumnSpan(content) : 0;
                    axisWidth = CalcColumnSpanAxisWidth(size.Width, content, columnSpan);

                    if (content.Area != null && content.Area.GetActualColumn(content) == content.Area.ColumnDefinitions.IndexOf(this))
                        content.ComputeDesiredSize(new Size(axisWidth, size.Height));

                    if (content.ShowAxisNextToOrigin && content.Area != null && content.Area.RowDefinitions.Count <= 1)
                    {
                        bool isContinue = false;
                        MeasureRectBasedOnOrigin(content, nearSizes, farSizes, ref isContinue);
                        if (isContinue)
                            continue;
                    }
                    if (content.OpposedPosition)
                    {
                        innerPadding = isOpposedFirstElement ? content.InsidePadding : 0;
                        if (farSizes.Count <= farIndex)
                        {
                            farSizes.Add(content.ComputedDesiredSize.Height - innerPadding);
                        }
                        else if (farSizes[farIndex] < (content.ComputedDesiredSize.Height - innerPadding))
                        {
                            farSizes[farIndex] = content.ComputedDesiredSize.Height - innerPadding;
                        }
                        farIndex++;
                        isOpposedFirstElement = false;
                    }
                    else
                    {
                        innerPadding = isFirstElement ? content.InsidePadding : 0;
                        if (nearSizes.Count <= nearIndex)
                        {
                            nearSizes.Add(content.ComputedDesiredSize.Height - innerPadding);
                        }
                        else if (nearSizes[nearIndex] < (content.ComputedDesiredSize.Height - innerPadding))
                        {
                            nearSizes[nearIndex] = content.ComputedDesiredSize.Height - innerPadding;
                        }
                        nearIndex++;
                        isFirstElement = false;
                    }
                }
            }
            
        }

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
                    if (content.DockPosition == ChartDock.Top)
                    {
                        if (farSizes.Count <= farIndex)
                        {
                            farSizes.Add(content.DesiredSize.Height);
                        }
                        else if (farSizes[farIndex] < (content.DesiredSize.Height))
                        {
                            farSizes[farIndex] = content.DesiredSize.Height;
                        }
                        farIndex++;
                    }
                    else
                    {
                        if (nearSizes.Count <= nearIndex)
                        {
                            nearSizes.Add(content.DesiredSize.Height);
                        }
                        else if (nearSizes[nearIndex] < (content.DesiredSize.Height))
                        {
                            nearSizes[nearIndex] = content.DesiredSize.Height;
                        }
                        nearIndex++;
                    }
                }
            }
        }

        internal void UpdateLegendsArrangeRect(
            double left,
            double width, 
            double areaHeight,
            List<double> nearSizes,
            List<double> farSizes)
        {
            int nearIndex = 0;
            int farIndex = 0;
            double nearTotalSize = nearSizes.Sum();
            double farTotalSize = farSizes.Sum();
            double sum = nearTotalSize;
            double axisWidth = 0;
            for (int i = 0; i < Legends.Count; i++)
            {
                var element = this.Legends[i];
                if (element != null && element.DockPosition != ChartDock.Floating && element.LegendPosition == LegendPosition.Outside)
                {
                    // Set ColumnSpan width value
                    axisWidth = element.XAxis != null && element.ChartArea != null ? CalcColumnSpanAxisWidth(width, element.XAxis, element.ChartArea.GetActualColumnSpan(element)) : width;
                    var desiredSize = element.DesiredSize;
                    if (element.DockPosition == ChartDock.Bottom)
                    {
                        element.ArrangeRect = new Rect(
                            left,
                            (areaHeight + sum - nearTotalSize),
                            axisWidth,
                            desiredSize.Height);
                        if (nearIndex < nearSizes.Count)
                            nearTotalSize -= nearSizes[nearIndex];
                        nearIndex++;
                    }
                    else
                    {
                        element.ArrangeRect = new Rect(
                            left, 
                            (farTotalSize - desiredSize.Height),
                            axisWidth,
                            desiredSize.Height);
                        if (farIndex < farSizes.Count)
                            farTotalSize -= farSizes[farIndex];

                        farIndex++;
                    }
                }
            }
        }

        internal void UpdateArrangeRect(
            double left,
            double width,
            double areaHeight,
            List<double> nearSizes,
            List<double> farSizes)
        {
            int nearIndex = 0;
            int farIndex = 0;

            //To get the 0 left when the depth axis is set.
            bool isOpposedFirstElement = true;
            bool isFirstElement = true;
            double nearTotalSize = nearSizes.Sum();
            double farTotalSize = farSizes.Sum();
            double innerPadding = 0;
            double axisWidth = 0;
            int actualColumnIndex = 0;
            int elementColumnIndex = 0;


            for (int i = 0; i < Axis.Count; i++)
            {
                ChartAxis element = this.Axis[i];
                if (element != null)
                {
                    //Set ColumnSpan width value
                    if (element.Area != null)
                    {
                        axisWidth = CalcColumnSpanAxisWidth(width, element, element.Area.GetActualColumnSpan(element));
                        actualColumnIndex = element.Area.ColumnDefinitions.IndexOf(this);
                        elementColumnIndex = element.Area.GetActualColumn(element);
                    }
                    Size desiredSize = element.ComputedDesiredSize;
                    try
                    {
                        if (element.ShowAxisNextToOrigin && element.Area != null &&
                            element.Area.RowDefinitions.Count <= 1)
                        {
                            bool isContinue = false;
                            ArragneRectBasedOnOrigin(element, elementColumnIndex, actualColumnIndex, left, axisWidth, areaHeight, ref isContinue);
                            if (isContinue)
                                continue;
                        }

                        if (element.OpposedPosition)
                        {
                            innerPadding = isOpposedFirstElement ? element.InsidePadding : 0;
                            if (elementColumnIndex == actualColumnIndex)
                                element.ArrangeRect = new Rect(
                                    left,
                                    (farTotalSize - desiredSize.Height) + innerPadding,
                                    axisWidth,
                                    desiredSize.Height);
                            element.Measure(new Size(element.ArrangeRect.Width, element.ArrangeRect.Height));
                            farTotalSize -= farSizes[farIndex];
                            farIndex++;
                            isOpposedFirstElement = false;
                        }
                        else
                        {
                            innerPadding = isFirstElement ? element.InsidePadding : 0;
                            if (elementColumnIndex == actualColumnIndex)
                                element.ArrangeRect = new Rect(
                                    left,
                                    (areaHeight - nearTotalSize) - innerPadding,
                                    axisWidth,
                                    desiredSize.Height);
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

        private static void OnColumnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var column = d as ChartColumnDefinition;

            if (column != null && column.Axis != null && column.Axis.Count > 0)
            {
                var area = column.Axis[0].Area;
                area.ScheduleUpdate();
            }
        }

        #endregion

        #region Private Methods

        private void MeasureRectBasedOnOrigin(ChartAxis content, List<double> nearSizes, List<double> farSizes, ref bool isContinue)
        {
            double value = 0d;
            ChartAxis chartAxis = null;
            if (content.Area.InternalPrimaryAxis == content || content.Area.InternalDepthAxis == content)
            {
                value = content.Area.InternalSecondaryAxis.ValueToCoefficientCalc(content.Origin);
                chartAxis = content.Area.InternalSecondaryAxis;
            }
            else if (content.Area.InternalPrimaryAxis.Orientation == Orientation.Vertical
                     && content.Area.InternalSecondaryAxis == content)
            {
                value = content.Area.InternalPrimaryAxis.ValueToCoefficientCalc(content.Origin);
                chartAxis = content.Area.InternalPrimaryAxis;
            }
            Size desiredSize = content.ComputedDesiredSize;
            if (0 < value && 1 > value)
            {
                if (content.HeaderPosition == AxisHeaderPosition.Far)
                {
                    double position = content.OpposedPosition ? (1 - value) * (content.Area.SeriesClipRect.Height - (chartAxis.ActualPlotOffset * 2)) + content.ActualPlotOffset + content.InsidePadding :
                                      value * (content.Area.SeriesClipRect.Height - (chartAxis.ActualPlotOffset * 2)) + chartAxis.ActualPlotOffset + content.InsidePadding;
                    if (content.OpposedPosition && position > desiredSize.Height - content.headerContent.DesiredSize.Height)
                    {
                        farSizes.Add(content.headerContent.DesiredSize.Height + headerMargin);
                    }
                    else if (position > desiredSize.Height - content.headerContent.DesiredSize.Height)
                    {
                        nearSizes.Add(content.headerContent.DesiredSize.Height + headerMargin);
                    }
                    isContinue = true;
                }
                else
                    isContinue = true;

            }
        }



        //Calculate ColumnSpan width value
        private double CalcColumnSpanAxisWidth(double width, ChartAxis axis, int columnSpan)
        {
            if (axis.Area != null)
            {
                int column = axis.Area.GetActualColumn(axis);
                if (axis.Area != null && columnSpan > 1 && column == axis.Area.ColumnDefinitions.IndexOf(this))
                {
                    var cols = axis.Area.ColumnDefinitions;
                    int j = cols.IndexOf(this), i = 0;
                    width = 0;
                    for (; j < cols.Count; j++)
                    {
                        if (i < columnSpan)
                        {
                            width += cols[j].ComputedWidth;
                            i++;
                        }
                    }
                }
            }

            return width;
        }
        
        private void ArragneRectBasedOnOrigin(ChartAxis element, int elementColumnIndex, int actualColumnIndex, double actualLeft, double axisWidth, double areaHeight, ref bool isContinue)
        {
            ChartAxis chartAxis = null;
            var desiredSize = element.ComputedDesiredSize;
            if (element.Area.InternalPrimaryAxis == element || element.Area.InternalDepthAxis == element)
                chartAxis = element.Area.InternalSecondaryAxis;
            else if (element.Area.InternalPrimaryAxis.Orientation == Orientation.Vertical
                     && element.Area.InternalSecondaryAxis == element)
                chartAxis = element.Area.InternalPrimaryAxis;
            if (chartAxis != null)
            {
                double value = chartAxis.ValueToCoefficientCalc(element.Origin);
                double plotOffset = chartAxis.ActualPlotOffset * 2;
                if (0 < value && 1 > value)
                {
                    Rect rect = ChartLayoutUtils.Subtractthickness(new Rect(new Point(0, 0), element.AvailableSize), element.Area.AxisThickness);
                    if (element.OpposedPosition && elementColumnIndex == actualColumnIndex)
                    {

                        double headerHeight = element.headerContent.DesiredSize.Height;
                        double position = (element.HeaderPosition == AxisHeaderPosition.Far) ? (1 - value) * (rect.Height - plotOffset) + chartAxis.ActualPlotOffset + element.InsidePadding : 0;
                        if (position > 0 && position > desiredSize.Height - element.headerContent.DesiredSize.Height)
                            element.ArrangeRect = new Rect(
                                actualLeft,
                                position - desiredSize.Height +
                                headerHeight + headerMargin,
                                axisWidth, 
                                desiredSize.Height);
                        else
                            element.ArrangeRect = new Rect(
                                actualLeft,
                                (((areaHeight - plotOffset) * (1 - value)) -
                                desiredSize.Height) +
                                element.InsidePadding + chartAxis.ActualPlotOffset,
                                axisWidth, 
                                desiredSize.Height);
                    }
                    else if (elementColumnIndex == actualColumnIndex)
                    {
                        double position = (element.HeaderPosition == AxisHeaderPosition.Far) ? value * (rect.Height - (chartAxis.ActualPlotOffset * 2)) + chartAxis.ActualPlotOffset + element.InsidePadding : 0;
                        if (position > 0 && position > desiredSize.Height - element.headerContent.DesiredSize.Height)
                            element.ArrangeRect = new Rect(
                                actualLeft,
                                ((rect.Height - plotOffset) * (1 - value)) -
                                element.InsidePadding + chartAxis.ActualPlotOffset,
                                axisWidth,
                                desiredSize.Height);
                        else
                            element.ArrangeRect = new Rect(
                                actualLeft,
                                ((areaHeight - plotOffset) * (1 - value)) -
                                element.InsidePadding + chartAxis.ActualPlotOffset,
                                axisWidth,
                                desiredSize.Height);
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
            if (Axis != null && Axis.Count > 0)
            {
                ChartAxis element = this.Axis.FirstOrDefault();
                if (element.Area != null)
                {
                    BorderLine.X1 = BorderLine.X2 = element.ArrangeRect.Left - element.Area.SeriesClipRect.Left;
                    BorderLine.Y1 = 0;
                    BorderLine.Y2 = element.Area.SeriesClipRect.Height;
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
