using System.Windows;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Controls;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Win32;
using System.IO;

namespace DexteraA
{
    public partial class MainWindow
    {
        private GameStatus _status;
        int _check = 2;

        public MainWindow()
        {
            InitializeComponent();
            Main.Left = (SystemParameters.WorkArea.Width - Width)/2 + SystemParameters.WorkArea.Left;
            Main.Top = (SystemParameters.WorkArea.Height - Height)/2 + SystemParameters.WorkArea.Top;
            _status = new GameStatus();
            _status = GameStatus.Stoped;
        }

        private void StartPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (View.GameReady())
            {
                if (_status == GameStatus.Stoped)
                {
                    StartPauseButton.Content = "Pause";
                    _status = GameStatus.Started;
                    if (View.StartStop(_check))
                    {
                        StopEvent();
                    }
                }
                else if (_status == GameStatus.Started)
                {
                    StartPauseButton.Content = "Start";
                    _status = GameStatus.Stoped;
                    View.StartStop(_check);
                }
            }
            else
                MessageBox.Show("На поле должен быть один игрок и выход!");
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            StartPauseButton.Content = "Start";
            _status = GameStatus.Stoped;
            View.Clear();
            StartPauseButton.IsEnabled = true;
        }

        private void SizeChangeButton_Click(object sender, RoutedEventArgs e)
        {
            View.Height = int.Parse(PlayHeigh.Text)*20;
            View.Width = int.Parse(PlayWidth.Text)*20;

            StartPauseButton.Content = "Start";
            View.Clear();
        }

        private void RadioA_Checked(object sender, RoutedEventArgs e)
        {
            _check = 1;
        }

        private void RadioB_Checked(object sender, RoutedEventArgs e)
        {
            _check = 2;
        }

        private void RadioC_Checked(object sender, RoutedEventArgs e)
        {
            _check = 3;
        }

        private void SetImageTool(object sender, RoutedEventArgs e)
        {
            if (((Image) sender).Name == "PlayerImage")
                View.CurrentTool = TypeOfTool.Player;
            else if (((Image) sender).Name == "WallImage")
                View.CurrentTool = TypeOfTool.Wall;
            else
                View.CurrentTool = TypeOfTool.Exit;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var win = new Window
            {
                Left = Main.Left + 200,
                Top = Main.Top + 200,
                SizeToContent = SizeToContent.WidthAndHeight,
                Foreground = Brushes.DarkBlue,
                FontFamily = new FontFamily("Comic Sans MS"),
                Background = Brushes.WhiteSmoke
            };
            string text = "\tОбщее:\n" +
                          "Кликните по значку объекта, который хотите разместить\n" +
                          "и расположите его на карте\tПро эвристику:\n" +
                          "Манх.обычная - быстрый просчет пути , но не самый лучший\n" +
                          "Манх.медленный - более медленный , но почти самый лучший путь\n" +
                          "Диагональный - что-то между, единственный способ подсчета, допускающий ходы по диагонали\n" +
                          "Если хотите увеличить игровое поле, то просто растяните окно\n";
            win.Content = text;
            win.Show();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void StopEvent()
        {
            StartPauseButton.Content = "Start";
            _status = GameStatus.Stoped;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ToFileSave toSave = new ToFileSave()
            {
                Cells = View.Cells,
                Width = View.Width,
                Height = View.Height
            };

            int witdth = int.Parse(toSave.Width.ToString(CultureInfo.InvariantCulture))/20,
                height = int.Parse(toSave.Height.ToString(CultureInfo.InvariantCulture))/20;
            List<List<int>> list = new List<List<int>>();

            for (int i = 0; i <= witdth; i++)
            {
                list.Add(new List<int>());
                for (int j = 0; j <= height; j++)
                {
                    list[i].Add(0);
                }
            }
            int[,] mas =
                new int[height,
                    witdth];
            // Настраиваем диалог для сохранения

            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "saved game files (*.map)|*.map|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;


            if (saveFileDialog.ShowDialog() == DialogResult)
                return;
            // получаем выбранный файл
            string filename = saveFileDialog.FileName;
            StreamWriter myStream;
            // сохраняем текст в файл


            int countPlayer = 0;
            foreach (Cell cell in toSave.Cells)
            {
                if (cell.Player)
                {
                    list[cell.X/20][cell.Y/20] = 0;
                     countPlayer++;
                }
                if (cell.Blocked)
                    list[cell.X/20][cell.Y/20] = 1;

                if (cell.Exit)
                    list[cell.X/20][cell.Y/20] = 2;
            }
            //транспонирование матрицы
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < witdth; j++)
                {
                    mas[i, j] = list[j][i];
                }
            }
            using (myStream = new StreamWriter(filename))
            {
                myStream.WriteLine(int.Parse(toSave.Width.ToString(CultureInfo.InvariantCulture))/20 + " " +
                                   int.Parse(toSave.Height.ToString(CultureInfo.InvariantCulture))/20);
            }


