using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SSFullscreenUIPanel : MonoBehaviour
{
    const int ConstrainButDontClip = 4;

    Component m_UIPanel;

    void Awake()
    {
        ResizePanel();
    }

    #if UNITY_EDITOR
    void Update()
    {
        ResizePanel();
    }
    #endif

    void ResizePanel()
    {
        if (m_UIPanel == null)
        {
            m_UIPanel = GetComponent("UIPanel");
        }

        if (m_UIPanel != null)
        {
            // UIRoot
            object uiRoot = SSReflection.GetPropValue(m_UIPanel, "root");

            // Get Size
            float h = (float)(int)SSReflection.GetPropValue(uiRoot, "activeHeight");
            float w = h * Screen.width / Screen.height;

            // Set clipping to ConstrainButDontClip
            SSReflection.SetPropValue(m_UIPanel, "clipping", ConstrainButDontClip);

            // Set clipping region
            SSReflection.SetPropValue(m_UIPanel, "baseClipRegion", new Vector4(0, 0, w, h));
        }
    }
}
