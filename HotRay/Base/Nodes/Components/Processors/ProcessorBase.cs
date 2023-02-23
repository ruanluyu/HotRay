using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Processors
{
    public class ProcessorBase<coreT>:ComponentBase
        where coreT:class, ICore, new()
    {

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

            // var ginport = typeof(InPort<>);
            // var goutport = typeof(OutPort<>);

            // var createInPortType = typeof(NodeBase).GetMethod(nameof(CreateInPort), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
            // var createOutPortType = typeof(NodeBase).GetMethod(nameof(CreateInPort), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;

            inPortList = inportProperties
                .Select(p => {
                    // var port = (createInPortType.MakeGenericMethod(p.PropertyType).Invoke(this, null) as InPort)!;
                    var port = CreateInPortWithType(p.PropertyType);
                    var attr = p.GetCustomAttribute<InPortAttribute>()!;
                    port.Name = attr.portName ?? $"inport-{attr.index}";
                    return port;
                    })
                .ToArray()!;

            outPortList = outportPorperties
                .Select(p => {
                    // var port = (createOutPortType.MakeGenericMethod(p.PropertyType).Invoke(this, null) as OutPort)!;
                    var port = CreateOutPortWithType(p.PropertyType);
                    var attr = p.GetCustomAttribute<OutPortAttribute>()!;
                    port.Name = attr.portName ?? $"outport-{attr.index}";
                    return port;
                })
                .ToArray()!;

            if (inPortList.Length <= 0) throw new ArgumentException($"{typeof(coreT)} does not contain connectable inports. ");

            core = new coreT();

            foreach (var p in inPortList)
            {
                p.Parent = this;
            }
            
            foreach (var p in outPortList)
            {
                p.Parent = this;
            }
        }

        public ProcessorBase(ProcessorBase<coreT> processor):this()
        {
            core = (processor.core.CloneCore() as coreT)!;
        }

        private void _SendInPortRaysToCore()
        {
            for (int i = 0; i < inportProperties.Length; i++)
            {
                var p = inportProperties[i];
                p.SetValue(core, inPortList[i].Ray);
                // inports[i].Ray = null;
            }
        }

        private void _SendOutPortRaysFromCore()
        {

            for (int i = 0; i < outportPorperties.Length; i++)
            {
                var p = outportPorperties[i];
                outPortList[i].Ray = (p.GetValue(core) as RayBase)!;
            }
        }

        public override Status OnActivated()
        {
            for (int i = 0; i < inPortList.Length; i++)
                if (inPortList[i].Ray == null)
                    return Status.Shutdown;

            _SendInPortRaysToCore();
            core.Process();
            _SendOutPortRaysFromCore();
            
            return Status.ShutdownAndEmit;
        }


        public override NodeBase CloneNode()
        {
            return new ProcessorBase<coreT>();
        }
    }
}
