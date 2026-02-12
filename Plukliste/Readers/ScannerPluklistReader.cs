using Plukliste.Models;

namespace Plukliste.Readers;

public class ScannerPluklistReader : IPluklistReader
{
    public Pluklist? Read(string filePath)
    {
        var lines = File.ReadAllLines(filePath);

        var pluklist = new Pluklist
        {
            Name = "Montør fra scanner",
            Type = "WELCOME",
            Lines = new List<Item>()
        };

        foreach (var line in lines)
        {
            var parts = line.Split(';');

            pluklist.Lines.Add(new Item
            {
                Amount = int.Parse(parts[0]),
                ProductID = parts[1],
                Title = parts[2],
                Type = Enum.Parse<ItemType>(parts[3])
            });
        }

        return pluklist;
    }
}
