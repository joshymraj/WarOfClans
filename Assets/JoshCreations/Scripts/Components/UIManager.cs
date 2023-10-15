using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace WarOfClans
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private UIEventChannel uiEventChannel;
        [SerializeField] private RectTransform redClanInventoryPanel;
        [SerializeField] private RectTransform blueClanInventoryPanel;

        [SerializeField] private GameObject inventoryPlacerPrefab;
        [SerializeField] private GameObject infantryHealthBarPrefab;
        [SerializeField] private Transform healthBarContainer;

        private List<InfantryPlacer> redClanInventoryItems;
        private List<InfantryPlacer> blueClanInventoryItems;
        private Vector3 inventoryItemInitPosition = new Vector3(0, 270, 0);
        private float inventoryItemOffset = 250;
        private float firstInventoryItemPosY = 270;

        private void Awake()
        {
            redClanInventoryItems = new List<InfantryPlacer>();
            blueClanInventoryItems = new List<InfantryPlacer>();
        }

        public void PositionPlacerUI(InfantryPlacementData infantryPlacementData)
        {
            switch (infantryPlacementData.clan)
            {
                case Clan.Red:
                    for (int i = 0; i < redClanInventoryItems.Count; i++)
                    {
                        if (redClanInventoryItems[i].PlacementData.placeableType == infantryPlacementData.placeableType)
                        {
                            redClanInventoryItems[i].gameObject.GetComponent<RectTransform>().localPosition = inventoryItemInitPosition;
                            redClanInventoryItems[i].gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, firstInventoryItemPosY - inventoryItemOffset * (i + 1)), 0.6f);
                            break;
                        }
                    }

                    break;
                case Clan.Blue:
                    for (int i = 0; i < blueClanInventoryItems.Count; i++)
                    {
                        if (blueClanInventoryItems[i].PlacementData.placeableType == infantryPlacementData.placeableType)
                        {
                            blueClanInventoryItems[i].gameObject.GetComponent<RectTransform>().localPosition = inventoryItemInitPosition;
                            blueClanInventoryItems[i].gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, firstInventoryItemPosY - inventoryItemOffset * (i + 1)), 0.6f);
                            break;
                        }
                    }

                    break;

                default:
                    break;
            }
            
        }

        public void LoadInfantryPlacer(InfantryPlacementData infantryPlacementData)
        {
            GameObject infantryPlacerGameObject = null;
            InfantryPlacer infantryPlacer = null;
            int itemCount = 0;

            switch (infantryPlacementData.clan)
            {
                case Clan.Red:
                    infantryPlacerGameObject = Instantiate(inventoryPlacerPrefab, redClanInventoryPanel);
                    infantryPlacer = infantryPlacerGameObject.GetComponent<InfantryPlacer>();
                    redClanInventoryItems.Add(infantryPlacer);
                    itemCount = redClanInventoryItems.Count;
                    break;

                case Clan.Blue:
                    infantryPlacerGameObject = Instantiate(inventoryPlacerPrefab, blueClanInventoryPanel);
                    infantryPlacer = infantryPlacerGameObject.GetComponent<InfantryPlacer>();
                    blueClanInventoryItems.Add(infantryPlacer);
                    itemCount = blueClanInventoryItems.Count;
                    break;

                default:
                    break;
            }

            if(infantryPlacerGameObject != null)
            {       
                RectTransform infantryPlacerRect = infantryPlacerGameObject.GetComponent<RectTransform>();
                infantryPlacer?.Init(infantryPlacementData);
                RegisterEvents(infantryPlacer);
                infantryPlacerRect.localPosition = inventoryItemInitPosition;
                infantryPlacerRect.DOAnchorPos(new Vector2(0, inventoryItemInitPosition.y - (inventoryItemOffset * itemCount)), 0.5f);
            }
        }

        public void AttachHealthBar(IInfantry infantry)
        {
            IHealthUI healthUI = Instantiate(infantryHealthBarPrefab, infantry.GetGameObject().transform.position, Quaternion.identity, healthBarContainer).GetComponent<HealthBar>();
            infantry.HealthUI = healthUI;
        }

        private void RegisterEvents(InfantryPlacer infantryPlacer)
        {
            if(uiEventChannel != null)
            {
                infantryPlacer.OnTapped += uiEventChannel.NotifyInfantryPlacerTapped;
                infantryPlacer.OnTapReleased += uiEventChannel.NotifyInfantryPlacerTapReleased;
                infantryPlacer.OnDragged += uiEventChannel.NotifyInfantryPlacerDragged;
            }
        }
    }
}
