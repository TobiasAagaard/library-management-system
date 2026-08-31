namespace Lms.Application.Models;

public class PremiumUser : User
{
    public const int PremiumBorrowLimit = BorrowLimit + 1;
    public override bool BorrowBook(Book book) => TryBorrow(book, PremiumBorrowLimit);
}
