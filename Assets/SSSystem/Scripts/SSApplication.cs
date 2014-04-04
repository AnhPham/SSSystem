using UnityEngine;
using System.Collections;

public class SSApplication
{
	public delegate void OnLoadedDelegate(GameObject root);

	private static OnLoadedDelegate m_OnLoaded;

	public static void LoadLevelAdditive(string sceneName, bool isAsync = false, OnLoadedDelegate onLoaded = null)
	{
		m_OnLoaded = onLoaded;

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
		if (m_OnLoaded != null)
		{
			m_OnLoaded (root);
		}
	}
}
