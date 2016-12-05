master_title:
ID: A number. Any's fine, but something large (5000-10000) should be good (the original songs use smaller numbers).
LEVEL: 1=Easy, 2=Normal... though there's no option for Mother in this file (the limit for the standard songs roster is expert)
TITLE: <--
TYPE: 0=All, 1=Cute(Pink), 2=Cool(Blue), 3=Fine(Yellow)
DISC_LV: Song difficulty rating
BPM: BPM
MUSIC: Put the ID after "music"
MISSION: The same as the ID (if you have multiple difficulties, use multiple mission numbers)
--don't change anything else


master_mission:
ID: Use "MISSION" from master_title
MISSION_SCORE_-: Just copy it from another song
MISSION_COMBO_-: S is a full combo, A is FC*0.7, B is FC*0.5, C is FC*0.3
MISSION_PLAY_-: Just copy it from another song


master_music:
ID: Use "ID" from master_title
FULL_COMBO_-: Just for the difficulties in master_title, use the COMBO_S (others are 999).
TYPE: Use "TYPE" from master_title
--don't change anything else




Other required files:
Rename the beatmap to "master_musicID_LEVEL.dec.json" (ID and LEVEL are from master_title)
img_discID.png: 320x320 cover
img_discID_mini.png: 160x160 kover
musicID.acb: The music
  To make it, we'll use DereTore. (https://github.com/OpenCGSS/DereTore)
    --You need "hcaenc_lite.dll" from "ADX2 LE". (http://www.adx2le.com/download/index.html)
    --If you're not in Japan, search on Google for "CriAtomPlayer sdk". Lots of other people have already uploaded it.
  Rename a .wav file to "musicID.wav".
  Put the .wav, hcaenc_lite.dll, and acb.bat in the DereTore folder.
  Drag the .wav onto acb.bat.