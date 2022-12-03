using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray
{
    public abstract class RayBase : IRay
    {

        public abstract object Clone();
    }


    public class RayBase<dataT> : RayBase
        where dataT: notnull
    {
        public virtual dataT Data
        {
            get; set;
        }

        public override object Clone()
        {
            return new RayBase<dataT>() { Data = Data };
        }
    }

    
}
