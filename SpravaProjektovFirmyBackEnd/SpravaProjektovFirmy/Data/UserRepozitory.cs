using System.Xml.Serialization;
using Microsoft.AspNetCore.Identity;
using SpravaProjektovFirmy.Dto;
using SpravaProjektovFirmy.Models;

namespace SpravaProjektovFirmy.Data;

public class UserRepozitory
{
    private readonly List<User> _userList;
    private readonly XmlSerializer _serializer = new (typeof(List<User>));
    private readonly string _path;
    private readonly PasswordHasher<User> _passwordHasher =  new ();

    public UserRepozitory(string path)
    {
        _path = path;
        _userList = GetUserList();
        
    }

    public User? RegisterUser(UserDto userDto)
    {
        var username = userDto.Username;
        var password = userDto.Password;
        
        var u = _userList.FindLast(x => x.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (u != null) return null;
        
        var user = new User
        {
            Id = _userList.Any() ? _userList.Max(x => x.Id) + 1 : 1,
            Name = username,
        };
        user.Password = _passwordHasher.HashPassword(user, password);
        _userList.Add(user);
        Save();
        return user;
    }

    public User? Login(UserDto userDto)
    {
        var username =  userDto.Username;
        var password = userDto.Password;

        var u = _userList.FirstOrDefault(x => x.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (u == null) return null;
        
        var check = _passwordHasher.VerifyHashedPassword(u, u.Password, password);
        if (check == PasswordVerificationResult.Failed) return null;

        if (check == PasswordVerificationResult.Success)
        {
            u.Password = _passwordHasher.HashPassword(u, password);
        }
        
        return u;
    }

    private List<User> GetUserList()
    {
        if (!File.Exists(_path)) return new List<User>();
        using var stream = File.OpenRead(_path);
        return (List<User>)_serializer.Deserialize(stream)!;
    }
    
    private void Save()
    {
        using (var stream = File.Create(_path)) _serializer.Serialize(stream, _userList);
    }
}