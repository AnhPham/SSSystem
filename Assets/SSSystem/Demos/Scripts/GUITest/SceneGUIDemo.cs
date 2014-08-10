using UnityEngine;
using System.Collections;

public class SceneGUIDemo : MonoBehaviour 
{
	protected virtual void OnGUIExtend()
	{
	}

	private void OnGUI()
	{
		GUILayout.BeginVertical ();

		OnGUIExtend ();

		if (GUILayout.Button("Screen 1_1"))
		{
			ScreenS1();
		}

        if (GUILayout.Button("Add Screen 1_2"))
        {
            AddScreen12();
        }

		if (GUILayout.Button("Screen 2_1"))
		{
			ScreenS2();
		}

        if (GUILayout.Button("Add Screen 2_2"))
        {
            AddScreen22();
        }

		if (GUILayout.Button("Load Menu"))
		{
			Menu();
		}

		if (GUILayout.Button("PopUp 1"))
		{
			PopUpP1();
		}

		if (GUILayout.Button("PopUp 2"))
		{
			PopUpP2();
		}

		if (GUILayout.Button("Close"))
		{
			Close();
		}

        if (GUILayout.Button("Close"))
        {
            Close();
        }

		if (GUILayout.Button("Hide Menu"))
		{
			HideMenu();
		}

		if (GUILayout.Button ("Loading On")) 
		{
			ShowLoading ();
		}

		if (GUILayout.Button ("Loading Off")) 
		{
			HideLoading ();
		}

		GUILayout.Space (20);

		if (GUILayout.Button("PopUp 1 & 2"))
		{
			PopUpP1();
			PopUpP2();
		}

		if (GUILayout.Button("Screen 1_2"))
		{
			ScreenS1(); // Current screen of this stack is Screen 1_2
		}

        if (GUILayout.Button("PopUp1 - Close"))
		{
			PopUpP1();
			Close ();
		}

		if (GUILayout.Button("Double PopUp2"))
		{
			PopUpP2();
			PopUpP2();
		}

        if (GUILayout.Button("Close"))
        {
            Close();
        }

        if (GUILayout.Button("Close"))
        {
            Close();
        }

        if (GUILayout.Button("Reset"))
        {
            Reset(); // Main screen of this stack is Screen 1_1
        }

		GUILayout.EndVertical ();
	}

	private void ShowLoading()
	{
		SSSceneManager.Instance.ShowLoading ();
	}

	private void HideLoading()
	{
		SSSceneManager.Instance.HideLoading ();
	}

	private void ShowMenu()
	{
		SSSceneManager.Instance.ShowMenu();
	}

	private void HideMenu()
	{
		SSSceneManager.Instance.HideMenu();
	}

	private void Close()
	{
		SSSceneManager.Instance.Close();
	}

	private void ScreenS1()
	{
		SSSceneManager.Instance.Screen("S1");
	}

	private void ScreenS2()
	{
		SSSceneManager.Instance.Screen("S2");
	}

	private void AddScreen12()
	{
		SSSceneManager.Instance.AddScreen("SS1");
	}

	private void AddScreen22()
	{
		SSSceneManager.Instance.AddScreen("SS2");
	}

	private void Menu()
	{
        SSSceneManager.Instance.LoadMenu("Menu");
	}

	private void PopUpP1()
	{
		SSSceneManager.Instance.PopUp("P1", "One");
	}

	private void PopUpP2()
	{
		SSSceneManager.Instance.PopUp
		(
			"P2",
			"Two", 
			(SSController ctrl) => 
			{
				P2 p2 = (P2)ctrl;
				p2.LeftMouseClick += OnP2LeftMouseClick;
			},
			(SSController ctrl) => 
			{
				P2 p2 = (P2)ctrl;
				p2.LeftMouseClick -= OnP2LeftMouseClick;
			}
		);
	}

    private void Reset()
    {
        SSSceneManager.Instance.Reset();
    }

	private void OnP2LeftMouseClick()
	{
		Debug.Log("SceneGUITest: P2 Click");
	}
}
