using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace RenommerEnFormatPhrase
{
    public class Fichiers
    {
        public Fichiers() { }
        public Fichiers(string nom, string chemin, long taille, DateTime creation, DateTime modification, string extension, bool repertoire)
        {
            Nom = nom;
            Chemin = chemin;
            Taille = taille;
            Creation = creation;
            Modification = modification;
            Extension = extension;
            Repertoire = repertoire;
        }
        public string Nom { get; set; }
        public string Chemin { get; set; }
        public long Taille { get; set; }
        public DateTime Creation { get; set; }
        public DateTime Modification { get; set; }
        public string Extension { get; set; }
        public bool Repertoire { get; set; }
        public override string ToString()
        {
            return $"{Nom} ({Taille} octets)";
        }
    }
    public static class FileUtils
    {
        public static List<Fichiers> ListFiles(string folder, bool recursive = false)
        {
            var result = new List<Fichiers>();
            if (!Directory.Exists(folder)) return result;
            var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            try
            {
                foreach (var path in Directory.GetFileSystemEntries(folder, "*.*", option))
                {
                    var attr = File.GetAttributes(path);
                    bool isDir = attr.HasFlag(FileAttributes.Directory);
                    result.Add(new Fichiers(Path.GetFileName(path), path, isDir ? 0 : new FileInfo(path).Length, File.GetCreationTime(path),
                        File.GetLastWriteTime(path), isDir ? "" : Path.GetExtension(path), isDir));
                }
            }
            catch (Exception ex) { Console.WriteLine("Erreur lors de l'enumeration : " + ex.Message); }
            return result;
        }
    }
    class Program
    {
        public static string BrowseForFolder()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Sélectionnez un dossier";
                dialog.ShowNewFolderButton = true;
                if (dialog.ShowDialog() == DialogResult.OK) return dialog.SelectedPath;
                return null;
            }
        }
        public string RetireUneSection(string valeur, int debut, int fin)
        {
            if (valeur.Length > 0)
            {
                if (fin > valeur.Length)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Erreur d'allocation la valeur de fin ne peut pas etre supperieure a la longueur de la chaine");
                    return valeur;
                }
                if (debut < 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Erreur d'allocation la valeur de commencement ne peut pas etre negative");
                    return valeur;

                }
                if (fin == valeur.Length) return valeur.Substring(valeur.Length - debut, fin);
            }
            return valeur;
        }
        public static void KillerDossiersGit(string chemin)
        {
            string git = chemin + ".git";
            try
            {
                if (Directory.Exists(git))
                {
                    foreach (string file in Directory.GetFiles(git, "*", SearchOption.AllDirectories))
                    {
                        File.SetAttributes(file, FileAttributes.Normal);//bye protection lecture seule
                    }
                    try
                    {
                        Directory.Delete(git, true);
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine($"Dossier {git} supprimé.");
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Erreur : " + ex.Message);
                        return;
                    }

                }
                else { return; }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }
        public static void ListerDossiersVS(string chemin)
        {
            string git = chemin + ".vs";
            try
            {
                if (Directory.Exists(git))
                {
                    foreach (string file in Directory.GetFiles(git, "*", SearchOption.AllDirectories))
                    {
                        File.SetAttributes(file, FileAttributes.Normal);//bye protection lecture seule
                    }
                    try
                    {
                        Directory.Delete(git, true);
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"Dossier {git} supprimé.");
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Erreur : " + ex.Message);
                        return;
                    }

                }
                else { return; }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }
        public static string FormatagePhrase(string valeur)
        {
            try
            {
                if (valeur != string.Empty)
                {
                    if (valeur.Length > 3)
                    {
                        return valeur.Substring(0, 1).ToUpper() + valeur.Substring(1, valeur.Length - 1).ToLower();
                    }
                    else { return valeur.ToUpper(); }
                }
                return string.Empty;
            }
            catch
            {
                Console.WriteLine(new Exception("La valeur de formatage est vide").Source);
                return string.Empty;
            }
        }
        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "RenommerEnFormatPhrase\tv:1.00.1\t(C) Patrice Waechter-Ebling";
            Console.WriteLine(Console.Title);
            Console.WriteLine("Execution: " + Application.ExecutablePath);
               string Dossier = BrowseForFolder() ?? "";
             string log = Dossier.Substring(0, 3).ToUpper() + Dossier.Substring(3, Dossier.Length - 3).ToLower() + "\\ProjetsVS.cmd";
            StreamWriter st = new StreamWriter(log, true, System.Text.Encoding.ASCII);
            Console.WriteLine(Dossier);
            if (Dossier != string.Empty)
            {
                Console.WriteLine($"Alayse: {Dossier}\nLog VS {log}\n");
                var items = FileUtils.ListFiles(Dossier, true);
                foreach (var item in items)
                {
                    if (item.Repertoire)
                    {
                        string root = item.Chemin+"\\";
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        if (item.Nom.ToLower() == ".vs")
                        {
                            st.WriteLine($"rd /s /q \"{item.Chemin}\"");
                        }
                        Console.WriteLine($"{item.Extension}\t{item.Modification}\t{FormatagePhrase(item.Nom)}");
                        KillerDossiersGit(root);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\t{item.Modification}\t{item.Extension}\t{item.Taille} octets\t{FormatagePhrase(item.Nom)}");
                    }
                }
            }
            //    Directory.Move(Dossier, Dossier1); 
            Console.ForegroundColor = ConsoleColor.White;
            st.Close();
            Console.WriteLine("Appuyez sur Entree pour quiter");
            Console.ReadLine();

        }
    }
}