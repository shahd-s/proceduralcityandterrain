using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PerlinNoiseGen : MonoBehaviour
{

    [Header("Attributes")]
    public float texScale;
    public int texHeight;
    public int texWidth;

    [Header("UI")]
    public Slider HSlider;  // Height Slider
    public Slider WSlider;  // Width Slider
    public Slider SSlider;  // Scale Slider
    public TextMeshProUGUI HVal;
    public TextMeshProUGUI WVal;
    public TextMeshProUGUI SVal;

    [Header("Mesh")]
    public meshGenerator mesh;

    public Renderer perlinRenderer;

    private float offsetX, offsetY;

    private void Start()
    {
        UpdateSliders();
        GeneratePerlinNoise();
    }

    public void UpdateSliders()
    {
        HVal.text = texHeight.ToString();
        WVal.text = texWidth.ToString();
        SVal.text = texScale.ToString();

        HSlider.value = texHeight;
        WSlider.value = texWidth;
        SSlider.value = texScale;
    }

    public void UpdateAttributes(int attributeID)
    {
        switch(attributeID)
        {
            case 0:     // If case is 0, update height of texture
                
                texHeight = int.Parse(HSlider.value.ToString());
                HVal.text = HSlider.value.ToString();
                break;

            case 1:     // If case is 1, update width of texture

                texWidth = int.Parse(WSlider.value.ToString());
                WVal.text = WSlider.value.ToString();
                break;

            case 2:     // If case is 2, update scale of texture

                texScale = SSlider.value;
                SVal.text = SSlider.value.ToString("f1");
                break;
        }

        mesh.changeVals(texScale, texWidth, texHeight);
        GeneratePerlinNoise();
    }

    
    public void GeneratePerlinNoise()
    {
        offsetX = Random.Range(0f, 10000f);
        offsetY = Random.Range(0f, 10000f);

        Texture2D tex = new Texture2D(texWidth, texHeight);

        for(int i = 0; i < texWidth; i++)
        {
            for(int j = 0; j < texHeight; j++)
           {
                float noise = CalculateNoise(i, j);
                Color noiseColor = AssignColor(noise);
               tex.SetPixel(i, j, noiseColor);
           }
        }

       tex.Apply();
       perlinRenderer.material.mainTexture = tex;
       mesh.changeVals(texScale, texWidth, texHeight);
    }


    public float CalculateNoise(int x, int y)
    {
        float pointX = (float)x / texWidth * texScale + offsetX;
        float pointY = (float)y / texHeight * texScale + offsetY;
        float noiseMap = Mathf.PerlinNoise(pointX, pointY);
        return noiseMap;
    }


    public Color AssignColor(float noiseMap)
    {
       Color colormap = new Color(noiseMap, noiseMap, noiseMap);

       return colormap;
    }

}
