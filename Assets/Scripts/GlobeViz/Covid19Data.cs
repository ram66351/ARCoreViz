using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//[Serializable]
public class Covid19Data 
{
    public string ProvinceORState;
    public string CountryORRegion;
    public float Lat;
    public float Long;
    public DateTime StartDate;
    public DateTime EndDate;
    public enum Status
    {
        confirmed, dead, recovered
    };
    public Status status;
    public int[] DayByDayData;

    

    public Covid19Data(string state, string country, float lat, float lon, DateTime fd, DateTime ld, Status s, int[] dayByDayData)
    {
        ProvinceORState = state;
        CountryORRegion = country;
        Lat = lat;
        Long = lon;
        StartDate = fd;
        EndDate = ld;
        DayByDayData = dayByDayData;
    }
}
