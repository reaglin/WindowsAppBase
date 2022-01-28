using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsAppBase.DomainCode
{
    public class Common
    {
        public static string HomePage = "~/DefaultLoggedIn.aspx";

        public static string ManagePage(string className)
        {
            return "~/Pages/ManageXMLPropertyObject.aspx?Class=" + className;
        }

        public static int getInt(TextBox tb)
        {
            try
            {
                return Convert.ToInt32(tb.Text);
            }
            catch
            {
                return 0;
            }
        }
        public static int getInt(TextBox tb, int min, int max)
        {
            try
            {
                int a = Convert.ToInt32(tb.Text);
                if (a < min) return min;
                if (a > max) return max;
                return a;
            }
            catch
            {
                return 0;
            }
        }

        public static int getInt(ComboBox cb)
        {
            try
            {
                return Convert.ToInt32(getString(cb));
            }
            catch
            {
                return 0;
            }
        }
        public static string getString(ComboBox cb)
        {
            try
            {
                return cb.Text;
            }
            catch
            {
                return "";
            }
        }
        public static double getDouble(ComboBox cb)
        {
            try
            {
                return Convert.ToDouble(getString(cb));
            }
            catch
            {
                return 0.0;
            }
        }

        public static string getString(double value, int decimalPlaces)
        {
            string formatString = "{0:N" + decimalPlaces.ToString().Trim() + "}";
            return String.Format(formatString, value);
        }

        public static string getString(TextBox tb)
        {
            try
            {
                return tb.Text;
            }
            catch
            {
                return "";
            }
        }

        public static double getDouble(TextBox tb)
        {
            try
            {
                return Convert.ToDouble(tb.Text);
            }
            catch
            {
                return 0.0;
            }
        }
        public static double getDouble(TextBox tb, double min, double max)
        {
            try
            {
                double a =  Convert.ToDouble(tb.Text);
                if (a < min) return min;
                if (a > max) return max;
                return a;
            }
            catch
            {
                return 0.0;
            }
        }



        public static double getDouble(MaskedTextBox tb)
        {
            try
            {
                return Convert.ToDouble(tb.Text);
            }
            catch
            {
                return 0.0;
            }
        }

        public static void setValue(ComboBox cb, string s)
        {
            try
            {
                cb.SelectedIndex = cb.FindStringExact(s);
            }
            catch
            {
                // do nothing
            }
        }

        public static void setValue(ComboBox cb, double d, string format = "#.##" )
        {
            try
            {
                cb.SelectedIndex = cb.FindStringExact(d.ToString(format));
                // if not found we will look for closest match
                if (cb.SelectedIndex == -1)
                {
                    foreach (Object c in cb.Items)
                    {
                        double diff = Math.Abs(Convert.ToDouble(c) - d);
                        if (diff == 0) cb.SelectedIndex = cb.FindStringExact(c.ToString());
                    }
                }
            }
            catch
            {
                // do nothing
            }
        }
        public static void setValue(TextBox tb, string s)
        {
            try
            {
                tb.Text = s;
            }
            catch
            {
                tb.Text = "";
            }
        }
        public static void setValue(TextBox tb, int i)
        {
            try
            {
                tb.Text = Convert.ToString(i);
            }
            catch
            {
                tb.Text = "";
            }
        }

        public static void setValue(TextBox tb, double d)
        {
            try
            {
                tb.Text = Convert.ToString(d);
            }
            catch
            {
                tb.Text = "";
            }
        }

        public static void setValue(MaskedTextBox tb, double d)
        {
            try
            {
                tb.Text = Convert.ToString(d);
            }
            catch
            {
                tb.Text = "";
            }
        }

        public static void setValue(TextBox tb, double d, int p)
        {
            try
            {
                tb.Text = d.ToString("N"+p.ToString());
            }
            catch
            {
                tb.Text = "";
            }

        }

        //public static T Clone<T>(T source)
        //{
        //    if (!typeof(T).IsSerializable)
        //    {
        //        throw new ArgumentException("The type must be serializable.", "source");
        //    }

        //    // Don't serialize a null object, simply return the default for that object
        //    if (Object.ReferenceEquals(source, null))
        //    {
        //        return default(T);
        //    }

        //    IFormatter formatter = new BinaryFormatter();
        //    Stream stream = new MemoryStream();
        //    using (stream)
        //    {
        //        formatter.Serialize(stream, source);
        //        stream.Seek(0, SeekOrigin.Begin);
        //        return (T)formatter.Deserialize(stream);
        //    }
        //}


        public static double ToDouble(string v)
        {
            try { return Convert.ToDouble(v); }
            catch { return 0; }
        }

        public static string ShortClassName(string name)
        {
            string fc = name;
            while (fc.Contains("."))
            {
                fc = fc.Substring(fc.IndexOf(".") + 1);
            }
            return fc;
        }

        //public static Dictionary<string, string> ClassLabels()
        //{
        //    // This routine returns a label for the interface for classes that use
        //    // the class name for identification of interface properties.

        //    var d = new Dictionary<string, string>();
        //    d.Add("Watershed", "Watershed");
        //    d.Add("Rainfall", "Rainfall");
        //    d.Add("CircularChannel", "Circular Channel");
        //    d.Add("TrapezoidalChannel", "Trapezoidal Channel");
        //    d.Add("DimensionlessRainfall", "Dimensionless Rainfall");
        //    return d;
        //}

        public static string ParameterString(string value, string label)
        {
            return label + ": " + value + "<br/>";
        }

        public static string ParameterString(double value, string label, int places)
        {
            return label + ": " + FormattedString(value, places) + "<br/>"; 
        }

        public static string ParameterString(int value, string label)
        {
            return label + ": " + value.ToString() + "<br/>";
        }
        //public static KeyValuePair<string, string> GetRandomClass()
        //{
        //    Dictionary<string, string> d = ClassLabels();
        //    Random r = new Random();
        //    int i = 0;
        //    int randIndex = r.Next(0, d.Values.Count);
        //    foreach (KeyValuePair<string, string> value in d)
        //    {
        //        if (i++ == randIndex)
        //            return value;
        //    }
        //    return new KeyValuePair<string, string>();
        //}

        #region "Validation Routines"
        public static bool ValidateGE(double v, double lower, string message, bool allowZero = true)
        {
            if (allowZero && v == 0) return true;
            if (lower >= v)
            {
                MessageBox.Show(message, "Error");
                return false;
            }
            return true;
        }

        public static bool ValidateRange(double v, double lower, double upper, string message, bool allowZero = true)
        {
            if (allowZero && v == 0) return true;
            if ((v <= lower ) || (v >= upper))
            {
                MessageBox.Show(message, "Error");
                return false;
            }
            return true;
        }

        public static double GetAverageOfDouble(double[] x, int maxRows)
        {
            double j = 0;
            double sum = 0.0;
            for (int i = 0; i < maxRows; i++)
            {
                if (x[i] != 0)
                {
                    sum += x[i];
                    j++;
                }
            }
            if (j > 0) return sum / j;
            return 0.0;
        }

        public static double GetSumOfDouble(double[] x, int maxRows)
        {
            double sum = 0.0;
            for (int i = 0; i < maxRows; i++)
            {
                if (x[i] != 0)
                {
                    sum += x[i];
                }
            }
            return sum;
        }


        #endregion

        #region "String Routines"
        //public static string GetClassLabel(string name)
        //{
        //    try
        //    {
        //        return ClassLabels()[ShortClassName(name)];
        //    }
        //    catch
        //    {
        //        return name;
        //    }

        //}

        public static string getUniqueFileName(string header, string extension)
        {
            int i = 1;

            string filename = header + i.ToString() + "." + extension;
            while (File.Exists(WorkingDirectory() + "\\" + filename))
            {
                i++;
                filename = header + i.ToString() + "." + extension;
            }
            return filename;
        }

        public static string WorkingDirectory()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\BMP Trains";
            Directory.CreateDirectory(path);
            return path;
        }

       

        public static void setButtonColor(bool defined, System.Windows.Forms.Button btn)
        {
            System.Drawing.Color dColor = System.Drawing.Color.LightGreen;
            System.Drawing.Color bColor = System.Drawing.Color.LightGray;

            if (defined) btn.BackColor = dColor; else btn.BackColor = bColor;
        }

        public static string FormattedString(double value, int places)
        {
            return String.Format("{0:N" + places.ToString().Trim() + "}", value);
        }

        public static string ConvertTabDelimitedToHtml(string input)
        {
            // Converts tab delimited \t for columns and new line \n for rows
            // to html but without header information
            string s = "<tr><td>" + input;
            s = s.Replace("\n", "</tr><tr><td>");
            s = s.Replace("\t", "</td><td>");
            s = s.Replace("<td><tr>", "<tr>");
            s += "</td></tr>";
            return s;
        }

        #endregion
 



        public static void OpenHelpUrl()
        {
            DomainCode.Common.OpenURL("http://roneaglin.online/bmptrains/help/");
        }

        public static void OpenURL(string url)
        {
            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch (Exception)
            {
                return;
            }
        }

        public static void OpenURL(int code)
        {
            try
            {
                string url = "http://storrmwater.ucf.edu/BMPVideo/?id=" + code.ToString();
                System.Diagnostics.Process.Start(url);
            }
            catch (Exception)
            {
                return;
            }
        }
        public static DialogResult ShowInputDialog(ref string prompt, string def = "00")
        {
            System.Drawing.Size size = new System.Drawing.Size(400, 140);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = "Name";

            System.Windows.Forms.Label label = new Label();
            label.Size = new System.Drawing.Size(size.Width - 10, 23);
            label.Location = new System.Drawing.Point(5, 5);
            label.Text = prompt;
            inputBox.Controls.Add(label);


            System.Windows.Forms.TextBox textBox = new TextBox();
            textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
            textBox.Location = new System.Drawing.Point(5, 46);
            textBox.Text = def;
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 79);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new System.Drawing.Point(size.Width - 80, 79);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            prompt = textBox.Text;
            return result;
        }
    }
}
