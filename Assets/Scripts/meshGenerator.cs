using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * References: 
 * Brackeys - MESH GENERATION in Unity - Basics :https://www.youtube.com/watch?v=eJEpeUH1EMg
 * Brackeys - PROCEDURAL TERRAIN in Unity! - Mesh Generation https://www.youtube.com/watch?v=64NblGkAabk
 * Brackeys - MESH COLOR in Unity - Terrain Generation : https://www.youtube.com/watch?v=lNyZ9K71Vhc
 */


[RequireComponent(typeof(MeshFilter))]
public class meshGenerator : MonoBehaviour
{

    #region COLOR CHANGE VARIABLES

    //colors array
    Color[] colors;

    //gradient
    public Gradient gradient;

    //height
    float minTerrainHeight;
    float maxTerrainHeight;

    #endregion


    #region MESH VARIABLES

    [Header("UI")]
    public Slider XSlider;  // X Slider
    public Slider ZSlider;  // Z Slider
    public Slider IntensitySlider;  // intensity slider
    public TextMeshProUGUI XVal; //X text
    public TextMeshProUGUI ZVal; //Z text
    public TextMeshProUGUI IVal; //intensity text

    //perlin noise reference
    public PerlinNoiseGen perlin;

    //scale
    public float texScale = 20f;

    //stored vertices
    Vector3[] vertices;

    //stored triangles
    int[] triangles;

    //mesh
    Mesh mesh;

    //size of mesh
    public int Xsize = 20;
    public int Zsize = 20;

    //offset
    public float offsetX = 100f;
    public float offsetY = 100f;

    //how intense the noise will be
    public float intensity = 7f;


    #endregion


    #region START/UPDATE

    void Start()
    {
        //create new mesh
        mesh = new Mesh();

        //set mesh to mesh filter
        GetComponent<MeshFilter>().mesh = mesh;

        UpdateSliders();
        CreateShape();
    }

    void Update()
    {
        UpdateMesh();
    }

    #endregion


    #region MESH 

    public void UpdateAttributes(int attributeID)
    {
        switch (attributeID)
        {
            case 0:     // If case is 0, update x
                Xsize = int.Parse(XSlider.value.ToString());
                XVal.text = Xsize.ToString();
                break;

            case 1:     // If case is 1, update z 
                Zsize = int.Parse(ZSlider.value.ToString());
                ZVal.text = Zsize.ToString();
                break;

            case 2:     // If case is 2, update intensity
                intensity = int.Parse(IntensitySlider.value.ToString());
                IVal.text = intensity.ToString();
                break;
        }
        CreateShape();
    }

    public void UpdateSliders()
    {
        XVal.text = Xsize.ToString();
        ZVal.text = Xsize.ToString();
        IVal.text = intensity.ToString();

        XSlider.value = Xsize;
        ZSlider.value = Zsize;
        IntensitySlider.value = intensity;

    }

    public void changeVals( float newScale, int newX, int newZ)
    {
        texScale = newScale;
        CreateShape();
    }

    //creates the shape 
    void CreateShape()
    {
        //set vertices size
        vertices = new Vector3[(Xsize + 1) * (Zsize + 1)];

        //add vertices
        for (int i = 0, z = 0; z <= Zsize; z++)
        {
            for (int x = 0; x <= Xsize; x++)
            {
                //height w perlin noise
                float height = perlin.CalculateNoise(z, x) * intensity;

                //add vertice
                vertices[i] = new Vector3(x, height, z);

                if (height > maxTerrainHeight)
                    maxTerrainHeight = height;

                if (height < minTerrainHeight)
                    minTerrainHeight = height;

                i++;
            }
        }

        //Triangles
        triangles = new int[Xsize * Zsize * 6];
        for (int z = 0, vert = 0, tris = 0; z < Zsize; z++)
        {
            for (int x = 0; x < Xsize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + Xsize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + Xsize + 1;
                triangles[tris + 5] = vert + Xsize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
        SetColors();
    }



    #endregion


    #region FUNCTIONS

    //for setting the colors of mesh
    private void SetColors()
    {
        colors = new Color[vertices.Length];
        for (int i = 0, z = 0; z <= Zsize; z++)
        {
            for (int x = 0; x <= Xsize; x++)
            {

                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
    }

    //setting the new gradient 
    public void SetNewGradient(Gradient newGradient)
    {
        gradient = newGradient;
        if (vertices!= null)
        {
            SetColors();
        }
    }

    //Updates the mesh
    void UpdateMesh()
    {
        //clear the mesh before adding new values
        mesh.Clear();

        //set new values
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        //calculate normals (lighting)
        mesh.RecalculateNormals();
    }

    #endregion

}
