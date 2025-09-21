using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;
using System;
using System.Diagnostics;
using System.Linq;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked_Previsao(object sender, EventArgs e)
        {
            string cidade = txt_cidade.Text?.Trim() ?? "";

            if (string.IsNullOrEmpty(cidade))
            {
                lbl_res.Text = "Preencha a cidade.";
                await DisplayAlert("Atenção", "Digite o nome da cidade para buscar a previsão.", "OK");
                return;
            }

            lbl_res.Text = $"Buscando dados para '{cidade}'...";

            try
            {
                Tempo t = await DataService.GetPrevisao(cidade);

                lbl_res.Text =
                    $"Latitude: {t.lat}\n" +
                    $"Longitude: {t.lon}\n" +
                    $"Nascer do Sol: {t.sunrise}\n" +
                    $"Por do Sol: {t.sunset}\n" +
                    $"Temp Máx: {t.temp_max}°C\n" +
                    $"Temp Min: {t.temp_min}°C\n" +
                    $"Descrição: {t.description}\n" +
                    $"Velocidade do Vento: {t.speed} m/s\n" +
                    $"Visibilidade: {t.visibility} m";

                string mapa = $"https://embed.windy.com/embed.html?" +
                              $"type=map&location=coordinates&metricRain=mm&metricTemp=°C" +
                              $"&metricWind=km/h&zoom=5&overlay=wind&product=ecmwf&level=surface" +
                              $"&lat={t.lat?.ToString().Replace(",", ".")}&lon={t.lon?.ToString().Replace(",", ".")}";

                wv_mapa.Source = mapa;
                Debug.WriteLine(mapa);
            }
            catch (KeyNotFoundException ex)
            {
                lbl_res.Text = $"Erro: {ex.Message}";
                await DisplayAlert("Cidade não encontrada", ex.Message, "OK");
            }
            catch (InvalidOperationException ex)
            {
                lbl_res.Text = $"Erro: {ex.Message}";
                await DisplayAlert("Sem conexão", ex.Message, "OK");
            }
            catch (UnauthorizedAccessException ex)
            {
                lbl_res.Text = $"Erro: {ex.Message}";
                await DisplayAlert("Erro de API", ex.Message, "OK");
            }
            catch (HttpRequestException ex)
            {
                lbl_res.Text = $"Erro: {ex.Message}";
                await DisplayAlert("Erro de comunicação", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                lbl_res.Text = $"Erro inesperado: {ex.Message}";
                await DisplayAlert("Erro inesperado", ex.Message, "OK");
            }
        }

        private async void Button_Clicked_Localizacao(object sender, EventArgs e)
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                Location? local = await Geolocation.Default.GetLocationAsync(request);

                if (local != null)
                {
                    lbl_coords.Text = $"Latitude: {local.Latitude}\nLongitude: {local.Longitude}";
                    await GetCidade(local.Latitude, local.Longitude);
                }
                else
                {
                    lbl_coords.Text = "Nenhuma localização encontrada.";
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Erro: Dispositivo não suporta", fnsEx.Message, "OK");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Erro: Localização desabilitada", fneEx.Message, "OK");
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Erro: Permissão da localização", pEx.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro inesperado", ex.Message, "OK");
            }
        }

        private async Task GetCidade(double lat, double lon)
        {
            try
            {
                var places = await Geocoding.Default.GetPlacemarksAsync(lat, lon);
                var place = places.FirstOrDefault();
                if (place != null)
                    txt_cidade.Text = place.Locality;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro: Obtenção do nome da cidade", ex.Message, "OK");
            }
        }
    }
}
