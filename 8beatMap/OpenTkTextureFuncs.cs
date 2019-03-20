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
            {
                bmp = new Bitmap(path);
            }
            catch (Exception e)
            {
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
            {
                bmp = new Bitmap(path);
            }
            catch (Exception e)
            {
                throw e;
            }


            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int bytesPerPixel = bmpData.Stride / bmp.Width;


            IntPtr monoDataScan = IntPtr.Zero;
            try
            {
                    monoDataScan = System.Runtime.InteropServices.Marshal.AllocHGlobal(bmp.Width * bmp.Height); // allocate as 8bpp
            }
            catch (Exception e)
            {
                if (monoDataScan != null) System.Runtime.InteropServices.Marshal.FreeHGlobal(monoDataScan);
                bmp.UnlockBits(bmpData);
                bmp.Dispose();
                throw e;
            }

            unsafe
            {
                byte* inputData = (byte*)bmpData.Scan0;
                byte* outputData = (byte*)monoDataScan;
                for (int i = 0; i < bmp.Height * bmpData.Stride; i += bytesPerPixel)
                {
                    outputData[i / bytesPerPixel] = inputData[i]; // just sample blue because it's easiest
                }
            }


            int tex = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, tex);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, -0.33f);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);
            

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Alpha, bmp.Width, bmp.Height, 0, PixelFormat.Alpha, PixelType.UnsignedByte, monoDataScan);

            System.Runtime.InteropServices.Marshal.FreeHGlobal(monoDataScan);

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
            {
                bmp = new Bitmap(path);
            }
            catch (Exception e)
            {
                throw e;
            }


            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int bytesPerPixel = bmpData.Stride / bmp.Width;


            IntPtr[] monoDataScans = new IntPtr[bytesPerPixel];
            try
            {
                for (int i = 0; i < bytesPerPixel; i++)
                    monoDataScans[i] = System.Runtime.InteropServices.Marshal.AllocHGlobal(bmp.Width * bmp.Height); // allocate as 8bpp
            }
            catch (Exception e)
            {
                foreach (IntPtr scan in monoDataScans)
                {
                    if (scan != null) System.Runtime.InteropServices.Marshal.FreeHGlobal(scan);
                }
                bmp.UnlockBits(bmpData);
                bmp.Dispose();
                throw e;
            }

            unsafe
            {
                byte*[] planes = new byte*[bytesPerPixel];
                for (int i = 0; i < bytesPerPixel; i++)
                    planes[i] = (byte*)monoDataScans[i];

                byte* inputData = (byte*)bmpData.Scan0;

                for (int i = 0; i < bmp.Height * bmpData.Stride; i += bytesPerPixel)
                {
                    for (int j = 0; j < bytesPerPixel; j++)
                        planes[j][i / bytesPerPixel] = inputData[i + j];
                }
            }


            int[] outtextures = new int[bytesPerPixel];
            for (int i = 0; i < bytesPerPixel; i++)
            {
                int tex = GL.GenTexture();
                outtextures[bytesPerPixel - 1 - i] = tex;

                GL.BindTexture(TextureTarget.Texture2D, tex);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, -0.33f);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);


                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Alpha, bmp.Width, bmp.Height, 0, PixelFormat.Alpha, PixelType.UnsignedByte, monoDataScans[i]);

                GL.Enable(EnableCap.Texture2D); // this is needed because an ATI bug apparently (not sure how recently)
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }

            foreach (IntPtr scan in monoDataScans)
            {
                System.Runtime.InteropServices.Marshal.FreeHGlobal(scan);
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
