using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Schema;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class EndlessTerrain : MonoBehaviour
{
    public const float maxViewDist = 450;
    public Transform viewer;

    public static Vector2 viewerPosition;
    int chunkSize;
    int chunksVisibleInViewDist;
    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    void Start() {
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDist = Mathf.RoundToInt(maxViewDist / chunkSize);
    }

    void Update() {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks(){

        for(int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++){
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }

        int currentChunkCordX = Mathf.RoundToInt(viewerPosition.x/chunkSize);
        int currentChunkCordY = Mathf.RoundToInt(viewerPosition.y/chunkSize);

        for(int yOffset = -chunksVisibleInViewDist; yOffset <= chunksVisibleInViewDist; yOffset++){
            for(int xOffset = -chunksVisibleInViewDist; xOffset <= chunksVisibleInViewDist; xOffset++){
                Vector2 viewedChunkCord = new Vector2(currentChunkCordX + xOffset, currentChunkCordY + yOffset);
                
                if(terrainChunkDictionary.ContainsKey(viewedChunkCord)){
                    terrainChunkDictionary[viewedChunkCord].UpdateTerrainChunk();
                    if(terrainChunkDictionary[viewedChunkCord].isVisible()){
                        terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCord]);
                    }
                }
                else {
                    terrainChunkDictionary.Add(viewedChunkCord, new TerrainChunk(viewedChunkCord, chunkSize, transform));
                }
            }
        }
    }

    public class TerrainChunk {
        
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        public TerrainChunk(Vector2 cord, int size, Transform parent){
            position = cord * size;
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);
            bounds = new Bounds(position, Vector2.one * size);

            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.position = positionV3;
            meshObject.transform.localScale = Vector3.one * size/10f;
            meshObject.transform.parent = parent;
            SetVisible(false); // default state
        }

        public void UpdateTerrainChunk() {
            float viewerDistFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDistFromNearestEdge <= maxViewDist;
            SetVisible(visible);
        }

        public void SetVisible(bool visible) {
            meshObject.SetActive (visible);
        }

        public bool isVisible() {
            return meshObject.activeSelf;
        }

    }
}
