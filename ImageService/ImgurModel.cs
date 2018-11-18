using System;
using System.Collections.Generic;
using System.Text;

namespace Dombo.ServiceProvider.ImageService
{
    public class ImageItem
    {
        public string link { get; set; }
    }


    public class UploadedImage
    {


        public ImageItem[] data { get; set; }
        public bool success { get; set; }
        public int status { get; set; }
    }

    public class UploadedImageResult
    {
        public string[] uploaded { get; private set; }

        public UploadedImageResult(ImageItem[] uploadedImageItemList)
        {
            uploaded = new List<ImageItem>(uploadedImageItemList).ConvertAll(x => x.link).ToArray();
        }
    }


}
