// See https://aka.ms/new-console-template for more information
using EDISE_lab;
using EDISE_lab.Utils;

var fileNames = new List<string>();

foreach (var arg in args)
{
    var extension = Path.GetExtension(arg).ToLower();
    if (extension != ".h" && extension != ".cpp")
    {
        Console.WriteLine("Invalid file type");
        continue;
    }
    var index = fileNames.FindIndex(x => x.StartsWith(Path.GetFileNameWithoutExtension(arg)));
    if (extension == ".h" && index != -1)
    {
        fileNames[index] = arg;
    }
    else
    {
        fileNames.Add(arg);
    }
}
foreach (var file in fileNames)
{
    Rewriter rewriter = new Rewriter();
    var fileContents = FileService.ReadFile(file);
    rewriter.BuildTree(fileContents);
    var result = rewriter.Rewrite();
    FileService.CreateAndWriteToFile(Path.GetFileNameWithoutExtension(file)+".cs", result);
}
