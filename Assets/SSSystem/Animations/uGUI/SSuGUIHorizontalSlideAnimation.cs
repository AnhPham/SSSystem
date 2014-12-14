using UnityEngine;
using System.Collections;

public class SSuGUIHorizontalSlideAnimation : SSuGUIBaseAnimation
{
    public override void PlayHide()
    {
        base.PlayHide();

        m_State = AnimType.HIDE;
        m_Time = Time.realtimeSinceStartup;
        m_MaxTime = TimeHide();

        m_Start = Vector2.zero;
        m_End = new Vector2(Screen.width, 0);
    }

    public override void PlayShow()
    {
        base.PlayShow();

        m_State = AnimType.SHOW;
        m_Time = Time.realtimeSinceStartup;
        m_MaxTime = TimeShow();

        m_Start = new Vector2(Screen.width, 0);
        m_End = Vector2.zero;
    }

    public override void PlayHideBack()
    {
        base.PlayHideBack();

        m_State = AnimType.HIDE_BACK;
        m_Time = Time.realtimeSinceStartup;
        m_MaxTime = TimeHideBack();

        m_Start = Vector2.zero;
        m_End = new Vector2(-Screen.width/2, 0);
    }

    public override void PlayShowBack()
    {
        base.PlayShowBack();

        m_State = AnimType.SHOW_BACK;
        m_Time = Time.realtimeSinceStartup;
        m_MaxTime = TimeShowBack();

        m_Start = new Vector2(-Screen.width/2, 0);
        m_End = Vector2.zero;
    }
}
