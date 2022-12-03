using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Processors
{


    public interface ICore
    {
        public void Process();

        public void CopyFrom(ICore other);
    }
}
