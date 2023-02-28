using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Utils
{
    public class Spread<rayT>: ComponentBase
        where rayT:RayBase
    {


        protected readonly InPort<rayT> inPort0;

        public Spread():this(2)
        {
        }

        public Spread(int count):base()
        {
            inPort0 = CreateInPort<rayT>();
            inPortList = new InPort[] { inPort0 };
            PortNum = count;
        }


        public Spread(Spread<rayT> other) :base(other)
        {
            inPort0 = CreateInPort<rayT>();
            inPortList = new InPort[] { inPort0 };
            PortNum = other.PortNum;
        }

        public int PortNum
        {
            get => outPortList.Length;
            set
            {
                if (value < 0) 
                {
                    PortNum = 0;
                    return;
                }
                ResetOutPortNum<rayT>(value);
            }
        }


        public override NodeBase CloneNode()
        {
            return new Spread<rayT>(this);
        }

        public override Task<Status> OnActivated()
        {
            if (PortNum == 0) return Status.ShutdownTask;


            var refRay = inPort0.Ray;
            // inPort0.Ray = null;
            if (refRay == outPortList[0].Ray) return Status.ShutdownTask;

            outPortList[0].Ray = refRay;
            for (int i = 1; i < outPortList.Length; i++)
                outPortList[i].Ray = refRay?.RayClone();

            return Status.ShutdownAndEmitTask;
        }

    }
}
