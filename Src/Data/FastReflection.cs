using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Contains members to hold PropertyInfo.
    /// </summary>
    internal static class FastReflectionCaches
    {
        static FastReflectionCaches()
        {
            MethodInvokerCache = new MethodInvokerCache();
            PropertyAccessorCache = new PropertyAccessorCache();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reviewed")]
        public static IFastReflectionCache<MethodInfo, IMethodInvoker> MethodInvokerCache { get; set; }

        public static IFastReflectionCache<PropertyInfo, IPropertyAccessor> PropertyAccessorCache { get; set; }
    }

    internal partial class MethodInvokerCache : FastReflectionCache<MethodInfo, IMethodInvoker>
    {
        protected override IMethodInvoker Create(MethodInfo key)
        {
            return FastReflectionFactories.MethodInvokerFactory.Create(key);
        }
    }
    internal static class FastReflectionFactories
    {
        static FastReflectionFactories()
        {
            MethodInvokerFactory = new MethodInvokerFactory();
        }

        public static IFastReflectionFactory<MethodInfo, IMethodInvoker> MethodInvokerFactory { get; set; }
    }

    internal partial class MethodInvokerFactory : IFastReflectionFactory<MethodInfo, IMethodInvoker>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        public IMethodInvoker Create(MethodInfo key)
        {
            return new MethodInvoker(key);
        }

        IMethodInvoker IFastReflectionFactory<MethodInfo, IMethodInvoker>.Create(MethodInfo key)
        {
            return this.Create(key);
        }
    }
    internal interface IFastReflectionFactory<TKey, TValue>
    {
        TValue Create(TKey key);
    }
    internal interface IFastReflectionCache<TKey, TValue>
    {
        TValue Get(TKey key);
    }
    internal interface IPropertyAccessor
    {
        Func<object, object> GetMethod
        {
            get;
        }

        object GetValue(object instance);

        void SetValue(object instance, object value);
    }
    internal partial class PropertyAccessorCache : FastReflectionCache<PropertyInfo, IPropertyAccessor>
    {
        protected override IPropertyAccessor Create(PropertyInfo key)
        {
            return new PropertyAccessor(key);
        }
    }
    internal partial class PropertyAccessor : IPropertyAccessor
    {
        public Func<object, object> GetMethod
        {
            get
            {
                return m_getter;
            }
        }
        private Func<object, object> m_getter;
        private MethodInvoker m_setMethodInvoker;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reviewed")]
        public PropertyInfo PropertyInfo { get; private set; }

        public PropertyAccessor(PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
            this.InitializeGet(propertyInfo);
            this.InitializeSet(propertyInfo);
        }

        private void InitializeGet(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead) return;

            // Target: (object)(((TInstance)instance).Property)

            // preparing parameter, object type
            var instance = System.Linq.Expressions.Expression.Parameter(typeof(object), "instance");

            // non-instance for static method, or ((TInstance)instance)
            var instanceCast = propertyInfo.GetGetMethod().IsStatic ? null :
                System.Linq.Expressions.Expression.Convert(instance, propertyInfo.DeclaringType);

            // ((TInstance)instance).Property
            var propertyAccess = System.Linq.Expressions.Expression.Property(instanceCast, propertyInfo);

            // (object)(((TInstance)instance).Property)
            var castPropertyValue = System.Linq.Expressions.Expression.Convert(propertyAccess, typeof(object));

            // Lambda expression
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<object, object>>(castPropertyValue, instance);

            this.m_getter = lambda.Compile();
        }

        private void InitializeSet(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanWrite || propertyInfo.GetSetMethod() == null) return;
            this.m_setMethodInvoker = new MethodInvoker(propertyInfo.GetSetMethod());// .GetSetMethod(true));
        }

        public object GetValue(object o)
        {
            if (this.m_getter == null)
            {
                throw new NotSupportedException("Get method is not defined for this property.");
            }

            return this.m_getter(o);
        }

        public void SetValue(object o, object value)
        {
            if (this.m_setMethodInvoker == null)
            {
                throw new NotSupportedException("Set method is not defined for this property.");
            }

            this.m_setMethodInvoker.Invoke(o, new object[] { value });
        }

        #region IPropertyAccessor Members

        object IPropertyAccessor.GetValue(object instance)
        {
            return this.GetValue(instance);
        }

        void IPropertyAccessor.SetValue(object instance, object value)
        {
            this.SetValue(instance, value);
        }

        #endregion
    }
    internal interface IMethodInvoker
    {
        object Invoke(object instance, params object[] parameters);
    }
    internal partial class MethodInvoker : IMethodInvoker
    {
        private Func<object, object[], object> m_invoker;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reviewed")]
        public MethodInfo MethodInfo { get; private set; }

        public MethodInvoker(MethodInfo methodInfo)
        {
            this.MethodInfo = methodInfo;
            this.m_invoker = CreateInvokeDelegate(methodInfo);
        }

        public object Invoke(object instance, params object[] parameters)
        {
            return this.m_invoker(instance, parameters);
        }

        private static Func<object, object[], object> CreateInvokeDelegate(MethodInfo methodInfo)
        {
            // Target: ((TInstance)instance).Method((T0)parameters[0], (T1)parameters[1], ...)

            // parameters to execute
            var instanceParameter = System.Linq.Expressions.Expression.Parameter(typeof(object), "instance");
            var parametersParameter = System.Linq.Expressions.Expression.Parameter(typeof(object[]), "parameters");

            // build parameter list
            var parameterExpressions = new List<System.Linq.Expressions.Expression>();
            var paramInfos = methodInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                // (Ti)parameters[i]
                BinaryExpression valueObj = System.Linq.Expressions.Expression.ArrayIndex(
                    parametersParameter, System.Linq.Expressions.Expression.Constant(i));
                UnaryExpression valueCast = System.Linq.Expressions.Expression.Convert(
                    valueObj, paramInfos[i].ParameterType);

                parameterExpressions.Add(valueCast);
            }

            // non-instance for static method, or ((TInstance)instance)
            var instanceCast = methodInfo.IsStatic ? null :
                System.Linq.Expressions.Expression.Convert(instanceParameter, methodInfo.DeclaringType);

            // static invoke or ((TInstance)instance).Method
            var methodCall = System.Linq.Expressions.Expression.Call(instanceCast, methodInfo, parameterExpressions);

            // ((TInstance)instance).Method((T0)parameters[0], (T1)parameters[1], ...)
            if (methodCall.Type == typeof(void))
            {
                var lambda = System.Linq.Expressions.Expression.Lambda<Action<object, object[]>>(
                        methodCall, instanceParameter, parametersParameter);

                Action<object, object[]> execute = lambda.Compile();
                return (instance, parameters) =>
                {
                    execute(instance, parameters);
                    return null;
                };
            }
            else
            {
                var castMethodCall = System.Linq.Expressions.Expression.Convert(methodCall, typeof(object));
                var lambda = System.Linq.Expressions.Expression.Lambda<Func<object, object[], object>>(
                    castMethodCall, instanceParameter, parametersParameter);

                return lambda.Compile();
            }
        }

        #region IMethodInvoker Members

        object IMethodInvoker.Invoke(object instance, params object[] parameters)
        {
            return this.Invoke(instance, parameters);
        }

        #endregion
    }
    internal abstract class FastReflectionCache<TKey, TValue> : IFastReflectionCache<TKey, TValue> where TKey :class
    {
        private Dictionary<TKey, TValue> m_cache = new Dictionary<TKey, TValue>();

        public TValue Get(TKey key)
        {
            TValue value = default(TValue);
            if (this.m_cache.TryGetValue(key, out value))
            {
                return value;
            }

            lock (key)
            {
                if (!this.m_cache.TryGetValue(key, out value))
                {
                    value = this.Create(key);
                    this.m_cache[key] = value;
                }
            }

            return value;
        }

        protected abstract TValue Create(TKey key);
    }
}
