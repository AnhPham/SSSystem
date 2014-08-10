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
}
