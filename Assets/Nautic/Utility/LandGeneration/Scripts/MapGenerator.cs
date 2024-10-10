#if UNITY_EDITOR

using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

/**
 * Class to create a terrain from given heightmap.
 * The gray values from the given image gets converted into heights.
 * More about how to use in the map sceneinfo.
 */

public class MapGenerator : OdinEditorWindow
{
    [MenuItem("NauticTrain/MapGenerator")]
    private static void OpenWindow()
    {
        GetWindow<MapGenerator>().Show();
    }
    
    public Texture2D HeightMapTexture;
    
    public Vector3 TerrainTileSize = new Vector3(115, 100, 115);
    public int TerrainHeightmapResolution = 33;
    
    [Button(ButtonSizes.Large)]
    void GenerateTerrain()
    {
        CreateFlatTerrainChunks();
        SetHeights();
    }
    
    private void CreateFlatTerrainChunks()
    {
        GameObject terrainHolder = GameObject.Find("Terrains");
        if (terrainHolder)
        {
            foreach (Transform child in terrainHolder.transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }
        else
        {
            terrainHolder = new GameObject("Terrains");
        }

        int tileCountY = HeightMapTexture.height / (TerrainHeightmapResolution - 1);
        int tileCountX = HeightMapTexture.width / (TerrainHeightmapResolution - 1);

        string terrainPath = "Assets/Nautic/Scenario/Systems/Mobile_Messina/Terrains";
        
        for (int i = 0; i < tileCountX; i++)
        {
            for (int j = 0; j < tileCountY; j++)
            {
                TerrainData data = new TerrainData();
                data.name = "Data" + i + "," + j;
                AssetDatabase.CreateAsset(data, terrainPath + "/Data/" + data.name + ".asset");
                
                Terrain tile = Terrain.CreateTerrainGameObject(data).GetComponent<Terrain>();
                tile.terrainData.heightmapResolution = TerrainHeightmapResolution + 1;
                tile.terrainData.size = TerrainTileSize;
                
                tile.name = "Tile" + i + "," + j;
                tile.transform.parent = terrainHolder.transform;
                tile.transform.position = new Vector3(i * TerrainTileSize.x, 0, j * TerrainTileSize.z);
                
                //PrefabUtility.SaveAsPrefabAsset(tile.gameObject, terrainPath);
            }
        }
    }

    private void SetHeights()
    {
        GameObject terrainHolder = GameObject.Find("Terrains");
        List<float[,]> mapChunks = CreateMapChunks();
        List<Terrain> extremaTerrains = new List<Terrain>();
        for (int i = 0; i < terrainHolder.transform.childCount; i++)
        {
            Terrain terrain = terrainHolder.transform.GetChild(i).GetComponent<Terrain>();
            TerrainData data = terrain.terrainData;
            bool isExtrema = true;
            
            float[,] mapChunk = mapChunks[i];
            
            float[,] newHeight = new float[TerrainHeightmapResolution, TerrainHeightmapResolution];
            
            for (int width = 0; width < TerrainHeightmapResolution; width++)
            {
                for (int height = 0; height < TerrainHeightmapResolution; height++)
                {
                    float tmp = mapChunk[height, width];
                    if (tmp > 0.01)
                        isExtrema = false;
                    if (tmp > 0.01)
                        tmp += .5f;

                    newHeight[width, height] = tmp;
                }
            }

            //newHeight = ApplyFilter(newHeight, TerrainHeightmapResolution, TerrainHeightmapResolution);
            if (isExtrema)
                extremaTerrains.Add(terrain);
            data.SetHeights(0, 0, newHeight);
            terrain.terrainData = data;
        }

        foreach (Terrain extremaTerrain in extremaTerrains)
        {
            DestroyImmediate(extremaTerrain.gameObject);
        }

        foreach (Transform mapchunk in terrainHolder.transform)
        {
            mapchunk.GetComponent<Terrain>().Flush();
        }
    }
    
    
    /**
     * Heights is a 2d vector [width, height] of a heightmap picture red values.
     * This vector is divided into 512x512 chunks. These chunks are used as heightvalues and new terrains are created with it.
     */
    private List<float[,]> CreateMapChunks()
    {
        float[,] heights = ReadHeightMap();
        List<float[,]> mapChunks = new List<float[,]>();

        int chunksInWidth = HeightMapTexture.width / (TerrainHeightmapResolution - 1);
        int chunksInHeight = HeightMapTexture.height / (TerrainHeightmapResolution - 1);

        for (int i = 0; i < chunksInWidth; i++)
        {
            for (int j = 0; j < chunksInHeight; j++)
            {
                float[,] heightmapChunk = new float[TerrainHeightmapResolution, TerrainHeightmapResolution];
                for (int x = 0; x < TerrainHeightmapResolution; x++)
                {
                    for (int y = 0; y < TerrainHeightmapResolution; y++)
                    {
                        int globalX = i * (TerrainHeightmapResolution - 1) + x;
                        int globalY = j * (TerrainHeightmapResolution - 1) + y;

                        // Anpassen der Randbedingungen
                        globalX = Mathf.Min(globalX, HeightMapTexture.width - 1);
                        globalY = Mathf.Min(globalY, HeightMapTexture.height - 1);

                        heightmapChunk[x, y] = heights[globalX, globalY];
                    }
                }
                mapChunks.Add(heightmapChunk);
            }
        }
        
        return mapChunks;
    }
    
    
    /**
     * Take given heightmap texture and store all red values as floats in array
     */
    private float[,] ReadHeightMap()
    {
        float[,] heights = new float[HeightMapTexture.width, HeightMapTexture.height];
        for (int i = 0; i < HeightMapTexture.width; i++)
        {
            for (int j = 0; j < HeightMapTexture.height; j++)
            {
                heights[i, j] = HeightMapTexture.GetPixel(i, j).grayscale;
            }
        }
        return heights;
    }

    // Apply gaussian filter to the given array
    float[,] ApplyFilter(float[,] heights, int arrayWidth, int arrayHeight)
    {
        float[,] filtedHeights = new float[arrayWidth, arrayHeight];
        for (int width = 0; width < arrayWidth; width++)
        {
            for (int height = 0; height < arrayHeight; height++)
            {
                int divider = 1;
                float pixelRValue = heights[width, height];
                if (pixelRValue > 0.4f)
                    continue;

                /**for (int i = width - (m_FilterSize / 2); i < width + (m_FilterSize / 2); i++)
                {
                    if (i < 0 || i > arrayWidth - 1)
                        continue;

                    for (int j = height - (m_FilterSize / 2); j < height + (m_FilterSize / 2); j++)
                    {
                        if (j < 0 || j > arrayHeight - 1)
                            continue;

                        divider++;
                        pixelRValue += heights[i, j];
                    }
                }*/

                filtedHeights[width, height] = 1 - pixelRValue / divider;
            }
        }

        return filtedHeights;
    }
}

#endif
