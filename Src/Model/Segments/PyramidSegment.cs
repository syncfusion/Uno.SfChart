using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
using Linesegment = Windows.UI.Xaml.Media.LineSegment;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart pyramid segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="PyramidSeries"/>
    /// <seealso cref="FunnelSeries"/>
    /// <seealso cref="FunnelSegment"/>  
    public partial class PyramidSegment : ChartSegment
    {
        #region Fields
        
        #region Internal Fields

        internal bool isExploded;

        /// WP-1076[Data marker label position support for pyramid series]
        internal double height = 0d;

        #endregion

        #region Private Fields

        private double y = 0d, explodedOffset = 0d;
        
        private Path segmentPath;

        private PathGeometry segmentGeometry;

        private double xData;

        private double yData;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Defines the pyramid path
        /// </summary>
        /// <param name="y"></param>
        /// <param name="height"></param>
        /// <param name="explodedOffset"></param>
        /// <param name="series"></param>
        /// <param name="isExploded"></param>      
        public PyramidSegment(double y, double height, double explodedOffset, PyramidSeries series, bool isExploded)
        {
            base.Series = series;
            this.y = y;
            this.height = height;
            this.isExploded = isExploded;
            this.explodedOffset = explodedOffset;
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

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

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
            segmentPath = new Path();
            SetVisualBindings(segmentPath);
            segmentPath.Tag = this;
            return segmentPath;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>      
        public override void Update(IChartTransformer transformer)
        {
            if (!this.IsSegmentVisible)
                segmentPath.Visibility = Visibility.Collapsed;
            else
                segmentPath.Visibility = Visibility.Visible;

            Rect rect = new Rect(0, 0, transformer.Viewport.Width, transformer.Viewport.Height);
            PyramidSeries series = Series as PyramidSeries;
            if (rect.IsEmpty)
                this.segmentPath.Data = null;
            else
            {
                if (this.isExploded)
                {
                    rect.X += explodedOffset;
                }
                double top = y;
                double bottom = y + height;
                double topRadius = 0.5d * (1d - y);
                double bottomRadius = 0.5d * (1d - bottom);
                PathFigure figure = new PathFigure();
                figure.StartPoint = new Point(rect.X + topRadius * rect.Width, rect.Y + top * rect.Height);
                Linesegment lineSeg1 = new Linesegment();
                lineSeg1.Point = new Point(rect.X + (1 - topRadius) * rect.Width, rect.Y + top * rect.Height);
                figure.Segments.Add(lineSeg1);
                Linesegment lineSeg3 = new Linesegment();
                lineSeg3.Point = new Point(rect.X + (1 - bottomRadius) * rect.Width, rect.Y + bottom * rect.Height - series.StrokeThickness / 2);
                figure.Segments.Add(lineSeg3);
                Linesegment lineSeg4 = new Linesegment();
                lineSeg4.Point = new Point(rect.X + bottomRadius * rect.Width, rect.Y + bottom * rect.Height - series.StrokeThickness / 2);
                figure.Segments.Add(lineSeg4);
                figure.IsClosed = true;
                this.segmentGeometry = new PathGeometry();
                this.segmentGeometry.Figures = new PathFigureCollection() { figure };
                segmentPath.Data = segmentGeometry;

                height = ((bottom - top) * rect.Height) / 2;
            }
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>       
        public override UIElement GetRenderedVisual()
        {
            return segmentPath;
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
        /// Method Implementation for set Binding to PyramidSegments properties.
        /// </summary>
        /// <param name="element"></param>      
        protected override void SetVisualBindings(Shape element)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Interior");
            element.SetBinding(Shape.FillProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Stroke");
            element.SetBinding(Shape.StrokeProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("StrokeThickness");
            element.SetBinding(Shape.StrokeThicknessProperty, binding);
        }

        #endregion

        internal override void Dispose()
        {
            if(segmentPath != null)
            {
                segmentPath.Tag = null;
                segmentPath = null;
            }
            base.Dispose();
        }

        #endregion
    }
}
