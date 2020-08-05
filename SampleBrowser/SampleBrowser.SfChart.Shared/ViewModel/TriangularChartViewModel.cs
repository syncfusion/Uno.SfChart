using System;
using System.Collections.Generic;
using System.Text;

namespace SampleBrowser.SfChart
{
    public class TriangularViewModel
    {
        public TriangularViewModel()
        {
            this.PyramidData = new List<TriangularModel>();

            PyramidData.Add(new TriangularModel() { Category = "License", Percentage = 15d });
            PyramidData.Add(new TriangularModel() { Category = "Other", Percentage = 18d });
            PyramidData.Add(new TriangularModel() { Category = "Sales", Percentage = 14d });
            PyramidData.Add(new TriangularModel() { Category = "Income", Percentage = 16d });
            PyramidData.Add(new TriangularModel() { Category = "Production", Percentage = 14d });
        }

        public IList<TriangularModel> PyramidData
        {
            get;
            set;
        }
    }
}
