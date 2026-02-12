using Plukliste.Models;
using System.Xml.Serialization;

namespace Plukliste.Readers;

public class XmlPluklistReader : IPluklistReader
{
    public Pluklist? Read(string filePath)
    {
        using FileStream file = File.OpenRead(filePath);
        var serializer = new XmlSerializer(typeof(Pluklist));
        return (Pluklist?)serializer.Deserialize(file);
    }
}

