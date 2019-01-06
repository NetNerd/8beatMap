8beatMapとはスマホ音楽ゲーム「8 beat Story♪」の譜面エディタープログラム。「Girls Beat Stage!」の譜面も読み込める（エディター機能はない）。
多い機能がありませんだけどかなり易しと思います。

8beatMap is a chart editor for the mobile rhythm game "8 beat Story♪". It can also read "Girls Beat Stage!" charts (but not lacks editing features for them).
It doesn't have a lot of features, but I think it's pretty easy to use.




使い方について　(Usage Notes):


ノードを置くのは左マウスボタンで。　消すは右ボタンで。

ノードのタイプを選ぶことはキーボードでも出来ます。
１：押す　　　Ｑ：同時に押す
２：長い押す　　　Ｗ：同時に長い押すの最初　　　Ｓ：同時に長い押すの最後
３：左へフリック　　　Ｅ：長い押すの最後での左へフリック
４：右へフリック　　　Ｒ：長い押すの最後での右へフリック
５：左へスワイプの端　　Ｔ：左へスワイプの真ん中で　　　Ｇ：右へスワイプから左に変わる
６：右へスワイプの端　　Ｙ：右へスワイプの真ん中で　　　Ｈ：左へスワイプから右に変わる

特別なノードタイプ：
・「同時に～」はピンク色のノード。　オート機能がありますので手動の置くのは要らない。
・「同時に長い押すの最初」は長い押すのファースト音符のピンク色版。　「同時に長い押すの最後」は長い押すのラスト音符のピンク色版。　　　※普通の長い押す（緑／同時じゃない）場合では特別なノードタイプは必要ない。
・「フリック」は一つのノードだけ、「スワイプ」は2つ以上のノードで。
・「スワイプの端」とはスワイプの最初や最後のノード、「スワイプの真ん中で」とは他のスワイプノード。
・「長い押すの最後でのフリック」はフリックのためだけ。スワイプは普通のノードタイプで。


※ズームと長さと全ノードを動かすの機能にはちょっとバグがあります。用いるの前に保存してください。
※今、絵を保存のはプログラムのフォルダーで「imgout.png」に書いてます。
※ノード音のタイミングは違う場合があります（でもたいていはいいと思います）。



譜面のファイルフォーマットは「.dec.json」。　ゲームと同じなんだけど復号したです（「dec」は「decrypt／デクリプト」から）。
復号しプログラムはここで込まない。




Use the left mouse button to place notes and the right one to delete them.

Note types can be selected with keyboard shortcuts too.
1: Tap    Q: SimulTap
2: Hold    W: SimulHoldStart    S: SimulHoldEnd
3: FlickLeft    E: HoldEndFlickLeft
4: FlickRight    R: HoldEndFlickRight
5: SwipeLeftStartEnd    T: SwipeLeftMid    G: SwipeChangeDirR2L
6: SwipeRightStartEnd    Y: SwipeRightMid    H: SwipeChangeDirL2R

Special note types:
-"Simul-" notes are the pink ones (at the same time as others). There's an auto feature, so you don't need to place them manually.
-"SimulHoldStart" is the pink version of the first note in a hold.  "SimulHoldEnd" is the pink version of the last note in a hold.    --You don't need a special note type for normal (green/non-simul) holds.
-Flicks are just single notes and holds are when there's more than one.
-"Swipe__StartEnd" notes are used at the start and end of swipes. "Swipe__Mid" notes are used for the other notes in them.
-"HoldEndFlick" notes are only for flicks. Swipes don't have a special type.


--The zoom, length, and note shift functions are a bit buggy. Please save before using them.
--Currently, the "save image" button saves to "imgout.png" in the same folder as the program.
--Note sound timing may be a little wrong (though it seems mostly good to me).



The file format for charts is ".dec.json". It's the same type as the game but decrypted (the "dec" comes from "decrypt").
A decryption program is not included.