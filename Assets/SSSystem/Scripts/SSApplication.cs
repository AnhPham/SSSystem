using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SSApplication
{
	public delegate void OnLoadedDelegate(GameObject root);

	private static Dictionary<string, OnLoadedDelegate> m_OnLoaded = new Dictionary<string, OnLoadedDelegate>();

    public static void LoadLevel(string sceneName, bool isAsync = false, bool isAdditive = false, OnLoadedDelegate onLoaded = null)
	{
		if (m_OnLoaded.ContainsKey (sceneName)) 
		{
			Debug.LogWarning ("Loaded this scene before. Please check again.");
			return;
		}

		m_OnLoaded.Add(sceneName, onLoaded);

		#if UNITY_5_2 || UNITY_5_1 || UNITY_5_0 || UNITY_4_6
		if (!isAsync)
		{
			if (isAdditive)
				Application.LoadLevelAdditive (sceneName);
			else
				Application.LoadLevel (sceneName);
		}
		else
		{
			if (isAdditive)
				Application.LoadLevelAdditiveAsync (sceneName);
			else
				Application.LoadLevelAsync (sceneName);
		}
		#else
		if (!isAsync)
		{
			if (isAdditive)
				UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
			else
				UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
		}
		else
		{
			if (isAdditive)
				UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
			else
				UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
		}
		#endif
	}

	public static void OnLoaded(GameObject root)
	{
		if (m_OnLoaded[root.name] != null)
		{
			m_OnLoaded[root.name] (root);
		}
	}

	public static void OnUnloaded(GameObject root)
	{
		if (m_OnLoaded.ContainsKey(root.name))
		{
			m_OnLoaded.Remove (root.name);
		}
	}
}
