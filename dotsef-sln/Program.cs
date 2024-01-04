using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

var solutions = new DirectoryInfo(Environment.CurrentDirectory).EnumerateFiles("*.sln").ToArray();

if (solutions.Length is 0)
{
    Console.Error.WriteLine("No solution files in the current directory");
    return 0;
}

if (solutions.Length is 1)
{
    var info = new ProcessStartInfo(solutions[0].FullName)
    {
        UseShellExecute = true
    };

    Process.Start(info);

    return 0;
}

Console.Error.Write("\x1b[?25l"); // hide

const string check = "\x1b[32m\x1b[39m";

var choice = 0;
for (var i = 0; i < solutions.Length; i++)
{
    var prefix = choice == i ? check : " ";
    var display = $" {prefix} {solutions[i].Name}";
    Console.Error.WriteLine(display);
}

Console.Error.Write($"\x1b[0G\x1b[{solutions.Length - choice}A");

ConsoleKeyInfo key;

while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
{
    if (key.Key == ConsoleKey.J && choice + 1 < solutions.Length)
    {
        Console.Error.Write($"   \x1b[1B\x1b[0G {check}\u001b[0G");
        choice++;
    }
    else if (key.Key == ConsoleKey.K && choice - 1 >= 0)
    {
        Console.Error.Write($"   \x1b[1A\x1b[0G {check}\u001b[0G");
        choice--;
    }
    else if (key.Key is ConsoleKey.Escape or ConsoleKey.Q)
    {
        Environment.Exit(1);
    }
}

Console.Error.WriteLine("\x1b[?25h"); // show

var info2 = new ProcessStartInfo(solutions[choice].FullName)
{
    UseShellExecute = true
};

try
{
    Process.Start(info2);
}
catch (Exception e) when (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && e is Win32Exception { NativeErrorCode: 1223 })
{
    // Suppress errors
}
catch (Exception e)
{
    Console.Error.WriteLine($"An error occurred: {e.GetType().Name}");
    return 1;
}

return 0;