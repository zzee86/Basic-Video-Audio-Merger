namespace Video_Audio_Merger;
public class VideoMerger
{
    public void MergeFiles(string[] args)
    {
        Console.WriteLine();
        Console.WriteLine("===== Merging Video + Audio Tracks =====");
        Console.WriteLine();
        Console.WriteLine("Enter path to video file:");

        string mainFile = "";
        if (args != null && args.Length >= 1)
        {
            mainFile = args[0];
            Console.WriteLine(mainFile);
        }
        else
            mainFile = Console.ReadLine()?.Trim('"');

        if (string.IsNullOrWhiteSpace(mainFile) || !File.Exists(mainFile))
        {
            Console.WriteLine("Error: Video file not found!");
            return;
        }

        Console.WriteLine("Enter paths to audio tracks, separated by commas:");
        string audioInput = "";
        if (args != null && args.Length > 1)
        {
            for(int i = 1; i < args.Length; i++)
            {
                audioInput += args[i] + ",";
            }
            audioInput = audioInput.TrimEnd(',');

            Console.WriteLine(audioInput);
        }
        else
            audioInput = Console.ReadLine();

        if (string.IsNullOrEmpty(audioInput) || audioInput.Length == 0)
        {
            Console.WriteLine("Error: You must provide at least one valid audio file!");
            return;
        }

        string[] audioFiles = audioInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                       .Select(a => a.Trim('"', ' ')).ToArray();

        var outputDir = Path.GetDirectoryName(mainFile) ?? Environment.CurrentDirectory;
        var nameWithoutExt = Path.GetFileNameWithoutExtension(mainFile);
        var outputFile = Path.Combine(outputDir, $"{nameWithoutExt}_merged.mp4");

        // Build FFmpeg input arguments
        string inputs = $"-i \"{mainFile}\" ";
        foreach (var audio in audioFiles)
        {
            inputs += $"-i \"{audio}\" ";
        }

        int totalAudioStreams = 1 + audioFiles.Length;

        // Merge all audio tracks into one
        string filter = "[0:a]";
        for (int i = 1; i < totalAudioStreams; i++)
        {
            filter += $"[{i}:a]";
        }
        filter += $"amerge=inputs={totalAudioStreams}[aout]";

        // only shows errors in the output
        var query = $"{inputs}-filter_complex \"{filter}\" -map 0:v -map [aout] -c:v copy -ac 2 \"{outputFile}\" -hide_banner -loglevel warning";

        var helper = new Helper();
        helper.RunFFmpeg(query);
        Console.WriteLine($"File saved to: {outputFile}");

        Console.WriteLine();
        Console.WriteLine("Press '1 convert to mp4 or any other key to return to main menu");
        char key = Console.ReadKey().KeyChar;
        Console.WriteLine();
        if (key == '1')
        {
            var videoConverter = new VideoConverter();
            videoConverter.ConvertVideo(new String[] { outputFile });
        }
    }
}
