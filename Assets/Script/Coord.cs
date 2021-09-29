using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 모든 타일의 좌표할당을 위한 구조체(Struct)
[System.Serializable]
public struct Coord
{
    public int x;
    public int y;

    public Coord(int _x, int _y)
    {
        x = _x;
        y = _y;
    }


    //구조체의 연산자 정의
    public static bool operator ==(Coord c1, Coord c2)
    {
        return (c1.x == c2.x) && (c1.y == c2.y);
    }
    public static bool operator !=(Coord c1, Coord c2)
    {
        return !(c1 == c2);
    }
}
