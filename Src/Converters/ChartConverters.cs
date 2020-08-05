using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using Windows.UI;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Converts Visibility to Boolean value and vice-versa.
    /// </summary>
   
    public partial class VisibilityToBooleanConverter : IValueConverter
    {
        /// <summary>
        ///  Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Visibility val = (Visibility)value;
            if (val == Visibility.Visible)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Return back the value 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            bool val = (bool)value;
            if (val)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }
    }

    /// <summary>
    /// Converts Boolean value to Visibility value and vice-versa.
    /// </summary>
    public partial class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isVisible = (bool)value;
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns>The value to be passed to the source object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Visibility visibility = (Visibility)value;
            return visibility == Visibility.Visible ? true : false;
        }
    }

    /// <summary>
    /// Converts the angle value by series IsTransposed.
    /// </summary>
    public partial class ConnectorRotationAngleConverter : IValueConverter
    {
        private ChartSeriesBase series;

        /// <summary>
        /// Called when instance created for ConnectorRotationAngleConverter
        /// </summary>
        /// <param name="series"></param>
        public ConnectorRotationAngleConverter(ChartSeriesBase series)
        {
            this.series = series;
        }

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double angle = (double)value;
            if (double.IsNaN(angle))
            {
                if (series.IsActualTransposed)
                    return 0;
                else
                    return 90;
            }
            else
                return value;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns>The value to be passed to the source object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }

    /// <summary>
    /// Resolves the color of the series or segment dynamically.
    /// </summary>
    
    public partial class InteriorConverter : IValueConverter
    {
        private ChartSeriesBase series;

        public InteriorConverter()
        {

        }

        /// <summary>
        /// Called when instance created for InteriorConverter
        /// </summary>
        /// <param name="series"></param>
        public InteriorConverter(ChartSeriesBase series)
        {
            this.series = series;
        }

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int index = int.Parse(parameter.ToString());
            return ChartExtensionUtils.GetInterior(series, index);
        }

        /// <summary>
        ///  Modifies the target data before passing it to the source object
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }

    /// <summary>
    /// Resolves the SegmentSelectionBrush of the series dynamically.
    /// </summary>
    
    public partial class SegmentSelectionBrushConverter : IValueConverter
    {
        private ChartSeriesBase series;

        /// <summary>
        /// Called when instance created for InteriorConverter
        /// </summary>
        /// <param name="series"></param>
        public SegmentSelectionBrushConverter(ChartSeriesBase series)
        {
            this.series = series;
        }

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
                return value;
            
            if(series !=null)
            { 
                if (series.Interior != null)
                    return series.Interior;
                else if (series.Palette != ChartColorPalette.None)
                {
                    int segmentIndex = int.Parse(parameter.ToString());
                    if (segmentIndex != -1 && series.ColorModel != null)
                        return series.ColorModel.GetBrush(segmentIndex);
                }
                else if (series.ActualArea != null
                    && series.ActualArea.Palette != ChartColorPalette.None && series.ActualArea.ColorModel != null)
                {
                    int serIndex = series.ActualArea.GetSeriesIndex(series);
                    if (serIndex >= 0)
                        return series.ActualArea.ColorModel.GetBrush(serIndex);
                    else if (series.ActualArea is SfChart)
                    {
                        serIndex = (series.ActualArea as SfChart).TechnicalIndicators.IndexOf(series as ChartSeries);
                        return series.ActualArea.ColorModel.GetBrush(serIndex);
                    }
                }
            }
            
            return null;
        }

        /// <summary>
        ///  Modifies the target data before passing it to the source object
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }

    /// <summary>
    /// Resolves the SeriesSelectionBrush of the series dynamically.
    /// </summary>
   
    public partial class SeriesSelectionBrushConverter : IValueConverter
    {
        private ChartSeriesBase series;

        /// <summary>
        /// Called when instance created for InteriorConverter
        /// </summary>
        /// <param name="series"></param>
        public SeriesSelectionBrushConverter(ChartSeriesBase series)
        {
            this.series = series;
        }

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (series != null)
            {
                Brush brush=series.ActualArea.GetSeriesSelectionBrush(series);
                if (brush != null)
                    return brush;
                else if (series.Interior != null)
                    return series.Interior;
                else if (series.Palette != ChartColorPalette.None)
                {
                    int segmentIndex = int.Parse(parameter.ToString());
                    if (segmentIndex != -1 && series.ColorModel != null)
                        return series.ColorModel.GetBrush(segmentIndex);
                }
                else if (series.ActualArea != null
                    && series.ActualArea.Palette != ChartColorPalette.None && series.ActualArea.ColorModel != null)
                {
                    int serIndex = series.ActualArea.GetSeriesIndex(series);
                    if (serIndex >= 0)
                        return series.ActualArea.ColorModel.GetBrush(serIndex);
                    else if (series.ActualArea is SfChart)
                    {
                        serIndex = (series.ActualArea as SfChart).TechnicalIndicators.IndexOf(series as ChartSeries);
                        return series.ActualArea.ColorModel.GetBrush(serIndex);
                    }
                }
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        /// <summary>
        ///  Modifies the target data before passing it to the source object
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }

    /// <summary>
    /// Returns the brush to be used based on the <see cref="ChartSeriesBase.Interior"/> property value.
    /// </summary>
  
    public partial class MultiInteriorConverter : IValueConverter
    {
        /// <summary>
        ///  Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return parameter;
            return value;
        }

        /// <summary>
        ///  Modifies the target data before passing it to the source object
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }

    /// <summary>
    /// Returns the rotate angle.
    /// </summary>
    
    public partial class DragElementRotateConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value == true ? 180 : 0;
        }

        /// <summary>
        ///  Modifies the target data before passing it to the source object
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
    /// <summary>
    /// set the margin for windows phone legend icon
    /// </summary>
    public partial class LegendMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var visibility = (Visibility)value;
            if (visibility == Visibility.Visible)
                return new Thickness(-20, 0, 0, 0);
            else
                return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }

     public partial class LabelContentPathConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ChartAdornment adornment = value as ChartAdornment;
            if (adornment == null) return value;

            return adornment.GetTextContent();          
        }

        /// <summary>
        ///  Modifies the target data before passing it to the source object
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }

    /// <summary>
    /// Represents a converter that returns the brush to axis label. 
    /// </summary>
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    public partial class LabelBackgroundConverter : IValueConverter
    {
         /// <summary>
         /// Modifies the source data before passing it to the target for display in the UI.
         /// </summary>
         /// <param name="value"></param>
         /// <param name="targetType"></param>
         /// <param name="parameter"></param>
         /// <param name="language"></param>
         /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ChartAdornment adornment = value as ChartAdornment;

            if (adornment == null)
                return value;
            if (adornment.CanHideLabel)
                return new SolidColorBrush(Colors.Transparent);
            else if (adornment.Series.ActualArea.SelectedSeriesCollection.Contains(adornment.Series)
                && adornment.Series.ActualArea.GetSeriesSelectionBrush(adornment.Series) != null
                && adornment.Series.adornmentInfo.HighlightOnSelection && adornment.Series is ChartSeries
                && adornment.Series.ActualArea.GetEnableSeriesSelection()
                && (adornment.Series.adornmentInfo.UseSeriesPalette || adornment.Background != null))
                return adornment.Series.ActualArea.GetSeriesSelectionBrush(adornment.Series);
            else if (IsAdornmentSelected(adornment))
                return (adornment.Series as ISegmentSelectable).SegmentSelectionBrush;
            else if (adornment.Series.adornmentInfo.UseSeriesPalette && adornment.Background == null)
                return adornment.Interior;

            return adornment.Background;
        }

        /// <summary>
        /// Method used to get the given adornment is selected or not
        /// </summary>
        /// <param name="adornment"></param>
        /// <returns></returns>
        private static bool IsAdornmentSelected(ChartAdornment adornment)
        {
            return adornment.Series.SelectedSegmentsIndexes.Contains(adornment.Series.ActualData.IndexOf(adornment.Item))
                   && adornment.Series is ISegmentSelectable && adornment.Series.adornmentInfo.HighlightOnSelection
                   && (adornment.Series as ISegmentSelectable).SegmentSelectionBrush != null
                   && (adornment.Series.adornmentInfo.UseSeriesPalette || adornment.Background != null);
        }

         /// <summary>
         ///  Modifies the target data before passing it to the source object
         /// </summary>
         /// <param name="value"></param>
         /// <param name="targetType"></param>
         /// <param name="parameter"></param>
         /// <param name="language"></param>
         /// <returns></returns>
         /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }

    public partial class LabelBorderBrushConverter : IValueConverter
    {
         /// <summary>
         /// Modifies the source data before passing it to the target for display in the UI.
         /// </summary>
         /// <param name="value"></param>
         /// <param name="targetType"></param>
         /// <param name="parameter"></param>
         /// <param name="language"></param>
         /// <returns></returns>
         public object Convert(object value, Type targetType, object parameter, string language)
         {
            ChartAdornment adornment = value as ChartAdornment;
            if (adornment == null)
                return value;

            ChartSeriesBase series = adornment.Series;
            ChartBase area = series.ActualArea;

            var selectableSegment = series as ISegmentSelectable;
            if (area.SelectedSeriesCollection.Contains(series)
                && area.GetSeriesSelectionBrush(series) != null && series is ChartSeries
                && series.adornmentInfo.HighlightOnSelection && area.GetEnableSeriesSelection())
                return area.GetSeriesSelectionBrush(series);
            else if (series.SelectedSegmentsIndexes.Contains(series.ActualData.IndexOf(adornment.Item))
               && selectableSegment != null && series.adornmentInfo.HighlightOnSelection
               && selectableSegment.SegmentSelectionBrush != null)
                return selectableSegment.SegmentSelectionBrush;
            else
                return adornment.BorderBrush;
        }

         /// <summary>
         ///  Modifies the target data before passing it to the source object
         /// </summary>
         /// <param name="value"></param>
         /// <param name="targetType"></param>
         /// <param name="parameter"></param>
         /// <param name="language"></param>
         /// <returns></returns>
         /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }

    public partial class LabelForegroundConverter : IValueConverter
    {
        Brush foregroundBrush = null;

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ChartAdornment adornment = value as ChartAdornment;

            if (adornment == null)
            {
                return new SolidColorBrush(Colors.White);
            }
            if (adornment.Foreground == foregroundBrush)
            {
                return new SolidColorBrush(Colors.White);
            }

            return adornment.Foreground;
        }

        /// <summary>
        ///  Modifies the target data before passing it to the source object
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
