using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DragonTracker.Helpers
{
    public static class ImageHelper
    {
        public static async Task<byte[]> EncodeImageAsync(IFormFile image)
        {
            if (image == null)
                return null;
            using var ms = new MemoryStream();
            await image.CopyToAsync(ms);
            return ms.ToArray();
        }
        public static string DecodeImage(byte[] image, string fileName)
        {
            var binary = Convert.ToBase64String(image);
            var ext = Path.GetExtension(fileName);
            return $"data:image/{ext};base64,{binary}";
        }
        public static string GetImageFileName(IFormFile image)
        {
            return image == null ? null : image.FileName;
        }


    }
}
