using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CameraMode
{
    Stage,  // �������� ���
    Lobby   // �κ� ���
}
public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float cameraAngleX = 60f;  // X�� ȸ�� ����
    [Range(5f, 20f)] public float cameraDistance = 11.18f;  // ī�޶�� ��ǥ ������ �Ÿ�
    public CameraMode cameraMode = CameraMode.Stage;  // ���� ī�޶� ���
    private Vector3 targetPositionStage;     // �ٶ󺸴� ��ǥ ��ġ
    private Vector3 targetPositionLobby;     // �ٶ󺸴� ��ǥ ��ġ


    private void Awake()
    {
        targetPositionStage = PositionInfo.Instance.StageCenter.position;
        targetPositionLobby = PositionInfo.Instance.LobbyCenter.position;


        EventManager.Subscribe<CameraMode>(EEventType.CameraChange, UpdateCameraPosition);
    }

    private void OnApplicationQuit()
    {
        EventManager.Unsubscribe<CameraMode>(EEventType.CameraChange, UpdateCameraPosition);
    }

    private void Start()
    {
        UpdateCameraPosition(cameraMode);
    }

    private void UpdateCameraPosition(CameraMode cameraMode)
    {
        // �������� ��ȯ
        float angleInRadian = cameraAngleX * Mathf.Deg2Rad;

        // ������ �Ÿ��� �̿��� Y�� Z ��ġ ���
        float yOffset = cameraDistance * Mathf.Sin(angleInRadian);
        float zOffset = -cameraDistance * Mathf.Cos(angleInRadian);

        Vector3 targetPosition;

        switch (cameraMode)
        {
            case CameraMode.Stage:
                targetPosition = targetPositionStage;
                break;
            case CameraMode.Lobby:
                targetPosition = targetPositionLobby;
                break;
            default:
                targetPosition = Vector3.zero;
                break;
        }
            

        // ī�޶� ��ġ ����
        transform.position = new Vector3(targetPosition.x, yOffset, targetPosition.z + zOffset);

        // ī�޶� ��ǥ���� �ٶ󺸵��� ȸ��
        transform.rotation = Quaternion.Euler(cameraAngleX, 0f, 0f);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateCameraPosition(cameraMode);
    }
#endif
}
