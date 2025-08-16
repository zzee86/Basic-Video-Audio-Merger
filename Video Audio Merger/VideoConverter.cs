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

        if (string.IsNullOrWhiteSpace(mainFile) || !File.Exists(mainFile))
        {
            Console.WriteLine("Error: Video file not found!");
            return;
        }

        mainFile = mainFile.TrimEnd(',');

        string[] videoFiles = mainFile.Split(',', StringSplitOptions.RemoveEmptyEntries)
                               .Select(a => a.Trim('"', ' ')).ToArray();

        var helper = new Helper();
        foreach (var inputFile in videoFiles)
        {
            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"Error: File not found: {inputFile}");
                continue;
            }

            var outputDir = Path.GetDirectoryName(inputFile) ?? Environment.CurrentDirectory;
            var nameWithoutExt = Path.GetFileNameWithoutExtension(inputFile);
            var outputFile = Path.Combine(outputDir, $"{nameWithoutExt}_converted.mp4");

            string query = $"-i \"{inputFile}\" -c:v h264_amf -quality quality -b:v 0 -pix_fmt yuv420p -c:a aac -b:a 128k \"{outputFile}\" -hide_banner -loglevel warning";

            helper.RunFFmpeg(query);

            Console.WriteLine($"File saved to: {outputFile}");
        }
    }
}

