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
            public string? template;
        }

        Parameters p;

        public Print() : base() 
        {
            Newline = true;
            Format = null;
            
        }
        public Print(Print<rayT> other) : base(other) 
        {
            p = other.p;
        }

        /*public override void OnCacheParameters()
        {
            base.OnCacheParameters();
            cached = exposed;
        }*/

        public bool Newline
        {
            get=>p.newline; set=>p.newline = value;
        }

        /// <summary>
        /// Used to format a ray to a string <br/>
        /// e.g. "F6", "-5,F2"
        /// </summary>
        public string? Format
        {
            get => p.format; set => p.format = value;
        }

        /// <summary>
        /// Used to insert formatted-string to a line. <br/>
        /// e.g. "PrintNode1 said: {0}"
        /// </summary>
        public string? Template
        {
            get => p.template; set => p.template = value;
        }

        public override NodeBase CloneNode()
        {
            return new Print<rayT>(this);
        }

        public override Task<Status> OnActivated()
        {
            if (inPort0.Ray is rayT objRay)
            {
                string info = p.format == null ? (objRay.ToString() ?? "null") : (objRay.ToString(p.format) ?? "null");
                
                if (p.template != null) info = string.Format(p.template, info);

                if (p.newline) Console.WriteLine(info);
                else Console.Write(info);
            }
            return Status.ShutdownTask;
        }

    }
}
