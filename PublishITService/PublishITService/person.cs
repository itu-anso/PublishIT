//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PublishITService
{
    using System;
    using System.Collections.Generic;
    
    public partial class person
    {
        public person()
        {
            this.profession = new HashSet<profession>();
        }
    
        public int person_id { get; set; }
        public string name { get; set; }
        public System.DateTime birthday { get; set; }
    
        public virtual ICollection<profession> profession { get; set; }
    }
}
