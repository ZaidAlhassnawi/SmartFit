using FitnessApp.DAL;



namespace Business
{
    public class clsUser
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int UserID { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; }
        public float Weight { get; set; }
        public float Height { get; set; }
        public string Gender { get; set; }
        public string ActivityLevel { get; set; }



        public clsUser()
        {
            DatabaseHelper.InitializeDatabase();
            this.UserID = -1;
            this.UserName = "Test";
            this.Age = 22;
            this.Weight = 73;
            this.Height = 170;
            this.Gender = "Male";
            this.ActivityLevel = "Middle";

            Mode = enMode.AddNew;

        }

        public clsUser(int UserID, string UserName, int Age, float Weight, float Height,
            string Gender, string ActivityLevel)
        {
            DatabaseHelper.InitializeDatabase();
            this.UserID = UserID;
            this.UserName = UserName;
            this.Age = Age;
            this.Weight = Weight;
            this.Height = Height;
            this.Gender = Gender;
            this.ActivityLevel = ActivityLevel;

            Mode = enMode.Update;
        }

        public static clsUser FindUserByID(int UserID)
        {
            string UserName = "", Gender = "", ActivityLevel = "";
            int Age = 0;
            float Weight = 0, Height = 0;

            bool isFound = DatabaseHelper.GetUserById(UserID, ref UserName, ref Age, ref Weight, ref Height, ref Gender, ref ActivityLevel);

            if (!isFound)
            {
                Console.WriteLine($"User with ID {UserID} not found."); // Log message
                return null;
            }

            return new clsUser(UserID, UserName, Age, Weight, Height, Gender, ActivityLevel);
        }



        private bool _AddNewUser()
        {
            this.UserID = DatabaseHelper.AddUser(this.UserName, this.Age, this.Weight, this.Height,
                this.Gender, this.ActivityLevel);

            return (this.UserID != -1);
        }

        private bool _UpdateUser()
        {
            return DatabaseHelper.UpdateUser(this.UserID, this.UserName, this.Age, this.Weight, this.Height, this.Gender,
                this.ActivityLevel);
        }


        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewUser())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;
                case enMode.Update:
                    return _UpdateUser();
            }

            return false;
        }
    }
}
