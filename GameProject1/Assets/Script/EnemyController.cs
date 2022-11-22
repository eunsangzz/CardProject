using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int hp;
    int damage;
    Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        pos = this.gameObject.transform.position;

        if(this.gameObject.name == "Enemy1(Clone)")
        {
            hp = 3;
            damage = 1;
        }
        else if(this.gameObject.name == "Enemy2(Clone)")
        {
            hp = 2;
            damage = 6;
        }
        else if(this.gameObject.name == "Enemy3(Clone)")
        {
            hp = 8;
            damage = 4;
        }
        else if(this.gameObject.name == "Boss1(Clone)")
        {
            hp = 30;
            damage = 4;
        }
        else if(this.gameObject.name == "Boss2(Clone)")
        {
            hp = 60;
            damage = 3;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(DataController.instance.gameData.EnemyAttack == true) //생성고려, 순서고려 해줘야함
        {
            GameObject player = GameObject.Find("Wood(Clone)");
            this.gameObject.transform.Translate(player.transform.position);
            this.gameObject.transform.Translate(pos);
        }
        if(hp == 0)
        {
            Destroy(this.gameObject);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "WoodSword(Clone)")
        {
            other.gameObject.GetComponent<PlayerController>().hp -= damage;
        }
        if(other.gameObject.name == "StoneSword(Clone")
        {
            other.gameObject.GetComponent<PlayerController>().hp -= damage;
        }
        if(other.gameObject.name == "IronSword(Clone)")
        {
            other.gameObject.GetComponent<PlayerController>().hp -= damage;
        }
    }
}
