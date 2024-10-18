using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using ResourceManager = Groupup.ResourceManager;

public class MapCuller : MonoBehaviour
{
    [SerializeField] private Transform _player;

    private ScenarioInterface _scenarioInterface;
    
    private Terrain _terrainWithPlayer;

    private Dictionary<string, Terrain> _activeTerrains = new();

    // counts all genoperations
    private int _waitForTileGen;

    private bool _adressablesLoaded;

    private void Awake()
    {
        _scenarioInterface = ResourceManager.GetInterface<ScenarioInterface>();
        ResourceManager.GetInterface<ObjectsInterface>().OnNauticObjectSelected += SetSelectedNauticObject;
    }

    private void Start()
    {
        Addressables.InitializeAsync().Completed += (handle) => {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // init view on startpoint
                _adressablesLoaded = true;
            }
            else
            {
                Debug.LogError("Failed to initialize Addressables");
            }
        };
    }

    public void SetSelectedNauticObject(NauticObject player)
    {
        _player = player.transform;
        
        string playerTileName = GetPlayerTileName();
        
        InstantiatePlayerTileAddressable(playerTileName);
    }

    private void Update()
    {
        if(!_player || !_terrainWithPlayer)
            return;

        if (_waitForTileGen > 0 || !_adressablesLoaded)
            return;
        
        string playerTileName = GetPlayerTileName();
        
        // if last known playertile the same, nothing changes
        if (string.Equals(_terrainWithPlayer.name, playerTileName) && _activeTerrains.Count > 1)
            return;

        if (_activeTerrains.TryGetValue(playerTileName, out var terrain))
        {
            _terrainWithPlayer = terrain;
            BuildGameField(playerTileName);
        }
        else
        {
            if (_waitForTileGen == 0)
                InstantiatePlayerTileAddressable(playerTileName);
        }
    }

    private string GetPlayerTileName()
    {
        // get pos of player and tile by name is on at this moment
        Vector3 pos = _player.position;
        int playerTileX = Mathf.RoundToInt(pos.x / _scenarioInterface.TileSize.x);
        int playerTileZ = Mathf.RoundToInt(pos.z / _scenarioInterface.TileSize.z);
        
        playerTileX = math.clamp(playerTileX, 0, 63);
        playerTileZ = math.clamp(playerTileZ, 0, 63);
        
        return "Tile" + playerTileX + "," + playerTileZ;
    }

    private void BuildGameField(string centerTileName)
    {
        // calculate all the tiles u want
        List<string> wantedTiles = new List<string>();
        Vector3 tilePosition = GetTileSpawnPosition(centerTileName);
        int tileXMin = Mathf.RoundToInt((tilePosition.x - _scenarioInterface.ViewRange) / _scenarioInterface.TileSize.x);
        int tileXMax = Mathf.RoundToInt((tilePosition.x + _scenarioInterface.ViewRange) / _scenarioInterface.TileSize.x);
        int tileZMin = Mathf.RoundToInt((tilePosition.z - _scenarioInterface.ViewRange) / _scenarioInterface.TileSize.z);
        int tileZMax = Mathf.RoundToInt((tilePosition.z + _scenarioInterface.ViewRange) / _scenarioInterface.TileSize.z);

        tileXMin = math.clamp(tileXMin, 0, 63);
        tileXMax = math.clamp(tileXMax, 0, 63);
        tileZMin = math.clamp(tileZMin, 0, 63);
        tileZMax = math.clamp(tileZMax, 0, 63);
        // save tilenames in list
        for (int i = tileXMin; i <= tileXMax; i++)
        {
            for (int j = tileZMin; j <= tileZMax; j++)
            {
                wantedTiles.Add("Tile" + i + "," + j);
            }
        }

        // look what u got
        Dictionary<string, Terrain> newActiveTiles = new Dictionary<string, Terrain>();
        List<Terrain> obsoleteTerrains = new List<Terrain>();
        foreach (string activeTerrainKey in _activeTerrains.Keys)
        {
            if (wantedTiles.Contains(activeTerrainKey))
            {
                newActiveTiles.Add(activeTerrainKey, _activeTerrains[activeTerrainKey]);
                wantedTiles[wantedTiles.IndexOf(activeTerrainKey)] = "";
            }
            else
            {
                obsoleteTerrains.Add(_activeTerrains[activeTerrainKey]);
            }
        }

        // delete terrains that are obsolete
        foreach (Terrain obsoleteTerrain in obsoleteTerrains)
        {
            Destroy(obsoleteTerrain);
        }

        _activeTerrains = newActiveTiles;

        wantedTiles.Remove(centerTileName);
        // initiate those left
        foreach (string name in wantedTiles)
        {
            if (string.IsNullOrEmpty(name))
                continue;
            
            InstantiateAddressable(name);
        }
    }

    private Vector3 GetTileSpawnPosition(string tileName)
    {
        string coords = tileName.Substring(4, tileName.Length - 4);
        int indexOfComma = coords.IndexOf(",");
        int x = int.Parse(coords.Substring(0, indexOfComma));
        int z = int.Parse(coords.Substring(indexOfComma + 1, coords.Length - indexOfComma - 1));
        
        return new Vector3(x * _scenarioInterface.TileSize.x, -8, z * _scenarioInterface.TileSize.z);
    }
    
    private void InstantiatePlayerTileAddressable(string tileName)
    {
        // tile is the allready activetile, maybe same object call this funktion again or another obj in same tile
        if (_terrainWithPlayer && _terrainWithPlayer.name == tileName)
        {
            BuildGameField(tileName);
        }
        // if needed tile is actually loaded take it as active, but destroy old one before
        else if (_activeTerrains.ContainsKey(tileName))
        {
            _terrainWithPlayer = _activeTerrains[tileName];
            BuildGameField(tileName);
        }
        // last option is to spawn a new one
        else
        {
            _terrainWithPlayer = null;
            // mark that a generation prozess is running
            _waitForTileGen++;

            Vector3 spawnPos = GetTileSpawnPosition(tileName);

            string tileLocation = "Assets/Nautic/Scenario/Systems/Mobile_Messina/Terrains/" + tileName + ".prefab";
            if (AddressableResourceExists(tileLocation))
            {
                // Lade das Addressable asynchron
                AsyncOperationHandle<GameObject> handle =
                    Addressables.InstantiateAsync(tileLocation, Vector3.zero, Quaternion.identity, transform);

                // Warte, bis das Addressable geladen ist
                handle.WaitForCompletion();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Terrain terrain = handle.Task.Result.GetComponent<Terrain>();
                    _waitForTileGen--;

                    _terrainWithPlayer = terrain;

                    handle.Task.Result.transform.localPosition = spawnPos;
                    handle.Task.Result.name = handle.Task.Result.name.Replace("(Clone)", "");
                    _activeTerrains.Add(handle.Task.Result.name, terrain);
                }
                else
                {
                    Debug.LogError("Fehler beim Instanziieren des Addressable: " + handle.OperationException);
                    _waitForTileGen--;
                }
            }
            BuildGameField(tileName);
        }
    }
    
    
    private void  InstantiateAddressable(string tileName)
    {
        Vector3 spawnPos = GetTileSpawnPosition(tileName);
        
        string tileLocation = "Assets/Nautic/Scenario/Systems/Mobile_Messina/Terrains/" + tileName + ".prefab";
        
        // Überprüfe, ob das Asset erfolgreich geladen wurde
        if (AddressableResourceExists(tileLocation))
        {
            // mark that a generation prozess is running
            _waitForTileGen++;
            
            // Lade das Addressable asynchron
            AsyncOperationHandle<GameObject> loadHandle = Addressables.InstantiateAsync(tileLocation, Vector3.zero, Quaternion.identity, transform);

            loadHandle.WaitForCompletion();
            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Terrain terrain = loadHandle.Task.Result.GetComponent<Terrain>();

                loadHandle.Task.Result.transform.localPosition = spawnPos;
                loadHandle.Task.Result.name = loadHandle.Task.Result.name.Replace("(Clone)", "");
                _activeTerrains.Add(loadHandle.Task.Result.name, terrain);
            }
            else
            {
                //Debug.LogError("Fehler beim Instanziieren des Addressable: " + loadHandle.OperationException);
            }
            _waitForTileGen--;
        }
        else
        {
            //Debug.LogError("Fehler beim laden des Addressable: " + tileLocation);
        }
    }
    
    private bool AddressableResourceExists(object key) {
        foreach (var l in Addressables.ResourceLocators) {
            if (l.Locate(key, typeof(GameObject), out var locs))
                return true;
        }
        return false;
    }
}
