using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using MinecraftNametags;
using GorillaLibrary.Content.Utilities;
using GorillaLibrary.Models;
using MinecraftNametags.Behaviours;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Networking;

[assembly: MelonInfo(typeof(Mod), "Minecraft Nametags", "1.0.0", "Estatic & Mia")]
[assembly: MelonGame("Another Axiom", "Gorilla Tag")]

namespace MinecraftNametags;

public class Mod : MelonMod
{
    public const string SIGNIFICANCE_URL =
        "https://raw.githubusercontent.com/Estati/MinecraftNametags/refs/heads/main/significance.txt";
    
    public static Mod Instance;
    public AssetBundle Bundle;
    
    public Dictionary<string, Significance> SignificanceMapping;
    
    public override void OnLateInitializeMelon()
    {
        base.OnLateInitializeMelon();

        SignificanceMapping =  new Dictionary<string, Significance>();
        SetupSignificance();
        
        Instance = this;
        
        Bundle = AssetBundleUtility.LoadBundle(System.Reflection.Assembly.GetExecutingAssembly(), "MinecraftNametags.Resources.minecraftnametags");
        
        GorillaTagger.OnPlayerSpawned(OnPlayerSpawned);
    }

    public void SetupSignificance()
    {
        GorillaLibrary.Utilities.WebRequestUtility.SendRequest(SIGNIFICANCE_URL, new WebRequest()
        {
            Method = RequestMethod.Get,
        }, (string result) =>
        {
            string[] lines = result.Split("\n");
            foreach (string line in lines)
            {
                try
                {
                    string UID = line.Split(':')[0];
                    string SIGNIFICANCE = line.Split(':')[1];

                    MelonLogger.Msg("UserID: " + UID);
                    MelonLogger.Msg("Significance: " + SIGNIFICANCE);
                
                    switch (SIGNIFICANCE)
                    {
                        case "DYE":
                            SignificanceMapping[UID] = Significance.Boyfriend;
                            break;
                        case "GOLD":
                            SignificanceMapping[UID] = Significance.Golden;
                            break;
                        case "CMD":
                            SignificanceMapping[UID] = Significance.Developer;
                            break;
                        case "DIAMOND":
                            SignificanceMapping[UID] = Significance.Known;
                            break;
                    }
                }
                catch {} //I DON'T KNOW WHAT IT IS, AND WHAT'S CAUSING IT, BUT I DON'T CARE ANYMORE, FUCKASS INDEX OUT OF BOUNDS ERROR OH MY GOD MAN -mia
            }
        }, (Exception e) =>
        {
            MelonLogger.Error(e);
            MelonLogger.Error(e.ToString());
            MelonLogger.Error("Message: " + e.Message);
            MelonLogger.Error("StackTrace: "+e.StackTrace);
            MelonLogger.Error("Source: "+e.Source);
        });
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