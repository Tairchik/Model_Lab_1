namespace Lab3
{
    internal class Model
    {
        private List<Clerk> workers; // Клерки за кассой 
        private int time = 480; // Время имитационной модели 
        private float mean = 2; // Математическое ожидание
        private int N = 100; // Число реализующих в интервале от 50 до 100
        private int num_workers; // Число клерков
        private int N_new = 0;

        public Model(int n = 3)
        {
            if (n <= 0)
            {
                throw new ArgumentException("Отрицательное число клерков");
            }
            
            num_workers = n;
        }

        public void StartModel()
        {
            Random random = new Random();
            int k = 0;
            float quantile = 2.7f;
            float epsilon = 0.1f;
            do
            {
                float[] middle_wait = new float[N];
                // Выполняем N раз для минимизирования погрешности
                for (int p = 0; p < N; p++)
                {
                    // Случайное время прихода
                    float tao = RandValue.GetExponentialRandom(mean);
                    workers = new List<Clerk>();
                    List<Customer> customers = new List<Customer>();


                    for (int i = 0; i < num_workers; i++)
                    {
                        workers.Add(new Clerk());
                    }

                    float time_counter = tao;
                    List<float> arrayTao = new List<float>();

                    while (time > time_counter)
                    {
                        // Пришел клиент
                        arrayTao.Add(tao);
                        Customer customer = new Customer();
                        customer.enter = time_counter;
                        customers.Add(customer);

                        // Находим минимальную длину очереди
                        int minQueueLength = workers.Min(c => c.customers_queue.Count);

                        // Выбираем всех клерков с этой длиной очереди
                        var availableClerks = workers.Where(c => c.customers_queue.Count == minQueueLength).ToList();

                        // Если один — выбираем его, если несколько — случайный из них
                        Clerk selectedClerk = availableClerks[random.Next(availableClerks.Count)];

                        // Добавляем в очередь
                        selectedClerk.customers_queue.Enqueue(customer);

                        // Расчет времени ухода клиента(-ов)
                        foreach (var clerk in workers)
                        {
                            // Если клерк закончил обслуживание покупателей
                            if (clerk.service_time <= time_counter)
                            {
                                // Считаем сколько нужно обслужить за раз из буфера (от 1 до 6)
                                int count_wait_clients = clerk.customers_queue.Count <= clerk.workload ? clerk.customers_queue.Count : clerk.workload;

                                if (count_wait_clients != 0)
                                {
                                    float time_go_to = RandValue.GetRandomFloat(0.5f, 1.5f); // Время пути к складу 
                                    float time_go_back = RandValue.GetRandomFloat(0.5f, 1.5f); // Время пути к кассе
                                                                                                // Время обслуживания
                                    float time_service = RandValue.Normal(3 * count_wait_clients, 0.2f * count_wait_clients);
                                    float time_calculation = RandValue.GetRandomFloat(1f, 3f); // Время расчета 

                                    // Остановка счетчика (моделирование окончено)
                                    if (time_go_to + time_service + time_go_back + time_calculation + time_counter > time)
                                    {
                                        break;
                                    }

                                    clerk.time_work += time_go_to + time_service + time_go_back + time_calculation + time_counter - clerk.service_time;

                                    // Время конца обслуживания клерка
                                    clerk.service_time = time_go_to + time_service + time_go_back + time_calculation + time_counter;

                                    for (int i = 0; i < count_wait_clients; i++)
                                    {
                                        Customer customer_exit = clerk.customers_queue.Dequeue();
                                        customer_exit.exit = clerk.service_time;
                                        clerk.customers_service.Add(customer_exit);
                                    }

                                    clerk.count_service += count_wait_clients;
                                    clerk.count_customers.Add(count_wait_clients);
                                }
                            }
                        }

                        tao = RandValue.GetExponentialRandom(mean);
                        time_counter += tao;
                    }

                    float middleWait = 0;

                    foreach (var clerk in workers)
                    {
                        middleWait += (float) clerk.customers_service.Sum(c => c.exit - c.enter) / clerk.customers_service.Count;
                    }
                    middleWait = middleWait / workers.Count;

                    middle_wait[p] = middleWait;

                    Console.WriteLine(GetStatisticByClerks());
                    Console.WriteLine();

                }

                // Оценка вероятности наступления 
                float m = middle_wait.Sum() / N;
                float d = 0;

                for (int i = 0; i < N; i++)
                {
                    d += (middle_wait[i] - m) * (middle_wait[i] - m);
                }

                d /= N;

                N_new = Convert.ToInt32(quantile * quantile * d / (epsilon * epsilon));

                Console.WriteLine($"Математическое ожидание: {m}\n<----------------------->");

                if (N_new > N)
                {
                    N = N_new;
                }
            } while (N_new >= N);
            Console.WriteLine($"Итоговое число N: {N}");
        }
        public string GetStatisticByClerks() 
        {
            float middle_count_customers = 0;
            float middle_wait_time = 0;
            foreach (var clerk in workers)
            {
                middle_wait_time += (float) clerk.customers_service.Sum(c => c.exit - c.enter) / clerk.customers_service.Count;
                middle_count_customers += (float) clerk.count_customers.Sum() / clerk.count_customers.Count;
            }
            middle_wait_time /= workers.Count;
            middle_count_customers /= workers.Count;

            return $"""
                Показатели эффективности
                Общее количество выполненных заказов: {workers.Sum(c => c.count_service)}
                Средняя загрузка клерков в часах: {workers.Sum(c => c.time_work) / workers.Count / 60}
                Средняя загрузка клерков в % от общего времени работы: {workers.Sum(c => c.time_work) / (workers.Count * time) * 100}
                Среднее число заявок, выполняемых за один выход в склад: {middle_count_customers}
                Среднее время ожидания клиентом выполнения заказа: {middle_wait_time}
                """;
        }

    }
}
