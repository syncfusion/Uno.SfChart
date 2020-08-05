using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Contains Chart resource dictionaries
    /// </summary>
    internal static class ChartDictionaries
    {
        [ThreadStatic]
        private static ResourceDictionary genericLegendDictionary;

        internal static ResourceDictionary GenericLegendDictionary
        {

            get
            {
                if (genericLegendDictionary == null)
                {

                    genericLegendDictionary = new ResourceDictionary()
                    {

#if WINDOWS_UAP
                        Source = new Uri(@"ms-appx:///Syncfusion.SfChart.Uno/Themes/Generic.Legend.xaml")
#endif
                    };

                }
                return genericLegendDictionary;
            }
        }

        [ThreadStatic]
        private static ResourceDictionary genericSymbolDictionary;

        internal static ResourceDictionary GenericSymbolDictionary
        {
            get
            {
                if(genericSymbolDictionary == null)
                {
                    genericSymbolDictionary = new ResourceDictionary()
                    {
#if WINDOWS_UAP
                        Source = new Uri(@"ms-appx:///Syncfusion.SfChart.Uno/Themes/Generic.Symbol.xaml")
#endif
                    };
                }

                return genericSymbolDictionary;
            }
        }

        [ThreadStatic]
        private static ResourceDictionary genericCommonDictionary;

        internal static ResourceDictionary GenericCommonDictionary
        {
            get
            {
                if(genericCommonDictionary == null)
                {
                    genericCommonDictionary = new ResourceDictionary()
                    {
#if WINDOWS_UAP
                        Source = new Uri(@"ms-appx:///Syncfusion.SfChart.Uno/Themes/Generic.Common.xaml")
#endif
                    };
                }

                return genericCommonDictionary;
            }
        }
    }
}
