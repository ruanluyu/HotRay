using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Port
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InPortAttribute : Attribute
    {
        public int index;
        public bool connectable;
        public string? portName;
        public string? portDescription;
        public InPortAttribute(int index, bool connectable = true, string? portName = null, string? portDescription = null)
        {
            this.index = index;
            this.connectable = connectable;
            this.portName = portName;
            this.portDescription = portDescription;
        }
    }
}
