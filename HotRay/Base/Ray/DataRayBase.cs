using HotRay.Base.Ray.Lite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray
{
    /*public abstract class DataRayBase : RayBase
    {
        public DataRayBase():base() { }

        public DataRayBase(DataRayBase other) : base(other) { }
    }*/


    public abstract class DataRayBase<dataT> : SignalRay
    {
        public abstract dataT Data
        {
            get; set;
        }

        public override Type? DataType()
        {
            return typeof(dataT);
        }

        public DataRayBase() : base() { }

        public DataRayBase(DataRayBase<dataT> other) : base(other) { }


    }
}
