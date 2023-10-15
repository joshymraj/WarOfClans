using UnityEngine;

namespace WarOfClans
{
    public class HealthBar : MonoBehaviour, IHealthUI
    {
        [SerializeField] private RectTransform level;

        public void ShowHealth(float healthInPercentage)
        {
            float healthLevel = healthInPercentage / 100f;

            if(healthLevel < 0)
            {
                healthLevel = 0;
            }

            level.localScale = new Vector3(healthLevel, 1, 1);
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}
