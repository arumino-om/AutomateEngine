﻿using NLua;
using NLua.Exceptions;

namespace AutomateEngine;

public static class Program
{
    public static void Main(string[] args)
    {
        using var lua = new Lua();
        if (args.Length > 0)
        {
            if (File.Exists(args[0])) lua.DoFile(args[0]);
            else
            {
                WriteError($"File not exist: {args[0]}");
                Interpreter(lua);
            }
        }
        else Interpreter(lua);
    }

    private static void Interpreter(Lua lua)
    {
        Console.WriteLine($"Automate Engine Interpreter (AE 1.0 / {lua["_VERSION"]})");
        while (true)
        {
            Console.Write(">> ");
            var expression = Console.ReadLine();
            if (string.IsNullOrEmpty(expression)) continue;
            if (expression == "exit") break;

            try
            {
                lua.DoString($"print({expression})");
            }
            catch (LuaScriptException le)
            {
                WriteError(le.Message);
            }
        }
        Console.WriteLine("Bye");
    }

    private static void WriteError(string value)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(value);
        Console.ResetColor();
    }
}