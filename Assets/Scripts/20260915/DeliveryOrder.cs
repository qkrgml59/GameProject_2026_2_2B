using UnityEngine;

//간단한 배달 주문
[System.Serializable]

public class DeliveryOrder
{
    public int orderld;
    public string restaurantName;
    public string customerName;
    public Building restaurantBuilding;
    public Building customerBuilding;

    public float ordertime;
    public float timeLimit;

    public float reward;
    public OrderState state;

    //생성자
    public DeliveryOrder(int id, Building restaurant, Building customer, float rewardAmount)
    {
        orderld = id;
        restaurantBuilding = restaurant;
        customerBuilding = customer;
        restaurantName = customer.buildingName;
        ordertime = Time.deltaTime;
        timeLimit = Random.Range(60f, 120f);
        reward = rewardAmount;
        state = OrderState.WaitingPickup;
    }

    public float GetRemainingTime()
    {
        return Mathf.Max(0f, timeLimit - (Time.time - ordertime));
    }

    public bool IsExpired()
    {
        return GetRemainingTime() <= 0f;
    }
}
