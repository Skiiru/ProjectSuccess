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
                   currentLevelString + ". \"" + t.task.getName() + "\": продолжительность - " + t.GetConvertedDuration(projectProps) + ", ресурсы - ";
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

        public void CreateReport(string path, List<TaskInformation> tasks, ProjectProperties projectProps)
        {
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(path, FileMode.Create));
            writer.StrictImageSequence = true;
            document.Open();

            document.Add(CreateParagraph("Проект \"" + projectProps.getProjectTitle() + "\"", headerFontSize, true));
            document.Add(CreateParagraph("Список задач", headerFontSize, true));

            int level = 0;
            foreach (TaskInformation t in tasks)
            {
                level++;
                string paragraphText =
                    level.ToString() + ". \"" + t.task.getName() + "\": продолжительность - " + t.GetConvertedDuration(projectProps) + ", ресурсы - ";
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

            document.Close();
            if (!File.Exists(path))
                throw new IOException("Something went wrong in file creating. File doesn\'t exists.");
        }
    }
}
