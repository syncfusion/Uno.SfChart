using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart bar segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="BarSeries"/>
   
    public partial class BarSegment : ChartSegment
    {
        #region Fields

        #region Protected Fields

        /// <summary>
        /// Variables declarations
        /// </summary>
        protected double Left = 0d, Top = 0d, Bottom = 0d, Right = 0d;

        /// <summary>
        /// barSegment variable declaration
        /// </summary>
        protected Rectangle barSegment;

        /// <summary>
        /// Variable declaration for SegmentCanvas
        /// </summary>
        protected Canvas SegmentCanvas;

        #endregion

        #region Internal Fields

        /// <summary>
        /// Variable declaration for segment width and height
        /// </summary>
        internal Size segmentSize;


        internal DataTemplate customTemplate;

        #endregion

        #region Private Fields

        private ContentControl control;

        private double rectX, rectY, width, height;

        private double yData, xData;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor 
        /// </summary>
        public BarSegment()
        {
            segmentSize = new Size();
        }

        /// <summary>
        /// Defines a Column Rect and Range
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="series"></param>
        public BarSegment(double x1, double y1, double x2, double y2, BarSeries series)
        {
            base.Series = series;
            SetData(x1, y1, x2, y2);
            customTemplate = series.CustomTemplate;
        }

        /// <summary>
        /// Called when instance created for BarSegment with following Parameters
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public BarSegment(double x1, double y1, double x2, double y2)
        {
            SetData(x1, y1, x2, y2);
        }

        #endregion

        #region Properties

        #region Public Properties
        
        /// <summary>
        /// Gets the data point value, bind with x for this segment.
        /// </summary>
        public double XData
        {
            get
            {
                return xData;
            }
            internal set
            {
                xData = value;
                OnPropertyChanged("XData");
            }
        }

        /// <summary>
        /// Gets the data point value, bind with y for this segment.
        /// </summary>
        public double YData
        {
            get
            {
                return yData;
            }
            internal set
            {
                yData = value;
                OnPropertyChanged("YData");
            }
        }

        /// <summary>
        /// Gets or sets the width of this segment
        /// </summary>
        public double Width
        {
            get { return width; }
            set 
            { 
                width = value; 
                OnPropertyChanged("Width"); 
            }
        }

        /// <summary>
        /// Gets or sets the height of this segment
        /// </summary>
        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                OnPropertyChanged("Height");
            }
        }

        /// <summary>
        /// Gets or sets the x position of the segment rect.
        /// </summary>
        public double RectX
        {
            get { return rectX; }
            set
            {
                rectX = value;
                OnPropertyChanged("RectX");
            }
        }

        /// <summary>
        /// Gets or sets the y position of the segment rect.
        /// </summary>
        public double RectY
        {
            get { return rectY; }
            set
            {
                rectY = value;
                OnPropertyChanged("RectY");
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="Values"></param>

        public override void SetData(params double[] Values)
        {
            Left = Values[0];
            Top = Values[1];
            Right = Values[2];
            Bottom = Values[3];
            XRange = new DoubleRange(Left, Right);
            YRange = new DoubleRange(Top, Bottom);
        }

        /// <summary>
        /// Used for creating UIElement for rendering this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="size">Size of the panel</param>
        /// <returns>
        /// returns UIElement
        /// </returns>
        public override UIElement CreateVisual(Size size)
        {
            if (customTemplate == null)
            {
                barSegment = new Rectangle();
                SetVisualBindings(barSegment);
                barSegment.Tag = this;
                return barSegment;
            }
            control = new ContentControl
            {
                Content = this,
                Tag = this,
                ContentTemplate = customTemplate
            };
            return control;
        }


        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>returns UIElement</returns>
        public override UIElement GetRenderedVisual()
        {
            if (customTemplate == null)
                return barSegment;
            return control;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Represents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        public override void Update(IChartTransformer transformer)
        {
            if (transformer != null)
            {
                ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
                double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
                bool xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
                double xStart = Math.Floor(cartesianTransformer.XAxis.VisibleRange.Start);
                double xEnd = Math.Ceiling(cartesianTransformer.XAxis.VisibleRange.End);
                double left = xIsLogarithmic ? Math.Log(Left, xBase) : Left;
                double right = xIsLogarithmic ? Math.Log(Right, xBase) : Right;

                if (!double.IsNaN(Top) && !double.IsNaN(Bottom) &&
                    (left <= xEnd && right >= xStart) &&
                    (!double.IsNaN(YData) || Series.ShowEmptyPoints))
                {
                    double spacing = (Series as ISegmentSpacing).SegmentSpacing;
                    Point tlpoint = transformer.TransformToVisible(Left, Top);
                    Point rbpoint = transformer.TransformToVisible(Right, Bottom);
                    rect = new Rect(tlpoint, rbpoint);
                    if (spacing > 0.0 && spacing <= 1)
                    {
                        if (Series.IsActualTransposed)
                        {
                            double leftpos = (Series as ISegmentSpacing).CalculateSegmentSpacing(spacing,
                                rect.Bottom, rect.Top);
                            rect.Y = leftpos;
                            segmentSize.Height = rect.Height = (1 - spacing) * rect.Height;
                        }
                        else
                        {
                            double leftpos = (Series as ISegmentSpacing).CalculateSegmentSpacing(spacing, rect.Right,
                                rect.Left);
                            rect.X = leftpos;
                            segmentSize.Width = rect.Width = (1 - spacing) * rect.Width;
                        }
                    }
                    else
                    {
                        segmentSize.Width = rect.Width;
                        segmentSize.Height = rect.Height;
                    }
                    if (barSegment != null)
                    {
                        barSegment.Height = segmentSize.Height = rect.Height;
                        barSegment.Width = segmentSize.Width = rect.Width;
                        barSegment.SetValue(Canvas.LeftProperty, rect.X);
                        barSegment.SetValue(Canvas.TopProperty, rect.Y);
                        barSegment.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        control.Visibility = Visibility.Visible;
                        RectX = rect.X;
                        RectY = rect.Y;
                        Width = rect.Width;
                        Height = rect.Height;
                    }
                }
                else if (customTemplate == null)
                    barSegment.Visibility = Visibility.Collapsed;
                else control.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="size"></param>
        public override void OnSizeChanged(Size size)
        {

        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Method Implementation for set Binding to ChartSegments properties.
        /// </summary>
        /// <param name="element"></param>
        protected override void SetVisualBindings(Shape element)
        {
            base.SetVisualBindings(element);
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Stroke");
            element.SetBinding(Shape.StrokeProperty, binding);
        }

        #endregion

        internal override void Dispose()
        {
            if(barSegment != null)
            {
                barSegment.Tag = null;
                barSegment = null;
            }
            base.Dispose();
        }

        #endregion
    }
}
