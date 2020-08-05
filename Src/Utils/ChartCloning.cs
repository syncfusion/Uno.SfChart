using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents a ChartCloning class implentation. 
    /// </summary>
    public static class ChartCloning
    {
        internal static void CloneControl(Control origianl, Control copy)
        {
            if (origianl != null&&copy!=null)
            {
                copy.DataContext = origianl.DataContext;
                copy.HorizontalAlignment = origianl.HorizontalAlignment;
                copy.Language = origianl.Language;
                copy.Background = origianl.Background;
                copy.BorderBrush = origianl.BorderBrush;
                copy.BorderThickness = origianl.BorderThickness;
                copy.FlowDirection = origianl.FlowDirection;
                copy.FontFamily = origianl.FontFamily;
                copy.FontSize = origianl.FontSize;
                copy.FontStretch = origianl.FontStretch;
                copy.FontStyle = origianl.FontStyle;
                copy.FontWeight = origianl.FontWeight;
                copy.Foreground = origianl.Foreground;
                copy.HorizontalContentAlignment = origianl.HorizontalContentAlignment;
                copy.IsEnabled = origianl.IsEnabled;
                copy.IsHitTestVisible = origianl.IsHitTestVisible;
                copy.IsTabStop = origianl.IsTabStop;
                copy.Language = origianl.Language;
                copy.Margin = origianl.Margin;
                copy.MaxHeight = origianl.MaxHeight;
                copy.MaxWidth = origianl.MaxWidth;
                copy.MinHeight = origianl.MinHeight;
                copy.MinWidth = origianl.MinWidth;
                copy.Opacity = origianl.Opacity;
                copy.Padding = origianl.Padding;
                copy.RenderTransform = origianl.RenderTransform;
                copy.RenderTransformOrigin = origianl.RenderTransformOrigin;
                copy.Style = origianl.Style;
                copy.TabIndex = origianl.TabIndex;
                copy.Tag = origianl.Tag;
                copy.Template = origianl.Template;
                copy.UseLayoutRounding = origianl.UseLayoutRounding;
                copy.VerticalAlignment = origianl.VerticalAlignment;
                copy.VerticalContentAlignment = origianl.VerticalContentAlignment;
                copy.Visibility = origianl.Visibility;
            }
        }
    }
}
