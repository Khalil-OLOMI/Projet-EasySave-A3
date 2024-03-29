﻿using EasySave.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace EasySave.ViewModels;

public class StateViewModel
{
    private const string state_file = "state.json";
    private const string filePath = "state.xml";
    private static readonly object stateLock = new object();

    public StateViewModel()
    {
        if (!File.Exists(state_file))
        {
            File.Create(state_file).Close();
            string json = JsonConvert.SerializeObject(new List<State>(), Formatting.Indented);
            File.WriteAllText(state_file, json);
        }

        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ObservableCollection<State>));
            using StreamWriter writer = new StreamWriter(filePath);
            xmlSerializer.Serialize(writer, new ObservableCollection<State>());
        }
    }

    public List<State> ReadStateFile()
    {
        try
        {
            string jsonContent = File.ReadAllText(state_file);
            return JsonConvert.DeserializeObject<List<State>>(jsonContent);
        }
        catch (Exception ex)
        {
            return new List<State>();
        }
    }

    public void WriteState(State state)
    {
        lock (stateLock) // Lock access to the file
        {
            List<State> states = new List<State>();
            // Vérifier si le fichier existe déjà
            string json = File.ReadAllText(state_file);
            // Désérialiser la chaîne JSON en une liste d'objets
            states = JsonConvert.DeserializeObject<List<State>>(json);
            // Ajouter le nouvel objet à la liste
            states.Add(state);

            // Sérialiser la liste mise à jour en une chaîne JSON
            string updatedJson = JsonConvert.SerializeObject(states, Formatting.Indented);

            // Écrire la chaîne JSON mise à jour dans le fichier
            File.WriteAllText(state_file, updatedJson);
        }
    }
    public static long GetDirectorySize(string directoryPath)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
        if (!directoryInfo.Exists)
        {
            return 0;
        }
        else
        {
            long directorySize = 0;

            // Add size of all files in the directory
            foreach (FileInfo file in directoryInfo.GetFiles("*", SearchOption.AllDirectories))
            {
                directorySize += file.Length;
            }

            return directorySize;
        }
    }

    public static double GetProgression(string source, int nbFiles)
        => (double)(FileNbre(source) - (FileNbre(source) - nbFiles)) / FileNbre(source) * 100;


    public static int FileNbre(string dossier)
    {
        int totalFichiers = 0;

        // Obtenez la liste des fichiers dans le dossier
        string[] fichiers = Directory.GetFiles(dossier);
        totalFichiers += fichiers.Length;

        // Obtenez la liste des sous-dossiers dans le dossier
        string[] sousDossiers = Directory.GetDirectories(dossier);

        // Parcourez récursivement les sous-dossiers pour compter les fichiers
        foreach (string sousDossier in sousDossiers)
        {
            totalFichiers += FileNbre(sousDossier);
        }

        return totalFichiers;
    }

    public ObservableCollection<State> ReadXMLState()
    {
        try
        {
            // Lire le contenu du fichier XML dans une chaîne
            string xml = File.ReadAllText(filePath);

            // Désérialiser la chaîne XML en une liste d'objets Log
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<State>));
            using (StringReader reader = new StringReader(xml))
            {
                return (ObservableCollection<State>)serializer.Deserialize(reader);
            }
        }
        catch (Exception ex)
        {
            return new ObservableCollection<State>();
        }
    }

    public void WriteStateXml(State state)
    {
        ObservableCollection<State> states = ReadXMLState();

        // Ajouter le nouvel objet à la liste
        states.Add(state);

        // Sérialiser la liste mise à jour en une chaîne XML
        XmlSerializer xmlSerializer = new(typeof(ObservableCollection<State>));
        using StreamWriter writer = new(filePath);

        xmlSerializer.Serialize(writer, states);
    }
}
