using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DeliveryDriver : MonoBehaviour
{
    [Header("배달원 설정")]
    public float moveSpeed = 8f;

    public float rotationSpeed = 10.0f;

    [Header("상태")]
    public float currentMoney = 0;
    public float batteryLevel = 100f;
    public int deliveryCount = 0;

    //Evnet 시스템
    [System.Serializable]
    public class DriverEvents
    {
        [Header("이동 Event")]
        public UnityEvent OnMoveStarted;
        public UnityEvent OnMoveStoped;

        [Header("상태변화 Event")]
        public UnityEvent<float> OnMoneychanged;
        public UnityEvent<float> OnBatteryChanged;
        public UnityEvent<int> OnDeliveryCountChanged;

        [Header("경고 Event")]
        public UnityEvent OnLowBattery;
        public UnityEvent OnLowBaterEmpty;
        public UnityEvent OnDeliveryCompleted;
        
    }

    public DriverEvents driverEvents;

    public bool isMoving = false;

    void Start()
    {
        //초기상태 이벤트 발생
        driverEvents.OnMoneychanged?.Invoke(currentMoney);
        driverEvents.OnBatteryChanged?.Invoke(batteryLevel);
        driverEvents.OnDeliveryCountChanged?.Invoke(deliveryCount);
    }

    void Update()
    {
        HandleMovement();
    }

    void ChangedBattery(float amount)
    {
        float oldBattery = batteryLevel;
        batteryLevel += amount;
        batteryLevel = Mathf.Clamp(batteryLevel, 0, 100);

        //배터리 변화 Evetn 발생
        driverEvents.OnBatteryChanged?.Invoke(batteryLevel);

        if(oldBattery>20f && batteryLevel <=20f)
        {
            driverEvents.OnLowBattery?.Invoke();             //배터리 부족 이벤트 발생
        }

        if (oldBattery > 20f && batteryLevel <= 20f)
        {
            driverEvents?.OnLowBaterEmpty.Invoke();             //배터리 방전 이벤트 발생
        }
    }

    void HandleMovement()
    {

        //입력 받기
        Vector2 input = Keyboard.current != null ? new Vector2(
            (Keyboard.current.dKey.isPressed ? 1 : 0) -
            (Keyboard.current.aKey.isPressed ? 1 : 0),
            (Keyboard.current.wKey.isPressed ? 1 : 0) -
            (Keyboard.current.sKey.isPressed ? 1 : 0)
            ) : Vector2.zero;

        Vector3 moveDirection = new Vector3(input.x, 0f, input.y);

        if (moveDirection.magnitude > 0.1f)
        {
            if (!isMoving)
            {
                StartMoving();
            }

            //이동처리
            moveDirection = moveDirection.normalized;
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

            //회전처리
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            ChangedBattery(-Time.deltaTime * 3.0f);          //이동할때마다 배터리 소모
        }
        else
        {
            if (isMoving)
                StopMoving();
        }


        //배터리 체크
        if (batteryLevel <= 0)
        {
            if(isMoving)
            {
                StopMoving();
            }
            return;

        }
    }

    void StartMoving()
    {
        isMoving = true;
        driverEvents.OnMoveStarted?.Invoke();
    }

    void StopMoving()
    {
        isMoving = false;
        driverEvents.OnMoveStoped?.Invoke();
    }

    public void AddMoney(float amount)
    {
        currentMoney += amount;
        driverEvents.OnMoneychanged?.Invoke(currentMoney);
    }

    public void CompleteDelivery()
    {
        deliveryCount++;
        float reward = Random.Range(3000, 8000);
        AddMoney(reward);
        driverEvents.OnDeliveryCountChanged?.Invoke(deliveryCount);
        driverEvents.OnDeliveryCompleted?.Invoke();
    }

    public void ChargeBattery()
    {
        ChangedBattery(100f - batteryLevel);
    }

    public string GetStatusText()
    {
        return $"돈 : {currentMoney:F0}원 | 배터리 : {batteryLevel:F1}% | 배달 : {deliveryCount}건";
    }

    public bool CanMove()
    {
        return batteryLevel > 0;
    }
}
