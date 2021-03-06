using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute
{
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
                            : Substitute.For(new[] {parameterInfo.ParameterType}, null))
                .ToArray();

            return constructor.Invoke(parameters) as TInterface;
        }

        private static ILookup<string, object> CreateConstructorInterfaceArgumentsLookup(IEnumerable<object> constructorArguments)
        {
            return constructorArguments
                .SelectMany(o
                    => o
                        .GetType()
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
}
