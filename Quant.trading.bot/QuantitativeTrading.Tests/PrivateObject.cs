using System;
using System.Reflection;

namespace QuantitativeTrading.Tests
{
    public class PrivateObject
    {
        private readonly object o;

        public PrivateObject(object o)
        {
            this.o = o;
        }

        public object Invoke(string methodName, params object[] args)
        {
            var methodInfo = o.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo == null)
            {
                throw new Exception($"Method'{methodName}' not found is class '{o.GetType()}'");
            }
            return methodInfo.Invoke(o, args);
        }
    }
}
