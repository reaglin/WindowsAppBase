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

        public string FileExtension { get; set; }

        #endregion


        private SampleObject anObject; 

        bool Modified { get; set; }
        
        // Constructor
        public SampleProject()
        {
            ProjectName = "Default";
            FileName = "Default.extension";
            FileExtension = "extension";

            anObject = new SampleObject();

            Modified = false;
        }

        public string Version()
        {
            return "Version: " + Application.ProductVersion;
        }

        public string VersionNumber()
        {
            return Application.ProductVersion;
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
            savefile.Filter = "Project Files (*." + FileExtension + ")|*." + FileExtension + "|All files (*.*)|*.*";

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

            //getCatchmentsFromXML(doc);
            //getCostScenariosFromXML(doc);
        }

        public new void ClearProperties()
        {
            // Code to reset or clear all properties
            base.ClearProperties();
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

        //private void getCostScenariosFromXML(XDocument doc)
        //{
        //    // Now to get each subelement Catchments
        //    var XElements = doc.Descendants().Where(p => p.Name.LocalName == "CostScenario");
        //    foreach (XElement element in XElements)
        //    {
        //        int id = Convert.ToInt32(element.FirstAttribute.Value);
        //        CostScenario c = getCostScenario(id);
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

        #region "Output Reporting"
        public override Dictionary<string, int> PropertyDecimalPlaces()
        {
            return new Dictionary<string, int>
                {
                    {"MeanAnnualRainfall", 2}
                };
        }

        public override Dictionary<string, string> PropertyLabels()
        {
            return new Dictionary<string, string>
                {
                    {"ProjectName", "Project Name"},
                    {"FileName", "Project File Name"},
                    {"RainfallZone", "Rainfall Zone"},
                    {"MeanAnnualRainfall", "Mean Annual Rainfall (in)"},
                    {"AnalysisType", "Analysis Type"},
                    {"RequiredNTreatmentEfficiency", "Required Nitrogen Removal Efficiency (%)"},
                    {"RequiredPTreatmentEfficiency", "Required Phosphorus Removal Efficiency (%)"},
                    {"CatchmentConfiguration", "Catchment Configuration"}
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
