//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time_Keeper.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class WorkRecord
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
    
        public virtual Project Project { get; set; }
    }
}