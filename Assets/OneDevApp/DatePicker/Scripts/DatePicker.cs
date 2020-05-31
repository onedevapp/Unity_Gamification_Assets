using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Globalization;

namespace OneDevApp
{
    public enum DatePickerRange
    {
        FORWARD_ONLY,
        BACKWARD_ONLY,
        ANY
    }

    // delegate Declaration
    public delegate void OnDaySelectedDelegate(DateTime date);

    public class DatePicker : MonoBehaviour
    {
        // event Declaration
        public static event OnDaySelectedDelegate OnDaySelectedEvent;
        public Text SelectedDateText;
        public string DateFormat = "dd-MM-yyyy";
        public DatePickerRange pickerRange = DatePickerRange.ANY;
        public GameObject DaysContainer;
        public GameObject MonthContainer;
        public GameObject YearsContainer;
        [SerializeField]
        private Text CurrentMonth;
        [SerializeField]
        private Text CurrentYear;
        private DayScript[] DayList;
        private DayScript[] MonthList;
        private DayScript[] YearList;
        private DateTime SelectedDate = DateTime.Today;
        private DateTime ReferenceDate = DateTime.Today;

        void Start()
        {
            if (DaysContainer)
                DayList = DaysContainer.GetComponentsInChildren<DayScript>();
            else
                DayList = gameObject.GetComponentsInChildren<DayScript>();
            if (MonthContainer) MonthList = MonthContainer.GetComponentsInChildren<DayScript>();
            if (YearsContainer) YearList = YearsContainer.GetComponentsInChildren<DayScript>();
            SetupVariables();
        }

        string GetMonth(int index)
        {
            string result = "January";
            switch (index)
            {
                case 1:
                    result = "January";
                    break;
                case 2:
                    result = "February";
                    break;
                case 3:
                    result = "March";
                    break;
                case 4:
                    result = "April";
                    break;
                case 5:
                    result = "May";
                    break;
                case 6:
                    result = "Jun";
                    break;
                case 7:
                    result = "July";
                    break;
                case 8:
                    result = "August";
                    break;
                case 9:
                    result = "September";
                    break;
                case 10:
                    result = "October";
                    break;
                case 11:
                    result = "November";
                    break;
                case 12:
                    result = "December";
                    break;
                default:
                    Debug.Log(" Error : Improbable month");
                    break;
            }
            return result;
        }
        void SetupVariables()
        {
            if (DaysContainer) DaysContainer.SetActive(true);
            if (MonthContainer) MonthContainer.SetActive(false);
            if (YearsContainer) YearsContainer.SetActive(false);
            CurrentMonth.text = GetMonth(ReferenceDate.Month);
            CurrentYear.text = ReferenceDate.Year.ToString();
            Invoke("Generate", 0.1f);
        }

        public void SetDateValue(string dateString)
        {
            String oldDT = DateTime
              .ParseExact(dateString, "dd-MM-yyyy", CultureInfo.InvariantCulture)
              .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            //DateTime oldDT = DateTime.Parse(dateString);
            DateTime myDate;
            if (!DateTime.TryParse(oldDT, out myDate))
            {
                // handle parse failure
                Debug.Log("Date Picker cannot use the given date, hence default date has been shown.");
                return;
            }

            SelectedDate = myDate;
            ReferenceDate = myDate;
            SetupVariables();
            // event calling / invoking
            OnDaySelectedEvent(myDate);
        }

