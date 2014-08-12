using UnityEngine;
using System.Collections;

public class Menu : SSController 
{
	public override void Awake()
	{
		BgmType = Bgm.SAME;
		BgmName = string.Empty;

		IsCache = true;
	}

    public void OnButtonScreen1Tap()
    {
        SceneManagerDemo.Instance.Screen("S1");
    }

    public void OnButtonScreen2Tap()
    {
        SceneManagerDemo.Instance.Screen("S2");
    }
}