            using (myStream = new StreamWriter(filename, true))
            {
               // myStream.WriteLine(mas);
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < witdth; j++)
                    {
                        myStream.Write(mas[i, j]);
                    }
                    myStream.Write("\r\n");
                }
            }
            using (myStream = new StreamWriter(filename, true))
            {
                myStream.WriteLine(countPlayer);
                foreach (Cell cell in toSave.Cells)
                {
                    if (cell.Player)
                        myStream.WriteLine(cell.X/20 + " " + cell.Y/20);
                }
                myStream.WriteLine(textBoxTimer.Text);
            }
        }


        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = "saved game files (*.map)|*.map|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };


            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                try
                {
                    string filename = openFileDialog.FileName;
                    using (StreamReader myStream = new StreamReader(filename))
                    {
                        string[] hw = myStream.ReadLine().Split(' ');
                        View.Height = int.Parse(hw[1])*20;
                        View.Width = int.Parse(hw[0])*20;
                        PlayHeigh.Text = hw[0];
                        PlayWidth.Text = hw[1];
                        int witdth = int.Parse(hw[0]),
                            height = int.Parse(hw[1]);
                        View.Cells.Clear();

                        int[,] mas =
                            new int[height,
                                witdth];

                        int[,] trans =
                            new int[witdth,
                                height];

                        //считывание
                        for (int i = 0; i < height; i++)
                        {
                            string str = myStream.ReadLine();
                            for (int j = 0; j < witdth; j++)
                            {
                                if (str != null) mas[i, j] = int.Parse(str[j].ToString());
                            }
                        }
                        //транспонирование
                        for (int i = 0; i < witdth; i++)
                        {
                            for (int j = 0; j < height; j++)
                            {
                                trans[i, j] = mas[j, i];
                            }
                        }
                        bool playerSet = false,
                            exitSet = false;


                        //считывание игроков
                        int player = int.Parse(myStream.ReadLine());
                        List<string> stList=new List<string>();

                        for (int i = 0; i < player; i++)
                        {
                            stList.Add(myStream.ReadLine());
                        }



                        for (int i = 0, k = 0; i < witdth; i++)
                        {
                            for (int j = 0; j < height; j++)
                            {

                                for (int l = 0; l < player; l++)
                                {
                                    string[] st = stList[l].Split(' ');
                                    if (int.Parse(st[0]) == i && int.Parse(st[1]) == j && trans[i, j] == 0)
                                    {
                                        View.CellsPost.Add(new Cell(i*20, j*20));
                                        View.CellsPost[k].Blocked = false;
                                        View.CellsPost[k].Exit = false;
                                        View.CellsPost[k].Player = true;
                                        playerSet = true;
                                        k++;
                                    }
                                }



                                if (trans[i, j] == 1)
                                {
                                    View.CellsPost.Add(new Cell(i*20, j*20));
                                    View.CellsPost[k].Blocked = true;
                                    View.CellsPost[k].Exit = false;
                                    View.CellsPost[k].Player = false;
                                    k++;
                                }
                                //if (trans[i, j] == 2)
                                //{
                                //    View.CellsPost.Add(new Cell(i*20, j*20));
                                //    View.CellsPost[k].Blocked = false;
                                //    View.CellsPost[k].Exit = false;
                                //    View.CellsPost[k].Player = true;
                                //    playerSet = true;
                                //    k++;
                                //}
                                if (trans[i, j] == 2)
                                {
                                    View.CellsPost.Add(new Cell(i * 20, j * 20));
                                    View.CellsPost[k].Blocked = false;
                                    View.CellsPost[k].Exit = true;
                                    View.CellsPost[k].Player = false;
                                    exitSet = true;
                                    k++;
                                }
                            }
                        }
                        textBoxTimer.Text = myStream.ReadLine();
                        View.LoadFromInfo(View.CellsPost, View.Width, View.Height, exitSet, playerSet);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void SaveButton_Click1(object sender, RoutedEventArgs e)
        {
            ToFileSave toSave = new ToFileSave()
            {
                Cells = View.Cells,
                Width = View.Width,
                Height = View.Height
            };

            // Настраиваем диалог для сохранения
            Stream myStream;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "saved game files (*.dbg)|*.dbg|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;

            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                if ((myStream = saveFileDialog.OpenFile()) != null)
                {
                    BinaryFormatter br = new BinaryFormatter();
                    br.Serialize(myStream, toSave);

                    myStream.Close();
                }
            }
        }

        private void LoadButton_Click1(object sender, RoutedEventArgs e)
        {
            Stream myStream;
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "saved game files (*.dbg)|*.dbg|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                try
                {
                    if ((myStream = openFileDialog.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            BinaryFormatter br = new BinaryFormatter();
                            ToFileSave toLoad = (ToFileSave) br.Deserialize(myStream);

                            View.Clear();
                            View.LoadFromInfo(toLoad.Cells, toLoad.Width, toLoad.Height);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}

