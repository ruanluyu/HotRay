﻿using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Utils
{
    public class NotGate : OneOneComponent<SignalRay, SignalRay>
    {
        public NotGate() : base() { }
        public NotGate(NotGate other) : base(other) { }
        public override NodeBase CloneNode()
        {
            return new NotGate(this);
        }

        public override Status OnEntry()
        {
            outPort0.Ray = SignalRay.SharedSignal;
            return Status.ShutdownAndEmit;
        }

        public override Status OnPortUpdate(PortBase inport)
        {
            return EmitSignalTo(outPort0, inport.Ray == null);
        }
    }
}
