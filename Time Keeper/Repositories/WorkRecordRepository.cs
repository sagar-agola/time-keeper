using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Time_Keeper.Database;
using Time_Keeper.Models;

namespace Time_Keeper.Repositories
{
    public interface IWorkRecordRepository
    {
        List<WorkRecordModel> GetAll();
        List<WorkRecordModel> GetByProject(int projectId);

        WorkRecordModel Create(WorkRecordModel model);
        void Update(WorkRecordModel model);
        void Remove(int id);
    }

    public class WorkRecordRepository : IWorkRecordRepository
    {
        private Time_KeeperEntities _context;

        public WorkRecordModel Create(WorkRecordModel model)
        {
            WorkRecord modelMapping = new WorkRecord()
            {
                ProjectId = model.ProjectId,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };

            using (_context = new Time_KeeperEntities())
            {
                _context.WorkRecords.Add(modelMapping);
                _context.SaveChanges();
            }

            model.Id = modelMapping.Id;

            return model;
        }

        public List<WorkRecordModel> GetAll()
        {
            List<WorkRecord> model = _context.WorkRecords
                .Include(record => record.Project)
                .ToList();

            List<WorkRecordModel> modelMapping = new List<WorkRecordModel>();

            foreach (WorkRecord record in model)
            {
                modelMapping.Add(MapWorkRecord(record));
            }

            return modelMapping;
        }

        public List<WorkRecordModel> GetByProject(int projectId)
        {
            using (_context = new Time_KeeperEntities())
            {
                List<WorkRecord> model = _context.WorkRecords
                    .Include(record => record.Project)
                    .Where(record => record.ProjectId == projectId)
                    .ToList();

                List<WorkRecordModel> modelMapping = new List<WorkRecordModel>();

                foreach (WorkRecord record in model)
                {
                    modelMapping.Add(MapWorkRecord(record));
                }

                return modelMapping;
            }
        }

        public void Remove(int id)
        {
            using (_context = new Time_KeeperEntities())
            {
                WorkRecord model = _context.WorkRecords.FirstOrDefault(record => record.Id == id);

                _context.WorkRecords.Remove(model);
                _context.SaveChanges();
            }
        }

        public void Update(WorkRecordModel model)
        {
            using (_context = new Time_KeeperEntities())
            {
                WorkRecord modelMapping = _context.WorkRecords.FirstOrDefault(record => record.Id == model.Id);

                modelMapping.ProjectId = model.ProjectId;
                modelMapping.StartDate = model.StartDate;
                modelMapping.EndDate = model.EndDate;

                _context.Entry(modelMapping).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        private WorkRecordModel MapWorkRecord(WorkRecord model)
        {
            ProjectModel project = new ProjectModel()
            {
                Id = model.Project.Id,
                Title = model.Project.Title
            };

            return new WorkRecordModel()
            {
                Id = model.Id,
                ProjectId = model.ProjectId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                ProjectModel = project
            };
        }
    }
}
