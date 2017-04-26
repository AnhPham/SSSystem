// This code is part of the SS-System library (https://github.com/AnhPham/SSSystem) maintained by Anh Pham (anhpt.csit@gmail.com).
// It is released for free under the MIT open source license (https://github.com/AnhPham/SSSystem/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

public class SSAutoAddUICamera : MonoBehaviour 
{
	private void Awake()
	{
		UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(gameObject, "Assets/SSSystem/Scripts/Compatibility/nGUI/SSAutoAddUICamera.cs (8,3)", "UICamera");
	}
}
