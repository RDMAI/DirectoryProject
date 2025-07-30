namespace InventoryService.WepAPI.Domain;

public class Box
{
    public static readonly string[] PossibleBoxSizes = ["S", "M", "L"];

    public Guid Id { get; private set; }
    public string Size { get; private set; }
    public bool IsActive { get; private set; }

    public Box(
        Guid id,
        string size,
        bool isActive)
    {
        Id = id;
        Size = size;
        IsActive = isActive;
    }

    // EF Core
    private Box() { }
}
