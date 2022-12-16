using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray
{
    public abstract class RayBase:IDisposable
    {
        

        public RayBase(): base() { }

        public RayBase(RayBase other): this() { }


        public abstract RayBase RayClone();


        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        
        ~RayBase()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }


    public abstract class RayBase<dataT> : RayBase
    {
        public abstract dataT Data
        {
            get; set;
        }

        public RayBase() : base() { }

        public RayBase(RayBase<dataT> other) : base(other) { }


    }

    
}
