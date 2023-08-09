using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerVfxManager : MonoBehaviour
{
    public VisualEffect footStep;
    public VisualEffect slash;
    public VisualEffect heal;

    public ParticleSystem blade01;
    public ParticleSystem blade02;
    public ParticleSystem blade03;

    public void UpdateFootStep(bool state)
    {
        if (state)
            footStep.Play();
        else
            footStep.Stop();
    }

    public void PlayBlade01()
    {
        blade01.Play();
    }

    public void PlayBlade02()
    {
        blade02.Play();
    }

    public void PlayBlade03()
    {
        blade03.Play();
    }

    public void StopBlade()
    {
        blade01.Simulate(0);
        blade01.Stop();

        blade02.Simulate(0);
        blade02.Stop();

        blade03.Simulate(0);
        blade03.Stop();
    }

    public void PlaySlash(Vector3 pos)
    {
        slash.transform.position = pos;

        slash.Play();
    }

    public void PlayHealVFX()
    {
        heal.Play();
    }
}
