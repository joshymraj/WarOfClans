using System;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfClans
{
    [CreateAssetMenu(fileName = "UIEventChannel", menuName = "War Of Clans/UI Event Channel")]
    public class UIEventChannel : ScriptableObject
    {
        public Action<InfantryPlacer> OnInfantryPlacerTapped
        {
            get;
            set;
        }

        public Action<InfantryPlacer> OnInfantryPlacerTapReleased
        {
            get;
            set;
        }

        public Action<InfantryPlacer, Vector2> OnInfantryPlacerDragged
        {
            get;
            set;
        }

        public Action OnCameraSwitchTapped
        {
            get;
            set;
        }

        public Action OnGameRestart
        {
            get;
            set;
        }

        public void NotifyInfantryPlacerTapped(InfantryPlacer infantryPlacer)
        {
            OnInfantryPlacerTapped?.Invoke(infantryPlacer);
        }

        public void NotifyInfantryPlacerTapReleased(InfantryPlacer infantryPlacer)
        {
            OnInfantryPlacerTapReleased?.Invoke(infantryPlacer);
        }

        public void NotifyInfantryPlacerDragged(InfantryPlacer infantryPlacer, Vector2 point)
        {
            OnInfantryPlacerDragged?.Invoke(infantryPlacer, point);
        }

        public void NotifyCameraSwitchTapped()
        {
            OnCameraSwitchTapped?.Invoke();
        }

        public void NotifyGameRestart()
        {
            OnGameRestart?.Invoke();
        }
    }
}
