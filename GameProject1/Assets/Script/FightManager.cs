using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    public GameObject WoodSword;
    public GameObject StoneSword;
    public GameObject IronSword;
    public GameObject Boss1Card;
    public GameObject Boss2Card;
    public GameObject Enemy1Card;
    public GameObject Enemy2Card;
    public GameObject Enemy3Card;
    int t;
    int u;
    // Start is called before the first frame update
    void Start()
    {
        t = -5;
        u = 0;
        for (int i = 0; i < DataController.instance.gameData.WoodSwordCount; i++)
        {
            if (t <= 5)
            {
                GameObject _Card1 = Instantiate(WoodSword, new Vector3(t, u, 0), Quaternion.identity);
                t++;
            }
            else
            {
                u = -1;
                if (t > 5) t = -5;
                GameObject _Card1 = Instantiate(WoodSword, new Vector3(t, u, 0), Quaternion.identity);
                t++;
            }
        }
        for (int i = 0; i < DataController.instance.gameData.StoneSwordCount; i++)
        {
            if (t <= 5)
            {
                GameObject _Card1 = Instantiate(StoneSword, new Vector3(t, u, 0), Quaternion.identity);
                t++;
            }
            else
            {
                u = -1;
                if (t > 5) t = -5;
                GameObject _Card1 = Instantiate(StoneSword, new Vector3(t, u, 0), Quaternion.identity);
                t++;
            }
        }
        for (int i = 0; i < DataController.instance.gameData.IronSwordCount; i++)
        {
            if (t <= 5)
            {
                GameObject _Card1 = Instantiate(IronSword, new Vector3(t, u, 0), Quaternion.identity);
                t++;
            }
            else
            {
                u = -1;
                if (t > 5) t = -5;
                GameObject _Card1 = Instantiate(IronSword, new Vector3(t, u, 0), Quaternion.identity);
                t++;
            }
        }

        if (DataController.instance.gameData.Stage == 1 && DataController.instance.gameData.BossStage == true)
        {
            GameObject _Card1 = Instantiate(Boss1Card, new Vector3(0, 3, 0), Quaternion.identity);

            for (int i = 0; i < 2; i++)
            {
                GameObject _Card2 = Instantiate(Enemy1Card, new Vector3((i * 3) - 3, 1, 0), Quaternion.identity);
            }
            GameObject _Card3 = Instantiate(Enemy2Card, new Vector3(3, 1, 0), Quaternion.identity);
        }
        else if (DataController.instance.gameData.Stage == 1 && DataController.instance.gameData.BossStage == false)
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject _Card1 = Instantiate(Enemy1Card, new Vector3((i * 2) - 4, 1, 0), Quaternion.identity);
            }
            for (int i = 0; i < 2; i++)
            {
                GameObject _Card2 = Instantiate(Enemy2Card, new Vector3((i * 2), 1, 0), Quaternion.identity);
            }
        }
        else if (DataController.instance.gameData.Stage == 2 && DataController.instance.gameData.BossStage == true)
        {
            GameObject _Card1 = Instantiate(Boss2Card, new Vector3(0, 3, 0), Quaternion.identity);

            for (int i = 0; i < 2; i++)
            {
                GameObject _Card2 = Instantiate(Enemy2Card, new Vector3((i * 3) - 3, 1, 0), Quaternion.identity);
            }
            GameObject _Card3 = Instantiate(Enemy3Card, new Vector3(3, 1, 0), Quaternion.identity);
        }
        else if (DataController.instance.gameData.Stage == 2 && DataController.instance.gameData.BossStage == false)
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject _Card1 = Instantiate(Enemy2Card, new Vector3((i * 2) - 4, 1, 0), Quaternion.identity);
            }
            for (int i = 0; i < 2; i++)
            {
                GameObject _Card2 = Instantiate(Enemy3Card, new Vector3((i * 2), 1, 0), Quaternion.identity);
            }
        }

        DataController.instance.gameData.Attack = true;

    }

    // Update is called once per frame
    void Update()
    {
        Fight();
    }

    void Fight()
    {
        int rand = Random.Range(0, 20);

        int attackrand = Random.Range(0, 5);

        if (rand < 16 && DataController.instance.gameData.Attack == true) //player attack
        {
            DataController.instance.gameData.PlayerAttack = true;
        }
        else if(rand >= 16 && DataController.instance.gameData.Attack == true)//Enemy1Card attack
        {
            DataController.instance.gameData.EnemyAttack = true;
        }
    }

}
