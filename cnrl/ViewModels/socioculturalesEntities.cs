using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace cnrl
{
   
    public partial class socioculturalesEntities : DbContext
    {

        public override int SaveChanges()
        {
            foreach (var auditableEntity in ChangeTracker.Entries<IAuditable>())
            {
                if (auditableEntity.State == EntityState.Added ||
                    auditableEntity.State == EntityState.Modified)
                {
                    // implementation may change based on the useage scenario, this
                    // sample is for forma authentication.
                    var currentUser = !string.IsNullOrEmpty(System.Web.HttpContext.Current?.User?.Identity?.Name)
                ? HttpContext.Current.User.Identity.Name
                : "Anonymous";
                    // modify updated date and updated by column for 
                    // adds of updates.
                    auditableEntity.Entity.DateModified = DateTime.Now;
                    auditableEntity.Entity.UserModified = currentUser;

                    // pupulate created date and created by columns for
                    // newly added record.
                    if (auditableEntity.State == EntityState.Added)
                    {
                        auditableEntity.Entity.DateCreated = DateTime.Now;
                        auditableEntity.Entity.UserCreated = currentUser;
                    }
                    else
                    {
                       
                        // we also want to make sure that code is not inadvertly
                        // modifying created date and created by columns 
                        auditableEntity.Entity.DateCreated = DateTime.Now;
                        auditableEntity.Entity.UserCreated = currentUser;
                        auditableEntity.Property(p => p.DateCreated).IsModified = false;
                        auditableEntity.Property(p => p.UserCreated).IsModified = false;

                        
                 
                    }
                }
            }

            return base.SaveChanges();
        }

    }
    public interface IAuditable
    {
        DateTime DateCreated { get; set; }
        String UserCreated { get; set; }
        DateTime DateModified { get; set; }
        String UserModified { get; set; }
    }

}