using System.Diagnostics;
using Video_Audio_Merger;
class Program
{
    static void Main(string[] args)
    {
        do
        {
            var videoMerger = new VideoMerger();
            videoMerger.MergeFiles(args);

            Console.WriteLine("Press 'R' to merge another file or any other key to exit...");
            char key = Console.ReadKey().KeyChar;
            Console.WriteLine();

            if (key != 'R' && key != 'r')
                break;
            Console.Clear();

        } while (true);
    }
}
