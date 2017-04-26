// This code is part of the SS-System library (https://github.com/AnhPham/SSSystem) maintained by Anh Pham (anhpt.csit@gmail.com).
// It is released for free under the MIT open source license (https://github.com/AnhPham/SSSystem/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

public class DemoScreen2 : SSController
{
    public void OnPlayButtonTap()
    {
        SSSceneManager.Instance.ShowLoading();

        StartCoroutine(SimulationLoadingTime());
    }

    private IEnumerator SimulationLoadingTime()
    {
        yield return new WaitForSeconds(2);

        SSSceneManager.Instance.Screen("DemoPlay", null,
            (ctrl) =>
            {
                SSSceneManager.Instance.HideLoading();
            }
        );
    }

    public void OnBackButtonTap()
    {
        SSSceneManager.Instance.Close();
    }
}
