using Video_Audio_Merger;
class Program
{
    static CancellationTokenSource cts = new CancellationTokenSource();

    static void Main(string[] args)
    {
        Task.Run(() => ListenKeys(cts.Token));
        do
        {
            Console.WriteLine("===== Main Menu =====");
            Console.WriteLine("Press ESC at any time to exit or 'C' to clear console...");
            Console.WriteLine();

            Console.WriteLine("Press '1' to merge a video + audio tracks OR Press '2' to convert a video to MP4");
            char selectMode = Console.ReadKey().KeyChar;
            Console.WriteLine();

            if (selectMode == '1')
            {
                var videoMerger = new VideoMerger();
                videoMerger.MergeFiles(args);
            }
            else if (selectMode == '2')
            {
                var videoConverter = new VideoConverter();
                videoConverter.ConvertVideo(args);
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        
        } while (true);
    }

    static void ListenKeys(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Escape)
                {
                    Console.WriteLine("\nExiting...");
                    Environment.Exit(0);
                }
            }
            Thread.Sleep(50);
        }
    }
}