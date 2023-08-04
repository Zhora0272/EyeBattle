using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Voodoo.Network
{
    public static class WebRequest 
    {
        private static Dictionary<int, IWebRequestHandler> idToResult = new Dictionary<int, IWebRequestHandler>();

        public static (string, string) requestHeader;

        public static void Get(string url, Action<UnityWebRequest> onSuccess = null, Action<UnityWebRequest> onError = null)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            SendAndCache(request, onSuccess, onError);
        }
        
        public static async Task<UnityWebRequest> GetAsync(string url)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            await SendAndCacheAsync(request);
            return request;
        }

        public static void Put(string url, string content, Action<UnityWebRequest> onSuccess = null, Action<UnityWebRequest> onError = null)
        {
            UnityWebRequest request = UnityWebRequest.Put(url, content);
            request.uploadHandler.contentType = "application/json";
            
            SendAndCache(request, onSuccess, onError);
        }

        public static async Task<UnityWebRequest> PutAsync(string url, string content)
        {
            UnityWebRequest request = UnityWebRequest.Put(url, content);
            request.uploadHandler.contentType = "application/json";

            await SendAndCacheAsync(request);
            return request;
        }

        public static void Post(string url, string content, Action<UnityWebRequest> onSuccess = null, Action<UnityWebRequest> onError = null)
        {
            UnityWebRequest request = new UnityWebRequest(url, "POST", new DownloadHandlerBuffer(), new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(content)))
            {
                uploadHandler = {contentType = "application/json"}
            };

            SendAndCache(request, onSuccess, onError);
        }
        
        public static async Task<UnityWebRequest> PostAsync(string url, string content)
        {
            UnityWebRequest request = new UnityWebRequest(url, "POST", new DownloadHandlerBuffer(), new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(content)))
            {
                uploadHandler = {contentType = "application/json"}
            };

            await SendAndCacheAsync(request);
            return request;
        }

        private static void SendAndCache(UnityWebRequest request, Action<UnityWebRequest> onSuccess = null, Action<UnityWebRequest> onError = null) 
        {
            ApplyHeader(request);

            UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();

            if (asyncOperation == null)
            {
                return;
            }

            idToResult.Add(asyncOperation.GetHashCode(), new WebRequestHandler(onSuccess, onError));
            asyncOperation.completed += OnAsyncOperationComplete;
        }

        private static async Task SendAndCacheAsync(UnityWebRequest request) 
        {
            ApplyHeader(request);

            UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();
            
            if (asyncOperation == null)
            {
                return;
            }
            
            await asyncOperation;
        }

        private static void ApplyHeader(UnityWebRequest request)
        {
            if (requestHeader.Item2 == null || requestHeader.Item2.Length <= 0)
            {
                return;
            }

            for (int i = 0; i < requestHeader.Item2.Length; i++)
            {
                char character = requestHeader.Item2[i];
                if (char.IsLetterOrDigit(character) == false && character != ' ')
                {
                    return;
                }
            }

            request.SetRequestHeader(requestHeader.Item1, requestHeader.Item2);
        }

        private static void OnAsyncOperationComplete(AsyncOperation operation)
        {
            UnityWebRequestAsyncOperation webOperation = operation as UnityWebRequestAsyncOperation;
            if (webOperation == null)
            {
                return;
            }

            IWebRequestHandler handler = idToResult.ContainsKey(webOperation.GetHashCode()) ? idToResult[webOperation.GetHashCode()] : null;
            if (handler == null)
            {
                return;
            }
            
            if (string.IsNullOrEmpty(webOperation.webRequest.error))
            {
                handler.OnSuccess(webOperation.webRequest);
            }
            else
            {
                handler.OnError(webOperation.webRequest);
            }

            webOperation.webRequest.Dispose();
            idToResult.Remove(operation.GetHashCode());
        }
        
        public static bool HadErrors(Task<UnityWebRequest> request) => string.IsNullOrEmpty(request.Result.error) == false;
        public static bool HadErrors(UnityWebRequest request) => string.IsNullOrEmpty(request.error) == false;
    }
}