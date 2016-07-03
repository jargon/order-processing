using System;

namespace OrderProcessing.Domain.Products
{
    public class Video : Product
    {
        public Uri DownloadURL { get; }

        public Video(string stockKeepingUnit, string videoTitle, Uri downloadURL) :
            base(ProductType.DigitalGood, stockKeepingUnit, productName: videoTitle)
        {
            this.DownloadURL = downloadURL;
        }
    }
}
