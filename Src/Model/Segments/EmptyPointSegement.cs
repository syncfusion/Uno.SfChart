using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart empty point segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <see cref="ChartSeriesBase.ShowEmptyPoints"/>   
    public partial class EmptyPointSegment : ScatterSegment
    {
        #region Fields

        #region Private Fields

        private double ypos;

        private double xpos;

        private double emptyPointSymbolHeight = 20;

        private double emptyPointSymbolWidth = 20;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for EmptyPointSegment with following arguments
        /// </summary>
        /// <param name="xData"></param>
        /// <param name="yData"></param>
        /// <param name="series"></param>
        /// <param name="isEmptyPointInterior"></param>
        public EmptyPointSegment(double xData, double yData, ChartSeriesBase series, bool isEmptyPointInterior)
        {
            base.Series = series;
            ScatterWidth = EmptyPointSymbolWidth;
            ScatterHeight = EmptyPointSymbolHeight;
            this.IsEmptySegmentInterior = isEmptyPointInterior;
            this.XData = xData;
            this.YData = yData;
            base.SetData(xData, yData);
        }

        #endregion
        
        #region Properties

        #region Public Properties
        
        /// <summary>
        /// Gets or sets empty point symbol height.
        /// </summary>    
        public double EmptyPointSymbolHeight
        {
            get
            {
                return emptyPointSymbolHeight;
            }
            set
            {
                emptyPointSymbolHeight = value;
            }
        }
        
        /// <summary>
        /// Gets or sets empty point symbol width.
        /// </summary>       
        public double EmptyPointSymbolWidth
        {
            get
            {
                return emptyPointSymbolWidth;
            }
            set
            {
                emptyPointSymbolWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the x coordinate of this segment.
        /// </summary>
        public double X
        {
            get
            {

                return xpos;
            }
            set
            {
                xpos = value;
                OnPropertyChanged("X");
            }
        }

        /// <summary>
        /// Gets or sets the y coordinate of this segment.
        /// </summary>       
        public double Y
        {
            get
            {

                return ypos;
            }
            set
            {
                ypos = value;
                OnPropertyChanged("Y");
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods
        
        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="point4"></param>
        [Obsolete("Use SetData(ChartPoint point1, ChartPoint point2, ChartPoint point3, ChartPoint point4)")]
        public override void SetData(Point point1, Point point2, Point point3, Point point4)
        {

        }

        public override void SetData(ChartPoint point1, ChartPoint point2, ChartPoint point3, ChartPoint point4)
        {

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
            if (Series.EmptyPointSymbolTemplate == null)
            {
                Ellipse ellipse = base.CreateVisual(size) as Ellipse;
                return ellipse;
            }
            else
            {
                ContentControl control = new ContentControl();
                control.Content = this;
                control.Width = EmptyPointSymbolWidth;
                control.Height = EmptyPointSymbolHeight;
                control.ContentTemplate = Series.EmptyPointSymbolTemplate;
                return control;
            }
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
                Point position = transformer.TransformToVisible(XData, YData);
                this.X = position.X - (EmptyPointSymbolWidth / 2);
                this.Y = position.Y - (EmptyPointSymbolHeight / 2);
            }
            if (Series.EmptyPointSymbolTemplate == null)
                base.Update(transformer);
        }

        #endregion

        #endregion
    }
}
