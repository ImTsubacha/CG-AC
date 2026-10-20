using UnityEngine;
using WiimoteApi;

public class OrbitalCamera : MonoBehaviour
{
    [Header("ターゲット設定")]
    [SerializeField] private Transform target; // ステージの中心点
    [SerializeField] private float distance = 7.0f;  // 中心からの距離（3x3用に調整）
    [SerializeField] private float height = 5.0f;    // カメラの高さ（3x3用に調整）

    [Header("操作設定")]
    [SerializeField] private float rotateSpeed = 100.0f; // 回転速度

    private float currentAngle = 0.0f;
    private Wiimote wiimote;

    void Start()
    {
        WiimoteManager.FindWiimotes();
        if (WiimoteManager.HasWiimote())
        {
            wiimote = WiimoteManager.Wiimotes[0];
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. 入力の取得（キーボードの左右キー / Wiiリモコン十字キー）
        float inputHorizontal = Input.GetAxis("Horizontal");

        if (wiimote == null && WiimoteManager.HasWiimote())
        {
            wiimote = WiimoteManager.Wiimotes[0];
        }

        if (wiimote != null)
        {
            int ret;
            do { ret = wiimote.ReadWiimoteData(); } while (ret > 0);

            if (wiimote.Button.d_left) inputHorizontal = -1f;
            if (wiimote.Button.d_right) inputHorizontal = 1f;
        }

        // 2. 角度の更新
        currentAngle += inputHorizontal * rotateSpeed * Time.deltaTime;

        // 3. ターゲット中心の回転計算
        Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
        Vector3 negDistance = new Vector3(0, height, -distance);
        Vector3 position = rotation * negDistance + target.position;

        transform.position = position;
        transform.LookAt(target.position);
    }
}