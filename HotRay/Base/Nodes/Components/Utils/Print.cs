using HotRay.Base.Port;
using HotRay.Base.Ray;
using HotRay.Base.Ray.Hot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Utils
{
    public class Print<rayT>:OneZeroComponent<rayT> where rayT:RayBase
    {
        public Print() : base() { }
        public Print(Print<rayT> other) : base(other) 
        {
            Newline = other.Newline;
        }

        public bool Newline
        {
            get; set;
        }

        public override NodeBase CloneNode()
        {
            return new Print<rayT>(this);
        }

        public override Status OnActivated()
        {
            if (inPort0.Ray is rayT objRay)
            {
                if (Newline) Console.WriteLine(objRay.ToString() ?? "");
                else Console.Write(objRay.ToString() ?? "");
            }
            return Status.Shutdown;
        }

    }
}
