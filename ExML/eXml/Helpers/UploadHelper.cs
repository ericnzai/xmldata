using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;

namespace eXml.Helpers
{
    public class UploadHelper
    {
        public const string UploadDirectory = "~/Content/Uploads/";

        public static void ucDragAndDrop_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {
                string fileName = Path.ChangeExtension(Path.GetRandomFileName(), ".jpg");
                string resultFilePath = UploadDirectory + fileName;
                using (Image original = Image.FromStream(e.UploadedFile.FileContent))
                using (Image thumbnail = ImageUtils.CreateThumbnailImage((Bitmap)original, ImageSizeMode.ActualSizeOrFit, new Size(350, 350)))
                {
                    ImageUtils.SaveToJpeg((Bitmap)thumbnail, HttpContext.Current.Request.MapPath(resultFilePath));
                }
                UploadingUtils.RemoveFileWithDelay(fileName, HttpContext.Current.Request.MapPath(resultFilePath), 5);
                IUrlResolutionService urlResolver = sender as IUrlResolutionService;
                if (urlResolver != null)
                    e.CallbackData = urlResolver.ResolveClientUrl(resultFilePath);
            }
        }
        public static void ucMultiSelection_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            string resultFileName = Path.GetRandomFileName() + "_" + e.UploadedFile.FileName;
            string resultFileUrl = UploadDirectory + resultFileName;
            string resultFilePath = HttpContext.Current.Request.MapPath(resultFileUrl);
            e.UploadedFile.SaveAs(resultFilePath);

            UploadingUtils.RemoveFileWithDelay(resultFileName, resultFilePath, 5);

            IUrlResolutionService urlResolver = sender as IUrlResolutionService;
            if (urlResolver != null)
            {
                string name = e.UploadedFile.FileName;
                string url = urlResolver.ResolveClientUrl(resultFileUrl);
                long sizeInKilobytes = e.UploadedFile.ContentLength / 1024;
                string sizeText = sizeInKilobytes.ToString() + " KB";
                e.CallbackData = name + "|" + url + "|" + sizeText;
            }
        }
        public static readonly UploadControlValidationSettings UploadValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".xls", ".xlsx" },
            MaxFileSize = 4194304
        };
    }
}