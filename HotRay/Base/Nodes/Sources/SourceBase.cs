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


        public abstract IPort[] OutputPorts
        {
            get;
        }

        public override void Init()
        {
            InitOutputPorts();
        }

        public virtual void InitOutputPorts()
        {
            var p = OutputPorts;
            if (p == null) return;
            for (int i = 0; i < p.Length; i++)
            {
                p[i].Ray = null;
            }
        }


    }
}
