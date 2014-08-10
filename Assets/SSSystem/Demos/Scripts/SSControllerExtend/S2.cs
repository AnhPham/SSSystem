using UnityEngine;
using System.Collections;

public class S2 : SSController 
{
	public override void Awake ()
	{
		BgmType = Bgm.PLAY;
		BgmName = "S2";

		IsCache = true;
	}

	public override void OnSet (object data)
	{
		Debug.Log("Set data : S2");
	}

    public override void OnKeyBack()
    {
        SceneManagerDemo.Instance.GoHome();
    }
}
