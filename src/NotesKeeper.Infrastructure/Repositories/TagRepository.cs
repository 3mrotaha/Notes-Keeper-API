using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotesKeeper.Core.Domain.Entities;
using NotesKeeper.Core.Domain.RepositoryContracts.TagRepositoryContracts;
using NotesKeeper.Infrastructure.ApplicationDbContext;
using System.Linq.Expressions;

namespace NotesKeeper.Infrastructure.Repositories
{
    public class TagRepository : ITagAddRepository, ITagGetRepository, ITagUpdateRepository, ITagDeleteRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<TagRepository> _logger;

        public TagRepository(AppDbContext dbContext, ILogger<TagRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<int> AddTag(Tag tag)
        {
            _logger.LogDebug("AddTag DB operation started for UserId {UserId}, Name '{Name}'", tag.UserId, tag.Name);
            _dbContext.Tags.Add(tag);
            int result = await _dbContext.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("Tag {TagId} '{Name}' persisted to DB", tag.Id, tag.Name);
                return tag.Id;
            }

            _logger.LogWarning("AddTag SaveChanges returned 0 rows for UserId {UserId}", tag.UserId);
            return -1;
        }

        public async Task<Tag?> GetTag(int tagId)
        {
            _logger.LogDebug("GetTag DB query for TagId {TagId}", tagId);
            var tag = await _dbContext.Tags.AsNoTracking()
                                        .FirstOrDefaultAsync(t => t.Id == tagId);
            if (tag is null)
                _logger.LogWarning("GetTag: TagId {TagId} not found in DB", tagId);

            return tag;
        }

        public async Task<Tag?> GetTag(string name)
        {
            _logger.LogDebug("GetTag DB query for Name '{Name}'", name);
            var tag = await _dbContext.Tags.AsNoTracking()
                                            .FirstOrDefaultAsync(t => t.Name == name);
            if (tag is null)
                _logger.LogWarning("GetTag: Tag with Name '{Name}' not found in DB", name);

            return tag;
        }

        public async Task<Tag?> GetTag(Expression<Predicate<Tag>> predicate)
        {
            _logger.LogDebug("GetTag (predicate) DB query initiated");
            var funcExpression = ExpressionConverter.ToFuncExpression(predicate);
            return await _dbContext.Tags.AsNoTracking()
                                    .FirstOrDefaultAsync(funcExpression);
        }

        public async Task<Tag?> UpdateTag(Tag tag)
        {
            _logger.LogDebug("UpdateTag DB operation for TagId {TagId}", tag.Id);
            Tag? existing = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Id == tag.Id);
            if (existing == null)
            {
                _logger.LogWarning("UpdateTag: TagId {TagId} not found in DB", tag.Id);
                return null;
            }

            
            existing.Comment = tag.Comment;
            existing.Name = tag.Name;

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Tag {TagId} updated in DB", tag.Id);
            return existing;
        }

        public async Task<bool> DeleteTag(int tagId)
        {
            _logger.LogDebug("DeleteTag DB operation for TagId {TagId}", tagId);
            Tag? tag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Id == tagId);
            if (tag == null)
            {
                _logger.LogWarning("DeleteTag: TagId {TagId} not found in DB", tagId);
                return false;
            }

            _dbContext.Tags.Remove(tag);
            int result = await _dbContext.SaveChangesAsync();
            if (result > 0)
                _logger.LogInformation("Tag {TagId} deleted from DB", tagId);
            else
                _logger.LogWarning("DeleteTag: SaveChanges returned 0 rows for TagId {TagId}", tagId);

            return result > 0;
        }

        public async Task<bool> DeleteTag(Expression<Predicate<Tag>> predicate)
        {
            _logger.LogDebug("DeleteTag (predicate) DB operation initiated");
            var funcExpression = ExpressionConverter.ToFuncExpression(predicate);
            Tag? tag = await _dbContext.Tags.FirstOrDefaultAsync(funcExpression);
            if (tag == null)
            {
                _logger.LogWarning("DeleteTag (predicate): no matching tag found in DB");
                return false;
            }

            _dbContext.Tags.Remove(tag);
            int result = await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Tag {TagId} deleted from DB via predicate", tag.Id);
            return result > 0;
        }

        public async Task<IEnumerable<Tag>?> GetTags(Expression<Predicate<Tag>> value)
        {
            _logger.LogDebug("GetTags (predicate) DB query initiated");
            var funcExpression = ExpressionConverter.ToFuncExpression(value);
            return await _dbContext.Tags.Where(funcExpression).ToListAsync();
        }
    }
}
