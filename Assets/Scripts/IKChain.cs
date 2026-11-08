using UnityEngine;
using System.Collections.Generic;

public class IKChain : MonoBehaviour
{
    [Header("IK 参数设置")]
    public int jointCount = 3;
    public float segmentLength = 1.0f;
    public int iterationCount = 15;
    public float threshold = 0.01f;

    [Header("拖拽目标小球")]
    public Transform target;

    [Header("加分：开启关节角度限制（推荐打开）")]
    public bool useAngleLimit = true;
    public float maxAngle = 60f;

    private List<Transform> joints = new List<Transform>();
    private Transform endEffector;

    void Start()
    {
        GenerateIKChain();
    }

    // 动态生成关节链，UI按钮调用这个函数
    public void GenerateIKChain()
    {
        // 删除上一次生成的旧关节
        foreach (var j in joints)
        {
            Destroy(j.gameObject);
        }
        joints.Clear();

        Transform last = transform; // Root，固定不动

        for (int i = 0; i < jointCount; i++)
        {
            GameObject jointObj = new GameObject($"Joint_{i}");
            jointObj.transform.parent = last;
            jointObj.transform.localPosition = new Vector3(segmentLength, 0, 0);
            joints.Add(jointObj.transform);
            last = jointObj.transform;
        }
        // 创建末端
        GameObject endObj = new GameObject("EndEffector");
        endObj.transform.parent = last;
        endObj.transform.localPosition = new Vector3(segmentLength, 0, 0);
        endEffector = endObj.transform;
    }

    void Update()
    {
        if (target == null || joints.Count == 0)
            return;
        CCDIKSolve();
    }

    void CCDIKSolve()
    {
        for (int iter = 0; iter < iterationCount; iter++)
        {
            // 从最后一个关节向前迭代（CCD核心）
            for (int i = joints.Count - 1; i >= 0; i--)
            {
                Transform currentJoint = joints[i];
                Vector3 endPos = endEffector.position;
                Vector3 targetPos = target.position;

                Vector3 toEnd = endPos - currentJoint.position;
                Vector3 toTarget = targetPos - currentJoint.position;

                Quaternion rot = Quaternion.FromToRotation(toEnd, toTarget);
                Quaternion newRot = rot * currentJoint.rotation;

                // 加分：角度限制，防止关节疯狂翻转
                if (useAngleLimit)
                {
                    float angleDiff = Quaternion.Angle(currentJoint.rotation, newRot);
                    if (angleDiff > maxAngle)
                    {
                        newRot = Quaternion.RotateTowards(currentJoint.rotation, newRot, maxAngle);
                    }
                }
                currentJoint.rotation = newRot;

                float dist = Vector3.Distance(endEffector.position, target.position);
                if (dist < threshold)
                    return;
            }
        }
    }

    // 绘制骨骼连线，可视化，方便写报告截图
    void OnDrawGizmos()
    {
        if (joints == null || joints.Count == 0 || endEffector == null)
            return;
        Gizmos.color = Color.green;
        Vector3 prePos = transform.position;
        foreach (var j in joints)
        {
            Gizmos.DrawLine(prePos, j.position);
            prePos = j.position;
        }
        Gizmos.DrawLine(prePos, endEffector.position);
    }
}