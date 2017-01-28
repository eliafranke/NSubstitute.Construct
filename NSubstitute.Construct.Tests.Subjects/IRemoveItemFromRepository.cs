using System;

namespace Tests.Subjects
{
    public interface IRemoveItemFromRepository
    {
        string RemoveItem(Guid item);
    }
}