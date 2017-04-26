// This code is part of the SS-System library (https://github.com/AnhPham/SSSystem) maintained by Anh Pham (anhpt.csit@gmail.com).
// It is released for free under the MIT open source license (https://github.com/AnhPham/SSSystem/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

public class SSuGUIVerticalSlideAnimation : SSuGUIBaseAnimation
{
    [SerializeField]
    SSDirection m_Direction;

    enum SSDirection
    {
        FROM_BOTTOM,
        FROM_TOP
    }

    public override void PlayHide()
    {
        base.PlayHide();

        m_State = AnimType.HIDE;
        m_Time = Time.realtimeSinceStartup;
        m_MaxTime = TimeHide();

        int sign = (m_Direction == SSDirection.FROM_TOP) ? 1 : -1;

        m_Start = Vector2.zero;
        m_End = new Vector2(0, Screen.height * sign);
    }

    public override void PlayShow()
    {
        base.PlayShow();

        m_State = AnimType.SHOW;
        m_Time = Time.realtimeSinceStartup;
        m_MaxTime = TimeShow();

        int sign = (m_Direction == SSDirection.FROM_TOP) ? 1 : -1;

        m_Start = new Vector2(0, Screen.height * sign);
        m_End = Vector2.zero;
    }
}
