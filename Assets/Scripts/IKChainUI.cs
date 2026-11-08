using UnityEngine;
using UnityEngine.UI;

public class IKChainUI : MonoBehaviour
{
    public IKChain ikChain;
    public InputField jointInput;

    public void OnGenerateButtonClick()
    {
        if (int.TryParse(jointInput.text, out int num))
        {
            // 限制范围，防止输入非法数字
            if (num >= 2 && num <= 10)
            {
                ikChain.jointCount = num;
                ikChain.GenerateIKChain();
            }
        }
    }
}