/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：GameObjectsMessageCollecter
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：20130126
// 修改日期：20130313
// 模块描述：对游戏对象进行导出
// 版本编号：V2.0
//----------------------------------------------------------------*/

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using Mogo.Game;

#region suffix
/*
public enum XmlMonsterField
{
    type_s,
    id_i,
    posx_f,
    posy_f,
    rotation_l
}

public enum XmlNPCField
{
    type_s,
    id_i,
    posx_f,
    posy_f,
    rotation_l
}

public enum XmlTeleportPointSrcField
{
    type_s,
    id_i,
    posx_f,
    posy_f,
    targetSceneId_i,
    destination_s
}

public enum XmlTeleportPointDesField
{
    type_s,
    id_i,
    posx_f,
    posy_f,
    label_s
}

public enum XmlSpawnPointField
{
    type_s,
    id_i,
    posx_f,
    posy_f,
    length_f,
    width_f,
    monsterID_l,
    monsterNumber_l,
    duration_i
}
 * */
#endregion

public enum XmlMonsterField
{
    type,
    id,
    posx,
    posy,
    rotation
}

public enum XmlNPCField
{
    id_i,
    name_i,
    mode_i,
    tips_l,
    mapx_i,
    mapy_i,
    rotation_l,
    dialogBoxImage_i,
    standbyAction_i
}

public enum XmlTeleportPointSrcField
{
    type,
    id,
    posx,
    posy,
    targetSceneId,
    targetX,
    targetY
}

public enum XmlTeleportPointDesField
{
    type,
    id,
    posx,
    posy,
    label
}

public enum XmlSpawnPointField
{
    type,
    id,
    monsterSpawntPoint,

    triggerType,
    preSpawnPointId,

    //monsterIdEasy,
    //monsterNumberEasy,
    //dropIdEasy,

    //monsterIdMedium,
    //monsterNumberMedium,
    //dropIdMedium,

    //monsterIdHard,
    //monsterNumberHard,
    //dropIdHard,

    levelID,

    triggeRangeX,
    triggeRangeY,
    triggeRangeLength,
    triggeRangeWidth,

    homerangeX,
    homerangeY,
    homerangeLength,
    homerangeWidth
}


/// <summary>
/// Export the gameobjects we want to record to an xml file
/// </summary>
public class GameObjectsMessageCollecter : MonoBehaviour {

    private const string EXPORT_FILE_PATH = "Assets/Resources/xml";
    private const string EXPORT_FILE_SUFFIX = ".xml";
    // private const string PREFAB_FILE_PATH = "Assets\\Prefabs";

    [MenuItem("Mogo/Export Space File")]
    public static void ExportXmlFile()
    {
        if (!Directory.Exists(EXPORT_FILE_PATH))
            Directory.CreateDirectory(EXPORT_FILE_PATH);

        //try
        //{
            GameObject[] gos = (GameObject[])(UnityEngine.Object.FindObjectsOfType(typeof(GameObject)));
            if (gos != null)
            {
                XmlDocument xmldoc = new XmlDocument();
                XmlNode root = xmldoc.CreateNode(XmlNodeType.Element, "root", "");

                XmlNode entities = xmldoc.CreateElement("entities");

                foreach (GameObject go in gos)
                {
                    // if (go.transform.parent == null &&
                    if (go.GetComponent<ExportAgent>() != null
                        || go.GetComponent<ServerTeleportPointSrc>() != null
                        || go.GetComponent<ServerTeleportPointDes>() != null
                        || go.GetComponent<SpawnPoint>() != null)
                        // )
                    {
                        ExportGameObjectMessage(xmldoc, entities, go);
                    }
                }

                root.AppendChild(entities);

                XmlNode mapname = xmldoc.CreateElement("mapname");
                mapname.InnerText = Application.loadedLevelName;
                root.AppendChild(mapname);

                xmldoc.AppendChild(root);

                foreach (GameObject go in gos)
                {
                    if (go.name.EndsWith("_SpawnPointAgent"))
                    {
                        xmldoc.Save(Path.Combine(EXPORT_FILE_PATH, "s" + go.name.Remove(5) + EXPORT_FILE_SUFFIX));
                        AssetDatabase.Refresh();
                        return;
                    }
                }

                xmldoc.Save(Path.Combine(EXPORT_FILE_PATH, "s" + Application.loadedLevelName.Remove(5) + EXPORT_FILE_SUFFIX));
                AssetDatabase.Refresh();
            }
        //}
        //catch (Exception ex)
        //{
        //    Mogo.Util.LoggerHelper.Debug(ex.Message);
        //}
    }


