using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;

namespace WindowsAppBase.DomainCode
{

    // Required interface for all classes that inherit from
    // XMLPropertyObject. This allows the object to be represented
    // as an XML string and stored in a file or database
    public interface IXmlPropertyObject
    {
        // Required Properties
        int id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string Owner { get; set; }

        // Required Methods
        void setID(int ID);
        string XmlSerialize();
        void XmlDeserialize(string xml);

        string ShortClassName();
        string FullClassName();

        string GetValue(string propertyName);
        void SetValue(string propertyName, string value);

        string getPropertyFromXml(string property, string xml);
        void setPropertyFromXml(string property, string xml);

        // For generating reports
        string AsHtmlTable();
        bool CanPlot();
    }


    public interface INameControl
    {
        string Name { get; set; }
        string Description { get; set; }
        string Owner { get; }
    }
    
    public interface IUserControlPropertyInput
    {
        Dictionary<string, string> InputProperties { get; set; }
        Dictionary<string, string> InputPropertyValues { get; set; }

        string GetPropertyValue(string name);
        void SetPropertyValue(string name, string value);
    }

    // XmlPropertyObject provides a base class for all objects that 
    // will be stored in a database or file.
    [Serializable]
    public abstract class XmlPropertyObject : IXmlPropertyObject
    {
        public static string UserAnonymous = "Anonymous";
        public static string UserGlobal = "Global";

        public int id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }

        protected XmlPropertyObject()
        {
            Name = "Default Name";
            Description = "Default Description";
            Owner = "Owner Name";
        }

        protected XmlPropertyObject(int ID) : this()
        {
            id = ID;
        }

        protected XmlPropertyObject(string xml) :this()
        {
            XmlDeserialize(xml);

        }

        public string FullClassName()
        {
            return this.GetType().ToString();
        }

        public string ShortClassName()
        {
            string fc = this.GetType().ToString();
            while (fc.Contains("."))
            {
                fc = fc.Substring(fc.IndexOf(".") + 1);
            }
            return fc;
        }
        public object CreateInstanceOf(string strFullyQualifiedName)
        {
            Type t = Type.GetType(strFullyQualifiedName);
            return Activator.CreateInstance(t);
        }

        public void setID(int ID)
        {
            //id = ID;
            //Select(ID);
        }

        public bool CanPlot()
        {
            return false;
        }

