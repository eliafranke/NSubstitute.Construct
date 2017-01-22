using System;

namespace Tests.Subjects
{
    internal class AddItemToRepository : IAddItemToRepository, IDisposable
    {
        public string AddItem(Guid item)
        {
            return "Add item: {0}" + item;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}