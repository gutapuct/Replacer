using ActsConsoleKirill.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Xml.Linq;

namespace ActsConsoleKirill.Models
{
    public class ActMaker
    {
        private readonly IReporter reporter;
        private readonly DateTime dateFrom;
        private readonly DateTime dateTo;
        private readonly int amountActs;

        private readonly string pathToFolderTemplates;
        private readonly string pathToResult;

        public const string CorrectTemplateFileFormat = "docx";
        public const string PlaceholderNumber = "number";
        public const string PlaceholderDate = "date";
        private const string Separator = "!!!";

        private List<string> pathTemplates;
        private int indexCurrentTemplate = 0;

        public ActMaker(IReporter reporter)
        {
            this.reporter = reporter;

            pathToFolderTemplates = ConfigurationManager.AppSettings["folderTemplatesPath"];
            if (String.IsNullOrWhiteSpace(pathToFolderTemplates))
            {
                throw new Exception(@"Неверно указан путь до папки с шаблонами. Пример: ""C:\templates""");
            }

            pathToResult = ConfigurationManager.AppSettings["folderResult"];
            if (String.IsNullOrWhiteSpace(pathToResult))
            {
                throw new Exception(@"Неверно указан путь для итогового документа. Пример: ""C:\Мои акты""");
            }

            if (!int.TryParse(ConfigurationManager.AppSettings["amountActs"], out amountActs))
            {
                throw new Exception(@"Неверно указано количество актов. Пример: ""10000"" (без пробелов)");
            }

            dateFrom = GetDateTimeValue(ConfigurationManager.AppSettings["dateFrom"]);
            dateTo = GetDateTimeValue(ConfigurationManager.AppSettings["dateTo"]);
        }

        public void Run()
        {
            var amountDays = (dateTo - dateFrom).TotalDays + 1;
            var amountActsPerDay = amountActs / amountDays;
            var pathToTempFolder = $"{Environment.CurrentDirectory.Split(':')[0]}:\\temp\\Acts_temp_{Guid.NewGuid().ToString()}";

            pathTemplates = GetPathTemplates();

            try
            {
                if (!Directory.Exists(pathToTempFolder))
                {
                    Directory.CreateDirectory(pathToTempFolder);
                }

                for (var i = 0; i < amountActs; i++)
                {
                    CreateAct(i, amountActsPerDay, pathToTempFolder);
                }
                
                for (var i = 0; i < pathTemplates.Count; i++)
                {
                    var files = GetStreamAllFiles(pathToTempFolder, i);
                    SaveNewFile(files);

                    files = null;
                    GC.Collect();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Directory.Exists(pathToTempFolder))
                {
                    var files = Directory.GetFiles(pathToTempFolder);

                    foreach (var file in files)
                    {
                        File.Delete(file);
                    }

                    Directory.Delete(pathToTempFolder);
                }
            }
        }

        private DateTime GetDateTimeValue(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                throw new Exception("Не найден параметр даты ОТ или даты ПО");
            }

            var values = value.Split('.');
            if (values.Count() != 3)
            {
                throw new Exception(@"Неверно указан формат даты. Пример: ""01.02.2019"" - день.месяц.год");
            }

            try
            {
                if (int.Parse(values[0]) <= 0 || int.Parse(values[0]) > 31)
                {
                    throw new Exception("День не может быть меньше единицы или больше 31");
                }

                if (int.Parse(values[1]) <= 0 || int.Parse(values[1]) > 12)
                {
                    throw new Exception("Месяц не может быть меньше единицы или больше 12");
                }

                if (int.Parse(values[2]) < 1950 || int.Parse(values[2]) > 2050)
                {
                    throw new Exception("Год не может быть меньше 1950 или больше 2050");
                }
            }
            catch(FormatException)
            {
                throw new Exception(@"Неверно указан формат даты. Пример: ""01.02.2019"" - день.месяц.год");
            }
            catch
            {
                throw;
            }

            var result = new DateTime(int.Parse(values[2]), int.Parse(values[1]), int.Parse(values[0]));

            return result;
        }

        private List<string> GetPathTemplates()
        {
            if (!Directory.Exists(pathToFolderTemplates))
            {
                throw new Exception(@"Неверно указан путь до папки с шаблонами. Пример: ""C:\templates""");
            }

            var files = Directory.GetFiles(pathToFolderTemplates);
            var correctFiles = files.Where(x => x.Contains('.') && x.Split('.').Last().ToLower() == CorrectTemplateFileFormat).ToList();

            if (files.Count() > correctFiles.Count())
            {
                reporter.WriteLine($@"Шаблоны могут быть только в формате "".docx"". В папке найдено некорректных файлов: {files.Count() - correctFiles.Count()}!");
            }

            return correctFiles;
        }

