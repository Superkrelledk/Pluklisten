using Plukliste.Models;

namespace Plukliste.Readers;

public interface IPluklistReader
{
    Pluklist? Read(string filePath);
}

