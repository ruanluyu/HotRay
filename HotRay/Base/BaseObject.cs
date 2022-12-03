using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base
{
    public class BaseObject
    {
        static ulong _UIDCounter = 1;

        public ulong UID { get; }

        public virtual string Name { get; set; }

        public BaseObject()
        {
            UID = _UIDCounter++;
            Name = "";
        }

        public BaseObject(BaseObject other)
            :this()
        {
            Name = other.Name;
        }

    }
}
