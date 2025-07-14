using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameHeroManager : BasicSingleton<InGameHeroManager>
{
    [SerializeField] private List<HeroControl> SummonedHeroList;

    public static HeroControl FindNearTarget(Vector3 pos)
    {
        HeroControl select = Instance.SummonedHeroList[0]; //���� �����
        var distance = Vector2.Distance(pos, Instance.SummonedHeroList[0].CenterPoint.position);

        HeroControl compare;
        int count = Instance.SummonedHeroList.Count;
        for (int i = 1; i < count; i++)
        {
            compare = Instance.SummonedHeroList[i];
            if (compare.IsHeroActive()) //����ִ� ��츸 ��ġ ����
            {
                var HeroDistance = Vector2.Distance(pos, compare.CenterPoint.position);
                if (HeroDistance < distance)
                {
                    distance = HeroDistance;
                    select = compare;
                }
            }
        }
        return select;
    }

    public static bool IsHeroActive() => Instance.SummonedHeroList.Exists(hero => hero.IsHeroActive());
  

    //��ȯó��
    public void SummonHero()
    {
        var newHero = HeroPoolManager.Instance.Pop(EPoolType.Hero);
        var heroComp = newHero.GetComponent<HeroControl>();

        heroComp.Init(SummonedHeroList.Count);

        // ���� ��ġ �� �ε��� ����
        heroComp.transform.position =  PositionInfo.Instance.HeroPos[SummonedHeroList.Count].position;


        
        SummonedHeroList.Add(heroComp);
    }
}
