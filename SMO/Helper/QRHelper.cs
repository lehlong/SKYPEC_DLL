using System;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;

using ZXing;
using ZXing.Common;

namespace SMO
{
    public static class QRHelper
    {
        public static IHtmlString GenerateQrCode(this HtmlHelper html, string value, string alt = "QR code", int height = 50, int width = 50, int margin = 0)
        {
            var qrWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions() { Height = height, Width = width, Margin = margin }
            };

            using (var q = qrWriter.Write(value))
            {
                using (var ms = new MemoryStream())
                {
                    q.Save(ms, ImageFormat.Png);
                    var img = new TagBuilder("img");
                    img.Attributes.Add("src", String.Format("data:image/png;base64,{0}", Convert.ToBase64String(ms.ToArray())));
                    img.Attributes.Add("alt", alt);
                    return MvcHtmlString.Create(img.ToString(TagRenderMode.SelfClosing));
                }
            }
        }

        public static string GenerateQrCode(string value, string alt = "QR code", int height = 50, int width = 50, int margin = 0)
        {
            var qrWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions() { Height = height, Width = width, Margin = margin }
            };

            using (var q = qrWriter.Write(value))
            {
                using (var ms = new MemoryStream())
                {
                    q.Save(ms, ImageFormat.Png);
                    var img = new TagBuilder("img");
                    img.Attributes.Add("src", String.Format("data:image/png;base64,{0}", Convert.ToBase64String(ms.ToArray())));
                    img.Attributes.Add("alt", alt);
                    return MvcHtmlString.Create(img.ToString(TagRenderMode.SelfClosing)).ToHtmlString();
                }
            }
        }
    }
}