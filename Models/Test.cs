using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication2.Models
{
    public class Test
    {
        [JsonProperty("KeyValue")]
        public KeyValue1 keyValue { get; set; }
    }
}