# <img alt="8beatMap Icon" src="https://raw.githubusercontent.com/NetNerd/8beatMap/master/icon/icon-96x96-fs8.png" width="48" height="48" align="top" /> 8beatMap

8beatMapはスマホ音楽ゲーム「8 beat Story♪」の譜面に対応したエディタープログラムです。「Girls Beat Stage!」の譜面にも対応しています。  
多くの機能はありませんが、誰でも簡単に扱えると思います。

　

8beatMap is a chart editor for the mobile rhythm game "8 beat Story♪". It also supports "Girls Beat Stage!" charts.  
It doesn't have a lot of features, but I think it's pretty easy to use.

　

### スクリーンショット　(Screenshots):

<img alt="メインウィンドウ　(Main Window)" src="https://raw.githubusercontent.com/NetNerd/8beatMap/master/screenshots/mainwindow-fs8.png" width="320" />　<img alt="プレビューウィンドウ　(Preview Window)" src="https://raw.githubusercontent.com/NetNerd/8beatMap/master/screenshots/previewwindow-fs8.png" width="434" />  
(8bs Dark skin)

　

### ダウンロード　(Download):

自動ビルド済みファイル は こちらから(AppVeyor)：https://ci.appveyor.com/project/ntnd/8beatmap/branch/master/artifacts  

8beatMapは Windows上の .NET Framework 4.5またはMonoで動作します。  
他のOSでの動作はNAudioによって制限されています。恐らくWineで動作可能です。  
Windows上Monoではコピー・ペーストが機能しないようです。（他のOSで機能するかどうかは不明）
  

8beatMap.1.0….zip をダウンロード、解凍し8beatMap.exeを実行してください。  
skins.zip は追加のスキンです（任意）。
追加のスキンが欲しい場合は両zipファイルをダウンロードし、解凍して、skinsフォルダを8beatMapフォルダへコピーしてください。
  
  

Automated builds are on AppVeyor: https://ci.appveyor.com/project/ntnd/8beatmap/branch/master/artifacts  

8beatMap runs on Windows with .NET Framework 4.5 or Mono.  
Cross-platform support is limited by NAudio. It will probably work using Wine.  
Copy and paste seems broken in Mono on Windows (might work on other platforms).

Download and extract 8beatMap.1.0….zip, then run 8beatMap.exe.  
skins.zip is just additional skins (optional). If you want the additional skins, download and extract both zip files, then copy the skins folder into the 8beatMap folder.

　

### 使い方について　(Usage Notes):

ノードを置く際はマウス 左ボタン。　消す際はマウス右ボタン。  
ノードタイプの選択はキーボードショートカットでも出来ます：  
||||
|:----|:---- |:----|
|1：通常タップ     |Q：同時押し||
|2：長押し         |W：同時に長押しの最初       |S：同時に長押しの最後| 
|3：左フリック     |E：長押しの最後で左フリック  ||
|4：右フリック     |R：長押しの最後で右フリック  ||
|5：左スワイプの端 |T：左スワイプの真ん中        |G：右スワイプから左に切替え|  
|6：右スワイプの端 |Y：右スワイプの真ん中        |H：左スワイプから右に切替え|
|7：GBSフリック    |U：同時にGBSフリック||
|8：GBS時計        |I：同時にGBS時計||
||||

特別なノードタイプ：  
- 「同時～」はピンク色のノード。自動でノードを置換する機能があるので手動でのセットは不要。※色はスキンで変更可能です
- 「同時に長押しの最初」は長押しのファースト音符のピンク色版。  
「同時に長押しの最後」は長押しのラスト音符のピンク色版。  
※普通の2:長押し（緑だけ／同時でない）場合では、特別なノードタイプの指定は必要ありません。  
- 「フリック」は一つのノードだけ、「スワイプ」は2つ以上のノードを指します。  
- 「スワイプの端」とはスワイプの最初や最後のノード、「スワイプの真ん中」とはその中間で扱うスワイプノード。  
- 「長押しの最後でフリック」はフリックのために用います。スワイプは普通のノードタイプで。
- 本家GBSではフリックやスワイプはありません。本家エビストでは「GBS～」は利用されていません。
　

