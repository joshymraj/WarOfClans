using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfClans {
    public class BattleManager : MonoBehaviour {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private SoldierEventChannel soldierEventChannel;
        [SerializeField] private UIEventChannel uiEventChannel;
        [SerializeField] private List<InfantryPlacementData> infantryPlacementDatalist;
        [SerializeField] private Transform placeablePreviewContainer;
        [SerializeField] private GameObject placeableArea;
        [SerializeField] private GameObject forbiddenArea;
        [SerializeField] private LayerMask battleField;
        [SerializeField] private Vector3 placeablePreviewOffset = new Vector3(0f, 0f, 1f);
        [SerializeField] private Vector3 redInfantryFieldPosition = new Vector3(0f, 0.01f, -14f);
        [SerializeField] private Vector3 blueInfantryFieldPosition = new Vector3(0f, 0.01f, 7f);

        private Dictionary<int, IInfantry> redClanInfantry;
        private Dictionary<int, IInfantry> blueClanInfantry;

        private int redClanInfantryIndex;
        private int blueClanInfantryIndex;

        private bool isDragging;

        void Start() {
            redClanInfantry = new Dictionary<int, IInfantry>();
            blueClanInfantry = new Dictionary<int, IInfantry>();

            redClanInfantryIndex = 1;
            blueClanInfantryIndex = 1;

            SubscribeEventChannels();
            LoadInfantryPlacers();
        }

        private void Restart() {
            redClanInfantry.Clear();
            blueClanInfantry.Clear();

            redClanInfantryIndex = 1;
            blueClanInfantryIndex = 1;
        }

        private void SubscribeEventChannels() {
            if (soldierEventChannel != null) {
                soldierEventChannel.OnStateChanged += OnInfantryStateChanged;
                soldierEventChannel.OnReachedTarget += OnInfantryReachedTarget;
            }

            if (uiEventChannel != null) {
                uiEventChannel.OnInfantryPlacerTapped += OnInfantryUITapped;
                uiEventChannel.OnInfantryPlacerTapReleased += OnInfantryUITapReleased;
                uiEventChannel.OnInfantryPlacerDragged += OnInfantryUIDragged;
            }
        }


        private void OnInfantryStateChanged(IInfantry infantry, SoldierState soldierState) {
            switch (soldierState) {
                case SoldierState.Dead:
                RemoveInfantry(infantry);
                Destroy(infantry.GetGameObject());
                break;
                default:
                break;
            }
        }

        private void OnInfantryReachedTarget(IInfantry infantry) {
            infantry.Attack();
        }

        private void OnInfantryUITapped(InfantryPlacer infantryPlacer) {
            switch (infantryPlacer.PlacementData.clan) {
                case Clan.Blue:
                placeableArea.transform.localPosition = blueInfantryFieldPosition;
                forbiddenArea.transform.localPosition = redInfantryFieldPosition;
                break;

                case Clan.Red:
                placeableArea.transform.localPosition = redInfantryFieldPosition;
                forbiddenArea.transform.localPosition = blueInfantryFieldPosition;
                break;

                default:
                break;
            }

            placeableArea.GetComponent<MeshRenderer>().enabled = true;
            forbiddenArea.GetComponent<MeshRenderer>().enabled = true;
        }

        private void OnInfantryUITapReleased(InfantryPlacer infantryPlacer) {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            infantryPlacer.Enable();
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, battleField)) {
                GameObject newInfantryGameObject = Instantiate(infantryPlacer.PlacementData.infantryPrefab,
                                                            hit.point + placeablePreviewOffset,
                                                            GetInfantryFaceAngle(infantryPlacer.PlacementData.clan));

                IAIInfantry newAIInfantry = newInfantryGameObject.GetComponent<Infantry>();
                INavigation navigation = newInfantryGameObject.GetComponent<AINavigationController>();
                newAIInfantry.Clan = infantryPlacer.PlacementData.clan;
                newAIInfantry.Init(infantryPlacer.PlacementData.infantryData, navigation);
                uiManager.AttachHealthBar(newAIInfantry);
                AddInfantry(newAIInfantry);

                InitiateEncounter(newAIInfantry);
                Debug.Log("Spawned Infantry");

                ClearPreview();
            }

            uiManager.PositionPlacerUI(infantryPlacer.PlacementData);

            placeableArea.GetComponent<MeshRenderer>().enabled = false;
            forbiddenArea.GetComponent<MeshRenderer>().enabled = false;
        }

        private void InitiateEncounter(IAIInfantry aiInfantry) {
            List<IInfantry> enemyInfantries = null;
            List<IInfantry> enemyIdleInfantries = null;
            IInfantry enemy = null;

            switch (aiInfantry.Clan) {
                case Clan.Red:
                if (blueClanInfantry.Count > 0) {
                    enemyInfantries = new List<IInfantry>();
                    enemyInfantries.AddRange(blueClanInfantry.Values);
                }
                break;

                case Clan.Blue:
                if (redClanInfantry.Count > 0) {
                    enemyInfantries = new List<IInfantry>();
                    enemyInfantries.AddRange(redClanInfantry.Values);
                }
                break;

                default:
                break;
            }

            if (enemyInfantries?.Count > 0) {
                enemyIdleInfantries = enemyInfantries.FindAll(x => x.GetState() == SoldierState.Idle);

                if (enemyIdleInfantries?.Count > 0) {
                    enemy = enemyIdleInfantries.OrderBy(x => Vector3.SqrMagnitude(x.GetGameObject().transform.position - aiInfantry.GetGameObject().transform.position)).First();
                } else {
                    enemy = enemyInfantries.OrderBy(x => Vector3.SqrMagnitude(x.GetGameObject().transform.position - aiInfantry.GetGameObject().transform.position)).First();
                }
            }

            if (enemy != null) {
                aiInfantry.Encounter(enemy);
                if (enemy.GetState() == SoldierState.Idle) {
                    enemy.Encounter(aiInfantry);
                }
            }
        }

        private void OnInfantryUIDragged(InfantryPlacer infantryPlacer, Vector2 point) {
            infantryPlacer.transform.Translate(point);

            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            bool planeHit = Physics.Raycast(ray, out hit, Mathf.Infinity, battleField);

            if (planeHit) {
                if (!isDragging) {
                    isDragging = true;
                    placeablePreviewContainer.position = hit.point;
                    infantryPlacer.Disable();
                    Instantiate(infantryPlacer.PlacementData.infantryPrefab,
                                hit.point + placeablePreviewOffset,
                                GetInfantryFaceAngle(infantryPlacer.PlacementData.clan),
                                placeablePreviewContainer);
                } else {
                    placeablePreviewContainer.position = hit.point;
                }
            } else {
                if (isDragging) {
                    isDragging = false;
                    infantryPlacer.Enable();

                    ClearPreview();
                }
            }
        }

        private Quaternion GetInfantryFaceAngle(Clan clan) {
            switch (clan) {
                case Clan.Red:
                return Quaternion.Euler(0, -90, 0);

                case Clan.Blue:
                return Quaternion.Euler(0, 90, 0);

                default:
                return Quaternion.identity;
            }
        }

        private void AddInfantry(IInfantry infantry) {
            switch (infantry.Clan) {
                case Clan.Red:
                infantry.ID = redClanInfantryIndex;
                redClanInfantry.Add(redClanInfantryIndex, infantry);
                redClanInfantryIndex += 1;
                break;

                case Clan.Blue:
                infantry.ID = blueClanInfantryIndex;
                blueClanInfantry.Add(blueClanInfantryIndex, infantry);
                blueClanInfantryIndex += 1;
                break;

                default:
                break;
            }
        }

        private void RemoveInfantry(IInfantry infantry) {
            switch (infantry.Clan) {
                case Clan.Red:
                redClanInfantry.Remove(infantry.ID);
                break;

                case Clan.Blue:
                blueClanInfantry.Remove(infantry.ID);
                break;

                default:
                break;
            }
        }

        private void ClearPreview() {
            for (int i = 0; i < placeablePreviewContainer.childCount; i++) {
                Destroy(placeablePreviewContainer.GetChild(i).gameObject);
            }
        }

        private void LoadInfantryPlacers() {
            foreach (InfantryPlacementData infantryPlacementData in infantryPlacementDatalist) {
                uiManager.LoadInfantryPlacer(infantryPlacementData);
            }
        }
    }
}
