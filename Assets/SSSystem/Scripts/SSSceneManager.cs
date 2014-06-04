/**
 * Created by Anh Pham on 2013/11/13
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

public enum SceneType
{
	SCREEN,
	SUB_SCREEN,
	POPUP,
	MENU,
	CLOSE,
	CLOSE_SUB,
	RESET,
	RES
}

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

public enum ForceType
{
	NO_FORCE,
	FORCE_DESTROY,
	FORCE_NO_DESTROY
}

public enum AnimType
{
	NO_ANIM,
	SHOW,
	SHOW_BACK,
	HIDE,
	HIDE_BACK
}

public class SceneData
{
	public string 				SceneName { get; private set; }
	public object 				Data { get; private set; }
	public SceneType 			Type { get; private set; }
	public SSCallBackDelegate 	OnActive { get; private set; }
	public SSCallBackDelegate 	OnDeactive { get; private set; }

	public SceneData(string sn, object dt, SSCallBackDelegate onActive, SSCallBackDelegate onDeactive, SceneType type)
	{
		SceneName = sn;
		Data = dt;
		Type = type;
		OnActive = onActive;
		OnDeactive = onDeactive;
	}
}

public class PopUpData
{
	/// <summary>
	/// Gets the data.
	/// </summary>
	/// <value>The data.</value>
	public object Data { get; protected set; }

	/// <summary>
	/// If a same popup was showed already, you can add data to it & don't show this popup anymore.
	/// </summary>
	/// <value><c>true</c> if you want to add data to the same popup; otherwise, <c>false</c>.</value>
	public bool IsAddData { get; protected set; }

	/// <summary>
	/// You can set to show this popup only when has no other popup.
	/// </summary>
	/// <value><c>true</c> if you only want to show this popup when has no other popup; otherwise, <c>false</c>.</value>
	public bool IsWaitNoPopUp { get; protected set; }

	public PopUpData(object data = null, bool isAddData = false, bool isWaitNoPopUp = false)
	{
		Data = data;
		IsAddData = isAddData;
		IsWaitNoPopUp = isWaitNoPopUp;
	}
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
	protected int SCENE_DISTANCE = 5000;			// The distance of loaded scenes in Base Scene
	protected int DEPTH_DISTANCE = 10;				// The camera depth distance of popup layers
	protected int SHIELD_TOP_INDEX = 9;				// The index of shield top
	#endregion

	#region Delegate
	public delegate void OnScreenStartChangeDelegate(string sceneName);
	public delegate void OnSubScreenStartChangeDelegate(string sceneName);
	public delegate void OnSceneActivedDelegate (string sceneName);
	#endregion

	#region Event
	public OnScreenStartChangeDelegate onScreenStartChange;
	public OnSubScreenStartChangeDelegate onSubScreenStartChange;
	public OnSceneActivedDelegate onSceneActived;
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
	public static SSSceneManager Instance			// Singleton
	{
		get { return m_Instance; }
	}
	#endregion

	#region Protected Member
	protected Stack<string> 					m_Stack = new Stack<string>();					// Popup stack
	protected Stack<string> 					m_StackSub = new Stack<string>();				// Sub-screen stack
	protected Queue<SceneData> 					m_Queue = new Queue<SceneData>();				// Command queue
	protected List<GameObject> 					m_ListShield = new List<GameObject>();			// List Shield
	protected Dictionary<string, GameObject> 	m_Dict = new Dictionary<string, GameObject>();	// Dictionary of loaded scenes

	protected GameObject m_Scenes;			// Scene container object
	protected GameObject m_Shields;			// Shield container object
	protected GameObject m_Menu;			// Menu object
	protected GameObject m_ShieldTop;		// Shield top object
	protected GameObject m_Loading;			// Loading object
	protected GameObject m_LoadingBack;		// Loading back object
	protected GameObject m_SolidCamera;		// Solid camera object (Lowest camera)
	protected GameObject m_Res;				// Resource object

	protected int m_LoadingCount;			// Loading counter
	protected bool m_IsBusy;				// Busy when scene is loading or scene-animation is playing.
	protected string m_GlobalBgm;			// Global BGM
	#endregion

	#region Public Function
	/// <summary>
	/// Load or active a main-scene. All sub-scenes or popups which are showing will be deactive.
	/// </summary>
	/// <param name="sceneName">Scene name.</param>
	/// <param name="data">Data type is object type, allows any object.</param>
	/// <param name="onActive">OnActive callback.</param>
	/// <param name="onDeactive">OnDeactive callback.</param>
	public void Screen(string sceneName, object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
		string sn = sceneName;

		if (m_IsBusy) 
		{
			Enqueue(sn, data, onActive, onDeactive, SceneType.SCREEN);
			return;
		}

		if (IsSameScreen(sn)) 
		{
			Dequeue();
			return;
		}

		m_IsBusy = true;

		// Remove from stack and deactive
		while (m_Stack.Count > 0)
		{
			string p = m_Stack.Pop();

			CloseScene (p, true);

			ShieldOff();
		}

		// Remove all sub
		CloseAllSubScene ();

		// Raise event
		if (onScreenStartChange != null)
		{
			onScreenStartChange (sceneName);
		}

		IEScreen(sn, data, onActive, onDeactive);
	}
	
	/// <summary>
	/// Reset a main-scene. All sub-scenes or popups which are showing will be deactive.
	/// </summary>
	/// <param name="sceneName">Scene name.</param>
	/// <param name="data">Data type is object type, allows any object.</param>
	/// <param name="onActive">OnActive callback.</param>
	/// <param name="onDeactive">OnDeactive callback.</param>
	[System.Obsolete("ResetScreen is deprecated, please use Reset instead.")]
	public void ResetScreen(string sceneName, object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
		Reset (data, onActive, onDeactive);
	}

	/// <summary>
	/// Reset the current main-scene. All sub-scenes or popups which are showing will be deactive.
	/// </summary>
	/// <param name="data">Data type is object type, allows any object.</param>
	/// <param name="onActive">OnActive callback.</param>
	/// <param name="onDeactive">OnDeactive callback.</param>
	public void Reset(object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
		string sn = StackBottom ();

		if (!string.IsNullOrEmpty (sn)) 
		{
			if (m_IsBusy) 
			{
				Enqueue (sn, data, onActive, onDeactive, SceneType.RESET);
				return;
			}

			m_IsBusy = true;

			// Remove from stack and deactive
			while (m_Stack.Count > 0)
			{
				string p = m_Stack.Pop();

				bool isScreen = (m_Stack.Count == 0);
				ForceType force = (isScreen) ? ForceType.FORCE_DESTROY : ForceType.NO_FORCE;

				CloseScene (p, true, force);
				ShieldOff();
			}

			// Remove all sub
			CloseAllSubScene ();

			IEScreen(sn, data, onActive, onDeactive);
		}
	}

	/// <summary>
	/// Load or active a sub-scene. The current sub-scenes which are showing will be deactive.
	/// </summary>
	/// <param name="sceneName">Scene name.</param>
	/// <param name="data">Data type is object type, allows any object.</param>
	/// <param name="onActive">OnActive callback.</param>
	/// <param name="onDeactive">OnDeactive callback.</param>
	public void SubScreen(string sceneName, object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
		string sn = sceneName;

		if (m_IsBusy) 
		{
			Enqueue(sn, data, onActive, onDeactive, SceneType.SUB_SCREEN);
			return;
		}

		if (IsExistSub(sn))
		{
			Dequeue();
			return;
		}

		m_IsBusy = true;

		// Raise event
		if (onSubScreenStartChange != null)
		{
			onSubScreenStartChange (sceneName);
		}

		IESubScreen(sn, data, onActive, onDeactive);
	}

	/// <summary>
	/// Load or active a popup.
	/// </summary>
	/// <param name="sceneName">Scene name.</param>
	/// <param name="data">Data type is object type, allows any object.</param>
	/// <param name="onActive">OnActive callback.</param>
	/// <param name="onDeactive">OnDeactive callback.</param>
	public void PopUp(string sceneName, object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
		string sn = sceneName;

		if (m_IsBusy) 
		{
			Enqueue(sn, data, onActive, onDeactive, SceneType.POPUP);
			return;
		}

		// If same popup
		if (IsPopUpShowed(sn)) 
		{
			bool isAddingData = IsAddingData (data);

			if (isAddingData)
			{
				GameObject sc = m_Dict [sn];
				SSController ct = sc.GetComponentInChildren<SSController>();

				if (ct != null)
				{
					ct.OnSetAdding (data);
				}

				Dequeue ();
			}
			else
			{
				Enqueue(sn, data, onActive, onDeactive, SceneType.POPUP);
			}

			return;
		}

		// Check is wait no popup
		bool isWaitNoPopUp = IsWaitNoPopUp (data);

		if (isWaitNoPopUp && m_Stack.Count >= 2)
		{
			Enqueue(sn, data, onActive, onDeactive, SceneType.POPUP);
			return;
		}

		m_IsBusy = true;

		IEPopUp(sn, data, onActive, onDeactive);
	}

	/// <summary>
	/// Load a menu (only once).
	/// </summary>
	/// <param name="sceneName">Scene name.</param>
	/// <param name="data">Data type is object type, allows any object.</param>
	/// <param name="onActive">OnActive callback.</param>
	/// <param name="onDeactive">OnDeactive callback.</param>
	public void LoadMenu(string sceneName, object data = null, SSCallBackDelegate onActive = null, SSCallBackDelegate onDeactive = null)
	{
		string sn = sceneName;

		if (m_IsBusy) 
		{
			Enqueue(sn, data, onActive, onDeactive, SceneType.MENU);
			return;
		}

		if (m_Menu != null) 
		{
			ShowMenu ();
			Dequeue();
			return;
		}

		m_IsBusy = true;

		IEMenu(sn, data, onActive, onDeactive);
	}

	/// <summary>
	/// Shows the menu.
	/// </summary>
	public void ShowMenu()
	{
		if (m_Menu != null) 
		{
			m_Menu.SetActive (true);
		}
	}

	/// <summary>
	/// Hides the menu.
	/// </summary>
	public void HideMenu()
	{
		if (m_Menu != null) 
		{
			m_Menu.SetActive (false);
		}
	}

	/// <summary>
	/// Shows the loading indicator
	/// </summary>
	/// <param name="alpha">Alpha of shield.</param>
	/// <param name="timeOut">Time out.</param>
	/// <param name="callBack">Call back.</param>
	public void ShowLoading(float alpha = 0.5f, float timeOut = 0, NoParamCallback callBack = null)
	{
		if (m_Loading == null) return;

		ShieldTopOn (alpha);
		m_Loading.SetActive (true);

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
		if (m_Loading == null || !m_Loading.activeInHierarchy) return;

		m_LoadingCount--;
		
		if (m_LoadingCount == 0 || isForceHide) 
		{
			m_LoadingCount = 0;
			ShieldTopOff ();
			m_Loading.SetActive (false);
		}

		StopCoroutine ("IEHideLoading");
	}

	/// <summary>
	/// Closes all sub scene.
	/// </summary>
	public void CloseAllSubScene()
	{
		while (m_StackSub.Count > 0) 
		{
			string top = m_StackSub.Pop ();

			if (!string.IsNullOrEmpty (top)) 
			{
				CloseScene (top, true);
			}
		}
	}

	/// <summary>
	/// Close sub.
	/// </summary>
	public void CloseSub(bool immediate = false)
	{
		if (m_IsBusy) 
		{
			Enqueue(null, immediate, null, null, SceneType.CLOSE_SUB);
			return;
		}

		m_IsBusy = true;

		StartCoroutine(IECloseSub(immediate));
	}

	/// <summary>
	/// Close the top popup. If has no popup, quit app.
	/// </summary>
	/// <param name="immediate">If set to <c>true</c> immediate, close popup without animation.</param>
	public void Close(bool immediate = false)
	{
		if (m_IsBusy) 
		{
			Enqueue(null, immediate, null, null, SceneType.CLOSE);
			return;
		}

		m_IsBusy = true;

		StartCoroutine(IEClose(immediate));
	}

	/// <summary>
	/// Load the res scene.
	/// </summary>
	/// <param name="sceneName">Scene name.</param>
	public void LoadRes(string sceneName)
	{
		string sn = sceneName;

		if (m_IsBusy) 
		{
			Enqueue(sn, null, null, null, SceneType.RES);
			return;
		}

		if (m_Res != null) 
		{
			Dequeue();
			return;
		}

		m_IsBusy = true;

		IERes (sn);
	}

	/// <summary>
	/// Unload the res scene.
	/// </summary>
	/// <param name="sceneName">Scene name.</param>
	public void UnloadRes(string sceneName)
	{
		string sn = sceneName;

		GameObject sc = m_Dict[sn];

		m_Dict.Remove(sn);
		OnSceneUnload (sc);
		Destroy(sc);
	}

	/// <summary>
	/// Determine whether the scene is active.
	/// </summary>
	/// <returns><c>true</c> if the scene is active; otherwise, <c>false</c>.</returns>
	/// <param name="sceneName">Scene name.</param>
	public bool IsSceneActive(string sceneName)
	{
		string sn = sceneName;

		if (!m_Dict.ContainsKey (sn))
		{
			return false;
		}

		GameObject sc = m_Dict[sn];

		return sc.activeInHierarchy;
	}

	/// <summary>
	/// Determine whether the scene is in queue.
	/// </summary>
	/// <returns><c>true</c> if the scene is in queue; otherwise, <c>false</c>.</returns>
	/// <param name="sceneName">Scene name.</param>
	public bool IsSceneInQueue(string sceneName)
	{
		string sn = sceneName;

		foreach (var scene in m_Queue)
		{
			if (string.Compare (sn, scene.SceneName) == 0)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Determine whether the scene is in stack.
	/// </summary>
	/// <returns><c>true</c> if the scene is in stack; otherwise, <c>false</c>.</returns>
	/// <param name="sceneName">Scene name.</param>
	public bool IsSceneInStack(string sceneName)
	{
		string sn = sceneName;

		return m_Stack.Contains (sn);
	}

	/// <summary>
	///  Determine whether the scene is active or in queue.
	/// </summary>
	/// <returns><c>true</c> if the scene is active & in queue; otherwise, <c>false</c>.</returns>
	/// <param name="sceneName">Scene name.</param>
	public bool IsSceneActiveOrInQueue(string sceneName)
	{
		string sn = sceneName;

		return (IsSceneActive (sn) || (IsSceneInQueue (sn)));
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

	/// <summary>
	/// Raises the lock event. When a popup is showed, the below scene will be locked. Override if necessary.
	/// </summary>
	/// <param name="scene">Scene which be locked</param>
	protected virtual void OnLock(GameObject scene)
	{
		SSController[] ctrls = scene.GetComponentsInChildren<SSController>(true);
		foreach (SSController ctrl in ctrls)
		{
			ctrl.OnLock();
		}
	}

	/// <summary>
	/// Raises the unlock event. When a popup is hided, the below scene will be unlocked. Override if necessary.
	/// </summary>
	/// <param name="scene">Scene which be unlocked</param>
	protected virtual void OnUnlock(GameObject scene)
	{
		SSController[] ctrls = scene.GetComponentsInChildren<SSController>(true);
		foreach (SSController ctrl in ctrls)
		{
			ctrl.OnUnlock();
		}
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
	/// Default first scene is 'First Scene Name', you can override this function.
	/// </summary>
	protected virtual void OnFirstSceneLoad()
	{
		Screen (m_FirstSceneName);
	}

	#if UNITY_EDITOR || UNITY_ANDROID
	protected virtual void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape) && !m_IsBusy) 
		{
			string sn = StackTop ();
			GameObject sc = m_Dict [sn];
			SSController ct = sc.GetComponentInChildren<SSController>();

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
	private void CreateLoadingsThenLoadFirstScene()
	{
		m_IsBusy = true;

		if (string.IsNullOrEmpty (m_LoadingSceneName))
		{
			AfterLoading ();
			return;
		}

		if (m_LoadingBack == null) 
		{
			SSApplication.LoadLevelAdditive (m_LoadingSceneName, m_IsLoadAsync, (GameObject root) =>
			{
				CreateLoadingBack(root);

				CreateLoadingTop();

				AfterLoading();
			});
		}
	}

	private void CreateLoadingBack(GameObject root)
	{
		m_LoadingBack = root;
		m_LoadingBack.name = m_LoadingBack.name + "Back";

		SetPosition (m_LoadingBack, -SHIELD_TOP_INDEX);
		SetCameras (m_LoadingBack, -SHIELD_TOP_INDEX);

		m_LoadingBack.SetActive (false);
	}

	private void CreateLoadingTop()
	{
		if (m_Loading == null) 
		{
			if (m_LoadingBack != null) 
			{
				m_Loading = Instantiate (m_LoadingBack) as GameObject;
				m_Loading.name = m_LoadingBack.name.Replace ("Back", "Top");

				SSRoot root = m_Loading.GetComponent<SSRoot> ();
				if (root == null)
				{
					Debug.LogWarning ("You must to attach SSRoot to the root object of loading scene!");
				}
				else
				{
					root.PreventLoadCallBack = true;
				}

				SetPosition (m_Loading, SHIELD_TOP_INDEX);
				SetCameras (m_Loading, SHIELD_TOP_INDEX);

				m_Loading.SetActive (false);
			}
		}
	}

	private void AfterLoading()
	{
		m_IsBusy = false;

		if (!string.IsNullOrEmpty (m_FirstSceneName)) 
		{
			OnFirstSceneLoad ();
		}

		Dequeue ();
	}

	private void LoadScene(string sn, NoParamCallback onLoaded)
	{
		if (m_Dict.ContainsKey(sn))
		{
			ActiveScene (sn);

			onLoaded ();
		}
		else
		{
			SSApplication.LoadLevelAdditive (sn, m_IsLoadAsync, (GameObject root) =>
			{
				GameObject scene = root;

				m_Dict.Add(sn, scene);

				scene.transform.parent = m_Scenes.transform;

				OnSceneLoad (scene);

				onLoaded ();
			});
		}
	}

	private void ActiveScene(string sn)
	{
		m_Dict[sn].SetActive(true);

		// Animation
		SSMotion an = m_Dict[sn].GetComponentInChildren<SSMotion>();
		if (an != null)
		{
			// We should bring this scene to somewhere far when re-active it.
			// Then the animation will automatically bring it back at next frame.
			// This trick remove flicker at the first frame.
			an.transform.localPosition = new Vector3(99999, 0, 0);
			an.transform.localScale = Vector3.one;
		}
	}

	private void DeactiveOrDestroy(string sn, ForceType force = ForceType.NO_FORCE)
	{
		bool isCache = true;

		GameObject sc = m_Dict[sn];
		SSController ct = sc.GetComponentInChildren<SSController>();

		// Set Event
		if (ct != null)
		{
			if (ct.OnDeactive != null) ct.OnDeactive(ct);
		}

		sc.SetActive(false);

		if (ct != null)
		{
			isCache = ct.IsCache;
		}

		if (force != ForceType.FORCE_NO_DESTROY) 
		{
			if (!isCache || force == ForceType.FORCE_DESTROY)
			{
				m_Dict.Remove(sn);
				OnSceneUnload (sc);
				Destroy(sc);
			}
		}
	}

	/// <summary>
	/// Hides the sub back.
	/// </summary>
	private void HideSubBack(NoParamCallback callback = null)
	{
		if (m_StackSub.Count > 0) 
		{
			string top = m_StackSub.Peek ();

			if (!string.IsNullOrEmpty (top)) 
			{
				CloseScene (top, false, ForceType.FORCE_NO_DESTROY, AnimType.HIDE_BACK, callback);
			}
		}
	}

	private void CloseScene(string sn, bool immediate, ForceType force = ForceType.NO_FORCE, AnimType animType = AnimType.HIDE, NoParamCallback callback = null)
	{
		// Event
		SSController ct = m_Dict[sn].GetComponentInChildren<SSController>();
		if (ct != null)
		{
			ct.OnHide();
		}

		// Play Animation
		if (!immediate)
		{
			StartCoroutine( IEPlayAnimation (sn, animType, () => 
			{
				// Deactive or Destroy
				DeactiveOrDestroy(sn, force);

				if (callback != null)
					callback();
			}));
		}
		else
		{
			// Deactive or Destroy
			DeactiveOrDestroy(sn, force);

			if (callback != null)
				callback();
		}
	}

	private GameObject CreateShield(int i)
	{
		// Instantiate from resources
		GameObject sh = Instantiate(Resources.Load("Shield")) as GameObject;
		sh.name = "Shield" + i;
		sh.transform.localPosition = new Vector3((i+0.5f) * SCENE_DISTANCE, 0, 0);
		sh.transform.parent = m_Shields.transform;

		// Set camera depth
		Camera c = sh.GetComponentInChildren<Camera>();
		c.depth = (i+1) * DEPTH_DISTANCE;

		return sh;
	}

	private void ShieldTopOn(float alpha)
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

	private void ShieldTopOff()
	{
		if (m_ShieldTop != null) 
		{
			m_ShieldTop.SetActive (false);

			// Unlock
			UnlockTopScene ();
		}
	}

	private void ShieldOn(int i)
	{
		ShieldOn (i, m_DefaultShieldColor);
	}

	private void ShieldOn(int i, Color color)
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

	private bool IsShieldActive(int i)
	{
		if (i >= m_ListShield.Count)
			return false;

		return (m_ListShield [i].activeInHierarchy);
	}

	private void ShieldOff()
	{
		int i = m_Stack.Count - 1;

		if (i < 0) return;

		m_ListShield[i].SetActive(false);

		// Unlock
		UnlockTopScene ();
	}

	private void LockTopScene()
	{
		// Check if has no scene in stack
		if (m_Stack.Count == 0)
			return;

		// Lock below scene
		GameObject scene = m_Dict[m_Stack.Peek()];
		OnLock(scene);

		// Lock Menu same time lock screen
		if (m_Stack.Count == 1 && m_Menu != null)
		{
			OnLock(m_Menu);
		}
	}

	private void UnlockTopScene()
	{
		// Check if has no scene in stack
		if (m_Stack.Count == 0)
			return;

		if (!m_Dict.ContainsKey (m_Stack.Peek())) return;

		// Unlock below scene
		GameObject scene = m_Dict[m_Stack.Peek()];
		OnUnlock(scene);

		// Unlock Menu same time unlock screen
		if (m_Stack.Count == 1 && m_Menu != null)
		{
			OnUnlock(m_Menu);
		}
	}

	private void SetPosition(string sn, int i)
	{
		GameObject sc = m_Dict[sn];

		SetPosition (sc, i);
	}

	private void SetPosition(GameObject sc, int i)
	{
		sc.transform.localPosition = new Vector3(i * SCENE_DISTANCE, SCENE_DISTANCE, 0);
	}

	private void SetCameras(string sn, float i)
	{
		GameObject sc = m_Dict[sn];

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

	private string StackBottom()
	{
		string r = null;
		foreach (string s in m_Stack)
		{
			r = s;
		}
		return r;
	}

	private string StackTop()
	{
		if (m_Stack.Count == 0) return null;
		return m_Stack.Peek();
	}

	private bool IsSameScreen(string sn)
	{
		string b = StackBottom();

		if (string.IsNullOrEmpty(b)) return false;
		if (string.Compare(b, sn) == 0) return true;

		return false;
	}

	private bool IsExistSub(string sn)
	{
		return m_StackSub.Contains (sn);
	}

	private bool IsTopSub(string sn)
	{
		if (m_StackSub.Count > 0) 
		{
			if (m_StackSub.Peek () == sn)
				return true;
		}

		return false;
	}

	private bool IsPopUpShowed(string sn)
	{
		foreach (string s in m_Stack)
		{
			if (string.Compare(s, sn) == 0) return true;
		}

		return false;
	}

	private void BgmSceneOpen(string curBgm, SSController ctrl)
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
					break;
				case Bgm.CUSTOM:
					StopBGM ();
					ctrl.CurrentBgm = ctrl.BgmName;
					break;
			}
		}
	}

	private void BgmSceneClose(SSController ctrl)
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

	private void ShowLoadingBack()
	{
		if (m_LoadingBack != null) 
		{
			m_LoadingBack.SetActive (true);
		}
	}

	private void HideLoadingBack()
	{
		if (m_LoadingBack != null) 
		{
			m_LoadingBack.SetActive (false);
		}
	}

	private void IECommon(string sn, int i, object data, SSCallBackDelegate onActive, SSCallBackDelegate onDeactive, SceneType type, bool isInStack = true)
	{
		IECommon (sn, i, i, data, onActive, onDeactive, type, isInStack);
	}

	private void IECommon(string sn, int ip, float ic, object data, SSCallBackDelegate onActive, SSCallBackDelegate onDeactive, SceneType type, bool isInStack = true)
	{
		// Defaut BGM
		string curBgm = string.Empty;

		// Active Shield
		ShieldOn(ip-1);

		// Focus lost
		if (isInStack && m_Stack.Count > 0)
		{
			string s = m_Stack.Peek();
			SSController c = m_Dict[s].GetComponentInChildren<SSController>();
			if (c != null)
			{
				curBgm = c.CurrentBgm;
				c.OnFocusLost();
			}
		}

		// Load or active scene
		LoadScene (sn, () =>
		{
			// Set Position
			SetPosition(sn, ip);

			// Set Cameras
			SetCameras(sn, ic);

			if (isInStack)
			{
				// Add to Stack
				m_Stack.Push(sn);
			}

			// Set event & data
			SSController ct = m_Dict[sn].GetComponentInChildren<SSController>();
			if (ct != null)
			{
				ct.OnActive = onActive;
				ct.OnDeactive = onDeactive;

				if (ct.OnActive != null) ct.OnActive(ct);

				ct.OnSet(data);
			}

			// Active Empty Shield
			ShieldOn (ip, new Color(0, 0, 0, 0));

			// Play if has animation
			StartCoroutine(IEPlayAnimation(sn, AnimType.SHOW, () =>
			{
				// Deactive Empty Shield
				ShieldOff ();

				// Event
				if (ct != null)
				{
					if (isInStack)
					{
						BgmSceneOpen(curBgm, ct);
					}
					ct.OnShow();
				}

				// Busy off
				m_IsBusy = false;

				// Set something by type
				SetByType(sn, type);

				// Event
				if (onSceneActived != null)
				{
					onSceneActived(sn);
				}

				// Check queue
				Dequeue();
			}));
		});
	}

	private void IECommonSub(string sn, int ip, float ic, object data, SSCallBackDelegate onActive, SSCallBackDelegate onDeactive, SceneType type, bool isInStack = true)
	{
		// Defaut BGM
		string curBgm = string.Empty;

		// Active Shield
		bool isShieldActived = IsShieldActive (0);

		// Active Empty shield
		if (!isShieldActived)
			ShieldOn (0, new Color(0, 0, 0, 0));

		// Load or active scene
		LoadScene (sn, () =>
		{
			// Focus lost
			if (isInStack && m_StackSub.Count > 0)
			{
				string s = m_StackSub.Peek();
				SSController c = m_Dict[s].GetComponentInChildren<SSController>();
				if (c != null)
				{
					curBgm = c.CurrentBgm;
					c.OnFocusLost();
				}
			}

			// Hide Stack top
			HideSubBack();

			// Set Position
			SetPosition(sn, ip);

			// Set Cameras
			SetCameras(sn, ic);

			if (isInStack)
			{
				// Add to Stack
				m_StackSub.Push(sn);
			}

			// Set event & data
			SSController ct = m_Dict[sn].GetComponentInChildren<SSController>();
			if (ct != null)
			{
				ct.OnActive = onActive;
				ct.OnDeactive = onDeactive;

				if (ct.OnActive != null) ct.OnActive(ct);

				ct.OnSet(data);
			}

			// No animation if is first sub-scene of stack
			AnimType animType = (m_StackSub.Count == 1) ? AnimType.NO_ANIM : AnimType.SHOW;

			// Play if has animation
			StartCoroutine(IEPlayAnimation(sn, animType, () =>
			{
				// Deactive Empty Shield
				if (!isShieldActived)
					ShieldOff ();

				// Event
				if (ct != null)
				{
					if (isInStack)
					{
						BgmSceneOpen(curBgm, ct);
					}
					ct.OnShow();
				}

				// Busy off
				m_IsBusy = false;

				// Set something by type
				SetByType(sn, type);

				// Event
				if (onSceneActived != null)
				{
					onSceneActived(sn);
				}

				// Check queue
				Dequeue();
			}));
		});
	}

	private IEnumerator IEPlayAnimation(string sn, AnimType animType, NoParamCallback callback = null)
	{
		SSMotion an = m_Dict[sn].GetComponentInChildren<SSMotion>();
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

			if (callback != null)
				callback ();
		}
		else
		{
			if (animType == AnimType.NO_ANIM) 
			{
				an.transform.localPosition = Vector3.zero;
			}

			if (callback != null)
				callback ();
		}
	}

	private void IEScreen(string sn, object data, SSCallBackDelegate onActive, SSCallBackDelegate onDeactive)
	{
		// Show system loading
		ShowLoadingBack ();

		// Common
		IECommon(sn, 0, data, onActive, onDeactive, SceneType.SCREEN);
	}

	private void IEPopUp(string sn, object data, SSCallBackDelegate onActive, SSCallBackDelegate onDeactive)
	{
		// Count
		int c = m_Stack.Count;

		// Next index
		int ni = c;

		// Common
		IECommon(sn, ni, data, onActive, onDeactive, SceneType.POPUP);
	}

	private void IEMenu(string sn, object data, SSCallBackDelegate onActive, SSCallBackDelegate onDeactive)
	{
		// Common
		IECommon(sn, -1, 0.8f, data, onActive, onDeactive, SceneType.MENU, false);
	}

	private void IESubScreen(string sn, object data, SSCallBackDelegate onActive, SSCallBackDelegate onDeactive)
	{
		// Common
		IECommonSub(sn, -2 - m_StackSub.Count, 0.3f + (float)m_StackSub.Count / DEPTH_DISTANCE, data, onActive, onDeactive, SceneType.SUB_SCREEN, true);
	}

	private IEnumerator IEClose(bool immediate)
	{
		// Nothing to close
		if (m_Stack.Count == 0) 
		{
			m_IsBusy = false;
			yield break;
		}

		// Lowest layer: Quit
		if (m_Stack.Count == 1) 
		{
			m_IsBusy = false;
			Debug.Log("Application.Quit");
			Application.Quit();
			yield break;
		}

		// Active Empty shield
		ShieldOn (m_Stack.Count - 1, new Color(0, 0, 0, 0));

		// Stack peek
		string sn = m_Stack.Peek();

		// Deactive Scene
		CloseScene (sn, immediate, ForceType.NO_FORCE, AnimType.HIDE, () => 
		{
			// Deactive Empty shield
			ShieldOff();

			// Stack pop
			sn = m_Stack.Pop();

			// Focus back
			if (m_Stack.Count > 0)
			{
				string s = m_Stack.Peek();
				SSController c = m_Dict[s].GetComponentInChildren<SSController>();
				if (c != null)
				{
					BgmSceneClose(c);
					c.OnFocusBack();
				}
			}

			// Deactive Shield
			ShieldOff();

			// Busy off
			m_IsBusy = false;

			// Event
			if (m_Stack.Count > 0)
			{
				string s = m_Stack.Peek();
				if (onSceneActived != null)
				{
					onSceneActived (s);
				}
			}

			// Check queue
			Dequeue();
		});
	}

	private IEnumerator IECloseSub(bool immediate)
	{
		// Nothing to close
		if (m_StackSub.Count == 0) 
		{
			m_IsBusy = false;
			yield break;
		}

		// Lowest layer: Quit
		if (m_StackSub.Count == 1) 
		{
			m_IsBusy = false;
			Debug.Log("Application.Quit");
			Application.Quit();
			yield break;
		}

		// Current state of shield 0
		bool isShieldActived = IsShieldActive (0);

		// Active Empty shield
		ShieldOn (0, new Color(0, 0, 0, 0));

		// Stack pop
		string sn = m_StackSub.Pop();

		// Deactive Scene
		CloseScene (sn, immediate, ForceType.NO_FORCE, AnimType.HIDE, () => 
		{
			// Deactive Empty shield
			if (!isShieldActived) 
			{
				ShieldOff ();
			}

			// Focus back
			if (m_StackSub.Count > 0)
			{
				string s = m_StackSub.Peek();
				SSController c = m_Dict[s].GetComponentInChildren<SSController>();
				if (c != null)
				{
					BgmSceneClose(c);
					c.OnFocusBack();
				}
			}

			// Busy off
			m_IsBusy = false;

			// Event
			if (m_StackSub.Count > 0)
			{
				string s = m_StackSub.Peek();
				if (onSceneActived != null)
				{
					onSceneActived (s);
				}
			}

			// Check queue
			Dequeue();
		});

		// Top of stack
		sn = m_StackSub.Peek();

		// Active scene
		ActiveScene (sn);
		StartCoroutine (IEPlayAnimation (sn, AnimType.SHOW_BACK));
	}

	private void IERes(string sn)
	{
		LoadScene (sn, () =>
		{
			// Busy off
			m_IsBusy = false;

			// Set something by type
			SetByType(sn, SceneType.RES);

			// Check queue
			Dequeue();
		});
	}

	private void SetByType(string sn, SceneType type)
	{
		switch (type) 
		{
			case SceneType.SCREEN:
				HideLoadingBack ();
				break;
			case SceneType.SUB_SCREEN:
				break;
			case SceneType.POPUP:
				break;
			case SceneType.MENU:
				m_Menu = m_Dict[sn];
				break;
			case SceneType.CLOSE:
				break;
			case SceneType.CLOSE_SUB:
				break;
			case SceneType.RESET:
				HideLoadingBack ();
				break;
			case SceneType.RES:
				m_Res = m_Dict [sn];
				m_Res.transform.localPosition = new Vector3 (99999, 0, 0);
				break;
			default:
				break;
		}
	}

	private void Enqueue(string sceneName, object data, SSCallBackDelegate onActive, SSCallBackDelegate onDeactive, SceneType type)
	{
		m_Queue.Enqueue(new SceneData(sceneName, data, onActive, onDeactive, type));
	}

	private void Dequeue()
	{
		if (m_Queue.Count == 0) return;

		// Check if this scene is popup
		SceneData sd = m_Queue.Peek ();
		if (sd.Type == SceneType.POPUP) 
		{
			// Check if scene was showed already
			if (IsPopUpShowed (sd.SceneName)) 
			{
				// Check if is adding data
				bool isAddingData = IsAddingData (sd.Data);

				if (!isAddingData)
				{
					Debug.Log ("Wait Close: Close current popup to do next command");
					return;
				}
			}

			// Check if wait no popup
			bool isWaitNoPopUp = IsWaitNoPopUp (sd.Data);

			// Check if wait no popup && has popup now
			if (isWaitNoPopUp && m_Stack.Count >= 2)
			{
				return;
			}
		}

		// Dequeue
		sd = m_Queue.Dequeue();
		switch (sd.Type) 
		{
		case SceneType.SCREEN:
				Screen(sd.SceneName, sd.Data, sd.OnActive, sd.OnDeactive);
				break;
		case SceneType.SUB_SCREEN:
				SubScreen(sd.SceneName, sd.Data, sd.OnActive, sd.OnDeactive);
				break;
		case SceneType.POPUP:
				PopUp(sd.SceneName, sd.Data, sd.OnActive, sd.OnDeactive);
				break;
		case SceneType.MENU:
				LoadMenu(sd.SceneName, sd.Data, sd.OnActive, sd.OnDeactive);
				break;
		case SceneType.CLOSE:
				Close((bool)sd.Data);
				break;
		case SceneType.CLOSE_SUB:
				CloseSub((bool)sd.Data);
				break;
		case SceneType.RESET:
				Reset(sd.Data, sd.OnActive, sd.OnDeactive);
				break;
		case SceneType.RES:
				LoadRes(sd.SceneName);
				break;
		}
	}

	private bool IsAddingData(object data)
	{
		bool isAddingData = false;

		if (data != null && data.GetType() == typeof(PopUpData))
		{
			PopUpData popUpData = (PopUpData)data;

			if (popUpData != null)
			{
				isAddingData = popUpData.IsAddData;
			}
		}

		return isAddingData;
	}

	private bool IsWaitNoPopUp(object data)
	{
		bool isWaitNoPopUp = false;

		if (data != null && data.GetType() == typeof(PopUpData))
		{
			PopUpData popUpData = (PopUpData)data;

			if (popUpData != null)
			{
				isWaitNoPopUp = popUpData.IsWaitNoPopUp;
			}
		}

		return isWaitNoPopUp;
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
	
	#endregion
	
}
