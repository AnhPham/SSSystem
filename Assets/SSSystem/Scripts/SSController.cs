/**
 * Created by Anh Pham on 2013/11/13
 * Email: anhpt.csit@gmail.com
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
	public bool 	IsLock 		{ get; private set; }
	public bool 	IsStarted 	{ get; private set; }
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
	/// If you want to override this Start() function. Don't forget call base.Start() in it.
	/// </summary>
	public virtual void Start()
	{
		if (SSSceneManager.Instance == null)
		{
			StartCoroutine (OnStartWithoutSceneManager ());
		}

		OnEnableFS ();
		IsStarted = true;
	}

	/// <summary>
	/// This event will be raised at start time you play the scene if your scene doesn't have SSSceneManager. Convenient for testing.
	/// </summary>
	public virtual IEnumerator OnStartWithoutSceneManager()
	{
		yield return 0;
	}

	/// <summary>
	/// FS means 'First in Start'.  The first call of OnEnable() is right after Awake(). The first call of OnEnableFS() is in Start().
	/// </summary>
	public virtual void OnEnableFS()
	{
	}

	/// <summary>
	/// Start to show
	/// </summary>
	public virtual void OnEnable()
	{
		if (IsStarted)
		{
			OnEnableFS ();
		}
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

	/// <summary>
	/// Raises the key back click event (for android)
	/// </summary>
	public virtual void OnKeyBack()
	{
		SSSceneManager.Instance.Close ();
	}
}