他のショートカットキー：  
P：プレビューウィンドウ  
/：譜面のノードIDを示す（デバッグのため）  
M：プレビューウィンドウの背景色を黒にする  
,（<）：プレビューウィンドウの背景色を灰色にする  
.（>）：プレビューウィンドウの背景色を白にする  
Ctrl+C：「コピーする小節の長さ」で設定した小節数をクリップボードにコピー  
Ctrl+V：コピーしたデータを再生位置でペースト  
Ctrl+Shift+V：左右反転ペースト  
Ctrl+Q：自動で同時押しに置換（Qは同時押しのショートカットキー）  
Ctrl+1：ノードを数える（1は通常タップのショートカットキー）  
Ctrl+D：譜面難易度（Dは難易度）  
Ctrl+B：クイックBPM設定  
Ctrl+I：譜面作品情報（Iはインフォメーション）  
（海外キーボードでは異なる可能性があります）  
　

※「譜面の画像出力」ボタンは、「imgout.png」を本プログラムフォルダーに出力します。  
※ノード音のタイミングが異なる場合があります（大抵の場合は問題ない範囲だと思います）。  
※拍子を設定の説明はこちら→[拍子を変わる (Using non 4:4 time signatures)](https://github.com/NetNerd/8beatMap/wiki/拍子を変わる-%28Using-non-4:4-time-signatures%29)。 （英語で書きました）  
　

譜面のファイルフォーマットは「.dec.json」。　ゲームと同じ形式ですが復号しました（「dec」は「decrypt／デクリプト」から）。
復号プログラムについては含みません。

　

Use the left mouse button to place notes and the right one to delete them.

Note types can be selected with keyboard shortcuts too:  
||||
|:----|:---- |:----|
|1: Tap                |Q: SimulTap||  
|2: Hold               |W: SimulHoldStart    |S: SimulHoldRelease  |
|3: FlickLeft          |E: HoldEndFlickLeft  ||
|4: FlickRight         |R: HoldEndFlickRight ||
|5: SwipeLeftStartEnd  |T: SwipeLeftMid      |G: SwipeChangeDirR2L  |
|6: SwipeRightStartEnd |Y: SwipeRightMid     |H: SwipeChangeDirL2R  |
|7: GBSFlick|U: GBSSimulFlick||
|8: GBSClock|I: GBSSimulClock||
||||



Special note types:  
- "Simul-" notes are the pink ones (at the same time as others). There's an auto feature, so you don't need to place them manually.    --Note: colours may change depending on skin.  
- "SimulHoldStart" is the pink version of the first note in a hold.  "SimulHoldRelease" is the pink version of the last note in a hold.    --You don't need a special note type for normal (green/non-simul) holds.  
-Flicks are just single notes and holds are when there's more than one.  
- "Swipe__StartEnd" notes are used at the start and end of swipes. "Swipe__Mid" notes are used for the other notes in them.  
- "HoldEndFlick" notes are only for flicks. Swipes don't have a special type.  
- In the actual game GBS, there are no flicks or swipes. In 8bs, the "GBS" types don't exist.  
　

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
　
  
--Currently, the "save image" button saves to "imgout.png" in the same folder as the program.  
--Note sound timing may be a little wrong (though it seems mostly good to me).    
--Intructions to change the time signature are here: [拍子を変わる (Using non 4:4 time signatures)](https://github.com/NetNerd/8beatMap/wiki/拍子を変わる-%28Using-non-4:4-time-signatures%29).  
　

The file format for charts is ".dec.json". It's the same type as the game but decrypted (the "dec" comes from "decrypt").
A decryption program is not included.