using HotRay.Base.Port;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Containers
{
    public class Box: ComponentBase
    {
        
        protected IPort[] inports = SharedEmptyPorts;
        protected IPort[] outports = SharedEmptyPorts;

        public override IPort[] InputPorts => inports;

        public override IPort[] OutputPorts => outports;

        public void SetInputPorts(params IPort[] ports) => inports = ports.Length == 0 ? SharedEmptyPorts : ports;
        
        public void SetOutputPorts(params IPort[] ports) => outports = ports.Length == 0 ? SharedEmptyPorts : ports;


        protected HashSet<INode> nodeSet = new HashSet<INode>();

        public void AddNode(INode node)
        {
            if (nodeSet.Contains(node)) return;
            nodeSet.Add(node);
        }

        public void RemoveNode(INode node)
        {
            if (!nodeSet.Contains(node)) return;
            nodeSet.Remove(node);
        }
    }
}
