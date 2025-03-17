using System;
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
                                       (Name, Age, Weight, Height, Gender, ActivityLevel)
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
                Console.WriteLine($"خطأ أثناء إضافة المستخدم: {ex.Message}");
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
                Console.WriteLine($"خطأ أثناء إضافة المستخدم: {ex.Message}");
                return -1;
            }
        }


        // دالة لإضافة بيانات مع البارامترات مثال
        public static int AddExercise(int PlanID,string ExerciseName,int Repetitions,int Sets,int Duration,float CaloriesBurned)
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
                Console.WriteLine($"خطأ أثناء إضافة المستخدم: {ex.Message}");
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
                Console.WriteLine($"خطأ أثناء إضافة المستخدم: {ex.Message}");
                return -1;
            }


        }


        //دالة لإضافة بيانات النشاط اليومي لليوزر
        public static int AddUserActivitie(int UserID, string Date, int Steps, float CaloriesBurned,int WorkoutDuration)
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
                Console.WriteLine($"خطأ أثناء إضافة المستخدم: {ex.Message}");
                return -1;
            }


        }


        // دالة عامة لتنفيذ الاستعلامات مع البارامترات
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
    }

    
}