using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication2.Models
{
    public class MasterRefineryModel
    {
        public int id { set; get; }
        public int RefineryId { set; get; }
        public string SubRefineryName { set; get; }
        public string SubRefineryCode { set; get; }
        public string CTTagValue { set; get; }
        public string UpTimeTag { set; get; }
        public string InputTag { set; get; }
        public string OutputTag { set; get; }
        public double UpTimePercentage { set; get; }
    }
}