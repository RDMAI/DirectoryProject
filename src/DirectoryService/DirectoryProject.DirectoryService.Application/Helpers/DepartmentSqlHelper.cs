using DirectoryProject.FileService.Contracts.Dto;

namespace DirectoryProject.DirectoryService.Application.Helpers;
public static class DepartmentSqlHelper
{
    public static FileLocation ConvertStringToFileLocation(string logoString)
    {
        var values = logoString.Split('|');
        string fileId = values[0];
        string location = values[1];
        return new FileLocation(fileId, location);
    }
}
