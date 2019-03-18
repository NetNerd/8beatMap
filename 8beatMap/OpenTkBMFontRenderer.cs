//using System;
//using System.Drawing;
//using OpenTK;
//using OpenTK.Graphics.OpenGL;

//namespace _8beatMap
//{
//    class OpenTkBMFontRenderer
//    {
//        private System.Collections.Generic.Dictionary<string, int> textures = new System.Collections.Generic.Dictionary<string, int>();

//        private void UnloadAllTextures()
//        {
//            GL.BindTexture(TextureTarget.Texture2D, 0); // make sure no textures are loaded before deleting
//            foreach (System.Collections.Generic.KeyValuePair<string, int> tex in textures)
//            {
//                UnloadTexture(tex.Value);
//            }
//            textures.Clear();
//        }

//        private void LoadFontTextures(BMFontReader.BMFont font)
//        {
//            for (int i = 0; i < font.PageTexPaths.Length; i++)
//            {
//                if (font.PageTexPaths[i] == null) continue;

//                string texkey = font.PageTexPaths[i];
//                string texpath = font.PageTexPaths[i];

//                if (font.CanLoad8Bit)
//                {
//                    if (!textures.ContainsKey(texkey))
//                        textures.Add(texkey, LoadTexture8BitGrayscale(texpath));
//                    else
//                        textures[texkey] = LoadTexture8BitGrayscale(texpath);
//                }
//                else
//                {
//                    if (!textures.ContainsKey(texkey))
//                        textures.Add(texkey, LoadTexture(texpath));
//                    else
//                        textures[texkey] = LoadTexture(texpath);
//                }

//                if (font.CommonInfo.Packed)
//                {
//                    int[] channelTextures = LoadTextureToSplitChannels(texpath);

//                    if (!textures.ContainsKey(texkey + "A"))
//                        textures.Add(texkey + "A", channelTextures[0]);
//                    else
//                        textures[texkey + "A"] = channelTextures[0];

//                    if (!textures.ContainsKey(texkey + "R"))
//                        textures.Add(texkey + "R", channelTextures[1]);
//                    else
//                        textures[texkey + "R"] = channelTextures[1];

//                    if (!textures.ContainsKey(texkey + "G"))
//                        textures.Add(texkey + "G", channelTextures[2]);
//                    else
//                        textures[texkey + "G"] = channelTextures[2];

//                    if (!textures.ContainsKey(texkey + "B"))
//                        textures.Add(texkey + "B", channelTextures[3]);
//                    else
//                        textures[texkey + "B"] = channelTextures[3];
//                }
//            }
//        }
//    }
//}
