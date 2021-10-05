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
            endX = Mathf.Max(endX, x + 1);
            endY = Mathf.Max(endY, y + 1);
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
    public GameObject this[int x, int y]
    {
        get
        {
            int tx = x - startX, ty = y - startY;
            if (tx < 0 || ty < 0 || tx >= tileGrid.GetLength(0) || ty >= tileGrid.GetLength(1))
                return null;
            
            return tileGrid[tx, ty];
        }
    }
    public GameObject this[float x, float y] => this[Mathf.FloorToInt(x), Mathf.FloorToInt(y)];
    public GameObject this[Vector2 pos] => this[pos.x, pos.y];

    /// <summary>
    /// Simulates the movement (with collision) of a box within the tilemap
    /// </summary>
    /// <param name="rect">The rectangle representing the box</param>
    /// <param name="velocity">The velocity of the box</param>
    /// <param name="fixedUpdate">Whether the code is being run on FixedUpdate or not</param>
    /// <returns>The velocity response vector</returns>
    public Vector2 MovementSimulation(Rect rect, Vector2 velocity, bool fixedUpdate = false)
    {
        // Calculate the delta
        var dpos = velocity * (fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime);

        // Simulate movement in Y
        var nextY = rect.y + dpos.y;
        var collisionY = nextY;

        if (velocity.y < 0)
        {
            // If there is a collision on the bottom side
            if (HorizontalStripeCollision(rect.x, nextY, rect.width))
                collisionY = Mathf.Ceil(nextY);
        }
        else if (velocity.y > 0)
        {
            // If there is a collision on the top side
            if (HorizontalStripeCollision(rect.x, nextY + rect.height, rect.width))
                collisionY = Mathf.Floor(nextY + rect.height) - rect.height;
        }

        // Simulate movement in X
        var nextX = rect.x + dpos.x;
        var collisionX = nextX;
        
        if (velocity.x < 0)
        {
            // If there's a collision on the left side
            if (VerticalStripeCollision(nextX, collisionY, rect.height))
                collisionX = Mathf.Ceil(nextX);
        }
        else if (velocity.x > 0)
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
