// Author: mcf
// Editor utility for adding convex MeshColliders to selected model hierarchies.

using UnityEditor;
using UnityEngine;

public class BatchAddConvexColliders : EditorWindow
{
    [MenuItem("Tools/物理工具/为选中物体一键添加Convex碰撞体")]
    public static void AddColliders()
    {
        GameObject[] selections = Selection.gameObjects;

        if (selections.Length == 0)
        {
            Debug.LogWarning("请先在层级面板中选中大模型！");
            return;
        }

        int addedCount = 0;
        int updatedCount = 0;

        foreach (GameObject root in selections)
        {
            MeshFilter[] meshFilters = root.GetComponentsInChildren<MeshFilter>(true);

            foreach (MeshFilter mf in meshFilters)
            {
                MeshCollider mc = mf.GetComponent<MeshCollider>();

                if (mc == null)
                {
                    mc = mf.gameObject.AddComponent<MeshCollider>();
                    addedCount++;
                }
                else
                {
                    updatedCount++;
                }

                mc.sharedMesh = mf.sharedMesh;
                mc.convex = true;
                EditorUtility.SetDirty(mc);
            }
        }

        Debug.Log($"处理完成！新增碰撞体：{addedCount} 个，更新配置：{updatedCount} 个。");
    }
}
