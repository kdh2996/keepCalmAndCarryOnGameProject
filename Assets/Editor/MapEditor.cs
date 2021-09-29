using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(Map))]

public class MapEditor : Editor {

    public override void OnInspectorGUI()
    {

        Map map = target as Map;

        // Inspector에 값이 갱신될 때만 맵 다시 생성.
        if (DrawDefaultInspector())
        {
            map.GenerateMap();
        }

        // Script내에서 값이 갱신되어 직접 맵을 생성하고 싶을 때.
        if (GUILayout.Button("Generate Map"))
        {
            map.GenerateMap();
        }

    }

}
