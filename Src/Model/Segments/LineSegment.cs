using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Represents chart line segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="LineSeries"/>
   
    public partial class LineSegment : ChartSegment
    {
        #region Fields

        #region Private Fields

        bool isSegmentUpdated;

        Line line;

        DataTemplate CustomTemplate;

        private double x1, x2, y1, y2;

        private double _yData, _y1Data, _xData, _x1Data;

        ContentControl control;

        #endregion

        #endregion

        #region Constructor

        public LineSegment()
        {

        }

        /// <summary>
        /// Called when instance created for LineSegment with following arguments
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="lineSeries"></param>
        public LineSegment(double x1, double y1, double x2, double y2, AdornmentSeries lineSeries, object item)
        {
            base.Series = lineSeries;
            base.Item = item;
            SetData(x1, y1, x2, y2);
            var actualLineSeries = lineSeries as LineSeries;
            if (actualLineSeries != null)
                CustomTemplate = actualLineSeries.CustomTemplate;
        }

        /// <summary>
        /// Called when instance created for LineSegment 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="X2"></param>
        /// <param name="Y2"></param>
        public LineSegment(double x1, double y1, double X2, double Y2, object item)
        {

        }

        #endregion

        #region Properties

        #region Public Properties
        
        /// <summary>
        /// Gets or sets the start point x value.
        /// </summary>
        public double X1Value { get; set; }

        /// <summary>
        /// Gets or sets the start point y value.
        /// </summary>
        public double Y1Value { get; set; }

        /// <summary>
        /// Gets or sets the end point x value.
        /// </summary>
        public double X2Value { get; set; }

        /// <summary>
        /// Gets or sets the end point y value.
        /// </summary>
        public double Y2Value { get; set; }

        /// <summary>
        /// Gets or sets the end point(y) of the line.
        /// </summary>
        public double X1
        {
            get { return x1; }
            set
            {
                x1 = value;
                OnPropertyChanged("X1");
            }
        }

        /// <summary>
        /// Gets or sets the end point(x) of the line.
        /// </summary>
        public double X2
        {
            get { return x2; }
            set
            {
                x2 = value;
                OnPropertyChanged("X2");
            }
        }

        /// <summary>
        /// Gets or sets the start point(y) of the line.
        /// </summary>
        public double Y1
        {
            get { return y1; }
            set
            {
                y1 = value;
                OnPropertyChanged("Y1");
            }
        }

        /// <summary>
        /// Gets or sets the end point(y) of the line.
        /// </summary>
        public double Y2
        {
            get { return y2; }
            set
            {
                y2 = value;
                OnPropertyChanged("Y2");
            }
        }

        /// <summary>
        /// Gets or sets the end data point x value, for this segment.
        /// </summary>
        public double X1Data
        {
            get { return _x1Data; }
            set
            {
                _x1Data = value;
                OnPropertyChanged("X1Data");
            }
        }

        /// <summary>
        /// Gets or sets the start data point value, bind with x for this segment.
        /// </summary>
        public double XData
        {
            get { return _xData; }
            set
            {
                _xData = value;
                OnPropertyChanged("XData");
            }
        }

        /// <summary>
        /// Gets or sets the start data point value, bind with y for this segment.
        /// </summary>
        public double YData
        {
            get { return _yData; }
            set
            {
                _yData = value;
                OnPropertyChanged("YData");
            }
        }

        /// <summary>
        /// Gets or sets the end data point y value, for this segment.
        /// </summary>
        public double Y1Data
        {
            get { return _y1Data; }
            set
            {
                _y1Data = value;
                OnPropertyChanged("Y1Data");
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
            X1Value = Values[0];
            Y1Value = Values[1];
            X2Value = Values[2];
            Y2Value = Values[3];
            XData = Values[0];
            X1Data = Values[2];
            YData = Values[1];
            Y1Data = Values[3];
            XRange = new DoubleRange(X1Value, X2Value);
            YRange = new DoubleRange(Y1Value, Y2Value);

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
                line = new Line();
                line.DataContext = this;
                SetVisualBindings(line);
                line.Tag = this;
                line.StrokeEndLineCap = PenLineCap.Round;
                line.StrokeStartLineCap = PenLineCap.Round;
                return line;

            }
            else
            {
                control = new ContentControl();
                control.Content = this;
                control.Tag = this;
                control.ContentTemplate = CustomTemplate;
                return control;
            }
        }
        
        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>

        public override UIElement GetRenderedVisual()
        {
            if (CustomTemplate == null)
                return line;
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
            ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;

            if (cartesianTransformer != null)
            {
                if (isSegmentUpdated)
                    Series.SeriesRootPanel.Clip = null;
                double xStart = cartesianTransformer.XAxis.VisibleRange.Start;
                double xEnd = cartesianTransformer.XAxis.VisibleRange.End;
                double xBase = cartesianTransformer.XAxis.IsLogarithmic
                    ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase
                    : 1;
                bool xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
                double left = xIsLogarithmic ? Math.Log(X1Value, xBase) : X1Value;
                double right = xIsLogarithmic ? Math.Log(X2Value, xBase) : X2Value;
                if ((left <= xEnd && right >= xStart) &&
                    ((!double.IsNaN(Y1Value) && !double.IsNaN(Y2Value)) || Series.ShowEmptyPoints))
                {
                    Point point1 = transformer.TransformToVisible(X1Value, Y1Value);
                    Point point2 = transformer.TransformToVisible(X2Value, Y2Value);

                    if (line != null)
                    {
                        line.X1 = point1.X;
                        line.Y1 = point1.Y;
                        line.X2 = point2.X;
                        line.Y2 = point2.Y;
                        line.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        control.Visibility = Visibility.Visible;
                        this.X1 = point1.X;
                        this.Y1 = point1.Y;
                        this.X2 = point2.X;
                        this.Y2 = point2.Y;
                    }
                }
                else if (CustomTemplate == null)
                {
                    line.ClearUIValues();
                    //WPF-16603 In Line series stroke is applied when there is emptypoint data
                    line.Visibility = Visibility.Collapsed;
                }
                else control.Visibility = Visibility.Collapsed;

                isSegmentUpdated = true;
            }
            else
            {
                ChartTransform.ChartPolarTransformer polarTransformer =
                    transformer as ChartTransform.ChartPolarTransformer;

                if (Series.ShowEmptyPoints || ((!double.IsNaN(Y1Value) && !double.IsNaN(Y2Value))))
                {
                    Point point1 = polarTransformer.TransformToVisible(X1Value, Y1Value);
                    Point point2 = polarTransformer.TransformToVisible(X2Value, Y2Value);

                    if (line != null)
                    {
                        line.Visibility = Visibility.Visible;
                        line.X1 = point1.X;
                        line.Y1 = point1.Y;
                        line.X2 = point2.X;
                        line.Y2 = point2.Y;
                    }
                    else
                    {
                        this.X1 = point1.X;
                        this.Y1 = point1.Y;
                        this.X2 = point2.X;
                        this.Y2 = point2.Y;
                    }
                }
                else if (line != null) //WPF-20827 - Polar series render wrongly
                    line.Visibility = Visibility.Collapsed;
            }
        }

        protected override void SetVisualBindings(Shape element)
        {
            base.SetVisualBindings(element);
            var polarSeries = Series as PolarRadarSeriesBase;
            if (polarSeries != null)
            {
                DoubleCollection seriesStrokeDashArray = polarSeries.StrokeDashArray;
                DoubleCollection seriesArrayCollection = new DoubleCollection();
                if (seriesStrokeDashArray != null && seriesStrokeDashArray.Count > 0)
                {
                    foreach (double value in seriesStrokeDashArray)
                        seriesArrayCollection.Add(value);
                    element.StrokeDashArray = seriesArrayCollection;
                }

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

        internal override void Dispose()
        {
            if(line != null)
            {
                line.Tag = null;
                line = null;
            }
            base.Dispose();
        }

        #endregion       
    }
}
