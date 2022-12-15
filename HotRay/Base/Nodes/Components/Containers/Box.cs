using HotRay.Base.Port;
using HotRay.Base.Ray;
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
        /// <summary>
        /// -1: unlimited, >=0 limited. default: -1 
        /// </summary>
        public int MaxNodePerTick { get; set; }



        public struct RoutineContext
        {
            public RoutineType routine;
            public NodeBase node;
        }

        public Box() : base() { MaxNodePerTick = -1; }
        public Box(Box other) : base(other) 
        {
            MaxNodePerTick = other.MaxNodePerTick;

            inports = other.inports.Select(p => p.ClonePort()).ToArray();
            outports = other.outports.Select(p => p.ClonePort()).ToArray();

            var old2newInMap = new Dictionary<IPort, IPort>();

            for (int i = 0; i < outports.Length; i++)
            {
                // We use outports here because outports of box is inports of internal. 
                if (other.outports[i].SourcePort is IPort) 
                    old2newInMap[other.outports[i]] = outports[i];
            }

            // Pass1: clone nodes. 
            foreach (var node in other.nodeSet)
            {
                var newnode = node.CloneNode();
                nodeSet.Add(newnode);
                IReadOnlyList<IPort> oldPorts = node.InPorts!;
                IReadOnlyList<IPort> newPorts = newnode.InPorts!;
                if (oldPorts.Count != newPorts.Count) continue;

                for (int i = 0; i < oldPorts.Count; i++)
                {
                    if(oldPorts[i].SourcePort is IPort)
                        old2newInMap[oldPorts[i]] = newPorts[i];
                }
            }

            // Pass2: clone connections. 
            foreach (var kv in old2newInMap)
            {
                old2newInMap[kv.Key.SourcePort!].ConnectTo(kv.Value);
            }
        }

        


        protected IPort[] inports = SharedEmptyPorts;
        protected IPort[] outports = SharedEmptyPorts;

        public override IReadOnlyList<IPort> InPorts => inports;

        public override IReadOnlyList<IPort> OutPorts => outports;

        public void ResetInputPortsWith(params PortBase[] ports)
        {
            ClearInConnections();
            inports = ports.Length == 0 ? SharedEmptyPorts : ports;
            InitInPorts();
        }

        public void ResetOutputPortsWith(params PortBase[] ports)
        {
            ClearOutConnections();
            outports = ports.Length == 0 ? SharedEmptyPorts : ports;
            InitOutPorts();
        }

        
        public Port<rayT> AddInPort<rayT>() where rayT:IRay
        {
            var ips = inports.ToList();
            var newp = new Port<rayT>();
            ips.Add(newp);
            inports = ips.ToArray();
            return newp;
        }

        public Port<rayT> AddOutPort<rayT>() where rayT:IRay
        {
            var ips = outports.ToList();
            var newp = new Port<rayT>();
            ips.Add(newp);
            outports = ips.ToArray();
            return newp;
        }



        protected HashSet<INode> nodeSet = new HashSet<INode>();
        protected Queue<RoutineContext> runThisTickRoutines = new Queue<RoutineContext>();
        protected Queue<RoutineContext> runNextTickRoutines = new Queue<RoutineContext>();


        /*private Queue<IPort>? _portCache;
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

        }*/

        public void RemoveNode(NodeBase node)
        {
            node.ClearInConnections();
            node.ClearOutConnections();
            nodeSet.Remove(node);
        }

        public nodeT CreateNode<nodeT>() where nodeT:NodeBase, new()
        {
            if (typeof(nodeT) == typeof(Space)) 
                throw new ArgumentException("Space cannot be created inside a Box or a Space, use Box instead. ");
            nodeT res = new nodeT();
            res.Parent = this;
            nodeSet.Add(res);
            return res;
        }

        public virtual IEnumerable<INode> GetNodesByName(string name) => nodeSet.Where(n => n.BaseObject.Name == name);


        public virtual INode? GetNodeByUID(UIDType uid) => nodeSet.Where(n=>n.BaseObject.UID == uid).FirstOrDefault();




        HashSet<IPort> portsHasResult = new HashSet<IPort>();
        bool running = false;

        
        public override void Init()
        {
            base.Init();

            runThisTickRoutines.Clear();
            runNextTickRoutines.Clear();

            running = false;
            portsHasResult.Clear();

            foreach (var node in nodeSet)
                node.Init();

        }


        Queue<IPort> _spreadBFS = new Queue<IPort>();
        protected void SpreadPortRays(IEnumerable<IPort> ports)
        {
            foreach (var ip in ports)
                _spreadBFS.Enqueue(ip);

            while (_spreadBFS.TryDequeue(out var ip))
            {
                ip.SendRay();
                if(ip.TargetPort?.BaseObject.Parent is INode node)
                {
                    if (node == this)
                    {
                        portsHasResult.Add(ip.TargetPort);
                    }
                    else
                    {
                        var status = node.OnPortUpdate(ip.TargetPort);
                        if(status.HasResult)
                        {
                            if(status.PortMask == null)
                            {
                                foreach (var op in node.OutPorts)
                                {
                                    _spreadBFS.Enqueue(op);
                                }
                            }
                            else
                            {
                                foreach (var op in status.PortMask)
                                {
                                    _spreadBFS.Enqueue(op);
                                }
                            }
                        }
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

        public override Status OnEntry()
        {
            foreach (var source in nodeSet)
            {
                var status = source.OnEntry();
                if (status.HasResult)
                {
                    SpreadPortRays(status.PortMask ?? source.OutPorts);
                }
            }
            if (runThisTickRoutines.Count > 0)
            {
                TurnOnRoutine();
                if (portsHasResult.Count > 0) 
                    return Status.ShutdownAndEmitWith(portsHasResult);
            }
            return Status.Shutdown;
        }

        public override Status OnPortUpdate(IPort inport)
        {
            SpreadPortRays(inport);
            TurnOnRoutine();
            var a = portsHasResult.ToArray();
            portsHasResult.Clear();
            return Status.ShutdownAndEmitWith(a);
        }

        
        void TurnOnRoutine()
        {
            if (!running)
            {
                running = true;
                RunRoutine(GetRoutine());
            }
        }


        protected RoutineType GetRoutine()
        {
            try
            {
                var maxNode = MaxNodePerTick;
                do
                {
                    portsHasResult.Clear();
                    int doneNode = 0;
                    while (true)
                    {
                        if (_cancel)
                        {
                            Log($"Canceled running. ");
                            yield break;
                        }
                        if(maxNode >= 0 && doneNode >= maxNode)
                        {
                            Log($"Exceed limitation {maxNode}. ");
                            yield break;
                        }
                        if(runThisTickRoutines.TryDequeue(out var rc))
                        {
                            if (rc.routine.MoveNext())
                            {
                                var status = rc.routine.Current;
                                if (status.HasResult) SpreadPortRays(rc.node.OutPorts);
                                if (!status.Finished) runNextTickRoutines.Enqueue(rc);
                            }
                            doneNode++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    yield return Status.WaitForNextStepAndEmitWith(portsHasResult);
                    portsHasResult.Clear();

                    (runNextTickRoutines, runThisTickRoutines)
                        =
                        (runThisTickRoutines, runNextTickRoutines);

                } while (runThisTickRoutines.Count > 0);
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


        bool _cancel = false;
        public void SendCancelSignal()
        {
            _cancel = true;
        }

        
    }
}
