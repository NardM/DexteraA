using System;

namespace DexteraA
{
    [Serializable()]
    public class Cell
    {
        public const int GPrice = 10; 
        
        public int X { get; set; }
        public int Y { get; set; }
        public int Gprice { get; set; }
        public int Fprice { get; set; }
        public int Hprice { get; set; }
        public Cell Parent { get; set; }
        public bool Blocked { get; set; }
        public bool Player { get; set; }
        public bool Exit { get; set; }

        public Cell(int x, int y , bool blocked)
        {
            Parent = null;
            
            X = x;
            Y = y;
            
            //Стоимость пути(по прямой 10)      
            Blocked = blocked;
        }

        public Cell(int x, int y)
        {
            Parent = null;
            X = x;
            Y = y;
            
            Player = false;
            Exit = false;
            Blocked = false;
        }

        //Эвристическая оценка
        public int HPrice(Cell finalcell, int herouisticmode)
        {
            switch (herouisticmode)
            {
                case 1://быстрое манхеттонское
                    return  (Math.Abs(X - finalcell.X) + Math.Abs(Y - finalcell.Y));
                case 2://медленное, но точное манхеттонское
                    return (Math.Abs(X - finalcell.X) + Math.Abs(Y - finalcell.Y))/5;
                case 3://диагональный расчет
                    {
                    int xDist = Math.Abs(X - finalcell.X);
                    int yDist = Math.Abs(Y - finalcell.Y);
                    if (xDist > yDist)
                        return (int)(1.4*yDist) + (xDist - yDist);
                    return (int)(1.4*xDist) + (yDist - xDist);
                    }
            }
            return 0;
        }
        
        public Cell this[int i]
        {
            get { return new Cell(X, Y); }
        }
        public override bool Equals(object obj)
        {
            Cell cell = obj as Cell;
            if (cell != null)
                return (X == cell.X && Y == cell.Y);
            return false;
        }
    }
}
