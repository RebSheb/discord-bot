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
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;


namespace NewBotRate.Modules
{
    public class OpenCV : ModuleBase<SocketCommandContext>
    {
        private CascadeClassifier _haarCascadeFaces = new CascadeClassifier("haarcascade_frontalface_default.xml");
        private CascadeClassifier _haarCascadeEyes = new CascadeClassifier("haarcascade_eye.xml");


        [Command("detectfaces"), Alias("detectf"), Summary("Detect faces in an image")]
        public async Task DetectFaces([Summary("URL for image")] string URL)
        {
            await ReplyAsync("Processing... This might take some time.");
            
            MemoryStream imgStream = new MemoryStream(await NewBotRate.Utils.HelperFuncs.DownloadFileBytesAsync(URL));

            if(imgStream != null)
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    NewBotRate.Utils.HelperFuncs.GetImageFactory(imgStream)
                        .Save(outStream);

                    Bitmap bmp = new Bitmap(outStream);

                    
                    Emgu.CV.Image<Bgr, Byte> img = new Emgu.CV.Image<Bgr, Byte>(bmp);
                    var gray = img.Convert<Gray, Byte>();

                    var faces = _haarCascadeFaces.DetectMultiScale(gray); //*, 1.1, 12, Size.Empty/*);
                    if(faces.Length == 0)
                    {
                        await ReplyAsync("No faces detected :(");
                        return;
                    }
                    foreach(var face in faces)
                    {
                        img.Draw(face, new Bgr(System.Drawing.Color.Red), 3);
                    }

                    await Context.Channel.SendFileAsync(new MemoryStream(img.ToJpegData()), "faces.jpg", text: $"Found {faces.Length} faces...");
                }
            }
        }

        [Command("detecteyes"), Alias("eyes","detecteyes"), Summary("Detect eyes in an image")]
        public async Task DetectEyes([Summary("URL for image")] string URL)
        {
            await ReplyAsync("Processing... This might take some time.");

            MemoryStream imgStream = new MemoryStream(await NewBotRate.Utils.HelperFuncs.DownloadFileBytesAsync(URL));

            if (imgStream != null)
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    NewBotRate.Utils.HelperFuncs.GetImageFactory(imgStream)
                        .Save(outStream);

                    Bitmap bmp = new Bitmap(outStream);


                    Emgu.CV.Image<Bgr, Byte> img = new Emgu.CV.Image<Bgr, Byte>(bmp);
                    var gray = img.Convert<Gray, Byte>();

                    var eyes = _haarCascadeEyes.DetectMultiScale(gray); //*, 1.1, 12, Size.Empty/*);
                    if (eyes.Length == 0)
                    {
                        await ReplyAsync("No eyes detected :(");
                        return;
                    }
                    foreach (var eye in eyes)
                    {
                        img.Draw(eye, new Bgr(System.Drawing.Color.Red), 3);
                    }

                    await Context.Channel.SendFileAsync(new MemoryStream(img.ToJpegData()), "eyes.jpg", text: $"Found {eyes.Length} faces...");
                }
            }
        }
    }
}
