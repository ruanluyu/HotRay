using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Port
{
    public interface IPort
    {
        public IRay? Ray { get; set; }


        public bool ConnectableTo(IPort targetPort);

        public void ConnectTo(IPort targetPort);

        public void DisconnectTo(IPort targetPort);

        public void ClearConnections();

        public void SendRay();

        public IReadOnlyList<IPort> TargetPorts { get;}

        public IPort? SourcePort { get; set; }


    }

}