        private void CreateAct(int index, double amountActsPerDay, string pathToTempFolder)
        {
            var date = dateFrom.AddDays(index / amountActsPerDay).Date;

            var indexForTemplate = indexCurrentTemplate % pathTemplates.Count;
            indexCurrentTemplate++;

            var fileName = GetFileName(pathTemplates[indexForTemplate]);
            var copyPath = $"{pathToTempFolder}\\{fileName}{Separator}{GetEpochTimeUtcNow()}.{index}.docx";         

            using (FileStream template = new FileStream(pathTemplates[indexForTemplate], FileMode.Open))
            {
                using (FileStream fs = new FileStream(copyPath, FileMode.Create))
                {
                    template.Seek(0, SeekOrigin.Begin);
                    template.CopyTo(fs);
                }
            }

            using (var doc = WordprocessingDocument.Open(copyPath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;
                var texts = body.Descendants<Text>();

                var tokenTexts = texts.Where(t => t.Text.ToLower().Contains(PlaceholderDate) || t.Text.ToLower().Contains(PlaceholderNumber));

                if (!tokenTexts.Any(t => t.Text.ToLower().Contains(PlaceholderDate)))
                {
                    throw new Exception($@"Шаблон {pathTemplates[indexForTemplate]} не содержит значение ""date""");
                }

                if (!tokenTexts.Any(t => t.Text.ToLower().Contains(PlaceholderNumber)))
                {
                    throw new Exception($@"Шаблон {pathTemplates[indexForTemplate]} не содержит значение ""number""");
                }

                foreach (var token in tokenTexts.Where(t => t.Text.ToLower().Contains(PlaceholderDate)))
                {
                    token.Text = token.Text.Replace(PlaceholderDate, date.ToString("dd.MM.yyyy"));
                    if (token.Text.Contains("\\n"))
                    {
                        var strs = token.Text.Split(new string[] { "\\n" }, StringSplitOptions.None);
                        token.Text = "";
                        for (var l = 0; l < strs.Length; l++)
                        {
                            token.Parent.AppendChild(new Text(strs[l]));
                            if (l != strs.Length - 1)
                            {
                                token.Parent.AppendChild(new Break());
                            }
                        }
                    }
                }

                foreach (var token in tokenTexts.Where(t => t.Text.ToLower().Contains(PlaceholderNumber)))
                {
                    token.Text = token.Text.Replace(PlaceholderNumber, (index+1).ToString());
                    if (token.Text.Contains("\\n"))
                    {
                        var strs = token.Text.Split(new string[] { "\\n" }, StringSplitOptions.None);
                        token.Text = "";
                        for (var l = 0; l < strs.Length; l++)
                        {
                            token.Parent.AppendChild(new Text(strs[l]));
                            if (l != strs.Length - 1)
                            {
                                token.Parent.AppendChild(new Break());
                            }
                        }
                    }
                }

                if (texts.Any())
                {
                    Paragraph PageBreakParagraph = new Paragraph(new Run(new Break() { Type = BreakValues.Page }));
                    doc.MainDocumentPart.Document.Body.Append(PageBreakParagraph);
                }

                doc.MainDocumentPart.Document.Save();
            }
        }

        private List<byte[]> GetStreamAllFiles(string pathToTempFolder, int indexTemplate)
        {
            var fileList = new List<byte[]>();
            var files = Directory.GetFiles(pathToTempFolder)
                .Where(file => GetFileName(file).Split(new string[] { Separator }, StringSplitOptions.None).First() == GetFileName(pathTemplates[indexTemplate]))
                .ToList();

            foreach (var file in files)
            {
                byte[] textByteArray = File.ReadAllBytes(file);
                fileList.Add(textByteArray);
            }

            return fileList;
        }

        private void SaveNewFile(IList<byte[]> files)
        {
            var result = OpenAndCombine(files);

            var date = DateTime.Now.ToString("yyyy.MM.dd hh-mm-ss.ffff");

            if (!Directory.Exists(pathToResult))
            {
                Directory.CreateDirectory(pathToResult);
            }

            var filePath = $"{pathToResult}\\Acts {date}.docx";

            using (var newFile = File.Create(filePath)) { }
            File.WriteAllBytes(filePath, result);
        }

        private byte[] OpenAndCombine(IList<byte[]> documents)
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

                    var bodies = new List<XElement>();

                    var documentsCount = documents.Count;
                    for (pointer = 1; pointer < documentsCount; pointer++)
                    {
                        //reporter.SendProgress(documentsCount, pointer, TypeProgressBar.CobineActs, connectionid);

                        WordprocessingDocument tempDocument = WordprocessingDocument.Open(new MemoryStream(documents[pointer]), true);
                        XElement tempBody = XElement.Parse(tempDocument.MainDocumentPart.Document.Body.OuterXml);

                        newBody.Add(tempBody);

                        bodies.Add(new XElement(newBody));
                        newBody.RemoveAll();
                    }

                    if (bodies.Any())
                    {
                        XElement resultBody = bodies[0];
                        for (var i = 1; i < bodies.Count; i++)
                        {
                            resultBody.Add(bodies[i]);
                        }
                        mainDocument.MainDocumentPart.Document.Body = new Body(resultBody.ToString());

                        mainDocument.MainDocumentPart.Document.Save();
                        mainDocument.Package.Flush();
                    }

                    //reporter.SendProgress(documentsCount, documentsCount, TypeProgressBar.CobineActs, connectionid);
                }
            }
            catch (OpenXmlPackageException oxmle)
            {
                //reporter.AddError(oxmle.Message, connectionid);
                throw;
            }
            catch (OutOfMemoryException)
            {
                //log message
                throw;
            }
            catch (Exception e)
            {
                //reporter.AddError(e.Message, connectionid);
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

        private string GetFileName(string pathToFile)
        {
            var fileNameWithExtension = pathToFile.Split('\\').Last();
            var fileName = String.Join(".", fileNameWithExtension.Split('.').Take(fileNameWithExtension.Split('.').Count() - 1).ToArray());

            return fileName;
        }

        public long GetEpochTimeUtcNow() => (long)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1).ToUniversalTime()).TotalMilliseconds;
    }
}
