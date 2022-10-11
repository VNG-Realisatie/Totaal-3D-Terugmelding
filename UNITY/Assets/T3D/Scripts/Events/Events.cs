using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Events : MonoBehaviour, IUniqueService
{
    public delegate void BimCityJsonEventHandler(string cityJson);
    public event BimCityJsonEventHandler BimCityJsonReceived;

    public void RaiseBimCityJsonReceived(string cityJson)
    {
        BimCityJsonReceived?.Invoke(cityJson);
    }

}
