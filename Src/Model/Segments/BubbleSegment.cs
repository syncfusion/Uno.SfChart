using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Represents chart bubble segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="BubbleSeries"/>
  
    public partial class BubbleSegment:ChartSegment
    {
        #region Fields

        private double segmentRadius, size;

        private Ellipse ellipseSegment;

        private double xPos, yPos;

        private DataTemplate customTemplate;

        private ContentControl control;

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        /// <param name="size"></param>
        /// <param name="series"></param>
        public BubbleSegment(double xPos, double yPos, double size, BubbleSeries series)
        {
            this.segmentRadius = size;
            this.customTemplate = series.CustomTemplate;
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets the data point value, bind with x for this segment.
        /// </summary>
        public double XData
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the data point value, bind with y for this segment.
        /// </summary>
        public double YData
        {

            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the size of the bubble segment.
        /// </summary>
        /// <remarks>
        /// It will not render the segment as in given size, its based on the other segments(as proportionate).
        /// </remarks>
        public double Size
        {
            get { return size; }
            set
            {
                size = value;
                OnPropertyChanged("Size");
            }
        }

        /// <summary>
        /// Gets or sets the segment radius in units of pixels.
        /// </summary>
        public double SegmentRadius
        {
            get
            {
                return segmentRadius;
            }
            set
            {
                segmentRadius = value;
                OnPropertyChanged("SegmentRadius");
            }
        }

        /// <summary>
        /// Gets or sets the x position of the segment rect.
        /// </summary>
        public double RectX
        {
            get { return xPos; }
            set
            {
                xPos = value;
                OnPropertyChanged("RectX");
            }
        }

        /// <summary>
        /// Gets or sets the y position of the segment rect.
        /// </summary>
        public double RectY
        {
            get { return yPos; }
            set
            {
                yPos = value;
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
            XData = Values[0];
            YData = Values[1];
            this.xPos = Values[0];
            this.yPos = Values[1];
            XRange = new DoubleRange(xPos, xPos);
            YRange = new DoubleRange(yPos, yPos);
        }

        /// <summary>
        /// Used for creating UIElement for rendering this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size">Size of the panel</param>
        /// <returns>
        /// retuns UIElement
        /// </returns>

        public override UIElement CreateVisual(Size size)
        {
            if (customTemplate == null)
            {
                ellipseSegment = new Ellipse();
                SetVisualBindings(ellipseSegment);
                ellipseSegment.Tag = this;
                return ellipseSegment;
            }
            control = new ContentControl { Content = this, Tag = this, ContentTemplate = customTemplate };
            return control;
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>

        public override UIElement GetRenderedVisual()
        {
            if (customTemplate == null)
                return ellipseSegment;
            return control;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>

        public override void Update(IChartTransformer transformer)
        {
            if (transformer != null)
            {
                ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
                double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
                bool xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
                double xStart = cartesianTransformer.XAxis.VisibleRange.Start;
                double xEnd = cartesianTransformer.XAxis.VisibleRange.End;
                double pos = xIsLogarithmic ? Math.Log(xPos, xBase) : xPos;

                if (pos >= xStart && pos <= xEnd && (!double.IsNaN(yPos) || Series.ShowEmptyPoints))
                {
                    Point point1 = transformer.TransformToVisible(xPos, yPos);

                    if (ellipseSegment != null)
                    {
                        ellipseSegment.Visibility = Visibility.Visible;
                        ellipseSegment.Height = ellipseSegment.Width = 2 * this.segmentRadius;
                        ellipseSegment.SetValue(Canvas.LeftProperty, point1.X - this.segmentRadius);
                        ellipseSegment.SetValue(Canvas.TopProperty, point1.Y - this.segmentRadius);
                    }
                    else
                    {
                        control.Visibility = Visibility.Visible;
                        RectX = point1.X - this.segmentRadius;
                        RectY = point1.Y - this.segmentRadius;
                        Size = SegmentRadius = 2 * this.segmentRadius;
                    }
                }
                else if (customTemplate == null)
                    ellipseSegment.Visibility = Visibility.Collapsed;
                else control.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size"></param>

        public override void OnSizeChanged(Size size)
        {

        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Method implementation for Set Binding to visual elements. 
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
            if(ellipseSegment != null)
            {
                ellipseSegment.Tag = null;
                ellipseSegment = null;
            }
            base.Dispose();
        }

        #endregion
    }
}
