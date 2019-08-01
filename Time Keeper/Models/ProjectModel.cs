using System.Collections.Generic;

namespace Time_Keeper.Models
{
    public class ProjectModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public List<WorkRecordModel> WorkRecordModels { get; set; }
    }
}
