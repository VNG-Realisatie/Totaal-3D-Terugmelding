using System.Collections;
using System.IO;
using UnityEngine;
using WebGLFileUploader;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using Netherlands3D;
using UnityEngine.Networking;
using System.Web;
using System.Threading.Tasks;
using SimpleJSON;
using T3D.Uitbouw;
using UnityEditor;

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
        private int x, y, w, h;
        private bool htmlIsVisible;
        private bool unityIsVisible => gameObject.activeInHierarchy;
        // Use this for initialization

        public Text debugText;
        public Slider slider;

        void Start()
        {
            Debug.Log("WebGLFileUploadManager.getOS: " + WebGLFileUploadManager.getOS);
            Debug.Log("WebGLFileUploadManager.isMOBILE: " + WebGLFileUploadManager.IsMOBILE);
            Debug.Log("WebGLFileUploadManager.getUserAgent: " + WebGLFileUploadManager.GetUserAgent);

            WebGLFileUploadManager.SetDebug(true);
            if (
#if UNITY_WEBGL && !UNITY_EDITOR
                    WebGLFileUploadManager.IsMOBILE 
#else
                    Application.isMobilePlatform
#endif
            )
            {
                htmlIsVisible = WebGLFileUploadManager.Show(false);
                WebGLFileUploadManager.SetDescription("Select image files (.png|.jpg|.gif)");

            }
            else
            {
                htmlIsVisible = WebGLFileUploadManager.Show(true);
                WebGLFileUploadManager.SetDescription("Drop image files (.png|.jpg|.gif) here");
            }
            WebGLFileUploadManager.SetImageEncodeSetting(true);
            //WebGLFileUploadManager.SetAllowedFileName("\\.(png|jpe?g|gif)$");
            WebGLFileUploadManager.SetAllowedFileName("\\.(skp|ifc|json)$"); // todo: allow only 1 file
            WebGLFileUploadManager.SetImageShrinkingSize(1280, 960);
            WebGLFileUploadManager.onFileUploaded += OnFileUploaded;

            ShowHTMLOverlayButton();
        }

        /// <summary>
        /// Raises the destroy event.
        /// </summary>
        void OnDestroy()
        {
            WebGLFileUploadManager.onFileUploaded -= OnFileUploaded;
            WebGLFileUploadManager.Dispose();
        }

        private void OnDisable()
        {
            Debug.Log("UploadModel OnEnable:false");
            WebGLFileUploadManager.Show(false, false);
        }

        private void OnEnable()
        {
            Debug.Log("UploadModel OnEnable:true");
            WebGLFileUploadManager.Show(false, true);
            
            RecalculatePositionAndSize();
            WebGLFileUploadManager.UpdateButtonPosition(x, y, w, h);
        }


        /// <summary>
        /// Raises the file uploaded event.
        /// </summary>
        /// <param name="files">Uploaded file infos.</param>
        private void OnFileUploaded(UploadedFileInfo[] files)
        {
            if (files.Length == 0)
            {
                Debug.Log("File upload Error!");
                debugText.text = "File upload Error!";
            }
            else                        
            {
                ServiceLocator.GetService<T3DInit>().HTMLData.HasFile = true;

                Debug.Log("File upload success! (result.Length: " + files.Length + ")");
                debugText.text = "File upload success!(result.Length: " + files.Length + ")";

                UploadedFileInfo file = files[0];
                if (file.isSuccess)
                {
                    Debug.Log("file.filePath: " + file.filePath + " exists:" + File.Exists(file.filePath));
                    debugText.text = "file.filePath: " + file.filePath + " exists:" + File.Exists(file.filePath);

                    var url = $"{Config.activeConfiguration.T3DAzureFunctionURL}api/uploadbim/{Uri.EscapeDataString(file.name)}";

                    StartCoroutine(UploadAndCheck(url, file.filePath));
                 
                }
            }
        }