    /// <summary>
    /// Export the message of game object
    /// </summary>
    /// <param name="xmldoc">Xml document</param>
    /// <param name="root">Root node of xml document</param>
    /// <param name="go">Gameobject</param>
    private static void ExportGameObjectMessage(XmlDocument xmldoc, XmlNode root, GameObject go)
    {
        XmlNode gameObjectNode = xmldoc.CreateElement("entity");

        //ActorParent tempActor = go.GetComponent<ActorParent>();
        //if (tempActor != null)
        //{
        //    EntityParent tempEntity = tempActor.GetEntity();
        //    if (tempEntity != null)
        //    {
        //        Type entityType = tempEntity.GetType();

        //        if (entityType == typeof(EntityMonster))
        //        {
        //            EntityMonster tempMonsterEntity = tempEntity as EntityMonster;
        //            // to do
        //            // 当前entityType没有填写
        //            // gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlMonsterField.type.ToString(), tempMonsterEntity.entityType));
        //            // 替代方案：
        //            gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlMonsterField.type.ToString(), entityType.ToString()));
        //            gameObjectNode.AppendChild(CreateGameObjectIntNode(xmldoc, XmlMonsterField.id.ToString(), tempMonsterEntity.ID));
        //            gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlMonsterField.posx.ToString(), tempMonsterEntity.Transform.position.x.ToString()));
        //            gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlMonsterField.posy.ToString(), tempMonsterEntity.Transform.position.z.ToString()));
        //            gameObjectNode.AppendChild(CreateGameObjectQuaternionNode(xmldoc, XmlMonsterField.rotation.ToString(), tempMonsterEntity.Transform.rotation));
        //        }
        //        else if (entityType == typeof(EntityNPC))
        //        {
        //            EntityNPC tempNPCEntity = tempEntity as EntityNPC;
        //            // to do
        //            // 当前entityType没有填写
        //            // gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlMonsterField.type.ToString(), tempMonsterEntity.entityType));
        //            // 替代方案：
        //            gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlNPCField.type.ToString(), entityType.ToString()));
        //            // to do 
        //            // NPC的Data还没有弄出来，所以这个ID是不对的
        //            gameObjectNode.AppendChild(CreateGameObjectIntNode(xmldoc, XmlNPCField.id.ToString(), tempNPCEntity.ID));
        //            gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlNPCField.posx.ToString(), tempNPCEntity.Transform.position.x.ToString()));
        //            gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlNPCField.posy.ToString(), tempNPCEntity.Transform.position.z.ToString()));
        //            gameObjectNode.AppendChild(CreateGameObjectQuaternionNode(xmldoc, XmlNPCField.rotation.ToString(), tempNPCEntity.Transform.rotation));
        //        }
        //    }
        //}

        ExportAgent agent = go.GetComponent<ExportAgent>();
        if (agent != null)
        {
            Type agentType = agent.GetType();

            if (agentType == typeof(MonsterExportAgent))
            {
                MonsterExportAgent tempMonsterAgent = agent as MonsterExportAgent;
                gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlMonsterField.type.ToString(), tempMonsterAgent.type));
                gameObjectNode.AppendChild(CreateGameObjectIntNode(xmldoc, XmlMonsterField.id.ToString(), tempMonsterAgent.ID));
                gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlMonsterField.posx.ToString(), tempMonsterAgent.theTransform.position.x.ToCMString()));
                gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlMonsterField.posy.ToString(), tempMonsterAgent.theTransform.position.z.ToCMString()));
                gameObjectNode.AppendChild(CreateGameObjectQuaternionNode(xmldoc, XmlMonsterField.rotation.ToString(), tempMonsterAgent.transform.rotation));
            }
            else if (agentType == typeof(NpcExportAgent))
            {
                //NpcExportAgent tempNPCAgent = agent as NpcExportAgent;

                //gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlNPCField.type.ToString(), tempNPCAgent.type));
                //gameObjectNode.AppendChild(CreateGameObjectIntNode(xmldoc, XmlNPCField.id.ToString(), tempNPCAgent.ID));
                //gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlNPCField.posx.ToString(), tempNPCAgent.theTransform.position.x.ToCMString()));
                //gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlNPCField.posy.ToString(), tempNPCAgent.theTransform.position.z.ToCMString()));
                //gameObjectNode.AppendChild(CreateGameObjectQuaternionNode(xmldoc, XmlNPCField.rotation.ToString(), tempNPCAgent.transform.rotation));
            }
        }

        // 不是ExportAgent
        else
        {
            GearParent[] tempGears = go.GetComponentsInChildren<GearParent>();
            if (tempGears != null)
            {
                foreach (GearParent tempGear in tempGears)
                {
                    Type gearType = tempGear.GetType();
                    if (gearType == typeof(ServerTeleportPointSrc))
                    {
                        ServerTeleportPointSrc tempTeleportPointSrcGear = tempGear as ServerTeleportPointSrc;
                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlTeleportPointSrcField.type.ToString(), "TeleportPointSrc"));

                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlTeleportPointSrcField.id.ToString(), tempTeleportPointSrcGear.ID.ToString()));
                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlTeleportPointSrcField.posx.ToString(), tempTeleportPointSrcGear.transform.position.x.ToCMString()));
                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlTeleportPointSrcField.posy.ToString(), tempTeleportPointSrcGear.transform.position.z.ToCMString()));
                        gameObjectNode.AppendChild(CreateGameObjectIntNode(xmldoc, XmlTeleportPointSrcField.targetSceneId.ToString(), tempTeleportPointSrcGear.targetSceneId));
                        //gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlTeleportPointSrcField.destination_s.ToString(), tempTeleportPointSrcGear.targetLabel));

                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlTeleportPointSrcField.targetX.ToString(), tempTeleportPointSrcGear.targetX.ToCMString()));
                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlTeleportPointSrcField.targetY.ToString(), tempTeleportPointSrcGear.targetY.ToCMString()));
                    }
                    else if (gearType == typeof(ServerTeleportPointDes))
                    {
                        ServerTeleportPointDes tempTeleportPointDesGear = tempGear as ServerTeleportPointDes;
                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlTeleportPointDesField.type.ToString(), "TeleportPointDes"));

                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlTeleportPointDesField.id.ToString(), tempTeleportPointDesGear.ID.ToString()));
                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlTeleportPointDesField.posx.ToString(), tempTeleportPointDesGear.transform.position.x.ToCMString()));
                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlTeleportPointDesField.posy.ToString(), tempTeleportPointDesGear.transform.position.z.ToCMString()));
                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlTeleportPointDesField.label.ToString(), tempTeleportPointDesGear.lable));
                    }

                    else if (gearType == typeof(SpawnPoint))
                    {
                        SpawnPoint tempSpawnPointGear = tempGear as SpawnPoint;

                        StringBuilder sb = new StringBuilder();
                        foreach (Transform point in tempSpawnPointGear.realMonsterSpawnPoint)
                        {
                            sb.Append(point.position.x.ToCMString() + "," + point.position.z.ToCMString() + ",");
                        }
                        sb.Remove(sb.Length - 1, 1);
                        string monsterSpawntPointString = sb.ToString();

                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlSpawnPointField.id.ToString(), tempSpawnPointGear.ID.ToString()));

                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlSpawnPointField.type.ToString(), tempSpawnPointGear.gearType));

                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlSpawnPointField.monsterSpawntPoint.ToString(), monsterSpawntPointString));

                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlSpawnPointField.triggerType.ToString(), tempSpawnPointGear.triggerType.ToString()));
                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlSpawnPointField.preSpawnPointId.ToString(), GetListElementString<int>(tempSpawnPointGear.preSpawnPointId)));

                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlSpawnPointField.levelID.ToString(), GetListElementString<int>(tempSpawnPointGear.levelId)));

                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlSpawnPointField.triggeRangeX.ToString(), tempSpawnPointGear.triggeRangex.ToCMString()));
                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlSpawnPointField.triggeRangeY.ToString(), tempSpawnPointGear.triggeRangey.ToCMString()));
                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlSpawnPointField.triggeRangeLength.ToString(), tempSpawnPointGear.triggeRangeLength.ToCMString()));
                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlSpawnPointField.triggeRangeWidth.ToString(), tempSpawnPointGear.triggeRangeWidth.ToCMString()));

                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlSpawnPointField.homerangeX.ToString(), tempSpawnPointGear.homerangex.ToCMString()));
                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlSpawnPointField.homerangeY.ToString(), tempSpawnPointGear.homerangey.ToCMString()));
                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlSpawnPointField.homerangeLength.ToString(), tempSpawnPointGear.homerangeLength.ToCMString()));
                        gameObjectNode.AppendChild(CreateGameObjectStringNode(xmldoc, XmlSpawnPointField.homerangeWidth.ToString(), tempSpawnPointGear.homerangeWidth.ToCMString()));
                    }
                }
            }
        }

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

public static class UtilsEX
{
    public static string ToCMString(this float mValue)
    {
        return ((int)(mValue * 100)).ToString();
    }
}