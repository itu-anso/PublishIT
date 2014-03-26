using System;
using System.Data.Entity;

namespace PublishITService
{
    /// <summary>
    /// Interface for every entity class in our database
    /// </summary>
    public interface IPublishITEntities : IDisposable
    {
        IDbSet<document> document { get; set; }
        IDbSet<format> format { get; set; }
        IDbSet<genre> genre { get; set; }
        IDbSet<media> media { get; set; }
        IDbSet<organization> organization { get; set; }
        IDbSet<person> person { get; set; }
        IDbSet<profession> profession { get; set; }
        IDbSet<rating> rating { get; set; }
        IDbSet<rent> rent { get; set; }
        IDbSet<role> role { get; set; }
        IDbSet<user> user { get; set; }
        IDbSet<video> video { get; set; }
        int SaveChanges();
    }
}