using System.Text;
using System.Text.RegularExpressions;
using DirectoryProject.DirectoryService.Domain.DepartmentValueObjects;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.ValueObjects;

namespace DirectoryProject.DirectoryService.Domain;

public class Department
{
    public Id<Department> Id { get; }
    public DepartmentName Name { get; private set; }
    public Id<Department>? ParentId { get; private set; }
    public Department? Parent { get; }
    public LTree Path { get; private set; }
    public short Depth { get; private set; }
    public int ChildrenCount { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }

    private List<DepartmentLocation> _departmentLocations = [];
    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations.AsReadOnly();

    private List<DepartmentPosition> _departmentPositions = [];
    public IReadOnlyList<DepartmentPosition> DepartmentPositions => _departmentPositions.AsReadOnly();

    public List<Department> Children { get; private set; }

    public Logo Logo { get; private set; }

    // for optimistic locking in EF Core
    private uint version;

    public static Result<Department> Create(
        Id<Department> id,
        DepartmentName name,
        Department? parent,
        DateTime createdAt)
    {
        var pathResult = CreatePath(name, parent?.Path);
        if (pathResult.IsFailure)
            return pathResult.Errors;

        return new Department(
            id: id,
            name: name,
            parentId: parent?.Id,
            path: pathResult.Value,
            depth: CalculateDepth(pathResult.Value),
            childrenCount: 0,
            createdAt: createdAt);
    }

    public Department Update(
        DepartmentName name,
        Id<Department>? parentId,
        string path)
    {
        Name = name;
        ParentId = parentId;
        Path = path;
        Depth = CalculateDepth(path);

        return this;
    }

    public Department UpdateLocations(IEnumerable<Id<Location>> locationIds)
    {
        _departmentLocations = locationIds.Select(lid => new DepartmentLocation(Id, lid)).ToList();

        return this;
    }

    public Department UpdateLogo(Logo logo)
    {
        Logo = logo;

        return this;
    }

    public Department IncreaseChildrenCount()
    {
        ChildrenCount++;
        return this;
    }

    public Department DecreaseChildrenCount()
    {
        if (ChildrenCount > 0)
            ChildrenCount--;
        return this;
    }

    public Department Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public Department Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public static short CalculateDepth(string path)
    {
        return (short)(path.Length - path.Replace(".", string.Empty).Length);
    }

    public static Result<LTree> CreatePath(DepartmentName name, string? parentPath)
    {
        var stringToSlug = ConvertStringToSlug(name.Value);

        if (parentPath is null)
            return Result<LTree>.Success(stringToSlug);

        if (parentPath.Contains(stringToSlug))
        {
            return Error.Conflict(
                "name.exists.in.path",
                "Department with the same name already exists in this branch");
        }

        return Result<LTree>.Success($"{parentPath}.{stringToSlug}");
    }

    public static string ConvertStringToSlug(string stringToSlug)
    {
        // replace all white spaces with -
        stringToSlug = new Regex(@"\s+").Replace(stringToSlug.Trim().ToLower(), "-");

        // replace all characters, specified in settings
        var sb = new StringBuilder();
        foreach (var ch in stringToSlug)
        {
            if (_replaceChars.TryGetValue(ch, out var replacement))
                sb.Append(replacement);
            else
                sb.Append(ch);
        }
        stringToSlug = sb.ToString();

        // remove all characters, except lower case latin, digits, '.' and '-'
        stringToSlug = new Regex(@"[^a-z0-9\-\.]").Replace(stringToSlug, string.Empty);

        return stringToSlug;
    }

    private static Dictionary<char, string> _replaceChars = [];
    public static UnitResult SetReplaceChars(Dictionary<char, string> replaceChars)
    {
        if (replaceChars.Count != 0)
            return Error.Failure("application.failure", "Could not set _replaceChars again");

        _replaceChars = replaceChars;
        return UnitResult.Success();
    }

    // EF Core
    private Department() { }

    private Department(
        Id<Department> id,
        DepartmentName name,
        Id<Department>? parentId,
        LTree path,
        short depth,
        int childrenCount,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        ParentId = parentId;
        Path = path;
        Depth = depth;
        ChildrenCount = childrenCount;
        CreatedAt = createdAt;
        UpdatedAt = CreatedAt;
    }
}
