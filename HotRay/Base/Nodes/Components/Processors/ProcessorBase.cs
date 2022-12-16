using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Processors
{
    public class ProcessorBase<coreT>:ComponentBase
        where coreT:class, ICore, new()
    {
        protected PortBase[] inports;
        protected PortBase[] outports;

        protected PropertyInfo[] inportProperties;
        protected PropertyInfo[] outportPorperties;

        protected coreT core;

        public ProcessorBase():base()
        {
            var t = typeof(coreT);

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            inportProperties = t.GetProperties(flags)
                .Where(p =>
                {
                    var a = p.GetCustomAttribute<InPortAttribute>();
                    return a != null && p.PropertyType.IsSubclassOf(typeof(RayBase));
                })
                .OrderBy(p => p.GetCustomAttribute<InPortAttribute>()!.index)
                .ToArray();

            outportPorperties = t.GetProperties(flags)
                .Where(p => p.GetCustomAttribute<OutPortAttribute>() != null && p.PropertyType.IsSubclassOf(typeof(RayBase)))
                .OrderBy(p => p.GetCustomAttribute<OutPortAttribute>()!.index)
                .ToArray();

            var gport = typeof(Port<>);
            var createPortType = typeof(NodeBase).GetMethod(nameof(CreatePort), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;

            inports = inportProperties
                .Select(p => {
                    var port = (createPortType.MakeGenericMethod(p.PropertyType).Invoke(this, null) as PortBase)!;
                    var attr = p.GetCustomAttribute<InPortAttribute>()!;
                    port.Name = attr.portName ?? $"inport-{attr.index}";
                    return port as PortBase;
                    })
                .ToArray()!;

            outports = outportPorperties
                .Select(p => {
                    var port = (createPortType.MakeGenericMethod(p.PropertyType).Invoke(this, null) as PortBase)!;
                    var attr = p.GetCustomAttribute<OutPortAttribute>()!;
                    port.Name = attr.portName ?? $"outport-{attr.index}";
                    return port as PortBase;
                })
                .ToArray()!;

            if (inports.Length <= 0) throw new ArgumentException($"{typeof(coreT)} does not contain connectable inports. ");

            core = new coreT();

            foreach (var p in inports.OfType<BaseObject>())
            {
                p.Parent = this;
            }
            
            foreach (var p in outports.OfType<BaseObject>())
            {
                p.Parent = this;
            }
        }

        public ProcessorBase(ProcessorBase<coreT> processor):this()
        {
            core = (processor.core.CloneCore() as coreT)!;
        }

        private void _SendInPortRays()
        {
            for (int i = 0; i < inportProperties.Length; i++)
            {
                var p = inportProperties[i];
                p.SetValue(core, inports[i].Ray);
                // inports[i].Ray = null;
            }
        }

        private void _SendOutPortRays()
        {

            for (int i = 0; i < outportPorperties.Length; i++)
            {
                var p = outportPorperties[i];
                outports[i].Ray = (p.GetValue(core) as RayBase)!;
            }
        }

        public override Status OnPortUpdate(PortBase inport)
        {
            if (inport.Ray == null) return Status.Shutdown;
            
            for (int i = 0; i < inports.Length; i++)
                if (inports[i].Ray == null)
                    return Status.Shutdown;

            _SendInPortRays();
            core.Process();
            _SendOutPortRays();
            
            return Status.ShutdownAndEmit;
        }


        public override NodeBase CloneNode()
        {
            return new ProcessorBase<coreT>();
        }

        public override IReadOnlyList<PortBase> InPorts => inports;

        public override IReadOnlyList<PortBase> OutPorts => outports;
    }
}
