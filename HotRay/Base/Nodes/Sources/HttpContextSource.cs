using HotRay.Base.Port;
using HotRay.Base.Ray.Hot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Sources
{
    public class HttpContextSource : SourceBase
    {
        private struct Parameters
        {
            public bool enableSecurity;
            public string uriPrefix;
        }

        Parameters p;

        public bool EnableSecurity { get => p.enableSecurity; set => p.enableSecurity = value; }

        /// <summary>
        /// Default: http://+:8080/ <br/><br/>
        /// Note: If the HTTPS is not working. It mostly means the SSL cirtification is not set in your machine. <br/>
        /// See <see cref="https://learn.microsoft.com/en-us/dotnet/framework/wcf/feature-details/how-to-create-temporary-certificates-for-use-during-development"/>
        /// </summary>
        public string URIPrefix { get => p.uriPrefix; set => p.uriPrefix = value; }


        OutPort<ObjectRay> outPort0;

        public HttpContextSource() : base() 
        {
            EnableSecurity = true;
            URIPrefix = "http://+:8080/";

            outPort0 = CreateOutPort<ObjectRay>();
            outPortList = new OutPort[] { outPort0 };
        }
        public HttpContextSource(HttpContextSource other) : base(other) 
        { 
            p = other.p;
            outPort0 = CreateOutPort<ObjectRay>();
            outPortList = new OutPort[] { outPort0 };
        }

        public override NodeBase CloneNode()
        {
            return new HttpContextSource(this);
        }

        public override async Task<Status> OnStart()
        {
            await base.OnStart();
            RunRoutine(AsSkipIfBusyRoutine(GetRoutine()));
            return Status.Shutdown;
        }


        async IAsyncEnumerator<Status> GetRoutine()
        {
            if (!HttpListener.IsSupported)
            {
                Log("HttpListener is not supported on this platform. ");
                yield return Status.Shutdown;
            }


            using HttpListener listener = new HttpListener();

            listener.Prefixes.Add(URIPrefix);
            listener.Start();

            // Log("Listening...");

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();

                outPort0.Ray = new ObjectRay() { Data = context };

                yield return Status.WaitForNextStepAndEmit;
            }
        }
    }
}
