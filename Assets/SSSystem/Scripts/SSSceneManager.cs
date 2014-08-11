/**
 * Created by Anh Pham on 2014/08/10
 * Email: anhpt.csit@gmail.com
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#region Delegate
public delegate void SSCallBackDelegate(SSController ctrl);
public delegate void NoParamCallback ();
#endregion

public enum Bgm
{
	/// <summary>
	/// When the scene changed, turn off BGM.
	/// </summary>
	NONE,

	/// <summary>
	/// When the scene changed, BGM will not be changed.
	/// </summary>
	SAME,

	/// <summary>
	/// When the scene changed, play a new BGM.
	/// </summary>
	PLAY,

	/// <summary>
	/// When the scene changed, turn off BGM.
	/// You will play BGM by your own code.
	/// You must to set the BgmName for SSController.
	/// </summary>
	CUSTOM
}

public enum AnimType
{
	NO_ANIM,
	SHOW,
	SHOW_BACK,
	HIDE,
	HIDE_BACK
}

public class CallbackData
{
	public float 			TimeOut { get; private set; }
	public NoParamCallback	Callback { get; private set; }

	public CallbackData(float timeOut, NoParamCallback callBack)
	{
		TimeOut = timeOut;
		Callback = callBack;
	}
}

public class SSSceneManager : MonoBehaviour 
{
	#region Const
	protected int SCENE_DISTANCE = 1000;			// The distance of loaded scenes in Base Scene
	protected int DEPTH_DISTANCE = 10;				// The camera depth distance of popup layers
	protected int SHIELD_TOP_INDEX = 9;				// The index of shield top
	#endregion

	#region Delegate
	public delegate void OnScreenStartChangeDelegate(string sceneName);
	public delegate void OnSceneActivedDelegate (string sceneName);
	#endregion

	#region Event
	public OnScreenStartChangeDelegate onScreenStartChange;
	public OnSceneActivedDelegate onSceneFocus;
	#endregion

	#region Serialize Field
	/// <summary>
	/// Loading scene name  (optional).
	/// </summary>
	[SerializeField]
	protected string m_LoadingSceneName;

	/// <summary>
	/// First scene name  (optional).
	/// </summary>
	[SerializeField]
	protected string m_FirstSceneName;

	/// <summary>
	/// Home scene name	 (optional)
	/// </summary>
	[SerializeField]
	protected string m_HomeSceneName;

	/// <summary>
	/// Default shield color.
	/// </summary>
	[SerializeField]
	protected Color m_DefaultShieldColor = new Color(0, 0, 0, 0.25f);

	/// <summary>
	/// // Check this if using UNIY PRO.
	/// </summary>
	[SerializeField]
	protected bool m_IsLoadAsync;
	#endregion

	#region Singleton
	protected static SSSceneManager m_Instance;
	public static SSSceneManager Instance
	{
		get { return m_Instance; }
	}
	#endregion

	#region Protected Member
	protected Stack<string> 					m_StackPopUp = new Stack<string>();						// Popup stack
	protected Stack<string> 					m_CurrentStackScreen = new Stack<string>();				// Popup stack
	protected List<GameObject> 					m_ListShield = new List<GameObject>();					// List Shield
	protected Dictionary<string, GameObject> 	m_DictAllScene = new Dictionary<string, GameObject>();	// Dictionary of loaded scenes
	protected Dictionary<string, Stack<string>> m_DictScreen = new Dictionary<string, Stack<string>>();	// Screen dict

	protected GameObject m_Scenes;			// Scene container object
	protected GameObject m_Shields;			// Shield container object
	protected GameObject m_Menu;			// Menu object
	protected GameObject m_ShieldTop;		// Shield top object
	protected GameObject m_ShieldEmpty;		// Shield empty object (higher all scene except top shield, alpha = 0)
	protected GameObject m_LoadingTop;		// Loading top object
	protected GameObject m_SolidCamera;		// Solid camera object (Lowest camera)

	protected int m_LoadingCount;			// Loading counter
	protected int m_ShieldEmptyCount;		// Shield empty counter
	protected bool m_IsBusy;				// Busy when scene is loading or scene-animation is playing
	protected bool m_CanClose;				// Force able to close even busy
	protected string m_GlobalBgm;			// Global BGM
	#endregion

	#region Public Function
	public virtual void Screen (string sceneName, object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
		StartCoroutine( IEScreen (sceneName, data, onActive, onDeactive) );
	}

    public virtual void Reset (object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
    {
        CloseAllPopUp();
        string sn = DestroyCurrentStack();
        Screen(sn, data, onActive, onDeactive);
    }

	public virtual void AddScreen (string sceneName, object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
		StartCoroutine( IEAddScreen (sceneName, data, onActive, onDeactive) );
	}

	public virtual void PopUp (string sceneName, object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
		StartCoroutine( IEPopUp (sceneName, data, onActive, onDeactive) );
	}

	public virtual void LoadMenu (string sceneName, object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
		StartCoroutine( IELoadMenu (sceneName, data, onActive, onDeactive) );
	}

	public virtual void Close (bool immediate = false, NoParamCallback callback = null)
	{
		StartCoroutine( IEClose (immediate, callback) );
	}

	public virtual void GoHome()
	{
		if (!string.IsNullOrEmpty(m_HomeSceneName))
		{
			if (m_CurrentStackScreen.Peek () == m_HomeSceneName) 
			{
				Quit ();
			}
			else
			{
				OpenScreen (m_HomeSceneName);
			}
		}
		else
		{
			Quit ();
		}
	}

	public void ShowMenu()
	{
		if (m_Menu != null) 
		{
			m_Menu.SetActive (true);
		}
	}

	public void HideMenu()
	{
		if (m_Menu != null) 
		{
			m_Menu.SetActive (false);
		}
	}

	public void DestroyInactiveScenes(params string[] exceptList)
	{
		// Screen stacks
        foreach (var screens in m_DictScreen)
		{
			string sn = screens.Key;
			if (IsNotInExcept(sn, exceptList) && (screens.Value != null && screens.Value.Count != 0))
			{
				DestroyScenesFrom(sn);
			}
		}

		// Rest scenes
		string[] restList = new string[m_DictAllScene.Keys.Count];
		m_DictAllScene.Keys.CopyTo(restList, 0);

		foreach (var sn in restList)
		{
			if (IsNotInExcept(sn, exceptList))
			{
				DestroyScenesFrom(sn);
			}
		}
	}

	public void BackToScreen()
	{
		StartCoroutine(IEBackScreen());
	}

	public void PreLoadScene(string sceneName)
	{
		string sn = sceneName;

		SSApplication.LoadLevelAdditive (sn, m_IsLoadAsync, (GameObject root) =>
		{
			GameObject scene = root;

			// Add to dictionary
			m_DictAllScene.Add(sn, scene);
			scene.transform.parent = m_Scenes.transform;

			// DeActive
			scene.SetActive (false);

			// Event
			OnSceneLoad (scene);
		});
	}

    public string DestroyCurrentStack()
    {
        Stack<string> stack = m_CurrentStackScreen;
        string s = null;

        while (stack != null && stack.Count >= 1)
        {
            s = stack.Pop();
            DestroyScene(s);
        }

        m_CurrentStackScreen = new Stack<string>();

        return s;
    }

	public void DestroyScenesFrom(string sceneName)
	{
		string sn = sceneName;

		// Check if in the dict
		if (!m_DictAllScene.ContainsKey(sn))
		{
			Debug.LogWarning("No exist scene to destroy: " + sn);
			return;
		}

		// Check in pop up stack
		if (m_StackPopUp.Contains(sn))
		{
			Debug.LogWarning("This pop up is active now: " + sn);
			return;
		}

		// Check in screen screen stacks
        foreach (var screens in m_DictScreen)
		{
			Stack<string> stack = screens.Value;

			if (stack.Contains(sn))
			{
				// Not in current stack
				if (stack != m_CurrentStackScreen)
				{
					if (stack.Count >= 1)
					{
						string s = stack.Pop();

						while (string.Compare(s, sn) != 0)
						{
							DestroyScene(s);
							s = stack.Pop();
						}
						DestroyScene(sn);
					}
				}
				else
				{
					Debug.LogWarning("Can't destroy this scene. It's in an active stack: " + sn);
					return;
				}

				// Break because found the stack contain scene.
				break;
			}
		}

		// If not in any stack, just destroy
		if (m_DictAllScene.ContainsKey(sn))
		{
			DestroyScene(sn);
			return;
		}
	}

	public bool IsSceneInAnyStack(string sceneName)
	{
		bool isInStack = false;

		if (m_CurrentStackScreen != null)
		{
			isInStack = m_CurrentStackScreen.Contains(sceneName);
		}

		isInStack = (isInStack || m_StackPopUp.Contains(sceneName));

		return isInStack;
	}

	/// <summary>
	/// Shows the loading indicator
	/// </summary>
	/// <param name="alpha">Alpha of shield.</param>
	/// <param name="timeOut">Time out.</param>
	/// <param name="callBack">Call back.</param>
    public void ShowLoading(float alpha = 0.5f, float timeOut = 0, NoParamCallback callBack = null, float delay = 0.5f)
	{
		if (m_LoadingTop == null) return;

		ShieldTopOn (alpha);
		m_LoadingTop.SetActive (true);

		m_LoadingCount++;

		if (timeOut > 0)
		{
			StartCoroutine ("IEHideLoading", new CallbackData(timeOut, callBack));
		}
	}

	/// <summary>
	/// Hides the loading indicator. If you called n-times ShowLoading, you must to call n-times HideLoading to hide.
	/// </summary>
	/// <param name="isForceHide">If set to <c>true</c> is force hide without counting.</param>
	public void HideLoading(bool isForceHide = false)
	{
		if (m_LoadingTop == null || !m_LoadingTop.activeInHierarchy) return;

		m_LoadingCount--;
		
		if (m_LoadingCount == 0 || isForceHide) 
		{
			m_LoadingCount = 0;
			ShieldTopOff ();
			m_LoadingTop.SetActive (false);
		}

		StopCoroutine ("IEHideLoading");
	}

	/// <summary>
	/// Get scene root.
	/// </summary>
	/// <returns>Scene Root.</returns>
	public GameObject SceneRoot()
	{
		return m_Scenes;
	}

	/// <summary>
	/// Set the global bgm. All scenes will have same BGM until ClearGlobalBgm() called.
	/// </summary>
	/// <param name="bgmName">Bgm name.</param>
	public void SetGlobalBgm(string bgmName)
	{
		m_GlobalBgm = bgmName;
	}

    /// <summary>
    /// Clears the global bgm.
    /// </summary>
	public void ClearGlobalBgm()
	{
		m_GlobalBgm = string.Empty;
	}
	#endregion

	#region Protected Function
	/// <summary>
	/// Awake this instance. Config something when app start. Override if necessary.
	/// </summary>
	protected virtual void Awake()
	{
		m_Instance = this;
		m_GlobalBgm = string.Empty;

		m_SolidCamera = Instantiate (Resources.Load ("SolidCamera")) as GameObject;
		m_SolidCamera.name = "SolidCamera";
		m_SolidCamera.transform.localPosition = new Vector3(-(SHIELD_TOP_INDEX+0.5f) * SCENE_DISTANCE, 0, 0);
		
		m_Scenes = new GameObject("Scenes");
		m_Shields = new GameObject("Shields");

		CreateLoadingsThenLoadFirstScene ();
	}

	protected virtual void CreateLoadingsThenLoadFirstScene()
	{
		if (string.IsNullOrEmpty (m_LoadingSceneName))
		{
			OnFirstSceneLoad ();
			return;
		}

		if (m_LoadingTop == null) 
		{
			SSApplication.LoadLevelAdditive (m_LoadingSceneName, m_IsLoadAsync, (GameObject root) =>
				{
					CreateLoadingTop(root);

					OnFirstSceneLoad ();
				});
		}
	}

	protected virtual void Quit()
	{
		Debug.Log ("Quit: Please override this");
	}

	/// <summary>
	/// Play the BGM. Override it.
	/// </summary>
	/// <param name="bgmName">Bgm name.</param>
	protected virtual void PlayBGM(string bgmName)
	{
		Debug.LogWarning("Play BGM: " + bgmName + ". You have to override function: PlayBGM");
	}

	/// <summary>
	/// Stops the BGM. Override it
	/// </summary>
	protected virtual void StopBGM()
	{
		Debug.LogWarning("Stop BGM. You have to override function: StopBGM");
	}

	/// <summary>
	/// Raises the scene load event.
	/// </summary>
	/// <param name="scene">Root object of loaded scene.</param>
	protected virtual void OnSceneLoad (GameObject scene)
	{

	}

	/// <summary>
	/// Raises the scene unload event.
	/// </summary>
	/// <param name="scene">Root object of unloaded scene.</param>
	protected virtual void OnSceneUnload (GameObject scene)
	{

	}

    /// <summary>
    /// Raises the animation finish event.
    /// </summary>
    /// <param name="sceneName">Scene name.</param>
    protected virtual void OnAnimationFinish(string sceneName)
    {
        Debug.Log("If you have the problem with NGUI which display not correctly when animation finish, you can SetDirty() to all UIPanel in UIPanel.list in this event for a refresh.");
    }

	/// <summary>
	/// Default first scene is 'First Scene Name', you can override this function.
	/// </summary>
	protected virtual void OnFirstSceneLoad()
	{
        if (!string.IsNullOrEmpty(m_FirstSceneName))
        {
            Screen (m_FirstSceneName);
        }
	}

	protected void CreateLoadingTop(GameObject root)
	{
		m_LoadingTop = root;
		m_LoadingTop.name = m_LoadingTop.name + "Top";

		SetPosition (m_LoadingTop, SHIELD_TOP_INDEX);
		SetCameras (m_LoadingTop, SHIELD_TOP_INDEX);

		m_LoadingTop.SetActive (false);
	}

	protected GameObject CreateShield(int i)
	{
		// Instantiate from resources
		GameObject sh = Instantiate(Resources.Load("Shield")) as GameObject;
		sh.name = "Shield" + i;
		sh.transform.localPosition = new Vector3(i * SCENE_DISTANCE, 0, 0);
		sh.transform.parent = m_Shields.transform;

		// Set camera depth
		Camera c = sh.GetComponentInChildren<Camera>();
		c.depth = (i+1) * DEPTH_DISTANCE;

		return sh;
	}

	protected void ShieldTopOn(float alpha)
	{
		if (m_ShieldTop == null) 
		{
			m_ShieldTop = CreateShield (SHIELD_TOP_INDEX-1);
		} else 
		{
			m_ShieldTop.SetActive (true);
		}

		MeshRenderer mesh = m_ShieldTop.GetComponentInChildren<MeshRenderer> ();
		mesh.material.color = new Color (0, 0, 0, alpha);

		// Lock
		LockTopScene ();
	}

	protected void ShieldTopOff()
	{
		if (m_ShieldTop != null) 
		{
			m_ShieldTop.SetActive (false);

			// Unlock
			UnlockTopScene ();
		}
	}

	protected void ShieldOn(int i)
	{
		ShieldOn (i, m_DefaultShieldColor);
	}

	protected void ShieldOn(int i, Color color)
	{
		if (i < 0) return;

		if (m_ListShield.Count <= i)
		{
			// Create shield
			GameObject sh = CreateShield (i);

			// Add to List
			m_ListShield.Add(sh);
		}
		else
		{
			m_ListShield[i].SetActive(true);
		}

		MeshRenderer mesh = m_ListShield[i].GetComponentInChildren<MeshRenderer> ();
		mesh.material.color = color;

		// Lock
		LockTopScene ();
	}

	protected bool IsShieldActive(int i)
	{
		if (i >= m_ListShield.Count)
			return false;

		return (m_ListShield [i].activeInHierarchy);
	}

	protected void ShieldOff()
	{
		int i = m_StackPopUp.Count;

		if (i < 0) return;

		if (m_ListShield.Count >= i)
			m_ListShield[i].SetActive(false);

		// Unlock
		UnlockTopScene ();
	}

	protected void LockTopScene()
	{
	}

	protected void UnlockTopScene()
	{
	}

	protected void SetPosition(string sn, int i)
	{
		GameObject sc = m_DictAllScene[sn];

		SetPosition (sc, i);
	}

	protected void SetPosition(GameObject sc, int i)
	{
		sc.transform.localPosition = new Vector3(i * SCENE_DISTANCE, SCENE_DISTANCE, 0);
	}

	protected void SetCameras(string sn, float i)
	{
		GameObject sc = m_DictAllScene[sn];

		SetCameras (sc, i);
	}

	protected void SetCameras(GameObject sc, float i)
	{
		// Sort by depth
		List<Camera> cams = new List<Camera>(sc.GetComponentsInChildren<Camera>(true));
		cams = cams.OrderBy(n => n.depth).ToList<Camera>();

		// Re-set depth
		int c = 0;
		foreach (Camera cam in cams)
		{
			AudioListener al = cam.GetComponent<AudioListener>();
			if (al != null) al.enabled = false;

			cam.tag = "Untagged";
			cam.clearFlags = CameraClearFlags.Depth;
			cam.depth = Mathf.RoundToInt(i * DEPTH_DISTANCE) + c + 1;
			c++;
		}

		// Re-sort camera list For NGUI
		if (cams.Count > 0) 
		{
			MonoBehaviour uicam = cams [0].GetComponent ("UICamera") as MonoBehaviour;

			if (uicam != null) 
			{
				uicam.enabled = false;
				uicam.enabled = true;
			}
		}
	}

	protected void BgmSceneOpen(string curBgm, SSController ctrl)
	{
		if (!string.IsNullOrEmpty (m_GlobalBgm))
		{
			PlayBGM (m_GlobalBgm);
		}
		else
		{
			switch (ctrl.BgmType)
			{
				case Bgm.NONE:
					StopBGM ();
					break;

				case Bgm.PLAY:
					ctrl.CurrentBgm = ctrl.BgmName;
					if (!string.IsNullOrEmpty (ctrl.BgmName))
					{
						PlayBGM (ctrl.BgmName);
					}
					break;

                case Bgm.SAME:
                    ctrl.CurrentBgm = curBgm;
                    if (!string.IsNullOrEmpty(curBgm))
                    {
                        PlayBGM(curBgm);
                    }
					break;
				case Bgm.CUSTOM:
					StopBGM ();
					ctrl.CurrentBgm = ctrl.BgmName;
					break;
			}
		}
	}

	protected void BgmSceneClose(SSController ctrl)
	{
		if (!string.IsNullOrEmpty (m_GlobalBgm))
		{
			// Do nothing
		}
		else
		{
			switch (ctrl.BgmType)
			{
				case Bgm.NONE:
					StopBGM ();
					break;

				case Bgm.PLAY:
				case Bgm.SAME:
				case Bgm.CUSTOM:
					if (!string.IsNullOrEmpty (ctrl.CurrentBgm))
					{
						PlayBGM (ctrl.CurrentBgm);
					}
					break;
			}
		}
	}

	protected IEnumerator IEPlayAnimation(string sn, AnimType animType, NoParamCallback callback = null)
	{
		SSMotion an = m_DictAllScene[sn].GetComponentInChildren<SSMotion>();
		if (an != null && animType != AnimType.NO_ANIM)
		{
			yield return null;

			NoParamCallback play = null;
			float time = 0;

			switch (animType) 
			{
				case AnimType.SHOW:
					play = an.PlayShow;
					time = an.TimeShow ();
					break;
				case AnimType.SHOW_BACK:
					play = an.PlayShowBack;
					time = an.TimeShowBack ();
					break;
				case AnimType.HIDE:
					play = an.PlayHide;
					time = an.TimeHide ();
					break;
				case AnimType.HIDE_BACK:
					play = an.PlayHideBack;
					time = an.TimeHideBack ();
					break;
			}

			if (play != null)
			{
				play ();
			}
			yield return StartCoroutine (Pause (time));
            yield return new WaitForEndOfFrame();

            OnAnimationFinish(sn);

			if (callback != null)
				callback ();
		}
		else
		{
			if (animType == AnimType.NO_ANIM && an != null) 
			{
				an.transform.localPosition = Vector3.zero;
			}

			if (callback != null)
				callback ();
		}
	}

	#if UNITY_EDITOR || UNITY_ANDROID
	protected virtual void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			SSController ct = null;

			if (m_StackPopUp.Count >= 1) 
			{
				ct = GetController (m_StackPopUp.Peek ());
			} 
			else
			{
				if (m_CurrentStackScreen != null && m_CurrentStackScreen.Count >= 1) 
				{
					ct = GetController (m_CurrentStackScreen.Peek ());
				}
			}

			if (ct != null) 
			{
				ct.OnKeyBack ();
			} 
			else 
			{
				Close ();
			}
		}
	}
	#endif
	#endregion

	#region Private Function
	private IEnumerator IEHideLoading(CallbackData callbackData)
	{
		yield return new WaitForSeconds (callbackData.TimeOut);

		HideLoading ();

		if (callbackData.Callback != null)
		{
			callbackData.Callback ();
		}
	}

	private IEnumerator Pause(float time)
	{
		float pauseEndTime = Time.realtimeSinceStartup + time;
		while (Time.realtimeSinceStartup < pauseEndTime)
		{
			yield return 0;
		}
		yield return 0;
	}


	private bool IsNotInExcept(string sn, params string[] exceptList)
	{
		bool isOk = true;
		for (int i = 0; i < exceptList.Length; i++)
		{
			if (string.Compare(exceptList[i], sn) == 0)
			{
				isOk = false;
				break;
			}
		}

		return isOk;
	}

	private void DestroyScene(string sceneName)
	{
		// Scene Name & Scene
		string sn = sceneName;
		GameObject sc = m_DictAllScene[sn];

		// Unload
		OnSceneUnload (sc);

		m_DictAllScene.Remove (sc.name);
		SSApplication.OnUnloaded (sc);

		// Destroy
		Destroy (sc);
	}

	private IEnumerator IEScreen (string sceneName, object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
		yield return StartCoroutine (IEWaitForNotBusy ());
		OpenScreen (sceneName, data, onActive, onDeactive);
	}

	private IEnumerator IEAddScreen (string sceneName, object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
		yield return StartCoroutine (IEWaitForNotBusy ());
		OpenScreenAdd (sceneName, data, false, onActive, onDeactive);
	}

	private IEnumerator IEPopUp (string sceneName, object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
		yield return StartCoroutine (IEWaitForNotBusy ());

        if (IsSceneActive(sceneName))
        {
            m_CanClose = true;
        }

        while (IsSceneActive (sceneName)) 
		{
			yield return new WaitForEndOfFrame ();
		}
		m_IsBusy = true;

		OpenPopUp (sceneName, data, false, onActive, onDeactive);
	}

    private bool IsSceneActive( string sceneName)
    {
        string sn = sceneName;

        if (m_DictAllScene.ContainsKey(sn))
        {
            return m_DictAllScene[sn].activeInHierarchy;
        }

        return false;
    }

	private IEnumerator IELoadMenu (string sceneName, object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
        yield return new WaitForEndOfFrame();
		OpenMenu (sceneName, onActive, onDeactive);
	}

    private void CloseAllPopUp()
    {
        while (m_StackPopUp.Count >= 1) 
        {
            ClosePopUp(true);
        }
    }

	private IEnumerator IEBackScreen ()
	{
		yield return StartCoroutine (IEWaitForNotBusy ());

        // Close all pop up
        CloseAllPopUp();

        // Close screens
		int n = m_CurrentStackScreen.Count;
		if (n == 2)
		{
			CloseAny();
		}
		else
		{
			while (m_CurrentStackScreen.Count > 1)
			{
				CloseAny(true);
			}
		}
	}

	private IEnumerator IEClose (bool immediate = false, NoParamCallback callback = null)
	{
        if (m_CanClose)
        {
            m_CanClose = false;
            CloseAny(immediate, callback);
        }
        else
        {
            yield return StartCoroutine(IEWaitForNotBusy());
            CloseAny(immediate, callback);
        }
	}

	private void CloseAny(bool imme = false, NoParamCallback callback = null)
	{
		if (m_StackPopUp.Count >= 1) 
		{
			ClosePopUp (imme, callback);
		} 
		else
		{
			if (m_CurrentStackScreen.Count > 1) 
			{
				CloseScreen (imme, callback);
			} 
			else 
			{
				if (m_CurrentStackScreen.Count == 1) 
				{
					// Do Nothing
				}
                m_IsBusy = false;
			}
		}
	}

	private GameObject GetRoot(string sceneName)
	{
		string sn = sceneName;

		if (m_DictAllScene.ContainsKey (sn)) 
		{
			return m_DictAllScene [sn];
		}

		return null;
	}

	private SSController GetController(string sceneName)
	{
		string sn = sceneName;

		if (m_DictAllScene.ContainsKey (sn)) 
		{
			GameObject sc = m_DictAllScene [sn];
			SSController[] cts = sc.GetComponentsInChildren<SSController> (true);

			if (cts.Length > 0) 
			{
				return cts [0];
			}
		}

		return null;
	}

	private void DestroyOrCache(GameObject sc, SSController ct)
	{
		if (ct != null) 
		{
			if (!ct.IsCache) 
			{
				OnSceneUnload (sc);

				m_DictAllScene.Remove (sc.name);
				SSApplication.OnUnloaded (sc);

				Destroy (sc);
			}
		}
	}

	private void ShowAndBgmChangeClose(string sceneName)
	{
		// Prev Scene
		SSController ct = GetController (sceneName);

		// On Show
		if (ct != null)
		{
            ct.OnFocus(true);
		}

		// On Scene Showed
		if (onSceneFocus != null)
		{
			onSceneFocus(sceneName);
		}

		// Bgm change
        if (ct != null)
        {
            BgmSceneClose(ct);
        }
	}

	private void ShowAndBgmChangeOpen(string sceneName, string curBgm)
	{
		// Prev Scene
		SSController ct = GetController (sceneName);

		// On Show
		if (ct != null)
		{
            ct.OnFocus(true);
            ct.OnShow();
		}

		// On Scene Showed
		if (onSceneFocus != null)
		{
			onSceneFocus(sceneName);
		}

		// Bgm change
        if (ct != null)
        {
            BgmSceneOpen(curBgm, ct);
        }
	}

	private void ActiveAScene(string sceneName)
	{
		string sn = sceneName;

		GameObject sc = GetRoot (sn);
		SSController ct = GetController (sn);

		// On Active
		if (ct != null)
		{
			if (ct.OnActive != null) 
			{
				ct.OnActive (ct);
				ct.OnActive = null;
			}
		}

		// Active true
		sc.SetActive (true);
	}

	private void DeactiveAScene(string sceneName)
	{
		string sn = sceneName;

		GameObject sc = GetRoot (sn);
		SSController ct = GetController (sn);

		// On Deactive
		if (ct != null)
		{
			if (ct.OnDeactive != null) 
			{
				ct.OnDeactive (ct);
				ct.OnDeactive = null;
			}
		}

		// Active false
		sc.SetActive (false);
	}

	private void SetActiveDeactiveCallback(string sceneName, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
		SSController ct = GetController (sceneName);

		if (ct != null) 
		{
			ct.OnActive = onActive;
			ct.OnDeactive = onDeactive;
		}
	}

	private void LoadOrActive(string sceneName, NoParamCallback onLoaded, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
		string sn = sceneName;

		if (m_DictAllScene.ContainsKey(sn))
		{
			// Callback
			SetActiveDeactiveCallback (sn, onActive, onDeactive);

			// Active
			ActiveAScene (sn);

			// Event
			onLoaded ();
		}
		else
		{
			SSApplication.LoadLevelAdditive (sn, m_IsLoadAsync, (GameObject root) =>
				{
					GameObject scene = root;

					// Add to dictionary
					m_DictAllScene.Add(sn, scene);
					scene.transform.parent = m_Scenes.transform;

					// Callback
					SetActiveDeactiveCallback (sn, onActive, onDeactive);

					// Active
					ActiveAScene (sn);

					// Event
					OnSceneLoad (scene);
					onLoaded ();
				});
		}
	}

    private void HideAll(Stack<string> stackScreen)
	{
		// Close all pop up
        CloseAllPopUp();

		// Get is cache
		bool isClearStackScreen = false;
		string bot = StackScreenBottom (stackScreen);
		if (!string.IsNullOrEmpty (bot)) 
		{
			SSController ct = GetController (bot);
			if (ct != null && !ct.IsCache) 
			{
				isClearStackScreen = true;
			}
		}

		// Close all screen of this stack
        foreach (var screen in stackScreen) 
		{
			SSController ct = GetController (screen);
			if (ct != null) 
			{
				ct.IsCache = !isClearStackScreen;
			}
			CmClose (screen, true);
		}

		// Clear if not cache
		if (isClearStackScreen) 
		{
			stackScreen.Clear ();
		}
	}

	private string StackScreen(string sn)
	{
		// Check Exist
		if (!m_DictScreen.ContainsKey (sn)) 
		{
			m_DictScreen.Add (sn, new Stack<string> ());
		}
		m_CurrentStackScreen = m_DictScreen [sn];

		// If empty, push then return sn
		if (m_CurrentStackScreen.Count == 0) 
		{
			m_CurrentStackScreen.Push (sn);
			return sn;
		}

		// If not empty, return top of stack
		return m_CurrentStackScreen.Peek ();
	}

	private string StackScreenBottom(Stack<string> stack)
	{
		if (stack == null)
			return null;

		string r = null;
		foreach (string s in stack)
		{
			r = s;
		}
		return r;
	}

	private string StackScreenBottom()
	{
		return StackScreenBottom (m_CurrentStackScreen);
	}

	private void ShowEmptyShield()
	{
		if (m_ShieldEmpty == null) 
		{
			m_ShieldEmpty = CreateShield (SHIELD_TOP_INDEX-2);
		} else 
		{
			m_ShieldEmpty.SetActive (true);
		}

		MeshRenderer mesh = m_ShieldEmpty.GetComponentInChildren<MeshRenderer> ();
		mesh.material.color = new Color (0, 0, 0, 0);

		m_ShieldEmptyCount++;

		// Lock
		LockTopScene ();
	}

	private void HideEmptyShield()
	{
		if (m_ShieldEmpty != null) 
		{
			m_ShieldEmptyCount--;

			if (m_ShieldEmptyCount == 0) 
			{
				m_ShieldEmpty.SetActive (false);

				// Unlock
				UnlockTopScene ();
			}
		}
	}

	private bool CanOpenPopUp(string sceneName)
	{
		if (m_StackPopUp.Contains (sceneName)) 
		{
			Debug.LogWarning ("This popup was added to stack!");
			return false;
		}

		return true;
	}

	private bool CanOpenScreen(string sceneName)
	{
		foreach (var pair in m_DictScreen) 
		{
			string bot = StackScreenBottom (pair.Value);

			bool isCurStack = (m_CurrentStackScreen == pair.Value);
			bool isInStack = pair.Value.Contains (sceneName);

            if (sceneName == bot && isCurStack) 
			{
                if (m_CurrentStackScreen.Count > 1)
                {
                    BackToScreen();
                }

				return false;
			}

			if (sceneName != bot && isInStack) 
			{
				Debug.LogWarning ("This screen was added to stack!");
				return false;
			}
		}

		return true;
	}

	private bool CanOpenScreenAdd(string sceneName)
	{
		foreach (var pair in m_DictScreen) 
		{
			bool isInStack = pair.Value.Contains (sceneName);

			if (isInStack)
			{
				Debug.LogWarning ("This screen was added to stack!");
				return false;
			}
		}

		return true;
	}

	private void CmClose(string sceneName, bool imme, NoParamCallback onAnimEnded = null)
	{
		string sn = sceneName;
		AnimType animType = (imme) ? AnimType.NO_ANIM : AnimType.HIDE;

		GameObject sc = GetRoot (sn);
		SSController ct = GetController (sn);

		if (ct != null) 
		{
			// Show Empty shield
			ShowEmptyShield ();

			// Hide
            ct.OnFocus (false);
            ct.OnHide ();

			StartCoroutine(IEPlayAnimation(sn, animType, () =>
				{
					// Deactive
					DeactiveAScene(sn);

					// Destroy or Cache
					DestroyOrCache(sc, ct);

					// Hide empty shield
					HideEmptyShield();

					// Next Step
					if (onAnimEnded != null) onAnimEnded();

					// No Busy
					m_IsBusy = false;
				}));
		}
	}

	private void ClosePopUp(bool imme, NoParamCallback callback = null)
	{
		string curSn = m_StackPopUp.Pop ();
		string preSn = string.Empty;

		if (m_StackPopUp.Count >= 1)
		{
			preSn = m_StackPopUp.Peek();
		}
		else
		{
            if (m_CurrentStackScreen != null && m_CurrentStackScreen.Count >= 1)
            {
                preSn = m_CurrentStackScreen.Peek();
            }
		}

		CmClose (curSn, imme, () => 
			{
				// Shield Off
				ShieldOff();

				// Show & BGM change
				ShowAndBgmChangeClose(preSn);

				// Callback
				if (callback != null)
				{
					callback();
				}
			});
	}

	private void CloseScreen(bool imme, NoParamCallback callback = null)
	{
		string curSn = m_CurrentStackScreen.Pop ();
		string preSn = string.Empty;

		// Check if has prev scene
		if (m_CurrentStackScreen.Count > 0) 
		{
			preSn = m_CurrentStackScreen.Peek ();
		}

		// Thread 1 (current scene animation)
		CmClose (curSn, imme, () => 
			{
				if (callback != null)
				{
					callback();
				}
			});

		// Thread 2 (previous scene animation)
		if (!string.IsNullOrEmpty (preSn)) 
		{
			// Active
			ActiveAScene (preSn);

			// Animation
			AnimType animType = (imme) ? AnimType.NO_ANIM : AnimType.SHOW_BACK;
			StartCoroutine (IEPlayAnimation (preSn, animType, () => 
				{
					// Show & BGM change
					ShowAndBgmChangeClose (preSn);
				}));
		}
		else
		{
			// Do nothing or application quit
		}
	}

	private void CmOpen(string sceneName, int ip, float ic, object data, bool imme, string curBgm, NoParamCallback onAnimEnded = null, NoParamCallback onLoaded = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
		string sn = sceneName;

		// Show Empty shield
		ShowEmptyShield ();

		LoadOrActive (sn, () => 
			{
				if (onLoaded != null)
				{
					onLoaded();
				}

				// Set camera and position
				SetCameras(sn, ic);
				SetPosition(sn, ip); 

				// On Set
				SSController ct = GetController(sn);
				if (ct != null)
				{
					ct.OnSet(data);
				}

				// Animation
				AnimType animType = (imme) ? AnimType.NO_ANIM : AnimType.SHOW;
				StartCoroutine (IEPlayAnimation (sn, animType, () => 
					{
						// Show & BGM change
						ShowAndBgmChangeOpen (sn, curBgm);

						// Hide empty shield
						HideEmptyShield();

						// Call back
						if (onAnimEnded != null) onAnimEnded();

						// No busy
						m_IsBusy = false;
					}));
			}, onActive, onDeactive);
	}

	private void OpenPopUp(string sceneName, object data = null, bool imme = false, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
		string sn = sceneName;

		// Check valid
		if (!CanOpenPopUp (sn)) 
		{
            m_IsBusy = false;
			return;
		}

		// Count
		int c = m_StackPopUp.Count + 1;

		// Next index
		int ip = c;
		float ic = c;

		// Prev Scene
		SSController ct = null;
		string curBgm = null;

		// Highest popup
		if (m_StackPopUp.Count >= 1)
		{
			string preSn = m_StackPopUp.Peek();

			ct = GetController(preSn);
		}
		// Or highest screen
		else if (m_CurrentStackScreen != null && m_CurrentStackScreen.Count >= 1)
		{
			string preSn = m_CurrentStackScreen.Peek();

			ct = GetController(preSn);
		}

		if (ct != null)
		{
			// Cur Bgm
			curBgm = ct.CurrentBgm;

			// Shield
			ShieldOn(m_StackPopUp.Count);
		}
		else
		{
			ShieldOn(0);
		}

		// Push stack
		m_StackPopUp.Push(sn);

		CmOpen (sn, ip, ic, data, imme, curBgm, () => 
			{
				// On Hide
				if (ct != null)
				{
                    ct.OnFocus (false);
				}
			}, null, onActive, onDeactive);
	}

	private void OpenScreen(string sceneName, object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
		string sn = sceneName;

		// Check valid
		if (!CanOpenScreen (sn)) 
		{
            m_IsBusy = false;
			return;
		}

		// Prev stack screen
		Stack<string> prevStack = m_CurrentStackScreen;

		// Set screen stack to current
		string top = StackScreen (sn);

		// onScreenStartChange
		if (onScreenStartChange != null) 
		{
			onScreenStartChange (sn);
		}

		if (sn == top) // First time load
		{
			CmOpen (sn, 0, 0, data, true, string.Empty, () => 
			{
				// Hide All
				HideAll (prevStack);
			}, null, onActive, onDeactive);
		} 
		else // Not first time
		{
			// Hide All
			HideAll (prevStack);

			// Just active scene
			ActiveAScene (top);

			// Show and Bgm
			SSController ct = GetController (top);
			if (ct != null) 
			{
                Debug.Log(ct.CurrentBgm);
				ShowAndBgmChangeOpen (top, ct.CurrentBgm);
			}

			m_IsBusy = false;
		}
	}

	private void OpenScreenAdd(string sceneName, object data = null, bool imme = false, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
		string sn = sceneName;

		// Check valid
		if (!CanOpenScreenAdd (sn)) 
		{
            m_IsBusy = false;
			return;
		}

		// Bot Scene
		string bot = StackScreenBottom();
		if (string.IsNullOrEmpty (bot)) 
		{
			Debug.LogWarning ("Main screen is not exist, can't add this screen above!");
			return;
		}

		// Next index
		int ip = -2 - m_CurrentStackScreen.Count;
		float ic = 0.3f + (float)m_CurrentStackScreen.Count / DEPTH_DISTANCE;

		// Prev Scene
		string preSn = m_CurrentStackScreen.Peek ();
		SSController ct = GetController (preSn);

		// Cur Bgm
		string curBgm = ct.CurrentBgm;

		// Thread 1
		CmOpen (sn, ip, ic, data, imme, curBgm, () => 
		{
			// Set IsCache of this scene same the IsCache of bottom scene of stack screen
			SSController newCt = GetController(sn);
			newCt.IsCache = true;

			// Push stack
			m_CurrentStackScreen.Push(sn);
		}, () => 
		{
			// Thread 2
			if (ct != null)
			{
                ct.OnFocus (false);
			}
			// Animation
			AnimType animType = (imme) ? AnimType.NO_ANIM : AnimType.HIDE_BACK;
			StartCoroutine (IEPlayAnimation (preSn, animType, () => 
				{
					DeactiveAScene(preSn);
				}));
		}, onActive, onDeactive);
	}

	private void OpenMenu(string sceneName, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
		string sn = sceneName;

		// Next index
		int ip = -1;
		float ic = 0.8f;

		CmOpen (sn, ip, ic, null, true, string.Empty, null, () => { m_Menu = m_DictAllScene[sn]; }, onActive, onDeactive);
	}

	private IEnumerator IEWaitForNotBusy()
	{
		while (m_IsBusy) 
		{
			yield return new WaitForEndOfFrame ();
		}

		m_IsBusy = true;
	}

    /*
	private void OnGUI()
	{
		GUILayout.BeginVertical ();

		if (GUILayout.Button("Print all stack"))
		{
			foreach (var item in m_DictScreen) 
            {
                Debug.Log ("Main: " + item.Key);
				foreach (var it in item.Value) 
				{
					Debug.Log (it);
				}
			}
		}

        if (GUILayout.Button("Print all popup"))
        {
            foreach (var item in m_StackPopUp)
            {
                Debug.Log (item);
            }
        }

        if (GUILayout.Button("Close Imme"))
        {
            Close(true);
        }

		GUILayout.EndVertical ();
	}
    */   

	#endregion
}
