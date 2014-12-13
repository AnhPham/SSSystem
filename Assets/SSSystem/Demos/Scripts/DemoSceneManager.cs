using UnityEngine;
using System.Collections;

public class DemoSceneManager : SSSceneManager
{
    protected override void OnFirstSceneLoad()
    {
        base.OnFirstSceneLoad();

        LoadMenu("DemoMenu");
    }
}
