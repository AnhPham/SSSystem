using UnityEngine;
using System.Collections;

public class SSRootAutoDestroy : SSRoot
{
    protected override void Awake()
    {
        base.Awake();

        if (Application.isPlaying)
        {
            if (SSSceneManager.Instance != null)
            {
                SSApplication.OnUnloaded(gameObject);
            }

            Destroy(gameObject);
        }
    }
}
