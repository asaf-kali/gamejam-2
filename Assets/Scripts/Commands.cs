using System.Collections.Generic;
using System.Linq;
using System;

public static class Commands
{
    private static Random r = new System.Random();
    public static readonly string[] allCommands = new string[] { "אאוס", "אדוניוס", "אדראנוס", "אורנוס", "אורסטוס", "אורפאוס",
     "אורקוס", "אייגיסתוס", "אינוויקטוס", "איניאוס", "איקריוס", "אסקלפיוס", "ארבוס", "ארוס", "אריסטיוס", "גנימדוס", "דדלוס",
      "דיומדוס", "דימוס", "היפנוס", "הליוס", "הרמאפרודיטוס", "הפייסטוס", "זפירוס", "חאריאטוס", "טיינרוס", "טראוס", "טרמיניוס",
       "יאנוס", "כריספוס", "לארוס", "לינוס", "מומוס", "מורוס", "מורפיאוס", "מירטילוס", "מרקוריוס", "נאופטולומוס", "נראוס", 
       "נרקיסוס", "סומנוס", "פברואוס", "פגסוס", "פובוס", "פונטוס", "פולינייקוס", "פילאדוס", "פילוקוקטוס", "פלוטוס", "פליסתנוס",
        "פניוס", "פרוטאוס", "פרומתאוס", "קווירינוס", "קראטוס", "תאומאוס", "תאנוס", "תנטוס", "אספסוס", "יעלטוס", "דליקסוס" };

    public const string COMMANDER = "COMMANDER";

    public static HashSet<string> RandomCommands(int size, params string[] except)
    {
        HashSet<string> pool = new HashSet<string>(allCommands.Except(except));
        if (pool.Count < size)
            throw new Exception("Unable to create set of size " + size + " with pool of only " + pool.Count + " commands");
        HashSet<string> commands = new HashSet<string>();
        while (commands.Count < size)
        {
            int index = r.Next(pool.Count);
            commands.Add(pool.ElementAt(index));
        }
        return commands;
    }

}