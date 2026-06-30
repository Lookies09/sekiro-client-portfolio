using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class ItemInfoArray
{
    public ItemInfo[] items;
}

public class InventoryManger : MonoBehaviour
{
    // 아이템 정보 스크립터블 오브젝트 참조
    [SerializeField] private ItemList itemList;

    // 인벤토리 크기 (슬롯 갯수)
    [SerializeField] private int inventorySize;

    // 획득한 아이템 리스트
    [SerializeField] private List<Item> hasItemList = new List<Item>();

    public event Action<List<Item>> OnItemListChanged;

    public List<Item> HasItemList { get => hasItemList; set => hasItemList = value; }
    public int InventorySize { get => inventorySize; set => inventorySize = value; }
   
    // 서버 url
    private string serverUrl_Set = "http://localhost:8080/unity/setItem";
    private string serverUrl_Get = "http://localhost:8080/unity/getItems";


    private void Start()
    {
        StartCoroutine(GetRequest());
    }    


    public bool AddItem(ItemInfo itemInfo)
    {
        Item hasItem = HasItemList.FirstOrDefault(item => item.ItemId == itemInfo.itemId);

        // 이미 획득한 소모성 아이템이면
        if (hasItem != null)
        {
            hasItem.ItemCount += itemInfo.count;

            ItemInfo setItem = new ItemInfo();
            setItem.itemId = itemInfo.itemId;
            setItem.count = hasItem.ItemCount;
            SendItemInfoToServer(setItem);
        }
        else
        {
            // 획득한 아이템 정보를 아이템 리스트에서 찾아 해당 아이템 생성
            Item itemTemplate = itemList.List.FirstOrDefault(item => item.ItemId == itemInfo.itemId).Clone();

            if (itemTemplate != null)
            {
                itemTemplate.ItemCount = itemInfo.count;
                // 인벤토리에 획득한 아이템 추가
                hasItemList.Add(itemTemplate);
                itemTemplate.OnItemChanged += HandleItemChanged;

                SendItemInfoToServer(itemInfo);

            }

        }

        OnItemListChanged.Invoke(hasItemList);

        return true;
    }
    private void HandleItemChanged(Item item)
    {
        // 수량만 바뀐 경우에도 UI 갱신을 유도
        OnItemListChanged?.Invoke(hasItemList);
    }


    // 아이템 추가 시 서버에 데이터를 전송하는 메서드
    public void SendItemInfoToServer(ItemInfo itemInfo)
    {
        // ItemInfo를 JSON 형태로 변환
        string json = JsonUtility.ToJson(itemInfo);

        // 서버로 보낼 POST 요청 생성
        StartCoroutine(PostRequest(json));
    }

    // POST 요청을 보내는 코루틴
    private IEnumerator PostRequest(string jsonData)
    {
        using (UnityWebRequest request = new UnityWebRequest(serverUrl_Set, "POST"))
        {
            // JSON 데이터를 요청에 담기
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            Debug.Log(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 요청 보내기
            yield return request.SendWebRequest();

            // 요청 결과 확인
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Debug.Log("아이템 저장완료");
            }
        }
    }

    private IEnumerator GetRequest()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(serverUrl_Get))  // GET 요청
        {
            // 요청 보내기
            yield return request.SendWebRequest();

            // 요청 결과 확인
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                // 서버에서 받은 JSON 데이터 출력 (디버깅용)
                Debug.Log("Received JSON: " + request.downloadHandler.text);

                // JSON 데이터를 Unity 객체로 변환 (배열 형식으로 처리)
                ItemInfo[] items = JsonUtility.FromJson<ItemInfoArray>("{\"items\":" + request.downloadHandler.text + "}").items;

                // 받은 아이템 리스트 출력
                foreach (ItemInfo item in items)
                {
                    Debug.Log("Item ID: " + item.itemId + ", Count: " + item.count);

                    AddItem(item);
                }
            }
        }
    }
}
