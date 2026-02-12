namespace Plukliste.Readers;

public static class PluklistReaderFactory
{
    public static IPluklistReader Create(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLowerInvariant();

        if (extension == ".xml")
            return new XmlPluklistReader();

        if (extension == ".csv" || extension == ".txt")
            return new ScannerPluklistReader();

        throw new NotSupportedException($"File type not supported: {extension}");
    }
}
