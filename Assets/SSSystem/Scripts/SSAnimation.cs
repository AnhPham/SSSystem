/**
 * Created by Anh Pham on 2013/11/13
 */

using UnityEngine;
using System.Collections;

public class SSAnimation : MonoBehaviour 
{
	[SerializeField]
	private AnimationClip m_ShowClip;

	[SerializeField]
	private AnimationClip m_HideClip;

	float m_TimeAtLastFrame = 0F;
	float m_TimeAtCurrentFrame = 0F;
	float m_DeltaTime = 0F;
	float m_AccumTime = 0F;

	AnimationState m_CurrState;
	bool m_IsPlaying = false;
	bool m_IsEndAnim = false;

	public float TimeShow()
	{
		return Time(m_ShowClip);
	}

	public float TimeHide()
	{
		return Time(m_HideClip);
	}

	public void PlayShow()
	{
		Play(m_ShowClip);
	}

	public void PlayHide()
	{
		Play(m_HideClip);
	}

	protected virtual void Awake()
	{
		transform.localPosition = new Vector3(99999, 0, 0);
	}

	protected virtual void Start()
	{
		if (SSSceneManager.Instance == null)
		{
			PlayShow();
		}
	}

	protected float Time(AnimationClip anim)
	{
		if (anim == null) 
		{
			return 0;
		}

		return anim.length;
	}

	protected void Play(AnimationClip anim)
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

	protected void Update()
	{
		m_TimeAtCurrentFrame = UnityEngine.Time.realtimeSinceStartup;
		m_DeltaTime = m_TimeAtCurrentFrame - m_TimeAtLastFrame;
		m_TimeAtLastFrame = m_TimeAtCurrentFrame; 

		if(m_IsPlaying) AnimationUpdate();
	}

	protected void AnimationUpdate()
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

	protected void PlayAnimation(Animation anim, string clip)
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
