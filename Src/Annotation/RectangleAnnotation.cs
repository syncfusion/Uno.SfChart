using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.UI.Xaml.Charts
{
    public partial class RectangleAnnotation : SolidShapeAnnotation
    {
        #region Methods

        #region Internal Override Methods

        internal override UIElement CreateAnnotation()
        {
            if (AnnotationElement != null && AnnotationElement.Children.Count == 0)
            {
                shape = new Rectangle();
                shape.Tag = this;
                SetBindings();
                AnnotationElement.Children.Add(shape);
                TextElementCanvas.Children.Add(TextElement);
                AnnotationElement.Children.Add(TextElementCanvas);
            }

            return AnnotationElement;
        }

        #endregion

        #region Protected Override Methods

        protected override DependencyObject CloneAnnotation(Annotation annotation)
        {
            return base.CloneAnnotation(new RectangleAnnotation());
        }

        #endregion

        #endregion
    }
}
