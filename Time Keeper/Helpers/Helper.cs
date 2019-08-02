using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Time_Keeper.Models;

namespace Time_Keeper.Helpers
{
    public static class Helper
    {
        public static DataTable ConvertToDatatable(List<ProjectModel> data)
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
