using FurryConventionRL.Core;
using FurryConventionRL.Systems;
using RLNET;
using RogueSharp.Random;
using System;

namespace FurryConventionRL
{
    class Game
    {
        //screen size in tiles
        private static readonly int _screenWidth = 100;
        private static readonly int _screenHeight = 70;
        private static RLRootConsole _rootConsole;

        //the map takes up most of the screen
        private static readonly int _mapWidth = 70;
        private static readonly int _mapHeight = 50;
        private static RLConsole _mapConsole;

        //stats make up the right side
        private static readonly int _statWidth = 30;
        private static readonly int _statHeight = 70;
        private static RLConsole _statConsole;

        //messages makes the bottom
        private static readonly int _messageWidth = 70;
        private static readonly int _messageHeight = 20;
        private static RLConsole _messageConsole;

        private static bool _renderRequired = true;
        public static Systems.CommandSystem CommandSystem { get; private set; }
        public static Player Player { get; set; }
        public static DungeonMap DungeonMap { get; private set; }
        public static IRandom Random { get; private set; }
        public static void Main()
        {
            string fontFileName = "terminal8x8.png";

            //establish seed for rng
            int seed = (int) DateTime.UtcNow.Ticks;
            Random = new DotNetRandom(seed);

            // Set the console window title
            string consoleTitle = "FurConRL - Seed {seed}";

            //tell RLNet to use the bitmap font
            //  as well as tell it each tile is 8x8px
            _rootConsole = new RLRootConsole(fontFileName, _screenWidth, _screenHeight, 8, 8, 1f, consoleTitle);
            _mapConsole = new RLConsole(_mapWidth, _mapHeight);
            _messageConsole = new RLConsole(_messageWidth, _messageHeight);
            _statConsole = new RLConsole(_statWidth, _statHeight);
            
            CommandSystem = new Systems.CommandSystem();

            MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 25, 10, 7);
            DungeonMap = mapGenerator.CreateMap();

            DungeonMap.UpdatePlayerFieldOfView();
            //handle RLNet's Update event
            _rootConsole.Update += OnRootConsoleUpdate;
            //handle RLNet's Render event
            _rootConsole.Render += OnRootConsoleRender;
            //begin the game loop
            _rootConsole.Run();
        }

        // event handler for Update event
        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {

            bool didPlayerAct = false;
            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();

            if (keyPress != null)
            {
                if (keyPress.Key == RLKey.K)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Core.Direction.Up);
                }
                else if (keyPress.Key == RLKey.J)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Core.Direction.Down);
                }
                else if (keyPress.Key == RLKey.H)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Core.Direction.Left);
                }
                else if (keyPress.Key == RLKey.L)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Core.Direction.Right);
                }
                else if (keyPress.Key == RLKey.Y)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Core.Direction.UpLeft);
                }
                else if (keyPress.Key == RLKey.U)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Core.Direction.UpRight);
                }
                else if (keyPress.Key == RLKey.B)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Core.Direction.DownLeft);
                }
                else if (keyPress.Key == RLKey.N)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Core.Direction.DownRight);
                }
                else if (keyPress.Key == RLKey.Escape)
                {
                    _rootConsole.Close();
                }
            }

            if (didPlayerAct)
            {
                _renderRequired = true;
            }

            //set bg color and text for each console
            _mapConsole.SetBackColor(0, 0, _mapWidth, _mapHeight, Core.Colors.FloorBackground);

            _messageConsole.SetBackColor(0, 0, _messageWidth, _messageHeight, Core.Swatch.DbDeepWater);
            _messageConsole.Print(1, 1, "Messages", Core.Colors.TextHeading);

            _statConsole.SetBackColor(0, 0, _statWidth, _statHeight, Core.Swatch.DbOldStone);
             _statConsole.Print(1, 1, "Stats", Core.Colors.TextHeading);
        }

        // event handler for Render event
        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            // don't redraw if nothing changes!!
            if (_renderRequired)
            {
                DungeonMap.Draw(_mapConsole);
                Player.Draw(_mapConsole, DungeonMap);



                RLConsole.Blit(_mapConsole, 0, 0, _mapWidth, _mapHeight,
                  _rootConsole, 0, 0);
                RLConsole.Blit(_statConsole, 0, 0, _statWidth, _statHeight,
                  _rootConsole, _mapWidth, 0);
                RLConsole.Blit(_messageConsole, 0, 0, _messageWidth, _messageHeight,
                  _rootConsole, 0, _screenHeight - _messageHeight);

                _rootConsole.Draw();

                _renderRequired = false;
            }
            
        }
    }
}
