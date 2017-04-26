﻿// This code is part of the SS-System library (https://github.com/AnhPham/SSSystem) maintained by Anh Pham (anhpt.csit@gmail.com).
// It is released for free under the MIT open source license (https://github.com/AnhPham/SSSystem/blob/master/LICENSE)

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
