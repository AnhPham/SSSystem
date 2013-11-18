﻿/**
 * Created by Anh Pham on 2013/11/13
 * Copyright (c) Anh Pham. All rights reserved.
 */

using UnityEngine;
using System.Collections;

public class SSController : MonoBehaviour 
{
	#region Event
	public SSCallBackDelegate OnActive;
	public SSCallBackDelegate OnDeactive;
	#endregion

	#region Config
	public Bgm 		BgmType { get; protected set; }
	public string 	BgmName { get; protected set; }
	public bool 	IsCache { get; protected set; }
	#endregion

	#region Public Member
	public string 	CurrentBgm { get; set; }
	#endregion

	#region Protected Member
	public bool 	IsLock { get; private set; }
	#endregion

	/// <summary>
	/// Config here
	/// </summary>
	public virtual void Awake()
	{
		BgmType = Bgm.NONE;
		BgmName = string.Empty;

		IsCache = true;
	}

	/// <summary>
	/// Start to show
	/// </summary>
	public virtual void OnEnable()
	{
	}

	/// <summary>
	/// Set data after starting to show
	/// </summary>
	/// <param name="data">Data type is object type, allows any object.</param>
	public virtual void OnSet(object data)
	{
	}

	/// <summary>
	/// Finish to show
	/// </summary>
	public virtual void OnShow()
	{
	}

	/// <summary>
	/// Start to hide
	/// </summary>
	public virtual void OnHide()
	{
	}

	/// <summary>
	/// Finish to hide
	/// </summary>
	public virtual void OnDisable()
	{
	}

	/// <summary>
	/// Focus back after finish to hide the pop up which above it
	/// </summary>
	public virtual void OnFocusBack()
	{
	}

	/// <summary>
	/// Focus lost after start to show the pop up which above it
	/// </summary>
	public virtual void OnFocusLost()
	{
	}

	/// <summary>
	/// Lock scene
	/// </summary>
	public virtual void OnLock()
	{
		IsLock = true;
	}

	/// <summary>
	/// Unlock scene
	/// </summary>
	public virtual void OnUnlock()
	{
		IsLock = false;
	}
}
