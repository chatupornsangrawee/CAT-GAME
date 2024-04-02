using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonControllerDIS : MonoBehaviour
{
    // กำหนดตัวแปร public เพื่อระบุวัตถุที่ต้องการเปิดใช้
    public GameObject objectToActivate;

    // กำหนดให้เรียกใช้เมื่อมีการคลิกปุ่ม
    public void OnButtonClick()
    {

            objectToActivate.SetActive(false);

    }
}
