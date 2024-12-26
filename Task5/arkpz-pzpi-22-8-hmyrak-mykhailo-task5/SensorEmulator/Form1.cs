using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SensorEmulator
{
    public partial class FormMain : Form
    {
        public string baseUrl;
        public string bearerToken;

        public FormMain()
        {
            InitializeComponent();
        }

        private async void buttonLogin_Click(object sender, EventArgs e)
        {
            baseUrl = textBoxUrl.Text;
            string login = textBoxLogin.Text;
            string password = textBoxPassword.Text;

            var token = await GetBearerTokenAsync(baseUrl, login, password);
            if (token != null)
            {
                bearerToken = token;
                MessageBox.Show("Login successful!");
            }
            else
            {
                MessageBox.Show("Login failed!");
            }
        }

        private async Task<string?> GetBearerTokenAsync(string baseUrl, string login, string password)
        {
            using var client = new HttpClient();
            var requestUrl = $"{baseUrl}/auth/login";

            var requestBody = new
            {
                username = login,
                password = password
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(requestUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<JsonElement>(responseContent);
                if (responseObject.TryGetProperty("token", out var tokenElement))
                {
                    return tokenElement.GetString();
                }
            }

            return null;
        }

        private async void buttonSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(bearerToken))
            {
                MessageBox.Show("Please login first.");
                return;
            }

            var climatMonitorRequest = new ClimatMonitorRequest
            {
                Temperature = double.TryParse(textBoxTemperature.Text, out var temp) ? temp : null,
                Wet = double.TryParse(textBoxWet.Text, out var wet) ? wet : null,
                Pressure = double.TryParse(textBoxPressure.Text, out var pressure) ? pressure : null
            };

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

        private async Task<bool> SendClimatMonitorDataAsync(string baseUrl, ClimatMonitorRequest request, string token)
        {
            using var client = new HttpClient();
            var requestUrl = $"{baseUrl}/climatmonitors";

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(requestUrl, content);
            return response.IsSuccessStatusCode;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
        }
    }

    public class ClimatMonitorRequest
    {
        public double? Temperature { get; set; }
        public double? Wet { get; set; }
        public double? Pressure { get; set; }
    }
}
