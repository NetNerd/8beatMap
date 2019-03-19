using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace _8beatMap
{
    static class OpenTkTextureLoadFuncs
    {
        public static int LoadTexture(string path)
        {
            Bitmap bmp;
            try
            { bmp = new Bitmap(path); }
            catch (Exception e)
            {
                bmp = new Bitmap(1, 1);
                throw e;
            }

            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);


            int tex = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, tex);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, -0.33f);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);


            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

            GL.Enable(EnableCap.Texture2D); // this is needed because an ATI bug apparently (not sure how recently)
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);


            bmp.UnlockBits(bmpData);
            bmp.Dispose();

            GL.BindTexture(TextureTarget.Texture2D, 0);

            return tex;
        }


        public static int LoadTexture8BitGrayscale(string path)
        {
            Bitmap bmp;
            try
            { bmp = new Bitmap(path); }
            catch (Exception e)
            {
                bmp = new Bitmap(1, 1);
                throw e;
            }


            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            byte[] dataBytes = new byte[bmp.Width * bmp.Height * 4];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, dataBytes, 0, bmp.Width * bmp.Height * 4);

            byte[] monodata = new byte[bmp.Width * bmp.Height];
            for (int i = 0; i < bmp.Height * bmp.Width * 4; i += 4)
            {
                monodata[i / 4] = dataBytes[i]; // just sample blue because it's easiest
            }


            int tex = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, tex);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, -0.33f);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);


            System.Drawing.Imaging.BitmapData bmpDataJustChannel = new System.Drawing.Imaging.BitmapData()
            { PixelFormat = System.Drawing.Imaging.PixelFormat.DontCare, Width = bmp.Width, Height = bmp.Height };

            bmpDataJustChannel.Scan0 = System.Runtime.InteropServices.Marshal.AllocHGlobal(monodata.Length);
            System.Runtime.InteropServices.Marshal.Copy(monodata, 0, bmpDataJustChannel.Scan0, monodata.Length);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Alpha, bmp.Width, bmp.Height, 0, PixelFormat.Alpha, PixelType.UnsignedByte, bmpDataJustChannel.Scan0);

            bmpDataJustChannel.Scan0 = IntPtr.Zero;
            System.Runtime.InteropServices.Marshal.FreeHGlobal(bmpDataJustChannel.Scan0);

            GL.Enable(EnableCap.Texture2D); // this is needed because an ATI bug apparently (not sure how recently)
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);


            bmp.UnlockBits(bmpData);
            bmp.Dispose();

            GL.BindTexture(TextureTarget.Texture2D, 0);

            return tex;
        }


        // loads in order ARGB
        public static int[] LoadTextureToSplitChannels(string path)
        {
            Bitmap bmp;
            try
            { bmp = new Bitmap(path); }
            catch (Exception e)
            {
                bmp = new Bitmap(1, 1);
                throw e;
            }


            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            byte[] dataBytes = new byte[bmp.Width * bmp.Height * 4];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, dataBytes, 0, bmp.Width * bmp.Height * 4);

            byte[][] planes = new byte[4][]
            { new byte[bmp.Width * bmp.Height], new byte[bmp.Width * bmp.Height], new byte[bmp.Width * bmp.Height], new byte[bmp.Width * bmp.Height] };
            for (int i = 0; i < bmp.Height * bmp.Width * 4; i += 4)
            {
                planes[0][i / 4] = dataBytes[i];
                planes[1][i / 4] = dataBytes[i + 1];
                planes[2][i / 4] = dataBytes[i + 2];
                planes[3][i / 4] = dataBytes[i + 3];
            }


            int[] outtextures = new int[4];
            for (int i = 0; i < 4; i++)
            {
                int tex = GL.GenTexture();
                outtextures[3 - i] = tex;

                GL.BindTexture(TextureTarget.Texture2D, tex);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, -0.33f);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);


                System.Drawing.Imaging.BitmapData bmpDataJustChannel = new System.Drawing.Imaging.BitmapData()
                { PixelFormat = System.Drawing.Imaging.PixelFormat.DontCare, Width = bmp.Width, Height = bmp.Height };

                bmpDataJustChannel.Scan0 = System.Runtime.InteropServices.Marshal.AllocHGlobal(planes[i].Length);
                System.Runtime.InteropServices.Marshal.Copy(planes[i], 0, bmpDataJustChannel.Scan0, planes[i].Length);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Alpha, bmp.Width, bmp.Height, 0, PixelFormat.Alpha, PixelType.UnsignedByte, bmpDataJustChannel.Scan0);

                bmpDataJustChannel.Scan0 = IntPtr.Zero;
                System.Runtime.InteropServices.Marshal.FreeHGlobal(bmpDataJustChannel.Scan0);

                GL.Enable(EnableCap.Texture2D); // this is needed because an ATI bug apparently (not sure how recently)
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }


            bmp.UnlockBits(bmpData);
            bmp.Dispose();

            GL.BindTexture(TextureTarget.Texture2D, 0);

            return outtextures;
        }

        public static void UnloadTexture(int tex)
        {
            GL.DeleteTexture(tex);
        }
    }
}
