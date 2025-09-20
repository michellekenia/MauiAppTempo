private async void Button_Clicked_Previsao(object sender, EventArgs e)
{
    try
    {
        if (!string.IsNullOrEmpty(txt_cidade.Text))
        {
            Tempo? t = await DataService.GetPrevisao(txt_cidade.Text);

            if (t != null)
            {
                string dados_previsao = "";

                dados_previsao = $"Latitude: {t.lat} \n" +
                                 $"Longitude: {t.lon} \n" +
                                 $"Nascer do Sol: {t.sunrise} \n" +
                                 $"Por do Sol: {t.sunset} \n" +
                                 $"Temp Máx: {t.temp_max} \n" +
                                 $"Temp Min: {t.temp_min} \n" +
                                 $"Descrição: {t.description} \n" +
                                 $"Velocidade do Vento: {t.speed} m/s \n" +
                                 $"Visibilidade: {t.visibility} metros";

                lbl_res.Text = dados_previsao;

                string mapa = $"https://embed.windy.com/embed.html?..." +
                              $"&lat={t.lat.ToString().Replace(",", ".")}&lon={t.lon.ToString().Replace(",", ".")}";
                
                wv_mapa.Source = mapa;
            }
            else
            {
                lbl_res.Text = "Sem dados de Previsão";
            }
        }
        else
        {
            lbl_res.Text = "Preencha a cidade.";
        }
    }
    catch (Exception ex)
    {
        await DisplayAlert("Ops", ex.Message, "OK");
    }
}