using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsAppBase;
using WindowsAppBase.DomainCode;


namespace WindowsAppBase.Forms
{
    
    public partial class frmEditObject : Form
    {
        // Declare the Object you will work on
        private SampleObject theObject;
        public frmEditObject()
        {
            InitializeComponent();
            theObject = new SampleObject();
            Globals.Project.PushValues(theObject);
        }

        private void frmAnObject_Load(object sender, EventArgs e)
        {
            setFormValues();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Getting Values from Text Boxes
            getValuesFromForm();

            // Update teh Global Object
            Globals.Project.PullValues(theObject);  
        }

        private void setFormValues()
        {
            tbAString.Text = theObject.aString;
            Common.setValue(tbADouble, theObject.aDouble);
            Common.setValue(tbAInt, theObject.aInt);
        }

        private void getValuesFromForm()
        {
            theObject.aString = tbAString.Text;
            theObject.aDouble = Common.getDouble(tbADouble);
            theObject.aInt = Common.getInt(tbAInt);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
