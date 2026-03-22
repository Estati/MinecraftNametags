using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GorillaLibrary.Content.Utilities;
using GorillaLibrary.Extensions;
using GorillaLibrary.Utilities;
using MelonLoader;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MinecraftNametags.Behaviours;

public enum Significance
{
    VIM,
    Known,      //DIAMOND
    Developer,  //CMD
    AAC,        
    Boyfriend,  //DYE
    Golden      //GOLD
}

/// <summary>
/// "Please, if any of this looks horrible to you (It definitely does), PLEASE PR this, I'm not a good coder by any means I'm just trying my best :("  -mia
/// "Also, please someone find a better way to sync the hearts in Paintbrawl, i've practically had to bruteforce the syncing for it to not break or whatever." -mia
/// </summary>
public class Nametag : MonoBehaviour
{
    public static List<Nametag> All = new();
    
    public VRRig rig;

    public GameObject nametag;
    public Canvas canvas;

    public Sprite[] healthSprite;
    public Sprite[] significanceSprite;
    
    public GameObject speakerIcon;
    
    public TextMeshProUGUI nameText;
    public Image outline;

    public RigContainer? rigContainer;
    
    public GameObject paintBrawlHealthParent;
    public GameObject[] paintbrawlHealth;

    public bool loaded;
    
    public Image significanceIcon;
    
    public void Awake()
    {
        All.Add(this);
        
        rig = GetComponent<VRRig>();
        MelonLogger.Msg("Loading nametag object...");
        
        nametag = Instantiate(Mod.Instance.Bundle.LoadAsset<GameObject>("Nametag"));
        healthSprite = Mod.Instance.Bundle.LoadAssetWithSubAssets<Sprite>("hearts_sheet");
        significanceSprite = Mod.Instance.Bundle.LoadAssetWithSubAssets<Sprite>("iconsheet");
        
        OnLoadComplete();
    }
    public void OnLoadComplete()
    {
        nametag.transform.SetParent(rig.transform, false);
        canvas = nametag.transform.GetChild(0).GetComponent<Canvas>();
        
        speakerIcon = canvas.transform.Find("Speaker").gameObject;
        nameText = canvas.transform.Find("Nameplate").gameObject.GetComponent<TextMeshProUGUI>();
        outline = canvas.transform.Find("Outline").GetComponent<Image>();
        significanceIcon = canvas.transform.Find("Icon").GetComponent<Image>();
        
        //"this is done horribly ;-;" -mia
        paintBrawlHealthParent = canvas.transform.Find("Paintbrawl Health").gameObject;
        paintbrawlHealth =
        [
            paintBrawlHealthParent.transform.GetChild(0).gameObject,
            paintBrawlHealthParent.transform.GetChild(1).gameObject,
            paintBrawlHealthParent.transform.GetChild(2).gameObject,
        ];
        
        rig.OnColorChanged += color =>
        {
            UpdateState();
        };
        rig.OnNameChanged += container =>
        {
            UpdateState();
        };

        loaded = true;
    }

    public void OnEnable()
    {
        UpdateState();
    }

    public void OnDisable()
    {
        rigContainer = null;
    }
    
    public void Update()
    {
        if (!loaded) return;
        if (rigContainer)
            speakerIcon.SetActive(rigContainer.Voice.IsSpeaking);

        canvas.transform.eulerAngles = new Vector3(GorillaTagger.Instance.mainCamera.transform.eulerAngles.x,
            GorillaTagger.Instance.mainCamera.transform.eulerAngles.y, 0);
    }

    public static void UpdateAllPaintbrawl()
    {
        if (!PhotonNetwork.InRoom) return;
        if (GorillaGameManager.instance == null) return;
        if (GorillaGameManager.instance is not GorillaPaintbrawlManager) return;
        
        foreach (Nametag nametag in Nametag.All)
            nametag.UpdatePaintbrawlState();
    }
    
    public void UpdatePaintbrawlState()
    {
        if (paintBrawlHealthParent.activeSelf)
        {
            for (int i = 0; i < paintbrawlHealth.Length; i++)
            {
                var image = paintbrawlHealth[i].GetComponent<Image>();
                bool isActive = rig.paintbrawlBalloons.balloons[i].activeSelf;

                image.sprite = isActive ? healthSprite[0] : healthSprite[1];
            }
        }
    }
    
    public void UpdateState()
    {
        if (GorillaGameManager.instance == null) return;
        if (rigContainer == null)
        {
            try
            {
                rigContainer = RigUtility.GetRig(rig.Creator);
            }
            catch
            {
                return;
            }
        }
        paintBrawlHealthParent.SetActive(GorillaGameManager.instance is GorillaPaintbrawlManager); //"Probably not the best way but it works :thinking:" -mia
        UpdatePaintbrawlState();
        
        nameText.text = rig.playerText1.text;
        outline.color = rig.playerColor;

        significanceIcon.enabled = IsSignificant(rig);
    }
    
    public void SetSignificanceIcon(int index)
    {
        if (significanceIcon.sprite != significanceSprite[index])
        {
            MelonLogger.Msg("Set significance icon to index "+index);
            significanceIcon.sprite = significanceSprite[index];
        }
    }
    
    public bool IsSignificant(VRRig rig)
    {
        if (rig.GetCosmetics().items.Any(x => x.itemName == "LBANI."))
        {
            SetSignificanceIcon(3); //AAC
            return true;
        }

        if (Mod.Instance.SignificanceMapping.TryGetValue(rig.Creator.UserId, out Significance significance))
        {
            SetSignificanceIcon((int)significance);
            return true;
        }
        
        return false;
    }
}