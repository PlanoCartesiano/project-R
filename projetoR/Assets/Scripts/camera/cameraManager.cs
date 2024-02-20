using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class cameraManager : MonoBehaviour
{
    public static cameraManager instance;

    [SerializeField] private CinemachineCamera[] _allCameras;

    [Header("Controls for lerping Y damping during player fall")]
    [SerializeField] private float _fallPanAmount = 0.25f;
    [SerializeField] private float _fallPanTime = 0.35f;
    public float _fallSpeedYDampingChangeThreshold = -0.05f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine _lerpYPanCoroutine;

    private CinemachineCamera _currentCamera;
    private CinemachinePositionComposer _positionComposer;

    private float _normYPanAmount;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        for (int i = 0; i < _allCameras.Length; i++)
        {
            if (_allCameras[i].enabled)
            {
                _currentCamera = _allCameras[i];

                _positionComposer = _currentCamera.GetComponent<CinemachinePositionComposer>();
            }
        }

        //_normYPanAmount = _positionComposer.Damping.y;
    }

    #region Lerp the Y Damping

    public void LerpYDamping(bool isPlayerFalling)
    {
        _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        float startDampAmount = _positionComposer.Damping.y;
        float endDampAmount = 0.25f;

        if (isPlayerFalling)
        {
            endDampAmount = _fallPanAmount;
            LerpedFromPlayerFalling = true;
        }
        else
        {
            endDampAmount = _normYPanAmount;
        }

        float elapsedTime = 0f;
        while(elapsedTime < _fallPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmout = Mathf.Lerp(endDampAmount, startDampAmount, (elapsedTime / _fallPanTime));
            _positionComposer.Damping.y = lerpedPanAmout;

           // yield return null;
        }

        yield return null;

        IsLerpingYDamping = false;
        //_positionComposer.Damping.y = 2f;
    }

    #endregion
}
