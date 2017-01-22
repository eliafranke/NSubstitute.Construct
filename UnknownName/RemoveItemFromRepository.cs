using System;

namespace Tests.Subjects
{
    internal class RemoveItemFromRepository : IRemoveItemFromRepository
    {
        public string RemoveItem(Guid item)
        {
            return "Remove item: {0}" + item;
        }
    }
}