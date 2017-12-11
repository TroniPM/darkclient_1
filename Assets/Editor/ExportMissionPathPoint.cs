using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;

public class ExportMissionPathPoint : MonoBehaviour
{
    private static string EXPORT_FILE_PATH = "Assets/Resources/data/MissionPathPoint";

    [MenuItem("Mogo/Export Mission PathPoint")]
    public static void ExportXmlFile()
    {
        if (!Directory.Exists(EXPORT_FILE_PATH))
            Directory.CreateDirectory(EXPORT_FILE_PATH);

        XmlDocument xmldoc = new XmlDocument();
        XmlNode root = xmldoc.CreateNode(XmlNodeType.Element, "root", "");

        GameObject[] gos = (GameObject[])(UnityEngine.Object.FindObjectsOfType(typeof(GameObject)));
        if (gos != null)
        {
            foreach (GameObject go in gos)
            {
                var script = go.GetComponent<MissionPathPoint>();
                if (script != null)
                {
                    XmlNode pathPoint = xmldoc.CreateElement("gears");

                    XmlNode idNode = xmldoc.CreateElement("id");
                    idNode.InnerText = script.id.ToString();
                    pathPoint.AppendChild(idNode);

                    XmlNode preIDNode = xmldoc.CreateElement("preID");
                    preIDNode.InnerText = script.preID.ToString();
                    pathPoint.AppendChild(preIDNode);

                    XmlNode typeNode = xmldoc.CreateElement("type");
                    typeNode.InnerText = ((int)script.isNormalType).ToString();
                    pathPoint.AppendChild(typeNode);

                    XmlNode isEnableNode = xmldoc.CreateElement("isEnable");
                    isEnableNode.InnerText = ((int)script.isEnable).ToString();
                    pathPoint.AppendChild(isEnableNode);

                    XmlNode pointTypeNode = xmldoc.CreateElement("isPointer");
                    pointTypeNode.InnerText = ((int)script.pointerType).ToString();
                    pathPoint.AppendChild(pointTypeNode);

                    XmlNode rangeNode = xmldoc.CreateElement("range");
                    rangeNode.InnerText = script.range.ToString("f2");
                    pathPoint.AppendChild(rangeNode);

                    XmlNode positionNode = xmldoc.CreateElement("position");
                    positionNode.InnerText = RemoveBrackets(script.pathPointPosition.ToString());
                    pathPoint.AppendChild(positionNode);

                    XmlNode rotationNode = xmldoc.CreateElement("rotation");
                    rotationNode.InnerText = RemoveBrackets(script.pathPointRotation.ToString());
                    pathPoint.AppendChild(rotationNode);

                    XmlNode sizeNode = xmldoc.CreateElement("size");
                    sizeNode.InnerText = RemoveBrackets(script.pathPointSize.ToString());
                    pathPoint.AppendChild(sizeNode);

                    XmlNode movePositionNode = xmldoc.CreateElement("movePosition");
                    movePositionNode.InnerText = RemoveBrackets(script.movePosition.ToString());
                    pathPoint.AppendChild(movePositionNode);

                    XmlNode deleteListNode = xmldoc.CreateElement("deleteList");

                    List<int> tempDeleteList = new List<int>();
                    foreach(var tempDeleteItem in script.deleteList)
                    {
                        tempDeleteList.Add(tempDeleteItem);
                    }
                    deleteListNode.InnerText = GetListElementString<int>(tempDeleteList);

                    pathPoint.AppendChild(deleteListNode);

                    root.AppendChild(pathPoint);
                }
            }
        }

        xmldoc.AppendChild(root);

        string EXPORT_FILE_NAME = Application.loadedLevelName.Remove(5) + "_PathPoint.xml";

        foreach (GameObject go in gos)
        {
            if (go.name.EndsWith("_PathPoint"))
            {
                xmldoc.Save(Path.Combine(EXPORT_FILE_PATH, go.name.Remove(5) + "_PathPoint.xml"));
                AssetDatabase.Refresh();
                return;
            }
        }

        xmldoc.Save(Path.Combine(EXPORT_FILE_PATH, EXPORT_FILE_NAME));
        AssetDatabase.Refresh();
    }

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

