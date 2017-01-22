<Query Kind="Program">
  <NuGetReference>NSubstitute</NuGetReference>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>NSubstitute</Namespace>
</Query>

void Main()
{
	IAddItemToRepository addItemToRepository = new AddItemToRepository();
	IUpdateItemInRepository updateItemInRepository = new UpdateItemInRepository();
	var with = Construct<IDoItemWithRepository, Repository>(addItemToRepository, updateItemInRepository);
	with.DoItem(Guid.NewGuid()).Dump();
}

private I Construct<I, T>(params object[] parametersToUse) 
	where I : class 
	where T : class
{
	var parameterTypesToUse = parametersToUse.SelectMany(p => p.GetType().GetInterfaces().Select(i => new
	{
		Interface = i.Name,
		Parameter = p
	})).ToArray();
	
	var constructor = typeof(T).GetConstructors().First();
	var parameterInfos = constructor.GetParameters();
	
	var parameters = new List<object>();
	foreach (var parameterInfo in parameterInfos)
	{
		var given = parameterTypesToUse.FirstOrDefault(ttu => ttu.Interface == parameterInfo.ParameterType.Name);
		if (given == null)
		{
			parameters.Add(Substitute.For(new[] { parameterInfo.ParameterType }, null));
		}
		else
		{
			parameters.Add(given.Parameter);
		}
	}
	
	return constructor.Invoke(parameters.ToArray()) as I;
}

}

public sealed class Repository :
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

	string IDoItemWithRepository.DoItem(Guid item)
	{
		return
			((IAddItemToRepository)this).AddItem(item) +
			((IUpdateItemInRepository)this).UpdateItem(item);
	}
}

public interface IDoItemWithRepository
{
	string DoItem(Guid item);
}

public interface IAddItemToRepository
{
	string AddItem(Guid item);
}

public interface IRemoveItemFromRepository
{
	string RemoveItem(Guid item);
}

public interface IUpdateItemInRepository
{
	string UpdateItem(Guid item);
}

public class AddItemToRepository : IAddItemToRepository, IDisposable
{
	public string AddItem(Guid item)
	{
		return "Add item: " + item;
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}

public class RemoveItemFromRepository : IRemoveItemFromRepository
{
	public string RemoveItem(Guid item)
	{
		return "Remove item: " + item;
	}
}

public class UpdateItemInRepository : IUpdateItemInRepository
{
	public string UpdateItem(Guid item)
	{
		return "Update item: " + item;
	}
}

public class asdf {