/**
 * Created by Anh Pham on 2014/08/10
 * Email: anhpt.csit@gmail.com
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DictObject = System.Collections.Generic.Dictionary<string, UnityEngine.GameObject>;

#region Delegate
public delegate void SSCallBackDelegate(SSController ctrl);
public delegate void NoParamCallback ();
#endregion

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
	/// Check this if using UNIY PRO.
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
    protected SSPopUpModule    m_PopUpModule;
    protected SSScreenModule   m_ScreenModule;
    protected SSShieldModule   m_ShieldModule;
    protected SSSoundModule    m_SoundModule;
    protected SSMenuModule     m_MenuModule;

    protected DictObject m_DictAllScene;    // Dictionary of loaded scenes
	
	protected GameObject m_Scenes;			// Scene container object
	protected GameObject m_LoadingTop;		// Loading top object
	protected GameObject m_SolidCamera;		// Solid camera object (Lowest camera)

	protected int   m_LoadingCount;			// Loading counter
	protected bool  m_IsBusy;				// Busy when scene is loading or scene-animation is playing
	protected bool  m_CanClose;				// Force able to close even busy
	#endregion

	#region Public Function
	public virtual void Screen (string sceneName, object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
		StartCoroutine( IEScreen (sceneName, data, onActive, onDeactive) );
	}

    public virtual void Reset (object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
    {
        m_PopUpModule.CloseAllPopUp();
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
            if (m_ScreenModule.Peek () == m_HomeSceneName) 
			{
				Quit ();
			}
			else
			{
                m_ScreenModule.OpenScreen (m_HomeSceneName);
			}
		}
		else
		{
			Quit ();
		}
	}

	public void ShowMenu()
	{
        m_MenuModule.Show();
	}

	public void HideMenu()
	{
        m_MenuModule.Hide();
	}

	public void DestroyInactiveScenes(params string[] exceptList)
	{
		// Screen stacks
        m_ScreenModule.DestroyInactiveScreens(exceptList);

		// Rest scenes
		string[] restList = new string[m_DictAllScene.Keys.Count];
		m_DictAllScene.Keys.CopyTo(restList, 0);

		foreach (var sn in restList)
		{
			if (SSTools.IsNotInExcept(sn, exceptList))
			{
				DestroyScenesFrom(sn);
			}
		}
	}

	public void BackToBottomScreen()
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
        return m_ScreenModule.DestroyCurrentStack();
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
        if (m_PopUpModule.IsPopUpInStack(sn))
        {
            Debug.LogWarning("This pop up is active now: " + sn);
            return;
        }

		// Check in screen screen stacks
        m_ScreenModule.DestroyScreensFrom(sn);

		// If not in any stack, just destroy
		if (m_DictAllScene.ContainsKey(sn))
		{
			DestroyScene(sn);
			return;
		}
	}

	public bool IsSceneInAnyStack(string sceneName)
	{
        string sn = sceneName;

        return (m_ScreenModule.IsScreenInAnyStack(sn) || m_PopUpModule.IsPopUpInStack(sn));
	}

    public bool IsSceneInActiveStack(string sceneName)
    {
        string sn = sceneName;

        return (m_ScreenModule.IsScreenInActiveStack(sn) || m_PopUpModule.IsPopUpInStack(sn));
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

        m_ShieldModule.ShieldTopOn (alpha);
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
            m_ShieldModule.ShieldTopOff ();
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
        m_SoundModule.SetGlobalBgm(bgmName);
	}

    /// <summary>
    /// Clears the global bgm.
    /// </summary>
	public void ClearGlobalBgm()
	{
        m_SoundModule.ClearGlobalBgm();
	}
	#endregion

	#region Protected Virtual Function
    /// <summary>
    /// Init somethings when app start. If you want to override it, don't forget base.Awake();
    /// </summary>
    protected virtual void Awake()
    {
        // Singleton
        m_Instance = this;

        // Scene Dictionary.
        m_DictAllScene = new DictObject();

        // We need a solid camera on the lowest layer. All other cameras will be set to depth camera automatically.
        m_SolidCamera = Instantiate (Resources.Load ("SolidCamera")) as GameObject;
        m_SolidCamera.name = "SolidCamera";
        m_SolidCamera.transform.localPosition = new Vector3(-(SSConst.SHIELD_TOP_INDEX+0.5f) * SSConst.SCENE_DISTANCE, 0, 0);

        // The scene container.
        m_Scenes = new GameObject("Scenes");

        // Init modules.
        InitModules();

        // Load 'Loading' scene, disable it, then call 'OnFirstSceneLoad()'
        CreateLoadingsThenLoadFirstScene ();
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

    /// <summary>
    /// Quit this app.
    /// </summary>
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
    }
    #if UNITY_EDITOR || UNITY_ANDROID
    protected virtual void Update()
    {
        if (Input.GetKeyDown (KeyCode.Escape)) 
        {
            SSController ct = null;

            if (m_PopUpModule.HasPopUpInStack()) 
            {
                ct = GetController (m_PopUpModule.Peek());
            } 
            else
            {
                if (m_ScreenModule.HasScreenInActiveStack()) 
                {
                    ct = GetController (m_ScreenModule.Peek ());
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
    private IEnumerator IEScreen (string sceneName, object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
    {
        yield return StartCoroutine (IEWaitForNotBusy ());
        m_ScreenModule.OpenScreen (sceneName, data, onActive, onDeactive);
    }

    private IEnumerator IEAddScreen (string sceneName, object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
    {
        yield return StartCoroutine (IEWaitForNotBusy ());
        m_ScreenModule.OpenScreenAdd (sceneName, data, false, onActive, onDeactive);
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
        SetBusy(true);

        m_PopUpModule.OpenPopUp (sceneName, data, false, onActive, onDeactive);
    }

    private IEnumerator IELoadMenu (string sceneName, object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
    {
        yield return new WaitForEndOfFrame();
        m_MenuModule.OpenMenu (sceneName, onActive, onDeactive);
    }

    private IEnumerator IEBackScreen ()
    {
        yield return StartCoroutine (IEWaitForNotBusy ());

        // Close all pop up
        m_PopUpModule.CloseAllPopUp();

        // Close screens
        if (m_ScreenModule.Count() >= 2)
        {
            m_ScreenModule.RemoveMiddleOfStack();
            CloseAny();
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

    private void InitModules()
    {
        m_PopUpModule = new SSPopUpModule();
        m_ScreenModule = new SSScreenModule();
        m_ShieldModule = new SSShieldModule();
        m_SoundModule = new SSSoundModule();
        m_MenuModule = new SSMenuModule();

        foreach (var modules in SSModule.list)
        {
            modules.open += OpenCommon;
            modules.close += CloseCommon;

            modules.playBGM += PlayBGM;
            modules.stopBGM += StopBGM;

            modules.shieldOn += ShieldOn;
            modules.shieldOff += ShieldOff;

            modules.getSceneDictionary += GetSceneDictionary;
            modules.getController += GetController;

            modules.onFocusScene += OnFocusScene;
            modules.onFocus += OnFocus;

            modules.focusByOpen += FocusByOpen;
            modules.focusByCloseOther += FocusByCloseOther;

            modules.setBusy += SetBusy;
            modules.backToBottomScreen += BackToBottomScreen;
            modules.hideAll += HideAll;
            modules.playAnimation += PlayAnimation;
            modules.onScreenStartChange += OnScreenStartChange;

            modules.activeAScene += ActiveAScene;
            modules.deactiveAScene += DeactiveAScene;
            modules.destroyScene += DestroyScene;
            modules.destroyOrCache += DestroyOrCache;

            modules.getHighestScene += GetHighestScene;
        }
    }

    private void CreateLoadingsThenLoadFirstScene()
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

    private void CreateLoadingTop(GameObject root)
    {
        m_LoadingTop = root;
        m_LoadingTop.name = m_LoadingTop.name + "Top";

        SetPosition (m_LoadingTop, SSConst.SHIELD_TOP_INDEX);
        SetCameras (m_LoadingTop, SSConst.SHIELD_TOP_INDEX);

        m_LoadingTop.SetActive (false);
    }

    private void SetPosition(string sn, int i)
    {
        GameObject sc = m_DictAllScene[sn];

        SetPosition (sc, i);
    }

    private void SetPosition(GameObject sc, int i)
    {
        sc.transform.localPosition = new Vector3(i * SSConst.SCENE_DISTANCE, SSConst.SCENE_DISTANCE, 0);
    }

    private void SetCameras(string sn, float i)
    {
        GameObject sc = m_DictAllScene[sn];

        SetCameras (sc, i);
    }

    private void SetCameras(GameObject sc, float i)
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
            cam.depth = Mathf.RoundToInt(i * SSConst.DEPTH_DISTANCE) + c + 1;
            c++;
        }
    }

    private void PlayAnimation(string sn, AnimType animType, NoParamCallback callback = null)
    {
        StartCoroutine(IEPlayAnimation(sn, animType, callback));
    }

    private IEnumerator IEPlayAnimation(string sn, AnimType animType, NoParamCallback callback = null)
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
        }
        else
        {
            if (animType == AnimType.NO_ANIM && an != null) 
            {
                an.transform.localPosition = Vector3.zero;
            }
        }

        // Animation finish
        OnAnimationFinish(sn);

        // Callback
        if (callback != null)
            callback ();
    }

    private void OnScreenStartChange(string sceneName)
    {
        if (onScreenStartChange != null)
        {
            onScreenStartChange(sceneName);
        }
    }

    private void HideAll(Stack<string> stackScreen)
    {
        // Close all pop up
        m_PopUpModule.CloseAllPopUp();

        // Get is cache
        bool isClearStackScreen = false;
        string bot = SSTools.GetStackBottom (stackScreen);
        if (!string.IsNullOrEmpty (bot)) 
        {
            SSController ct = GetController (bot);
            if (ct != null && !ct.IsCache) 
            {
                isClearStackScreen = true;
            }
        }

        // Close all screen of this stack
        if (stackScreen.Count >= 1)
        {
            string sn = stackScreen.Peek();

            SSController ct = GetController (sn);
            if (ct != null) 
            {
                ct.IsCache = true;
            }
            CloseCommon (sn, true);
        }

        // Clear if not cache
        if (isClearStackScreen) 
        {
            DestroyScenesFrom(bot);
            stackScreen.Clear ();
        }
    }

    private void SetBusy(bool isBusy)
    {
        m_IsBusy = isBusy;
    }

    private string GetHighestScene()
    {
        if (m_PopUpModule.HasPopUpInStack())
        {
            return m_PopUpModule.Peek();
        }

        foreach (var module in SSModule.list)
        {
            if (module is IUnderPopUp)
            {
                IUnderPopUp up = (IUnderPopUp)module;
                if (up.IsBgmDecider())
                {
                    return up.GetCurrentSceneName();
                }
            }
        }

        return null;
    }

    private void ShieldOn(int i)
    {
        m_ShieldModule.ShieldOn (i, m_DefaultShieldColor);
    }

    private void ShieldOff()
    {
        int i = m_PopUpModule.Count();
        m_ShieldModule.ShieldOff(i);
    }

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

    private bool IsSceneActive( string sceneName)
    {
        string sn = sceneName;

        if (m_DictAllScene.ContainsKey(sn))
        {
            return m_DictAllScene[sn].activeInHierarchy;
        }

        return false;
    }

	private void CloseAny(bool imme = false, NoParamCallback callback = null)
	{
        if (m_PopUpModule.HasPopUpInStack()) 
		{
            m_PopUpModule.ClosePopUp (imme, callback);
		} 
		else
		{
            if (m_ScreenModule.Count() > 1) 
			{
                m_ScreenModule.CloseScreen (imme, callback);
			} 
			else 
			{
                if (m_ScreenModule.Count() == 1) 
				{
					// Do Nothing
				}
                SetBusy(false);
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

                return;
			}
		}
	}

    private void DestroyOrCache(string sceneName)
    {
        string sn = sceneName;

        GameObject sc = GetRoot(sn);
        SSController ct = GetController(sn);

        DestroyOrCache(sc, ct);
    }

	private void FocusByCloseOther(string sceneName)
	{
		// Prev Scene
        string sn = sceneName;
		SSController ct = GetController (sn);

		// On Show
        OnFocusScene(sn, ct, true);

		// On Scene Showed
		if (onSceneFocus != null)
		{
			onSceneFocus(sn);
		}

		// Bgm change
        if (ct != null)
        {
            m_SoundModule.BgmWhenCloseOtherScene(ct);
        }
	}

	private void FocusByOpen(string sceneName, string curBgm)
	{
		// Prev Scene
        string sn = sceneName;
		SSController ct = GetController (sn);

		// On Show
		if (ct != null)
		{
            OnFocusScene(sn, ct, true);
            ct.OnShow();
		}

		// On Scene Showed
		if (onSceneFocus != null)
		{
			onSceneFocus(sn);
		}

		// Bgm change
        if (ct != null)
        {
            m_SoundModule.BgmWhenOpenScene(curBgm, ct);
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

                // Config
                SSController ct = GetController(sn);
                if (ct != null) 
                {
                    ct.Root = scene;
                    ct.Config();
                }

				// Event
				OnSceneLoad (scene);
				onLoaded ();
			});
		}
	}

    private DictObject GetSceneDictionary()
    {
        return m_DictAllScene;
    }

    private void OpenCommon(string sceneName, int ip, float ic, object data, bool imme, string curBgm, NoParamCallback onAnimEnded = null, NoParamCallback onLoaded = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
    {
        string sn = sceneName;

        // Show Empty shield
        m_ShieldModule.ShowEmptyShield ();

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
                    FocusByOpen (sn, curBgm);

                    // Hide empty shield
                    m_ShieldModule.HideEmptyShield();

                    // Call back
                    if (onAnimEnded != null) onAnimEnded();

                    // No busy
                    SetBusy(false);
                }));
            }, onActive, onDeactive);
    }

	private void CloseCommon(string sceneName, bool imme, NoParamCallback onAnimEnded = null)
	{
		string sn = sceneName;
		AnimType animType = (imme) ? AnimType.NO_ANIM : AnimType.HIDE;

		GameObject sc = GetRoot (sn);
		SSController ct = GetController (sn);

		if (ct != null) 
		{
			// Show Empty shield
            m_ShieldModule.ShowEmptyShield ();

			// Hide
            OnFocusScene(sn, ct, false);
            ct.OnHide ();

			StartCoroutine(IEPlayAnimation(sn, animType, () =>
			{
				// Deactive
				DeactiveAScene(sn);

				// Destroy or Cache
				DestroyOrCache(sc, ct);

				// Hide empty shield
                m_ShieldModule.HideEmptyShield();

				// Next Step
				if (onAnimEnded != null) onAnimEnded();

				// No Busy
				SetBusy(false);
			}));
		}
	}

	private IEnumerator IEWaitForNotBusy()
	{
		while (m_IsBusy) 
		{
			yield return new WaitForEndOfFrame ();
		}

		SetBusy(true);
	}

    private void OnFocus(bool isFocus)
    {
        foreach (var module in SSModule.list)
        {
            if (module is IUnderPopUp)
            {
                IUnderPopUp up = (IUnderPopUp)module;
                string sn = up.GetCurrentSceneName();

                if (!string.IsNullOrEmpty(sn))
                {
                    OnFocusScene(sn, GetController(sn), isFocus);
                }
            }
        }
    }

    private void OnFocusScene(string sceneName, SSController ct, bool isFocus)
    {
        string sn = sceneName;

        if (ct != null)
        {
            ct.IsFocus = isFocus;
            ct.OnFocus(isFocus);
        }
            
        if (!string.IsNullOrEmpty(sn) && m_DictAllScene.ContainsKey(sn))
        {
            GameObject sc = m_DictAllScene[sn];

            Canvas[] cvs = sc.GetComponentsInChildren<Canvas>(true);
            foreach (var cv in cvs)
            {
                cv.receivesEvents = isFocus;
            }
        }
    }

    /*
	private void OnGUI()
	{
		GUILayout.BeginVertical ();

        m_ScreenManager.OnGUI();
        m_PopUpManager.OnGUI();

		GUILayout.EndVertical ();
	}
    */   

	#endregion
}
