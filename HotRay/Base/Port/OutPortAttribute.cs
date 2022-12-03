using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Port
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OutPortAttribute : Attribute
    {
        public int index;
        public string? portName;
        public string? portDescription;
        public OutPortAttribute(int index, string? portName = null, string? portDescription = null)
        {
            this.index = index;
            this.portName = portName;
            this.portDescription = portDescription;
        }
    }
}
