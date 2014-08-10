using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SSApplication
{
	public delegate void OnLoadedDelegate(GameObject root);

	private static Dictionary<string, OnLoadedDelegate> m_OnLoaded = new Dictionary<string, OnLoadedDelegate>();

	public static void LoadLevelAdditive(string sceneName, bool isAsync = false, OnLoadedDelegate onLoaded = null)
	{
		if (m_OnLoaded.ContainsKey (sceneName)) 
		{
			Debug.LogWarning ("Loaded this scene before. Please check again.");
			return;
		}

		m_OnLoaded.Add(sceneName, onLoaded);

		if (!isAsync)
		{
			Application.LoadLevelAdditive (sceneName);
		}
		else
		{
			Application.LoadLevelAdditiveAsync (sceneName);
		}
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
