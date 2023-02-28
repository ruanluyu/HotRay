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
    public class HttpContextSource : MTSTSourceBase
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

        public override async IAsyncEnumerator<Status> OnBigBangTask()
        {
            if (!HttpListener.IsSupported)
            {
                Log("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                yield return Status.Shutdown;
            }
            

            // Create a listener.
            using HttpListener listener = new HttpListener();
            // Add the prefixes.
            // URI prefixes are required,
            listener.Prefixes.Add(URIPrefix);
            listener.Start();
            Log("Listening...");
            while(true)
            {
                HttpListenerContext context = await listener.GetContextAsync();

                outPort0.Ray = new ObjectRay() { Data = context };

                yield return Status.WaitForNextStepAndEmit;
            }
        }
    }
}
