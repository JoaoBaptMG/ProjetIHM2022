using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MyTilemap : MonoBehaviour
{
    // Marker script
    public GameObject[] TileItems;

    GameObject[,] tileGrid;
    int startX, startY;

    private void Awake()
    {
        // Compute the tile grid
        startX = int.MaxValue; startY = int.MaxValue;
        int endX = int.MinValue, endY = int.MinValue;

        foreach (var tr in GetComponentsInChildren<Transform>())
        {
            if (tr == transform) continue;
            int x = Mathf.FloorToInt(tr.position.x), y = Mathf.FloorToInt(tr.position.y);
            startX = Mathf.Min(startX, x);
            startY = Mathf.Min(startY, y);
            endX = Mathf.Max(startX, x + 1);
            endY = Mathf.Max(startY, y + 1);
        }

        tileGrid = new GameObject[endX - startX, endY - startY];
        foreach (var tr in GetComponentsInChildren<Transform>())
        {
            if (tr == transform) continue;
            int x = Mathf.FloorToInt(tr.position.x), y = Mathf.FloorToInt(tr.position.y);
            tileGrid[x - startX, y - startY] = tr.gameObject;
        }
    }

    // Indexers
    public GameObject this[int x, int y] => tileGrid[x - startX, y - startY];
    public GameObject this[float x, float y] => this[Mathf.FloorToInt(x), Mathf.FloorToInt(y)];
    public GameObject this[Vector2 pos] => this[pos.x, pos.y];
}
