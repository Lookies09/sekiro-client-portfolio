using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUiManager : MonoBehaviour
{
    public Dictionary<string, GameObject> panelDic = new Dictionary<string, GameObject> ();

    [SerializeField] private PlayerInputComponent playerInput;
    [SerializeField] private InventoryManger inventoryManger;

    [Header("패널 들")]
    [SerializeField] private GameObject inventoryPanelHeader;
    [SerializeField] private GameObject itemInfoUi_Object;

    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject itemsPanel;
    [SerializeField] private GameObject skillPanel;
    [SerializeField] private GameObject settingPanel;


    [SerializeField] private GameObject equipmentItemPanel;
    [SerializeField] private GameObject equipmentSkillPanel;


    [SerializeField] private GameObject lanternPanel;
    [SerializeField] private GameObject teleportPanel;

    [SerializeField] private ItemInfoUi itemInfoUi;

    [Header("아이템 정보 셀")]
    // 아이템 정보 셀 프리펩
    [SerializeField] private GameObject itemInfoCellPrefab;

    // 장착아이템 정보 셀 프리펩
    [SerializeField] private GameObject equipmentItemInfoCellPrefab;

    // 스크롤뷰의 콘텐츠뷰 참조
    [SerializeField] private Transform cellContentView;

    // 장착아이템 선택 스크롤뷰의 콘텐츠뷰 참조
    [SerializeField] private Transform equipmentCellContentView;

    [Header("사용 아이템 버튼 들")]
    [SerializeField] private UseItemButton[] useItemButtons;

    private UseItemButton currentUseItemButton;

    private List<GameObject> itemInfoCells = new List<GameObject>();
    private List<GameObject> equipmentItemInfoCells = new List<GameObject>();


    public event Action<List<Item>> OnEquipItemListChanged;

    private void OnEnable()
    {
        playerInput.OnInventoryStateChanged += SetInventory;
        inventoryManger.OnItemListChanged += UpdateInventoryUI;
    }

    private void Start()
    {
        panelDic.Add("Main", mainPanel);
        panelDic.Add("Items", itemsPanel);
        panelDic.Add("Skill", skillPanel);
        panelDic.Add("Setting", settingPanel);



        panelDic.Add("Equipment", equipmentItemPanel);

        SetInventory(false);
    }    

    public void SetInventory(bool isOpen)
    {
        Time.timeScale = isOpen ? 0 : 1;

        inventoryPanelHeader.SetActive(isOpen);
        itemInfoUi_Object.SetActive(isOpen);

        foreach (var kvp in panelDic)
        {
            kvp.Value.SetActive(isOpen && kvp.Key == "Main");
        }
    }

    public void ActivePanel(string name)
    {
        if (panelDic.TryGetValue(name, out GameObject panel))
        {
            if (panel.activeSelf) return;

            if (name.Equals("Setting") && itemInfoUi_Object.activeSelf)
            {
                itemInfoUi_Object.SetActive(false);
            }
            else
            {
                itemInfoUi_Object.SetActive(true);
            }
            // 모든 패널 비활성화
            foreach (var kvp in panelDic)
            {
                kvp.Value.SetActive(false);
            }

            panel.SetActive(true);
        }
    }

    public void OnAddItemInfoCallback(Item item)
    {
        GameObject itemInfoCell = Instantiate(itemInfoCellPrefab, cellContentView);
        GameObject itemInfoCell2 = Instantiate(equipmentItemInfoCellPrefab, equipmentCellContentView);

        var cell1 = itemInfoCell.GetComponent<ItemInfoCell>();
        var cell2 = itemInfoCell2.GetComponent<ItemInfoCell>();

        cell1.Init(item, true);
        cell2.Init(item, false);

        // 이벤트 구독
        cell2.OnEquipItemChanged += HandleEquipItemSelected;
    }

    public void UpdateInventoryUI(List<Item> hasItemList)
    {
        int count = hasItemList.Count;

        // 아이템 셀 관리
        for (int i = 0; i < count; i++)
        {
            GameObject cellObj;
            GameObject equipCellObj;

            // 기존 셀이 있다면 재사용
            if (i < itemInfoCells.Count)
            {
                cellObj = itemInfoCells[i];
                equipCellObj = equipmentItemInfoCells[i];
                cellObj.SetActive(true);
                equipCellObj.SetActive(true);
            }
            else
            {
                // 없으면 새로 생성 후 리스트에 추가
                cellObj = Instantiate(itemInfoCellPrefab, cellContentView);
                equipCellObj = Instantiate(equipmentItemInfoCellPrefab, equipmentCellContentView);

                itemInfoCells.Add(cellObj);
                equipmentItemInfoCells.Add(equipCellObj);

                var cell2 = equipCellObj.GetComponent<ItemInfoCell>();
                cell2.OnEquipItemChanged += HandleEquipItemSelected;
            }

            var cell1 = cellObj.GetComponent<ItemInfoCell>();
            var cell2Again = equipCellObj.GetComponent<ItemInfoCell>();

            cell1.Init(hasItemList[i], true);
            cell2Again.Init(hasItemList[i], false);
        }

        // 남은 셀은 비활성화
        for (int i = count; i < itemInfoCells.Count; i++)
        {
            itemInfoCells[i].SetActive(false);
            equipmentItemInfoCells[i].SetActive(false);
        }
    }


    public void SetCurrentUseItemButton(UseItemButton useItemButton)
    {
        currentUseItemButton = useItemButton;
    }

    public void HandleEquipItemSelected(Item item)
    {
        currentUseItemButton.SetUseItemButton(item);
        ActivePanel("Main");

        List<Item> items = new List<Item>();
        foreach (var useItemButton in useItemButtons)
        {
            if (useItemButton.item == null) continue;
            items.Add(useItemButton.item);
        }
        OnEquipItemListChanged.Invoke(items);
    }
}
