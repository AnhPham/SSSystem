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
