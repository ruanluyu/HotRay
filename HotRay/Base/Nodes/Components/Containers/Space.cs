using HotRay.Base.Nodes.Sources;
using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Containers
{
    public class Space:Box
    {
        public Space():base() { }
        public Space(Space other):base(other) 
        {
            MsPerTick = other.MsPerTick;
            PrintTick = other.PrintTick;
        }

        public override INode CloneNode()
        {
            return new Space(this);
        }

        public double MsPerTick
        {
            get;set;
        }
        public bool PrintTick
        {
            get;set;
        }

        public Task Run()
        {
            return Task.Run(async () =>
            {
                var routine = GetSpaceRoutine();
                int ms = (int)Math.Round(MsPerTick);
                while (routine.MoveNext())
                {
                    await Task.Delay(ms);
                }
            });
        }

        IEnumerator GetSpaceRoutine()
        {
            foreach (var source in nodeSet.OfType<SourceBase>())
            {
                var status = source.OnEntry();
                if(status.HasResult)
                {
                    SpreadPortRays(source.OutPorts);
                }
            }
            var routine = GetRoutine();
            long tick = 0;
            while(routine.MoveNext())
            {
                if (PrintTick) Console.WriteLine($"Tick {tick++} done. ");
                yield return null;
            }
        }


    }
}
