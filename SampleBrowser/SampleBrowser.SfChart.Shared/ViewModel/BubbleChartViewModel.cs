using System;
using System.Collections.Generic;
using System.Text;

namespace SampleBrowser.SfChart
{
    public class BubbleSeriesViewModel
    {
        public List<LineModel> BubbleData { get; set; }

        public BubbleSeriesViewModel()
        {
            BubbleData = new List<LineModel>()
            {
                new LineModel(92.2, 117.8, 0.347, "China"),
                new LineModel(74, 116.5, 1.241, "India"),
                new LineModel(90.4, 226.0, 0.238, "Indonesia"),
                new LineModel(99.4, 252.2, 0.312, "US"),
                new LineModel(88.6, 211.3, 0.197, "Brazil"),
                new LineModel(99, 220.7, 0.0818, "Germany"),
                new LineModel(72, 322.0, 0.0826, "Egypt"),
                new LineModel(99.6, 413.4, 0.143, "Russia"),
                new LineModel(99, 255.2, 0.128, "Japan"),
                new LineModel(86.1, 114.0, 0.115, "Mexico"),
                new LineModel(61.3, 311.45, 0.162, "Nigeria"),
                new LineModel(82.2, 293.97, 0.7, "Hong Kong"),
            };
        }
    }
}
