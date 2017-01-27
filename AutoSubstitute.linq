<Query Kind="Program">
  <NuGetReference>NSubstitute</NuGetReference>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>NSubstitute</Namespace>
</Query>

void Main()
{
	var tmClass = new TypicalMicrosoftClass();
	IAddItemToRepository addItemToRepository = new AddItemToRepository();
	IUpdateItemInRepository updateItemInRepository = new UpdateItemInRepository();
	var with = Construct<Repository, Repository>(tmClass, addItemToRepository, updateItemInRepository);
	//with.DoItem(Guid.NewGuid()).Dump();
	with.GetConstructionTime().Dump();
}

private I Construct<I, T>(params object[] parametersToUse) 
	where I : class 
	where T : class
{
	var parameterInterfacesToUse = parametersToUse.SelectMany(p => p.GetType().GetInterfaces().Select(i => new
	{
		Interface = i.Name,
		Parameter = p
	})).ToArray();

	var parameterTypesToUse = parametersToUse.Where(p => !p.GetType().GetInterfaces().Any()).Select(p => new
	{
		TypeName = p.GetType().Name,
		Parameter = p
	});
	
	var constructor = typeof(T).GetConstructors().First();
	var parameterInfos = constructor.GetParameters();
	
	var parameters = new List<object>();
	foreach (var parameterInfo in parameterInfos)
	{
		var givenInterface = parameterInterfacesToUse.FirstOrDefault(ttu => ttu.Interface == parameterInfo.ParameterType.Name);
		var givenType = parameterTypesToUse.FirstOrDefault(ttu => ttu.TypeName == parameterInfo.ParameterType.Name);
		if (givenType != null)
		{
			parameters.Add(givenType.Parameter);
		}
		else if (givenInterface != null)
		{
			parameters.Add(givenInterface.Parameter);
		}
		else
		{
			parameters.Add(Substitute.For(new[] { parameterInfo.ParameterType }, null));
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
	private readonly TypicalMicrosoftClass _typicalMicrosoftClass;
	private readonly IAddItemToRepository _addItemToRepository;
	private readonly IUpdateItemInRepository _updateItemInRepository;
	private readonly IRemoveItemFromRepository _removeItemFromRepository;
	private readonly IDoItemWithRepository _doItemWithRepository;

	public Repository(
		TypicalMicrosoftClass typicalMicrosoftClass,
		IAddItemToRepository addItemToRepository,
		IUpdateItemInRepository updateItemInRepository,
		IRemoveItemFromRepository removeItemFromRepository,
		IDoItemWithRepository doItemWithRepository)
	{
		_typicalMicrosoftClass = typicalMicrosoftClass;
		_addItemToRepository = addItemToRepository;
		_updateItemInRepository = updateItemInRepository;
		_removeItemFromRepository = removeItemFromRepository;
		_doItemWithRepository = doItemWithRepository;
	}

	public string GetConstructionTime()
	{
		return _typicalMicrosoftClass.GetConstructionTime();
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

public sealed class TypicalMicrosoftClass
{
	private readonly string _constructionTime;
	
	public TypicalMicrosoftClass()
	{
		_constructionTime = DateTime.Now.ToLongTimeString();
	}
	
	public string GetConstructionTime()
	{
		return _constructionTime;
	}
}

public class asdf {