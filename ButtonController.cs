using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    // กำหนดตัวแปร public เพื่อระบุวัตถุที่ต้องการเปิดใช้
    public GameObject objectToActivate;

    // กำหนดให้เรียกใช้เมื่อมีการคลิกปุ่ม
    public void OnButtonClick()
    {
        // ตรวจสอบว่า objectToActivate ไม่เป็น null และยังไม่ถูกเปิดใช้งานอยู่
        if (objectToActivate != null && !objectToActivate.activeSelf)
        {
            // เปิดใช้งานวัตถุที่กำหนด
            objectToActivate.SetActive(true);
        }
    }
}
