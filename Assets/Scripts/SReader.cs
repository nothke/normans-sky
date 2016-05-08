using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SReader
{
    public static string[] GetLines(string path, string key)
    {
        if (!File.Exists(path))
            Debug.LogWarning("SReader: File named " + path + " doesn't exist");

        string[] allLines = File.ReadAllLines(path);

        List<string> lines = new List<string>();

        int startInt = -1;


        for (int i = 0; i < allLines.Length; i++)
        {
            if (allLines[i] == "#" + key)
            {
                startInt = i + 1;
                break;
            }
        }

        if (startInt == -1)
        {
            Debug.LogWarning("SReader: Key #" + key + " not found");
            return null;
        }

        for (int j = startInt; j < allLines.Length; j++)
        {
            if (!allLines[j].StartsWith("//") && allLines[j] != "")
            {
                if (allLines[j].StartsWith("#"))
                    break;

                lines.Add(allLines[j]);
            }
        }

        return lines.ToArray();
    }

    public static string GetRandom(string[] lines)
    {
        if (lines == null) return "";
        if (lines.Length == 0) return "";

        return lines[Random.Range(0, lines.Length)];
    }
}
