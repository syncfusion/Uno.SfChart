// <copyright file="PieSegment3D.cs" company="Syncfusion. Inc">
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
namespace Syncfusion.UI.Xaml.Charts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Windows.ApplicationModel;
    using Windows.Foundation;
    using Windows.UI.Xaml;

    /// <summary>
    /// Class implementation for PieSegment3D
    /// </summary>
    public partial class PieSegment3D : ChartSegment3D
    {
        #region Dependency Property Registratipon

        /// <summary>
        /// The DependencyProperty for <see cref="ActualStartValue"/> property.
        /// </summary>
        public static readonly DependencyProperty ActualStartValueProperty =
            DependencyProperty.Register(
                "ActualStartValue",
                typeof(double),
                typeof(PieSegment3D),
                new PropertyMetadata(0d));

        /// <summary>
        /// The DependencyProperty for <see cref="ActualEndValue"/> property.
        /// </summary>
        public static readonly DependencyProperty ActualEndValueProperty =
            DependencyProperty.Register(
                "ActualEndValue",
                typeof(double),
                typeof(PieSegment3D),
                new PropertyMetadata(0d, OnValuesChanged));

        #endregion

        #region Fields

        private const double DtoR = Math.PI / 180d;
        
        private readonly SfChart3D area;
        private readonly PieSeries3D series3D;

        private double inSideRadius;
        private double depth;
        private double radius;
        private int pieIndex;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PieSegment3D"/> class.
        /// </summary>
        public PieSegment3D()
        {
            Points = new List<Point>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PieSegment3D"/> class.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="center">The center.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="height">The height.</param>
        /// <param name="r">The r.</param>
        /// <param name="i">The i.</param>
        /// <param name="y">The y.</param>
        /// <param name="insideRadius">The inside radius.</param>
        public PieSegment3D(ChartSeries3D series, Vector3D center, double start, double end, double height, double r, int i, double y, double insideRadius)
        {
            Points = new List<Point>();

            if (series.ToggledLegendIndex.Contains(i))
                this.IsSegmentVisible = false;
            else
                this.IsSegmentVisible = true;
            Series = series3D = series as PieSeries3D;
            area = series.Area;
            Item = series.ActualData[i];
            StartValue = start;
            EndValue = end;
            depth = height;
            radius = r;
            if (series3D != null)
            {
                pieIndex = series3D.GetPieSeriesIndex();
            }

            Index = i;
            YData = y;
            Center = center;
            inSideRadius = insideRadius;
            if (series.CanAnimate) return;
            if (series3D.StartAngle < series3D.EndAngle)
            {
                ActualStartValue = start;
                ActualEndValue = end - start;
            }
            else
            {
                ActualStartValue = end;
                ActualEndValue = start - end;
            }
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the y value of this segment (data point).
        /// </summary>
        public double YData { get; set; }

        /// <summary>
        /// Gets or sets the x value of this segment (data point).
        /// </summary>
        public double XData { get; set; }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        internal int Index { get; set; }

        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        internal List<Point> Points { get; set; }

        /// <summary>
        /// Gets or sets the center.
        /// </summary>
        internal Vector3D Center { get; set; }

        /// <summary>
        /// Gets or sets the start value.
        /// </summary>
        internal double StartValue { get; set; }

        /// <summary>
        /// Gets or sets the end value.
        /// </summary>
        internal double EndValue { get; set; }

        /// <summary>
        /// Gets or sets the actual start value.
        /// </summary>
        /// <value>
        /// The actual start value.
        /// </value>
        internal double ActualStartValue
        {
            get { return (double)GetValue(ActualStartValueProperty); }
            set { SetValue(ActualStartValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the actual end value.
        /// </summary>
        /// <value>
        /// The actual end value.
        /// </value>
        internal double ActualEndValue
        {
            get { return (double)GetValue(ActualEndValueProperty); }
            set { SetValue(ActualEndValueProperty, value); }
        }

        #endregion

        #endregion

        #region Abstract
        /// <summary>
        /// Used for creating UIElement for rendering this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="size">Size of the panel</param>
        /// <returns>
        /// returns UI Element
        /// </returns>
        public override UIElement CreateVisual(Size size)
        {
            return null;
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>returns UIElement</returns>
        public override UIElement GetRenderedVisual()
        {
            return null;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Represents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        public override void Update(IChartTransformer transformer)
        {
            CreateSector();
        }

        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="size">The Size</param>
        public override void OnSizeChanged(Size size)
        {
        }

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="values">The PieSegment Values</param>
        public override void SetData(params double[] values)
        {
            StartValue = values[0];
            EndValue = values[1];
            depth = values[2];
            radius = values[3];
            YData = values[4];
            var center = new Vector3D(values[5], values[6], values[7]);
            Center = center;
            inSideRadius = values[8];
            if (series3D.StartAngle < series3D.EndAngle)
            {
                ActualStartValue = values[0];
                ActualEndValue = values[1] - values[0];
            }
            else
            {
                ActualStartValue = values[1];
                ActualEndValue = values[0] - values[1];
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Creates the sector.
        /// </summary>
        /// <returns>Returns the sector.</returns>
        internal Polygon3D[][] CreateSector()
        {
            Points.Clear();
            var count = (int)Math.Ceiling(ActualEndValue / 6d);
            if (count < 1d) return null;
            var res = new Polygon3D[4][];
            var f = ActualEndValue / count;

            var opts = new Point[count + 1];
            var ipts = new Point[count + 1];

            for (var i = 0; i < count + 1; i++)
            {
                var ox = (float)(Center.X + radius * Math.Cos((ActualStartValue + i * f) * DtoR));
                var oy = (float)(Center.Y + radius * Math.Sin((ActualStartValue + i * f) * DtoR));

                opts[i] = new Point(ox, oy);

                var ix = (float)(Center.X + inSideRadius * Math.Cos((ActualStartValue + i * f) * DtoR));
                var iy = (float)(Center.Y + inSideRadius * Math.Sin((ActualStartValue + i * f) * DtoR));

                ipts[i] = new Point(ix, iy);
                Points.Add(new Point(ox, oy));
            }

            var oplgs = new Polygon3D[count];

            #region outside rounded polygons

            for (var i = 0; i < count; i++)
            {
                Vector3D[] vts =
                {
                    new Vector3D(opts[i].X, opts[i].Y, 0),
                    new Vector3D(opts[i].X, opts[i].Y, depth),
                    new Vector3D(opts[i + 1].X, opts[i + 1].Y, depth),
                    new Vector3D(opts[i + 1].X, opts[i + 1].Y, 0)
                };

                oplgs[i] = new Polygon3D(vts, this, Index, Stroke, StrokeThickness, Interior);
                oplgs[i].CalcNormal(vts[0], vts[1], vts[2]);
                oplgs[i].CalcNormal();
            }

            res[1] = oplgs;
            #endregion

            #region inside rounded polygons for doughnut

            if (inSideRadius > 0)
            {
                var iplgs = new Polygon3D[count];

                for (int i = 0; i < count; i++)
                {
                    var vts = new[]
                    {
                        new Vector3D(ipts[i].X, ipts[i].Y, 0),
                        new Vector3D(ipts[i].X, ipts[i].Y, depth),
                        new Vector3D(ipts[i + 1].X, ipts[i + 1].Y, depth),
                        new Vector3D(ipts[i + 1].X, ipts[i + 1].Y, 0)
                    };

                    iplgs[i] = new Polygon3D(vts, this, Index, Stroke, StrokeThickness, Interior);
                    iplgs[i].CalcNormal(vts[0], vts[1], vts[2]);
                    iplgs[i].CalcNormal();
                }

                res[3] = iplgs;
            }
            #endregion

            #region front and backside polygons(similar 2D accumulation)

            var tvtxs = new List<Vector3D>();
            var bvtxs = new List<Vector3D>();

            for (int i = 0; i < count + 1; i++)
            {
                tvtxs.Add(new Vector3D(opts[i].X, opts[i].Y, 0));
                bvtxs.Add(new Vector3D(opts[i].X, opts[i].Y, depth));
            }

            if (inSideRadius > 0)
            {
                for (int i = count; i > -1; i--)
                {
                    tvtxs.Add(new Vector3D(ipts[i].X, ipts[i].Y, 0));
                    bvtxs.Add(new Vector3D(ipts[i].X, ipts[i].Y, depth));
                }
            }
            else
            {
                tvtxs.Add(Center);
                bvtxs.Add(new Vector3D(Center.X, Center.Y, depth));
            }

            var poly1 = new Polygon3D(tvtxs.ToArray(), this, Index, Stroke, StrokeThickness, Interior);
            poly1.CalcNormal(tvtxs.ToArray()[0], tvtxs.ToArray()[1], tvtxs.ToArray()[2]);
            poly1.CalcNormal();
            var poly2 = new Polygon3D(bvtxs.ToArray(), this, Index, Stroke, StrokeThickness, Interior);
            poly2.CalcNormal(bvtxs.ToArray()[0], bvtxs.ToArray()[1], bvtxs.ToArray()[2]);
            poly2.CalcNormal();
            res[0] = new[] { poly1, poly2 };
            #endregion

            #region Inside two polygons for every segments

            if (inSideRadius > 0)
            {
                Vector3D[] rvts =
                {
                    // To avoid overlap subtract 0.1 value for every segment start value(90 to 270 dgree)
                    // and add 0.1 value for (270-360 and 0-90 degree)
                    new Vector3D(opts[0].X, (ActualStartValue >= 0 && ActualStartValue <= 90) || (ActualStartValue >= 270 && ActualStartValue <= 360) ? opts[0].Y + 0.1 : opts[0].Y - 0.1, 0),
                    new Vector3D(opts[0].X, (ActualStartValue >= 0 && ActualStartValue <= 90) || (ActualStartValue >= 270 && ActualStartValue <= 360) ? opts[0].Y + 0.1 : opts[0].Y - 0.1, depth),
                    new Vector3D(ipts[0].X, ipts[0].Y, depth),
                    new Vector3D(ipts[0].X, ipts[0].Y, 0)
                };

                Vector3D[] lvts =
                {
                    new Vector3D(opts[count].X, opts[count].Y, 0),
                    new Vector3D(opts[count].X, opts[count].Y, depth),
                    new Vector3D(ipts[count].X, ipts[count].Y, depth),
                    new Vector3D(ipts[count].X, ipts[count].Y, 0)
                };

                var poly3 = new Polygon3D(rvts, this, Index, Stroke, StrokeThickness, Interior);
                poly3.CalcNormal(rvts[0], rvts[1], rvts[2]);
                poly3.CalcNormal();

                var poly4 = new Polygon3D(lvts, this, Index, Stroke, StrokeThickness, Interior);
                poly4.CalcNormal(lvts[0], lvts[1], lvts[2]);
                poly4.CalcNormal();

                res[2] = new[]
                {
                    poly3, poly4
                };
            }
            else
            {
                Vector3D[] rvts =
                {
                    new Vector3D(opts[0].X, (ActualStartValue >= 0 && ActualStartValue <= 90) || (ActualStartValue >= 270 && ActualStartValue <= 360) ? opts[0].Y + 0.1 : opts[0].Y - 0.1, 0),
                    new Vector3D(opts[0].X, (ActualStartValue >= 0 && ActualStartValue <= 90) || (ActualStartValue >= 270 && ActualStartValue <= 360) ? opts[0].Y + 0.1 : opts[0].Y - 0.1, depth),
                    new Vector3D(Center.X, Center.Y, depth),
                    new Vector3D(Center.X, Center.Y, 0)
                };

                Vector3D[] lvts =
                {
                    new Vector3D(opts[count].X, opts[count].Y, 0),
                    new Vector3D(opts[count].X, opts[count].Y, depth),
                    new Vector3D(Center.X, Center.Y, depth),
                    new Vector3D(Center.X, Center.Y, 0)
                };

                var poly5 = new Polygon3D(rvts, this, Index, Stroke, StrokeThickness, Interior);
                poly5.CalcNormal(rvts[0], rvts[1], rvts[2]);
                poly5.CalcNormal();

                var poly6 = new Polygon3D(lvts, this, Index, Stroke, StrokeThickness, Interior);
                poly6.CalcNormal(lvts[0], lvts[1], lvts[2]);
                poly6.CalcNormal();

                res[2] = new[]
                {
                   poly5,
                   poly6
                };
            }
            #endregion
            return res;
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Updates the segment when Y value changed.
        /// </summary>
        /// <param name="d">The Dependency Object</param>
        /// <param name="e">The Event Arguments.</param>
        private static void OnValuesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var segment = (d as PieSegment3D);
            if (segment != null)
            {
                segment.ScheduleRender();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Renders the segment at the given schedules.
        /// </summary>
        private void ScheduleRender()
        {
#if NETFX_CORE
            if (DesignMode.DesignModeEnabled)
                OnValuesChanged();
#endif
        }

        /// <summary>
        /// Updates the segment when <see cref="ActualEndValue"/> value changed.
        /// </summary>
        private void OnValuesChanged()
        {
            if (!series3D.EnableAnimation) return;
            var indexOf = series3D.Segments.IndexOf(this);
            var polygons = area.Graphics3D.GetVisual();
            var items = polygons.Where(item => item.Tag == this);
            foreach (var item in items.ToList())
            {
                polygons.Remove(item);
            }

            if (indexOf != series3D.Segments.Count - 1) return;
            series3D.UpdateOnSeriesBoundChanged(Size.Empty);

            if (series3D.adornmentInfo != null)
            {
                var adornments = area.Graphics3D.GetVisual().OfType<UIElement3D>().ToList();
                foreach (var item in adornments)
                {
                    area.Graphics3D.Remove(item);
                    area.Graphics3D.AddVisual(item);
                }
            }

            if (pieIndex != 0) return;
            ScheduleView();
        }

        /// <summary>
        /// Updates the segment schedule on view changed.
        /// </summary>
        private void ScheduleView()
        {
#if NETFX_CORE
            if (DesignMode.DesignModeEnabled)
                OnViewChanged();
#endif
        }

        /// <summary>
        /// Updates the segment on view changed.
        /// </summary>
        private void OnViewChanged()
        {
            area.Graphics3D.PrepareView();
            area.Graphics3D.View(area.RootPanel);
        }

        #endregion

        #endregion
    }
}
