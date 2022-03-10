using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Kremis.Utility.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kremis.Utility.Helpers
{
    public class FileHelper
    {
        public static List<T> GetJsonData<T>(string fullFileName)
        {
            //string data = File.ReadAllText(fullFileName, Encoding.GetEncoding("iso-8859-1"));
            string data = File.ReadAllText(fullFileName);
            List<T> list = JsonSerializer.Deserialize<List<T>>(data);
            return list;
        }

        public static string GetFolderName(DocumentOwnerEnum documentOwnerEnum, string webRootPath)
        {
            string folderName = string.Empty;
            if (documentOwnerEnum == DocumentOwnerEnum.Customer)
            {
                folderName = Path.Combine(webRootPath, ConstantHelper.DEFAULT_CUSTOMER_DOCUMENT_FOLDER_NAME);
            }
            else if (documentOwnerEnum == DocumentOwnerEnum.LandTitle)
            {
                folderName = Path.Combine(webRootPath, ConstantHelper.DEFAULT_LAND_TITLE_DOCUMENT_FOLDER_NAME);
            }
            else if (documentOwnerEnum == DocumentOwnerEnum.Parcel)
            {
                folderName = Path.Combine(webRootPath, ConstantHelper.DEFAULT_PARCEL_DOCUMENT_FOLDER_NAME);
            }
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);

            return folderName;
        }

        public static void CreateFile(IFormFile file, DocumentOwnerEnum documentOwnerEnum, string webRootPath, string previousFileName = null)
        {
            if (file == null)
                return;

            string fileName = $"{file.FileName}";
            string folderName = GetFolderName(documentOwnerEnum, webRootPath);

            if (previousFileName != null)
            {
                var previousPath = Path.Combine(folderName, previousFileName); 
                if (System.IO.File.Exists(previousPath))
                {
                    System.IO.File.Delete(previousPath);
                }
            }

            var newPath = Path.Combine(folderName, fileName);
            using var filesStreams = new FileStream(newPath, FileMode.Create);
            file.CopyTo(filesStreams);
        }

        public static void DeleteFile(DocumentOwnerEnum documentOwnerEnum, string webRootPath, string fileName)
        {
            var folderName = FileHelper.GetFolderName(documentOwnerEnum, webRootPath);
            var documentPath = Path.Combine(folderName, fileName);
            if (System.IO.File.Exists(documentPath))
            {
                System.IO.File.Delete(documentPath);
            }
        }

        public static IActionResult DiplayPDF(DocumentOwnerEnum documentOwnerEnum, string webRootPath, string fileName)
        {
            var folderName = FileHelper.GetFolderName(documentOwnerEnum, webRootPath);
            var documentPath = Path.Combine(folderName, fileName);

            byte[] pdfBytes = System.IO.File.ReadAllBytes(documentPath);
            MemoryStream ms = new(pdfBytes);
            return new FileStreamResult(ms, "application/pdf");
        }
    }
}
