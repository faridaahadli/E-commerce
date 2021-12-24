using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;
using Castle.Core.Internal;
using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.File;
using CRMHalalBackEnd.Repository;
using FileLibrary;
using FileLibrary.FileInter;
using Swashbuckle.Swagger;
using File = CRMHalalBackEnd.Models.File.File;

namespace CRMHalalBackEnd.Controllers.Image
{

    [JwtRoleAuthentication(Actor = "Company")]
    // todo Burda JwtRoleAuthentication olcag amma getActiveUserId ve UserId deyisilmelidi
    public class FileController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly StoreRepository _storeRepository = new StoreRepository();
        private readonly FileRepository _fileRepository = new FileRepository();
        //private readonly List<string> _fileTypeList = new List<string>() { "image", "excel", "word", "pdf" };
        //private readonly List<string> _allowedFileType = new List<string>() { "jpeg", "jpg", "png", /*"webp", "svg",*/"gif", "doc", "docx", "xls", "xlsx", "pdf" };
        // todo icazeli filetype-lari oyrenib qeyd etmek ve en yaxsi usulu tapmaq .
        [HttpPost]
        [ResponseType(typeof(Response))]
        public async Task<IHttpActionResult> PostFileSave()
        {
            Response<List<FileDto>> response = null;
            string imageType = String.Empty;
            string imageData = String.Empty;
            int errorUploadFile = 0;
            string userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var data = new { files = new List<File>(), datas = new List<byte[]>() };
            var provider = new MultipartMemoryStreamProvider();
            if (!Request.Content.IsMimeMultipartContent())
                throw new Exception("Dəstəklənməyən Format !!!");

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (var providerContent in provider.Contents)
                {
                    if (providerContent.Headers.ContentDisposition.Name.Trim('\"').Equals("image_type"))
                    {
                        // watermark,product
                        var val = await providerContent.ReadAsByteArrayAsync();
                        var valStr = Encoding.Default.GetString(val);
                        imageType = valStr;
                    }

                    if (providerContent.Headers.ContentDisposition.Name.Trim('\"').Equals("image_data"))
                    {
                        var val = await providerContent.ReadAsByteArrayAsync();
                        var valStr = Encoding.Default.GetString(val);
                        // {products/groupId , shops/shopId/category , shops/shopId/slider , shops/shopId/logo}
                        imageData = valStr;
                    }

                    if (providerContent.Headers.ContentDisposition.Name.Trim('\"').Equals("image"))
                    {
                        var byteArr = await providerContent.ReadAsByteArrayAsync();
                        File file = new File();
                        file.OriginalFileName = providerContent.Headers.ContentDisposition.FileName.Trim('\"');
                        file.MimeType = providerContent.Headers.ContentType.MediaType;
                        file.Size = byteArr.Length;
                        file.Extension = Path.GetExtension(file.OriginalFileName).ToLower();
                        file.FileType = file.MimeType.Split('/')[0];
                        file.Path = $"/files/store/{tenantId}/{imageType}/{(!imageData.IsNullOrEmpty() ? imageData + "/" : "")}";
                        if (!FileTypeCheck.IsFileValidation(file.Extension.Split('.').Last(), byteArr))
                        {
                            errorUploadFile++;
                            continue;
                        }
                        if (file.Extension.Equals(".gif") && imageType.Equals("products"))
                        {
                            errorUploadFile++;
                            continue;
                        }

                        if (!FileTypeCheck.IsFileValidation(file.Extension.Split('.').Last(), byteArr))
                        {
                            errorUploadFile++;
                            continue;
                        }

                        if (file.Extension.Equals(".gif") && imageType.Equals("products"))
                        {
                            errorUploadFile++;
                            continue;
                        }

                        data.files.Add(file);
                        data.datas.Add(byteArr);
                    }
                }

                var returnFile = FileInsert(imageType, imageData, data, userId);
                try
                {
                    response = new Response<List<FileDto>>()
                    {
                        Code = (int)HttpStatusCode.Created,
                        Success = true,
                        Data = returnFile,
                        Message = errorUploadFile.ToString()
                    };
                }
                catch
                {
                    // ignored
                }
            }
            catch (Exception ex)
            {
                response = new Response<List<FileDto>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            return Ok(response);
        }

