using System;

namespace OrderProcessing.Domain.Products
{
    /// <summary>
    /// A book can either be a paper book, an e-book or an audio-book. If it is not a physical book, it must have a
    /// download URL.
    /// </summary>
    public class Book : Product
    {
        public enum Medium
        {
            Print,
            EBook,
            AudioBook
        }

        private static Product.Type MapToProductType(Medium bookMedium)
        {
            return (bookMedium == Medium.Print) ?
                Product.Type.PhysicalGood : Product.Type.DigitalGood;
        }

        public Medium BookMedium { get; }
        public string ISBN { get; }
        public Uri DownloadURL { get; }

        public Book(Medium medium, string stockKeepingUnit, string isbn, string title, Uri downloadURL = null) :
            base(MapToProductType(medium), stockKeepingUnit, productName: title)
        {
            if (medium == Medium.Print && downloadURL != null)
                throw new ArgumentException("Print books don't have a download URL", paramName: nameof(downloadURL));
            if (medium != Medium.Print && downloadURL == null)
                throw new ArgumentException("Digital medium books must have a download URL", paramName: nameof(downloadURL));

            this.BookMedium = medium;
            this.ISBN = isbn;
            this.DownloadURL = downloadURL;
        }
    }
}
