using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HotRay.Base.Nodes.INode;

namespace HotRay.Base.Nodes
{
    public abstract class NodeBase: BaseObject, INode
    {
        public NodeBase() : base()
        {
            
        }

        public NodeBase(NodeBase other): base(other)
        {

        }

        public virtual void Init()
        {

        }

        public virtual IEnumerator<Status> GetRoutine()
        {
            yield break;
        }


    }
}
