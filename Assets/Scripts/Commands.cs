using System.Collections.Generic;
using System.Linq;
using System;

public static class Commands
{
    public static readonly string[] allCommands = new string[] { "אודיסיאה", "אוהמרוס", "זאוס", "אפרודיטה", "זפירוס", "איריס", "מורוס" };

    public const string COMMANDER = "COMMANDER";

    public static HashSet<string> RandomCommands(int size, params string[] except)
    {
        HashSet<string> pool = new HashSet<string>(allCommands.Except(except));
        if (pool.Count < size)
            throw new Exception("Unable to create set of size " + size + " with pool of only " + pool.Count + " commands");
        HashSet<string> commands = new HashSet<string>();
        System.Random r = new System.Random();
        while (commands.Count < size)
        {
            int index = r.Next(pool.Count);
            commands.Add(pool.ElementAt(index));
        }
        return commands;
    }

}