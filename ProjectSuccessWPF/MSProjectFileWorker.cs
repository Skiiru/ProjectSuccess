using System;
using System.Collections.Generic;
using System.Text;
using net.sf.mpxj;
using net.sf.mpxj.mpp;
using net.sf.mpxj.reader;
using System.IO;

namespace ProjectSuccessWPF
{
    class MSProjectFileWorker
    {
        public ProjectReader projectReader;
        public ProjectFile projectFile;

        public MSProjectFileWorker()
        {
            projectReader = new MPPReader();
        }

        public void ReadFile(string Path)
        {
            if (!File.Exists(Path))
                throw new IOException("File doesn/'t exist!");
            projectFile = projectReader.read(Path);
        }
    }
}
