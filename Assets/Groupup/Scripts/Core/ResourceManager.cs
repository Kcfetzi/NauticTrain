using UnityEngine;
using System.Collections.Generic;

namespace Groupup
{
    /**
     * Container class for all groupup objects
     */
    public static class ResourceManager
    {
        public static List<SceneInfoPreset> InfoPresets { get; private set; } = new List<SceneInfoPreset>();
        public static List<InterfaceSOBase> Interfaces = new List<InterfaceSOBase>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            LoadFromResources();
        }

    #if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            LoadFromResources();
        }
    #endif

        public static void Refresh()
        {
            LoadFromResources();
        }
        
        // load all groupup objects
        private static void LoadFromResources()
        {
            Interfaces.Clear();
            InfoPresets.Clear();
            
            SceneInfoPreset[] infoPresetArray = Resources.LoadAll<SceneInfoPreset>("ScenePresets");
            InfoPresets.AddRange(infoPresetArray);

            InterfaceSOBase[] interfaceArray = Resources.LoadAll<InterfaceSOBase>("");
            Interfaces.AddRange(interfaceArray);
        }
        
        // get a specific interface
        public static T GetInterface<T>() where T : InterfaceSOBase
        {
            foreach (InterfaceSOBase channel in Interfaces)
            {
                var typedChannel = channel as T;
                if (typedChannel is not null)
                {
                    return typedChannel;
                }
            }

            Debug.LogError("Channel not found!");
            return null;
        }

        public static SceneInfoPreset GetSceneInfoPresetByName(string presetName)
        {
            foreach (SceneInfoPreset sceneInfoPreset in InfoPresets)
            {
                if (sceneInfoPreset.presetName == presetName)
                    return sceneInfoPreset;
            }
            
            Debug.LogError("SceneInfoPreset not found!");
            return null;
        }
    }
}