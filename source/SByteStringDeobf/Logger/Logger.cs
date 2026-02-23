#region Logger

using System.Drawing;
using System.Threading;

public static class Logger
{
    private static int startColumn = 0;
    private static int prefixWidth = 10;

    public static bool Animated = false;
    public static int AnimationDelay = 1;

    private static void TypeWrite(string prefix, string message, Color color)
    {
        Colorful.Console.CursorLeft = startColumn;

        prefix = prefix.PadRight(prefixWidth);

        Colorful.Console.Write(prefix, Color.WhiteSmoke);

        if (!Animated)
        {
            Colorful.Console.WriteLine(message, color);
            return;
        }

        foreach (char c in message)
        {
            Colorful.Console.Write(c.ToString(), color);
            Thread.Sleep(AnimationDelay);
        }

        Colorful.Console.WriteLine();
    }

    public static void Info(string message)
    {
        TypeWrite("[INFO]", message, Color.Cyan);
    }

    public static void Success(string message)
    {
        TypeWrite("[SUCCESS]", message, Color.LimeGreen);
    }

    public static void Warn(string message)
    {
        TypeWrite("[WARN]", message, Color.Maroon);
    }

    public static void Error(string message)
    {
        TypeWrite("[ERROR]", message, Color.Red);
    }

    public static void Custom(string message, Color color)
    {
        TypeWrite("", message, color);
    }

    public static void Inline(string message, Color color)
    {
        TypeWrite("", message, color);
    }
}

#endregion