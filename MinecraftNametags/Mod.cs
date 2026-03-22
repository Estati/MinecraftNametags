using System.Linq;
using MelonLoader;
using MinecraftNametags;
using GorillaLibrary.Content.Utilities;
using MinecraftNametags.Behaviours;
using UnityEngine;

[assembly: MelonInfo(typeof(Mod), "Minecraft Nametags", "1.0.0", "Estatic & Mia")]
[assembly: MelonGame("Another Axiom", "Gorilla Tag")]

namespace MinecraftNametags;

public class Mod : MelonMod
{
    public static Mod Instance;
    public AssetBundle Bundle;
    
    public override void OnLateInitializeMelon()
    {
        base.OnLateInitializeMelon();

        Instance = this;
        
        Bundle = AssetBundleUtility.LoadBundle(System.Reflection.Assembly.GetExecutingAssembly(), "MinecraftNametags.Resources.minecraftnametags");
        
        GorillaTagger.OnPlayerSpawned(OnPlayerSpawned);
    }

    public void OnPlayerSpawned()
    {
        // "There's probably a WAY better method of doing this but this is the only way I know how to do this.. :(" -mia
        foreach (GameObject rigObject in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(x => x.name == "Gorilla Player Networked(Clone)"))
        {
            MelonLogger.Msg("Added nametag component to #"+rigObject.GetInstanceID());
            rigObject.AddComponent<Nametag>();
        }
    }
}