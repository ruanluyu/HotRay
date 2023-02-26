using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray.Hot
{

    public class ImageRay<channelT> : MemoryRay<channelT>
        where channelT:struct
    {

        public ImageRay() : base() 
        { 
            width = height = 0; 
            channels = 4; 
        }

        public ImageRay(ImageRay<channelT> other) : base(other) 
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

        public virtual channelT Get(int x, int y, int channelID)
        {
            if (Data == null) return default;
            return Data[(y * width + x)* channels + channelID];
        }

        public virtual void Set(int x, int y, int channelID, channelT c)
        {
            if (Data == null) return;
            Data[(y * width + x) * channels + channelID] = c;
        }

        /// <summary>
        /// Note: You may not write to data you sent after calling this function. 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="channels"></param>
        /// <param name="data"></param>
        /// <exception cref="ArgumentException"></exception>
        public virtual void Set(int width, int height, int channels, channelT[] data)
        {
            long l = width * height * channels;
            if (l != data.Length) throw new ArgumentException("Size mismatch");
            this.width = width;
            this.height = height;
            this.channels = channels;
            Data = data;
        }

        public virtual void Fill(channelT c)
        {
            if (Data == null) return;
            for (int i = 0; i < Data.Length; i++)
            {
                Data[i] = c;
            }
        }

        public override RayBase RayClone()
        {
            return new ImageRay<channelT>(this);
        }

    }
}
