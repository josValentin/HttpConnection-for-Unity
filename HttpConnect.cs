using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace HttpConnection
{
    /*
     * Code to help with Unity Http Requests, created by jos_valentin
     */

    //--------------------- SELECTABLE HTTP REQUEST METHODS
    public enum REQUEST_METHOD
    {
        POST,
        GET,
        PUT,
        DELETE
    }

    public static class HttpConnect
    {

        #region FUNCTIONS (JSON DATA NEEDED)

        /// <summary>
        /// Request to server with a desired url sending json. Returns a json/text data
        /// </summary>
        /// <param name="thisObj">The object where the coroutine will be executed</param>
        public static void HTTP_REQUEST(this MonoBehaviour thisObj, string requestName, string url, REQUEST_METHOD httpMethod, string json, Action<string> OnComplete, Action<HttpErrorResponse> OnError = null)
        {
            thisObj.StartCoroutine(I_HTTP_REQUEST(requestName, url, httpMethod, json, OnComplete, OnError));
        }

        /// <summary>
        /// Request to server with a desired url sending json. Returns byte[]
        /// </summary>       
        /// <param name="thisObj">The object where the coroutine will be executed</param>
        public static void HTTP_REQUEST(this MonoBehaviour thisObj, string requestName, string url, REQUEST_METHOD httpMethod, string json, Action<byte[]> OnComplete, Action<HttpErrorResponse> OnError = null)
        {
            thisObj.StartCoroutine(I_HTTP_REQUEST(requestName, url, httpMethod, json, OnComplete, OnError));
        }


        /// <summary>
        /// Request to server with a desired url sending json, needs to be authorized with a token. Returns a json/text data
        /// </summary>
        /// <param name="thisObj">The object where the coroutine will be executed</param>
        public static void HTTP_REQUEST_AUTH(this MonoBehaviour thisObj, string requestName, string url, REQUEST_METHOD httpMethod, string json, string token, Action<string> OnComplete, Action<HttpErrorResponse> OnError = null)
        {
            thisObj.StartCoroutine(I_HTTP_REQUEST_AUTH(requestName, url, httpMethod, json, token, OnComplete, OnError));
        }


        /// <summary>
        /// Request to download objects from server with a desired url sending json. Returns byte[]
        /// </summary>
        /// <param name="thisObj">The object where the coroutine will be executed</param>
        public static void HTTP_REQUEST_DOWNLOAD(this MonoBehaviour thisObj, string requestName, string url, REQUEST_METHOD httpMethod, string json, Action<byte[]> OnDownloadComplete, Action<HttpErrorResponse> OnError = null, Action<float> OnDownloading = null)
        {
            thisObj.StartCoroutine(I_HTTP_REQUEST_DOWNLOAD(requestName, url, httpMethod, json, OnDownloadComplete, OnError, OnDownloading));
        }

        #endregion

        #region FUNCTIONS (NO JSON DATA NEEDED)

        /// <summary>
        /// Get from server with desired url. Returns a json/text data
        /// </summary>
        /// <param name="thisObj">The object where the coroutine will be executed</param>
        public static void GET_REQUEST(this MonoBehaviour thisObj, string requestName, string url, Action<string> OnComplete, Action<HttpErrorResponse> OnError = null)
        {
            thisObj.StartCoroutine(I_GET_REQUEST(url, requestName, OnComplete, OnError));
        }

        /// <summary>
        /// Get from server with desired url. Returns byte[]
        /// </summary>
        /// <param name="thisObj">The object where the coroutine will be executed</param>
        public static void GET_REQUEST(this MonoBehaviour thisObj, string requestName, string url, Action<byte[]> OnComplete, Action<HttpErrorResponse> OnError = null)
        {
            thisObj.StartCoroutine(I_GET_REQUEST(url, requestName, OnComplete, OnError));
        }

        /// <summary>
        /// Get from server with a desired url, needs to be authorized with a token. Returns a json/text data
        /// </summary>
        /// <param name="thisObj">The object where the coroutine will be executed</param>
        public static void GET_REQUEST_AUTH(this MonoBehaviour thisObj, string requestName, string url, string Token, Action<string> OnComplete, Action<HttpErrorResponse> OnError = null)
        {
            thisObj.StartCoroutine(I_GET_REQUEST_AUTH(requestName, url, Token, OnComplete, OnError));
        }

        /// <summary>
        /// Get from server with a desired url, download bytes. Returns byte[]
        /// </summary>
        /// <param name="thisObj">The object where the coroutine will be executed</param>
        public static void GET_REQUEST_DOWNLOAD(this MonoBehaviour thisObj, string requestName, string url, Action<byte[]> OnDownloadComplete, Action<HttpErrorResponse> OnError = null, Action<float> OnDownloading = null)
        {
            thisObj.StartCoroutine(I_GET_REQUEST_DOWNLOAD(requestName, url, OnDownloadComplete, OnError, OnDownloading));
        }
        #endregion

        #region COROUTINES (BACKSTAGE)

        //----------- JSON DATA NEEDED
        private static IEnumerator I_HTTP_REQUEST(string requestName, string url, REQUEST_METHOD httpMethod, string json, Action<string> OnComplete, Action<HttpErrorResponse> OnError = null)
        {

            Debug.Log($"(Http Request) {requestName} \n URL : {url} | JSON : {json}");

            var jsonBinary = System.Text.Encoding.UTF8.GetBytes(json);

            DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();

            UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
            uploadHandlerRaw.contentType = "application/json";

            string method = "";

            switch (httpMethod)
            {
                case REQUEST_METHOD.POST:
                    method = "POST";
                    break;
                case REQUEST_METHOD.GET:
                    method = "GET";
                    break;
                case REQUEST_METHOD.PUT:
                    method = "PUT";
                    break;
                case REQUEST_METHOD.DELETE:
                    method = "DELETE";
                    break;
                default:
                    break;
            }

            using (UnityWebRequest request = new UnityWebRequest(url, method, downloadHandlerBuffer, uploadHandlerRaw))
            {
                yield return request.SendWebRequest();

                if (ErrorOnRequest(request))
                {
                    Debug.LogError($"(ERROR) in [{ requestName}] : {request.error}");

                    string responseError = request.downloadHandler.text;
                    if (!string.IsNullOrEmpty(responseError))
                        Debug.Log($"(Error Response) from [{requestName}] : {responseError}");

                    OnError?.Invoke(HttpErrorResponse.Create(request.responseCode, responseError));
                }
                else
                {

                    string response = request.downloadHandler.text;

                    Debug.Log($"(Response) from [{requestName}] : {response}");

                    OnComplete?.Invoke(response);

                }
            }
        }

        private static IEnumerator I_HTTP_REQUEST(string requestName, string url, REQUEST_METHOD httpMethod, string json, Action<byte[]> OnComplete, Action<HttpErrorResponse> OnError = null)
        {
            Debug.Log($"(Http Request) {requestName} \n URL : {url} | JSON : {json}");

            var jsonBinary = System.Text.Encoding.UTF8.GetBytes(json);

            DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();

            UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
            uploadHandlerRaw.contentType = "application/json";

            string method = "";

            switch (httpMethod)
            {
                case REQUEST_METHOD.POST:
                    method = "POST";
                    break;
                case REQUEST_METHOD.GET:
                    method = "GET";
                    break;
                case REQUEST_METHOD.PUT:
                    method = "PUT";
                    break;
                case REQUEST_METHOD.DELETE:
                    method = "DELETE";
                    break;
                default:
                    break;
            }

            using (UnityWebRequest request = new UnityWebRequest(url, method, downloadHandlerBuffer, uploadHandlerRaw))
            {
                yield return request.SendWebRequest();

                if (ErrorOnRequest(request))
                {
                    Debug.LogError($"(ERROR) in [{ requestName}] : {request.error}");

                    string responseError = request.downloadHandler.text;
                    if (!string.IsNullOrEmpty(responseError))
                        Debug.Log($"(Error Response) from [{requestName}] : {responseError}");

                    OnError?.Invoke(HttpErrorResponse.Create(request.responseCode, responseError));
                }
                else
                {
                    byte[] response = request.downloadHandler.data;

                    Debug.Log($"(Received Data) from [{requestName}]");

                    OnComplete?.Invoke(response);

                }
            }
        }

        private static IEnumerator I_HTTP_REQUEST_AUTH(string requestName, string url, REQUEST_METHOD httpMethod, string json, string token, Action<string> OnComplete, Action<HttpErrorResponse> OnError = null)
        {
            Debug.Log($"(Http Request) {requestName} \n URL : {url} | JSON : {json}");


            var jsonBinary = System.Text.Encoding.UTF8.GetBytes(json);

            DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();

            UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
            uploadHandlerRaw.contentType = "application/json";

            string method = "";

            switch (httpMethod)
            {
                case REQUEST_METHOD.POST:
                    method = "POST";
                    break;
                case REQUEST_METHOD.GET:
                    method = "GET";
                    break;
                case REQUEST_METHOD.PUT:
                    method = "PUT";
                    break;
                case REQUEST_METHOD.DELETE:
                    method = "DELETE";
                    break;
                default:
                    break;
            }

            using (UnityWebRequest request = new UnityWebRequest(url, method, downloadHandlerBuffer, uploadHandlerRaw))
            {
                request.SetRequestHeader("Authorization", "Token " + token);

                yield return request.SendWebRequest();

                if (ErrorOnRequest(request))
                {
                    Debug.LogError($"(ERROR) in [{ requestName}] : {request.error}");

                    string responseError = request.downloadHandler.text;
                    if (!string.IsNullOrEmpty(responseError))
                        Debug.Log($"(Error Response) from [{requestName}] : {responseError}");

                    OnError?.Invoke(HttpErrorResponse.Create(request.responseCode, responseError));
                }
                else
                {
                    string response = request.downloadHandler.text;

                    Debug.Log($"(Response) from [{requestName}] : {response}");

                    OnComplete?.Invoke(response);

                }
            }
        }

        private static IEnumerator I_HTTP_REQUEST_DOWNLOAD(string requestName, string url, REQUEST_METHOD httpMethod, string json, Action<byte[]> OnDownloadComplete, Action<HttpErrorResponse> OnError = null, Action<float> OnDownloading = null)
        {
            Debug.Log($"(Http Request) {requestName} \n URL : {url} | JSON : {json}");

            var jsonBinary = System.Text.Encoding.UTF8.GetBytes(json);

            DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();

            UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
            uploadHandlerRaw.contentType = "application/json";

            string method = "";

            switch (httpMethod)
            {
                case REQUEST_METHOD.POST:
                    method = "POST";
                    break;
                case REQUEST_METHOD.GET:
                    method = "GET";
                    break;
                case REQUEST_METHOD.PUT:
                    method = "PUT";
                    break;
                case REQUEST_METHOD.DELETE:
                    method = "DELETE";
                    break;
                default:
                    break;
            }

            using (UnityWebRequest request = new UnityWebRequest(url, method, downloadHandlerBuffer, uploadHandlerRaw))
            {
                request.downloadHandler = new DownloadHandlerBuffer();

                request.SendWebRequest();

                while (!request.isDone && !ErrorOnRequest(request))
                {
                    Debug.Log($"(Download Progress) in [{requestName}] : [{request.downloadProgress}]");
                    OnDownloading?.Invoke(request.downloadProgress);

                    yield return null;
                }

                if (ErrorOnRequest(request))
                {
                    Debug.LogError($"(ERROR) in [{ requestName}] : {request.error}");
                    string responseError = request.downloadHandler.text;
                    if (!string.IsNullOrEmpty(responseError))
                        Debug.Log($"(Error Response) from [{requestName}] : {responseError}");

                    OnError?.Invoke(HttpErrorResponse.Create(request.responseCode, responseError));
                    yield break;
                }
                else
                {

                    Debug.Log($"(Download Complete) from [{requestName }]");

                    OnDownloadComplete?.Invoke(request.downloadHandler.data);
                }
            }
        }

        //----------- NO JSON DATA NEEDED

        private static IEnumerator I_GET_REQUEST(string url, string requestName, Action<string> OnComplete, Action<HttpErrorResponse> OnError = null)
        {
            Debug.Log($"(Http Request) {requestName} \n URL : {url}");

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {

                yield return request.SendWebRequest();

                if (ErrorOnRequest(request))
                {
                    Debug.LogError($"(ERROR) in [{ requestName}] : {request.error}");
                    string responseError = request.downloadHandler.text;

                    if (!string.IsNullOrEmpty(responseError))
                        Debug.Log($"(Error Response) from [{requestName}] : {responseError}");

                    OnError?.Invoke(HttpErrorResponse.Create(request.responseCode, responseError));
                }
                else
                {
                    string response = request.downloadHandler.text;

                    Debug.Log($"(Response) from [{requestName}] : {response}");

                    OnComplete?.Invoke(response);

                }
            }
        }

        private static IEnumerator I_GET_REQUEST(string url, string requestName, Action<byte[]> OnComplete, Action<HttpErrorResponse> OnError = null)
        {
            Debug.Log($"(Http Request) {requestName} \n URL : {url}");

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {

                yield return request.SendWebRequest();

                if (ErrorOnRequest(request))
                {
                    Debug.LogError($"(ERROR) in [{ requestName}] : {request.error}");
                    string responseError = request.downloadHandler.text;

                    if (!string.IsNullOrEmpty(responseError))
                        Debug.Log($"(Error Response) from [{requestName}] : {responseError}");

                    OnError?.Invoke(HttpErrorResponse.Create(request.responseCode, responseError));
                }
                else
                {
                    byte[] response = request.downloadHandler.data;

                    Debug.Log($"(Received Data) from [{requestName}]");

                    OnComplete?.Invoke(response);

                }
            }
        }


        private static IEnumerator I_GET_REQUEST_AUTH(string requestName, string url, string Token, Action<string> OnComplete, Action<HttpErrorResponse> OnError = null)
        {
            Debug.Log($"(Http Request) {requestName} \n URL : {url}");

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Authorization", "Token " + Token);

                yield return request.SendWebRequest();

                if (ErrorOnRequest(request))
                {

                    Debug.LogError($"(ERROR) in [{ requestName}] : {request.error}");

                    string responseError = request.downloadHandler.text;
                    if (!string.IsNullOrEmpty(responseError))
                        Debug.Log($"(Error Response) from [{requestName}] : {responseError}");

                    OnError?.Invoke(HttpErrorResponse.Create(request.responseCode, responseError));
                }
                else
                {
                    string response = request.downloadHandler.text;

                    Debug.Log($"(Response) from [{requestName}] : {response}");

                    OnComplete?.Invoke(response);

                }
            }
        }

        private static IEnumerator I_GET_REQUEST_DOWNLOAD(string requestName, string url, Action<byte[]> OnDownloadComplete, Action<HttpErrorResponse> OnError = null, Action<float> OnDownloading = null)
        {
            Debug.Log($"(Http Request) {requestName} \n URL : {url}");

            using (UnityWebRequest request = new UnityWebRequest(url))
            {

                request.downloadHandler = new DownloadHandlerBuffer();

                request.SendWebRequest();

                while (!request.isDone && !ErrorOnRequest(request))
                {
                    OnDownloading?.Invoke(request.downloadProgress);
                    Debug.Log($"(Download Progress) in [{requestName}] : [{request.downloadProgress}]");

                    yield return null;
                }


                if (ErrorOnRequest(request))
                {

                    Debug.LogError($"(ERROR) in [{ requestName}] : {request.error}");

                    string responseError = request.downloadHandler.text;
                    if (!string.IsNullOrEmpty(responseError))
                        Debug.Log($"(Error Response) from [{requestName}] : {responseError}");

                    OnError?.Invoke(HttpErrorResponse.Create(request.responseCode, responseError));
                    yield break;
                }
                else
                {
                    Debug.Log($"(Download Complete) from [{requestName }]");

                    OnDownloadComplete?.Invoke(request.downloadHandler.data);
                }
            }
        }

        #endregion

        /// <summary>
        /// Check the error result on a request
        /// </summary>
        /// <returns>True if the request was not successful</returns>
        private static bool ErrorOnRequest(UnityWebRequest request)
        {
            /* If the unity version is older 
             * and throws error with the return statement, 
             * try uncommenting the following line */

            //return (request.isNetworkError || request.isHttpError);

            return (request.result != UnityWebRequest.Result.Success && request.result != UnityWebRequest.Result.InProgress);
        }

    }

    /*
     * The general codes generated by the server response
     * Can be used for comparisons
     */
    public struct HttpGeneralResponseCodes
    {
        public const long OK = 200;
        public const long INTERNAL_SERVER_ERROR = 500;
        public const long BAD_GATEWAY = 502;
        public const long SERVER_UNAVAILABLE = 503;
        public const long GATEWAY_TIMEOUT = 504;
    }

    /*
     * This struct represents the information of an error server response
     */
    public struct HttpErrorResponse
    {
        /// <summary>
        /// The code generated by a server response
        /// </summary>
        public readonly long code;

        /// <summary>
        /// The json or text data generated by a server response
        /// </summary>
        public readonly string text;

        public HttpErrorResponse(long code, string text)
        {
            this.code = code;
            this.text = text;
        }

        public static HttpErrorResponse Create(long code, string text)
        {
            return new HttpErrorResponse(code, text);
        }
    }
}

