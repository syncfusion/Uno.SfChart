using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace Syncfusion.UI.Xaml.Charts
{
    internal partial class AxisMarker : ShapeAnnotation
    {
        #region Fields

        #region Internal Fields

        internal ContentControl markerContent;

        #endregion

        #endregion

        #region Properties

        #region Internal Properties

        internal Canvas MarkerCanvas { get; set; }

        internal ShapeAnnotation ParentAnnotation { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        /// <summary>
        /// Updates the annotation
        /// </summary>
        public override void UpdateAnnotation()
        {
            if (markerContent != null)
            {
                Rect heightAndWidthRect;
                Point ensurePoint, positionedPoint;
                Size desiredSize;
                Point textPosition;
                ChartAxis axis = null;
                if (XAxis != null && YAxis != null && X1 != null && Y1 != null && Y2 != null && X1 != null)
                {
                    markerContent.Visibility = Visibility.Visible;
                    if (XAxis.Orientation == Orientation.Vertical)
                    {
                        if (XAxis is LogarithmicAxis && ParentAnnotation is VerticalLineAnnotation)
                        {
                            x1 = Convert.ToDouble(X1);
                            x2 = Convert.ToDouble(X2);
                        }
                        else
                        {
                            x1 = ConvertData(X1, XAxis);
                            x2 = ConvertData(X2, XAxis);
                        }

                        if (YAxis is LogarithmicAxis && ParentAnnotation is HorizontalLineAnnotation)
                        {
                            y1 = Convert.ToDouble(Y1);
                            y2 = Convert.ToDouble(Y2);
                        }
                        else
                        {
                            y1 = ConvertData(Y1, YAxis);
                            y2 = ConvertData(Y2, YAxis);
                        }
                    }
                    else
                    {
                        if (XAxis is LogarithmicAxis && ParentAnnotation is HorizontalLineAnnotation)
                        {
                            x1 = Convert.ToDouble(X1);
                            x2 = Convert.ToDouble(X2);
                        }
                        else
                        {
                            x1 = ConvertData(X1, XAxis);
                            x2 = ConvertData(X2, XAxis);
                        }

                        if (YAxis is LogarithmicAxis && ParentAnnotation is VerticalLineAnnotation)
                        {
                            y1 = Convert.ToDouble(Y1);
                            y2 = Convert.ToDouble(Y2);
                        }
                        else
                        {
                            y1 = ConvertData(Y1, YAxis);
                            y2 = ConvertData(Y2, YAxis);
                        }
                    }

                    Point point = (XAxis.Orientation == Orientation.Horizontal)
                        ? new Point(
                          this.Chart.ValueToPointRelativeToAnnotation(XAxis, x1),
                          this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1))
                        : new Point(
                          this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1),
                          this.Chart.ValueToPointRelativeToAnnotation(XAxis, x1));
                    Point point2 = (XAxis.Orientation == Orientation.Horizontal) 
                        ? new Point(
                          Chart.ValueToPointRelativeToAnnotation(XAxis, x2),
                          this.Chart.ValueToPointRelativeToAnnotation(YAxis, y2)) 
                        : new Point(
                          this.Chart.ValueToPointRelativeToAnnotation(YAxis, y2),
                          Chart.ValueToPointRelativeToAnnotation(XAxis, x2));
                    point.Y = (double.IsNaN(point.Y)) ? 0 : point.Y;
                    point.X = (double.IsNaN(point.X)) ? 0 : point.X;
                    point2.Y = (double.IsNaN(point2.Y)) ? 0 : point2.Y;
                    point2.X = (double.IsNaN(point2.X)) ? 0 : point2.X;

                    if (ParentAnnotation is VerticalLineAnnotation)
                    {
                        markerContent.Content = XAxis.Orientation == Orientation.Horizontal ? GetXAxisContent() : GetYAxisContent();
                        axis = (XAxis.Orientation == Orientation.Horizontal) ? XAxis : YAxis;
                    }
                    else
                    {
                        axis = (XAxis.Orientation == Orientation.Vertical) ? XAxis : YAxis;
                        markerContent.Content = XAxis.Orientation == Orientation.Vertical ? GetXAxisContent() : GetYAxisContent();
                    }

                    heightAndWidthRect = new Rect(point, point2);
                    ensurePoint = this.EnsurePoint(point, point2);
                    desiredSize = new Size(heightAndWidthRect.Width, heightAndWidthRect.Height);
                    positionedPoint = GetElementPosition(new Size(heightAndWidthRect.Width, heightAndWidthRect.Height), ensurePoint);
                    markerContent.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    if (ParentAnnotation is VerticalLineAnnotation)
                        textPosition = (ParentAnnotation as VerticalLineAnnotation).GetAxisLabelPosition(desiredSize, positionedPoint, new Size(markerContent.DesiredSize.Width, markerContent.DesiredSize.Height));
                    else
                        textPosition = (ParentAnnotation as HorizontalLineAnnotation).GetAxisLabelPosition(desiredSize, positionedPoint, new Size(markerContent.DesiredSize.Width, markerContent.DesiredSize.Height));

                    if (axis.Visibility != Visibility.Collapsed)
                    {
                        Canvas.SetLeft(markerContent, textPosition.X);
                        Canvas.SetTop(markerContent, textPosition.Y);
                    }
                    else
                    {
                        markerContent.Visibility = Visibility.Collapsed;
                    }

                    RotatedRect = new Rect(
                        textPosition.X, 
                        textPosition.Y,
                        markerContent.DesiredSize.Width, 
                        markerContent.DesiredSize.Height);
                }
                else
                {
                    markerContent.Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region Internal Methods

        internal override UIElement CreateAnnotation()
        {
            if (MarkerCanvas == null)
            {
                MarkerCanvas = new Canvas();
                markerContent = new ContentControl();
                markerContent.ContentTemplate = (ParentAnnotation as StraightLineAnnotation).AxisLabelTemplate != null ? (ParentAnnotation as StraightLineAnnotation).AxisLabelTemplate :
                    ChartDictionaries.GenericCommonDictionary["AxisLabel"] as DataTemplate;
                this.CanDrag = ParentAnnotation.CanDrag;
                SetBindings();
                MarkerCanvas.Children.Add(markerContent);
            }

            return MarkerCanvas;
        }

        #endregion

        #region Protected Methods

        protected override void SetBindings()
        {
        }

        #endregion

        #region Private Methods

        private static bool CheckPointRange(double point, ChartAxis axis)
        {
            return (point <= axis.VisibleRange.End && point >= axis.VisibleRange.Start);
        }

        object GetXAxisContent()
        {
            markerContent.Visibility = !CheckPointRange(x1, XAxis) ? Visibility.Collapsed : Visibility.Visible;
            return XAxis is NumericalAxis || XAxis is LogarithmicAxis ? Convert.ToDecimal(X1).ToString("0.##") : XAxis.GetLabelContent(x1);
        }

        object GetYAxisContent()
        {
            markerContent.Visibility = !CheckPointRange(y1, YAxis) ? Visibility.Collapsed : Visibility.Visible;
            return YAxis is NumericalAxis || YAxis is LogarithmicAxis ? Convert.ToDecimal(Y1).ToString("0.##") : XAxis.GetLabelContent(y1);
        }

        #endregion

        #endregion
    }
}
