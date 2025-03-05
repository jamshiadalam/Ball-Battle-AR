using UnityEngine;

namespace BallBattleAR
{
    [System.Serializable]
    public class GameParameters
    {
        public int matchesPerGame = 5;
        public float matchTimeLimit = 140f;
        public int energyBarLimit = 6;
        public float energyRegenRate = 0.5f;

        public int attackerEnergyCost = 2;
        public int defenderEnergyCost = 3;

        public float spawnTime = 0.5f;
        public float attackerReactivateTime = 2.5f;
        public float defenderReactivateTime = 4f;

        public float attackerSpeed = 1.5f;
        public float defenderSpeed = 1.0f;
        public float carryingSpeed = 0.75f;
        public float ballSpeed = 1.5f;
        public float returnSpeed = 2.0f;
        public float rotationSpeed = 50f;
        public float detectionRange = 0.35f;
    }
}