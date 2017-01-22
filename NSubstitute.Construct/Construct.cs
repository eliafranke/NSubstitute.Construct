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
            where TType : class
        {
            var constructors = typeof(TType).GetConstructors();
            if (constructors.Length > 1)
            {
                throw new ArgumentNullException(ExceptionMessageOneCustontructor);
            }
            var constructor = constructors.Single();

            var constructorArgumentsToUse = CreateConstructorArgumentsDictionary(constructorArguments);
            var parameters = constructor.GetParameters().Select(
                    parameterInfo =>
                        constructorArgumentsToUse.ContainsKey(parameterInfo.ParameterType.Name)
                            ? constructorArgumentsToUse[parameterInfo.ParameterType.Name]
                            : Substitute.For(new[] {parameterInfo.ParameterType}, null))
                .ToArray();

            return constructor.Invoke(parameters) as TInterface;
        }

        private static Dictionary<string, object> CreateConstructorArgumentsDictionary(object[] constructorArguments)
        {
            return constructorArguments
                .SelectMany(o
                    => o
                        .GetType()
                        .GetInterfaces()
                        .Select(i
                            => new
                            {
                                Interface = i.Name,
                                Parameter = o
                            }))
                .Distinct()
                .ToDictionary(k => k.Interface, v => v.Parameter);
        }
    }
}