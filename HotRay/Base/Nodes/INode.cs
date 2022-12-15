using HotRay.Base.Port;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes
{
    public interface INode
    {

        void Init();

        public BaseObject BaseObject { get; }

        
        Status OnPortUpdate(IPort inport);

        Status OnEntry();

        IReadOnlyList<IPort> InPorts
        {
            get;
        }

        IReadOnlyList<IPort> OutPorts
        {
            get;
        }

        public INode CloneNode();

    }
}
