using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SSPopUpModule : SSModule
{
    protected Stack<string> m_StackPopUp = new Stack<string>();

    public void OpenPopUp(string sceneName, object data = null, bool imme = false, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
    {
        string sn = sceneName;

        // Count
        int c = m_StackPopUp.Count + 1;

        // Next index
        int ip = c;
        float ic = c;

        // Prev Scene
        SSController ct = null;
        string curBgm = null;

        string preSn = GetHighestScene();
        if (!string.IsNullOrEmpty(preSn))
        {
            ct = GetController(preSn);
        }

        // Shield
        if (ct != null)
        {
            // Cur Bgm
            curBgm = ct.CurrentBgm;

            // Shield
            ShieldOn(m_StackPopUp.Count);
        }
        else
        {
            ShieldOn(0);
        }

        // Lost Focus Pre scene
        OnFocus(preSn, ct, false);

        // Lost Focus Menu
        OnFocusMenu(false);

        // Push stack
        m_StackPopUp.Push(sn);

        OpenCommon (sn, ip, ic, data, imme, curBgm, () => 
        {
        }, null, onActive, onDeactive);
    }

    public void ClosePopUp(bool imme, NoParamCallback callback = null)
    {
        string curSn = m_StackPopUp.Pop ();
        string preSn = GetHighestScene();
            
        CloseCommon (curSn, imme, () => 
        {
            // Shield Off
            ShieldOff();

            // Show & BGM change
            FocusByCloseOther(preSn);

            // Focus Menu
            if (m_StackPopUp.Count == 0)
            {
                OnFocusMenu(true);
            }

            // Callback
            if (callback != null)
            {
                callback();
            }
        });
    }

    public void CloseAllPopUp()
    {
        while (m_StackPopUp.Count >= 1) 
        {
            ClosePopUp(true);
        }
    }

    public bool IsPopUpInStack(string sn)
    {
        return (m_StackPopUp.Contains(sn));
    }

    public bool HasPopUpInStack()
    {
        return (m_StackPopUp.Count > 0);
    }

    public int Count()
    {
        return m_StackPopUp.Count;
    }

    public string Peek()
    {
        return m_StackPopUp.Peek();
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Print all popup"))
        {
            foreach (var item in m_StackPopUp)
            {
                Debug.Log (item);
            }
        }
    }
}
