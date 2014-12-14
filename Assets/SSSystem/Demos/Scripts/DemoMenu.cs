using UnityEngine;
using System.Collections;
using System.Reflection;

using UnityEngine.UI;

public class DemoMenu : SSController
{
    [SerializeField]
    GameObject m_Tap1Image;

    [SerializeField]
    GameObject m_Tap2Image;

    public override void OnEnable()
    {
        base.OnEnable();

        if (SSSceneManager.Instance != null)
        {
            SSSceneManager.Instance.onScreenStartChange += OnScreenStartChange;
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();

        if (SSSceneManager.Instance != null)
        {
            SSSceneManager.Instance.onScreenStartChange -= OnScreenStartChange;
        }
    }

    public void OnTab1ButtonTap()
    {
        SSSceneManager.Instance.Screen("DemoScreen1");
    }

    public void OnTab2ButtonTap()
    {
        SSSceneManager.Instance.Screen("DemoScreen3");
    }

    void OnScreenStartChange(string sceneName)
    {
        switch (sceneName)
        {
            case "DemoScreen1":
                SetColor(m_Tap1Image, Color.gray);
                SetColor(m_Tap2Image, Color.white);
                break;
            case "DemoScreen3":
                SetColor(m_Tap2Image, Color.gray);
                SetColor(m_Tap1Image, Color.white);
                break;
        }
    }

    void SetColor(GameObject go, Color color)
    {
        // uGUI
        Component image = go.GetComponent("Image");
        if (image != null)
        {
            SetColorReflection(image, color);
        }

        // nGUI
        Component uiWidget = go.GetComponent("UIWidget");
        if (uiWidget != null)
        {
            SetColorReflection(uiWidget, color);
        }
    }

    void SetColorReflection(Component comp, Color color)
    {
        SSReflection.SetPropValue(comp, "color", color);
    }
}
