using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using NotesKeeper.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotesKeeper.Infrastructure.Interceptors
{
    public class DateOfCreationInterceptor : SaveChangesInterceptor
    {
        private readonly ILogger<DateOfCreationInterceptor>? _logger;
        public DateOfCreationInterceptor(ILogger<DateOfCreationInterceptor>? logger = null)
        {
            _logger = logger;
        }
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData == null)
                return result;


            foreach (var entry in eventData.Context.ChangeTracker.Entries())
            {
                if (entry is null || entry.State != EntityState.Added || !(entry.Entity is IAtDateCreatable entity))
                    continue;

                _logger?.LogInformation("{Interceptor} applied interceptor", nameof(DateOfCreationInterceptor));

                entity.CreatedAt = DateTime.UtcNow;
            }


            return result;
        }
    }
}
