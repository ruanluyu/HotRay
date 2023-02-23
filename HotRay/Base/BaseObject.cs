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
                LogEvent = null;
                if(_parent != null) LogEvent += _parent.Log;
            }
            get => _parent;
        }

        public struct UIDType : IComparable
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

            public int CompareTo(object? obj)
            {
                if(obj is UIDType uidobj)
                    return uid.CompareTo(uidobj.uid);
                return -1;
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



        public virtual T? GetNearestParent<T>(int skipCounter = 0) where T:BaseObject
        {
            BaseObject? curParent = Parent;
            while(curParent != null)
            {
                if (curParent is T b)
                {
                    if(skipCounter <= 0) 
                        return b;
                    skipCounter--;
                }
                curParent = curParent.Parent;
            }
            return null;
        }

        public string UIDPath
        {
            get
            {
                Stack<string> pathRecord = new Stack<string>();
                pathRecord.Push(UID.ToString());
                BaseObject? curParent = Parent;
                while (curParent != null)
                {
                    pathRecord.Push(curParent.UID.ToString());
                    curParent = curParent.Parent;
                }
                return string.Join("/",pathRecord);
            }
        }

    }
}
