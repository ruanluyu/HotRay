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
        private struct Parameters
        {
            public bool newline;
            public string? format;
        }

        Parameters exposed, cached;

        public Print() : base() 
        {
            Newline = true;
            Format = null;
        }
        public Print(Print<rayT> other) : base(other) 
        {
            exposed = other.exposed;
        }

        public override void OnCacheParameters()
        {
            base.OnCacheParameters();
            cached = exposed;
        }

        public bool Newline
        {
            get=>exposed.newline; set=>exposed.newline = value;
        }

        /// <summary>
        /// This option will be treated as an <seealso cref="IFormatProvider"/> to format a string. <br/>
        /// e.g. To add "The cat said: " in front of any message. 
        /// Set this to "The cat said: {0}" <br/>
        /// See also: <seealso cref="string.Format(IFormatProvider?, string, object?)"/>
        /// </summary>
        public string? Format
        {
            get => exposed.format; set => exposed.format = value;
        }

        public override NodeBase CloneNode()
        {
            return new Print<rayT>(this);
        }

        public override Task<Status> OnActivated()
        {
            if (inPort0.Ray is rayT objRay)
            {
                string info = objRay.ToString() ?? "null";
                if (cached.format != null) string.Format(cached.format, info);
                if (cached.newline) Console.WriteLine(info);
                else Console.Write(info);
            }
            return Status.ShutdownTask;
        }

    }
}
