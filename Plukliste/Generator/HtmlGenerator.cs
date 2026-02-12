using Plukliste.Models;
namespace Plukliste.Services;

public static class HtmlGenerator
{
    public static void Generate(Pluklist plukliste, string templateFile, string outputPath)
    {
        string template = File.ReadAllText(templateFile);

        template = template.Replace("[Name]", plukliste.Name)
                           .Replace("[Adresse]", plukliste.Adresse);

        string linesHtml = "";

        foreach (var item in plukliste.Lines)
        {
            linesHtml += $"<tr><td>{item.Amount}</td>" +
                         $"<td>{item.Type}</td>" +
                         $"<td>{item.ProductID}</td>" +
                         $"<td>{item.Title}</td></tr>\n";
        }

        template = template.Replace("[Plukliste]", linesHtml);

        File.WriteAllText(outputPath, template);
    }
}
