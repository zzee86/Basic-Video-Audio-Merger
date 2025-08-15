using System.Diagnostics;

namespace Video_Audio_Merger;
public class VideoMerger
{
    public void MergeFiles()
    {
        Console.WriteLine("Enter path to video file:");
        string mainFile = Console.ReadLine()?.Trim('"');

        if (string.IsNullOrWhiteSpace(mainFile) || !File.Exists(mainFile))
        {
            Console.WriteLine("Error: Video file not found!");
            return;
        }

        Console.WriteLine("Enter paths to audio tracks, separated by commas:");
        string audioInput = Console.ReadLine();
        string[] audioFiles = audioInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                       .Select(a => a.Trim('"', ' ')).ToArray();

        if (audioFiles.Length == 0)
        {
            Console.WriteLine("Error: You must provide at least one valid audio file!");
            return;
        }

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
        var arguments = $"{inputs}-filter_complex \"{filter}\" -map 0:v -map [aout] -c:v copy -ac 2 \"{outputFile}\" -hide_banner -loglevel error";

        RunFFmpeg(arguments);
        Console.WriteLine($"File saved to: {outputFile}");
    }
    private void RunFFmpeg(string args)
    {
        Process ffmpeg = new Process();
        ffmpeg.StartInfo.FileName = "ffmpeg";
        ffmpeg.StartInfo.Arguments = args;
        ffmpeg.StartInfo.RedirectStandardError = true;
        ffmpeg.StartInfo.UseShellExecute = false;
        ffmpeg.StartInfo.CreateNoWindow = true;

        ffmpeg.Start();
        Console.WriteLine(ffmpeg.StandardError.ReadToEnd());
        ffmpeg.WaitForExit();
    }
}
