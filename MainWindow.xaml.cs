using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace ApiColombia
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();
            ConfigureHttpClient();
            LoadDepartments();
        }

        private void ConfigureHttpClient()
        {
            _httpClient.BaseAddress = new Uri("https://api-colombia.com/api/v1/");
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private async void LoadDepartments()
        {
            try
            {
                var response = await _httpClient.GetAsync("Department");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var departments = JsonSerializer.Deserialize<List<Department>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                DepartmentComboBox.ItemsSource = departments;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando departamentos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DepartmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DepartmentComboBox.SelectedItem is Department selectedDepartment)
            {
                try
                {
                    var response = await _httpClient.GetAsync($"Department/{selectedDepartment.Id}/touristicattractions");
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    var attractions = JsonSerializer.Deserialize<List<TouristicAttraction>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    var detailWindow = new AttractionDetailWindow(attractions)
                    {
                        Owner = this,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };
                    detailWindow.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error cargando atracciones: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        
    }

    public class Department
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class TouristicAttraction
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<string> Images { get; set; } = new List<string>();

        public string MainImage => Images?.Count > 0 ?
            (Images[0].StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? Images[0]
                : $"https://api-colombia.com{Images[0]}")
            : string.Empty;
    }
}