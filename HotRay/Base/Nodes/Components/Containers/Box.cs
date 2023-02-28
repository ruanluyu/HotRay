using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace HotRay.Base.Nodes.Components.Containers
{

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

            var old2new = new Dictionary<BaseObject, BaseObject>();

            old2new[other] = this;

            for (int i = 0; i < inPortInnerReflectionList.Length; i++)
            {
                old2new[other.inPortInnerReflectionList[i]] = inPortInnerReflectionList[i];
            }

            for (int i = 0; i < outPortInnerReflectionList.Length; i++)
            {
                old2new[other.outPortInnerReflectionList[i]] = outPortInnerReflectionList[i];
            }



            // Pass1: clone nodes. 
            foreach (var node in other.nodeSet)
            {
                var newnode = node.CloneNode();
                newnode.Parent = this;
                nodeSet.Add(newnode);
                old2new[node] = newnode;
                if (node.InPorts.Count == newnode.InPorts.Count)
                {
                    for (int i = 0; i < node.InPorts.Count; i++)
                    {
                        old2new[node.InPorts[i]] = newnode.InPorts[i];
                    }
                }
                if (node.OutPorts.Count == newnode.OutPorts.Count)
                {
                    for (int i = 0; i < node.OutPorts.Count; i++)
                    {
                        old2new[node.OutPorts[i]] = newnode.OutPorts[i];
                    }
                }

            }

            // 2. Copy connections.
            foreach (var kv in old2new)
            {
                if(kv.Key is OutPort oldOutPort && kv.Value is OutPort newOutPort)
                {
                    if (oldOutPort.TargetPort == null) continue;
                    newOutPort.ConnectTo((old2new[oldOutPort.TargetPort] as InPort)!);
                }
            }

            if(_extraInfo != null)
                foreach (var key in _extraInfo.Keys)
                {
                    var extraData = _extraInfo[key];
                    extraData.CallbackInfoSet = extraData.CallbackInfoSet.Select(cbi => new ExtraInfoData.CallbackInfo()
                    {
                        Receiver= (old2new[cbi.Receiver] as NodeBase)!,
                        Callback = cbi.Callback
                    }).ToHashSet();
                    _extraInfo[key] = extraData;
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
            public IAsyncEnumerator<Status> routine;
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
                    // Log($"{port.Parent} ==[[{port.Ray?.ToString() ?? "null"}]]==> {node}");
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

        public void RegisterNodeToNextTick(NodeBase node)
        {
            lock(NodeActiveAtNextTick)
            {
                if (!NodeActiveAtNextTick.Contains(node)) // To prevent unlimited ActivateMeNextTick()
                {
                    NodeActiveAtNextTick.Add(node);
                    ActivateMeNextTick();
                }
            }
        }

        public virtual void RegisterRoutine(NodeBase node, IAsyncEnumerator<Status> routine)
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

        public override async Task<Status> OnStart()
        {
            await Task.WhenAll(nodeSet.Select(async (node) =>
            {
                var status = await node.OnStart();
                if (status.HasResult)
                {
                    SpreadPortRays(status.PortMask ?? node.OutPorts);
                }
            }).ToArray());

            SyncOutPortReflections();

            TurnOnRoutineIfNeeded();
            return Status.ShutdownAndEmitWith(outPortList.Where(p => p.ChangedSinceLastCheck));
        }

        public override async Task OnPause()
        {
            await base.OnPause();
            await Task.WhenAll(nodeSet.Select(node => node.OnPause()).ToArray());
        }

        public override async Task OnUnpause()
        {
            await base.OnUnpause();
            await Task.WhenAll(nodeSet.Select(node => node.OnUnpause()).ToArray());
        }

        public override async Task OnStop()
        {
            await base.OnStop();
            await Task.WhenAll(nodeSet.Select(node => node.OnStop()).ToArray());
        }


        public override Task<Status> OnActivated()
        {
            SyncInPortReflections();
            SpreadPortRays(inPortInnerReflectionList.Where(p => p.ChangedSinceLastCheck));
            SyncOutPortReflections();

            TurnOnRoutineIfNeeded();

            return Status.ShutdownAndEmitWithTask(outPortList.Where(p=>p.ChangedSinceLastCheck));
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



        protected async IAsyncEnumerator<Status> GetBoxRoutine()
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


                    if (cancelToken.IsCancellationRequested) yield return Status.Shutdown;

                    await Task.WhenAll(NodeActiveAtThisTick.Select(async node =>
                    {
                        var status = await node.OnActivated();
                        if (status.HasResult)
                        {
                            lock (portsToBeSpread)
                                foreach (var port in status.PortMask ?? node.OutPorts)
                                {
                                    portsToBeSpread.Add(port);
                                }
                        }
                    }).ToArray());
                    if (cancelToken.IsCancellationRequested) yield return Status.Shutdown;
                    NodeActiveAtThisTick.Clear();

                    (routineCached, routineCurrent)
                        =
                        (routineCurrent, routineCached);


                    await Task.WhenAll(routineCurrent.Select(async rc =>
                    {
                        var routine = rc.routine;
                        if (await routine.MoveNextAsync())
                        {
                            var status = routine.Current;
                            if (status.HasResult)
                            {
                                lock (portsToBeSpread)
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
                    }).ToArray());
                    routineCurrent.Clear();
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


        public struct ExtraInfoData
        {
            public struct CallbackInfo
            {
                public delegate void SetDataDelegate(NodeBase receiver, object? oldValue, object? newValue);
                /// <summary>
                /// Receiver will be sent to Callback if Callback is invoked. 
                /// </summary>
                public NodeBase Receiver { get; set; }
                public SetDataDelegate Callback { get; set; }

                public override bool Equals([NotNullWhen(true)] object? obj)
                {
                    if(obj is CallbackInfo other)
                        return (Receiver == other.Receiver) && (Callback == other.Callback);
                    return false;
                }

                public override int GetHashCode()
                {
                    unchecked
                    {
                        var hash1 = Receiver.GetHashCode();
                        var hash2 = Callback.GetHashCode();
                        hash2 = (hash2 << 16) | (hash2 >> 16);
                        return hash1 ^ hash2;
                    }
                }

                public static bool operator ==(CallbackInfo left, CallbackInfo right)
                {
                    return left.Equals(right);
                }

                public static bool operator !=(CallbackInfo left, CallbackInfo right)
                {
                    return !left.Equals(right);
                }
            }


            public ExtraInfoData()
            {
                data = null;
                CallbackInfoSet = new HashSet<CallbackInfo>();
            }

            public HashSet<CallbackInfo> CallbackInfoSet { get; set; }
            object? data;
            public object? Data
            {
                set
                {
                    foreach (var cbi in CallbackInfoSet)
                    {
                        cbi.Callback(cbi.Receiver, data, value);
                    }
                    data = value;
                }
                get
                {
                    return data;
                }
            }

            
            
            
        }

        Dictionary<string, ExtraInfoData>? _extraInfo = null;
        Dictionary<string, ExtraInfoData> ExtraInfo
        {
            get
            {
                if (_extraInfo == null) _extraInfo = new Dictionary<string, ExtraInfoData>();
                return _extraInfo;
            }
            set
            {
                _extraInfo = value;
            }
        }

        /// <summary>
        /// Register callbacks to link parameter of child node to parent box. <br/>
        /// Note: callbackInfo.Callback should only modify parameters of its first argument. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callbackInfo">Note: callbackInfo.Callback should only modify parameters of its first argument. </param>
        public void RegisterSetExtraInfoCallback(string key, ExtraInfoData.CallbackInfo callbackInfo)
        {
            if(!nodeSet.Contains(callbackInfo.Receiver) && callbackInfo.Receiver != this)
            {
                Log($"Receiver {callbackInfo.Receiver} is neither a child of {this} nor the box itself. ");
                return;
            }
            if (!ExtraInfo.ContainsKey(key)) ExtraInfo.Add(key, new ExtraInfoData());
            ExtraInfo[key].CallbackInfoSet.Add(callbackInfo);
        }

        public void UnregisterSetExtraInfoCallback(string key, ExtraInfoData.CallbackInfo callbackInfo)
        {
            if (ExtraInfo.TryGetValue(key, out var v))
            {
                v.CallbackInfoSet.Remove(callbackInfo);
            }
        }

        public void SetExtraInfo(string key, object? value)
        {
            if (string.IsNullOrEmpty(key)) return;
            if (ExtraInfo.TryGetValue(key, out var v))
            {
                v.Data = value;
            }
            else
            {
                ExtraInfo[key] = new ExtraInfoData() { Data = value };
            }
        }

        public object? GetExtraInfo(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            Box? curNode = this;
            while (curNode != null)
            {
                if (curNode._extraInfo != null && curNode._extraInfo.TryGetValue(key, out var res))
                {
                    return res.Data;
                }
                if (curNode.Parent is Box p)
                {
                    curNode = p;
                }
                else
                {
                    return null;
                }
            }
            return null;
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


        Space? currentSpaceCache;

        public override BaseObject? Parent { 
            get => base.Parent; 
            set
            {
                base.Parent = value;
                currentSpaceCache = base.GetCurrentSpace();
            }
        }

        public override Space? GetCurrentSpace()
        {
            if (currentSpaceCache == null)
            {
                currentSpaceCache = base.GetCurrentSpace();
            }
            return currentSpaceCache;
        }
    }
}
