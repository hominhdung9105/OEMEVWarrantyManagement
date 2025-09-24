<<<<<<<< HEAD:OEMEVWarrantyManagement.Application/Dtos/TokenResponseDto.cs
﻿namespace OEMEVWarrantyManagement.Application.Dtos
========
﻿namespace OEMEVWarrantyManagement.API.Models.Response
>>>>>>>> Format-Response:OEMEVWarrantyManagement.API/Models/Response/TokenResponseDto.cs
{
    public class TokenResponseDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public required string EmployeeId { get; set; }
    }
}
