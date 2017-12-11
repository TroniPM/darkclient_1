using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using Mogo.Game;
using Mogo.Util;


public class ExportGear : MonoBehaviour
{
    private const string GEAR_FILE_PATH = "TrapStudio";
    private const string EXPORT_GEAR_FILE_PATH = "Resources/Scences/Traps";

    private const string EXPORT_FILE_PATH = "Assets/Resources/data/xml";
    private const string EXPORT_FILE_NAME = "GearData.xml";

    private const string EXPORT_PERFAB_PATH = "Assets/Resources/Scences/Traps/";

    [MenuItem("Mogo/Export Gears")]
    public static void ExportXmlFile()
    {
        #region Export Gear

        XmlDocument xmldoc = new XmlDocument();
        // xmldoc.Load(Path.Combine(EXPORT_FILE_PATH, EXPORT_FILE_NAME));
        // XmlNode root = xmldoc.GetElementsByTagName("root")[0];
        
        // int index = root.ChildNodes.Count + 1;
        
        XmlNode root = xmldoc.CreateNode(XmlNodeType.Element, "root", "");
        int index = root.ChildNodes.Count + 1;

        //GameObject[] gos = (GameObject[])(UnityEngine.Object.FindObjectsOfType(typeof(GameObject)));

        var gs = ExportScenesManager.GetFromRoot<GameObject>(GEAR_FILE_PATH, true, ".prefab");

        List<GameObject> gos = new List<GameObject>();
        Queue<Transform> queueTrans = new Queue<Transform>();

        foreach (var g in gs)
        {
            queueTrans.Enqueue((g as GameObject).transform);
        }

        while (queueTrans.Count != 0)
        {
            Transform temp = queueTrans.Dequeue();
            gos.Add(temp.gameObject);
            foreach (Transform child in temp)
                queueTrans.Enqueue(child);
        }

        if (gos != null)
        {
            LoggerHelper.Debug("All GameObjects Totally: " + gos.Count);

            foreach (GameObject go in gos)
            {
                if (!go)
                    continue;
                
                LoggerHelper.Debug("GameObject: " + go.name);

                var gps = go.GetComponents<GearParent>();
                if (gps != null)
                {
                    foreach (var g in gps)
                    {
                        Type t = g.GetType();
                        XmlNode entities = xmldoc.CreateElement("gears");

                        LoggerHelper.Debug("gear: " + g);

                        XmlNode gearID = xmldoc.CreateElement("id");
                        gearID.InnerText = index.ToString();
                        entities.AppendChild(gearID);
                        index++;

                        XmlNode gearType = xmldoc.CreateElement("type");
                        gearType.InnerText = t.Name;
                        entities.AppendChild(gearType);

                        LoggerHelper.Debug("type: " + t.Name);

                        XmlNode gearGameObjectName = xmldoc.CreateElement("gameObjectName");

                        Transform xmlTransform = go.transform;
                        string xmlNameStr = xmlTransform.name;

                        while (xmlTransform.parent != null)
                        {
                            xmlNameStr = xmlTransform.parent.name + "/" + xmlNameStr;
                            xmlTransform = xmlTransform.parent;
                        }
                        gearGameObjectName.InnerText = xmlNameStr;
                        entities.AppendChild(gearGameObjectName);

                        LoggerHelper.Debug("gameObjectName: " + xmlNameStr);

                        XmlNode gearMapID = xmldoc.CreateElement("map");
                        gearMapID.InnerText = int.Parse(xmlNameStr.Remove(5)).ToString();
                        entities.AppendChild(gearMapID);

                        StringBuilder propName = new StringBuilder();
                        StringBuilder propType = new StringBuilder();
                        StringBuilder propValue = new StringBuilder();

                        //foreach (var prop in t.GetFields(~(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.FlattenHierarchy)))
                        foreach (var prop in t.GetFields())
                        {
                            var v = prop.GetValue(g);
                            LoggerHelper.Debug("Field: " + v);

                            Type vt = prop.FieldType;
                            LoggerHelper.Debug("Field Type: " + vt);

                            if (vt.IsArray)
                            {
                                Type pt = vt.GetElementType();

                                if (pt == typeof(AnimationClip))
                                {
                                    propName.Append(prop.Name);
                                    propName.Append("|");
                                    propType.Append(pt.ToString());
                                    propType.Append("|");

                                    var animationChipArray = v as AnimationClip[];

                                    string valueStr = "";

                                    foreach (var animationChip in animationChipArray)
                                    {
                                        valueStr += animationChip.name + "anim";
                                        valueStr += ":";
                                    }

                                    if (valueStr.Length > 0)
                                        valueStr.Remove(valueStr.Length - 1, 1);

                                    propValue.Append(valueStr);
                                    propValue.Append("|");
                                }
                                else if (pt.IsSubclassOf(typeof(GearParent)) || pt == typeof(GearParent))
                                {
                                    propName.Append(prop.Name);
                                    propName.Append("|");
                                    propType.Append(pt.ToString());
                                    propType.Append("|");

                                    GearParent[] theGearArray = v as GearParent[];

                                    string valueStr = "";
                                    string innerValueStr = "";

                                    if (theGearArray != null && theGearArray.Length > 0)
                                    {
                                        for (int i = 0; i < theGearArray.Length; i++)
                                        {
                                            var theTransform = theGearArray[i].gameObject.transform;

                                            LoggerHelper.Debug("FFFFFFFFFFFFFFFFFF " + valueStr);

                                            innerValueStr = theTransform.name;

                                            while (theTransform.parent != null)
                                            {
                                                innerValueStr = theTransform.parent.name + "/" + innerValueStr;
                                                theTransform = theTransform.parent;
                                            }

                                            valueStr += innerValueStr;
                                            valueStr += ":";
                                        }

                                        if (valueStr.Length > 0)
                                            valueStr = valueStr.Remove(valueStr.Length - 1, 1);

                                        propValue.Append(valueStr);
                                        propValue.Append("|");
                                    }
                                    else
                                    {
                                        propValue.Append("|");
                                    }
                                }
                                else if (pt == typeof(GameObject))
                                {
                                    LoggerHelper.Debug("Going to deal with: " + pt);
                                    propName.Append(prop.Name);
                                    propName.Append("|");
                                    propType.Append(pt.ToString());
                                    propType.Append("|");

                                    GameObject[] theGameOjectArray = v as GameObject[];

                                    string valueStr = "";
                                    string innerValueStr = "";

                                    if (theGameOjectArray != null && theGameOjectArray.Length > 0)
                                    {
                                        for (int i = 0; i < theGameOjectArray.Length; i++)
                                        {
                                            Transform theTransform = theGameOjectArray[i].transform;

                                            innerValueStr = theTransform.name;

                                            while (theTransform.parent != null)
                                            {
                                                innerValueStr = theTransform.parent.name + "/" + innerValueStr;
                                                theTransform = theTransform.parent;
                                            }

                                            valueStr += innerValueStr;
                                            valueStr += ":";
                                        }

                                        if (valueStr.Length > 0)
                                            valueStr = valueStr.Remove(valueStr.Length - 1, 1);

                                        propValue.Append(valueStr);
                                        propValue.Append("|");

                                    }
                                    else
                                    {
                                        propValue.Append("|");
                                    }
                                }
                                else if (pt == typeof(Transform))
                                {
                                    LoggerHelper.Debug("Going to deal with: " + pt);
                                    propName.Append(prop.Name);
                                    propName.Append("|");
                                    propType.Append(pt.ToString());
                                    propType.Append("|");

                                    Transform[] theTransformArray = v as Transform[];

                                    string valueStr = "";
                                    string innerValueStr = "";

                                    if (theTransformArray != null && theTransformArray.Length > 0)
                                    {
                                        for (int i = 0; i < theTransformArray.Length; i++)
                                        {
                                            Transform theTransform = theTransformArray[i];

                                            innerValueStr = theTransform.name;

                                            while (theTransform.parent != null)
                                            {
                                                innerValueStr = theTransform.parent.name + "/" + innerValueStr;
                                                theTransform = theTransform.parent;
                                            }

                                            valueStr += innerValueStr;
                                            valueStr += ":";
                                        }

                                        if (valueStr.Length > 0)
                                            valueStr = valueStr.Remove(valueStr.Length - 1, 1);

                                        propValue.Append(valueStr);
                                        propValue.Append("|");
                                    }
                                    else
                                    {
                                        propValue.Append("|");
                                    }
                                }
                                else if (pt == typeof(Animation))
                                {
                                    LoggerHelper.Debug("Going to deal with: " + pt);
                                    propName.Append(prop.Name);
                                    propName.Append("|");
                                    propType.Append(pt.ToString());
                                    propType.Append("|");

                                    Animation[] theAnimationArray = v as Animation[];

                                    string valueStr = "";
                                    string innerValueStr = "";

                                    if (theAnimationArray != null && theAnimationArray.Length > 0)
                                    {
                                        for (int i = 0; i < theAnimationArray.Length; i++)
                                        {
                                            Transform theTransform = theAnimationArray[i].gameObject.transform;

                                            innerValueStr = theTransform.name;

                                            while (theTransform.parent != null)
                                            {
                                                innerValueStr = theTransform.parent.name + "/" + innerValueStr;
                                                theTransform = theTransform.parent;
                                            }

                                            valueStr += innerValueStr;
                                            valueStr += ":";
                                        }

                                        if (valueStr.Length > 0)
                                            valueStr = valueStr.Remove(valueStr.Length - 1, 1);

                                        propValue.Append(valueStr);
                                        propValue.Append("|");
                                    }
                                    else
                                    {
                                        propValue.Append("|");
                                    }
                                }
                                else if (pt == typeof(Vector3))
                                {
                                    LoggerHelper.Debug("Going to deal with: " + pt);
                                    propName.Append(prop.Name);
                                    propName.Append("|");
                                    propType.Append(pt.ToString());
                                    propType.Append("|");

                                    Vector3[] theVectorArray = v as Vector3[];

                                    string valueStr = "";
                                    string innerValueStr = "";

                                    if (theVectorArray != null && theVectorArray.Length > 0)
                                    {
                                        for (int i = 0; i < theVectorArray.Length; i++)
                                        {
                                            innerValueStr = theVectorArray[i].x + "," + theVectorArray[i].y + "," + theVectorArray[i].z;

                                            valueStr += innerValueStr;
                                            valueStr += ":";
                                        }

                                        if (valueStr.Length > 0)
                                            valueStr = valueStr.Remove(valueStr.Length - 1, 1);

                                        propValue.Append(valueStr);
                                        propValue.Append("|");
                                    }
                                    else
                                    {
                                        propValue.Append("|");
                                    }
                                }
                                else if (pt == typeof(int))
                                {
                                    LoggerHelper.Debug("Going to deal with: " + pt);
                                    propName.Append(prop.Name);
                                    propName.Append("|");
                                    propType.Append(pt.ToString());
                                    propType.Append("|");

                                    int[] theIntArray = v as int[];

                                    string valueStr = "";
                                    string innerValueStr = "";

                                    if (theIntArray != null && theIntArray.Length > 0)
                                    {
                                        for (int i = 0; i < theIntArray.Length; i++)
                                        {
                                            innerValueStr = theIntArray[i].ToString();

                                            valueStr += innerValueStr;
                                            valueStr += ":";
                                        }

                                        if (valueStr.Length > 0)
                                            valueStr = valueStr.Remove(valueStr.Length - 1, 1);

                                        propValue.Append(valueStr);
                                    }

                                    propValue.Append("|");
                                }
                                else
                                {
                                    LoggerHelper.Debug("Can't find a way for this gear");
                                    propValue.Append("|");

                                    //propName.Append(prop.Name);
                                    //propName.Append(",");
                                    //propType.Append(pt.ToString());
                                    //propType.Append(",");
                                    //propValue.Append(v.ToString());
                                    //propValue.Append(",");
                                }

                            }
                            else
                            {
                                if (vt == typeof(AnimationClip))
                                {
                                    propName.Append(prop.Name);
                                    propName.Append("|");
                                    propType.Append(vt.ToString());
                                    propType.Append("|");

                                    if ((v as AnimationClip) != null)
                                        propValue.Append((v as AnimationClip).name + ".anim");
                                    else
                                        propValue.Append("null.anim");

                                    propValue.Append("|");
                                }
                                else if (vt.IsSubclassOf(typeof(GearParent)))
                                {
                                    propName.Append(prop.Name);
                                    propName.Append("|");
                                    propType.Append(vt.ToString());
                                    propType.Append("|");
                                    Transform theTransform = (v as GearParent).gameObject.transform;

                                    string valueStr = theTransform.name;

                                    while (theTransform.parent != null)
                                    {
                                        valueStr = theTransform.parent.name + "/" + valueStr;
                                        theTransform = theTransform.parent;
                                    }

                                    propValue.Append(valueStr);
                                    propValue.Append("|");
                                }
                                else if (vt == typeof(GameObject))
                                {
                                    LoggerHelper.Debug("Going to deal with: " + vt);
                                    propName.Append(prop.Name);
                                    propName.Append("|");
                                    propType.Append(vt.ToString());
                                    propType.Append("|");
                                    Transform theTransform = (v as GameObject).transform;

                                    string valueStr = theTransform.name;

                                    while (theTransform.parent != null)
                                    {
                                        valueStr = theTransform.parent.name + "/" + valueStr;
                                        theTransform = theTransform.parent;
                                    }

                                    propValue.Append(valueStr);
                                    propValue.Append("|");
                                }
                                else if (vt == typeof(Animation))
                                {
                                    LoggerHelper.Debug("Going to deal with: " + vt);
                                    propName.Append(prop.Name);
                                    propName.Append("|");
                                    propType.Append(vt.ToString());
                                    propType.Append("|");
                                    Transform theTransform = (v as Animation).gameObject.transform;

                                    if (theTransform != null)
                                    {
                                        string valueStr = theTransform.name;

                                        while (theTransform.parent != null)
                                        {
                                            valueStr = theTransform.parent.name + "/" + valueStr;
                                            theTransform = theTransform.parent;
                                            LoggerHelper.Debug("Going to deal with: " + valueStr);
                                        }

                                        propValue.Append(valueStr);
                                        propValue.Append("|");
                                    }
                                }
                                else if (vt == typeof(Transform))
                                {
                                    LoggerHelper.Debug("Going to deal with: " + vt);
                                    propName.Append(prop.Name);
                                    propName.Append("|");
                                    propType.Append(vt.ToString());
                                    propType.Append("|");
                                    Transform theTransform = v as Transform;

                                    if (theTransform != null)
                                    {
                                        string valueStr = theTransform.name;

                                        while (theTransform.parent != null)
                                        {
                                            valueStr = theTransform.parent.name + "/" + valueStr;
                                            theTransform = theTransform.parent;
                                            LoggerHelper.Debug("Going to deal with: " + valueStr);
                                        }

                                        propValue.Append(valueStr);
                                        propValue.Append("|");
                                    }
                                }
                                else if (vt.BaseType == typeof(Enum))
                                {
                                    propName.Append(prop.Name);
                                    propName.Append("|");
                                    propType.Append(vt.ToString());
                                    propType.Append("|");
                                    propValue.Append(Enum.Format(vt, v, "d"));
                                    propValue.Append("|");
                                }
                                else if (vt == typeof(Vector3))
                                {
                                    propName.Append(prop.Name);
                                    propName.Append("|");
                                    propType.Append(vt.ToString());
                                    propType.Append("|");

                                    Vector3 vec = (Vector3)v;
                                    propValue.Append(vec.x + "," + vec.y + "," + vec.z);
                                    propValue.Append("|");
                                }
                                else
                                {
                                    propName.Append(prop.Name);
                                    propName.Append("|");
                                    propType.Append(vt.ToString());
                                    propType.Append("|");
                                    propValue.Append(v.ToString());
                                    propValue.Append("|");
                                }
                            }
                        }
                        string propNameStr = propName.ToString();
                        string propTypeStr = propType.ToString();
                        string propValueStr = propValue.ToString();

                        if (propNameStr.Length > 0)
                            propNameStr = propNameStr.Remove(propNameStr.Length - 1);

                        if (propTypeStr.Length > 0)
                            propTypeStr = propTypeStr.Remove(propTypeStr.Length - 1);

                        if (propValueStr.Length > 0)
                            propValueStr = propValueStr.Remove(propValueStr.Length - 1);

                        XmlNode argNames = xmldoc.CreateElement("argNames");
                        argNames.InnerText = propNameStr;
                        entities.AppendChild(argNames);

                        XmlNode argTypes = xmldoc.CreateElement("argTypes");
                        argTypes.InnerText = propTypeStr;
                        entities.AppendChild(argTypes);

                        XmlNode args = xmldoc.CreateElement("args");
                        args.InnerText = propValueStr;
                        entities.AppendChild(args);

                        root.AppendChild(entities);
                        LoggerHelper.Debug("Gear Data Export End");
                    }
                }
            }
            LoggerHelper.Debug("Saving Data");
            xmldoc.AppendChild(root);
            xmldoc.Save(Path.Combine(EXPORT_FILE_PATH, EXPORT_FILE_NAME));
            LoggerHelper.Warning("All Gears Data Export End");
        }

        #endregion


        #region Copy GameObject

        foreach (var g in gs)
        {
            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(g), EXPORT_PERFAB_PATH + (g as GameObject).name + ".prefab");
        }

