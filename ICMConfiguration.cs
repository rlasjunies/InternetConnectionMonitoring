using System.Collections.Generic;

namespace icm
{
    public record ICMConfiguration {
        public List<string> servers {get;set;}
        public int frequency {get;set;}
        public bool loginfile {get;set;}
        public bool loginfiledisconnected {get;set;}
    }
}