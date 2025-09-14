using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManager : MonoBehaviour
{
    [Header("UI ���")]
    public Text statusText;
    public Text messageText;
    public Slider batterySlider;
    public Image batteryFill;

    [Header("���� ������Ʈ")]
    public DeliveryDriver driver;


    private void Start()
    {
        if(driver != null)
        {
            // Event ����
            driver.driverEvents.OnMoneyChanged.AddListener(UpdateMoney);
            driver.driverEvents.OnBatteryChanged.AddListener(UpdateBattery);
            driver.driverEvents.OnDeliveryCountChanged.AddListener(UpdateDeliveryCount);
            driver.driverEvents.OnMoveStarted.AddListener(OnMoveStarted);
            driver.driverEvents.OnMoveStoped.AddListener(OnMoveStopped);
            driver.driverEvents.OnLowBattery.AddListener(OnLowBattery);
            driver.driverEvents.OnLowBatteryEmpty.AddListener(OnBatteryEmpty);
            driver.driverEvents.OnDeliveryCompleted.AddListener(OnDeliveryCompleted);
        }
        UpdateUI();
    }

    void Update()
    {
        if (statusText != null && driver != null)
        {
            statusText.text = driver.GetStatusText();
        }
    }

    private void OnDestroy()
    {
        if (driver != null)
        {
            // Event ���� ����
            driver.driverEvents.OnMoneyChanged.RemoveListener(UpdateMoney);
            driver.driverEvents.OnBatteryChanged.RemoveListener(UpdateBattery);
            driver.driverEvents.OnDeliveryCountChanged.RemoveListener(UpdateDeliveryCount);
            driver.driverEvents.OnMoveStarted.RemoveListener(OnMoveStarted);
            driver.driverEvents.OnMoveStoped.RemoveListener(OnMoveStopped);
            driver.driverEvents.OnLowBattery.RemoveListener(OnLowBattery);
            driver.driverEvents.OnLowBatteryEmpty.RemoveListener(OnBatteryEmpty);
            driver.driverEvents.OnDeliveryCompleted.RemoveListener(OnDeliveryCompleted);
        }
    }
    void ShowMessage(string message, Color color)
    {
        if(messageText != null)
        {
            messageText.text = message;
            messageText.color = color;
            StartCoroutine(ClearMessageAgterDelay(2f));
        }
    }

    IEnumerator ClearMessageAgterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if(messageText != null)
        {
            messageText.text = "";
        }
    }

    public void UpdateMoney(float money)
    {
        ShowMessage($"�� : {money}��", Color.green);
    }

    public void UpdateBattery(float battery)
    {
        if (batterySlider != null)
        {
            batterySlider.value = battery / 100f;
        }
        if (batteryFill != null)
        {
            if (battery > 50f)
            {
                batteryFill.color = Color.green;
            }
            else if (battery > 20f)
            {
                batteryFill.color = Color.yellow;
            }
            else
            {
                batteryFill.color = Color.red;
            }
        }
    }

    public void UpdateDeliveryCount(int count)
    {
        ShowMessage($"��޿Ϸ� : {count}��", Color.blue);    
    }

    public void OnMoveStarted()
    {
        ShowMessage("�̵� ����", Color.cyan);
    }

    public void OnMoveStopped()
    {
        ShowMessage("�̵� ����", Color.gray);
    }

    public void OnLowBattery()
    {
        ShowMessage("���͸� ����", Color.red);
    }

    public void OnBatteryEmpty()
    {
        ShowMessage("���͸� ����", Color.red);
    }

    public void OnDeliveryCompleted()
    {
        ShowMessage("��� �Ϸ�",Color.green);
    }

    void UpdateUI()
    {
        if(driver != null)
        {
            UpdateMoney(driver.currentMoney);
            UpdateBattery(driver.batteryLevel);
            UpdateDeliveryCount(driver.deliveryCount);
        }
    }
}
