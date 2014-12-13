using UnityEngine;
using System.Collections;

public class SSuGUIScaleAnimation : SSuGUIBaseAnimation
{
    Vector3 m_BeforeEnd;
    protected Vector3 m_StartV3;
    protected Vector3 m_EndV3;

    float m_BeforeEndTime;

    public override void Reset(AnimType animType)
    {
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
        m_Time = 0;
        m_MaxTime = TimeHide();

        m_StartV3 = Vector3.one;
        m_EndV3 = Vector3.zero;

        m_BeforeEnd = new Vector3(1.15f, 1.15f, 1f);
        m_BeforeEndTime = m_MaxTime * 0.3f;
    }

    public override void PlayShow()
    {
        m_State = AnimType.SHOW;
        m_Time = 0;
        m_MaxTime = TimeShow();

        m_StartV3 = Vector3.zero;
        m_EndV3 = Vector3.one;

        m_BeforeEnd = new Vector3(1.15f, 1.15f, 1f);
        m_BeforeEndTime = m_MaxTime * 0.7f;
    }

    protected override void Update()
    {
        if (m_State != AnimType.NO_ANIM)
        {
            if (m_Time == 0)
            {
                m_RectTrans.anchoredPosition = Vector3.zero;
            }

            m_Time += Time.deltaTime;

            if (m_Time <= m_BeforeEndTime)
            {
                m_RectTrans.localScale = Vector3.Lerp(m_StartV3, m_BeforeEnd, m_Time / m_BeforeEndTime);
            }
            else
            {
                m_RectTrans.localScale = Vector3.Lerp(m_BeforeEnd, m_EndV3, (m_Time - m_BeforeEndTime) / (m_MaxTime - m_BeforeEndTime));
            }

            if (m_Time >= m_MaxTime)
            {
                m_State = AnimType.NO_ANIM;
            }
        }
    }
}
