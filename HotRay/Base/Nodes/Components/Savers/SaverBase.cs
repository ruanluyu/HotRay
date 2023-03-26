using HotRay.Base.Ray;
using HotRay.Base.Ray.Lite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Savers
{
    public abstract class SaverBase<rayT>:OneOneComponent<rayT, BoolRay> where rayT : RayBase
    {
        public SaverBase() : base()
        {
            FileName = "unknown";
            FileFolder = "~";
            Overwrite = false;
        }

        public SaverBase(SaverBase<rayT> other): base(other)
        {
            FileName = other.FileName;
            FileFolder = other.FileFolder;
            Overwrite = other.Overwrite;
        }


        protected abstract  Task<bool> OnSave(rayT ray);

        public override async Task<Status> OnActivated()
        {
            if (inPort0.ChangedSinceLastCheck)
            {
                if(inPort0.Ray is rayT ray)
                {
                    EmitRayTo(outPort0, new BoolRay() { Data = await OnSave(ray) });
                }
                else
                {
                    EmitRayTo(outPort0, null);
                }
            }
            else
            {
                EmitRayTo(outPort0, null);
            }
            return Status.ShutdownAndEmit;
        }

        public string FileName { set; get; }

        public string FileFolder { set; get; }

        public string FilePath
        {
            get
            {
                return Path.Combine(FileFolder, FileName);
            }
        }

        public bool Overwrite { set; get; }
    }
}
