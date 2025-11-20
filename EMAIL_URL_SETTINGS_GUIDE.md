# Email URL Settings - H??ng d?n c?u hình

## T?ng quan

?ã tách riêng các URL dùng cho email thành m?t section ??c l?p `EmailUrlSettings` trong `appsettings.json` ?? d? qu?n lý và b?o trì h?n.

## Lý do thay ??i

### Tr??c ?ây:
1. **URL xác nh?n l?ch h?n**: Dùng `AppSettings.Issuer` (không rõ ràng và gây nh?m l?n)
2. **URL ??t l?ch Campaign**: Hardcode `null` ho?c `"#"` trong code (không linh ho?t)
3. **URL ??t l?ch chung**: Không có, hardcode `"#"`

### Sau khi thay ??i:
- T?t c? URL ???c qu?n lý t?p trung trong `EmailUrlSettings`
- D? dàng thay ??i URL khi tri?n khai (dev, staging, production)
- Code rõ ràng h?n và d? b?o trì

## C?u trúc m?i

### 1. File c?u hình: `appsettings.json`

```json
"EmailUrlSettings": {
    "AppointmentConfirmationUrl": "http://localhost:5173/confirmappointment",
    "CampaignBookingUrl": "http://localhost:5173/book-campaign",
    "GeneralBookingUrl": "http://localhost:5173/book-appointment"
}
```

### 2. Class config: `EmailUrlSettings.cs`

```csharp
public class EmailUrlSettings
{
    /// <summary>
    /// Base URL cho xác nh?n l?ch h?n (ví d?: http://localhost:5173/confirmappointment)
    /// Dùng ?? t?o link xác nh?n trong email
    /// </summary>
    public string AppointmentConfirmationUrl { get; set; } = string.Empty;

    /// <summary>
    /// Base URL cho ??t l?ch/ki?m tra Campaign (ví d?: http://localhost:5173/book-campaign)
    /// Dùng trong email thông báo campaign
    /// </summary>
    public string CampaignBookingUrl { get; set; } = string.Empty;

    /// <summary>
    /// Base URL cho ??t l?ch chung (ví d?: http://localhost:5173/book-appointment)
    /// Dùng trong email no-show và các link ??t l?ch chung khác
    /// </summary>
    public string GeneralBookingUrl { get; set; } = string.Empty;
}
```

## Chi ti?t s? d?ng

### 1. AppointmentConfirmationUrl
**Dùng trong:**
- `SendAppointmentConfirmationEmailAsync`: Email xác nh?n l?ch h?n m?i
- `SendAppointmentRescheduledEmailAsync`: Email thông báo ??i l?ch h?n

**Format URL:**
```
{AppointmentConfirmationUrl}?appointmentId={guid}&token={hmac_token}
```

**Ví d?:**
```
http://localhost:5173/confirmappointment?appointmentId=123e4567-e89b-12d3-a456-426614174000&token=abc123xyz
```

### 2. CampaignBookingUrl
**Dùng trong:**
- `SendCampaignPartIssueEmailAsync`: Email thông báo campaign/recall cho khách hàng

**Format URL:**
```
{CampaignBookingUrl}
```

**Ví d?:**
```
http://localhost:5173/book-campaign
```

**L?u ý:** 
- N?u không c?u hình (?? tr?ng), nút "Book an Inspection" s? không hi?n th? trong email
- Có th? truy?n `bookingUrl` parameter ?? override URL m?c ??nh

### 3. GeneralBookingUrl
**Dùng trong:**
- `SendAppointmentNoShowEmailAsync`: Email thông báo khách hàng v?ng m?t

**Format URL:**
```
{GeneralBookingUrl}
```

**Ví d?:**
```
http://localhost:5173/book-appointment
```

**L?u ý:** 
- N?u không c?u hình (?? tr?ng), nút "Book New Appointment" s? không hi?n th?

## C?u hình cho các môi tr??ng khác nhau

### Development (localhost)
```json
"EmailUrlSettings": {
    "AppointmentConfirmationUrl": "http://localhost:5173/confirmappointment",
    "CampaignBookingUrl": "http://localhost:5173/book-campaign",
    "GeneralBookingUrl": "http://localhost:5173/book-appointment"
}
```

### Staging
```json
"EmailUrlSettings": {
    "AppointmentConfirmationUrl": "https://staging.yourcompany.com/confirmappointment",
    "CampaignBookingUrl": "https://staging.yourcompany.com/book-campaign",
    "GeneralBookingUrl": "https://staging.yourcompany.com/book-appointment"
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

## Migration t? h? th?ng c?

### AppSettings.Issuer (KHÔNG còn dùng cho email)

**Tr??c ?ây:**
```csharp
var confirmUrl = $"{_appSettings.Issuer?.TrimEnd('/')}?appointmentId={appointmentId}&token={token}";
```

**Bây gi?:**
```csharp
var baseUrl = _emailUrlSettings.AppointmentConfirmationUrl?.TrimEnd('/');
var confirmUrl = $"{baseUrl}?appointmentId={appointmentId}&token={token}";
```

### L?u ý quan tr?ng:
- `AppSettings.Issuer` v?n ???c gi? l?i và dùng cho **JWT Token validation**
- **KHÔNG nên dùng** `AppSettings.Issuer` cho email URLs n?a ?? tránh nh?m l?n

## Checklist tri?n khai

- [x] T?o `EmailUrlSettings.cs` trong `OEMEVWarrantyManagement.Share/Configs/`
- [x] Thêm section `EmailUrlSettings` vào `appsettings.json`
- [x] ??ng ký `EmailUrlSettings` trong `Program.cs`
- [x] C?p nh?t `EmailService` ?? inject và s? d?ng `EmailUrlSettings`
- [x] C?p nh?t `AppointmentService` ?? inject và s? d?ng `EmailUrlSettings`
- [ ] C?p nh?t `appsettings.json` cho các môi tr??ng khác (staging, production)
- [ ] Test các email functions v?i URL m?i
- [ ] C?p nh?t frontend ?? handle các URL endpoints này

## L?i ích c?a gi?i pháp này

1. **Tách bi?t rõ ràng**: Email URLs riêng bi?t v?i JWT settings
2. **D? c?u hình**: Thay ??i URL ch? c?n s?a appsettings.json
3. **Linh ho?t**: Có th? có URL khác nhau cho t?ng lo?i email
4. **D? b?o trì**: Code clean h?n, ít hardcode
5. **An toàn h?n**: Validate URL tr??c khi g?i email
6. **Documentation t?t h?n**: M?i URL có comment gi?i thích rõ ràng

## Troubleshooting

### Email không hi?n th? nút action
**Nguyên nhân:** URL trong `EmailUrlSettings` ?? tr?ng ho?c null

**Gi?i pháp:** C?u hình URL t??ng ?ng trong `appsettings.json`

### Token validation failed
**Nguyên nhân:** ?ang dùng nh?m `EmailUrlSettings.AppointmentConfirmationUrl` thay vì `AppSettings.Issuer` cho JWT

**Gi?i pháp:** 
- JWT validation v?n dùng `AppSettings.Issuer` 
- Email URLs dùng `EmailUrlSettings`

### URL không ?úng môi tr??ng
**Nguyên nhân:** Quên c?p nh?t `appsettings.{Environment}.json`

**Gi?i pháp:** T?o file riêng cho t?ng environment ho?c dùng environment variables
