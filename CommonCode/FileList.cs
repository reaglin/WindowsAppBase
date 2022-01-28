using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Allows for a list of current files.
namespace WindowsAppBase.DomainCode
{
    class FileList : XmlPropertyObject
    {
        public const string RecentFiles = "RecentFiles.xml";
        public string File1 { get; set; }
        public string File2 { get; set; }
        public string File3 { get; set; }
        public string File4 { get; set; }
        public string File5 { get; set; }
        public string File6 { get; set; }
       
        public List<string> files;

        // Constructor - build a File List for Open
        public FileList()
        {
            files = new List<String>();
            OpenFileList();
        }

        // Build the FileList adding a new File
        public FileList(string fn)
        {
            files = new List<String>();
            OpenFileList();
            createListFromFilenames();
            AddFile(fn);
            SetValues();
        }

        public void SetValues()
        {
            int n = files.Count;
            if (n == 0) return;
            File1 = files[0]; if (n == 1) return;
            File2 = files[1]; if (n == 2) return;
            File3 = files[2]; if (n == 3) return;
            File4 = files[3]; if (n == 4) return;
            File5 = files[4]; if (n == 5) return;
            File6 = files[5];
        }

        public void AddFile(string filename)
        {
            // Adds the file to the list
            if (filename == null) return;
            
            if (files.Contains<String>(filename))
            {
                files.RemoveAll(x => x == filename);
                files.Insert(0, filename);
            }
            else
            {
                files.Insert(0, filename);
            }
        }

        public bool SaveFileList()
        {
            SetValues();
            string filename = Common.WorkingDirectory() + "\\" + RecentFiles;
            try
            {
                using (StreamWriter sw = new StreamWriter(filename))
                    sw.WriteLine(this.AsXML());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string OpenFileList()
        {
            string filename = Common.WorkingDirectory() + "\\" + RecentFiles;
            //{
            if (File.Exists(filename))
            {
                string s = File.ReadAllText(filename);
                s = s.Replace('&', '_');
                this.fromXML(s);
            }
            
            return "";
        }

        public void createListFromFilenames()
        {
            // Reverse order as the Add always adds to the first element
            if (File6 != "") AddFile(File6);
            if (File5 != "") AddFile(File5);
            if (File4 != "") AddFile(File4);
            if (File3 != "") AddFile(File3);
            if (File2 != "") AddFile(File2);
            if (File1 != "") AddFile(File1);

        }

        public void FillListBox(ListBox lb)
        {
            if (File1 == null) return;
            if (File1 != "") lb.Items.Add(File1);
            if (File2 != "") lb.Items.Add(File2);
            if (File3 != "") lb.Items.Add(File3);
            if (File4 != "") lb.Items.Add(File4);
            if (File5 != "") lb.Items.Add(File5);
            if (File6 != "") lb.Items.Add(File6);
        }

        public override Dictionary<string, string> PropertyLabels()
        {
            return new Dictionary<string, string>
            {
            };
        }

        public override Dictionary<string, int> PropertyDecimalPlaces()
        {
            return new Dictionary<string, int>
            {
            };
        }
    }
}
