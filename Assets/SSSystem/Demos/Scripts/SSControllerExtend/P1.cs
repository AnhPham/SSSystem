using UnityEngine;
using System.Collections;

public class P1 : SSController 
{
	public override void Awake ()
	{
		BgmType = Bgm.SAME;
		BgmName = string.Empty;

		IsCache = false;
	}

    public void OnButtonCloseTap()
    {
        SceneManagerDemo.Instance.Close();
    }

    public void OnButtonPopUp2Tap()
    {
        SceneManagerDemo.Instance.PopUp("P2");
    }
}
