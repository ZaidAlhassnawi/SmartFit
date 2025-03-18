using FitnessApp.DAL;

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

    }
}
