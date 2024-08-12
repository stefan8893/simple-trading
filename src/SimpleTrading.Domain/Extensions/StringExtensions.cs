﻿using System.Runtime.CompilerServices;

namespace SimpleTrading.Domain.Extensions;

public static class StringExtensions
{
    public static string LocalizeMe(this string s,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        var initialColor = Console.ForegroundColor;

        Console.Write("Please localize:");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"{filePath} -> ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"{memberName}:${lineNumber}");
        Console.WriteLine("");

        Console.ForegroundColor = initialColor;

        return s;
    }
}