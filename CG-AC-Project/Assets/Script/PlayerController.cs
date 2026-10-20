using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveDistance = 1.0f;  // 1マスの距離（ブロック0.9 + 隙間0.1）
    [SerializeField] private float moveDuration = 0.4f;  // 移動にかかる時間（秒）
    [SerializeField] private float rotateDuration = 0.3f; // 回転にかかる時間（秒）

    // SequenceManager から呼ばれるコマンド実行処理
    public IEnumerator ExecuteCommand(string command)
    {
        switch (command)
        {
            case "1歩進む":
                yield return MoveForward();
                break;

            case "右を向く":
                yield return Rotate(90f);
                break;

            case "左を向く":
                yield return Rotate(-90f);
                break;
        }
    }

    // 1マスなめらかに前進する
    // 1マスなめらかに前進する
    // 1マスなめらかに前進する
    // 1マスなめらかに前進する
    private IEnumerator MoveForward()
    {
        // プレイヤーの位置そのまま（Y=0.7）、またはわずかに上げる程度にする
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        float rayDistance = moveDistance;

        // シーン画面に判定線（赤色）を表示
        Debug.DrawRay(rayOrigin, transform.forward * rayDistance, Color.red, 1.0f);

        // LayerMaskを作成し、"Player"レイヤーを除外する
        int playerLayerIndex = LayerMask.NameToLayer("Player");
        if (playerLayerIndex != -1)
        {
            int playerLayerMask = 1 << playerLayerIndex;
            if (Physics.Raycast(rayOrigin, transform.forward, out RaycastHit hit, rayDistance, ~playerLayerMask))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    Debug.Log("壁に阻まれました！");
                    yield break; // 壁があるので前進をキャンセル
                }
            }
        }

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + transform.forward * moveDistance;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
    }

    // なめらかに90度回転する（正の値で右回転、負の値で左回転）
    private IEnumerator Rotate(float angle)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, angle, 0);
        float elapsedTime = 0f;

        while (elapsedTime < rotateDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / rotateDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
    }
}