        [HttpPost]
        [ResponseType(typeof(Response))]
        public async Task<IHttpActionResult> PostAllOfficeFile()
        {
            Response<List<FileDto>> response = null;
            string officeFile = String.Empty;
            string userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var data = new { files = new List<File>(), datas = new List<byte[]>() };
            var provider = new MultipartMemoryStreamProvider();
            if (!Request.Content.IsMimeMultipartContent())
                throw new Exception("Dəstəklənməyən Format !!!");

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (var providerContent in provider.Contents)
                {

                    if (providerContent.Headers.ContentDisposition.Name.Trim('\"').Equals("officeFile"))
                    {
                        var byteArr = await providerContent.ReadAsByteArrayAsync();
                        File file = new File();
                        file.OriginalFileName = providerContent.Headers.ContentDisposition.FileName.Trim('\"');
                        file.MimeType = providerContent.Headers.ContentType.MediaType;
                        file.Size = byteArr.Length;
                        file.Extension = Path.GetExtension(file.OriginalFileName).ToLower();
                        file.FileType = file.MimeType.Split('/')[0];
                        file.Path = $"/files/store/{tenantId}/mail/";

                        data.files.Add(file);
                        data.datas.Add(byteArr);
                    }
                }

                var returnFile = AllOfficeFileInsert(data, userId);
                try
                {
                    response = new Response<List<FileDto>>()
                    {
                        Code = (int)HttpStatusCode.Created,
                        Success = true,
                        Data = returnFile
                    };
                }
                catch
                {
                    // ignored
                }
            }
            catch (Exception ex)
            {
                response = new Response<List<FileDto>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            return Ok(response);
        }
        [HttpPost]
        [ResponseType(typeof(Response))]
        public async Task<IHttpActionResult> PostFileSaveExcel()
        {
            Response<List<FileDto>> response = null;
            string excel = String.Empty;
            string userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var data = new { files = new List<File>(), datas = new List<byte[]>() };
            var provider = new MultipartMemoryStreamProvider();
            if (!Request.Content.IsMimeMultipartContent())
                throw new Exception("Dəstəklənməyən Format !!!");

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (var providerContent in provider.Contents)
                {

                    if (providerContent.Headers.ContentDisposition.Name.Trim('\"').Equals("excel"))
                    {
                        var byteArr = await providerContent.ReadAsByteArrayAsync();
                        File file = new File();
                        file.OriginalFileName = providerContent.Headers.ContentDisposition.FileName.Trim('\"');
                        file.MimeType = providerContent.Headers.ContentType.MediaType;
                        file.Size = byteArr.Length;
                        file.Extension = Path.GetExtension(file.OriginalFileName).ToLower();
                        file.FileType = file.MimeType.Split('/')[0];
                        file.Path = $"/files/store/{tenantId}/excel/";

                        data.files.Add(file);
                        data.datas.Add(byteArr);
                    }
                }

                var returnFile = FileInsertExcel(data, userId);
                try
                {
                    response = new Response<List<FileDto>>()
                    {
                        Code = (int)HttpStatusCode.Created,
                        Success = true,
                        Data = returnFile
                    };
                }
                catch
                {
                    // ignored
                }
            }
            catch (Exception ex)
            {
                response = new Response<List<FileDto>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            return Ok(response);
        }
        private List<FileDto> FileInsert(string imageType, string imageData, dynamic data, string userId)
        {
            List<File> files = data.files;
            List<byte[]> datas = data.datas;
            int size = files.Count;
            if (size != datas.Count)
                throw new HttpResponseException(HttpStatusCode.InternalServerError);

            for (int i = 0; i < size; i++)
            {
                File file = files[i];
                file.FileName = GenerateFileName(); /*file.OriginalFileName.Split('.')[0];*/
                byte[] byteArr = datas[i];
                ImageFile image = new ImageFile();
                // todo Burada file -in contenine gore yoxlanmasi olacag

                if (imageType.Equals("watermark"))
                {
                    image.Load(byteArr, file.Extension).Resize(250).Optimize(60).Save(HostingEnvironment.MapPath($"~/{file.Path}"),
                        file.FileName);
                }
                else if (imageType.Equals("products"))
                {
                    string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
                    file.Extension = ".jpg";
                    FileDto watermarkPath = _storeRepository.GetWatermark(tenantId);
                    if (watermarkPath != null)
                    {
                        MemoryStream watermark =
                            new MemoryStream(
                                System.IO.File.ReadAllBytes(HostingEnvironment.MapPath("~" + watermarkPath.FilePath)));
                        image.Load(byteArr, file.Extension).Resize(600).AddWatermark(watermark.ToArray()).Optimize(60).ChangeBackground(Color.White).Save(HostingEnvironment.MapPath($"~/{file.Path}"),
                            file.FileName);
                    }
                    else
                    {
                        image.Load(byteArr, file.Extension).Resize(600).Optimize(60).ChangeBackground(Color.White).Save(HostingEnvironment.MapPath($"~/{file.Path}"),
                            file.FileName);
                    }
                }
                else if (imageType.Equals("slider"))
                {
                    image.Load(byteArr, file.Extension).Resize(1320).Optimize(70).Save(HostingEnvironment.MapPath($"~/{file.Path}"),
                        file.FileName);
                }

                else if (imageType.Equals("categoryIcon"))
                {
                    image.Load(byteArr, file.Extension).Resize(100).Optimize(55).Save(HostingEnvironment.MapPath($"~/{file.Path}"),
                        file.FileName);
                }
                else if (imageType.Equals("gridIconId"))
                {
                    image.Load(byteArr, file.Extension).Resize(500).Optimize(70).Save(HostingEnvironment.MapPath($"~/{file.Path}"),
                        file.FileName);
                }
                else if (imageType.Equals("favicon"))
                {
                    image.Load(byteArr, file.Extension).Resize(40).Optimize(55).Save(HostingEnvironment.MapPath($"~/{file.Path}"),
                        file.FileName);
                }
                else if (imageType.Equals("designbg"))
                {
                    image.Load(byteArr, file.Extension).Resize(1400).Optimize(60).Save(HostingEnvironment.MapPath($"~/{file.Path}"),
                        file.FileName);
                }
                else if (imageType.Equals("g-promo"))
                {
                    image.Load(byteArr, file.Extension).Resize(600).Optimize(60).Save(HostingEnvironment.MapPath($"~/{file.Path}"),
                        file.FileName);
                }
                else if (imageType.Equals("logo") || imageType.Equals("mainLogo"))
                {
                    image.Load(byteArr, file.Extension).Resize(500).Optimize(80).Save(HostingEnvironment.MapPath($"~/{file.Path}"),
                        file.FileName);
                }
                else if (imageType.Equals("socialMediaLogo"))
                {
                    image.Load(byteArr, file.Extension).Resize(75).Optimize(70).Save(HostingEnvironment.MapPath($"~/{file.Path}"),
                        file.FileName);
                }
            }



            return _fileRepository.ReturnInsertFileId(files, userId);
        }

