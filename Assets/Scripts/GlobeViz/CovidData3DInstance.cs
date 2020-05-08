using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GoogleARCore.Examples.ObjectManipulation;
public class CovidData3DInstance : MonoBehaviour
{
   // [SerializeField]
    public Covid19Data CovidData_Confirmed;
    public Covid19Data CovidData_Death;
    public Covid19Data CovidData_Recovered;
    public SpriteRenderer sRenderer;
    public GameObject highlighter;
    // Start is called before the first frame update
    public Vector3 posOnSphere;
    public Transform Earth;
    public Color32 Color;
    public Color32 SelectionColor;
    public Color32 UnSelectionColor;

    public static GameObject SelectedLocation;
    public float newVal;
    public GameObject SelectionHighlight;
    public float EnlargeScale = 3;

    //public GameObject Label;
    public GameObject Label2;
    public TextMesh infoText;
    public TextMesh locText;
    public TextMesh countryText, confText, deathText, recoText;

    public float thresholdDistance = 0.25f;

    public delegate void ClickedOnData(Transform t);
    public static ClickedOnData ClickedOnDataEvent;

    public Vector3 initScale;
    public static Dictionary<string, GameObject> dataInstanceDict = new Dictionary<string, GameObject>();

    public LineRenderer lr;
    private Vector3 InitScale;
    public void Init(Covid19Data covidData, Transform earth, Color32 color)
    {

        initScale = transform.localScale;
        //SelectionHighlight.SetActive(false);
        CovidData_Confirmed = covidData;
        Earth = earth;
        posOnSphere = AppUtils.LatLonToPositionOnSphere(CovidData_Confirmed.Lat, CovidData_Confirmed.Long, Earth.localScale.x * 2.1f);
        transform.localPosition = posOnSphere;
        Color = color;
        Vector3 dir = new Vector3(0, 1, 0);
        Vector3 crossDir = Vector3.Cross(dir, posOnSphere);
        float angle = Vector3.Angle(dir, posOnSphere);
        transform.Rotate(crossDir, angle, Space.Self);
        float limit = 500;
        float val = CovidData_Confirmed.DayByDayData[CovidData_Confirmed.DayByDayData.Length - 1];
        if (val < limit)
        {
            newVal = AppUtils.Remap(val, 0, limit, 0, 255);
        }
        else
        {
            newVal = 255;
        }
         
        UnSelectionColor = new Color32((byte)newVal, 255, 0, 255);
        sRenderer.color = UnSelectionColor;

        if (CovidData_Confirmed.ProvinceORState == "")
            locText.text = "Not mentioned";
        else
            locText.text = CovidData_Confirmed.ProvinceORState;

        locText.text += ", "+ CovidData_Confirmed.CountryORRegion;

        //Label.SetActive(false);
        Label2.SetActive(false);
        ClickedOnDataEvent += AmINear;
        DataBars.ClickedOnBarEvent += ShowOrHideMe;
        RotationManipulator.RotationGestureHandler += UpdateLineRenderePos;
        sRenderer.enabled = false;

        if (covidData.CountryORRegion == "US")
        {
            if (UnityEngine.Random.Range(1, 100) % 2 == 0)
            {
                gameObject.SetActive(false);
            }
            //transform.localScale /= 10;
        }

        lr = gameObject.GetComponent<LineRenderer>();
        lr.enabled = false;
        lr.startWidth = lr.endWidth = 0.001f;

        countryText.text = CovidData_Confirmed.ProvinceORState;
        InitScale = transform.localScale;
    }

    void OnApplicationQuit()
    {
        ClickedOnDataEvent -= AmINear;
        DataBars.ClickedOnBarEvent -= ShowOrHideMe;
        RotationManipulator.RotationGestureHandler -= UpdateLineRenderePos;
    }

