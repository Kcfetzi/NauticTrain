using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{
    [SerializeField] private Camera RenderCamera;
    [SerializeField] private RenderTexture TargetTexture;
    
    
    [Button(ButtonSizes.Large, Stretch = false), GUIColor(0, 1, 0)]
    public void RenderMap()
    {
        gameObject.SetActive(true);
        // Rendere das Bild einmal auf die Render-Textur
        RenderCamera.Render();
        
        RenderTexture.active = TargetTexture;
        
        Texture2D texture = new Texture2D(TargetTexture.width, TargetTexture.height, TextureFormat.RGBA32, false);
        
        texture.ReadPixels(new Rect(0, 0, TargetTexture.width, TargetTexture.height), 0, 0);
        texture.Apply();
        
        RenderTexture.active = null;
        
        byte[] bytes = texture.EncodeToJPG();

        // Erzeuge den Pfad, um die Datei zu speichern
        string filePath = "radar_map.jpg";

        // Speichere das Byte-Array als PNG-Datei
        File.WriteAllBytes(filePath, bytes);
        
        gameObject.SetActive(false);
    }
    
}
