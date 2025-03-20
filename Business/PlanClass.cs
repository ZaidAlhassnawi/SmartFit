using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Text;
using FitnessApp.DAL;
using Newtonsoft.Json;

namespace Business
{
    public class clsPlan
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int PlanID { get; set; }
        public int UserID { get; set; }
        public string PlanName { get; set; }
        public string PlanDetails { get; set; }
        public string CreatedAt { get; set; }

        public clsPlan()
        {
            this.PlanID = -1;
            this.UserID = -1;
            this.PlanName = "";
            this.PlanDetails = "";
            this.CreatedAt = DateTime.Now.ToString();

            Mode = enMode.AddNew;
        }

        private clsPlan(int PlanID, int UserID, string PlanName, string PlanDetails, string CreatedAt)
        {
            this.PlanID = PlanID;
            this.UserID = UserID;
            this.PlanName = PlanName;
            this.PlanDetails = PlanDetails;
            this.CreatedAt = CreatedAt;

            Mode = enMode.Update;
        }


        public static clsPlan FindWorkoutPlanByUserID(int UserID)
        {
            int PlanID = -1;
            string PlanName = "", PlanDetalis = "", CreatedAt = "";

            bool isFound = DatabaseHelper.GetWorkoutPlansByUserId(UserID, ref PlanID, ref PlanName, ref PlanDetalis, ref CreatedAt);

            if (isFound)
                return new clsPlan(PlanID, UserID, PlanName, PlanDetalis, CreatedAt);
            else
                return null;
        }

        private bool _AddWorkoutPlan()
        {
            this.PlanID = DatabaseHelper.AddWorkoutPlans(this.UserID, this.PlanName, this.PlanDetails, this.CreatedAt);

            return (this.PlanID != -1);
        }

        private bool _UpdateWorkoutPlan()
        {
            return DatabaseHelper.UpdateWorkoutPlan(this.PlanID, this.PlanName, this.PlanDetails);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddWorkoutPlan())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;
                case enMode.Update:
                    return _UpdateWorkoutPlan();
            }

            return false;
        }

        public static PromptResult CreatePlan(Prompt PromptObject)
        {
            try
            {
                // Build the prompt based on the user's input
                string prompt = BuildPrompt(PromptObject);

                using (HttpClient client = new HttpClient())
                {
                    // Add API Key to the header
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {DecodeBase64(File.ReadAllText("SecretFile.txt").Trim())}");

                    // Prepare the request payload (JSON format)
                    var requestBody = new
                    {
                        model = "gpt-3.5-turbo",
                        messages = new List<object> { new { role = "user", content = prompt } }
                    };

                    string jsonContent = JsonConvert.SerializeObject(requestBody);
                    StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = client.PostAsync(ConfigurationManager.AppSettings["apiUrl"], content).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Error calling AI API: {response.StatusCode}");
                    }

                    string aiResponse = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("🟢 AI Response: " + aiResponse); // Debugging

                    // Check if response is empty
                    if (string.IsNullOrWhiteSpace(aiResponse))
                        throw new Exception("AI response is empty.");

                    string[] responseParts = aiResponse.Split(new[] { "----" }, StringSplitOptions.None);

                    if (responseParts.Length < 2)
                        throw new Exception("Invalid AI response format.");

                    Dictionary<string, string> firstPart = new Dictionary<string, string>
            {
                { "Field1", responseParts[0].Split("\n")[0] },  // Calorie Intake
                { "Field2", responseParts[0].Split("\n")[1] },  // Water Intake
                { "Field3", responseParts[0].Split("\n")[2] }   // Exercise Time
            };

                    Dictionary<string, string> secondPart = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseParts[1]);

                    if (secondPart == null || secondPart.Count == 0)
                        throw new Exception("Exercise data is missing or not formatted correctly.");

                    Dictionary<string, (int reps, int duration)> exercises = new Dictionary<string, (int reps, int duration)>();

                    foreach (var item in secondPart)
                    {
                        string[] exerciseData = item.Value.Split(',');

                        if (exerciseData.Length == 2)
                        {
                            if (int.TryParse(exerciseData[0], out int reps) && int.TryParse(exerciseData[1], out int duration))
                            {
                                exercises[item.Key] = (reps, duration);
                            }
                            else
                            {
                                Console.WriteLine($"⚠️ Skipping exercise: {item.Key} (Invalid format)");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"⚠️ Skipping exercise: {item.Key} (Unexpected format)");
                        }
                    }

                    // Create and return the final result
                    return new PromptResult
                    {
                        CalorieIntake = Convert.ToInt32(firstPart["Field1"]),
                        AmountOfWater = Convert.ToInt32(firstPart["Field2"]),
                        ExerciseTime = Convert.ToInt32(firstPart["Field3"]),
                        Exercises = exercises
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ ERROR: " + ex.Message);
                return null; // Handle error gracefully
            }
        }


        public static string BuildPrompt(Prompt promptObject)
        {
            StringBuilder promptBuilder = new StringBuilder();

            promptBuilder.AppendLine($"Age: {promptObject.Age}");
            promptBuilder.AppendLine($"Height: {promptObject.Height}");
            promptBuilder.AppendLine($"Weight: {promptObject.Weight}");
            promptBuilder.AppendLine($"Exercise Time: {promptObject.ExerciseTime}");
            promptBuilder.AppendLine($"Exercise Days: {string.Join(", ", promptObject.ExerciseDayes)}");
            promptBuilder.AppendLine($"Favorite Exercises: {string.Join(", ", promptObject.Favoriteexercises)}");
            promptBuilder.AppendLine($"Exercise Goal: {promptObject.ExercisesGoal}");

            promptBuilder.AppendLine("Please create a detailed exercise plan that includes:");
            promptBuilder.AppendLine("1. The amount of water to drink daily.");
            promptBuilder.AppendLine("2. The recommended calorie intake.");
            promptBuilder.AppendLine("3. Total Exercise Time.");
            promptBuilder.AppendLine("4. A list of exercises with sets, reps, and duration.");

            return promptBuilder.ToString();
        }

        public class Prompt
        {
            public string Age, Height, Weight, ExerciseTime;
            public string? ExerciseDayes, Favoriteexercises;
            public string? ExercisesGoal;
        }

        public class PromptResult
        {
            public int CalorieIntake, AmountOfWater, ExerciseTime;
            public Dictionary<string, (int reps, int duration)>? Exercises;
        }

        static string DecodeBase64(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
