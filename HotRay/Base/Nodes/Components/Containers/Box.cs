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

        public Box() : base() { }
        public Box(Box other) : base(other) 
        {
            
        }


        protected IPort[] inports = SharedEmptyPorts;
        protected IPort[] outports = SharedEmptyPorts;

        public override IReadOnlyList<IPort> InPorts => inports;

        public override IReadOnlyList<IPort> OutPorts => outports;

        public void SetInputPorts(params IPort[] ports)
        {
            ClearInConnections();
            inports = ports.Length == 0 ? SharedEmptyPorts : ports;
            InitInPorts();
        }

        public void SetOutputPorts(params IPort[] ports)
        {
            ClearOutConnections();
            outports = ports.Length == 0 ? SharedEmptyPorts : ports;
            InitOutPorts();
        }


        protected HashSet<INode> nodeSet = new HashSet<INode>();
        protected Queue<RoutineContext> runThisTickRoutines = new Queue<RoutineContext>();
        protected Queue<RoutineContext> runNextTickRoutines = new Queue<RoutineContext>();
        protected HashSet<INode> registeredThisTickNode = new HashSet<INode>();
        protected HashSet<INode> registeredNextTickNode = new HashSet<INode>();


        private Queue<IPort>? _portCache;
        private Queue<IPort> _PortCache
        {
            get
            {
                if (_portCache == null) _portCache = new Queue<IPort>();
                return _portCache;
            }
        }
        public void AddNode(INode node)
        {
            if (nodeSet.Contains(node)) return;
            
            _LocalizeNode(node);
        }

        void _LocalizeNode(INode node)
        {
            nodeSet.Add(node);

            {
                _PortCache.Clear();
                foreach (var ip in node.InPorts)
                {
                    var sp = ip.SourcePort;
                    if (sp == null) continue;
                    if(sp.BaseObject.Parent is INode spParent)
                    {
                        if (!nodeSet.Contains(spParent))
                        {
                            var newp = (ip.ClonePort() as PortBase)!;
                            newp.Parent = this;

                            sp.ConnectTo(newp);
                            newp.ConnectTo(ip);

                            _PortCache.Enqueue(newp);
                        }
                    }
                }
                var nl = inports.ToList();
                nl.AddRange(_PortCache);
                inports = nl.ToArray();
            }

            {
                _PortCache.Clear();
                foreach (var op in node.OutPorts)
                {
                    var tp = op.TargetPort;
                    if (tp == null) continue;
                    if(tp.BaseObject.Parent is INode pnode)
                    {
                        if (!nodeSet.Contains(pnode))
                        {
                            var newp = (op.ClonePort() as PortBase)!;
                            newp.Parent = this;

                            op.ConnectTo(newp);
                            newp.ConnectTo(tp);

                            _PortCache.Enqueue(newp);
                        }
                    }
                }
                var nl = outports.ToList();
                nl.AddRange(_PortCache);
                outports = nl.ToArray();
            }
            _PortCache.Clear();

        }

        public void RemoveNode(NodeBase node)
        {
            node.ClearInConnections();
            node.ClearOutConnections();
            nodeSet.Remove(node);
        }

        public nodeT CreateNode<nodeT>() where nodeT:NodeBase, new()
        {
            nodeT res = new nodeT();
            res.Parent = this;
            AddNode(res);
            return res;
        }

        public virtual IEnumerable<INode> GetNodesByName(string name) => nodeSet.Where(n => n.BaseObject.Name == name);


        public virtual INode? GetNodeByUID(UIDType uid) => nodeSet.Where(n=>n.BaseObject.UID == uid).FirstOrDefault();


        

        bool hasResult = false;
        bool running = false;

        
        public override void Init()
        {
            base.Init();
            runThisTickRoutines.Clear();
            runNextTickRoutines.Clear();
            registeredNextTickNode.Clear();
            registeredThisTickNode.Clear();
            running = false;
            hasResult = false;

            
        }

        void SpreadPortRays(IReadOnlyList<IPort> ports)
        {
            foreach (var ip in ports)
            {
                ip.SendRay();
                if(ip.TargetPort?.BaseObject.Parent is INode node)
                {
                    if (node == this)
                    {
                        hasResult = true;
                    }
                    else
                    {
                        node.OnPortUpdate(ip.TargetPort);
                    }
                }
            }
        }

        void SpreadPortRays(params IPort[] ports)
        {
            SpreadPortRays(ports as IReadOnlyList<IPort>);
        }

        public virtual void RegisterRoutine(NodeBase node, RoutineType routine)
        {
            runThisTickRoutines.Enqueue(new RoutineContext()
            {
                routine = routine,
                node = node,
            });
        }

        public override void OnPortUpdate(IPort inport)
        {
            SpreadPortRays(inport);
            if (!running)
            {
                RunRoutine(GetRoutine());
            }
        }

        RoutineType GetRoutine()
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

        public virtual IEnumerator<INode> GetNodeEnumerator(int? remainDepth = 8, HashSet<Box>? searchedBox = null)
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

        public override INode CloneNode()
        {
            return new Box(this);
        }

    }
}
