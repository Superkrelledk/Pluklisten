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
            Lines = new List<PluklisteLine>()
        };

        foreach (var line in lines)
        {
            var parts = line.Split(';');

            pluklist.Lines.Add(new PluklisteLine
            {
                Amount = int.Parse(parts[0]),
                ProductID = parts[1],
                Title = parts[2],
                Type = parts[3]
            });
        }

        return pluklist;   // 🔴 DET HER MÅ IKKE MANGLE
    }
}
