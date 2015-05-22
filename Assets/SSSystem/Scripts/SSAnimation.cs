/**
 * Created by Anh Pham on 2013/11/13
 * Email: anhpt.csit@gmail.com
 */

using UnityEngine;
using System.Collections;

public class SSAnimation : SSMotion 
{
	[SerializeField]
	private AnimationClip m_ShowClip;

	[SerializeField]
	private AnimationClip m_HideClip;

	[SerializeField]
	private AnimationClip m_ShowBackClip;

	[SerializeField]
	private AnimationClip m_HideBackClip;

	float m_TimeAtLastFrame = 0F;
	float m_TimeAtCurrentFrame = 0F;
	float m_DeltaTime = 0F;
	float m_AccumTime = 0F;

	AnimationState m_CurrState;
	bool m_IsPlaying = false;
	bool m_IsEndAnim = false;

    /// <summary>
    /// Reset() called after activating a game object and before playing an animation.
    /// </summary>
    /// <param name="animType">Animation type.</param>
    public override void Reset(AnimType animType)
    {
        base.Reset(animType);

        switch (animType)
        {
            case AnimType.HIDE:
                PlayOnlyZeroFrame(m_HideClip);
                break;
            case AnimType.HIDE_BACK:
                PlayOnlyZeroFrame(m_HideBackClip);
                break;
            case AnimType.SHOW:
                PlayOnlyZeroFrame(m_ShowClip);
                break;
            case AnimType.SHOW_BACK:
                PlayOnlyZeroFrame(m_ShowBackClip);
                break;
            default:
                break;
        }
    }

	/// <summary>
	/// Time of show - animation by second.
	/// </summary>
	/// <returns>Time show.</returns>
	public override float TimeShow()
	{
		return Time(m_ShowClip);
	}

	/// <summary>
	/// Time of hide - animation by second.
	/// </summary>
	/// <returns>Time hide.</returns>
	public override float TimeHide()
	{
		return Time(m_HideClip);
	}

	/// <summary>
	/// Play the the show - animation.
	/// </summary>
	public override void PlayShow()
	{
		Play(m_ShowClip);
	}

	/// <summary>
	/// Play the the hide - animation.
	/// </summary>
	public override void PlayHide()
	{
		Play(m_HideClip);
	}

	/// <summary>
	/// Time of show back - animation by second.
	/// </summary>
	/// <returns>Time show.</returns>
	public override float TimeShowBack()
	{
		return Time(m_ShowBackClip);
	}

	/// <summary>
	/// Time of hide back - animation by second.
	/// </summary>
	/// <returns>Time hide.</returns>
	public override float TimeHideBack()
	{
		return Time(m_HideBackClip);
	}

	/// <summary>
	/// Play the the show back - animation.
	/// </summary>
	public override void PlayShowBack()
	{
		Play(m_ShowBackClip);
	}

	/// <summary>
	/// Play the the hide back - animation.
	/// </summary>
	public override void PlayHideBack()
	{
		Play(m_HideBackClip);
	}

	/// <summary>
	/// Update function.
	/// </summary>
	protected override void Update()
	{
		m_TimeAtCurrentFrame = UnityEngine.Time.realtimeSinceStartup;
		m_DeltaTime = m_TimeAtCurrentFrame - m_TimeAtLastFrame;
		m_TimeAtLastFrame = m_TimeAtCurrentFrame; 

		if(m_IsPlaying) AnimationUpdate();
	}

	/// <summary>
	/// Start function.
	/// </summary>
	protected override void Start()
	{
		if (SSSceneManager.Instance == null)
		{
			PlayShow();
		}
	}

	private float Time(AnimationClip anim)
	{
		if (anim == null) 
		{
			return 0;
		}

		return anim.length;
	}

    private void PlayPrepare(AnimationClip anim)
    {
        if (anim == null)
        {
            return;
        }

        if (GetComponent<Animation>() == null)
        {
            gameObject.AddComponent<Animation>();
            GetComponent<Animation>().playAutomatically = false;
            GetComponent<Animation>().cullingType = AnimationCullingType.AlwaysAnimate;
        }

        if (GetComponent<Animation>().GetClip(anim.name) == null)
        {
            GetComponent<Animation>().AddClip(anim, anim.name);
        }

        GetComponent<Animation>().clip = anim;
    }

	private void Play(AnimationClip anim)
	{
        PlayPrepare(anim);

        GetComponent<Animation>().Stop();

		PlayAnimation (GetComponent<Animation>(), anim.name);
	}

    private void PlayOnlyZeroFrame(AnimationClip anim)
    {
        PlayPrepare(anim);

        GetComponent<Animation>().Play();
        GetComponent<Animation>()[anim.name].time = 0;
        GetComponent<Animation>().Sample();
    }

	private void AnimationUpdate()
	{
		if (m_IsEndAnim == true) 
		{
			m_CurrState.enabled = false;
			m_IsPlaying = false;
			return;
		}

		m_AccumTime += m_DeltaTime;
		if (m_AccumTime >= m_CurrState.length) 
		{
			m_AccumTime = m_CurrState.length;
			m_IsEndAnim = true;
		}
		m_CurrState.normalizedTime = m_AccumTime/m_CurrState.length; 
	}

	private void PlayAnimation(Animation anim, string clip)
	{
		m_AccumTime = 0F;
		m_CurrState = anim[clip];
		m_CurrState.weight = 1;
		m_CurrState.blendMode = AnimationBlendMode.Blend;
		m_CurrState.wrapMode = WrapMode.Once;
		m_CurrState.normalizedTime = 0;
		m_CurrState.enabled = true;
		m_IsPlaying = true;
		m_IsEndAnim = false;
	}
}
