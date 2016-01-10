using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eXml.Models;

namespace eXml.Abstractions
{
    public interface IConvertToXmlService
    {
        void ProcessExcelSheet(UploadFileModel model, string fileName, string savePath);
    }
}
