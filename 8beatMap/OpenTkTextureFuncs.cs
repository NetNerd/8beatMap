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

            int bytesPerPixel = bmpData.Stride / bmp.Width;
            //if (bytesPerPixel < 4) return 0;
            //(don't actually need to check when just accessing one byte from each pixel anyway)


            IntPtr monoDataScan = System.Runtime.InteropServices.Marshal.AllocHGlobal(bmp.Width * bmp.Height); // allocate as 8bpp

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
            { bmp = new Bitmap(path); }
            catch (Exception e)
            {
                bmp = new Bitmap(1, 1);
                throw e;
            }


            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int bytesPerPixel = bmpData.Stride / bmp.Width;
            if (bytesPerPixel < 4)
            {
                bmp.UnlockBits(bmpData);
                bmp.Dispose();
                return new int[] { 0, 0, 0, 0 }; ;
            }


            IntPtr[] monoDataScans = new IntPtr[]
                {
                    System.Runtime.InteropServices.Marshal.AllocHGlobal(bmp.Width * bmp.Height), // allocate as 8bpp
                    System.Runtime.InteropServices.Marshal.AllocHGlobal(bmp.Width * bmp.Height),
                    System.Runtime.InteropServices.Marshal.AllocHGlobal(bmp.Width * bmp.Height),
                    System.Runtime.InteropServices.Marshal.AllocHGlobal(bmp.Width * bmp.Height),
                };

            unsafe
            {
                byte*[] planes = new byte*[4]
                { (byte*)monoDataScans[0], (byte*)monoDataScans[1], (byte*)monoDataScans[2], (byte*)monoDataScans[3] };
                byte* inputData = (byte*)bmpData.Scan0;

                for (int i = 0; i < bmp.Height * bmpData.Stride; i += bytesPerPixel)
                {
                    planes[0][i / bytesPerPixel] = inputData[i];
                    planes[1][i / bytesPerPixel] = inputData[i + 1];
                    planes[2][i / bytesPerPixel] = inputData[i + 2];
                    planes[3][i / bytesPerPixel] = inputData[i + 3];
                }
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
