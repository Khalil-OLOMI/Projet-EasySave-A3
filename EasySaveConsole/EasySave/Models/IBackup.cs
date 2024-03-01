namespace EasySave.Models;

public interface IBackup
{
    string Name { get; set; }
    string Source { get; set; }
    string Cible { get; set; }
    string Type { get; set; }
    string Status { get; set; }
    double Progression { get; set; }
    int NbreFile {  get; set; }
    long EncryptTime { get; set; }
    void Copy(string source, string cible);
    void Play();
    void Pause();
    void Stop();
    bool IsPaused();
    void Resume();
}
