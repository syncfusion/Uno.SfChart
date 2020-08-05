using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;

namespace Syncfusion.UI.Xaml.Charts
{
    public abstract partial class StraightLineAnnotation : LineAnnotation
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="AxisLabelTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty AxisLabelTemplateProperty =
            DependencyProperty.Register(
                "AxisLabelTemplate",
                typeof(DataTemplate),
                typeof(StraightLineAnnotation),
                new PropertyMetadata(null, OnAxisLabelTemplateChanged));

        /// <summary>
        /// The Dependencyproperty for <see cref="ShowAxisLabel"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowAxisLabelProperty =
            DependencyProperty.Register(
                "ShowAxisLabel",
                typeof(bool),
                typeof(StraightLineAnnotation),
                new PropertyMetadata(false, OnShowAxisLabelChanged));

        #endregion

        #region Fields

        #region Internal Fields

        internal AxisMarker AxisMarkerObject = new AxisMarker();

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public StraightLineAnnotation()
        {
            AxisMarkerObject.ParentAnnotation = this;
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when annotation drag is started.
        /// </summary>
        public new event EventHandler DragStarted
        {
            add
            {
                this.AxisMarkerObject.DragStarted += value;
                this.AxisMarkerObject.ParentAnnotation.DragStarted += value;
            }

            remove
            {
                this.AxisMarkerObject.DragStarted -= value;
                this.AxisMarkerObject.ParentAnnotation.DragStarted -= value;
            }
        }

        /// <summary>
        /// Occurs while dragging the annotation.
        /// </summary>
        public new event EventHandler<AnnotationDragDeltaEventArgs> DragDelta
        {
            add
            {
                this.AxisMarkerObject.DragDelta += value;
                this.AxisMarkerObject.ParentAnnotation.DragDelta += value;
            }

            remove
            {
                this.AxisMarkerObject.DragDelta -= value;
                this.AxisMarkerObject.ParentAnnotation.DragDelta -= value;
            }
        }

        /// <summary>
        /// Occurs when annotation drag is completed.
        /// </summary>
        public new event EventHandler<AnnotationDragCompletedEventArgs> DragCompleted
        {
            add
            {
                this.AxisMarkerObject.DragCompleted += value;
                this.AxisMarkerObject.ParentAnnotation.DragCompleted += value;
            }

            remove
            {
                this.AxisMarkerObject.DragCompleted -= value;
                this.AxisMarkerObject.ParentAnnotation.DragCompleted -= value;
            }
        }

        /// <summary>
        /// Occurs when Syncfusion.UI.Xaml.Charts.Annotation becomes selected.
        /// </summary>
        public new event EventHandler Selected
        {
            add
            {
                this.AxisMarkerObject.Selected += value;
                this.AxisMarkerObject.ParentAnnotation.Selected += value;
            }

            remove
            {
                this.AxisMarkerObject.Selected -= value;
                this.AxisMarkerObject.ParentAnnotation.Selected -= value;
            }
        }

        /// <summary>
        /// Occurs when Syncfusion.UI.Xaml.Charts.Annotation becomes unselected.
        /// </summary>
        public new event EventHandler UnSelected
        {
            add
            {
                this.AxisMarkerObject.UnSelected += value;
                this.AxisMarkerObject.ParentAnnotation.UnSelected += value;
            }

            remove
            {
                this.AxisMarkerObject.UnSelected -= value;
                this.AxisMarkerObject.ParentAnnotation.UnSelected -= value;
            }
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the custom template for the axis label.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        public DataTemplate AxisLabelTemplate
        {
            get { return (DataTemplate)GetValue(AxisLabelTemplateProperty); }
            set { SetValue(AxisLabelTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable or disable the annotation label displaying in axis.
        /// </summary>
        public bool ShowAxisLabel
        {
            get { return (bool)GetValue(ShowAxisLabelProperty); }
            set { SetValue(ShowAxisLabelProperty, value); }
        }

        #endregion

        #region Internal Override Properties

        internal override SfChart Chart
        {
            get
            {
                return chart;
            }

            set
            {
                if (chart != null)
                    chart.SizeChanged -= OnChartSizeChanged;
                chart = value;
                if (chart != null)
                    chart.SizeChanged += OnChartSizeChanged;
                SetAxisFromName();
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Internal Override Methods

        internal override bool IsPointInsideRectangle(Point point)
        {
            return RotatedRect.Contains(point);
        }

        internal override void OnVisibilityChanged()
        {
            if (this.chart != null && this.chart.AnnotationManager != null)
            {
                this.IsVisbilityChanged = true;

                if (Visibility.Equals(Visibility.Collapsed))
                    this.chart.AnnotationManager.AddOrRemoveAnnotations(this, true);
                else
                    this.chart.AnnotationManager.AddOrRemoveAnnotations(this, false);
            }
        }

        #endregion

        #region Protected Methods

        protected void SetAxisMarkerValue(object X1, object X2, object Y1, object Y2, AxisMode axisMode)
        {
            AxisMarkerObject.X1 = X1;
            AxisMarkerObject.X2 = X2;
            AxisMarkerObject.Y1 = Y1;
            AxisMarkerObject.Y2 = Y2;
            AxisMarkerObject.XAxisName = this.XAxisName;
            AxisMarkerObject.YAxisName = this.YAxisName;
            AxisMarkerObject.YAxis = this.YAxis;
            AxisMarkerObject.XAxis = this.XAxis;
            AxisMarkerObject.DraggingMode = axisMode;
        }

        #endregion

        #region Private Static Methods

        private static void OnAxisLabelTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StraightLineAnnotation).OnAxisTemplateChanged(e.NewValue);
        }

        private static void OnShowAxisLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as StraightLineAnnotation).OnShowAxisLabelChanged(Convert.ToBoolean(e.NewValue));
        }

        #endregion

        #region Private Methods

        void OnChartSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.CoordinateUnit == Charts.CoordinateUnit.Pixel)
                UpdateAnnotation();
        }

        private void OnAxisTemplateChanged(object axisTemplate)
        {
            var markerContent = AxisMarkerObject.markerContent;

            if (markerContent == null) return;

            markerContent.ContentTemplate = axisTemplate == null ?
                ChartDictionaries.GenericCommonDictionary["AxisLabel"] as DataTemplate :
            axisTemplate as DataTemplate;
        }

        private void OnShowAxisLabelChanged(bool isShow)
        {
            if (isShow)
            {
                UpdateAnnotation();
            }
            else if (AxisMarkerObject != null)
            {
                Chart.AnnotationManager.AddOrRemoveAnnotations(AxisMarkerObject, true);
            }
        }

        #endregion

        #endregion
    }
}