        private List<FileDto> FileInsertExcel(dynamic data, string userId)
        {
            List<File> files = data.files;
            List<byte[]> datas = data.datas;

            int size = files.Count;
            if (size != datas.Count)
                throw new HttpResponseException(HttpStatusCode.InternalServerError);

            for (int i = 0; i < size; i++)
            {
                File file = files[i];
                file.FileName = GenerateFileName(); /*file.OriginalFileName.Split('.')[0];*/
                byte[] byteArr = datas[i];
                ExcelFile excel = new ExcelFile();
                // todo Burada file -in contenine gore yoxlanmasi olacag
                if (!FileTypeCheck.IsFileValidation(file.Extension.Split('.').Last(), byteArr))
                {
                    deleteFileFromFileName(files);
                    throw new Exception("Yuklənən File-da Xəta var");
                }

                excel.Load(byteArr, file.Extension).Save(HostingEnvironment.MapPath($"~/{file.Path}"),
                        file.FileName);

            }
            return _fileRepository.ReturnInsertFileId(files, userId);
        }


        private List<FileDto> AllOfficeFileInsert(dynamic data, string userId)
        {
            List<File> files = data.files;
            List<byte[]> datas = data.datas;

            int size = files.Count;
            if (size != datas.Count)
                throw new HttpResponseException(HttpStatusCode.InternalServerError);

            for (int i = 0; i < size; i++)
            {
                File file = files[i];
                file.FileName = GenerateFileName(); /*file.OriginalFileName.Split('.')[0];*/
                byte[] byteArr = datas[i];
                AllOfficeFile ofiiceFile = new AllOfficeFile();
                // todo Burada file -in contenine gore yoxlanmasi olacag
                if (!FileTypeCheck.IsFileValidation(file.Extension.Split('.').Last(), byteArr))
                {
                    deleteFileFromFileName(files);
                    throw new Exception("Yuklənən File-da Xəta var");
                }

                ofiiceFile.Load(byteArr, file.Extension).Save(HostingEnvironment.MapPath($"~/{file.Path}"),
                        file.FileName);

            }
            return _fileRepository.ReturnInsertFileId(files, userId);
        }
        private void deleteFileFromFileName(List<File> files)
        {
            foreach (var file in files)
            {
                try
                {
                    if (System.IO.File.Exists(HostingEnvironment.MapPath($"~{file.Path}{file.FileName}{file.Extension}")))
                        System.IO.File.Delete(HostingEnvironment.MapPath($"~{file.Path}{file.FileName}{file.Extension}"));
                }
                catch (Exception ex)
                {
                    throw new Exception("Daxili xəta baş verdi");
                }
            }
        }

        private string GenerateFileName()
        {
            string fileName = Guid.NewGuid().ToString();
            return fileName;
        }
    }
}
