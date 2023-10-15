using System;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfClans
{
    [CreateAssetMenu(fileName = "SoldierEventChannel", menuName = "War Of Clans/Soldier Event Channel")]
    public class SoldierEventChannel : ScriptableObject
    {
        public Action<IInfantry, SoldierState> OnStateChanged
        {
            get;
            set;
        }

        public Action<IInfantry> OnReachedTarget
        {
            get;
            set;
        }

        public void NotifyPlayerStateChanged(IInfantry infantry, SoldierState state)
        {
            OnStateChanged?.Invoke(infantry, state);
        }

        public void NotifyPlayerReachedTarget(IInfantry infantry)
        {
            OnReachedTarget?.Invoke(infantry);
        }
    }
}
