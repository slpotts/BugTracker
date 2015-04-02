namespace BugTracker.Migrations
{
    using BugTracker.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            //ContextKey = "BugTracker.Models.ApplicationDbContext";
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
            var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!context.TicketStatuses.Any(r => r.Name == "New"))
                context.TicketStatuses.Add(new TicketStatus { Name = "New" });

            if (!context.TicketStatuses.Any(r => r.Name == "Assigned"))
                context.TicketStatuses.Add(new TicketStatus { Name = "Assigned" });

            if (!context.TicketStatuses.Any(r => r.Name == "Resolved"))
                context.TicketStatuses.Add(new TicketStatus { Name = "Resolved" });

            if (!context.TicketPriorities.Any(r => r.Name == "High"))
                context.TicketPriorities.Add(new TicketPriority { Name = "High" });

            if (!context.TicketPriorities.Any(r => r.Name == "Normal"))
                context.TicketPriorities.Add(new TicketPriority { Name = "Normal" });

            if (!context.TicketPriorities.Any(r => r.Name == "Low"))
                context.TicketPriorities.Add(new TicketPriority { Name = "Low" });

            if (!context.TicketTypes.Any(r => r.Name == "Bug Fix"))
                context.TicketTypes.Add(new TicketType { Name = "Bug Fix" });

            if (!context.TicketTypes.Any(r => r.Name == "Feature Request"))
                context.TicketTypes.Add(new TicketType { Name = "Feature Request" });

            if (!context.Roles.Any(r => r.Name == "Administrator"))
                rm.Create(new IdentityRole { Name = "Administrator" });

            if (!context.Roles.Any(r => r.Name == "Project Manager"))
                rm.Create(new IdentityRole { Name = "Project Manager" });

            if (!context.Roles.Any(r => r.Name == "Developer"))
                rm.Create(new IdentityRole { Name = "Developer" });

            if (!context.Roles.Any(r => r.Name == "Submitter"))
                rm.Create(new IdentityRole { Name = "Submitter" });

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var Me = context.Users.FirstOrDefault(u => u.Email == "steven@fleetinginfinity.com");

            if (Me == null)
                um.Create(new ApplicationUser
                {
                    UserName = "steven@fleetinginfinity.com",
                    Email = "steven@fleetinginfinity.com",
                    FirstName = "Steven",
                    LastName = "Potts"
                }, "Abc123!");

            var user = um.FindByEmail("steven@fleetinginfinity.com");
            if (!um.IsInRole(user.Id, "Administrator"))
                um.AddToRole(user.Id, "Administrator");
        }
    }
}
