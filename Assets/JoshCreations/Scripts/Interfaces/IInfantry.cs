using UnityEngine;

namespace WarOfClans
{
    public interface IInfantry : IDamageable
    {
        int ID { get; set; }

        Clan Clan { get; set; }        

        SoldierState GetState();

        GameObject GetGameObject();

        void Init(InfantryData infantryData);

        void Encounter(IInfantry enemy);

        void Attack();        
    }
}
