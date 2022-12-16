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

        public Box() : base() { MaxNodePerTick = -1; }
        public Box(Box other) : base(other) 
        {
            MaxNodePerTick = other.MaxNodePerTick;

            inports = other.inports.Select(p => p.ClonePort()).ToArray();
            outports = other.outports.Select(p => p.ClonePort()).ToArray();

            var old2newInMap = new Dictionary<PortBase, PortBase>();

            for (int i = 0; i < outports.Length; i++)
            {
                // We use outports here because outports of box is inports of internal. 
                if (other.outports[i].SourcePort is PortBase) 
                    old2newInMap[other.outports[i]] = outports[i];
            }

            // Pass1: clone nodes. 
            foreach (var node in other.nodeSet)
            {
                var newnode = node.CloneNode();
                nodeSet.Add(newnode);
                IReadOnlyList<PortBase> oldPorts = node.InPorts!;
                IReadOnlyList<PortBase> newPorts = newnode.InPorts!;
                if (oldPorts.Count != newPorts.Count) continue;

                for (int i = 0; i < oldPorts.Count; i++)
                {
                    if(oldPorts[i].SourcePort is PortBase)
                        old2newInMap[oldPorts[i]] = newPorts[i];
                }
            }

            // Pass2: clone connections. 
            foreach (var kv in old2newInMap)
            {
                old2newInMap[kv.Key.SourcePort!].ConnectTo(kv.Value);
            }
        }

        


        protected PortBase[] inports = SharedEmptyPorts;
        protected PortBase[] outports = SharedEmptyPorts;

        public override IReadOnlyList<PortBase> InPorts => inports;

        public override IReadOnlyList<PortBase> OutPorts => outports;

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

        
        public Port<rayT> AddInPort<rayT>() where rayT:RayBase
        {
            var ips = inports.ToList();
            var newp = new Port<rayT>();
            ips.Add(newp);
            inports = ips.ToArray();
            return newp;
        }

        public Port<rayT> AddOutPort<rayT>() where rayT:RayBase
        {
            var ips = outports.ToList();
            var newp = new Port<rayT>();
            ips.Add(newp);
            outports = ips.ToArray();
            return newp;
        }

        public struct RoutineContext
        {
            public NodeBase node;
            public RoutineType routine;
        }


        protected HashSet<NodeBase> nodeSet = new HashSet<NodeBase>();

        protected HashSet<NodeBase> activatedAtNextTick = new HashSet<NodeBase>();
        protected HashSet<NodeBase> activatedAtThisTick = new HashSet<NodeBase>();

        protected LinkedList<RoutineContext> routineCached = new LinkedList<RoutineContext>();
        protected LinkedList<RoutineContext> routineCurrent = new LinkedList<RoutineContext>();
        

        /*private Queue<PortBase>? _portCache;
        private Queue<PortBase> _PortCache
        {
            get
            {
                if (_portCache == null) _portCache = new Queue<PortBase>();
                return _portCache;
            }
        }
        public void AddNode(NodeBase node)
        {
            if (nodeSet.Contains(node)) return;
            
            _LocalizeNode(node);
        }

        void _LocalizeNode(NodeBase node)
        {
            nodeSet.Add(node);

            {
                _PortCache.Clear();
                foreach (var ip in node.InPorts)
                {
                    var sp = ip.SourcePort;
                    if (sp == null) continue;
                    if(sp.BaseObject.Parent is NodeBase spParent)
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
                    if(tp.BaseObject.Parent is NodeBase pnode)
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

        public virtual IEnumerable<NodeBase> GetNodesByName(string name) => nodeSet.Where(n => n.Name == name);


        public virtual NodeBase? GetNodeByUID(UIDType uid) => nodeSet.Where(n=>n.UID == uid).FirstOrDefault();




        bool _running = false;

        
        public override void Init()
        {
            base.Init();


            activatedAtNextTick.Clear();
            activatedAtThisTick.Clear();

            routineCached.Clear();
            routineCurrent.Clear();

            _cancel = false;
            _running = false;

            foreach (var node in nodeSet)
                node.Init();

        }


        protected void SpreadPortRays(IEnumerable<PortBase> ports)
        {
            foreach (var port in ports)
            {
                port.SendRay();
                if (port.TargetPort?.Parent is NodeBase node)
                {
                    if (node == this)
                    {
                        continue;
                    }
                    else
                    {
                        RegisterNodeToNextTick(node);
                    }
                }
            }
        }


        void SpreadPortRays(params PortBase[] ports)
        {
            SpreadPortRays(ports as IReadOnlyList<PortBase>);
        }

        void RegisterNodeToNextTick(NodeBase node)
        {
            activatedAtNextTick.Add(node);
        }

        public virtual void RegisterRoutine(NodeBase node, RoutineType routine)
        {
            routineCached.AddLast(new RoutineContext()
            {
                node = node,
                routine = routine,
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

            TurnOnRoutineIfNeeded();

            return Status.ShutdownAndEmitWith(outports.Where(p => p.RayChanged()));
        }

        public override Status OnActivated()
        {
            SpreadPortRays(inports.Where(p => p.RayChanged()));

            TurnOnRoutineIfNeeded();

            return Status.ShutdownAndEmitWith(outports.Where(p=>p.RayChanged()));
        }

        
        void TurnOnRoutineIfNeeded()
        {
            if (!_running)
            {
                if (activatedAtNextTick.Count > 0 || routineCached.Count > 0)
                {
                    _running = true;
                    RunRoutine(GetRoutine());
                }
            }
        }



        protected RoutineType GetRoutine()
        {
            try
            {
                var maxNode = MaxNodePerTick;
                var portsToBeSpread = new Queue<PortBase>(); 

                while (activatedAtNextTick.Count > 0 || routineCached.Count > 0)
                {

                    (activatedAtNextTick, activatedAtThisTick)
                        =
                        (activatedAtThisTick, activatedAtNextTick);

                    

                    if ((maxNode >= 0) && ((activatedAtThisTick.Count + routineCurrent.Count)>= maxNode))
                    {
                        Log($"Exceeded {nameof(MaxNodePerTick)} limitation: {MaxNodePerTick}. ");
                        yield break;
                    }


                    foreach (var node in activatedAtThisTick)
                    {
                        if (_cancel)
                        {
                            Log($"Canceled running. ");
                            yield break;
                        }

                        var status = node.OnActivated();
                        if (status.HasResult)
                        {
                            foreach (var port in status.PortMask ?? node.OutPorts)
                                portsToBeSpread.Enqueue(port);
                        }
                    }
                    activatedAtThisTick.Clear();

                    (routineCached, routineCurrent)
                        =
                        (routineCurrent, routineCached);

                    while (routineCurrent.Count > 0)
                    {
                        if (_cancel)
                        {
                            Log($"Canceled running. ");
                            yield break;
                        }

                        var rc = routineCurrent.First!.Value;
                        routineCurrent.RemoveFirst();
                        var routine = rc.routine;
                        if(routine.MoveNext())
                        {
                            var status = routine.Current;

                            if (status.HasResult)
                            {
                                foreach (var port in status.PortMask ?? rc.node.OutPorts)
                                    portsToBeSpread.Enqueue(port);
                            }
                            if (!status.Finished) routineCached.AddLast(rc);
                        }
                        else
                        {
                            Log($"Routine from {rc.node} is terminated unexpected. Please use 'yield return Status.Shutdown' instead of 'yield break';");
                        }
                    }

                    SpreadPortRays(portsToBeSpread);
                    portsToBeSpread.Clear();

                    yield return Status.WaitForNextStepAndEmitWith(outports.Where(p=>p.RayChanged()));

                }
            }
            finally
            {
                _running = false;
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

        public override NodeBase CloneNode()
        {
            return new Box(this);
        }


        bool _cancel = false;
        public void SendCancelSignal()
        {
            _cancel = true;
        }


        private bool disposedValue;
        protected override void Dispose(bool disposing)
        {
            if(!disposedValue)
            {
                if (disposing)
                {
                    foreach (var node in nodeSet)
                    {
                        node.Dispose();
                    }
                    nodeSet.Clear();
                }
                disposedValue = true;
            }
            base.Dispose(disposing);
        }


        public string GetRayDescriptions()
        {
            StringBuilder sb = new StringBuilder();
            // sb.AppendLine("Ray: ");
            foreach (var node in nodeSet)
            {
                foreach (var port in node.OutPorts)
                {
                    if (port.RayChangedReadOnly)
                    {
                        sb.AppendLine($"{port.Parent} =={port.Ray?.ToString() ?? "null"}==> {port.TargetPort?.Parent}");
                    }
                }
            }
            return sb.ToString();
        }

        public string GetCachedNodeDescriptions()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Next tick node: ");
            foreach (var node in activatedAtNextTick)
            {
                sb.AppendLine(node.ToString());
            }
            sb.AppendLine();
            sb.AppendLine("This tick node: ");
            foreach (var node in activatedAtNextTick)
            {
                sb.AppendLine(node.ToString());
            }
            sb.AppendLine();
            sb.AppendLine("Cached routine: ");
            foreach (var rc in routineCached)
            {
                sb.AppendLine(rc.node.ToString());
            }

            sb.AppendLine();
            sb.AppendLine("Current routine: ");
            foreach (var rc in routineCurrent)
            {
                sb.AppendLine(rc.node.ToString());
            }


            return sb.ToString();
        }
    }
}
