using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class DeliveryUIManager : MonoBehaviour
{
    [Header("UI 요소")]
    public Text statusText;
    public Text messageText;
    public Slider batterSlider;
    public Image batteryFill;

    public DeliveryDriver driver;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(driver !=null)
        {
            driver.driverEvents.OnMoneychanged.AddListener(UpdateMoney);
            driver.driverEvents.OnBatteryChanged.AddListener(UpdateBattery);
            driver.driverEvents.OnDeliveryCountChanged.AddListener(UpdateDeliveryCount);
            driver.driverEvents.OnMoveStarted.AddListener(OnMoveStarted);
            driver.driverEvents.OnMoveStoped.AddListener(OnMoveStopped);
            driver.driverEvents.OnLowBattery.AddListener(OnLowBattery);
            driver.driverEvents.OnLowBaterEmpty.AddListener(onBatteryEmpty);
            driver.driverEvents.OnDeliveryCompleted.AddListener(OnDeliveryCompledted);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(statusText != null)
        {
            statusText.text = driver.GetStatusText();
        }
    }

    void ShowMessage(string Message, Color color)
    {
        if(messageText !=null)
        {
            messageText.text = Message;
            messageText.color = color;
            StartCoroutine(ClearMessageAfterDelay(2f));
        }
    }

    IEnumerator ClearMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if(messageText !=null)
        {
            messageText.text = "";
        }
    }

    void UpdateMoney(float money)
    {
        ShowMessage($"돈 : {money}원", Color.green);
    }

    void UpdateBattery(float battery)
    {
        if(batterSlider !=null)
        {
            batterSlider.value = battery / 100f;
        }

        if(batteryFill !=null)
        {
            if (battery > 50f)
            {
                batteryFill.color = Color.green;
            }
            else if(battery > 20f)
            {
                batteryFill.color = Color.green;
            }
            else
            {
                batteryFill.color = Color.red;
            }
        }
    }

    void UpdateDeliveryCount(int count)
    {
        ShowMessage($"배달 완료 : {count}건", Color.blue);
    }
   
    void OnMoveStarted()
    {
        ShowMessage("이동 시작", Color.cyan);
    }

    void OnMoveStopped()
    {
        ShowMessage("이동 정지", Color.gray);
    }

    void OnLowBattery()
    {
        ShowMessage("배터리 부족", Color.red);
    }

    void onBatteryEmpty()
    {
        ShowMessage("배터리 방전!", Color.red);
    }

    void OnDeliveryCompledted()
    {
        ShowMessage("배달 완료", Color.green);
    }

    void updateUI()
    {
        if(driver !=null)
        {
            UpdateMoney(driver.currentMoney);
            UpdateBattery(driver.batteryLevel);
            UpdateDeliveryCount(driver.deliveryCount);
        }
    }

    void OnDestroy()
    {
        if(driver != null)
        {
            driver.driverEvents.OnMoneychanged.RemoveListener(UpdateMoney);
            driver.driverEvents.OnBatteryChanged.RemoveListener(UpdateBattery);
            driver.driverEvents.OnDeliveryCountChanged.RemoveListener(UpdateDeliveryCount);
            driver.driverEvents.OnMoveStarted.RemoveListener(OnMoveStarted);
            driver.driverEvents.OnMoveStoped.RemoveListener(OnMoveStopped);
            driver.driverEvents.OnLowBattery.RemoveListener(OnLowBattery);
            driver.driverEvents.OnLowBaterEmpty.RemoveListener(onBatteryEmpty);
            driver.driverEvents.OnDeliveryCompleted.RemoveListener(OnDeliveryCompledted);
        }
      
    }
}
