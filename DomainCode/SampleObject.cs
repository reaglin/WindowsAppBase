using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsAppBase.DomainCode
{
    public class SampleObject : XmlPropertyObject
    {
        #region "Properties"
        public String aString { get; set; }
        public Double aDouble { get; set; }
        public int aInt { get; set; }
        #endregion

        #region Constructor
        public SampleObject()
        {
            aString = "String Example";
            aDouble = 3.14159;
            aInt = 1;
        }
        #endregion

        #region "Requred Abstract Override"
        public override Dictionary<string, string> PropertyLabels()
        {
            return new Dictionary<string, string>
            {
                {"aString", "An Example of a String" },
                {"aDouble", "An Example of a Double Precision" },
                {"aInt", "An Example of an Integer" }
            };
        }

        public override Dictionary<string, int> PropertyDecimalPlaces()
        {
            return new Dictionary<string, int>
            {
                {"aDouble", 2 }
            };
        }
        #endregion

#region "Populate and Retrieve"
        public void PushValues(SampleObject passedObject)
        {
            // Pushes Values to the Passed Object
            passedObject.aString = aString;
            passedObject.aDouble = aDouble; 
            passedObject.aInt = aInt;
        }

        public void PullValues(SampleObject passedObject)
        {
            // Pulls Values from the Passed Obejct
            aString = passedObject.aString;
            aDouble = passedObject.aDouble;
            aInt = passedObject.aInt;
        }
        #endregion
    }
}
