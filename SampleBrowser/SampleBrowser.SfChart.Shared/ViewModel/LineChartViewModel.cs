using System;
using System.Collections.Generic;
using System.Text;

namespace SampleBrowser.SfChart
{
    public class LineChartViewModel
    {
        public LineChartViewModel()
        {
            this.Data = new List<LineModel>();
            Data.Add(new LineModel() { XValue = "Sun", YValue = 26 });
            Data.Add(new LineModel() { XValue = "Mon", YValue = 21 });
            Data.Add(new LineModel() { XValue = "Tue", YValue = 30 });
            Data.Add(new LineModel() { XValue = "Wed", YValue = 28 });
            Data.Add(new LineModel() { XValue = "Thu", YValue = 29 });
            Data.Add(new LineModel() { XValue = "Fri", YValue = 24 });
            Data.Add(new LineModel() { XValue = "Sat", YValue = 30 });

            this.SplineData = new List<LineModel>();
            SplineData.Add(new LineModel() { XValue = "1995", YValue = 103 });
            SplineData.Add(new LineModel() { XValue = "1997", YValue = 221 });
            SplineData.Add(new LineModel() { XValue = "1999", YValue = 80 });
            SplineData.Add(new LineModel() { XValue = "2001", YValue = 110 });
            SplineData.Add(new LineModel() { XValue = "2003", YValue = 80 });
            SplineData.Add(new LineModel() { XValue = "2005", YValue = 160 });
            SplineData.Add(new LineModel() { XValue = "2007", YValue = 200 });
        }

        public IList<LineModel> Data
        {
            get;
            set;
        }

        public IList<LineModel> SplineData
        {
            get;
            set;
        }
    }
}
