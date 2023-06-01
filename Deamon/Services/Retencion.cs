using Deamon.Models;
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
    private Paths paths = new Paths();
    private protected Config config;

    public Retencion(int id, int destinationId, int retencion, int packageLimit, Config config)
    {
        this.path = paths.RoamingPath + @$"\Retencion_{destinationId}.txt";

        this.retencion = retencion;
        this.packageLimit = packageLimit;

        if (!File.Exists(this.path))
        {
            FileStream stream = File.Create(this.path);
            stream.Close();
        }

        this.config = config;
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
                //int limit = data.Count;
                for (int i = 0; i < packageLimit; i++)
                {
                    if(config.IsCompressed)
                        File.Delete(data[0]);
                    else
                        Directory.Delete(data[0], true);

                    data.RemoveAt(0);
                }
            }
        }
    }
}
