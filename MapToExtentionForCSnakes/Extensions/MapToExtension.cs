using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using ClassImport.Attributes;
using CSnakes.Runtime.Python;

namespace ClassImport.Extensions;

public static class MapToExtension
{
    private static readonly MethodInfo _baseMethod = typeof(MapToExtension).GetMethod(nameof(_getValueFromPython), BindingFlags.Static | BindingFlags.NonPublic)!;
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache = new();
    public static TSource MapTo<TSource>(this PyObject pyObj)
    {
        var obj = Activator.CreateInstance<TSource>();
        var properties  = _propertyCache.GetOrAdd(typeof(TSource), t => t.GetProperties()
                                                                                            .Where(p => p.GetCustomAttribute<PythonPropertyName>() is not null)
                                                                                            .ToArray());
        foreach (var prop in properties)
        {
            var attrInfo = prop.GetCustomAttribute<PythonPropertyName>()!;
            var type = prop.PropertyType;
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
        return obj;
    }
    private static TSource _getValueFromPython<TSource>(PyObject obj, string propName)
    {
        return obj.GetAttr(propName).As<TSource>();
    }
    
    private static readonly ConcurrentDictionary<Type, Func<PyObject, string, object>> _methodCache = new();

    private static Func<PyObject, string, object> _getOrCompile(Type type)
    {
        return _methodCache.GetOrAdd(type, t =>
        {
            var method = _baseMethod.MakeGenericMethod(t);
            var objParam = Expression.Parameter(typeof(PyObject), "obj");
            var nameParam = Expression.Parameter(typeof(string), "name");
            var call = Expression.Call(method, objParam, nameParam);
            var cast = Expression.Convert(call, typeof(object));
            return Expression.Lambda<Func<PyObject, string, object>>(cast, objParam, nameParam).Compile();
        });
    }
}
