using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray
{
    public interface IRay
    {
        public IRay RayClone();

        public string? ToString();
    }



}
