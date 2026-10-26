namespace SpravaProjektovFirmy.Models;

public class Project
{
    public string Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Abbreviation { get; set; }
    public string Customer { get; set; }
}