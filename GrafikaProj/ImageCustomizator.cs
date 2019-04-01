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

    }
}
