using NotesKeeper.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NotesKeeper.Core.DTOs.TagDTOs
{
    public class TagAddRequest
    {
        [Required(ErrorMessage = "{0} Can't be null or empty")]
        [MaxLength(50, ErrorMessage = "Tag {0} Can't exceed the max length of {1} characters")]
        [MinLength(2, ErrorMessage = "Tag {0} Can't be less than the min length of {1} characters")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "{0} Can't be null or empty")]
        public Guid UserId { get; set; }

        [MaxLength(500, ErrorMessage = "Tag {0} Can't exceed the max length of {1} characters")]
        public string? Comment { get; set; }


        public Tag ToTag()
        {
            return new Tag
            {
                Name = this.Name,
                UserId = this.UserId,
                Comment = this.Comment
            };
        }
    }
}
