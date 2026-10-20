using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SequenceManager : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Transform sequenceArea;
    [SerializeField] private GameObject blockPrefab;

    [Header("Player Reference")]
    [SerializeField] private PlayerController player; // 3D空間のプレイヤー

    private List<string> instructionSequence = new List<string>();
    private bool isExecuting = false; // 実行中フラグ

    public void AddMoveBlock()
    {
        if (isExecuting) return;
        CreateBlock("1歩進む");
    }

    public void AddTurnRightBlock()
    {
        if (isExecuting) return;
        CreateBlock("右を向く");
    }

    public void AddTurnLeftBlock()
    {
        if (isExecuting) return;
        CreateBlock("左を向く");
    }

    // ブロック生成の共通処理
    // ブロック生成の共通処理
    private void CreateBlock(string text)
    {
        if (blockPrefab == null || sequenceArea == null) return;

        // SequenceAreaの子要素として新しいブロックを生成
        GameObject newBlock = Instantiate(blockPrefab, sequenceArea);

        // 生成したブロックのテキストを書き換え
        TMP_Text tmpText = newBlock.GetComponentInChildren<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = text;
        }

        // リストに命令を追加
        instructionSequence.Add(text);

        // ボタンコンポーネントを取得
        UnityEngine.UI.Button btn = newBlock.GetComponent<UnityEngine.UI.Button>();
        if (btn != null)
        {
            // ★【重要】プレハブに残っている古い「On Click (AddMoveBlock)」を消去！
            btn.onClick.RemoveAllListeners();

            // 個別削除イベントだけを登録
            btn.onClick.AddListener(() => RemoveBlock(newBlock));
        }
    }

    // 【追加】タップされた特定のブロックを個別削除する処理
    private void RemoveBlock(GameObject block)
    {
        // 実行中は削除させない
        if (isExecuting) return;

        // 青エリア内での順番（何番目の子要素か）を取得してリストから削除
        int index = block.transform.GetSiblingIndex();
        if (index >= 0 && index < instructionSequence.Count)
        {
            instructionSequence.RemoveAt(index);
        }

        // ゲームオブジェクト（見た目）を削除
        Destroy(block);
    }

    // 【追加】実行ボタンが押された時の処理
    public void OnClickExecute()
    {
        if (isExecuting || instructionSequence.Count == 0) return;
        StartCoroutine(ExecuteRoutine());
    }

    // 命令を1つずつ順番に実行するコルーチン
    private IEnumerator ExecuteRoutine()
    {
        isExecuting = true;

        foreach (string command in instructionSequence)
        {
            if (player != null)
            {
                // プレイヤーに命令を送って動作完了を待つ
                yield return player.ExecuteCommand(command);
            }
            yield return new WaitForSeconds(0.2f); // コマンド間の少しの間隔
        }

        isExecuting = false;
    }

    // 【追加】リセット（クリア）機能
    public void ClearSequence()
    {
        if (isExecuting) return;

        instructionSequence.Clear();
        foreach (Transform child in sequenceArea)
        {
            Destroy(child.gameObject);
        }
    }
}