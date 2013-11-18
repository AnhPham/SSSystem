using UnityEngine;
using System.Collections;

public class SceneGUITutorial : SceneGUI
{
	protected override void OnGUIExtend ()
	{
		base.OnGUIExtend ();

		if (GUILayout.Button("Tutorial On"))
		{
			SceneManagerTutorial.Inst.TutorialOn ();
		}

		if (GUILayout.Button("Tutorial Off"))
		{
			SceneManagerTutorial.Inst.TutorialOff ();
		}
	}
}
