// This code is part of the SS-System library (https://github.com/AnhPham/SSSystem) maintained by Anh Pham (anhpt.csit@gmail.com).
// It is released for free under the MIT open source license (https://github.com/AnhPham/SSSystem/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class SSAutoSceneManager : MonoBehaviour
{
    #if UNITY_EDITOR
    void Awake()
    {
        AutoAddEmptyScene();
    }

    void OnValidate()
    {
        AutoAddEmptyScene();
    }

    private void AutoAddEmptyScene()
    {
        if (!Application.isPlaying)
        {
            string emptyScene = SSFile.GetPathTemplateFile("SSEmpty.unity");
            emptyScene = emptyScene.Replace(System.IO.Path.DirectorySeparatorChar, '/');

            var scenes = EditorBuildSettings.scenes;

            foreach (var scene in scenes)
            {
                // Check if exist, return
                if (scene.path.CompareTo(emptyScene) == 0)
                {
                    return;
                }
            }

            // If not exist
            var newScenes = new EditorBuildSettingsScene[scenes.Length + 1];

            for (int i = 0; i < scenes.Length; i++)
            {
                newScenes[i] = scenes[i];
            }

            EditorBuildSettingsScene emptySettingScene = new EditorBuildSettingsScene(emptyScene, true);
            newScenes[newScenes.Length - 1] = emptySettingScene;

            // Save
            EditorBuildSettings.scenes = newScenes;
            EditorApplication.SaveAssets();
        }
    }
    #endif
}
