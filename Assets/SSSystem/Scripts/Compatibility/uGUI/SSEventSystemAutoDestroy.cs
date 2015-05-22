using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SSEventSystemAutoDestroy : EventSystem
{
    static SSEventSystemAutoDestroy instance;

    protected override void Awake()
    {
        base.Awake();

        if (instance != null)
        {
			GameObject go = instance.gameObject;

            go.SetActive(false);
            Destroy(go);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
