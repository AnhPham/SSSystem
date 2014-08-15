using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SSTools
{
    public static bool IsNotInExcept(string sn, params string[] exceptList)
    {
        bool isOk = true;
        for (int i = 0; i < exceptList.Length; i++)
        {
            if (string.Compare(exceptList[i], sn) == 0)
            {
                isOk = false;
                break;
            }
        }

        return isOk;
    }

    public static string GetStackBottom(Stack<string> stack)
    {
        if (stack == null)
            return null;

        string r = null;
        foreach (string s in stack)
        {
            r = s;
        }
        return r;
    }
}
