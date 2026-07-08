using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ConsoleApp1
{
    class Dyct
    {
        public string FLanguage { get; set; } // From language
        public string TLanguage { get; set; } // To language
        public Dictionary<string, List<string>> Words { get; set; }

        public Dyct(string fLanguage, string tLanguage)
        {
            FLanguage = fLanguage;
            TLanguage = tLanguage;
            Words = new Dictionary<string, List<string>>();
        }

        public Dyct() { }

        public string Name => $"{FLanguage}-{TLanguage}";

        public void AddWord(string word, List<string> translations)
        {
            word = word.ToLower().Trim();
            if (!Words.ContainsKey(word))
            {
                Words.Add(word, translations);
                Console.WriteLine($"Word '{word}' successfully added.");
            }
            else
            {
                Console.WriteLine($"Word '{word}' already exists. Use the add translation option instead.");
            }
        }
        public void AddTranslation(string word, string translation)
        {
            word = word.ToLower().Trim();
            if (Words.ContainsKey(word))
            {
                if (!Words[word].Contains(translation))
                {
                    Words[word].Add(translation);
                    Console.WriteLine($"Translation '{translation}' added to the word '{word}'.");
                }
                else
                {
                    Console.WriteLine("This translation already exists.");
                }
            }
            else
            {
                Console.WriteLine($"Word '{word}' not found.");
            }
        }

        public void RemoveWord(string word)
        {
            word = word.ToLower().Trim();
            if (Words.Remove(word))
            {
                Console.WriteLine($"Word '{word}' and all its translations have been removed.");
            }
            else
            {
                Console.WriteLine($"Word '{word}' not found.");
            }
        }
        public void RemoveTranslation(string word, string translation)
        {
            word = word.ToLower().Trim();
            if (Words.ContainsKey(word))
            {
                var translations = Words[word];

                if (translations.Count <= 1 && translations.Contains(translation))
                {
                    Console.WriteLine("Error! You cannot delete the last remaining translation for this word.");
                    return;
                }

                if (translations.Remove(translation))
                {
                    Console.WriteLine($"Translation '{translation}' removed for the word '{word}'.");
                }
                else
                {
                    Console.WriteLine($"Translation '{translation}' not found.");
                }
            }
            else
            {
                Console.WriteLine($"Word '{word}' not found.");
            }
        }

        public void ChangeWord(string oldWord, string newWord)
        {
            oldWord = oldWord.ToLower().Trim();
            newWord = newWord.ToLower().Trim();

            if (Words.ContainsKey(oldWord))
            {
                if (Words.ContainsKey(newWord))
                {
                    Console.WriteLine($"Word '{newWord}' already exists in the dictionary.");
                    return;
                }
                var translations = Words[oldWord];
                Words.Remove(oldWord);
                Words.Add(newWord, translations);
                Console.WriteLine($"Word '{oldWord}' changed to '{newWord}'.");
            }
            else
            {
                Console.WriteLine($"Word '{oldWord}' not found.");
            }
        }
        public void ChangeTranslation(string word, string oldTranslation, string newTranslation)
        {
            word = word.ToLower().Trim();
            if (Words.ContainsKey(word))
            {
                var translations = Words[word];
                int index = translations.IndexOf(oldTranslation);
                if (index != -1)
                {
                    translations[index] = newTranslation;
                    Console.WriteLine($"Translation successfully changed to '{newTranslation}'.");
                }
                else
                {
                    Console.WriteLine($"Translation '{oldTranslation}' not found.");
                }
            }
            else
            {
                Console.WriteLine($"Word '{word}' not found.");
            }
        }

        public void SearchWord(string word)
        {
            word = word.ToLower().Trim();
            if (Words.ContainsKey(word))
            {
                Console.WriteLine($"Translations for '{word}': {string.Join(", ", Words[word])}");
            }
            else
            {
                Console.WriteLine($"Word '{word}' not found in this dictionary.");
            }
        }
        public void ExportWord(string word)
        {
            word = word.ToLower().Trim();
            if (Words.ContainsKey(word))
            {
                string fileName = $"{word}_export.txt";
                string content = $"{word} ({Name}) -> {string.Join(", ", Words[word])}";
                File.WriteAllText(fileName, content);
                Console.WriteLine($"Result successfully exported to file: {fileName}");
            }
            else
            {
                Console.WriteLine($"Word '{word}' not found for export.");
            }
        }
        public void Show()
        {
            Console.WriteLine($"\n--- Dictionary ({FLanguage} -> {TLanguage}) ---");
            if (Words.Count == 0) Console.WriteLine("The dictionary is empty.");
            foreach (var entry in Words)
            {
                Console.WriteLine($"{entry.Key} — {string.Join(", ", entry.Value)}");
            }
        }
    }

    class Menu
    {
        public static void ShowMainMenu()
        {
            Console.WriteLine("\n=== MAIN MENU ===");
            Console.WriteLine("1. Create a new dictionary");
            Console.WriteLine("2. Choose a dictionary to work with");
            Console.WriteLine("3. Exit");
        }

        public static void ShowDictionaryMenu(string dictName)
        {
            Console.WriteLine($"\n=== WORKING WITH DICTIONARY [{dictName}] ===");
            Console.WriteLine("1. Add word");
            Console.WriteLine("2. Add translation variant");
            Console.WriteLine("3. Change word");
            Console.WriteLine("4. Change translation");
            Console.WriteLine("5. Remove word");
            Console.WriteLine("6. Remove translation");
            Console.WriteLine("7. Search translation");
            Console.WriteLine("8. Export word to file");
            Console.WriteLine("9. Show entire dictionary");
            Console.WriteLine("0. Back to main menu");
        }
    }

    internal class Program
    {
        private const string DataFolder = "Dictionaries";
        static List<Dyct> dictionaries = new List<Dyct>();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.Unicode;

            LoadDictionaries();

            bool exitApp = false;
            while (!exitApp)
            {
                Console.Clear();
                Menu.ShowMainMenu();
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateDictionary();
                        break;
                    case "2":
                        SelectAndWorkWithDictionary();
                        break;
                    case "3":
                        SaveDictionaries();
                        exitApp = true;
                        Console.WriteLine("Thank you for using the application!");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        PauseForUser();
                        break;
                }
            }
        }

        static void CreateDictionary()
        {
            Console.Write("Enter source language (e.g., English): ");
            string fromLang = Console.ReadLine().Trim();
            Console.Write("Enter target language (e.g., Ukrainian): ");
            string toLang = Console.ReadLine().Trim();

            if (string.IsNullOrEmpty(fromLang) || string.IsNullOrEmpty(toLang))
            {
                Console.WriteLine("Language names cannot be empty!");
                PauseForUser();
                return;
            }

            var newDict = new Dyct(fromLang, toLang);
            dictionaries.Add(newDict);
            SaveDictionaries();
            Console.WriteLine($"Dictionary '{newDict.Name}' successfully created and saved!");
            PauseForUser();
        }

        static void SelectAndWorkWithDictionary()
        {
            if (dictionaries.Count == 0)
            {
                Console.WriteLine("No dictionaries available. Please create at least one.");
                PauseForUser();
                return;
            }

            Console.WriteLine("\nAvailable Dictionaries:");
            for (int i = 0; i < dictionaries.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {dictionaries[i].Name}");
            }

            Console.Write("Select dictionary number: ");
            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > dictionaries.Count)
            {
                Console.WriteLine("Invalid number.");
                PauseForUser();
                return;
            }

            Dyct currentDict = dictionaries[index - 1];
            bool backToMain = false;

            while (!backToMain)
            {
                Console.Clear();
                Menu.ShowDictionaryMenu(currentDict.Name);
                Console.Write("Select a sub-menu option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter word: ");
                        string word = Console.ReadLine()?.Trim();
                        Console.Write("Enter the first translation variant: ");
                        string trans = Console.ReadLine()?.Trim();

                        if (!string.IsNullOrEmpty(word) && !string.IsNullOrEmpty(trans))
                        {
                            currentDict.AddWord(word, new List<string> { trans });
                            SaveDictionaries();
                        }
                        else
                        {
                            Console.WriteLine("Word or translation cannot be empty!");
                        }
                        PauseForUser();
                        break;

                    case "2":
                        Console.Write("Enter existing word: ");
                        string w2 = Console.ReadLine()?.Trim();
                        Console.Write("Enter additional translation: ");
                        string t2 = Console.ReadLine()?.Trim();

                        if (!string.IsNullOrEmpty(w2) && !string.IsNullOrEmpty(t2))
                        {
                            currentDict.AddTranslation(w2, t2);
                            SaveDictionaries();
                        }
                        else
                        {
                            Console.WriteLine("Word or translation cannot be empty!");
                        }
                        PauseForUser();
                        break;

                    case "3":
                        Console.Write("Which word do you want to change: ");
                        string oldW = Console.ReadLine();
                        Console.Write("Enter new word value: ");
                        string newW = Console.ReadLine();
                        currentDict.ChangeWord(oldW, newW);
                        SaveDictionaries();
                        PauseForUser();
                        break;

                    case "4":
                        Console.Write("Enter word: ");
                        string w4 = Console.ReadLine();
                        Console.Write("Which translation do you want to change: ");
                        string oldT = Console.ReadLine();
                        Console.Write("Enter new translation: ");
                        string newT = Console.ReadLine();
                        currentDict.ChangeTranslation(w4, oldT, newT);
                        SaveDictionaries();
                        PauseForUser();
                        break;

                    case "5":
                        Console.Write("Which word do you want to delete (along with its translations): ");
                        string w5 = Console.ReadLine();
                        currentDict.RemoveWord(w5);
                        SaveDictionaries();
                        PauseForUser();
                        break;

                    case "6":
                        Console.Write("Enter word: ");
                        string w6 = Console.ReadLine();
                        Console.Write("Which translation variant do you want to remove: ");
                        string t6 = Console.ReadLine();
                        currentDict.RemoveTranslation(w6, t6);
                        SaveDictionaries();
                        PauseForUser();
                        break;

                    case "7":
                        Console.Write("Enter word to search: ");
                        string w7 = Console.ReadLine();
                        currentDict.SearchWord(w7);
                        PauseForUser();
                        break;

                    case "8":
                        Console.Write("Enter word to export: ");
                        string w8 = Console.ReadLine();
                        currentDict.ExportWord(w8);
                        PauseForUser();
                        break;

                    case "9":
                        currentDict.Show();
                        PauseForUser();
                        break;

                    case "0":
                        backToMain = true;
                        break;

                    default:
                        Console.WriteLine("Invalid choice.");
                        PauseForUser();
                        break;
                }
            }
        }

        static void PauseForUser()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static void SaveDictionaries()
        {
            try
            {
                if (!Directory.Exists(DataFolder))
                    Directory.CreateDirectory(DataFolder);

                foreach (var dict in dictionaries)
                {
                    string filePath = Path.Combine(DataFolder, $"{dict.Name}.json");
                    string json = JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(filePath, json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving files: {ex.Message}");
            }
        }

        static void LoadDictionaries()
        {
            try
            {
                if (Directory.Exists(DataFolder))
                {
                    string[] files = Directory.GetFiles(DataFolder, "*.json");
                    foreach (var file in files)
                    {
                        string json = File.ReadAllText(file);
                        var dict = JsonSerializer.Deserialize<Dyct>(json);
                        if (dict != null)
                        {
                            dictionaries.Add(dict);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading dictionaries: {ex.Message}");
            }
        }
    }
}
