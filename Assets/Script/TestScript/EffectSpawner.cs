using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpawner : MonoBehaviour {

    public Effect exEffect;
    public Effect InstantEffect;

    public EffectParent exEffectParent;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClickEffectSpwan()
    {
        Vector3 newPos = new Vector3(exEffectParent.transform.localPosition.x + 2, exEffectParent.transform.localPosition.y, exEffectParent.transform.localPosition.z);

        InstantEffect = Instantiate(exEffect, newPos, Quaternion.identity) as Effect;

    }
}
