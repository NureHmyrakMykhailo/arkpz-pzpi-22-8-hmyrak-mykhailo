using System;

public class User
{
    public string Name { get; set; }
    public int Age { get; set; }
}

public class UserService
{
    // Оновлює ім'я користувача, видаляючи зайві пробіли та переводячи в верхній регістр
    public void UpdateUserName(User user)
    {
        user.Name = user.Name.Trim().ToUpper();
    }

    // Перевіряє, чи вік користувача достатній
    public void ValidateUserAge(User user)
    {
        if (user.Age < 18)
        {
            throw new InvalidOperationException("User is underage");
        }
    }

    // Зберігає дані користувача до бази даних (приклад)
    public void SaveToDatabase(User user)
    {
        Console.WriteLine($"User {user.Name} with age {user.Age} has been saved to the database.");
    }

    // Основний метод, який обробляє дані користувача
    public void ProcessUserData(User user)
    {
        UpdateUserName(user);
        ValidateUserAge(user);
        SaveToDatabase(user);
    }
}

class Program
{
    static void Main()
    {
        // Створюємо екземпляр користувача
        User user = new User { Name = " john doe ", Age = 20 };

        // Створюємо екземпляр сервісу для роботи з користувачами
        UserService userService = new UserService();

        try
        {
            // Викликаємо метод обробки даних користувача
            userService.ProcessUserData(user);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
