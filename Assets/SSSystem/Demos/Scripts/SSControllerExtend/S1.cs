using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class S1 : SSController 
{
    Toggle m_ToggleCache;

	public override void Config ()
	{
		BgmType = Bgm.PLAY;
		BgmName = "S1";

        IsCache = true;
	}

    public override void OnSet(object data)
    {
        if (m_ToggleCache == null)
        {
            m_ToggleCache = GetComponentInChildren<Toggle>();
        }

        m_ToggleCache.isOn = IsCache;
    }

    public override void OnKeyBack()
    {
        SceneManagerDemo.Instance.GoHome();
    }

    public void OnButtonPopUp1Tap()
    {
        StartCoroutine(SimulateLoadingThenOpenPopUp1());
    }

    public void OnButtonPopUp2Tap()
    {
        SceneManagerDemo.Instance.PopUp("P2");
    }

    public void OnButtonAddScreen3Tap()
    {
        SceneManagerDemo.Instance.AddScreen("S3");
    }

    private IEnumerator SimulateLoadingThenOpenPopUp1()
    {
        SceneManagerDemo.Instance.ShowLoading();

        yield return new WaitForSeconds(1);

        SceneManagerDemo.Instance.HideLoading();

        SceneManagerDemo.Instance.PopUp("P1");
    }

    public void OnToggleCacheChange(bool isOn)
    {
        IsCache = isOn;
    }
}
