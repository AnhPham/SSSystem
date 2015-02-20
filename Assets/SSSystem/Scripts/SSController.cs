/**
 * Created by Anh Pham on 2013/11/13
 * Email: anhpt.csit@gmail.com
 */

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SSAutoController))]
public class SSController : MonoBehaviour 
{
	#region Event
	public SSCallBackDelegate OnActive;
	public SSCallBackDelegate OnDeactive;
	#endregion

	#region Config
	public Bgm 		BgmType { get; set; }
	public string 	BgmName { get; set; }
	public bool 	IsCache { get; set; }
	#endregion

	#region Public Member
	public string 	CurrentBgm  { get; set; }
    public bool     IsStarted   { get; private set; }
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
            OnStartWithoutSceneManager();
		}

		OnEnableFS ();
		IsStarted = true;
	}

	/// <summary>
    /// This event will not be raised if you open this scene from a Base scene contains SSSceneManager. Write anything here for testing. It's called at Start().
	/// </summary>
    public virtual void OnStartWithoutSceneManager()
	{
	}

	/// <summary>
	/// FS means 'First in Start'.  The first call of OnEnable() is right after Awake(). The first call of OnEnableFS() is in Start().
	/// </summary>
	public virtual void OnEnableFS()
	{
	}

	/// <summary>
    /// Raise the event which right after scene loaded or actived. An Unity default event which be called when set active true a game object.
	/// </summary>
	public virtual void OnEnable()
	{
		if (IsStarted)
		{
			OnEnableFS ();
		}
	}

	/// <summary>
    /// Raise the event which right after scene's enable. You can get the transfer-data here. 
	/// </summary>
	/// <param name="data">Data type is object type, allows any object.</param>
	public virtual void OnSet(object data)
	{
	}

	/// <summary>
    /// Raises the event when show-animation complete (only one time when this scene's opened by Screen(), AddScreen(), PopUp() of SSSceneManager)
    /// OnShow() is always called after OnSet(), but we don't know the order of OnShow() and Start() in case the scene has no animation. 
	/// </summary>
	public virtual void OnShow()
	{
	}

	/// <summary>
    /// Raises the event when hide-animation start (only one time when this scene's closed by Close() of SSSceneManager)
	/// </summary>
	public virtual void OnHide()
	{
	}

	/// <summary>
    /// Raises the event when hide-animation complete. An Unity default event which be called when set active false a game object.
	/// </summary>
	public virtual void OnDisable()
	{
	}

	/// <summary>
    /// Raises the event when this scene gets focus or loses focus.
	/// </summary>
    public virtual void OnFocus(bool isFocus)
	{
	}

	/// <summary>
    /// Raises the key back click event (for android). By default, the Close() method of SSSceneManager will be called.
	/// </summary>
	public virtual void OnKeyBack()
	{
		SSSceneManager.Instance.Close ();
	}
}
