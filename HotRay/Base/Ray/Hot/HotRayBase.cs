using HotRay.Base.Ray.Lite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray.Hot
{
    public abstract class HotRayBase<dataT> : DataRayBase<dataT?>
        where dataT : class
    {
        public HotRayBase() : base() { }
        public HotRayBase(HotRayBase<dataT> other) : base(other) {  }

        public override dataT? Data { get; set; }

        public override string ToString()
        {
            return Data?.ToString() ?? "";
        }


        private bool disposedValue;
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Data = null;
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
            base.Dispose(disposing);
        }
    }
}
