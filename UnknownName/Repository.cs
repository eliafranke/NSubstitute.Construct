using System;

namespace Tests.Subjects
{
    internal sealed class Repository :
        IAddItemToRepository,
        IUpdateItemInRepository,
        IRemoveItemFromRepository,
        IDoItemWithRepository
    {
        private readonly IAddItemToRepository _addItemToRepository;
        private readonly IUpdateItemInRepository _updateItemInRepository;
        private readonly IRemoveItemFromRepository _removeItemFromRepository;
        private readonly IDoItemWithRepository _doItemWithRepository;

        public Repository(
            IAddItemToRepository addItemToRepository,
            IUpdateItemInRepository updateItemInRepository,
            IRemoveItemFromRepository removeItemFromRepository,
            IDoItemWithRepository doItemWithRepository)
        {
            _addItemToRepository = addItemToRepository;
            _updateItemInRepository = updateItemInRepository;
            _removeItemFromRepository = removeItemFromRepository;
            _doItemWithRepository = doItemWithRepository;
        }

        string IAddItemToRepository.AddItem(Guid item)
            => _addItemToRepository.AddItem(item);

        string IUpdateItemInRepository.UpdateItem(Guid item)
            => _updateItemInRepository.UpdateItem(item);

        string IRemoveItemFromRepository.RemoveItem(Guid item)
            => _removeItemFromRepository.RemoveItem(item);

        public string DoItem(Guid item)
            => _doItemWithRepository.DoItem(item);
    }
}