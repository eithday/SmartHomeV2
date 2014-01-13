using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SmartHome.Common.Utilities
{
  public class XmlParser
  {
    public partial class parsedElement
    {
      public parsedElement(string element, string value)
      {
        this.currentElement = element;
        this.currentValue = value;
      }

      public string CurrentElement
      {
        get { return currentElement; }
        set
        {
          currentElement = value;
        }
      } string currentElement;

      public string CurrentValue
      {
        get { return currentValue; }
        set
        {
          currentValue = value;
        }
      }  string currentValue;
    }

    public partial class parsedAttribute
    {
      public parsedAttribute(string el, string elVal, string att, string attValue)
      {
        this.currentAttribute = att;
        this.currentAttValue = attValue;
        this.currentElement = el;
        this.currentValue = elVal;
      }

      public string CurrentAttribute
      {
        get { return currentAttribute; }
        set
        {
          currentAttribute = value;
        }
      } string currentAttribute;

      public string CurrentAttValue
      {
        get { return currentAttValue; }
        set
        {
          currentAttValue = value;
        }
      }  string currentAttValue;

      public string CurrentElement
      {
        get { return currentElement; }
        set
        {
          currentElement = value;
        }
      } string currentElement;

      public string CurrentValue
      {
        get { return currentValue; }
        set
        {
          currentValue = value;
        }
      }  string currentValue;
    }

    string currentElement;
    string currentValue;
    string currentAttribute;
    string currentAttributeValue;
    parsedElement pE;
    parsedAttribute pA;

    public XmlParser()
    {
      clearCurrent();
      registerNewParsedElements();
    }

    private void clearCurrent()
    {
      currentElement = "";
      currentValue = "";
      currentAttribute = "";
      currentAttributeValue = "";
    }

    private void registerNewParsedElements()
    {
      pE = new parsedElement(currentElement, currentValue);
      pA = new parsedAttribute(currentElement, currentValue, currentAttribute, currentAttributeValue);
    }

    public void parseXML(string filePath)
    {
      XmlReader xmlReader = XmlReader.Create(filePath);
      while (xmlReader.Read())
      {
        //Get the nodeType and set it's value for reference     
        //Only want to Parse information on Text, CDATA, or Attribute so values are set properly
        switch (xmlReader.NodeType)
        {
          case XmlNodeType.Element:
            currentElement = xmlReader.Name;
            break;
          case XmlNodeType.Text:
            currentValue = xmlReader.Value;
            pE.CurrentElement = currentElement;
            pE.CurrentValue = currentValue;
            Messenger.Default.Send<parsedElement>(pE);
            break;
          case XmlNodeType.CDATA:
            currentValue = xmlReader.Value;
            break;
          case XmlNodeType.EndElement:
            //Clear current element to reset for next element
            currentElement = "";
            break;
        }

        //Get any Attribute for the current element for reference
        if (xmlReader.HasAttributes)
        {
          //clear attribute information from any previous element
          //  clearAttributeInformation();
          //set currentAttribute for reference
          //currentAttribute = xmlReader.Name;

          //Cycle through and parse each Attribute and store for reference                       
          while (xmlReader.MoveToNextAttribute())
          {
            currentAttribute = xmlReader.Name;
            currentAttributeValue = xmlReader.Value;
            pA.CurrentElement = currentElement;
            pA.CurrentValue = currentValue;
            pA.CurrentAttribute = currentAttribute;
            pA.CurrentAttValue = currentAttributeValue;
            Messenger.Default.Send<parsedAttribute>(pA);
          }
          // Move the reader back to the element node.
          xmlReader.MoveToElement();
        }
      }
    }
  }
}
