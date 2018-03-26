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
                if (t.OvertimeWorkValue != 0)
                {
                    string paragraphText =
                       currentLevelString + ". \"" + t.TaskName + "\" (" + t.CompletePercentage + "%) : " +
                       t.GetDurations() +
                       ", стоимость - " + t.Cost + projectProps.getCurrencySymbol() +
                       ", оставшаяся стоимость - " + t.RemainingCost + projectProps.getCurrencySymbol() +
                       ", перерасход - " + t.OverCost + projectProps.getCurrencySymbol() +
                       ", ресурсы - ";
                    if (t.Resources.Count != 0)
                    {
                        foreach (ResourceInformation res in t.Resources)
                            paragraphText += res.ResourceName + ", ";
                        //Replasing "," with "." after last item
                        paragraphText = paragraphText.Remove(paragraphText.Length - 2, 2) + ".";
                    }
                    else
                        paragraphText += " не указаны.";
                    document.Add(CreateParagraph(paragraphText, textFontSize, false));
                }
                ParseTaskHierarhyIntoText(t.ChildTasks, currentLevelString, projectProps);
            }
        }

        public void CreateReport(
            string path,
            List<TaskInformation> tasks,
            List<ResourceInformation> resources,
            ProjectProperties projectProps,
            List<ChartContainer> charts,
            ProjectRate rate)
        {
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(path, FileMode.Create));
            writer.StrictImageSequence = true;
            document.Open();

            document.Add(CreateParagraph("Проект \"" + projectProps.getProjectTitle() + "\"", headerFontSize, true));

            document.Add(CreatePhrase(Environment.NewLine + "Инфомация о проекте: ", textFontSize, true));

            int projectCost = projectProps.getBaselineCost().intValue();
            int cost = projectCost;
            double overcost = 0;
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

            if (rate != null)
            {
                document.Add(CreateParagraph("Оценки проекта", headerFontSize, true));

                if (!double.IsNaN(rate.ProjectOvertime))
                {
                    string s = "Перерасход времени: " + Math.Round(rate.ProjectOvertime, 2) + "ч.";
                    document.Add(CreateParagraph(s, textFontSize, false));
                }

                if (!double.IsNaN(rate.ProjectOverCost))
                {
                    string s = "Перерасход срдеств: " + Math.Round(rate.ProjectOverCost, 2) + projectProps.getCurrencySymbol() + ".";
                    document.Add(CreateParagraph(s, textFontSize, false));
                }

                if (!double.IsNaN(rate.MeanTaskDuration))
                {
                    string s = "Средняя продолжительность задач: " + Math.Round(rate.MeanTaskDuration, 2) + "ч.";
                    document.Add(CreateParagraph(s, textFontSize, false));
                }

                if (!double.IsNaN(rate.RecourcesTotalOverworkTime))
                {
                    string s = "Суммарный перерасход времени ресурсов: " + Math.Round(rate.RecourcesTotalOverworkTime, 2) + "ч.";
                    document.Add(CreateParagraph(s, textFontSize, false));
                }

                if (!double.IsNaN(rate.ProjectOverCostPercentage))
                {
                    string s = "Оценка перерасхода средств: " + Math.Round(rate.ProjectOverCostPercentage, 2) + "%, " + rate.GetOvercostRateString() + ".";
                    document.Add(CreateParagraph(s, textFontSize, false));
                }

                if(!double.IsNaN(rate.MeanTaskDurationRate))
                {
                    string s = "Оценка средней продолжительности задач: " + Math.Round(rate.MeanTaskDurationRate, 2) + "%, " + rate.GetMeanTaskDurationString() + '.';
                    document.Add(CreateParagraph(s, textFontSize, false));
                }

                if (!double.IsNaN(rate.MeanTaskDurationRate))
                {
                    string s = "Оценка перерасхода времени: " + Math.Round(rate.ProjectOvertimeRate, 2) + "%, " + rate.GetProjectOvertimeString() + '.';
                    document.Add(CreateParagraph(s, textFontSize, false));
                }
            }

            #region Tasks
            document.Add(CreateParagraph("Список задач с отклонениями", headerFontSize, true));

            int level = 0;
            foreach (TaskInformation t in tasks)
            {
                level++;
                if (t.OvertimeWorkValue != 0)
                {
                    string paragraphText = level.ToString() + ". \"" + t.TaskName + "\": " + t.GetDurations();
                    if (t.OvertimeWork != "0.0")
                        paragraphText += ", переработка - " + t.OvertimeWork;
                    paragraphText += ", ресурсы - ";
                    if (t.Resources.Count != 0)
                    {
                        foreach (ResourceInformation res in t.Resources)
                            paragraphText += res.ResourceName + ", ";
                        //Replasing "," with "." after last item
                        paragraphText = paragraphText.Remove(paragraphText.Length - 2, 2) + ".";
                    }
                    else
                        paragraphText += "не указаны.";
                    document.Add(CreateParagraph(paragraphText, textFontSize, false));
                }
                ParseTaskHierarhyIntoText(t.ChildTasks, level.ToString(), projectProps);
            }
            #endregion

            //Space between parts
            document.Add(CreateParagraph(Environment.NewLine, textFontSize, false));

            #region Resources
            document.Add(CreateParagraph("Список ресурсов с отклонениями", headerFontSize, true));
            foreach (ResourceInformation resInf in resources)
            {
                //Sometimes there is "fake" resource in project, idk why
                if (resInf.ResourceName != "Undefined" && resInf.OvertimeWorkDurationValue != 0)
                    document.Add(CreateParagraph(
                        resInf.ResourceName + " (" + resInf.Cost + "): время работы - " + resInf.WorkDuration + ", переработки - " + resInf.OvertimeWorkDuration,
                        textFontSize,
                        false));
            }

            #endregion

            //Space between parts
            document.Add(CreateParagraph(Environment.NewLine + "Графики" + Environment.NewLine, headerFontSize, true));

            #region Charts
            if (charts != null && charts.Count > 0)
            {
                foreach (ChartContainer c in charts)
                {
                    document.Add(Image.GetInstance(c.Chart, System.Drawing.Imaging.ImageFormat.Jpeg));
                    document.Add(CreateParagraph(c.Header, textFontSize, true));
                    if (c.Text != string.Empty)
                        document.Add(CreateParagraph(Environment.NewLine + c.Text, textFontSize, false));
                }
            }
            #endregion

            document.Close();
            document.Dispose();
            if (!File.Exists(path))
                throw new IOException("Something went wrong in file creating. File doesn\'t exists.");
        }
    }
}
