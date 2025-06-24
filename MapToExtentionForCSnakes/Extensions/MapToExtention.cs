using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using ClassImport.Attributes;
using CSnakes.Runtime.Python;

namespace ClassImport.Extensions;

public static class MapToExtention
{
    private static readonly MethodInfo _baseMethod = typeof(MapToExtention).GetMethod(nameof(_getValueFromPython), BindingFlags.Static | BindingFlags.NonPublic)!;
    public static TSource MapTo<TSource>(this PyObject pyObj) where TSource: class
    {
        var obj = Activator.CreateInstance<TSource>();
        foreach (var prop in typeof(TSource).GetProperties())
        {
            var attrInfo = prop.GetCustomAttribute<PythonPropertyName>();
            if (attrInfo is null) continue;
            var type = prop.PropertyType;
            var method = _getOrCompile(type);
            prop.SetValue(obj,  method(pyObj, attrInfo.Name));
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
