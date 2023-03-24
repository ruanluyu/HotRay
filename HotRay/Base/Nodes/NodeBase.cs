using HotRay.Base.Nodes.Components.Containers;
using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes
{
    public abstract class NodeBase: BaseObject, IDisposable
    {
        

        protected static readonly InPort[] SharedEmptyInPorts = new InPort[0];
        protected static readonly OutPort[] SharedEmptyOutPorts = new OutPort[0];

        protected InPort CreateInPortWithType(Type rayType)
        {
            if (!rayType.IsSubclassOf(typeof(RayBase))) throw new ArgumentException("rayType");
            var createInPortType = typeof(NodeBase).GetMethod(nameof(CreateInPort), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
            return (createInPortType.MakeGenericMethod(rayType).Invoke(this, null) as InPort)!;
        }
        protected OutPort CreateOutPortWithType(Type rayType)
        {
            if (!rayType.IsSubclassOf(typeof(RayBase))) throw new ArgumentException("rayType");
            var createOutPortType = typeof(NodeBase).GetMethod(nameof(CreateOutPort), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
            return (createOutPortType.MakeGenericMethod(rayType).Invoke(this, null) as OutPort)!;
        }

        protected InPort<rayT> CreateInPort<rayT>() where rayT : RayBase
        {
            var p = new InPort<rayT>();
            p.Parent = this;
            return p;
        }
        protected OutPort<rayT> CreateOutPort<rayT>() where rayT : RayBase
        {
            var p = new OutPort<rayT>();
            p.Parent = this;
            return p;
        }

        protected InPort<rayT> CreatePortFrom<rayT>(InPort<rayT> other) where rayT : RayBase
        {
            var p = new InPort<rayT>(other);
            p.Parent = this;
            return p;
        }

        protected OutPort<rayT> CreatePortFrom<rayT>(OutPort<rayT> other) where rayT : RayBase
        {
            var p = new OutPort<rayT>(other);
            p.Parent = this;
            return p;
        }



        public NodeBase() : base()
        {
        }

        public NodeBase(NodeBase other): base(other)
        {
        }


        public virtual void Init()
        {
            InitInPorts();
            InitOutPorts();
        }


        public abstract IReadOnlyList<InPort> InPorts
        {
            get;
        }


        public abstract IReadOnlyList<OutPort> OutPorts
        {
            get;
        }


        


        private static portT? _GetPortByIndex<portT>(IReadOnlyList<portT> list, int id) where portT : PortBase
        {
            if (id < 0 || id >= list.Count) return null;
            return list[id];
        }
        private static portT? _GetPortByUID<portT>(IReadOnlyList<portT> list, UIDType uid) where portT : PortBase
        {
            return list.FirstOrDefault(p => p.UID == uid);
        }

        private static IEnumerable<portT> _GetPortsByName<portT>(IReadOnlyList<portT> list, string name) where portT : PortBase
        {
            return list.Where(p => p.Name == name);
        }

        public virtual InPort? GetInPortByIndex(int id) => _GetPortByIndex(InPorts, id);
        public virtual OutPort? GetOutPortByIndex(int id) => _GetPortByIndex(OutPorts, id);

        public virtual InPort? GetInPortByUID(UIDType uid) => _GetPortByUID(InPorts, uid);
        public virtual OutPort? GetOutPortByUID(UIDType uid) => _GetPortByUID(OutPorts, uid);

        public virtual IEnumerable<InPort> GetInPortsByName(string name) => _GetPortsByName(InPorts, name);
        public virtual IEnumerable<OutPort> GetOutPortsByName(string name) => _GetPortsByName(OutPorts, name);



        public virtual void ClearInConnections()
        {
            foreach (var p in InPorts)
            {
                p?.SourcePort?.BreakConnection();
            }
        }
        public virtual void ClearOutConnections()
        {
            foreach (var p in OutPorts)
            {
                p?.BreakConnection();
            }
        }

        public virtual void InitInPorts()
        {
            foreach (var p in InPorts)
            {
                p.Ray = null;
                p.Parent = this;
            }
        }

        public virtual void InitOutPorts()
        {
            foreach (var p in OutPorts)
            {
                p.Ray = null;
                p.Parent = this;
            }
        }

        public abstract NodeBase CloneNode();


        /// <summary>
        /// Will be called when <seealso cref="Space.StartAndRunAsync"/> is called. <br/>
        /// This function will return in the same tick.  <br/><br/>
        /// If your node expects more than one tick: <br/>
        /// Call <seealso cref="RunRoutine(IEnumerator{Status})"/> in this function. <br/>
        /// See also: <seealso cref="Base.Nodes.Sources.PulseSource"/>. 
        /// </summary>
        /// <param name="inport"></param>
        /// <returns>
        /// Status that describes: 1. If the node have result; 2. OutPorts you want to emit. <br />
        /// Supported returns: <br />
        /// 1. <seealso cref="Status.Shutdown"/>: No result <br />
        /// 2. <seealso cref="Status.ShutdownAndEmit"/>: Has result <br />
        /// 3. <seealso cref="Status.ShutdownAndEmitWith"/>: Has result, and want to emit.  <br />
        /// </returns>
        public virtual Task<Status> OnStart()
        {
            return Status.ShutdownTask;
        }

        public virtual Task OnPause()
        {
            return Task.CompletedTask;
        }

        public virtual Task OnUnpause()
        {
            return Task.CompletedTask;
        }


        public virtual Task OnStop()
        {
            return Task.CompletedTask;
        }



        /// <summary>
        /// This function will be called before <seealso cref="OnActivated"/>. <br/>
        /// Cache parameters in this callback and NERVER do anything else here. 
        /// </summary>
        /*public virtual void OnCacheParameters()
        {

        }*/

        /// <summary>
        /// Will be called if any of the inports recieved rays. <br/>
        /// This function will be called after 1 tick the first time any in-ports' ray changed.  <br/>
        /// This function is called only one time in the same tick. <br/>
        /// If the node has lifetime more than one tick: <br/>
        /// Call <seealso cref="RunRoutine(IEnumerator{Status})"/> in this function. <br/>
        /// See also: <seealso cref="Base.Nodes.Components.Utils.Delayer{rayT}"/>
        /// </summary>
        /// <param name="inport">The inport that holds new ray data. </param>
        /// <returns>
        /// Status that describes: 1. If the node have result; 2. OutPorts you want to emit. <br />
        /// Supported returns: <br />
        /// 1. <seealso cref="Status.Shutdown"/>: No result <br />
        /// 2. <seealso cref="Status.ShutdownAndEmit"/>: Has result <br />
        /// 3. <seealso cref="Status.ShutdownAndEmitWith"/>: Has result, and want to emit.  <br />
        /// </returns>
        public virtual Task<Status> OnActivated()
        {
            return Status.ShutdownTask;
        }


        /// <summary>
        /// Register a routine to parent node. 
        /// </summary>
        /// <param name="routine">
        /// It is recommanded to design your routine with yield-return-scheme. <br />
        /// The routine registered will be immediately run at the same tick.  <br />
        /// Supported status: <br />
        /// 1. <seealso cref="Status.Shutdown"/> <br />
        /// 2. <seealso cref="Status.ShutdownAndEmit"/> <br />
        /// 3. <seealso cref="Status.ShutdownAndEmitWith"/> <br />
        /// 4. <seealso cref="Status.WaitForNextStep"/> <br />
        /// 5. <seealso cref="Status.WaitForNextStepAndEmit"/> <br />
        /// 6. <seealso cref="Status.WaitForNextStepAndEmitWith"/> <br /><br />
        /// Quick start reference: <seealso cref="Sources.PulseSource.GetRoutine"/><br />
        /// </param>
        protected void RunRoutine(IAsyncEnumerator<Status> routine)
        {
            Box.GetParentBoxOf(this)?.RegisterRoutine(this, routine);
        }


        protected void ActivateMeNextTick()
        {
            if(Parent is Box b) b.RegisterNodeToNextTick(this);
        }



        protected static Status EmitSignalTo(OutPort<SignalRay> outport, bool isHigh)
        {
            if(isHigh)
            {
                if (outport.Ray == null)
                {
                    outport.Ray = SignalRay.SharedSignal;
                    return Status.ShutdownAndEmit;
                }
            }
            else
            {
                if (outport.Ray != null)
                {
                    outport.Ray = null;
                    return Status.ShutdownAndEmit;
                }
            }
            return Status.Shutdown;
        }

        protected static Task<Status> EmitSignalToTask(OutPort<SignalRay> outport, bool isHigh)
            => Task.FromResult(EmitSignalTo(outport,isHigh));

        protected static Status EmitRayTo(OutPort outport, RayBase? ray)
        {
            if (outport.Ray != ray)
            {
                outport.Ray = ray;
                return Status.ShutdownAndEmitWith(outport);
            }
            return Status.Shutdown;
        }

        protected static Task<Status> EmitRayToTask(OutPort outport, RayBase? ray)
            => Task.FromResult(EmitRayTo(outport, ray));


        /// <summary>
        /// This function wraps a routine to a skippable routine. <br/><br/>
        /// A skippable routine is a routine that skips ticks when busy. <br/>
        /// e.g. When a routine is working with networking, the parent Box will be blocked until the networking task is done. 
        /// While it is wrapped as a "SkipIfBusyRoutine", the parent Box skips current tick and will try to get result in the next tick. <br/>
        /// See: <see cref="Sources.HttpContextSource.OnStart"/>
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        protected async IAsyncEnumerator<Status> AsSkipIfBusyRoutine(IAsyncEnumerator<Status>? routine)
        {
            if(routine == null)
            {
                yield return Status.Shutdown;
            }
            else
            {
                while (true)
                {
                    var task = routine.MoveNextAsync();
                    while (true)
                    {
                        if (task.IsCompleted)
                        {
                            if (task.Result)
                            {
                                yield return routine.Current;
                                break;
                            }
                            else
                            {
                                Log("Warnning: Use 'yield return Status.Shutdown' to close the routine instead of 'yield break'. ");
                                yield return Status.Shutdown;
                            }
                        }
                        else
                        {
                            yield return Status.WaitForNextStep;
                        }
                    }
                }
            }
        }




        HttpClient? _httpClient = null;

        public HttpClient HttpClient
        {
            get
            {
                if (_httpClient == null) _httpClient = new HttpClient();
                return _httpClient;
            }
            set
            {
                _httpClient = value;
            }
        }








        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ClearInConnections();
                    ClearOutConnections();
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }





        ~NodeBase()
        {
             // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
             Dispose(disposing: false);
        }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


    }
}
