using System.Windows.Media.Imaging;

namespace ProjectSuccessWPF
{
    class ChartContainer
    {
        public string Header { get; set; }
        public string Text { get; set; }
        public System.Drawing.Bitmap Chart { get; set; }

        public ChartContainer(string header, string text, System.Drawing.Bitmap chart)
        {
            Header = header;
            Text = text;
            Chart = chart;
        }

        public ChartContainer(string header, System.Drawing.Bitmap chart) : this(header, string.Empty, chart) { }
    }
}
