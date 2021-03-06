namespace WindowsAppBase
{
    public partial class StartupForm : Form
    {
        public StartupForm()
        {
            InitializeComponent();
            Globals.Project = new DomainCode.SampleProject();
        }

        private void StartupForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = Globals.Project.AsXML();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            textBox1.Text = Globals.Project.Open();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Globals.Project.Save();
        }

        public void OpenFile(string filename)
        {

            string res = Globals.Project.openFromFile(filename);
            if (res == "")
            {
                Globals.Project.FileName = filename;
            }
            else
            {
                MessageBox.Show(res);
            }
            textBox1.Text = Globals.Project.AsXML();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Form form = new Forms.frmEditObject();
            form.ShowDialog();

            textBox1.Text = Globals.Project.AsXML();
        }

        private void btnDisplay_Click(object sender, EventArgs e)
        {
            textBox1.Text = Globals.Project.AsXML();
        }
    }
}