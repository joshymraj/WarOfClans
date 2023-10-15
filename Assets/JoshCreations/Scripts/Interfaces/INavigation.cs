using System;

using UnityEngine;

namespace WarOfClans
{
    public interface INavigation
    {
        Action OnStartNavigation { get; set; }
        Action OnStopNavigation { get; set; }
        Action OnDestinationReached { get; set; }
   
        void StartNavigation(Vector3 destination, float speed);
        void UpdateDestination(Vector3 destination);
        void StopNavigation();
    }
}
