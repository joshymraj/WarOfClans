using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace WarOfClans
{
    public class Infantry : MonoBehaviour, IAIInfantry
    {
        [SerializeField] private SoldierEventChannel soldierEventChannel;

        [Header("Animation States")]
        [SerializeField] private List<string> attackAnimationStates;
        [SerializeField] private float attackAnimationDuration;
        [Space()]
        [SerializeField] private List<string> deathAnimationStates;
        [SerializeField] private float deathAnimationDuration;
        [Space()]
        [SerializeField] private string damageAnimationState;
        [Space()]
        [SerializeField] private string moveAnimationState;

        private Animator animator;
        private INavigation navigator;
        private InfantryData soldierData;
        private float health;
        private SoldierState state;
        private IInfantry enemyInfantry;
        private Transform enemyTransform;

        public int ID
        {
            get;
            set;
        }

        public Clan Clan
        {
            get;
            set;
        }

        public IHealthUI HealthUI
        {
            get;
            set;
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (state == SoldierState.Encountering)
            {
                navigator.UpdateDestination(enemyTransform.position);
            }

            if (state != SoldierState.Dead)
            {
                HealthUI?.SetPosition(transform.position);
            }
        }

        private void RegisterEvents()
        {
            if (soldierEventChannel != null)
            {
                navigator.OnDestinationReached += OnReachedTarget;
            }
        }

        private IEnumerator AttackTillDeath()
        {
            while (state == SoldierState.Attacking && enemyInfantry.GetState() != SoldierState.Dead)
            {
                animator.SetTrigger(attackAnimationStates[Random.Range(0, attackAnimationStates.Count)]);
                enemyInfantry.TakeDamage(soldierData.damagePerAttack);
                yield return new WaitForSeconds(Random.Range(soldierData.maxAttackSpeed, soldierData.minAttackSpeed)); //TODO: Change the name to attackReflex which makes more sense.
            }

            if(enemyInfantry.GetState() == SoldierState.Dead)
            {
                state = SoldierState.Idle;
            }
        }

        private IEnumerator Die()
        {
            animator.SetBool(deathAnimationStates[Random.Range(0, deathAnimationStates.Count)], true);

            yield return new WaitForSeconds(deathAnimationDuration);

            OnStateChanged();
        }

        public SoldierState GetState()
        {
            return state;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void Init(InfantryData infantryData)
        {
            soldierData = infantryData;
            state = SoldierState.Idle;
            health = 100;
            if (soldierData.maxAttackSpeed < attackAnimationDuration)
            {
                soldierData.maxAttackSpeed = attackAnimationDuration;
            }
            RegisterEvents();
        }

        public void Init(InfantryData infantryData, INavigation navigationController)
        {
            soldierData = infantryData;
            navigator = navigationController;
            state = SoldierState.Idle;
            health = 100;
            if (soldierData.maxAttackSpeed < attackAnimationDuration)
            {
                soldierData.maxAttackSpeed = attackAnimationDuration;
            }
            RegisterEvents();
        }

        public void Attack()
        {
            if (state == SoldierState.Dead)
            {
                return;
            }

            state = SoldierState.Attacking;
            OnStateChanged();
            StartCoroutine(AttackTillDeath());
        }

        public void Encounter(IInfantry enemy)
        {
            if (state == SoldierState.Dead)
            {
                return;
            }

            enemyInfantry = enemy;
            enemyTransform = enemy.GetGameObject().transform;
            navigator.StartNavigation(enemyTransform.position, soldierData.maxMovementSpeed);
            animator.SetFloat(moveAnimationState, soldierData.maxMovementSpeed);
            state = SoldierState.Encountering;
            OnStateChanged();
        }

        public void TakeDamage(float damage)
        {
            if (state == SoldierState.Dead)
            {
                return;
            }

            health -= damage;

            animator.SetTrigger(damageAnimationState);
            HealthUI?.ShowHealth(health);

            if (health <= 0)
            {
                state = SoldierState.Dead;

                if (HealthUI != null)
                {                    
                    Destroy(HealthUI.GetGameObject());
                }
                
                StartCoroutine(Die());
            }
        }

        private void OnStateChanged()
        {
            soldierEventChannel.NotifyPlayerStateChanged(this, state);
        }

        private void OnReachedTarget()
        {
            animator.SetFloat(moveAnimationState, 0);
            soldierEventChannel.NotifyPlayerReachedTarget(this);
        }
    }
}
