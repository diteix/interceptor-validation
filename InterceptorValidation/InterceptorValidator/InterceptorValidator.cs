using Interceptor.Interfaces;
using System.Collections.Generic;
using System.Reflection;

namespace Interceptor
{
    public class InterceptorValidator
    {
        private static IList<string> _errors = new List<string>();

        public static IEnumerable<string> Errors { get { foreach (var item in _errors) { yield return item; } } }

        public static void Validate(params object[] args)
        {
            foreach (var arg in args)
            {
                if (!(arg is IValidate))
                {
                    continue;
                }

                var e = (arg as IValidate).Validate();

                if (string.IsNullOrEmpty(e))
                {
                    return;
                }

                _errors.Add(e);
            }
        }

        public static void Create(string assemblyPath)
        {
            //string codeBase = Assembly.LoadFile(assemblyPath).CodeBase;
            ////string codeBase = Assembly.GetCallingAssembly().CodeBase;
            //UriBuilder uri = new UriBuilder(codeBase);
            //string path = Uri.UnescapeDataString(uri.Path);

            Injector.InjectAttribute(assemblyPath, typeof(InterceptorValidator).GetMethod(nameof(InterceptorValidator.Validate), BindingFlags.Public | BindingFlags.Static));
        }
    }
}
