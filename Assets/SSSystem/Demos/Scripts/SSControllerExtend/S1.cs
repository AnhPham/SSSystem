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

	public override void OnSet (object data)
	{
		Debug.Log("Set data : S1");
	}

    public override void OnKeyBack()
    {
        SceneManagerDemo.Instance.GoHome();
    }
}
