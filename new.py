import random


n = 100
m = 2
sigma = 1

random_nums = []

for i in range(n):
    random_nums.append(random.uniform(0, 1))
summa = sum(random_nums)
z = (summa - m) / sigma