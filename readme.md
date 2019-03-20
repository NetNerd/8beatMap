# <img alt="8beatMap Icon" src="https://raw.githubusercontent.com/NetNerd/8beatMap/master/icon/icon-96x96-fs8.png" width="48" height="48" align="middle" /> 8beatMap

8beatMapとはスマホ音楽ゲーム「8 beat Story♪」の譜面エディタープログラム。「Girls Beat Stage!」の譜面も対応しています。  
多い機能がありませんだけどかなり易しと思います。

8beatMapはWindows上.NET Framework 4.5またはMonoで動作します。  
他のOSで動作のはNAudioによって制限されています。多分Wineで動作出来る。  
Windows上Monoではコピー・ペーストが機能しないみたい。（他のOSで機能するかどうか分からない）

自動ビルドはAppVeyorで：https://ci.appveyor.com/project/ntnd/8beatmap/branch/master/artifacts  
8beatMap.1.0….zipをダウンロードと解凍して、そして8beatMap.exeを実行してください。  
skins.zipは追加のスキンだけ（任意）。追加のスキンが欲しいの場合では両zipファイルをダウンロードと解凍して、さらにskinsフォルダを8beatMapフォルダまでコーピーしてください。

　

8beatMap is a chart editor for the mobile rhythm game "8 beat Story♪". It also supports "Girls Beat Stage!" charts.  
It doesn't have a lot of features, but I think it's pretty easy to use.

8beatMap runs on Windows with .NET Framework 4.5 or Mono.  
Cross-platform support is limited by NAudio. It will probably work using Wine.  
Copy and paste seems broken in Mono on Windows (might work on other platforms).

Automated builds are on AppVeyor: https://ci.appveyor.com/project/ntnd/8beatmap/branch/master/artifacts  
Download and extract 8beatMap.1.0….zip, then run 8beatMap.exe.  
skins.zip is just additional skins (optional). If you want the additional skins, download and extract both zip files, then copy the skins folder into the 8beatMap folder.

　

### スクリーンショット　(Screenshots):

<img alt="メインウィンドウ　(Main Window)" src="https://raw.githubusercontent.com/NetNerd/8beatMap/master/screenshots/mainwindow-fs8.png" width="320" height="270" />　<img alt="プレビューウィンドウ　(Preview Window)" src="https://raw.githubusercontent.com/NetNerd/8beatMap/master/screenshots/previewwindow-fs8.png" width="434" height="259"  />  
(8bs Dark skin)

　

### 使い方について　(Usage Notes):

ノードを置くのは左マウスボタンで。　消すは右ボタンで。

ノードのタイプを選ぶのはキーボードでも出来ます：  
1：押す　　　Q：同時に押す  
2：長い押す　　　W：同時に長い押すの最初　　　S：同時に長い押すの最後  
3：左へフリック　　　E：長い押すの最後で左へフリック  
4：右へフリック　　　R：長い押すの最後で右へフリック  
5：左へスワイプの端　　　T：左へスワイプの真ん中で　　　G：右へスワイプから左に変わる  
6：右へスワイプの端　　　Y：右へスワイプの真ん中で　　　H：左へスワイプから右に変わる  
7：GBSフリック　　　U：同時にGBSフリック  
8：GBS時計　　　I：同時にGBS時計

特別なノードタイプ：
・「同時に～」はピンク色のノード。　オート機能がありますので手動の置くのは要らない。
・「同時に長い押すの最初」は長い押すのファースト音符のピンク色版。　「同時に長い押すの最後」は長い押すのラスト音符のピンク色版。　　　※普通の長い押す（緑／同時じゃない）場合では特別なノードタイプは必要ない。
・「フリック」は一つのノードだけ、「スワイプ」は2つ以上のノードで。
・「スワイプの端」とはスワイプの最初や最後のノード、「スワイプの真ん中で」とは他のスワイプノード。
・「長い押すの最後でフリック」はフリックのためだけ。スワイプは普通のノードタイプで。
・本物のGBSではフリックやスワイプはありません。本物のエビストでは「GBS～」のタイプはありません。  
　

