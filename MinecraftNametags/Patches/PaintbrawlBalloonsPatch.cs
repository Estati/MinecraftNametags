using HarmonyLib;
using MelonLoader;
using MinecraftNametags.Behaviours;

namespace MinecraftNametags.Patches;

[HarmonyPatch(typeof(PaintbrawlBalloons))]
public class PaintbrawlBalloonsPatch
{
    [HarmonyPatch("OnEnable")]
    [HarmonyPatch("PopBalloon")]
    public static void Postfix(PaintbrawlBalloons __instance)
    {
        Nametag.UpdateAllPaintbrawl();
    }
}