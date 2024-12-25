using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SensorEmulator
{
    public partial class FormMain : Form
    {
        // Base URL of the API
        public string baseUrl;

        // Bearer token for authentication
        public string bearerToken;

        public FormMain()
        {
            InitializeComponent();
        }

        // Event handler for the login button
        private async void buttonLogin_Click(object sender, EventArgs e)
        {
            baseUrl = textBoxUrl.Text; // Get the API base URL from the text box
            string login = textBoxLogin.Text; // Get the username from the text box
            string password = textBoxPassword.Text; // Get the password from the text box

            // Attempt to retrieve a bearer token
            var token = await GetBearerTokenAsync(baseUrl, login, password);
            if (token != null)
            {
                bearerToken = token; // Store the token if login is successful
                MessageBox.Show("Login successful!");
            }
            else
            {
                MessageBox.Show("Login failed!"); // Show error if login fails
            }
        }

        // Method to retrieve a bearer token from the server
        private async Task<string?> GetBearerTokenAsync(string baseUrl, string login, string password)
        {
            using var client = new HttpClient();
            var requestUrl = $"{baseUrl}/auth/login"; // Construct the login endpoint URL

            var requestBody = new
            {
                username = login,
                password = password
            };

            // Serialize login data to JSON
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send the login request
            var response = await client.PostAsync(requestUrl, content);
            if (response.IsSuccessStatusCode) // Check if login is successful
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<JsonElement>(responseContent);

                // Extract the token from the response
                if (responseObject.TryGetProperty("token", out var tokenElement))
                {
                    return tokenElement.GetString();
                }
            }

            return null; // Return null if login fails
        }

        // Event handler for the send data button
        private async void buttonSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(bearerToken)) // Check if user is logged in
            {
                MessageBox.Show("Please login first.");
                return;
            }

            // Create a request object from the user input
            var climatMonitorRequest = new ClimatMonitorRequest
            {
                Temperature = double.TryParse(textBoxTemperature.Text, out var temp) ? temp : null,
                Wet = double.TryParse(textBoxWet.Text, out var wet) ? wet : null,
                Pressure = double.TryParse(textBoxPressure.Text, out var pressure) ? pressure : null
            };

            // Send the data to the API
            var success = await SendClimatMonitorDataAsync(baseUrl, climatMonitorRequest, bearerToken);
            if (success)
            {
                MessageBox.Show("Data sent successfully!");
            }
            else
            {
                MessageBox.Show("Failed to send data.");
            }
        }

        // Method to send climat monitor data to the server
        private async Task<bool> SendClimatMonitorDataAsync(string baseUrl, ClimatMonitorRequest request, string token)
        {
            using var client = new HttpClient();
            var requestUrl = $"{baseUrl}/climatmonitors"; // Construct the endpoint URL

            // Add the bearer token to the request headers
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Serialize the request object to JSON
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send the request
            var response = await client.PostAsync(requestUrl, content);
            return response.IsSuccessStatusCode; // Return true if the request is successful
        }

        // Event handler for form load (currently unused)
        private void FormMain_Load(object sender, EventArgs e)
        {
        }
    }

    // Class representing the structure of a climat monitor request
    public class ClimatMonitorRequest
    {
        public double? Temperature { get; set; } // Temperature parameter
        public double? Wet { get; set; }        // Wetness parameter
        public double? Pressure { get; set; }   // Pressure parameter
    }
}