他のショートカットキー：  
P：プレビューウィンドウ  
/：譜面でノードIDを示す（デバッグのため）  
M：プレビューウィンドウの背景色を黒にする  
,（<）：プレビューウィンドウの背景色を灰色にする  
.（>）：プレビューウィンドウの背景色を白にする  
Ctrl+C：「コピー長さ」で設定した小節数をクリップボードにコピー  
Ctrl+V：コピーしたデータを再生位置でペースト  
Ctrl+Shift+V：左右反転ペースト  
Ctrl+Q：オート同時風ノード（Qは同時に押すのショートカットキー）  
Ctrl+1：ノードを数える（1は押すのショートカットキー）  
Ctrl+D：譜面難易度（Dはディフィカルティ）  
Ctrl+B：迅速BPM設定  
Ctrl+I：譜面作品情報（Iはインフォメーション）  
（海外キーボードでは異なる可能性があります）  
　

※ズームと長さと全ノードを動かすの機能にはちょっとバグがあります。用いるの前に保存してください。  
※今、絵を保存のはプログラムのフォルダーで「imgout.png」に書いてます。  
※ノード音のタイミングは違う場合があります（でもたいていはいいと思います）。  
　

譜面のファイルフォーマットは「.dec.json」。　ゲームと同じなんだけど復号したです（「dec」は「decrypt／デクリプト」から）。
復号しプログラムはここで込まない。

　

Use the left mouse button to place notes and the right one to delete them.

Note types can be selected with keyboard shortcuts too:
1: Tap　　　Q: SimulTap  
2: Hold　　　W: SimulHoldStart　　　S: SimulHoldRelease  
3: FlickLeft　　　E: HoldEndFlickLeft  
4: FlickRight　　　R: HoldEndFlickRight  
5: SwipeLeftStartEnd　　　T: SwipeLeftMid　　　G: SwipeChangeDirR2L  
6: SwipeRightStartEnd　　　Y: SwipeRightMid　　　H: SwipeChangeDirL2R  
7: GBSFlick　　　U: GBSSimulFlick  
8: GBSClock　　　I: GBSSimulClock  
　

Special note types:  
-"Simul-" notes are the pink ones (at the same time as others). There's an auto feature, so you don't need to place them manually.  
-"SimulHoldStart" is the pink version of the first note in a hold.  "SimulHoldRelease" is the pink version of the last note in a hold.    --You don't need a special note type for normal (green/non-simul) holds.  
-Flicks are just single notes and holds are when there's more than one.  
-"Swipe__StartEnd" notes are used at the start and end of swipes. "Swipe__Mid" notes are used for the other notes in them.  
-"HoldEndFlick" notes are only for flicks. Swipes don't have a special type.  
-In the actual game GBS, there are no flicks or swipes. In 8bs, the "GBS" types don't exist.  
　

Other shortcut keys:  
P: Preview window  
/: Show note IDs on chart (for debugging)  
M: Make preview window background black  
,(<): Make preview window background grey  
.(>): Make preview window background white  
Ctrl+C: Copy the amount of bars set as copy length (starting from the playhead) to the clipboard  
Ctrl+V: Paste data from the clipboard at the playhead  
Ctrl+Shift+V: Paste mirrored  
Ctrl+Q: Auto SumulNotes (Q is SimulTap note type)  
Ctrl+1: Note Count (1 is Tap note type)  
Ctrl+D: Difficulty  
Ctrl+B: Quickly set BPM  
Ctrl+I: Chart Info  
(may differ with international keyboard layouts)  
　

--The zoom, length, and note shift functions are a bit buggy. Please save before using them.  
--Currently, the "save image" button saves to "imgout.png" in the same folder as the program.  
--Note sound timing may be a little wrong (though it seems mostly good to me).  
　

The file format for charts is ".dec.json". It's the same type as the game but decrypted (the "dec" comes from "decrypt").
A decryption program is not included.