    void UpdateLineRenderePos()
    {
        if(lr.enabled)
        {
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, GraphMaster.Instance.smallGraphContainer.position);
        }  
    }

    void ShowOrHideMe(string Country)
    {
        BoxCollider bcoll = gameObject.GetComponent<BoxCollider>();
        if(Country == CovidData_Confirmed.CountryORRegion)
        {
            sRenderer.enabled = true;
            bcoll.enabled = true;
            //SelectionHighlight.SetActive(true);
        }
        else
        {
            sRenderer.enabled = false;
            bcoll.enabled = false;
            //SelectionHighlight.SetActive(false);
        }
    }

    private void AmINear(Transform t)
    {
        float dist = Vector3.Distance(t.position, transform.position);
        if(dist <thresholdDistance)
        {
            //Label.SetActive(true);
        }
        else
        {
            //Label.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        SimulateMouseDown();

        PlotAllGraphs();

        FocusOnData();

    }

    void SimulateMouseDown()
    {
        if (SelectedLocation != gameObject)
        {
            CovidData3DInstance instance3D;
            if (SelectedLocation != null)
            {
                //SelectedLocation.transform.localScale /= 2;
                StartCoroutine(ScaleAnimation(0.1f, SelectedLocation.transform, SelectedLocation.transform.localScale, initScale));
                //SelectedLocation.GetComponentInChildren<SpriteRenderer>().color = UnSelectionColor;
                instance3D = SelectedLocation.GetComponent<CovidData3DInstance>();
                //instance3D.SelectionHighlight.SetActive(false);
                instance3D.lr.enabled = false;
                if (!instance3D.isInTrigger)
                {//instance3D.Label.SetActive(false);
                 //Label.SetActive(false);
                    instance3D.Label2.SetActive(false);
                }
                instance3D.SelectionHighlight.SetActive(false);
                locText.color = new Color(255, 255, 255, 255);
                infoText.color = new Color(255, 255, 255, 255);
            }
            SelectedLocation = gameObject;
            //SelectedLocation.transform.localScale *= 2;
            StartCoroutine(ScaleAnimation(0.4f, SelectedLocation.transform, SelectedLocation.transform.localScale, initScale * EnlargeScale));
            //SelectedLocation.GetComponentInChildren<SpriteRenderer>().color = SelectionColor;
            instance3D = SelectedLocation.GetComponent<CovidData3DInstance>();
            //instance3D.SelectionHighlight.SetActive(true);
            instance3D.Label2.SetActive(true);
            instance3D.SelectionHighlight.SetActive(true);
            lr.enabled = true;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, GraphMaster.Instance.smallGraphContainer.position);
            //locText.color = new Color(255, 0, 0, 255);
            //infoText.color = new Color(255, 0, 0, 255);
            //ClickedOnDataEvent(transform);

        }


        Debug.Log("Country : " + CovidData_Confirmed.CountryORRegion + ", Max " + CovidData_Confirmed.DayByDayData[CovidData_Confirmed.DayByDayData.Length - 1]);



        UpdateInfo("Confirmed : " + CovidData_Confirmed.DayByDayData[CovidData_Confirmed.DayByDayData.Length - 1]);

        infoText.text = "Confirmed : " + CovidData_Confirmed.DayByDayData[CovidData_Confirmed.DayByDayData.Length - 1];
        confText.text = CovidData_Confirmed.DayByDayData[CovidData_Confirmed.DayByDayData.Length - 1] +"";
        if (CovidData_Death != null)
        {
            infoText.text += "\nDeath : " + CovidData_Death.DayByDayData[CovidData_Death.DayByDayData.Length - 1];
            deathText.text = CovidData_Death.DayByDayData[CovidData_Death.DayByDayData.Length - 1] +"";
        }
            

        if (CovidData_Recovered != null)
        {
            infoText.text += "\nRecovered : " + CovidData_Recovered.DayByDayData[CovidData_Recovered.DayByDayData.Length - 1];
            recoText.text = CovidData_Recovered.DayByDayData[CovidData_Recovered.DayByDayData.Length - 1] +"";
        }
            

        //GraphMaster.Instance.PlotConfirmed(CovidData_Confirmed);
        //if (CovidData_Death != null)
        //{
        //    GraphMaster.Instance.PlotDeath(CovidData_Death);
        //}

        //if (CovidData_Recovered != null)
        //{
        //    GraphMaster.Instance.PlotRecovered(CovidData_Recovered);
        //}

        //GraphMaster.Instance.EnableGraph(true);
    }

    void PlotAllGraphs()
    {
        GraphMaster.Instance.PlotConfirmed(CovidData_Confirmed);

        UpdateInfo("Confirmed : " + CovidData_Confirmed.DayByDayData[CovidData_Confirmed.DayByDayData.Length - 1]);

        if (CovidData_Death != null)
        {
            GraphMaster.Instance.PlotDeath(CovidData_Death);
        }

        if (CovidData_Recovered != null)
        {
            GraphMaster.Instance.PlotRecovered(CovidData_Recovered);
        }

        GraphMaster.Instance.EnableGraph(true);
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

    public void UpdateInfo(string info)
    {
        infoText.text += info;
    }

    public static string lastKey;
    public bool isInTrigger = false;
    public string StaticStirng;
    public string myCountry;
    void OnTriggerEnter()
    {

        //if(!CovidData3DInstance.dataInstanceDict.ContainsKey(CovidData_Confirmed.ProvinceORState))
        //    CovidData3DInstance.dataInstanceDict.Add(CovidData_Confirmed.ProvinceORState, gameObject);

        lastKey = CovidData_Confirmed.ProvinceORState;// + CovidData_Confirmed.Lat + CovidData_Confirmed.Long;
        SimulateMouseDown();
        isInTrigger = true;
        //PlotAllGraphs();
        //GraphMaster.Instance.PlaceGraphAtLocation(transform.position);
    }

   

    void OnTriggerStay()
    {
        //if(CovidData3DInstance.dataInstanceDict.Count > 0)
        //{
        //    lastKey = CovidData3DInstance.dataInstanceDict.First().Key;          
        //}

        //StaticStirng = lastKey;
        //myCountry = CovidData_Confirmed.ProvinceORState; //;+ CovidData_Confirmed.Lat + CovidData_Confirmed.Long ;

        //if (lastKey == myCountry)
        //{
        //    StartCoroutine(ScaleAnimation(0.4f, transform, initScale, initScale * (EnlargeScale / 4)));
        //    highlighter.SetActive(true);
        //}
        //else
        //{
        //    StartCoroutine(ScaleAnimation(0.4f, transform, initScale, initScale));
        //    highlighter.SetActive(false);
        //}
        //isInTrigger = true;
        StartCoroutine(ScaleAnimation(0.4f, transform, initScale, initScale * (EnlargeScale / 4)));
        highlighter.SetActive(true);
        
    
       
    }

    void OnTriggerExit()
    {
        //isInTrigger = false;
        lr.enabled = false;
        
        StartCoroutine(ScaleAnimation(0.4f, transform, initScale, initScale));
        highlighter.SetActive(false);
        //Label.SetActive(false);
        Label2.SetActive(false);
        isInTrigger = false;
        //if(!CovidData3DInstance.isInTrigger)
        //GraphMaster.Instance.PlaceGraphOutSide();
        //if (CovidData3DInstance.dataInstanceDict.ContainsKey(CovidData_Confirmed.ProvinceORState))
        //    CovidData3DInstance.dataInstanceDict.Remove(CovidData_Confirmed.ProvinceORState);
    }
}
