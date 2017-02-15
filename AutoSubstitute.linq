<Query Kind="Program">
  <NuGetReference>Microsoft.ServiceFabric.Actors</NuGetReference>
  <NuGetReference>NSubstitute</NuGetReference>
  <Namespace>NSubstitute</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.ServiceFabric.Actors.Runtime</Namespace>
  <Namespace>Microsoft.ServiceFabric.Actors</Namespace>
</Query>

void Main()
{
	var tmClass = new TypicalMicrosoftClass();
	var actorId = ActorId.CreateRandom();
	IAddItemToRepository addItemToRepository = new AddItemToRepository();
	IUpdateItemInRepository updateItemInRepository = new UpdateItemInRepository();
	IFindItemInRepository findItemInRepository = new FindItemInRepository();
	
	var constructedObject = Construct.For<IDoItemWithRepository, Repository>(
		actorId,
		tmClass,
		addItemToRepository,
		PerUserRepositoryFactoryUtilities.Create(updateItemInRepository),
		PerUserRepositoryFactoryUtilities.Create(findItemInRepository));
	
	constructedObject.DoItem(Guid.NewGuid()).Dump();
	//constructedObject.GetConstructionTime().Dump();
}
}

public static class Construct
{
	internal static string ExceptionMessageOneCustontructor =
		"NSubstitute.Construct only supports one Constructor per Type.";

	public static TType For<TType>(params object[] constructorArguments)
		where TType : class
	{
		return For<TType, TType>(constructorArguments);
	}

	public static TInterface For<TInterface, TType>(params object[] constructorArguments)
		where TInterface : class
		where TType : class, TInterface
	{
		var constructors = typeof(TType).GetConstructors();
		if (constructors.Length > 1)
		{
			throw new ArgumentNullException(ExceptionMessageOneCustontructor);
		}
		var constructor = constructors.Single();

		var constructorInterfaceArgumentsLookup = CreateConstructorInterfaceArgumentsLookup(constructorArguments);
		var constructorTypeArgumentsLookup = CreateConstructorTypeArgumentsLookup(constructorArguments);
		var parameters = constructor.GetParameters().Select(
				parameterInfo =>
					constructorInterfaceArgumentsLookup.Contains(parameterInfo.ParameterType.FullName)
					? constructorInterfaceArgumentsLookup[parameterInfo.ParameterType.FullName].First()
					: constructorTypeArgumentsLookup.Contains(parameterInfo.ParameterType.FullName)
						? constructorTypeArgumentsLookup[parameterInfo.ParameterType.FullName].First()
						: Substitute.For(new[] { parameterInfo.ParameterType }, null))
			.ToArray();

		return constructor.Invoke(parameters) as TInterface;
	}

	private static ILookup<string, object> CreateConstructorInterfaceArgumentsLookup(IEnumerable<object> constructorArguments)
	{
		return constructorArguments
			.SelectMany(o
				=> o
					.GetType().Dump()
					.GetInterfaces()
					.Select(i
						=> new
						{
							Interface = i.FullName,
							Parameter = o
						}))
			.Distinct()
			.ToLookup(k => k.Interface, v => v.Parameter);
	}

	private static ILookup<string, object> CreateConstructorTypeArgumentsLookup(IEnumerable<object> constructorArguments)
	{
		return constructorArguments
			.Where(p => p.GetType().IsSealed || !p.GetType().GetInterfaces().Any())
			.Select(p
				=> new
				{
					TypeName = p.GetType().FullName,
					Parameter = p
				})
			.Distinct()
			.ToLookup(k => k.TypeName, v => v.Parameter);
	}
}

