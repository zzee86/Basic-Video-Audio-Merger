using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;

namespace Video_Audio_Merger;
public class VideoConverter
{
    public void ConvertVideo(string[] args)
    {
        Console.WriteLine();
        Console.WriteLine("===== Converting Video =====");
        Console.WriteLine();

        Console.WriteLine("Enter paths to videos, separated by commas:");

        string mainFile = "";
        if (args != null && args.Length >= 1)
        {
            foreach (var item in args)
                mainFile += item + ",";
            mainFile = mainFile.TrimEnd(',');
            Console.WriteLine(mainFile);
        }
        else
            mainFile = Console.ReadLine()?.Trim('"');

        mainFile = mainFile.TrimEnd(',');

        string[] videoFiles = mainFile.Split(',', StringSplitOptions.RemoveEmptyEntries)
                               .Select(a => a.Trim('"', ' ')).ToArray();

        var helper = new Helper();
        var dict = new HashSet<string>();

        Console.WriteLine($"\nDelete original file after conversion? [Y/N]");
        var key = Console.ReadKey().Key;


        Console.WriteLine($"\nOverwrite existing files? [Y/N]");
        var overwrite = Console.ReadKey().Key;

        var count = 0;
        foreach (var inputFile in videoFiles)
        {
            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"Error: File not found: {inputFile}");
                continue;
            }

            var outputDir = Path.GetDirectoryName(inputFile) ?? Environment.CurrentDirectory;
            dict.Add(outputDir);
            var nameWithoutExt = Path.GetFileNameWithoutExtension(inputFile);
            var outputFile = Path.Combine(outputDir, $"{nameWithoutExt}_converted.mp4");

            var finalOutput = "";
            if (key == ConsoleKey.Y)
                finalOutput = Path.Combine(outputDir, $"{nameWithoutExt}.mp4");

            string query = "";
            if (overwrite == ConsoleKey.Y)
                query = $"-i \"{inputFile}\" -c:v h264_amf -quality quality -b:v 0 -pix_fmt yuv420p -c:a aac -b:a 128k -y \"{outputFile}\"";
            else
                query = $"-i \"{inputFile}\" -c:v h264_amf -quality quality -b:v 0 -pix_fmt yuv420p -c:a aac -b:a 128k -n \"{outputFile}\"";

            helper.RunFFmpeg(query);

            if (File.Exists(outputFile))
            {

                if (key == ConsoleKey.Y)
                {
                    try
                    {
                        var creationTime = File.GetCreationTime(inputFile);
                        var lastWriteTime = File.GetLastWriteTime(inputFile);
                        var lastAccessTime = File.GetLastAccessTime(inputFile);

                        FileSystem.DeleteFile(inputFile, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

                        File.Move(outputFile, finalOutput);

                        // Preserve timestamps on the new file
                        File.SetCreationTime(finalOutput, creationTime);
                        File.SetLastWriteTime(finalOutput, lastWriteTime);
                        File.SetLastAccessTime(finalOutput, lastAccessTime);

                        Console.WriteLine($"Original file deleted, converted file saved to: {finalOutput}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to delete original file: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Original file kept, converted file saved to: {outputFile}");
                }

                count++;
                Console.WriteLine();
                Console.WriteLine($"===== Converted {count}/{videoFiles.Length} Videos =====");
            }
            else
            {
                Console.WriteLine($"Conversion failed for {inputFile}. Original file not deleted.");
            }
        }

        Console.WriteLine($"Open Output Folder? [Y/N]");
        var folder = Console.ReadKey().Key;
        if (folder == ConsoleKey.Y)
        {
            foreach (var item in dict)
            {
                Process.Start("explorer.exe", item);
            }
        }
    }
}

