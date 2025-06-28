namespace DirectoryProject.SharedService.Core.DTOs;

public record LocationDTO(
    Guid Id,
    string Name,
    AddressDTO Address,
    string TimeZone);
