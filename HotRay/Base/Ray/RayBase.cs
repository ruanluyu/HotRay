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


    public abstract class RayBase<dataT> : RayBase
    {
        public virtual dataT Data
        {
            get; set;
        }
    }
}
