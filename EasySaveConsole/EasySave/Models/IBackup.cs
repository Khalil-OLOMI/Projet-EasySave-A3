﻿namespace EasySave.Models;

public interface IBackup
{
    string Name { get; set; }
    string Source { get; set; }
    string Cible { get; set; }
    string Type { get; set; }
    string Status { get; set; }
    void Copy(string source, string cible);
}
