using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp4
{
    class Edmonds_Karp_algorithm
    {
        point[] points;//Массив вершин
        int[][] lines;//Массив рёбер между вершинами
        int begin;//Вершина, из которой ведётся передача
        int end;//Вершина, в которую долны попасть данные
        public Edmonds_Karp_algorithm(ref point[] _points,ref int[][] _lines)//Конструктор класса
        {
            points = _points;
            lines = _lines;
        }
        public class point //Экземпляр вершины
        {
            int number_point;//Номер вершины
            bool passing;//Пройдена ли вершина
            int label;//Расстояние до вершины от начальной (метка)
            public int[] Maxworkload;//Максимальная пропускная способность линии передачи (ребра)
            public List<int> paths;//Реконструкция пути
            public point(int number_point_l, int[] Maxworkload_l)//Конструктор
            {
                number_point = number_point_l;
                passing = false;
                label = 99999;
                Maxworkload = Maxworkload_l;
                paths = new List<int>();
            }
            public bool passing_l
            {
                get
                {
                    return passing;
                }
                set
                {
                    passing = value;
                }
            }
            public int number_point_l
            {
                get
                {
                    return number_point;
                }
                set
                {
                    number_point = value;
                }
            }
            public int label_l
            {
                get
                {
                    return label;
                }
                set
                {
                    label = value;
                }
            }
        }
        public bool EdmondsAlgorithm(int _begin,int _end, int work)//Алгоритм Эдмондса-Карпа, на вход подаётся начальная и конечная вершина в передаче, а также вес задачи
        {
            Reset();
            begin = _begin;
            end = _end;
            bypass(begin,work);
            if (points[end].label_l == 99999) return false;
            for(int counter = 0; counter < points[end].paths.Count-1; counter++)//Используя реконструированный маршрут, уменьшаем пропускную способность линии передачи (ребра)
            {
                //Поочередно уменьшаем вес ребер, через которые пролегает путь
                int firstTop = points[end].paths[counter];//Первая промежуточная вершина
                int secondTop = points[end].paths[counter + 1];//Вторая промежуточная вершина
                lines[firstTop][secondTop] -= work; //Уменьшаем пропускную способность между этими вершинами
                lines[secondTop][firstTop] -= work;
            }
            return true;
        }
        void bypass(int number,int work)//Реализуем поиск кратчайшего пути через алгоритм Дейкстры, передаётся начальный узел передачи
        {
            int active_point = number;//Рассматриваемая вершина графа
            change_labels(active_point,work);//Изменение меток соседних вершин
            var enumeration = from element in points where element.label_l != 99999 && element.passing_l != true orderby element.label_l select element;//Ищем элементы, которые были помечены, но не были пройдены
            while (enumeration.Count() != 0)
            {
                foreach (var elements in enumeration)
                {
                    change_labels(elements.number_point_l - 1,work);//Изменение меток соседних вершин
                }
            }

        }
        void change_labels(int point,int work)//Изменение меток
        {
            if (point == begin)//Если вершина начальная
            {
                points[point].paths.Add(point);//Указываем, что из начальной вершины нет пути до самой себя
                for (int counter = 0; counter < lines[point].Length; counter++)
                {
                    if (counter == point) points[counter].label_l = 99999;//Путь из начального элемента до самого себя равен 0
                    else if (lines[point][counter] != 0 && points[counter].passing_l != true && lines[point][counter] >= work )//Если существует путь в вершину и она не пройденная, а также имеет подходящую пропускную способность, то изменяем её метку.
                    {
                        points[counter].label_l = lines[point][counter];
                        points[counter].paths.AddRange(points[point].paths);//Первоначальное заполнение путей
                        points[counter].paths.Add(counter);
                    }
                }
                points[point].passing_l = true;//Вершина пройдена
            }
            else//Если вершина не начальная
            {
                for (int counter = 0; counter < lines[point].Length; counter++)
                {
                    //Если существует путь в вершину и она не пройденная, а также, если сумма метки переданной вершины (point) и длины ребра до соседней вершины (counter) меньше,
                    //чем метка соседней вершины (counter), то изменяем метку соседней вершины (counter).
                    if (lines[point][counter] != 0 && points[counter].passing_l != true && (points[point].label_l + lines[point][counter]) < points[counter].label_l && lines[point][counter] >= work)
                    {
                        points[counter].label_l = points[point].label_l + lines[point][counter];
                        points[counter].paths.Clear();//Если метка стала меньше, то полностью изменяем путь (присваиваем путь той вершины, из которой получился более короткий путь)
                        points[counter].paths.AddRange(points[point].paths);
                        points[counter].paths.Add(counter);
                    }
                }
                points[point].passing_l = true;//Вершина пройдена
            }
        }
        void Reset()//Обнуляем метки и список с реконструированными путями для каждой вершины
        {
            for(int counter = 0; counter < points.Length; counter++)
            {
                points[counter].label_l = 99999;//Обнуляем метки
                points[counter].passing_l = false;//Переводим вершину в непройденное состояние
                points[counter].paths.Clear();//Обнуляем список вершин из реконструированного пути
            }
        }
    }
}
