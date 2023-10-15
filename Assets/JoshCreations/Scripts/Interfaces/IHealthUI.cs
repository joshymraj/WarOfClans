using UnityEngine;

namespace WarOfClans
{
    public interface IHealthUI
    {
        void ShowHealth(float healthInPercentage);

        void SetPosition(Vector3 position);

        GameObject GetGameObject();
    }
}
