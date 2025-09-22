using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private CharacterData data;

    /// <summary>
    /// 매 턴이 시작할 때 실행
    /// </summary>
    public  void InitTurn()
    {
        RecoveryActionPoint();
    }

    public  void OnDamaged(uint damage)
    {
        data.Hp -= CalcDamage(damage);
        if(data.Hp < 0)
        {
            data.Hp = 0;
            OnDie();
        }
    }

    public virtual void OnDie()
    {

    }
    private void RecoveryActionPoint()
    {
        data.ActionPoint += data.Speed;
        Mathf.Clamp(data.ActionPoint, 0, 10);
    }
    private uint CalcDamage(uint damage) => damage - data.Defense;
}
