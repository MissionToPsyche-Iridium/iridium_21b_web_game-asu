using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyDeathHandler
{
    void OnDeath();
    void OnHit();
}
