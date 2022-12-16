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



        protected HashSet<NodeBase> nodeSet = new HashSet<NodeBase>();
        protected Queue<RoutineContext> runThisTickRoutines = new Queue<RoutineContext>();
        protected Queue<RoutineContext> runNextTickRoutines = new Queue<RoutineContext>();


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

        public virtual IEnumerable<NodeBase> GetNodesByName(string name) => nodeSet.Where(n => n.BaseObject.Name == name);


        public virtual NodeBase? GetNodeByUID(UIDType uid) => nodeSet.Where(n=>n.BaseObject.UID == uid).FirstOrDefault();




        HashSet<PortBase> portsHasResult = new HashSet<PortBase>();
        bool _running = false;

        
        public override void Init()
        {
            base.Init();


            runThisTickRoutines.Clear();
            runNextTickRoutines.Clear();

            _cancel = false;
            _running = false;
            portsHasResult.Clear();

            foreach (var node in nodeSet)
                node.Init();

        }


        Queue<PortBase> _spreadBFS = new Queue<PortBase>();

        void _SpreadPortRaysRecursively()
        {
            while (_spreadBFS.TryDequeue(out var ip))
            {
                if (!_PassNodeLimitationCheck())
                {
                    _spreadBFS.Clear();
                    return;
                }
                ip.SendRay();
                if (ip.TargetPort?.BaseObject.Parent is NodeBase node)
                {
                    if (node == this)
                    {
                        portsHasResult.Add(ip.TargetPort);
                    }
                    else
                    {
                        var status = node.OnPortUpdate(ip.TargetPort);
                        if (status.HasResult)
                        {
                            IEnumerable<PortBase> targetPorts = status.PortMask ?? node.OutPorts;
                            foreach (var op in targetPorts)
                            {
                                _spreadBFS.Enqueue(op);
                            }
                        }
                        _doneNode++;
                    }
                }
            }
        }


        protected void SpreadPortRays(IEnumerable<PortBase> ports)
        {
            _spreadBFS.Clear();

            foreach (var ip in ports)
                _spreadBFS.Enqueue(ip);

            _SpreadPortRaysRecursively();
        }


        void SpreadPortRays(params PortBase[] ports)
        {
            SpreadPortRays(ports as IReadOnlyList<PortBase>);
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

        public override Status OnPortUpdate(PortBase inport)
        {
            SpreadPortRays(inport);
            TurnOnRoutine();
            var a = portsHasResult.ToArray();
            portsHasResult.Clear();
            return Status.ShutdownAndEmitWith(a);
        }

        
        void TurnOnRoutine()
        {
            if (!_running)
            {
                _running = true;
                RunRoutine(GetRoutine());
            }
        }

        int _doneNode = 0;
        int _maxNode = -1;

        bool _PassNodeLimitationCheck()
        {
            if(_cancel)
            {
                Log($"Cancellation detected, quiting ray-spread loop... ");
                return false;
            }
            if (_maxNode >= 0 && _doneNode >= _maxNode)
            {
                Log($"Exceed limitation {_maxNode}. ");
                return false;
            }
            return true;
        }

        protected RoutineType GetRoutine()
        {
            try
            {
                _maxNode = MaxNodePerTick;
                do
                {
                    portsHasResult.Clear();
                    _doneNode = 0;
                    while (true)
                    {
                        if (_cancel)
                        {
                            Log($"Canceled running. ");
                            yield break;
                        }
                        if(!_PassNodeLimitationCheck())
                        {
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
                            _doneNode++;
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

    }
}
