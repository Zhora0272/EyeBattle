using UnityEngine;
using System;
using System.Text;
using TMPro;
using UniRx;
using UnityEngine.UI;

public class SalaryCalculatorTheMonth : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private int _salary;
    [Space] 
    [SerializeField] private Button _settingsButton;
    [SerializeField] private SettingsView _settingsView;
    [Space]
    public int _currentMonthDays;
    public int _days;
    [Space]
    public bool Days;
    public bool Hour;
    public bool Minute;
    public bool Second;
    [Space]
    public float preDays;
    public float preHour;
    public float preMinute;
    public float preSecond;
    [Space]
    [SerializeField] private int salaryGetDay;

    private void Awake()
    {
        _screenText = new StringBuilder();
        
        Application.targetFrameRate = 24;
        Application.runInBackground = true;
        
        _settingsButton.onClick.AddListener(() =>
        {
            _settingsView.Activate();
        });
    }
    
    private void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            NumberCalculate();
        }).AddTo(this);
    }

    private float _moneyPrecentage;
    private StringBuilder _screenText;
    
    private void NumberCalculate()
    {
        _currentMonthDays = DateTime.DaysInMonth(DateTime.Now.Day,DateTime.Now.Month);

        _moneyPrecentage = _salary;

        if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
        {
            _textMeshPro.text = "Time to chill";
            Observable.Timer(TimeSpan.FromSeconds(15)).Subscribe(_ =>
            {
                Application.Quit();
            }).AddTo(this);
            return;
        } 
            
        var currentDay = DateTime.Now;
            
        _days = currentDay.Day - salaryGetDay;


        if (Days)
        {
            preDays = (int)(_moneyPrecentage / _currentMonthDays * _days);
            _screenText.Append(preDays + "\n" + "\n");
        }

        if (Hour)
        {
            preHour = (int)(_moneyPrecentage / _currentMonthDays / 24f * currentDay.Hour);
            _screenText.Append("d " + preHour + "\n");
        }

        if (Minute)
        {
            preMinute = (int)(_moneyPrecentage / _currentMonthDays / 24f / 60f * currentDay.Minute);
            _screenText.Append("h " + preMinute + "\n");
        }

        if (Second)
        {
            preSecond = (int)(_moneyPrecentage / _currentMonthDays / 24f / 60f / 60f * currentDay.Second);
            _screenText.Append("m " + preSecond + "\n");
        }

        _textMeshPro.text = _screenText.ToString();

        _screenText.Clear();

        /*"y." + DateTime.Today.DayOfYear * (_moneyPrecentage / _currentMonthDays) + "\n" + "\n" +
         ((int)((preDays - 1) + (preHour - 1) + (preMinute - 1) + preSecond))
         + "\n" + "\n" +
         "d." + Mathf.Round(preHour)
         + "\n" +
         "h." + Mathf.Round(preMinute)
         + "\n" +
         "m." + Mathf.Round(preSecond) + " s." + DateTime.Now.Second
         + "\n" + DateTime.Today.DayOfWeek
         + "\n" + DateTime.Today.DayOfYear;*/

    }
}
