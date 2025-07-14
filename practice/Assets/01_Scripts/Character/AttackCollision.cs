using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class AttackCollision : MonoBehaviour
{
    [SerializeField] CharacterBase Attacker;    // �����ϴ� ��ü
    [SerializeField] private TargetType TargetTag;
    private enum TargetType { Enemy, Hero };
    [HideInInspector] public bool singleTargetforEnemy = false;

    private void OnEnable()
    {
        if (TargetTag == TargetType.Hero) { singleTargetforEnemy = false; } //������ ��� ����Ÿ�ٸ� �ߵ��ϵ��� �ϴ� ������
    }


    private void OnTriggerEnter(Collider collision)
    {
        GiveDamage(collision);
    }

    protected virtual void GiveDamage(Collider collision)
    {
        AttackInfo AttackInfo = new AttackInfo();
        AttackInfo.AttackerWorldPosition = Attacker.transform.position;

        if (collision.CompareTag(TargetTag.ToString()) == false)
            return;

        switch (TargetTag)
        {
            // �÷��̾� -> ��
            case TargetType.Enemy:

                // �⺻ ����
                //AttackInfo.Damage = StatManager.Instance.FinalAttack_Melee;
                AttackInfo.Damage = Random.Range(1, 10);    // ���� ���� �� �ӽ� ���ݷ�
                AttackInfo.AttackType = EAttackType.Normal;
                AttackInfo.AttackerType = EAttackerType.Hero;

                // ġ��Ÿ üũ
                //float CriRandom = Random.Range(0, 100f);
                //float PlayerCri = StatManager.Instance.FinalCriticalRate;
                //if (CriRandom < PlayerCri)
                //{
                //    AttackInfo.Damage = AttackInfo.Damage * StatManager.Instance.FinalCritical;
                //    AttackInfo.AttackType = EAttackType.Critical;
                //}

                //AttackInfo.EffectType = EFXPoolType.HitEffect_Orange;
                AttackInfo.HitCount = 1;

                EnemyControl Target = collision.gameObject.GetComponent<EnemyControl>();

                Target.TakeHit(AttackInfo);

                break;
            // �� -> �÷��̾�
            case TargetType.Hero:
                EnemyControl Enemy = Attacker.GetComponent<EnemyControl>();

                if (!Enemy.Target) { return; } //������ ���̿� �׾ target�� ���ŵȰ�� return
                if (Enemy.Target != collision.GetComponent<CharacterBase>()) { return; } //Ÿ�ٸ� ������
                if (singleTargetforEnemy) { return; } //�״¼��� �ٸ� Ÿ������ ��ȯ�Ǿ� ����Ÿ�ݵǴ°� ����

                AttackInfo.Damage = Enemy.Info.AttackPower;
                AttackInfo.AttackType = EAttackType.Normal;
                //AttackInfo.EffectType = EFXPoolType.HitEffect_White;
                AttackInfo.HitCount = 1;

                Enemy.Target.TakeHit(AttackInfo);
                singleTargetforEnemy = true;

                break;
            default:
                break;

            
        }
    }

    // ���� ������Ʈ �Ҹ�
    protected virtual void PushObject()
    {

    }
}
