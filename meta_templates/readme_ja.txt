「master_title」：
ID：ナンバー。なんでもいいけど大きなナンバー（5000～10000）はいいと思います（クロノスの曲は小さなナンバー）。
LEVEL：1=EASY、2=NORMAL...　でもそのファイルにはMOTHERではない（普通の曲に限界はEXPERT）
TITLE：曲名
TYPE：0=オール、1=キュート、2=クール、3=ファイン
DISC_LV：音楽Lv.（難度の数字）
BPM：BPM
MUSIC：「music」にIDを追加
MISSION：IDと同じはいい（多い難度の場合で多いMISSIONナンバーを使う）
※他の事を変わりないでください


「master_mission」：
ID：「master_title」からの「MISSION」です
MISSION_SCORE_◯：他の曲からコピーはいい。
MISSION_COMBO_◯：Sはフルコンボ。Aはフルコンボ×0.7。Bはフルコンボ×0.5。Cはフルコンボ×0.3。
MISSION_PLAY_◯：他の曲からコピーはいい。


「master_music」：
ID：「master_title」からの「ID」です
FULL_COMBO_◯：「master_title」での難度だけで、COMBO_Sを書く（他の難度は999）
TYPE：「master_title」からの「TYPE」です
※他の事を変わりないでください




他の要るファイル：
譜面のネームを「master_musicID_LEVEL.dec.json」に変わる（IDとLEVELは「master_title」から）
「img_discID.png」：320x320pxのカバー
「img_discID_mini.png」：160x160pxのカバー
「musicID.acb」：音楽
　　作り方は「DereTore」で。　（https://github.com/OpenCGSS/DereTore）
　　　　※「ADX2 LE」からの「hcaenc_lite.dll」も要る。　（http://www.adx2le.com/download/index.html）
　　　　※外人のために、グーグルで「CriAtomPlayer sdk」に探す。多いの人もうアップした。
　　.wavファイルのネームを「musicID.wav」に変わる。
　　.wavとhcaenc_lite.dllとacb.batをDereToreのフォルダーに置く。
　　.wavをacb.batにドラッグする。