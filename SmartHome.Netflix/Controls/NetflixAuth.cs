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
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Xml.Serialization;

namespace SmartHome.Netflix.Controls
{
  public class NetflixAuth
  {
    // This holds the authorization data that gives us access to
    // user's account. Once obtained, it is persisted for future
    // invocations of the application. Using a Hashtable here instead
    // of a StringDictionary so that we can use generic XML serialization.
    private Hashtable _accessInfo = new Hashtable();

    // indicates whether or not we have authorization to access the user's account
    private bool _authorized = false;

    // file where authoriztion info is persisted
    private string _archive;

    /// <summary>
    /// Creates an instance of a Netflix authorization class.
    /// </summary>
    /// <param name="archiveFna">Archive filename. Can be identity-specific.</param>
    public NetflixAuth(string archiveFna)
    {
      _archive = archiveFna;
      _authorized = LoadAccessInfo();
    }

    /// <summary>
    /// Indicates whether or not the user has granted account access to the application.
    /// </summary>
    public bool Authorized
    {
      get { return _authorized; }
    }

    /// <summary>
    /// Returns the user identity, null if not yet authorized
    /// </summary>
    public string UserID
    {
      get { return (string)_accessInfo["user_id"]; }
    }

    /// <summary>
    /// Returns the OAuth token, null if not yet authorized
    /// </summary>
    public string Token
    {
      get { return (string)_accessInfo["oauth_token"]; }
    }

    /// <summary>
    /// Returns the OAuth token secret, null if not yet authorized
    /// </summary>
    public string Secret
    {
      get { return (string)_accessInfo["oauth_token_secret"]; }
    }

    /// <summary>
    /// Saves the access authorization bits for later use.
    /// </summary>
    /// <param name="authReply">Authorization request response.</param>
    /// <returns>True if the operation succeeded.</returns>
    public bool SaveAuthorization(string authReply)
    {
      ParseResponse(authReply, _accessInfo);
      _authorized = SaveAccessInfo();
      return _authorized;
    }

    /// <summary>
    /// general purpose response parser
    /// </summary>
    /// <param name="buf">Respose string.</param>
    /// <param name="ht">Hashtable to receive the results.</param>
    public void ParseResponse(string buf, Hashtable ht)
    {
      string[] elements = buf.Split('&');
      ht.Clear();
      foreach (string elem in elements)
      {
        string[] item = elem.Split('=');
        ht.Add(item[0], item[1]);
      }
    }

    // this is just a quick & dirty data serialization
    // todo: come up with a more secure/reliable mechanism,
    // and also support per-user settings

    // called by initialization
    private bool LoadAccessInfo()
    {
      if (!File.Exists(_archive))
        return false;
      StreamReader sr = new StreamReader(_archive);
      MySerializer.Deserialize((TextReader)sr, _accessInfo);
      return true;
    }

    // called when access authorization is received
    private bool SaveAccessInfo()
    {
      StreamWriter sw = new StreamWriter(_archive);
      MySerializer.Serialize((TextWriter)sw, _accessInfo);
      return true;
    }
  }

  /// <summary>
  /// General purpose IDictionary object serializer
  /// </summary>
  public class MySerializer
  {
    public class Entry
    {
      public object Key;
      public object Value;
      public Entry() { }
      public Entry(object key, object value)
      {
        Key = key;
        Value = value;
      }
    }

    public static void Serialize(TextWriter writer, IDictionary dictionary)
    {
      List<Entry> entries = new List<Entry>(dictionary.Count);
      foreach (object key in dictionary.Keys)
      {
        entries.Add(new Entry(key, dictionary[key]));
      }
      XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
      serializer.Serialize(writer, entries);
    }

    public static void Deserialize(TextReader reader, IDictionary dictionary)
    {
      dictionary.Clear();
      XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
      List<Entry> list = (List<Entry>)serializer.Deserialize(reader);
      foreach (Entry entry in list)
      {
        dictionary[entry.Key] = entry.Value;
      }
    }
  }
}