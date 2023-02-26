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
            dynamicConversion = null; 
        }
        public InPort(InPort other) : base(other)
        {
            SourcePort = null;
            dynamicConversion = null;
        }


        OutPort? sourcePort;
        Delegate? dynamicConversion;
        public OutPort? SourcePort
        {
            get
            {
                return sourcePort;
            }
            set
            {
                dynamicConversion = null;
                if (value == null)
                {
                    return;
                }
                var sourceType = value.RayType;
                var thisType = RayType;
                if(thisType != sourceType)
                {
                    var sourceTypeParam = Expression.Parameter(sourceType);
                    dynamicConversion = Expression.Lambda(Expression.Convert(sourceTypeParam, thisType), sourceTypeParam).Compile();
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
                    if(dynamicConversion == null)
                    {
                        base.Ray = value;
                    }
                    else
                    {
                        base.Ray = dynamicConversion.DynamicInvoke(value) as RayBase;
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
