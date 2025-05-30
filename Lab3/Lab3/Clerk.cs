﻿namespace Lab3
{
    internal class Clerk
    {
        // Список покупателей, которых клерк обслужил 
        public List<Customer> customers_service = new List<Customer>();
        // Очередь на кассу
        public Queue<Customer> customers_queue = new Queue<Customer>();

        // ------ Статистические данные -------
        public float service_time = 0;    // Время до возвращения клерка на кассу, для обслуживания следующих покупателей 
        public float time_work = 0;       // Общее время работы 
        public readonly int workload = 6; // Максимальное количество клиентов на обслуживание за один раз
        public int count_service = 0;     // Количество обслуженных
        public List<int> count_customers = new List<int>();  // Число обслуженных покупателей за один выход
    }
}
