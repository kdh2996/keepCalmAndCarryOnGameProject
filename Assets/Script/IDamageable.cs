using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Idamageable
{
    void TakeHit(float damage, RaycastHit hit);

    bool TakeDamage(float damage);

}
