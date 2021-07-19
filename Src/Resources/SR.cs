using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;

namespace Syncfusion.UI.Xaml.Charts
{
    sealed class SR
    {
        internal static ResourceManager ResourceManager { get; set; }
        private ResourceLoader resources;
        private static SR loader = null;
        private SR()
        {           
            //http://msdn.microsoft.com/en-us/library/windows/apps/xaml/Hh965329%28v=win.10%29.aspx
            ResourceLoader localizedManager = GetLocalizedResourceManager();
            if (localizedManager == null)
            {
                this.resources = ResourceLoader.GetForCurrentView("Syncfusion.SfChart.Uno/Syncfusion.SfChart.Uno.Resources");               
            }
            else
            {
                this.resources = localizedManager;
            }
        }
        private static SR GetLoader()
        {
            lock (typeof(SR))
            {
                if (SR.loader == null)
                    SR.loader = new SR();
                return SR.loader;
            }
        }
        private static ResourceLoader GetLocalizedResourceManager()
        {
            try
            {
                if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    return null;
                }
                if (Application.Current != null)
                {
                    ResourceLoader manager = ResourceLoader.GetForCurrentView("Syncfusion.SfChart.Uno/Syncfusion.SfChart.Uno.Resources");
                    return manager;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
        internal static string GetString(CultureInfo culture, string name)
        {
            if(ResourceManager != null)
            {
                string value = string.Empty;

                try
                {
                    value = ResourceManager.GetString(name, culture);
                }

                catch (MissingManifestResourceException)
                {

                }

                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }

            SR sr = SR.GetLoader();
            if (sr == null)
                return null;
            string localizedString = string.Empty;            
            if (sr.resources == null)
                return null;
            localizedString= sr.resources.GetString(name);
            if (string.IsNullOrEmpty(localizedString))
            {
                var resources = ResourceLoader.GetForCurrentView("Syncfusion.SfChart.Uno/Syncfusion.SfChart.UWP.Resources");
                localizedString = resources.GetString(name);
            }                
            return localizedString;
        }
    }
}

