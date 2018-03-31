using net.sf.mpxj;
using net.sf.mpxj.mpp;
using net.sf.mpxj.reader;
using System.IO;

namespace ProjectSuccessWPF
{
    class MSProjectFileWorker
    {
        ProjectReader projectReader;
        public ProjectFile ProjectFile { get; private set; }

        public MSProjectFileWorker()
        {
            projectReader = new MPPReader();
        }

        public void ReadFile(string Path)
        {
            if (!File.Exists(Path))
                throw new IOException("File doesn/'t exist!");
            ProjectFile = projectReader.read(Path);
        }
    }
}
