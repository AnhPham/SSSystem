using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class DemoMenu : SSController
{
    [SerializeField]
    Image m_Tap1Image;

    [SerializeField]
    Image m_Tap2Image;

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
                m_Tap1Image.color = Color.gray;
                m_Tap2Image.color = Color.white;
                break;
            case "DemoScreen3":
                m_Tap2Image.color = Color.gray;
                m_Tap1Image.color = Color.white;
                break;
        }
    }
}
