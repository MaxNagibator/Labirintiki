namespace LabirintBlazorApp.Dto
{
    public class road
    {
        public road next; //ссылка на следующий элемент   
        public road prev; //ссылка на предыдущий элемент
        public int cur; //запись значения в список
        public road head = null; //ссылка на начало списка
        public road end = null; //ссылка на конец списка
        public road s = null; //ссылка на первый элемент
        public road()
        { //конструктор списка
            end = null;
        }
        public void add(int e)
        { //добавление элемента в список
            road l = new road();
            if (head == null)
            { //если список пуст
                l.cur = e;
                end = l;
                l.next = null;
                head = l;
                s = l;
            }
            else
            { //если в списке есть хотя бы один элемент
                s = end;
                end = l;
                s.next = l;
                l.cur = e;
                l.prev = s;
                l.next = null;
                s = l;
            }
        }
        public int getlast()
        {//ф-я возвращает последний элемент
            return end.cur;
        }
        public int size()
        { //ф-я возвращает размер списка
            int i = 0;
            road t = head;
            while (t != null)
            {
                i++;
                t = t.next;
            }
            return i;
        }
        public void print()
        { //ф-я выводит на экран весь список
            road t = head;
            while (t != null)
            {
                Console.Write(t.cur);
                t = t.next;
            }
        }
        public int deq()
        { //ф-я возвращает последний элемент при этом удаляя его из списка  
            int q;
            q = end.cur;
            if (size() > 1)
            {
                end = end.prev;
                end.next = null;
            }
            else
            {
                head = null;
                end = null;
            }
            return q;
        }
    }
}
