using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;

namespace DexteraA
{
    class Logic
    {
        private List<Cell> _openCell,_closeCell,_neibrCells;
        public List<Cell> FinalPath;
        private Cell  _startCell, _currentCell;
        public Cell _finalCell;

        public Logic(List<Cell> cell)
        {
            _openCell = new List<Cell>();
            _closeCell = new List<Cell>();
            _neibrCells = new List<Cell>();
            FinalPath = new List<Cell>();
            _startCell = cell[0];
            _finalCell = cell[1];
            for (int i = 2; i < cell.Count; i++)
                _closeCell.Add(cell[i]);
        }
        public void Searching()
        {
            _currentCell = _startCell;
            _openCell.Add(_startCell);
            while (_openCell != null)
            {                             
                LowestFPriceCell(_openCell);               
                if (_currentCell.Equals(_finalCell))
                {
                    Finalpath(); 
                    break;                      
                }             
                _openCell.Remove(_currentCell);     
                _closeCell.Add(_currentCell);    
                AddNeighbours();
                foreach (var neibrHood in _neibrCells)
                {
                    if (_closeCell.Contains(neibrHood))
                        continue;
                    if (!_openCell.Contains(neibrHood))
                    {
                        _openCell.Add(neibrHood);
                        neibrHood.Parent = _currentCell;
                        neibrHood.Hprice = neibrHood.HPrice(_finalCell);
                        neibrHood.Gprice = neibrHood.GPrice(_currentCell);
                        neibrHood.Fprice = neibrHood.HPrice(_finalCell) + neibrHood.GPrice(_currentCell); 
                        continue;
                    }
                    if(_currentCell.GPrice(_currentCell.Parent) + neibrHood.GPrice(_currentCell) < neibrHood.GPrice(_currentCell.Parent))
                    {
                        neibrHood.Parent = _currentCell.Parent;
                        neibrHood.Hprice = neibrHood.HPrice(_finalCell);
                        neibrHood.Gprice = neibrHood.GPrice(_currentCell) + neibrHood.GPrice(_currentCell);
                        neibrHood.Fprice = neibrHood.HPrice(_finalCell) + neibrHood.GPrice(_currentCell); 
                    }                   
                }                                        
            }
               
            string text = _closeCell.Count.ToString() + " open = " + _openCell.Count.ToString();
            
            MessageBox.Show(text); 
            
        }
        //запись окончательного пути в массив
        private void Finalpath()
        {
            var temp = _currentCell;                
            while(temp != _startCell)
            {
                temp = temp.Parent;
                if (!FinalPath.Contains(temp))                  
                    FinalPath.Add(temp);
            }                
        }
        //нахождение наименьшего F(для установки его родителем)
        private void LowestFPriceCell(List<Cell> openCell)
        {
            int F = 1000000;
            for (int i = 0; i < openCell.Count; i++)
                if (F > openCell[i].GPrice(_openCell[i].Parent) + openCell[i].HPrice(_finalCell))
                {
                    F = openCell[i].Fprice;
                    _currentCell = openCell[i];
                    _currentCell.Hprice = _currentCell.HPrice(_finalCell);
                    _currentCell.Gprice = _currentCell.GPrice(_currentCell.Parent);
                    _currentCell.Fprice = _currentCell.Hprice + _currentCell.Gprice;

                }
        }
        //доделать! не красиво!
        private void AddNeighbours()
        {
            _neibrCells.Clear();
            _neibrCells.Add(new Cell(_currentCell.X - 20, _currentCell.Y - 20));
            _neibrCells.Add(new Cell(_currentCell.X - 20, _currentCell.Y));
            _neibrCells.Add(new Cell(_currentCell.X - 20, _currentCell.Y + 20));
            _neibrCells.Add(new Cell(_currentCell.X + 20, _currentCell.Y - 20));
            _neibrCells.Add(new Cell(_currentCell.X + 20, _currentCell.Y));
            _neibrCells.Add(new Cell(_currentCell.X + 20, _currentCell.Y + 20));
            _neibrCells.Add(new Cell(_currentCell.X, _currentCell.Y - 20));
            _neibrCells.Add(new Cell(_currentCell.X, _currentCell.Y + 20));
        }
    }
    /*
     *  if (!_openCell.Contains(neibrHood))
                    {
                        _openCell.Add(neibrHood);
                        neibrHood.Parent = _currentCell;
                        neibrHood.Hprice = neibrHood.HPrice(_finalCell);
                        neibrHood.Gprice = neibrHood.GPrice(_currentCell);
                        neibrHood.Fprice = neibrHood.HPrice(_finalCell) + neibrHood.GPrice(_currentCell); 
                        continue;
                    }
                    if(_currentCell.GPrice(_currentCell.Parent) + neibrHood.GPrice(_currentCell) > neibrHood.GPrice(_currentCell.Parent))
                    {
                        neibrHood.Parent = _currentCell;
                        neibrHood.Hprice = neibrHood.HPrice(_finalCell);
                        neibrHood.Gprice = neibrHood.GPrice(_currentCell);
                        neibrHood.Fprice = neibrHood.HPrice(_finalCell) + neibrHood.GPrice(_currentCell); 
                    }       
     */
}
