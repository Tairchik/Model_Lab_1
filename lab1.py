import numpy as np


# ЦПТ
def generate_normal_central_limit(n, size, a=0, sigma=1):
    uniform_samples = []
    # Генерация и подсчет суммы чисел. Генерация по n чисел из выборки size
    for _ in range(size):
        uniform_sum = sum(np.random.uniform(0, 1, n)) 
        uniform_samples.append(uniform_sum)
    normal_samples = (np.array(uniform_samples) - n / 2) / np.sqrt(n / 12)
    return a + sigma * normal_samples  # Приведение к N(2,1)


# Бокс-Маллера
def generate_normal_box_muller(size, a=0, sigma=1):
    # Два числа с равномерным распределением в интервале (0, 1)
    x1 = np.random.uniform(0, 1, size // 2)
    x2 = np.random.uniform(0, 1, size // 2)

    # Коэффициенты для пары yn и yn+1
    r = (-2 * np.log(x1)) ** 0.5
    theta = 2 * np.pi * x2

    # Пара некоррелированных, нормально распределённых случайных чисел
    y1 = r * np.cos(theta)
    y2 = r * np.sin(theta) 

    # Объеденили
    normal_samples = np.concatenate((y1, y2))[:size]

    return a + sigma * normal_samples  # Приведение к N(2,1)


# Критерий Пирсона
def pearson_chi_square_test(sample, bins=10, mean=2, sigma=1):
    hist, bin_edges = np.histogram(sample, bins=bins)
    bin_widths = np.diff(bin_edges)
    expected_freq = []
    for i in range(len(bin_edges) - 1):
        exp_freq = (np.exp(-0.5 * ((bin_edges[i] - mean) / sigma) ** 2) / (
            sigma * np.sqrt(2 * np.pi))) * bin_widths[i] * len(sample)
        expected_freq.append(exp_freq)
    chi_square_stat = np.sum((hist - expected_freq) ** 2 / expected_freq)
    return chi_square_stat


# Генерация выборок
size = 1000
sample_clt = generate_normal_central_limit(12, size, 2, 1)
sample_box_muller = generate_normal_box_muller(size)

# Проверка критерием Пирсона
chi_square_clt = pearson_chi_square_test(sample_clt)
chi_square_box_muller = pearson_chi_square_test(sample_box_muller)

print("Пирсон CLT:", chi_square_clt)
print("Пирсон Box-Muller:", chi_square_box_muller)
