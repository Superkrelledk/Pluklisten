//Eksempel på funktionel kodning hvor der kun bliver brugt et model lag

using Plukliste.Models;
using Plukliste.Readers;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Plukliste;

class PluklisteProgram
{
    private static ConsoleColor _standardColor;
    private const string ExportDirectory = "export";
    private const string ImportDirectory = "import";
    private const string PrintDirectory = "print";

    static void Main()
    {
        //Arrange
        char readKey = ' ';
        List<string> files;
        var index = -1;
        ConsoleColor color = ConsoleColor.White;
        _standardColor = Console.ForegroundColor;
        Directory.CreateDirectory(ImportDirectory);
        Directory.CreateDirectory(ExportDirectory);
        Directory.CreateDirectory(PrintDirectory);


        files = LoadFiles();

        //ACT
        while (readKey != 'Q')
        {
            if (files.Count == 0)
            {
                Console.WriteLine("No files found.");
            }
            else
            {
                if (index == -1) index = 0;

                Console.WriteLine($"Plukliste {index + 1} af {files.Count}");
                Console.WriteLine($"\nfile: {files[index]}");

                var plukliste = LoadPluklistFromFile(files[index]);

                //print plukliste
                if (plukliste != null && plukliste.Lines != null)
                {
                    Console.WriteLine("\n{0, -13}{1}", "Name:", plukliste.Name);
                    Console.WriteLine("{0, -13}{1}", "Plukliste:", plukliste.Plukliste);

                    Console.WriteLine("\n{0,-7}{1,-9}{2,-20}{3}", "Antal", "Type", "Produktnr.", "Navn");
                    foreach (var item in plukliste.Lines)
                    {
                        Console.WriteLine("{0,-7}{1,-9}{2,-20}{3}", item.Amount, item.Type, item.ProductID, item.Title);
                    }
                }
            }

            //Print options
            Console.WriteLine("\n\nOptions:");
            PrintOptionsOutputText('Q', "uit");

            if (index >= 0)
            {
                PrintOptionsOutputText('A', "fslut plukseddel");
            }
            if (index > 0)
            {
                PrintOptionsOutputText('F', "orrige plukseddel");
            }
            if (index < files.Count - 1)
            {
                PrintOptionsOutputText('N', "æste plukseddel");
            }
            PrintOptionsOutputText('G', "enindlæs pluksedler");

            readKey = Console.ReadKey().KeyChar;
            readKey = char.ToUpper(readKey);
            Console.Clear();

            WithColor(ConsoleColor.Red, () =>
            {
                switch (readKey)
                {
                    case 'G':
                        files = LoadFiles();
                        index = -1;
                        Console.WriteLine("Pluklister genindlæst");
                        break;
                    case 'F':
                        if (index > 0) index--;
                        break;
                    case 'N':
                        if (index < files.Count - 1) index++;
                        break;
                    case 'A':
                        var plukliste = LoadPluklistFromFile(files[index]);

                        if (plukliste != null)
                        {
                            GeneratePrintHTML(plukliste, files[index]);
                        }

                        MoveFileToImport(files[index]);
                        Console.WriteLine($"Plukseddel {files[index]} afsluttet.");
                        files.Remove(files[index]);
                        if (index == files.Count) index--;
                        break;
                }
            });
        }
    }

    static void WithColor(ConsoleColor color, Action writeAction)
    {
        var originalColor = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = color;
            writeAction();
        }
        finally
        {
            Console.ForegroundColor = originalColor;
        }
    }

    static void PrintOptionsOutputText(char key, string description)
    {
        WithColor(ConsoleColor.Green, () => Console.Write(key));
        Console.WriteLine(description);
    }

    static List<string> LoadFiles()
    {
        return Directory.EnumerateFiles(ExportDirectory).ToList();
    }

    public static void GeneratePrintHTML(Pluklist plukliste, string xmlFilePath)
    {
        string templateFile = plukliste.Type switch
        {
            "OPGRADE" => "PRINT-OPGRADE.html",
            "OPSIGELSE" => "PRINT-OPSIGELSE.html",
            "WELCOME" => "PRINT-WELCOME.html",
            _ => "PRINT-WELCOME.html"
        };

        string template = File.ReadAllText(templateFile);

        var replacements = new Dictionary<string, string>
        {
            {"[Name]", plukliste.Name ?? "" },
            {"[Adresse]", plukliste.Adresse ?? "" }
        };

        foreach (var r in replacements)
        {
            template = template.Replace(r.Key, r.Value);
        }

        string linesHtml = "";

        foreach (var item in plukliste.Lines)
        {
            linesHtml += $"<tr><td>{item.Amount}</td>" +
                         $"<td>{item.Type}</td>" +
                         $"<td>{item.ProductID ?? ""}</td>" +
                         $"<td>{item.Title ?? ""}</td></tr>\n";
        }

        template = template.Replace("[Plukliste]", linesHtml);

        var fileName = Path.GetFileNameWithoutExtension(xmlFilePath) + ".html";
        var outputPath = Path.Combine(PrintDirectory, fileName);

        File.WriteAllText(outputPath, template, System.Text.Encoding.UTF8);
        
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = outputPath,
            UseShellExecute = true
        });
    }

    static void MoveFileToImport(string filePath)
    {
        var fileWithoutPath = Path.GetFileName(filePath);
        var destinationPath = Path.Combine(ImportDirectory, fileWithoutPath);
        File.Move(filePath, destinationPath, overwrite: true);
    }

    static Pluklist? LoadPluklistFromFile(string filePath)
    {
        try
        {
            var reader = PluklistReaderFactory.Create(filePath);
            return reader.Read(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading file {filePath}: {ex.Message}");
            return null;
        }
    }

}
