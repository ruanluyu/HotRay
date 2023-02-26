using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray.Hot
{

    public class ImageRay<colorT> : MemoryRay<colorT>
        where colorT:struct
    {

        public ImageRay() : base() 
        { 
            width = height = 0; 
            channels = 4; 
        }

        public ImageRay(ImageRay<colorT> other) : base(other) 
        {
            width = other.width;
            height = other.height;
            channels = other.channels;
        }


        int width, height, channels;

        public virtual void SetSize(int width, int height, int channels = 4)
        {
            this.width = width;
            this.height = height;
            this.channels = channels;
            long len = width * height * channels;
            ResizeMemory(len);
        }

        public virtual int Width => Empty ? 0 : width;
        public virtual int Height => Empty ? 0 : height;
        public virtual int Channels => Empty ? 0 : channels;

        public virtual colorT Get(int x, int y, int channelID)
        {
            if (Data == null) return default;
            return Data[(y * width + x)* channels + channelID];
        }

        public virtual void Set(int x, int y, int channelID, colorT c)
        {
            if (Data == null) return;
            Data[(y * width + x) * channels + channelID] = c;
        }

        public virtual void Set(int width, int height, int channels, colorT[] data)
        {
            long l = width * height * channels;
            if (l != data.Length) throw new ArgumentException("Size mismatch");
            this.width = width;
            this.height = height;
            this.channels = channels;
            Data = data;
        }

        public virtual void Fill(colorT c)
        {
            if (Data == null) return;
            for (int i = 0; i < Data.Length; i++)
            {
                Data[i] = c;
            }
        }

        public override RayBase RayClone()
        {
            return new ImageRay<colorT>(this);
        }

    }
}
