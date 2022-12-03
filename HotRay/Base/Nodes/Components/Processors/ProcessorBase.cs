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
        where coreT:ICore, new()
    {
        protected IPort[] inports;
        protected IPort[] outports;

        protected PropertyInfo[] inportProperties;
        protected PropertyInfo[] outportPorperties;

        protected coreT core;

        public ProcessorBase()
        {
            var t = typeof(coreT);

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            inportProperties = t.GetProperties(flags)
                .Where(p =>
                {
                    var a = p.GetCustomAttribute<InPortAttribute>();
                    return a != null && a.connectable && p.PropertyType.IsSubclassOf(typeof(RayBase));
                })
                .OrderBy(p => p.GetCustomAttribute<InPortAttribute>()!.index)
                .ToArray();

            outportPorperties = t.GetProperties(flags)
                .Where(p => p.GetCustomAttribute<OutPortAttribute>() != null && p.PropertyType.IsSubclassOf(typeof(RayBase)))
                .OrderBy(p => p.GetCustomAttribute<OutPortAttribute>()!.index)
                .ToArray();

            var gport = typeof(Port<>);

            inports = inportProperties
                .Select(p => (Activator.CreateInstance(gport.MakeGenericType(p.PropertyType)) as IPort)!)
                .ToArray()!;

            outports = outportPorperties
                .Select(p => (Activator.CreateInstance(gport.MakeGenericType(p.PropertyType)) as IPort)!)
                .ToArray()!;

            if (inports.Length <= 0) throw new ArgumentException($"{typeof(coreT)} does not contain connectable inports. ");

            core = new coreT();
        }

        private void _SendInPortRays()
        {
            for (int i = 0; i < inportProperties.Length; i++)
            {
                var p = inportProperties[i];
                p.SetValue(core, inports[i].Ray);
                inports[i].Ray = null;
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

        public override IEnumerator<Status> GetRoutine()
        {
            for (int i = 0; i < inports.Length; i++)
            {
                if (inports[i].Ray == null)
                    yield return Status.Shutdown;
            }
            _SendInPortRays();
            core.Process();
            _SendOutPortRays();
            yield return Status.EmitAndShutdown;
        }


        public override IPort[] InputPorts => inports;

        public override IPort[] OutputPorts => outports;
    }
}
