using UnityEngine;
using WiimoteApi;

public class WiimoteAllButtonsCheck : MonoBehaviour
{
    private Wiimote wiimote;

    // 前フレームのボタン状態を記録する変数
    private bool lastA, lastB, lastPlus, lastMinus, lastHome, lastOne, lastTwo;
    private bool lastUp, lastDown, lastLeft, lastRight;

    [Header("Aボタン押下中の動作設定")]
    [SerializeField] private float rotateSpeed = 100f; // 回転速度

    void Start()
    {
        WiimoteManager.FindWiimotes();
        if (WiimoteManager.HasWiimote())
        {
            wiimote = WiimoteManager.Wiimotes[0];
            wiimote.SendPlayerLED(true, false, false, false);
            Debug.Log("【接続成功】すべてのボタンの入力待機中...");
        }
        else
        {
            Debug.LogWarning("Wiiリモコンが見つかりません。接続を確認してください。");
        }
    }

    void Update()
    {
        // 未接続時の自動再接続処理
        if (wiimote == null)
        {
            WiimoteManager.FindWiimotes();
            if (WiimoteManager.HasWiimote())
            {
                wiimote = WiimoteManager.Wiimotes[0];
                wiimote.SendPlayerLED(true, false, false, false);
                Debug.Log("【再接続成功】すべてのボタンの入力待機中...");
            }
            return;
        }

        // リモコンデータの読み込み
        int ret;
        do
        {
            ret = wiimote.ReadWiimoteData();
        } while (ret > 0);

        // =========================================================
        // ★【追加】Aボタンを押している「間ずっと」実行される処理
        // =========================================================
        if (wiimote.Button.a)
        {
            // 例: このスクリプトがついているオブジェクトを回転させる
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);

            // ※毎フレーム実行したい処理（移動・チャージ・カメラ操作など）をここに書く
        }

        // --- 各ボタンのトリガー（押した瞬間/離した瞬間）のログ表示 ---

        // メインボタン
        CheckButton("Aボタン", wiimote.Button.a, ref lastA);
        CheckButton("Bボタン", wiimote.Button.b, ref lastB);

        // メニュー系ボタン
        CheckButton("＋ (プラス)", wiimote.Button.plus, ref lastPlus);
        CheckButton("－ (マイナス)", wiimote.Button.minus, ref lastMinus);
        CheckButton("HOMEボタン", wiimote.Button.home, ref lastHome);

        // 数字ボタン
        CheckButton("1ボタン", wiimote.Button.one, ref lastOne);
        CheckButton("2ボタン", wiimote.Button.two, ref lastTwo);

        // 十字キー
        CheckButton("十字キー：上", wiimote.Button.d_up, ref lastUp);
        CheckButton("十字キー：下", wiimote.Button.d_down, ref lastDown);
        CheckButton("十字キー：左", wiimote.Button.d_left, ref lastLeft);
        CheckButton("十字キー：右", wiimote.Button.d_right, ref lastRight);
    }

    // ボタンの「押された瞬間」と「離された瞬間」を判定してログを出す補助関数
    void CheckButton(string buttonName, bool currentStatus, ref bool lastStatus)
    {
        if (currentStatus && !lastStatus)
        {
            Debug.Log($"【DOWN】{buttonName} が押されました！");
        }
        else if (!currentStatus && lastStatus)
        {
            Debug.Log($"【 UP 】{buttonName} が離されました。");
        }
        lastStatus = currentStatus;
    }

    void OnApplicationQuit()
    {
        if (wiimote != null) WiimoteManager.Cleanup(wiimote);
    }
}