        public void Generate()
        {
            int month = ReferenceDate.Month;
            int year = ReferenceDate.Year;
            DateTime dateTime = new DateTime(year, month, 1);
            int day = (int)dateTime.DayOfWeek;
            int no_of_days_in_month = DateTime.DaysInMonth(year, month);
            for (int i = 0; i < DayList.Length; i++)
            {
                if (i < day || i >= (day + no_of_days_in_month))
                {
                    DayList[i].gameObject.SetActive(false);
                    continue;
                }
                DateTime date = new DateTime(year, month, (i - day) + 1);

                switch (pickerRange)
                {
                    case DatePickerRange.ANY:
                        break;
                    case DatePickerRange.FORWARD_ONLY:
                        if (date < DateTime.Today)
                        {
                            DayList[i].gameObject.SetActive(false);
                            continue;
                        }
                        break;
                    case DatePickerRange.BACKWARD_ONLY:
                        if (date > DateTime.Today)
                        {
                            DayList[i].gameObject.SetActive(false);
                            continue;
                        }
                        break;
                }
                DayList[i].gameObject.SetActive(true);
                DayList[i].Setup(date.Day.ToString(), date, SelectedDate == date);
                Button btn = DayList[i].GetComponentInChildren<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    // delegate Instantiation
                    OnDaySelectedDelegate simpleDelegate = new OnDaySelectedDelegate(OnDaySelected);
                    simpleDelegate(date);
                });
            }
        }

        public void GenerateYears()
        {
            int month = ReferenceDate.Month;
            int year = ReferenceDate.Year;
            int minYear = year - YearList.Length / 2;

            for (int i = 0; i < YearList.Length; i++)
            {
                DateTime date = new DateTime(minYear + i, month, 1);
                switch (pickerRange)
                {
                    case DatePickerRange.ANY:
                        break;
                    case DatePickerRange.FORWARD_ONLY:
                        if (date < DateTime.Today)
                        {
                            YearList[i].gameObject.SetActive(false);
                            continue;
                        }
                        break;
                    case DatePickerRange.BACKWARD_ONLY:
                        if (date > DateTime.Today)
                        {
                            YearList[i].gameObject.SetActive(false);
                            continue;
                        }
                        break;
                }

                YearList[i].gameObject.SetActive(true);
                YearList[i].Setup(date.Year.ToString(), date, ReferenceDate.Year == date.Year);
                Button btn = YearList[i].GetComponentInChildren<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    OnYearOrMonthSelected(date);
                });
            }
        }

        public void GenerateMonths()
        {
            int month = ReferenceDate.Month;
            int year = ReferenceDate.Year;

            for (int i = 0; i < MonthList.Length; i++)
            {
                DateTime date = new DateTime(year, i + 1, 1);
                switch (pickerRange)
                {
                    case DatePickerRange.ANY:
                        break;
                    case DatePickerRange.FORWARD_ONLY:
                        if (date < DateTime.Today)
                        {
                            MonthList[i].gameObject.SetActive(false);
                            continue;
                        }
                        break;
                    case DatePickerRange.BACKWARD_ONLY:
                        if (date > DateTime.Today)
                        {
                            MonthList[i].gameObject.SetActive(false);
                            continue;
                        }
                        break;
                }

                MonthList[i].gameObject.SetActive(true);
                MonthList[i].Setup(GetMonth(i + 1), date, ReferenceDate.Month == date.Month);
                Button btn = MonthList[i].GetComponentInChildren<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    OnYearOrMonthSelected(date);
                });
            }
        }

        bool ValidateForwardPickOnly(DateTime date)
        {
            switch (pickerRange)
            {
                case DatePickerRange.ANY:
                    return true;
                case DatePickerRange.FORWARD_ONLY:
                    if (date < DateTime.Today)
                    {
                        return false;
                    }
                    break;
                case DatePickerRange.BACKWARD_ONLY:
                    if (date > DateTime.Today)
                    {
                        return false;
                    }
                    break;
            }

            return true;
        }

        public void ShowYears()
        {
            DaysContainer.SetActive(false);
            MonthContainer.SetActive(false);
            YearsContainer.SetActive(true);

            Invoke("GenerateYears", 0.1f);
        }

        public void ShowMonths()
        {
            DaysContainer.SetActive(false);
            MonthContainer.SetActive(true);
            YearsContainer.SetActive(false);
            Invoke("GenerateMonths", 0.1f);
        }

        public void OnYearOrMonthSelected(DateTime date)
        {
            ReferenceDate = date;
            SetupVariables();
        }
        public void OnYearsFastInc()
        {
            DateTime date = new DateTime(YearList[YearList.Length - 1].dateTime.Year + YearList.Length / 2, ReferenceDate.Month, 1);
            ReferenceDate = date;
            Debug.Log(date);
            GenerateYears();
        }
        public void OnYearsFastDec()
        {
            DateTime date = new DateTime(YearList[0].dateTime.Year - YearList.Length / 2, ReferenceDate.Month, 1);
            ReferenceDate = date;
            Debug.Log(date);
            GenerateYears();
        }

        public void OnYearInc()
        {
            if (!ValidateForwardPickOnly(ReferenceDate.AddYears(1)))
                return;
            ReferenceDate = ReferenceDate.AddYears(1);
            SetupVariables();
        }
        public void OnYearDec()
        {
            if (!ValidateForwardPickOnly(ReferenceDate.AddYears(-1)))
                return;
            ReferenceDate = ReferenceDate.AddYears(-1);
            SetupVariables();
        }
        public void OnMonthInc()
        {
            if (!ValidateForwardPickOnly(ReferenceDate.AddMonths(1)))
                return;
            ReferenceDate = ReferenceDate.AddMonths(1);
            SetupVariables();

        }
        public void OnMonthDec()
        {
            if (!ValidateForwardPickOnly(ReferenceDate.AddMonths(-1)))
                return;
            ReferenceDate = ReferenceDate.AddMonths(-1);
            SetupVariables();

        }
        public void OnDaySelected(DateTime date)
        {
            // event calling / invoking
            OnDaySelectedEvent(date);
            SelectedDate = date;
            ReferenceDate = date;
            SetupVariables();
        }
        public void OnToday()
        {
            ReferenceDate = DateTime.Today;
            SetupVariables();
        }
        public void OnTomo()
        {
            ReferenceDate = DateTime.Today.AddDays(1);
            SetupVariables();
        }
        public void OnCurrentSelectedDay()
        {
            ReferenceDate = SelectedDate;
            SetupVariables();
        }
        public DateTime GetSelectedDate()
        {
            return SelectedDate;
        }
    }
}