using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public string? Description { get; set; }

        
        

        
        public static Box? GetParentBoxOf(InPort port)
        {
            Box? parentBox = port.GetNearestParent<Box>();
            if (parentBox == null) return null;
            if (parentBox.inPortList.Contains(port)) return GetParentBoxOf(parentBox); // GrandParentBox
            return parentBox;
        }

        public static Box? GetParentBoxOf(OutPort port)
        {
            Box? parentBox = port.GetNearestParent<Box>();
            if (parentBox == null) return null;
            if (parentBox.outPortList.Contains(port)) return GetParentBoxOf(parentBox); // GrandParentBox
            return parentBox;
        }

        public static Box? GetParentBoxOf(NodeBase node)
        { 
            return node.GetNearestParent<Box>();
        }

        protected OutPort[] inPortInnerReflectionList;
        protected InPort[] outPortInnerReflectionList;

        public IReadOnlyList<OutPort> InPortInnerReflections => inPortInnerReflectionList;
        public IReadOnlyList<InPort> OutPortInnerReflections => outPortInnerReflectionList;

        public Box() : base() { 
            MaxNodePerTick = -1;
            Description = null;
            inPortInnerReflectionList = SharedEmptyOutPorts;
            outPortInnerReflectionList = SharedEmptyInPorts;
        }
        public Box(Box other) : base(other) 
        {
            if (Parent is Box b)
            {
                b.nodeSet.Add(this);
            }

            MaxNodePerTick = other.MaxNodePerTick;

            Description = other.Description;

            inPortList = other.inPortList.Select(p => {
                var cp = (p.ClonePort() as InPort)!;
                cp.Parent = this;
                return cp;
                }).ToArray();
            outPortList = other.outPortList.Select(p => {
                var cp = (p.ClonePort() as OutPort)!;
                cp.Parent = this;
                return cp;
            }).ToArray();

            inPortInnerReflectionList = SharedEmptyOutPorts;
            outPortInnerReflectionList = SharedEmptyInPorts;

            UpdateInPortReflections();
            UpdateOutPortReflections();

            var old2newOutPorts = new Dictionary<OutPort, OutPort>();
            var old2newInPorts = new Dictionary<InPort, InPort>();

            for (int i = 0; i < inPortInnerReflectionList.Length; i++)
            {
                old2newOutPorts[other.inPortInnerReflectionList[i]] = inPortInnerReflectionList[i];
            }

            for (int i = 0; i < outPortInnerReflectionList.Length; i++)
            {
                old2newInPorts[other.outPortInnerReflectionList[i]] = outPortInnerReflectionList[i];
            }



            // Pass1: clone nodes. 
            foreach (var node in other.nodeSet)
            {
                var newnode = node.CloneNode();
                newnode.Parent = this;
                nodeSet.Add(newnode);
                if (node.InPorts.Count == newnode.InPorts.Count)
                {
                    for (int i = 0; i < node.InPorts.Count; i++)
                    {
                        old2newInPorts[node.InPorts[i]] = newnode.InPorts[i];
                    }
                }
                if (node.OutPorts.Count == newnode.OutPorts.Count)
                {
                    for (int i = 0; i < node.OutPorts.Count; i++)
                    {
                        old2newOutPorts[node.OutPorts[i]] = newnode.OutPorts[i];
                    }
                }

            }

            // 2. Copy connections.
            foreach (var outPortKV in old2newOutPorts)
            {
                if (outPortKV.Key.TargetPort == null) continue;
                outPortKV.Value.ConnectTo(old2newInPorts[outPortKV.Key.TargetPort]);
            }



        }

        void UpdateInPortReflections()
        {
            var oldList = inPortInnerReflectionList;
            inPortInnerReflectionList = new OutPort[inPortList.Length];
            for (int i = 0; i < inPortList.Length; i++)
            {
                if(i < oldList.Length && oldList[i].RayType == inPortList[i].RayType)
                {
                    inPortInnerReflectionList[i] = oldList[i];
                }
                else
                {
                    inPortInnerReflectionList[i] = CreateOutPortWithType(inPortList[i].RayType);
                }
            }
            for (int i = inPortInnerReflectionList.Length; i < oldList.Length; i++)
            {
                oldList[i].BreakConnection();
            }
        }

        void UpdateOutPortReflections()
        {
            var oldList = outPortInnerReflectionList;
            outPortInnerReflectionList = new InPort[outPortList.Length];
            for (int i = 0; i < outPortList.Length; i++)
            {
                if (i < oldList.Length && oldList[i].RayType == outPortList[i].RayType)
                {
                    outPortInnerReflectionList[i] = oldList[i];
                }
                else
                {
                    outPortInnerReflectionList[i] = CreateInPortWithType(outPortList[i].RayType);
                }
            }
            for (int i = outPortInnerReflectionList.Length; i < oldList.Length; i++)
            {
                oldList[i].SourcePort?.BreakConnection();
            }
        }

        public override void ClearInConnections()
        {
            base.ClearInConnections();
            foreach (var node in inPortInnerReflectionList)
            {
                node.BreakConnection();
            }
        }

        public override void ClearOutConnections()
        {
            base.ClearOutConnections();
            foreach (var node in outPortInnerReflectionList)
            {
                if (node.SourcePort != null)
                {
                    node.SourcePort.BreakConnection();
                }
            }
        }

        public override void InitInPorts()
        {
            base.InitInPorts();
            foreach (var p in inPortInnerReflectionList)
            {
                p.Ray = null;
                p.Parent = this;
            }
        }
        public override void InitOutPorts()
        {
            base.InitOutPorts();
            foreach (var p in outPortInnerReflectionList)
            {
                p.Ray = null;
                p.Parent = this;
            }
        }



        public void ResetInputPortsWith(params InPort[] ports)
        {
            ClearInConnections();
            inPortList = ports.Length == 0 ? SharedEmptyInPorts : ports;
            UpdateInPortReflections();
            InitInPorts();
        }

        public void ResetOutputPortsWith(params OutPort[] ports)
        {
            ClearOutConnections();
            outPortList = ports.Length == 0 ? SharedEmptyOutPorts : ports;
            UpdateOutPortReflections();
            InitOutPorts();
        }



        public InPort<rayT> AddInPort<rayT>() where rayT : RayBase
        {
            var ips = inPortList.ToList();
            var newp = CreateInPort<rayT>();
            ips.Add(newp);
            inPortList = ips.ToArray();
            UpdateInPortReflections();
            return newp;
        }

        public OutPort<rayT> AddOutPort<rayT>() where rayT : RayBase
        {
            var ips = outPortList.ToList();
            var newp = CreateOutPort<rayT>();
            ips.Add(newp);
            outPortList = ips.ToArray();
            UpdateOutPortReflections();
            return newp;
        }

        public struct RoutineContext
        {
            public NodeBase node;
            public RoutineType routine;
        }


        protected HashSet<NodeBase> nodeSet = new HashSet<NodeBase>();

        protected HashSet<NodeBase> NodeActiveAtNextTick = new HashSet<NodeBase>();
        protected HashSet<NodeBase> NodeActiveAtThisTick = new HashSet<NodeBase>();

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


            NodeActiveAtNextTick.Clear();
            NodeActiveAtThisTick.Clear();

            routineCached.Clear();
            routineCurrent.Clear();

            _running = false;

            foreach (var node in nodeSet)
                node.Init();

        }


        protected void SpreadPortRays(IEnumerable<OutPort> ports)
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


        void SpreadPortRays(params OutPort[] ports)
        {
            SpreadPortRays(ports as IReadOnlyList<OutPort>);
        }

        void RegisterNodeToNextTick(NodeBase node)
        {
            lock(NodeActiveAtNextTick)
                NodeActiveAtNextTick.Add(node);
        }

        public virtual void RegisterRoutine(NodeBase node, RoutineType routine)
        {
            lock(routineCached)
                routineCached.AddLast(new RoutineContext()
                {
                    node = node,
                    routine = routine,
                });
        }

        void SyncInPortReflections()
        {
            for (int i = 0; i < inPortInnerReflectionList.Length; i++)
            {
                inPortInnerReflectionList[i].Ray = inPortList[i].Ray;
            }
        }
        void SyncOutPortReflections()
        {
            for (int i = 0; i < outPortInnerReflectionList.Length; i++)
            {
                outPortList[i].Ray = outPortInnerReflectionList[i].Ray;
            }
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
            SyncOutPortReflections();

            TurnOnRoutineIfNeeded();
            return Status.ShutdownAndEmitWith(outPortList.Where(p => p.ChangedSinceLastCheck));
        }

        public override Status OnActivated()
        {
            SyncInPortReflections();
            SpreadPortRays(inPortInnerReflectionList.Where(p => p.ChangedSinceLastCheck));
            SyncOutPortReflections();

            TurnOnRoutineIfNeeded();

            return Status.ShutdownAndEmitWith(outPortList.Where(p=>p.ChangedSinceLastCheck));
        }

        
        void TurnOnRoutineIfNeeded()
        {
            if (!_running)
            {
                if (NodeActiveAtNextTick.Count > 0 || routineCached.Count > 0)
                {
                    _running = true;
                    RunRoutine(GetBoxRoutine());
                }
            }
        }



        protected RoutineType GetBoxRoutine()
        {
            try
            {
                var maxNode = MaxNodePerTick;
                var portsToBeSpread = new HashSet<OutPort>();
                var currentSpace = GetCurrentSpace()!;
                var cancelToken = currentSpace.cancellationToken;

                if (cancelToken.IsCancellationRequested) yield return Status.Shutdown;

                while (NodeActiveAtNextTick.Count > 0 || routineCached.Count > 0)
                {
                    if (cancelToken.IsCancellationRequested) yield return Status.Shutdown;

                    (NodeActiveAtNextTick, NodeActiveAtThisTick)
                        =
                        (NodeActiveAtThisTick, NodeActiveAtNextTick);

                    

                    if ((maxNode >= 0) && ((NodeActiveAtThisTick.Count + routineCurrent.Count)>= maxNode))
                    {
                        Log($"Exceeded {nameof(MaxNodePerTick)} limitation: {MaxNodePerTick}. ");
                        yield return Status.Shutdown;
                    }


                    Task[] activeTasks = NodeActiveAtThisTick.Select(node => Task.Run(() =>
                    {
                        var status = node.OnActivated();
                        if (status.HasResult)
                        {
                            lock (portsToBeSpread)
                                foreach (var port in status.PortMask ?? node.OutPorts)
                                {
                                        portsToBeSpread.Add(port);
                                }
                        }
                    }, cancelToken)).ToArray();
                    if (cancelToken.IsCancellationRequested) yield return Status.Shutdown;
                    Task.WaitAll(activeTasks);
                    if (cancelToken.IsCancellationRequested) yield return Status.Shutdown;
                    NodeActiveAtThisTick.Clear();

                    (routineCached, routineCurrent)
                        =
                        (routineCurrent, routineCached);


                    Task[] routineTasks = routineCurrent.Select(rc => Task.Run(() =>
                    {
                        var routine = rc.routine;
                        if (routine.MoveNext())
                        {
                            var status = routine.Current;

                            if (status.HasResult)
                            {
                                lock(portsToBeSpread)
                                    foreach (var port in status.PortMask ?? rc.node.OutPorts)
                                        portsToBeSpread.Add(port);
                            }
                            if (!status.Finished)
                            {
                                RegisterRoutine(rc.node, rc.routine);
                            }
                        }
                        else
                        {
                            Log($"Routine from {rc.node} is terminated unexpected. Please use 'yield return Status.Shutdown' instead of 'yield break';");
                        }
                    }, cancelToken)).ToArray();
                    routineCurrent.Clear();

                    Task.WaitAll(routineTasks);
                    if (cancelToken.IsCancellationRequested) yield return Status.Shutdown;

                    SpreadPortRays(portsToBeSpread);
                    portsToBeSpread.Clear();

                    SyncOutPortReflections();
                    yield return Status.WaitForNextStepAndEmitWith(outPortList.Where(p=>p.ChangedSinceLastCheck));
                }
                _running = false;
            }
            finally
            {
                _running = false;
            }
            yield return Status.Shutdown;
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



        public virtual string LayoutToString()
        {

            var sb = new StringBuilder();
            sb.AppendLine($"==== {this} ====");
            foreach (var node in nodeSet.OrderBy(e=>e.ToString()))
            {
                if(node is Box bnode)
                {
                    sb.AppendLine(string.Join("\n",bnode.LayoutToString().Split("\n").Select(l=>$"  {l}")));
                }
                else
                {
                    sb.AppendLine(node.ToString());
                }
            }
            sb.AppendLine($"---- Connections ----");
            foreach (var port in nodeSet.OrderBy(e => e.ToString()).SelectMany(n=>n.OutPorts).Concat(inPortInnerReflectionList))
            {
                if(port.TargetPort != null)
                {
                    NodeBase parentNode = (port.Parent as NodeBase)!;
                    NodeBase tparentNode = (port.TargetPort.Parent as NodeBase)!;

                    sb.AppendLine($"{parentNode} {parentNode.OutPorts.ToList().IndexOf(port)} ---> {tparentNode} {tparentNode.InPorts.ToList().IndexOf(port.TargetPort)}");
                }
            }
            sb.AppendLine($"==== {this} end=");
            return sb.ToString();
        }
    }
}
