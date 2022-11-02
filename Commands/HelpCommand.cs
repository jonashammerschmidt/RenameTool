internal class HelpCommand
    {

        private static readonly string Logo = string.Join(
            Environment.NewLine,
            @"    ____                                ______            __",
            @"   / __ \___  ____  ____ _____ ___  ___/_  __/___  ____  / /",
            @"  / /_/ / _ \/ __ \/ __ `/ __ `__ \/ _ \/ / / __ \/ __ \/ /",
            @" / _, _/  __/ / / / /_/ / / / / / /  __/ / / /_/ / /_/ / /",
            @"/_/ |_|\___/_/ /_/\__,_/_/ /_/ /_/\___/_/  \____/\____/_/");

        public static void WriteHelp()
        {
            WriteLogo();
            Console.WriteLine("Commands:\n RenameTool <search> <replace>\n");
        }

        private static void WriteLogo()
        {
            ConsoleWriteLineColor(Logo);
            Console.WriteLine();
            Console.WriteLine("Version: " + GetAppVersion());
        }

        private static void ConsoleWriteLineColor(string text)
        {
            ConsoleColor consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkBlue;

            Console.WriteLine(text);

            Console.ForegroundColor = consoleColor;
        }

        private static string GetAppVersion()
        {
            return typeof(HelpCommand).Assembly.GetName().Version!.ToString();
        }
    }