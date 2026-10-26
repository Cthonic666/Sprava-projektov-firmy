using System.Xml.Serialization;
using SpravaProjektovFirmy.Dto;
using SpravaProjektovFirmy.Models;

namespace SpravaProjektovFirmy.Data;

public class ProjectRepozitory
{
    private readonly string _path;
    private readonly XmlSerializer _serializer = new (typeof(List<Project>));
    private readonly List<Project> _projectList;
    
    public ProjectRepozitory(string path)
    {
        _path = path;
        _projectList = GetProjects();
    }

    public List<Project> GetAllProjects(int idUser)
    {
        return _projectList.Where(x => x.UserId == idUser).ToList();
    }
    
    
    public Project GetProjectById(string idProject, int idUser)
    {
        return ReturnProjectWithCheck(idProject, idUser);
    }

    public Project AddProject(ProjectDto project, int idUser)
    {
        var id = (_projectList.Count + 1).ToString();
        var p = new Project()
        {
            Id = "prj" + id,
            UserId = idUser,
            Name = project.Name,
            Abbreviation = project.Abbreviation,
            Customer = project.Customer
        };
        _projectList.Add(p);
        Save();
        return p;
    }

    public Project UpdateProject(ProjectDto project, int idUser, string idProject)
    {
        
        if (!_projectList.Any(x => x.Id == idProject)) return null;
        var p = ReturnProjectWithCheck(idProject, idUser);
        p.Name = project.Name;
        p.Abbreviation = project.Abbreviation;
        p.Customer = project.Customer;

        Save();
        return p;
    }

    public void DeleteProject(string idProject, int idUser)
    {
        var p = ReturnProjectWithCheck(idProject, idUser);
        _projectList.Remove(p);
        Save();
    }
    
    public List<Project> GetProjects()
    {
        if (!File.Exists(_path)) return new List<Project>();
        using var stream = File.OpenRead(_path);
        return (List<Project>)_serializer.Deserialize(stream)!;
    }

    private void Save()
    {
        using (var stream = File.Create(_path)) _serializer.Serialize(stream, _projectList);
    }

    private Project ReturnProjectWithCheck(string idProject, int idUser)
    {
        return _projectList.FirstOrDefault(x => x.Id == idProject && x.UserId == idUser)!;
    }
}