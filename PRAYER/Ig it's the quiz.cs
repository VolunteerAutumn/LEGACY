using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

// === I M P O R T A N T ===
// I sent this task to you cuz I didn't know what to submit to SMTP
// while I did use ai to format my code and help me fix some bugs, around 90% of the code was written by me personally
// And I am more than proud
// It's also not my fault I didn;t know what to submit to smtp as when I opened the task it just showed the c# exam tasks
// glhf (watch the videoproof it's workig if smth fails)

namespace QuizApplication
{
    #region Models

    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string BirthDate { get; set; }
        public List<QuizResult> History { get; set; } = new List<QuizResult>();
    }

    public class QuizResult
    {
        public string QuizTitle { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class Question
    {
        public string Text { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public List<int> CorrectOptionIndices { get; set; } = new List<int>();
    }

    public class Quiz
    {
        public string Title { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
    }

    #endregion

    class Program
    {
        private const string UsersFile = "users.json";
        private const string QuizzesFile = "quizzes.json";

        private static List<User> _users = new List<User>();
        private static List<Quiz> _quizzes = new List<Quiz>();
        private static User _currentUser = null;

        static void Main(string[] sender)
        {
            LoadData();
            EnsureDefaultData();
            MainMenu();
        }

        #region Data Persistence

        private static void LoadData()
        {
            if (File.Exists(UsersFile))
            {
                string json = File.ReadAllText(UsersFile);
                _users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            }
            if (File.Exists(QuizzesFile))
            {
                string json = File.ReadAllText(QuizzesFile);
                _quizzes = JsonSerializer.Deserialize<List<Quiz>>(json) ?? new List<Quiz>();
            }
        }

        private static void SaveData()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(UsersFile, JsonSerializer.Serialize(_users, options));
            File.WriteAllText(QuizzesFile, JsonSerializer.Serialize(_quizzes, options));
        }

        private static void EnsureDefaultData()
        {
            if (!_users.Any(u => u.Login.ToLower() == "admin"))
            {
                _users.Add(new User { Login = "admin", Password = "adminpassword", BirthDate = "01/01/2000" });
                SaveData();
            }

            // Seed Biology Quiz
            if (!_quizzes.Any(q => q.Title.Equals("Biology", StringComparison.OrdinalIgnoreCase)))
            {
                var biologyQuiz = new Quiz { Title = "Biology" };
                biologyQuiz.Questions.Add(new Question
                {
                    Text = "Which of the following are considered 'powerhouses of the cell' where ATP production occurs?",
                    Options = new List<string> { "Nucleus", "Mitochondria", "Ribosomes", "Golgi Apparatus" },
                    CorrectOptionIndices = new List<int> { 1 }
                });
                biologyQuiz.Questions.Add(new Question
                {
                    Text = "Which base pairs are found in DNA? (Select ALL that apply)",
                    Options = new List<string> { "Adenine", "Thymine", "Uracil", "Guanine" },
                    CorrectOptionIndices = new List<int> { 0, 1, 3 }
                });
                biologyQuiz.Questions.Add(new Question
                {
                    Text = "What is the primary gas produced as a byproduct of photosynthesis?",
                    Options = new List<string> { "Carbon Dioxide", "Nitrogen", "Oxygen", "Hydrogen" },
                    CorrectOptionIndices = new List<int> { 2 }
                });
                _quizzes.Add(biologyQuiz);
            }

            // Seed History Quiz
            if (!_quizzes.Any(q => q.Title.Equals("History", StringComparison.OrdinalIgnoreCase)))
            {
                var historyQuiz = new Quiz { Title = "History" };
                historyQuiz.Questions.Add(new Question
                {
                    Text = "Who was the first President of the United States?",
                    Options = new List<string> { "Thomas Jefferson", "Abraham Lincoln", "George Washington", "John Adams" },
                    CorrectOptionIndices = new List<int> { 2 }
                });
                historyQuiz.Questions.Add(new Question
                {
                    Text = "Which empires fought in the Punic Wars? (Select ALL that apply)",
                    Options = new List<string> { "Roman Republic", "Carthage", "Persian Empire", "Ancient Greece" },
                    CorrectOptionIndices = new List<int> { 0, 1 }
                });
                historyQuiz.Questions.Add(new Question
                {
                    Text = "In which year did World War II end?",
                    Options = new List<string> { "1918", "1939", "1945", "1950" },
                    CorrectOptionIndices = new List<int> { 2 }
                });
                _quizzes.Add(historyQuiz);
            }

            // Seed Geography Quiz
            if (!_quizzes.Any(q => q.Title.Equals("Geography", StringComparison.OrdinalIgnoreCase)))
            {
                var geographyQuiz = new Quiz { Title = "Geography" };
                geographyQuiz.Questions.Add(new Question
                {
                    Text = "What is the capital city of Australia?",
                    Options = new List<string> { "Sydney", "Melbourne", "Canberra", "Brisbane" },
                    CorrectOptionIndices = new List<int> { 2 }
                });
                geographyQuiz.Questions.Add(new Question
                {
                    Text = "Which of the following countries share a land border with Brazil? (Select ALL that apply)",
                    Options = new List<string> { "Argentina", "Colombia", "Chile", "Peru" },
                    CorrectOptionIndices = new List<int> { 0, 1, 4 }
                });
                geographyQuiz.Questions.Add(new Question
                {
                    Text = "Which river is known as the longest river in the world?",
                    Options = new List<string> { "Amazon River", "Nile River", "Yangtze River", "Mississippi River" },
                    CorrectOptionIndices = new List<int> { 1 }
                });
                _quizzes.Add(geographyQuiz);
            }

            // Seed Pokemon Quiz
            if (!_quizzes.Any(q => q.Title.Equals("Pokemon", StringComparison.OrdinalIgnoreCase)))
            {
                var pokemonQuiz = new Quiz { Title = "Pokemon" };
                pokemonQuiz.Questions.Add(new Question
                {
                    Text = "Which of the following are Starter Pokémon from the Kanto Region (Gen 1)? (Select ALL that apply)",
                    Options = new List<string> { "Bulbasaur", "Pikachu", "Charmander", "Squirtle" },
                    CorrectOptionIndices = new List<int> { 0, 2, 3 }
                });
                pokemonQuiz.Questions.Add(new Question
                {
                    Text = "What type is the Pokémon 'Gyarados'?",
                    Options = new List<string> { "Water / Dragon", "Water / Flying", "Pure Water", "Water / Dark" },
                    CorrectOptionIndices = new List<int> { 1 }
                });
                pokemonQuiz.Questions.Add(new Question
                {
                    Text = "How many Eeveelutions (evolutions of Eevee) exist as of Generation 8/9?",
                    Options = new List<string> { "3", "6", "8", "9" },
                    CorrectOptionIndices = new List<int> { 2 }
                });
                _quizzes.Add(pokemonQuiz);
            }

            // Fallback legacy demo quiz initialization if database is entirely fresh
            if (!_quizzes.Any(q => q.Title.Equals("General Knowledge", StringComparison.OrdinalIgnoreCase)))
            {
                var demoQuiz = new Quiz { Title = "General Knowledge" };
                for (int i = 1; i <= 20; i++)
                {
                    demoQuiz.Questions.Add(new Question
                    {
                        Text = $"Demo Question {i}: Which options are correct? (Select 1 and 2)",
                        Options = new List<string> { "Option A (Correct)", "Option B (Correct)", "Option C", "Option D" },
                        CorrectOptionIndices = new List<int> { 0, 1 }
                    });
                }
                _quizzes.Add(demoQuiz);
            }

            SaveData();
        }

        #endregion

        #region UI Flow

        private static void MainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== WELCOME TO THE QUIZ APPLICATION ===");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. Admin Utility (Manage Quizzes)");
                Console.WriteLine("4. Exit");
                Console.Write("Select an option: ");

                switch (Console.ReadLine())
                {
                    case "1": Login(); break;
                    case "2": Register(); break;
                    case "3": AdminLogin(); break;
                    case "4": Environment.Exit(0); break;
                    default: Console.WriteLine("Invalid option. Press any key..."); Console.ReadKey(); break;
                }
            }
        }

        private static void Login()
        {
            Console.Clear();
            Console.WriteLine("=== LOGIN ===");
            Console.Write("Username: ");
            string username = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();

            User user = _users.FirstOrDefault(u => u.Login.Equals(username, StringComparison.OrdinalIgnoreCase) && u.Password == password);

            if (user != null)
            {
                _currentUser = user;
                UserDashboard();
            }
            else
            {
                Console.WriteLine("Invalid credentials! Press any key to return...");
                Console.ReadKey();
            }
        }

        private static void Register()
        {
            Console.Clear();
            Console.WriteLine("=== REGISTRATION ===");
            Console.Write("Enter new username: ");
            string username = Console.ReadLine();

            if (_users.Any(u => u.Login.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("This username already exists! Press any key...");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            Console.Write("Enter birth date (DD/MM/YYYY): ");
            string dob = Console.ReadLine();

            _users.Add(new User { Login = username, Password = password, BirthDate = dob });
            SaveData();

            Console.WriteLine("Registration successful! You can now log in. Press any key...");
            Console.ReadKey();
        }

        private static void UserDashboard()
        {
            while (_currentUser != null)
            {
                Console.Clear();
                Console.WriteLine($"=== USER DASHBOARD: Welcome, {_currentUser.Login} ===");
                Console.WriteLine("1. Start a New Quiz");
                Console.WriteLine("2. View My Past Results");
                Console.WriteLine("3. View Top 20 Leaderboard");
                Console.WriteLine("4. Settings (Change Password/DOB)");
                Console.WriteLine("5. Logout");
                Console.Write("Select an option: ");

                switch (Console.ReadLine())
                {
                    case "1": StartQuiz(); break;
                    case "2": ViewPersonalHistory(); break;
                    case "3": ViewLeaderboard(); break;
                    case "4": AccountSettings(); break;
                    case "5": _currentUser = null; break;
                }
            }
        }

        #endregion

        #region Gameplay Logic

        private static void StartQuiz()
        {
            Console.Clear();
            Console.WriteLine("=== SELECT A QUIZ CATEGORY ===");
            for (int i = 0; i < _quizzes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_quizzes[i].Title}");
            }
            Console.WriteLine($"{_quizzes.Count + 1}. Mixed Quiz (Random questions)");
            Console.Write("Choose category index: ");

            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > _quizzes.Count + 1)
            {
                Console.WriteLine("Invalid selection. Press any key...");
                Console.ReadKey();
                return;
            }

            List<Question> activeQuestions = new List<Question>();
            string quizTitle = "";

            if (choice == _quizzes.Count + 1)
            {
                quizTitle = "Mixed Quiz";
                var allQuestions = _quizzes.SelectMany(q => q.Questions).ToList();
                var random = new Random();
                activeQuestions = allQuestions.OrderBy(x => random.Next()).Take(20).ToList();
            }
            else
            {
                var selectedQuiz = _quizzes[choice - 1];
                quizTitle = selectedQuiz.Title;
                activeQuestions = selectedQuiz.Questions.Take(20).ToList();
            }

            if (activeQuestions.Count < 20)
            {
                Console.WriteLine($"Warning: The selected pool only contains {activeQuestions.Count} questions (Minimum 20 requested).");
                Console.WriteLine("Press any key to play anyway or adjust your database via admin console.");
                Console.ReadKey();
            }

            int correctAnswersCount = 0;

            for (int i = 0; i < activeQuestions.Count; i++)
            {
                Console.Clear();
                Console.WriteLine($"Quiz: {quizTitle} | Question {i + 1} of {activeQuestions.Count}\n");
                var q = activeQuestions[i];
                Console.WriteLine(q.Text);

                for (int j = 0; j < q.Options.Count; j++)
                {
                    Console.WriteLine($"  [{j + 1}] {q.Options[j]}");
                }

                Console.Write("\nYour answer(s) (If multiple, separate with commas e.g: 1,3): ");
                string input = Console.ReadLine();

                // Parse input options indices
                var userIndices = input.Split(',').Select(s => int.TryParse(s.Trim(), out int val) ? val - 1 : -1).Where(val => val >= 0).OrderBy(v => v).ToList();

                var correctIndices = q.CorrectOptionIndices.OrderBy(v => v).ToList();

                if (userIndices.SequenceEqual(correctIndices))
                {
                    correctAnswersCount++;
                }
            }

            Console.Clear();
            Console.WriteLine("=== QUIZ FINISHED ===");
            Console.WriteLine($"Your Score: {correctAnswersCount} / {activeQuestions.Count}");

            var newResult = new QuizResult
            {
                QuizTitle = quizTitle,
                Score = correctAnswersCount,
                TotalQuestions = activeQuestions.Count,
                DateTime = DateTime.Now
            };
            _currentUser.History.Add(newResult);
            SaveData();

            int rank = CalculateRank(quizTitle, correctAnswersCount);
            Console.WriteLine($"Your position on the global '{quizTitle}' board: #{rank}");
            Console.WriteLine("\nPress any key to return to dashboard...");
            Console.ReadKey();
        }

        private static int CalculateRank(string quizTitle, int score)
        {
            var allScores = _users.SelectMany(u => u.History)
                                  .Where(r => r.QuizTitle.Equals(quizTitle, StringComparison.OrdinalIgnoreCase))
                                  .Select(r => r.Score)
                                  .ToList();

            allScores.Add(score);
            var ordered = allScores.OrderByDescending(s => s).ToList();
            return ordered.IndexOf(score) + 1;
        }

        private static void ViewPersonalHistory()
        {
            Console.Clear();
            Console.WriteLine($"=== HISTORY FOR {_currentUser.Login} ===");
            if (!_currentUser.History.Any())
            {
                Console.WriteLine("No played quizzes found.");
            }
            foreach (var record in _currentUser.History)
            {
                Console.WriteLine($"[{record.DateTime:g}] {record.QuizTitle}: {record.Score}/{record.TotalQuestions}");
            }
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
        }

        private static void ViewLeaderboard()
        {
            Console.Clear();
            Console.WriteLine("=== CHOOSE LEADERBOARD CATEGORY ===");
            var categories = _users.SelectMany(u => u.History).Select(h => h.QuizTitle).Distinct().ToList();

            if (!categories.Any())
            {
                Console.WriteLine("No records available in global leaderboard databases yet.");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i]}");
            }
            Console.Write("Select category: ");
            if (int.TryParse(Console.ReadLine(), out int idx) && idx > 0 && idx <= categories.Count)
            {
                string targetCategory = categories[idx - 1];

                // Gather all entries flattened
                var leaderBoardRaw = from user in _users
                                     from history in user.History
                                     where history.QuizTitle == targetCategory
                                     select new { user.Login, history.Score, history.TotalQuestions, history.DateTime };

                var top20 = leaderBoardRaw.OrderByDescending(x => x.Score).ThenBy(x => x.DateTime).Take(20).ToList();

                Console.Clear();
                Console.WriteLine($"=== TOP 20: {targetCategory} ===");
                int placement = 1;
                foreach (var entry in top20)
                {
                    Console.WriteLine($"{placement,-4} | User: {entry.Login,-12} | Score: {entry.Score}/{entry.TotalQuestions} ({entry.DateTime:d})");
                    placement++;
                }
            }
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
        }

        private static void AccountSettings()
        {
            Console.Clear();
            Console.WriteLine("=== PROFILE SETTINGS ===");
            Console.WriteLine("1. Change Password");
            Console.WriteLine("2. Change Birth Date");
            Console.Write("Select choice: ");
            string opt = Console.ReadLine();

            if (opt == "1")
            {
                Console.Write("Enter new password: ");
                _currentUser.Password = Console.ReadLine();
                SaveData();
                Console.WriteLine("Password updated successfully!");
            }
            else if (opt == "2")
            {
                Console.Write("Enter new birth date (DD/MM/YYYY): ");
                _currentUser.BirthDate = Console.ReadLine();
                SaveData();
                Console.WriteLine("Birth date updated successfully!");
            }
            Console.ReadKey();
        }

        #endregion

        #region Admin Utility Logic

        private static void AdminLogin()
        {
            Console.Clear();
            Console.WriteLine("=== UTILITY ADMIN AUTHORIZATION ===");
            Console.Write("Admin Username: ");
            string adminUser = Console.ReadLine();
            Console.Write("Admin Password: ");
            string adminPass = Console.ReadLine();

            if (adminUser.ToLower() == "admin" && _users.Any(u => u.Login.ToLower() == "admin" && u.Password == adminPass))
            {
                AdminDashboard();
            }
            else
            {
                Console.WriteLine("Access denied! Invalid Admin Credentials. Press any key...");
                Console.ReadKey();
            }
        }

        private static void AdminDashboard()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ADMIN CONTROLLER PANEL ===");
                Console.WriteLine("1. Create a New Quiz Section");
                Console.WriteLine("2. Edit/Add Questions to Existing Quiz");
                Console.WriteLine("3. Back to Main Application Interface");
                Console.Write("Command: ");

                switch (Console.ReadLine())
                {
                    case "1": AdminCreateQuiz(); break;
                    case "2": AdminEditQuiz(); break;
                    case "3": return;
                }
            }
        }

        private static void AdminCreateQuiz()
        {
            Console.Write("Enter the Title for the new Quiz Section: ");
            string title = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(title) || _quizzes.Any(q => q.Title.Equals(title, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Invalid title or quiz section already exists.");
                Console.ReadKey();
                return;
            }

            var newQuiz = new Quiz { Title = title };
            _quizzes.Add(newQuiz);
            SaveData();

            Console.WriteLine($"Quiz section '{title}' initialized! Head to editing menu to add questions. Press any key...");
            Console.ReadKey();
        }

        private static void AdminEditQuiz()
        {
            Console.Clear();
            if (!_quizzes.Any())
            {
                Console.WriteLine("No quizzes available to manage.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("=== SELECT QUIZ TO EDIT ===");
            for (int i = 0; i < _quizzes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_quizzes[i].Title} ({_quizzes[i].Questions.Count} questions)");
            }
            Console.Write("Selection index: ");
            if (int.TryParse(Console.ReadLine(), out int idx) && idx > 0 && idx <= _quizzes.Count)
            {
                var targetQuiz = _quizzes[idx - 1];
                Console.Clear();
                Console.WriteLine($"Editing Section: {targetQuiz.Title}");
                Console.WriteLine("1. Add a Question");
                Console.WriteLine("2. Clear all questions");
                Console.Write("Selection: ");

                string selection = Console.ReadLine();
                if (selection == "1")
                {
                    Console.Write("Enter Question Statement: ");
                    string qText = Console.ReadLine();

                    Console.Write("How many answer choices will this question have?: ");
                    int.TryParse(Console.ReadLine(), out int optionsCount);

                    var options = new List<string>();
                    for (int o = 0; o < optionsCount; o++)
                    {
                        Console.Write($" Enter Option Text #{o + 1}: ");
                        options.Add(Console.ReadLine());
                    }

                    Console.Write("Enter correct option numbers separated by commas (e.g. 1,2): ");
                    string answersInput = Console.ReadLine();
                    var correctIndices = answersInput.Split(',').Select(s => int.TryParse(s.Trim(), out int val) ? val - 1 : -1).Where(val => val >= 0 && val < optionsCount).ToList();

                    targetQuiz.Questions.Add(new Question
                    {
                        Text = qText,
                        Options = options,
                        CorrectOptionIndices = correctIndices
                    });

                    SaveData();
                    Console.WriteLine("Question saved successfully!");
                    Console.ReadKey();
                }
                else if (selection == "2")
                {
                    targetQuiz.Questions.Clear();
                    SaveData();
                    Console.WriteLine("Cleared.");
                    Console.ReadKey();
                }
            }
        }

        #endregion
    }
}
