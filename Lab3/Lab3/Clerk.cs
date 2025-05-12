using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    internal class Clerk
    {
        // Список покупателей, которых клерк обслуживает в данный момент времени 
        public List<Customer> customers_service = new List<Customer>();
        // Очередь на кассу
        public Queue<Customer> customers_queue = new Queue<Customer>();

        // ------ Статистические данные -------
        public float service_time = 0;             // Время до возвращения клерка на кассу, для обслуживания следующих покупателей 
        public float time_work = 0;            // Общее время работы 
        public readonly int workload = 6;              // Максимальное количество клиентов на обслуживание за один раз
        public int count_service = 0;         // Количество обслуженных
        public List<int> count_customers = new List<int>();      // Число обслуженных покупателей за один выход
        public List<float> middle_wait_time = new List<float>(); // Для подсчета среднего времени ожидания 
    }
}
