using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Sita/Sembunyikan item dari map segera!
            StageManager.instance.SembunyikanItemDiMap();

            // Perintahkan BattleSystem untuk menyiapkan HP & Soal dulu
            if (BattleSystem.instance != null && StageManager.instance != null)
            {
                // Ambil jumlah item terambil saat ini dari StageManager untuk dikirim sebagai HP
                int totalItem = StageManager.instance.listItemTerambil.Count;
                BattleSystem.instance.PersiapkanBattle(totalItem);
            }

            // 2. Siapkan dan mulai battle
            BattleSystem.instance.MulaiBattle();
            
            // 3. Munculkan UI HP dan Arena
            StageManager.instance.MunculkkanBattleArena();
            StageManager.instance.MunculkanHP();
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StageManager.instance.SembunyikanBattleUI();
        }
    }
}