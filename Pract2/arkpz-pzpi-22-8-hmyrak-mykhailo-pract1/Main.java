import java.util.Scanner;

class MaxFinder {
    // Функція для пошуку максимального числа
    public static int findMax(int[] numbers) {
        int max = Integer.MIN_VALUE;
        for (int number : numbers) {
            if (number > max) {
                max = number;
            }
        }
        return max;
    }

    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);

        try {
            // Запитуємо розмір масиву
            System.out.print("Введіть кількість чисел: ");
            int n = scanner.nextInt();

            // Перевірка на коректне введення розміру масиву
            if (n <= 0) {
                System.out.println("Розмір масиву має бути більше нуля.");
                return;
            }

            // Ініціалізуємо масив
            int[] numbers = new int[n];

            // Заповнюємо масив числами
            System.out.println("Введіть " + n + " чисел:");
            for (int i = 0; i < n; i++) {
                numbers[i] = scanner.nextInt();
            }

            // Знаходимо та виводимо максимальне число
            int maxNumber = findMax(numbers);
            System.out.println("Найбільше число: " + maxNumber);
        } catch (Exception e) {
            System.out.println("Виникла помилка введення. Перевірте правильність введених даних.");
        } finally {
            scanner.close();
        }
    }
}

