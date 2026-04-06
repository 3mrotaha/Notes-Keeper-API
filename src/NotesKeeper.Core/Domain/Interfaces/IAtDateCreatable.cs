using System;
using System.Collections.Generic;
using System.Text;

namespace NotesKeeper.Core.Domain.Interfaces
{
    public interface IAtDateCreatable
    {
        public DateTime CreatedAt { get; set; }
    }
}
