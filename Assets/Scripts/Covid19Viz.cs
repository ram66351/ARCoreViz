using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
public class Covid19Viz : MonoBehaviour
{
    public TextAsset csvData;
    public bool isDataReceived = false;
    private string[,] Grid;
    public GameObject DataPointPrefab;
    public Covid19Data.Status status;
    public Color32 dataColor;
    public Transform Earth;

    public static Dictionary<string, GameObject> CovidLoc = new Dictionary<string, GameObject>();
    // Start is called before the first frame update
    public static Covid19Viz Instance;

    public delegate void StopPlaneDetection();
    public static StopPlaneDetection StopPlaneDetectionEvent;
    public static Dictionary<string, int> NoOfCasesConfirmed = new Dictionary<string, int>();
    public static Dictionary<string, int> NoOfCasesDead = new Dictionary<string, int>();
    public static Dictionary<string, int> NoOfCasesRecovered = new Dictionary<string, int>();
    public static Dictionary<string, List<Covid19Data>> CountryConfirmedData = new Dictionary<string, List<Covid19Data>>();
    public static Dictionary<string, List<Covid19Data>> CountryDeathData = new Dictionary<string, List<Covid19Data>>();
    public static Dictionary<string, List<Covid19Data>> CountryRecoveredData = new Dictionary<string, List<Covid19Data>>();

    void Start()
    {
        Init();
    }

    public void Init()
    {
        Instance = this;
        Earth = transform;
        CSVLoader.CSVLoaderThread(csvData.text, ReceiveData);


        if (Application.platform == RuntimePlatform.Android)
        {
            StopPlaneDetectionEvent();
        }
    }

    void ReceiveData(string[,] data)
    {
        Grid = data;
        Debug.Log("Received Data ! " + Grid.GetUpperBound(0) + ", " + Grid.GetUpperBound(1));
        isDataReceived = true;
    }



    void LateUpdate()
    {
        if (isDataReceived)
        {
            PopulateData();
        }
    }

    void PopulateData()
    {
       int row = Grid.GetUpperBound(0);
       int col = Grid.GetUpperBound(1);

        DateTime StartDt, EndDt;
        CultureInfo culture = new CultureInfo("en-US");

        Debug.Log(Grid[4, 0] +", "+ Grid[row -2, 0]);

        DateTime.TryParse(Grid[4, 0], culture, DateTimeStyles.None, out StartDt);
        DateTime.TryParse(Grid[row - 2, 0], culture, DateTimeStyles.None, out EndDt);
        int totalDays = (EndDt - StartDt).Days;
        GraphMaster.Instance.SetupDates(StartDt, EndDt);
        Debug.Log(StartDt +", "+ EndDt +", days : "+totalDays);
        for (int i = 0; i < col; i++)
        {
            float lat, lon = 0;
            if (!float.TryParse(Grid[2, i], out lat))
            {
                continue;
            }

            if (!float.TryParse(Grid[3, i], out lon))
            {
                continue;
            }

            DateTime dt = DateTime.Now;

            int[] DayByDay = new int[totalDays];
            for (int j = 4; j < row - 2; ++j)
            {
                int val = 0;
                int.TryParse(Grid[j, i], out val);
                DayByDay[j - 4] = val;
            }

            Covid19Data covidData = new Covid19Data(Grid[0, i],
                                                    Grid[1, i],
                                                    lat,
                                                    lon,
                                                    StartDt,
                                                    EndDt,
                                                    status,
                                                    DayByDay
            );

            string loc = covidData.CountryORRegion + lat.ToString() + lon.ToString();
            //Order of Priority
            //Confirmed is the primary Data, then death and then recovered
            //so the 3Dinstance is created based on confirmed and then updating that instance with death and recovered
            if (status == Covid19Data.Status.confirmed)
            {
                if (NoOfCasesConfirmed.ContainsKey(covidData.CountryORRegion))
                {
                    NoOfCasesConfirmed[covidData.CountryORRegion] += covidData.DayByDayData[covidData.DayByDayData.Length - 1];
                }
                else
                {
                    NoOfCasesConfirmed.Add(covidData.CountryORRegion, covidData.DayByDayData[covidData.DayByDayData.Length - 1]);
                }

                if(CountryConfirmedData.ContainsKey(covidData.CountryORRegion))
                {
                    CountryConfirmedData[covidData.CountryORRegion].Add(covidData);
                }
                else
                {
                    List<Covid19Data> list = new List<Covid19Data>();
                    list.Add(covidData);
                    CountryConfirmedData[covidData.CountryORRegion] = list;
                }
            }
            else if(status == Covid19Data.Status.dead)
            {
                if (NoOfCasesDead.ContainsKey(covidData.CountryORRegion))
                {
                    NoOfCasesDead[covidData.CountryORRegion] += covidData.DayByDayData[covidData.DayByDayData.Length - 1];
                }
                else
                {
                    NoOfCasesDead.Add(covidData.CountryORRegion, covidData.DayByDayData[covidData.DayByDayData.Length - 1]);
                }

                if (CountryDeathData.ContainsKey(covidData.CountryORRegion))
                {
                    CountryDeathData[covidData.CountryORRegion].Add(covidData);
                }
                else
                {
                    List<Covid19Data> list = new List<Covid19Data>();
                    list.Add(covidData);
                    CountryDeathData[covidData.CountryORRegion] = list;
                }
            }
            else if (status == Covid19Data.Status.recovered)
            {
                if (NoOfCasesRecovered.ContainsKey(covidData.CountryORRegion))
                {
                    NoOfCasesRecovered[covidData.CountryORRegion] += covidData.DayByDayData[covidData.DayByDayData.Length - 1];
                }
                else
                {
                    NoOfCasesRecovered.Add(covidData.CountryORRegion, covidData.DayByDayData[covidData.DayByDayData.Length - 1]);
                }

                if (CountryRecoveredData.ContainsKey(covidData.CountryORRegion))
                {
                    CountryRecoveredData[covidData.CountryORRegion].Add(covidData);
                }
                else
                {
                    List<Covid19Data> list = new List<Covid19Data>();
                    list.Add(covidData);
                    CountryRecoveredData[covidData.CountryORRegion] = list;
                }
            }



            if (CovidLoc.ContainsKey(loc))
            {
                //Debug.Log("Key Already Existing " + loc +", at : " + i);
                if(status == Covid19Data.Status.dead)
                {
                   //Debug.Log("Adding Covid19 death data");
                    CovidLoc[loc].GetComponent<CovidData3DInstance>().CovidData_Death = covidData;
                }
                else if(status == Covid19Data.Status.recovered)
                {
                    CovidLoc[loc].GetComponent<CovidData3DInstance>().CovidData_Recovered = covidData;
                }
                
            }
            else
            {
                GameObject dataPointClone = Instantiate(DataPointPrefab, transform) as GameObject;
                dataPointClone.GetComponent<CovidData3DInstance>().Init(covidData, transform, dataColor);
                CovidLoc.Add(loc, dataPointClone);

                if(covidData.CountryORRegion == "US")
                {
                    if(i%10 != 0)
                    {
                        dataPointClone.SetActive(false);
                    }
                }
            }
        }

        //foreach(var entry in CountryConfirmedData)
        //{
        //    Debug.Log(entry.Key +" "+entry.Value.Count);
        //}

        isDataReceived = false;
    }
}
