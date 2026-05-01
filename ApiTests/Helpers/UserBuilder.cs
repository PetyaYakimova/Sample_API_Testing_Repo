using Newtonsoft.Json;

public class UserBuilder
{
    private string _name = "Default Name";
    private string _username = "defaultuser";
    private string _email = "default@test.com";

    public UserBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public string Build()
    {
        var obj = new
        {
            name = _name,
            username = _username,
            email = _email
        };

        return JsonConvert.SerializeObject(obj);
    }
}