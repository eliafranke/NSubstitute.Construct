using System.Collections.Generic;
using System.Linq;

namespace NSubstitute
{
    public static class Construct
    {
        public static TType For<TType>(params object[] parametersToUse)
            where TType : class
        {
            return For<TType, TType>(parametersToUse);
        }

        public static TInterface For<TInterface, TType>(params object[] parametersToUse)
            where TInterface : class
            where TType : class
        {
            var parameterTypesToUse = parametersToUse.SelectMany(p => p.GetType().GetInterfaces().Select(i => new
            {
                Interface = i.Name,
                Parameter = p
            })).ToArray();

            var constructor = typeof(TType).GetConstructors().First();
            var parameterInfos = constructor.GetParameters();

            var parameters = new List<object>();
            foreach (var parameterInfo in parameterInfos)
            {
                var given = parameterTypesToUse.FirstOrDefault(ttu => ttu.Interface == parameterInfo.ParameterType.Name);
                parameters.Add(given == null
                    ? Substitute.For(new[] { parameterInfo.ParameterType }, null)
                    : given.Parameter);
            }

            return constructor.Invoke(parameters.ToArray()) as TInterface;
        }
    }
}