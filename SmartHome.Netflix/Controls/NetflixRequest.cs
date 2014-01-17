/*
 * Netflix API Basics
 * Copyright (c) 2009 Dave Cook Consulting, LLC
 *                    http://www.netdave.com
 * 
 * This code released under the Code Project Open License
 * http://www.codeproject.com/info/cpol10.aspx
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Data.SQLite;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHome.Netflix.Controls
{
  class NetflixRequest : OAuthBase
  {
    private string xmlBuffer;
    // for all requests
    private string _consumerKey;
    private string _consumerSecret;

    // for protected requests
    private string _access_token;
    private string _access_secret;

    #region Constructors
    /// <summary>
    /// Constructor for Non-authenticated and Signed requests
    /// </summary>
    /// <param name="ConsumerKey">Your Netflix developer key.</param>
    /// <param name="ConsumerSecret">Your Netflix developer secret.</param>
    public NetflixRequest(string ConsumerKey, string ConsumerSecret)
    {
      _consumerKey = ConsumerKey;
      _consumerSecret = ConsumerSecret;
    }

    /// <summary>
    /// Constructor for Protected requests
    /// </summary>
    /// <param name="ConsumerKey">Your Netflix developer key.</param>
    /// <param name="ConsumerSecret">Your Netflix developer secret.</param>
    /// <param name="UserToken">Access token for subscriber's account.</param>
    /// <param name="UserSecret">Access token secret for subscriber's account.</param>
    public NetflixRequest(string ConsumerKey, string ConsumerSecret,
                          string AccessToken, string AccessSecret)
      : this(ConsumerKey, ConsumerSecret)
    {
      _access_token = AccessToken;
      _access_secret = AccessSecret;
    }
    #endregion Constructors

    #region Non-Authenticated Requests
    /// <summary>
    /// Non-Authenticated Request
    /// </summary>
    /// <param name="requestUrl">Resource to query.</param>
    /// <returns>Request response XML in a string.</returns>
    public string NonAuthRequestString(string requestUrl)
    {
      return ServiceRequestString(ConstructNonAuthRequest(requestUrl));
    }

    /// <summary>
    /// Non-Authenticated Request
    /// </summary>
    /// <param name="requestUrl">Resource to query.</param>
    /// <returns>Request response in an XmlDocument object.</returns>
    public XmlDocument NonAuthRequestXml(string requestUrl)
    {
      return ServiceRequestXml(ConstructNonAuthRequest(requestUrl));
    }
    #endregion Non-Authenticated Requests

    #region Signed Requests
    /// <summary>
    /// Signed Request
    /// </summary>
    /// <param name="requestUrl">Resource to query.</param>
    /// <returns>XmlDocument object containing the results of the request.</returns>
    public XmlDocument SignedRequest(string requestUrl)
    {
      // outputs
      string normalizedUrl;
      string normalizedRequestParameters;

      // generate request signature
      string sig = GenerateSignature(new Uri(requestUrl),
                                     _consumerKey, _consumerSecret,
                                     null, null,	// token , tokenSecret (not used)
                                     "GET", GenerateTimeStamp(), GenerateNonce(),
                                     out normalizedUrl, out normalizedRequestParameters);
      // put it all together to construct the signed request
      string reqUrl = normalizedUrl +
                      "?" + normalizedRequestParameters +
                      "&oauth_signature=" + UrlEncode(sig);
      return ServiceRequest(reqUrl);
    }

    // internal method for common web requests
    // note that the caller handles all exceptions
    private XmlDocument ServiceRequest(string requestUrl)
    {
      WebRequest req = WebRequest.Create(requestUrl);
      WebResponse rsp = req.GetResponse();
      XmlDocument xDoc = new XmlDocument();
      xDoc.Load(rsp.GetResponseStream());
      rsp.Close();
      return xDoc;
    }

    /// <summary>
    /// Signed Request
    /// </summary>
    /// <param name="requestUrl">Resource to query.</param>
    /// <returns>Request response XML in a string.</returns>
    public string SignedRequestString(string requestUrl)
    {
      return ServiceRequestString(ConstructSignedRequest(requestUrl));
    }

    /// <summary>
    /// Signed Request
    /// </summary>
    /// <param name="requestUrl">Resource to query.</param>
    /// <returns>Request response in an XmlDocument object.</returns>
    public XmlDocument SignedRequestXml(string requestUrl)
    {
      return ServiceRequestXml(ConstructSignedRequest(requestUrl));
    }
    #endregion Signed Requests

    #region Protected Requests
    /// <summary>
    /// Protected Request
    /// </summary>
    /// <param name="requestUrl">Resource to query.</param>
    /// <returns>Request response XML in a string.</returns>
    public string ProtectedRequest(string requestUrl)
    {
      return ServiceRequestString(ConstructProtectedRequest(requestUrl, "GET"));
    }

    /// <summary>
    /// Protected Request
    /// </summary>
    /// <param name="requestUrl">Resource to query.</param>
    /// <returns>Request response in an XmlDocument object.</returns>
    public XmlDocument ProtectedRequestXml(string requestUrl)
    {
      return ServiceRequestXml(ConstructProtectedRequest(requestUrl, "GET"));
    }

    /// <summary>
    /// Protected Request
    /// </summary>
    /// <param name="requestUrl">Resource to access.</param>
    /// <returns>Request response XML in a string.</returns>
    public string ProtectedRequest(string requestUrl, string Method)
    {
      return ServiceRequestString(ConstructProtectedRequest(requestUrl, Method), Method);
    }

    /// <summary>
    /// Protected Request
    /// </summary>
    /// <param name="requestUrl">Resource to access.</param>
    /// <returns>Request response in an XmlDocument object.</returns>
    public Task<XmlDocument> ProtectedRequestXml(string requestUrl, string Method)
    {
      return Task.Run<XmlDocument>(() => ServiceRequestXml(ConstructProtectedRequest(requestUrl, Method), Method));
    }

    public void CatalogRequest(string requestUrl, string Method)
    {
      catalogRequest(ConstructProtectedRequest(requestUrl, Method));
    }
    #endregion Protected Requests

    #region General Purpose Internal Functions
    /// <summary>
    /// Constructs a Non-Authorized service request
    /// </summary>
    /// <param name="requestUrl">Resource to query.</param>
    /// <returns>Constructed request string.</returns>
    private string ConstructNonAuthRequest(string requestUrl)
    {
      // Rather than munge the original base class code, I've yoinked the bits I need to here
      List<QueryParameter> parameters = new List<QueryParameter>();
      parameters.Add(new QueryParameter(OAuthConsumerKeyKey, _consumerKey));
      if (_additionalParameters.Count > 0)
      {
        foreach (DictionaryEntry de in _additionalParameters)
        {
          parameters.Add(new QueryParameter(de.Key.ToString(), de.Value.ToString()));
        }
      }
      parameters.Sort(new QueryParameterComparer());

      // construct the REST request
      return requestUrl + "?" + NormalizeRequestParameters(parameters);
    }

    /// <summary>
    /// Constructs a Signed service request
    /// </summary>
    /// <param name="requestUrl">Resource to query.</param>
    /// <returns>Constructed request string.</returns>
    private string ConstructSignedRequest(string requestUrl)
    {
      // outputs
      string normalizedUrl;
      string normalizedRequestParameters;

      // generate request signature
      string sig = GenerateSignature(new Uri(requestUrl),
                                     _consumerKey, _consumerSecret,
                                     null, null,	// token , tokenSecret (not used)
                                     "GET", GenerateTimeStamp(), GenerateNonce(),
                                     out normalizedUrl, out normalizedRequestParameters);
      // put it all together to construct the signed request
      return normalizedUrl +
             "?" + normalizedRequestParameters +
             "&oauth_signature=" + UrlEncode(sig);
    }

    /// <summary>
    /// Constructs a Protected service request
    /// </summary>
    /// <param name="requestUrl">Resource to access.</param>
    /// <returns>Constructed request string.</returns>
    private string ConstructProtectedRequest(string requestUrl, string Method)
    {
      if (_access_token == null || _access_token.Length == 0 ||
          _access_secret == null || _access_secret.Length == 0)
      {
        //Exception ex = new Exception("Protected Request failed: Account access tokens required.");
        //throw ex;
      }

      // outputs
      string normalizedUrl;
      string normalizedRequestParameters;

      // generate request signature
      string sig = GenerateSignature(new Uri(requestUrl),
                                     _consumerKey, _consumerSecret,
                                     _access_token, _access_secret,
                                     Method, GenerateTimeStamp(), GenerateNonce(),
                                     out normalizedUrl, out normalizedRequestParameters);
      // put it all together to construct the signed request
      return normalizedUrl +
             "?" + normalizedRequestParameters +
             "&oauth_signature=" + UrlEncode(sig);
    }

    /// <summary>
    /// Common Web service request method.
    /// Note: It is up to the caller to handle exceptions.
    /// </summary>
    /// <param name="requestUrl">Resource to query.</param>
    /// <returns>Request response XML in a string.</returns>
    private string ServiceRequestString(string requestUrl)
    {
      return ServiceRequestString(requestUrl, "GET");
    }

    /// <summary>
    /// Common Web service request method.
    /// Note: It is up to the caller to handle exceptions.
    /// </summary>
    /// <param name="requestUrl">Resource to access.</param>
    /// <param name="Method">Access method.</param>
    /// <returns>Request response XML in a string.</returns>
    private string ServiceRequestString(string requestUrl, string Method)
    {
      WebRequest req = WebRequest.Create(requestUrl);
      req.Method = Method;
      if (Method == "POST")
      {
        // WebRequest requres this for POST operations
        req.ContentLength = 0;
      }
      WebResponse rsp = req.GetResponse();
      StreamReader sr = new StreamReader(rsp.GetResponseStream());
      return sr.ReadToEnd();
    }

    /// <summary>
    /// Common Web service request method.
    /// Note: It is up to the caller to handle exceptions.
    /// </summary>
    /// <param name="requestUrl">Resource to query.</param>
    /// <returns>Request response in an XmlDocument object.</returns>
    private XmlDocument ServiceRequestXml(string requestUrl)
    {
      return ServiceRequestXml(requestUrl, "GET");
    }

    /// <summary>
    /// Common Web service request method.
    /// Note: It is up to the caller to handle exceptions.
    /// </summary>
    /// <param name="requestUrl">Resource to access.</param>
    /// <param name="Method">Access method.</param>
    /// <returns>Request response in an XmlDocument object.</returns>
    private XmlDocument ServiceRequestXml(string requestUrl, string Method)
    {
      XmlDocument xDoc = new XmlDocument();
      var req = (HttpWebRequest)WebRequest.Create(requestUrl);
      WebResponse rsp = req.GetResponse();
      req.Method = Method;
      if (Method == "POST")
      {
        // WebRequest requres this for POST operations
        req.ContentLength = 0;
      }
      xDoc.Load(rsp.GetResponseStream());
      return xDoc;
    }

    private void catalogRequest(string requestUrl)
    {
      var req = (HttpWebRequest)WebRequest.Create(requestUrl);
      WebResponse rsp = req.GetResponse();

      if (rsp.ResponseUri.AbsoluteUri.Contains("http://cdn-api.netflix.com/api/v3/current/Catalog2"))
      {
        DateTime startTime = DateTime.UtcNow;
        WebRequest request = WebRequest.Create(rsp.ResponseUri.AbsoluteUri);
        request.Timeout = 20000000;
        //request.Headers.Add("Accept-Encoding", "gzip");
        WebResponse response = request.GetResponse();
        using (Stream responseStream = response.GetResponseStream())
        {
          using (Stream fileStream = File.OpenWrite("c:\\temp\\netflixStreamingCatalog.xml"))
          {
            byte[] buffer = new byte[10240];
            int bytesRead = responseStream.Read(buffer, 0, 10240);
            while (bytesRead > 0)
            {
              fileStream.Write(buffer, 0, bytesRead);
              DateTime nowTime = DateTime.UtcNow;
              if ((nowTime - startTime).TotalMinutes > 120)
              {
                throw new ApplicationException(
                    "Download timed out");
              }
              bytesRead = responseStream.Read(buffer, 0, 10240);
            }
          }
        }
      }
    }

    #endregion General Purpose Internal Functions
  }
}
