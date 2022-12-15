using HotRay.Base.Nodes.Components.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base
{
    public class BaseObject
    {
        private BaseObject? _parent;
        public BaseObject? Parent { 
            set
            {
                _parent = value;
                if(_parent == null)
                {
                    LogEvent = null;
                }
                else
                {
                    LogEvent += _parent.Log;
                }
            }
            get => _parent;
        }

        public struct UIDType
        {
            static ulong counter = 1;
            ulong uid;
            public static UIDType Create()
            {
                return new UIDType() { uid = unchecked(counter++) };
            }

            public static bool operator ==(UIDType l, UIDType r) => l.Equals(r);
            public static bool operator !=(UIDType l, UIDType r) => !l.Equals(r);

            public override bool Equals(object? obj)
            {
                if (obj is UIDType u) return uid == u.uid;
                return false;
            }

            public override int GetHashCode()
            {
                return uid.GetHashCode();
            }

            public override string ToString()
            {
                return uid.ToString();
            }
        }

        

        public UIDType UID { get; }

        public virtual string Name { get; set; }

        public BaseObject()
        {
            UID = UIDType.Create();
            Name = "";
        }

        public BaseObject(BaseObject other)
            :this()
        {
            Name = other.Name;
            Parent = other.Parent;
        }


        public override string? ToString()
        {
            var display = string.IsNullOrEmpty(Name) ? this.GetType().Name : Name;
            return $"{display}<uid: {UID}>";
        }

        public override int GetHashCode()
        {
            return UID.GetHashCode();
        }


        public event Action<string>? LogEvent;
        protected virtual void Log(string message)
        {
            if (LogEvent == null) return;
            LogEvent.Invoke($"{this}: {message}");
        }


        

    }
}
