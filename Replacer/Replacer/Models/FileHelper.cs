using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Replacer.Enums;

namespace Replacer.Models
{
    public static class FileHelper
    {
        public static void DeleteDirectoryAndFilesInThat(string path)
        {
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path);

                foreach (var file in files)
                {
                    File.Delete(file);
                }

                Directory.Delete(path);
            }
        }

        public static IList<byte[]> GetStreamAllFiles(string pathToTempFolder)
        {
            var fileList = new List<byte[]>();
            var files = Directory.GetFiles(pathToTempFolder);

            foreach (var file in files)
            {
                byte[] textByteArray = File.ReadAllBytes(file);
                fileList.Add(textByteArray);
            }

            return fileList;
        }

        public static void SaveNewFile(IList<byte[]> files, string connectionid)
        {
            var result = OpenAndCombine(files, connectionid);

            var date = DateTime.Now.ToString("yyyy.MM.dd hh-mm-ss.ffff");
            var folderToSaveFile = ConfigurationManager.AppSettings.Get("PathToSaveFile");

            if (String.IsNullOrWhiteSpace(folderToSaveFile))
                folderToSaveFile = "C:\\";

            if (!Directory.Exists(folderToSaveFile))
                Directory.CreateDirectory(folderToSaveFile);

            var filePath = $"{folderToSaveFile}\\Acts {date}.docx";

            using (var newFile = File.Create(filePath)) { }
            File.WriteAllBytes(filePath, result);
        }

        private static byte[] OpenAndCombine(IList<byte[]> documents, string connectionid)
        {
            MemoryStream mainStream = new MemoryStream();

            mainStream.Write(documents[0], 0, documents[0].Length);
            mainStream.Position = 0;

            int pointer = 1;
            byte[] ret;

            var reporter = new WebReporter();

            try
            {
                using (WordprocessingDocument mainDocument = WordprocessingDocument.Open(mainStream, true))
                {

                    XElement newBody = XElement.Parse(mainDocument.MainDocumentPart.Document.Body.OuterXml);

                    var documentsCount = documents.Count;
                    for (pointer = 1; pointer < documentsCount; pointer++)
                    {
                        reporter.SendProgress(documentsCount, pointer, TypeProgressBar.CobineActs, connectionid);

                        WordprocessingDocument tempDocument = WordprocessingDocument.Open(new MemoryStream(documents[pointer]), true);
                        XElement tempBody = XElement.Parse(tempDocument.MainDocumentPart.Document.Body.OuterXml);

                        newBody.Add(tempBody);
                        mainDocument.MainDocumentPart.Document.Body = new Body(newBody.ToString());
                        mainDocument.MainDocumentPart.Document.Save();
                        mainDocument.Package.Flush();
                    }
                    reporter.SendProgress(documentsCount, documentsCount, TypeProgressBar.CobineActs, connectionid);
                }
            }
            catch (OpenXmlPackageException oxmle)
            {
                reporter.AddError(oxmle.Message, connectionid);
                throw;
            }
            catch (Exception e)
            {
                reporter.AddError(e.Message, connectionid);
                throw;
            }
            finally
            {
                ret = mainStream.ToArray();
                mainStream.Close();
                mainStream.Dispose();
            }
            return (ret);
        }
    }
}
