using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    bool Damagable { get; }
    Transform Transform { get; }
    void TakeDamage(float damage);
}