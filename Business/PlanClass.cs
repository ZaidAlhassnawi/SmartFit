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

        /////////////////////////////////////////////////////////////////////////////////////

        public static async Task<string> CreatePlanAsync(Prompt promptObject)
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
                        messages = new[]
                        {
                        new { role = "user", content = new object[] { new { type = "text", text = prompt } } }
                    }
                    };

                    string jsonContent = JsonConvert.SerializeObject(requestBody);
                    StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                    string aiResponse = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"API Error: {response.StatusCode}\nDetails: {aiResponse}");

                    dynamic result = JsonConvert.DeserializeObject(aiResponse);
                    string aiGeneratedText = result?.choices?[0]?.message?.content ?? throw new Exception("Invalid AI response format.");

                    return aiGeneratedText; // Returning only the raw AI-generated text
                }
            }
            catch (Exception ex)
            {
                return "❌ ERROR: " + ex.Message;
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
