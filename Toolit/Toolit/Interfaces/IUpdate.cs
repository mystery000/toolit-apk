using System;

namespace Toolit
{
    interface IUpdate<T>
    {
        string Id { get; }
        DateTime Modified { get; }
        bool Update(T that);
        bool Update(string that);
    }
}
