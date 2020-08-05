using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using Windows.Foundation;
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml.Media;
using Windows.UI;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Contains utility methods to manipulate data.
    /// </summary>
  
    public partial class ChartDataUtils
    {
        internal static object GetPropertyDescriptor(object obj, string path)
        {
            IPropertyAccessor propertyAccessor = null;
            if (path.Contains(".") || path.Contains("["))
            {
                if (path.Contains("."))
                {
                    string[] childProperties = path.Split('.');
                    int i = 0;
                    object parentObj = obj;
                    while (i != childProperties.Length)
                    {
                        var xPropertyInfo = GetPropertyInfo(parentObj, childProperties[i]);
                        if (xPropertyInfo != null)
                            propertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(xPropertyInfo);
                        object Val = propertyAccessor.GetValue(parentObj);
                        if (i == childProperties.Length - 1)
                        {
                            return Val;
                        }
                        i++;
                    }
                }
                else if (path.Contains("["))
                {
                    int index = Convert.ToInt32(path.Substring(path.IndexOf('[') + 1, path.IndexOf(']') - path.IndexOf('[') - 1));
                    string tempPath = path.Replace(path.Substring(path.IndexOf('[')), string.Empty);
                    var propertyInfo = GetPropertyInfo(obj, tempPath);
                    if (propertyInfo != null)
                        propertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo);
                    object Val = propertyAccessor.GetValue(obj);
                    IList array = Val as IList;
                    if (array != null && array.Count > index)
                        return array[index];
                }
            }
            else if ((obj.GetType() == typeof(DictionaryEntry)) || (obj.GetType().ToString().Contains("KeyValuePair")))
            {

                var propertyInfo = GetPropertyInfo(obj, "Value"); 
                if (propertyInfo != null)
                    propertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo);
                object valueObj = propertyAccessor.GetValue(obj);

                if (valueObj != null && path != "Key")
                {
                    var propertyInfo1 = GetPropertyInfo(valueObj, path); 
                    if (propertyInfo1 != null)
                        propertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo1);
                    return propertyAccessor.GetValue(valueObj);
                }
                else
                {
                    var propertyInfo2 = GetPropertyInfo(obj, path);
                    if (propertyInfo2 != null)
                        propertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo2);
                    return propertyAccessor.GetValue(obj);
                }
            }
            else
            {
                var propertyInfo = GetPropertyInfo(obj, path);
                if (propertyInfo != null)
                    propertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo);
                return propertyAccessor.GetValue(obj);
            }
            return null;
        }

        ///<summary>
        /// Gets the object by path.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <param name="path">The path value.</param>
        /// <returns>Returns the object</returns>
        public static object GetObjectByPath(object obj, string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var xmlElement = obj as XmlElement;
                if (xmlElement != null)
                {
                    obj = xmlElement.GetAttribute(path);
                }
                else
                {
                    try
                    {
                        return GetPropertyDescriptor(obj, path);

                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            return obj;
        }


        /// <summary>
        /// Converts to double.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>The double value</returns>
        /// <seealso cref="ChartDataUtils"/>
        public static int ConvertPathObjectToPositionValue(object obj)
        {
            int value = 0;
            try
            {
                value = Convert.ToInt32(obj);
            }
            catch
            {
                value = Int32.MinValue;
            }
            return value;
        }

        /// <summary>
        /// Gets the double by path.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <param name="path">The path value.</param>
        /// <returns>The double value</returns>
        public static double GetPositionalPathValue(object obj, string path)
        {

            return ConvertPathObjectToPositionValue(GetObjectByPath(obj, path));
        }

        /// <summary>
        /// Gets the property from the specified object.
        /// </summary>
        /// <param name="obj">Object to retrieve a property.</param>
        /// <param name="path">Property name</param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo(object obj, string path)
        {
#if SyncfusionFramework4_0
            return obj.GetType().GetTypeInfo().GetDeclaredProperty(path);
#else
               return obj.GetType().GetRuntimeProperty(path);
#endif
        }

        internal static double ObjectToDouble(object obj)
        {
            if (obj is DateTime)
            {
                return ((DateTime)obj).ToOADate();
            }
            else
            {
                double result = 0;
                string val = obj.ToString();
                double.TryParse(val, out result);
                return result;
            }
        }
    }
    /// <summary>
    /// Custom comaprer to compare the chart points by x-value.
    /// </summary>
    public partial class PointsSortByXComparer : Comparer<Point>
    {
        #region Members
        /// <summary>
        /// Initializes diff
        /// </summary>
        private double diff;
        #endregion

        /// <summary>
        /// Compares the specified p1 with the specified p2.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <returns>
        /// negative value if point1 &lt; point2
        /// <para>
        /// zero if point1 = point2.
        /// </para>
        /// <para>
        /// positive value if point1 &gt; point2
        /// </para>
        /// </returns>
        public override int Compare(Point point1,Point point2)
        {
            diff = point1.X - point2.X;
            if (diff == 0)
            {
                return 0;
            }

            return diff < 0 ? -1 : 1;
        }
    }

    /// <summary>
    /// ChartColorModifed To modify a given color.
    /// </summary>
    internal static class ChartColorModifier
    {
        /// <summary>
        /// Gets the darkened color which was set.
        /// </summary>
        /// <param name="brush">The point1.</param>
        /// <param name="darkCoefficient">The point2.</param>
        /// <returns>      
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        internal static Brush GetDarkenedColor(Brush brush, double darkCoefficient)
        {
            var color = (brush is SolidColorBrush) ? (brush as SolidColorBrush).Color
                         : (brush is GradientBrush) && (brush as GradientBrush).GradientStops.Count > 0
                         ? (brush as GradientBrush).GradientStops[0].Color
                         : new SolidColorBrush(Colors.Transparent).Color;

            var alpha = (byte)color.A;
            var red = (byte)(color.R * darkCoefficient);
            var green = (byte)(color.G * darkCoefficient);
            var blue = (byte)(color.B * darkCoefficient);

          return new SolidColorBrush(Color.FromArgb(alpha, red, green, blue));
        }
    }
}
