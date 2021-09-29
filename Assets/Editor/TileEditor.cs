using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapTile))]

public class TileEditor : Editor
{
    public bool ApplyEditor = false;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        if (ApplyEditor)
        {
            MapTile mapTile = target as MapTile;

            mapTile.ApplyTile();
        }
    }
}
