using System.Collections;
using System.IO;
using UnityEngine;
using WebGLFileUploader;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using Netherlands3D;
using UnityEngine.Networking;

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

        /// <summary>
        /// Raises the file uploaded event.
        /// </summary>
        /// <param name="result">Uploaded file infos.</param>
        private void OnFileUploaded(UploadedFileInfo[] result)
        {
            if (result.Length == 0)
            {
                Debug.Log("File upload Error!");
            }
            else
            {
                Debug.Log("File upload success! (result.Length: " + result.Length + ")");
            }

            foreach (UploadedFileInfo file in result)
            {
                if (file.isSuccess)
                {
                    Debug.Log("file.filePath: " + file.filePath + " exists:" + File.Exists(file.filePath));
                    var extension = Path.GetExtension(file.filePath);
                    Debug.Log("file extension: " + extension);
                    if (extension == ".skp")
                    {
                        var url = Config.activeConfiguration.T3DAzureFunctionURL + "sketchup2cityjson/";
                        StartCoroutine(ProcessFileConversion(url, file.filePath));
                    }
                    else if (extension == ".ifc")
                    {
                        var url = Config.activeConfiguration.T3DAzureFunctionURL + "UploadBim/";
                        StartCoroutine(ProcessFileConversion(url, file.filePath));
                    }

                    //Texture2D texture = new Texture2D(2, 2);
                    //texture.LoadImage(byteArray);
                    //gameObject.GetComponent<Renderer>().material.mainTexture = texture;

                    //Debug.Log("File.ReadAllBytes:byte[].Length: " + byteArray.Length);

                    break;
                }
            }
        }

        private IEnumerator ProcessFileConversion(string url, string filePath)
        {
            byte[] byteArray = File.ReadAllBytes(filePath);
            UnityWebRequest req = UnityWebRequest.Put(url, byteArray);
            Debug.Log("sending webRequest to " + url);
            yield return req.SendWebRequest();
            Debug.Log("sent webRequest");

            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
            {
                ErrorService.GoToErrorPage(req.error);
            }
            else
            {
                Debug.Log("request success");
                print(req.downloadHandler.text);
            }
        }

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
            UpdateHTMLButtonVisibility();

            if (htmlIsVisible)
            {
                RecalculatePositionAndSize();
                WebGLFileUploadManager.UpdateButtonPosition(x, y, w, h);
            }
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
