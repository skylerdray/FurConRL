using FurryConventionRL.Core;
using RogueSharp.DiceNotation;

namespace FurryConventionRL.Furries
{
    public class Fox : Furry
    {
        public static Fox Create( int level )
        {
            int health = Dice.Roll("2D5");
            return new Fox
            {
                Attack = Dice.Roll("1D3") + level / 3,
                AttackChance = Dice.Roll("25D3"),
                Awareness = 10,
                Color = Colors.FoxColor,
                Defense = Dice.Roll("1D3") + level / 3,
                DefenseChance = Dice.Roll("10D4"),
                Gold = Dice.Roll("5D5"),
                Health = health,
                MaxHealth = health,
                Name = "Fox",
                Speed = 14,
                Symbol = 'f'
            };
        }
    }
}
