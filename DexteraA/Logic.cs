using System.Collections.Generic;
using System.Linq;

namespace DexteraA
{
    class Logic
    {
        private List<Cell> _openCell,_closeCell,_neibrCells;
        public Stack<Cell> FinalPath;
        public bool Finded { get; set; }
        private readonly Cell _startCell,_finalCell;
        private Cell _currentCell;
        private int _maxAreaX;
        private int _maxAreaY;

        public Logic(List<Cell> cell, Cell startCell, Cell endCell, int maxAreaX, int maxAreaY)
        {
            Finded = false;
            Swap(cell, startCell, 0);
            Swap(cell, endCell, 1);

            _openCell = new List<Cell>();
            _closeCell = new List<Cell>();
            _neibrCells = new List<Cell>();
            FinalPath = new Stack<Cell>();
            _startCell = cell[0];
            _finalCell = cell[1];

            _maxAreaX = maxAreaX;
            _maxAreaY = maxAreaY;

            for (int i = 2; i < cell.Count; i++)
                _closeCell.Add(cell[i]);
        }

        // метод реализующий поиск пути
        public void Searching(int check)
        {                        
            _startCell.Gprice = 0;
            _startCell.Hprice = _startCell.HPrice(_finalCell,check);
            _startCell.Fprice = _startCell.Hprice;
            _startCell.Parent = _startCell;
            _openCell.Add(_startCell);
            while (_openCell.Count != 0)
            {   
                LowestFPriceCell();                                  
                if (_currentCell.Equals(_finalCell))
                {
                    _finalCell.Parent = _currentCell;
                    Finalpath();
                    Finded = true;
                    break;                      
                }                    
                _openCell.Remove(_currentCell);     
                _closeCell.Add(_currentCell);               
                AddNeighbours();
                foreach (var neibrHood in _neibrCells)
                {
                    if (_closeCell.Contains(neibrHood) || neibrHood.Blocked)
                        continue;
                    int g = _currentCell.Gprice + Cell.GPrice;
                    
                    bool flag = false;
                    if (!_openCell.Contains(neibrHood))
                    {
                        _openCell.Add(neibrHood);
                        flag = true;
                    }                   
                    else if (g < Cell.GPrice + _currentCell.Gprice)
                            flag = true;
                    if (flag)
                    {
                        neibrHood.Parent = _currentCell;
                        neibrHood.Gprice = g;
                        neibrHood.Hprice = neibrHood.HPrice(_finalCell,check);
                        neibrHood.Fprice = neibrHood.Gprice + neibrHood.Hprice;
                    }
                }                           
            }                       
        }

        private void Swap(List<Cell> cells, Cell toSwap, int indexAtSwap)
        {
            int cellIndex = cells.IndexOf(toSwap);

            Cell bufCel = cells[cellIndex];
            cells[cellIndex] = cells[indexAtSwap];
            cells[indexAtSwap] = bufCel;
        }

        // запись окончательного пути в массив
        private void Finalpath()
        {
            var temp = _finalCell.Parent;
                    
            while(temp != _startCell)
            {
                temp = temp.Parent;        
                if (temp == _startCell)
                    break;
                if (!FinalPath.Contains(temp))                  
                    FinalPath.Push(temp);
            }                
        }
        //нахождение наименьшего F(для установки его родителем)
        private void LowestFPriceCell()
        {
            int F = 100000;
            foreach (var cell in _openCell)
            {
                if (F > cell.Fprice)
                {
                    _currentCell = cell; 
                    F = cell.Fprice;
                }
            }               
        }

        private bool CheckAreaOnX(int curX)
        {
            return (curX > _maxAreaX) || (curX < 0);
        }

        private bool CheckAreaOnY(int curY)
        {
            return (curY > _maxAreaY) || (curY < 0);
        }

        // добавление соседов точки
        private void AddNeighbours()
        {
            _neibrCells.Clear();
             
            if (_currentCell.X - 20 > 0)
                _neibrCells.Add(new Cell(_currentCell.X - 20, _currentCell.Y));

            if (_currentCell.X + 20 < _maxAreaX)
                _neibrCells.Add(new Cell(_currentCell.X + 20, _currentCell.Y));
            
            if (_currentCell.Y - 20 > 0)
                _neibrCells.Add(new Cell(_currentCell.X, _currentCell.Y - 20));

            if (_currentCell.Y + 20 < _maxAreaY)
                _neibrCells.Add(new Cell(_currentCell.X, _currentCell.Y + 20));
        }
    }
    
}
