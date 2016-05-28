using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace DexteraA
{
    //класс вывода информации на эжкран
    public class View : FrameworkElement
    {
        private Logic _search;
        private DrawingVisual _grid, _wall, _ball;
        private bool _playerSet, _exitSet;
        private Stack<Cell> _allFindedPlayersPh;
        public TypeOfTool CurrentTool { get; set; }
        VisualCollection _visuals;
        private int _x, _y;
        private readonly DispatcherTimer _timer;
        public List<Cell> Cells;
        public List<Cell> CellsPost; 
        public VisualCollection Visuals
        {
            get { return _visuals; }
            set { _visuals = value; }
        }
        private bool _flag,_whiteflag;
        private string _pointSave="";

        public View()
        {            
            _timer = new DispatcherTimer();
            CellsPost = new List<Cell>();
              Cells = new List<Cell>();
            _visuals = new VisualCollection(this); 
            _timer.Interval = new TimeSpan(0,0,0,0,100); 
            _timer.Tick += timer_Tick;
            _allFindedPlayersPh = new Stack<Cell>();       
            DrawGrid();
        }

        public bool StartStop(int check)
        {
            bool fails = false;
            int x, y;
            Cell endCell =
                (from cell in Cells
                 where cell.Exit
                 select cell).Single();

            if(_timer.IsEnabled == false)  
            {
                _timer.Start();

                List<Cell> tempHolder = new List<Cell>();

                foreach (Cell startCell in Cells)
                {
                    if (startCell.Player)
                    {
                        XyCoords(new Point(startCell.X, startCell.Y), out x, out y);
                        _pointSave += (x/20).ToString() + " " + (y/20).ToString() + '\n';
                        _search = new Logic(Cells.ToList(), startCell, endCell, Convert.ToInt32(Width), Convert.ToInt32(Height));
                        _search.Searching(check);
                        
                        if (!_search.Finded)
                        {
                            MessageBox.Show("У одного из игроков не может быть найден путь!");
                            fails = true;
                            _timer.Stop();
                            break;
                        }

                        tempHolder.AddRange(_search.FinalPath);
                    }
                }
                    tempHolder.Reverse();
                    _allFindedPlayersPh = new Stack<Cell>(tempHolder);
            }
            else
                _timer.Stop();
            return fails;
        }
        
        public bool GameReady() 
        {
            return _playerSet && _exitSet;
        }

        public void LoadFromInfo(List<Cell> cells, double width, double height)
        {
            _exitSet = false;
            _playerSet = false;
            Width = width;
            Height = height;
            
            foreach (Cell cell in cells)
            {
                if (cell.Player)
                {
                    DrawBall(new Point(cell.X, cell.Y), Brushes.Green, TypeOfTool.Player);
                    _playerSet = true;
                }
                else if (cell.Exit)
                {
                    DrawBall(new Point(cell.X, cell.Y), Brushes.DarkOrange, TypeOfTool.Exit);
                    _exitSet = false;
                }
                else
                {
                    DrawWall(new Point(cell.X, cell.Y));
                }
            }
        }

        public void LoadFromInfo(List<Cell> cells, double width, double height, bool exitSet, bool playerSet)
        {
            _exitSet = false;
            _playerSet = false;
            if (exitSet) _exitSet = true;
            if (playerSet) _playerSet = true;
            Width = width;
            Height = height;

            foreach (Cell cell in cells)
            {
                if (cell.Player)
                {
                    DrawBall(new Point(cell.X, cell.Y), Brushes.Green, TypeOfTool.Player);
                    _playerSet = true;
                }
                else if (cell.Exit)
                {
                    DrawBall(new Point(cell.X, cell.Y), Brushes.DarkOrange, TypeOfTool.Exit);
                    _exitSet = false;
                }
                else
                {
                    DrawWall(new Point(cell.X, cell.Y));
                }
            }
            if (exitSet) _exitSet = true;
        }
        //очистка экрана
        public void Clear()
        {
            Cells.Clear();
            _timer.Stop();
            _grid.Children.Clear();

            _exitSet = false;
            _playerSet = false;
        }

        void timer_Tick(object sender, EventArgs e)
        {  
            if(_allFindedPlayersPh.Count != 0)
            {
                Cell temp = _allFindedPlayersPh.Pop();
                DrawBall(new Point(temp.X, temp.Y), Brushes.Red);
            }                                       
        }
        //отрисовка шаров
        public void DrawBall(Point point, Brush brush)
        {
            XyCoords(point, out _x, out _y);

            _ball = new DrawingVisual();
            using (DrawingContext dc = _ball.RenderOpen())
            {
                dc.DrawEllipse(brush, new Pen(Brushes.Black, 1), new Point(_x + 10, _y + 10), 6, 6);
            }
            _grid.Children.Add(_ball);
        }

        public void DrawBall(Point point, Brush brush, TypeOfTool toolToDraw)
        {
            DrawBall(point, brush);

            XyCoords(point, out _x, out _y);
            Cells.Add(new Cell(_x, _y));


            if (toolToDraw == TypeOfTool.Player)
            {
                Cells[Cells.Count - 1].Player = true;
            }
            else if (toolToDraw == TypeOfTool.Exit)
                Cells[Cells.Count - 1].Exit = true; 
        }
        //отрисовка стенок
        public void DrawWall(Point point)
        {
            XyCoords(point, out _x, out _y);
            _wall = new DrawingVisual();
            using (DrawingContext dc = _wall.RenderOpen())
            {
                dc.DrawRectangle(Brushes.Blue,new Pen(Brushes.Red,0.4), new Rect(new Point(_x,_y),new Point(_x+20,_y+20)));
            }
            //что бы не было повторения координат
            if (!Cells.Contains(new Cell(_x, _y)))
            {            
                Cells.Add(new Cell(_x, _y, true));
                _grid.Children.Add(_wall);
            }
        }
        //закраска ошибочных стенок
        public void DrawWhitewall(Point point)
        {
            XyCoords(point,out _x,out _y);
            _wall = new DrawingVisual();
            using (DrawingContext dc = _wall.RenderOpen())
            {
                dc.DrawRectangle(Brushes.White, new Pen(Brushes.Blue, 0.4), new Rect(new Point(_x, _y), new Point(_x + 20, _y + 20)));
            }
            //проверка на существование стенки 
            if (Cells.Contains(new Cell(_x, _y)) && (!Cells[Cells.IndexOf(new Cell(_x, _y))].Player && !Cells[Cells.IndexOf(new Cell(_x, _y))].Exit))
            {           
                _grid.Children.Add(_wall);
                Cells.Remove(new Cell(_x, _y)); 
            } 
        }
        
        //приведение полученных координат в нужный вид
        public void XyCoords(Point point,out int x,out int y)
        {
            x = (int)Math.Round(point.X / 10);
            if (x % 2 != 0)
                x = (x - 1) * 10;
            else if (x * 10 > point.X)
                x = (x - 2) * 10;
            else
                x = x * 10;
            y = (int)Math.Round(point.Y / 10);
            if (y % 2 != 0)
                y = (y - 1) * 10;
            else if (y * 10 > point.Y)
                y = (y - 2) * 10;
            else
                y = y * 10;
        }

        //отрисовка грида и задней стенки
        public void DrawGrid()
        {
            _grid = new DrawingVisual();
            using (DrawingContext dc = _grid.RenderOpen())
            {         
                dc.DrawRectangle(Brushes.GhostWhite, new Pen(Brushes.White,0),new Rect(new Point(0,0),new Point(2000,2000)) );
                for (int i = 0; i < 100; i++)
                {
                    dc.DrawLine(new Pen(Brushes.Blue,0.5),new Point(_x,0),new Point(_x,2000));
                    _x += 20;
                    dc.DrawLine(new Pen(Brushes.Blue,0.5),new Point(0,_y),new Point(2000,_y));
                    _y += 20;                    
                }            
            }
            _visuals.Add(_grid);
        }
       
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (CurrentTool == TypeOfTool.Player)
            {
                DrawBall(e.GetPosition(this), Brushes.Green, CurrentTool);
                _playerSet = true;
            }
            else if (CurrentTool == TypeOfTool.Exit && !_exitSet)
            {
                DrawBall(e.GetPosition(this), Brushes.DarkOrange, CurrentTool);
                _exitSet = true;
            }
            else if (CurrentTool == TypeOfTool.Wall)
            {
                DrawWall(e.GetPosition(this));
                _flag = true;
            }
            else
                MessageBox.Show("Нельзя ставить несколько точек для выхода!");
        }
        
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            if(_flag && CurrentTool == TypeOfTool.Wall)
                DrawWall(e.GetPosition(this));
            if(_whiteflag && CurrentTool == TypeOfTool.Wall)
                DrawWhitewall(e.GetPosition(this));
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            _flag = false;
        }
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            DrawWhitewall(e.GetPosition(this));
            _whiteflag = true;

        }
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
            _whiteflag = false;
        }
        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }
        protected override int VisualChildrenCount
        {
            get
            {
                return _visuals.Count;
            }
        }

        
    }

}
