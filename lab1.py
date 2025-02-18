import matplotlib.pyplot as plt
import numpy as np

# Генерируем данные
x = np.linspace(0, 10, 100)
y = np.sin(x)

# Строим график
plt.plot(x, y, label='sin(x)', color='b')
plt.xlabel('X')
plt.ylabel('Y')
plt.title('График функции sin(x)')
plt.legend()
plt.grid()

# Показываем график
plt.show()
