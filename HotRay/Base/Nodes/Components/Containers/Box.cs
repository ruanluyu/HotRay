using HotRay.Base.Port;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace HotRay.Base.Nodes.Components.Containers
{
    using RoutineType = IEnumerator<Status>;
    public class Box: ComponentBase
    {
        public struct RoutineContext
        {
            public RoutineType routine;
            public NodeBase node;
        }

        protected PortBase[] inports = SharedEmptyPorts;
        protected PortBase[] outports = SharedEmptyPorts;

        public override IReadOnlyList<PortBase> InPorts => inports;

        public override IReadOnlyList<PortBase> OutPorts => outports;

        public void SetInputPorts(params PortBase[] ports)
        {
            ClearInConnections();
            inports = ports.Length == 0 ? SharedEmptyPorts : ports;
            InitInPorts();
        }

        public void SetOutputPorts(params PortBase[] ports)
        {
            ClearOutConnections();
            outports = ports.Length == 0 ? SharedEmptyPorts : ports;
            InitOutPorts();
        }


        protected HashSet<NodeBase> nodeSet = new HashSet<NodeBase>();
        protected Queue<RoutineContext> runThisTickRoutines = new Queue<RoutineContext>();
        protected Queue<RoutineContext> runNextTickRoutines = new Queue<RoutineContext>();
        protected HashSet<NodeBase> registeredThisTickNode = new HashSet<NodeBase>();
        protected HashSet<NodeBase> registeredNextTickNode = new HashSet<NodeBase>();

        public void AddNode(NodeBase node)
        {
            if (nodeSet.Contains(node)) return;
            node.ClearInConnections();
            node.ClearOutConnections();
            nodeSet.Add(node);
        }

        public void RemoveNode(NodeBase node)
        {
            nodeSet.Remove(node);
        }

        public nodeT CreateNode<nodeT>() where nodeT:NodeBase, new()
        {
            nodeT res = new nodeT();
            res.Parent = this;
            AddNode(res);
            return res;
        }

        public virtual IEnumerable<NodeBase> GetNodesByName(string name) => nodeSet.Where(n => n.Name == name);


        public virtual NodeBase? GetNodeByUID(UIDType uid) => nodeSet.Where(n=>n.UID == uid).FirstOrDefault();


        

        bool hasResult = false;
        bool running = false;
        public override void Init()
        {
            base.Init();
            running = false;
            hasResult = false;
        }
        
        void SpreadPortRays(IReadOnlyList<PortBase> ports)
        {
            foreach (var ip in ports)
            {
                ip.SendRay();
                foreach (var port in ip.TargetPorts.OfType<PortBase>())
                {
                    if (port.Parent is NodeBase node)
                    {
                        if (node == this)
                        {
                            hasResult = true;
                        }
                        else
                        {
                            if (!registeredThisTickNode.Contains(node) && nodeSet.Contains(node))
                            {
                                runThisTickRoutines.Enqueue(new RoutineContext()
                                {
                                    routine = node.GetRoutine(),
                                    node = node,
                                });
                                registeredThisTickNode.Add(node);
                            }
                        }
                    }
                }
            }
            
        }

        public override RoutineType GetRoutine()
        {
            if (running) yield break;
            
            try
            {
                do
                {
                    (runNextTickRoutines, runThisTickRoutines)
                        =
                        (runThisTickRoutines, runNextTickRoutines);

                    (registeredThisTickNode, registeredNextTickNode)
                        =
                        (registeredNextTickNode, registeredThisTickNode);

                    hasResult = false;

                    SpreadPortRays(InPorts);

                    while (runThisTickRoutines.TryDequeue(out var rc))
                    {
                        registeredThisTickNode.Remove(rc.node);
                        if (rc.routine.MoveNext())
                        {
                            var status = rc.routine.Current;
                            if (status.HasResult) SpreadPortRays(rc.node.OutPorts);
                            if (!status.Finished) runNextTickRoutines.Enqueue(rc);
                        }
                    }
                    yield return hasResult ? Status.EmitAndWaitForNextStep : Status.WaitForNextStep;
                    hasResult = false;
                } while (runNextTickRoutines.Count > 0 || runThisTickRoutines.Count > 0);
            }
            finally
            {
                running = false;
            }
            
        }

        public virtual IEnumerator<NodeBase> GetNodeEnumerator(int? remainDepth = 8, HashSet<Box>? searchedBox = null)
        {
            if (searchedBox == null) searchedBox = new HashSet<Box>();
            if (remainDepth != null && remainDepth <= 0) yield break;
            foreach (var n in nodeSet)
            {
                yield return n;
                if (n is Box b && !searchedBox.Contains(b))
                {
                    var ite = b.GetNodeEnumerator(remainDepth == null ? null : remainDepth - 1, searchedBox);
                    while (ite.MoveNext())
                    {
                        yield return ite.Current;
                    }
                    searchedBox.Add(b);
                }
            }
        }
    }
}
