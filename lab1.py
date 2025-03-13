import numpy as np
import matplotlib.pyplot as plt


# ЦПТ
def generate_normal_central_limit(n, size, a=2, sigma=1):
    uniform_samples = []
    # Генерация и подсчет суммы чисел. Генерация по n чисел из выборки size
    for _ in range(size):
        uniform_sum = np.sum(np.random.uniform(0, 1, n))
        uniform_samples.append(uniform_sum)
    normal_samples = (np.array(uniform_samples) - n / 2) / np.sqrt(n / 12)
    return a + sigma * normal_samples  # Приведение к N(2,1)


# Бокс-Маллера
def generate_normal_box_muller(size, a=2, sigma=1):
    # Два числа с равномерным распределением в интервале (0, 1)
    x1 = np.random.uniform(0, 1, size // 2)
    x2 = np.random.uniform(0, 1, size // 2)

    # Коэффициенты для пары yn и yn+1
    r = (-2 * np.log(x1)) ** 0.5
    theta = 2 * np.pi * x2

    # Пара некоррелированных, нормально распределённых случайных чисел
    y1 = r * np.cos(theta)
    y2 = r * np.sin(theta)

    # Объеденили массивы yn и yn+1
    normal_samples = np.concatenate((y1, y2))[:size]

    return a + sigma * normal_samples  # Приведение к N(2,1)


def empirical_cdf(sample, x):
    """Вычисляет эмпирическую функцию распределения (ЭФР)"""
    return np.sum(sample <= x) / len(sample)


def smirnov_test_manual(sample1, sample2):
    sample1 = np.sort(sample1)
    sample2 = np.sort(sample2)

    # Объединяем все значения из двух выборок
    combined = np.sort(np.concatenate((sample1, sample2)))

    # Вычисляем разности ЭФР для всех значений
    d_max = 0
    for x in combined:
        f1 = empirical_cdf(sample1, x)
        f2 = empirical_cdf(sample2, x)
        d_max = max(d_max, abs(f1 - f2))

    # Оценка критического значения для уровня значимости 0.05
    n1, n2 = len(sample1), len(sample2)
    critical_value = np.sqrt(abs(np.log(0.05) * (1/n1 + 1/n2) / 2))
    # Приближенная формула

    # Проверка гипотезы
    if d_max < critical_value:
        result = "Выборки принадлежат одной генеральной совокупности"
    else:
        result = "Выборки НЕ принадлежат одной генеральной совокупности"

    return d_max, critical_value, result


def gaps(sample, bins=10):
    # Определяем min и max
    min_val, max_val = min(sample), max(sample)

    # Определяем ширину интервала
    bin_width = (max_val - min_val) / bins

    # Создаём границы интервалов
    bin_edges = [min_val + i * bin_width for i in range(bins + 1)]

    # Подсчёт количества значений в каждом интервале
    bin_counts = [0] * bins  # Создаём массив для хранения количества элементов
    for value in sample:
        for i in range(bins):
            # Проверяем, попадает ли значение в интервал
            if bin_edges[i] <= value < bin_edges[i + 1]:
                bin_counts[i] += 1
                break
    # Добавляем значения в последний интервал
    bin_counts[-1] += sum(sample >= bin_edges[-2])
    return bin_edges, bin_counts, bin_width


# Создаем гистограмму
def manual_histogram(sample1, sample2, bins=10):
    bin_edges, bin_counts, bin_width = gaps(sample1, bins)
    bin_edges1, bin_counts1, bin_width1 = gaps(sample2, bins)
    d_max, critical_value, result = smirnov_test_manual(sample1, sample2)
    # Строим гистограмму
    plt.bar(bin_edges[:-1], bin_counts, width=bin_width,
            edgecolor="black", alpha=1, label="ЦПТ")
    plt.bar(bin_edges1[:-1], bin_counts1, width=bin_width1,
            edgecolor="black", alpha=0.7, label="Бокс-Маллер")
    plt.xlabel("Значение")
    plt.ylabel("Частота")
    plt.title("Гистограмма")
    plt.legend()
    info_text = f"{result}\nКритическое значение: {critical_value:.5f}\
        \nТеоритическое значение: {d_max:.5f}"
    plt.gcf().text(0.15, 0.75, info_text, fontsize=10,
                   bbox=dict(facecolor='white', alpha=0.5))

    plt.show()


# Генерация выборок
size = 10000
sample_clt = generate_normal_central_limit(12, size, 2, 1)
sample_box_muller = generate_normal_box_muller(size, 2, 1)

# Проверка критерием Пирсонаchi_squareclt = pearson_chi_square_test(sample_clt)

# print(pearson_chi_square_test(sample_clt))

manual_histogram(sample_clt, sample_box_muller, 200)
