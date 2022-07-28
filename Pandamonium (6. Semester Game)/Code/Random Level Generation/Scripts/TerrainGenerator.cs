using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    //[System.Serializable]
    //public struct TextureThreshold
    //{
    //    public Texture2D tex;
    //    [Range(0, 1f)] public float threshold;
    //}

    public RenderTexture renderText;

    [Header("Terrain")]
    public Terrain terrain;
    public int terrainDimensions = 512;
    public int pathHeight = 10;
    public int hillHeight = 20;
    public int pathWidth = 10;

    [Header("TerrainTextures")]
    //public TextureThreshold[] texThresholds;
    public Texture2D grass;
    public Texture2D path;
    public Texture2D stone;

    [Header("Blur")]
    public int blurRadius = 1;
    public int blurIterations = 1;

    private Texture2D heightmap;
    private LinearBlur blur = new LinearBlur();

    //perlin noise
    [System.Serializable]
    public struct PerlinPass
    {
        public float perlinScale;
        [Range(0, 1f)] public float intensity;
        public float positionOffset;
        public bool extremeHills;
        [Range(0, 1f)] public float hillThreshold;
    }

    [Header("Perlin")]
    public PerlinPass[] perlinPasses;
    public float PerlinHeightOffset { get => (float)pathHeight / (float)hillHeight; }

    private int alphamapRes;
    private float[,,] splatMapData;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            RegenerateTerrain();
        }
    }

    public void RegenerateTerrain()
    {
        float rndPerlinOffset = Random.Range(0, 10000f);

        terrain.terrainData.size = new Vector3(terrainDimensions, hillHeight, terrainDimensions);
        TerrainLayer[] newTextures = new TerrainLayer[3];
        newTextures[0] = new TerrainLayer();
        newTextures[0].diffuseTexture = grass;
        newTextures[1] = new TerrainLayer();
        newTextures[1].diffuseTexture = path;
        newTextures[2] = new TerrainLayer();
        newTextures[2].diffuseTexture = stone;
        terrain.terrainData.terrainLayers = newTextures;

        alphamapRes = terrain.terrainData.alphamapResolution;
        splatMapData = terrain.terrainData.GetAlphamaps(0, 0, alphamapRes, alphamapRes);

        heightmap = renderText.ToTexture2D();
        Color[] mapPixels = heightmap.GetPixels(0);
        float[,] heightMapData = new float[513, 513];
        
        int index = 0;

        for (int y = 0; y < 513; y++)
        {
            for (int x = 0; x < 513; x++)
            {
                float heightValue = mapPixels[index].r;

                if (heightValue >= 1f) //perlin only affects the top layer
                {
                    heightValue *= PerlinHeightOffset;
                    foreach(var pass in perlinPasses)
                    {
                        if(pass.extremeHills) heightValue += GetPerlinValue01(x, y, pass.hillThreshold, pass.intensity, pass.perlinScale, pass.positionOffset + rndPerlinOffset) * (1 - PerlinHeightOffset);
                        else heightValue += GetPerlinValue(x, y, pass.intensity, pass.perlinScale, pass.positionOffset+ rndPerlinOffset) * (1 - PerlinHeightOffset);
                    }

                    SetSplatValue(y, x, 0);
                }
                else/* if (heightValue >= PerlinHeightOffset)*/
                {
                    SetSplatValue(y, x, 1);
                }
                //else
                //{
                //    SetSplatValue(y, x, 2);
                //}

                heightMapData[y, x] = heightValue;
                index++;
            }
        }

        terrain.terrainData.SetHeights(0, 0, heightMapData);
        terrain.terrainData.SetAlphamaps(0, 0, splatMapData);

        //second pass to blurr the heightmap
        RenderTexture map = terrain.terrainData.heightmapTexture;
        heightmap = map.ToTexture2D();
        heightmap = blur.Blur(heightmap, blurRadius, blurIterations);
        mapPixels = heightmap.GetPixels(0);

        index = 0;
        for (int y = 0; y < 513; y++)
        {
            for (int x = 0; x < 513; x++)
            {
                float heightValue = mapPixels[index].r;
                heightMapData[y, x] = heightValue;
                index++;
            }
        }

        terrain.terrainData.SetHeights(0, 0, heightMapData);
    }

    void SetSplatValue(int x, int y, int splat)
    {
        if (x == 512 || y == 512) return;
        for (int i = 0; i < splatMapData.GetLength(2); i++)
        {
            if (i == splat)
            {
                splatMapData[x, y, i] = 1;
            }
            else
            {
                splatMapData[x, y, i] = 0;
            }
        }
    }

    float GetPerlinValue(float x, float y, float intensity = 1, float scaleMultiplier = 1f, float offset = 0f)
    {
        x = x / 513f * scaleMultiplier;
        y = y / 513f * scaleMultiplier;
        float perlin = Mathf.PerlinNoise(x + offset, y + offset) * intensity;
        return perlin;
    }

    float GetPerlinValue01(float x, float y, float threshold, float intensity = 1f, float scaleMultiplier = 1f, float offset = 0f)
    {
        float perlin = GetPerlinValue(x, y, intensity, scaleMultiplier, offset);

        if (perlin > threshold)
        {
            perlin = 1f;
        }
        else
        {
            perlin = 0f;
        }

        return perlin;
    }
}
