using System;

namespace Tests.Subjects
{
    public interface IAddItemToRepository
    {
        string AddItem(Guid item);
    }
}