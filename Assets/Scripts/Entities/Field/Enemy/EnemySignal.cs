using UnityEngine;


namespace TurnBased.Entities.Field { 
    
    /// <summary>
    /// ���ʹ� ���ݽ� ��Ʈ�� �ñ׳��� ������ Ŭ����
    /// </summary>
    public class EnemySignal : MonoBehaviour
    {
        // �θ� ������Ʈ���� ������ ���ʹ� ��ũ��Ʈ
        public F_Enemy enemy;

        private void Awake()
        {
            // ���ʹ� ��ũ��Ʈ�� ����������
            if(enemy == null)
                // �θ� ������Ʈ���� ���ʹ� ��ũ��Ʈ�� �����´�
                enemy = GetComponentInParent<F_Enemy>();

            // ���ʹ� ��ũ��Ʈ�� ã�� ������ ���
            if (enemy == null)
                Debug.Log("�θ� ������Ʈ���� F_Enemy�� ã�� ���߽��ϴ�");
        }

        public void Signal()
        {
            // ���ʹ̰� ������� �ʴٸ�
            if (enemy != null)
                // �θ� ������Ʈ�� ���ʹ� ��ũ��Ʈ���� ��Ʈ �ñ׳��� ȣ���Ѵ�
                enemy.hit_signal();

            // ���ʹ̰� ����ִٸ�
            else
                Debug.Log("�θ� enemy�� �����ϴ�");

        }
    }

}