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
    GameObject[] tileItems;

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
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Label("This is a custom label.");

        if (EditorGUILayout.DropdownButton(new GUIContent($"Current tile: {curTileName}"), FocusType.Passive))
            ShowDropdownMenu();

        if (GUILayout.Button("Relink all objects"))
            RelinkPrefabObjects();

        base.OnInspectorGUI();
    }

    private void RelinkPrefabObjects()
    {
        if (target == null) return;

        MyTilemap myTarget = target as MyTilemap;
        tileItems = myTarget.TileItems;

        var gameObjectsToChange = new List<GameObject>(myTarget.transform.childCount);

        foreach (Transform objTransform in myTarget.transform)
        {
            if (!PrefabUtility.IsPartOfAnyPrefab(objTransform.gameObject))
                gameObjectsToChange.Add(objTransform.gameObject);
        }

        foreach (var obj in gameObjectsToChange)
        {
            GameObject foundTile = null;

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

            var newObj = PrefabUtility.InstantiatePrefab(foundTile, obj.transform.parent) as GameObject;
            Undo.RegisterCreatedObjectUndo(newObj, "Relink tiles");
            newObj.transform.position = obj.transform.position;

            Undo.DestroyObjectImmediate(obj.transform.gameObject);
        }
    }

    private void ShowDropdownMenu()
    {
        tileItems = (target as MyTilemap).TileItems;

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
            var name = tileItems[i]?.name ?? "<not assigned>";
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
            var tilePos = new Vector2(tx + 0.5f, ty + 0.5f);

            var transform = (target as MyTilemap).transform;
            foreach (var collider in Physics2D.OverlapPointAll(tilePos))
                if (collider != null && collider.transform.parent == transform)
                    Undo.DestroyObjectImmediate(collider.gameObject);

            if (tileIdx > 0)
            {
                var obj = tileItems[tileIdx - 1];
                if (obj != null)
                {
                    var tile = PrefabUtility.InstantiatePrefab(obj, transform) as GameObject;
                    Undo.RegisterCreatedObjectUndo(tile, $"Place tile {curTileName}");
                    tile.transform.position = new Vector3(tx + 0.5f, ty + 0.5f, 0.0f);
                }
            }

            prevtx = tx;
            prevty = ty;
        } 
    }
}
