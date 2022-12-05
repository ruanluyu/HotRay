using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray
{
    public class StringRay : RayBase<string?>
    {
        public StringRay() : base() { }
        public StringRay(StringRay other) : base(other) { Data = other.Data; }

        public override string? Data { set; get; }

        public override IRay RayClone()
        {
            return new StringRay(this);
        }

    }
}
