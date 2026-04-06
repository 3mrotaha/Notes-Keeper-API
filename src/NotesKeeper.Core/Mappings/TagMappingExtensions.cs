using NotesKeeper.Core.Domain.Entities;
using NotesKeeper.Core.DTOs.TagDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotesKeeper.Core.Mappings
{
    public static class TagMappingExtensions
    {
        public static TagResponse ToTagResponse(this Tag tag)
        {
            return new TagResponse
            {
                Id = tag.Id,
                Name = tag.Name,
                UserId = tag.UserId,
                Comment = tag.Comment,
                CreatedAt = tag.CreatedAt,
            };
        }
    }
}
