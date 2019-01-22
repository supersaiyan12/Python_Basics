using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication2.Models
{
    public class KeyValue1
    {

       public string key { get; set; }

       public double value { get; set; }


    }


    public class KeyValueBatch
    {

        public string key { get; set; }

        public double value { get; set; }

        public string BatchName { get; set; }

        public bool IsmanualSubmit { get; set; }


    }
}