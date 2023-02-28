using HotRay.Base.Port;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Sources
{
    public abstract class SourceBase : NodeBase
    {
        public SourceBase():base()
        {
            outPortList = SharedEmptyOutPorts;
        }

        public SourceBase(SourceBase other): base(other)
        {
            outPortList = SharedEmptyOutPorts;
        }


        protected OutPort[] outPortList;
        public override sealed IReadOnlyList<InPort> InPorts  => SharedEmptyInPorts;
        public override IReadOnlyList<OutPort> OutPorts  => outPortList;


        public override sealed async Task<Status> OnActivated()
        {
            return await base.OnActivated();
        }
    }
}
