using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class SelectedAddressPanel : MonoBehaviour
{
    [SerializeField]
    private Text text;

    public void SetText(JSONNode extensiveAddressNode)
    {
        print(extensiveAddressNode.ToString());
        var dataContainer = ServiceLocator.GetService<T3DInit>().HTMLData;
        dataContainer.City = extensiveAddressNode["woonplaatsNaam"];
        dataContainer.Street = extensiveAddressNode["korteNaam"];
        dataContainer.HouseNumber = extensiveAddressNode["huisnummer"];
        dataContainer.HouseNumberAddition = extensiveAddressNode["huisnummertoevoeging"];
        dataContainer.ZipCode = extensiveAddressNode["postcode"];

        dataContainer.BagId = extensiveAddressNode["pandIdentificaties"][0];
        //this.verblijfsobject_id = adres.adresseerbaarObjectIdentificatie;
        //this.bagids = adres.pandIdentificaties;
        //this.huisletter = adres.huisletter;
        //this.huisnummertoevoeging = adres.huisnummertoevoeging;

        text.text = "<b>Dit is het gekozen adres:</b>\n" +
            dataContainer.Street + " " + dataContainer.HouseNumber + dataContainer.HouseNumberAddition + "\n" +
            dataContainer.ZipCode + " " + dataContainer.City + "\n\n" +
            "<b>Over dit adres hebben we de volgende gegevens gevonden:</b>\n" +
            "Bouwjaar: " + "xxxx" + "\n" +
            "PerceelOppervlakte: " + "0000" + "m²";

        //todo: request and set RDPosition in HTMLData, re-enable loading building. request and set monument/beschermdstatus
        //ServiceLocator.GetService<T3DInit>().LoadBuilding();

        //todo: only enable next button when building and perceel are loaded
        //RestrictionChecker.ActiveBuilding.BuildingDataIsProcessed &&
        //RestrictionChecker.ActivePerceel.IsLoaded

        //todo: generate a session id if none is present
    }
}
