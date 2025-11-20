# Tóm t?t thay ??i - Email URL Management

## V?n ??
- URL cho các nút trong email ?ang b? **hardcode** ho?c dùng nh?m `AppSettings.Issuer`
- Có **2 lo?i link khác nhau**:
  1. **Xác nh?n l?ch h?n** (confirmation link v?i token)
  2. **??t l?ch Campaign/General** (booking link)

## Gi?i pháp
T?o **section riêng** `EmailUrlSettings` trong `appsettings.json` v?i 3 URL:

```json
"EmailUrlSettings": {
    "AppointmentConfirmationUrl": "http://localhost:5173/confirmappointment",
    "CampaignBookingUrl": "http://localhost:5173/book-campaign",
    "GeneralBookingUrl": "http://localhost:5173/book-appointment"
}
```

## Files ?ã thay ??i

### 1. **T?o m?i**
- `OEMEVWarrantyManagement.Share/Configs/EmailUrlSettings.cs` - Class config cho email URLs

### 2. **C?p nh?t**
- `appsettings.json` - Thêm section `EmailUrlSettings`
- `Program.cs` - ??ng ký `EmailUrlSettings`
- `EmailService.cs` - Inject và s? d?ng `EmailUrlSettings`
- `AppointmentService.cs` - Inject và s? d?ng `EmailUrlSettings`

## Mapping URLs v?i Email Types

| Email Type | URL Setting | Ví d? |
|------------|------------|-------|
| Appointment Confirmation | `AppointmentConfirmationUrl` | `http://localhost:5173/confirmappointment?appointmentId=...&token=...` |
| Appointment Rescheduled | `AppointmentConfirmationUrl` | `http://localhost:5173/confirmappointment?appointmentId=...&token=...` |
| Campaign Part Issue | `CampaignBookingUrl` | `http://localhost:5173/book-campaign` |
| Appointment No-Show | `GeneralBookingUrl` | `http://localhost:5173/book-appointment` |

## L?i ích

? **Tách bi?t rõ ràng** - Email URLs không còn l?n l?n v?i JWT settings
? **D? c?u hình** - Ch? c?n s?a `appsettings.json` khi deploy
? **Linh ho?t** - Có th? có URL khác nhau cho t?ng lo?i email
? **Clean code** - Không còn hardcode URL trong code
? **Safe** - Validate URL tr??c khi g?i, không show button n?u URL tr?ng

## Cách s? d?ng

### Development
```json
"EmailUrlSettings": {
    "AppointmentConfirmationUrl": "http://localhost:5173/confirmappointment",
    "CampaignBookingUrl": "http://localhost:5173/book-campaign",
    "GeneralBookingUrl": "http://localhost:5173/book-appointment"
}
```

### Production
```json
"EmailUrlSettings": {
    "AppointmentConfirmationUrl": "https://yourcompany.com/confirmappointment",
    "CampaignBookingUrl": "https://yourcompany.com/book-campaign",
    "GeneralBookingUrl": "https://yourcompany.com/book-appointment"
}
```

## L?u ý quan tr?ng

?? **AppSettings.Issuer v?n gi? nguyên** - Dùng cho JWT Token validation, KHÔNG dùng cho email
?? **N?u ?? tr?ng URL** - Button t??ng ?ng s? không hi?n th? trong email
?? **Test k?** - Sau khi deploy c?n test các email functions

## Next Steps

1. ? Code changes hoàn t?t và build thành công
2. ? C?p nh?t `appsettings.Production.json` v?i production URLs
3. ? Test các email functions trong dev environment
4. ? Deploy và verify trên staging/production
5. ? Update frontend ?? handle các URL endpoints
