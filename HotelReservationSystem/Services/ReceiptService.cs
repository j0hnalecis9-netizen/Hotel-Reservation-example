using HotelReservationSystem.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HotelReservationSystem.Services;

public interface IReceiptService
{
    byte[] BuildPdf(ReceiptViewModel vm);
}

public class ReceiptService : IReceiptService
{
    public byte[] BuildPdf(ReceiptViewModel vm)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Content().Column(col =>
                {
                    col.Item().Text("Hotel Reservation Receipt").FontSize(22).Bold();
                    col.Item().Text($"Booking Ref: {vm.BookingReference}");
                    col.Item().Text($"Guest: {vm.GuestName}");
                    col.Item().Text($"Room: {vm.Room}");
                    col.Item().Text($"Check-in: {vm.CheckIn:yyyy-MM-dd}");
                    col.Item().Text($"Check-out: {vm.CheckOut:yyyy-MM-dd}");
                    col.Item().Text($"Nights: {vm.Nights}");
                    col.Item().Text($"Payment: {vm.PaymentMethod}");
                    col.Item().Text($"Add-ons: {string.Join(", ", vm.AddOns)}");
                    col.Item().Text($"Total: {vm.Total:C}").FontSize(16).Bold().FontColor(Colors.Blue.Medium);
                });
            });
        }).GeneratePdf();
    }
}
