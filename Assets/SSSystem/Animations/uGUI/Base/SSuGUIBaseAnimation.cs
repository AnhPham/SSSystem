using UnityEngine;
using System.Collections;

public class SSuGUIBaseAnimation : SSMotion
{
    public const float COMMON_TIME = 0.25f;

    protected Vector2 m_Start;
    protected Vector2 m_End;
    protected RectTransform m_RectTrans;

    protected float m_Time;
    protected float m_MaxTime;

    protected override void Awake()
    {
        base.Awake();

        if (m_RectTrans == null)
        {
            m_RectTrans = GetComponent<RectTransform>();
        }
    }

    protected override void Start()
    {
        base.Start();

        if (SSSceneManager.Instance == null)
        {
            PlayShow();
        }
    }

    public override void Reset(AnimType animType)
    {
        if (m_RectTrans == null)
        {
            m_RectTrans = GetComponent<RectTransform>();
        }
    }

    public override float TimeHide()
    {
        return COMMON_TIME;
    }

    public override float TimeShow()
    {
        return COMMON_TIME;
    }

    public override float TimeHideBack()
    {
        return COMMON_TIME;
    }

    public override float TimeShowBack()
    {
        return COMMON_TIME;
    }

    protected override void Update()
    {
        base.Update();

        if (m_State != AnimType.NO_ANIM)
        {
            float time = Time.realtimeSinceStartup - m_Time;
            m_RectTrans.anchoredPosition = Vector2.Lerp(m_Start, m_End, time / m_MaxTime);

            if (time >= m_MaxTime)
            {
                m_State = AnimType.NO_ANIM;
            }
        }
    }
}
