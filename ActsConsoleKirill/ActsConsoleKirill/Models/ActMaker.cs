using ActsConsoleKirill.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using OpenXmlPowerTools;
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
        private readonly int startNumberOfActs;

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
                throw new Exception(@"Неверно указано количество актов. Пример: ""2000"" (без пробелов)");
            }

            if (amountActs > 10000 || amountActs < 1)
            {
                throw new Exception(@"Укажите количество актов не более 10000.");
            }

            if (!int.TryParse(ConfigurationManager.AppSettings["startNumberOfActs"], out startNumberOfActs))
            {
                throw new Exception(@"Неверно указан первый номер акта. Пример: ""1"" (без пробелов)");
            }

            if (startNumberOfActs < 1)
            {
                throw new Exception(@"Начальное число должно быть положительным!.");
            }

            dateFrom = GetDateTimeValue(ConfigurationManager.AppSettings["dateFrom"]);
            dateTo = GetDateTimeValue(ConfigurationManager.AppSettings["dateTo"]);
        }

        public void Run()
        {
            var amountDays = (dateTo - dateFrom).TotalDays + 1;
            var amountActsPerDay = amountActs / amountDays;
            var pathToTempFolder = $"{ Environment.CurrentDirectory.Split(':')[0]}:\\temp\\Acts_temp_{Guid.NewGuid().ToString()}";

            pathTemplates = GetPathTemplates();

            try
            {
                if (!Directory.Exists(pathToTempFolder))
                {
                    Directory.CreateDirectory(pathToTempFolder);
                }

                reporter.WriteLine("Начало создания актов!");
                reporter.Write("Создано: ");
                for (var i = startNumberOfActs; i < startNumberOfActs + amountActs; i++)
                {
                    CreateAct(i, amountActsPerDay, pathToTempFolder);

                    if (i % 100 == 0 && i != 0)
                    {
                        reporter.Write((i - startNumberOfActs + 1).ToString());
                    }
                }

                reporter.WriteLine();
                reporter.WriteLine($"Готово. Всего актово создано: {amountActs.ToString()}");

                SaveNewFile(pathToTempFolder);
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

                reporter.WriteLine("Все акты сделаны. Акты можно найти по пути: " + pathToResult);
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
                reporter.WriteLine();
            }

            return correctFiles;
        }

        private void CreateAct(int index, double amountActsPerDay, string pathToTempFolder)
        {
            var date = dateFrom.AddDays(index / amountActsPerDay).Date;

            var indexForTemplate = indexCurrentTemplate % pathTemplates.Count;
            indexCurrentTemplate++;

            var copyPath = $"{pathToTempFolder}\\{GetEpochTimeUtcNow()}.{index}.docx";         

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
                    token.Text = token.Text.Replace(PlaceholderNumber, index.ToString());
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

                //if (texts.Any())
                //{
                //    Paragraph PageBreakParagraph = new Paragraph(new Run(new Break() { Type = BreakValues.Page }));
                //    doc.MainDocumentPart.Document.Body.Append(PageBreakParagraph);
                //}

                doc.MainDocumentPart.Document.Save();
            }
        }

        private void SaveNewFile(string pathToTempFolder)
        {
            reporter.WriteLine("Объединение актов (может занять много времени).");

            var sources = new List<Source>();
            var files = Directory.GetFiles(pathToTempFolder);

            foreach (var f in files)
            {
                sources.Add(new Source(new WmlDocument(f)));
            }

            var pathToTempSources = $@"{pathToTempFolder}\temp1";
            if (!Directory.Exists(pathToTempSources))
            {
                Directory.CreateDirectory(pathToTempSources);
            }

            CreateFinalSources(sources, pathToTempSources);
            reporter.WriteLine("Подождите еще немножко....");

            if (!Directory.Exists(pathToResult))
            {
                Directory.CreateDirectory(pathToResult);
            }

            var date = DateTime.Now.ToString("yyyy.MM.dd hh-mm-ss.ffff");
            var filePath = $"{pathToResult}\\Acts {date}.docx";

            sources.Clear();
            files = Directory.GetFiles($@"{pathToTempFolder}\temp1");

            foreach (var f in files)
            {
                sources.Add(new Source(new WmlDocument(f)));
            }

            var mergedDoc = DocumentBuilder.BuildDocument(sources);
            mergedDoc.SaveAs(filePath);

            foreach (var file in files)
            {
                File.Delete(file);
            }

            Directory.Delete(pathToTempSources);

            GC.Collect();
            reporter.WriteLine("Сохранено!");
        }

        private void CreateFinalSources(List<Source> sources, string pathToTempSources)
        {
            reporter.Write("Начинается объединение актов: ");

            var skip = 0;
            var take = 50;
            while (true)
            {
                if (skip * take >= sources.Count)
                {
                    break;
                }

                var currentSources = sources.Skip(skip * take).Take(take).ToList();

                var mergedDoc = DocumentBuilder.BuildDocument(currentSources);
                mergedDoc.SaveAs($"{pathToTempSources}\\{GetEpochTimeUtcNow()}.docx");

                skip++;
                GC.Collect();

                reporter.Write((skip * take).ToString());
            }
            reporter.WriteLine();
        }

        public long GetEpochTimeUtcNow() => (long)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1).ToUniversalTime()).TotalMilliseconds;
    }
}
