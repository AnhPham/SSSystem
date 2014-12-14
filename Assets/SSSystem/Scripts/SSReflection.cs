using UnityEngine;
using System.Collections;
using System.Reflection;

public class SSReflection
{
    public static object GetPropValue(object src, string propName)
    {
        return src.GetType().GetProperty(propName).GetValue(src, null);
    }

    public static void SetPropValue(object src, string propName, object data)
    {
        src.GetType().InvokeMember (
            propName, 
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
            System.Type.DefaultBinder,
            src,
            new object[] {data}
        );
    }
}
