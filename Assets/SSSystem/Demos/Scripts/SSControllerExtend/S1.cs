using UnityEngine;
using System.Collections;

public class S1 : SSController 
{
	public override void Awake ()
	{
		BgmType = Bgm.PLAY;
		BgmName = "S1";

		IsCache = true;
	}

    public override void OnKeyBack()
    {
        SceneManagerDemo.Instance.GoHome();
    }

    public void OnButtonPopUp1Tap()
    {
        SceneManagerDemo.Instance.PopUp("P1");
    }

    public void OnButtonPopUp2Tap()
    {
        SceneManagerDemo.Instance.PopUp("P2");
    }

    public void OnButtonAddScreen3Tap()
    {
        SceneManagerDemo.Instance.AddScreen("S3");
    }
}
