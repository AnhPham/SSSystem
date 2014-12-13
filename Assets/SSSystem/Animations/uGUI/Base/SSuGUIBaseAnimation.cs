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

        m_RectTrans = GetComponent<RectTransform>();

        // We should bring this scene to somewhere far when it awake.
        // Then the animation will automatically bring it back at next frame.
        // This trick remove flicker at the first frame.

        m_RectTrans.anchoredPosition = new Vector2(99999, 0);
    }

    protected override void Start()
    {
        base.Start();

        if (SSSceneManager.Instance == null)
        {
            PlayShow();
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
            m_Time += Time.deltaTime;
            m_RectTrans.anchoredPosition = Vector2.Lerp(m_Start, m_End, m_Time / m_MaxTime);

            if (m_Time >= m_MaxTime)
            {
                m_State = AnimType.NO_ANIM;
            }
        }
    }
}
