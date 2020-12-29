using System;
using System.Collections.Generic;
using System.Text;

namespace MCSkinDownloader.Models
{
    public class SearchResult
    {
        public string DisplayText { get; set; }
        public object Value { get; set; }


        public override string ToString()
        {
            return DisplayText;
        }
    }
}
