namespace Plukliste.Readers;

public static class PluklistReaderFactory
{
    public static IPluklistReader Create(string filePath)
    {
        if (filePath.EndsWith(".xml"))
            return new XmlPluklistReader();

        if (filePath.EndsWith(".txt"))
            return new ScannerPluklistReader();

        throw new NotSupportedException("File type not supported");
    }
}
