using System;
using System.Collections.Generic;
using System.Text;
using NotesKeeper.Core.DTOs.NoteDTOs;

namespace NotesKeeper.Core.DTOs.TagDTOs
{
    public class TagResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid UserId { get; set; }
        public string? Comment { get; set; }        
        public DateTime CreatedAt { get; set; }
    }
}
