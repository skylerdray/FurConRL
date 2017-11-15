using RogueSharp;
using RLNET;

namespace FurryConventionRL.Core
{
    public class DungeonMap : Map
    {
        public void Draw( RLConsole mapConsole )
        {
            mapConsole.Clear();
            foreach ( Cell cell in GetAllCells() )
            {
                SetConsoleSymbolForCell( mapConsole, cell );
            }
        }

        private void SetConsoleSymbolForCell( RLConsole console, Cell cell )
        {
            // when we haven't seen the cell, we don't want to draw it
            if ( !cell.IsExplored)
            {
                return;
            }

            // things currently in FOV should be lighter
            if ( IsInFov( cell.X, cell.Y ) )
            {
                // . for walkable tiles
                if (cell.IsWalkable) 
                {
                    console.Set(cell.X, cell.Y, Colors.FloorFov, Colors.FloorBackgroundFov, '.');
                }
                // # for walls
                else
                {
                    console.Set(cell.X, cell.Y, Colors.WallFov, Colors.WallBackgroundFov, '#');
                }

            }
            // outside FoV make it darker
            else
            {
                if (cell.IsWalkable)
                {
                    console.Set(cell.X, cell.Y, Colors.Floor, Colors.FloorBackground, '.');
                }
                else
                {
                    console.Set(cell.X, cell.Y, Colors.Wall, Colors.WallBackground, '#');
                }
            }
        }

        public void UpdatePlayerFieldOfView()
        {
            Player player = Game.Player;
            ComputeFov(player.X, player.Y, player.Awareness, true);

            foreach ( Cell cell in GetAllCells() )
            {
                if ( IsInFov( cell.X, cell.Y ) )
                {
                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                }
            }
        }

        public bool SetActorPosition( Actor actor, int x, int y)
        {
            if ( GetCell( x, y ).IsWalkable )
            {
                SetIsWalkable(actor.X, actor.Y, true);
                // Update the actor's position
                actor.X = x;
                actor.Y = y;
                // The new cell the actor is on is now not walkable
                SetIsWalkable(actor.X, actor.Y, false);
                // Don't forget to update the field of view if we just repositioned the player
                if (actor is Player)
                {
                    UpdatePlayerFieldOfView();
                }
                return true;
            }
            return false;
        }

        public void SetIsWalkable ( int x, int y, bool isWalkable )
        {
            Cell cell = GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
        }
    }
}
