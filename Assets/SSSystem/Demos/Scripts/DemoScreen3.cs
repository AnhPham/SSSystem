using UnityEngine;
using System.Collections;

public class DemoScreen3 : SSController
{
    public void OnPopupButtonTap()
    {
        SSSceneManager.Instance.PopUp("DemoPopup", new DemoPopupData("This is a popup", DemoPopupType.OK));
    }
}
