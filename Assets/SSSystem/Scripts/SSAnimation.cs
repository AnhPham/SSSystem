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
	/// Awake function.
	/// </summary>
	protected override void Awake()
	{
		// We should bring this scene to somewhere far when it awake.
		// Then the animation will automatically bring it back at next frame.
		// This trick remove flicker at the first frame.
		transform.localPosition = new Vector3(99999, 0, 0);
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

	private void Play(AnimationClip anim)
	{
		if (anim == null)
		{
			return;
		}

		if (animation == null)
		{
			gameObject.AddComponent<Animation>();
			animation.playAutomatically = false;
			animation.cullingType = AnimationCullingType.AlwaysAnimate;
		}

		if (animation.GetClip(anim.name) == null)
		{
			animation.AddClip(anim, anim.name);
		}

		animation.clip = anim;
		PlayAnimation (animation, anim.name);
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
