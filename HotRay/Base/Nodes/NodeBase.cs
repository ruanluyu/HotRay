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
    public abstract class NodeBase: BaseObject, INode
    {

        protected static readonly IPort[] SharedEmptyPorts = new IPort[0];


        protected Port<rayT> CreatePort<rayT>() where rayT : IRay
        {
            var p = new Port<rayT>();
            p.Parent = this;
            return p;
        }

        protected Port<rayT> CreatePortFrom<rayT>(Port<rayT> other) where rayT : IRay
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

        public BaseObject BaseObject
        {
            get
            {
                return this;
            }
        }

        public virtual void Init()
        {
            InitInPorts();
            InitOutPorts();
        }


        public abstract IReadOnlyList<IPort> InPorts
        {
            get;
        }


        public abstract IReadOnlyList<IPort> OutPorts
        {
            get;
        }


        


        private static IPort? _GetPortByIndex(IReadOnlyList<IPort> list, int id)
        {
            if (id < 0 || id >= list.Count) return null;
            return list[id];
        }
        private static IPort? _GetPortByUID(IReadOnlyList<IPort> list, UIDType uid)
        {
            return list.FirstOrDefault(p => p.BaseObject.UID == uid);
        }

        private static IEnumerable<IPort> _GetPortsByName(IReadOnlyList<IPort> list, string name)
        {
            return list.Where(p => p.BaseObject.Name == name);
        }

        public virtual IPort? GetInPortByIndex(int id) => _GetPortByIndex(InPorts, id);
        public virtual IPort? GetOutPortByIndex(int id) => _GetPortByIndex(OutPorts, id);

        public virtual IPort? GetInPortByUID(UIDType uid) => _GetPortByUID(InPorts, uid);
        public virtual IPort? GetOutPortByUID(UIDType uid) => _GetPortByUID(OutPorts, uid);

        public virtual IEnumerable<IPort> GetInPortsByName(string name) => _GetPortsByName(InPorts, name);
        public virtual IEnumerable<IPort> GetOutPortsByName(string name) => _GetPortsByName(OutPorts, name);

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
                p.BaseObject.Parent = this;
            }
        }

        public virtual void InitOutPorts()
        {
            foreach (var p in OutPorts)
            {
                p.Ray = null;
                p.BaseObject.Parent = this;
            }
        }

        public abstract INode CloneNode();


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
        /// This function will return in the same tick.  <br/>
        /// This function may be called more than one times in the same tick. <br/>
        /// This function may be called more than one times with the same in-port. <br/><br/>
        /// If the node has lifetime more than one tick: <br/>
        /// Call <seealso cref="RunRoutine(IEnumerator{Status})"/> in this function. <br/>
        /// See also: <seealso cref="Base.Nodes.Components.Utils.Delayer{rayT}.OnPortUpdate"/>
        /// </summary>
        /// <param name="inport">The inport that holds new ray data. </param>
        /// <returns>
        /// Report is this node has results or not. <br />
        /// Supported returns: <br />
        /// 1. <seealso cref="Status.Shutdown"/>: No result <br />
        /// 2. <seealso cref="Status.ShutdownAndEmit"/>: Has result <br />
        /// 3. <seealso cref="Status.ShutdownAndEmitWith"/>: Has result <br />
        /// </returns>
        public virtual Status OnPortUpdate(IPort inport)
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


        protected static Status EmitSignalAndShutDown(Port<SignalRay> outport, bool isHigh)
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

    }
}
