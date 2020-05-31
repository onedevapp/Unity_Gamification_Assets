using OneDevApp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DatePickerDemo : MonoBehaviour
{
    public Text dobValue;
    public DatePicker datePicker;
    public DatePicker datePicker2;
    private System.DateTime selectedDateTime;



    private void OnEnable()
    {
        selectedDateTime = DateTime.Today;
        //Event listener for date selection for dynamic method calling
        DatePicker.OnDaySelectedEvent += OnDaySelected;
    }

    public void OnDaySelected(DateTime date)
    {
        selectedDateTime = date;
        dobValue.text = date.ToString("dd-MM-yyyy");
    }

    private void OnDisable()
    {
        //REMOVE Event listener for date selection and ITS MANDATORY to remove events when not required
        DatePicker.OnDaySelectedEvent -= OnDaySelected;
        dobValue.text = string.Empty;
    }

    public void SetDateManually()
    {
        datePicker.SetDateValue(DateTime.Today.AddYears(-40).ToString("dd-MM-yyyy"));
        datePicker2.SetDateValue(DateTime.Today.AddYears(-40).ToString("dd-MM-yyyy"));
    }
}
