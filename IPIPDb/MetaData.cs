using Newtonsoft.Json;
using System.Collections.Generic;

namespace IPIP.Net
{
    /**
     * @copyright IPIP.net
     */

    public class MetaData
    {
        [JsonProperty("build")]
        public int Build { get; set; }

        [JsonProperty("ip_version")]
        public int IPVersion { get; set; }

        [JsonProperty("node_count")]
        public int NodeCount { get; set; }

        [JsonProperty("languages")]
        public Dictionary<string, int> Languages { get; set; }

        [JsonProperty("fields")]
        public string[] Fields { get; set; }

        [JsonProperty("total_size")]
        public int TotalSize { get; set; }
    }
}