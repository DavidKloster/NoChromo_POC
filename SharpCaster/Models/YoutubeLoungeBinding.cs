using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCaster.Models
{

    public class Screen
    {
        public string screenId { get; set; }
        public int refreshIntervalInMillis { get; set; }
        public int remoteRefreshIntervalMs { get; set; }
        public int refreshIntervalMs { get; set; }
        public int loungeTokenLifespanMs { get; set; }
        public string loungeToken { get; set; }
        public int remoteRefreshIntervalInMillis { get; set; }
        public long expiration { get; set; }
    }

    public class YoutubeLoungeBinding
    {
        public List<Screen> screens { get; set; }
    }
}
