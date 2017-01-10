using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;

public class ElasticEmail : MonoBehaviour {

    private const string ApiKey = "cdcae26d-1fb8-4565-9b8f-1495d1023c26";
    private const string Address = "https://api.elasticemail.com/v2/email/send";

    public static void Send(string subject, string bodyText)
    {
        var values = new NameValueCollection
            {
                {"apikey", ApiKey},
                {"from", "inspec2t-feedback@playgen.com"},
                {"fromName", "Resource Force Feedback"},
                {"to", "inspec2t-feedback@playgen.com"},
                {"subject", subject},
                {"bodyText", bodyText},
                {"isTransactional", "true"}
            };

        // Fix for authentication decryption failure: found here http://stackoverflow.com/questions/4926676/mono-https-webrequest-fails-with-the-authentication-or-decryption-has-failed
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

        var response = SendEmail(Address, values);

        Debug.Log(response);
    }

    private static string SendEmail(string address, NameValueCollection values)
    {
        using (var client = new WebClient())
        {
            try
            {
                var apiResponse = client.UploadValues(address, values);
                return Encoding.UTF8.GetString(apiResponse);

            }
            catch (Exception ex)
            {
                return "Exception caught: " + ex.Message + "\n" + ex.StackTrace;
            }
        }
    }

    private static bool MyRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        var isOk = true;
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            foreach (X509ChainStatus t in chain.ChainStatus)
            {
                if (t.Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    var chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                    }
                }
            }
        }
        return isOk;
    }
}
