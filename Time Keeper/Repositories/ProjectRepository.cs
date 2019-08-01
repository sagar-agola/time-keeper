using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Time_Keeper.Database;
using Time_Keeper.Models;

namespace Time_Keeper.Repositories
{
    public interface IProjectRepository
    {
        List<ProjectModel> GetAll();
        ProjectModel Get(int id);
        ProjectModel Get(string name);

        ProjectModel Create(ProjectModel model);
        void Remove(int id);
    }

    public class ProjectRepository : IProjectRepository
    {
        private Time_KeeperEntities _context;

        public ProjectModel Create(ProjectModel model)
        {
            Project modelMapping = new Project()
            {
                Title = model.Title
            };

            using (_context = new Time_KeeperEntities())
            {
                _context.Projects.Add(modelMapping);
                _context.SaveChanges();
            }

            model.Id = modelMapping.Id;

            return model;
        }

        public ProjectModel Get(int id)
        {
            using (_context = new Time_KeeperEntities())
            {
                Project model = _context.Projects
                    .Include(project => project.WorkRecords)
                    .FirstOrDefault(project => project.Id == id);

                return MapProjectModel(model);
            }
        }

        public ProjectModel Get(string name)
        {
            using (_context = new Time_KeeperEntities())
            {
                Project model = _context.Projects
                .Include(project => project.WorkRecords)
                .FirstOrDefault(project => project.Title == name);

                return MapProjectModel(model);
            }
        }

        public List<ProjectModel> GetAll()
        {
            using (_context = new Time_KeeperEntities())
            {
                List<Project> model = _context.Projects
                    .Include(project => project.WorkRecords)
                    .ToList();

                List<ProjectModel> modelMapping = new List<ProjectModel>();

                foreach (Project project in model)
                {
                    modelMapping.Add(MapProjectModel(project));
                }

                return modelMapping;
            }
        }

        public void Remove(int id)
        {
            using (_context = new Time_KeeperEntities())
            {
                Project model = _context.Projects.FirstOrDefault(project => project.Id == id);

                _context.Projects.Remove(model);
                _context.SaveChanges();
            }
        }

        private ProjectModel MapProjectModel(Project model)
        {
            ProjectModel modelMapping = new ProjectModel()
            {
                Id = model.Id,
                Title = model.Title
            };

            List<WorkRecordModel> workRecordModels = new List<WorkRecordModel>();

            foreach (WorkRecord workRecord in model.WorkRecords)
            {
                WorkRecordModel workRecordModel = new WorkRecordModel()
                {
                    Id = workRecord.Id,
                    ProjectId = workRecord.ProjectId,
                    StartDate = workRecord.StartDate,
                    EndDate = workRecord.EndDate,
                };

                workRecordModels.Add(workRecordModel);
            }

            modelMapping.WorkRecordModels = workRecordModels;

            return modelMapping;
        }
    }
}
