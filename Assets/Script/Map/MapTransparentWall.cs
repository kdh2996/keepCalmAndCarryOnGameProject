using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTransparentWall : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        // 투명 벽에 닿으면 파괴.
        if (collision.collider.tag == "PlayerArrow")
        {
            print("== PLAYER ARROW COLLIDE WALL ==");
            Destroy(collision.collider.gameObject);
        }
    }

}
