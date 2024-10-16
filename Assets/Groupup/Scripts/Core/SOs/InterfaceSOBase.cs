using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Groupup
{
    /**
     * Baseclass for all interfaces
     */
    public class InterfaceSOBase : ScriptableObject
    {
        public string SceneName;
        public string Author;
        public string CreationDate;
        public string ContactEmail;
        public Object RootFolder;
        [PropertySpace(SpaceBefore = 10, SpaceAfter = 40)]
        [TextArea(10, 10)] public string Notes;
        
        
        public UnityAction OnSceneLoaded;
        public bool IsActive;

        public void SceneLoaded()
        {
            IsActive = true;
            OnSceneLoaded?.Invoke();
        }
    }
}