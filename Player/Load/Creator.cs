using NLog;
using Player.Load.Element;
using Player.Model;
using Player.Model.Action;
using System;

namespace Player.Load
{
    /// <summary>
    /// Creates keyboard model inside code.
    /// Used mainly for development.
    /// </summary>
    class Creator
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public static KeyboardModel CreateDevKeyboard(int id)
        {
            Keyboard kb = null;

            switch (id)
            {
                case 1:
                    kb = CreateDevKeyboard1(); break;
                case 2:
                    kb = CreateDevKeyboard2(); break;
                case 3:
                    kb = CreateDevKeyboard3(); break;
                case 4:
                    kb = CreateDevKeyboard4(); break;
                case 5:
                    kb = CreateDevKeyboard5(); break;
                default:
                    throw new ArgumentException("There is no dev keyboard for given id!");
            }

            kb.Validate();
            return kb;
        }

        private static Keyboard CreateDevKeyboard1()
        {
            logger.Trace("CreateTestKeyboard1()...");

            Keyboard kb = new Keyboard();
            kb.Name = "DevKB1";

            Action<string, string> addGrid = (string id, string target) =>
            {
                Grid g = CreateFilledFeaturedGrid(id, 1, 1, (Button b, int x, int y) =>
                {
                    b.SetText("next");
                    b.AddAction(new SwitchGridActionParameter(target));
                });
                g.ScanParams = CreateScanParams("linear", active: false);
                kb.AddGrid(g);
            };

            addGrid("g1", "g2");
            addGrid("g2", "g3");
            addGrid("g3", "g1");

            kb.DefaultGridId = "g1";
            return kb;
        }

        /// <summary>Testing buttons with dimension greater 1.</summary>
        private static Keyboard CreateDevKeyboard2()
        {
            logger.Trace("CreateTestKeyboard2()...");

            Keyboard kb = new Keyboard();
            kb.Name = "DevKB2";
            kb.ScanParams = CreateScanParams(null, false);

            Grid g1 = CreateEmptyGrid("g1", 3, 1);

            Button b1 = new Button("b1");
            b1.SetPosition(0, 0, 2, 1);
            b1.SetText("big");
            g1.AddButton(b1);

            Button b2 = new Button("b2");
            b2.SetPosition(2, 0);
            b2.SetText("normal");
            g1.AddButton(b2);

            kb.AddGrid(g1);
            kb.DefaultGridId = g1.Id;

            return kb;
        }

        private static Keyboard CreateDevKeyboard3()
        {
            logger.Trace("CreateTestKeyboard3()...");

            Keyboard kb = new Keyboard();
            kb.Name = "DevKB3";

            Func<string, string, Grid> createAndAddGrid = (string id, string target) =>
            {
                Grid g = CreateFilledFeaturedGrid(id, 3, 3, (Button b, int x, int y) =>
                {
                    if (x == 0 && y == 0)
                    {
                        b.SetText("next");
                        b.AddAction(new SwitchGridActionParameter(target));
                    }
                });
                kb.AddGrid(g);
                return g;
            };

            bool scannerActive = true;
            createAndAddGrid("g1", "g2").ScanParams = CreateScanParams("row-column", scannerActive);
            createAndAddGrid("g2", "g3").ScanParams = CreateScanParams("column-row", scannerActive); ;
            createAndAddGrid("g3", "g1").ScanParams = CreateScanParams("linear", scannerActive); ;
            
            kb.DefaultGridId = "g2";

            return kb;
        }

        private static Keyboard CreateDevKeyboard4()
        {
            logger.Trace("CreateTestKeyboard4()...");

            Keyboard kb = new Keyboard();
            kb.Name = "DevKB4";

            Action<Button, int, int> features = (Button b, int x, int y) =>
            {
                if (x == 0 && y == 0)
                {
                    b.SetText("tami");
                    b.AddAction(new TTSActionParameter("hier sage ich ganz laut tami"));
                }
                else if (x == 1 && y == 0)
                {
                    b.SetText("sewi");
                    b.AddAction(new TTSActionParameter("hier sage ich ganz laut sewi"));
                }
                else if (x == 0 && y == 1)
                {
                    b.SetText("much");
                    b.AddAction(new LogActionParameter("Bla bla bla"));
                    b.AddAction(new TimeActionParameter(TimeSpan.FromSeconds(2)));
                    b.AddAction(new TTSActionParameter("einiges geht ab hier"));
                }
            };

            Grid g1 = CreateFilledFeaturedGrid("g1", 2, 2, features);
            g1.ScanParams = CreateScanParams("linear", true);
            kb.AddGrid(g1);

            kb.DefaultGridId = g1.Id;

            return kb;
        }

        /// <summary>Testing non full grids (grids with empty or NOP buttons).</summary>
        private static Keyboard CreateDevKeyboard5()
        {
            logger.Trace("CreateTestKeyboard5()...");

            Keyboard kb = new Keyboard();
            kb.Name = "DevKB5";
            kb.ScanParams = CreateScanParams("linear", true);

            Grid g1 = CreateEmptyGrid("g1", 5, 5);

            for (int x = 0; x < g1.Cols; x = x + 2)
                for (int y = 0; y < g1.Rows; y = y + 2)
                    g1.AddButton(CreateBasicButton(x, y));
            
            kb.AddGrid(g1);
            kb.DefaultGridId = g1.Id;

            return kb;
        }

        private static Grid CreateEmptyGrid(string id, int cols, int rows)
        {
            Grid g = new Grid(id);
            g.SetDimension(cols, rows);
            return g;
        }

        private static Button CreateBasicButton(int x, int y)
        {
            string id = String.Format("btn{0}-{1}", x, y);

            Button b = new Button(id);
            b.SetPosition(x, y);
            b.SetText(id);
            return b;
        }

        private static Grid CreateFilledFeaturedGrid(string id, int cols, int rows, Action<Button, int, int> features)
        {
            Grid g = CreateEmptyGrid(id, cols, rows);
            FillFeaturedGrid(g, features);
            return g;
        }

        private static void FillFeaturedGrid(Grid g, Action<Button, int, int> features)
        {
            for (int x = 0; x < g.Cols; x++)
                for (int y = 0; y < g.Rows; y++)
                    g.AddButton(CreateFeaturedButton(x, y, features));
        }

        private static Button CreateFeaturedButton(int x, int y, Action<Button, int, int> features)
        {
            Button b = CreateBasicButton(x, y);

            features(b, x, y);

            return b;
        }

        private static ScannerParam CreateScanParams(string type, bool active = true)
        {
            ScannerParam sp = new ScannerParam();

            sp.ScannerActive = active;
            sp.ScannerType = type;

            return sp;
        }
    }
}
