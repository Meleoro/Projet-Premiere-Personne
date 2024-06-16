using UnityEngine;

public class TerrainDetector
{
    private Terrain terrain;
    public TerrainData terrainData;
    private int alphamapWidth;
    private int alphamapHeight;
    private float[,,] splatmapData;
    private int numTextures;
    private int rockIndex;

    public TerrainDetector(TerrainData data, Terrain ter, int rockIndex)
    {
        terrain = ter;
        terrainData = data;
        alphamapWidth = terrainData.alphamapWidth;
        alphamapHeight = terrainData.alphamapHeight;
        this.rockIndex = rockIndex;

        splatmapData = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
        numTextures = splatmapData.Length / (alphamapWidth * alphamapHeight);
    }

    private Vector3 ConvertToSplatMapCoordinate(Vector3 worldPosition)
    {
        Vector3 splatPosition = new Vector3();
        Vector3 terPosition = terrain.transform.position;
        splatPosition.x = ((worldPosition.x - terPosition.x) / terrain.terrainData.size.x) * terrain.terrainData.alphamapWidth;
        splatPosition.z = ((worldPosition.z - terPosition.z) / terrain.terrainData.size.z) * terrain.terrainData.alphamapHeight;
        return splatPosition;
    }

    public bool GetIsWalkingOnRock(Vector3 position)
    {
        Vector3 terrainCord = ConvertToSplatMapCoordinate(position);

        if (splatmapData[(int)terrainCord.z, (int)terrainCord.x, rockIndex] > 0.23f)
        {
            return true;
        }

        return false;
    }

}