using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.Drawing.Text;
using Microsoft.Extensions.Logging;

namespace ImageGenerator.Controllers
{
    /// <summary>
    /// Generator of Images
    /// </summary>
	[Route("api/[controller]")]
	public class GeneratorController : Controller
	{
        private enum Position {
            Bottom = 0,
            Top = 1
        }

        public ILogger<GeneratorController> Logger { get; }

        public GeneratorController(ILogger<GeneratorController> logger, IHostingEnvironment hostingEnvironment)
        {
            logger.LogInformation("****" + hostingEnvironment.ContentRootPath);
            Logger = logger;
        }

        /// <summary>
        /// Generate image for Pinterest
        /// </summary>
        /// <returns>Url to the resource</returns>
        /// <param name="file">Image</param>
        /// <param name="textTop">Text top</param>
        /// <param name="textBottom">Text bottom</param>
        /// <param name="height">Height of the text holders. Defaults 200px</param>
        /// 

        [HttpPost("/post-image")]
		public IActionResult Post(IFormFile file, string textTop, string textBottom, [DefaultValue(200)]int height = 200, float fontSize = 40.0f)
		{
            Logger.LogInformation("***Processing POST***");
            Bitmap image;
			using (var memoryStream = new MemoryStream())
			{
				file.CopyTo(memoryStream);
				image = new Bitmap(memoryStream);
			}

			Bitmap newImage = new Bitmap(image.Width, image.Height + height * 2);
			using (Graphics g = Graphics.FromImage(newImage))
			{
				// Draw base image
				Rectangle rect = new Rectangle(0, height, image.Width, image.Height);
				g.DrawImageUnscaledAndClipped(image, rect);

                setText(System.Net.WebUtility.HtmlDecode(textTop).Replace("\\n", "\r\n"), image, g, newImage, Position.Top, height, fontSize);
                setText(System.Net.WebUtility.HtmlDecode(textBottom).Replace("\\n", "\r\n"), image, g, newImage, Position.Bottom, height, fontSize);

                var guid = Guid.NewGuid();

                var extension = Path.GetExtension(file.FileName);

                newImage.Save("wwwroot/" + guid + extension);

                var resourceLocation = "http://" + Request.Host + "/" + guid + extension;

                return Created(resourceLocation, resourceLocation);

				//ImageConverter converter = new ImageConverter();
				//byte[] b = (byte[])converter.ConvertTo(newImage, typeof(byte[]));  
				//return File(b, "image/jpeg");
			}
		}


        private void setText(string text, Bitmap image, Graphics g, Bitmap newImage, Position position, int h, float fontSize) {

            var height = position == Position.Bottom ? image.Height + (h) : 0;

			g.FillRectangle(new SolidBrush(System.Drawing.ColorTranslator.FromHtml("#000000")), 0, height, newImage.Width, (h));

			PrivateFontCollection collection = new PrivateFontCollection();
            collection.AddFontFile(@"Times New Roman Bold.ttf");
			collection.AddFontFile(@"Times New Roman.ttf");

			// Create font
			Font font = new Font(collection.Families.First(), fontSize, FontStyle.Bold);

			// Create text position (play with positioning)
			PointF point = new PointF(0, image.Height);

			// Draw text
			RectangleF rec = new RectangleF(0, height, newImage.Width, (h));

			StringFormat sf = new StringFormat();
			sf.LineAlignment = StringAlignment.Center;
			sf.Alignment = StringAlignment.Center;

			g.DrawString(text, font, Brushes.White, rec, sf);
        }
	}
}
