using System;
using System.Collections.Generic;
using System.Text;

namespace SampleBrowser.SfChart
{
    public class AreaChartViewModel
    {
        public AreaChartViewModel()
        {
            this.Data = new List<LineModel>();
            Data.Add(new LineModel() { XValue = "1995", YValue = 103 });
            Data.Add(new LineModel() { XValue = "1997", YValue = 221 });
            Data.Add(new LineModel() { XValue = "1999", YValue = 80 });
            Data.Add(new LineModel() { XValue = "2001", YValue = 110 });
            Data.Add(new LineModel() { XValue = "2003", YValue = 80 });
            Data.Add(new LineModel() { XValue = "2005", YValue = 160 });
            Data.Add(new LineModel() { XValue = "2007", YValue = 200 });

            this.SplineAreaData = new List<LineModel>();
            SplineAreaData.Add(new LineModel() { XValue = "1995", YValue = 113 });
            SplineAreaData.Add(new LineModel() { XValue = "1997", YValue = 181 });
            SplineAreaData.Add(new LineModel() { XValue = "1999", YValue = 180 });
            SplineAreaData.Add(new LineModel() { XValue = "2001", YValue = 90 });
            SplineAreaData.Add(new LineModel() { XValue = "2003", YValue = 180 });
            SplineAreaData.Add(new LineModel() { XValue = "2005", YValue = 110 });
            SplineAreaData.Add(new LineModel() { XValue = "2007", YValue = 220 });
        }

        public IList<LineModel> Data
        {
            get;
            set;
        }

        public IList<LineModel> SplineAreaData
        {
            get;
            set;
        }
    }
}
