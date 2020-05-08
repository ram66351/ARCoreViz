using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class DataBars : MonoBehaviour
{
    public GameObject Bar;
    public Vector3 posOnSphere;
    public int TotalConfirmed;
    public int TotalDeath;
    public int TotalRecovered;
    public List<CovidExtraInfo> ExtraInfo;
    private string Country;
    public Dictionary<string, int> AgeDictionary = new Dictionary<string, int>();
    public Dictionary<string, int> SexDictionary = new Dictionary<string, int>();
    public int[] DayByDayInfo;
    public List<GameObject> Graphlines = new List<GameObject>();
    public Material linemat_conf, linemat_dead, linemat_recovered;
    public TextMesh Label;

    public delegate void ClickedOnBar(string country);
    public static ClickedOnBar ClickedOnBarEvent;

    public static GameObject SelectedLocation;
    public Color32 SelectionColor;
    public Color32 UnSelectionColor;
    public float EnlargeScale = 3;

    
    public void Init(string country, List<CovidExtraInfo> extraInfo, Vector2 loc, Transform earth, int totalConfirmed)
    {
        Country = country;
        Label.text = Country;
        ExtraInfo = extraInfo;
        TotalConfirmed = totalConfirmed;
        posOnSphere = AppUtils.LatLonToPositionOnSphere(loc.x, loc.y, earth.localScale.x * 2);
        transform.localPosition = posOnSphere;
        Vector3 dir = new Vector3(0, 1, 0);
        Vector3 crossDir = Vector3.Cross(dir, posOnSphere);
        float angle = Vector3.Angle(dir, posOnSphere);
        transform.Rotate(crossDir, angle, Space.Self);
        float scaleY = Mathf.Clamp(TotalConfirmed / 1000, 2, 15);
        Bar.transform.localScale = new Vector3(Bar.transform.localScale.x, scaleY, Bar.transform.localScale.z);

        Label.transform.localPosition = new Vector3(0, scaleY/100 + 0.02f, 0);

        BoxCollider col1 = Bar.GetComponent<BoxCollider>();
        BoxCollider col2 = gameObject.AddComponent<BoxCollider>();
        scaleY = scaleY / 100;
        col2.size = new Vector3(col1.size.x, scaleY , col1.size.z);

        col2.center = new Vector3(col2.center.x, scaleY/2, col2.center.z);
        //col2.isTrigger = true;


        col1.enabled = false;
        ClickedOnBarEvent += ShowOrHideGraph;
        //Getting Age and Numbers

        for (int i=0; i < ExtraInfo.Count; i++)
        {
            float age;
            if (float.TryParse(ExtraInfo[i].Age, out age))
            {
               
                for (int j = 0; j < 90; j += 10)
                {
                    if (age >= j && age <= j + 9)
                    {
                        string key = (j + "_to_" + (j + 9)).ToString();
                        if (AgeDictionary.ContainsKey(key))
                        {
                            AgeDictionary[key] += 1;
                        }
                        else
                        {
                            AgeDictionary[key] = 1;
                        }
                    }
                }
            }
            
            if(ExtraInfo[i].Sex == "male")
            {
                if(SexDictionary.ContainsKey("male"))
                {
                    SexDictionary["male"] += 1;
                }
                else
                {
                    SexDictionary.Add("male", 1);
                }
            }
            else if(ExtraInfo[i].Sex == "female")
            {
                if (SexDictionary.ContainsKey("female"))
                {
                    SexDictionary["female"] += 1;
                }
                else
                {
                    SexDictionary.Add("female", 1);
                }
            }
            else
            {
                if (SexDictionary.ContainsKey("unknown"))
                {
                    SexDictionary["unknown"] += 1;
                }
                else
                {
                    SexDictionary.Add("unknown", 1);
                }
            }
        }

        StartCoroutine(PlotGraphWithDelay(0.5f));
        //Getting Confirmed cases DayByDay
    }

    void OnApplicationQuit()
    {

        ClickedOnBarEvent -= ShowOrHideGraph;
    }

    void ShowOrHideGraph(string country)
    {
        if (country == Country)
        {
            //Graphline.gameObject.SetActive(true);
            GraphLineVisibility(true);
        }
        else
        {
            //Graphline.gameObject.SetActive(false);
            GraphLineVisibility(false);
        }
    }

    void GraphLineVisibility(bool enable)
    {
        for(int i=0; i<Graphlines.Count; i++)
        {
            if (Graphlines[i] != null)
                Graphlines[i].SetActive(enable);
        }
    }

    void OnMouseDown()
    {
        SimulateMouseDown();
    }

    void SimulateMouseDown()
    {
        ClickedOnBarEvent(Country);

        string symptoms = "";
        for (int x = 0; x < ExtraInfo.Count; x++)
        {
            symptoms += ExtraInfo[x].symptoms + ", ";
        }

        GraphMaster.Instance.PlotCountryInfo(Country, TotalConfirmed, Covid19Viz.NoOfCasesDead[Country], Covid19Viz.NoOfCasesRecovered[Country], symptoms);
        //PlotGraph();
        //for (int i = 0; i < ExtraInfo.Count; i++)
        //{
        //    Debug.Log("Age : "+ ExtraInfo[i].Age);
        //}

        if (SelectedLocation != gameObject)
        {
            if (SelectedLocation != null)
            {
                //SelectedLocation.transform.localScale /= 2;
                StartCoroutine(ScaleAnimation(0.1f, SelectedLocation.transform, SelectedLocation.transform.localScale, SelectedLocation.transform.localScale / EnlargeScale));
                SelectedLocation.GetComponentInChildren<TextMesh>().color = UnSelectionColor;
            }

            SelectedLocation = gameObject;
            //SelectedLocation.transform.localScale *= 2;

            StartCoroutine(ScaleAnimation(0.4f, SelectedLocation.transform, SelectedLocation.transform.localScale, SelectedLocation.transform.localScale * EnlargeScale));
            SelectedLocation.GetComponentInChildren<TextMesh>().color = SelectionColor;
        }

        FocusOnData();
        GraphMaster.Instance.EnableGraph(false);
        AgeGraph.Instance.Plot(AgeDictionary);

        //foreach (var entry in SexDictionary)
        //{
        //    Debug.Log(entry.Key + " : " + entry.Value);
        //}
    }

    IEnumerator ScaleAnimation(float dt, Transform obj, Vector3 from, Vector3 to)
    {
        //obj.localScale = from;
        float t = 0;
        while (t < 3)
        {
            yield return null;
            t += Time.deltaTime / dt;
            obj.localScale = Vector3.Lerp(from, to, t);
        }
    }

    public void FocusOnData()
    {
        Transform earth = Covid19Viz.Instance.Earth;
        Quaternion oldRotation = earth.rotation;
        earth.rotation = Quaternion.identity;
        Vector3 fromDirection = (transform.position - earth.position).normalized;
        Vector3 toDirection = (Camera.main.transform.position - earth.position).normalized;
        Quaternion fromRotation = Quaternion.Inverse(Quaternion.LookRotation(fromDirection, Vector3.up));
        Quaternion targetRotation = Quaternion.LookRotation(toDirection, Vector3.up) * fromRotation;
        StartCoroutine(AnimateEarthRotation(0.8f, earth, oldRotation, targetRotation));
    }

    IEnumerator AnimateEarthRotation(float dt, Transform earth, Quaternion from, Quaternion to)
    {
        earth.rotation = from;
        float t = 0;
        while (t < 3)
        {
            yield return null;
            t += Time.deltaTime / dt;
            earth.rotation = Quaternion.Lerp(from, to, t);
        }

    }

IEnumerator PlotGraphWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlotGraph();
    }
    void PlotGraph()
    {
        if (Covid19Viz.CountryConfirmedData.ContainsKey(Country))
        {
            var data = Covid19Viz.CountryConfirmedData[Country];

            for (int b = 0; b < data.Count; b++)
            {
                if (b == 0)
                {
                    DayByDayInfo = data[0].DayByDayData;

                }
                for (int c = 0; c < DayByDayInfo.Length; c++)
                {
                    DayByDayInfo[c] += data[b].DayByDayData[c];
                }
            }
        }
        else
        {
            Debug.Log("Country Not Found !");
        }
        GraphMaster.Instance.PlotAllCountryConfirmed(DayByDayInfo, Country, linemat_conf, GetGraphLine, Covid19Data.Status.confirmed);

        if (Covid19Viz.CountryDeathData.ContainsKey(Country))
        {
            var data = Covid19Viz.CountryDeathData[Country];

            for (int b = 0; b < data.Count; b++)
            {
                if (b == 0)
                {
                    DayByDayInfo = data[0].DayByDayData;

                }
                for (int c = 0; c < DayByDayInfo.Length; c++)
                {
                    DayByDayInfo[c] += data[b].DayByDayData[c];
                }
            }
        }
        else
        {
            Debug.Log("Country Not Found !");
        }
        GraphMaster.Instance.PlotAllCountryConfirmed(DayByDayInfo, Country, linemat_dead, GetGraphLine, Covid19Data.Status.dead);

        if (Covid19Viz.CountryRecoveredData.ContainsKey(Country))
        {
            var data = Covid19Viz.CountryRecoveredData[Country];

            for (int b = 0; b < data.Count; b++)
            {
                if (b == 0)
                {
                    DayByDayInfo = data[0].DayByDayData;

                }
                for (int c = 0; c < DayByDayInfo.Length; c++)
                {
                    DayByDayInfo[c] += data[b].DayByDayData[c];
                }
            }
        }
        else
        {
            Debug.Log("Country Not Found !");
        }
        GraphMaster.Instance.PlotAllCountryConfirmed(DayByDayInfo, Country, linemat_recovered, GetGraphLine, Covid19Data.Status.recovered);


        ClickedOnBarEvent("");
    }

    void GetGraphLine(GameObject graphline)
    {
        Graphlines.Add(graphline);
    }

    //void OnTriggerEnter()
    //{
    //    StartCoroutine(ScaleAnimation(0.4f, transform, transform.localScale, transform.localScale * EnlargeScale));
    //}

    //void OnTriggerExit()
    //{
    //    StartCoroutine(ScaleAnimation(0.4f, transform, transform.localScale, transform.localScale / EnlargeScale));
    //}
}
