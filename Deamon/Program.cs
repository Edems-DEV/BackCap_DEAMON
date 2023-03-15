using System.Net.Http.Json;

namespace Deamon;

internal class Program
{
    static async void Main(string[] args)
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("http://localhost:5035");

        //User user = await client.GetFromJsonAsync<User>("/api/users/1");
        //Console.WriteLine(user.Name);
    }
}
