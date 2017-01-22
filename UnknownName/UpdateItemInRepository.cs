using System;

namespace Tests.Subjects
{
    internal class UpdateItemInRepository : IUpdateItemInRepository
    {
        public string UpdateItem(Guid item)
        {
            return "Updateitem: {0}" + item;
        }
    }
}