using UnityEngine;
using System.Collections;

public class DemoSceneManager : SSSceneManager
{
    protected override void OnFirstSceneLoad()
    {
        base.OnFirstSceneLoad();

        Application.targetFrameRate = 60;

        LoadMenu("DemoMenu");
    }
}
