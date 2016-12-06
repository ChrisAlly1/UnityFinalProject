using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using powerUp = FirstPersonController.PowerUp;

public class Inventory : MonoBehaviour {
    private const int SIZE = 5;

    private RawImage[] inventory = new RawImage[SIZE];
    public Texture[] powerups;
    public Texture border;
    private powerUp[] currentInventory = new powerUp[SIZE];
    private Dictionary<powerUp, Texture> textureList = new Dictionary<powerUp, Texture>();

    private int numberPowerups;

    void Start() {
        inventory[0] = GameObject.FindGameObjectWithTag("Inventory1").GetComponent<RawImage>();
        inventory[1] = GameObject.FindGameObjectWithTag("Inventory2").GetComponent<RawImage>();
        inventory[2] = GameObject.FindGameObjectWithTag("Inventory3").GetComponent<RawImage>();
        inventory[3] = GameObject.FindGameObjectWithTag("Inventory4").GetComponent<RawImage>();
        inventory[4] = GameObject.FindGameObjectWithTag("Inventory5").GetComponent<RawImage>();

        textureList.Add(powerUp.NONE, border);
        textureList.Add(powerUp.FAST_MOVE, powerups[0]);
        textureList.Add(powerUp.DOUBLE_DAMAGE, powerups[1]);
        textureList.Add(powerUp.FAST_FIRE, powerups[2]);
        textureList.Add(powerUp.JUMP_HIGH, powerups[3]);
        textureList.Add(powerUp.INVINCIBLE, powerups[4]);

        for (int x = 0; x < 5; x++) {
            currentInventory[x] = powerUp.NONE;
        }

        numberPowerups = 0;
    }

    void Update() {
        for (int x = 0; x < 5; x++) {
            inventory[x].texture = textureList[currentInventory[x]];
        }
    }

    public void AddPowerUp(powerUp newPower) {
        if (numberPowerups != 5) {
            int spot = 0;
            for (int x = 0; x < 5; x++) {
                if (currentInventory[x] == powerUp.NONE) {
                    spot = x;
                    break;
                }
            }

            currentInventory[spot] = newPower;
            numberPowerups++;
        }
    }

    public void RemovePowerUp(int spot) {
        if (numberPowerups != 0) {
            currentInventory[spot - 1] = powerUp.NONE;
            numberPowerups--;
        }
    }

    public powerUp getPowerUp(int spot) {
        return currentInventory[spot - 1];
    }
}