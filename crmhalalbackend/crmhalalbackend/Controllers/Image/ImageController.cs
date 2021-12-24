using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;
using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.File;
using CRMHalalBackEnd.repository;
using CRMHalalBackEnd.Repository;
using Swashbuckle.Swagger;
using WebApi.Jwt.Filters;

namespace CRMHalalBackEnd.Controllers.Image
{
    [JwtAuthentication]
    public class ImageController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();

        [HttpPost]
        [ResponseType(typeof(Response))]
        public async Task<IHttpActionResult> PostImageSave()
        {
            Response<List<Models.File.FileDto>> response;
            List<Models.File.File> images = new List<Models.File.File>();
            List<Models.File.FileDto> returnImages;
            var userId = _controllerActions.getUserId((ClaimsIdentity)User.Identity);
            var provider = new MultipartMemoryStreamProvider();
            var data = new byte[] { };
            string _groupId;
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
                var value = await provider.Contents[0].ReadAsByteArrayAsync();
                _groupId = System.Text.Encoding.Default.GetString(value);
                var imgRepo = new FileRepository();
                if (!Directory.Exists(System.Web.Hosting.HostingEnvironment.MapPath(string.Format("~/files/products/{0}", _groupId))))
                    Directory.CreateDirectory(System.Web.Hosting.HostingEnvironment.MapPath(string.Format("~/files/products/{0}", _groupId)));

                foreach (var providerContent in provider.Contents)
                {
                    if (providerContent.Headers.ContentDisposition.Name.Trim('\"').Equals("image"))
                    {
                        if (!providerContent.Headers.ContentType.MediaType.Split('/')[0].Equals("image"))
                        {
                            try
                            {
                                Directory.Delete(HostingEnvironment.MapPath(string.Format("~/files/products/{0}", _groupId)));
                            }
                            catch (Exception e)
                            {
                            }
                            throw new Exception("Incorrect media type");
                        }

                        Models.File.File img = new Models.File.File();
                        data = await providerContent.ReadAsByteArrayAsync();
                        img.OriginalFileName = providerContent.Headers.ContentDisposition.FileName.Trim('\"');
                        img.MimeType = providerContent.Headers.ContentType.MediaType;
                        img.Size = data.Length;
                        img.Extension = Path.GetExtension(img.OriginalFileName);
                        img.FileName = GenerateImageName();
                        img.FileType = img.MimeType.Split('/')[0];
                        img.Path = $"/files/products/{_groupId}/";
                        try
                        {
                            System.IO.File.WriteAllBytes(
                                System.Web.Hosting.HostingEnvironment.MapPath(
                                    string.Format("~/files/products/{0}/{1}{2}", _groupId, img.FileName,
                                        img.Extension)), data);
                        }
                        catch (Exception ex)
                        {
                        }
                        images.Add(img);
                    }
                }

                returnImages = imgRepo.ReturnInsertFileId(images, userId);


                response = new Response<List<Models.File.FileDto>>()
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = returnImages
                };

            }
            catch (Exception ex)
            {
                response = new Response<List<Models.File.FileDto>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }

            return Ok(response);
        }

        private string GenerateImageName()
        {
            string fileName = Guid.NewGuid().ToString();
            return fileName;
        }
    }
}
