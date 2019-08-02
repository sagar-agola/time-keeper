using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Time_Keeper.Helpers;
using Time_Keeper.Models;
using Time_Keeper.Repositories;

namespace Time_Keeper
{
    public partial class Analysis : Form
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IWorkRecordRepository _workRecordRepository;

        Func<ChartPoint, string> labelPoint = chartPoint => string.Format("{0} ({1:p})", chartPoint.Y, chartPoint.Participation);

        public Analysis()
        {
            InitializeComponent();
            _projectRepository = new ProjectRepository();
            _workRecordRepository = new WorkRecordRepository();
        }

        private void Analysis_Load(object sender, EventArgs e)
        {
            List<ProjectModel> projects = _projectRepository.GetAll();

            cboProjects.DataSource = Helper.ConvertToDatatable(projects);
            cboProjects.DisplayMember = "Title";
            cboProjects.ValueMember = "Id";

            cboProjects.SelectedIndex = -1;

            pieChart.LegendLocation = LegendLocation.Bottom;
        }

        private void cboProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProjects.SelectedIndex != -1)
            {
                bool parsed = int.TryParse(cboProjects.SelectedValue.ToString(), out int projectId);

                if (parsed)
                {
                    List<WorkRecordModel> workRecords = _workRecordRepository.GetByProject(projectId);
                    List<PieModel> list = new List<PieModel>();
                    SeriesCollection series = new SeriesCollection();

                    for (int i = 0; i < workRecords.Count; i++)
                    {
                        if (list.Any(x => x.Date == workRecords[i].StartDate.ToShortDateString()))
                        {
                            PieModel model = list.First(x => x.Date == workRecords[i].StartDate.ToShortDateString());

                            if (workRecords[i].EndDate != null)
                            {
                                model.Count += (int)((DateTime)workRecords[i].EndDate - workRecords[i].StartDate).TotalSeconds;
                            }
                        }
                        else
                        {
                            if (workRecords[i].EndDate != null)
                            {
                                PieModel model = new PieModel
                                {
                                    Date = workRecords[i].StartDate.ToShortDateString(),
                                    Count = (int)((DateTime)workRecords[i].EndDate - workRecords[i].StartDate).TotalSeconds
                                };

                                list.Add(model);
                            }
                        }
                    }

                    foreach (PieModel item in list)
                    {
                        series.Add(new PieSeries()
                        {
                            Title = item.Date,
                            Values = new ChartValues<int> { item.Count },
                            DataLabels = true,
                            LabelPoint = labelPoint
                        });
                    }

                    pieChart.Series = series;
                }
            }
        }
    }
}
