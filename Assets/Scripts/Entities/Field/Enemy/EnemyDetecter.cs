using UnityEngine;
using System.Linq;

namespace TurnBased.Entities.Field { 
    
    /// <summary>
    /// �÷��̾ Ž���ϴ� ��ɸ� ����ϴ� Ŭ����
    /// </summary>
    public class EnemyDetector
    {
        /// <summary>
        /// ���� ��ġ���� ������ �������� �ִ� ������Ʈ�� Ư�� �±׸� ���� ������Ʈ���� ù��°�� Ž���Ѵ�
        /// </summary>
        /// <param name="origin">Ž���� ������ ��ġ</param>
        /// <param name="range">Ž�� �� ����</param>
        /// <param name="targetTag">Ž�� �� �±�</param>
        /// <returns>Ž���� GameObejct ���ٸ� null�� ��ȯ</returns>
        public GameObject Detect(Vector3 origin, float range, string targetTag)
        {
            // ���� ���� ���� ��� �ݶ��̴��� Ž��
            Collider[] hits = Physics.OverlapSphere(origin, range);
            
            // hits �迭����, �±װ� targetTag�� ù��° collider�� ���� ������Ʈ�� ��ȯ
            var targetColl = hits.FirstOrDefault(collider => collider.CompareTag(targetTag));

            // �±װ� �´� �ݶ��̴��� �ִٸ� �ش� ���� ������Ʈ�� ��ȯ, ������ null�� ��ȯ
            return targetColl?.gameObject;
        }
      
    }
    

}