using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Msagl;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.Msagl.Drawing;
using System.Drawing;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WpfApp4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Edmonds_Karp_algorithm EKA;//Экземпляр класса, где реализуется алгоритм Эдмондса-Карпа
        Edmonds_Karp_algorithm.point[] points;//Массив, содержащий информацию о вершинах графа
        public ObservableCollection<InfoAboutEdge> infoAboutEdges = new ObservableCollection<InfoAboutEdge>();//Коллекция, содержащая информацию о загруженности рёбер графа
        int[][] lines;//Массив, содержащий информацию о длине ребер
        List<TextBox> BaseTextBoxes = new List<TextBox>();//Список, содержащий все textBox на вкладке создания матрицы смежности
        IEnumerable<TextBox> elements = null;//Используется для поиска TextBox с нужными характеристиками
        int selected = 0;//Показывает сколько вершин будет в графе (число, которое выбрал пользователь)
        bool[,] check;//Проверяет было ли пройдено то или иное ребро в графа (используется в выводе информации о графе)
        bool DirectedGraph = false;//Проверяет, является ли граф ориентированным
        public MainWindow()
        {
            string path = Environment.CurrentDirectory + "//Photo//";
            try
            {
                System.IO.Directory.Delete(path, true);
            }
            catch (System.IO.DirectoryNotFoundException) { }
            System.IO.Directory.CreateDirectory(path);
            InitializeComponent();
            OperationsWithGraph.IsEnabled = false;
        }

        public ObservableCollection<InfoAboutEdge> InfoAboutEdges
        {
            get { return infoAboutEdges; }
        }
        public void Grap(Queue<int> path)//Создание модели графа с помощью MSAGL
        {
            Graph graph = new Graph("");
            check = new bool[selected - 1, selected - 1];
            for (int counter = 0; counter < lines.Count(); counter++)
            {
                for (int counter2 = 0; counter2 < lines.Count(); counter2++)
                {
                    if (lines[counter][counter2] != 0 && check[counter, counter2] != true && counter!=counter2)
                    {
                        check[counter, counter2] = true;
                        check[counter2, counter] = true;
                        graph.AddEdge(Convert.ToString(counter + 1), Convert.ToString(lines[counter][counter2]), Convert.ToString(counter2 + 1));
                        if (!DirectedGraph) graph.AddEdge(Convert.ToString(counter2 + 1), Convert.ToString(lines[counter][counter2]), Convert.ToString(counter + 1));
                        graph.FindNode(Convert.ToString(counter+1)).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
                        graph.FindNode(Convert.ToString(counter2+1)).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
                    }
                }
            }
            while (path.Count != 0)
            {
                graph.FindNode(Convert.ToString(path.Dequeue())).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Aqua;
            }
            GraphRenderer renderer = new GraphRenderer
            (graph);
            renderer.CalculateLayout();
            int width = 150;
            Bitmap bitmap = new Bitmap(width, (int)(graph.Height *
            (width / graph.Width)));
            string randomFileName = System.IO.Path.GetRandomFileName();
            randomFileName = randomFileName.Replace(".", "");
            var pathLink = Environment.CurrentDirectory + "//Photo//" + randomFileName + ".png";
            renderer.Render(bitmap);
            bitmap.Save(pathLink);
            GraphImage.Source = new BitmapImage(new Uri(pathLink));
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)//Выполняется при выборе пользователем количества вершин в графе
        {
            DirectedGraph = false; //Проверяет ориентированный ли граф (влияет на привязку)
            MessageBoxResult result = MessageBox.Show("Граф является ориентированным?", "Подтверждение", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes) DirectedGraph = true; //Если пользоветель нажал "Да", то граф ориентированный
            if (result == MessageBoxResult.Cancel) return;
            InputGrid.Children.Clear();
            InputGrid.ColumnDefinitions.Clear();
            InputGrid.RowDefinitions.Clear();
            UnselectedText.Opacity = 0;//Убираем первоначальный текст в comboBox
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem comboBoxItem = (ComboBoxItem)comboBox.SelectedItem;
            selected = int.Parse(comboBoxItem.Tag.ToString()) + 1;//Узнаём выбранный элемент (сколько вершин будет в графе)
            CreateMatrix(selected);//Создаём матрицу смежности
            CreateBindings();
        }
        void CreateMatrix(int size)//Создаём матрицу смежности
        {
            TextBox textBox = new TextBox();
            textBox.TextAlignment = TextAlignment.Center;
            textBox.FontSize = 25;
            for (int counter_column = 0; counter_column < size; counter_column++)//Создаём матрицу смежности для заполнения
            {
                if (counter_column == 0)
                {
                    ColumnDefinition columnDefinition = new ColumnDefinition();//Создаем экземляр колонки и добавляем её в Grid
                    columnDefinition.Width = new GridLength(30);
                    InputGrid.ColumnDefinitions.Add(columnDefinition);
                }
                else
                {
                    ColumnDefinition columnDefinition = new ColumnDefinition();//Создаем экземляр колонки и добавляем её в Grid
                    InputGrid.ColumnDefinitions.Add(columnDefinition);
                }
                for (int counter_row = 0; counter_row < size; counter_row++)//Создаём строки и добавляем в них элементы
                {
                    if ((counter_column == 0 || counter_row == 0) || (counter_column == counter_row))//Добавляем номера вершин в ячейки Grid
                    {
                        textBox = new TextBox();//Создаём экземпляр textBlock и добавляем в крайние ячейки с соответствующим номером
                        if (counter_column == 0 && counter_row == 0) textBox.Text = "";
                        else if (counter_column == 0) textBox.Text = Convert.ToString(counter_row);
                        else textBox.Text = Convert.ToString(counter_column);
                        textBox.Tag = new Locations(counter_column, counter_row); //Заносим расположение ячейки, в котором находится поле, в имя поля
                        textBox.TextAlignment = TextAlignment.Center;
                        textBox.FontSize = 20;
                        textBox.IsReadOnly = true;
                        textBox.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(204, 217, 85));
                        textBox.IsTabStop = false;
                        if (counter_column == counter_row && counter_column != 0 && counter_row != 0) textBox.Text = "0";
                        //textBox.BorderThickness = new Thickness(1);
                        RowDefinition rowDefinition = new RowDefinition();
                        rowDefinition.Height = GridLength.Auto;
                        InputGrid.RowDefinitions.Add(rowDefinition);
                        Grid.SetColumn(textBox, counter_column);//Прописываем а какой столбец заносить
                        Grid.SetRow(textBox, counter_row);//Прописываем а какую строку заносить
                        InputGrid.Children.Add(textBox);//Заносим
                        BaseTextBoxes.Add(textBox);//Заносим textBox в список
                    }
                    else
                    {
                        textBox = new TextBox();//Создаём ячейки и добавляем в них textBox для заполнения матрицы смежности
                        string location = Convert.ToString(counter_column) + Convert.ToString(counter_row);
                        textBox.Tag = new Locations(counter_column, counter_row); //Заносим расположение ячейки, в котором находится поле, в имя поля
                        textBox.TextAlignment = TextAlignment.Center;
                        textBox.FontSize = 25;
                        textBox.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(204, 217, 85));
                        RowDefinition rowDefinition = new RowDefinition();
                        InputGrid.RowDefinitions.Add(rowDefinition);
                        Grid.SetColumn(textBox, counter_column);
                        Grid.SetRow(textBox, counter_row);
                        InputGrid.Children.Add(textBox);
                        BaseTextBoxes.Add(textBox);//Заносим textBox в список
                    }
                }
            }
        }
        void CreateBindings()//Создание привязок текста полей для ввода (если граф неориентированный)
        {
            if (DirectedGraph == true) return; //Если граф ориентированный, выходим
            for (int counter = 0; counter < BaseTextBoxes.Count; counter++)
            {
                Binding binding = new Binding();
                binding.Path = new PropertyPath("Text");
                binding.Mode = BindingMode.TwoWay;
                TextBox textBox = (TextBox)BaseTextBoxes[counter];
                TextBox textbox1 = new TextBox();
                Locations locations = (Locations)textBox.Tag;
                if ((locations.Column == locations.Row) || locations.Row == 0 || locations.Column == 0) continue;
                var find = from element in BaseTextBoxes
                           where ((Locations)element.Tag).Column == locations.Row && ((Locations)element.Tag).Row == locations.Column
                           select element;
                foreach (var element in find)
                {
                    textbox1 = element;
                }
                binding.Source = textbox1;
                textBox.SetBinding(TextBox.TextProperty, binding);
            }
        }
        class Locations//Сохраняет данные ячейки, в которой расположено поле  textBox
        {
            int column;//Номер столбца
            int row;//Номер строки
            bool binding;//Записывает была ли выполнена привязка этого поля к другому (если граф неориентированный)
            public Locations(int _column, int _row)
            {
                column = _column;
                row = _row;
                binding = false;
            }
            public int Column
            {
                get { return column; }
            }
            public int Row
            {
                get { return row; }
            }
            public bool Binding
            {
                get { return binding; }
                set { binding = value; }
            }
        }
        private void InputGrid_MouseEnter(object sender, MouseEventArgs e)//Подсвечивание ячейки, на которую навели курсор мыши
        {
            TextBox textBox = (TextBox)sender;
            textBox.Focus();
        }
        private void InputGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            var element = (UIElement)e.Source;
            int column = Grid.GetColumn(element);//Колонка, в которой находится textBox, на который навели курсор мыши
            int row = Grid.GetRow(element);//Строка, в которой находится textBox, на который навели курсор мыши
            //Выделим все ячеки слева и справа от той, на которую наведён курсор
            //Или выделим весь столбец или строку целиком
            if (column == 0 && row == 0)
            {
                elements = null;
                return;
            }
            if (column == 0)//Если навели курсор на первую ячейку строки, то выделяем всю строку
            {
                elements = from counter in BaseTextBoxes
                           where ((Locations)counter.Tag).Row == row
                           select counter;

            }
            else if (row == 0)//Если навели курсор на первую ячейку столбца, то выделяем весь столбец
            {
                elements = from counter in BaseTextBoxes
                           where ((Locations)counter.Tag).Column == column
                           select counter;

            }
            else//Выделяем все ячейки слева и сверху от заданной
            {
                elements = from counter in BaseTextBoxes
                           where (((Locations)counter.Tag).Column == column && ((Locations)counter.Tag).Row <= row) ||
                           (((Locations)counter.Tag).Row == row && ((Locations)counter.Tag).Column <= column)
                           select counter;
            }
            foreach (var counter in elements)
            {
                counter.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 230, 153));
                counter.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(232, 39, 39));
            }
        }

        private void InputGrid_LostFocus(object sender, RoutedEventArgs e)//Возвращение первоначального цвета ячеек при потере фокуса
        {
            if (elements != null)
            {
                foreach (var counter in elements)
                {
                    counter.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(204, 217, 85));
                    counter.Foreground = System.Windows.Media.Brushes.Black;
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)//Обработчик кнопки для создания графа по матрице смежности
        {
            CreateGraph();
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)//Очистить таблицу (очистить только текста в ячейках)
        {
            for (int counter = 0; counter < BaseTextBoxes.Count(); counter++)
            {
                if (BaseTextBoxes[counter].IsReadOnly != true) BaseTextBoxes[counter].Text = "";
            }
            OperationsWithGraph.IsEnabled = false;
        }
        void CreateGraph()//Создание графа и заполения матрицы с путями и открытие окна для загрузки рёбер
        {
            input_line();//Заносим матрицу смежности в массив
            input_point();//Заносим информацию о каждой вершине графа в массив
            Queue<int> path=new Queue<int>();
            Grap(path);
            EKA = new Edmonds_Karp_algorithm(ref points, ref lines);//Инициализируем экземпляр класса алгоритма Эдмондса-Карпа
            string Alert = "Граф успешно создан" + Environment.NewLine + "Открылась возможность загрузки рёбер графа"; //Оповещение об успешном создании графа
            MessageBox.Show(Alert, "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
            OperationsWithGraph.IsEnabled = true;
            OperationsWithGraph.Focus();//Переходим фокус вкладке, в которой происходит загрузка графа
            InputInformationAboutTopsInListView();//Вывод информации о рёбрах графа
            InputNumberTopsInComboBox();
        }
        void input_point()//Заносим информацию о каждой вершине графа в массив
        {
            points = new Edmonds_Karp_algorithm.point[selected - 1];
            for (int counter = 0; counter < lines.Count(); counter++)
            {
                int[] temporary = new int[lines.Count()];
                Array.Copy(lines[counter], 0, temporary, 0, lines.Count());
                points[counter] = new Edmonds_Karp_algorithm.point(counter+1, temporary);
            }
        }
        void input_line() //Заносим матрицу смежности в массив
        {
            lines = new int[selected - 1][];//Объявляем массив с длиной ребер
            for (int counter = 1; counter < selected; counter++)
            {
                lines[counter - 1] = new int[selected - 1];//Объявляем массив с ребрами для каждой вершины
                for (int counter2 = 1; counter2 < selected; counter2++)
                {
                    var find = from element in BaseTextBoxes where ((Locations)element.Tag).Column == counter && ((Locations)element.Tag).Row == counter2 select element;
                    foreach (var element in find)
                    {
                        try
                        {
                            int indexColumn = ((Locations)element.Tag).Column - 1;
                            int indexRow = ((Locations)element.Tag).Row - 1;
                            if (element.Text == "" || element.Text == "0") lines[indexColumn][indexRow] = 0;
                            else lines[indexColumn][indexRow] = int.Parse(element.Text);
                        }
                        catch (FormatException) { MessageBox.Show("Вводите только цифры", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                    }
                }
            }
        }
        public class InfoAboutEdge : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;
                if (handler != null)
                    handler(this, new PropertyChangedEventArgs(propertyName));
            }
            //string numberPoint;//Номер вершины
            string infoEdge;//Информация о пути
            string infoWork;//Информация о загруженности пути
            public InfoAboutEdge(string _infoEdge, string _infoWork/*, string _numberPoint*/)
            {
                infoEdge = _infoEdge;
                infoWork = _infoWork;
            }
            public string InfoEdge
            {
                get { return infoEdge; }
                set
                {
                    if (value != infoEdge)
                    {
                        infoEdge = value;
                        OnPropertyChanged("InfoEdge");
                    }
                }
            }
            public string InfoWork
            {
                get { return infoWork; }
                set
                {
                    if (value != infoWork)
                    {
                        infoWork = value;
                        OnPropertyChanged("InfoWork");
                    }
                }
            }

        }
        void InputInformationAboutTopsInListView()//Вывод информации о рёбрах графа 
        {
            check = new bool[selected - 1, selected - 1];
            string InfoEdge;
            string InfoEdgeWork;
            int Busy;
            for (int counter = 0; counter < selected - 1; counter++)
            {
                for (int counter2 = 0; counter2 < selected - 1; counter2++)
                {
                    if (check[counter, counter2] != true && lines[counter][counter2] != 0)
                    {
                        int tops1 = counter + 1;
                        int tops2 = counter2 + 1;
                        InfoEdge = tops1 + " -> " + tops2;
                        check[counter, counter2] = true;
                        if (DirectedGraph == false)
                        {
                            InfoEdge += " (" + tops2 + " -> " + tops1 + ")";
                            check[counter2, counter] = true;
                        }
                        Busy = points[counter].Maxworkload[counter2] - lines[counter][counter2];
                        InfoEdgeWork = Busy + " / " + points[counter].Maxworkload[counter2];
                        infoAboutEdges.Add(new InfoAboutEdge(InfoEdge, InfoEdgeWork));
                    }

                }
            }
        }
        void InputNumberTopsInComboBox()//Вводим номера вершин в comboBox's, находящиеся во вкладке загрузки графа
        {
            for (int number = 1; number < selected; number++)
            {
                BeginPath.Items.Add(number);//Заполняем 1-й comboBox
                EndPath.Items.Add(number);//Заполняем 2-й comboBox
            }
        }

        private void SizeWork_MouseEnter(object sender, MouseEventArgs e)//Вхождение курсора мыши в textBox, в который вводится величина задачи
        {
            if (SizeWork.Opacity == 0.5)
            {
                SizeWork.Focus();
                SizeWork.Text = "";
                SizeWork.Opacity = 1;
            }
        }

        private void SizeWork_MouseLeave(object sender, MouseEventArgs e)//Покидание курсора мыши textBox, в который вводится величина задачи
        {
            if (SizeWork.Text == "")
            {
                SizeWork.Text = "Введите вес задачи";
                SizeWork.Opacity = 0.5;
            }
            OperationsWithGraph.Focus();
        }

        private void TransferToTask_Click(object sender, RoutedEventArgs e)//Передача задачи в алгоритм Эдмондса-Карпа
        {
            if (!CheckOnNull()) return;
            int Begin = int.Parse(BeginPath.Text) - 1;//Начало пути
            int End = int.Parse(EndPath.Text) - 1;//Конец пути
            int Size = int.Parse(SizeWork.Text);//Величина заявки
            bool check = EKA.EdmondsAlgorithm(Begin, End, Size);//Передача пути и заявки в алгоритм
            if (check) UpdateInfoAboutPoint(Begin, End);
            else MessageBox.Show("Нет возможности совершить передачу." + Environment.NewLine + "Все ребра заполнены или не существует пути."+Environment.NewLine+"Измените вершины или умешьшите размер задачи","Переполнение",MessageBoxButton.OK,MessageBoxImage.Error);
           
        }
        void UpdateInfoAboutPoint(int begin, int end)//Обновить информацию о ребре
        {
            string FindEdge;
            int tops1;
            int tops2;
            int index;
            int Busy;
            string Path=null;
            string InfoEdgeWork;
            Queue<int> pathLink = new Queue<int>();
            for (int counter = 0; counter < points[end].paths.Count - 1; counter++)
            {
                tops1 = points[end].paths[counter]+1;
                tops2 = points[end].paths[counter + 1]+1;
                if (counter == 0) pathLink.Enqueue(tops1);
                pathLink.Enqueue(tops2);
                FindEdge = tops1 + " -> " + tops2;
                if(DirectedGraph==false) FindEdge += " (" + tops2 + " -> " + tops1 + ")";
                var find = from element in infoAboutEdges where element.InfoEdge == FindEdge select element;
                foreach (var element in find)
                {
                    index = infoAboutEdges.IndexOf(element);
                    Busy = points[tops1-1].Maxworkload[tops2-1] - lines[tops1-1][tops2-1];
                    InfoEdgeWork = Busy + " / " + points[tops1-1].Maxworkload[tops2-1];
                    infoAboutEdges[index].InfoWork = InfoEdgeWork;
                }
            }
            for (int counter = 0; counter < points[end].paths.Count; counter++)
            {
                tops1 = points[end].paths[counter] + 1;
                if (counter < points[end].paths.Count - 1) Path += tops1 + " -> ";
                else Path += tops1;

            }
            Grap(pathLink);
            PathOutput.Opacity = 1;
            PathOutput.Text = "Маршрут из" + (begin+1) + " в " + (end+1) + ": " + Path;
        }
        bool CheckOnNull()//Проверка на незаполненные поля
        {
            if (BeginPath.SelectedItem == null)
            {
                MessageBox.Show("Введите начальную вершину", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (EndPath.SelectedItem == null)
            {
                MessageBox.Show("Введите конечную вершину", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (SizeWork.Text == "")
            {
                MessageBox.Show("Введите величину задачи", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (BeginPath.Text == EndPath.Text)
            {
                MessageBox.Show("Номер начальной и конечной вершины не должны совпадать", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            try
            {
                int check = int.Parse(SizeWork.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Вводите только числа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
    }
}
