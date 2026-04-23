using UnityEngine;
using UnityEditor;

public class BatchAddConvexColliders : EditorWindow
{
    [MenuItem("Tools/物理工具/为选中物体一键添加Convex碰撞体")]
    public static void AddColliders()
    {
        // 获取选中的所有物体
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
            // 关键：遍历包含 MeshFilter 的子物体，因为碰撞体依赖网格数据
            MeshFilter[] meshFilters = root.GetComponentsInChildren<MeshFilter>(true);

            foreach (MeshFilter mf in meshFilters)
            {
                MeshCollider mc = mf.GetComponent<MeshCollider>();

                if (mc == null)
                {
                    // 如果没有碰撞体，则添加
                    mc = mf.gameObject.AddComponent<MeshCollider>();
                    addedCount++;
                }
                else
                {
                    updatedCount++;
                }

                // 统一配置：关联网格并勾选 Convex
                mc.sharedMesh = mf.sharedMesh;
                mc.convex = true;
                
                // 标记为已修改，确保撤销系统能记录，且场景能保存
                EditorUtility.SetDirty(mc);
            }
        }

        Debug.Log($"处理完成！新增碰撞体：{addedCount} 个，更新配置：{updatedCount} 个。");
    }
}