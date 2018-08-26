using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;


using Discord;
using Discord.Commands;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;

namespace NewBotRate.Modules
{
    public class Images : ModuleBase<SocketCommandContext>
    {
        [Command("resize"), Alias("rz"), Summary("Resize a image")]
        public async Task ResizeImage([Summary("The Y/Width to rezize to")] int YVal,
            [Summary("The X/Height to resize to")] int XVal,
            [Summary("URL of the image to resize")] string URL)
        {
            await ReplyAsync("Processing... This might take some time.");
            Size imgSize = new Size(YVal, XVal);
            MemoryStream inMemStream = new MemoryStream(await NewBotRate.Utils.HelperFuncs.DownloadFileBytesAsync(URL));
            if(inMemStream != null)
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (ImageFactory imgFac = new ImageFactory(preserveExifData: true))
                    {
                        imgFac.Load(inMemStream)
                            .Resize(imgSize)
                            .Format(new JpegFormat { Quality = 70 })
                            .Save(outStream);
                    }
                    await Context.Channel.SendFileAsync(outStream, "resized.jpg", text:$"{outStream.Length/1024} KB");
                    return;
                }
            }
            await ReplyAsync("Something bad happened... Bad URL perhaps?");
            return;


        }

        [Command("jpeg"), Alias("jpegify"), Summary("JPEG an image")]
        public async Task Jpegify([Summary("URL to jpegify")] string URL, [Summary("JPEG quality")] int quality)
        {
            await ReplyAsync("Processing... This might take some time.");

            MemoryStream imgStream = new MemoryStream(await NewBotRate.Utils.HelperFuncs.DownloadFileBytesAsync(URL));
            if(imgStream != null)
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (ImageFactory imgFac = new ImageFactory(preserveExifData: true))
                    {
                        imgFac.Load(imgStream)
                            .Format(new JpegFormat { Quality = quality })
                            .Save(outStream);
                    }

                    await Context.Channel.SendFileAsync(outStream, "jpegified.jpg", text: $"Quality:{quality}  {outStream.Length / 1024}KB");
                    return;
                }
            }
            await ReplyAsync("Something bad happened... Bad URL perhaps?");
            return;
        }

    }
}
