using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int hp;
    int damage;
    Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        pos = this.gameObject.transform.position;;

        if(this.gameObject.name == "WoodSword(Clone)")
        {
            hp = 3;
            damage = 2;
        }
        else if(this.gameObject.name == "StoneSword(Clone)")
        {
            hp = 5;
            damage = 4;
        }
        else if(this.gameObject.name == "IronSword(Clone)")
        {
            hp =7;
            damage = 6;
        }
    }

    // Update is called once per frame
    void Update()
    {   
        if(hp == 0)
        {
            Destroy(this.gameObject);
            if(this.gameObject.name == "WoodSword(Clone)") DataController.instance.gameData.WoodSwordCount -=1;
            if(this.gameObject.name == "StoneSword(Clone)") DataController.instance.gameData.StoneSwordCount -=1;
            if(this.gameObject.name == "IronSword(Clone)") DataController.instance.gameData.IronSwordCount -=1;
        } 
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.name == "Boss1(Clone)" && DataController.instance.gameData.PlayerAttack == true)
        {
            DataController.instance.gameData.Boss1Hp -= damage;
            this.gameObject.transform.Translate(pos);
            DataController.instance.gameData.PlayerAttack =false;
        }
        else if(other.name == "Boss2(Clone)" && DataController.instance.gameData.PlayerAttack == true)
        {
            DataController.instance.gameData.Boss2Hp -= damage;
            this.gameObject.transform.Translate(pos);
            DataController.instance.gameData.PlayerAttack =false;
        }
        else if(other.name == "Enemy1(Clone)" && DataController.instance.gameData.PlayerAttack == true)
        {
            other.gameObject.GetComponent<EnemyController>().hp -= damage;
            this.gameObject.transform.Translate(pos);
            DataController.instance.gameData.PlayerAttack =false;
        }
        else if(other.name == "Enemy2(Clone)" && DataController.instance.gameData.PlayerAttack == true)
        {
            other.gameObject.GetComponent<EnemyController>().hp -= damage;
            this.gameObject.transform.Translate(pos);
            DataController.instance.gameData.PlayerAttack =false;
        }
        else if(other.name == "Enemy3(Clone)" && DataController.instance.gameData.PlayerAttack == true)
        {
            other.gameObject.GetComponent<EnemyController>().hp -= damage;
            this.gameObject.transform.Translate(pos);
            DataController.instance.gameData.PlayerAttack =false;
        }
    }
}
