using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Port
{
    public abstract class InPort : PortBase
    {
        public InPort() { 
            SourcePort = null;
            needConversion = false; 
        }
        public InPort(InPort other) : base(other)
        {
            SourcePort = null;
            needConversion = false;
        }


        OutPort? sourcePort;
        bool needConversion;
        public OutPort? SourcePort
        {
            get
            {
                return sourcePort;
            }
            set
            {
                needConversion = false;
                if (value == null)
                {
                    return;
                }
                var sourceType = value.RayType;
                var thisType = RayType;
                if(thisType != sourceType)
                {
                    needConversion = true;
                }
                sourcePort = value;
            }
        }



        public override RayBase? Ray 
        { 
            get
            {
                return base.Ray;
            }
            set 
            {
                if(value == null)
                {
                    base.Ray = null;
                }
                else
                {
                    if(needConversion)
                    {
                        base.Ray = value.CastTo(RayType);
                    }
                    else
                    {
                        base.Ray = value;
                    }
                }
            }
        }

    }


    public class InPort<rayT> : InPort where rayT:RayBase
    {
        public InPort() { }
        public InPort(InPort<rayT> other) : base(other)
        {
            SourcePort = null;
        }

        public override Type RayType => typeof(rayT);

        public override PortBase ClonePort()
        {
            var np = new InPort<rayT>(this);
            return np;
        }
    }
}
