using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WindowsAppBase.DomainCode
{
    public class SampleProject : XmlPropertyObject
    {
        #region "Properties"
        // All Properties here are project global accessible through Globals.project

        public const string SessionId = "ProjectID";
        public string ProjectName { get; set; }
        public string FileName { get; set; }
        public string DefaultFileExtension { get; set; }

        public const string FileFilter = "Project files(*.extension)|*.extension";

        public const string DefaultWorkingDirectory = "//BMP Trains";

        private SampleObject anObject; 
        bool Modified { get; set; }
        #endregion

        // Constructor
        public SampleProject()
        {
            ProjectName = "Default";
            FileName = "Default.extension";
            DefaultFileExtension = "extension";

            anObject = new SampleObject();

            Modified = false;
        }

        #region "Static Methods"


        #endregion

        public string Version()
        {
            return "Version: " + Application.ProductVersion;
        }

        public string VersionNumber()
        {
            return Application.ProductVersion;
        }

        // This is an example - SampleObject is simply a Sample for demonstration 
        // purposes. 
        public SampleObject theObject()
        {
            return anObject;
        }

        // These can be created by any property that is an object in SampleProject
        // use the passed value as any object and they will be polymorphic
        public void PullValues(SampleObject passedObject)
        {
            // This will populate the values of the internal global object into the passed object.
            anObject.PullValues(passedObject);
        }

        public void PushValues(SampleObject passedObject)
        {
            anObject.PushValues(passedObject);
        }


        #region "Project Interface Routines"

        #endregion

        #region "Open and Save as XML"

        public void Save()
        {
            
            SaveFileDialog savefile = new SaveFileDialog();
            // set a default file name

            savefile.InitialDirectory = Common.WorkingDirectory();

            savefile.FileName = Globals.Project.getFileName();
            // set filters - this can be done in properties as well
            savefile.Filter = "Project Files (*." + DefaultFileExtension + ")|*." + DefaultFileExtension + "|All files (*.*)|*.*";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                savefile.FileName = savefile.FileName.Replace('&', '_');
                if ( saveToFile(savefile.FileName))
                {
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                    MessageBox.Show("File " + savefile.FileName + " Saved Successfully");

                    FileList fl = new FileList(savefile.FileName);
                    fl.SaveFileList();
                }
                else
                {
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                    MessageBox.Show("File " + savefile.FileName + " Error occurred in save");
                }
            }
        }

        public new string AsXML()
        {
            // Add any embedded objects here - they
            // should be added as a string into the String Array
            string s = base.AsXML("1", new string[] {
                anObject.AsXML()
            }); ; ;
            s = s.Replace("&", "&amp;");

            return s;
        }

        // Samples

        //public string CatchmentsAsXML()
        //{
        //    string s = "";
        //    foreach (KeyValuePair<int,Catchment> kvp in Catchments)
        //    {
        //        Catchment c = (Catchment)kvp.Value;
        //        s += c.AsXML(Convert.ToString(kvp.Key));
        //    }
        //    return s;
        //}

        //public string CostScenariosAsXML()
        //{
        //    string s = "";
        //    if (CostScenarios == null) return s;
        //    foreach (KeyValuePair<int, CostScenario> kvp in CostScenarios)
        //    {
        //        CostScenario c = (CostScenario)kvp.Value;
        //        s += c.AsXML(Convert.ToString(kvp.Key));
        //    }
        //    return s;
        //}


        public new void fromXML(string xml)
        {
            if (xml == "") return;
            if (xml == null) return;

            var doc = XDocument.Parse(xml);

            ClearProperties();
            // This will get all root elements of the current object
            XmlDeserialize(doc);
            getSampleObjectFromXML(doc);

            // Examples of other approaches of collections of objects
            //getCatchmentsFromXML(doc);

        }

        public new void ClearProperties()
        {
            // Code to reset or clear all properties
            base.ClearProperties();
        }


        public void populateObjectFromXDocument(XmlPropertyObject obj,  XDocument doc)
        {
            // An Object obj that is a child of class XmlPropertyObject 
            // can be populated from XML. This will poplate that object
            
            // Get the Class name from the passed object
            String objName = System.ComponentModel.TypeDescriptor.GetClassName(obj);
            objName = objName.Substring(objName.LastIndexOf(".") + 1);

            // Get the first instance of the xml wrappered by the class name
            // in the XML document

            if (doc.Descendants(objName) == null) return;
            obj.fromXML(doc.Descendants(objName).FirstOrDefault().ToString());
        }

        private void getSampleObjectFromXML(XDocument doc) 
        {
            // This is not really needed, it only redirects a call to 
            populateObjectFromXDocument(anObject, doc);
            //populateObjectFromXDocument(anObject, "SampleObject", doc);
            //if (doc.Descendants("SampleObject") == null) return;
            // Assumes ONLY 1 Sample Object - but can be used for any embedded xml string
            // representing and object values. 
            //anObject.fromXML(doc.Descendants("SampleObject").FirstOrDefault().ToString());
        }



        // Samples of getting an XMLPropertyObject from XML

        //private void getCatchmentsFromXML(XDocument doc)
        //{
        //    // Now to get each subelement Catchments
        //    var XElements = doc.Descendants().Where(p => p.Name.LocalName == "Catchment");
        //    foreach (XElement element in XElements)
        //    {
        //        int id = Convert.ToInt32(element.FirstAttribute.Value);
        //        Catchment c = getCatchment(id);
        //        c.fromXML(element.ToString());
        //    }
        //}
        #endregion

        #region "File Open and Save - Embedded Object Independent"
        public bool saveToFile(string filename)
        {
            FileName = filename;
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

        public bool saveToFile()
        {
            string filename = Path.GetFileNameWithoutExtension(this.FileName);
            if ((filename == "") || (filename == null))
                filename = Common.WorkingDirectory() + "\\" + SampleProject.getUniqueFileName();

            return saveToFile(filename);            
        }
        public string WorkingDirectory()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + DefaultWorkingDirectory; // "\\BMP Trains";
            Directory.CreateDirectory(path);
            return path;
        }
        public string Open()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = this.WorkingDirectory();
            dlg.Filter = DomainCode.SampleProject.FileFilter;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string fileName = dlg.FileName;
                return this.Open(fileName);
            }
            return "";
        }
        public string Open(String filename)
        {
            string res = this.openFromFile(filename);
            if (res == "")
            {
                this.FileName = filename;
            }
            else
            {
                MessageBox.Show(res);
            }
            return this.AsXML();
        }
        public string openFromFile(string filename)
        {    
                //try
                //{
                    if (File.Exists(filename))
                    {
                        string s = File.ReadAllText(filename);
                        this.fromXML(s);                        
                    }
            //Calculate();
            return "";
        }

        public string getFileName()
        {
            if (FileName == "") return SampleProject.getUniqueFileName();
            else return Path.GetFileNameWithoutExtension(this.FileName);
        }

        public static string getUniqueFileName()
        {
            return Common.getUniqueFileName("Project File_Name", "extension");
        }
        #endregion

        #region "Output Reporting (Required Abstract Methods)"

        public override Dictionary<string, string> PropertyLabels()
        {
            // Syntax: ProepertyName, Property Label
            return new Dictionary<string, string>
                {
                    {"ProjectName", "Project Name"},
                    {"FileName", "Project File Name"}
            };
        }
        public override Dictionary<string, int> PropertyDecimalPlaces()
        {
            // Syntax: Property Name, Default Decimal places for display
            return new Dictionary<string, int>
                {
                    {"AnyFloat", 2}
                };
        }

        public new string AsHtmlTable()
        {
            string s = "<h1>Project Information</h1>";
            s += base.AsHtmlTable();
            return s;
        }

        #endregion

        #region "Caclulation Routines - Embedded Object Dependent"
        #endregion

    }
}
