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
                        tao = RandValue.GetExponentialRandom(mean);
                        time_counter += tao;
                    }

                    // Расчет delta t
                    float a = arrayTao.Sum() / customers.Count; // Мат. ожидание

                    float deltaT = a / 10;

                    time_counter = 0;
                    int u = 0;
                    // Имитационная модель
                    while (time > time_counter)
                    {
                        if (time_counter >= customers[u].enter)
                        {
                            // Пришел клиент
                            Customer customer = customers[u];

                            // Определяем к какому продавцу отправится клиент
                            // 1. Если есть продавец с наименьшей очередью
                            // 2. Если наименьшей очереди нет, то случайно

                            // Находим минимальную длину очереди
                            int minQueueLength = workers.Min(c => c.customers_queue.Count);

                            // Выбираем всех клерков с этой длиной очереди
                            var availableClerks = workers.Where(c => c.customers_queue.Count == minQueueLength).ToList();

                            // Если один — выбираем его, если несколько — случайный из них
                            Clerk selectedClerk = availableClerks[random.Next(availableClerks.Count)];

                            // Добавляем в очередь
                            selectedClerk.customers_queue.Enqueue(customer);

                            u++;
                            if (u >= customers.Count) break; 
                        }

                        // Алгоритм работы клерков
                        foreach (var clerk in workers)
                        {
                            // Клерк обслуживает
                            if (clerk.service_time > 0)
                            {
                                clerk.service_time -= deltaT;
                                clerk.time_work += deltaT;

                                // Обслужил?
                                if (clerk.service_time <= 0)
                                {
                                    int count_customers_service = clerk.customers_service.Count;
                                    clerk.customers_service.Clear(); // Освобождаем клерка
                                    clerk.count_service += count_customers_service;
                                    clerk.service_time = 0;

                                    // Запоминаем время обслуживание перед освобождением очереди
                                    for (int i = 0; i < count_customers_service; i++)
                                    {
                                        clerk.middle_wait_time.Add(time_counter - clerk.customers_queue.ElementAt(i).enter);
                                    }
                                    // Освобождаем очередь клерка
                                    for (int i = 0; i < count_customers_service; i++)
                                    {
                                        clerk.customers_queue.Dequeue();
                                    }

                                    clerk.count_customers.Add(count_customers_service);
                                }
                            }
                            // Клерк в готовности обслужить новых клиентов
                            if (clerk.service_time == 0 && clerk.customers_queue.Count > 0)
                            {
                                int count_customer_service = clerk.customers_queue.Count >= 6 ? 6 : clerk.customers_queue.Count; 
                                float time_go_to = RandValue.GetRandomFloat(0.5f, 1.5f); // Время пути к складу 
                                float time_go_back = RandValue.GetRandomFloat(0.5f, 1.5f); // Время пути к кассе
                                                                                            // Время обслуживания
                                float time_service = RandValue.Normal(3 * count_customer_service, 0.2f * count_customer_service);
                                float time_calculation = RandValue.GetRandomFloat(1f, 3f); // Время расчета 

                                clerk.service_time = time_go_to + time_service + time_go_back + time_calculation;

                                // Добавляем из очереди в буфер обслуживания 
                                for (int i = 0; i < count_customer_service; i++)
                                {
                                    clerk.customers_service.Add(clerk.customers_queue.ElementAt(i));
                                }
                            }
                        }
                        time_counter += deltaT;
                    }

                    float result_middle_wait_time = 0;
                    foreach (var clerk in workers)
                    {
                        result_middle_wait_time += clerk.middle_wait_time.Sum() / clerk.middle_wait_time.Count;
                    }
                    result_middle_wait_time /= num_workers;


                    middle_wait[p] = result_middle_wait_time;

                    Console.WriteLine(GetStatisticByOneClerk());
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
        public string GetStatisticByOneClerk() 
        {
            float middle_count_customers = 0;
            float middle_wait_time = 0;
            foreach (var clerk in workers)
            {
                middle_wait_time += (float) clerk.middle_wait_time.Sum() / clerk.middle_wait_time.Count;
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
