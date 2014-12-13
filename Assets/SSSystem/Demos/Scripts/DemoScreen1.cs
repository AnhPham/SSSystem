using UnityEngine;
using System.Collections;

public class DemoScreen1 : SSController
{
    public void OnNextButtonTap()
    {
        SSSceneManager.Instance.AddScreen("DemoScreen2");
    }
}
