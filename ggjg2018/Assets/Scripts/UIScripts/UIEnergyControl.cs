using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnergyControl : MonoBehaviour
{
    public Text energyText;

    public Slider energyBar;

    public int maxEnergy = 10;
    public int usedEnergy = 1;

    void Start()
    {
        energyText.text = usedEnergy + " / " + maxEnergy;
        energyBar.maxValue = maxEnergy;
        energyBar.value = usedEnergy;
    }

    void Update()
    {
        energyBar.maxValue = maxEnergy;

        if (Input.GetKeyDown("]"))
        {
            increaseMaxEnergy(1);
        }
        if (Input.GetKeyDown("["))
        {
            decreaseMaxEnergy(1);
        }
        if (Input.GetKeyDown("0"))
        {
            increaseUsedEnergy(1);
        }
        if (Input.GetKeyDown("9"))
        {
            decreaseUsedEnergy(1);
        }
    }

    public void increaseMaxEnergy(int value)
    {
        maxEnergy = maxEnergy + value;
        energyText.text = usedEnergy + " / " + maxEnergy;
        energyBar.value = usedEnergy;
    }

    public void decreaseMaxEnergy(int value)
    {
        maxEnergy = maxEnergy - value;
        energyText.text = usedEnergy + " / " + maxEnergy;
        energyBar.value = usedEnergy;
    }

    public void increaseUsedEnergy(int value)
    {
        usedEnergy = usedEnergy + value;
        energyText.text = usedEnergy + " / " + maxEnergy;
        energyBar.value = usedEnergy;
    }

    public void decreaseUsedEnergy(int value)
    {
        usedEnergy = usedEnergy - value;
        energyText.text = usedEnergy + " / " + maxEnergy;
        energyBar.value = usedEnergy;
    }
}
