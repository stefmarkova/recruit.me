﻿namespace RecruitMe.Data
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    using RecruitMe.Data.Common.Models;
    using RecruitMe.Data.Models;
    using RecruitMe.Data.Models.EnumModels;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private static readonly MethodInfo SetIsDeletedQueryFilterMethod =
            typeof(ApplicationDbContext).GetMethod(
                nameof(SetIsDeletedQueryFilter),
                BindingFlags.NonPublic | BindingFlags.Static);

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Candidate> Candidates { get; set; }

        public DbSet<Document> Documents { get; set; }

        public DbSet<Employer> Employers { get; set; }

        public DbSet<JobApplication> JobApplications { get; set; }

        public DbSet<JobOffer> JobOffers { get; set; }

        public DbSet<JobApplicationStatus> ApplicationStatuses { get; set; }

        public DbSet<DocumentCategory> DocumentCategories { get; set; }

        public DbSet<FileExtension> FileExtensions { get; set; }

        public DbSet<JobLevel> JobLevels { get; set; }

        public DbSet<JobSector> JobSectors { get; set; }

        public DbSet<JobType> JobTypes { get; set; }

        public DbSet<Language> Languages { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public DbSet<CandidateLanguage> CandidateLanguages { get; set; }

        public DbSet<CandidateSkill> CandidateSkills { get; set; }

        public DbSet<JobOfferLanguage> JobOfferLanguages { get; set; }

        public DbSet<JobOfferSkill> JobOfferSkills { get; set; }

        public DbSet<JobOfferJobType> JobOfferJobTypes { get; set; }

        public DbSet<JobApplicationDocument> JobApplicationDocuments { get; set; }

        public override int SaveChanges() => this.SaveChanges(true);

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.ApplyAuditInfoRules();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            this.SaveChangesAsync(true, cancellationToken);

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            this.ApplyAuditInfoRules();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Needed for Identity models configuration
            base.OnModelCreating(builder);

            ConfigureUserIdentityRelations(builder);

            ConfigureEntityRelations(builder);

            EntityIndexesConfiguration.Configure(builder);

            var entityTypes = builder.Model.GetEntityTypes().ToList();

            // Set global query filter for not deleted entities only
            var deletableEntityTypes = entityTypes
                .Where(et => et.ClrType != null && typeof(IDeletableEntity).IsAssignableFrom(et.ClrType));
            foreach (var deletableEntityType in deletableEntityTypes)
            {
                var method = SetIsDeletedQueryFilterMethod.MakeGenericMethod(deletableEntityType.ClrType);
                method.Invoke(null, new object[] { builder });
            }

            // Disable cascade delete
            var foreignKeys = entityTypes
                .SelectMany(e => e.GetForeignKeys().Where(f => f.DeleteBehavior == DeleteBehavior.Cascade));
            foreach (var foreignKey in foreignKeys)
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        private static void ConfigureUserIdentityRelations(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>()
                .HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(e => e.Logins)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(e => e.Roles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigureEntityRelations(ModelBuilder builder)
        {
            builder.Entity<CandidateLanguage>()
                .HasKey(cl => new { cl.CandidateId, cl.LanguageId });
            builder.Entity<CandidateLanguage>()
                .HasOne(cl => cl.Candidate)
                .WithMany(c => c.Languages)
                .HasForeignKey(cl => cl.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CandidateLanguage>()
               .HasOne(cl => cl.Language)
               .WithMany(l => l.Candidates)
               .HasForeignKey(cl => cl.LanguageId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CandidateSkill>()
                .HasKey(cs => new { cs.CandidateId, cs.SkillId });
            builder.Entity<CandidateSkill>()
                .HasOne(cs => cs.Candidate)
                .WithMany(c => c.Skills)
                .HasForeignKey(cs => cs.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CandidateSkill>()
               .HasOne(cs => cs.Skill)
               .WithMany(s => s.Candidates)
               .HasForeignKey(cs => cs.SkillId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobApplicationDocument>()
               .HasKey(jad => new { jad.JobApplicationId, jad.DocumentId });
            builder.Entity<JobApplicationDocument>()
                .HasOne(jad => jad.JobApplication)
                .WithMany(ja => ja.Documents)
                .HasForeignKey(jad => jad.JobApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobApplicationDocument>()
               .HasOne(jad => jad.Document)
               .WithMany(d => d.JobApplications)
               .HasForeignKey(jad => jad.DocumentId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobOfferLanguage>()
                .HasKey(jol => new { jol.JobOfferId, jol.LanguageId });
            builder.Entity<JobOfferLanguage>()
                .HasOne(jol => jol.Language)
                .WithMany(l => l.JobOffers)
                .HasForeignKey(jol => jol.LanguageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobOfferLanguage>()
               .HasOne(jol => jol.JobOffer)
               .WithMany(jo => jo.Languages)
               .HasForeignKey(jol => jol.JobOfferId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobOfferSkill>()
                .HasKey(jos => new { jos.JobOfferId, jos.SkillId });
            builder.Entity<JobOfferSkill>()
                .HasOne(jos => jos.Skill)
                .WithMany(s => s.JobOffers)
                .HasForeignKey(jos => jos.SkillId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobOfferSkill>()
               .HasOne(jos => jos.JobOffer)
               .WithMany(jo => jo.Skills)
               .HasForeignKey(jos => jos.JobOfferId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobOfferJobType>()
                .HasKey(jojt => new { jojt.JobOfferId, jojt.JobTypeId });
            builder.Entity<JobOfferJobType>()
                .HasOne(jojt => jojt.JobOffer)
                .WithMany(jo => jo.JobTypes)
                .HasForeignKey(jojt => jojt.JobOfferId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobOfferJobType>()
               .HasOne(jojt => jojt.JobType)
               .WithMany(jt => jt.JobOffers)
               .HasForeignKey(jojt => jojt.JobTypeId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Candidate>()
                .HasOne(c => c.ApplicationUser)
                .WithOne(au => au.Candidate)
                .HasForeignKey<Candidate>(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Employer>()
                .HasOne(e => e.ApplicationUser)
                .WithOne(au => au.Employer)
                .HasForeignKey<Employer>(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void SetIsDeletedQueryFilter<T>(ModelBuilder builder)
            where T : class, IDeletableEntity
        {
            builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
        }

        private void ApplyAuditInfoRules()
        {
            var changedEntries = this.ChangeTracker
                .Entries()
                .Where(e =>
                    e.Entity is IAuditInfo &&
                    (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in changedEntries)
            {
                var entity = (IAuditInfo)entry.Entity;
                if (entry.State == EntityState.Added && entity.CreatedOn == default)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                }
                else
                {
                    entity.ModifiedOn = DateTime.UtcNow;
                }
            }
        }
    }
}
