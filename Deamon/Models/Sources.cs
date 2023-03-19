using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Deamon.Models;

public class Sources
{
    public int Id { get; set; }

    public int Id_Config { get; set; }

    public string Path { get; set; }
}
