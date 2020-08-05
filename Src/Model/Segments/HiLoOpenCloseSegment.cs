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
    /// Represents chart HiLoOpenClose segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="HiLoOpenCloseSeries"/> 
    public partial class HiLoOpenCloseSegment : ChartSegment
    {
        #region Fields

        #region Private Fields
        
        private Canvas canvas;

        private ChartPoint hipoint;

        private ChartPoint lowpoint;

        private ChartPoint sopoint;

        private ChartPoint eopoint;

        private ChartPoint scpoint;

        private ChartPoint ecpoint;

        private Line hiLoline;

        private Line closeLine;

        private Line openLine;

        private bool isBull;

        private Brush bullFillColor, bearFillColor;

        #endregion

        #endregion

        #region Constructor
        
        /// <summary>
        /// Constructor
        /// </summary>
        public HiLoOpenCloseSegment()
        {

        }

        /// <summary>
        /// Called when instance created for HiLoOpenCloseSegment
        /// </summary>
        /// <param name="hghpoint"></param>
        /// <param name="lowpoint"></param>
        /// <param name="sopoint"></param>
        /// <param name="eopoint"></param>
        /// <param name="scpoint"></param>
        /// <param name="ecpoint"></param>
        /// <param name="isbull"></param>
        /// <param name="series"></param>
        [Obsolete("Use HiLoOpenCloseSegment(ChartPoint point1, ChartPoint point2, ChartPoint point3, ChartPoint point4, ChartPoint point5, ChartPoint point6, bool isbull, HiLoOpenCloseSeries series, object item)")]
        public HiLoOpenCloseSegment(Point hghpoint, Point lowpoint, Point sopoint, Point eopoint, Point scpoint, Point ecpoint, bool isbull, HiLoOpenCloseSeries series, object item)
        {
            base.Series = series;
            base.Item = item;
        }

        public HiLoOpenCloseSegment(ChartPoint highpoint, ChartPoint lowpoint, ChartPoint sopoint, ChartPoint eopoint, ChartPoint scpoint, ChartPoint ecpoint, bool isbull, HiLoOpenCloseSeries series, object item)
        {
            base.Series = series;
            base.Item = item;
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets the actual color used to paint the interior of the segment.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush ActualInterior
        {
            get
            {
                if (Series.ActualArea.SelectedSeriesCollection.Contains(Series)
                    && Series.ActualArea.GetEnableSeriesSelection() && Series.ActualArea.GetSeriesSelectionBrush(Series) != null)
                    return Series.ActualArea.GetSeriesSelectionBrush(Series);
                else if ((Series as ISegmentSelectable).SegmentSelectionBrush != null && IsSegmentSelected())
                    return (Series as ISegmentSelectable).SegmentSelectionBrush;
                else
                    return this.Series.Interior != null ? Series.Interior : IsBull
                            ? BullFillColor : BearFillColor;
            }
        }

        /// <summary>
        /// Gets or sets the interior of the segment represents bear value.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush BearFillColor
        {
            get
            {
                return bearFillColor == null
                    ? this.Interior : bearFillColor;
            }
            set
            {
                if (bearFillColor != value)
                {
                    bearFillColor = value;
                    OnPropertyChanged("ActualInterior");
                }
            }
        }

        /// <summary>
        /// Gets or sets the interior of the segment represents bull value.
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush BullFillColor
        {
            get
            {
                return bullFillColor == null
                    ? this.Interior : bullFillColor;
            }
            set
            {
                if (bullFillColor != value)
                {
                    bullFillColor = value;
                    OnPropertyChanged("ActualInterior");
                }
            }
        }

        /// <summary>
        /// Gets or sets the high value of this segment.
        /// </summary>
        public double High { get; set; }

        /// <summary>
        /// Gets or sets the low value of this segment.
        /// </summary>
        public double Low { get; set; }

        /// <summary>
        /// Gets or sets the open value of this segment.
        /// </summary>
        public double Open { get; set; }

        /// <summary>
        /// Gets or sets the close value of this segment.
        /// </summary>
        public double Close { get; set; }

        #endregion

        #region Private Properties

        private bool IsBull
        {
            get
            {
                return isBull;
            }
            set
            {
                if (isBull != value)
                {
                    isBull = value;
                    OnPropertyChanged("ActualInterior");
                }
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
        /// <param name="hipoint"></param>
        /// <param name="lopoint"></param>
        /// <param name="sopoint"></param>
        /// <param name="eopoint"></param>
        /// <param name="scpoint"></param>
        /// <param name="ecpoint"></param>
        /// <param name="isbull"></param>

        [Obsolete("Use SetData(ChartPoint point1, ChartPoint point2, ChartPoint point3, ChartPoint point4, ChartPoint point5, ChartPoint point6, bool isbull)")]
        public override void SetData(Point hipoint, Point lopoint, Point sopoint, Point eopoint, Point scpoint, Point ecpoint, bool isbull)
        {
            this.hipoint.X = hipoint.X; this.hipoint.Y = hipoint.Y;
            this.lowpoint.X = lopoint.X; this.lowpoint.Y = lopoint.Y;
            this.sopoint.X = sopoint.X; this.sopoint.Y = sopoint.Y;
            this.eopoint.X = eopoint.X; this.eopoint.Y = eopoint.Y;
            this.scpoint.X = scpoint.X; this.scpoint.Y = scpoint.Y;
            this.ecpoint.X = ecpoint.X; this.ecpoint.Y = ecpoint.Y;
            this.IsBull = isbull;
            var alignedValues = AlignHiLoSegment(sopoint.Y, scpoint.Y, hipoint.Y, lopoint.Y);
            this.hipoint.Y = alignedValues[0];
            this.lowpoint.Y = alignedValues[1];
            XRange = new DoubleRange(ChartMath.Min(scpoint.X, ecpoint.X, sopoint.X, eopoint.X), ChartMath.Max(scpoint.X, ecpoint.X, sopoint.X, eopoint.X));
            YRange = new DoubleRange(lopoint.Y, hipoint.Y);
        }

        public override void SetData(ChartPoint hipoint, ChartPoint lopoint, ChartPoint sopoint, ChartPoint eopoint, ChartPoint scpoint, ChartPoint ecpoint, bool isbull)
        {
            this.hipoint = hipoint;
            this.lowpoint = lopoint;
            this.sopoint = sopoint;
            this.eopoint = eopoint;
            this.scpoint = scpoint;
            this.ecpoint = ecpoint;
            this.IsBull = isbull;
            var alignedValues = AlignHiLoSegment(sopoint.Y, scpoint.Y, hipoint.Y, lopoint.Y);
            this.hipoint.Y = alignedValues[0];
            this.lowpoint.Y = alignedValues[1];
            XRange = new DoubleRange(ChartMath.Min(scpoint.X, ecpoint.X, sopoint.X, eopoint.X), ChartMath.Max(scpoint.X, ecpoint.X, sopoint.X, eopoint.X));
            YRange = new DoubleRange(lopoint.Y, hipoint.Y);
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
            canvas = new Canvas();
            hiLoline = new Line();

            SetVisualBindings(hiLoline);
            canvas.Children.Add(hiLoline);

            openLine = new Line();
            SetVisualBindings(openLine);
            canvas.Children.Add(openLine);

            closeLine = new Line();
            SetVisualBindings(closeLine);
            canvas.Children.Add(closeLine);
            hiLoline.Tag = openLine.Tag = closeLine.Tag = this;
            return canvas;
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>

        public override UIElement GetRenderedVisual()
        {
            return canvas;
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

                double left = xIsLogarithmic ? Math.Log(XRange.Start, xBase) : XRange.Start;
                double right = xIsLogarithmic ? Math.Log(XRange.End, xBase) : XRange.End;

                double xStart = cartesianTransformer.XAxis.VisibleRange.Start;
                double xEnd = cartesianTransformer.XAxis.VisibleRange.End;

                if ((left <= xEnd && right >= xStart) || Series.ShowEmptyPoints)
                {
                    Point hiPoint = transformer.TransformToVisible(hipoint.X, hipoint.Y);
                    Point loPoint = transformer.TransformToVisible(lowpoint.X, lowpoint.Y);
                    Point startopenpoint = transformer.TransformToVisible(sopoint.X, sopoint.Y);
                    Point endopenpoint = transformer.TransformToVisible(eopoint.X, eopoint.Y);
                    Point startclosepoint = transformer.TransformToVisible(scpoint.X, scpoint.Y);
                    Point endclosepoint = transformer.TransformToVisible(ecpoint.X, ecpoint.Y);
                    if (!double.IsNaN(hipoint.Y) && !double.IsNaN(lowpoint.Y))
                    {
                        hiLoline.X1 = hiPoint.X;
                        hiLoline.Y1 = hiPoint.Y;
                        hiLoline.X2 = loPoint.X;
                        hiLoline.Y2 = loPoint.Y;
                    }
                    else
                        hiLoline.ClearUIValues();

                    if (!double.IsNaN(sopoint.Y) && !double.IsNaN(eopoint.Y))
                    {
                        this.openLine.X1 = startopenpoint.X;
                        this.openLine.Y1 = startopenpoint.Y;
                        this.openLine.X2 = endopenpoint.X;
                        this.openLine.Y2 = endopenpoint.Y;
                    }
                    else
                        openLine.ClearUIValues();

                    if (!double.IsNaN(scpoint.Y) && !double.IsNaN(ecpoint.Y))
                    {
                        this.closeLine.X1 = startclosepoint.X;
                        this.closeLine.Y1 = startclosepoint.Y;
                        this.closeLine.X2 = endclosepoint.X;
                        this.closeLine.Y2 = endclosepoint.Y;
                    }
                    else
                        closeLine.ClearUIValues();
                }
                else
                {
                    hiLoline.ClearUIValues();
                    openLine.ClearUIValues();
                    closeLine.ClearUIValues();
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

        #region Protected Override Methods

        /// <summary>
        /// Method Implementation for set Binding to ChartSegments properties.
        /// </summary>
        /// <param name="element"></param>
        protected override void SetVisualBindings(Shape element)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("ActualInterior");
            element.SetBinding(Shape.StrokeProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("StrokeThickness");
            element.SetBinding(Shape.StrokeThicknessProperty, binding);
        }
        
        /// <summary>
        /// Called when Property changed 
        /// </summary>
        /// <param name="name"></param>
        protected override void OnPropertyChanged(string name)
        {
            if (name == "Interior")
                name = "ActualInterior";
            base.OnPropertyChanged(name);
        }

        #endregion

        internal override void Dispose()
        {
            if (canvas != null)
            {
                canvas.Children.Clear();
                canvas = null;
            }
            if (openLine != null)
            {
                openLine.Tag = null;
                openLine = null;
            }
            if (closeLine != null)
            {
                closeLine.Tag = null;
                closeLine = null;
            }
            if (hiLoline != null)
            {
                hiLoline.Tag = null;
                hiLoline = null;
            }
            base.Dispose();
        }

        #endregion
    }
}
