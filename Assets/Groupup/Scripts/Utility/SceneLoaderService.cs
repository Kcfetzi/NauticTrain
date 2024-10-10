using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Groupup
{
    public static class SceneLoaderService
    {
        /**
         * Unloads and loads given scenes. Can show the loadingscreen while translating between loading and unloading
         */
        public static List<AsyncOperation> LoadScenes(List<InterfaceSOBase> scenesToLoad)
        {
            List<AsyncOperation> operations = LoadScenesAsync(scenesToLoad);
            return operations;
        }
        
        // unload given scenes, if scenes to unload is null all active scenes get unloaded
        private static void UnloadScenes(List<InterfaceSOBase> scenesToUnload)
        {
            foreach (InterfaceSOBase info in scenesToUnload)
            {
                info.IsActive = false;
                Scene scene = SceneManager.GetSceneByName(info.SceneName);
                if (scene.isLoaded)
                {
                    SceneManager.UnloadSceneAsync(scene);
                }
            }
        }
        
        // unload all scenes
        public static void UnloadAllScenes()
        {
            foreach (InterfaceSOBase info in ResourceManager.Interfaces)
            {
                info.IsActive = false;
                Scene scene = SceneManager.GetSceneByName(info.SceneName);
                if (scene.isLoaded)
                {
                    SceneManager.UnloadSceneAsync(scene);
                }
            }
        }
        
        // Scene should be added to given stack
        public static List<AsyncOperation> AddScenesAsync(List<InterfaceSOBase> scenesToLoad)
        {
            List<AsyncOperation> loadOperations = new List<AsyncOperation>();

            if (!SceneManager.GetSceneByName("RootScene").isLoaded)
                SceneManager.LoadScene("RootScene", LoadSceneMode.Additive);
            
            foreach (InterfaceSOBase info in scenesToLoad)
            {
                info.IsActive = false;
                if (SceneManager.GetSceneByName(info.SceneName).isLoaded)
                    continue;

                loadOperations.Add(SceneManager.LoadSceneAsync(info.SceneName, LoadSceneMode.Additive));
            }
            return loadOperations;
        }
        
        // Unloads all and setup new scenestack
        public static List<AsyncOperation> LoadScenesAsync(List<InterfaceSOBase> scenesToLoad)
        {
            List<AsyncOperation> loadOperations = new List<AsyncOperation>();
            
            if (!SceneManager.GetSceneByName("RootScene").isLoaded)
                SceneManager.LoadScene("RootScene", LoadSceneMode.Single);
            else
                UnloadAllScenes();

            foreach (InterfaceSOBase info in scenesToLoad)
            {
                info.IsActive = false;
                if (SceneManager.GetSceneByName(info.SceneName).isLoaded)
                    continue;

                loadOperations.Add(SceneManager.LoadSceneAsync(info.SceneName, LoadSceneMode.Additive));
            }

            return loadOperations;
        }
    }
}
