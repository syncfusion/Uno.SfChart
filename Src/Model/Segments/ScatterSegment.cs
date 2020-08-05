using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI;
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
    /// Represents chart scatter segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="ScatterSeries"/>

    public partial class ScatterSegment : ChartSegment
    {
        #region Fields
        
        #region Internal Fields

        internal DataTemplate CustomTemplate;

        #endregion

        #region Protected Fields

        /// <summary>
        /// EllipseSegment property declarations
        /// </summary>
        protected Ellipse EllipseSegment;

        #endregion

        #region Private Fields

        private double yData, xData;

        private double scatterWidth;

        private double scatterHeight;

        private double xPos, yPos;

        private ContentControl control;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ScatterSegment()
        {

        }

        /// <summary>
        /// Called when instance created for Scattersegment
        /// </summary>
        /// <param name="xpos"></param>
        /// <param name="ypos"></param>
        /// <param name="series"></param>
        public ScatterSegment(double xpos, double ypos, ScatterSeries series)
        {
            CustomTemplate = series.CustomTemplate;
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
        /// Gets or sets the width of the scatter segment.
        /// </summary>
        public double ScatterWidth
        {
            get { return scatterWidth; }
            set
            {
                if (scatterWidth != value)
                {
                    scatterWidth = value;
                    OnPropertyChanged("ScatterWidth");
                }
            }
        }


        /// <summary>
        /// Gets or sets the height of the scatter segment.
        /// </summary>
        public double ScatterHeight
        {
            get { return scatterHeight; }
            set
            {
                if (scatterHeight != value)
                {
                    scatterHeight = value;
                    OnPropertyChanged("ScatterHeight");
                }
            }
        }


        /// <summary>
        /// Gets or sets the X position of the segment rect.
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
        /// Gets or sets the Y position of the segment rect.
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
            xPos = Values[0];
            yPos = Values[1];
            if (!double.IsNaN(xPos))
                XRange = DoubleRange.Union(xPos);
            else
                XRange = DoubleRange.Empty;
            if (!double.IsNaN(yPos))
                YRange = DoubleRange.Union(yPos);
            else
                YRange = DoubleRange.Empty;
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
            if (CustomTemplate == null)
            {
                EllipseSegment = new Ellipse();
                Binding binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("ScatterWidth");
                EllipseSegment.Tag = this;
                EllipseSegment.SetBinding(Ellipse.WidthProperty, binding);
                binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("ScatterHeight");
                EllipseSegment.SetBinding(Ellipse.HeightProperty, binding);
                EllipseSegment.Tag = this;
                SetVisualBindings(EllipseSegment);
                return EllipseSegment;
            }
            control = new ContentControl { Content = this, Tag = this, ContentTemplate = CustomTemplate };
            return control;
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>

        public override UIElement GetRenderedVisual()
        {
            if (CustomTemplate == null)
                return EllipseSegment;
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
            ChartTransform.ChartCartesianTransformer cartesianTransformer =
                transformer as ChartTransform.ChartCartesianTransformer;
            if (cartesianTransformer != null)
            {
                double xBase = cartesianTransformer.XAxis.IsLogarithmic
                    ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase
                    : 1;
                bool xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
                double xStart = cartesianTransformer.XAxis.VisibleRange.Start;
                double xEnd = cartesianTransformer.XAxis.VisibleRange.End;
                double edgeValue = xIsLogarithmic ? Math.Log(xPos, xBase) : xPos;
                if (edgeValue >= xStart && edgeValue <= xEnd && (!double.IsNaN(YData) || Series.ShowEmptyPoints))
                {
                    Point point1 = transformer.TransformToVisible(xPos, yPos);

                    var emptyPointSegment = this as EmptyPointSegment;
                    if (emptyPointSegment != null)
                    {
                        ScatterHeight = emptyPointSegment.EmptyPointSymbolHeight;
                        ScatterWidth = emptyPointSegment.EmptyPointSymbolWidth;
                    }
                    else
                    {
                        // To prevent the issues when scattersegment is used in other series.
                        var series = (Series as ScatterSeries);
                        if (series != null)
                        {
                            ScatterHeight = series.ScatterHeight;
                            ScatterWidth = series.ScatterWidth;
                        }
                    }

                    if (EllipseSegment != null)
                    {
                        EllipseSegment.SetValue(Canvas.LeftProperty, point1.X - ScatterWidth / 2);
                        EllipseSegment.SetValue(Canvas.TopProperty, point1.Y - ScatterHeight / 2);
                    }
                    else
                    {
                        control.Visibility = Visibility.Visible;
                        RectX = point1.X - (ScatterWidth / 2);
                        RectY = point1.Y - (ScatterHeight / 2);
                    }
                }
                else if (CustomTemplate == null)
                {
                    ScatterHeight = 0;
                    ScatterWidth = 0;
                }
                else
                    control.Visibility = Visibility.Collapsed;
            }
            else
            {
                ScatterWidth = 0;
                ScatterHeight = 0;
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
            if(EllipseSegment != null)
            {
                EllipseSegment.Tag = null;
                EllipseSegment = null;
            }
            if(control != null)
            {
                control.Tag = null;
                control = null;
            }
            base.Dispose();
        }

        #endregion
    }
}
