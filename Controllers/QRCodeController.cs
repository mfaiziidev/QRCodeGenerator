using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using ZXing;
using ZXing.Common;

namespace QRCodeGenerator.Controllers
{
    public class QRCodeController : ApiController
    {
        [HttpGet]
        [BasicAuthentication] // Custom Basic Authentication filter
        public HttpResponseMessage Get(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Data parameter is required");
            }

            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 350,
                    Width = 350
                }
            };

            var bitmap = barcodeWriter.Write(data);

            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(ms.ToArray());
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                return response;
            }
        }
    }

    // Custom Basic Authentication filter
    public class BasicAuthenticationAttribute : System.Web.Http.Filters.AuthorizationFilterAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var authHeader = actionContext.Request.Headers.Authorization;

            if (authHeader != null && authHeader.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrEmpty(authHeader.Parameter))
            {
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter));
                var username = credentials.Split(':')[0];
                var password = credentials.Split(':')[1];

                // Validate credentials (e.g., check against a database or configuration)
                if (IsAuthorized(username, password))
                {
                    return; // Authorization successful
                }
            }

            // Unauthorized access
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate", "Basic");
        }

        private bool IsAuthorized(string username, string password)
        {
            // Example: Replace with your authentication logic
            return username == "faizan" && password == "faizan123";
        }
    }
}
