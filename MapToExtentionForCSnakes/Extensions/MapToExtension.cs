using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using ClassImport.Attributes;
using CSnakes.Runtime.Python;

namespace ClassImport.Extensions;

public static class MapToExtension
{
    /*Caching*/
    private static readonly Type[] _types = [typeof(int), typeof(string), typeof(long), typeof(float), typeof(double), typeof(byte[]), typeof(bool), typeof(IReadOnlyList<>), typeof(IReadOnlyDictionary<,>), typeof(ValueTuple<>), typeof(Nullable<>), typeof(IGeneratorIterator<,,>), typeof(IPyBuffer), typeof(Task<>), typeof(void)];
    private static readonly ConcurrentDictionary<Type, Func<PyObject, string, object>> _methodCache = new();
    private static readonly MethodInfo _getValueGenericMethod = typeof(MapToExtension).GetMethod(nameof(_getValueFromPython), BindingFlags.Static | BindingFlags.NonPublic)!;
    private static readonly MethodInfo _mapToMethod = typeof(MapToExtension).GetMethod(nameof(MapTo), BindingFlags.Static | BindingFlags.Public)!;
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache = new();
    private static readonly ConcurrentDictionary<Type, Func<PyObject, object>> _mapToMethodCache = new();
    public static TTarget MapTo<TTarget>(this PyObject pyObj)
    {
        var obj = Activator.CreateInstance<TTarget>();
        var properties  = _propertyCache.GetOrAdd(typeof(TTarget), t => t.GetProperties()
                                                                                            .Where(p => p.GetCustomAttribute<PythonPropertyName>() is not null)
                                                                                            .ToArray());
        foreach (var prop in properties)
        {
            var attrInfo = prop.GetCustomAttribute<PythonPropertyName>()!;
            var type = prop.PropertyType;
            if (_types.Contains(type) || (type.IsGenericType && _types.Contains(type.GetGenericTypeDefinition())))
            {
                try
                {
                    var method = _getOrCompile(type);
                    prop.SetValue(obj, method(pyObj, attrInfo.Name));
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to map property {prop.Name} on {type.Name}", ex);
                }
            }
            else
            {
                    var recursiveMapTo = _mapToExpression(type);
                    var childObj = pyObj.GetAttr(attrInfo.Name);
                    prop.SetValue(obj, recursiveMapTo(childObj));
            }
        }
        return obj;
    }
    private static TSource _getValueFromPython<TSource>(PyObject obj, string propName)
    {
        return obj.GetAttr(propName).As<TSource>();
    }
    
    private static Func<PyObject, string, object> _getOrCompile(Type type)
    {
        return _methodCache.GetOrAdd(type, t =>
        {
            var method = _getValueGenericMethod.MakeGenericMethod(t);
            var objParam = Expression.Parameter(typeof(PyObject), "obj");
            var nameParam = Expression.Parameter(typeof(string), "name");
            var call = Expression.Call(method, objParam, nameParam);
            var cast = Expression.Convert(call, typeof(object));
            return Expression.Lambda<Func<PyObject, string, object>>(cast, objParam, nameParam).Compile();
        });
    }

    private static Func<PyObject, object> _mapToExpression(Type type)
    {
        return _mapToMethodCache.GetOrAdd(type, t =>
        {
            var method = _mapToMethod.MakeGenericMethod(t);
            var objParam = Expression.Parameter(typeof(PyObject), "obj");
            var call = Expression.Call(method, objParam);
            var cast = Expression.Convert(call, typeof(object));
            return Expression.Lambda<Func<PyObject, object>>(cast, objParam).Compile();
        });
    }
}
