using System;
using System.Windows.Forms;
using Time_Keeper.Models;
using Time_Keeper.Repositories;

namespace Time_Keeper
{
    public partial class CreateProject : Form
    {
        private readonly IProjectRepository _projectRepository;

        public CreateProject()
        {
            InitializeComponent();
            _projectRepository = new ProjectRepository();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (Valid())
            {
                ProjectModel model = new ProjectModel()
                {
                    Title = txtTitle.Text
                };

                try
                {
                    model = _projectRepository.Create(model);

                    Hide();
                    Main mainForm = new Main();
                    mainForm.Closed += (s, args) => Close();
                    mainForm.Show();
                }
                catch (Exception)
                {
                    lblError.Text = "Exception occured!";
                    txtTitle.Text = string.Empty;
                    txtTitle.Focus();
                }
            }
        }

        private bool Valid()
        {
            if (string.IsNullOrEmpty(txtTitle.Text))
            {
                lblError.Text = "Project Title is required field.";
                return false;
            }

            if (txtTitle.Text.Length >= 50)
            {
                lblError.Text = "Maximum 50 charaters allowed.";
                return false;
            }

            return true;
        }
    }
}
