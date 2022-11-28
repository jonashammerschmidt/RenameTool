namespace RenameTool.Tools
{
    public static class ArgumentParser
    {
        public static bool HasArgument(string[] args, params string[] argumentAlternatives)
        {
            return args.Any(argumentAlternatives.Contains);
        }

        public static string? ExtractArgument(string[] args, params string[] argumentAlternatives)
        {
            int index = Array.FindIndex(args, argumentAlternatives.Contains);
            if (index == -1 || args.Length <= index + 1)
            {
                return null;
            }

            return args[index + 1];
        }
    }
}