        public string[] SplitOnNewLine(string s)
        {
            return s.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public string[] SplitOnTab(string s)
        {
            return s.Split(new String[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public List<string> AsListAtIndex(int index, string property)
        {
            // property = Multi-line property list delimited \n - columns delimited by \t
            // returns a single column specified by index

            string[] s = SplitOnNewLine(property);
            List<string> l = new List<string>();

            foreach (string row in s)
            {
                string[] cells = SplitOnTab(row);
                l.Add(cells[index]);
            }
            return l;
        }

        public string FullRowAtIndex(int index, string value, string property)
        {
            // property = Multi-line property list delimited \n - columns delimited by \t
            // returns a single column specified by index

            string[] s = SplitOnNewLine(property);

            foreach (string row in s)
            {
                string[] cells = SplitOnTab(row);
                if (cells[index] == value) return row;
            }
            return string.Empty;
        }

        #region XML Serialize


        public string XmlSerialize()
        {
            string s = String.Empty;
            string fc = ShortClassName();

            while (fc.Contains("."))
            {
                fc = fc.Substring(fc.IndexOf(".") + 1);
            }
            
            s += "<" + fc + ">\n";
            
            s += "<FullClassName>" + FullClassName() + "</FullClassName>\n";
            foreach (var property in this.GetType().GetProperties())
            {
                s += " <" + property.Name + ">";
                string t = GetValue(property);
                t = t.Replace('&', '_');
                if (property.PropertyType.Name == "Double")
                {
                    t = t.Replace("$", "");
                    t = t.Replace(",", "");
                }

                s += t;
                s += "</" + property.Name + ">";
                s += "\n";
            }

            s += "</" + fc + ">\n";
            return s;
        }

        public void setFrom(IXmlPropertyObject src)
        {
            // This sets all corresponding property values from a source object
            foreach (var property in GetType().GetProperties())
            {
                try
                {
                    this.SetValue(property, src.GetValue(property.Name));
                }
                catch
                {
                    // If no corresponding property, do nothing
                }

            }
        }

        // Serialize Creates an XML version of the properties for saving (with object id)
        public string XmlSerialize(string id)
        {
            string s = String.Empty;
            string fc = ShortClassName();

            while (fc.Contains("."))
            {
                fc = fc.Substring(fc.IndexOf(".") + 1);
            }
            if (id == "")
            {
                s += "<" + fc + ">\n";
            }
            else
            {
                s += "<" + fc + " id = \"" + id + "\">\n";
            }
            s += "<FullClassName>" + FullClassName() + "</FullClassName>\n";
            foreach (var property in this.GetType().GetProperties())
            {
                s += "  <" + property.Name + ">";
                string t = GetValue(property);
                t = t.Replace("&", "_");
                if (property.PropertyType.Name == "Double")
                {
                    t = t.Replace("$", "");
                    t = t.Replace(",", "");
                }
                s += t;
                s += "</" + property.Name + ">";
                s += "\n";
            }

            s += "</" + fc + ">\n";
            return s;
        }
        // Serialize Creates an XML version of the properties for saving (with object id)
        public string XmlSerialize(string id, string[] subs)
        {
            string s = String.Empty;
            string fc = ShortClassName();

            while (fc.Contains("."))
            {
                fc = fc.Substring(fc.IndexOf(".") + 1);
            }
            if (id == "")
            {
                s += "<" + fc + ">\n";
            }
            else
            {
                s += "<" + fc + " id = \"" + id + "\">\n";
            }
            // For every property in the Class - create the entry
            s += "<FullClassName>" + FullClassName() + "</FullClassName>\n";
            foreach (var property in this.GetType().GetProperties())
            {
                s += " <" + property.Name + ">";
                string t = GetValue(property);
                t = t.Replace('&', '_');
                if (property.PropertyType.Name == "Double") { 
                    t = t.Replace("$", "");
                    t = t.Replace(",", "");
                }
                s += t;
                s += "</" + property.Name + ">";
                s += "\n";
            }

            foreach (string sub in subs)
            {
                s += sub;
            }

            s += "</" + fc + ">\n";
            return s;
        }

        public string AsXML()
        {
            return XmlSerialize();
        }

        public virtual string AsXML(string id)
        {
            return XmlSerialize(id);
        }

        public string AsXML(string id, string[] subs)
        {
            return XmlSerialize(id, subs);
        }

        public void fromXML(string xml)
        {
            XmlDeserialize(xml);
        }

        public string GetValue(string propertyName)
        {
            PropertyInfo pi = this.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            return GetValue(pi);
        }

        public string GetValue(string propertyName, int places)
        {
            PropertyInfo pi = this.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            return GetValue(pi, places);
        }


        public string GetValue(PropertyInfo pi)
        {
            if (pi == null) return string.Empty;
            int places = PropertyDecimalPlaces(pi.Name);
            
            if (pi.GetValue(this) == null) return String.Empty;
            switch (pi.PropertyType.Name)
            {
                case "Double":
                    return GetValue(pi, places);
                case "Single":
                    return GetValue(pi, places);
                default:
                    return Convert.ToString(pi.GetValue(this));
            }
        }

        public string GetValue(PropertyInfo pi, int places)
        {
            if (places < 0) // Values less than 0 are $
            {
                if (pi.GetValue(this).ToString() == "NaN") return "$ 0.00";
                // Specifically to deal with numeric formats
                string formatString1 = "{0:C}" ;
                

                return String.Format(formatString1, pi.GetValue(this));
            }

            if (pi.GetValue(this).ToString() == "NaN") return "00";
            // Specifically to deal with numeric formats
            string formatString = "{0:N" + places.ToString().Trim() + "}";
            if (places == 0) return Convert.ToDouble(pi.GetValue(this)).ToString("#");

            return String.Format(formatString, pi.GetValue(this));
        }

        public string GetValue(double d, int places)
        {
            string formatString = "{0:N" + places.ToString().Trim() + "}";
            if (places == 0) return d.ToString("#");

            return String.Format(formatString, d);
        }

        public string GetValue(ArrayList al, int places)
        {
            string s = "\n";
            string formatString = "{0:N" + places.ToString().Trim() + "}";

            foreach (Double d in al)
            {
                s += String.Format(formatString, d) + "\n";
            }
            return s;
        }

        public void GetFromControl(INameControl uc)
        {
            Name = uc.Name;
            Owner = uc.Owner;
            Description = uc.Description;
        }

        public void SetControlValues(INameControl uc)
        {
            uc.Name = Name;
            uc.Description = Description;
        }

        #endregion

        #region XML Deserialize
        // Deserialize retrieves values from XML and
        // Fills in properties
        public void XmlDeserialize(string xml)
        {
            //Deserialize(xml);
            var doc = XDocument.Parse(xml);
            XmlDeserialize(doc);
        }

        public void XmlDeserialize(XDocument doc)
        {
            foreach (XElement xe in doc.Root.Elements())
            {
                SetValue(xe.Name.ToString(), xe.Value);
            }

        }

        public void SetValue(string propertyName, string value)
        {
            PropertyInfo pi = this.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (null != pi && pi.CanWrite) //&& value != String.Empty
            {
                SetValue(pi, value);
            }
        }

        
        public void SetValue(PropertyInfo pi, string value)
        {
            // This set the value of the current object property to the passed value
            var numerics = new List<string> { "Int32", "Int64", "Double", "Single" };
            if (numerics.Contains(pi.PropertyType.Name) && (value == String.Empty))
            {
                value = "0";
            }

            try
            {
                switch (pi.PropertyType.Name)
                {
                    default:
                        pi.SetValue(this, Convert.ChangeType(value, pi.PropertyType), null);
                        break;
                }

            }
            catch
            {

                // default is to do nothing;
            }
        }

        public void ClearProperties()
        {
            // Clears values of all properties of object

            foreach (var property in this.GetType().GetProperties())
            {
                SetValue(property.Name, String.Empty);
            }
        }

        public void setPropertyFromXml(string property, string xml)
        {
            string value = getPropertyFromXml(property, xml);
            SetValue(property, value);
        }

        public string getPropertyFromXml(string property, string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            // Gets the value fom XML
            try { return doc.Root.Element(property).Value; }
            catch { return String.Empty; }
        }


        #endregion

        #region Database Operations
        //public void Select(int ID)
        //{
        //    DataTable dt = XmlObjectDatabase.Select(ID);
        //    if (dt.Rows.Count == 0) return;

        //    XmlDeserialize(dt.Rows[0]["XmlData"].ToString());
        //    Name = dt.Rows[0]["Name"].ToString();
        //    Description = dt.Rows[0]["Description"].ToString();
        //    Owner = dt.Rows[0]["OwnerUserID"].ToString();
        //    id = ID;
        //}

        //public void SelectForName(string name)
        //{
        //    DataTable dt = XmlObjectDatabase.SelectForName(name);
        //    if (dt.Rows.Count == 0) return;

        //    XmlDeserialize(dt.Rows[0]["XmlData"].ToString());
        //    id = Convert.ToInt32(dt.Rows[0]["id"]);
        //    Name = dt.Rows[0]["Name"].ToString();
        //    Description = dt.Rows[0]["Description"].ToString();
        //    Owner = dt.Rows[0]["OwnerUserID"].ToString();
        //}


        //public DataTable SelectAll(string ownerID)
        //{

        //    return XmlObjectDatabase.SelectAll(ownerID, ShortClassName());
        //}

        //public DataTable SelectGlobal()
        //{
        //    return XmlObjectDatabase.SelectAll(UserGlobal, ShortClassName());
        //}

        //public DataTable SelectAll()
        //{
        //    if (String.IsNullOrEmpty(Owner)) Owner = UserAnonymous;
        //    return XmlObjectDatabase.SelectAll(new string[] { UserGlobal, Owner }, ShortClassName());
        //}

        //public int SelectCount()
        //{
        //    return XmlObjectDatabase.SelectCount(Owner, ShortClassName());
        //}

        //public int SelectCount(string ownerID)
        //{
        //    return XmlObjectDatabase.SelectCount(ownerID, ShortClassName());
        //}

        //public string Insert()
        //{
        //    return XmlObjectDatabase.Insert(this);
        //}

        //public string Insert(string name)
        //{
        //    //Ensures that the name is unique
        //    int count = XmlObjectDatabase.SelectCountForName(name, ShortClassName());
        //    if (count == 0)
        //    {
        //        return Insert();
        //    }
        //    else
        //    {
        //        if (id != 0) return Update();
        //    }
        //    return string.Empty;
        //}

        //public string InsertGlobal()
        //{
        //    string owner = Owner;
        //    Owner = UserGlobal;
        //    string ret = XmlObjectDatabase.Insert(this);
        //    Owner = owner;
        //    return ret;
        //}

        //public string Update()
        //{
        //    if (id == 0)
        //        return Insert();
        //    else
        //        return XmlObjectDatabase.Update(this);
        //}

        //public string Delete()
        //{
        //    if (id == 0) return "Object ID Not Set";
        //    return XmlObjectDatabase.Delete(id);
        //}

        #endregion

        #region User Interface Methods

        public string AsHtmlTable()
        {
            return this.AsHtmlTable(this.PropertyLabels());
        }        

        public string AsHtmlTable(Dictionary<string, string> p)
        {
            string s = "<table>";
            foreach (KeyValuePair<string, string> pair in p)
            {
                s += "<tr>";
                s += "<td>" + (pair.Value == "" ? pair.Key : pair.Value) + "</td>";
                string v = GetValue(pair.Key);
                s += "<td>" + v + "</td>";
                s += "</tr>";
            }
            s += "</table>";
            return s;
        }

        public string AsHtmlTable(string[] header, string values)
        {
            // This accepts a string that is tab delimited by columns, new line delimited for rows
            // String array header specifies headers
            if (values == null) return "";

            string s = "<table>";
            s += "<tr>";
            foreach (string h in header)
            {
                s += "<td>";
                s += h;
                s += "</td>";
            }
            s += "</tr>";

            // Now the data

            string[] rows = values.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string row in rows)
            {
                s += "<tr>";
                string[] columns = row.Split(new String[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string column in columns)
                {
                    s += "<td>";
                    s += column;
                    s += "</td>";
                }
                s += "</tr>";
            }
            s += "</table>";
            return s;
        }

        public void GetPropertiesFromUserControl(Dictionary<string, string> d, IUserControlPropertyInput uc)
        {
            // Sets the properties from a User Control that implements IUserControlPropertyInput
            // d - Dictionary of properties to set.

            foreach (KeyValuePair<string, string> p in d)
            {
                this.SetValue(p.Key, uc.GetPropertyValue(p.Key));
            }
        }

        public void SetPropertValuesInUserControl(Dictionary<string, string> d, IUserControlPropertyInput uc)
        {
            foreach (KeyValuePair<string, string> p in d)
            {
                uc.SetPropertyValue(p.Key, GetValue(p.Key).ToString());
            }
        }

        // Used in output to set labels for input and output
        public abstract Dictionary<string, string> PropertyLabels();
        public abstract Dictionary<string, int> PropertyDecimalPlaces();
        public int PropertyDecimalPlaces(string propertyName)
        {
            try { return PropertyDecimalPlaces()[propertyName]; }
            catch { return 3; }
        }

        public Dictionary<String, String> PropertyValues()
        {
            var d = this.GetType().GetProperties().ToDictionary(property => property.Name, 
                                property => Convert.ToString(property.GetValue(this)));

            return d;
        }

        // Lists as key - all properties USED IN INPUT, values are input labels
        //        public abstract Dictionary<string, string> InputProperties();
        /*
        public Dictionary<string, string> InputPropertyValues()
        {
            // Answers a dictionary of all values for properties
            // that are used in input
            return InputPropertyValues(InputProperties());
        }
        */

        public Dictionary<string, string> InputPropertyValues(Dictionary<string, string> properties)
        {
            // Answers a dictionary of all values for properties
            // that are used in input

            Dictionary<string, string> d2 = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> p in properties)
            {
                string v = GetValue(p.Key);
                if (v == "0") v = String.Empty;
                d2.Add(p.Key, v);
            }
            return d2;
        }

        public string ArrayListAsString(ArrayList al, int places)
        {
            if (al == null) return String.Empty;
            string s = String.Empty;
            string FormatString = "{0:N" + places.ToString().Trim() + "}";

            foreach (double d in al)
            {
                s += String.Format(FormatString, d) + "\n";
            }
            return s;
        }

        public string AsCode(string objectName = "")
        {
            string s = String.Empty;
            string fc = ShortClassName();

            while (fc.Contains("."))
            {
                fc = fc.Substring(fc.IndexOf(".") + 1);
            }

            foreach (var property in this.GetType().GetProperties())
            {
                if (objectName != "") s += objectName + ".";
                s += property.Name + " = ";
                if (property.PropertyType.Name == "String")
                {
                    string t = GetValue(property).Replace("\t", "\\t");
                    t = t.Replace("\n", "\\n");
                    if (t != "") s += "\"" + t + "\"";
                }
                else
                {
                    s += GetValue(property);
                }
                s += ";\n";
            }

            return s;
        }

        #endregion

        #region Static Helper Methods

        public Dictionary<string, string> Add(Dictionary<string, string> original, Dictionary<string, string> toBeAdded)
        {
            foreach (var newValue in toBeAdded)
            {
                try
                {
                    original.Add(newValue.Key, newValue.Value);
                }
                catch
                {
                    // Do Nothing 
                }
            }
            return original;
        }

        public Dictionary<string, int> Add(Dictionary<string, int> original, Dictionary<string, int> toBeAdded)
        {
            foreach (var newValue in toBeAdded)
            {
                try
                {
                    original.Add(newValue.Key, newValue.Value);
                }
                catch
                {
                    // Do Nothing 
                }
            }
            return original;
        }

        public static ArrayList AsArrayList(string s, string type = "Double", string[] delimiter = null)
        {
            if (String.IsNullOrEmpty(s)) return new ArrayList();

            if (delimiter == null) delimiter = new string[] { "\n", " ", ";", "\t" };

            // remove double spaces
            s = s.TrimEnd();
            s = s.TrimStart();
            while (s.Contains("  "))
            {
                s = s.Replace("  ", " ");
            }

            // makes a string array of all values
            string[] vals = s.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            // Reinitialize the ArrayList
            var arraylist = new ArrayList();
            int i = 0;
            foreach (string val in vals)
            {
                try
                {
                    switch (type)
                    {
                        case "Double":
                            arraylist.Add(Convert.ToDouble(val));
                            break;
                        case "Integer":
                            arraylist.Add(Convert.ToInt32(val));
                            break;
                        default:
                            arraylist.Add(val);
                            break;
                    }
                    i++;
                }
                catch
                { // Do nothing 
                }
            }
            return arraylist;
        }

        public List<double> AsListDouble(string s)
        {
            var desc = new List<double>();
            string[] rows = Description.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string row in rows)
            {
                desc.Add(Convert.ToDouble(row));
            }
            return desc;
        }

        public Dictionary<string, string> AsDictionary(string data)
        {
            //values has first element as key and second element as value
            data = data.TrimEnd();
            data = data.TrimStart();
            while (data.Contains("  "))
            {
                data = data.Replace("  ", " ");
            }

            var dictionary = new Dictionary<string, string>();
            // makes a string array of all values
            string[] vals = data.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string v in vals)
            {
                string[] xy = v.Split(new String[] { "\n", " ", ";", ",", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                dictionary.Add(xy[0], xy[1]);
            }
            return dictionary;
        }

        public static DateTime AsDateTime(string time)
        {
            DateTime ret = DateTime.MinValue;
            try
            {
                ret = Convert.ToDateTime(time);
            }
            catch
            {
                ret = DateTime.MinValue;
            }
            return ret;
        }

        public static string AsString(double value, int decimalPlaces)
        {
            string formatString = "{0:N" + decimalPlaces.ToString().Trim() + "}";
            return String.Format(formatString, value);
        }

        public static string AsString(ArrayList al, string type = "Double", int decimalPlaces = 0)
        {
            if (al == null) return String.Empty;

            string s = String.Empty;
            string formatString = "{0:N" + decimalPlaces.ToString().Trim() + "}";

            foreach (var d in al)
            {
                switch (type)
                {
                    case "Double":
                        s += String.Format(formatString, (double)d) + "\n";
                        break;
                    default:
                        s += Convert.ToString(d) + "\n";
                        break;
                }
            }
            return s;
        }

        public static string AsString(Dictionary<string, string> values)
        {
            string s = String.Empty;
            foreach (KeyValuePair<string, string> pair in values)
            {
                s += pair.Key + "\t" + pair.Value + "\n";
            }
            return s.Trim();
        }

        public static string AsCleanString(string s, string type, int decimalPlaces = 0)
        {
            string[] vals = s.Split(new String[] { "\n", " ", ";", "\t" }, StringSplitOptions.RemoveEmptyEntries);

            // Reinitialize the ArrayList
            var arraylist = new ArrayList();
            int i = 0;
            foreach (string val in vals)
            {
                try
                {
                    switch (type)
                    {
                        case "Double":
                            arraylist.Add(Convert.ToDouble(val));
                            break;
                        case "Integer":
                            arraylist.Add(Convert.ToInt32(val));
                            break;
                        default:
                            arraylist.Add(val);
                            break;
                    }
                    i++;
                }
                catch
                { // Do nothing 
                }
            }
            
            return AsString(arraylist, type, decimalPlaces);
        }

        public static string AsString(Dictionary<string, double> dict, int decimalPlaces = 0)
        {
            if (dict == null) return String.Empty;

            string s = String.Empty;
            string formatString = "{0:N" + decimalPlaces.ToString().Trim() + "}";

            foreach (var d in dict)
            {
                s += d.Key + "\t";
                s += String.Format(formatString, (d.Value)) + "\n";
            }
            return s;
        }


        public static string AsString(double[] d, int decimalPlaces)
        {
            string s = String.Empty;
            string formatString = "{0:N" + decimalPlaces.ToString().Trim() + "}";

            for (int i = 0; i < d.Length; i++)
            {
                s += String.Format(formatString, d[i]) + "\n";
            }
            return s;
        }

        public static string AsString(double[] a, double[] b, int decimalPlaces)
        {
            string s = String.Empty;
            string formatString = "{0:N" + decimalPlaces.ToString().Trim() + "}";

            for (int i = 0; i < a.Length; i++)
            {
                s += String.Format(formatString, a[i]);
                s += "\t";
                s += String.Format(formatString, b[i]);
                s += "\n";
            }
            return s;
        }

        public static string[] AsStringArray(string property)
        {
            string[] vals = property.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var s = new string[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                s[i] = Convert.ToString(vals[i]);
            }
            return s;
        }

        public static string[] AsStringArray(double[] values)
        {
            var s = new string[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                s[i] = Convert.ToString(values[i]);
            }
            return s;
        }


        public static object[] AsObjectArray(ArrayList al)
        {
            var objects = new object[al.Count];

            int i = 0;
            foreach (object o in al)
            {
                objects[i] = (double)o;
                i++;
            }
            return objects;
        }

        public static object[] AsObjectArray(string s)
        {
            ArrayList al = AsArrayList(s);
            return AsObjectArray(al);
        }

        public static double[] AsDoubleArray(ArrayList al)
        {
            var objects = new double[al.Count];

            int i = 0;
            foreach (object o in al)
            {
                objects[i] = (double)o;
                i++;
            }
            return objects;
        }

        public static double[] AsDoubleArray(ArrayList al, int length)
        {
            var objects = new double[length];

            for (int i = 0; i < length; i++)
            {
                try
                {
                    objects[i] = (double)al[i];
                }
                catch
                {
                    objects[i] = 0.0;
                }
            }
            return objects;
        }


        public static double[,] AsDoubleMatrix(string s)
        {
            // Designed for column data of lookup tables
            // rows separated by \n  columns by \t

            ArrayList rows = AsArrayList(s, "String", new string[] { "\n" });
            int nRows = rows.Count;

            ArrayList cols = AsArrayList(rows[0].ToString(), "Double", new string[] { "\t" });
            int nCols = cols.Count;

            double[,] values = new double[nRows, nCols];

            for (int i = 0; i < nRows; i++)
            {
                ArrayList col = AsArrayList(rows[i].ToString(), "Double", new string[] { "\t" });
                for (int j = 0; j < nCols; j++)
                {
                    values[i, j] = Convert.ToDouble(col[j]);
                }
            }
            return values;
        }

        public static double[] AsDoubleArray(string s)
        {
            ArrayList al = AsArrayList(s);
            return AsDoubleArray(al);
        }

        public static double[] AsDoubleArray(string s, int length)
        {
            ArrayList al = AsArrayList(s);
            return AsDoubleArray(al, length);
        }

        public static string GetUserName()
        {
            return "Anonymous";
        }

        public static string FormattedNumber(double number, int decimalPlaces)
        {
            string FormatString = "{0:N" + decimalPlaces.ToString().Trim() + "}";
            return String.Format(FormatString, number);
        }

        #endregion

    }


}