        #endregion
    }

    [MenuItem("Mogo/Remove Gear Scripts")]
    public static void RemoveGearsScript()
    {
        var gsSpare = ExportScenesManager.GetFromRoot<GameObject>(EXPORT_GEAR_FILE_PATH, true, ".prefab");

        LoggerHelper.Debug("gsSpare: " + gsSpare.Count);

        List<GameObject> gosSpare = new List<GameObject>();
        Queue<Transform> queueTransSpare = new Queue<Transform>();

        foreach (var gSpare in gsSpare)
        {
            queueTransSpare.Enqueue((gSpare as GameObject).transform);
        }

        while (queueTransSpare.Count != 0)
        {
            Transform temp = queueTransSpare.Dequeue();
            gosSpare.Add(temp.gameObject);
            foreach (Transform child in temp)
                queueTransSpare.Enqueue(child);
        }

        LoggerHelper.Debug("gosSpare: " + gosSpare.Count);

        foreach (GameObject gSpare in gosSpare)
        {
            var gpsSpare = gSpare.GetComponents<GearParent>();
            foreach (var gpSpare in gpsSpare)
            {
                LoggerHelper.Debug("gpSpare: " + gpSpare.name);
                DestroyImmediate(gpSpare, true);
            }

            var gsfxsSpare = gSpare.GetComponents<SfxHandler>();
            foreach (var gsfxSpare in gsfxsSpare)
            {
                DestroyImmediate(gsfxSpare, true);
            }

            var gmotorsSpare = gSpare.GetComponents<MogoSimpleMotor>();
            foreach (var gmotorSpare in gmotorsSpare)
            {
                DestroyImmediate(gmotorSpare, true);
            }
        }
    }
}