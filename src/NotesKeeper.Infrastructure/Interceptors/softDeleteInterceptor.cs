using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using NotesKeeper.Core.Domain.Entities;
using NotesKeeper.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotesKeeper.Infrastructure.Interceptors
{
    public class softDeleteInterceptor : SaveChangesInterceptor
    {
        private readonly ILogger<softDeleteInterceptor>? _logger;
        public softDeleteInterceptor(ILogger<softDeleteInterceptor>? logger = null) { 
            _logger = logger;
        }
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData == null) return result;

            foreach(var entry in eventData.Context.ChangeTracker.Entries().ToList())
            {
                if(entry is null || entry.State != EntityState.Deleted || !(entry.Entity is ISoftDeletable entity)) continue;

                _logger?.LogInformation("{Interceptor} applied interceptor", nameof(softDeleteInterceptor));

                entry.State = EntityState.Modified;
                entity.IsDeleted = true;
                
                // for tags, remove the tag assignments
                if(entry.Entity is Tag tag)
                {
                    // explicitly load the assignments
                    await eventData.Context.Entry(tag).Collection(t => t.TagsAssignments).LoadAsync();
                    eventData.Context.RemoveRange(tag.TagsAssignments);
                }
            }


            return result;
        }
    }
}
