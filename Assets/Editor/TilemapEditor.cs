using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(MyTilemap))]
public class TilemapEditor : Editor
{
    int curTile;
    string curTileName;
    TileAttributes[] tileItems;
    Dictionary<Vector2Int, TileAttributes> tileGrid;

    static readonly Color[] MouseColors = new Color[]
    {
        new Color(0f, 0f, 1f, 0.25f),
        new Color(1f, 1f, 0f, 0.25f)
    };

    int curMouseDown = -1;
    int prevtx, prevty;

    private void OnEnable()
    {
        curTile = 0;
        curTileName = "none";
        prevtx = -1;
        prevty = -1;
        tileGrid = new Dictionary<Vector2Int, TileAttributes>();
        RemoveUngridAndDuplicateTiles();
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Label("This is a custom label.");

        if (EditorGUILayout.DropdownButton(new GUIContent($"Current tile: {curTileName}"), FocusType.Passive))
            ShowDropdownMenu();

        if (GUILayout.Button("Relink all objects"))
            RelinkPrefabObjects();

        if (GUILayout.Button("Remove not on grid and duplicate tiles"))
            RemoveUngridAndDuplicateTiles();

        base.OnInspectorGUI();
    }

    private void RelinkPrefabObjects()
    {
        var myTarget = target as MyTilemap;
        if (myTarget == null) return;
        tileItems = myTarget.tileItems;

        var gameObjectsToChange = new List<GameObject>(myTarget.transform.childCount);

        foreach (Transform objTransform in myTarget.transform)
        {
            if (!PrefabUtility.IsPartOfAnyPrefab(objTransform.gameObject))
                gameObjectsToChange.Add(objTransform.gameObject);
        }

        foreach (var obj in gameObjectsToChange)
        {
            TileAttributes foundTile = null;

            foreach (var tile in tileItems)
            {
                if (tile == null) continue;
                if (obj.name.StartsWith(tile.name))
                {
                    foundTile = tile;
                    break;
                }
            }

            if (foundTile == null) continue;

            var newObj = PrefabUtility.InstantiatePrefab(foundTile, obj.transform.parent) as TileAttributes;
            Undo.RegisterCreatedObjectUndo(newObj, "Relink tiles");
            newObj.transform.position = obj.transform.position;

            Undo.DestroyObjectImmediate(obj.transform.gameObject);
        }
    }

    void RemoveUngridAndDuplicateTiles()
    {
        RelinkPrefabObjects();

        var myTarget = target as MyTilemap;
        if (myTarget == null) return;
       
        tileGrid.Clear();

        foreach (Transform childTransform in myTarget.transform)
        {
            var pos = new Vector2Int(Mathf.FloorToInt(childTransform.position.x), Mathf.FloorToInt(childTransform.position.y));
            if (pos.x + 0.5f != childTransform.position.x || pos.y + 0.5f != childTransform.position.y) continue;
            if (!tileGrid.ContainsKey(pos))
            {
                var tile = childTransform.GetComponent<TileAttributes>();
                if (tile != null) tileGrid[pos] = tile;
            }
        }

        foreach (Transform childTransform in myTarget.transform)
        {
            var pos = new Vector2Int(Mathf.FloorToInt(childTransform.position.x), Mathf.FloorToInt(childTransform.position.y));
            if (!tileGrid.ContainsKey(pos) || tileGrid[pos] != childTransform.GetComponent<TileAttributes>())
                Undo.DestroyObjectImmediate(childTransform.gameObject);
        }
    }

    private void ShowDropdownMenu()
    {
        var myTarget = target as MyTilemap;
        if (myTarget == null) return;
        tileItems = myTarget.tileItems;

        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("none"), false, delegate
        {
            curTile = 0;
            curTileName = "none";
            Repaint();
        });

        for (int i = 0; i < tileItems.Length; i++)
        {
            int val = i + 1;
            var name = tileItems[i] != null ? tileItems[i].name : "<not assigned>";
            menu.AddItem(new GUIContent(name), false, delegate
            {
                curTile = val;
                curTileName = name;
                Repaint();
            });
        }

        menu.ShowAsContext();
    }

    void OnSceneGUI()
    {
        if (target == null) return;

        if (Event.current.type == EventType.MouseDown)
        {
            var btn = Event.current.button;
            if (btn == 0) curMouseDown = btn;
        }

        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            curMouseDown = -1;
            prevtx = prevty = -1;
        }

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        var pos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
        pos.z = 0;

        var tx = Mathf.FloorToInt(pos.x);
        var ty = Mathf.FloorToInt(pos.y);

        if (curMouseDown == 0) PlaceTile(tx, ty, curTile);
        else if (curMouseDown == 1) PlaceTile(tx, ty, 0);

        using (new Handles.DrawingScope(MouseColors[curMouseDown + 1]))
        {
            Handles.DrawAAConvexPolygon(new Vector2(tx, ty), new Vector2(tx + 1, ty),
                new Vector2(tx + 1, ty + 1), new Vector2(tx, ty + 1));

            SceneView.RepaintAll();
        }
    }

    private void PlaceTile(int tx, int ty, int tileIdx)
    {
        if (prevtx != tx || prevty != ty)
        {
            var transform = (target as MyTilemap).transform;
            if (transform == null) return;

            var tileId = new Vector2Int(tx, ty);

            if (tileGrid.ContainsKey(tileId))
            {
                Undo.DestroyObjectImmediate(tileGrid[tileId].gameObject);
                if (tileIdx == 0) tileGrid.Remove(tileId);
            }

            if (tileIdx > 0)
            {
                var tile = tileItems[tileIdx - 1];
                if (tile != null && PrefabUtility.IsPartOfAnyPrefab(tile))
                {
                    var newTile = PrefabUtility.InstantiatePrefab(tile, transform) as TileAttributes;
                    Undo.RegisterCreatedObjectUndo(newTile, $"Place tile {curTileName}");
                    newTile.transform.position = new Vector3(tx + 0.5f, ty + 0.5f, 0.0f);
                    tileGrid[tileId] = newTile;
                }
            }

            prevtx = tx;
            prevty = ty;
        } 
    }
}
