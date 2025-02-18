import numpy as np
import scipy.stats as stats
import matplotlib.pyplot as plt

# Параметры
a = 2  # Среднее
sigma = 1  # Стандартное отклонение
n = 100  # Размер выборки

# Генерация первой выборки с использованием центральной предельной теоремы
sample1 = np.random.normal(loc=a, scale=sigma, size=n)

# Генерация второй выборки с использованием метода Бокса-Маллера
Xn = np.random.uniform(0, 1, n//2)
Xn1 = np.random.uniform(0, 1, n//2)

Y1 = np.sqrt(-2 * np.log(Xn)) * np.cos(2 * np.pi * Xn1)
Y2 = np.sqrt(-2 * np.log(Xn)) * np.sin(2 * np.pi * Xn1)

sample2 = a + sigma * np.concatenate((Y1, Y2))

# Проведение t-теста
t_stat, p_value = stats.ttest_ind(sample1, sample2)

# Визуализация
fig, axs = plt.subplots(3, 1, figsize=(10, 15))

# Гистограмма первой выборки
axs[0].hist(sample1, bins=20, alpha=0.5, color='blue', density=True, label='Выборка 1 (ЦПТ)')
axs[0].set_title('Выборка 1 (ЦПТ)')
axs[0].set_xlabel('Значения')
axs[0].set_ylabel('Плотность вероятности')
axs[0].legend()
axs[0].grid()

# Плотность распределения для первой выборки
xmin, xmax = axs[0].get_xlim()
x = np.linspace(xmin, xmax, 100)
p1 = stats.norm.pdf(x, a, sigma)
axs[0].plot(x, p1, 'b', linewidth=2)

# Гистограмма второй выборки
axs[1].hist(sample2, bins=20, alpha=0.5, color='orange', density=True, label='Выборка 2 (Бокс-Маллера)')
axs[1].set_title('Выборка 2 (Бокс-Маллера)')
axs[1].set_xlabel('Значения')
axs[1].set_ylabel('Плотность вероятности')
axs[1].legend()
axs[1].grid()

# Плотность распределения для второй выборки
p2 = stats.norm.pdf(x, a, sigma)
axs[1].plot(x, p2, 'orange', linewidth=2)

# Совместная гистограмма
axs[2].hist(sample1, bins=20, alpha=0.5, color='blue', density=True, label='Выборка 1 (ЦПТ)')
axs[2].hist(sample2, bins=20, alpha=0.5, color='orange', density=True, label='Выборка 2 (Бокс-Маллера)')
axs[2].set_title('Совместная гистограмма')
axs[2].set_xlabel('Значения')
axs[2].set_ylabel('Плотность вероятности')
axs[2].legend()
axs[2].grid()

# Общий заголовок
plt.suptitle('Сравнение двух выборок', fontsize=16)
plt.tight_layout(rect=[0, 0, 1, 0.96])  # Увеличиваем пространство для общего заголовка
plt.show()

# Вывод результатов t-теста
print(f"t-статистика: {t_stat}")
print(f"p-значение: {p_value}")

# Уровень значимости
alpha = 0.05
if p_value < alpha:
    print("Отвергаем нулевую гипотезу: выборки принадлежат разным генеральным совокупностям.")
else:
    print("Не отвергаем нулевую гипотезу: выборки могут принадлежать одной и той же генеральной совокупности.")
