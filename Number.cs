using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionAppTwitter
{
    public class Number
    {

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }
        public string Friends { get; set; }
        public string Followers { get; set; }

    }
}
