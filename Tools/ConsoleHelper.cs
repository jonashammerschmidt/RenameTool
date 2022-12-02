namespace RenameTool.Tools
{
    internal class ConsoleHelper
    {
        private static int writeSkips = 0;
        private static int lastLength = 0;

        public static void Rewrite(int skips, string line, params object[] args)
        {
            if (writeSkips++ > skips)
            {
                writeSkips = 0;
            }
            else
            {
                return;
            }

            line = string.Format(line, args);

            while (line.Length < lastLength)
            {
                line += " ";
            }

            if (line.Length > Console.WindowWidth)
            {
                line = line.Substring(0, Console.WindowWidth);
            }

            Console.Write("\r" + line);
            lastLength = line.Length;
        }

        public static void Rewrite(string line, params object[] args)
        {
            line = string.Format(line, args);

            while (line.Length < lastLength)
            {
                line += " ";
            }

            if (line.Length > Console.WindowWidth)
            {
                line = line.Substring(0, Console.WindowWidth);
            }

            Console.Write("\r" + line);
            lastLength = line.Length;
        }
    }
}