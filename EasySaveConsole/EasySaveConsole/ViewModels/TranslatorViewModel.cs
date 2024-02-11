using System;
using System.IO;
using System.Threading.Tasks;
using EasySaveConsole.Models;
using Newtonsoft.Json;

// Classe pour la traduction avec DeepL
public class DeepLTranslator
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    // Constructeur prenant une clé API en argument
    public DeepLTranslator(string apiKey)
    {
        _httpClient = new HttpClient();
        _apiKey = apiKey;

        // Définir l'en-tête d'autorisation pendant l'initialisation
        _httpClient.DefaultRequestHeaders.Add("Authorization", "DeepL-Auth-Key " + _apiKey);
    }

    // Méthode asynchrone pour traduire le texte
    public async Task<string> TranslateAsync(string text)
    {
        try
        {
            // Obtenir la langue cible à partir de la configuration
            var config = Config.LoadConfig();
            string targetLang = config.TargetLanguage;

            // Construire la requête
            var requestData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("text", text),
                new KeyValuePair<string, string>("target_lang", targetLang)
            });

            // Envoyer la requête à l'API DeepL
            var response = await _httpClient.PostAsync("https://api-free.deepl.com/v2/translate", requestData);
            response.EnsureSuccessStatusCode();

            // Lire et retourner le texte traduit
            var responseBody = await response.Content.ReadAsStringAsync();
            var translationResponse = JsonConvert.DeserializeObject<DeepLTranslationResponse>(responseBody);
            return translationResponse.Translations[0].Text;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur de traduction : {ex.Message}");
            return null;
        }
    }
}

// Classe représentant la réponse de traduction de DeepL
public class DeepLTranslationResponse
{
    [JsonProperty("translations")]
    public DeepLTranslation[] Translations { get; set; }
}

// Classe représentant une traduction individuelle
public class DeepLTranslation
{
    [JsonProperty("text")]
    public string Text { get; set; }
}
