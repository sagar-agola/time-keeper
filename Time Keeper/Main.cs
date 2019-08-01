using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Time_Keeper.Models;
using Time_Keeper.Repositories;

namespace Time_Keeper
{
    public partial class Main : Form
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IWorkRecordRepository _workRecordRepository;

        public Main()
        {
            InitializeComponent();
            _projectRepository = new ProjectRepository();
            _workRecordRepository = new WorkRecordRepository();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            List<ProjectModel> projects = _projectRepository.GetAll();

            cboProjects.DataSource = ConvertToDatatable(projects);
            cboProjects.DisplayMember = "Title";
            cboProjects.ValueMember = "Id";

            cboProjects.SelectedIndex = -1;

            btnStartWork.Enabled = false;
            btnFinishWork.Enabled = false;
        }

        private void cboProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            grid.DataSource = null;

            if (cboProjects.SelectedIndex != -1)
            {
                bool parsed = int.TryParse(cboProjects.SelectedValue.ToString(), out int projectId);

                if (parsed)
                {
                    List<WorkRecordModel> workRecords = _workRecordRepository.GetByProject(projectId);

                    if (workRecords.Count > 0)
                    {
                        DataTable dt = new DataTable();

                        dt.Columns.Add("Id", typeof(int));
                        dt.Columns.Add("Project Name", typeof(string));
                        dt.Columns.Add("Start Date", typeof(string));
                        dt.Columns.Add("End Date", typeof(string));

                        foreach (WorkRecordModel workRecord in workRecords)
                        {
                            DataRow dr = dt.NewRow();

                            dr["Id"] = workRecord.Id;
                            dr["Project Name"] = workRecord.ProjectModel.Title;
                            dr["Start Date"] = workRecord.StartDate;
                            dr["End Date"] = workRecord.EndDate == null ? "-" : (object)workRecord.EndDate;

                            dt.Rows.Add(dr);
                        }

                        grid.DataSource = dt;

                        grid.Columns[0].Width = 50;
                        grid.Columns[grid.Columns.Count - 1].Width = 200;
                        grid.Columns[grid.Columns.Count - 2].Width = 200;

                        DataRow lastRow = dt.Rows[dt.Rows.Count - 1];

                        if (lastRow["End Date"].ToString() == "-")
                        {
                            btnStartWork.Enabled = false;
                            btnFinishWork.Enabled = true;
                        }
                        else
                        {
                            btnStartWork.Enabled = true;
                            btnFinishWork.Enabled = false;
                        }
                    }
                }
            }
        }

        private void btnCreateProject_Click(object sender, EventArgs e)
        {
            CreateProject createProjectForm = new CreateProject();
            createProjectForm.Show();
        }

        private void btnStartWork_Click(object sender, EventArgs e)
        {
            if (cboProjects.SelectedIndex != -1)
            {
                bool parsed = int.TryParse(cboProjects.SelectedValue.ToString(), out int projectId);

                if (parsed)
                {
                    WorkRecordModel model = new WorkRecordModel
                    {
                        StartDate = DateTime.Now,
                        ProjectId = projectId
                    };

                    _workRecordRepository.Create(model);

                    cboProjects_SelectedIndexChanged(sender, e);
                }
            }
        }

        private void btnFinishWork_Click(object sender, EventArgs e)
        {
            WorkRecordModel model = new WorkRecordModel
            {
                Id = int.Parse(grid.Rows[grid.Rows.Count - 1].Cells[0].Value.ToString()),
                StartDate = DateTime.Parse(grid.Rows[grid.Rows.Count - 1].Cells[2].Value.ToString()),
                EndDate = DateTime.Now,
                ProjectId = _projectRepository.Get(grid.Rows[grid.Rows.Count - 1].Cells[1].Value.ToString()).Id
            };

            _workRecordRepository.Update(model);

            cboProjects_SelectedIndexChanged(sender, e);
        }

        private static DataTable ConvertToDatatable(List<ProjectModel> data)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(ProjectModel));
            DataTable table = new DataTable();

            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            object[] values = new object[props.Count];

            foreach (ProjectModel project in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(project);
                }
                table.Rows.Add(values);
            }

            return table;
        }
    }
}
