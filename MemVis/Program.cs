using System;

namespace MemVis
{
    internal static class Program
    {
        private static readonly BitmapMemoryManager Manager = new(MemoryUnit.Mb(100));

        static void Main(string[] args)
        {
            Console.WriteLine("first_fit - First fit");
            Console.WriteLine("best_fit - Best fit");
            Console.WriteLine("nru - Not recently used");
            Console.Write("Choose alg: ");

            string cmd = Console.ReadLine();

            if (cmd == "first_fit")
            {
                Manager.Algorithm = new FirstFit();

                Console.WriteLine("Pre allocate processes? y\\n");

                if (Console.ReadLine() == "y")
                {
                    PreAllocateProcesses();
                }

                FirstAlgorithmProcess();
                return;
            }

            if (cmd == "best_fit")
            {
                Manager.Algorithm = new BestFit();

                Console.WriteLine("Pre allocate processes? y\\n");

                if (Console.ReadLine() == "y")
                {
                    PreAllocateProcesses();
                }

                FirstAlgorithmProcess();
                return;
            }

            if (cmd == "nru")
            {
                Nru();
                return;
            }
        }

        private static void Nru()
        {
            Console.Clear();
            string input;

            Console.Write("Input frames count: ");
            int frames = int.Parse(Console.ReadLine());

            NRU nru = new(frames);

            Console.Write("Input frame: ");
            while ((input = Console.ReadLine()) != "-")
            {
                if (!int.TryParse(input, out var frame))
                {
                    Err("Invalid input");
                    continue;
                }

                nru.Push(new Frame { Number = frame });
                Console.Write("Input frame: ");
            }

            Console.WriteLine($"Faults: {nru.Faults}");
        }

        private static void PreAllocateProcesses()
        {
            for (int i = 0; i < 10; i++)
            {
                Manager.LoadToMemory(new Process(512 * (i + 1)));
            }
        }

        private static void FirstAlgorithmProcess()
        {
            Console.Clear();

            while (true)
            {
                RenderMenu();
                HandleInput();
            }
        }

        private static void HandleNewProcess()
        {
            Console.Write("Process size: ");

            if (!int.TryParse(Console.ReadLine(), out int memory) || memory <= 0)
            {
                Err("Invalid input");
                return;
            }

            Manager.LoadToMemory(new Process(memory));
        }

        private static void HandleKillProcess()
        {
            Console.Write("Process pid: ");

            if (!Guid.TryParse(Console.ReadLine(), out var pid))
            {
                Err("Invalid input");
                return;
            }

            foreach (var entry in Manager.Head)
            {
                if (entry.Type is MemoryBitmapEntryType.Process && entry.Process.Pid == pid)
                {
                    entry.Process.Kill();
                }
            }
        }

        private static void HandleProcessList()
        {
            foreach (var entry in Manager.Head)
            {
                if (entry.Type is MemoryBitmapEntryType.Process)
                {
                    Console.WriteLine($"PID: {entry.Process.Pid}. Size: {entry.Process.Size}");
                }
            }
        }

        private static void HandleView()
        {
            Console.WriteLine(MemoryBitmapViewBuilder.BuildView(Manager.Head));
        }

        static void HandleInput()
        {
            string input = Console.ReadLine();

            Action handler = input.ToLower() switch
            {
                "ps" => HandleProcessList,
                "kill" => HandleKillProcess,
                "view" => HandleView,
                "new" => HandleNewProcess,
                "cls" => Console.Clear,
                _ => () => Err("Invalid command")
            };

            handler();
        }

        static void RenderMenu()
        {
            Console.WriteLine(new string('-', 31));
            Console.WriteLine("|ps - List processes".PadRight(30) + '|');
            Console.WriteLine("|" + new string('-', 30));
            Console.WriteLine("|kill - Kill process by pid".PadRight(30) + '|');
            Console.WriteLine("|" + new string('-', 30));
            Console.WriteLine("|view - View memory bitmap".PadRight(30) + '|');
            Console.WriteLine("|" + new string('-', 30));
            Console.WriteLine("|new - Start new process".PadRight(30) + '|');
            Console.WriteLine(new string('-', 31));
        }

        public static void Err(string text)
        {
            var old = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ForegroundColor = old;
        }
    }
}
