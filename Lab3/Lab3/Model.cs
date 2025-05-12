using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Lab3
{
    internal class Model
    {
        private List<Clerk> workers; // Клерки за кассой 
        private int time = 480; // Время имитационной модели 
        private float mean = 2; // Математическое ожидание
        private int N = 50; // Число реализующих в интервале от 50 до 100
        private List<float> middle_wait_time; // Для подсчета среднего времени ожидания 
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
                    float tao = Rand_stats.GetExponentialRandom(mean);
                    middle_wait_time = new List<float>();
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
                        tao = Rand_stats.GetExponentialRandom(mean);
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
                                }
                            }
                            // Клерк в готовности обслужить новых клиентов
                            if (clerk.service_time == 0 && clerk.customers_queue.Count > 0)
                            {
                                int count_customer_service = clerk.customers_queue.Count > 6 ? 6 : clerk.customers_queue.Count; 
                                float time_go_to = Rand_stats.GetRandomFloat(0.5f, 1.5f); // Время пути к складу 
                                float time_go_back = Rand_stats.GetRandomFloat(0.5f, 1.5f); // Время пути к кассе
                                                                                            // Время обслуживания
                                float time_service = Rand_stats.Normal(3 * count_customer_service, 0.2f * count_customer_service);
                                float time_calculation = Rand_stats.GetRandomFloat(1f, 3f); // Время расчета 

                                clerk.service_time = time_go_to + time_service + time_go_back + time_calculation;

                                // Добавляем из очереди в буфер обслуживания 
                                for (int i = 0; i < count_customer_service; i++)
                                {
                                    clerk.customers_service.Add(clerk.customers_queue.ElementAt(i));
                                }
                                clerk.count_customers.Add(count_customer_service);
                            }
                        }
                        time_counter += deltaT;
                    }

                    middle_wait[p] = workers.Sum(c => c.middle_wait_time.Sum()) / workers.Sum(c => c.middle_wait_time.Count);

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
            return $"""
                Показатели эффективности
                Общее количество выполненных заказов: {workers.Sum(c => c.count_service)}
                Средняя загрузка клерков в часах: {workers.Sum(c => c.time_work) / workers.Count / 60}
                Средняя загрузка клерков в % от общего времени работы: {workers.Sum(c => c.time_work) / (workers.Count * time) * 100}
                Среднее число заявок, выполняемых за один выход в склад: {workers.Sum(c => c.count_customers.Sum()) / workers.Sum(c => c.count_customers.Count)}
                Среднее время ожидания клиентом выполнения заказа: {workers.Sum(c => c.middle_wait_time.Sum()) / workers.Sum(c => c.middle_wait_time.Count)}
                """;
        }

    }
}
