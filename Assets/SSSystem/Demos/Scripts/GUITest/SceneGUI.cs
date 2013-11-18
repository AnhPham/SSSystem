using UnityEngine;
using System.Collections;

public class SceneGUI : MonoBehaviour 
{
	private bool isLoadedMenu;

	protected virtual void OnGUIExtend()
	{
	}

	private void OnGUI()
	{
		GUILayout.BeginVertical ();

		OnGUIExtend ();

		if (GUILayout.Button("Screen 1"))
		{
			ScreenS1();
		}

		if (GUILayout.Button("Screen 2"))
		{
			ScreenS2();
		}

		if (GUILayout.Button("Load Menu"))
		{
			Menu();
		}

		if (GUILayout.Button("Sub 1"))
		{
			SubScreenS1();
		}

		if (GUILayout.Button("Sub 2"))
		{
			SubScreenS2();
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

		if (GUILayout.Button("Close Sub"))
		{
			CloseSub();
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

		if (GUILayout.Button("Screen 1 + Sub 1 + Menu"))
		{
			ShowLoadingBlack ();
			ScreenS1();
			SubScreenS1();
			Menu ();
		}

		if (GUILayout.Button("PopUp 1 & 2"))
		{
			PopUpP1();
			PopUpP2();
		}

		if (GUILayout.Button("Screen 2"))
		{
			ScreenS2();
		}

		if (GUILayout.Button("Hide Menu"))
		{
			HideMenu();
		}

		GUILayout.Space (20);

		if (GUILayout.Button("SUPER"))
		{
			ShowLoadingBlack ();
			ScreenS1();
			SubScreenS1();
			Menu();
			PopUpP1();
			PopUpP2();
			Close();
			Close();
			SubScreenS2 ();
			ScreenS1();
			SubScreenS1();
			PopUpP1();
			PopUpP2();
			ScreenS2();
			SubScreenS2();
			PopUpP1();
			PopUpP2();
			Close();
			Close();
			ScreenS1();
			SubScreenS2();
			PopUpP1();
			PopUpP2();
		}

		if (GUILayout.Button("Same popup"))
		{
			ScreenS2();

			PopUpP1();
			PopUpP1();
			PopUpP1();
		}

		GUILayout.EndVertical ();
	}

	private void ShowLoading()
	{
		SSSceneManager.Instance.ShowLoading ();
	}

	private void ShowLoadingBlack()
	{
		if (isLoadedMenu) return;
		SSSceneManager.Instance.ShowLoading (1);
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

	private void CloseSub()
	{
		SSSceneManager.Instance.CloseSubScene();
	}

	private void ScreenS1()
	{
		SSSceneManager.Instance.Screen("S1");
	}

	private void ScreenS2()
	{
		SSSceneManager.Instance.Screen("S2");
	}

	private void SubScreenS1()
	{
		SSSceneManager.Instance.SubScreen("SS1");
	}

	private void SubScreenS2()
	{
		SSSceneManager.Instance.SubScreen("SS2");
	}

	private void Menu()
	{
		SSSceneManager.Instance.LoadMenu
		(
			"Menu",
			null, 
			(SSController ctrl) => 
			{
				isLoadedMenu = true;
				HideLoading();
			},
			(SSController ctrl) => 
			{

			}
		);
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

	private void OnP2LeftMouseClick()
	{
		Debug.Log("SceneGUITest: P2 Click");
	}
}