#if UNITY_EDITOR
        public void UploadFromEditor()
        {
            string path = EditorUtility.OpenFilePanel("Laad bestand", "", "ifc,skp");
            FileInfo finfo = new FileInfo(path);

            if (path.Length != 0)
            {
                var url = $"{Config.activeConfiguration.T3DAzureFunctionURL}api/uploadbim/{Uri.EscapeDataString( finfo.Name )}";
                StartCoroutine(UploadAndCheck(url, path));
                
            }
        }
#endif

        IEnumerator UploadAndCheck(string url, string filePath)
        {
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
            else
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
                    ServiceLocator.GetService<Events>().RaiseBimCityJsonReceived(result);
                }
                else
                {
                    debugText.text = result;
                    ErrorService.GoToErrorPage(result);
                }
            }
            

        }




        //public IEnumerator ProcessFileConversion(string url, string filePath)
        //{
        //    byte[] byteArray = File.ReadAllBytes(filePath);
        //    UnityWebRequest req = UnityWebRequest.Put(url, byteArray);
        //    Debug.Log("sending webRequest to " + url);
        //    yield return req.SendWebRequest();
        //    Debug.Log("sent webRequest");

        //    if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
        //    {
        //        ErrorService.GoToErrorPage(req.error);
        //    }
        //    else
        //    {
        //        Debug.Log("request success");
        //        print(req.downloadHandler.text);
        //    }
        //}

        /// <summary>
        /// Raises the back button click event.
        /// </summary>
        public void OnBackButtonClick()
        {
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
            SceneManager.LoadScene("WebGLFileUploaderExample");
#else
            Application.LoadLevel("WebGLFileUploaderExample");
#endif
        }

        /// <summary>
        /// Raises the switch button overlay state button click event.
        /// </summary>
        public void ShowHTMLOverlayButton()
        {
            //WebGLFileUploadManager.Show(false, !WebGLFileUploadManager.IsOverlay);
            RecalculatePositionAndSize();
            htmlIsVisible = WebGLFileUploadManager.Show(false, true, x, y, w, h);
            WebGLFileUploadManager.UpdateButtonPosition(x, y, w, h);
        }

        private void RecalculatePositionAndSize()
        {
            var r = GetComponent<RectTransform>();
            var canvas = r.GetComponentInParent<Canvas>();

            //set anchor and pivot to left top, as this is where the HTML button is anchored
            r.anchorMin = new Vector2(0, 1);
            r.anchorMax = new Vector2(0, 1);
            r.pivot = new Vector2(0, 1);

            //Vector2 rectpos = new Vector2(r.rect.x, r.rect.y);
            //var screenpoint = RectTransformUtility.PixelAdjustPoint(rectpos, transform, canvas);
            //x = (int)screenpoint.x;
            //y = (int)screenpoint.y;

            x = (int)transform.position.x;
            y = (int)transform.position.y;

            w = (int)(r.sizeDelta.x * canvas.scaleFactor);
            h = (int)(r.sizeDelta.y * canvas.scaleFactor);

        }

        public void SetX(string input)
        {
            x = int.Parse(input);
        }
        public void SetY(string input)
        {
            y = int.Parse(input);
        }
        public void SetW(string input)
        {
            w = int.Parse(input);
        }
        public void SetH(string input)
        {
            h = int.Parse(input);
        }

        private void Update()
        {
            //UpdateHTMLButtonVisibility();

            //if (htmlIsVisible)
            //{
            //    RecalculatePositionAndSize();
            //    WebGLFileUploadManager.UpdateButtonPosition(x, y, w, h);
            //}
        }

        private void UpdateHTMLButtonVisibility()
        {
            if (htmlIsVisible && !unityIsVisible)
            {
                WebGLFileUploadManager.Hide();
            }
            else if (!htmlIsVisible && unityIsVisible)
            {
                htmlIsVisible = WebGLFileUploadManager.Show(false);
            }
        }

        

    }
}
