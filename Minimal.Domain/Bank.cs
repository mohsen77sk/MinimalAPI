namespace Minimal.Domain;

public class Bank
{
    public int Id { get; set; }

    public string Code { get; set; } = default!;

    public string Name { get; set; } = default!;

    public bool IsActive { get; set; }

}