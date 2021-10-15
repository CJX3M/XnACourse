using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace GameProject
{
    /// <summary>
    /// All the constants used in the game
    /// </summary>
    public static class GameConstants
    {
        // resolution
        public const int WindowWidth = 800;
        public const int WindowHeight = 600;

        // projectile characteristics
        public const float TeddyBearProjectileSpeed = 0.3f;
        public const int TeddyBearProjectileDamage = 5;
        public const int TeddyBearProjectileOffset = 20;
        public const float FrenchFriesProjectileSpeed = 0.4f;
        public const int FrenchFriesProjectileDamage = 5;
        public const int FrenchFriesProjectileOffset = 10;

        // bear characteristics
        public const int MaxBears = 5;
        public const int BearPoints = 10;
        public const int BearDamage = 10;
        public const float MinBearSpeed = 0.1f;
        public const float BearSpeedRange = 0.2f;
        public const int BearMinFiringDelay = 500;
        public const int BearFiringRateRange = 1000;

        // burger characteristics
        public const int BurgerInitialHealth = 100;
        public const int BurgerMovementAmount = 10;
        public const int BurgerTotalCooldownMilliseconds = 500;

        // explosion hard-coded animation info. There are better
        // ways to do this, we just don't know enough to use them yet
        public const int ExplosionFramesPerRow = 3;
        public const int ExplosionNumRows = 3;
        public const int ExplosionNumFrames = 9;
        public const int ExplosionTotalFrameMilliseconds = 10;

        // display support
        const int DisplayOffset = 35;
        public const string ScorePrefix = "Score: ";
        public static readonly Vector2 ScoreLocation =
            new Vector2(DisplayOffset, DisplayOffset);
        public const string HealthPrefix = "Health: ";
        public static readonly Vector2 HealthLocation =
            new Vector2(DisplayOffset, 2 * DisplayOffset);

        // spawn location support
        public const int SpawnBorderSize = 100;
    }
}
