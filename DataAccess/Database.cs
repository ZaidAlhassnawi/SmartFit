using System.Data;
using System.Data.SQLite;
using System.IO;

namespace FitnessApp.DAL
{
    public static class DatabaseHelper
    {
        private static string databasePath = "fitness.db";  
        private static string connectionString = $"Data Source={databasePath};Version=3;";

        // إنشاء اتصال بقاعدة البيانات
        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(connectionString);
        }

        // تهيئة الجداول عند التشغيل الأول
        public static void InitializeDatabase()
        {
            if (!File.Exists(databasePath))
            {
                SQLiteConnection.CreateFile(databasePath);
                using (var conn = GetConnection())
                {
                    conn.Open();
                    CreateAllTables(conn);
                }
            }
        }

        private static void CreateAllTables(SQLiteConnection conn)
        {
            // جدول المستخدمين
            ExecuteNonQuery(conn, @"
                CREATE TABLE IF NOT EXISTS Users (
                    UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Age INTEGER NOT NULL CHECK(Age >= 18),
                    Weight REAL NOT NULL CHECK(Weight > 0),
                    Height REAL NOT NULL CHECK(Height > 0),
                    Gender TEXT NOT NULL CHECK(Gender IN ('Male', 'Female')),
                    ActivityLevel TEXT NOT NULL CHECK(ActivityLevel IN ('Low', 'Middle', 'High'))
                );");

            // جدول خطط التمارين
            ExecuteNonQuery(conn, @"
                CREATE TABLE IF NOT EXISTS WorkoutPlans (
                    PlanID INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserID INTEGER NOT NULL,
                    PlanName TEXT NOT NULL,
                    PlanDetails TEXT,
                    CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
                );");

            // جدول التمارين الفردية
            ExecuteNonQuery(conn, @"
                CREATE TABLE IF NOT EXISTS Exercises (
                    ExerciseID INTEGER PRIMARY KEY AUTOINCREMENT,
                    PlanID INTEGER NOT NULL,
                    ExerciseName TEXT NOT NULL,
                    Repetitions INTEGER CHECK(Repetitions > 0),
                    Sets INTEGER CHECK(Sets > 0),
                    Duration INTEGER CHECK(Duration > 0),
                    CaloriesBurned REAL,
                    FOREIGN KEY (PlanID) REFERENCES WorkoutPlans(PlanID) ON DELETE CASCADE
                );");

            // جدول خطط التغذية
            ExecuteNonQuery(conn, @"
                CREATE TABLE IF NOT EXISTS NutritionPlans (
                    PlanID INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserID INTEGER NOT NULL,
                    PlanDetails TEXT NOT NULL,
                    CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
                );");

            // جدول النشاط اليومي
            ExecuteNonQuery(conn, @"
                CREATE TABLE IF NOT EXISTS UserActivities (
                    ActivityID INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserID INTEGER NOT NULL,
                    Date TEXT DEFAULT CURRENT_DATE,
                    Steps INTEGER DEFAULT 0 CHECK(Steps >= 0),
                    CaloriesBurned REAL DEFAULT 0 CHECK(CaloriesBurned >= 0),
                    WorkoutDuration INTEGER DEFAULT 0 CHECK(WorkoutDuration >= 0),
                    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
                );");
        }

        // دالة مساعدة لتنفيذ الاستعلامات
        private static void ExecuteNonQuery(SQLiteConnection conn, string query)
        {
            using (var cmd = new SQLiteCommand(query, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        // دالة لإضافة بيانات لجدول اليوزر
        public static int AddUser(string UserName, int Age, float Weight, float Height, string Gender,
            string ActivityLevel)
        {
            int UserId = -1;
            try
            {
                string query = @"INSERT INTO Users 
                                       (Name,Age, Weight, Height, Gender, ActivityLevel)
                                VALUES (@Name, @Age, @Weight, @Height, @Gender, @ActivityLevel)";

                SQLiteParameter[] parameters =
                {
                new SQLiteParameter("@Name", UserName),
                new SQLiteParameter("@Age", Age),
                new SQLiteParameter("@Weight", Weight),
                new SQLiteParameter("@Height", Height),
                new SQLiteParameter("@Gender", Gender),
                new SQLiteParameter("@ActivityLevel", ActivityLevel)
            };

                UserId = ExecuteParametrizedQuery(query, parameters);
                return UserId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
                return -1;
            }
        }


        // دالة لإضافة بيانات لجدول خطط التمارين
        public static int AddWorkoutPlans(int UserID, string PlanName, string PlanDetails, string CreatedAt)
        {
            int PlanID = -1;
            try
            {
                string query = @"INSERT INTO WorkoutPlans 
                                       (UserID, PlanName, PlanDetails, CreatedAt)
                                VALUES (@UserID, @PlanName, @PlanDetails, @CreatedAt)";

                SQLiteParameter[] parameters =
                {
                new SQLiteParameter("@UserID", UserID),
                new SQLiteParameter("@PlanName", PlanName),
                new SQLiteParameter("@PlanDetails", PlanDetails),
                new SQLiteParameter("@CreatedAt", CreatedAt),

            };

                PlanID = ExecuteParametrizedQuery(query, parameters);
                return PlanID;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
                return -1;
            }
        }


        // دالة لإضافة بيانات مع البارامترات مثال
        public static int AddExercise(int PlanID, string ExerciseName, int Repetitions, int Sets, int Duration, float CaloriesBurned)
        {
            int ExerciseID = -1;
            try
            {
                string query = @"
                INSERT INTO Exercises 
                (PlanID,ExerciseName, Repetitions, Sets, Duration, CaloriesBurned)
                VALUES (@PlanID,@ExerciseName, @Repetitions, @Sets, @Duration, @CaloriesBurned)";

                SQLiteParameter[] parameters =
                {
                new SQLiteParameter("@PlanID", PlanID),
                new SQLiteParameter("@ExerciseName", ExerciseName),
                new SQLiteParameter("@Repetitions", Repetitions),
                new SQLiteParameter("@Sets", Sets),
                new SQLiteParameter("@Duration", Duration),
                new SQLiteParameter("@ExerciseName", CaloriesBurned)

                };

                ExerciseID = ExecuteParametrizedQuery(query, parameters);

                return ExerciseID;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
                return -1;
            }


        }

        //دالة لإضافة بيانات خطة التغذية
        public static int AddNutritionPlan(int UserID, string PlanDetails, string CreatedAt)
        {
            int NutritionPlanID = -1;
            try
            {
                string query = @"INSERT INTO NutritionPlans 
                                        (UserID,  PlanDetails,  CreatedAt)
                                 VALUES (@UserID, @PlanDetails, @CreatedAt)";

                SQLiteParameter[] parameters =
                {
                new SQLiteParameter("@UserID", UserID),
                new SQLiteParameter("@PlanDetails", PlanDetails),
                new SQLiteParameter("@CreatedAt", CreatedAt),

                };

                NutritionPlanID = ExecuteParametrizedQuery(query, parameters);

                return NutritionPlanID;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
                return -1;
            }


        }


        //دالة لإضافة بيانات النشاط اليومي لليوزر
        public static int AddUserActivitie(int UserID, string Date, int Steps, float CaloriesBurned, int WorkoutDuration)
        {
            int ActivitieID = -1;
            try
            {
                string query = @"INSERT INTO UserActivities 
                                        (UserID,  Date,  Steps, CaloriesBurned,WorkoutDuration)
                                 VALUES (@UserID, @Date, @Steps, @CaloriesBurned, @WorkoutDuration)";

                SQLiteParameter[] parameters =
                {
                new SQLiteParameter("@UserID", UserID),
                new SQLiteParameter("@Date", Date),
                new SQLiteParameter("@Steps", Steps),
                new SQLiteParameter("@CaloriesBurned", CaloriesBurned),
                new SQLiteParameter("@WorkoutDuration", WorkoutDuration),

                };

                ActivitieID = ExecuteParametrizedQuery(query, parameters);

                return ActivitieID;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
                return -1;
            }


        }


        public static int ExecuteParametrizedQuery(string query, SQLiteParameter[] parameters)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }


        ////Updates///

        public static bool UpdateUser(int userId, string name, int age, double weight, double height, string gender, string activityLevel)
        {
            string query = @"UPDATE Users SET Name = @Name, Age = @Age, Weight = @Weight, 
                     Height = @Height, Gender = @Gender, ActivityLevel = @ActivityLevel
                     WHERE UserID = @UserID";

            SQLiteParameter[] parameters =
            {
                 new SQLiteParameter("@UserID", userId),
                 new SQLiteParameter("@Name", name),
                 new SQLiteParameter("@Age", age),
                 new SQLiteParameter("@Weight", weight),
                 new SQLiteParameter("@Height", height),
                 new SQLiteParameter("@Gender", gender),
                 new SQLiteParameter("@ActivityLevel", activityLevel)
            };

            return (ExecuteParametrizedQuery(query, parameters) != 0);
        }

        public static bool UpdateWorkoutPlan(int planId, string planName, string planDetails)
        {
            string query = @"UPDATE WorkoutPlans SET PlanName = @PlanName, PlanDetails = @PlanDetails
                     WHERE PlanID = @PlanID";

            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@PlanID", planId),
                new SQLiteParameter("@PlanName", planName),
                new SQLiteParameter("@PlanDetails", planDetails)
            };

            return (ExecuteParametrizedQuery(query, parameters) != 0);
        }


        ////تحديث خطة تمرين معين
        public static bool UpdateExercise(int exerciseId, string exerciseName, int repetitions, int sets, int duration, float caloriesBurned)
        {
            string query = @"UPDATE Exercises SET ExerciseName = @ExerciseName, Repetitions = @Repetitions, 
                     Sets = @Sets, Duration = @Duration, CaloriesBurned = @CaloriesBurned
                     WHERE ExerciseID = @ExerciseID";

            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@ExerciseID", exerciseId),
                new SQLiteParameter("@ExerciseName", exerciseName),
                new SQLiteParameter("@Repetitions", repetitions),
                new SQLiteParameter("@Sets", sets),
                new SQLiteParameter("@Duration", duration),
                new SQLiteParameter("@CaloriesBurned", caloriesBurned)
            };

            return (ExecuteParametrizedQuery(query, parameters) != 0);
        }

        //تحديث نشاط معين للمستخدم
        public static bool UpdateUserActivity(int activityId, int steps, float caloriesBurned, int workoutDuration)
        {
            string query = @"UPDATE UserActivities SET Steps = @Steps, CaloriesBurned = @CaloriesBurned, 
                     WorkoutDuration = @WorkoutDuration WHERE ActivityID = @ActivityID";

            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@ActivityID", activityId),
                new SQLiteParameter("@Steps", steps),
                new SQLiteParameter("@CaloriesBurned", caloriesBurned),
                new SQLiteParameter("@WorkoutDuration", workoutDuration)
            };

            return (ExecuteParametrizedQuery(query, parameters) != 0);
        }

        private static SQLiteDataReader ExecuteReader(string query, SQLiteParameter[] parameters)
        {
            var conn = GetConnection();
            conn.Open();

            using (var cmd = new SQLiteCommand(query, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                return cmd.ExecuteReader(CommandBehavior.CloseConnection); // يغلق الاتصال عند إغلاق الـ reader
            }
        }

        public static bool GetUserById(int userId, ref string UserName, ref int Age, ref float Weight, ref float Height, ref string Gender, ref string ActivityLevel)
        {
            bool isFound = false;

            string query = "SELECT Name, Age, Weight, Height, Gender, ActivityLevel FROM Users WHERE UserID = @UserID";
            SQLiteParameter[] parameters = { new SQLiteParameter("@UserID", userId) };

            using (var reader = ExecuteReader(query, parameters))
            {
                if (reader != null && reader.Read()) // تأكد أن هناك بيانات قبل القراءة
                {
                    isFound = true;

                    UserName = reader["Name"].ToString();
                    Age = Convert.ToInt32(reader["Age"]);
                    Weight = Convert.ToSingle(reader["Weight"]);
                    Height = Convert.ToSingle(reader["Height"]);
                    Gender = reader["Gender"].ToString();
                    ActivityLevel = reader["ActivityLevel"].ToString();
                }
            }

            return isFound;
        }

        public static SQLiteDataReader GetAllUsers()
        {
            string query = "SELECT * FROM Users";
            return ExecuteReader(query, null);
        }

        public static bool GetWorkoutPlansByUserId(int UserID, ref int PlanID, ref string PlanName, ref string PlanDetails, ref string CreatedAt)
        {
            bool isFound = false;

            string query = "SELECT * FROM WorkoutPlans WHERE UserID = @UserID";
            SQLiteParameter[] parameters = { new SQLiteParameter("@UserID", UserID) };

            using (var reader = ExecuteReader(query, parameters))
            {
                if (reader.Read())  // التحقق من وجود بيانات
                {
                    isFound = true;

                    PlanID = (int)reader["PlanID"];
                    PlanName = (string)reader["PlanName"];
                    PlanDetails = (string)reader["PlanDetails"];
                    CreatedAt = (string)reader["CreatedAt"];

                }
            }

            return isFound; // في حال تم العثور على المستخدم تكون النتيجة ترو غير ذلك فولس
        }

        // البحث عن جميع التمارين ضمن خطة معينة
        public static bool GetExercisesByPlanId(int PlanID, ref int ExerciseID, ref string ExerciseName, ref int Repetitions, ref int Sets, ref int Duration, ref float CaloriesBurned)
        {
            bool isFound = false;

            string query = "SELECT * FROM Exercises WHERE PlanID = @PlanID";
            SQLiteParameter[] parameters = { new SQLiteParameter("@PlanID", PlanID) };

            using (var reader = ExecuteReader(query, parameters))
            {
                if (reader.Read())  // التحقق من وجود بيانات
                {
                    isFound = true;

                    ExerciseID = (int)reader["ExerciseID"];
                    ExerciseName = (string)reader["ExerciseName"];
                    Repetitions = (int)reader["Repetitions"];
                    Sets = (int)reader["Sets"];
                    Duration = (int)reader["Duration"];
                    CaloriesBurned = (float)reader["CaloriesBurned"];

                }
            }

            return isFound; // في حال تم العثور على المستخدم تكون النتيجة ترو غير ذلك فولس
        }

        //البحث عن نشاط يومي لمستخدم في تاريخ معين
        public static bool GetUserActivityByDate(int UserID, string Date, ref int ActivitieID, ref int Steps, ref float CaloriesBurned, ref int WorkoutDuration)
        {
            bool isFound = false;

            string query = "SELECT * FROM UserActivities WHERE UserID = @UserID AND Date = @Date";
            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@UserID", UserID),
                new SQLiteParameter("@Date", Date)
            };

            using (var reader = ExecuteReader(query, parameters))
            {
                if (reader.Read())  // التحقق من وجود بيانات
                {
                    isFound = true;

                    ActivitieID = (int)reader["ActivitieID"];
                    Steps = (int)reader["Steps"];
                    CaloriesBurned = (float)reader["CaloriesBurned"];
                    WorkoutDuration = (int)reader["WorkoutDuration"];
                }
            }

            return isFound; // في حال تم العثور على المستخدم تكون النتيجة ترو غير ذلك فولس   

        }

        public static bool GetNutritionPlan(int UserID, ref int NutritionPlanID, ref string PlanDetails, ref string CreatedAt)
        {
            bool isFound = false;

            string query = "SELECT * FROM NutritionPlans WHERE UserID = @UserID;";
            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@UserID", UserID),
            };

            using (var reader = ExecuteReader(query, parameters))
            {
                if (reader.Read())  // التحقق من وجود بيانات
                {
                    isFound = true;

                    NutritionPlanID = (int)reader["NutritionPlanID"];
                    PlanDetails = (string)reader["PlanDetails"];
                    CreatedAt = (string)reader["CreatedAt"];

                }
            }

            return isFound; // في حال تم العثور على المستخدم تكون النتيجة ترو غير ذلك فولس      

        }

    }

}
