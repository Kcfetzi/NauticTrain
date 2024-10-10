#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Groupup
{
    public static class SceneCreater
    {
        /**
         * Method called by editor script, if user click create in create scene.
         */
        public static void CreateScene(string sceneName, string authorName, string email, string notes, string path)
        {
            sceneName = $"{char.ToUpper(sceneName[0])}{sceneName[1..]}";
            
            CreateFolderAndScene(path, sceneName);
            CreateInterfaceScript(path, sceneName);
            CreateControllerScript(path, sceneName);
            CreateDevControllerScript(path, sceneName);

            PlayerPrefs.SetInt("groupup_create", 1);
            AssetDatabase.Refresh();
        }
        
        /**
         * Create the pre defined folder structure and create a scene in it
         */
        private static void CreateFolderAndScene(string path, string name)
        {
            string folderPath = Path.Combine(path, name);
            string scriptsPath = Path.Combine(folderPath, "Scripts");
            
            PlayerPrefs.SetString("groupup_rootPath", folderPath);

            // Create folder
            AssetDatabase.CreateFolder(path, name);
            AssetDatabase.CreateFolder(folderPath, "Scripts");
            AssetDatabase.CreateFolder(folderPath, "Prefabs");
            AssetDatabase.CreateFolder(folderPath, "Resources");
            AssetDatabase.CreateFolder(folderPath + "/Resources", "Interface");
            AssetDatabase.CreateFolder(scriptsPath, "Interface");
            AssetDatabase.CreateFolder(scriptsPath, "Dev");

            // Create scene
            string sceneName = name + ".unity";
            string scenePath = Path.Combine(folderPath, sceneName);
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(newScene, scenePath);
            AddSceneToBuild(scenePath);
        }
        

        /**
         * Create the controller script that is set into scene below in after script compile section
         */
        private static void CreateControllerScript(string path, string name)
        {
            string scriptName = name + "Controller";
            string interfaceName = name + "Interface";
            
            PlayerPrefs.SetString("groupup_controller", scriptName);

            string scriptContent = Templater.GetControllerCode(scriptName, interfaceName);

            string folderPath = Path.Combine(path, name, "Scripts");
            string scriptPath = Path.Combine(folderPath, scriptName + ".cs");

            File.WriteAllText(scriptPath, scriptContent);
        }

        /**
         * Create the interface script that is set into scene below in after script compile section.
         * The interface is a scriptableobject and is used to communicate with the scene
         */
        private static void CreateInterfaceScript(string path, string name)
        {
            string scriptName = name + "Interface";

            PlayerPrefs.SetString("groupup_interface", scriptName);
            
            string scriptContent = Templater.GetInterfaceCode(scriptName);

            string folderPath = Path.Combine(path, name, "Scripts", "Interface");
            string scriptPath = Path.Combine(folderPath, scriptName + ".cs");

            File.WriteAllText(scriptPath, scriptContent);
        }

        /**
         * Create a test controller script that is set into scene below in after script compile section
         */
        private static void CreateDevControllerScript(string path, string name)
        {
            string scriptName = name + "DevController";
            string interfaceName = name + "Interface";
            
            PlayerPrefs.SetString("groupup_devController", scriptName);

            string scriptContent = Templater.GetDevControllerCode(scriptName, interfaceName);

            string folderPath = Path.Combine(path, name, "Scripts", "Dev");
            string scriptPath = Path.Combine(folderPath, scriptName + ".cs");

            File.WriteAllText(scriptPath, scriptContent);
        }

        /**
         * The created scene needs to get add to the unity buildsettings in order to can get loaded
         */
        private static void AddSceneToBuild(string scenePath)
        {
            EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

            // Überprüfe, ob die Szene bereits im Build enthalten ist
            foreach (EditorBuildSettingsScene buildScene in buildScenes)
            {
                if (buildScene.path == scenePath)
                {
                    Debug.Log(scenePath + " is already in the build settings.");
                    return;
                }
            }

            // Füge die Szene zum Build hinzu
            EditorBuildSettingsScene newScene = new EditorBuildSettingsScene(scenePath, true);
            EditorBuildSettingsScene[] newBuildScenes = new EditorBuildSettingsScene[buildScenes.Length + 1];
            buildScenes.CopyTo(newBuildScenes, 0);
            newBuildScenes[newBuildScenes.Length - 1] = newScene;
            EditorBuildSettings.scenes = newBuildScenes;

            Debug.Log(scenePath + " has been added to the build settings.");
        }
        
        /**
         * This is called after Assetdatabase.Reload is triggerd.
         */
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void CreateAssetWhenReady()
        {
            if(EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += CreateAssetWhenReady;
                return;
            }
        
            EditorApplication.delayCall += CreateAssets;
        }
        
        /**
         * This method creates all assets from new compiled service.
         */
        private static void CreateAssets()
        {
            // was a new service created recently?
            if (PlayerPrefs.GetInt("groupup_create") != 1)
                return;
            
            // create new service
            if (PlayerPrefs.GetInt("groupup_create") == 1)
            {
                try
                {
                    // get all created strings from created scene
                    string interfaceName = PlayerPrefs.GetString("groupup_interface");
                    string controllerName = PlayerPrefs.GetString("groupup_controller");
                    string devControllerName = PlayerPrefs.GetString("groupup_devController");
                    string rootPath = PlayerPrefs.GetString("groupup_rootPath");

                    // create channel asset
                    InterfaceSOBase newChannel = ScriptableObject.CreateInstance(interfaceName) as InterfaceSOBase;
                    AssetDatabase.CreateAsset(newChannel, rootPath + "/Resources/Interface/" + interfaceName + ".asset");

                    // create container
                    GameObject szeneObjects = new GameObject("------------ SceneObjects ------------");
                    GameObject devObjects = new GameObject("--------- DevelopmentObjects ---------");

                    // create service controller and make a prefab from it
                    GameObject serviceController = new GameObject(controllerName);
                    serviceController.transform.parent = szeneObjects.transform;
                    serviceController.AddComponent(Type.GetType(controllerName));
                    PrefabUtility.SaveAsPrefabAssetAndConnect(serviceController,
                        rootPath + "/Prefabs/" + controllerName + ".prefab", InteractionMode.AutomatedAction);

                    // create dev controller and make a prefab from it
                    GameObject devController = new GameObject(devControllerName);
                    devController.transform.parent = devObjects.transform;
                    devController.AddComponent(Type.GetType(devControllerName));
                    PrefabUtility.SaveAsPrefabAssetAndConnect(devController,
                        rootPath + "/Prefabs/" + devControllerName + ".prefab", InteractionMode.AutomatedAction);
                    
                    // create sceneloader
                    GameObject sceneLoader = new GameObject("SceneLoader");
                    sceneLoader.transform.parent = devObjects.transform;
                    sceneLoader.AddComponent<SceneLoader>();
                    
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    EditorSceneManager.SaveOpenScenes();

                    AssetDatabase.SaveAssets();
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
            
            PlayerPrefs.SetInt("groupup_create", 0);
            EditorApplication.delayCall -= CreateAssets;
            ResourceManager.Refresh();
        }
        
    }
}

#endif
