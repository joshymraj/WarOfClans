using System;
using System.Collections;
using UnityEngine;

using Pathfinding;

namespace WarOfClans
{
    public class AINavigationController : AIDestinationSetter, INavigation
    { 

        public Action OnStartNavigation
        {
            get;
            set;
        }

        public Action OnStopNavigation
        {
            get;
            set;
        }

        public Action OnDestinationChanged
        {
            get;
            set;
        }

        public Action OnDestinationReached
        {
            get;
            set;
        }

        private IEnumerator MoveToTarget()
        {
            ai.SearchPath();

            while (!ai.reachedEndOfPath)
            {
                yield return null;
            }

            OnDestinationReached?.Invoke();
        }

        public void StartNavigation(Vector3 destination, float speed)
        {
            ai.destination = destination;
            ai.maxSpeed = speed;

            StartCoroutine(MoveToTarget());

            OnStartNavigation?.Invoke();
        }

        public void UpdateDestination(Vector3 destination)
        {
            ai.destination = destination;
        }

        public void StopNavigation()
        {
            StopAllCoroutines();

            ai.isStopped = true;
            OnStopNavigation?.Invoke();
        }
    }
}
