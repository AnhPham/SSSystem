using UnityEngine;
using System.Collections;

public class SSuGUIScaleAnimation : SSuGUIBaseAnimation
{
    Vector3 m_BeforeEnd;
    protected Vector3 m_StartV3;
    protected Vector3 m_EndV3;

    float m_BeforeEndTime;
    bool m_Reposition;

    public override void Reset(AnimType animType)
    {
        base.Reset(animType);

        switch (animType)
        {
            case AnimType.HIDE:
                m_RectTrans.localScale = Vector3.one;
                break;
            case AnimType.SHOW:
                m_RectTrans.localScale = Vector3.zero;
                break;
            default:
                break;
        }
    }

    public override void PlayHide()
    {
        m_State = AnimType.HIDE;
        m_Time = Time.realtimeSinceStartup;
        m_MaxTime = TimeHide();

        m_StartV3 = Vector3.one;
        m_EndV3 = Vector3.zero;

        m_BeforeEnd = new Vector3(1.15f, 1.15f, 1f);
        m_BeforeEndTime = m_MaxTime * 0.3f;
        m_Reposition = false;
    }

    public override void PlayShow()
    {
        m_State = AnimType.SHOW;
        m_Time = Time.realtimeSinceStartup;
        m_MaxTime = TimeShow();

        m_StartV3 = Vector3.zero;
        m_EndV3 = Vector3.one;

        m_BeforeEnd = new Vector3(1.15f, 1.15f, 1f);
        m_BeforeEndTime = m_MaxTime * 0.7f;
        m_Reposition = false;
    }

    protected override void Update()
    {
        if (m_State != AnimType.NO_ANIM)
        {
            if (!m_Reposition)
            {
                m_RectTrans.anchoredPosition = Vector3.zero;
                m_Reposition = true;
            }

            float time = Time.realtimeSinceStartup - m_Time;

            if (time <= m_BeforeEndTime)
            {
                m_RectTrans.localScale = Vector3.Lerp(m_StartV3, m_BeforeEnd, time / m_BeforeEndTime);
            }
            else
            {
                m_RectTrans.localScale = Vector3.Lerp(m_BeforeEnd, m_EndV3, (time - m_BeforeEndTime) / (m_MaxTime - m_BeforeEndTime));
            }

            if (time >= m_MaxTime)
            {
                m_State = AnimType.NO_ANIM;
            }
        }
    }
}
