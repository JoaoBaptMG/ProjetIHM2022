using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTilemap : MonoBehaviour
{
    // Marker script
    public TileAttributes[] tileItems;

    TileAttributes[,] tileGrid;
    int startX, startY;

    private void Start()
    {
        // Compute the tile grid
        startX = int.MaxValue; startY = int.MaxValue;
        int endX = int.MinValue, endY = int.MinValue;

        foreach (var tile in GetComponentsInChildren<TileAttributes>())
        {
            if (tile.transform == transform) continue;
            int x = Mathf.FloorToInt(tile.transform.position.x), y = Mathf.FloorToInt(tile.transform.position.y);
            startX = Mathf.Min(startX, x);
            startY = Mathf.Min(startY, y);
            endX = Mathf.Max(endX, x + 1);
            endY = Mathf.Max(endY, y + 1);
        }

        tileGrid = new TileAttributes[endX - startX, endY - startY];
        foreach (var tile in GetComponentsInChildren<TileAttributes>())
        {
            if (tile.transform == transform) continue;
            int x = Mathf.FloorToInt(tile.transform.position.x), y = Mathf.FloorToInt(tile.transform.position.y);
            tileGrid[x - startX, y - startY] = tile;
        }
    }

    // Indexers
    public TileAttributes this[int x, int y]
    {
        get
        {
            int tx = x - startX, ty = y - startY;
            if (tx < 0 || ty < 0 || tx >= tileGrid.GetLength(0) || ty >= tileGrid.GetLength(1))
                return null;
            
            return tileGrid[tx, ty];
        }
    }
    public TileAttributes this[float x, float y] => this[Mathf.FloorToInt(x), Mathf.FloorToInt(y)];
    public TileAttributes this[Vector2 pos] => this[pos.x, pos.y];

    /// <summary>
    /// Simulates the movement (with collision) of a box within the tilemap
    /// </summary>
    /// <param name="rect">The rectangle representing the box</param>
    /// <param name="velocity">The velocity of the box</param>
    /// <param name="fixedUpdate">Whether the code is being run on FixedUpdate or not</param>
    /// <returns>The velocity response vector</returns>
    public Vector2 MovementSimulation(Rect rect, Vector2 deltaPosition)
    {
        // Simulate movement in Y
        var nextY = rect.y + deltaPosition.y;
        var collisionY = nextY;

        if (deltaPosition.y < 0)
        {
            // If there is a collision on the bottom side
            if (HorizontalStripeCollision(rect.x, nextY, rect.width))
                collisionY = Mathf.Ceil(nextY);
        }
        else if (deltaPosition.y > 0)
        {
            // If there is a collision on the top side
            if (HorizontalStripeCollision(rect.x, nextY + rect.height, rect.width))
                collisionY = Mathf.Floor(nextY + rect.height) - rect.height;
        }

        // Simulate movement in X
        var nextX = rect.x + deltaPosition.x;
        var collisionX = nextX;
        
        if (deltaPosition.x < 0)
        {
            // If there's a collision on the left side
            if (VerticalStripeCollision(nextX, collisionY, rect.height))
                collisionX = Mathf.Ceil(nextX);
        }
        else if (deltaPosition.x > 0)
        {
            // If there's a collision on the right side
            if (VerticalStripeCollision(nextX + rect.width, collisionY, rect.height))
                collisionX = Mathf.Floor(nextX + rect.width) - rect.width;
        }

        return new Vector2(collisionX - nextX, collisionY - nextY);
    }

    bool HorizontalStripeCollision(float x, float y, float width)
    {
        int tx1 = Mathf.FloorToInt(x);
        int tx2 = Mathf.CeilToInt(x + width);
        int ty = Mathf.FloorToInt(y);

        for (int tx = tx1; tx < tx2; tx++)
            if (this[tx, ty]) return true;
        return false;
    }

    bool VerticalStripeCollision(float x, float y, float height)
    {
        int tx = Mathf.FloorToInt(x);
        int ty1 = Mathf.FloorToInt(y);
        int ty2 = Mathf.CeilToInt(y + height);

        for (int ty = ty1; ty < ty2; ty++)
            if (this[tx, ty]) return true;
        return false;
    }
}
