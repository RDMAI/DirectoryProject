using DirectoryProject.DirectoryService.Application.DTOs;
using DirectoryProject.DirectoryService.Application.LocationHandlers.UpdateLocation;

namespace DirectoryProject.DirectoryService.WebAPI.Requests;

public record UpdateLocationRequest(
    string Name,
    AddressDTO Address,
    string TimeZone)
{
    public UpdateLocationCommand ToCommand(
        Guid id)
    {
        return new UpdateLocationCommand(id, Name, Address, TimeZone);
    }
}
