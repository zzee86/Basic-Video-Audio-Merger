using System.Diagnostics;

namespace Video_Audio_Merger;
public class Helper
{
    public void RunFFmpeg(string args)
    {
        Console.WriteLine("\n===== Running FFMPEG =====\n");

        Process ffmpeg = new Process();
        ffmpeg.StartInfo.FileName = "ffmpeg";
        ffmpeg.StartInfo.Arguments = args;

        // Let FFmpeg use the current console
        ffmpeg.StartInfo.UseShellExecute = false;  
        ffmpeg.StartInfo.CreateNoWindow = false;   

        ffmpeg.Start();       
        ffmpeg.WaitForExit();
    }
}
