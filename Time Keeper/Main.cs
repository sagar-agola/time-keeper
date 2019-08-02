using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Time_Keeper.Models;
using Time_Keeper.Repositories;
using Time_Keeper.Helpers;

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

            cboProjects.DataSource = Helper.ConvertToDatatable(projects);
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

                        dt.Columns.Add("Project Name", typeof(string));
                        dt.Columns.Add("Start Date", typeof(string));
                        dt.Columns.Add("End Date", typeof(string));

                        foreach (WorkRecordModel workRecord in workRecords)
                        {
                            DataRow dr = dt.NewRow();

                            dr["Project Name"] = workRecord.ProjectModel.Title;
                            dr["Start Date"] = workRecord.StartDate;
                            dr["End Date"] = workRecord.EndDate == null ? "-" : (object)workRecord.EndDate;

                            dt.Rows.Add(dr);
                        }

                        grid.DataSource = dt;

                        grid.Columns[0].Width = 240;
                        grid.Columns[1].Width = 200;
                        grid.Columns[2].Width = 200;

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
                    else
                    {
                        btnStartWork.Enabled = true;
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

        private void btnViewAnalysis_Click(object sender, EventArgs e)
        {
            Analysis analysis = new Analysis();
            analysis.Show();
        }
    }
}
