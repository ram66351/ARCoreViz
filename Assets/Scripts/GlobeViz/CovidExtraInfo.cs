using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CovidExtraInfo 
{
    public string Country;
    public string ID;
    public string Age;
    public string Sex;
    public string City;
    public string Province;
    public string wuhan_not_wuhan;
    public float Latitude;
    public float Longitude;
    public string date_onset_symptoms;
    public string date_admission_hospital;
    public string date_confirmation;
    public string symptoms;

        public CovidExtraInfo(string country, float lat, float lon, string age, string sex)
        {
            Country = country;
            Latitude = lat;
            Longitude = lon;
            Age = age;
            Sex = sex;
        }
   }
