using HotRay.Base.Nodes.Components.Containers;
using HotRay.Base.Nodes.Components.Filters;
using HotRay.Base.Nodes.Components.Processors;
using HotRay.Base.Nodes.Components.Utils;
using HotRay.Base.Nodes.Sources;
using HotRay.Base.Ray.Lite;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotRay.Base.Ray.Hot;
using HotRay.Base.Nodes.Components.Savers;

namespace HotRay.Examples
{
    /// <summary>
    /// Download a picture and save it to local. 
    /// </summary>
    internal static class Test4LoadImage
    {

        public static void Run()
        {
            Space space = new Space() { TicksPerSecond = 20, PrintTickInfo = false };
            space.LogEvent += s => Console.WriteLine(s);

            var pulse = space.CreateNode<PulseSource>();
            var imageFilter = space.CreateNode<SignalToImage>();
            var imageSaver = space.CreateNode<ImageSaver>();


            pulse.Interval = 0;
            pulse.Count = 1;

            // The artwork is from the anime "Made In Abyss" by @Ryousuke_Nak and @tukushiA in Twitter. 
            imageFilter.ImagePath = "https://pw.yuelili.com/wp-content/uploads/2023/02/thumb-1920-880845.jpg";
            imageFilter.CacheImage = true;
            imageSaver.FileFolder = "";
            imageSaver.Overwrite = true;
            imageSaver.FileName = "lyza-and-riko";
            imageSaver.EncodeFormat = ImageSaver.Format.PNG;

            pulse.OutPorts[0].ConnectTo(imageFilter.InPorts[0]);
            imageFilter.OutPorts[0].ConnectTo(imageSaver.InPorts[0]);
            

            
            var task = space.BigBangAsync();
            task.Wait();

        }
    }
}
