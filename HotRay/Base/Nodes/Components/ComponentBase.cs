using HotRay.Base.Nodes.Sources;
using HotRay.Base.Port;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components
{
    public abstract class ComponentBase : SourceBase
    {
        public ComponentBase() : base()
        {

        }

        public ComponentBase(ComponentBase other) : base(other)
        {

        }


        public abstract IPort[] InputPorts
        {
            get;
        }

        public override void Init()
        {
            base.Init();
            InitInputPorts();
        }

        public virtual void InitInputPorts()
        {
            var p = InputPorts;
            if (p == null) return;
            for (int i = 0; i < p.Length; i++)
            {
                p[i].Ray = null;
            }
        }

    }
}
