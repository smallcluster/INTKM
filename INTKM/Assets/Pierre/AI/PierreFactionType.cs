using System.Collections;
using System.Collections.Generic;

public static class PierreFactionType
{
    public const string dogFaction = "D";
    public const string catFaction = "C";
    public const string lizardFaction = "L";
    public const string playerFaction = "P";

    public static bool AreAllies(string faction1, string faction2)
    {
        return (
            faction1.Contains(dogFaction) && faction2.Contains(dogFaction) ||
            faction1.Contains(catFaction) && faction2.Contains(catFaction) ||
            faction1.Contains(lizardFaction) && faction2.Contains(lizardFaction)
            );
    } 
}
