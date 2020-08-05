using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.UI.Xaml.Charts;
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart Histogram segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="HistogramSeries"/>
  
    public partial class HistogramSegment : ColumnSegment
    {
        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="series"></param>
        public HistogramSegment(double x1,double y1,double x2,double y2, HistogramSeries series):base(x1,y1,x2,y2)
        {
            base.Series = series;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal List<object> Data { get; set; }

        #endregion
    }

    /// <summary>
    /// Class implementation for HistogramDistributionSegment
    /// </summary>
   
    public partial class HistogramDistributionSegment : ChartSegment
    {
        #region Fields

        #region Internal Fields

        internal PointCollection distributionPoints;
        
        #endregion

        #region Private Fields

        private Polyline polyLine;

        private PointCollection Points;

        #endregion

        #endregion

        #region Constructor  

        /// <summary>
        /// Called when instance created for HistogramDistributionSegment
        /// </summary>
        /// <param name="distributionPoints"></param>
        /// <param name="series"></param>
        public HistogramDistributionSegment(PointCollection distributionPoints, HistogramSeries series)
        {
            base.Series = series;
            this.distributionPoints = distributionPoints;
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets the data point value, bind with x for this segment.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reviewed")]
        public double XData
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the data point value, bind with y for this segment.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reviewed")]
        public double YData
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the stroke color of normal distribution curve
        /// </summary>
        public Brush ActualStrokeColor
        {
            get
            {
                var color = (Series as HistogramSeries).CurveColor;
                return color == null ? Interior : color;
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
            polyLine = new Polyline();
            SetVisualBindings(polyLine);
            polyLine.Tag = this;
            return polyLine;
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>      
        public override UIElement GetRenderedVisual()
        {
            return polyLine;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>

        public override void Update(IChartTransformer transformer)
        {
            this.Points = new PointCollection();

            foreach (var item in distributionPoints)
            {
                Point point = transformer.TransformToVisible(item.X, item.Y);
                Points.Add(point);
            }
            this.polyLine.Points = Points;
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
        /// Method Implementation for set Binding to ChartSegments properties.
        /// </summary>
        /// <param name="element"></param>
        protected override void SetVisualBindings(Shape element)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("ActualStrokeColor");
            element.SetBinding(Shape.StrokeProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("StrokeThickness");
            element.SetBinding(Shape.StrokeThicknessProperty, binding);
        }

        #endregion

        #endregion
    }
}
