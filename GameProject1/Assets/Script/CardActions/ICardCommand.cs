using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardCommand
{
    int WorkerCost { get; }//일꾼 수
    bool CanExecute(GameData gd);// 자원 확인
    IEnumerator Execute(CardManager cm, GameData gd); // 행동
}
