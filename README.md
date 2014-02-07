SSSystem 1.2.0
========

SS-System for Unity3d: Scene &amp; UI Manager!

---

<b>RELEASE NOTES:</b>

Version 1.2.0

<b>(IMPORTANT)</b>  You must to edit all Start() function of scripts which inherit SSController.<br>
.FROM : void Start() { // Your code } <br>
.TO   : protected override void Start() { base.Start(); // Your code }

- SSController: Start() is virtual from now.
- SSController: Add OnStartWithoutSceneManager() just for testing easier.
- SSController: Add OnEnableFS(). FS means 'First in Start'. The first call of OnEnable() is right after Awake(). The first call of OnEnableFS() is in Start().
- SSSceneManager: Add public function: LoadRes(string sceneName) and UnloadRes(string sceneName) for the scene which contains only resources.
- SSSceneManager: Add virtual function OnFirstSceneLoad.
- SSSceneManager: Add event onScreenStartChange(string sceneName).
- SSSceneManager: Fixed a bug which OnHide() is not be call when we call Screen().
- SSRootScale: Add Anchor TOP/CENTER/BOTTOM. Fixed bug.
- Demo: Deleted Resources folder, Add Sound folder.

Version 1.1.0

- SSSceneManager: ResetScreen is deprecated, please use Reset instead. Fixed a bug when Reset.
- SSMotion: Add SSMotion. SSAnimation extends SSMotion. Now you can extend SSMotion to make your own animation scripts, which use other tween libraries. Don't forget ignore time scale of tween!

Version 1.0.11

- SSSceneManager: Fixed bug top shield, Changed default alpha top shield to 0.5
- SSSceneManager: Add function HideLoading(bool isForceHide = false)

…

---


I . How to use Demo:

1. Add all scenes in SSSystem/Demos/Scenes to Build Setting
2. Play this scene: SSSystem/Demos/Scenes/Base/BaseSound

II. All documents:

1. Presentation: <a>http://anh-pham.appspot.com/sssystem/en/sssystem.pdf</a>
2. How to use (step by step): <b>Hello World</b><br>
<a>http://anh-pham.appspot.com/sssystem/en/sshello.pdf</a><br>
or:
Offline document: Assets/SSSystem/Documents/sshello.pdf
3. How to use (step by step): <b>Advance</b><br>
<a>http://anh-pham.appspot.com/sssystem/en/ssadvance.pdf</a><br>
or:
Offline document: Assets/SSSystem/Documents/ssadvance.pdf
4. Script Reference:
<a>http://anh-pham.appspot.com/sssystem/</a>

III. Home page:
<a>http://anh-pham.appspot.com</a>

IV. Email contact:
<a>anhpt.csit@gmail.com</a>





