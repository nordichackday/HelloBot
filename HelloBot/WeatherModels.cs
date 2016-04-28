using System;
using System.Collections.Generic;

namespace HelloBot.Weather
{
    [Serializable]
    public class WeatherNowNextNext
    {
        public string Df { get; set; }
        public string Dtt { get; set; }
        public DateTime Dt { get; set; }
        public int Wd { get; set; }
        public int Ws { get; set; }
        public int C { get; set; }
        public int T { get; set; }
        public int S { get; set; }
        public string Prosa { get; set; }
        public double P { get; set; }
        public string W { get; set; }
        public string Sr { get; set; }
        public string Ss { get; set; }
        public string Dl { get; set; }
    }

    [Serializable]
    public class WeatherNextFiveDay
    {
        public string Df { get; set; }
        public string Dtt { get; set; }
        public DateTime Dt { get; set; }
        public int Wd { get; set; }
        public int Ws { get; set; }
        public int C { get; set; }
        public int T { get; set; }
        public int S { get; set; }
        public string Prosa { get; set; }
        public double P { get; set; }
        public string W { get; set; }
        public string Sr { get; set; }
        public string Ss { get; set; }
        public string Dl { get; set; }
    }

    [Serializable]
    public class Detailed
    {
        public List<WeatherNowNextNext> WeatherNowNextNext { get; set; }
        public List<WeatherNextFiveDay> WeatherNextFiveDays { get; set; }
    }

    [Serializable]
    public class Location
    {
        public string N { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Wiki { get; set; }
        public string Fc { get; set; }
        public string Fn { get; set; }
        public int Gid { get; set; }
        public object Refresh { get; set; }
        public int? p { get; set; }
        public object Tst { get; set; }
    }

    [Serializable]
    public class PointForecastShort
    {
        public string Df { get; set; }
        public string Dtt { get; set; }
        public DateTime Dt { get; set; }
        public int Wd { get; set; }
        public int Ws { get; set; }
        public int C { get; set; }
        public int T { get; set; }
        public int S { get; set; }
        public string Prosa { get; set; }
        public double P { get; set; }
        public string W { get; set; }
        public string Sr { get; set; }
        public string Ss { get; set; }
        public string Dl { get; set; }
    }

    [Serializable]
    public class WeatherLocation
    {
        public Location Location { get; set; }
        public List<PointForecastShort> PointForecastShort { get; set; }
    }

    [Serializable]
    public class WeatherObject
    {
        public List<Detailed> Detailed { get; set; }
        public List<WeatherLocation> WeatherLocation { get; set; }
    }

    [Serializable]
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Type { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public int Zoom { get; set; }
        public string Admin1Code { get; set; }
    }
   
}