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
    
    public partial class genre
    {
        public genre()
        {
            this.media = new HashSet<media>();
        }
    
        public int genre_id { get; set; }
        public string genre1 { get; set; }
    
        public virtual ICollection<media> media { get; set; }
    }
}