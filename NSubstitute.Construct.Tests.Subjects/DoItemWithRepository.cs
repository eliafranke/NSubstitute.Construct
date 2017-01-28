using System;

namespace Tests.Subjects
{
    internal class DoItemWithRepository : IDoItemWithRepository
    {
        public string DoItem(Guid item)
        {
            return "Do item: {0}" + item;
        }
    }
}