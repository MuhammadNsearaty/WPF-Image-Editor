using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AForge.Imaging.Filters;

namespace ImageEditor
{
    class Filters
    {
        private int lastCol;
        private Bitmap ApplyFilter(System.Drawing.Image img , ColorMatrix cmPicture)
        {
            Bitmap bmpInverted = new Bitmap(img.Width, img.Height);
            ImageAttributes ia = new ImageAttributes();
            ia.SetColorMatrix(cmPicture);
            Graphics g = Graphics.FromImage(bmpInverted);
            g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia);
            g.Dispose();
            return bmpInverted;
        }
       /* private unsafe Bitmap Blur(Bitmap image, Rectangle rectangle, Int32 blurSize)
        {
            Bitmap blurred = new Bitmap(image.Width, image.Height);
            using (Graphics graphics = Graphics.FromImage(blurred))
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            BitmapData blurredData = blurred.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                                                        ImageLockMode.ReadWrite, blurred.PixelFormat);
            int bitsPerPixel = Image.GetPixelFormatSize(blurred.PixelFormat);
            byte* scan0 = (byte*)blurredData.Scan0.ToPointer();
            for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;
                    for (int x = xx; (x < xx + blurSize && x < image.Width); x++)
                    {
                        for (int y = yy; (y < yy + blurSize && y < image.Height); y++)
                        {
                            byte* data = scan0 + y * blurredData.Stride + x * bitsPerPixel / 8;

                            avgB += data[0];
                            avgG += data[1];
                            avgR += data[2];
                            blurPixelCount++;
                        }
                    }
                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;
                    for (int x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++)
                    {
                        for (int y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++)
                        {
                            byte* data = scan0 + y * blurredData.Stride + x * bitsPerPixel / 8;
                            data[0] = (byte)avgB;
                            data[1] = (byte)avgG;
                            data[2] = (byte)avgR;
                        }
                    }
                }
            }
            blurred.UnlockBits(blurredData);
            return blurred;
        }*/
        public Bitmap ConfusionFilter(Image img)
        {
            ColorMatrix cmPicture = new ColorMatrix(new float[][]
            {
            new float[]{.393f+0.3f, .349f, .272f, 0, 0},
            new float[]{.769f, .686f+0.2f, .534f, 0, 0},
            new float[]{.189f, .168f, .131f+0.9f, 0, 0},
            new float[]{0, 0, 0, 1, 0},
            new float[]{0, 0, 0, 0, 1}
            });
            return ApplyFilter(img , cmPicture);
        }
        public Bitmap GammaController(Bitmap myBitMap, float value)
        {
            float gamma = 0.04f * value;
            Bitmap newBitmap = new Bitmap(myBitMap.Width, myBitMap.Height);
            Graphics g = Graphics.FromImage(newBitmap);
            ImageAttributes ia = new ImageAttributes();
            ia.SetGamma(gamma);
            g.DrawImage(myBitMap, new Rectangle(0, 0, myBitMap.Width, myBitMap.Height),
                0, 0, myBitMap.Width, myBitMap.Height, GraphicsUnit.Pixel, ia);
            g.Dispose();
            ia.Dispose();
            return newBitmap;
        }
        public Bitmap GrayFilter(Image img)
        {
            ColorMatrix cmPicture = new ColorMatrix(new float[][]
            {
                    new float[]{0.299f, 0.299f, 0.299f, 0, 0},
                    new float[]{0.587f, 0.587f, 0.587f, 0, 0},
                    new float[]{0.114f, 0.114f, 0.114f, 0, 0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{0, 0, 0, 0, 0}
            });
            return ApplyFilter(img, cmPicture);
        }
        public Bitmap FlashLightFilter(Image img)
        {
            ColorMatrix cmPicture = new ColorMatrix(new float[][]
            {
                      new float[]{1.1f, 0, 0, 0, 0},
                      new float[]{0, 1.1f, 0, 0, 0},
                      new float[]{0, 0, 1.1f, 0, 0},
                      new float[]{0, 0, 0, 1, 0},
                      new float[]{0, 0, 0, 0, 1}
            });
            return ApplyFilter(img,cmPicture);
        }
        public Bitmap BlueFrozenFilter(Image img)
        {
            ColorMatrix cmPicture = new ColorMatrix(new float[][]
            {
                   new float[]{1+0.3f, 0, 0, 0, 0},
                   new float[]{0, 1+0f, 0, 0, 0},
                   new float[]{0, 0, 1+5f, 0, 0},
                   new float[]{0, 0, 0, 1, 0},
                   new float[]{0, 0, 0, 0, 1}
            });
            return ApplyFilter(img,cmPicture);
        }
        public Bitmap BlueWinterFilter(Image img)
        {
            ColorMatrix cmPicture = new ColorMatrix(new float[][]
            {
                    new float[]{1,0,0,0,0},
                    new float[]{0,1,0,0,0},
                    new float[]{0,0,1,0,0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{0, 0, 1, 0, 1}
            });
           return ApplyFilter(img,cmPicture);
        }
        public Bitmap HueController(Image img , float red , float green , float blue)
        {
            ColorMatrix cmPicture = new ColorMatrix(new float[][]
            {
                    new float[]{1+red, 0, 0, 0, 0},
                    new float[]{0, 1+green, 0, 0, 0},
                    new float[]{0, 0, 1+blue, 0, 0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{0, 0, 0, 0, 1}
            });
            return ApplyFilter(img,cmPicture);
        }
       /* public  Bitmap AdjustContrast(Bitmap Image, float Value)
        {
            Value = (100.0f + Value) / 100.0f;
            Value *= Value;
            Bitmap NewBitmap = (Bitmap)Image.Clone();
            BitmapData data = NewBitmap.LockBits(
                new Rectangle(0, 0, NewBitmap.Width, NewBitmap.Height),
                ImageLockMode.ReadWrite,
                NewBitmap.PixelFormat);
            int Height = NewBitmap.Height;
            int Width = NewBitmap.Width;

            unsafe
            {
                for (int y = 0; y < Height; ++y)
                {
                    byte* row = (byte*)data.Scan0 + (y * data.Stride);
                    int columnOffset = 0;
                    for (int x = 0; x < Width; ++x)
                    {
                        byte B = row[columnOffset];
                        byte G = row[columnOffset + 1];
                        byte R = row[columnOffset + 2];

                        float Red = R / 255.0f;
                        float Green = G / 255.0f;
                        float Blue = B / 255.0f;
                        Red = (((Red - 0.5f) * Value) + 0.5f) * 255.0f;
                        Green = (((Green - 0.5f) * Value) + 0.5f) * 255.0f;
                        Blue = (((Blue - 0.5f) * Value) + 0.5f) * 255.0f;

                        int iR = (int)Red;
                        iR = iR > 255 ? 255 : iR;
                        iR = iR < 0 ? 0 : iR;
                        int iG = (int)Green;
                        iG = iG > 255 ? 255 : iG;
                        iG = iG < 0 ? 0 : iG;
                        int iB = (int)Blue;
                        iB = iB > 255 ? 255 : iB;
                        iB = iB < 0 ? 0 : iB;

                        row[columnOffset] = (byte)iB;
                        row[columnOffset + 1] = (byte)iG;
                        row[columnOffset + 2] = (byte)iR;

                        columnOffset += 4;
                    }
                }
            }
            NewBitmap.UnlockBits(data);
            return NewBitmap;
        }*/
        public Bitmap Emboss(Bitmap myBitMap)
        {
            Bitmap nB = new Bitmap(myBitMap.Width, myBitMap.Height);

            for (int x = 1; x <= myBitMap.Width - 1; x++)
            {
                for (int y = 1; y <= myBitMap.Height - 1; y++)
                {
                    nB.SetPixel(x, y, Color.DarkGray);
                }
            }

            for (int x = 1; x <= myBitMap.Width - 1; x++)
            {
                for (int y = 1; y <= myBitMap.Height - 1; y++)
                {
                    try
                    {
                        Color pixel = myBitMap.GetPixel(x, y);

                        int colVal = (pixel.R + pixel.G + pixel.B);

                        if (lastCol == 0) lastCol = (pixel.R + pixel.G + pixel.B);

                        int diff;

                        if (colVal > lastCol) { diff = colVal - lastCol; } else { diff = lastCol - colVal; }

                        if (diff > 100)
                        {
                            nB.SetPixel(x, y, Color.Gray);
                            lastCol = colVal;
                        }


                    }
                    catch (Exception) { }
                }
            }

            for (int y = 1; y <= myBitMap.Height - 1; y++)
            {

                for (int x = 1; x <= myBitMap.Width - 1; x++)
                {
                    try
                    {
                        Color pixel = myBitMap.GetPixel(x, y);

                        int colVal = (pixel.R + pixel.G + pixel.B);

                        if (lastCol == 0) lastCol = (pixel.R + pixel.G + pixel.B);

                        int diff;

                        if (colVal > lastCol) { diff = colVal - lastCol; } else { diff = lastCol - colVal; }

                        if (diff > 100)
                        {
                            nB.SetPixel(x, y, Color.Gray);
                            lastCol = colVal;
                        }

                    }
                    catch (Exception) { }
                }

            }

            return nB;
        }
        public  Bitmap Blur(Bitmap image)
        {
           GaussianBlur filter = new GaussianBlur(4, 20);
           return filter.Apply(image);
        }
        public Bitmap BrightnessAdjust(Bitmap myBitMap , float value)
        {
            BrightnessCorrection filter = new BrightnessCorrection((int)value);
            return filter.Apply(myBitMap);
        }
        public Bitmap JitterFilter(Bitmap img)
        {
            Jitter jitter = new Jitter();
            return jitter.Apply(img);
        }
        public Bitmap AutoCorrection(Bitmap img)
        {
            GammaCorrection filter = new GammaCorrection(0.5);
            return filter.Apply(img);
        }
        public Bitmap SepiaFilter(Bitmap img)
        {
            Sepia sepia = new Sepia();
            return sepia.Apply(img);
        }
        public Bitmap YCbCrFilter(Bitmap img)
        {
            YCbCrFiltering filter = new YCbCrFiltering();
            filter.Cb = new AForge.Range(-0.2f, 0.0f);
            filter.Cr = new AForge.Range(0.26f, 0.5f);
            return filter.Apply(img);
        }
        public Bitmap PosterizationFilter(Bitmap img)
        {
            SimplePosterization filter = new SimplePosterization();
            return filter.Apply(img);
        }
        public Bitmap WaterWaveFilter(Bitmap img)
        {
            WaterWave filter = new WaterWave();
            filter.HorizontalWavesCount = 10;
            filter.HorizontalWavesAmplitude = 5;
            filter.VerticalWavesCount = 3;
            filter.VerticalWavesAmplitude = 15;
            return filter.Apply(img);
        }
        public Bitmap RedMask(Bitmap img)
        {
            ChannelFiltering filter = new ChannelFiltering();
            filter.Red = new AForge.IntRange(0, 255);
            filter.Green = new AForge.IntRange(100, 255);
            filter.Blue = new AForge.IntRange(100, 255);
            return filter.Apply(img);

        }
        public Bitmap MeanFilter(Bitmap img)
        {
            Median filter = new Median();
            return filter.Apply(img);
        }
        public Bitmap YCbCrLinearFilter(Bitmap img)
        {
            YCbCrLinear filter = new YCbCrLinear();
            filter.InCb = new AForge.Range(-0.276f, 0.163f);
            filter.InCr = new AForge.Range(-0.202f, 0.500f);
            return filter.Apply(img);
        }
        public Bitmap SmothFilter(Bitmap img)
        {
            PointedMeanFloodFill filter = new PointedMeanFloodFill();
            filter.Tolerance = Color.FromArgb(150, 92, 92);
            filter.StartingPoint = new AForge.IntPoint(150, 100);
            return filter.Apply(img);
        }
        public Bitmap GaussianSharpenFilter(Bitmap img)
        {
            GaussianSharpen filter = new GaussianSharpen(4, 11);
            return filter.Apply(img);
        }
        public Bitmap ExtractChannelFilter(Bitmap img)
        {
            ExtractChannel filter = new ExtractChannel(AForge.Imaging.RGB.G);
            return filter.Apply(img);
        }
        public Bitmap RotateChannelsFilter(Bitmap img)
        {
            RotateChannels filter = new RotateChannels();
            return filter.Apply(img);
        }

        public Bitmap TransparecyController(Bitmap myBitmap , Byte alpha)
        {
            Bitmap res = new Bitmap(myBitmap);
            Color c = Color.Black;
            Color v = Color.Black;
            int av = 0;
            for (int i = 0; i < myBitmap.Width; i++)
            {
                for(int j = 0; j < myBitmap.Height; j++)
                {
                    c = myBitmap.GetPixel(i, j);
                    v = Color.FromArgb(alpha, c.R, c.G, c.B);
                    res.SetPixel(i, j, v);
                }
            }
            return res;
        }
        public Bitmap ResizeBicubic(Bitmap img ,int width ,int high){
            ResizeBicubic filter = new ResizeBicubic( width, high );

            Bitmap clone = new Bitmap(img.Width, img.Height,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            using (Graphics gr = Graphics.FromImage(clone))
            {
                gr.DrawImage(img, new Rectangle(0, 0, clone.Width, clone.Height));
            }

            return filter.Apply(clone);
        }

        public Bitmap ResizeBilinear(Bitmap img ,int width ,int high){
            ResizeBilinear filter = new ResizeBilinear(width,high);
            return filter.Apply(img);
        }
        public Bitmap ResizeNearestNeighbor(Bitmap img, int width , int high){
        ResizeNearestNeighbor filter = new ResizeNearestNeighbor( width, high );
        return filter.Apply(img);
        }

        }
   
}
 
