using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.DAL;

namespace Business
{
    public class clsUserActivitie
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int ActivitieID { get; set; }
        public int UserID { get; set; }
        public string Date { get; set; }
        public int Steps { get; set; }
        public float CaloriesBurned  { get; set; }
        public int WorkoutDuration { get; set; }

        public clsUserActivitie()
        {
            this.ActivitieID = -1;
            this.UserID = -1;
            this.Date = DateTime.Now.ToString();
            this.Steps = 0;
            this.CaloriesBurned = 0;
            this.WorkoutDuration = 0;

            Mode = enMode.AddNew;
        }

        private clsUserActivitie(int ActivitieID, int UserID, string Date, int Steps, float CaloriesBurned,
            int WorkoutDuration)
        {
            this.ActivitieID = ActivitieID;
            this.UserID = UserID;
            this.Date = Date;
            this.Steps = Steps;
            this.CaloriesBurned = CaloriesBurned;
            this.WorkoutDuration = WorkoutDuration;

            Mode = enMode.Update;
        }

        public static clsUserActivitie FindUserActivitieByUserIDAndDate(int UserID,string Date)
        {
            int ActivitieID = -1, Steps = 0, WorkoutDuration = 0;
            float CaloriesBurned = 0;

            bool isFound = DatabaseHelper.GetUserActivityByDate(UserID, Date, ref ActivitieID, ref Steps, ref CaloriesBurned,
                ref WorkoutDuration);

            if (isFound)
                return new clsUserActivitie(ActivitieID, UserID, Date, Steps, CaloriesBurned, WorkoutDuration);
            else
                return null;
        }

        private bool _AddUserActivitie()
        {
            this.ActivitieID = DatabaseHelper.AddUserActivitie(this.UserID,this.Date,this.Steps,this.CaloriesBurned,
                this.WorkoutDuration);

            return (this.ActivitieID != -1);
        }

        private bool _UpdateUserActivity()
        {
            return DatabaseHelper.UpdateUserActivity(this.ActivitieID,this.Steps,this.CaloriesBurned,
                this.WorkoutDuration);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddUserActivitie())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;
                case enMode.Update:
                    return _UpdateUserActivity();
            }

            return false;
        }
    }
}
