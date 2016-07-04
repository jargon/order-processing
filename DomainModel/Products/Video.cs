using System;

namespace OrderProcessing.Domain.Products
{
    /// <summary>
    /// A <see cref="Video"/> either represents a physical disc or an online video to be streamed. If it is not a
    /// physical video, it must have a streaming URL.
    /// </summary>
    public class Video : Product
    {
        public enum Medium
        {
            Disc,
            Online
        }

        private static Product.Type MapToProductType(Medium videoMedium)
        {
            return (videoMedium == Medium.Disc) ?
                Product.Type.PhysicalGood : Product.Type.DigitalGood;
        }

        public Uri StreamingURL { get; }

        public Video(Medium videoMedium, string stockKeepingUnit, string videoTitle, Uri streamingURL = null) :
            base(MapToProductType(videoMedium), stockKeepingUnit, productName: videoTitle)
        {
            if (videoMedium == Medium.Disc && streamingURL != null)
                throw new ArgumentException(paramName: nameof(streamingURL), message: "Videos on disc don't have a download URL");
            if (videoMedium == Medium.Online && streamingURL == null)
                throw new ArgumentException(paramName: nameof(streamingURL), message: "Online videos must have a download URL");

            this.StreamingURL = streamingURL;
        }
    }
}
