using System.Collections;
using System.Collections.Generic;
using Netherlands3D.T3D.Uitbouw;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SelectedAddressPanel : MonoBehaviour
{
    [SerializeField]
    private Text text;
    private string sourceText = "<b>Dit is het gekozen adres:</b>\n" +
                                "{0} {1}{2}\n" + // straat nummer nummertoevoeging
                                "{3} {4}\n\n" + // postcode stad
                                "<b>Over dit adres hebben we de volgende gegevens gevonden:</b>\n";
    private string bouwjaarInfo = "Bouwjaar: {0}";
    private string perceelOppervlakteInfo = "PerceelOppervlakte: {0}m²";
    private string mainInfoText;
    public string oorspronkelijkBouwjaar { get; private set; }
    public string perceelOppervlakte { get; private set; }
    private string isMonumentText = "Het gebouw is een rijksmonument";
    private string isBeschermdText = "Het gebouw ligt in een rijksbeschermd stads- of dorpsgezicht";

    private void OnEnable()
    {
        ServiceLocator.GetService<MetadataLoader>().PerceelDataLoaded += Instance_PerceelDataLoaded;
    }

    private void Instance_PerceelDataLoaded(object source, PerceelDataEventArgs args)
    {
        var area = Mathf.RoundToInt(args.Area);
        var data = ServiceLocator.GetService<T3DInit>().HTMLData;
        FormatExtraInfoText(oorspronkelijkBouwjaar, area.ToString(), data.IsMonument, data.IsBeschermd);
    }

    private void OnDisable()
    {
        ServiceLocator.GetService<MetadataLoader>().PerceelDataLoaded -= Instance_PerceelDataLoaded;
    }

    public void SetText(JSONNode extensiveAddressNode)
    {
        var dataContainer = ServiceLocator.GetService<T3DInit>().HTMLData;
        dataContainer.City = extensiveAddressNode["woonplaatsNaam"];
        dataContainer.Street = extensiveAddressNode["korteNaam"];
        dataContainer.HouseNumber = extensiveAddressNode["huisnummer"];
        dataContainer.HouseNumberAddition = extensiveAddressNode["huisnummertoevoeging"];
        dataContainer.ZipCode = extensiveAddressNode["postcode"];
        //this.verblijfsobject_id = adres.adresseerbaarObjectIdentificatie;
        //this.bagids = adres.pandIdentificaties;
        //this.huisletter = adres.huisletter;
        //this.huisnummertoevoeging = adres.huisnummertoevoeging;


        mainInfoText = string.Format(sourceText, dataContainer.Street, dataContainer.HouseNumber, dataContainer.HouseNumberAddition, dataContainer.ZipCode, dataContainer.City);
        text.text = mainInfoText;
        StartCoroutine(GetBouwjaarText(dataContainer.BagId));
        //"<b>Dit is het gekozen adres:</b>\n" +
        //dataContainer.Street + " " + dataContainer.HouseNumber + dataContainer.HouseNumberAddition + "\n" +
        //dataContainer.ZipCode + " " + dataContainer.City + "\n\n" +
        //"<b>Over dit adres hebben we de volgende gegevens gevonden:</b>\n" +
        //"Bouwjaar: {0}\n" +
        //"PerceelOppervlakte: {1}m²";

        //todo: request and set RDPosition in HTMLData, re-enable loading building. request and set monument/beschermdstatus
        //ServiceLocator.GetService<T3DInit>().LoadBuilding();

        //todo: only enable next button when building and perceel are loaded
        //RestrictionChecker.ActiveBuilding.BuildingDataIsProcessed &&
        //RestrictionChecker.ActivePerceel.IsLoaded

        //todo: generate a session id if none is present
    }

    private IEnumerator GetBouwjaarText(string bagId)
    {
        var uri = $"https://api.bag.kadaster.nl/lvbag/individuelebevragingen/v2/panden/{bagId}";
        var uwr = UnityWebRequest.Get(uri);
        uwr.SetRequestHeader("X-Api-Key", T3DInit.X_API_KEY);
        uwr.SetRequestHeader("Accept-Crs", T3DInit.ACCEPT_CRS);

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(uwr.error);
        }
        else
        {
            var node = JSONNode.Parse(uwr.downloadHandler.text);
            var data = ServiceLocator.GetService<T3DInit>().HTMLData;
            FormatExtraInfoText(node["pand"]["oorspronkelijkBouwjaar"], perceelOppervlakte, data.IsMonument, data.IsBeschermd);
        }
    }

    public void FormatExtraInfoText(string bouwjaar, string perceelOppervlakte, bool isMonument, bool isBeschermd)
    {
        print(bouwjaar + "\t" + perceelOppervlakte + "\t" + isMonument + "\t" + isBeschermd); 
        text.text = mainInfoText;
        if (bouwjaar != string.Empty)
        {
            oorspronkelijkBouwjaar = bouwjaar;
            text.text += string.Format(bouwjaarInfo, bouwjaar) + "\n";
        }
        if (perceelOppervlakte != string.Empty)
        {
            this.perceelOppervlakte = perceelOppervlakte;
            text.text += string.Format(perceelOppervlakteInfo, perceelOppervlakte) + "\n";
        }
        if (isMonument)
        {
            text.text += isMonumentText + "\n";
        }
        if (isBeschermd)
        {
            text.text = text.text + isBeschermdText + "\n";
        }
        print(text.text);
    }
}
