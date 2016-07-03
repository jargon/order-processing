namespace OrderProcessing.Domain.Products
{
    public enum BookType
    {
        PBook,
        EBook,
        Audiobook
    }

    public class Book : Product
    {
        private static ProductType MapToProductType(BookType bookType)
        {
            return (bookType == BookType.PBook) ?
                ProductType.PhysicalGood : ProductType.DigitalGood;
        }

        public BookType BookType { get; }
        public string ISBN { get; }

        public Book(BookType bookType, string stockKeepingUnit, string isbn, string title) :
            base(MapToProductType(bookType), stockKeepingUnit, productName: title)
        {
            this.BookType = bookType;
            this.ISBN = isbn;
        }
    }
}
