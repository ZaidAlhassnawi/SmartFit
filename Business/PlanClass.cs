using System.Net.Http;
using System.Text;
using System.Windows;
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

        /////////////////////////////////////////////////////////////////////////////////////

        public static PromptResult CreatePlan(Prompt promptObject)
        {
            try
            {
                string prompt = BuildPrompt(promptObject);
                string apiKey = "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";
                string apiUrl = "https://openrouter.ai/api/v1/chat/completions";

                using (HttpClient client = new HttpClient())
                {
                    var requestBody = new
                    {
                        model = "google/gemma-3-1b-it:free",
                        messages = new[] {
                    new { role = "user", content = new object[] { new { type = "text", text = prompt } } }
                }
                    };

                    string jsonContent = JsonConvert.SerializeObject(requestBody);
                    StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                    HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;

                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"API Error: {response.StatusCode}\nDetails: {response.Content.ReadAsStringAsync().Result}");

                    string aiResponse = response.Content.ReadAsStringAsync().Result;

                    // Debugging: Log the raw AI response
                    MessageBox.Show("🔍 AI Response: " + aiResponse);

                    dynamic result = JsonConvert.DeserializeObject(aiResponse);
                    string aiGeneratedText = result?.choices?[0]?.message?.content ?? throw new Exception("Invalid AI response format.");

                    // Split response into parts (ensure AI is formatting it correctly)
                    string[] responseParts = aiGeneratedText.Split(new[] { "----" }, StringSplitOptions.None);
                    if (responseParts.Length < 2)
                        throw new Exception("Invalid AI response format.");

                    // Parse first part (Extracting numerical values)
                    string[] firstPartLines = responseParts[0].Split('\n');
                    if (firstPartLines.Length < 3)
                        throw new Exception("Insufficient data in AI response.");

                    int calorieIntake = int.TryParse(firstPartLines[0].Trim(), out int ci) ? ci : 0;
                    int amountOfWater = int.TryParse(firstPartLines[1].Trim(), out int aw) ? aw : 0;
                    int exerciseTime = int.TryParse(firstPartLines[2].Trim(), out int et) ? et : 0;

                    // Deserialize second part (Exercises Dictionary)
                    Dictionary<string, string> secondPart = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseParts[1]);

                    // Processing exercises (Ensuring valid format)
                    Dictionary<string, (int reps, int duration)> exercises = new();
                    foreach (var item in secondPart)
                    {
                        string[] exerciseData = item.Value.Split(',');
                        if (exerciseData.Length == 2 &&
                            int.TryParse(exerciseData[0], out int reps) &&
                            int.TryParse(exerciseData[1], out int duration))
                        {
                            exercises[item.Key] = (reps, duration);
                        }
                    }

                    // Return only the required values
                    return new PromptResult
                    {
                        CalorieIntake = calorieIntake,
                        AmountOfWater = amountOfWater,
                        ExerciseTime = exerciseTime,
                        Exercises = exercises
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ ERROR: " + ex.Message);
                return null;
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////

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
