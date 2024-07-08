[Serializable]
public class CurrentUser
{
    public CurrentUser() { }
   
    public CurrentUser(string name, string password, string? birthdate, int online, string? ip)
    {
        UserName = name;
        Password = password;
        BirthDate = birthdate;
        Online = online;
        IP = ip;
    }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string? BirthDate { get; set; }
    public int Online { get; set; }
    public string? IP { get; set; }
    public override string ToString()
    {
        return $"{UserName}";
    }
}
