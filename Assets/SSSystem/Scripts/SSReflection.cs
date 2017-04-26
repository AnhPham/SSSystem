// This code is part of the SS-System library (https://github.com/AnhPham/SSSystem) maintained by Anh Pham (anhpt.csit@gmail.com).
// It is released for free under the MIT open source license (https://github.com/AnhPham/SSSystem/blob/master/LICENSE)

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
