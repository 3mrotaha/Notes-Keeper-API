using System;
using System.Collections.Generic;
using System.Text;

namespace NotesKeeper.Core.Domain.Interfaces
{
    public interface ISoftDeletable
    {
        public bool IsDeleted { get; set; }
    }
}
