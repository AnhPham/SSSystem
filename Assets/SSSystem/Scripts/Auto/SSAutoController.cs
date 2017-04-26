// This code is part of the SS-System library (https://github.com/AnhPham/SSSystem) maintained by Anh Pham (anhpt.csit@gmail.com).
// It is released for free under the MIT open source license (https://github.com/AnhPham/SSSystem/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SSAutoController : MonoBehaviour
{
    SSRoot Root;

    void Awake()
    {
        CreateRoot();
    }

    void OnValidate()
    {
        CreateRoot();
    }

    void CreateRoot()
    {
        if (Root == null && !Application.isPlaying)
        {
            Transform root = gameObject.transform;
            while (root.parent != null)
            {
                root = root.parent;
            }

            SSRoot ssRoot = root.GetComponent<SSRoot>();
            if (ssRoot == null)
            {
                root.gameObject.AddComponent<SSRoot>();
            }

            Root = ssRoot;
        }
    }
}
