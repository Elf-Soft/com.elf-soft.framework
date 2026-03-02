using System.Collections.Generic;

namespace ElfSoft.Framework
{
    public interface IGameData<T>
    {
        public IReadOnlyList<T> Entries { get; }
    }
}
