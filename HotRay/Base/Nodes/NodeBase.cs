using HotRay.Base.Nodes.Components.Containers;
using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes
{
    public abstract class NodeBase: BaseObject, IDisposable
    {

        protected static readonly PortBase[] SharedEmptyPorts = new PortBase[0];
        

        protected Port<rayT> CreatePort<rayT>() where rayT : RayBase
        {
            var p = new Port<rayT>();
            p.Parent = this;
            return p;
        }

        protected Port<rayT> CreatePortFrom<rayT>(Port<rayT> other) where rayT : RayBase
        {
            var p = new Port<rayT>(other);
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


        public abstract IReadOnlyList<PortBase> InPorts
        {
            get;
        }


        public abstract IReadOnlyList<PortBase> OutPorts
        {
            get;
        }


        


        private static PortBase? _GetPortByIndex(IReadOnlyList<PortBase> list, int id)
        {
            if (id < 0 || id >= list.Count) return null;
            return list[id];
        }
        private static PortBase? _GetPortByUID(IReadOnlyList<PortBase> list, UIDType uid)
        {
            return list.FirstOrDefault(p => p.UID == uid);
        }

        private static IEnumerable<PortBase> _GetPortsByName(IReadOnlyList<PortBase> list, string name)
        {
            return list.Where(p => p.Name == name);
        }

        public virtual PortBase? GetInPortByIndex(int id) => _GetPortByIndex(InPorts, id);
        public virtual PortBase? GetOutPortByIndex(int id) => _GetPortByIndex(OutPorts, id);

        public virtual PortBase? GetInPortByUID(UIDType uid) => _GetPortByUID(InPorts, uid);
        public virtual PortBase? GetOutPortByUID(UIDType uid) => _GetPortByUID(OutPorts, uid);

        public virtual IEnumerable<PortBase> GetInPortsByName(string name) => _GetPortsByName(InPorts, name);
        public virtual IEnumerable<PortBase> GetOutPortsByName(string name) => _GetPortsByName(OutPorts, name);

        public virtual void ClearInConnections()
        {
            foreach (var p in InPorts)
            {
                p.ClearConnections();
            }
        }
        public virtual void ClearOutConnections()
        {
            foreach (var p in OutPorts)
            {
                p.ClearConnections();
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
        /// Will be called when <seealso cref="Space.Run"/>. <br/>
        /// This function will return in the same tick.  <br/><br/>
        /// Processing more than one tick: <br/>
        /// Call <seealso cref="RunRoutine(IEnumerator{Status})"/> in this function. <br/>
        /// See also: <seealso cref="Base.Nodes.Sources.PulseSource"/>
        /// </summary>
        /// <param name="inport"></param>
        /// <returns>
        /// Report is this node has results and want to send them to connected nodes. <br />
        /// Supported returns: <br />
        /// 1. <seealso cref="Status.Shutdown"/>: No result <br />
        /// 2. <seealso cref="Status.ShutdownAndEmit"/>: Has result <br />
        /// 3. <seealso cref="Status.ShutdownAndEmitWith"/>: Has result <br />
        /// </returns>
        public virtual Status OnEntry()
        {
            return Status.Shutdown;
        }


        /// <summary>
        /// Will be called if any of the inports recieved rays. <br/>
        /// This function will be called after 1 tick the first time any in-ports' ray changed.  <br/>
        /// This function may be called more than one times in the same tick. <br/>
        /// This function may be called more than one times with the same in-port. <br/><br/>
        /// If the node has lifetime more than one tick: <br/>
        /// Call <seealso cref="RunRoutine(IEnumerator{Status})"/> in this function. <br/>
        /// See also: <seealso cref="Base.Nodes.Components.Utils.Delayer{rayT}"/>
        /// </summary>
        /// <param name="inport">The inport that holds new ray data. </param>
        /// <returns>
        /// Report is this node has results or not. <br />
        /// Supported returns: <br />
        /// 1. <seealso cref="Status.Shutdown"/>: No result <br />
        /// 2. <seealso cref="Status.ShutdownAndEmit"/>: Has result <br />
        /// 3. <seealso cref="Status.ShutdownAndEmitWith"/>: Has result <br />
        /// </returns>
        public virtual Status OnActivated()
        {
            return Status.Shutdown;
        }


        /// <summary>
        /// Register a routine to parent node. 
        /// </summary>
        /// <param name="routine">
        /// It is recommanded to design your routine with yield-return-scheme. <br />
        /// Supported status: <br />
        /// 1. <seealso cref="Status.Shutdown"/> <br />
        /// 2. <seealso cref="Status.ShutdownAndEmit"/> <br />
        /// 3. <seealso cref="Status.ShutdownAndEmitWith"/> <br />
        /// 4. <seealso cref="Status.WaitForNextStep"/> <br />
        /// 5. <seealso cref="Status.WaitForNextStepAndEmit"/> <br />
        /// 6. <seealso cref="Status.WaitForNextStepAndEmitWith"/> <br /><br />
        /// Quick start reference: <seealso cref="Sources.PulseSource.GetRoutine"/><br />
        /// </param>
        protected void RunRoutine(IEnumerator<Status> routine)
        {
            if (Parent is Box b)
            {
                b.RegisterRoutine(this, routine);
            }
        }




        protected static Status EmitSignalTo(Port<SignalRay> outport, bool isHigh)
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

        protected static Status EmitRayTo(PortBase outport, RayBase? ray)
        {
            if (outport.Ray != ray)
            {
                outport.Ray = ray;
                return Status.ShutdownAndEmit;
            }
            return Status.Shutdown;
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
