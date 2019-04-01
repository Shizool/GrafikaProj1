using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GrafikaProj
{
    class ImageCustomizator
    {
        BitmapSource sourceBitmap;
        BitmapSource customizedBitmap;
        int brightness;
        public ImageCustomizator()
        {

        }

        public void SetSource(BitmapSource source)
        {
            BitmapSource grayImage = new FormatConvertedBitmap(source, PixelFormats.Gray8, null, 0);
            this.sourceBitmap = grayImage;
            this.customizedBitmap = grayImage;
        }

        public BitmapSource GetCustomizedSource()
        {
            return customizedBitmap;
        }

        public void SetBrightness(int value)
        {
            this.brightness = value;
        }

        public void ApplyFilters()
        {
            if (sourceBitmap != null)
            {
                byte[] pixelsArray = new byte[sourceBitmap.PixelHeight * sourceBitmap.PixelWidth];
                sourceBitmap.CopyPixels(pixelsArray, sourceBitmap.PixelWidth, 0);
                for (int x = 0; x < pixelsArray.Length; x++)
                {
                    if (pixelsArray[x] + this.brightness < 0)
                    {
                        pixelsArray[x] = 0;
                    }
                    else if (pixelsArray[x] + this.brightness <= 255)
                    {
                        pixelsArray[x] = (byte)(pixelsArray[x] + this.brightness);
                    }
                    else
                    {
                        pixelsArray[x] = 255;
                    }
                }
                BitmapSource temp = BitmapSource.Create(sourceBitmap.PixelWidth, sourceBitmap.PixelHeight, sourceBitmap.DpiX, sourceBitmap.DpiY, PixelFormats.Gray8, null, pixelsArray, sourceBitmap.PixelWidth);
                this.customizedBitmap = temp;
            }
        }

    }
}
