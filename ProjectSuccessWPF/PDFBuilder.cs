using iTextSharp.text;
using iTextSharp.text.pdf;
using net.sf.mpxj;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProjectSuccessWPF
{
    class PDFBuilder
    {
        Document document;

        //Font
        public Font.FontFamily fontFamily = Font.FontFamily.TIMES_ROMAN;
        BaseFont font;
        public BaseColor fontColor = BaseColor.BLACK;

        //Font sizes
        int textFontSize = 14;
        int headerFontSize = 16;

        //Time formate
        public TimeUnit timeFormat = TimeUnit.HOURS;

        public PDFBuilder()
        {
            document = new Document();
            string ttf = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIAL.TTF");
            this.font = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        }

        Phrase CreatePhrase(string text, int fontSize, bool isHeader)
        {
            //Style is an integer constant
            int style = isHeader ? Font.BOLD : Font.NORMAL;
            return new Phrase(text, new Font(font, fontSize, Font.NORMAL, fontColor));
        }

        Paragraph CreateParagraph(string text, int fontSize, bool isHeader)
        {
            Paragraph paragraph = new Paragraph(this.CreatePhrase(text, fontSize, isHeader));
            if (isHeader)
                paragraph.Alignment = Element.ALIGN_CENTER;
            return paragraph;
        }

        void ParseTaskHierarhyIntoText(List<TaskInformation> tasks, string levelStr, ProjectProperties projectProps)
        {
            int level = 0;
            foreach (TaskInformation t in tasks)
            {
                level++;
                string currentLevelString = levelStr + "." + level.ToString();
                string paragraphText =
                   currentLevelString + ". \"" + t.taskName + "\" (" + t.completePecrentage + ") : " + t.GetDurations() + ", ресурсы - ";
                if (t.resources.Count != 0)
                {
                    foreach (Resource res in t.resources)
                        paragraphText += res.getName() + ", ";
                    //Replasing "," with "." after last item
                    paragraphText = paragraphText.Remove(paragraphText.Length - 2, 2) + ".";
                }
                else
                    paragraphText += "указаны в подзадачах либо отсутствуют.";

                document.Add(CreateParagraph(paragraphText, textFontSize, false));
                ParseTaskHierarhyIntoText(t.childTasks, currentLevelString, projectProps);
            }
        }

        public void CreateReport(string path, List<TaskInformation> tasks, List<ResourceInformation> resources, ProjectProperties projectProps)
        {
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(path, FileMode.Create));
            writer.StrictImageSequence = true;
            document.Open();

            document.Add(CreateParagraph("Проект \"" + projectProps.getProjectTitle() + "\"", headerFontSize, true));

            document.Add(CreatePhrase(Environment.NewLine + "Инфомация о проекте: ", textFontSize, true));

            string cost = projectProps.getBaselineCost().toString();
            int remainCost = projectProps.getBaselineCost().intValue() - projectProps.getActualCost().intValue();
            string projectCost = cost + " (осталось - " + remainCost.ToString() + "" + projectProps.getCurrencySymbol() + ")";
            string projectTime = projectProps.getStartDate().toString() + " - " + projectProps.getFinishDate();

            document.Add(CreatePhrase("сроки проекта: " + projectTime + "; стоимость проекта: " + projectCost, textFontSize, false));
            document.Add(CreatePhrase("; число ресурсов - " + resources.Count, textFontSize, false));

            int taskCount = 0;
            foreach (TaskInformation t in tasks)
            {
                taskCount += t.SubTusksCount();
            }
            document.Add(CreatePhrase("; всего задач - " + taskCount + "." + Environment.NewLine, textFontSize, false));

            #region Tasks
            document.Add(CreateParagraph("Список задач", headerFontSize, true));

            int level = 0;
            foreach (TaskInformation t in tasks)
            {
                level++;
                string paragraphText = level.ToString() + ". \"" + t.taskName + "\": " + t.GetDurations();//продолжительность - " + t.GetDuration(projectProps);
                if (t.overtimeWork != "0.0")
                    paragraphText += ", переработка - " + t.overtimeWork;
                paragraphText += ", ресурсы - ";
                if (t.resources.Count != 0)
                {
                    foreach (Resource res in t.resources)
                        paragraphText += res.getName() + ", ";
                    //Replasing "," with "." after last item
                    paragraphText = paragraphText.Remove(paragraphText.Length - 2, 2) + ".";
                }
                else
                    paragraphText += "указаны в подзадачах либо отсутствуют.";

                document.Add(CreateParagraph(paragraphText, textFontSize, false));
                ParseTaskHierarhyIntoText(t.childTasks, level.ToString(), projectProps);
            }
            #endregion

            //Space between parts
            document.Add(CreateParagraph(Environment.NewLine, textFontSize, false));

            #region Resources
            document.Add(CreateParagraph("Список ресурсов", headerFontSize, true));
            foreach (ResourceInformation resInf in resources)
            {
                document.Add(CreateParagraph(
                    resInf.resourceName + " (" + resInf.cost + "): время работы - " + resInf.workDuration + ", переработки - " + resInf.overtimeWorkDuration,
                    textFontSize,
                    false));
            }

            #endregion

            document.Close();
            if (!File.Exists(path))
                throw new IOException("Something went wrong in file creating. File doesn\'t exists.");
        }
    }
}
