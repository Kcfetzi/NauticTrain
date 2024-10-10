using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Groupup
{
    /**
     * This singleton is the controller for all scenes.
     * It can load and unload scenes.
     * This class needs to be in the root class, where it can translate between scenes via loadingscreen
     */
    public class SceneLoader : MonoBehaviour
    {
        private static SceneLoader instance;

        public static SceneLoader Instance => instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                DestroyImmediate(transform.parent ? transform.parent.gameObject : gameObject);
            }
            else
            {
                instance = this;
                if (transform.parent == null)
                    DontDestroyOnLoad(gameObject);
            }
            if (loadingCanvas)
                loadingCanvas.SetActive(false);
        }

        [SerializeField] private SceneInfoPreset startPreset;
        [SerializeField] private bool startWithLoadingscreen;

        private SceneInfoPreset _activePreset;

        // all scenes from this preset loaded?
        public bool PresetFullyLoaded => _activePreset.fullyLoaded;
        public UnityAction OnActivePresetFullyLoaded;

        private void Start()
        {
            if (startPreset)
            {
                LoadPresetByName(startPreset.presetName, startWithLoadingscreen);
            }
            else
                Debug.Log("No starting scenes available");
        }

        /**
         * Unloads and loads given scenes. Can show the loadingscreen while translating between loading and unloading
         */
        public void LoadScenes(List<InterfaceSOBase> scenesToLoad, bool showLoadingScreen)
        {
            List<AsyncOperation> loadingActions = SceneLoaderService.LoadScenes(scenesToLoad);
            
            if (showLoadingScreen)
                StartCoroutine(LoadingScreen(loadingActions));
        }

        public void LoadPresetByName(string presetName, bool showLoadingScreen)
        {
            _activePreset = ResourceManager.GetSceneInfoPresetByName(presetName);
            _activePreset.fullyLoaded = false;
            LoadScenes(_activePreset.scenes, showLoadingScreen);
        }

        [SerializeField] private GameObject loadingCanvas;
        [SerializeField] private Image progressImage;
        [SerializeField] private float startAndEndDelay = 0;

        private IEnumerator LoadingScreen(List<AsyncOperation> loadOperations)
        {
            float totalProgress = 0f;
            progressImage.fillAmount = totalProgress;
            loadingCanvas.SetActive(true);

            yield return new WaitForSeconds(startAndEndDelay);

            // Überprüfe den Ladefortschritt aller Szenen
            while (totalProgress < loadOperations.Count)
            {
                totalProgress = 0f;

                foreach (AsyncOperation asyncOperation in loadOperations)
                {
                    totalProgress += asyncOperation.progress == 1.0f ? 1 : 0;
                }

                // Berechne den Durchschnittsfortschritt
                float averageProgress = totalProgress / loadOperations.Count;

                // Aktualisiere das Bild entsprechend des Durchschnittsfortschritts
                progressImage.fillAmount = averageProgress;

                yield return null;
            }

            progressImage.fillAmount = 1;
            yield return new WaitForSeconds(startAndEndDelay);

            foreach (AsyncOperation asyncOperation in loadOperations)
            {
                asyncOperation.allowSceneActivation = true;
            }

            _activePreset.fullyLoaded = true;
            OnActivePresetFullyLoaded?.Invoke();

            loadingCanvas.SetActive(false);
        }
    }
}