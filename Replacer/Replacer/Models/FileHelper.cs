using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        public static void SaveNewFile(IList<byte[]> files)
        {
            var result = OpenAndCombine(files);

            var date = DateTime.Now.ToString("yyyy.MM.dd hh-mm-ss.ffff");
            //TODO (Where save a new file
            var filePath = $"{Environment.CurrentDirectory.Split(':')[0]}:\\Acts {date}.docx";

            using (var newFile = File.Create(filePath)) { }
            File.WriteAllBytes(filePath, result);
        }

        private static byte[] OpenAndCombine(IList<byte[]> documents)
        {
            MemoryStream mainStream = new MemoryStream();

            mainStream.Write(documents[0], 0, documents[0].Length);
            mainStream.Position = 0;

            int pointer = 1;
            byte[] ret;
            try
            {
                using (WordprocessingDocument mainDocument = WordprocessingDocument.Open(mainStream, true))
                {

                    XElement newBody = XElement.Parse(mainDocument.MainDocumentPart.Document.Body.OuterXml);

                    for (pointer = 1; pointer < documents.Count; pointer++)
                    {
                        if (pointer % 5 == 0)
                        {
                            Console.Write(".");
                        }

                        WordprocessingDocument tempDocument = WordprocessingDocument.Open(new MemoryStream(documents[pointer]), true);
                        XElement tempBody = XElement.Parse(tempDocument.MainDocumentPart.Document.Body.OuterXml);

                        newBody.Add(tempBody);
                        mainDocument.MainDocumentPart.Document.Body = new Body(newBody.ToString());
                        mainDocument.MainDocumentPart.Document.Save();
                        mainDocument.Package.Flush();
                    }
                }
            }
            catch (OpenXmlPackageException oxmle)
            {
                Console.WriteLine($"Error while merging files: Document index {0}. {oxmle.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while merging files: Document index {0}. {e.Message}");
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
