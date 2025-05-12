namespace Lab3
{
    internal class RandValue
    {
        private static Random random = new Random();

        public static float GetExponentialRandom(float mean)
        {
            // Параметр λ
            float lambda = 1.0f / mean;

            // Генерация равномерного числа от 0 до 1
            float u = (float)random.NextDouble();

            // Возвращаем значение по формуле экспоненциального распределения
            return (float)-Math.Log(1 - u) / lambda;
        }

        public static float GetRandomFloat(float min, float max)
        {
            float range = max - min;
            return (float)(random.NextDouble() * range + min);
        }

        public static float Normal(float M, float sigma)
        {
            int q = 100;

            float x = (float)random.NextDouble();
            for (int j = 0; j < q - 1; j++)
            {
                x += (float)random.NextDouble(); // возвращает равномерное [0, 1]
            }
            float z = (float)(x - q / 2) / (float)(Math.Sqrt(q / 12));
            float y = z * sigma + M;


            return y;
        }

    }
}