public sealed class Repository :
	IAddItemToRepository,
	IRemoveItemFromRepository,
	IDoItemWithRepository
{
	private readonly ActorId _actorId;
	private readonly TypicalMicrosoftClass _typicalMicrosoftClass;
	private readonly IAddItemToRepository _addItemToRepository;
	private readonly IPerUserRepositoryFactory<IUpdateItemInRepository> _updateItemInRepositoryFactory;
	private readonly IPerUserRepositoryFactory<IFindItemInRepository> _findItemInRepositoryFactory;
	private readonly IRemoveItemFromRepository _removeItemFromRepository;
	private readonly IDoItemWithRepository _doItemWithRepository;

	public Repository(
		ActorId actorId,
		TypicalMicrosoftClass typicalMicrosoftClass,
		IAddItemToRepository addItemToRepository,
	 	IPerUserRepositoryFactory<IUpdateItemInRepository> updateItemInRepositoryFactory,
		IPerUserRepositoryFactory<IFindItemInRepository> findItemInRepositoryFactory,
		IRemoveItemFromRepository removeItemFromRepository,
		IDoItemWithRepository doItemWithRepository)
	{
		_actorId = actorId;
		_typicalMicrosoftClass = typicalMicrosoftClass;
		_addItemToRepository = addItemToRepository;
		_updateItemInRepositoryFactory = updateItemInRepositoryFactory;
		_findItemInRepositoryFactory = findItemInRepositoryFactory;
		_removeItemFromRepository = removeItemFromRepository;
		_doItemWithRepository = doItemWithRepository;
	}

	public string GetConstructionTime()
	{
		return _typicalMicrosoftClass.GetConstructionTime() + _actorId.ToString();
	}

	string IAddItemToRepository.AddItem(Guid item)
		=> _addItemToRepository.AddItem(item);

	string IRemoveItemFromRepository.RemoveItem(Guid item)
		=> _removeItemFromRepository.RemoveItem(item);

	string IDoItemWithRepository.DoItem(Guid item)
	{
		return
			((IAddItemToRepository)this).AddItem(item) +
			(_updateItemInRepositoryFactory.Create()).UpdateItem(item) +
			(_findItemInRepositoryFactory.Create()).FindItem(item) +
			((IRemoveItemFromRepository)this).RemoveItem(item);
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

public interface IUpdateItemInRepository : IPerUserRepository
{
	string UpdateItem(Guid item);
}

public interface IFindItemInRepository : IPerUserRepository
{
	string FindItem(Guid item);
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

public class FindItemInRepository : IFindItemInRepository
{
	public string FindItem(Guid item)
	{
		return "Find item: " + item;
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

public interface IPerUserRepositoryFactory<out TRepository> where TRepository : class, IPerUserRepository
{
	TRepository Create();
}

public interface IPerUserRepository { }

public class PerUserRepositoryFactory<TRepository> : IPerUserRepositoryFactory<TRepository>
		where TRepository : class, IPerUserRepository, new()
{
	TRepository IPerUserRepositoryFactory<TRepository>.Create()
	{
		return new TRepository();
	}
}

public static class PerUserRepositoryFactoryUtilities
{
	public static IPerUserRepositoryFactory<TRepository> Create<TRepository>()
		where TRepository : class, IPerUserRepository
	{
		var factory = Substitute.For<IPerUserRepositoryFactory<TRepository>>();

		factory
			.Create()
			.Returns(Substitute.For<TRepository>());

		return factory;
	}

	public static IPerUserRepositoryFactory<TRepository> Create<TRepository>(TRepository repository)
		where TRepository : class, IPerUserRepository
	{
		var factory = Substitute.For<IPerUserRepositoryFactory<TRepository>>();

		factory
			.Create()
			.Returns(repository);

		return factory;
	}
}

public class asdf
{

//private I Construct<I, T>(params object[] parametersToUse) 
//	where I : class 
//	where T : class
//{
//	parametersToUse.Select(p => p.GetType()).Dump();
//	
//	var parameterInterfacesToUse = parametersToUse.SelectMany(p => p.GetType().GetInterfaces().Select(i => new
//	{
//		Interface = i.Name,
//		Parameter = p
//	})).ToArray();
//
//	var parameterTypesToUse = parametersToUse.Where(p => !p.GetType().GetInterfaces().Any() || p.GetType().IsSealed).Select(p => new
//	{
//		TypeName = p.GetType().Name,
//		Parameter = p
//	});
//	
//	var constructor = typeof(T).GetConstructors().First();
//	var parameterInfos = constructor.GetParameters();
//	
//	var parameters = new List<object>();
//	foreach (var parameterInfo in parameterInfos)
//	{
//		var givenInterface = parameterInterfacesToUse.FirstOrDefault(ttu => ttu.Interface == parameterInfo.ParameterType.Name);
//		var givenType = parameterTypesToUse.FirstOrDefault(ttu => ttu.TypeName == parameterInfo.ParameterType.Name);
//		if (givenType != null)
//		{
//			parameters.Add(givenType.Parameter);
//		}
//		else if (givenInterface != null)
//		{
//			parameters.Add(givenInterface.Parameter);
//		}
//		else
//		{
//			parameters.Add(Substitute.For(new[] { parameterInfo.ParameterType }, null));
//		}
//	}
//	
//	return constructor.Invoke(parameters.ToArray()) as I;
//}
