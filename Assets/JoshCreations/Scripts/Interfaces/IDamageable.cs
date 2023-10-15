using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfClans
{
    public interface IDamageable
    {
        IHealthUI HealthUI { get; set; }

        void TakeDamage(float damage);
    }
}
