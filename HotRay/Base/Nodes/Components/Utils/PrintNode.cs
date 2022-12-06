using HotRay.Base.Ray.Hot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Utils
{
    public class PrintNode:OneZeroComponent<ObjectRay>
    {
        public PrintNode() : base() { }
        public PrintNode(PrintNode other) : base(other) 
        {
            Newline = other.Newline;
        }

        public bool Newline
        {
            get;set;
        }

        public override INode CloneNode()
        {
            return new PrintNode(this);
        }

        public override IEnumerator<Status> GetRoutine()
        {
            if(inPort0.Ray is ObjectRay objRay)
            {
                if (Newline) Console.WriteLine(objRay.Data?.ToString() ?? "");
                else Console.Write(objRay.Data?.ToString() ?? "");
                yield return Status.Shutdown;
            }
        }
    }
}
