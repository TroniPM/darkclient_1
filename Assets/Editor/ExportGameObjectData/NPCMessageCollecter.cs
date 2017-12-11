using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using Mogo.Game;

public class NPCMessageCollecter : MonoBehaviour
{
    private const string EXPORT_FILE_PATH = "Assets/Resources/xml";
    private const string EXPORT_FILE_NAME = "NPC.xml";

    [MenuItem("Mogo/Export NPC")]
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
                if (go.GetComponent<NpcExportAgent>() != null)
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

        NpcExportAgent tempNPCAgent = go.GetComponent<NpcExportAgent>();

        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlNPCField.id_i.ToString(), tempNPCAgent.ID.ToString()));
        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlNPCField.name_i.ToString(), tempNPCAgent.name_i.ToString()));
        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlNPCField.mode_i.ToString(), tempNPCAgent.mode_i.ToString()));
        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlNPCField.tips_l.ToString(), tempNPCAgent.tips_l.ToString()));
        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlNPCField.mapx_i.ToString(), tempNPCAgent.mapx_i.ToCMString()));
        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlNPCField.mapy_i.ToString(), tempNPCAgent.mapy_i.ToCMString()));
        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlNPCField.rotation_l.ToString(), RemoveBrackets(tempNPCAgent.rotation_l)));
        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlNPCField.dialogBoxImage_i.ToString(), tempNPCAgent.dialogBoxImage_i.ToString()));
        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlNPCField.standbyAction_i.ToString(), tempNPCAgent.standbyAction_i.ToString()));

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
    /// Create a node for a int message of game object
    /// </summary>
    /// <param name="xmldoc">Xml document</param>
    /// <param name="field">Xml field</param>
    /// <param name="i">int message</param>
    /// <returns>A node for the int message of game object<</returns>
    private static XmlNode CreateGameObjectIntNode(XmlDocument xmldoc, string field, int i)
    {
        return CreateGeneralInfoNode(xmldoc, field, i.ToString());
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
    ///  Create a node for a quaternion message of game object
    /// </summary>
    /// <param name="xmldoc">Xml documen</param>
    /// <param name="field">Xml field</param>
    /// <param name="vec">quaternion of game object</param>
    /// <returns>A node for the quaternion message of game object</returns>
    private static XmlNode CreateGameObjectQuaternionNode(XmlDocument xmldoc, string field, Quaternion qua)
    {
        return CreateGeneralInfoNode(xmldoc, field, RemoveBrackets(qua.ToString()));
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