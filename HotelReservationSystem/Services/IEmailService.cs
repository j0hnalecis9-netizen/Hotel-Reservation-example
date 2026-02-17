namespace HotelReservationSystem.Services;

public interface IEmailService
{
    Task SendReceiptAsync(string email, string subject, string body);
}
