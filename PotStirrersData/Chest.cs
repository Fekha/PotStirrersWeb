//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Chest
    {
        public int ChestId { get; set; }
        public int UserId { get; set; }
        public Nullable<int> ChestSize { get; set; }
        public bool IsOpened { get; set; }
    
        public virtual Player Player { get; set; }
    }
}
