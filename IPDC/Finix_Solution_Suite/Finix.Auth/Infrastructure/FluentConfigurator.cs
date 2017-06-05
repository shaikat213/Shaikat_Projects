//using Finix.HRM.Infrastructure.Models_and_Mappiings.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.Infrastructure
{
    public static class FluentConfigurator
    {
        public static void ConfigureGenericSettings(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //modelBuilder.Properties<string>().Configure(c => c.HasColumnType("varchar"));
            //Do Settings
        }

        public static void ConfigureOne2OneMappings(DbModelBuilder modelBuilder)
        {

        }

        public static void ConfigureMany2ManyMappings(DbModelBuilder modelBuilder)
        {
            ConfigureAuthMappings(modelBuilder);
            ConfigureHRMany2ManyMappings(modelBuilder);
        }
        #region Many2ManyMappings

        private static void ConfigureAuthMappings(DbModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<User>()
                 .HasMany(x => x.Roles)
                 .WithMany(x => x.Users)
                 .Map(m =>
                 {
                     m.MapLeftKey("UserId");
                     m.MapRightKey("RoleId");
                     m.ToTable("UserRole");
                 });

            modelBuilder.Entity<Application>()
                .HasMany(a => a.Modules)
                .WithMany(a => a.Applications)
                .Map(m =>
                {
                    m.MapLeftKey("ApplicationId");
                    m.MapRightKey("ModuleId");
                    m.ToTable("ApplicationModule");
                });
            modelBuilder.Entity<Menu>()
                .HasMany(m => m.Tasks)
                .WithMany(t => t.Menus)
                .Map(m =>
                {
                    m.MapLeftKey("MenuId");
                    m.MapRightKey("TaskId");
                    m.ToTable("MenuTask");
                });

        }

        private static void ConfigureHRMany2ManyMappings(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Institute>()
            //    .HasMany(x => x.Degrees)
            //    .WithMany(x => x.Institutes)
            //    .Map(m =>
            //    {
            //        m.MapLeftKey("InstituteId");
            //        m.MapRightKey("DegreeId");
            //        m.ToTable("InstituteDegree");
            //    });

            //modelBuilder.Entity<TrainingVendor>()
            //    .HasMany(x => x.Trainings)
            //    .WithMany(x => x.TrainingVendors)
            //    .Map(m =>
            //    {
            //        m.MapLeftKey("TrainingVendorId");
            //        m.MapRightKey("TrainingId");
            //        m.ToTable("TrainingVendorTraining");
            //    });

            //modelBuilder.Entity<Designation>()
            //    .HasMany(x => x.KPIs)
            //    .WithMany(x => x.Designations)
            //    .Map(m =>
            //    {
            //        m.MapLeftKey("DesignationId");
            //        m.MapRightKey("KPIId");
            //        m.ToTable("DesignationKPI");
            //    });


        }

        #endregion

        #region One2OneMappings

        private static void ConfigureStudentMappings(DbModelBuilder modelBuilder)
        {
            //student & studentAddress
            //RemoveAutoIncrement<StudentAddress>(modelBuilder);
            //  modelBuilder.Entity<StudentAddress>().HasRequired(ad => ad.Student).WithOptional(s => s.StudentAddress);
        }

        #endregion

        private static void RemoveAutoIncrement<T>(DbModelBuilder modelBuilder) where T : Entity
        {
            modelBuilder.Entity<T>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }
}
