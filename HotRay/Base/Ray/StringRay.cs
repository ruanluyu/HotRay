using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray
{
    public class StringRay : DataRayBase<string?>
    {
        public StringRay() : base() { }
        public StringRay(StringRay other) : base(other) { Data = other.Data; }

        public override string? Data { set; get; }

        public override RayBase RayClone()
        {
            return new StringRay(this);
        }
        public override string ToString()
        {
            var d = Data;
            if (d == null) return "null string";
            if (d == "") return "empty string";
            return d;
        }
    }
}
