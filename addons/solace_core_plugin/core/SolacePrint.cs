using System;
using System.Collections.Generic;
using Godot;

namespace Solace.addons.solace_core_plugin.core;

/// <summary>
/// Solace Core's Console/Logging utilities
/// </summary>
public static class SolacePrint
{
    public enum PrintType
    {
        Meta,
        NamedObject
    }

    private const string TagMeta = "[META]";
    private const string TagGodotObject = "[OBJ]";

    private static readonly Queue<string> PrintingQueue = new();
    private static readonly string Stamp = FormatStamp(Random.Shared.Next());

    public static void ApplyPrintQueue()
    {
        while (PrintingQueue.TryDequeue(out var s))
        {
            GD.Print(s);
        }
    }

    private static void EnqueuePrint(string s)
    {
        PrintingQueue.Enqueue(s);
    }


    public static void Print(string s, PrintType tagType)
    {
        EnqueuePrint($"{Stamp} {GetTag(tagType)} {s}");
    }

    private static string GetTag(PrintType tagType)
    {
        return tagType switch
        {
            PrintType.Meta => TagMeta,
            PrintType.NamedObject => TagGodotObject,
            _ => throw new ArgumentOutOfRangeException(nameof(tagType), tagType, null)
        };
    }

    private static string FormatStamp(int i)
    {
        // Formats given integer for a quick & short identification string. 
        return $"{i % 256 + 16:X}";
    }
}