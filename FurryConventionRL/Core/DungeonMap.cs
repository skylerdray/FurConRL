using RogueSharp;
using RLNET;
using System.Collections.Generic;

namespace FurryConventionRL.Core
{
    public class DungeonMap : Map
    {
        public List<Rectangle> Rooms;

        private readonly List<Furry> _furries;

        public DungeonMap()
        {
            _furries = new List<Furry>();
            Rooms = new List<Rectangle>();
        }
        public void Draw( RLConsole mapConsole )
        {
            mapConsole.Clear();
            foreach ( Cell cell in GetAllCells() )
            {
                SetConsoleSymbolForCell( mapConsole, cell );
            }
            foreach ( Furry furry in _furries)
            {
                furry.Draw(mapConsole, this);
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
        public void AddPlayer(Player player)
        {
            Game.Player = player;
            SetIsWalkable(player.X, player.Y, false);
            UpdatePlayerFieldOfView();
        }

        public void AddFurry( Furry furry )
        {
            _furries.Add(furry);

            SetIsWalkable(furry.X, furry.Y, false);
        }

        public Point GetRandomWalkableLocationInRoom(Rectangle room)
        {
            if (DoesRoomHaveWalkableSpace(room))
            {
                for (int i = 0; i < 100; i++)
                {
                    int x = Game.Random.Next(1, room.Width - 2) + room.X;
                    int y = Game.Random.Next(1, room.Height - 2) + room.Y;
                    if (IsWalkable(x, y))
                    {
                        return new Point(x, y);
                    }
                }
            }

            // If we didn't find a walkable location in the room return null
            return null;
        }

        public bool DoesRoomHaveWalkableSpace(Rectangle room)
        {
            for (int x = 1; x <= room.Width - 2; x++)
            {
                for (int y = 1; y <= room.Height - 2; y++)
                {
                    if (IsWalkable(x + room.X, y + room.Y))
                    {
                        return true;
                    }
                }
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
