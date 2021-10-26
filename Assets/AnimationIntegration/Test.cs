using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform targetBone;
    public Vector3 rot;
    
    public void LateUpdate()
    {
        targetBone.localEulerAngles = rot;
    }
}
