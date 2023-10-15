using UnityEngine;

namespace WarOfClans
{
    [CreateAssetMenu(fileName = "InfantryPlacementData", menuName = "War Of Clans/Infantry Placement Data")]
    public class InfantryPlacementData : ScriptableObject
    {
        public GameObject infantryPrefab;
        public Sprite infantryUI;
        public Clan clan;
        public PlaceableType placeableType;

        public InfantryData infantryData;
    }
}
