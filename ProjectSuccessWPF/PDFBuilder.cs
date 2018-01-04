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
                   currentLevelString + ". \"" + t.TaskName + "\" (" + t.CompletePecrentage + "%) : " +
                   t.GetDurations() +
                   ", стоимость - " + t.Cost + projectProps.getCurrencySymbol() +
                   ", оставшаяся стоимость - " + t.RemainingCost + projectProps.getCurrencySymbol() + 
                   ", перерасход - " + t.OverCost + projectProps.getCurrencySymbol() +
                   ", ресурсы - ";
                if (t.Resources.Count != 0)
                {
                    foreach (Resource res in t.Resources)
                        paragraphText += res.getName() + ", ";
                    //Replasing "," with "." after last item
                    paragraphText = paragraphText.Remove(paragraphText.Length - 2, 2) + ".";
                }
                else
                    paragraphText += "указаны в подзадачах либо отсутствуют.";

                document.Add(CreateParagraph(paragraphText, textFontSize, false));
                ParseTaskHierarhyIntoText(t.ChildTasks, currentLevelString, projectProps);
            }
        }

        public void CreateReport(string path, List<TaskInformation> tasks, List<ResourceInformation> resources, ProjectProperties projectProps)
        {
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(path, FileMode.Create));
            writer.StrictImageSequence = true;
            document.Open();

            document.Add(CreateParagraph("Проект \"" + projectProps.getProjectTitle() + "\"", headerFontSize, true));

            document.Add(CreatePhrase(Environment.NewLine + "Инфомация о проекте: ", textFontSize, true));

            int projectCost = projectProps.getBaselineCost().intValue();
            int cost = projectCost;
            float overcost = 0;
            int taskCount = 0;
            int remainProjectCost = projectProps.getBaselineCost().intValue() - projectProps.getActualCost().intValue();
            int remainCost = remainProjectCost;
            foreach (TaskInformation t in tasks)
            {
                taskCount += t.SubTusksCount();
                overcost += t.OverCost;
                if (projectCost == 0)
                    cost += t.Cost;
                if (remainProjectCost == 0)
                    remainCost += t.RemainingCost;
            }

            string projectCostStr = 
                cost + projectProps.getCurrencySymbol() + 
                " (осталось - " + remainCost + projectProps.getCurrencySymbol() + ") " +
                "перерасход - " + overcost + projectProps.getCurrencySymbol();
            string projectTime = projectProps.getStartDate().toString() + " - " + projectProps.getFinishDate();

            document.Add(CreatePhrase("сроки проекта: " + projectTime + "; стоимость проекта: " + projectCostStr, textFontSize, false));
            document.Add(CreatePhrase("; число ресурсов - " + resources.Count, textFontSize, false));

            document.Add(CreatePhrase("; всего задач - " + taskCount + "." + Environment.NewLine, textFontSize, false));

            #region Tasks
            document.Add(CreateParagraph("Список задач", headerFontSize, true));

            int level = 0;
            foreach (TaskInformation t in tasks)
            {
                level++;
                string paragraphText = level.ToString() + ". \"" + t.TaskName + "\": " + t.GetDurations();
                if (t.OvertimeWork != "0.0")
                    paragraphText += ", переработка - " + t.OvertimeWork;
                paragraphText += ", ресурсы - ";
                if (t.Resources.Count != 0)
                {
                    foreach (Resource res in t.Resources)
                        paragraphText += res.getName() + ", ";
                    //Replasing "," with "." after last item
                    paragraphText = paragraphText.Remove(paragraphText.Length - 2, 2) + ".";
                }
                else
                    paragraphText += "указаны в подзадачах либо отсутствуют.";

                document.Add(CreateParagraph(paragraphText, textFontSize, false));
                ParseTaskHierarhyIntoText(t.ChildTasks, level.ToString(), projectProps);
            }
            #endregion

            //Space between parts
            document.Add(CreateParagraph(Environment.NewLine, textFontSize, false));

            #region Resources
            document.Add(CreateParagraph("Список ресурсов", headerFontSize, true));
            foreach (ResourceInformation resInf in resources)
            {
                //Sometimes there is "fake" resource in project, idk why
                if (resInf.ResourceName != "Undefined")
                    document.Add(CreateParagraph(
                        resInf.ResourceName + " (" + resInf.Cost + "): время работы - " + resInf.WorkDuration + ", переработки - " + resInf.OvertimeWorkDuration,
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
