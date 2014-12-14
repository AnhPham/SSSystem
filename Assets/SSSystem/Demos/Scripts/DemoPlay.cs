using UnityEngine;
using System.Collections;

public class DemoPlay : SSController
{
    [SerializeField]
    NavMeshAgent m_Agent;

    [SerializeField]
    Transform m_Target;

    [SerializeField]
    Transform m_MoveButton;

    public override void OnShow()
    {
        SSSceneManager.Instance.HideMenu();
    }

    public override void OnHide()
    {
        SSSceneManager.Instance.ShowMenu();
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            m_Agent.destination = m_Target.position;
        }
    }

    public void OnPauseButtonTap()
    {
        SSSceneManager.Instance.PopUp("DemoPopup", new DemoPopupData("Are you sure to quit?", DemoPopupType.YES_NO), 
            (ctrl) =>
            {
                DemoPopup popup = (DemoPopup)ctrl;
                popup.onYesButtonTap += OnYesButtonTap;
            },
            (ctrl) =>
            {
                DemoPopup popup = (DemoPopup)ctrl;
                popup.onYesButtonTap -= OnYesButtonTap;
            });
    }

    private void OnYesButtonTap()
    {
        SSSceneManager.Instance.Screen("DemoScreen1");
    }

    public void OnMoveButtonTap()
    {
        Vector3 pos = m_Target.position;
        pos.z *= -1;
        m_Target.position = pos;
    }
}
