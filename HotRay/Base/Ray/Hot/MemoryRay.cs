using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray
{
    public class MemoryRay : RayBase<byte[]>
    {
        static readonly byte[] sharedEmptyArray = new byte[0];

        public MemoryRay()
        {
            Data = sharedEmptyArray;
        }

        public override object Clone()
        {
            return new MemoryRay() { Data = Data.ToArray() };
        }
    }
}
