using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

public static class MeshGenerator {
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1)/ -2f;
        float topleftZ = (height - 1)/ 2f;

        int meshSimplificationIncrement = (levelOfDetail == 0)? 1 : levelOfDetail * 2;
        int verticesPerLine = (width-1)/meshSimplificationIncrement+1;

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
        int vertexIndex = 0;

        for(int y = 0; y < height; y += meshSimplificationIncrement){
            for(int x = 0; x < width; x += meshSimplificationIncrement){
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x,y]) * heightMultiplier, topleftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x/(float)width, y/(float)height);

                if(x < width - 1 && y < height - 1){
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine); // left triangle (imagine square)
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1); // right triagle
                }

                vertexIndex++;
            }
        }

        return meshData;
    }

}

public class MeshData {
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight) {
        vertices = new Vector3[meshHeight * meshWidth];
        uvs = new Vector2[meshWidth * meshHeight]; // percentage between 0-1
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6]; // learn how this formula comes about


    }

    public void AddTriangle(int a, int b, int c){
        triangles[triangleIndex] = a;
        triangles[triangleIndex+1] = b;
        triangles[triangleIndex+2] = c;

        triangleIndex += 3;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
