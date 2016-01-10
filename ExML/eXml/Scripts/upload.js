(function () {
    var DXUploadedFilesContainer = {
        nameCellStyle: "",
        sizeCellStyle: "",
        useExtendedPopup: false,

        AddFile: function (fileName, fileUrl, fileSize) {
            var self = DXUploadedFilesContainer;
            var builder = ["<tr>"];

            builder.push("<td class='nameCell'");
            if (self.nameCellStyle)
                builder.push(" style='" + self.nameCellStyle + "'");
            builder.push(">");
            self.BuildLink(builder, fileName, fileUrl);
            builder.push("</td>");

            builder.push("<td class='sizeCell'");
            if (self.sizeCellStyle)
                builder.push(" style='" + self.sizeCellStyle + "'");
            builder.push(">");
            builder.push(fileSize);
            builder.push("</td>");

            builder.push("</tr>");

            var html = builder.join("");
            DXUploadedFilesContainer.AddHtml(html);
        },
        Clear: function () {
            DXUploadedFilesContainer.ReplaceHtml("");
        },
        BuildLink: function (builder, text, url) {
            builder.push("<a target='blank' onclick='return DXDemo.ShowScreenshotWindow(event, this, " + this.useExtendedPopup + ");'");
            builder.push(" href='" + url + "'>");
            builder.push(text);
            builder.push("</a>");
        },
        AddHtml: function (html) {
            var fileContainer = document.getElementById("uploadedFilesContainer"),
                fullHtml = html;
            if (fileContainer) {
                var containerBody = fileContainer.tBodies[0];
                fullHtml = containerBody.innerHTML + html;
            }
            DXUploadedFilesContainer.ReplaceHtml(fullHtml);
        },
        ReplaceHtml: function (html) {
            var builder = ["<table id='uploadedFilesContainer' class='uploadedFilesContainer'><tbody>"];
            builder.push(html);
            builder.push("</tbody></table>");
            var contentHtml = builder.join("");
            FilesRoundPanel.SetContentHtml(contentHtml);
        },
        ApplySettings: function (nameCellStyle, sizeCellStyle, useExtendedPopup) {
            var self = DXUploadedFilesContainer;
            self.nameCellStyle = nameCellStyle;
            self.sizeCellStyle = sizeCellStyle;
            self.useExtendedPopup = useExtendedPopup;
        }
    };
    window.DXUploadedFilesContainer = DXUploadedFilesContainer;
})();