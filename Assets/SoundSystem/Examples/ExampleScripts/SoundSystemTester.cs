using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;

public class SoundSystemTester : MonoBehaviour
{
    [SerializeField] MusicEvent _songA;
    [SerializeField] MusicEvent _songB;
    [SerializeField] MusicEvent _songC;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _songA.Play(8f);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            _songB.Play(8f);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            _songC.Play(8f);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MusicManager.Instance.DecreaseLayerIndex(5);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MusicManager.Instance.IncreaseLayerIndex(5);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MusicManager.Instance.StopMusic(1);
        }
    }
}
