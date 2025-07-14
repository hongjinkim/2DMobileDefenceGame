using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebResponseTimeHandler : BaseUTCRequestHandler
{
    [SerializeField]
    private string https = "www.";

    [SerializeField]
    private int TimeOutSeconds = 12;


    private bool onPending = false;
        
    protected override void OnRequest()
    {
        if (onPending == false)
        {
            onPending = true;
            StartCoroutine(RequestWebResponse());
        }
    }

    private IEnumerator RequestWebResponse()
    {
        if (printLog) Debug.Log($"<{GetType()}> {https}�� �ð����� ��û�߽��ϴ�.");

        yield return new WaitForSecondsRealtime(0.1f);

        UnityWebRequest request = UnityWebRequest.Get($"https://{https}");
        request.timeout = TimeOutSeconds;

        yield return request.SendWebRequest();
        onPending = false;

        if (request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                var text = request.GetResponseHeader("date");
                var nowUtc = DateTimeOffset.Parse(text);

                OnSuccess?.Invoke(nowUtc);
                ClearCallbacks();
            }
            catch 
            {
                if (printLog) Debug.Log($"[!] <{GetType()}> {https}���� ������ �ð����� ��ȯ���� ���߽��ϴ�.");
                OnFailure?.Invoke();
                ClearCallbacks();
            }
        }
        else
        {
            if (printLog) Debug.Log($"[!] <{GetType()}> {https}���� �ð��� �������� ���߽��ϴ�.");
            OnFailure?.Invoke();
            ClearCallbacks();
        }
    }

}

