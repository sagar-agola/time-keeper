using System;

namespace Time_Keeper.Models
{
    public class WorkRecordModel
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public ProjectModel ProjectModel { get; set; }
    }
}