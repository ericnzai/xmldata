using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eXml.Abstractions;
using eXml.Models;

namespace eXml.Services
{
    public class XmlProvider
    {
        IConvertToXmlService _xmlConverter = null;
        UploadFileModel _model = null;
        string _fileName = "";
        string _savePath = "";

        public XmlProvider(IConvertToXmlService xmlConverter, UploadFileModel model,
            string fileName, string savePath)
        {
            this._xmlConverter = xmlConverter;
            this._model = model;
            this._fileName = fileName;
            this._savePath = savePath;
        }
        public void ConvertToXml()
        {
            _xmlConverter.ProcessExcelSheet(_model, _fileName, _savePath);
        }
    }
}