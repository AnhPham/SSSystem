using UnityEngine;
using System.Collections;

public class P2 : SSController 
{
	public override void Awake ()
	{
		BgmType = Bgm.NONE;
		BgmName = string.Empty;

		IsCache = false;
	}

    public void OnButtonCloseTap()
    {
        SceneManagerDemo.Instance.Close();
    }

    public void OnButtonPopUp1Tap()
    {
        SceneManagerDemo.Instance.PopUp("P1");
    }
}
