namespace Sport_Retake_scheuer;

public class MyLogger
{
    public static void LogInfo(string message)
    {
        Console.WriteLine($"[INFO] {DateTime.Now}: {message}");
    }

    public static void LogWarning(string message)
    {
        Console.WriteLine($"[WARN] {DateTime.Now}: {message}");
    }

    public static void LogError(string message, Exception ex = null)
    {
        if (ex != null)
            Console.WriteLine($"[ERROR] {DateTime.Now}: {message} Exception: {ex}");
        else
            Console.WriteLine($"[ERROR] {DateTime.Now}: {message}");
    }
}

