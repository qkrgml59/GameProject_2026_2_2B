using UnityEngine;
using UnityEngine.Events;

public class Building : MonoBehaviour
{
    [Header("건물 정보")]
    public BuildingType BuildingType;
    public string buildingName = "건물";

    [System.Serializable]
    public class BuildingEvents
    {
        public UnityEvent<string> OnDriverEntered;
        public UnityEvent<string> OnDriverExited;
        public UnityEvent<BuildingType> OnServiceUsed;
    }

    public BuildingEvents buildingEvnts;


    void Start()
    {
        SetupBuiding();
    }

    void HandleDriverService(DeliveryDriver dirver)
    {
        switch (BuildingType)
        {
            case BuildingType.Restaurant:
                Debug.Log($"{buildingName}에서 음식을 픽업했습니다.");
                break;
            case BuildingType.Customer:
                Debug.Log($"{buildingName} 배달 완료.");
                dirver.CompleteDelivery();
                break;
            case BuildingType.ChargingStation:
                Debug.Log($"{buildingName}에서 배터리를 충전했습니다.");
                dirver.ChargeBattery();
                break;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        DeliveryDriver driver = other.GetComponent<DeliveryDriver>();
        if(driver !=null)
        {
            buildingEvnts.OnDriverEntered?.Invoke(buildingName);
            HandleDriverService(driver);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DeliveryDriver driver = other.GetComponent<DeliveryDriver>();
        if(driver !=null)
        {
            buildingEvnts.OnDriverExited?.Invoke(buildingName);
            Debug.Log($"{buildingName}을 떠났습니다.");
        }
    }

    void SetupBuiding()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = renderer.material;

            switch (BuildingType)
            {
                case BuildingType.Restaurant:
                    mat.color = Color.red;
                    break;
                case BuildingType.Customer:
                    mat.color = Color.green;
                    buildingName = "고객 집";
                    break;
                case BuildingType.ChargingStation:
                    mat.color = Color.yellow;
                    buildingName = "충전소";
                    break;
            }
        }
           
    }
}
