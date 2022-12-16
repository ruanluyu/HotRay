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

        }

        public SourceBase(SourceBase other): base(other)
        {

        }



        public override sealed IReadOnlyList<PortBase> InPorts
        {
            get => SharedEmptyPorts;
        }


        public override sealed Status OnActivated()
        {
            return base.OnActivated();
        }
    }
}
