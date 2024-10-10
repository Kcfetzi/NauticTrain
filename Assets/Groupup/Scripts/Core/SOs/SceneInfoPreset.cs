using System.Collections.Generic;
using UnityEngine;

namespace Groupup
{
    /**
     * Container class for a group of scenes
     */
    public class SceneInfoPreset : ScriptableObject
    {
        // data variables
        public string presetName;
        public List<InterfaceSOBase> scenes;

        // if this preset is fully loaded
        public bool fullyLoaded = false;
        
        // editor needed variables
        public bool visible = false;
    }
}
