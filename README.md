SS-System 1.3.0
========

SS-System for Unity: Scene &amp; UI Manager!

---

<b>RELEASE NOTES:</b>

Version 1.3.0

- <b>(IMPORTANT)</b> Made a big change to support <b>'Switch View Animation'</b> looks like when you tap an item on the list of iOS.
It works only with 'Sub-Screens' of SS-System, which become a stack now. 
SSMotion & SSAnimation now have 4 slots for adding animations.
I also included two more default slide animations, now we have four ones, just drag them correspondingly to SSAnimation of your scene.
Call SubScreen(scene 1) then SubScreen(scene 2) for example.

Version 1.2.2

- (NEW) SSSceneManager: Add SetGlobalBgm() and ClearGlobalBgm().
- (FIX) SSRootScale: Fixed a bug which not work fine with the newest NGUI versions.

Version 1.2.1

<b>(IMPORTANT)</b>- Made a big change in SSSceneManager for resolving the problem of 1-frame delay in OnSet method, and now it SUPPORT UNITY FREE!

- SSSceneManager: Add class PopUpData
- SSSceneManager: Fix a bug which crash when lock/unlock scene.
- SSSceneManager: Add Default Shield Color, Remove Default Shield Alpha.
- SSController: Add virtual method OnSetAdding()

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





