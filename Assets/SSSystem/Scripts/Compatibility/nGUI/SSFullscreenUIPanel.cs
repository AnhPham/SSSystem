// This code is part of the SS-System library (https://github.com/AnhPham/SSSystem) maintained by Anh Pham (anhpt.csit@gmail.com).
// It is released for free under the MIT open source license (https://github.com/AnhPham/SSSystem/blob/master/LICENSE)

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
