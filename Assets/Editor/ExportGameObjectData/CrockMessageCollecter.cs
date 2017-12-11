using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using Mogo.Game;

public enum XmlCrockField
{
    id_i,
    type_i,
    mapx_i,
    mapy_i,
    rotation_l
}

public class CrockMessageCollecter : MonoBehaviour
{
    private const string EXPORT_FILE_PATH = "Assets/Resources/xml";
    private const string EXPORT_FILE_NAME = "Crock.xml";

    [MenuItem("Mogo/Export Crock")]
    public static void ExportXmlFile()
    {
        if (!Directory.Exists(EXPORT_FILE_PATH))
            Directory.CreateDirectory(EXPORT_FILE_PATH);

        GameObject[] gos = (GameObject[])(UnityEngine.Object.FindObjectsOfType(typeof(GameObject)));
        if (gos != null)
        {
            XmlDocument xmldoc = new XmlDocument();
            XmlNode root = xmldoc.CreateNode(XmlNodeType.Element, "root", "");

            foreach (GameObject go in gos)
            {
                if (go.GetComponent<CrockExportAgent>() != null)
                {
                    ExportGameObjectMessage(xmldoc, root, go);
                }
            }

            xmldoc.AppendChild(root);
            xmldoc.Save(Path.Combine(EXPORT_FILE_PATH, EXPORT_FILE_NAME));

            AssetDatabase.Refresh();
        }
    }

    private static void ExportGameObjectMessage(XmlDocument xmldoc, XmlNode root, GameObject go)
    {
        XmlNode gameObjectNode = xmldoc.CreateElement("node");

        CrockExportAgent tempCrockAgent = go.GetComponent<CrockExportAgent>();

        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlCrockField.id_i.ToString(), tempCrockAgent.ID.ToString()));
        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlCrockField.type_i.ToString(), tempCrockAgent.type_i.ToString()));
        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlCrockField.mapx_i.ToString(), tempCrockAgent.mapx_i.ToString()));
        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlCrockField.mapy_i.ToString(), tempCrockAgent.mapy_i.ToString()));
        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlCrockField.rotation_l.ToString(), RemoveBrackets(tempCrockAgent.rotation_l)));

        root.AppendChild(gameObjectNode);
    }


    /// <summary>
    /// Create a general information xml node
    /// </summary>
    /// <param name="xmldoc">Xml document</param>
    /// <param name="field">field name</param>
    /// <param name="str">value</param>
    /// <returns>A general information </returns>
    private static XmlNode CreateGeneralInfoNode(XmlDocument xmldoc, string field, string str)
    {
        XmlNode transformVec = xmldoc.CreateElement(field);
        transformVec.InnerText = str;
        return transformVec;
    }

    /// <summary>
    /// Create a node for a string message of game object
    /// </summary>
    /// <param name="xmldoc">Xml document<</param>
    /// <param name="field">Xml field</param>
    /// <param name="str">string message</param>
    /// <returns>A node for the string message of game object</returns>
    private static XmlNode CreateGameObjectStringNode(XmlDocument xmldoc, string field, string str)
    {
        return CreateGeneralInfoNode(xmldoc, field, str.ToString());
    }


    /// <summary>
    /// Create a node for a vector3 message of game object
    /// </summary>
    /// <param name="xmldoc">Xml document</param>
    /// <param name="field">Xml field</param>
    /// <param name="transform">vector3 of game object</param>
    /// <returns>A node for the vector3 message of game object</returns>
    private static XmlNode CreateGameObjectVecotr3Node(XmlDocument xmldoc, string field, Vector3 vec)
    {
        return CreateGeneralInfoNode(xmldoc, field, vec.ToString());
    }


    /// <summary>
    /// Remove the brackets of a string, usually the string is create by the ToString() method of a object in
    /// Vecter2, Vecter3 or Vecter4 type.
    /// </summary>
    /// <param name="str">Source string</param>
    /// <returns>Target string without brackets</returns>
    private static string RemoveBrackets(string sourceStr)
    {
        return sourceStr.Replace("(", "").Replace(")", "");
    }


    private static string GetListElementString<T>(List<T> source)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in source)
        {
            sb.Append(item.ToString() + ", ");
        }
        return sb.Length == 0 ? sb.ToString() : sb.Remove(sb.Length - 2, 2).ToString();
    }
}
