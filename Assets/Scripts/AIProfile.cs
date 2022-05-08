using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AI Profile", menuName ="ScriptableObjects/새로운 AI Profile")]
public class AIProfile : ScriptableObject
{
    [Header("공격 성향")]
    public bool targetPlayer = true;
    [Header("수치가 높을 수록 해당 행동을 할 확률이 높아짐")]
    [Header("목표 유닛 가중치")]
    [Range(0f, 1f)] public float closest = 0f;
    [Range(0f, 1f)] public float lowHP = 0f;
    [Range(0f, 1f)] public float highestLevel = 0;
    [Range(0f, 1f)] public float highestAttack = 0;
    public bool changeTarget = true;
    [Header("목표 유닛 특수 가중치")]
    [Range(0f, 1f)] public float warriror = 0f;
    [Range(0f, 1f)] public float witch = 0f;
    [Header("행동 원리")]
    [Range(0f, 1f)] public float meleeAttack = 0f;
    [Range(0f, 1f)] public float rangeAttack = 0f;
    [Range(0f, 1f)] public float magicAttack = 0f;
    [Range(0f, 1f)] public float magicDebuff = 0f;
    [Range(0f, 1f)] public float magicHeal = 0f;
    [Range(0f, 1f)] public float magicBuff = 0f;

}
