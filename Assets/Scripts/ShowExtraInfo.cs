using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowExtraInfo : MonoBehaviour
{
    public TextAsset csvData;
    public bool isDataReceived = false;
    private string[,] Grid;
    public GameObject DataBarPrefab;
    public Dictionary<string, List<CovidExtraInfo>> ExtraInfoDictionary = new Dictionary<string, List<CovidExtraInfo>>();
    
    // Start is called before the first frame update
    void Start()
    {
        CSVLoader.CSVLoaderThread(csvData.text, ReceiveData);
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

    public void PopulateData()
    {
        int row = Grid.GetUpperBound(0);
        int col = Grid.GetUpperBound(1);

        //DateTime StartDt, EndDt;
        //CultureInfo culture = new CultureInfo("en-US");
        //DateTime.TryParse(Grid[4, 0], culture, DateTimeStyles.None, out StartDt);
        //DateTime.TryParse(Grid[row - 1, 0], culture, DateTimeStyles.None, out EndDt);
        //int totalDays = (EndDt - StartDt).Days + 1;
        //Debug.Log(StartDt + ", " + EndDt + ", days : " + totalDays);

        for (int i = 0; i < col; i++)
        {
            float lat, lon = 0;
            if (!float.TryParse(Grid[7, i], out lat))
            {
                continue;
            }

            if (!float.TryParse(Grid[8, i], out lon))
            {
                continue;
            }

            string country = Grid[5, i];
            if (Grid[5, i] == "United States")
                country = "US";

            CovidExtraInfo extraInfo = new CovidExtraInfo(country, lat, lon, Grid[1, i], Grid[2, i]);
            extraInfo.City = Grid[3, i];
            extraInfo.Province = Grid[4, i];
            extraInfo.symptoms = Grid[13, i];

            List<CovidExtraInfo> covidList;

            if (ExtraInfoDictionary.ContainsKey(extraInfo.Country))
            {
                covidList = ExtraInfoDictionary[extraInfo.Country];
            }
            else
            {
                covidList = new List<CovidExtraInfo>();
                ExtraInfoDictionary.Add(extraInfo.Country, covidList);
            }
            covidList.Add(extraInfo);
        }

        isDataReceived = false;

        foreach(KeyValuePair<string, List<CovidExtraInfo>> entry in ExtraInfoDictionary)
        {
            // do something with entry.Value or entry.Key
           
            GameObject databar = Instantiate(DataBarPrefab, transform);
            string country = entry.Key;
            databar.name = country;
            if (ExtraInfoDictionary.ContainsKey(country) && LoadCountries.Instance.CountryDict.ContainsKey(country))
            {
                if (Covid19Viz.NoOfCasesConfirmed.ContainsKey(country))
                {
                    List<CovidExtraInfo> infoList = ExtraInfoDictionary[country];
                    Vector2 pos = LoadCountries.Instance.CountryDict[country];
                    databar.GetComponent<DataBars>().Init(country, infoList, pos, transform, Covid19Viz.NoOfCasesConfirmed[entry.Key]);
                } 
            }
        }

        Debug.Log("Total Recored : "+ ExtraInfoDictionary.Count);
        foreach(var entry in ExtraInfoDictionary)
        {
            Debug.Log(entry.Key);
        }
    }


        // Update is called once per frame
        void Update()
    {
        
    }
}
