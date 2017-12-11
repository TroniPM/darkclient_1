using UnityEngine;
using System.Collections;


public class CombineSkinnedMeshes : MonoBehaviour
{

    public bool generateTriangleStrips = true;
    public bool castShadows = true;
    public bool receiveShadows = true;



    public static int VERTEX_NUMBER = 0; 
    public static int BONE_NUMBER = 0;

    void Start()
    {
        Mogo.Util.LoggerHelper.Debug("CombineSkinnedMeshsssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss");
        ReCombineSkinnedMeshes();
    }

    void ReCombineSkinnedMeshes()
    {
        VERTEX_NUMBER = 0;
        BONE_NUMBER = 0;
        Component[] allsmr = GetComponentsInChildren(typeof(SkinnedMeshRenderer));

        for (int i = 0; i < allsmr.Length; ++i)
        {
            if (allsmr[i].name == name || ((SkinnedMeshRenderer)allsmr[i]).sharedMesh == null)
                continue;

            VERTEX_NUMBER += ((SkinnedMeshRenderer)allsmr[i]).sharedMesh.vertices.Length;
            BONE_NUMBER += ((SkinnedMeshRenderer)allsmr[i]).bones.Length;
        }
        Matrix4x4 myTransform = transform.worldToLocalMatrix;
        Hashtable materialToMesh = new Hashtable();

        Hashtable boneHash = new Hashtable();

        Transform[] totalBones = new Transform[BONE_NUMBER];

        Matrix4x4[] totalBindPoses = new Matrix4x4[BONE_NUMBER];

        BoneWeight[] totalBoneWeight = new BoneWeight[VERTEX_NUMBER];


        int offset = 0;
        int b_offset = 0;
        Transform[] usedBones = new Transform[totalBones.Length];


        for (int i = 0; i < allsmr.Length; i++)
        {
            if (allsmr[i].name == name || ((SkinnedMeshRenderer)allsmr[i]).sharedMesh == null)
                continue;

            SkinnedMeshRenderer smrenderer = (SkinnedMeshRenderer)allsmr[i];

            MeshCombineUtility.MeshInstance instance = new MeshCombineUtility.MeshInstance();

            instance.mesh = smrenderer.sharedMesh;

            if (smrenderer != null && smrenderer.enabled && instance.mesh != null)
            {

                instance.transform = myTransform * smrenderer.transform.localToWorldMatrix;


                Material[] materials = smrenderer.sharedMaterials;

                for (int m = 0; m < materials.Length; m++)
                {

                    instance.subMeshIndex = System.Math.Min(m, instance.mesh.subMeshCount - 1);

                    ArrayList objects = (ArrayList)materialToMesh[materials[m]];
                    if (objects != null)
                    {
                        objects.Add(instance);
                    }
                    else
                    {
                        objects = new ArrayList();
                        objects.Add(instance);
                        materialToMesh.Add(materials[m], objects);
                    }
                }

                for (int x = 0; x < smrenderer.bones.Length; x++)
                {

                    bool flag = false;
                    for (int j = 0; j < totalBones.Length; j++)
                    {
                        if (usedBones[j] != null)

                            if ((smrenderer.bones[x] == usedBones[j]))
                            {
                                flag = true;
                                break;
                            }
                    }

                    if (!flag)
                    {
                        for (int f = 0; f < totalBones.Length; f++)
                        {
                            if (usedBones[f] == null)
                            {
                                usedBones[f] = smrenderer.bones[x];
                                break;
                            }
                        }
                        totalBones[offset] = smrenderer.bones[x];
                        boneHash.Add(smrenderer.bones[x].name, offset);

                        totalBindPoses[offset] = smrenderer.bones[x].worldToLocalMatrix * transform.localToWorldMatrix;

                        offset++;

                    }

                }

                for (int x = 0; x < smrenderer.sharedMesh.boneWeights.Length; x++)
                {
                    totalBoneWeight[b_offset] = recalculateIndexes(smrenderer.sharedMesh.boneWeights[x], boneHash, smrenderer.bones);
                    b_offset++;
                }

                ((SkinnedMeshRenderer)allsmr[i]).enabled = false;

            }


        }
        foreach (DictionaryEntry de in materialToMesh)
        {
            ArrayList elements = (ArrayList)de.Value;
            MeshCombineUtility.MeshInstance[] instances = (MeshCombineUtility.MeshInstance[])elements.ToArray(typeof(MeshCombineUtility.MeshInstance));

            //int i = 0;

            //foreach (var item in instances)
            //{
            //    Mogo.Util.LoggerHelper.Debug(item.mesh.name + " !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            //    Mesh mesh = Resources.Load(item.mesh.name) as Mesh;
            //    instances[i++].mesh = mesh;
            //}

            if (materialToMesh.Count == 1)
            {

                if (GetComponent(typeof(SkinnedMeshRenderer)) == null)
                {
                    gameObject.AddComponent<SkinnedMeshRenderer>();
                }

                SkinnedMeshRenderer objRenderer = (SkinnedMeshRenderer)GetComponent(typeof(SkinnedMeshRenderer));
                objRenderer.sharedMesh = MeshCombineUtility.Combine(instances, generateTriangleStrips);
                objRenderer.material = (Material)de.Key;

                objRenderer.castShadows = castShadows;
                objRenderer.receiveShadows = receiveShadows;

                objRenderer.sharedMesh.bindposes = totalBindPoses;

                objRenderer.sharedMesh.boneWeights = totalBoneWeight;


                objRenderer.bones = totalBones;

                objRenderer.sharedMesh.RecalculateNormals();
                objRenderer.sharedMesh.RecalculateBounds();

                objRenderer.enabled = true;


            }
            else
            {
                Mogo.Util.LoggerHelper.Debug("More Than One Material !!!!!! " + materialToMesh.Count);
                //GameObject go = new GameObject("CombinedSkinnedMesh");
                //go.transform.parent = transform;
                //go.transform.localScale = Vector3.one;
                //go.transform.localRotation = Quaternion.identity;
                //go.transform.localPosition = Vector3.zero;
                //go.AddComponent(typeof(SkinnedMeshRenderer));
                //((SkinnedMeshRenderer)go.GetComponent(typeof(SkinnedMeshRenderer))).material = (Material)de.Key;

                //SkinnedMeshRenderer objRenderer = (SkinnedMeshRenderer)go.GetComponent(typeof(SkinnedMeshRenderer));
                //objRenderer.sharedMesh = MeshCombineUtility.Combine(instances, generateTriangleStrips);

                //objRenderer.sharedMesh.bindposes = totalBindPoses;

                //objRenderer.sharedMesh.boneWeights = totalBoneWeight;

                //objRenderer.bones = totalBones;

                //objRenderer.sharedMesh.RecalculateNormals();
                //objRenderer.sharedMesh.RecalculateBounds();

                //objRenderer.enabled = true;

            }
        }
    }


    static Component[] revertComponent(Component[] comp)
    {
        Component[] result = new Component[comp.Length];
        int x = 0;
        for (int i = comp.Length - 1; i >= 0; i--)
        {
            result[x++] = comp[i];
        }

        return result;
    }


    static BoneWeight recalculateIndexes(BoneWeight bw, Hashtable boneHash, Transform[] meshBones)
    {
        BoneWeight retBw = bw;
        retBw.boneIndex0 = (int)boneHash[meshBones[bw.boneIndex0].name];
        retBw.boneIndex1 = (int)boneHash[meshBones[bw.boneIndex1].name];
        retBw.boneIndex2 = (int)boneHash[meshBones[bw.boneIndex2].name];
        retBw.boneIndex3 = (int)boneHash[meshBones[bw.boneIndex3].name];
        return retBw;
    }
}