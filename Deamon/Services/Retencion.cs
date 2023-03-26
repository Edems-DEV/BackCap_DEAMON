using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Services;
public class Retencion
{
    private int retencion;
    private int packageLimit;
    private string path;
    private List<string> data = new List<string>();

    public Retencion(int id, int destinationId, int retencion, int packageLimit)
    {
        string workingDirectory = Directory.GetCurrentDirectory();
        this.path = workingDirectory + $"/{id}_{destinationId}.txt";

        this.retencion = retencion;
        this.packageLimit = packageLimit;

        if (!File.Exists(this.path))
        {
            FileStream stream = File.Create(this.path);
            stream.Close();
        }

    }

    #region Full Backup retenion
    public void FullReadRetancion()
    {

        using (StreamReader reader = new StreamReader(this.path))
        {
            while (!reader.EndOfStream)
            {
                this.data.Add(reader.ReadLine());
            }
        }

        if (retencion == data.Count)
        {
            Directory.Delete(data[0], true);
            // jedno nastala chyba že se file odmítl smazat. Už jsem ji nezreplikoval. To Do try catch
            data.RemoveAt(0);
        }

    }

    public void WriteRetencion(string path)
    {
        using (StreamWriter writer = new(this.path))
        {
            foreach (string item in data)
            {
                writer.WriteLine(item);
            }
            writer.WriteLine(path);
        }
    }
    #endregion

    public void ReadRetencion()
    {
        using (StreamReader reader = new StreamReader(this.path))
        {
            while (!reader.EndOfStream)
            {
                this.data.Add(reader.ReadLine());
            }



            if (retencion * packageLimit == data.Count)
            {
                int limit = data.Count;
                for (int i = 0; i < limit / 2; i++)
                {
                    Directory.Delete(data[0], true);
                    data.RemoveAt(0);
                }
            }
        }
    }
}
