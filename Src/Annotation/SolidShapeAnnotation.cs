using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;

namespace Syncfusion.UI.Xaml.Charts
{
    public abstract partial class SolidShapeAnnotation : ShapeAnnotation
    {
        #region Dependency Property Registrations

        /// <summary>
        /// The DependencyProperty for <see cref="Angle"/> property.
        /// </summary>
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register(
                "Angle",
                typeof(double),
                typeof(SolidShapeAnnotation),
                new PropertyMetadata(0d, OnUpdatePropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ResizingMode"/> property.
        /// </summary>
        public static readonly DependencyProperty ResizingModeProperty =
            DependencyProperty.Register(
                "ResizingMode",
                typeof(AxisMode),
                typeof(SolidShapeAnnotation),
                new PropertyMetadata(AxisMode.All, OnResizingPathChanged));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the rotation angle for Annotation.
        /// </summary>
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the resizing direction for the annotation.
        /// </summary>
        /// <value>
        ///     <c>AxisMode.Horizontal</c> 
        ///     <c>AxisMode.Vertical</c> 
        ///     <c>AxisMode.All</c> 
        /// </value>
        public AxisMode ResizingMode
        {
            get { return (AxisMode)GetValue(ResizingModeProperty); }
            set { SetValue(ResizingModeProperty, value); }
        }

        #endregion

        #region Methods

        #region Private Static Methods

        private static void OnUpdatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var annotation = sender as Annotation;
            if (annotation != null) annotation.UpdatePropertyChanged(args);
        }

        private static void OnResizingPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var solidShapeAnnotation = d as SolidShapeAnnotation;
            if (e.NewValue != e.OldValue && solidShapeAnnotation.Chart != null)
                solidShapeAnnotation.UpdateResizingPath((AxisMode)e.NewValue);
        }

        #endregion

        #region Private Methods

        private void UpdateResizingPath(AxisMode path)
        {
            AnnotationResizer annotationResizer = Chart.AnnotationManager.AnnotationResizer;
            if (annotationResizer != null)
            {
                if (annotationResizer.ResizingMode != path)
                {
                    annotationResizer.ResizingMode = path;
                    annotationResizer.ResizerControl.ChangeView();
                }
            }
        }

        #endregion

        #endregion
    }
}
