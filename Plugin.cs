using BepInEx;
using CarCompany.Patches;
using CarCompany.Scripts;
using GameNetcodeStuff;
using HarmonyLib;
using RuntimeNetcodeRPCValidator;
using UnityEngine;

namespace CarCompany
{
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "Mervus.CarCompany";
        public const string PLUGIN_NAME = "CarCompany";
        public const string PLUGIN_VERSION = "0.6.1";
    }
    
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(RuntimeNetcodeRPCValidator.MyPluginInfo.PLUGIN_GUID, RuntimeNetcodeRPCValidator.MyPluginInfo.PLUGIN_VERSION)]
    internal class CarModBase : BaseUnityPlugin
    {
        internal new static BepInEx.Logging.ManualLogSource Logger { get; private set; } = null!;
        
        private readonly Harmony _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        internal static CarModBase Instance;
        private NetcodeValidator netcodeValidator;
        
        internal static AssetBundle Bundle;
        internal static GameObject FiatPrefab;
        
        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;

            if (Instance  == null)
            {
                Instance = this;
            }
            _harmony.PatchAll(typeof(CarModBase));
            _harmony.PatchAll(typeof(PlayerControllerBPatch));
            _harmony.PatchAll(typeof(NetworkManagerPatch));
            _harmony.PatchAll(typeof(StartOfRound));
        
            FiatPrefab = null;
            
            string folderLocation = Instance.Info.Location;
            folderLocation = folderLocation.TrimEnd("CarCompany.dll".ToCharArray());
            Logger.LogInfo("CarMod: Folder location " +  folderLocation);
            Bundle = AssetBundle.LoadFromFile(folderLocation + "fiat");

            if (Bundle != null)
            {
                FiatPrefab = Bundle.LoadAsset<GameObject>("assets/fiat.prefab");
                Logger.LogInfo("Successfully loaded bundle, " + FiatPrefab);
            }
            else
            {
                Logger.LogError("Asset Bundle could not be loaded");
            }
            
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            
            netcodeValidator = new NetcodeValidator(PluginInfo.PLUGIN_GUID);
            netcodeValidator.PatchAll();
            
            netcodeValidator.BindToPreExistingObjectByBehaviour<HandlePlayerOnCar, PlayerControllerB>();
            netcodeValidator.BindToPreExistingObjectByBehaviour<MervusNetworkHandler, StartOfRound>();
        }
        
    }
}