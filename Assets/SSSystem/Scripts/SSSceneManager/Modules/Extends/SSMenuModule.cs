using UnityEngine;
using System.Collections;

public class SSMenuModule : SSModule
{
    protected GameObject m_Menu;

    public void OpenMenu(string sceneName, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
    {
        string sn = sceneName;

        int ip = -1;
        float ic = 0.8f;

        OpenCommon (sn, ip, ic, null, true, string.Empty, null, () => { m_Menu = GetSceneDictionary()[sn]; }, onActive, onDeactive);
    }

    public void Show()
    {
        if (m_Menu != null) 
        {
            if (!m_Menu.activeInHierarchy)
            {
                m_Menu.SetActive(true);

                SSController ct = GetController(Name());
                if (ct != null)
                {
                    ct.OnFocus(true);
                    ct.OnShow();
                }
            }
        }
    }

    public void Hide()
    {
        if (m_Menu != null) 
        {
            if (m_Menu.activeInHierarchy)
            {
                SSController ct = GetController(Name());
                if (ct != null)
                {
                    ct.OnFocus(false);
                    ct.OnHide();
                }

                m_Menu.SetActive(false);
            }
        }
    }

    public bool IsExistMenu()
    {
        return (m_Menu != null);
    }

    public string Name()
    {
        return m_Menu.name;
    }
}
