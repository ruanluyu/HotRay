using HotRay.Base.Port;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes
{
    public interface INode
    {

        void Init();

        

        /// <summary>
        /// yield return false: no result and need continuously run at next step. <para />
        /// yield return true: has result and need continuously run at next step. <para />
        /// yield break or quit: deactivates node, will be activated again when any of the in-ports recieved data. <para />
        /// </summary>
        /// <returns>The routine</returns>
        IEnumerator<Status> GetRoutine();

        IReadOnlyList<PortBase> InPorts
        {
            get;
        }

        IReadOnlyList<PortBase> OutPorts
        {
            get;
        }

        public INode CloneNode();

    }
}
