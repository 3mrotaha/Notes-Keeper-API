using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using NotesKeeper.Core.Domain.Entities;
using NotesKeeper.Core.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotesKeeper.Infrastructure.ApplicationDbContext
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public virtual DbSet<Reminder> Reminders { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        
        public AppDbContext(DbContextOptions options) : base(options) { }        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>(eb =>
            {
                eb.HasMany(a => a.Notes)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

                eb.HasMany(a => a.Tags)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Reminder>(eb =>
            {
                eb.ToTable("Reminders", schema: "Notifications")
                .HasKey(r => r.Id);
            });

            modelBuilder.Entity<Note>(eb =>
            {
                eb.ToTable("Notes", schema: "Contents")
                .HasKey(n => n.Id);

                eb.HasOne(n => n.Reminder)
                .WithOne()
                .HasForeignKey<Reminder>(r => r.NoteId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Tag>(eb =>
            {
                eb.ToTable("Tags", schema: "Contents")
                .HasKey(tag => tag.Id);

                eb.HasQueryFilter(t => !t.IsDeleted); // return only the undeleted tags

                eb.HasMany(t => t.Notes)
                .WithMany(n => n.Tags)
                .UsingEntity<TagsAssignments>(
                j =>
                    j.HasOne(ta => ta.Note)
                    .WithMany(n => n.TagsAssignments)
                    .HasForeignKey(ta => ta.NoteId)
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                    j.HasOne(ta => ta.Tag)
                    .WithMany(t => t.TagsAssignments)
                    .HasForeignKey(ta => ta.TagId)
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("TagsAssignments", schema: "Contents");
                    j.HasKey(ta => new { ta.NoteId, ta.TagId });
                });                
            });



            base.OnModelCreating(modelBuilder);
        }
    }
}
