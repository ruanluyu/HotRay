using HotRay.Base.Nodes.Components.Processors;
using HotRay.Base.Port;
using HotRay.Base.Ray.Lite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Examples
{
    class AdderCore : ICore
    {
        [InPort(0)]
        IntRay? a { get; set; }
        [InPort(1)]
        IntRay? b { get; set; }

        [OutPort(0)]
        IntRay? o { get; set; }

        public ICore CloneCore() => new AdderCore();

        public void Process()
        {
            o = new IntRay() { Data = a.Data + b.Data };
        }
    }
}
