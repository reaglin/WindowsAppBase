using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjecName_ProjectVersion
{
    public class InterfaceCommon
    {
        public static void setBrowserStyles(WebBrowser w)
        {
            // This is reserved to set custom styles for the WB control

        }

        public static string Yes()
        {
            return "<span style='color:green; font-weight:bold'> Yes</span>";
        }
        public static string No()
        {
            return "<span style='color:red; font-weight:bold'> No</span>";
        }

        public static string YesNo(Boolean val)
        {
            if (val)
                return Yes();
            else
                return No();
        }

    }
}
