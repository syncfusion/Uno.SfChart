using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart HiLo segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="HiLoSeries"/>
    public partial class HiLoSegment : ChartSegment
    {
        #region Fields

        #region Private Fields

        private double lowValue;

        private double highValue;

        private double xVal;

        private Line segLine;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for HiLoSegment
        /// </summary>
        /// <param name="xVal"></param>
        /// <param name="hghValue"></param>
        /// <param name="lwValue"></param>
        /// <param name="series"></param>
        public HiLoSegment(double xVal, double hghValue, double lwValue, HiLoSeries series, object item)
        {
            base.Series = series;
            base.Item = item;
            SetData(xVal, hghValue, lwValue);
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the high(top) value bind with this segment.
        /// </summary>
        public double High { get; set; }

        /// <summary>
        /// Gets or sets the low(top) value bind with this segment.
        /// </summary>
        public double Low { get; set; }

        /// <summary>
        /// Gets or sets the x value of the segment.
        /// </summary>
        public object XValue { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="Values"></param>
        public override void SetData(params double[] Values)
        {
            this.highValue = Values[1];
            this.lowValue = Values[2];
            this.xVal = Values[0];
            XRange = new DoubleRange(Values[0], Values[0]);
            if (!double.IsNaN(Values[1]) || !double.IsNaN(Values[2]))
                YRange = DoubleRange.Union(Values[1], Values[2]);
            else
                YRange = DoubleRange.Empty;

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
            segLine = new Line();
            segLine.Tag = this;
            SetVisualBindings(segLine);
            return segLine;
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>returns UIElement</returns>
        public override UIElement GetRenderedVisual()
        {
            return segLine;
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
                double xStart = cartesianTransformer.XAxis.VisibleRange.Start;
                double xEnd = cartesianTransformer.XAxis.VisibleRange.End;
                double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
                bool xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
                double edgeValue = xIsLogarithmic ? Math.Log(xVal, xBase) : xVal;
                if (edgeValue >= xStart && edgeValue <= xEnd && ((!double.IsNaN(highValue) && !double.IsNaN(lowValue)) || Series.ShowEmptyPoints))
                {
                    Point hipoint = transformer.TransformToVisible(xVal, highValue);
                    Point lopoint = transformer.TransformToVisible(xVal, lowValue);
                    segLine.X1 = hipoint.X;
                    segLine.Y1 = hipoint.Y;
                    segLine.X2 = lopoint.X;
                    segLine.Y2 = lopoint.Y;
                }
                else
                {
                    segLine.ClearUIValues();
                }
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

        internal override void Dispose()
        {
            if(segLine != null)
            {
                segLine.Tag = null;
                segLine = null;
            }
            base.Dispose();
        }

        #endregion
    }
}
