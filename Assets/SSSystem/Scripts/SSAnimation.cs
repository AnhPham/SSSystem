/**
 * Created by Anh Pham on 2013/11/13
 * Copyright (c) Anh Pham. All rights reserved.
 */

using UnityEngine;
using System.Collections;

public class SSAnimation : MonoBehaviour 
{
	[SerializeField]
	private AnimationClip m_ShowClip;

	[SerializeField]
	private AnimationClip m_HideClip;

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

		//transform.localPosition = Vector3.zero;

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
		animation.Play(anim.name);
	}
}
