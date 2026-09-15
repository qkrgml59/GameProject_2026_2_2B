using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Profiling;

public class DeliveryOrderSystem : MonoBehaviour
{
    [Header("주문 설정")]
    public float ordergenratelnterval = 15f;
    public int maxActiveOrders = 8;

    [Header("게임 상태")]
    public int totalOrdersGenerated = 0;
    public int completedOrders = 0;
    public int expiredOrders = 0;

    //주문 리스트
    private List<DeliveryOrder> currentorders = new List<DeliveryOrder>();

    //Building 참조
    private List<Building> restaurants = new List<Building>();
    private List<Building> customers = new List<Building>();

    [System.Serializable]
    //Event 시스템
    public class OrderSystemEvents
    {
        public UnityEvent<DeliveryOrder> OnNewOrdeAdded;
        public UnityEvent<DeliveryOrder> OnOrderPickedUp;
        public UnityEvent<DeliveryOrder> OnorderCompleted;
        public UnityEvent<DeliveryOrder> OnOrderExpired;
    }

     public OrderSystemEvents orderEvents;
        public DeliveryDriver driver;

    void Start()
    {
        driver = FindFirstObjectByType<DeliveryDriver>();
        FindAllBuilding();

        //초기주문생성
        StartCoroutine(GenerateInitialOrder());
        StartCoroutine(orderGenerator());
        StartCoroutine(ExpiredOrderChecker());
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 400, 1300));

        GUILayout.Label("===배달 주문===");
        GUILayout.Label($"활성 주문 : {currentorders.Count}개");
        GUILayout.Label($"픽업 대기 : {GetPickWaitingCount()}개");
        GUILayout.Label($"배달 대기 : {GetDeliveryWaitingCount()}개");
        GUILayout.Label($"완료 : {completedOrders}개 | 만료 : {expiredOrders}");

        GUILayout.Space(10);

        foreach(DeliveryOrder order in currentorders)
        {
            string status = order.state == OrderState.WaitingPickup ? "픽업 대기" : "배달대기";
            float timeLeft = order.GetRemainingTime();

            GUILayout.Label($"#{order.orderld}:{order.restaurantName} -> {order.customerName}");
            GUILayout.Label($"{status} | {timeLeft:F0}초 남음");
        }

        GUILayout.EndArea();
    }

    void FindAllBuilding()
    {
        Building[] allBuildings = FindObjectsByType<Building>(FindObjectsSortMode.None);

        foreach (Building building in allBuildings)
        {
            if (building.BuildingType == BuildingType.Restaurant)
            {
                restaurants.Add(building);
            }
            else if (building.BuildingType == BuildingType.Customer)
            {
                customers.Add(building);
            }

        }

        Debug.Log($"음식접 {restaurants.Count}개 고객 {customers.Count}명 발견");
    }

    void CreateNewOrder()
    {
        if (restaurants.Count == 0 || customers.Count == 0) return;

        //랜덤 음식점과 고객 선택
        Building randomRestaurant = restaurants[Random.Range(0, restaurants.Count)];
        Building randomCustomer = customers[Random.Range(0, customers.Count)];

        //같은 건물이면 다시 선택
        if(randomRestaurant == randomCustomer)
        {
            randomCustomer = customers[Random.Range(0, customers.Count)];
        }

        float reward = Random.Range(3000f, 8000f);

        DeliveryOrder newOrder = new DeliveryOrder(++totalOrdersGenerated, randomRestaurant, randomCustomer, reward);

        currentorders.Add(newOrder);
        orderEvents.OnNewOrdeAdded?.Invoke(newOrder);
    }

    void PickupOrder(DeliveryOrder order)
    {
        order.state = OrderState.PickedUp;
        orderEvents.OnOrderPickedUp?.Invoke(order);
    }

    void CompletedOrder(DeliveryOrder order)
    {
        order.state = OrderState.Completed;
        completedOrders++;

        if(driver!=null)
        {
            driver.AddMoney(order.reward);
        }

        currentorders.Remove(order);
        orderEvents.OnorderCompleted?.Invoke(order);

    }

    void ExpireOrder(DeliveryOrder order)
    {
        order.state = OrderState.Epired;
        expiredOrders++;

        currentorders.Remove(order);
        orderEvents.OnOrderExpired?.Invoke(order);
    }

    public List<DeliveryOrder> GetCurrentOrders()
    {
        return new List<DeliveryOrder>(currentorders);
    }

    public int GetPickWaitingCount()
    {
        int count = 0;
        foreach (DeliveryOrder order in currentorders)
        {
            if (order.state == OrderState.WaitingPickup) count++;
        }
        return count;
    }

    public int GetDeliveryWaitingCount()
    {
        int count = 0;
        foreach(DeliveryOrder order in currentorders)
        {
            if (order.state == OrderState.PickedUp) count++;
        }

        return count;
    }

    DeliveryOrder FindOrderForPickup(Building restaurant)
    {
        foreach(DeliveryOrder order in currentorders)
        {
            if (order.restaurantBuilding == restaurant && order.state == OrderState.WaitingPickup)
            {
                return order;
            }
        }

        return null;
    }

    DeliveryOrder FindOrderForDelivery(Building customer)
    {
        foreach(DeliveryOrder order in currentorders)
        {
            if(order.customerBuilding == customer && order.state == OrderState.PickedUp)
            {
                return order;
            }
        }

        return null;
    }

    public void OnDriverEnteredRestaurant(Building restaurant)
    {
        DeliveryOrder orderToPickup = FindOrderForPickup(restaurant);

        if(orderToPickup != null)
        {
            PickupOrder(orderToPickup);
        }
    }

    public void OnDriverEnteredCustorm(Building customer)
    {
        DeliveryOrder orderToDeliver = FindOrderForDelivery(customer);
        if(orderToDeliver !=null)
        {
            CompletedOrder(orderToDeliver);
        }
    }

    IEnumerator GenerateInitialOrder()
    {
        yield return new WaitForSeconds(1f);

        for(int i = 0; i< 3; i++)
        {
            CreateNewOrder();
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator orderGenerator()
    {
        while(true)
        {
            yield return new WaitForSeconds(ordergenratelnterval);
            if(currentorders.Count < maxActiveOrders)
            {
                CreateNewOrder();
            }
        }
    }

    IEnumerator ExpiredOrderChecker()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            List<DeliveryOrder> expiredOrders = new List<DeliveryOrder>();

            foreach(DeliveryOrder order in currentorders)
            {
                if(order.IsExpired() && order.state != OrderState.Completed)
                {
                    expiredOrders.Add(order);
                }
            }

            foreach(DeliveryOrder expired in expiredOrders)
            {
                ExpireOrder(expired);
            }
        }
    }
}
