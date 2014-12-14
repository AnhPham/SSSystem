using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEngine.UI;

public class DemoPopupData
{
    public string Title;
    public DemoPopupType Type;

    public DemoPopupData(string title, DemoPopupType type)
    {
        Title = title;
        Type = type;
    }
}

public enum DemoPopupType
{
    OK,
    YES_NO
}

public class DemoPopup : SSController
{
    public delegate void OnYesButtonTapDelegate();
    public delegate void OnNoButtonTapDelegate();
    public delegate void OnOkButtonTapDelegate();

    public OnYesButtonTapDelegate onYesButtonTap;
    public OnNoButtonTapDelegate onNoButtonTap;
    public OnOkButtonTapDelegate onOkButtonTap;

    [SerializeField]
    GameObject m_Text;

    [SerializeField]
    GameObject m_ButtonOk;

    [SerializeField]
    GameObject m_ButtonYes;

    [SerializeField]
    GameObject m_ButtonNo;

    DemoPopupType m_Type = DemoPopupType.OK;

    public override void OnSet(object data)
    {
        DemoPopupData popupData = (DemoPopupData)data;

        SetText(m_Text, popupData.Title);
        m_Type = popupData.Type;

        m_ButtonYes.SetActive(m_Type == DemoPopupType.YES_NO);
        m_ButtonNo.SetActive(m_Type == DemoPopupType.YES_NO);
        m_ButtonOk.SetActive(m_Type == DemoPopupType.OK);
    }

    public void OnYesButtonTap()
    {
        if (onYesButtonTap != null)
        {
            onYesButtonTap();
        }
    }

    public void OnNoButtonTap()
    {
        if (onNoButtonTap != null)
        {
            onNoButtonTap();
        }

        SSSceneManager.Instance.Close();
    }

    public void OnOkButtonTap()
    {
        if (onOkButtonTap != null)
        {
            onOkButtonTap();
        }

        SSSceneManager.Instance.Close();
    }

    void SetText(GameObject go, string text)
    {
        // uGUI
        Component uGuiText = go.GetComponent("Text");
        if (uGuiText != null)
        {
            SetColorReflection(uGuiText, text);
        }

        // nGUI
        Component nGuiLabel = go.GetComponent("UILabel");
        if (nGuiLabel != null)
        {
            SetColorReflection(nGuiLabel, text);
        }
    }

    void SetColorReflection(Component comp, string text)
    {
        SSReflection.SetPropValue(comp, "text", text);
    }
}
