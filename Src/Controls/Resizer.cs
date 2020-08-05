// <copyright file="Resizer.cs" company="Syncfusion. Inc">
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
    using System.Diagnostics;
    using System.Linq;
#if NETFX_CORE
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Documents;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
#endif

    /// <summary>
    /// Represents the <see cref="Resizer"/> class.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Apply Template overriding access modifier changes in wpf platform")]
    public sealed partial class Resizer : Control
    {
        #region Fields

        private Thumb resizeTopLeft, resizeMiddleLeft, resizeBottomLeft, resizeTopMiddle, resizeBottomMiddle, resizeBottomRight, resizeTopRight, resizeMiddleRight;

        private SfChart chart;

        private bool isSwapY = false, isSwapX = false, isAxis, isRotated = false;

        private double x1, y1, x2, y2;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Resizer"/> class.
        /// </summary>
        public Resizer()
        {
            this.DefaultStyleKey = typeof(Resizer);
        }

        #endregion

        #region Properties

        #region Internal Properties

        /// <summary>
        /// Gets or sets the annotation resizer.
        /// </summary>
        internal AnnotationResizer AnnotationResizer { get; set; }

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets the annotation resizer x axis.
        /// </summary>
        private ChartAxis XAxis
        {
            get { return AnnotationResizer.XAxis; }
        }

        /// <summary>
        /// Gets the annotation resizer y axis.
        /// </summary>
        private ChartAxis YAxis
        {
            get { return AnnotationResizer.YAxis; }
        }

        /// <summary>
        /// Gets or sets the actual x1 value.
        /// </summary>
        private double ActualX1
        {
            get
            {
                if (isAxis)
                    return chart.ValueToPointRelativeToAnnotation(XAxis, Annotation.ConvertData(AnnotationResizer.X1, XAxis));
                else
                    return Convert.ToDouble(AnnotationResizer.X1);
            }

            set
            {
                AnnotationResizer.X1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the actual x2 value.
        /// </summary>
        private double ActualX2
        {
            get
            {
                if (isAxis)
                    return chart.ValueToPointRelativeToAnnotation(XAxis, Annotation.ConvertData(AnnotationResizer.X2, XAxis));
                else
                    return Convert.ToDouble(AnnotationResizer.X2);
            }

            set
            {
                AnnotationResizer.X2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the actual y1 value.
        /// </summary>
        private double ActualY1
        {
            get
            {
                if (isAxis)
                    return chart.ValueToPointRelativeToAnnotation(YAxis, AnnotationResizer.ConvertData(AnnotationResizer.Y1, YAxis));
                else
                    return Convert.ToDouble(AnnotationResizer.Y1);
            }

            set
            {
                AnnotationResizer.Y1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the actual y2 value.
        /// </summary>
        private double ActualY2
        {
            get
            {
                if (isAxis)
                    return chart.ValueToPointRelativeToAnnotation(YAxis, AnnotationResizer.ConvertData(AnnotationResizer.Y2, YAxis));
                else
                    return Convert.ToDouble(AnnotationResizer.Y2);
            }

            set
            {
                AnnotationResizer.Y2 = value;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Internal Methods
        
        internal void Dispose()
        {
            chart = null;
            AnnotationResizer = null;
        }

        /// <summary>
        /// Changes the view.
        /// </summary>
        internal void ChangeView()
        {
            Resizer obj = AnnotationResizer.ResizerControl;
            if (obj != null)
            {
                if (AnnotationResizer.ResizingMode == AxisMode.Horizontal)
                {
                    obj.resizeBottomLeft.Visibility = Visibility.Collapsed;
                    obj.resizeBottomRight.Visibility = Visibility.Collapsed;
                    obj.resizeBottomMiddle.Visibility = Visibility.Collapsed;
                    obj.resizeMiddleLeft.Visibility = Visibility.Visible;
                    obj.resizeMiddleRight.Visibility = Visibility.Visible;
                    obj.resizeTopLeft.Visibility = Visibility.Collapsed;
                    obj.resizeTopMiddle.Visibility = Visibility.Collapsed;
                    obj.resizeTopRight.Visibility = Visibility.Collapsed;
                }
                else if (AnnotationResizer.ResizingMode == AxisMode.Vertical)
                {
                    obj.resizeBottomLeft.Visibility = Visibility.Collapsed;
                    obj.resizeBottomRight.Visibility = Visibility.Collapsed;
                    obj.resizeBottomMiddle.Visibility = Visibility.Visible;
                    obj.resizeMiddleLeft.Visibility = Visibility.Collapsed;
                    obj.resizeMiddleRight.Visibility = Visibility.Collapsed;
                    obj.resizeTopLeft.Visibility = Visibility.Collapsed;
                    obj.resizeTopMiddle.Visibility = Visibility.Visible;
                    obj.resizeTopRight.Visibility = Visibility.Collapsed;
                }
                else
                {
                    obj.resizeBottomLeft.Visibility = Visibility.Visible;
                    obj.resizeBottomRight.Visibility = Visibility.Visible;
                    obj.resizeBottomMiddle.Visibility = Visibility.Visible;
                    obj.resizeMiddleLeft.Visibility = Visibility.Visible;
                    obj.resizeMiddleRight.Visibility = Visibility.Visible;
                    obj.resizeTopLeft.Visibility = Visibility.Visible;
                    obj.resizeTopMiddle.Visibility = Visibility.Visible;
                    obj.resizeTopRight.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Maps the actual value to pixels.
        /// </summary>
        internal void MapActualValueToPixels()
        {
            if (ActualX1 < ActualX2)
            {
                x1 = ActualX1;
                x2 = ActualX2;
            }
            else
            {
                x1 = ActualX2;
                x2 = ActualX1;
            }

            if (ActualY1 < ActualY2)
            {
                y1 = ActualY1;
                y2 = ActualY2;
            }
            else
            {
                y1 = ActualY2;
                y2 = ActualY1;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Applies the templates for resizer.
        /// </summary>
#if NETFX_CORE
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            isAxis = AnnotationResizer.CoordinateUnit == CoordinateUnit.Axis;
            isRotated = isAxis && XAxis.Orientation != Orientation.Horizontal;
            resizeTopLeft = this.GetTemplateChild("resizeTopLeft") as Thumb;
            resizeMiddleLeft = this.GetTemplateChild("resizeMiddleRight") as Thumb;
            resizeBottomLeft = this.GetTemplateChild("resizeBottomLeft") as Thumb;
            resizeTopMiddle = this.GetTemplateChild("resizeTopMiddle") as Thumb;
            resizeBottomMiddle = this.GetTemplateChild("resizeBottomMiddle") as Thumb;
            resizeTopRight = this.GetTemplateChild("resizeTopRight") as Thumb;
            resizeMiddleRight = this.GetTemplateChild("resizeMiddleLeft") as Thumb;
            resizeBottomRight = this.GetTemplateChild("resizeBottomRight") as Thumb;
            resizeBottomLeft.DragDelta += ResizeBottomLeft_DragDelta;
            resizeBottomMiddle.DragDelta += ResizeBottomMiddle_DragDelta;
            resizeBottomRight.DragDelta += ResizeBottomRight_DragDelta;
            resizeMiddleLeft.DragDelta += ResizeMiddleLeft_DragDelta;
            resizeMiddleRight.DragDelta += ResizeMiddleRight_DragDelta;
            resizeTopLeft.DragDelta += ResizeTopLeft_DragDelta;
            resizeTopMiddle.DragDelta += ResizeTopMiddle_DragDelta;
            resizeTopRight.DragDelta += ResizeTopRight_DragDelta;
            
            resizeBottomLeft.DragCompleted += OnDragCompleted;
            resizeTopRight.DragCompleted += OnDragCompleted;
            resizeTopMiddle.DragCompleted += OnDragCompleted;
            resizeTopLeft.DragCompleted += OnDragCompleted;
            resizeMiddleRight.DragCompleted += OnDragCompleted;
            resizeMiddleLeft.DragCompleted += OnDragCompleted;
            resizeBottomMiddle.DragCompleted += OnDragCompleted;
            resizeBottomRight.DragCompleted += OnDragCompleted;

            chart = AnnotationResizer.Chart;
            CheckCoordinateValue();
            MapActualValueToPixels();
            ChangeView();
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Updates the <see cref="Resizer"/> on drag completed.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Event Arguments</param>
        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            chart.AnnotationManager.RaiseDragCompleted();
        }

        /// <summary>
        /// Checks the co-ordinate value.
        /// </summary>
        private void CheckCoordinateValue()
        {
            if (ActualX1 > ActualX2)
                isSwapX = true;
            if (ActualY1 > ActualY2)
                isSwapY = true;
        }

        /// <summary>
        /// Updates the resizer bounds on resizing.
        /// </summary>
        /// <param name="horizontalChange">The Horizontal Changed Value</param>
        /// <param name="verticalChange">The Vertical Changed Value.</param>
        /// <param name="isLeft">Is Left Change</param>
        /// <param name="isTop">Is Top Change</param>
        private void Move(double horizontalChange, double verticalChange, bool isLeft, bool isTop)
        {
            if (isRotated)
            {
                double temp = horizontalChange;
                horizontalChange = verticalChange;
                verticalChange = temp;
            }

            x1 = isLeft ? (x1 + horizontalChange) < x2 ? (x1 + horizontalChange) : x1 : x1;
            x2 = !isLeft ? (x2 + horizontalChange) > x1 ? (x2 + horizontalChange) : x2 : x2;
            y1 = isTop ? (y1 + verticalChange) < y2 ? (y1 + verticalChange) : y1 : y1;
            y2 = !isTop ? (y2 + verticalChange) > y1 ? (y2 + verticalChange) : y2 : y2;
            MapPixelToActualValue();
        }

        /// <summary>
        /// Drag delta operations for the top resizing.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Drag Delta Event Arguments</param>
        private void ResizeTopRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            AnnotationResizer.IsResizing = true;
            Move(e.HorizontalChange, e.VerticalChange, isRotated, !isRotated);
        }

        /// <summary>
        /// Maps the co-ordinates values to points.
        /// </summary>
        private void MapPixelToActualValue()
        {
            double x1Value, x2Value, y1Value, y2Value;

            if (isSwapX)
            {
                x1Value = isAxis ? SfChart.PointToAnnotationValue(XAxis, isRotated ? new Point(0, x2) : new Point(x2, 0)) : x2;
                x2Value = isAxis ? SfChart.PointToAnnotationValue(XAxis, isRotated ? new Point(0, x1) : new Point(x1, 0)) : x1;
            }
            else
            {
                x1Value = isAxis ? SfChart.PointToAnnotationValue(XAxis, isRotated ? new Point(0, x1) : new Point(x1, 0)) : x1;
                x2Value = isAxis ? SfChart.PointToAnnotationValue(XAxis, isRotated ? new Point(0, x2) : new Point(x2, 0)) : x2;
            }

            if (isSwapY)
            {
                y2Value = isAxis ? SfChart.PointToAnnotationValue(YAxis, isRotated ? new Point(y1, 0) : new Point(0, y1)) : y1;
                y1Value = isAxis ? SfChart.PointToAnnotationValue(YAxis, isRotated ? new Point(y2, 0) : new Point(0, y2)) : y2;
            }
            else
            {
                y2Value = isAxis ? SfChart.PointToAnnotationValue(YAxis, isRotated ? new Point(y2, 0) : new Point(0, y2)) : y2;
                y1Value = isAxis ? SfChart.PointToAnnotationValue(YAxis, isRotated ? new Point(y1, 0) : new Point(0, y1)) : y1;
            }

            AnnotationManager annotationManager = chart.AnnotationManager;

            ShapeAnnotation annotation = annotationManager.CurrentAnnotation as ShapeAnnotation;

            AnnotationManager.SetPosition(
                annotationManager.PreviousPoints, 
                annotation.X1,
                annotation.X2, 
                annotation.Y1,
                annotation.Y2);

            AnnotationManager.SetPosition(annotationManager.CurrentPoints, x1Value, x2Value, y1Value, y2Value);
            annotationManager.RaiseDragStarted(); // Raise the DragStarted event
            annotationManager.RaiseDragDelta(); // Raise the DragDelta event

            if (!annotationManager.DragDeltaArgs.Cancel)
            {
                ActualX1 = x1Value;
                ActualY1 = y1Value;
                ActualX2 = x2Value;
                ActualY2 = y2Value;
            }
        }

        /// <summary>
        /// Drag delta operations for the top middle resizing.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Drag Delta Event Arguments</param>
        private void ResizeTopMiddle_DragDelta(object sender, DragDeltaEventArgs e)
        {
            AnnotationResizer.IsResizing = true;
            Move(0, e.VerticalChange, true, true);
        }

        /// <summary>
        /// Drag delta operations for the top left resizing.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Drag Delta Event Arguments</param>
        private void ResizeTopLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            AnnotationResizer.IsResizing = true;
            Move(e.HorizontalChange, e.VerticalChange, true, true);
        }

        /// <summary>
        /// Drag delta operations for the middle right resizing.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Drag Delta Event Arguments</param>
        private void ResizeMiddleRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            AnnotationResizer.IsResizing = true;
            Move(e.HorizontalChange, 0, false, false);
        }

        /// <summary>
        /// Drag delta operations for the middle left resizing.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Drag Delta Event Arguments</param>
        private void ResizeMiddleLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            AnnotationResizer.IsResizing = true;
            Move(e.HorizontalChange, 0, true, true);
        }

        /// <summary>
        /// Drag delta operations for the bottom right resizing.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Drag Delta Event Arguments</param>
        private void ResizeBottomRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            AnnotationResizer.IsResizing = true;
            Move(e.HorizontalChange, e.VerticalChange, false, false);
        }

        /// <summary>
        /// Drag delta operations for the bottom middle resizing.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Drag Delta Event Arguments</param>
        private void ResizeBottomMiddle_DragDelta(object sender, DragDeltaEventArgs e)
        {
            AnnotationResizer.IsResizing = true;
            Move(0, e.VerticalChange, false, false);
        }

        /// <summary>
        /// Drag delta operations for the bottom left resizing.
        /// </summary>
        /// <param name="sender">The Sender Object</param>
        /// <param name="e">The Drag Delta Event Arguments</param>
        private void ResizeBottomLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            AnnotationResizer.IsResizing = true;
            Move(e.HorizontalChange, e.VerticalChange, !isRotated, isRotated);
        }

        #endregion

        #endregion
    }
}