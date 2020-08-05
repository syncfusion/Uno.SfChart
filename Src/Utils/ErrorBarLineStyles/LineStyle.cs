using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Syncfusion.UI.Xaml.Charts
{
    public partial class LineStyle : DependencyObject
    {
        internal ChartSeriesBase Series;

        public LineStyle()
        {
            
        }
        
        public LineStyle(ChartSeriesBase series)
        {
            Series = series;
        }
        /// <summary>
        /// The DependencyProperty for <see cref="Stroke"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(LineStyle), 
            new PropertyMetadata(new SolidColorBrush(Colors.Cyan)));
        
        /// <summary>
        /// Gets or sets a stroke brush for the line
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush Stroke
        {
            get { return (Brush) GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeThickness"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(LineStyle), 
            new PropertyMetadata(2d));

        /// <summary>
        /// Gets or sets a stroke thickness for the line
        /// </summary>
        public double StrokeThickness
        {
            get { return (double) GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeDashCap"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeDashCapProperty =
            DependencyProperty.Register("StrokeDashCap", typeof(PenLineCap), typeof(LineStyle), 
            new PropertyMetadata(PenLineCap.Flat));

        /// <summary>
        /// Gets or sets a stroke dash cap for the line
        /// </summary>
        public PenLineCap StrokeDashCap
        {
            get { return (PenLineCap)GetValue(StrokeDashCapProperty); }
            set { SetValue(StrokeDashCapProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeEndLineCap"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeEndLineCapProperty =
            DependencyProperty.Register("StrokeEndLineCap", typeof(PenLineCap), typeof(LineStyle),
            new PropertyMetadata(PenLineCap.Flat));

        /// <summary>
        /// Gets or sets a stroke end cap for the line
        /// </summary>
        public PenLineCap StrokeEndLineCap
        {
            get { return (PenLineCap)GetValue(StrokeEndLineCapProperty); }
            set { SetValue(StrokeEndLineCapProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeLineJoin"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeLineJoinProperty =
            DependencyProperty.Register("StrokeLineJoin", typeof(PenLineJoin), typeof(LineStyle),
            new PropertyMetadata(PenLineJoin.Bevel));

        /// <summary>
        /// Gets or sets the line join for the stroke of the line.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.Media.PenLineJoin"/>
        /// </value>
        public PenLineJoin StrokeLineJoin
        {
            get { return (PenLineJoin)GetValue(StrokeLineJoinProperty); }
            set { SetValue(StrokeLineJoinProperty, value); }
        }
       
        /// <summary>
        /// The DependencyProperty for <see cref="StrokeMiterLimit"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeMiterLimitProperty =
            DependencyProperty.Register("StrokeMiterLimit", typeof(double), typeof(LineStyle), 
            new PropertyMetadata(1d));

        /// <summary>
        /// Gets or sets a limit on the ratio of the miter length to half the <see cref="StrokeThickness"/> of the shape.
        /// A Double that represents the distance within the dash pattern where a dash begins.
        /// </summary>
        /// <value>
        /// See <see cref="System.Windows.Shapes.StrokeMiterLimit"/> property.
        /// </value>
        public double StrokeMiterLimit
        {
            get { return (double)GetValue(StrokeMiterLimitProperty); }
            set { SetValue(StrokeMiterLimitProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeDashOffset"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeDashOffsetProperty =
            DependencyProperty.Register("StrokeDashOffset", typeof(double), typeof(LineStyle), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the stroke dash offset for the line.
        /// </summary>
        /// <value>
        /// The double value.
        /// </value>
        public double StrokeDashOffset
        {
            get { return (double)GetValue(StrokeDashOffsetProperty); }
            set { SetValue(StrokeDashOffsetProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for <see cref="StrokeDashArray"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(LineStyle),
            new PropertyMetadata(null, OnPropertyChange));

        /// <summary>
        /// Gets or sets the stroke dash array for the line.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.Shapes.StrokeDashArray"/>.
        /// </value>
        public DoubleCollection StrokeDashArray
        {
            get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
            set { SetValue(StrokeDashArrayProperty, value); }
        }
        
        private static void OnPropertyChange(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {            
            var lineStyle = obj as LineStyle;
            if (lineStyle == null)
                return;

            var chartSeries = lineStyle.Series;
            if (chartSeries != null && e.NewValue != null)
            {
                foreach (var segment in chartSeries.Segments)
                {
                    var collection = (DoubleCollection)e.NewValue;
                    if (collection != null && collection.Count > 0)
                    {
                        var doubleCollection = new DoubleCollection();
                        var doubleCollection1 = new DoubleCollection();
                        foreach (var value in collection)
                        {
                            doubleCollection.Add(value);
                            doubleCollection1.Add(value);
                        }

                        var errorBarSeries = chartSeries as ErrorBarSeries;
                        var errorBarSegment = segment as ErrorBarSegment;

                        if (errorBarSeries != null && errorBarSegment != null)
                        {
                            if (lineStyle == errorBarSeries.HorizontalLineStyle)
                                errorBarSegment.HorLine.StrokeDashArray = doubleCollection;
                            if (lineStyle == errorBarSeries.HorizontalCapLineStyle)
                            {
                                errorBarSegment.HorRightCapLine.StrokeDashArray = doubleCollection;
                                errorBarSegment.HorLeftCapLine.StrokeDashArray = doubleCollection1;
                            }
                            if (lineStyle == errorBarSeries.VerticalLineStyle)
                                errorBarSegment.VerLine.StrokeDashArray = doubleCollection;
                            if (lineStyle == errorBarSeries.VerticalCapLineStyle)
                            {
                                errorBarSegment.VerBottomCapLine.StrokeDashArray = doubleCollection;
                                errorBarSegment.VerTopCapLine.StrokeDashArray = doubleCollection1;
                            }
                        }
                    }
                }
            }
        }
    }
}
