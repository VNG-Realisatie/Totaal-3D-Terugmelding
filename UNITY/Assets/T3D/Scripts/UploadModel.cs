using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;
using Netherlands3D;
using SimpleJSON;
using Netherlands3D.Events;

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

namespace WebGLFileUploaderExample
{
    /// <summary>
    /// File Upload example.
    /// </summary>
    public class UploadModel : MonoBehaviour
    {
        public Text debugText;
        public Slider slider;

        public bool IsLoading { get; private set; } = false;

        [SerializeField]
        private StringEvent ModelCityJSONReceived;

        /// <summary>
        /// Raises the file uploaded event.
        /// </summary>
        /// <param name="files">Uploaded file infos.</param>
        public void OnFileUploaded(string file)
        {            
            ServiceLocator.GetService<T3DInit>().HTMLData.HasFile = true;

            FileInfo finfo = new FileInfo(file);

            
            Debug.Log("file: " + file);
            debugText.text = "file: " + file;

            var url = $"{Config.activeConfiguration.T3DAzureFunctionURL}api/uploadbim/{Uri.EscapeDataString(finfo.Name)}";

#if UNITY_WEBGL && !UNITY_EDITOR
            StartCoroutine(UploadAndCheck(url, Path.Combine(Application.persistentDataPath,file)));
#else
            StartCoroutine(UploadAndCheck(url, file));
#endif
            
        }

        IEnumerator UploadAndCheck(string url, string filePath)
        {
            IsLoading = true;

            CoString result = new CoString();
            CoBool success = new CoBool();

            debugText.text = "Status: \"Uploaden bestand...\"";

            yield return StartCoroutine(UploadBimUtility.UploadFile(result, success, slider, url, filePath));

            if (success == false)
            {
                debugText.text = $"Status: \"Probleem met uploaden: {result}\"";
                yield break;
            }

            var jsonResult = JSON.Parse(result);

            bool isIfc = false;

            if (filePath.ToLower().EndsWith(".skp"))
            {
                ServiceLocator.GetService<T3DInit>().HTMLData.ModelId = null;
                ServiceLocator.GetService<T3DInit>().HTMLData.BlobId = jsonResult["blobId"];
                debugText.text = "Status: \"Sketchup bestand geconverteerd\"";
            }
            else if (filePath.ToLower().EndsWith(".ifc"))
            {
                ServiceLocator.GetService<T3DInit>().HTMLData.ModelId = jsonResult["modelId"];
                ServiceLocator.GetService<T3DInit>().HTMLData.BlobId = null;
                debugText.text = "Status: \"IFC bestand geupload\"";

                var urlIfc = Config.activeConfiguration.T3DAzureFunctionURL + $"api/getbimversionstatus/{ServiceLocator.GetService<T3DInit>().HTMLData.ModelId}";
                yield return StartCoroutine(UploadBimUtility.CheckBimVersion(debugText, urlIfc, result, success));
            }

            //the file has been successfully converted to CityJson, now lets get the CityJson
            if (success)
            {
                yield return StartCoroutine(UploadBimUtility.GetBimCityJson(result, success));

                //The CityJson has been downloaded, now lets visualize it
                if (success)
                {
                    Debug.Log("-------BimCityJsonReceived");
                    ModelCityJSONReceived.Invoke(result);
                }
                else
                {
                    debugText.text = result;
                    ErrorService.GoToErrorPage(result);
                }
            }

            IsLoading = false;
        }

    }
}
